using System;
using System.Data;
using System.Security.Principal;
using System.Threading;
using System.Xml;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using SpotfireIntegration.Common;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CBVUtilities;
using FormDBLib;
using System.Collections;
using System.Linq;

namespace COEServiceLib
{
    public class COEService
    {
        #region Fields

        private IPrincipal principal = null;
        private Dictionary<COEHitList, COEHitListCache> hitLists = new Dictionary<COEHitList, COEHitListCache>();
        private Version serverVersion;
        private COEDataViewBO theCOEDataviewBO;

        #endregion

        #region Properties

        public bool IsAuthenticated
        {
            get
            {
                if (this.principal == null)
                {
                    return false;
                }

                using (new PrincipalContext(this.principal))
                {
                    return COEPrincipal.IsAuthenticated;
                }
            }
        }

        /// <summary>
        /// Gets the framework server version
        /// </summary>
        public Version ServerVersion
        {
            get { return serverVersion; }
        }

        public bool IsFromCBVN { get; set; }

        public string CBVNFormName { get; set; }

        public COEHitList CBVNHitList { get; set; }

        public string UserName { get; set; }

        public string ServerName { get; set; }

        /// <summary>
        /// Gets the COE connection settings for use of remoting and data compression
        /// </summary>
        public COEPreferenceSettings TheCOEPreferenceSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the COEDataviewBO object which is currently used by Datalytix
        /// </summary>
        public COEDataViewBO TheCOEDataviewBO
        {
            get { return theCOEDataviewBO; }
            set
            {
                theCOEDataviewBO = value;
            }
        }

        #endregion

        public COEService()
        {
            //initailize an instance of COEConnectionSettings to set the remoting related preferences
            TheCOEPreferenceSettings = new COEPreferenceSettings();
        }

        #region Methods

        public void SetServer(string cslaDataPortalUrl, string cslaDataPortalProxy, string dataSourceName)
        {
            ConfigurationManager.AppSettings["CslaDataPortalUrl"] = cslaDataPortalUrl;
            ConfigurationManager.AppSettings["CslaDataPortalProxy"] = cslaDataPortalProxy;

            // We want the SpotFire Addin to use the COEFramework configuration data used by 
            // ChemBioViz .Net so we set the same AppName
            ConfigurationManager.AppSettings["AppName"] = "CBVN";

            if (dataSourceName != null)
            {
                DBMSTypeData dbmsd = ConfigurationUtilities.GetDBMSTypeData(DBMSType.ORACLE);
                dbmsd.DataSource = dataSourceName;
            }
        }

        // copied from FormDBLib.SetServer
        private void SetServer(String serverName)
        {
            // factored out, shared by the two login methods below
            bool bIs2Tier = CBVUtil.StartsWith(serverName, CBVConstants.MODE_2T);
            if (bIs2Tier)
            {
                ConfigurationManager.AppSettings["CslaDataPortalUrl"] = "";
                ConfigurationManager.AppSettings["CslaDataPortalProxy"] = null;

                this.ServerName = CBVUtil.BeforeFirstDelimiter(CBVUtil.AfterFirstDelimiter(serverName, '/').Trim(), '/').Trim();

                String sInputName = CBVUtil.AfterDelimiter(serverName, '/').Trim();
                if (!String.IsNullOrEmpty(sInputName))
                {
                    // following stmt throws exception if we already attempted a 3T login
                    DBMSTypeData dbmsd = ConfigurationUtilities.GetDBMSTypeData(DBMSType.ORACLE);
                    dbmsd.DataSource = sInputName;
                }
            }
            else
            {
                //check for after '/' delimiter when server name is provided as url
                this.ServerName = CBVUtil.AfterDelimiter(serverName, '/').Trim();
                if (string.IsNullOrEmpty(this.ServerName))
                {
                    //if not url then check for regular server name
                    this.ServerName = CBVUtil.BeforeFirstDelimiter(serverName, '/').Trim();
                }

                // otherwise set to configured proxy and given serve
                string proxyClass = @"Csla.DataPortalClient.WebServicesProxy";
                string proxyAssembly = @"Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30";

                // COESettings are read from a custom COESettings.cofig file using custom reading 
                // code referenced in AssemblySettings.dll
                if (TheCOEPreferenceSettings.UseRemoting)
                {
                    if (TheCOEPreferenceSettings.CompressData)
                    {
                        proxyClass = @"Csla.DataPortalClient.CompressedRemotingProxy";
                        proxyAssembly = @"Csla.Compression, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30";
                    }
                    else
                    {
                        proxyClass = @"Csla.DataPortalClient.RemotingProxy";
                    }
                }

                String pProxyEntry = string.Format("{0}, {1}", proxyClass, proxyAssembly);
                String sServerStr = MakeURL(serverName);
                ConfigurationManager.AppSettings["CslaDataPortalUrl"] = sServerStr;
                ConfigurationManager.AppSettings["CslaDataPortalProxy"] = pProxyEntry;
            }
        }

        // copied from CBVN MRUEntry and mdified to read from dll settings instead of appsettings
        private String MakeURL(String server)
        {
            // Pick between SOAP or Remoting portal based on app config setting
            string file = @"WebServicePortal.asmx";
            if (TheCOEPreferenceSettings.UseRemoting)
            {
                if (TheCOEPreferenceSettings.CompressData)
                {
                    file = @"RemotingPortalCompressed.rem";
                }
                else
                {
                    file = @"RemotingPortal.rem";
                }
            }

            if (CBVUtil.EndsWith(server, ".asmx") || CBVUtil.EndsWith(server, ".rem"))          // server string has the whole url
                return server;
            else if (CBVUtil.StartsWith(server, "http"))    // as in "https://servername"
                return String.Format("{0}/COEWebServiceHost/{1}", server, file);
            else if (server.Contains("(SSL)"))
                return String.Format("https://{0}/COEWebServiceHost/{1}", CBVUtil.BeforeDelimiter(server, '(').Trim(), file);
            else
                return String.Format("http://{0}/COEWebServiceHost/{1}", server, file);
        }


        internal string DataSourceName
        {
            get
            {
                DBMSTypeData dbmsd = ConfigurationUtilities.GetDBMSTypeData(DBMSType.ORACLE);
                return dbmsd.DataSource;
            }
        }

        public bool Login(string username, string password, string server)
        {
            this.UserName = username;
            SetServer(server);
            return Login(username, password);
        }

        internal bool Login(string username, string password)
        {
            using (new PrincipalContext())
            {
                bool success;

                COEPrincipal.Logout();
                try
                {
                    success = COEPrincipal.Login(username, password);
                    if (success)
                    {
                        //assign framework server version from COEConnectionInfo
                        serverVersion = COEPrincipal.COEConnectionInfo.ServerFrameworkVersion;
                        this.principal = Thread.CurrentPrincipal;
                    }
                }
                catch (Csla.DataPortalException)
                {
                    success = false;
                }

                return success;
            }
        }

        /// <summary>
        /// Method to log out from the system
        /// </summary>
        public static void LogOut()
        {
            using (new PrincipalContext())
            {
                try
                {
                    COEPrincipal.Logout();
                }
                catch
                {
                    throw;
                }
            }
        }

        public bool Login(string authenticationTicket)
        {
            // Don't login again if we're already authenticated with the same ticket.
            if (IsAuthenticated)
            {
                using (new PrincipalContext(this.principal))
                {
                    if (COEPrincipal.Token == authenticationTicket)
                    {
                        return true;
                    }
                }
            }

            using (new PrincipalContext())
            {
                bool success;

                COEPrincipal.Logout();
                try
                {
                    success = COEPrincipal.Login(authenticationTicket);
                    if (success)
                    {
                        this.principal = Thread.CurrentPrincipal;
                    }
                }
                catch (Csla.DataPortalException)
                {
                    success = false;
                }

                return success;
            }
        }

        public bool Login()
        {
            SpotfireCOEAuthenticationPromptModel loginModel = GetLoginModel();
            SpotfireCOEAuthenticationPromptView loginView = new SpotfireCOEAuthenticationPromptView(loginModel);
            if (loginView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Login(loginModel.Username, loginModel.Password, loginModel.Server))
                {
                    loginModel.Save();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method to login against the chem office enterprise server using the filled login model so that the Spotfire progress window can be displayed to user
        /// </summary>
        /// <param name="loginModel">Filled login model object</param>
        /// <returns>Returns boolean value</returns>
        public bool Login(SpotfireCOEAuthenticationPromptModel loginModel)
        {
            if (Login(loginModel.Username, loginModel.Password, loginModel.Server))
            {
                loginModel.Save();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get the filled instance of the SpotfireCOEAuthenticationPromptModel object
        /// </summary>
        /// <param name="loginModel">login model object</param>
        /// <returns>returns boolean value, true when model is filled and ok button is pressed else false</returns>
        public bool LoginModel(out SpotfireCOEAuthenticationPromptModel loginModel)
        {
            loginModel = GetLoginModel();
            SpotfireCOEAuthenticationPromptView loginView = new SpotfireCOEAuthenticationPromptView(loginModel);
            if (loginView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return true;
            }
            return false;
        }

        public SpotfireCOEAuthenticationPromptModel GetLoginModel()
        {
            SpotfireCOEAuthenticationPromptModel loginModel = new SpotfireCOEAuthenticationPromptModel();
            loginModel.MRUList = Properties.Settings.Default.LoginMRUList;
            return loginModel;
        }

        private COEHitListCache GetHitListCache(COEHitList hitList, bool forceReload, bool filterChildHits)
        {
            COEHitListCache hitListCache = new COEHitListCache();
            try
            {
                if (!forceReload)
                {
                    if (this.hitLists.TryGetValue(hitList, out hitListCache))
                    {
                        return hitListCache;
                    }
                }

                COEHitListCache.DataViewLoader dataViewLoader = new COEHitListCache.DataViewLoader(
                    delegate
                    {
                        return LoadDataViewBO(hitList);
                    });

                COEHitListCache.DataSetLoader dataSetLoader = new COEHitListCache.DataSetLoader(
                    delegate
                    {
                        return LoadDataSet(hitList, filterChildHits);
                    });

                this.hitLists[hitList] = hitListCache = new COEHitListCache(dataViewLoader, dataSetLoader);
            }
            catch (System.Security.SecurityException)
            {
                //handle security exception and throw again
                throw;
            }
            catch (Exception ex)
            {
                //bool isFormulaCriteria = false;
                if (ex.Message.Contains("ORA-00942: table or view does not exist"))
                {
                    throw new Exception("ORA-00942: table or view does not exist");
                }
                else
                {
                    #region Commented code
                    ////Same fix applied at GetHitListInfo
                    ////Verifying If Molecule Formula is invlaid
                    //if (hitList != null && hitList.SearchCriteria != null)
                    //{
                    //    for (int i = 0; i < hitList.SearchCriteria.Items.Count; i++)
                    //    {
                    //        SearchCriteria.SearchCriteriaItem itm = (SearchCriteria.SearchCriteriaItem)hitList.SearchCriteria.Items[i];
                    //        if (itm.Criterium is SearchCriteria.FormulaCriteria && !string.IsNullOrEmpty(itm.Criterium.Value))
                    //        {
                    //            hitList.SearchCriteria.Items.Remove(itm);
                    //            try
                    //            {
                    //                ExecuteUsingCOEPrincipal(delegate()
                    //                {
                    //                    isFormulaCriteria = true;
                    //                    GetHitListCache(hitList, forceReload, filterChildHits);
                    //                    //FormWizard.Properties.Resources.Molecular_Formula_Validation
                    //                    //If HitlistInfo is successful here then we can say that the Formula is invalid.
                    //                    //CSCARTRIDGE is throwing an error at: CSCARTRIDGE.FormulaContains
                    //                    throw new Exception("Invalid Formula");
                    //                });
                    //            }
                    //            catch
                    //            {
                    //                throw;
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (!isFormulaCriteria)
                    #endregion
                    throw;
                }
            }
            return hitListCache;
        }

        public COEDataViewBO LoadDataViewBO(COEHitList hitList)
        {
            COEDataViewBO dataViewBO = null;
            //if the local copy of dataview bo is availabel and the id of dataview is same as stored i COEHitList object then return the local copy; otherwise get the fresh copy of dataview from database
            if (this.theCOEDataviewBO != null && this.theCOEDataviewBO.ID == hitList.DataViewID)
            {
                dataViewBO = this.theCOEDataviewBO;
            }
            else
            {
                ExecuteUsingCOEPrincipal(delegate() { dataViewBO = COEDataViewBO.Get(hitList.DataViewID); });
                this.TheCOEDataviewBO = dataViewBO;
            }
            return dataViewBO;
        }

        private DataSet LoadDataSet(COEHitList hitList, bool filterChildHits)
        {
            DataSet dataSet = new DataSet();
            ExecuteUsingCOEPrincipal(delegate()
            {
                try
                {
                    COESearch search = new COESearch();
                    PagingInfo pi = new PagingInfo();
                    pi.Start = 1;
                    pi.RecordCount = hitList.NumHits;
                    pi.HitListType = hitList.HitListType;
                    pi.HitListID = hitList.HitListID;
                    pi.FilterChildData = filterChildHits;

                    DataSet dsLocal = new DataSet();
                    int rowsPerPage = TheCOEPreferenceSettings.PagingSize;  //Seeting page size from Spotfire Preferences for the current User's Group 
                    int totalRow = 0;
                    int prevTotalRow = 0;
                    int currentRowCount = 0;
                    do
                    {
                        int nFirst = 0;
                        int nLast = 0;
                        if (rowsPerPage > 0)
                        {
                            nFirst = (totalRow / rowsPerPage) * rowsPerPage;
                            nLast = nFirst + rowsPerPage - 1;

                            // pagingInfo already contains hitlistID unless retrieve all
                            pi.Start = nFirst + 1;
                            pi.End = nLast + 1;
                            pi.RecordCount = nLast - nFirst + 1;
                        }
                        else
                        {
                            pi.End = 0;
                            pi.RecordCount = 0;
                        }

                        //get dataset using dv id
                        dsLocal = search.GetData(hitList.ResultsCriteria, pi, hitList.DataViewID, "no", new OrderByCriteria(), hitList.SearchCriteria);

                        if (dsLocal != null && dsLocal.Tables.Count > 0)
                        {
                            prevTotalRow = totalRow;
                            if (dsLocal.Tables[0].Rows.Count > 0)
                            {
                                totalRow += rowsPerPage;
                                //set current row count from dataset base table
                                currentRowCount = dsLocal.Tables[0].Rows.Count;
                            }

                            foreach (DataTable dtLocal in dsLocal.Tables)
                            {
                                if (!dataSet.Tables.Contains(dtLocal.TableName))
                                {
                                    dataSet.Tables.Add(dtLocal.Copy());
                                }
                                else
                                {
                                    foreach (DataRow localRow in dtLocal.Rows)
                                    {
                                        DataRow newRow = dataSet.Tables[dtLocal.TableName].NewRow();

                                        foreach (DataColumn dctemp in dtLocal.Columns)
                                        {
                                            newRow[dctemp.ColumnName] = localRow[dctemp.ColumnName];
                                        }

                                        dataSet.Tables[dtLocal.TableName].Rows.Add(newRow);
                                    }
                                }
                            }
                        }
                    } while (prevTotalRow != totalRow && rowsPerPage == currentRowCount && rowsPerPage > 0); //condition added to check if current rows from dataset are less tha page size then break the loop

                }
                catch (Exception ex)
                {
                    string strMessage = GetExceptionMessage(ex);
                    Exception coeEx = GetCOEException(ex);
                    if (coeEx is CambridgeSoft.COE.Framework.Types.Exceptions.SQLGeneratorException)
                    {
                        strMessage = strMessage.Trim();
                        strMessage = strMessage.Replace(" ", "");
                        strMessage = strMessage.ToLower();
                        strMessage = strMessage.Replace("sqlgeneratorvertex", "");
                        strMessage = strMessage.Replace("notfound", "");

                        TableNameIdMapper theTableNameIdMapper = TableNameIdMapper.Instance();
                        if (theTableNameIdMapper != null)
                        {
                            try
                            {
                                string tableName = theTableNameIdMapper.TableNameIdCollectionFromFile[Convert.ToInt32(strMessage)];
                                throw new Exception(string.Format("Table or field - {0} in dataview is missing", tableName), coeEx);
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new Exception(string.Format("Table or field with id {0} not found", strMessage), coeEx);
                            }
                        }
                        else
                        {
                            throw new Exception("Table or field in dataview is missing", coeEx);
                        }
                    }
                    throw coeEx;
                }
            });
            return dataSet;
        }

        /// <summary>
        /// Gets the exception message from root exception
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <returns>rturns message from root level exception</returns>
        private string GetExceptionMessage(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }
            return GetExceptionMessage(ex.InnerException);
        }

        /// <summary>
        /// Gets the COE exception from current exception
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <returns>returns the COE exception object</returns>
        private Exception GetCOEException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex;
            }
            return GetCOEException(ex.InnerException);
        }

        public HitListInfo GetHitListInfo(int dataViewId, SearchCriteria searchCriteria)
        {
            HitListInfo hitListInfo = null;
            try
            {
                ExecuteUsingCOEPrincipal(delegate()
                {
                    COESearch coesearch = new COESearch();
                    hitListInfo = coesearch.GetHitList(searchCriteria, dataViewId);
                });
            }
            catch
            {
                #region CommentedCode
                //bool isFormulaCriteria = false;
                ////Same fix applied at GetHitListCache
                ////Verifying If Molecule Formula is invlaid
                //for (int i = 0; i < searchCriteria.Items.Count; i++)
                //{
                //    SearchCriteria.SearchCriteriaItem itm = (SearchCriteria.SearchCriteriaItem)searchCriteria.Items[i];
                //    if (itm.Criterium is SearchCriteria.FormulaCriteria && !string.IsNullOrEmpty(itm.Criterium.Value))
                //    {
                //        searchCriteria.Items.Remove(itm);
                //        try
                //        {
                //            ExecuteUsingCOEPrincipal(delegate()
                //            {
                //                isFormulaCriteria = true;
                //                GetHitListInfo(dataViewId, searchCriteria);
                //                //FormWizard.Properties.Resources.Molecular_Formula_Validation
                //                //If HitlistInfo is successful here then we can say that the Formula is invalid.
                //                //CSCARTRIDGE is throwing an error at: CSCARTRIDGE.FormulaContains
                //                throw new Exception("Invalid Formula");
                //            });
                //        }
                //        catch
                //        {
                //            throw;
                //        }
                //    }
                //}
                //if (!isFormulaCriteria)
                #endregion
                throw;
            }
            return hitListInfo;
        }

        /// <summary>
        /// Retrieve the data of the dataview based on the searchCriteria and Result Criteria
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="resultsCriteria"></param>
        /// <param name="dataviewId"></param>
        /// <param name="filterChildHits"></param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, int dataviewId, bool filterChildHits)
        {
            DataSet dsTemp = null;

            try
            {
                ExecuteUsingCOEPrincipal(delegate()
                {
                    COESearch coesearch = new COESearch();
                    PagingInfo pagingInfo = new PagingInfo();
                    pagingInfo.Start = 1;
                    pagingInfo.RecordCount = 0;//No Paging
                    pagingInfo.End = 0;
                    pagingInfo.FilterChildData = filterChildHits;
                    //passing DV object to COESearch instaed of DV id to avoid loading of DV copy from database again
                    dsTemp = coesearch.GetData(resultsCriteria, pagingInfo, this.TheCOEDataviewBO.COEDataView, "no", new OrderByCriteria(), searchCriteria);
                });
            }
            catch
            {
                throw;
            }
            return dsTemp;
        }

        /// <summary>
        /// Retrieve the data of the dataview based on the searchCriteria and Result Criteria
        /// </summary>
        /// <param name="searchCriteria">SearchCriteria object</param>
        /// <param name="resultsCriteria">ResultsCriteria object</param>
        /// <param name="dataview">COEDataView object</param>
        /// <param name="filterChildHits">boolean value to determine the child hits filtering</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, COEDataView dataview, bool filterChildHits)
        {
            DataSet dsTemp = null;
            try
            {
                ExecuteUsingCOEPrincipal(delegate()
                {
                    COESearch coesearch = new COESearch();
                    PagingInfo pagingInfo = new PagingInfo();
                    pagingInfo.Start = 1;
                    pagingInfo.RecordCount = 0;//No Paging
                    pagingInfo.End = 0;
                    pagingInfo.FilterChildData = filterChildHits;
                    //passing DV object to COESearch instaed of DV id to avoid loading of DV copy from database again
                    dsTemp = coesearch.GetData(resultsCriteria, pagingInfo, dataview, "no", new OrderByCriteria(), searchCriteria);
                });
            }
            catch
            {
                throw;
            }
            return dsTemp;
        }

        /// <summary>
        /// Method that will return the dataset based on the hitlist id specified
        /// </summary>
        /// <param name="hitList">COEHitList object contains search criteria and result criteria</param>
        /// <param name="hitListID">Hit list id for which the dataset needs to be generated</param>
        /// <returns>returns the dataset filled with the results returned for specified hitlist id</returns>
        public DataSet GetDataSet(COEHitList hitList, int hitListID, bool filterChildHits)
        {
            hitList.HitListID = hitListID;
            return GetDataSet(hitList, filterChildHits);
        }

        public DataSet GetDataSet(COEHitList hitList, bool filterChildHits)
        {
            try
            {
                COEHitListCache hitListCache = GetHitListCache(hitList, true, filterChildHits);
                return hitListCache.DataSet;
            }
            catch (System.Security.SecurityException sEx)
            {
                throw sEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetDataSet(COEHitList hitList, string tableName, int tableID, bool filterChildHits)
        {
            COEHitListCache hitListCache = GetHitListCache(hitList, false, filterChildHits);
            string tableNameFromDataset = string.Empty;

            CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable dataViewTable = hitListCache.DataViewBO.COEDataView.Tables.getById(tableID);
            if (dataViewTable != null)
            {
                tableName = !string.IsNullOrEmpty(dataViewTable.Alias) ? dataViewTable.Alias : dataViewTable.Name;
                tableNameFromDataset = "Table_" + dataViewTable.Id;
            }

            // Create a new DataSet for a child table.
            DataTable childTable = hitListCache.DataSet.Tables[tableNameFromDataset];
            DataSet childDataSet = new DataSet(tableName);
            DataTable copiedTable = childDataSet.Tables.Add(tableName);
            if (childTable != null)
            {
                copiedTable.Load(childTable.CreateDataReader());
            }
            return childDataSet;
        }

        public COEDataViewBO GetDataViewBO(COEHitList hitList, bool filterChildHits)
        {
            COEHitListCache hitListCache = GetHitListCache(hitList, false, filterChildHits);
            return hitListCache.DataViewBO;
        }

        public COEDataViewManagerBO GetDataViewManagerBO(COEDataView coeDataView)
        {
            COEDataViewManagerBO theCOEDataViewManagerBO = null;
            ExecuteUsingCOEPrincipal(delegate()
            {
                theCOEDataViewManagerBO = COEDataViewManagerBO.NewManager(coeDataView);
            });
            return theCOEDataViewManagerBO;
        }

        /// <summary>
        /// It will retruns the record count for hitlist search against hitlist.
        /// </summary>
        /// <param name="hitList"></param>
        /// <returns></returns>
        public int GetNewHitListRecordCount(COEHitList hitList, bool filterChildHits)
        {
            DataSet ds = GetDataSet(hitList, filterChildHits);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0].Rows.Count;
            return 0;
        }

        public COEDataView GetDataView(int dataViewId)
        {
            COEDataView dataView = null;
            //return if the dataview bo local copy is available and having same id as that of stored in coehitlist object; 
            //otherwise fetch a new copy and assign to local variable for further use
            if (this.theCOEDataviewBO != null && this.theCOEDataviewBO.ID == dataViewId)
            {
                dataView = this.TheCOEDataviewBO.COEDataView;
            }
            else
            {
                ExecuteUsingCOEPrincipal(delegate()
                {
                    this.TheCOEDataviewBO = COEDataViewBO.Get(dataViewId);
                });
                dataView = this.TheCOEDataviewBO.COEDataView;
            }
            return dataView;
        }

        public COEDataViewBOList GetDataViews()
        {
            COEDataViewBOList dataViewList = null;
            ExecuteUsingCOEPrincipal(delegate() { dataViewList = COEDataViewBOList.GetDataViewListAndNoMaster(); });
            return dataViewList;
        }

        internal delegate void Executable();
        internal void ExecuteUsingCOEPrincipal(Executable executable)
        {
            using (new PrincipalContext(this.principal))
            {
                executable.Invoke();
                this.principal = Thread.CurrentPrincipal;
            }
        }

        /// <summary>
        /// for getting the relation keys of child tables        
        /// </summary>
        /// <param name="dv">COEDataview</param>
        /// <returns></returns>        
        private Dictionary<int, int> GetTableKeys(COEDataView dv)
        {
            Dictionary<int, int> tableKeyList = new Dictionary<int, int>();

            //Collecting Child Tables
            foreach (COEDataView.Relationship rel in dv.Relationships)
            {
                if ((rel.Parent == dv.Basetable) && !tableKeyList.ContainsKey(rel.Child))
                {
                    tableKeyList.Add(rel.Child, rel.ChildKey);
                }
            }
            //Collecting Grand Child Tables
            foreach (COEDataView.Relationship rel in dv.Relationships)
            {
                if (tableKeyList.ContainsKey(rel.Parent) && !tableKeyList.ContainsKey(rel.Child))
                {
                    tableKeyList.Add(rel.Child, rel.ChildKey);
                }
            }
            return tableKeyList;
        }

        /// <summary>
        /// To validate child rows with max row limit        
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="resultsCriteria"></param>
        /// <param name="dataView"></param>
        /// <param name="maxRowLimit"></param>
        /// <param name="filterChildHits"></param>
        /// <param name="warningMessage"></param>
        /// <param name="messageTitle"></param>
        /// <returns>true if limit exceeded else false</returns>
        public bool CheckChildMaxRow(SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, COEDataView dataView, int maxRowLimit, bool filterChildHits, string warningMessage, string messageTitle)
        {
            bool IsExceedMaxRowLimit = false;
            try
            {
                ResultsCriteria newChildResultsCriteria = new ResultsCriteria();

                if (resultsCriteria.Tables.Count() > 1)
                {
                    ResultsCriteria.ResultsCriteriaTable rCriteriaTable = new ResultsCriteria.ResultsCriteriaTable(dataView.Basetable);
                    rCriteriaTable.Criterias = new List<ResultsCriteria.IResultsCriteriaBase>();
                    string[] ChildTableNames = new string[resultsCriteria.Tables.Count() - 1];
                    short childIndex = -1;
                    Dictionary<int, int> tablesKeyList = GetTableKeys(dataView);
                    foreach (ResultsCriteria.ResultsCriteriaTable rcTbl in resultsCriteria.Tables)
                    {
                        CambridgeSoft.COE.Framework.Common.ResultsCriteria.Field firstField = new CambridgeSoft.COE.Framework.Common.ResultsCriteria.Field();
                        COEDataView.DataViewTable dvTable = dataView.Tables.getById(rcTbl.Id);

                        if (rcTbl.Id == dataView.Basetable)
                        {
                            int baseTablePK = Convert.ToInt32(dataView.BaseTablePrimaryKey);
                            if (baseTablePK > 0)
                            {
                                firstField.Id = baseTablePK;
                                //coverity fix
                                COEDataView.Field field = dvTable.Fields.getById(baseTablePK);
                                if(field != null)
                                    firstField.Alias = field.Name;
                            }
                            else
                            {
                                if (dvTable.Fields.Count > 0)
                                {
                                    firstField.Id = dvTable.Fields[0].Id;
                                    firstField.Alias = dvTable.Fields[0].Name;
                                }
                            }
                            rCriteriaTable.Criterias.Add(firstField);
                            continue;
                        }
                        else
                        {
                            int tableKey = tablesKeyList[rcTbl.Id];
                            firstField.Id = tableKey;
                            //coverity fix
                            COEDataView.Field field = dvTable.Fields.getById(tableKey);
                            if(field != null)
                                firstField.Alias = field.Name;

                            ResultsCriteria.AggregateFunction aggregate = new ResultsCriteria.AggregateFunction();
                            aggregate.Parameters = new List<ResultsCriteria.IResultsCriteriaBase>(1);
                            aggregate.Parameters.Add(firstField);
                            aggregate.FunctionName = "count";

                            rCriteriaTable.Criterias.Add(aggregate);
                            ChildTableNames[++childIndex] = dvTable.Alias;//string.Format("{0} ({1})", dvTable.Name, dvTable.Alias);
                        }
                    }
                    newChildResultsCriteria.Add(rCriteriaTable);
                    System.Data.DataSet dsTemp = GetDataSet(searchCriteria, newChildResultsCriteria, dataView.DataViewID, filterChildHits);

                    if (dsTemp != null && dsTemp.Tables.Count > 0)
                    {
                        int[] ChildRowsCount = new int[dsTemp.Tables[0].Columns.Count - 1];
                        foreach (System.Data.DataRow dr in dsTemp.Tables[0].Rows)
                        {
                            for (int colIndex = 0; colIndex < ChildRowsCount.Length; colIndex++)
                            {
                                ChildRowsCount[colIndex] += Convert.ToInt32(dr[colIndex + 1]);
                            }
                        }
                        for (int colIndex = 0; colIndex < ChildRowsCount.Length; colIndex++)
                        {
                            if (ChildRowsCount[colIndex] > maxRowLimit)
                            {
                                System.Windows.Forms.MessageBox.Show(String.Format(warningMessage, ChildRowsCount[colIndex], ChildTableNames[colIndex], maxRowLimit), messageTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                                IsExceedMaxRowLimit = true;
                                break;
                            }
                        }
                    }
                }
            }
            //catch (System.Net.WebException we)
            //{
            //    IsExceedMaxRowLimit = true;
            //    if (we.Message.Equals("The operation has timed out"))
            //        System.Windows.Forms.MessageBox.Show("Unable to process.\nRemove table/tables from ResultsCriteria and try again.", messageTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

            //}
            catch
            {
                throw;
            }
            return IsExceedMaxRowLimit;
        }

        /// <summary>
        /// Method to get the row count from base table of the currently seelcted dataview
        /// </summary>
        /// <returns>returns the number of rows from base table</returns>
        public int GetFastRowCount()
        {
            int rowCount = 0;
            ExecuteUsingCOEPrincipal(delegate()
            {
                COESearch theCOESearch = new COESearch();
                rowCount = theCOESearch.GetFastRecordCount(TheCOEDataviewBO.COEDataView);
            });
            return rowCount;
        }

        #endregion

        #region Nested Classes

        private class PrincipalContext : IDisposable
        {
            private IPrincipal oldPrincipal;

            public PrincipalContext()
            {
                this.oldPrincipal = Thread.CurrentPrincipal;
            }

            public PrincipalContext(IPrincipal newPrincipal)
                : this()
            {
                if (newPrincipal != null)
                {
                    Thread.CurrentPrincipal = newPrincipal;
                }
            }

            public void Dispose()
            {
                Thread.CurrentPrincipal = this.oldPrincipal;
            }
        }

        /// <summary>
        /// Class to manage csla connection properties like use of remoting and data compression stored in Spotfire preferences
        /// </summary>
        public class COEPreferenceSettings
        {
            #region Variables
            bool useRemoting;
            bool compressData;
            int pagingSize;
            int maxRows;

            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets whether to use the remoting for CSLA connection
            /// </summary>
            public bool UseRemoting
            {
                get { return useRemoting; }
                set { useRemoting = value; }
            }

            /// <summary>
            /// Gets or sets whether to use the data compression for CSLA connection
            /// </summary>
            public bool CompressData
            {
                get { return compressData; }
                set { compressData = value; }
            }

            /// <summary>
            /// Gets or sets page row count from preferences
            /// </summary>
            public int PagingSize
            {
                get { return pagingSize; }
                set { pagingSize = value; }
            }

            /// <summary>
            /// Gets or sets the maximum row limit
            /// </summary>
            public int MaxRows
            {
                get { return maxRows; }
                set { maxRows = value; }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes an instance of the COEConnectionSettings  class
            /// </summary>
            internal COEPreferenceSettings()
            {

            }
            #endregion
        }

        #endregion
    }

    /// <summary>
    /// class to manage the table name and id from dxp file or datasource
    /// </summary>
    public class TableNameIdMapper
    {
        #region Variables
        Dictionary<int, string> tableNameIdCollection;
        Dictionary<int, string> tableNameIdCollectionFromFile;
        int _dataViewId;
        static TableNameIdMapper theTableNameIdMapper;
        static object lockObject = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or set the dataview id
        /// </summary>
        public int DataViewId
        {
            set
            {
                if (_dataViewId != value)
                {
                    _dataViewId = value;
                    tableNameIdCollection = new Dictionary<int, string>();
                    tableNameIdCollectionFromFile = new Dictionary<int, string>();
                }
            }
            get
            {
                return _dataViewId;
            }
        }

        /// <summary>
        /// Gets the table name id collection
        /// </summary>
        public Dictionary<int, string> TableNameIdCollection
        {
            get { return tableNameIdCollection; }
        }

        /// <summary>
        /// Gets the table name and id collection from file
        /// </summary>
        public Dictionary<int, string> TableNameIdCollectionFromFile
        {
            get { return tableNameIdCollectionFromFile; }
        }
        #endregion

        private TableNameIdMapper()
        {
            tableNameIdCollection = new Dictionary<int, string>();
            tableNameIdCollectionFromFile = new Dictionary<int, string>();
        }

        /// <summary>
        /// Initializes an instance of the TableNameIdMapper class
        /// </summary>
        /// <returns>returns object of TableNameIdMapper</returns>
        public static TableNameIdMapper Instance()
        {
            lock (lockObject)
            {
                if (theTableNameIdMapper == null)
                {
                    theTableNameIdMapper = new TableNameIdMapper();
                }
                return theTableNameIdMapper;
            }
        }

        /// <summary>
        /// Adds entry of table name and id to collection
        /// </summary>
        /// <param name="tableId">table id</param>
        /// <param name="tableName">table name</param>
        public void AddEntry(int tableId, string tableName)
        {
            if (!tableNameIdCollectionFromFile.ContainsKey(tableId))
            {
                tableNameIdCollectionFromFile.Add(tableId, tableName);
            }
        }
    }

}

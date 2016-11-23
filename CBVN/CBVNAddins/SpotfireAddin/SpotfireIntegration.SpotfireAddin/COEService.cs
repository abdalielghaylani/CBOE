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

namespace SpotfireIntegration.SpotfireAddin
{
    class COEService
    {
        #region Fields

        private IPrincipal principal = null;
        private Dictionary<COEHitList,COEHitListCache> hitLists = new Dictionary<COEHitList,COEHitListCache>();
        private Version serverVersion;

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

        #endregion

        #region Methods

        internal void SetServer(string cslaDataPortalUrl, string cslaDataPortalProxy, string dataSourceName)
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
                // otherwise set to configured proxy and given serve
                string proxyClass = @"Csla.DataPortalClient.WebServicesProxy";
                string proxyAssembly = @"Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30";
                
                // COESettings are read from a custom COESettings.cofig file using custom reading 
                // code referenced in AssemblySettings.dll
                if (COESettings.Default.UseRemoting)
                {

                    if (COESettings.Default.CompressRemotingData)
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
            if (COESettings.Default.UseRemoting)
            {
                if (COESettings.Default.CompressRemotingData)
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

        internal bool Login(string username, string password, string server)
        {
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

        internal bool Login(string authenticationTicket)
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

        internal bool Login()
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

        internal SpotfireCOEAuthenticationPromptModel GetLoginModel()
        {
            SpotfireCOEAuthenticationPromptModel loginModel = new SpotfireCOEAuthenticationPromptModel();
            loginModel.MRUList = Properties.Settings.Default.LoginMRUList;
            return loginModel;
        }

        private COEHitListCache GetHitListCache(COEHitList hitList, bool forceReload)
        {
            COEHitListCache hitListCache;

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
                    return LoadDataSet(hitList);
                });

            this.hitLists[hitList] = hitListCache = new COEHitListCache(dataViewLoader, dataSetLoader);
            return hitListCache;
        }

        private COEDataViewBO LoadDataViewBO(COEHitList hitList)
        {
            COEDataViewBO dataViewBO = null;
            ExecuteUsingCOEPrincipal(delegate() { dataViewBO = COEDataViewBO.Get(hitList.DataViewID); });
            return dataViewBO;
        }

        private DataSet LoadDataSet(COEHitList hitList)
        {
            DataSet dataSet = null;
            ExecuteUsingCOEPrincipal(delegate()
            {
                COESearch search = new COESearch();
                COEHitListBO hitListBO = COEHitListBO.Get(hitList.HitListType, hitList.HitListID);
                PagingInfo pi = new PagingInfo();
                pi.Start = 1;
                pi.RecordCount = hitListBO.NumHits;
                pi.HitListType = hitListBO.HitListType;
                pi.HitListID = hitListBO.HitListID;
                dataSet = search.GetData(hitList.ResultsCriteria, pi, hitList.DataViewID, "yes", new OrderByCriteria(),hitList.SearchCriteria);
            });
            return dataSet;
        }

        internal HitListInfo GetHitListInfo(int dataViewId, SearchCriteria searchCriteria)
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
                throw;
            }
            return hitListInfo;
        }

        /// <summary>
        /// Method that will return the dataset based on the hitlist id specified
        /// </summary>
        /// <param name="hitList">COEHitList object contains search criteria and result criteria</param>
        /// <param name="hitListID">Hit list id for which the dataset needs to be generated</param>
        /// <returns>returns the dataset filled with the results returned for specified hitlist id</returns>
        internal DataSet GetDataSet(COEHitList hitList, int hitListID)
        {
            hitList.HitListID = hitListID;
            return GetDataSet(hitList);
        }

        internal DataSet GetDataSet(COEHitList hitList)
        {
            COEHitListCache hitListCache = GetHitListCache(hitList, false);
            return hitListCache.DataSet;
        }

        internal DataSet GetDataSet(COEHitList hitList, string tableName)
        {
            COEHitListCache hitListCache = GetHitListCache(hitList, false);

            // Create a new DataSet for a child table.
            DataTable childTable = hitListCache.DataSet.Tables[tableName];
            DataSet childDataSet = new DataSet(tableName);
            DataTable copiedTable = childDataSet.Tables.Add(tableName);
            copiedTable.Load(childTable.CreateDataReader());

            return childDataSet;
        }

        internal COEDataViewBO GetDataViewBO(COEHitList hitList)
        {
            COEHitListCache hitListCache = GetHitListCache(hitList, false);
            return hitListCache.DataViewBO;
        }

        internal COEDataView GetDataView(int dataViewId)
        {
            COEDataView dataView = null;
            ExecuteUsingCOEPrincipal(delegate() { dataView = COEDataViewBO.Get(dataViewId).COEDataView; });
            return dataView;
        }

        internal COEDataViewBOList GetDataViews()
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

        #endregion
    }
}

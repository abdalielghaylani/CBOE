using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using Infragistics.Win.UltraWinTree;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using CBVUtilities;

namespace FormDBLib
{
    public class FormDbMgr
    {
        #region Variables
        private Login m_login;
        private MRUList m_mruList;
        private String m_authTicket;
        private COEDataViewBOList m_dataViews;
        private COEDataView m_selectedDataView;
        private String m_selectedDataViewBOName;
        private COEDataView.DataViewTable m_selectedDVTable;
        private List<COEDataView.DataViewTable> m_subTables;
        private int m_sessionID;
        private ResultsCriteria m_resultsCriteria;
        private ResultsCriteria m_fullResultsCriteria;
        private String m_dvNameOfFullCriteria;
        private int m_baseTableRecordCount;
        private DbObjectBank m_publicFormBank;
        private DbObjectBank m_privateFormBank;
        private int m_nodeKey = 0;
        private Form m_ownerForm;
        private PrivilegeChecker m_privChecker;
        private bool m_bGrandchildsOK;
        private bool m_bFilterChildHits;
        private static bool m_bShowSSSHilites;
        public static List<int> dataviews_Id_List = null;

        #endregion

        #region Properties
        public COEDataViewBOList DataViews
        {
            get { return m_dataViews; }
            set { m_dataViews = value; }
        }
        //---------------------------------------------------------------------
        public Login Login
        {
            get { return m_login; }
            set { m_login = value; }
        }
        //---------------------------------------------------------------------
        public MRUList MRUList
        {
            get { return m_mruList; }
            set { m_mruList = value; }
        }
        //---------------------------------------------------------------------
        public Form Owner
        {
            get { return m_ownerForm; }
        }
        //---------------------------------------------------------------------
        public int SessionID
        {
            get { return m_sessionID; }
        }
        //---------------------------------------------------------------------
        public int BaseTableRecordCount
        {
            get
            {
                if (m_baseTableRecordCount == -1 && m_selectedDataView != null)
                {
                    COESearch coeSearch = new COESearch();
                    m_baseTableRecordCount = coeSearch.GetExactRecordCount(m_selectedDataView);
                }
                return m_baseTableRecordCount;
            }
            set { m_baseTableRecordCount = value; }
        }
        //---------------------------------------------------------------------
        public String SelectedDataViewBOName
        {
            get { return m_selectedDataViewBOName; }
            set { m_selectedDataViewBOName = value; }
        }
        //---------------------------------------------------------------------
        public String AppName
        {
            get { return m_selectedDataViewBOName; }
            set { m_selectedDataViewBOName = value; }
        }
        //---------------------------------------------------------------------
        public String TableName
        {
            get { return (SelectedDataViewTable == null) ? "" : SelectedDataViewTable.Name; }
            set { if (SelectedDataViewTable != null) SelectedDataViewTable.Name = value; }
        }
        //---------------------------------------------------------------------
        public String TableAlias
        {
            get
            {
                return (SelectedDataViewTable == null) ? "" :
                    String.IsNullOrEmpty(SelectedDataViewTable.Alias) ? TableName :
                    SelectedDataViewTable.Alias;
            }
        }
        //---------------------------------------------------------------------
        public int AppID
        {
            get { return (SelectedDataView == null) ? 0 : SelectedDataView.DataViewID; }
            set { if (SelectedDataView != null) SelectedDataView.DataViewID = value; }
        }
        //---------------------------------------------------------------------
        public int TableID
        {
            get { return (SelectedDataViewTable == null) ? 0 : SelectedDataViewTable.Id; }
            set { if (SelectedDataViewTable != null) SelectedDataViewTable.Id = value; }
        }
        //---------------------------------------------------------------------
        public COEDataView SelectedDataView
        {
            get { return m_selectedDataView; }
            set { m_selectedDataView = value; }
        }
        //---------------------------------------------------------------------
        public COEDataView.DataViewTable SelectedDataViewTable
        {
            get { return m_selectedDVTable; }
            set { m_selectedDVTable = value; }
        }
        //---------------------------------------------------------------------
        public ResultsCriteria ResultsCriteria
        {
            get { return m_resultsCriteria; }
            set { m_resultsCriteria = value; }
        }
        //---------------------------------------------------------------------
        public ResultsCriteria FullResultsCriteria
        {
            // RC containing the entire dataview
            get { return m_fullResultsCriteria; }
            set { m_fullResultsCriteria = value; }
        }
        //---------------------------------------------------------------------
        public String DvNameOfFullCriteria
        {
            // name of dataview which FullResultsCriteria represents
            get { return m_dvNameOfFullCriteria; }
            set { m_dvNameOfFullCriteria = value; }
        }
        //---------------------------------------------------------------------
        public DbObjectBank PublicFormBank
        {
            get { return m_publicFormBank; }
            set { m_publicFormBank = value; }
        }
        //---------------------------------------------------------------------
        public DbObjectBank PrivateFormBank
        {
            get { return m_privateFormBank; }
            set { m_privateFormBank = value; }
        }
        //---------------------------------------------------------------------
        public PrivilegeChecker PrivilegeChecker
        {
            get { return m_privChecker; }
        }
        //---------------------------------------------------------------------
        public bool GrandChildFormsOK
        {
            get { return m_bGrandchildsOK; }
            set { m_bGrandchildsOK = value; }
        }
        //---------------------------------------------------------------------
        public bool FilterChildHits
        {
            get { return m_bFilterChildHits; }
            set { m_bFilterChildHits = value; }
        }
        //---------------------------------------------------------------------
        public static bool ShowSSSHilites
        {
            get { return m_bShowSSSHilites; }
            set { m_bShowSSSHilites = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public FormDbMgr(Form owner)
        {
            m_baseTableRecordCount = -1;
            m_selectedDataView = null;
            m_selectedDVTable = null;
            m_resultsCriteria = null;
            m_ownerForm = owner;
            m_fullResultsCriteria = null;
            m_subTables = null;
            m_mruList = null;
            m_privChecker = new PrivilegeChecker();
            m_bGrandchildsOK = true;
            m_bFilterChildHits = true;
        }
        #endregion

        #region Methods
        private ResultsCriteria CloneRC(ResultsCriteria rc)
        {
            ResultsCriteria rcClone = new ResultsCriteria();
            if (rc != null)
            {
                String rcXml = rc.ToString();
                rcClone.GetFromXML(rcXml);
            }
            return rcClone;
        }
        //---------------------------------------------------------------------
        public List<String> GetBaseFieldNames(bool bUseAliases, bool bExcludeStructure)
        {
            // returns all fields in dataview, no child tables
            List<String> names = new List<string>();
            COEDataView.DataViewTable t = this.SelectedDataViewTable;
            foreach (COEDataView.Field f in t.Fields)
            {
                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                if (bExcludeStructure && bIsStructure)
                    continue;
                if (f.Visible == false) continue;   // CSBR-131778
                String name = f.Name;
                if (bUseAliases && !String.IsNullOrEmpty(f.Alias))
                    name = f.Alias;
                names.Add(name);
            }
            return names;
        }
        //---------------------------------------------------------------------
        // Add fields to delimiter export without the structure
        private void AddFieldNamesToDelimList(COEDataView.DataViewTable t, List<String> names, bool bMainTable)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                if (f.Visible == false) continue;   // CSBR-131778
                String name = f.Name;
                if (bIsStructure && bMainTable)
                {
                    names.Add("Formula");
                    names.Add("Molweight");
                }
                if (!String.IsNullOrEmpty(f.Alias)) // CSBR-117750: use alias
                    name = f.Alias;
                if (!bMainTable)
                { 
					// CBOE-303, CBOE-1763, CBOE-1764 removed the ":" and placed "."
                    name = String.Concat(t.Alias, ".", name);
                    if (bIsStructure)
                    {
                        names.Add(String.Concat(t.Alias, ".", "Formula"));
                        names.Add(String.Concat(t.Alias, ".", "Molweight"));
                    }
                }
                if (f.IndexType != COEDataView.IndexTypes.CS_CARTRIDGE)
                    names.Add(name);
            }
        }
        //----------------------------------------------------------------------
        private void AddFieldNamesToList(COEDataView.DataViewTable t, List<String> names, bool bMainTable)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                if (f.Visible == false) continue;   // CSBR-131778
                String name = f.Name;
                if (bIsStructure && bMainTable)
                {
                    names.Add("Formula");
                    names.Add("Molweight");
                }
                if (!String.IsNullOrEmpty(f.Alias)) // CSBR-117750: use alias
                    name = f.Alias;
                if (!bMainTable)
                {
                    name = String.Concat(t.Name, ":", name);
                    if (bIsStructure)
                    {
                        names.Add(String.Concat(t.Name, ":", "Formula"));
                        names.Add(String.Concat(t.Name, ":", "Molweight"));
                    }
                }
                names.Add(name);
            }
        }
        public List<String> GetFieldNames(bool bWithChildFields, bool bWithGrandchildFields)
        {
            // returns all fields in dataview, including child tables (as "table:field")
            // CSBR-152679: Don't just *assume* the first table is the base table!
            List<String> names = new List<string>();
            COEDataView dataView = this.SelectedDataView;
            COEDataView.DataViewTable mainTable = dataView.Tables.getById(dataView.Basetable);
            AddFieldNamesToList(mainTable, names, true);
            foreach (COEDataView.DataViewTable t in dataView.Tables)
            {
                bool bMainTable = (t.Id == dataView.Basetable);
                if (bMainTable)                                   // already done                    
                    continue;
                // Not necessary for Table's with no relation
                else if (IsChildTable(t) || IsGrandchildTable(t)) //Fixed 151872 check whether the table is child or Grandchild before adding field names to list
                    AddFieldNamesToList(t, names, false);
            }
            return names;
        }

        //---------------------------------------------------------------------

        public List<String> GetDelimExportFieldNames()
        {
            // returns all fields in dataview, including child tables (as "table:field")            
            List<String> names = new List<string>();
            COEDataView dataView = this.SelectedDataView;
            COEDataView.DataViewTable mainTable = dataView.Tables.getById(dataView.Basetable);
            AddFieldNamesToDelimList(mainTable, names, true);
            foreach (COEDataView.DataViewTable t in dataView.Tables)
            {
                bool bMainTable = (t.Id == dataView.Basetable);
                if (bMainTable)                                   // already done                    
                    continue;
                // Not necessary for Table's with no relation
                else if (IsChildTable(t))
                    AddFieldNamesToDelimList(t, names, false);
            }
            return names;
        }
        //---------------- CBOE-1763 and CBOE-1764 ---------------------------
        /// <summary>
        /// get all the field names for sdf snd excel export types
        /// </summary>
        /// <param name="bWithChildFields"></param>
        /// <param name="bWithGrandchildFields"></param>
        /// <returns></returns>
        public List<String> GetExportFieldNames(bool bWithChildFields, bool bWithGrandchildFields)
        {
            // returns all fields in dataview, including child tables (as "table:field")
            // CSBR-152679: Don't just *assume* the first table is the base table!
            List<String> names = new List<string>();
            COEDataView dataView = this.SelectedDataView;
            COEDataView.DataViewTable mainTable = dataView.Tables.getById(dataView.Basetable);
            AddExportFieldNamesToList(mainTable, names, true);
            foreach (COEDataView.DataViewTable t in dataView.Tables)
            {
                bool bMainTable = (t.Id == dataView.Basetable);
                if (bMainTable)                                   // already done                    
                    continue;
                // Not necessary for Table's with no relation
                else if (IsChildTable(t) || IsGrandchildTable(t)) //Fixed 151872 check whether the table is child or Grandchild before adding field names to list
                    AddExportFieldNamesToList(t, names, false);
            }
            return names;
        }

        private void AddExportFieldNamesToList(COEDataView.DataViewTable t, List<String> names, bool bMainTable)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                if (f.Visible == false) continue;   // CSBR-131778
                String name = f.Name;
                if (bIsStructure && bMainTable)
                {
                    names.Add("Formula");
                    names.Add("Molweight");
                }
                if (!String.IsNullOrEmpty(f.Alias)) // CSBR-117750: use alias
                    name = f.Alias;
                if (!bMainTable)
                {
                    name = String.Concat(t.Alias, ".", name);
                    if (bIsStructure)
                    {
                        names.Add(String.Concat(t.Alias, ".", "Formula"));
                        names.Add(String.Concat(t.Alias, ".", "Molweight"));
                    }
                }
                names.Add(name);
            }
        }
        // End
        //---------------------------------------------------------------------
        public static bool IsNumericField(COEDataView.Field f)
        {
            return (f.DataType == COEDataView.AbstractTypes.Integer ||
                    f.DataType == COEDataView.AbstractTypes.Real);
        }
        //---------------------------------------------------------------------
        public bool Select(String appName, String tableName, bool bWithSubforms)
        {
            // select given app/table for future fetches; create RC
            // TO DO: select by ID instead?

            //m_bGrandchildsOK = bWithGrandchilds;

            if (!SelectDataView(appName))
                return false;
            if (!SelectDataViewTable(tableName))
                return false;

            m_resultsCriteria = CreateResultsCriteria(bWithSubforms);
            m_baseTableRecordCount = -1;	// recount when needed

            // make a copy of the new rc .. it represents the full dataview
            m_fullResultsCriteria = CloneRC(m_resultsCriteria);
            return true;
        }
        //---------------------------------------------------------------------

        #region Login methods
        //---------------------------------------------------------------------
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
                this.ClearCSLAObjects(proxyAssembly);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseRemoting"]))
                {

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["CompressRemotingData"]))
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
                String sServerStr = MRUEntry.MakeURL(serverName);
                ConfigurationManager.AppSettings["CslaDataPortalUrl"] = sServerStr;
                ConfigurationManager.AppSettings["CslaDataPortalProxy"] = pProxyEntry;
            }
        }
        //----------------------------------------------------------------------
        private void ClearCSLAObjects(string proxyAssembly) // Clears Data portal proxy when ever User attempt's to login with invalid Server name.
        {
            Type type = null;
            object obj = null;
            Assembly assembly = AppDomain.CurrentDomain.Load(proxyAssembly.Trim());
            type = assembly.GetType("Csla.DataPortal");
            FieldInfo field = type.GetField("_portal", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(obj, null);
        }
        //---------------------------------------------------------------------        
        private bool ChangingMode(MRUEntry mru1, MRUEntry mru2)
        {
            bool bIs2Tier1 = CBVUtil.StartsWith(mru1.Server, CBVConstants.MODE_2T);
            bool bIs2Tier2 = CBVUtil.StartsWith(mru2.Server, CBVConstants.MODE_2T);
            return bIs2Tier1 != bIs2Tier2;
        }
        //---------------------------------------------------------------------
        public bool DoLoginDialog()
        {
            m_login = new Login(m_mruList);
            MRUEntry mruFirstTry = new MRUEntry();

            // allow three tries to get it right
            // return false if user cancels or fails to login; will cause app to exit

            for (int tryNum = 0; tryNum < 3; ++tryNum)
            {
                // show login dialog
                DialogResult result = m_login.ShowDialog();
                if (result == DialogResult.Cancel)
                    break;

                // get current data from dialog as mru entry
                MRUEntry mru = m_login.CurrentMRU;

                // do not allow empty username
                if (String.IsNullOrEmpty(mru.UserName))
                    continue;

                // do not allow change between 2T and 3T after first attempt
                if (tryNum == 0)
                {
                    mruFirstTry = mru;
                }
                else if (ChangingMode(mru, mruFirstTry))
                {
                    MessageBox.Show("To change between 2-Tier and 3-Tier, cancel and restart the application.");
                    if (tryNum > 1) --tryNum;   // don't count this in the number of tries
                    continue;
                }

                // set ConfigurationManager.AppSettings for 2T vs 3T
                SetServer(mru.Server);

                // login
                try
                {
                    Debug.WriteLine("Login with user= " + mru.UserName + " pwd= " + mru.DecrPasswd + " server= " + mru.Server);

                    CBVTimer.StartTimer(true, "Login", true);
                    COEPrincipal.Login(mru.UserName, mru.DecrPasswd);
                    CBVTimer.EndTimer();

                    // on success, add mru to list and/or bring it to top
                    m_login.AddOrSelectMRU(mru);
                    m_sessionID = COEUser.SessionID;
                    return true;
                }
                catch (Exception ex)
                {
                    CBVUtil.ReportError(ex, "Login failure");
                    // continue to next try
                }
            }
            m_login = null;
            return false;
        }
        //---------------------------------------------------------------------
        public bool DoLogin(String username, String password, String serverName)
        {
            // login without dialog -- for command-line login
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(serverName))
            {
                m_login = new Login(m_mruList);
                m_login.UserName = username;
                m_login.Password = password;
                m_login.SavePasswd = true;

                MRUEntry mru = new MRUEntry(serverName, username, password, true);
                m_login.AddOrSelectMRU(mru);
                SetServer(serverName);

                try
                {
                    CBVTimer.StartTimer(true, "Login", true);
                    COEPrincipal.Login(m_login.UserName, m_login.Password);
                    CBVTimer.EndTimer();
                    m_sessionID = COEUser.SessionID;
                    return true;
                }
                catch (Exception ex)
                {
                    CBVUtil.ReportError(ex);
                    m_login = null;
                    m_sessionID = 0;
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nYou need to provide user, password and server name");
                return false;
            }
        }
        //---------------------------------------------------------------------
        public String GetUpdatedTicket(String authTicket)
        {
            String s = authTicket;
            coesinglesignon.SingleSignOn sso = new coesinglesignon.SingleSignOn();
            sso.Url = String.Format("http://{0}/COESingleSignOn/singlesignon.asmx", this.Login.CurrentMRU.Server);
            bool bValid = false;
            try
            {
                bValid = sso.ValidateTicket(authTicket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                bValid = false;
            }
            if (!bValid)
                s = sso.GetAuthenticationTicket(m_login.UserName, m_login.Password).ToString();
            else
                s = sso.RenewTicket(authTicket);
            return s;
        }
        //---------------------------------------------------------------------
        public bool DoLoginWithTicket(String authTicket, String serverName)
        {
            // to obtain a ticket, go to: http://mothball/coesinglesignon/singlesignon.asmx

            coesinglesignon.SingleSignOn sso = new coesinglesignon.SingleSignOn();
            sso.Url = String.Format("http://{0}/COESingleSignOn/singlesignon.asmx", serverName);
            bool bValid = false;
            try
            {
                bValid = sso.ValidateTicket(authTicket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                bValid = false;
            }
            if (!bValid)
                return false;
            String username = sso.GetUserFromTicket(authTicket);

            m_login = new Login(m_mruList);
            m_login.UserName = username;    // suggested by C. Keenan
            MRUEntry mru = new MRUEntry(serverName, username, "", false);
            m_login.AddOrSelectMRU(mru);
            SetServer(serverName);

            try
            {
                Debug.WriteLine("Login with ticket of length " + authTicket.Length.ToString());

                CBVTimer.StartTimer(true, "Ticket Login", true);
                COEPrincipal.Login(authTicket);
                CBVTimer.EndTimer();

                // add mru to list and/or bring it to top
                m_login.AddOrSelectMRU(mru);
                m_sessionID = COEUser.SessionID;
                m_authTicket = authTicket;

                // start a timer for renewals
                Timer t = new Timer();
                t.Interval = 600000;  // 10 mins
                t.Tick += new EventHandler(t_Tick);
                t.Start();
                return true;
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex, "Ticket login failure");
            }
            return false;
        }
        //---------------------------------------------------------------------
        void t_Tick(object sender, EventArgs e)
        {
            coesinglesignon.SingleSignOn sso = new coesinglesignon.SingleSignOn();
            m_authTicket = sso.RenewTicket(m_authTicket);
            Debug.WriteLine("TICKET RENEWED");
        }
        //---------------------------------------------------------------------
        public bool LoginIfNeeded()
        {
            if (m_login == null)
                return DoLoginDialog();
            return true;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Dataviews methods
        public bool GetDataViews()
        {
            CBVTimer.StartTimerWithMessage(true, "Retrieving data views", true);
            try
            {
#if DEBUG
                bool bUseCached = true;     // disable to avoid caching
                if (bUseCached && this.HaveCachedDataViews())
                {
                    Debug.WriteLine("RETRIEVING CACHED DATAVIEWS");
                    RetrieveCachedDataViews();
                    CBVTimer.EndTimer();
                    return true;
                }
#endif
                m_dataViews = COEDataViewBOList.GetDataViewListAndNoMaster();

                if (m_selectedDataView != null)
                {
                    foreach (COEDataViewBO item in m_dataViews)
                    {
                        if (m_selectedDataView.DataViewID == item.COEDataView.DataViewID)
                        {
                            m_selectedDataView = item.COEDataView;
                            break;
                        }
                    }
                }

                dataviews_Id_List = new List<int>();
                DbObjectBank.m_dv_ID_name_pair = new Dictionary<int, string>();
                foreach (COEDataViewBO dataview in m_dataViews)
                {
                    dataviews_Id_List.Add(dataview.ID);
                    DbObjectBank.m_dv_ID_name_pair.Add(dataview.ID, dataview.Name);
                }

#if DEBUG
                string saveQueryHistory = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationDefaultsData().SearchServiceData.SaveQueryHistory;
                if (string.IsNullOrEmpty(saveQueryHistory) || (saveQueryHistory.ToLower() != "yes" && saveQueryHistory != "true"))
                    Debug.WriteLine("SaveQueryHistory must be enabled in order to filter child data");

                bool bSaveToCache = false;  // enable if dataviews change and need to be re-saved
                if (bSaveToCache)
                {
                    DumpDataViews();
                    SaveDataViews();
                    Debug.WriteLine("SAVED CACHED DATAVIEWS");
                }
#endif
            }
            catch (System.Exception e)
            {
                throw new FormDBLib.Exceptions.ObjectBankException(string.Concat("Cannot retrieve data views: ", e.Message));
            }
            finally
            {
                CBVTimer.EndTimer();
            }
            return true;
        }
        //---------------------------------------------------------------------
        private UltraTreeNode AddTableAndChildren(COEDataViewBO dataView, COEDataView.DataViewTable table,
                                                UltraTree treeView, UltraTreeNode parentNode, List<int> idsUsed)
        {
            // recursive: add given table to tree, then add children
            if (idsUsed.Contains(table.Id))
                return null;

            // display friendly name if available
            String tableDisplayName = table.Alias;
            if (String.IsNullOrEmpty(tableDisplayName))
                tableDisplayName = table.Name;
            UltraTreeNode tnode = parentNode.Nodes.Add(m_nodeKey.ToString(), tableDisplayName);

            m_nodeKey++;
            idsUsed.Add(table.Id);

            // find children using relations table, then call this routine recursively
            foreach (COEDataView.Relationship rel in dataView.COEDataView.Relationships)
            {
                if (rel.Parent == table.Id)
                {
                    COEDataView.DataViewTable subtable = FindDVTableByID(rel.Child, dataView.COEDataView);
                    if (subtable != null)
                        AddTableAndChildren(dataView, subtable, treeView, tnode, idsUsed);
                }
            }
            return tnode;
        }
        //---------------------------------------------------------------------
        public static bool IsChildTable(COEDataView.DataViewTable dvTable, COEDataViewBO dataView)
        {
            bool result = false;
            int relationParent = -1;
            // true if table is listed among relations as being a child
            foreach (COEDataView.Relationship rel in dataView.COEDataView.Relationships)
            {
                if (rel.Child == dvTable.Id)
                {
                    relationParent = rel.Parent;
                    result = true;
                    break;
                }
            }

            if (relationParent != -1)
            {
                //check for circular dependency
                if (AreAllChildTables(dataView))
                {
                    foreach (COEDataView.Relationship rel in dataView.COEDataView.Relationships)
                    {
                        if (rel.Parent == dvTable.Id && rel.Child != relationParent)
                        {
                            result = false;
                            break;
                        }
                        result = true;
                    }
                }
            }

            return result;
        }
        //---------------------------------------------------------------------
        private static bool AreAllChildTables(COEDataViewBO dataView)
        {
            bool result = true;
            bool isChild = false;
            foreach (var table in dataView.COEDataView.Tables)
            {
                // true if table is listed among relations as being a child
                foreach (COEDataView.Relationship rel in dataView.COEDataView.Relationships)
                {
                    if (rel.Child == table.Id)
                    {
                        isChild = true;
                        break;
                    }   
                    isChild = false;
                }
                if (!isChild)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        //---------------------------------------------------------------------
        public static bool IsLookupTable(COEDataView.DataViewTable dvTable, COEDataViewBO dataView)
        {
            // true if table is not linked with any table
            foreach (COEDataView.Relationship rel in dataView.COEDataView.Relationships)
                if (rel.Parent == dvTable.Id)
                {
                    COEDataView.DataViewTable subtable = FindDVTableByID(rel.Child, dataView.COEDataView);
                    if (subtable != null)
                        return false;
                }
            return true;
        }
        //---------------------------------------------------------------------
        public void DataViewsToTree(UltraTree treeView)
        {
            m_nodeKey = 0;

            if (m_dataViews != null && treeView != null)
            {
                treeView.Nodes.Clear();

                // loop dataviews, adding each with its tables to treeview
                // CSBR-156114 Change to foreach to a for loop so we can address list
                // items to replace dataviews that were not properly fetched
                COEDataViewBO dataView = null;
                for (int i = 0; i < m_dataViews.Count; i++)
                {
                    dataView = (COEDataViewBO)m_dataViews[i];
                    List<int> idsUsed = new List<int>();
                    UltraTreeNode node = treeView.Nodes.Add(m_nodeKey.ToString(), dataView.Name);
                    m_nodeKey++;
                    UltraTreeNode lookupNode = new UltraTreeNode(m_nodeKey.ToString(), CBVConstants.LOOKUP_NAME);
                    m_nodeKey++;
                    if (dataView.COEDataView.Tables.Count == 0)
                    {
                        COEDataViewBO dataViewByID = COEDataViewBO.Get(dataView.ID);
                        // CSBR-156114 Store the newly fetched dv in the list of dataviews
                        // so the correct dv will be there for later use
                        m_dataViews[i] = dataViewByID;
                        TablesToTree(dataViewByID, treeView, lookupNode, node, idsUsed);
                    }
                    else
                    {
                        TablesToTree(dataView, treeView, lookupNode, node, idsUsed);
                    }
                    //adding lookup node only when it contains any table
                    if (lookupNode.Nodes.Count > 0)
                        node.Nodes.Add(lookupNode); // appending Lookup node as last node for a dataview
                }
                treeView.HideSelection = false;
                treeView.Refresh();
            }
        }

        private void TablesToTree(COEDataViewBO dataView, UltraTree treeView, UltraTreeNode lookupNode, UltraTreeNode node, List<int> idsUsed)
        {
            foreach (COEDataView.DataViewTable dataViewTable in dataView.COEDataView.Tables)
            {
                if (IsChildTable(dataViewTable, dataView))  // do not add children before parents
                    continue;
                if ((IsLookupTable(dataViewTable, dataView)) && !(string.Equals(dataViewTable.Name, dataView.BaseTable)))
                {
                    lookupNode.Nodes.Add(m_nodeKey.ToString(), dataViewTable.Alias);
                    m_nodeKey++;
                    idsUsed.Add(dataViewTable.Id);
                }
                else
                {
                    UltraTreeNode tableNode = AddTableAndChildren(dataView, dataViewTable, treeView, node, idsUsed);
                    if (tableNode != null && dataView.COEDataView == m_selectedDataView && dataViewTable == m_selectedDVTable)
                        treeView.ActiveNode = tableNode;
                }
            }

        }
        //---------------------------------------------------------------------
        private void AddItem(ref String s, String token)
        {
            if (!String.IsNullOrEmpty(s))
                s += ",";
            if (token.Contains(" "))
                token = String.Concat("\"", token, "\"");
            s += token;
        }
        //---------------------------------------------------------------------
        public void DumpDVToCSV(String dvname)
        {
            COEDataViewBO dvbo = FindDVBOByName(dvname);
            if (dvbo == null) return;
            COEDataView dataView = dvbo.COEDataView;

            String header = String.Empty;
            AddItem(ref header, "Table");
            AddItem(ref header, "Table_ID");
            AddItem(ref header, "Table_Alias");
            AddItem(ref header, "Field");
            AddItem(ref header, "Field_ID");
            AddItem(ref header, "Field_Alias");
            AddItem(ref header, "Visible");
            AddItem(ref header, "Datatype");
            Debug.WriteLine(header);

            foreach (COEDataView.DataViewTable dvt in dataView.Tables)
            {
                foreach (COEDataView.Field f in dvt.Fields)
                {
                    String sData = String.Empty;
                    AddItem(ref sData, dvt.Name);
                    AddItem(ref sData, CBVUtil.IntToStr(dvt.Id));
                    AddItem(ref sData, dvt.Alias);
                    AddItem(ref sData, f.Name);
                    AddItem(ref sData, CBVUtil.IntToStr(f.Id));
                    AddItem(ref sData, f.Alias);
                    AddItem(ref sData, f.Visible.ToString());
                    AddItem(ref sData, f.DataType.ToString());
                    Debug.WriteLine(sData);
                }
            }
        }
        //---------------------------------------------------------------------
        public void DumpDataViews()
        {
            foreach (COEDataViewBO dataView in m_dataViews)
            {
                Debug.WriteLine(String.Concat("Dataview ", dataView.Name, " dvid ", dataView.ID));

                String sXml = dataView.COEDataView.ToString();
                Debug.WriteLine(sXml);

                foreach (COEDataView.DataViewTable dataViewTable in dataView.COEDataView.Tables)
                {
                    Debug.WriteLine(String.Concat("   Table ", dataViewTable.Name, " ", dataViewTable.Id, " Alias ", dataViewTable.Alias));

                    foreach (COEDataView.Field f in dataViewTable.Fields)
                        Debug.WriteLine(String.Concat("      Field ", f.Name, " ", f.Id, " Alias ", f.Alias));
                }
                foreach (COEDataView.Relationship relation in dataView.COEDataView.Relationships)
                    Debug.WriteLine(String.Concat("   Relation ", relation.Child, " ", relation.Parent));
            }
        }
        //---------------------------------------------------------------------
        public bool SelectDataViewByID(int dvid)
        {
            return SelectDataViewByNameOrID(null, dvid);
        }
        //---------------------------------------------------------------------
        public bool SelectDataView(String dvname)
        {
            return SelectDataViewByNameOrID(dvname, 0);
        }
        //---------------------------------------------------------------------
        public void CheckForStaleRCs()
        {
            COEDataView selDataView = m_selectedDataView;
            int selDVID = selDataView.DataViewID;
            int baseTableID = selDataView.Basetable;
            int baseTableRC = -1, baseTableFullRC = -1;

            ResultsCriteria curRC = this.ResultsCriteria;
            //Coverity Bug Fix : CID 13030 
            if (curRC != null && curRC.Tables.Count > 0)
            {
                baseTableRC = curRC.Tables[0].Id;

                ResultsCriteria curRCFull = this.FullResultsCriteria;
                if (curRCFull != null && curRCFull.Tables.Count > 0)
                {
                    baseTableFullRC = curRC.Tables[0].Id;
                }
            }
            if (baseTableID != baseTableRC || baseTableID != baseTableFullRC)
            {
                this.ResultsCriteria = null;
                this.FullResultsCriteria = null;
            }
        }
        //---------------------------------------------------------------------
        public bool SelectDataViewByNameOrID(String dvname, int dvid)
        {
            bool ret = false;
            if (m_dataViews != null)
            {
                foreach (COEDataViewBO dataView in m_dataViews)
                {
                    bool bMatch = (dvid > 0 && dvid == dataView.ID) ||
                                  (!String.IsNullOrEmpty(dvname) && CBVUtil.Eqstrs(dvname, dataView.Name));
                    if (bMatch)
                    {
                        m_selectedDataView = dataView.COEDataView;
                        m_selectedDataViewBOName = dataView.Name;
                        CheckForStaleRCs();
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }
        //---------------------------------------------------------------------
        public COEDataViewBO FindDVBOByName(String dvname)
        {
            if (m_dataViews != null)
            {
                foreach (COEDataViewBO dataView in m_dataViews)
                {
                    if (CBVUtil.Eqstrs(dvname, dataView.Name))
                        return dataView;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public COEDataView FindDVByName(String dvname)
        {
            COEDataViewBO dvbo = FindDVBOByName(dvname);
            if (dvbo != null)
                return dvbo.COEDataView;
            return null;
        }
        //---------------------------------------------------------------------
        public void UnselectDataView()
        {
            m_selectedDataView = null;
            m_selectedDataViewBOName = "";
        }
        //---------------------------------------------------------------------
        public bool IsGrandchildTable(COEDataView.DataViewTable subtable)
        {
            int baseTableID = m_selectedDataView.Basetable;
            foreach (COEDataView.Relationship rel in m_selectedDataView.Relationships)
            {
                if (rel.Child == subtable.Id && rel.Parent != baseTableID)
                    return true;
            }
            return false;
        }
        // Fixed 151872 
        public bool IsChildTable(COEDataView.DataViewTable subTable)
        {

            int baseTableID = m_selectedDataView.Basetable;
            foreach (COEDataView.Relationship rel in m_selectedDataView.Relationships)
            {
                if (rel.Child == subTable.Id && rel.Parent == baseTableID)
                    return true;
            }
            return false;
        } // end
        //---------------------------------------------------------------------
        private void GatherSubtables(int parentTableID)
        {
            foreach (COEDataView.Relationship rel in m_selectedDataView.Relationships)
            {
                if (rel.Parent == parentTableID)
                {
                    COEDataView.DataViewTable subtable = FindDVTableByID(rel.Child);
                    if (subtable != null)
                    {
                        m_subTables.Add(subtable);
                        if (m_bGrandchildsOK && parentTableID != subtable.Id)
                        {
                            //check if the table is already added to collection before adding a new, this will prevent a table adding when circular realtionship is assigned
                            if(m_subTables.Find(p=>p.Id == parentTableID) == null)
                                GatherSubtables(subtable.Id);
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public void GatherChildTablesEx()
        {
            m_subTables = new List<COEDataView.DataViewTable>();
            int baseTableID = m_selectedDataView.Basetable;
            GatherSubtables(baseTableID);
        }
        //---------------------------------------------------------------------
        private static void DoAddChildSources(BindingSource parentSource, DataSet dataSet, DataTable parentTable, List<BindingSource> blist)
        {
            foreach (DataRelation rel in dataSet.Relations)
            {
                if (rel.ParentTable == parentTable)
                {
                    BindingSource subBindingSource = new BindingSource();
                    subBindingSource.DataSource = parentSource;
                    subBindingSource.DataMember = rel.ToString();
                    blist.Add(subBindingSource);
                    DoAddChildSources(subBindingSource, dataSet, rel.ChildTable, blist);
                }
            }
        }
        //---------------------------------------------------------------------
        public static List<BindingSource> GetSubBindingSources(BindingSource parentSource)
        {
            List<BindingSource> blist = new List<BindingSource>();
            if (parentSource != null && parentSource.DataSource != null)
            {
                DataSet dataSet = parentSource.DataSource as DataSet;
                //Coverity Bug Fix CID 13029 
                if (dataSet != null)
                    DoAddChildSources(parentSource, dataSet, dataSet.Tables[0], blist);
            }
            return blist;
        }
        //---------------------------------------------------------------------
        public bool SelectDataViewTableByID(int tid)
        {
            return SelectDataViewTableByNameOrID(null, tid);
        }
        //---------------------------------------------------------------------
        public bool SelectDataViewTable(String tblname)
        {
            return SelectDataViewTableByNameOrID(tblname, 0);
        }
        //---------------------------------------------------------------------
        public bool SelectDataViewTableByNameOrID(String tblname, int id)
        {
            bool ret = false;
            if (m_selectedDataView != null)
            {
                foreach (COEDataView.DataViewTable dvTable in m_selectedDataView.Tables)
                {
                    // new 7/16/09: given tblname may be an alias
                    // new 12/09: prefer match by id
                    bool bMatch = (id > 0 && id == dvTable.Id) ||
                                    (!String.IsNullOrEmpty(tblname) &&
                                    (CBVUtil.Eqstrs(tblname, dvTable.Name) || CBVUtil.Eqstrs(tblname, dvTable.Alias)));
                    if (bMatch)
                    {
                        m_selectedDVTable = dvTable;
                        m_selectedDataView.Basetable = dvTable.Id;
                        // CSBR-115707: Dataviews passed to server should be by ID, not full xml
                        m_selectedDataView.DataViewHandling = COEDataView.DataViewHandlingOptions.MERGE_CLIENT_AND_SERVER_DATAVIEW;

                        // gather any child tables into subtables list
                        GatherChildTablesEx();
                        ret = true;
                    }
                }
            }
            return ret;
        }
        //---------------------------------------------------------------------
        public String DVTableNameToDSTableName(String dvTableName)
        {
            COEDataView.DataViewTable dvTable = FindDVTableByName(dvTableName);
            if (dvTable == null)
                return String.Empty;
            String dsTableName = String.Concat("Table_", CBVUtil.IntToStr(dvTable.Id));
            return dsTableName;
        }
        //---------------------------------------------------------------------
        public COEDataView.DataViewTable DSTableNameToDVTable(String dsTableName)
        {
            // dsTableName is like "Table_22"; return actual table from dataview
            int tableId = CBVUtil.StrToInt(CBVUtil.AfterDelimiter(dsTableName, '_'));
            if (tableId > 0)
            {
                COEDataView.DataViewTable dvTable = FindDVTableByID(tableId);
                if (dvTable != null)
                    return dvTable;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public String DSTableNameToDVTableName(String dsTableName)
        {
            COEDataView.DataViewTable dvTable = DSTableNameToDVTable(dsTableName);
            return (dvTable == null) ? dsTableName : dvTable.Name;
        }
        //---------------------------------------------------------------------
        public COEDataView.DataViewTable FindDVTableByID(int tableID)
        {
            return FindDVTableByID(tableID, m_selectedDataView);
        }
        //---------------------------------------------------------------------
        public static COEDataView.DataViewTable FindDVTableByID(int tableID, COEDataView dataView)
        {
            foreach (COEDataView.DataViewTable dvTable in dataView.Tables)
                if (dvTable.Id == tableID)
                    return dvTable;
            return null;
        }
        //---------------------------------------------------------------------
        public COEDataView.DataViewTable FindDVTableByName(String tableName)
        {
            return FindDVTableByName(tableName, m_selectedDataView);
        }
        //---------------------------------------------------------------------
        public static COEDataView.DataViewTable FindDVTableByName(String tableName, COEDataView dataView)
        {
            if (dataView != null)
            {
                foreach (COEDataView.DataViewTable dvTable in dataView.Tables)
                {
                    // CSBR-143846: use case-independent match instead of equality
                    // bug happens when user enters lowercase "tablename:field" into button arg by hand instead of picking from list
                    if (CBVUtil.Eqstrs(dvTable.Name, tableName))
                        return dvTable;
                    if (CBVUtil.Eqstrs(dvTable.Alias, tableName))
                        return dvTable;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static COEDataView.DataViewTable FindDVTableWithField(int fieldID, COEDataView dataView)
        {
            if (dataView != null)
            {
                foreach (COEDataView.DataViewTable dvTable in dataView.Tables)
                {
                    foreach (COEDataView.Field f in dvTable.Fields)
                        if (f.Id == fieldID)
                            return dvTable;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field FindDVFieldByName(COEDataView.DataViewTable t, String fieldName)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                //if (CBVUtil.Eqstrs(fieldName, f.Name)) // Getting DV field names based on names will create issues in cases when Alias Fields are created.So always fetching DV Field Names based on alias.
                //    return f;
                if (CBVUtil.Eqstrs(fieldName, f.Alias))
                {
                    Debug.WriteLine("NAME DOES NOT MATCH BUT ALIAS DOES: " + fieldName);
                    return f;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field FindDVFieldByID(COEDataView.DataViewTable t, int fieldID)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                if (fieldID == f.Id)
                    return f;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static String GetFrameworkVersion()
        {
            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            VersionInfo version = ((COEIdentity)principal.Identity).COEConnection.VersionInfo;
            return version.ServerFrameworkVersion.ToString();
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field FindStructureField(COEDataView.DataViewTable t, COEDataView d)
        {
            foreach (COEDataView.Field f in t.Fields)
            {
                if (f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE ||
                    f.MimeType == COEDataView.MimeTypes.CHEMICAL_X_CDX)
                {
                    return f;
                }
            }

            // Try again by Checking for structure lookup
            foreach (COEDataView.Field f in t.Fields)
            {
                if (f.LookupFieldId > 0)  // is lookup
                {
                    // is lookup display structure?
                    COEDataView.Field lf = d.GetFieldById(f.LookupDisplayFieldId);
                    if (lf.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE ||
                        lf.MimeType == COEDataView.MimeTypes.CHEMICAL_X_CDX)  // is structure mime or indexed
                    {
                        // return the actual, not lookup field
                        return f;
                    }
                }
            }

            // try again by checking  for alias
            foreach (COEDataView.Field f in t.Fields)
            {
                if (CBVUtil.Eqstrs(f.Alias, "STRUCTURE"))
                {
                    return f;
                }
            }

            return null;
        }
        //---------------------------------------------------------------------
        public COEDataView.DataViewTable FindDVTable(DataTable t)
        {
            return FindDVTable(t, m_selectedDataView);
        }
        //---------------------------------------------------------------------
        public static COEDataView.DataViewTable FindDVTable(DataTable t, COEDataView dataView)
        {
            int undPos = t.TableName.IndexOf('_');
            if (undPos <= 0) return null;
            int tableID = Int32.Parse(t.TableName.Substring(undPos + 1));
            COEDataView.DataViewTable dvTable = FindDVTableByID(tableID, dataView);
            return dvTable;
        }
        //---------------------------------------------------------------------
        public COEDataView.Field FindDVField(DataTable t, DataColumn c)
        {
            return FindDVField(t, c, m_selectedDataView);
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field FindDVField(DataTable t, DataColumn c, COEDataView dataView)
        {
            COEDataView.DataViewTable dvTable = FindDVTable(t, dataView);
            //coverity Bug Fix : CID 13110 
            if (dvTable != null)
            {
                COEDataView.Field dvField = FindDVFieldByName(dvTable, c.ColumnName);
                return dvField;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public bool IsLinkingField(COEDataView.Field field)
        {
            foreach (COEDataView.Relationship rel in m_selectedDataView.Relationships)
            {
                if ((rel.Child == field.ParentTableId) && (rel.ChildKey == field.Id))
                    return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        private COEDataView.Field PKField()
        {
            // return primary key field of selected table
            COEDataView.Field dvField = null;
            String sColID = SelectedDataViewTable.PrimaryKey;
            if (!String.IsNullOrEmpty(sColID))
            {
                int pkFieldID = CBVUtil.StrToInt(sColID);
                dvField = FindDVFieldByID(SelectedDataViewTable, pkFieldID);
            }
            return dvField;
        }
        //---------------------------------------------------------------------
        public String PKFieldAlias()
        {
            // return alias of primary key field of selected table
            String pkColAlias = String.Empty;
            COEDataView.Field dvField = PKField();
            if (dvField != null)
                pkColAlias = dvField.Alias;
            return pkColAlias;
        }
        //---------------------------------------------------------------------
        public String PKFieldName()
        {
            // return name of primary key field of selected table
            String pkColName = String.Empty;
            COEDataView.Field dvField = PKField();
            if (dvField != null)
                pkColName = dvField.Name;
            return pkColName;
        }
        //---------------------------------------------------------------------
        public bool HasPK(String dataViewName, String tableName)
        {
            COEDataView dataView = FindDVByName(dataViewName);
            COEDataView.DataViewTable dvTable = (dataView == null) ? null : FindDVTableByName(tableName, dataView);

            // pk field must be present and also non-zero
            String sPK = (dvTable == null) ? String.Empty : dvTable.PrimaryKey;
            if (String.IsNullOrEmpty(sPK)) return false;
            if (CBVUtil.StrToInt(sPK) <= 0) return false;
            return true;
        }

        //---------------------------------------------------------------------
        private static COEDataView.DataViewTable GetSubtableAndField(FormDbMgr formDbMgr, String fullDescr, ref String fieldName)
        {
            COEDataView.DataViewTable t = null;
			// CBOE-303, CBOE-1763, CBOE-1764 removed the ":" and placed "."
            if (fullDescr != null && fullDescr.Contains("."))
            {
                fieldName = CBVUtil.AfterDelimiter(fullDescr, '.');
                String tableName = CBVUtil.BeforeDelimiter(fullDescr, '.');
                t = formDbMgr.FindDVTableByName(tableName);
            }
            return t;
        }
        //---------------------------------------------------------------------
        public void UnapplySortStringToRC()
        {
            ResultsCriteria rc = this.ResultsCriteria;
            foreach (ResultsCriteria.ResultsCriteriaTable rctable in rc.Tables)
                foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rctable.Criterias)
                    rcbase.OrderById = 0;
        }
        //---------------------------------------------------------------------
        public void ApplySortStringToRC(String sortStr)
        {
            // this needs to go in formdblib, where query.cs can call it
            if (String.IsNullOrEmpty(sortStr))
                return;

            ResultsCriteria rc = this.ResultsCriteria;
            SortData sData = SortData.StringToSortData(sortStr);
            int nSortIndex = 1;
            foreach (SortField sortField in sData)
            {
                FormDbMgr.ApplySortToRCEx(this, sortField.m_fieldName, sortField.m_bAscending, nSortIndex > 1, nSortIndex);
                nSortIndex++;
            }
        }
        //---------------------------------------------------------------------
        public static void ApplySortToRCEx(FormDbMgr formDbMgr, String fieldName,
                              bool bAscending, bool bAppend, int nextIndex)
        {
            ResultsCriteria rc = formDbMgr.ResultsCriteria;
            COEDataView.DataViewTable t = formDbMgr.SelectedDataViewTable;
            //Coverity Bug Fix : CID 13111 
            if (t != null && fieldName != null && fieldName.Contains(":"))
                t = GetSubtableAndField(formDbMgr, fieldName, ref fieldName);
            // add first sort value choice
            COEDataView.Field fld = (t != null) ? FormDbMgr.FindDVFieldByName(t, fieldName) : null;  // fails to find aggreg field
            int fid = (fld == null) ? 0 : fld.Id;
            bool bUseAlias = fld == null;
            ApplySortToRC(rc, fieldName, fid, bAscending, bAppend, nextIndex, bUseAlias);
        }
        //---------------------------------------------------------------------
        public static void ApplySortToRC(ResultsCriteria rc, String fieldName, int fldID,
                               bool bAscending, bool bAppend, int nextIndex, bool bUseAlias)
        {
            // modify RC to mark given field as sortable
            // if bAppend, add this mark; if not, add this and clear all others
            // WARNING: table is not specified for given fieldname => all criteria in all tables must have unique aliases or id's
            ResultsCriteria.SortDirection dir = bAscending ? ResultsCriteria.SortDirection.ASC : ResultsCriteria.SortDirection.DESC;

            // first clear all OrderBy flags (CSBR-111878)
            if (!bAppend)
            {
                foreach (ResultsCriteria.ResultsCriteriaTable rctable in rc.Tables)
                    foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rctable.Criterias)
                        rcbase.OrderById = 0;
            }

            // then assign new ones, giving sequential indices
            foreach (ResultsCriteria.ResultsCriteriaTable rctable in rc.Tables)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rctable.Criterias)
                {
                    bool bMatch = false;
                    if ((rcbase is ResultsCriteria.Formula || rcbase is ResultsCriteria.MolWeight) &&
                                                CBVUtil.Eqstrs(rcbase.Alias, fieldName))
                    {
                        bMatch = true;
                    }
                    else if (rcbase is ResultsCriteria.Field && (rcbase as ResultsCriteria.Field).Id == fldID)
                    {
                        bMatch = true;
                        if (bUseAlias && !String.IsNullOrEmpty(rcbase.Alias))
                            bMatch = CBVUtil.Eqstrs(rcbase.Alias, fieldName);
                    }
                    else if (bUseAlias && !String.IsNullOrEmpty(rcbase.Alias))    // for aggregate functions
                    {
                        bMatch = CBVUtil.Eqstrs(rcbase.Alias, fieldName);
                    }
                    if (bMatch)
                    {
                        rcbase.OrderById = nextIndex;
                        rcbase.Direction = dir;
                        return; // break;  ... jump out of both loops after sort field is found
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Form Trees methods
        public void CreateObjectBanks()
        {
            m_publicFormBank = new DbObjectBank(m_login.UserName, true);
            m_privateFormBank = new DbObjectBank(m_login.UserName, false);
        }
        #endregion

        #region Results Criteria methods
        public void AddTableToRC(COEDataView.DataViewTable t, ResultsCriteria rc)
        {
            // add main or subtable to results criteria, with all its fields
            // for any struct field, add fmla and molwt
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            rcTable.Id = t.Id;
            foreach (COEDataView.Field f in t.Fields)
            {
                // CSBR-118059: do not include fields marked as nonvisible
                if (f.Visible == false)
                    continue;
                ResultsCriteria.Field resultf = new ResultsCriteria.Field();
                resultf.Id = f.Id;
                rcTable.Criterias.Add(resultf);

                // CSBR-155343: Recognize structure lookups
                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE ||
                    FormDbMgr.IsStructureLookupField(f, SelectedDataView);
                if (bIsStructure)
                    FormDbMgr.AddFmlaMolwtFields(f.Id, rcTable);
            }
            rc.Tables.Add(rcTable);
        }
        //---------------------------------------------------------------------
        public static void AddFmlaMolwtFields(int structID, ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            ResultsCriteria.Formula resultf = new ResultsCriteria.Formula();
            resultf.Alias = "Formula";
            resultf.Id = structID;
            rcTable.Criterias.Add(resultf);

            ResultsCriteria.MolWeight resultm = new ResultsCriteria.MolWeight();
            resultm.Alias = "Molweight";
            resultm.Id = structID;
            rcTable.Criterias.Add(resultm);
        }
        //---------------------------------------------------------------------
        public ResultsCriteria CreateResultsCriteria(bool bWithSubforms)
        {
            // build results criteria from selected table and subtables
            // this is not for building a resultscriteria from a form (for that see FormViewToRC in formutils.cs)
            ResultsCriteria rc = new ResultsCriteria();
            if (m_selectedDVTable != null)
            {
                AddTableToRC(m_selectedDVTable, rc);
                if (bWithSubforms)
                {
                    foreach (COEDataView.DataViewTable subtable in m_subTables)
                        AddTableToRC(subtable, rc);
                }
            }
            return rc;
        }
        //---------------------------------------------------------------------
        // this duplicates code in FormUtils.cs, but we can't reference that here
        private static ResultsCriteria.ResultsCriteriaTable RCFindTable(ResultsCriteria rc, int tableID)
        {
            foreach (ResultsCriteria.ResultsCriteriaTable table in rc.Tables)
                if (table.Id == tableID) return table;
            return null;
        }
        //---------------------------------------------------------------------
        public List<String> GetTableList()
        {
            List<String> tlist = new List<string>();
            foreach (COEDataView.DataViewTable dvTable in SelectedDataView.Tables)
                tlist.Add(dvTable.Alias);
            return tlist;
        }
        //---------------------------------------------------------------------
        public List<String> GetFieldList(bool bWithStructure, bool bWithBlankAtTop, bool bNumericOnly,
                                        int tableID, bool bUseFullRC)
        {
            // return list of fields from base table of RC; used to fill combo boxes
            // if tableID non-zero, then it identifies a child table
            List<String> flist = new List<string>();
            if (bWithBlankAtTop)
                flist.Add("");

            ResultsCriteria resultsCriteria = bUseFullRC ? FullResultsCriteria : ResultsCriteria;
            if (resultsCriteria != null && resultsCriteria.Tables.Count > 0)
            {
                ResultsCriteria.ResultsCriteriaTable rcTable = (tableID == 0) ?
                   resultsCriteria.Tables[0] : RCFindTable(resultsCriteria, tableID);
                int rcTableID = (tableID == 0 && rcTable != null) ? rcTable.Id : tableID;   // mod for CSBR-128494

                COEDataView.DataViewTable dvTable = FindDVTableByID(rcTableID, this.SelectedDataView);
                if (dvTable != null && rcTable != null)
                {
                    foreach (ResultsCriteria.IResultsCriteriaBase rcBase in rcTable.Criterias)
                    {
                        if (bNumericOnly && rcBase is ResultsCriteria.Formula)
                            continue;
                        if (rcBase is ResultsCriteria.AggregateFunction && !String.IsNullOrEmpty(rcBase.Alias))
                        {
                            flist.Add(rcBase.Alias);
                        }
                        else if (rcBase is ResultsCriteria.MolWeight || rcBase is ResultsCriteria.Formula)
                        {
                            flist.Add(rcBase.Alias);
                        }
                        else if (rcBase is ResultsCriteria.Field)
                        {
                            COEDataView.Field dvField = FindDVFieldByID(dvTable, (rcBase as ResultsCriteria.Field).Id);
                            if (dvField != null)
                            {
                                bool bIsStructure = dvField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                                if (!bWithStructure && bIsStructure)
                                    continue;
                                bool bIsNumeric = IsNumericField(dvField);
                                if (bNumericOnly && !bIsNumeric)
                                    continue;
                                String fldName = dvField.Name;
                                if (!String.IsNullOrEmpty(dvField.Alias))   // CSBR-117750
                                    fldName = dvField.Alias;
                                flist.Add(fldName);
                            }
                        }
                    }
                }
            }
            return flist;
        }
        #endregion

        #region Datasets
        public void PrepDataSet(DataSet dataSet)
        {
            PrepDataSet(dataSet, m_selectedDataView);
        }
        //---------------------------------------------------------------------
        private static void FillMarksColumn(DataTable t, DataColumn col)
        {
            // alternate checked/unchecked for testing
            Boolean boolval = true;
            foreach (DataRow row in t.Rows)
            {
                row[col] = boolval;
                boolval = !boolval;
            }
        }
        //---------------------------------------------------------------------
        public static bool IsStructureLookupField(COEDataView.Field f, COEDataView dataView)
        {
            if (f.LookupDisplayFieldId > 0)
            {
                COEDataView.DataViewTable tLookup = FindDVTableWithField(f.LookupDisplayFieldId, dataView);
                if (tLookup != null)
                {
                    COEDataView.Field fLookup = FindDVFieldByID(tLookup, f.LookupDisplayFieldId);
                    if (fLookup != null)
                        return fLookup.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        public static void PrepDataSet(DataSet dataSet, COEDataView dataView)
        {
            // set extendedprops on table cols to pass field info to form and grid makers
            // e.g.: col must have non-blank mimetype if grid is to recognize it as a structure col
            // use the dataview to get the column info
            if (dataSet == null) return;
            foreach (DataTable t in dataSet.Tables)
            {
                COEDataView.DataViewTable dvTable = FindDVTable(t, dataView);
                //Coverity Bug Fix : CID 13112 
                t.ExtendedProperties["alias"] = (dvTable != null) ? dvTable.Alias : string.Empty;

                // TEMPORARY: add a column for checkmarks
                bool bAddCheckCol = false;
                if (bAddCheckCol)
                {
                    t.Columns.Add("Marked", typeof(Boolean));
                    FillMarksColumn(t, t.Columns["Marked"]);
                }
                foreach (DataColumn c in t.Columns)
                {
                    COEDataView.Field dvf = FindDVField(t, c, dataView);
                    if (dvf != null)
                    {
                        c.ExtendedProperties["Alias"] = dvf.Alias;
                        c.ExtendedProperties["Default"] = dvf.IsDefault;
                    }

                    bool bIsStructure = dvf != null && dvf.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;

                    // CSBR-114391: handle structure lookup field
                    if (!bIsStructure && (dvf != null) && (dvf.LookupDisplayFieldId > 0))
                    {
                        if (IsStructureLookupField(dvf, dataView))
                            bIsStructure = true;
                    }
                    bool bIsFmla = CBVUtil.Eqstrs(c.ColumnName, "Formula"), bIsMolwt = CBVUtil.Eqstrs(c.ColumnName, "Molweight");
                    if (bIsStructure)
                        c.ExtendedProperties["mimetype"] = "cdxb64";
                    if (bIsFmla || bIsMolwt)
                    {
                        c.ExtendedProperties["Alias"] = c.ColumnName;
                        c.ExtendedProperties["Default"] = true;
                    }

                }
            }
        }
        //---------------------------------------------------------------------
        public static void DebugDatasetNoXml(DataSet dataSet)
        {
            // for dumping a faulty dataset with multiple parents per child (where read/write xml fail)
            foreach (DataTable table in dataSet.Tables)
            {
                Debug.WriteLine(String.Concat("Table: ", table.TableName));
                foreach (DataRow dataRow in table.Rows)
                {
                    String sRow = String.Empty;
                    foreach (DataColumn dataCol in table.Columns)
                    {
                        Object oData = dataRow[dataCol.ColumnName];
                        Debug.WriteLine(String.Concat(dataCol.ColumnName, "=", oData.ToString()));
                    }
                }
            }
            foreach (DataRelation relation in dataSet.Relations)
            {
                Debug.WriteLine(String.Concat("Relation: ", relation.RelationName, " par=", relation.ParentTable,
                    " chi=", relation.ChildTable));
            }
        }
        //---------------------------------------------------------------------
        public static void DebugDataset(DataSet dataSet)
        {
            String sXml = dataSet.GetXml();
            Debug.WriteLine(String.Concat("Dataset: ", sXml));

            foreach (DataRelation relation in dataSet.Relations)
            {
                Debug.WriteLine(String.Concat("Relation: ", relation.RelationName, " par=", relation.ParentTable,
                    " chi=", relation.ChildTable));
            }
        }
        #endregion

        #region Caching
        private String GetDVCacheFileName()
        {
            String dbname = Login.ParseServerMachineName(this.Login.ServerDisplay);
            String fnDviews = String.Format("{0}\\dv_cache_{1}.xml", Application.CommonAppDataPath, dbname);
            return fnDviews;
        }
        //---------------------------------------------------------------------
        public bool HaveCachedDataViews()
        {
            return File.Exists(GetDVCacheFileName());
        }
        //---------------------------------------------------------------------
        public void RetrieveCachedDataViews()
        {
            // create new dataviewBO list from xml file
            m_dataViews = COEDataViewBOList.NewList();
            String selectDataView = String.Empty;
            XmlTextReader reader = new XmlTextReader(GetDVCacheFileName());
            reader.MoveToContent();
            reader.Read();
            while (reader.Read())
            {
                String sName = reader.GetAttribute("name");
                String sDescr = reader.GetAttribute("description");
                String sId = reader.GetAttribute("id");
                String sXml = reader.ReadInnerXml();
                if (sName == null || sXml == null || sXml.Length == 0)
                    break;

                COEDataView dview = new COEDataView();
                dview.GetFromXML(sXml);

                COEDataViewBO dviewBO = COEDataViewBO.New(sName, sDescr, dview, null);

                // new 3/11: use id now saved based on dvbo
                int dviewIDInXml = dview.DataViewID, dviewIDSaved = CBVUtil.StrToInt(sId);
                if (dviewIDInXml != dviewIDSaved)
                    Debug.WriteLine(String.Format("MISMATCHED DVIEW IDs: dvxml={0} dvtbl={1}", dviewIDInXml, dviewIDSaved));
                dviewBO.ID = dviewIDSaved;

                m_dataViews.Add(dviewBO);
                if (m_dataViews.Count == 1)
                    selectDataView = dviewBO.Name;
            }
            // must specify selected dview before calling PrepDataSet
            SelectDataView(selectDataView);
        }
        //---------------------------------------------------------------------
        public void SaveDataViews()
        {
            // write dataset to one file
            //m_dataset.WriteXml(fnDataset, XmlWriteMode.WriteSchema);

            // write all dataviews to file
            XmlDocument xdoc = new XmlDocument();
            XmlElement root = xdoc.CreateElement("dataviews");
            xdoc.AppendChild(root);

            foreach (COEDataViewBO dataView in m_dataViews)
            {
                XmlElement dvElement = xdoc.CreateElement("dataviewbo");
                dvElement.SetAttribute("id", dataView.ID.ToString());
                dvElement.SetAttribute("name", dataView.Name);
                dvElement.SetAttribute("description", dataView.Description);
                dvElement.InnerXml = CBVUtil.RemoveDocHeader8(dataView.COEDataView.ToString());
                root.AppendChild(dvElement);
            }
            XmlTextWriter xmlTextWriter = new XmlTextWriter(GetDVCacheFileName(), null);
            xmlTextWriter.Formatting = Formatting.Indented;
            xdoc.Save(xmlTextWriter);
        }
        #endregion

        #endregion

    }
}

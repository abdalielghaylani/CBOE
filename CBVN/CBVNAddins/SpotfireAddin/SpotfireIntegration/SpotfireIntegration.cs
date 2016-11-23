using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CBVUtilities;
using ChemBioViz.NET;
using ChemControls;
using FormDBLib;
using Infragistics.Win.UltraWinGrid;
using SpotfireIntegration.Common;

namespace SpotfireIntegration
{
    /// <summary>
    ///  Manages the integration between a form and a single Spotfire
    ///  instance.  The lifetime of this object is the same as the Spotfire
    ///  process.
    /// </summary>
    internal class SpotfireIntegration : IDisposable
    {
        private IPrincipal principal;
        private ChemBioVizForm form;
        private DuplexChannelFactory<ISpotfireCOETableLoader> channelFactory;
        private ISpotfireCOETableLoader tableLoaderProxy;
        private System.Timers.Timer keepAliveTimer;

        private COEHitList hitList;
        private string baseTableName;
        private string formName;
        private string formAddinArgs;
        private List<int> selectedRecords;

        private bool updatingSelection = false;
        private bool editResultCriteriaSF = false;
        private bool filterChildHits = false;
        private string serverName = string.Empty;
        private string userName = string.Empty;

        internal ChemBioVizForm Form
        {
            get { return this.form; }
        }

        public enum SpotfireIntegrationState
        {
            Ready,          // connected
            Starting,       // starting up spotfire
            Connecting,     // spotfire already running; connecting
            Disconnected,   // failed to connect
            Unsynched       // connected but not interacting
        }

        private SpotfireIntegrationState state;
        private object stateLock = new object();

        public SpotfireIntegrationState State
        {
            get
            {
                lock (this.stateLock)
                {
                    return this.state;
                }
            }

            internal set
            {
                lock (this.stateLock)
                {
                    this.state = value;

                    // tell form the state changed, so it can update menu
                    if (this.Form != null)
                    {
                        this.Form.BeginInvoke(
                            new MethodInvoker(delegate
                            {
                                IPrincipal p = Thread.CurrentPrincipal;
                                Thread.CurrentPrincipal = this.principal;
                                try
                                {
                                    //Coverity Bug Fix CID 13085 
                                    lock (this.stateLock)
                                    {
                                        this.state = value; // seems to need to do this again in some cases
                                        string menuTitle = (new SpotfireAddinMenu()).Title; // use menu title to identify ourself
                                        this.Form.UpdateAddinMenu(menuTitle);//, (int)value);
                                    }
                                }
                                catch (Exception e)
                                {
                                    CBVUtil.ReportError(e, "Spotfire addin: Error updating state");
                                }
                                Thread.CurrentPrincipal = p;
                            }));
                    }
                }
            }
        }

        internal void EndIntegration()
        {
            State = SpotfireIntegrationState.Disconnected;
            UnregisterEvents();
            // this.form = null; .. need this to stick around a little longer, to update menus
            this.hitList = null;
            this.tableLoaderProxy = null;
            if (this.keepAliveTimer != null)
            {
                this.keepAliveTimer.Enabled = false;
                this.keepAliveTimer = null;
            }
            if (this.channelFactory != null)
            {
                try
                {
                    // fails if in faulted state during close of SF document
                    this.channelFactory.Close();

                }
                catch (Exception e)
                {
                    //CBVUtil.ReportError(e, "Spotfire addin: Error closing channel factory");
                    Debug.WriteLine(e.Message);
                }
                this.channelFactory = null;
            }
        }

        internal bool StartIntegration(ChemBioVizForm form)
        {
            State = SpotfireIntegrationState.Disconnected;
            if (form == null)
                return false;

            // Unregister events in previous form
            UnregisterEvents();

            // Register form events
            this.form = form;
            Query query = form.CurrQuery;
            FormDbMgr formDbMgr = form.FormDbMgr;

            // make sure we have a form with some data; no need to warn user -- menu should prevent getting here
            if (formDbMgr == null || formDbMgr.SelectedDataView == null || query == null)
            {
                //CSBR-151976. Here for Chembioviz form, Name and AddinArguments properties are checked for not null and assinged to public form variable and return true to call LinkToSpotFire method. 
                if (form.Name != null && form.AddinArguments != null)
                {
                    this.form = form;
                    this.formName = form.Name;
                    this.formAddinArgs = form.AddinArguments;
                    State = SpotfireIntegrationState.Starting;
                    RegisterEvents();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            RegisterEvents();

            // Record the form data that will be pushed into Spotfire.
            this.principal = Thread.CurrentPrincipal;
            this.hitList = new COEHitList(
                query.HitListID,
                (query.IsSaved ? HitListType.SAVED : HitListType.TEMP),
                query.NumHits,
                formDbMgr.SelectedDataView.DataViewID,
                formDbMgr.ResultsCriteria);

            int baseTableID = formDbMgr.SelectedDataView.Basetable;
            this.baseTableName = formDbMgr.SelectedDataView.Tables.getById(baseTableID).Alias;
            //CSBR:151920
            this.formName = form.FormName;//formName is send to set the DocumentTitle
            this.formAddinArgs = Form.AddinArguments;
            this.selectedRecords = Form.GetSelectedRecordNos();
            this.filterChildHits = formDbMgr.FilterChildHits;
            this.serverName = formDbMgr.Login.Server;
            this.userName = formDbMgr.Login.UserName;

            State = SpotfireIntegrationState.Starting;
            return true;
        }

        internal bool ReportSizeLimit()
        {
            if (this.Form.Pager != null)
            {
                SizeExceededDialog dlg = new SizeExceededDialog(this.Form.Pager.ListSize, Properties.settings.Default.MaxRows);
                dlg.Show();
            }
            return false;
        }

        internal bool ExceedsListSizeLimit()
        {
            //CSBR-151976.When no form is selected at CBVN and try to use 'Opn Analysis'.An exception is raised for that false is retuen.
            try
            {
                int curListSize = this.Form.Pager.ListSize;
                int maxListSize = Properties.settings.Default.MaxRows;
                return maxListSize > 0 && maxListSize >= curListSize; // CSBR-156732 Fixed
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal bool DoLoad(out bool bCancelled)
        {
            string authenticationTicket = CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Token;
            string cslaDataPortalUrl = ConfigurationManager.AppSettings["CslaDataPortalUrl"];
            string cslaDataPortalProxy = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
            string dataSource = null;
            if (String.IsNullOrEmpty(cslaDataPortalUrl) || String.IsNullOrEmpty(cslaDataPortalProxy))
            {
                DBMSTypeData dbmsd = ConfigurationUtilities.GetDBMSTypeData(DBMSType.ORACLE);
                dataSource = dbmsd.DataSource;
            }

            // force rebuild if loading from file
            String sArgs = formAddinArgs;
            bool bLoadingFromFile = !String.IsNullOrEmpty(sArgs);
            bool bForceReload = bLoadingFromFile;   // or if specified by caller somehow?

            bCancelled = false;
            bool bWarn = bForceReload ?
                Properties.settings.Default.WarnOnRebuild : Properties.settings.Default.WarnOnRefresh;
            if (!SpotfireIntegrationAddin.IsSpotfireRunning())
                bWarn = false;

            // if hitlist is larger than user-set limit, can't proceed
            if (!ExceedsListSizeLimit()) // CSBR-156732 Fixed
            {
                ReportSizeLimit();
                bCancelled = true;
            }
            else if (bWarn && this.State != SpotfireIntegrationState.Starting
                        && this.State != SpotfireIntegrationState.Disconnected)
            {
                // Check if a COE hitlist is currently open in Spotfire.  If it is, check with
                // the user whether to replace it. (unless forcing reload, or open hitlist not compatible with current)
                COEHitList openHitList = null;
                try
                {
                    openHitList = this.tableLoaderProxy.GetCOEHitList();
                }
                catch (CommunicationException) { }
                bool bHasValidHitlist = false;
                //CSBR-151976. Checking for hitlist property not ?equal to null. here when user selects 'Open Analysis' with selecting a form at CBVN the hitlist will be null. 
                if (this.hitList != null)
                {
                    // also check that base tables are the same?
                    bHasValidHitlist = (openHitList != null) && (openHitList.DataViewID == this.hitList.DataViewID);
                }
                else
                {
                    //No need to check for base tables are same
                    bHasValidHitlist = (openHitList != null);
                }

                if (bForceReload || !bHasValidHitlist)
                {
                    if (openHitList != null)    // no need to confirm if no analysis open
                    {
                        if (!ConfirmAnalysisReplace())
                            bCancelled = true;
                    }
                }
                else if (bHasValidHitlist)
                {
                    if (!ConfirmListReplace())
                        bCancelled = true;
                }
            }
            if (bCancelled)
                return false;

            // if form provides filename arg, it is the dxp file to be opened on load
            //CSBR:151920 formName parameter is added to tableLoaderProxy to set Document Title.
            int maxRows = Properties.settings.Default.MaxRows;
            if (bLoadingFromFile)
            {
                string resultMessage = tableLoaderProxy.LoadTablesFromFile(hitList, baseTableName, formName, sArgs,
                    authenticationTicket, cslaDataPortalUrl, cslaDataPortalProxy, dataSource, maxRows, this.serverName, this.userName);
                if (resultMessage != null)
                {
                    MessageBox.Show(resultMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                tableLoaderProxy.LoadTablesFromCOE(hitList, baseTableName, formName, authenticationTicket,
                    cslaDataPortalUrl, cslaDataPortalProxy, dataSource, bForceReload, maxRows, filterChildHits, this.serverName, this.userName);
            }
            tableLoaderProxy.Subscribe();
            return true;
        }

        internal bool LinkToSpotfire()
        {
            this.State = SpotfireIntegrationState.Disconnected;
            if (this.channelFactory == null)
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.ReaderQuotas.MaxStringContentLength = 65536;
                this.channelFactory = new DuplexChannelFactory<ISpotfireCOETableLoader>(
                    new SpotfireCallbacks(this),
                    binding,
                    new EndpointAddress(Uris.SpotfireServiceUri));
            }

            // Check if Spotfire is already available.
            if (SpotfireIntegrationAddin.IsSpotfireRunning())
            {
                try
                {
                    this.tableLoaderProxy = this.channelFactory.CreateChannel();
                    this.tableLoaderProxy.Ping();
                    this.State = SpotfireIntegrationState.Connecting;
                }
                catch (SystemException e)
                {
                    this.tableLoaderProxy = null;
                    this.State = SpotfireIntegrationState.Starting;
                }
            }
            else
            {
                this.State = SpotfireIntegrationState.Starting;
            }

            if (this.State == SpotfireIntegrationState.Starting)
            {
                // Assume that no Spotfire instance is running with an active WCF service.
                // Start one, and then continue trying to communicate in a loop until we succeed or time-out.
                FileAssociation fileAssocation = new FileAssociation();
                Process sfProcess;
                try
                {
                    string sSFExePath = fileAssocation.Get(".dxp");
                    sfProcess = Process.Start(sSFExePath);
                }
                catch (Win32Exception e)
                {
                    CBVUtil.ReportError(e, "Spotfire addin: Error launching Spotfire");
                    MessageBox.Show("ChemBioViz is unable to launch Spotfire.  Either it is not\n" +
                        "installed, or you do not have permissions to run it.");
                    this.State = SpotfireIntegrationState.Disconnected;
                    return false;
                }
                // could set breakpoint here to attach debugger to SF process; or just attach while waiting for SF to start up

                // 240 attempts * 500 ms == 2 minutes
                for (int i = 0; i < 240; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        if (sfProcess.HasExited == true) //CSBR-151572 Fixed set the i to attempts-1 when the process is terminated
                        {
                            i = 239;
                        }
                        this.tableLoaderProxy = this.channelFactory.CreateChannel();
                        this.tableLoaderProxy.Ping();
                        this.State = SpotfireIntegrationState.Connecting;
                        break;
                    }
                    catch (CommunicationException e)
                    {
                    }
                    catch (Exception e)
                    {
                        CBVUtil.ReportError(e, "Spotfire addin: Error connecting to Spotfire");
                    }
                }
                if (this.State == SpotfireIntegrationState.Starting)
                {
                    this.State = SpotfireIntegrationState.Disconnected;
                    this.tableLoaderProxy = null;
                    if (sfProcess.HasExited == true) //CSBR-151572 Fixed check to display appropriate message
                        MessageBox.Show("Spotfire is terminated by user.");
                    else
                        MessageBox.Show("ChemBioViz has timed out while waiting for Spotfire to launch.");
                    return false;
                }
            }

            // Now we're ready to connect.
            Debug.Assert(this.State == SpotfireIntegrationState.Connecting);
            bool success = false, bCancelled = false;
            try
            {
                success = DoLoad(out bCancelled);
            }
            catch (FaultException<TableLoadFault> e)
            {
                CBVUtil.ReportError(e, "Spotfire addin: Error loading tables into Spotfire");
                //MessageBox.Show(string.Format("Spotfire is unable to load the form because: {0}", e.Detail.Message));
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex, "Spotfire addin: Error loading tables into Spotfire");
                MessageBox.Show(string.Format("Error loading tables into Spotfire: {0}", ex.ToString()));
            }

            if (success)
            {
                // select initial record; mark as connected
                State = SpotfireIntegrationState.Ready;
                if (this.selectedRecords != null)
                {
                    TellSpotfireRecordChanged(this.selectedRecords);
                    this.selectedRecords = null;
                }
                this.keepAliveTimer = new System.Timers.Timer(300000);
                this.keepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(keepAliveTimer_Elapsed);
                this.keepAliveTimer.Enabled = true;
            }
            else
            {
                State = SpotfireIntegrationState.Disconnected;
                this.tableLoaderProxy = null;
            }
            return success;
        }

        private void keepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SpotfireController.GetInstance().PingSpotfire();

        }

        internal bool PingSpotfire()
        {
            if (this.tableLoaderProxy != null)
            {
                try
                {
                    this.tableLoaderProxy.Ping();
                    return true;
                }
                catch (CommunicationException)
                {
                }
            }

            return false;
        }

        private void UnregisterEvents()
        {
            if (this.Form != null)
            {
                this.Form.RecordChanged -= Form_RecordChanged;
                this.Form.RecordsetChanged -= Form_RecordsetChanged;
                this.Form.CBVFormClosed -= Form_FormClosed;

                foreach (FormTab formTab in this.Form.TabManager.Tabs)
                {
                    if (formTab.IsGridView())
                    {
                        ChemDataGrid cdg = formTab.Control as ChemDataGrid;
                        //Coverity Bug Fix CID :12914 
                        if (cdg != null)
                        {
                            cdg.AfterSelectChange -= ChemDataGrid_AfterSelectChange;
                        }
                    }
                }
            }
        }
        private void RegisterEvents()
        {
            if (this.Form != null)
            {
                this.Form.CBVFormClosed += new ChemBioVizForm.CBVFormClosedEventHandler(Form_FormClosed);
                this.Form.RecordChanged += Form_RecordChanged;
                this.Form.RecordsetChanged += new ChemBioVizForm.RecordsetChangedEventHandler(Form_RecordsetChanged);

                foreach (FormTab formTab in this.Form.TabManager.Tabs)
                {
                    if (formTab.IsGridView())
                    {
                        ChemDataGrid cdg = formTab.Control as ChemDataGrid;
                        //Coverity Bug Fix CID : 12913 
                        if (cdg != null)
                        {
                            cdg.AfterSelectChange += ChemDataGrid_AfterSelectChange;
                        }
                    }
                }

            }
        }

        internal void SelectRows(int dataViewID, int hitListID, List<int> rowIndexes)
        {
            // This method is called by the Spotfire view whenever the user changes the row selection
            // in Spotfire.  We update the selected row in CBVN to match.

            // Call out to the GUI thread that owns the form.
            this.Form.BeginInvoke(
                new MethodInvoker(delegate
                {
                    if (rowIndexes.Count == 0)
                    {
                        return;
                    }
                    if (this.State != SpotfireIntegrationState.Ready)   // JD
                    {
                        return;
                    }

                    // Make sure that CBVN and Spotfire are looking at the same data.
                    if (this.Form.FormDbMgr.SelectedDataView == null ||
                        this.Form.FormDbMgr.SelectedDataView.DataViewID != dataViewID ||
                        this.Form.CurrQuery.HitListID != hitListID)
                    {
                        return;
                    }

                    IPrincipal p = Thread.CurrentPrincipal;
                    Thread.CurrentPrincipal = this.principal;

                    try
                    {
                        this.updatingSelection = true;

                        // Move the form to the selected row.
                        // In grid mode, this will also update the active row.
                        this.Form.DoMove(Pager.MoveType.kmGoto, rowIndexes[0]);

                        FormTab formTab = this.Form.TabManager.CurrentTab;
                        ChemDataGrid cdg = formTab.Control as ChemDataGrid;
                        //Coverity Bug Fix CID 12912 
                        if (cdg != null && formTab.IsGridView())
                        {
                            // Update the grid selection

                            foreach (UltraGridRow row in cdg.Selected.Rows)
                            {
                                row.Selected = false;
                            }

                            foreach (int rowIndex in rowIndexes)
                            {
                                UltraGridRow row;
                                try
                                {
                                    row = cdg.Rows[rowIndex];
                                }
                                catch (System.IndexOutOfRangeException)
                                {
                                    continue;
                                }
                                row.Selected = true;
                            }
                        }
                    }
                    finally
                    {
                        this.updatingSelection = false;
                    }

                    Thread.CurrentPrincipal = p;
                }));
        }

        internal void ResultsCriteriaChanged(ResultsCriteria newResultsCriteria)
        {
            if (StateNotReady())
            {
                return;
            }
            // CSBR-153130: check whether the sort order of the new results criteria
            // is different from the sort order in CBVN, and end the integration if it is.
            ResultsCriteria ourResultsCriteria = this.hitList.ResultsCriteria;
            foreach (ResultsCriteria.ResultsCriteriaTable theirTable in newResultsCriteria.Tables)
            {
                foreach (ResultsCriteria.ResultsCriteriaTable ourTable in ourResultsCriteria.Tables)
                {
                    if (theirTable.Id == ourTable.Id)
                    {
                        IList<ResultsCriteria.IResultsCriteriaBase> theirSortOrder = GetSortOrder(theirTable);
                        IList<ResultsCriteria.IResultsCriteriaBase> ourSortOrder = GetSortOrder(ourTable);
                        if (!SortOrderEquals(theirSortOrder, ourSortOrder))
                        {
                            MessageBox.Show("Synchronization with Spotfire has been disrupted because the sort order " +
                                            "of the data in Spotfire has been changed.",
                                            "Spotfire Unsynched", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.editResultCriteriaSF = true;
                            this.State = SpotfireIntegrationState.Unsynched;
                            return;
                        }
                        break;
                    }
                }
            }
        }

        private static IList<ResultsCriteria.IResultsCriteriaBase> GetSortOrder(ResultsCriteria.ResultsCriteriaTable table)
        {
            List<ResultsCriteria.IResultsCriteriaBase> orderCriteria = new List<ResultsCriteria.IResultsCriteriaBase>(
                table.Criterias.Where(c => c.OrderById > 0));
            orderCriteria.Sort((a, b) => a.OrderById.CompareTo(b.OrderById));
            return orderCriteria;
        }

        private static bool SortOrderEquals(IList<ResultsCriteria.IResultsCriteriaBase> first, IList<ResultsCriteria.IResultsCriteriaBase> second)
        {
            if (first.Count != second.Count)
            {
                return false;
            }
            for (int i = 0; i < first.Count; i++)
            {
                if (first[i].Direction != second[i].Direction || first[i].GetType() != second[i].GetType())
                {
                    return false;
                }
                if (first[i] is ResultsCriteria.Field)
                {
                    ResultsCriteria.Field firstField = (ResultsCriteria.Field)first[i];
                    ResultsCriteria.Field secondField = (ResultsCriteria.Field)second[i];
                    if (firstField.Id != secondField.Id)
                    {
                        return false;
                    }
                }
                else if (first[i] is ResultsCriteria.AggregateFunction)
                {
                    ResultsCriteria.AggregateFunction firstAggregate = (ResultsCriteria.AggregateFunction)first[i];
                    ResultsCriteria.AggregateFunction secondAggregate = (ResultsCriteria.AggregateFunction)second[i];
                    if (firstAggregate.FunctionName != secondAggregate.FunctionName ||
                        firstAggregate.Parameters != secondAggregate.Parameters)
                    {
                        return false;
                    }
                }
                else if (first[i].Alias != second[i].Alias)
                {
                    return false;
                }
            }
            return true;
        }

        internal void SpotfireDocumentChanged(COEHitList newHitList)
        {
            // if (this.hitList == null || ! this.hitList.Equals(newHitList))
            if (newHitList == null) // Fixed CSBR-166789 Only call EndIntegration() asyncronously when spotfire document is changed and newHitlist is null 
            {
                SpotfireController.GetInstance().EndIntegration();
            }
        }

        internal void SpotfireExiting()
        {
            SpotfireController.GetInstance().EndIntegration();
        }

        private bool StateNotReady()
        {
            return this.State != SpotfireIntegrationState.Ready;// && this.State != SpotfireIntegrationState.Unsynched;
        }

        public void Form_RecordsetChanged(object sender, RecordsetChangedEventArgs e)
        {
            if (this.updatingSelection || StateNotReady())
            {
                return;
            }
            this.updatingSelection = true;  // prevent recursion
            SpotfireController.GetInstance().CBVNRecordsetChanged(sender, e);
            this.updatingSelection = false;
        }

        private void Form_RecordChanged(object sender, RecordChangedEventArgs e)
        {
            // Ignore this event if it occurs as a result of a change in Spotfire,
            // or if the Spotfire integration is still starting up to avoid deadlock.
            if (this.updatingSelection || StateNotReady())
            {
                bool bOpenAnalysis = Properties.settings.Default.OpenAnalysis;
                if (bOpenAnalysis)
                {
                    this.State = SpotfireIntegrationState.Unsynched;
                }
                return;
            }
            this.updatingSelection = true;  // prevent recursion
            SpotfireController.GetInstance().CBVNRecordChanged(sender, e);
            this.updatingSelection = false;
        }

        void ChemDataGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            // Ignore this event if it occurs as a result of a change in Spotfire,
            // or if the Spotfire integration is still starting up to avoid deadlock.
            if (this.updatingSelection || StateNotReady())
            {
                return;
            }
            SpotfireController.GetInstance().CBVNGridSelectionChanged(sender, e);
        }

        private void Form_FormClosed(object sender, ChemBioViz.NET.CBVFormClosedEventArgs e)
        {
            if (this.tableLoaderProxy != null && this.hitList != null
                            && this.State == SpotfireIntegrationState.Ready     // prevent showing more than once
                            && SpotfireIntegrationAddin.IsSpotfireRunning())    // JD: no warning if SF already closed
            {
                DialogResult result = MessageBox.Show(
                    "This action will disrupt the link to Spotfire. Do you\n" +
                    "wish to close the document currently open in Spotfire?",
                    "Spotfire Integration",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // When this is called, the form is already being cleaned up,
                    // so pull the form data from the stored COEHitList instead.
                    try
                    {
                        this.tableLoaderProxy.CloseCOEDocument(
                            this.hitList.DataViewID,
                            this.hitList.HitListID);
                    }
                    catch (CommunicationObjectFaultedException)
                    {
                        // We've already lost communication.  Ignore.
                    }
                }
            }
            this.State = SpotfireIntegrationState.Disconnected;
            // remove form events?
        }

        internal void CBVNRecordChanged(object sender, RecordChangedEventArgs e)
        {
            if (this.channelFactory == null || this.Form != sender)
            {
                return;
            }
            List<int> selectedRecords = new List<int>(1);
            selectedRecords.Add(e.cbvrecno);

            TellSpotfireRecordChanged(selectedRecords);
        }

        internal bool ConfirmAnalysisReplace()
        {
            RebuildWarningDialog dlg = new RebuildWarningDialog();
            DialogResult result = dlg.ShowDialog();
            if (dlg.IsChecked)
                Properties.settings.Default.WarnOnRebuild = false;
            return result == DialogResult.Yes;
        }

        internal bool ConfirmListReplace()
        {
            RefreshWarningDialog dlg = new RefreshWarningDialog();
            DialogResult result = dlg.ShowDialog();
            if (dlg.IsChecked)
                Properties.settings.Default.WarnOnRefresh = false;
            return result == DialogResult.Yes;
        }

        internal void CBVNRecordsetChanged(object sender, RecordsetChangedEventArgs e)
        {
            if (this.channelFactory == null || this.Form != sender)
            {
                return;
            }
            // if resultsCriteria has changed, force an update
            bool bRCChanged = false;
            string newRCStr = e.CurrentRC;
            if (this.hitList != null)
            {
                String ourRCStr = this.hitList.ResultsCriteriaXML;
                bRCChanged = !ourRCStr.Equals(newRCStr);
            }
            if (bRCChanged)
                e.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.Push;

            // consult prefs: warn about updating, auto-refresh
            bool bForce = e.ChangeAction == RecordsetChangedEventArgs.RecordsetChangeAction.Push;
            bool bWarn = Properties.settings.Default.WarnOnRefresh;
            bool bAutoRefresh = Properties.settings.Default.AutoRefresh;

            e.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.Push;
            if (!bAutoRefresh && !bForce)
                e.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.NoPush;
            else if (bWarn)
                e.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.Ask;

            // warn if new list exceeds size limit 
            bool bExceedsLimit = ExceedsListSizeLimit();
            if (!bExceedsLimit && e.ChangeAction != RecordsetChangedEventArgs.RecordsetChangeAction.NoPush) // CSBR-166766 Fixed
            {
                ReportSizeLimit();
                this.State = SpotfireIntegrationState.Unsynched;
                return;
            }

            switch (e.ChangeAction)
            {
                case RecordsetChangedEventArgs.RecordsetChangeAction.NoPush:
                    this.State = SpotfireIntegrationState.Unsynched;
                    return;
                case RecordsetChangedEventArgs.RecordsetChangeAction.Ask:
                    if (!ConfirmListReplace())
                    {
                        // this.State = SpotfireIntegrationState.Unsynched;
                        return;
                    }
                    break;
                case RecordsetChangedEventArgs.RecordsetChangeAction.Push:
                    break;
            }
            bool bOpenAnalysis = Properties.settings.Default.OpenAnalysis;
            int hitListID = this.Form.CurrQuery.HitListID;
            HitListType hitListType = this.Form.CurrQuery.IsSaved ? HitListType.SAVED : HitListType.TEMP;
            int numHits = this.Form.CurrQuery.NumHits;
            int dataViewID = this.Form.FormDbMgr.SelectedDataView.DataViewID;
            this.hitList = new COEHitList(hitListID, hitListType, numHits, dataViewID, newRCStr);

            this.State = SpotfireIntegrationState.Connecting;
            List<int> selectedRecords = this.Form.GetSelectedRecordNos();
            // Release the application thread now that we've got all the data we need from it.
            SpotfireController.GetInstance().ReleaseWaitingThreads();
            bool frmModified = this.form.Modified;
            try
            {
                if (this.tableLoaderProxy != null)
                {
                    if ((bRCChanged) || this.editResultCriteriaSF) ////CSBR-167455 CSBR-167458 CSBR-167349 fixed
                    {
                        this.editResultCriteriaSF = false;
                        this.tableLoaderProxy.CBVNResultsCriteriaChanged(newRCStr);
                    }
                    else
                    {
                        if (bOpenAnalysis)//Check if 'Open Analysis' option is selected or not.
                        {
                            //CSBR-151966. Get the opened document hitlist, which is opened using open Analysis
                            COEHitList openHitList = null;
                            try
                            {
                                openHitList = this.tableLoaderProxy.GetCOEHitList();
                            }
                            catch (CommunicationException) { }
                            //Assigning the opened document hitlist parameters to CBVNRecordsetChanged method, which will update the SF document Hitlist parameter at application.UpdateSpotfireDataTable()method at SpotfireExtensionMethods.cs
                            hitListID = openHitList.HitListID;
                            hitListType = openHitList.HitListType;
                            numHits = openHitList.NumHits;
                        }
                    }
                    this.tableLoaderProxy.CBVNRecordsetChanged(hitListID, hitListType, numHits);
                }
            }
            catch (FaultException<TableLoadFault> ex)
            {
                CBVUtil.ReportError(ex, "Spotfire addin: Error updating Spotfire recordset");
                //MessageBox.Show(String.Format("Spotfire is unable to update the form because: {0}", ex.Detail.Message));
                this.State = SpotfireIntegrationState.Unsynched;
                return;
            }
            catch (SystemException ex)
            {
                CBVUtil.ReportError(ex, "Spotfire addin: Error updating Spotfire recordset");
                MessageBox.Show(String.Format("Error updating Spotfire recordset: {0}", ex.ToString()));
                this.State = SpotfireIntegrationState.Disconnected;
                return;
            }

            TellSpotfireRecordChanged(selectedRecords);
            this.State = SpotfireIntegrationState.Ready;
        }

        internal void CBVNGridSelectionChanged(object sender, AfterSelectChangeEventArgs e)
        {
            if (this.channelFactory == null)
            {
                return;
            }
            List<int> selectedRecords = this.Form.GetSelectedRecordNos();
            TellSpotfireRecordChanged(selectedRecords);
        }

        private void TellSpotfireRecordChanged(List<int> selectedRecords)
        {
            try
            {
                if (this.Form.FormDbMgr.SelectedDataView != null) //CSBR-153634
                {
                    if (this.tableLoaderProxy != null)
                        this.tableLoaderProxy.CBVNRecordChanged(
                            this.Form.FormDbMgr.SelectedDataView.DataViewID,
                            this.Form.CurrQuery.HitListID, selectedRecords);
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                // If we get an exception here, it probably just means that Spotfire
                // has exited.  Ignore.
                // JD: this also happens if we pass a zero hitlist id (as when Retrieve All)
                this.State = SpotfireIntegrationState.Unsynched;
                CBVUtil.ReportError(ex, "Spotfire addin: Error updating Spotfire marking");
                //this.tableLoaderProxy = null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //CSBR: 150876.channelFactory object is made null. in-order to avoid the exception in EndIntegration method.
            this.channelFactory = null;
            EndIntegration();
        }

        #endregion
    }
}

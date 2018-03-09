using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.Common;

namespace Manager.Forms.DataViewManager.ContentArea
{
    public partial class DataviewBoard : GUIShellPage, ICallbackEventHandler
    {


        #region Properties
        private string PreviousSelectedNode
        {
            get
            {
                return (string)Session["PreviousSelectedNode"];
            }
            set
            {
                Session["PreviousSelectedNode"] = value;
            }
        }

        private COEDataViewManagerBO DataViewManager
        {
            get
            {
                return this.Master.GetDataViewBO().DataViewManager;
            }
        }
        #endregion

        #region Page Life Cycle Events
        protected override void OnInit(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
            this.DoneImageButton.ButtonClicked += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);
            this.TablesTreeViewUserControl.RemoveSchemaLink.Click += new EventHandler(RemoveSchemaLink_Click);
            base.OnInit(e);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void RemoveSchemaLink_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hdRemoveSchemaConfirmation.Value) && hdRemoveSchemaConfirmation.Value.Equals("1"))
                {
                    RemoveSchema(this.TablesTreeViewUserControl.SelectedDatabase);
                    this.TablesTreeViewUserControl.ReBindData(GetSchema());
                    BindInfo();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                hdRemoveSchemaConfirmation.Value = "0";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.SetDefaultAction(this.DoneImageButton.GetButtonUniqueID());
            this.SubscribeToUCEvents();
            if (!Page.IsPostBack)
            {
                this.SetControlsAttributtes();
                this.SetRemoveButtonVisibility();
                this.BindInfo();
            }
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added condition to check if previous page is NewDataView and it contains the paramcaller attribute.
            //This will be the case if the session ends while editing dataview.
            if (Page.PreviousPage != null &&
               ((Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTableAndFields") || Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTags"))
               || (Page.PreviousPage.AppRelativeVirtualPath.Contains("NewDataView.aspx") && this.Page.ClientQueryString.Contains(Constants.PageStates.Edit_DV.ToString()) && this.Page.ClientQueryString.Contains(Constants.ParamCaller))
                ))
            {
                this.TablesTreeViewUserControl.SelectedTableID = Page.Request.QueryString.Get(Constants.ParamCaller);
                if (Page.PreviousPage.AppRelativeVirtualPath.Contains("NewDataView.aspx"))
                {
                    //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
                    //Here setting value of paramcaller to SelectedTableID of Tabalesummary so that it can directly open table with this id.  
                    int tableid = -1;
                    int.TryParse(Page.Request.QueryString.Get(Constants.ParamCaller), out tableid);
                    this.TableSummary.SelectedTableId = tableid;
                }
            }
            else if (this.PreviousSelectedNode != null)
            {
                this.TablesTreeViewUserControl.SelectedTableID = this.PreviousSelectedNode;
            }
            this.Toolbar.DatabaseName = this.TablesTreeViewUserControl.SelectedDatabase;

            //Jira ID - 543, Disabling Refresh/Remove Schema buttons when there is only one published schema in Edit Master, user can not delete all schemas
            DropDownList schemaDropDownList = ((DropDownList)this.TablesTreeViewUserControl.FindControl("SchemaDropDownList"));
            if (schemaDropDownList.Items.Count > 1)
            {
                this.Toolbar.RemoveSchemaButton.Enabled = true;
            }
            else
            {
                this.Toolbar.RemoveSchemaButton.Enabled = false;
            }

            //CSBR-158344: Disabling Submit buttton when there are no published schemas in Edit Master
            if (schemaDropDownList.Items.Count >= 1)
            {
                this.Toolbar.SubmitButton.Enabled = true;
                this.DoneImageButton.Enabled = true;
                this.Toolbar.RefreshSchemaButton.Enabled = true;
                this.TablesTreeViewUserControl.RemoveSchemaLink.Enabled = true;
                this.TablesTreeViewUserControl.AddTableLink.Enabled = true;
            }
            else
            {
                this.Toolbar.SubmitButton.Enabled = false;
                this.DoneImageButton.Enabled = false;
                this.Toolbar.RefreshSchemaButton.Enabled = false;
                this.TablesTreeViewUserControl.RemoveSchemaLink.Enabled = false;
                this.TablesTreeViewUserControl.AddTableLink.Enabled = false;
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Event Handlers
        void DoneImageButton_ButtonClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (Page.IsValid)
            {
                Response.Redirect("ValidationSummary.aspx", false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void CancelImageButton_ButtonClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.GetDataViewBO().DataViewManager = COEDataViewManagerBO.NewManager(this.Master.GetDataViewBO().COEDataView);
            this.GoHome();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void TableSummaryUC_TableDeleted(object sender, CommandEventArgs e)
        {
            int tableId = (int)e.CommandArgument;
            this.TablesTreeViewUserControl.DeleteTable(tableId);
            TableBO tbl = this.DataViewManager.Tables.GetTable(tableId);
            string schema = tbl.DataBase;
            this.DataViewManager.Tables.Remove(tableId);
            this.SchemaSummary.BindSchemaDetails(schema);
            this.Toolbar.BindToolbar(schema);
            this.TablesTreeViewUserControl.DataBind(this.DataViewManager.Tables, this.DataViewManager.BaseTableId);
        }

        void TableSummaryUC_TableDuplicated(object sender, CommandEventArgs e)
        {
            int tableId = (int)e.CommandArgument;
            TableBO tableBO = this.GetTablesList().GetTable(tableId);
            this.DuplicateTable.DataBind(tableBO);
        }

        void DuplicateTableUC_Duplicate(object sender, CommandEventArgs e)
        {
            int tableId = (int)e.CommandArgument;
            TableBO tableBO = this.GetTablesList().GetTable(tableId);
            this.TablesTreeViewUserControl.SelectedTableID = tableBO.ID.ToString();
        }

        void DuplicateTableUC_Cancel(object sender, CommandEventArgs e)
        {
            TableListBO masterTables = this.GetTablesList();
            this.TablesTreeViewUserControl.DataBind(masterTables, this.DataViewManager.BaseTableId);
        }

        #endregion

        #region Methods
        public string GetSelectedDivJS()
        {
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added condition to check if previous page is NewDataView and it contains the paramcaller attribute.
            //This will be the case if the session ends while editing dataview.
            if (this.PreviousSelectedNode != null || (Page.PreviousPage != null &&
                   ((Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTableAndFields") ||
                   Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTags"))
                   || (Page.PreviousPage.AppRelativeVirtualPath.Contains("NewDataView.aspx")
                   && this.Page.ClientQueryString.Contains(Constants.PageStates.Edit_DV.ToString())
                   && this.Page.ClientQueryString.Contains(Constants.ParamCaller))
                   )))
            {
                TableBO table = this.DataViewManager.Tables.GetTable(int.Parse(this.TablesTreeViewUserControl.SelectedTableID));
                if (table != null)
                    return "YAHOO.DataviewBoardNS.callServer('SelectTable: " + table.ID + "', new YAHOO.widget.Record({" + string.Format("tableschema: \"{0}\", tablealias: \"{1}\", tableid: \"{2}\", tablename: \"{3}\", isbasetable: \"{4}\"", table.DataBase, table.Alias, table.ID, table.Name, (table.ID == this.DataViewManager.BaseTableId)) + "}));";
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        private void SubscribeToUCEvents()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (this.TableSummary != null)
            {
                this.TableSummary.TableDeleted += new CommandEventHandler(TableSummaryUC_TableDeleted);
                this.TableSummary.TableDuplicated += new CommandEventHandler(TableSummaryUC_TableDuplicated);
            }

            if (this.DuplicateTable != null)
            {
                this.DuplicateTable.Cancel += new CommandEventHandler(DuplicateTableUC_Cancel);
                this.DuplicateTable.Duplicate += new CommandEventHandler(DuplicateTableUC_Duplicate);
            }

            if (this.Toolbar != null)
            {
                this.Toolbar.Cancel += new EventHandler(CancelImageButton_ButtonClicked);
                this.Toolbar.Submit += new EventHandler(DoneImageButton_ButtonClicked);
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to set all the controls attributtes as Text, tooltip, etc...
        /// </summary>
        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEDataViewBO currentDataViewBO = this.Master.GetDataViewBO();
            if (Request["IsMaster"] != null && bool.Parse(Request["IsMaster"]))
            {
                if (currentDataViewBO == null || currentDataViewBO.ID != 0)
                {
                    currentDataViewBO = COEDataViewBO.GetMasterSchema();
                    this.Master.SetDataViewBO(currentDataViewBO);
                }
            }
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added null check since currentDataviewBo can be null
            if (currentDataViewBO != null)
                this.Master.SetPageTitle(String.Format(Resources.Resource.SelectTables_Pagel_Title, currentDataViewBO.Name, currentDataViewBO.ID));
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Method to set visibility of Remove button
        /// </summary>
        private void SetRemoveButtonVisibility()
        {
            //CBOE-885
            COEDataViewManagerBO theCOEDataViewManagerBO = DataViewManager;
            if (theCOEDataViewManagerBO != null && theCOEDataViewManagerBO.DataViewId == 0 && string.IsNullOrEmpty(theCOEDataViewManagerBO.BaseTable))
            {
                this.TablesTreeViewUserControl.RemoveSchemaLink.Visible = false;
            }
            else
            {
                this.TablesTreeViewUserControl.RemoveSchemaLink.Visible = true;
            }
        }

        /// <summary>
        /// Redirects to the Home page
        /// </summary>
        private void GoHome()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.ClearAllCurrentDVSessionVars();
            Response.Redirect(Constants.PublicContentAreaFolder + "Home.aspx", false);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to bind the datasource info of the UserControl.
        /// </summary>
        private void BindInfo()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            int baseTableID = this.GetBaseTableID();
            TableListBO tables = null;
            if (Request["IsMaster"] != null && bool.Parse(Request["IsMaster"]) && Session["SchemaChange"] != null && Convert.ToBoolean(Session["SchemaChange"]) == true)
            {
                COEDataViewBO theCOEDataViewBO = COEDataViewBO.GetMasterSchema();
                this.Master.SetDataViewBO(theCOEDataViewBO);
                Session["SchemaChange"] = false;
            }
            tables = this.GetTablesList();
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added null check since tables can be null
            if (tables != null)
            {
                if (this.TablesTreeViewUserControl != null)
                {
                    this.TablesTreeViewUserControl.DataBind(tables, baseTableID);
                    this.Toolbar.SetButtonControl(tables.Count);
                    this.hdnIsBaseTable.Value = Convert.ToString(tables.Count);
                }
                if (this.SchemaSummary != null && tables != null && tables.Count > 0)
                {
                    string databaseName = this.TablesTreeViewUserControl.GetSchemasFromBaseTableID(baseTableID);
                    if (!string.IsNullOrEmpty(databaseName))
                        this.SchemaSummary.BindSchemaDetails(databaseName);
                    else
                        this.SchemaSummary.BindSchemaDetails(tables[0].DataBase);
                }
                else
                {
                    //Coverity Fix :CID 19875
                    if (this.SchemaSummary != null)
                    {
                        this.SchemaSummary.BindSchemaDetails(string.Empty);
                    }
                }
            }
            else	//redirect to home page
                GoHome();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Gets a merged list of tables to be used as datasource
        /// </summary>
        /// <returns>A list of the available tables to choose by a user</returns>
        private TableListBO GetTablesList()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //Get the current DataViewBO
            COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added null check since dataviewBo can be null
            return dataViewBO == null ? null : dataViewBO.DataViewManager.Tables;
        }

        /// <summary>
        /// Gets the Base Table Id
        /// </summary>
        /// <returns>Id of the base table</returns>
        private int GetBaseTableID()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            int retVal = int.MinValue;
            COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
            if (dataViewBO != null)
                retVal = dataViewBO.DataViewManager.BaseTableId;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return retVal;
        }

        private void RemoveSchema(string database)
        {
            try
            {
                List<TableBO> tables = this.DataViewManager.Tables.GetTablesByDB(database);

                foreach (TableBO tbl in tables)
                {
                    if (this.DataViewManager.BaseTableId != tbl.ID)
                        this.DataViewManager.Tables.Remove(tbl.ID);
                }
                this.DataViewManager.RemoveOrphanRelationships();

            }
            catch
            {
                throw;
            }
        }

        private List<string> GetSchema()
        {
            List<string> schemas = null;
            try
            {
                schemas = new List<string>();
                if (this.DataViewManager.Tables.Count > 0)
                {
                    foreach (TableBO table in this.DataViewManager.Tables)
                    {
                        if (!schemas.Contains(table.DataBase))
                            schemas.Add(table.DataBase);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return schemas;
        }

        #endregion

        #region ICallbackEventHandler Members
        string result = string.Empty;
        string ICallbackEventHandler.GetCallbackResult()
        {
            return result;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("FilterSchema: "))
            {
                string database = eventArgument.Replace("FilterSchema: ", string.Empty);
                result = "FilterSchema: " + this.TablesTreeViewUserControl.GetTablesDataSource(database);
                result += this.SchemaSummary.GetPkDataSource(database);
                result += this.SchemaSummary.GetRelDataSource(database);
                result += this.SchemaSummary.GetLookupDataSource(database);
            }
            else if (eventArgument.StartsWith("SelectTable: "))
            {
                int tableId = int.Parse(eventArgument.Replace("SelectTable: ", string.Empty));
                this.TableSummary.SelectedTableId = tableId;
                result = "SelectTable: " + this.TableSummary.GetPkDataSource(tableId);
                result += this.TableSummary.GetRelDataSource(tableId);
                result += this.TableSummary.GetLookupDataSource(tableId);
                result += this.TableSummary.GetTagsDataSource(tableId);
                result += this.TableSummary.GetIndexesDataSource(tableId);
                TableBO currentTable = this.DataViewManager.Tables.GetTable(tableId);
                //bool blnPresent = this.DataViewManager.IsDatabasePresentInMater(currentTable.DataBase);
                //if (!blnPresent)
                //    result += "Message: " + Resources.Resource.SchemaDeleteFromMaster_Label_Text;
                //else
                result += "Message: ";

                result += "AliasValue:" + System.Web.HttpUtility.HtmlDecode(currentTable.Alias);

            }
            else if (eventArgument.StartsWith("DeleteTable: "))
            {
                int tableId = int.Parse(eventArgument.Replace("DeleteTable: ", string.Empty));
                TableBO tbl = this.DataViewManager.Tables.GetTable(tableId);
                if (tbl != null)
                {
                    string database = tbl.DataBase;
                    string message = string.Empty;
                    this.TablesTreeViewUserControl.DeleteTable(tableId);
                    this.DataViewManager.Tables.Remove(tbl);
                    this.DataViewManager.Relationships.Remove(new List<int>(new int[] { tbl.ID }));
                    Session["SchemaChange"] = true;
                    result = "DeleteTable: " + this.TablesTreeViewUserControl.GetTablesDataSource(database);
                    result += this.SchemaSummary.GetPkDataSource(database);
                    result += this.SchemaSummary.GetRelDataSource(database);
                    result += this.SchemaSummary.GetLookupDataSource(database);
                    result += "Message: " + message.Trim();
                }
            }
            else if (eventArgument.StartsWith("RemoveSchema: "))
            {
                string database = eventArgument.Replace("RemoveSchema: ", string.Empty);
                this.RemoveSchema(database);

                result = "RemoveSchema: window.location.href = 'DataviewBoard.aspx';";
            }
            else if (eventArgument.StartsWith("DuplicateTable: "))
            {
                int tableId = int.Parse(eventArgument.Replace("DuplicateTable: ", string.Empty));
                TableBO tableBO = this.GetTablesList().GetTable(tableId);
                string script = this.DuplicateTable.DataBind(tableBO);
                result = "DuplicateTable: YAHOO.DataviewBoardNS.showRightPanelDiv('DuplicateTableDiv'); ";
                result += script;
            }
            else if (eventArgument.StartsWith("TableDuplicated: "))
            {
                string[] tableInfo = eventArgument.Replace("TableDuplicated: ", string.Empty).Split(',');
                int tableId = int.Parse(tableInfo[0]);
                string tableName = tableInfo[1];
                TableBO tbl = this.DataViewManager.Tables.GetTable(tableId);

                //Coverity Fixes : CBOE-313 :CID-11854 
                if (tbl != null)
                {
                    string database = tbl.DataBase;

                    bool isDuplicate = this.TableSummary.IsDuplicateTableAlias(database, tableName);
                    if (!isDuplicate)
                    {
                        int newTableId = this.DataViewManager.Tables.CloneAndAddTable(tableId, "");
                        tableId = newTableId;
                        TableBO table = this.DataViewManager.Tables.GetTable(tableId);
                        if (table != null)
                            table.Alias = tableName;
                    }

                    result = "TableDuplicated: " + this.TablesTreeViewUserControl.GetTablesDataSource(database);
                    result += this.SchemaSummary.GetPkDataSource(database);
                    result += this.SchemaSummary.GetRelDataSource(database);
                    result += this.SchemaSummary.GetLookupDataSource(database);
                    result += "document.getElementById('" + this.TableSummary.SelectedTableIDHiddenClientID + "').value = '" + tableId.ToString() + "'";
                    if (isDuplicate)
                        result += "Message: Warning! " + Resources.Resource.DuplicateTableAlias + "\n" + Resources.Resource.DuplicateTableError;
                    else
                        result += "Message: ";
                }
            }
        }

        #endregion
    }
}

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
using System.Reflection;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

public partial class Forms_DataViewManager_ContentArea_SelectMasterDVTables : GUIShellPage
{
    #region Properties
    private string PreviousSelectedNode
    {
        get
        {
            return (string) Session["PreviousSelectedNode"];
        }
        set
        {
            Session["PreviousSelectedNode"] = value;
        }
    }
    
    private COEDataViewBO MergedDataViewBO {
        get
        {
            if(Session[Constants.COEDataViewBO] == null)
            {
                COEDataViewBO masterDV = COEDataViewBO.GetMasterSchema();
                foreach(COEDatabaseBO db in this.GetPublishedSchemas())
                {
                    this.MergeDataView(db.COEDataView, masterDV);
                }
                Session[Constants.COEDataViewBO] = masterDV;
            }
            return (COEDataViewBO) Session[Constants.COEDataViewBO];
        }
    }
    #endregion

    #region Variables
    List<string> _schemasInDataView = new List<string>();
    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Button EventHandlers
        this.DoneImageButton.ButtonClicked += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        #endregion

        this.ScriptManager1.RegisterAsyncPostBackControl(this.SelectTablesUserControl);
        this.ScriptManager1.RegisterPostBackControl(this.DoneImageButton);
        this.ScriptManager1.RegisterPostBackControl(this.CancelImageButton);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SubscribeToUCEvents();
        if (!Page.IsPostBack)
        {
            this.ReadURLParams();
            this.SetControlsAttributtes();
            this.BindInfo();
        }
        if( Page.PreviousPage != null && 
            (Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTableAndFields") || 
            Page.PreviousPage.AppRelativeVirtualPath.Contains("EditTags")))
        {
            this.SelectTablesUserControl.SelectedNode = Page.Request.QueryString.Get(Constants.ParamCaller);
        }
        else if(this.PreviousSelectedNode != null)
        {
            this.SelectTablesUserControl.SelectedNode = this.PreviousSelectedNode;
        }

        this.ConfirmationAreaUserControl.Visible = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (MergedDataViewBO != null)
        {
            bool areTablesSelected = MergedDataViewBO.DataViewManager.Tables.GetTablesFromMasterSchema(false).Count > 0;
            this.Toolbar.SubmitButton.Enabled = this.DoneImageButton.Enabled = MergedDataViewBO.IsSavable && areTablesSelected;
        }
    }
    #endregion
    
    #region Event Handlers
    void DoneImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(Page.IsValid)
        {
            this.GetSelectedTables();
            Response.Redirect("ValidationSummary.aspx", false);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoHome();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void AddSchemaUC_SchemaCancelAdd(object sender, CommandEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SchemaSummary.Visible = this.DuplicateTable.Visible = this.AddSchemasUserControl.Visible = this.TableSummary.Visible = false;
        this.AddSchemasUserControl.Visible = false;
        if (!String.IsNullOrEmpty(this.SelectTablesUserControl.SelectedNode))
        {
            int tableId;
            if (int.TryParse(this.PreviousSelectedNode, out tableId))
            {
                this.TableSummary.Visible = true;
            }
            else
            {
                this.SchemaSummary.Visible = true;
                this.SchemaSummary.BindSchemaDetails(this.PreviousSelectedNode);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void AddSchemasUserControl_ErrorOcurred(object sender, EventArgs e)
    {
        this.ErrorAreaUserControl.Text = this.AddSchemasUserControl.ErrorMessage;
        this.ErrorAreaUserControl.Visible = true;
        this.SchemaSummary.Visible = this.DuplicateTable.Visible = this.AddSchemasUserControl.Visible = this.TableSummary.Visible = false;
        this.AddSchemasUserControl.Visible = true;
        this.AddSchemasUserControl.DataBind(this.GetSchemasToAddToDataView());
    }

    void AddSchemaUC_SchemaAdded(object sender, CommandEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (sender is Forms_Public_UserControls_ImageButton)
        {
            if (!string.IsNullOrEmpty(((Forms_Public_UserControls_ImageButton)sender).CommandArgument))
                this.AddSchema(((Forms_Public_UserControls_ImageButton)sender).CommandArgument);
        }
        this.DuplicateTable.Visible = this.TableSummary.Visible = this.AddSchemasUserControl.Visible = false;
        this.SchemaSummary.Visible = true;
        this.SelectTablesUserControl.ExpandedNode = this.SelectTablesUserControl.SelectedNode = (string) e.CommandArgument + (string) e.CommandArgument;
        this.ErrorAreaUserControl.Visible = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void SelectTablesUC_NodeSelected(object sender, CommandEventArgs e)
    {
        /* Parent DataKey: "NAME=CHEMACXDB|ACTION=DoNothing"
        Child DataKey: "ID=150|NAME=VW_PROPERTIES|DATABASE=CHEMACXDB|ALIAS=VW_PROPERTIES" */

        Infragistics.WebUI.UltraWebNavigator.Node nodeClicked = (Infragistics.WebUI.UltraWebNavigator.Node)e.CommandArgument;
        string selectedTable = nodeClicked.DataKey.ToString().Split('|')[0];
        if(!string.IsNullOrEmpty(selectedTable))
        {
            if(selectedTable.StartsWith("ID="))
            {
                //It is a Table Node
                selectedTable = selectedTable.Remove(0, 3);
                this.TableSummary.Visible = true;
                this.TableSummary.BindTableDetails(nodeClicked.DataKey.ToString(), nodeClicked.Checked);
                this.DuplicateTable.Visible = this.SchemaSummary.Visible = this.AddSchemasUserControl.Visible = false;
                this.PreviousSelectedNode = selectedTable;
                this.Toolbar.BindToolbar(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.DATABASE));
            }
            else
            {
                //It is a Schema Node
                this.SchemaSummary.Visible = true;
                string dbName = Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.NAME);
                this.SchemaSummary.BindSchemaDetails(dbName);
                this.DuplicateTable.Visible = this.TableSummary.Visible = this.AddSchemasUserControl.Visible = false;
                this.PreviousSelectedNode = dbName;
                this.Toolbar.BindToolbar(dbName);
            }
        }
        this.ErrorAreaUserControl.Visible = false;
    }

    void SelectTablesUC_NodeChecked(object sender, CommandEventArgs e)
    {
        Infragistics.WebUI.UltraWebNavigator.Node nodeClicked = ((Infragistics.WebUI.UltraWebNavigator.Node) e.CommandArgument);
        this.ModifyDataView(nodeClicked);
        string selectedTable = nodeClicked.DataKey.ToString().Split('|')[0];
        if (!string.IsNullOrEmpty(selectedTable))
        {
            if (selectedTable.StartsWith("NAME="))
            {
                this.SchemaSummary.BindSchemaDetails(this.SchemaSummary.DatabaseName);
            }
        }
        if(nodeClicked.Selected)
        {
            SelectTablesUC_NodeSelected(sender, e);
        }
        else
        {
            this.TableSummary.RefreshSummary();
        }
    }

    void SelectTablesUC_SortApplied(object sender, EventArgs e)
    {
        this.ErrorAreaUserControl.Visible = false;
    }

    void TableSummaryUC_TableDeleted(object sender, CommandEventArgs e)
    {
        int tableId = (int) e.CommandArgument;
        this.SelectTablesUserControl.DeleteTable(tableId, false);
        this.TableSummary.BindTableDetails(tableId, false);
    }

    void TableSummaryUC_TableDuplicated(object sender, CommandEventArgs e)
    {
        this.SchemaSummary.Visible = this.TableSummary.Visible = this.AddSchemasUserControl.Visible = false;
        this.DuplicateTable.Visible = true;
        int tableId = (int) e.CommandArgument;
        TableBO tableBO = this.MergedDataViewBO.DataViewManager.Tables.GetTable(tableId);
        this.DuplicateTable.DataBind(tableBO);
    }

    void TableSummary_EditTable(object sender, CommandEventArgs e)
    {
        //this.GetSelectedTables();
    }

    void DuplicateTableUC_Duplicate(object sender, CommandEventArgs e)
    {
        this.DuplicateTable.Visible = this.SchemaSummary.Visible = this.AddSchemasUserControl.Visible = false;
        this.TableSummary.Visible = true;
        int tableId = (int) e.CommandArgument;
        TableBO tableBO = this.MergedDataViewBO.DataViewManager.Tables.GetTable(tableId);
        this.SelectTablesUserControl.AddTable(tableBO);
        this.SelectTablesUserControl.SelectedNode = tableBO.ID.ToString();
    }

    void DuplicateTableUC_Cancel(object sender, CommandEventArgs e)
    {
        this.DuplicateTable.Visible = this.SchemaSummary.Visible = this.AddSchemasUserControl.Visible = false;
        this.TableSummary.Visible = true;
        List<int> tableIds = this.GetSelectedTableIds();
        this.SelectTablesUserControl.DataBind(MergedDataViewBO.DataViewManager.Tables, tableIds, int.MinValue);
    }

    private List<int> GetSelectedTableIds()
    {
        List<int> list = new List<int>();
        foreach(TableBO tbl in MergedDataViewBO.DataViewManager.Tables)
        {
            if(!tbl.FromMasterSchema)
                list.Add(tbl.ID);
        }
        return list;
    }

    void ToolbarUC_AddSchema(object sender, EventArgs e)
    {
        this.AddSchemasUserControl.Visible = true;
        this.AddSchemasUserControl.DataBind(this.GetSchemasToAddToDataView());
        this.ErrorAreaUserControl.Visible = this.DuplicateTable.Visible = this.TableSummary.Visible = this.SchemaSummary.Visible = false;
    }

    void ToolbarUC_SchemaRemoved(object sender, CommandEventArgs e)
    {
        this.ErrorAreaUserControl.Visible = this.DuplicateTable.Visible = this.TableSummary.Visible = this.AddSchemasUserControl.Visible = false;
        this.SchemaSummary.Visible = true;

        List<int> tableIds = this.GetSelectedTableIds();
        this.SelectTablesUserControl.DataBind(this.MergedDataViewBO.DataViewManager.Tables, tableIds, int.MinValue);
        this.SelectTablesUserControl.SelectedNode = this.SelectTablesUserControl.FirstNode;

        string databaseName = e.CommandArgument.ToString();
        this.ModifyDataView(databaseName, "SCHEMA", true);
        if(tableIds.Count == 0)
        {
            this.SchemaSummary.Visible = false;
            this.Toolbar.BindToolbar(string.Empty);
        }
    }

    void Toolbar_RefreshSchema(object sender, CommandEventArgs e)
    {
        List<int> tableIds = this.GetSelectedTableIds();
        this.SelectTablesUserControl.DataBind(this.MergedDataViewBO.DataViewManager.Tables, tableIds, int.MinValue);
    }
    #endregion

    #region Methods
    private void SubscribeToUCEvents()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(this.AddSchemasUserControl != null)
        {
            this.AddSchemasUserControl.SchemaAdded += new CommandEventHandler(AddSchemaUC_SchemaAdded);
            this.AddSchemasUserControl.Cancel += new CommandEventHandler(AddSchemaUC_SchemaCancelAdd);
            this.AddSchemasUserControl.ErrorOcurred += new EventHandler<EventArgs>(AddSchemasUserControl_ErrorOcurred);
        }

        if(this.SelectTablesUserControl != null)
        {
            this.SelectTablesUserControl.NodeSelected += new CommandEventHandler(SelectTablesUC_NodeSelected);
            this.SelectTablesUserControl.NodeChecked += new CommandEventHandler(SelectTablesUC_NodeChecked);
            this.SelectTablesUserControl.SortApplied += new EventHandler(SelectTablesUC_SortApplied);
        }

        if(this.TableSummary != null)
        {
            this.TableSummary.TableDeleted += new CommandEventHandler(TableSummaryUC_TableDeleted);
            this.TableSummary.TableDuplicated += new CommandEventHandler(TableSummaryUC_TableDuplicated);
            this.TableSummary.EditTable += new CommandEventHandler(TableSummary_EditTable);
        }

        if(this.DuplicateTable != null)
        {
            this.DuplicateTable.Cancel += new CommandEventHandler(DuplicateTableUC_Cancel);
            this.DuplicateTable.Duplicate += new CommandEventHandler(DuplicateTableUC_Duplicate);
        }

        if(this.Toolbar != null)
        {
            this.Toolbar.AddSchema += new EventHandler(ToolbarUC_AddSchema);
            this.Toolbar.SchemaRemoved += new CommandEventHandler(ToolbarUC_SchemaRemoved);
            this.Toolbar.RefreshSchema += new CommandEventHandler(Toolbar_RefreshSchema);
            this.Toolbar.Cancel += new EventHandler(CancelImageButton_ButtonClicked);
            this.Toolbar.Submit += new EventHandler(DoneImageButton_ButtonClicked);
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to bind the datasource info of the UserControl.
    /// </summary>
    private void BindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> tableIds = this.GetSelectedTableIds();
        this.GetSchemasInDataView(this.MergedDataViewBO);
        if(string.IsNullOrEmpty(this.PreviousSelectedNode))
            this.SelectTablesUserControl.ExpandedNode = "NONE";
        if (this.SelectTablesUserControl.Visible)
            this.SelectTablesUserControl.DataBind(this.MergedDataViewBO.DataViewManager.Tables, tableIds, int.MinValue);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to bind the datasource info of the UserControl.
    /// </summary>
    /// <param name="schema">Schema just added which we should show as selected all the tables</param>
    private void BindInfo(string schema)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> tableIds = this.GetSelectedTableIds();
        if (this.SelectTablesUserControl.Visible)
            this.SelectTablesUserControl.DataBind(this.MergedDataViewBO.DataViewManager.Tables, tableIds, int.MinValue);
        if(this.AddSchemasUserControl.Visible)
        {
            this.AddSchemasUserControl.DataBind(this.GetSchemasToAddToDataView());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ReadURLParams()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!string.IsNullOrEmpty(Request[Constants.RecreateDV]) && bool.Parse(Request[Constants.RecreateDV]) == true)
        {
            this.Master.SetDataViewBO(COEDataViewBO.GetMasterSchema());
            foreach(COEDatabaseBO db in this.GetPublishedSchemas())
            {
                this.MergeDataView(db.COEDataView, this.MergedDataViewBO);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void InvalidPageRequest(bool masterDBExists)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.HideControls();
        if(masterDBExists)
            this.Master.DisplayMessagesPage(Constants.MessagesCode.MasterDataViewExists, GUIShellTypes.MessagesButtonType.Back);
        else
            this.Master.DisplayMessagesPage(Constants.MessagesCode.InvalidDataView, GUIShellTypes.MessagesButtonType.Back);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void HideControls()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DuplicateTable.Visible = this.SchemaSummary.Visible = this.TableSummary.Visible = this.SelectTablesUserControl.Visible = this.AddSchemasUserControl.Visible = this.DoneImageButton.Visible = this.Toolbar.SubmitButton.Visible = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void AddSchema(string schemToAdd)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Get DataView and Merge it to the current Master DataView.
        COEDatabaseBO databaseBO = COEDatabaseBO.Get(schemToAdd);
        if (databaseBO != null)
        {
            if (databaseBO.COEDataView != null)
            {
                this.MergeDataView(databaseBO.COEDataView, this.MergedDataViewBO);
                this.BindInfo(schemToAdd);
                ModifyDataView(databaseBO.Name, "SCHEMA", false);
            }
        }

        BindInfo(schemToAdd);
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void MergeDataView(COEDataView dataView, COEDataViewBO dvBO)
    {
        RelationshipListBO relationshipsBO = RelationshipListBO.NewRelationShipListBO(dataView.Relationships);
        TableListBO tableListBO = TableListBO.NewTableListBO(dataView.Tables);
        dvBO.DataViewManager.Merge(tableListBO, relationshipsBO);
    }

    /// <summary>
    /// Get all the selected tables in the UserControl
    /// </summary>
    private void GetSelectedTables()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> selectedTablesIds = null;
        COEDataViewBO dataViewBO = MergedDataViewBO;
        if(this.SelectTablesUserControl.Visible)
            selectedTablesIds = this.SelectTablesUserControl.UnBind();
        //Mark as FromMasterSchema those selected tables for further steps.
        foreach (int tableId in selectedTablesIds)
        {
            TableBO table = dataViewBO.DataViewManager.Tables.GetTable(tableId);
            if (table != null) table.FromMasterSchema = false;
        }
        if (dataViewBO != null)
        {
            if (dataViewBO.DataViewManager.Tables.Count != selectedTablesIds.Count)
            {
                //Find not selected tables.
                /*List<int> tablesIdsToRemove = this.FindTablesToRemove(selectedTablesIds, dataViewBO.DataViewManager.Tables);
                if (tablesIdsToRemove.Count > 0)
                {
                    dataViewBO.DataViewManager.Tables.Remove(tablesIdsToRemove);
                    //Remove relationships that have ids of tables that are not present in the current DV.
                    dataViewBO.DataViewManager.Relationships.Remove(tablesIdsToRemove);
                    dataViewBO.DataViewManager.RemoveOrphanRelationships();
                }*/
            }
        }
        else
        {
            //TODO: Handle this kind of error
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Finds all the tables Id to remove from the current tables object
    /// </summary>
    /// <param name="selectedTables">All selected tables by the user</param>
    /// <param name="tables">Full set of tables in the current DataView</param>
    /// <returns>just the difference among the two params</returns>
    /*private List<int> FindTablesToRemove(List<int> selectedTables, TableListBO tables)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> tablesToRemoveIdsList = new List<int>();
        foreach (TableBO table in tables)
        {
            if (!selectedTables.Contains(table.ID))
                tablesToRemoveIdsList.Add(table.ID);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return tablesToRemoveIdsList;
    }*/

    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.SelectTablesMasterDataview_Pagel_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private COEDatabaseBOList GetPublishedToAdd()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDatabaseBOList publishedSchemas = COEDatabaseBOList.GetList(true);
        foreach (string schema in _schemasInDataView)
            publishedSchemas.Remove(COEDatabaseBO.Get(schema)); //Do a directly remove
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return publishedSchemas;
    }

    private void GetSchemasInDataView(COEDataViewBO dataView)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        foreach (TableBO table in dataView.DataViewManager.Tables)
        {
            if (!_schemasInDataView.Contains(table.DataBase))
                _schemasInDataView.Add(table.DataBase);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private COEDatabaseBOList GetPublishedSchemas()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDatabaseBOList publishedDataBases = COEDatabaseBOList.GetList(true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return publishedDataBases;
    }

    private COEDatabaseBOList GetUnPublishedSchemas()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Get all the schema in the Db instance.(Unpublish ones)
        COEDatabaseBOList unpublishedSchemas = COEDatabaseBOList.GetList(false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return unpublishedSchemas;
    }

    private COEDatabaseBOList GetSchemasToAddToDataView()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDatabaseBOList schemas = this.GetUnPublishedSchemas();
        foreach (COEDatabaseBO schema in this.GetPublishedToAdd())
            schemas.Add(schema);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return schemas;
    }

    private void ModifyDataView(Infragistics.WebUI.UltraWebNavigator.Node nodeClicked)
    {
        if(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
        {
            this.ModifyDataView(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.NAME), "SCHEMA", nodeClicked.Checked);
        }
        else if(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.Grouping.ToString())
        {
            this.ModifyDataView(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.NAME), nodeClicked.Text.ToUpper(), nodeClicked.Checked);
        }
        else
        {
            this.ModifyDataView(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.ID), "TABLE", nodeClicked.Checked);

        }
    }
    
    private void ModifyDataView(string identifier, string type, bool toAdd)
    {
        if(type == "SCHEMA")
        {
            foreach(TableBO table in this.MergedDataViewBO.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && table.ID != this.MergedDataViewBO.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else if(type == "TABLES")
        {
            foreach(TableBO table in this.MergedDataViewBO.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && !table.IsView && table.ID != this.MergedDataViewBO.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else if(type == "VIEWS")
        {
            foreach(TableBO table in this.MergedDataViewBO.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && table.IsView && table.ID != this.MergedDataViewBO.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else
        {
            foreach(TableBO table in this.MergedDataViewBO.DataViewManager.Tables)
            {
                if(table.ID.ToString() == identifier && table.ID != this.MergedDataViewBO.DataViewManager.BaseTableId)
                {
                    table.FromMasterSchema = !toAdd;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Go to home page
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        this.Session.Remove("PreviousSelectedNode");
        Response.Redirect(Constants.PublicContentAreaFolder + "Home.aspx", false);
        //Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false); UpdatePanel does not like transfers at all
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

}

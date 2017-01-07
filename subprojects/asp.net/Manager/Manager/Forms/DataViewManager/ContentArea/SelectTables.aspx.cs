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

public partial class Forms_DataViewManager_ContentArea_SelectTables : GUIShellPage
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
            this.SetControlsAttributtes();
            this.BindInfo();
        }
        if (Page.PreviousPage != null && 
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
        COEDataViewBO dataviewBO = this.Master.GetDataViewBO();
        if (dataviewBO != null)
        {
            bool areTablesSelected = dataviewBO.DataViewManager.Tables.GetTablesFromMasterSchema(false).Count > 0;
            this.Toolbar.SubmitButton.Enabled = this.DoneImageButton.Enabled = dataviewBO.IsSavable && areTablesSelected;
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
            //Server.Transfer("ValidationSummary.aspx", false); UpdatePanel does not like transfers at all
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoHome();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void SelectTablesUC_NodeSelected(object sender, CommandEventArgs e)
    {
        /* Parent DataKey: "NAME=CHEMACXDB|ACTION=DoNothing"
        Child DataKey: "ID=150|NAME=VW_PROPERTIES|DATABASE=CHEMACXDB|ALIAS=VW_PROPERTIES" */

        Infragistics.WebUI.UltraWebNavigator.Node nodeClicked = (Infragistics.WebUI.UltraWebNavigator.Node) e.CommandArgument;
        string selectedTable = nodeClicked.DataKey.ToString().Split('|')[0];
        if(!string.IsNullOrEmpty(selectedTable))
        {
            if(selectedTable.StartsWith("ID="))
            {
                //It is a Table Node
                selectedTable = selectedTable.Remove(0, 3);
                this.TableSummary.Visible = true;
                this.TableSummary.BindTableDetails(nodeClicked.DataKey.ToString(), nodeClicked.Checked);
                this.DuplicateTable.Visible = this.SchemaSummary.Visible = false;
                this.PreviousSelectedNode = selectedTable;
                this.Toolbar.BindToolbar(Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.DATABASE));
            }
            else
            {
                //It is a Schema Node
                this.SchemaSummary.Visible = true;
                string dbName = Utilities.GetParamInDataKey(nodeClicked.DataKey.ToString(), Constants.DataKeysParam.NAME);
                this.SchemaSummary.BindSchemaDetails(dbName);
                this.DuplicateTable.Visible = this.TableSummary.Visible = false;
                this.PreviousSelectedNode = dbName;
                this.Toolbar.BindToolbar(dbName);
            }
        }
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
        //TODO: Check if anything is needed here
    }

    void TableSummaryUC_TableDeleted(object sender, CommandEventArgs e)
    {
        int tableId = (int) e.CommandArgument;
        this.SelectTablesUserControl.DeleteTable(tableId, false);
        this.TableSummary.BindTableDetails(tableId, false);
    }

    void TableSummaryUC_TableDuplicated(object sender, CommandEventArgs e)
    {
        this.SchemaSummary.Visible = this.TableSummary.Visible = false;
        this.DuplicateTable.Visible = true;
        int tableId = (int) e.CommandArgument;
        TableBO tableBO = this.GetTablesList().GetTable(tableId);
        this.DuplicateTable.DataBind(tableBO);
    }

    void TableSummary_EditTable(object sender, CommandEventArgs e)
    {
        //this.GetSelectedTables();
    }

    void DuplicateTableUC_Duplicate(object sender, CommandEventArgs e)
    {
        this.DuplicateTable.Visible = this.SchemaSummary.Visible = false;
        this.TableSummary.Visible = true;
        int tableId = (int) e.CommandArgument;
        TableBO tableBO = this.GetTablesList().GetTable(tableId);
        this.SelectTablesUserControl.AddTable(tableBO);
        this.SelectTablesUserControl.SelectedNode = tableBO.ID.ToString();
    }

    void DuplicateTableUC_Cancel(object sender, CommandEventArgs e)
    {
        this.DuplicateTable.Visible = this.SchemaSummary.Visible = false;
        this.TableSummary.Visible = true;
        List<int> tableIds = this.GetBeforeMergeTableIds();
        TableListBO masterTables = this.GetTablesList();
        this.SelectTablesUserControl.DataBind(masterTables, tableIds, this.DataViewManager.BaseTableId);
    }

    void ToolbarUC_AddSchema(object sender, EventArgs e)
    {
        this.DuplicateTable.Visible = this.TableSummary.Visible = this.SchemaSummary.Visible = false;
    }

    void ToolbarUC_SchemaRemoved(object sender, CommandEventArgs e)
    {
        this.DuplicateTable.Visible = this.TableSummary.Visible = false;
        this.SchemaSummary.Visible = true;

        List<int> tableIds = this.GetBeforeMergeTableIds();
        TableListBO masterTables = this.GetTablesList();
        this.SelectTablesUserControl.DataBind(masterTables, tableIds, int.MinValue);
        //this.GetSelectedTables();
        string databaseName = e.CommandArgument.ToString();
        this.ModifyDataView(databaseName, "SCHEMA", true);
    }
    #endregion

    #region Methods
    private void SubscribeToUCEvents()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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
            this.Toolbar.Cancel += new EventHandler(CancelImageButton_ButtonClicked);
            this.Toolbar.Submit += new EventHandler(DoneImageButton_ButtonClicked);
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Get all the selected tables in the UserControl
    /// </summary>
    private void GetSelectedTables()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> selectedTablesIds = null;
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.SelectTablesUserControl != null)
            selectedTablesIds = this.SelectTablesUserControl.UnBind();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO currentDataViewBO = this.Master.GetDataViewBO();
        this.Master.SetPageTitle(String.Format(Resources.Resource.SelectTables_Pagel_Title, currentDataViewBO.Name, currentDataViewBO.ID));
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Redirects to the Home page
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Response.Redirect(Constants.PublicContentAreaFolder + "Home.aspx", false);
        //Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false); UpdatePanel does not like transfers at all
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to bind the datasource info of the UserControl.
    /// </summary>
    private void BindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> tableIds = this.GetBeforeMergeTableIds();
        if(string.IsNullOrEmpty(this.PreviousSelectedNode))
            this.SelectTablesUserControl.ExpandedNode = "NONE";
        if (this.SelectTablesUserControl != null)
            this.SelectTablesUserControl.DataBind(this.GetTablesList(), tableIds, this.GetBaseTableID());
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
        //Get the master Schema
        COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();//.GetMasterDataView(); 
        //Merge tables if the master has
        if(masterDataViewBO.DataViewManager.Tables.Count > 0)
            dataViewBO.DataViewManager.Merge(masterDataViewBO.DataViewManager.Tables, masterDataViewBO.DataViewManager.Relationships);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataViewBO.DataViewManager.Tables;
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

    /// <summary>
    /// List of tables to show as selected the first time in the webtree
    /// </summary>
    /// <returns>List of tables to display as selected checkboxes</returns>
    private List<int> GetBeforeMergeTableIds()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        List<int> retVal = new List<int>();
        foreach(TableBO table in this.Master.GetDataViewBO().DataViewManager.Tables)
        {
            if(!table.FromMasterSchema)
                retVal.Add(table.ID);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
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
            foreach(TableBO table in this.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && table.ID != this.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else if(type == "TABLES")
        {
            foreach(TableBO table in this.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && !table.IsView && table.ID != this.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else if(type == "VIEWS")
        {
            foreach(TableBO table in this.DataViewManager.Tables)
            {
                if(table.DataBase == identifier && table.IsView && table.ID != this.DataViewManager.BaseTableId)
                    table.FromMasterSchema = !toAdd;
            }
        }
        else
        {
            foreach(TableBO table in this.DataViewManager.Tables)
            {
                if(table.ID.ToString() == identifier && table.ID != this.DataViewManager.BaseTableId)
                {
                    table.FromMasterSchema = !toAdd;
                    break;
                }
            }
        }
    }
    #endregion

}

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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using Infragistics.WebUI.UltraWebNavigator;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using System.Text;
using System.Text.RegularExpressions;

public partial class Lookup : System.Web.UI.UserControl
{
    #region Variables
    private string _defaultJoinType = COEDataView.JoinTypes.OUTER.ToString();
    private Constants.SortCriterias _defaultCriteria = Constants.SortCriterias.Database_ASC;
    #endregion

    #region Properties

    /// <summary>
    /// Current DataView that I'm working on.
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
        }
        set
        {
            if (value != null && Session[Constants.COEDataViewBO] != null)
                Session[Constants.COEDataViewBO] = value;
        }
    }

    /// <summary>
    /// Selected field to be the lookup field.
    /// </summary>
    private int SelectedFieldId
    {
        get
        {
            return ViewState[Constants.FieldId] != null ? (int)ViewState[Constants.FieldId] : int.MinValue;
        }
        set
        {
           ViewState[Constants.FieldId] = value;
        }
    }

    /// <summary>
    /// Field to set up a lookup field
    /// </summary>
    private int SourceFieldId
    {
        get
        {
            return ViewState[Constants.SourceFieldId] != null ? (int)ViewState[Constants.SourceFieldId] : int.MinValue;
        }
        set
        {
            ViewState[Constants.SourceFieldId] = value;
        }
    }

    /// <summary>
    /// Check if the fields table can be shown or not.
    /// </summary>
    private bool DisplayFields
    {
        get
        {
            return this.DataViewBOUltraWebTree.SelectedNode != null;
        }
    }

    /// <summary>
    /// Checks if every required element for valid lookup is acomplish.
    /// </summary>
    public bool IsValidLookup
    {
        get
        {
            bool isValid = true;
            this.ErrorMessage = string.Empty;
            if(this.DataViewBOUltraWebTree.CheckedNodes.Count == 0)
            {
                isValid = false;
                this.ErrorMessage += "No lookup field selected.<br>";
            }
            if(string.IsNullOrEmpty(this.LookupDisplayFieldRadioList.SelectedValue))
            {
                isValid = false;
                this.ErrorMessage += "No lookup display field selected.<br>";
            }
            if(string.IsNullOrEmpty(this.SortingDropDownList.SelectedValue))
            {
                isValid = false;
                this.ErrorMessage += "No sorting selected.<br>";
            }
            return isValid;
        }
    }

    /// <summary>
    /// Is it is not a valid lookup, this property holds all the associated errors..
    /// </summary>
    private string ErrorMessage
    {
        get
        {
            return ViewState["ErrorMessage"] != null ? (string) ViewState["ErrorMessage"] : string.Empty;
        }
        set
        {
            ViewState["ErrorMessage"] = value;
        }
    }
    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Button Handlers
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.ClearImageButton.ButtonClicked += new EventHandler<EventArgs>(ClearImageButton_ButtonClicked);
        this.OKImageButton.ButtonClicked += new EventHandler<EventArgs>(OKImageButton_ButtonClicked);
        this.DeleteImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        #endregion
        this.DataViewBOUltraWebTree.NodeChecked += new NodeCheckedEventHandler(DataViewBOUltraWebTree_NodeChecked);
        base.OnInit(e);
        this.LookupFieldTitleLabel.Visible = this.LookupDisplayFieldRadioList.Visible = false;
        this.LookupSortOrderTitleLabel.Visible = this.SortingDropDownList.Visible = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        StringBuilder jscriptText = new StringBuilder();
        string jscriptKey = "DataViewBOWebtree";
        jscriptText.Append(@"<script language='javascript' type='text/javascript'>");

        jscriptText.Append(@"function DataViewBOUltraWebTree_NodeCheckedClientSide(treeId, nodeId, bChecked) {");
        jscriptText.Append(@"var oldSelectedNodeId = document.getElementById('" + this.SelectedNode_DataViewBOHidden.ClientID + "').value;");
        jscriptText.Append(@"if (oldSelectedNodeId != nodeId) {");
        jscriptText.Append(@"var oldSelectedNode = igtree_getNodeById(oldSelectedNodeId);");
        jscriptText.Append(@"if ( typeof(oldSelectedNode) != 'undefined' && oldSelectedNode != null) ");
        jscriptText.Append(@"oldSelectedNode.setChecked(false); ");
        jscriptText.Append(@"document.getElementById('" + this.SelectedNode_DataViewBOHidden.ClientID + "').value = nodeId;");
        jscriptText.Append(@"}");
        jscriptText.Append(@"}");

        jscriptText.Append(@"function ClearCurrentSelections(){");
        jscriptText.Append(@"var currentDVNodeId = document.getElementById('" + this.SelectedNode_DataViewBOHidden.ClientID + "').value;");
        jscriptText.Append(@"var currentDVNode = igtree_getNodeById(currentDVNodeId);");
        jscriptText.Append(@"if ( typeof(currentDVNode) != 'undefined' && currentDVNode != null){ ");
        jscriptText.Append(@"currentDVNode.setChecked(false); ");
        jscriptText.Append(@"document.getElementById('" + this.SelectedNode_DataViewBOHidden.ClientID + "').value = ''; }");
        jscriptText.Append(@"}");

        jscriptText.Append(@"</script>");

        if(!this.Page.ClientScript.IsClientScriptBlockRegistered(jscriptKey))
            Page.ClientScript.RegisterStartupScript(this.GetType(), jscriptKey, jscriptText.ToString());
        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Event Handlers
    void OKImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int tableId = this.UnBind();
        if(this.IsValidLookup)
            this.GoToPreviousPage(tableId);
        else
            this.DisplayErrorMessage(this.ErrorMessage);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoToPreviousPage(this.MarkForRemovalAndUnBind());
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void ClearImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ClearSelectedLookUp();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoToPreviousPage(this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(this.SourceFieldId));
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DataViewBOUltraWebTree_NodeChecked(object sender, WebTreeNodeCheckedEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!(Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == "DoNothing"))
        {
            e.Node.Selected = true;
            this.SelectedFieldId = Convert.ToInt32(Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ID));
            if(this.DisplayFields)
                this.BindDisplayFields(Convert.ToInt32(Utilities.GetParamInDataKey(e.Node.Parent.DataKey.ToString(), Constants.DataKeysParam.ID)));
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Clear all selected valued for a lookup 
    /// </summary>
    /// <remarks>Except of course Source lookup fieldId</remarks>
    private void ClearSelectedLookUp()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.LookupFieldTitleLabel.Visible = this.LookupDisplayFieldRadioList.Visible = false;
        this.LookupSortOrderTitleLabel.Visible = this.SortingDropDownList.Visible = false;
        this.LookupDisplayFieldRadioList.Items.Clear();
        this.SortingDropDownList.ClearSelection();
        this.SetSelectedSorting(_defaultCriteria.ToString());
        this.SelectedNode_DataViewBOHidden.Value = String.Empty;
        this.DataViewBOUltraWebTree.SelectedNode = null;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Display possible fields to select according the selected field (table) in the web tree.
    /// </summary>
    /// <param name="tableId">Id of the table to display all of its fields</param>
    private void BindDisplayFields(int tableId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (tableId >= 0 && this.SelectedFieldId != tableId)
        { //We will need to rebind
            TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(tableId);
            if (table != null && this.DisplayFields)
                this.BindLookupDisplayFields(table.Fields);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Display all the fields in the RadioList
    /// </summary>
    /// <param name="fields">List of fields to display</param>
    private void BindLookupDisplayFields(FieldListBO fields)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Clean previous radio items in the list.
        this.LookupDisplayFieldRadioList.Items.Clear();
        this.LookupFieldTitleLabel.Visible = this.LookupDisplayFieldRadioList.Visible = true;
        this.LookupSortOrderTitleLabel.Visible = this.SortingDropDownList.Visible = true;
        foreach (FieldBO field in fields)
        {
            if (!field.FromMasterSchema && field.ID != this.SelectedFieldId)
            {
                string fieldText = string.IsNullOrEmpty(field.Alias) ? field.Name : field.Name + "(" + field.Alias + ")";
                this.LookupDisplayFieldRadioList.Items.Add(new ListItem(fieldText, field.ID.ToString()));
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Set lots of controls attributes as text, tooltip, etc.
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.LookupTitle.Text = Resources.Resource.LookupTitle_Label_Text;
        this.AddSortingItems();
        this.SetSelectedSorting(_defaultCriteria.ToString());
        this.LookupFieldTitleLabel.Text = Resources.Resource.DisplayFieldTitle_Label_Text;
        this.LookupSortOrderTitleLabel.Text = Resources.Resource.LookupSortOrder_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Bind the datasource to the control in the current UC.;
    /// </summary>
    /// <param name="dataViewBO">Datasource</param>
    /// <param name="sourceField">Source field to set up a lookup</param>
    public void DataBind(COEDataViewBO dataViewBO, FieldBO sourceField)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewBO = dataViewBO;
        this.SourceFieldId = sourceField.ID;
        this.SetControlsAttributes();
        if (dataViewBO != null)
        {
            if (sourceField.LookupFieldId < 0)
            {
                this.BindWebTree(dataViewBO.DataViewManager.Tables, int.MinValue);
                this.BindDisplayFields(int.MinValue);
            }
            else
            {
                this.BindWebTree(dataViewBO.DataViewManager.Tables, sourceField.LookupFieldId);
                this.BindDisplayFields(dataViewBO.DataViewManager.Tables.GetTableIdByFieldId(sourceField.LookupFieldId));
                this.SetLookUpValues(sourceField);
            }
        }
        this.DisplayParentLookupInfo(sourceField);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetLookUpValues(FieldBO field)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SelectedFieldId = field.LookupFieldId;
        this.SetSelectedSorting(field.LookupSortOrder.ToString());

        this.LookupDisplayFieldRadioList.ClearSelection();
        ListItem itemToSelect = this.LookupDisplayFieldRadioList.Items.FindByValue(field.LookupDisplayFieldId.ToString());
        if (itemToSelect != null)
            itemToSelect.Selected = true;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Return info selected by the user to the page that is making the call.
    /// </summary>
    /// <returns>The selected tableId (where the soruce fieldId is part of it)</returns>
    public int UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        if (this.IsValidLookup)
        {
            FieldBO sourceField = this.DataViewBO.DataViewManager.Tables.GetField(this.SourceFieldId);
            if (sourceField != null)
            {
                sourceField.LookupFieldId = this.SelectedFieldId;
                sourceField.LookupDisplayFieldId = Convert.ToInt32(this.LookupDisplayFieldRadioList.SelectedValue);
                sourceField.LookupSortOrder = (COEDataView.SortDirection)Enum.Parse(typeof(COEDataView.SortDirection), this.SortingDropDownList.SelectedValue);
                retVal = this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(this.SourceFieldId);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    /// <summary>
    /// Removes the selected lookup and returns the tableid as expected from unbinding
    /// </summary>
    /// <returns>table id of the bound table</returns>
    public int MarkForRemovalAndUnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        FieldBO sourceField = this.DataViewBO.DataViewManager.Tables.GetField(this.SourceFieldId);
        int retVal = int.MinValue;
        if (sourceField != null)
        {
            sourceField.LookupFieldId = int.MinValue;
            sourceField.LookupDisplayFieldId = int.MinValue;
            retVal = this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(sourceField.ID);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    /// <summary>
    /// Display a web tree with the possible values to choose a lookup field.
    /// </summary>
    /// <param name="tables">List of tables</param>
    /// <param name="selectedNodeId">In case it's a edit, show as selected the field according the given fieldId</param>
    private void BindWebTree(TableListBO tables, int selectedNodeId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewBOUltraWebTree.Nodes.Clear();
        List<Node> nodeList = this.CreateDataSource(tables, selectedNodeId);
        foreach (Node node in nodeList)
            this.DataViewBOUltraWebTree.Nodes.Add(node);

        this.SetDefaultExpandedNode();
        this.SetCheckedNode();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetDefaultExpandedNode()
    {
        this.DataViewBOUltraWebTree.CollapseAll();
        if(this.DataViewBOUltraWebTree.Nodes.Count > 0)
        {
            this.DataViewBOUltraWebTree.Nodes[0].Expanded = true;
            if(this.DataViewBOUltraWebTree.Nodes[0].Nodes.Count > 0)
                this.DataViewBOUltraWebTree.Nodes[0].Nodes[0].Expanded = true;
        }
    }

    private void SetCheckedNode()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Regex pattern = new Regex("_");
        if (this.DataViewBOUltraWebTree.CheckedNodes.Count > 0)
        {
            this.DataViewBOUltraWebTree.SelectedNode = (Node)this.DataViewBOUltraWebTree.CheckedNodes[0];//We assume just one node should be checked!
            this.SelectedNode_DataViewBOHidden.Value = pattern.Replace(this.DataViewBOUltraWebTree.ClientID, "") + ((Node)this.DataViewBOUltraWebTree.CheckedNodes[0]).GetIdString();
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplayParentLookupInfo(FieldBO field)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        TableBO table = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(field.ID);
        string schemaText = table.DataBase;
        string tableText = !string.IsNullOrEmpty(table.Alias) ? table.Alias : table.Name;
        string fieldText = !string.IsNullOrEmpty(field.Alias) ? field.Alias : field.Name;
        this.SelectedFieldNameLabel.Text = schemaText + "." + tableText + "." + fieldText;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private List<Node> CreateDataSource(TableListBO tables, int selectedNodeId)
    {
        List<Node> nodes = new List<Node>();
        List<string> databasesList = new List<string>();
        List<TableBO> filteredList;
        Node node = null;
        int currentTableID = this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(this.SourceFieldId);
        foreach (TableBO table in tables)
        {
            if(!databasesList.Contains(table.DataBase))
            {
                filteredList = tables.GetTablesByDBAndFromMasterSchemaExcludingTables(table.DataBase, false, currentTableID.ToString());
                if(filteredList.Count > 0)
                {
                    node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(filteredList, Constants.SortCriterias.Database_ASC, selectedNodeId));
                    if(Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
                    {
                        node.CheckBox = CheckBoxes.False;
                        node.Checked = false;
                    }
                    nodes.Add(node);
                    databasesList.Add(table.DataBase);
                }
            }
        }
        return nodes;
    }

    private void AddSortingItems()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.SortingDropDownList.Items.Count == 0) //Create dropdown items.
        {
            foreach (string item in Enum.GetNames(typeof(COEDataView.SortDirection)))
                this.SortingDropDownList.Items.Add(new ListItem(item, item));
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetSelectedSorting(string selectedItem)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.SortingDropDownList.Items.Count > 0)
        {
            if (string.IsNullOrEmpty(selectedItem))
                selectedItem = _defaultCriteria.ToString();

            //Coverity Fixes: CBOE-313
            if (this.SortingDropDownList.Items != null && this.SortingDropDownList.Items.Count > 0)
            {
                ListItem listItem = this.SortingDropDownList.Items.FindByValue(selectedItem);
                if (listItem != null)
                {
                    this.SortingDropDownList.ClearSelection();
                    listItem.Selected = true;
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void GoToPreviousPage(int selectedTable)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(selectedTable >= 0)
        {
            string queryString = "?" + Constants.ParamCaller + "=" + selectedTable;
            Server.Transfer("EditTableAndFields.aspx" + queryString, false);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplayErrorMessage(string message)
    {
        Forms_Public_UserControls_ErrorArea errorAreaUC = (Forms_Public_UserControls_ErrorArea) ((Forms_DataViewManager_ContentArea_DefineLookup) this.Page).Master.FindControlInPage("ErrorAreaUserControl");
        if(string.IsNullOrEmpty(message))
            errorAreaUC.Visible = false;
        else
        {
            errorAreaUC.Text = message;
            errorAreaUC.Visible = true;
        }
    }
    #endregion
}

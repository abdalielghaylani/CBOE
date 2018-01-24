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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Text;
using Csla;
using System.Text.RegularExpressions;

public partial class Relationships : System.Web.UI.UserControl
{
    #region Variables
    private enum ExistingRelControls
    {
        FromRelationshipLabel,
        ToRelationshipLabel,
        EditButton,
        SeparatorLabel,
        DeleteButton,
        JoinTypeLabel,
        SchemaLabel,
        FromTitleLabel,
        ToTitleLabel,
    }

    public bool ValidRelationships
    {
        get { return this.CanSaveRelationship(); }
    }

    private string _defaultJoinType = COEDataView.JoinTypes.INNER.ToString();
    #endregion

    #region Properties

    /// <summary>
    /// Current COEDataViewBO
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] == null ? null : (COEDataViewBO)Session[Constants.COEDataViewBO];
        }
    }

    private int Parent_Relationship
    {
        get
        {
            return ViewState[Constants.Parent] == null ? int.MinValue : (int)ViewState[Constants.Parent];
        }
        set
        {
            ViewState[Constants.Parent] = value;
        }
    }

    private int ParentKey_Relationship
    {
        get
        {
            return ViewState[Constants.ParentKey] == null ? int.MinValue : (int)ViewState[Constants.ParentKey];
        }
        set
        {
            ViewState[Constants.ParentKey] = value;
        }
    }

    private int PreviousParent_Relationship
    {
        get
        {
            return ViewState[Constants.PreviousParent] == null ? int.MinValue : (int) ViewState[Constants.PreviousParent];
        }
        set
        {
            ViewState[Constants.PreviousParent] = value;
        }
    }

    private int PreviousParentKey_Relationship
    {
        get
        {
            return ViewState[Constants.PreviousParentKey] == null ? int.MinValue : (int) ViewState[Constants.PreviousParentKey];
        }
        set
        {
            ViewState[Constants.PreviousParentKey] = value;
        }
    }

    private int ChildKey_Relationship
    {
        get
        {
            return ViewState[Constants.ChildKey] == null ? int.MinValue : (int)ViewState[Constants.ChildKey];
        }
        set
        {
            ViewState[Constants.ChildKey] = value;
        }
    }

    private int Child_Relationship
    {
        get
        {
            return ViewState[Constants.Child] == null ? int.MinValue : (int)ViewState[Constants.Child];
        }
        set
        {
            ViewState[Constants.Child] = value;
        }
    }

    private COEDataView.JoinTypes JoinType_Relationship
    {
        get
        {
            return ViewState[Constants.JoinType] == null ? (COEDataView.JoinTypes)Enum.Parse(typeof(COEDataView.JoinTypes), _defaultJoinType) : (COEDataView.JoinTypes)ViewState[Constants.JoinType];
        }
        set
        {
            ViewState[Constants.JoinType] = value;
        }
    }

    private COEDataView.JoinTypes PreviousJoinType_Relationship
    {
        get
        {
            return ViewState[Constants.PreviousJoinType] == null ? (COEDataView.JoinTypes) Enum.Parse(typeof(COEDataView.JoinTypes), _defaultJoinType) : (COEDataView.JoinTypes) ViewState[Constants.PreviousJoinType];
        }
        set
        {
            ViewState[Constants.PreviousJoinType] = value;
        }
    }
    
    private int CurrentEditingRel
    {
        get
        {
            return ViewState[Constants.CurrentEditingRel] == null ? int.MinValue : (int)ViewState[Constants.CurrentEditingRel];
        }
        set
        {
            ViewState[Constants.CurrentEditingRel] = value;
        }
    }

    private string Action
    {
        get
        {
            return ViewState[Constants.Action] != null ? ViewState[Constants.Action].ToString() : string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState[Constants.Action] = value;
        }
    }
    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
        else
            this.InvalidRelationshipsText.Visible = !this.ValidRelationships;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CurrentDVBOUltraWebTree.NodeChecked += new NodeCheckedEventHandler(CurrentDVBOUltraWebTree_NodeChecked);
        this.JoinTypesDropDown.SelectedIndexChanged += new EventHandler(JoinTypesDropDown_SelectedIndexChanged);
        #region Button Events
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.OKImageButton.ButtonClicked += new EventHandler<EventArgs>(OKImageButton_ButtonClicked);
        this.DeleteImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.DeleteRelationship();
        string url = "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + ((Forms_DataViewManager_ContentArea_DefineRelationships) this.Page).ParamCaller;
        Server.Transfer(url, false);
    }

    void OKImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.UnBind();
        if(!this.ValidRelationships)
        {
            return;
        }
        string url = ((Forms_DataViewManager_ContentArea_DefineRelationships)this.Page).Caller + ".aspx?" + Constants.ParamCaller + "=" + ((Forms_DataViewManager_ContentArea_DefineRelationships)this.Page).ParamCaller;
        Server.Transfer(url, false);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CancelCurrentEdition();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void JoinTypesDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(((DropDownList)sender).SelectedValue))
            this.DisplaySelectedJoin(((DropDownList)sender).SelectedValue);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CurrentDVBOUltraWebTree_NodeChecked(object sender, WebTreeNodeCheckedEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!(Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == "DoNothing"))
        {
            //Show the selected Node in the correct Label.
            foreach(Node checkedNode in CurrentDVBOUltraWebTree.CheckedNodes)
            {
                checkedNode.Checked = false;
            }
            e.Node.Checked = true;
            e.Node.Selected = true;
            this.DisplaySelectedNode(e.Node.Parent.DataKey.ToString(), e.Node.DataKey.ToString(), true);
            this.InvalidRelationshipsText.Visible = !this.ValidRelationships;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        StringBuilder jscriptText = new StringBuilder();
        string jscriptKey = "CurrentDVBOWebtree";
        jscriptText.Append(@"<script language='javascript' type='text/javascript'>");
        
        jscriptText.Append(@"function CurrentDVBOUltraWebTree_NodeChecked(treeId, nodeId, bChecked) {");
            jscriptText.Append(@"var oldSelectedNodeId = document.getElementById('" + this.SelectedNode_CurrentDVBOHidden.ClientID + "').value;");
            jscriptText.Append(@"if (oldSelectedNodeId != nodeId) {");
                jscriptText.Append(@"var oldSelectedNode = igtree_getNodeById(oldSelectedNodeId);");
                jscriptText.Append(@"if ( typeof(oldSelectedNode) != 'undefined' && oldSelectedNode != null) ");
                    jscriptText.Append(@"oldSelectedNode.setChecked(false); ");
                jscriptText.Append(@"document.getElementById('" + this.SelectedNode_CurrentDVBOHidden.ClientID + "').value = nodeId;");
            jscriptText.Append(@"}");
        jscriptText.Append(@"}");

        jscriptText.Append(@"function ClearCurrentSelections(){");
        jscriptText.Append(@"var currentDVNodeId = document.getElementById('" + this.SelectedNode_CurrentDVBOHidden.ClientID + "').value;");
        jscriptText.Append(@"var currentDVNode = igtree_getNodeById(currentDVNodeId);");
        jscriptText.Append(@"if ( typeof(currentDVNode) != 'undefined' && currentDVNode != null){ ");
            jscriptText.Append(@"currentDVNode.setChecked(false); ");
            jscriptText.Append(@"document.getElementById('" + this.SelectedNode_CurrentDVBOHidden.ClientID + "').value = ''; }");
        jscriptText.Append(@"}");
        jscriptText.Append(@"</script>");

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(jscriptKey))
            Page.ClientScript.RegisterStartupScript(this.GetType(), jscriptKey, jscriptText.ToString());

        base.OnPreRender(e);

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    private void ClearCurrentRelationShipSelection()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CurrentDVBOUltraWebTree.SelectedNode = null;
        this.SelectedFromFieldLabel.Text = String.Empty;
        this.SelectedFromTableLabel.Text = String.Empty;

        this.SelectionJoinTypeLabel.Text = this.JoinTypesDropDown.SelectedValue = _defaultJoinType;
        
        foreach(Node checkedNode in this.CurrentDVBOUltraWebTree.CheckedNodes)
        {
            checkedNode.Checked = false;
        }

        if(this.PreviousParentKey_Relationship != int.MinValue)
        {
            foreach(Node node in this.CurrentDVBOUltraWebTree.Nodes)
            {
                bool quit = false;
                if(Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
                {
                    foreach(Node tableNode in node.Nodes)
                    {
                        foreach(Node fieldNode in tableNode.Nodes)
                        {
                            if(Utilities.GetParamInDataKey(fieldNode.DataKey.ToString(), Constants.DataKeysParam.ID) == this.PreviousParentKey_Relationship.ToString())
                            {
                                fieldNode.Checked = true;
                                this.DisplaySelectedNode(fieldNode.Parent.DataKey.ToString(), fieldNode.DataKey.ToString(), true);
                                quit = true;
                            }
                        }
                        if(quit)
                            break;
                    }
                }
                else
                {
                    foreach(Node fieldNode in node.Nodes)
                    {
                        if(Utilities.GetParamInDataKey(fieldNode.DataKey.ToString(), Constants.DataKeysParam.ID) == this.PreviousParentKey_Relationship.ToString())
                        {
                            fieldNode.Checked = true;
                            this.DisplaySelectedNode(fieldNode.Parent.DataKey.ToString(), fieldNode.DataKey.ToString(), true);
                            quit = true;
                        }
                        if(quit)
                            break;
                    }
                }
                if(quit)
                {
                    break;
                }
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private bool CanSaveRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool retVal = false;
        if(this.ParentKey_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
        {
            FieldBO parentKey = this.DataViewBO.DataViewManager.Tables.GetField(this.ParentKey_Relationship);
            FieldBO childKey = this.DataViewBO.DataViewManager.Tables.GetField(this.ChildKey_Relationship);
            if(parentKey.LookupFieldId <= 0 && childKey.LookupFieldId <= 0)
            {
                if(parentKey.DataType == childKey.DataType)
                {
                    if(parentKey.ID != childKey.ID)
                    {
                        // check its not a circular relationship
                        RelationshipBO rel = this.DataViewBO.DataViewManager.Relationships.Get(childKey.ID, parentKey.ID);
                        if(this.CurrentEditingRel != int.MinValue || rel == null || rel.FromMasterSchema)
                            retVal = true;
                        else
                            this.InvalidRelationshipsTextLabel.Text = Resources.Resource.CircularRelationship_Label_Text;
                    }
                    else
                        this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationshipSameField_Label_Text;
                }
                else
                    this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationshipDifferentTypes_Label_Text;
            }
            else
                this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationshipIsLookup_Label_Text;
        }
        else
            this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationShips_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    private void DeleteRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(this.Parent_Relationship > int.MinValue && this.ParentKey_Relationship > int.MinValue && this.Child_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
        {
            this.DataViewBO.DataViewManager.Relationships.Get(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship, this.ChildKey_Relationship).FromMasterSchema = true;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void UpdateCurrentRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(CanSaveRelationship())
        {
            RelationshipBO currentRelationship = null;
            if(this.CurrentEditingRel != int.MinValue)
            {
                currentRelationship = this.DataViewBO.DataViewManager.Relationships.Get(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship, this.ChildKey_Relationship);
                if (currentRelationship != null)
                {
                    currentRelationship.Child = this.Child_Relationship;
                    currentRelationship.ChildKey = this.ChildKey_Relationship;
                    currentRelationship.Parent = this.Parent_Relationship;
                    currentRelationship.ParentKey = this.ParentKey_Relationship;
                    currentRelationship.JoinType = this.JoinType_Relationship;
                    currentRelationship.FromMasterSchema = false;
                }
                    //CSBR-140336
                else
                {
                    this.DataViewBO.DataViewManager.Relationships.Remove(PreviousParentKey_Relationship, ChildKey_Relationship);
                    currentRelationship = RelationshipBO.NewRelationship(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship, this.ChildKey_Relationship, this.JoinType_Relationship);
                    this.DataViewBO.DataViewManager.Relationships.Add(currentRelationship);
                }
            }
            else
            {
                currentRelationship = RelationshipBO.NewRelationship(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship,  this.ChildKey_Relationship, this.JoinType_Relationship);
                this.DataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void CancelCurrentEdition()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + ((Forms_DataViewManager_ContentArea_DefineRelationships) this.Page).ParamCaller;
        Server.Transfer(url, false);
        //this.ClearCurrentRelationShipSelection();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplaySelectedJoin(string selectedJoin)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            this.JoinType_Relationship = (COEDataView.JoinTypes)Enum.Parse(typeof(COEDataView.JoinTypes), selectedJoin);
            this.SelectionJoinTypeLabel.Text = selectedJoin;
        }
        catch
        {
            //TODO: Show an error message or select something by default.
        }
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplaySelectedNode(object tableDataKey, object fieldDataKey, bool IsLeftTree)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int tableID = -1;
        int fieldID = -1;
        if (tableDataKey != null || fieldDataKey != null)
        {
            if (tableDataKey is string && fieldDataKey is string)
            {
                if (!string.IsNullOrEmpty((string)tableDataKey) && !string.IsNullOrEmpty((string)fieldDataKey))
                {
                    string tempTableID = Utilities.GetParamInDataKey((string)tableDataKey, Constants.DataKeysParam.ID);
                    string tempFieldID = Utilities.GetParamInDataKey((string)fieldDataKey, Constants.DataKeysParam.ID);
                    int.TryParse(tempTableID, out tableID);
                    int.TryParse(tempFieldID, out fieldID);
                }
            }
            else if (tableDataKey is int && fieldDataKey is int)
            {
                tableID = (int)tableDataKey;
                fieldID = (int)fieldDataKey;
            }
            else if (tableDataKey == null && fieldDataKey is int)
            {
                fieldID = (int)fieldDataKey;
                tableID = this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(fieldID);
            }
            //TODO: Given a FieldId find the tableID.
            //Table stuff
            TableBO selectedTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableID);
            if (selectedTable != null)
            {
                if (IsLeftTree)
                {
                    this.SelectedFromTableLabel.Text = selectedTable.DataBase.ToUpper() + "." + Utilities.FormatTableText(selectedTable.Name, selectedTable.Alias);
                    this.Parent_Relationship = selectedTable.ID;
                }
                else
                {
                    this.SelectedToTableLabel.Text = selectedTable.DataBase.ToUpper() + "." + Utilities.FormatTableText(selectedTable.Name, selectedTable.Alias);
                    this.Child_Relationship = selectedTable.ID;
                }
            }
            //Field stuff

            FieldBO selectedField = this.DataViewBO.DataViewManager.Tables.GetField(fieldID);
            if (selectedField != null)
            {
                if (IsLeftTree)
                {
                    this.SelectedFromFieldLabel.Text = Utilities.FormatFieldText(selectedField.Name, selectedTable.Alias);
                    this.ParentKey_Relationship = fieldID; ;
                }
                else
                {
                    this.SelectedToFieldLabel.Text = Utilities.FormatFieldText(selectedField.Name, selectedTable.Alias);
                    this.ChildKey_Relationship = fieldID;
                }
            }
        }
        else
        {
            this.SelectedFromFieldLabel.Text = String.Empty;
            this.SelectedFromTableLabel.Text = String.Empty;
        }
       Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplaySelectedJoinType(string joinType)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!string.IsNullOrEmpty(joinType))
            this.SelectionJoinTypeLabel.Text = joinType;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public void DataBind(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.BindCurrentDVBOTree(dataViewBO.DataViewManager.Tables, -1);
        this.AddJoinTypeItems(_defaultJoinType);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public void DataBind(COEDataViewBO dataViewBO, int childKey, string action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Action = action;
        //Filter the Selected childKey
        List<RelationshipBO> datasource = dataViewBO.DataViewManager.Relationships.GetByChildKey(childKey);
        bool relationshipSelected = datasource.Count > 0 && !datasource[0].FromMasterSchema;
        if (childKey > 0)
        {
            this.ChildKey_Relationship = childKey;
            //For now, we just support one relationship to edit
            if (datasource.Count > 0)
            {
                this.CurrentEditingRel = 0;
                this.PreviousParentKey_Relationship = this.ParentKey_Relationship = datasource[0].ParentKey;
                this.PreviousParent_Relationship = datasource[0].Parent;
                this.PreviousJoinType_Relationship = datasource[0].JoinType;
            }
        }

        if (relationshipSelected)
        {
            this.BindCurrentDVBOTree(dataViewBO.DataViewManager.Tables, this.ParentKey_Relationship);
            this.AddJoinTypeItems(datasource[0].JoinType.ToString());
            this.DisplaySelectedNode(null, this.ChildKey_Relationship, false);
            this.DisplaySelectedNode(null, this.ParentKey_Relationship, true);
        }
        else
        {
            this.BindCurrentDVBOTree(dataViewBO.DataViewManager.Tables, -1);
            this.AddJoinTypeItems(_defaultJoinType);
            this.DisplaySelectedNode(null, this.ChildKey_Relationship, false);
            this.DisplaySelectedNode(null, null, true);
        }
        
        Regex pattern = new Regex("_");
        if (this.CurrentDVBOUltraWebTree.CheckedNodes.Count > 0)
            this.SelectedNode_CurrentDVBOHidden.Value = pattern.Replace(this.CurrentDVBOUltraWebTree.ClientID, "") + ((Node)this.CurrentDVBOUltraWebTree.CheckedNodes[0]).GetIdString();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public COEDataViewBO UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!string.IsNullOrEmpty(this.Action))
            this.UpdateCurrentRelationship();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return this.DataViewBO; //This obj already has all the changes.
    }

    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CurrentDVBOLabel.Text = Resources.Resource.CurrentDataView_Label_Text;
        this.JoinLabel.Text = Resources.Resource.JoinType_Label_Text;
        this.DeleteImageButton.ButtonText = this.DeleteImageButton.ImageToolTip = Resources.Resource.Delete_Button_Text;
        this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationShips_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void BindCurrentDVBOTree(TableListBO tables, int selectedNodeId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        this.CurrentDVBOUltraWebTree.Nodes.Clear();
        List<Node> nodeList = this.CreateDataSource(tables, selectedNodeId);
        foreach (Node node in nodeList)
            this.CurrentDVBOUltraWebTree.Nodes.Add(node);

        this.SetDefaultExpandedNode();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    
    private void SetDefaultExpandedNode()
    {
        this.CurrentDVBOUltraWebTree.CollapseAll();
        
        foreach(Node dbNode in this.CurrentDVBOUltraWebTree.Nodes)
        {
            string basetableDatabase = this.DataViewBO.DataViewManager.Tables.GetTable(this.DataViewBO.DataViewManager.BaseTableId) == null ? string.Empty : this.DataViewBO.DataViewManager.Tables.GetTable(this.DataViewBO.DataViewManager.BaseTableId).DataBase;
            if(basetableDatabase == dbNode.Text)
            {
                dbNode.Expanded = true;
                foreach(Node node in dbNode.Nodes)
                {
                    if(this.DataViewBO.DataViewManager.BaseTableId.ToString() == Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.ID))
                    {
                        node.Expanded = true;
                        break;
                    }
                }
                break;
            }
        }
    }

    private List<Node> CreateDataSource(TableListBO tables, int selectedNodeId)
    {
        List<Node> nodes = new List<Node>();
        List<string> databasesList = new List<string>();
        List<TableBO> filteredList;
        string tempDBName = String.Empty;
        int currentTable = this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(this.ChildKey_Relationship);
        foreach (TableBO table in tables)
        {
            if (!databasesList.Contains(table.DataBase))
            {
                filteredList = tables.GetTablesByDBAndFromMasterSchemaExcludingTables(table.DataBase, false, currentTable.ToString());
                if(filteredList.Count > 0)
                {
                    Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(filteredList, Constants.SortCriterias.Database_ASC, selectedNodeId, true));
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

    /// <summary>
    /// Adds all the items in the JoinType enum to the drop down.
    /// </summary>
    /// <param name="selectedItem">Selected item</param>
    private void AddJoinTypeItems(string selectedItem)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.JoinTypesDropDown.Items.Count == 0) //Create dropdown items.
        {
            foreach (string item in Enum.GetNames(typeof(COEDataView.JoinTypes)))
            {
                ListItem currentItem = new ListItem(item, item);
                if (item.ToUpper() == selectedItem.ToUpper())
                {
                    currentItem.Selected = true;
                    this.SelectionJoinTypeLabel.Text = selectedItem.ToUpper();
                }
                this.JoinTypesDropDown.Items.Add(currentItem);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(selectedItem) && (string.IsNullOrEmpty(this.JoinTypesDropDown.SelectedValue) || this.JoinTypesDropDown.SelectedValue != selectedItem))
            {
                if (this.JoinTypesDropDown.Items.FindByValue(selectedItem) != null)
                {
                    this.JoinTypesDropDown.SelectedIndex = this.JoinTypesDropDown.Items.IndexOf(this.JoinTypesDropDown.Items.FindByValue(selectedItem));
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

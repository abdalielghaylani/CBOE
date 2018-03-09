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
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;


public partial class Forms_DataViewManager_ContentArea_DefineRelationships : GUIShellPage, ICallbackEventHandler
{
    #region Variables
    private string _defaultJoinType = COEDataView.JoinTypes.INNER.ToString();
    private string strError = string.Empty;
    #endregion

    #region Properties
    /// <summary>
    /// Page that is calling this page. Useful to remember where I have to go back, or go next.(Because this last, we cannot rely on page.previous)
    /// </summary>
    public string Caller
    {
        get
        {
            return ViewState[Constants.Caller] != null ? ViewState[Constants.Caller].ToString() : String.Empty;
        }
        set
        {
            ViewState[Constants.Caller] = value;
        }
    }

    /// <summary>
    /// Parameter to pass to the called page
    /// </summary>
    /// <remarks>Take a look of Caller details</remarks>
    public string ParamCaller
    {
        get
        {
            return ViewState[Constants.ParamCaller] != null ? ViewState[Constants.ParamCaller].ToString() : String.Empty;
        }
        set
        {
            ViewState[Constants.ParamCaller] = value;
        }
    }    

    public bool ValidRelationships
    {
        get
        {
            strError = this.CanSaveRelationship();
            return string.IsNullOrEmpty(strError);
        }
    }

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
            return int.Parse(this.SelectedTableIDHidden.Value);
        }
        set
        {
            this.SelectedTableIDHidden.Value = value.ToString();
        }
    }

    private int ParentKey_Relationship
    {
        get
        {
            return int.Parse(this.SelectedFieldIDHidden.Value);
        }
        set
        {
            this.SelectedFieldIDHidden.Value = value.ToString();
        }
    }

    private int PreviousParent_Relationship
    {
        get
        {
            return ViewState[Constants.PreviousParent] == null ? int.MinValue : (int)ViewState[Constants.PreviousParent];
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
            return ViewState[Constants.PreviousParentKey] == null ? int.MinValue : (int)ViewState[Constants.PreviousParentKey];
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
            return ViewState[Constants.PreviousJoinType] == null ? (COEDataView.JoinTypes)Enum.Parse(typeof(COEDataView.JoinTypes), _defaultJoinType) : (COEDataView.JoinTypes)ViewState[Constants.PreviousJoinType];
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

    private string DefaultDatabase
    {
        get
        {
            return ViewState["DefaultDatabase"] == null ? string.Empty : ViewState["DefaultDatabase"].ToString();
        }
        set
        {
            ViewState["DefaultDatabase"] = value;
        }
    }
    #endregion

    #region Page Life Cycle
    protected override void OnInit(EventArgs e)
    {
        #region Button Events
        this.TopCancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.TopOKImageButton.ButtonClicked += new EventHandler<EventArgs>(OKImageButton_ButtonClicked);
        this.TopDeleteImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        this.BottomCancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.BottomOKImageButton.ButtonClicked += new EventHandler<EventArgs>(OKImageButton_ButtonClicked);
        this.BottomDeleteImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.Caller = Request[Constants.Caller];
            this.ParamCaller = Request[Constants.ParamCaller];
            this.ChildKey_Relationship = int.Parse(Request[Constants.ChildKey]);
            this.Child_Relationship = int.Parse(this.ParamCaller);
            TableBO tbl = DataViewBO.DataViewManager.Tables.GetTable(this.Child_Relationship);
            //CBOE-801
            string db = GetTableSchemaName(tbl.DataBase);
            this.DefaultDatabase = string.IsNullOrEmpty(db) ? tbl.DataBase:db;
            //end CBOE-801
            FieldBO fld = tbl.Fields.GetField(this.ChildKey_Relationship);
            string joinType = _defaultJoinType;
            if (!string.IsNullOrEmpty(Request[Constants.ParentKey]))
            {
                RelationshipBO rel = this.DataViewBO.DataViewManager.Relationships.Get(int.Parse(Request[Constants.ParentKey]), this.ChildKey_Relationship);
                this.PreviousParentKey_Relationship = rel.ParentKey;
                this.PreviousParent_Relationship = rel.Parent;
                this.PreviousJoinType_Relationship = rel.JoinType;

                this.SelectedFieldIDHidden.Value = rel.ParentKey.ToString();
                this.SelectedTableIDHidden.Value = rel.Parent.ToString();

                TableBO parentTbl = DataViewBO.DataViewManager.Tables.GetTable(this.Parent_Relationship);
                this.DefaultDatabase = parentTbl.DataBase;
                FieldBO parentFld = parentTbl.Fields.GetField(this.ParentKey_Relationship);

                this.SelectedFromFieldLabel.Text = parentTbl.DataBase + "." + parentTbl.Alias + "." + parentFld.Alias;
                joinType = rel.JoinType.ToString();

                this.CurrentEditingRel = 1;
            }
            this.SelectedToFieldLabel.Text = tbl.DataBase + "." + tbl.Alias + "." + fld.Alias;
            this.SetControlsAttributtes();
            this.SelectParentRelationshipLabel.Text = string.Format(this.SelectParentRelationshipLabel.Text, this.SelectedToFieldLabel.Text, fld.DataType);
            this.AddJoinTypeItems(joinType);

            this.SchemaDropDownList.DataSource = GetSchemas();
            this.SchemaDropDownList.SelectedValue = this.DefaultDatabase;
            this.SchemaDropDownList.DataBind();
        }
    }
    #endregion

    #region Event Handlers
    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.DeleteRelationship();
        string url = "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + this.ParamCaller;
        Server.Transfer(url, false);
    }

    void OKImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.UnBind();
        if (!this.ValidRelationships)
        {
            this.InvalidRelationshipsText.Style.Add("display", "block");
            this.InvalidRelationshipsTextLabel.Text = strError;
            strError = string.Empty;
            return;
        }
        string url = this.Caller + ".aspx?" + Constants.ParamCaller + "=" + this.ParamCaller;
        Server.Transfer(url, false);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CancelCurrentEdition();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }    

    #endregion

    #region Methods
    public COEDataViewBO UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.JoinType_Relationship = (COEDataView.JoinTypes)Enum.Parse(typeof(COEDataView.JoinTypes), this.JoinTypesDropDown.SelectedValue);
        this.UpdateCurrentRelationship();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return this.DataViewBO; //This obj already has all the changes.
    }

    //CBOE-1185
    /// <summary>
    /// Check circurlar dependency is exists in relationship in dataview
    /// </summary>
    /// <returns>Return boolean</returns>
    private bool CheckCircularDependency()
    {
        bool isCircularDependancy = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,MethodBase.GetCurrentMethod().Name);
        try
        {
            if (this.ParentKey_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
            {
                if (this.DataViewBO != null && this.DataViewBO.DataViewManager != null && this.DataViewBO.DataViewManager.Tables != null && this.DataViewBO.DataViewManager.Tables.Count > 0)
                {
                    COEDataViewManagerBO coeBO = this.DataViewBO.DataViewManager;
                    
                    int parent_TableID = coeBO.Tables.GetTableByFieldId(this.ParentKey_Relationship).ID;
                    int grandChild_TableID = coeBO.Tables.GetTableByFieldId(this.ChildKey_Relationship).ID;
                    
                    if (coeBO.Relationships != null && coeBO.Relationships.Count > 0)
                    {
                        List<ParentChild> ParentChildList = new List<ParentChild>();
                        foreach (RelationshipBO item in coeBO.Relationships)
                        {
                            ParentChildList.Add(new ParentChild(item.Parent, item.Child));
                        }

                        ParentChild theParentChild = null;
                        List<ParentChild> tempChild = ParentChildList.FindAll(ff => ff.Child == grandChild_TableID);
                        if (tempChild != null && tempChild.Count > 0)
                        {
                            theParentChild = TreeSearchChild(grandChild_TableID, parent_TableID, ParentChildList);
                        }
                        else
                        {
                            theParentChild = TreeSearchParent(grandChild_TableID, parent_TableID, ParentChildList);
                        }
                        if (theParentChild != null)
                            isCircularDependancy = true;
                    }
                }
            }
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,MethodBase.GetCurrentMethod().Name);
        }       
      return isCircularDependancy;
    }

    //CBOE-1185
    /// <summary>
    /// Searches the tree from Child to Parent
    /// </summary>
    /// <param name="source">Child table id</param>
    /// <param name="target">Parent table id</param>
    /// <param name="ParentChildList">List of relationship</param>
    /// <returns></returns>
    private ParentChild TreeSearchChild(int source, int target, List<ParentChild> ParentChildList)
    {
        ParentChild parentChildTemp = null;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            if (ParentChildList != null && ParentChildList.Count > 0)
            {
                List<ParentChild> tempChild = ParentChildList.FindAll(ff => ff.Child == source);

                if (tempChild != null && tempChild.Count > 0)
                {
                    foreach (ParentChild item in tempChild)
                    {
                        if (item.Parent == target && item.Child == source)
                            continue;      // To check already existing relationship
                        else
                            if (item.Parent == target)
                            {
                                parentChildTemp = item;
                                break;
                            }
                            else
                                return TreeSearchChild(item.Parent, target, ParentChildList);
                    }
                }
            }
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        return parentChildTemp;
    }

    //CBOE-1185
    /// <summary>
    /// Searches the tree from Parent to Child
    /// </summary>
    /// <param name="source">Child table id</param>
    /// <param name="target">Parent table id</param>
    /// <param name="ParentChildList">List of relationship</param>
    /// <returns></returns>
    private ParentChild TreeSearchParent(int source, int target, List<ParentChild> ParentChildList)
    {
        ParentChild parentChildTemp = null;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            if (ParentChildList != null && ParentChildList.Count > 0)
            {
                List<ParentChild> tempParent = ParentChildList.FindAll(ff => ff.Parent == source);

                if (tempParent != null && tempParent.Count > 0)
                {
                    foreach (ParentChild item in tempParent)
                    {
                        if (item.Parent == target && item.Child == source)
                            continue;      // To check already existing relationship
                        else
                            if (item.Child == target)
                            {
                                parentChildTemp = item;
                                break;
                            }
                            else
                                return TreeSearchParent(item.Child, target, ParentChildList);
                    }
                }
            }
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        return parentChildTemp;
    }

    //CBOE-1674
    /// <summary>
    /// Method checks child has multiple parents or not
    /// Return TRUE if multiple parents found.
    /// </summary>
    /// <returns>Returns the table name of existing parent</returns>
    private string IsChildHaveMultipleParents()
    {       
        string strError = string.Empty;
        try
        {
            if (this.ParentKey_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
            {
                int parent_TableID = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(this.ParentKey_Relationship).ID;
                int child_TableID = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(this.ChildKey_Relationship).ID;
                List<RelationshipBO> lstRelationshipBO = this.DataViewBO.DataViewManager.Relationships.GetByChildId(child_TableID);
                if (lstRelationshipBO != null && lstRelationshipBO.Count > 0)
                {
                    List<int> lstParent = new List<int>();
                    // For disctinct parent list
                    foreach (RelationshipBO item in lstRelationshipBO)
                    {
                        if (!lstParent.Contains(item.Parent))
                            lstParent.Add(item.Parent);
                    }
                    int tableID = lstParent.Find(i => i == parent_TableID);
                    if (tableID <= 0)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        foreach (int item in lstParent)
                        {
                            if (item != parent_TableID)
                            {
                                TableBO tableBO = this.DataViewBO.DataViewManager.Tables.GetTable(item);
                                if (tableBO != null)
                                    sb.Append(tableBO.Name + ",");
                            }
                        }
                        if (sb.Length > 0)
                            strError = sb.ToString().Remove(sb.ToString().Length - 1);
                    }
                }
            }
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        return strError;
    }

    private string CanSaveRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            if (this.ParentKey_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
            {
                FieldBO parentKey = this.DataViewBO.DataViewManager.Tables.GetField(this.ParentKey_Relationship);
                FieldBO childKey = this.DataViewBO.DataViewManager.Tables.GetField(this.ChildKey_Relationship);
                if (parentKey != null && childKey != null) // CSBR-153472. User is not allowed to Submit if parent field is not selected.
                {
                    if (parentKey.LookupFieldId <= 0 && childKey.LookupFieldId <= 0)
                    {
                        if (parentKey.DataType == childKey.DataType)
                        {
                            string  strError =IsChildHaveMultipleParents();
                            if (string.IsNullOrEmpty(strError))
                            {
                                if (parentKey.ID != childKey.ID)
                                {
                                    // check its not a circular relationship
                                    RelationshipBO rel = this.DataViewBO.DataViewManager.Relationships.Get(childKey.ID, parentKey.ID);
                                    if (rel == null)
                                        return string.Empty;
                                    else
                                        return Resources.Resource.CircularRelationship_Label_Text;
                                }
                                else
                                    return Resources.Resource.InvalidRelationshipSameField_Label_Text;
                            }
                            else
                                return Resources.Resource.InvalidChildTableMultipleParents + strError;  //CBOE-1674
                        }
                        else
                            return Resources.Resource.InvalidRelationshipDifferentTypes_Label_Text;
                    }
                    else
                        return Resources.Resource.InvalidRelationshipIsLookup_Label_Text;
                }
                else
                    return Resources.Resource.SelectParentTable_Label_Text; // CSBR-153472.
            }
            else
                return Resources.Resource.InvalidRelationShips_Label_Text;
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
    }

    private void DeleteRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.Parent_Relationship > int.MinValue && this.ParentKey_Relationship > int.MinValue && this.Child_Relationship > int.MinValue && this.ChildKey_Relationship > int.MinValue)
        {
            this.DataViewBO.DataViewManager.Relationships.Remove(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship, this.ChildKey_Relationship);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void UpdateCurrentRelationship()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.ValidRelationships)
        {
            RelationshipBO currentRelationship = null;
            if (this.CurrentEditingRel != int.MinValue)
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
                currentRelationship = RelationshipBO.NewRelationship(this.Parent_Relationship, this.ParentKey_Relationship, this.Child_Relationship, this.ChildKey_Relationship, this.JoinType_Relationship);
                this.DataViewBO.DataViewManager.Relationships.Add(currentRelationship);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void CancelCurrentEdition()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + this.ParamCaller;
        Server.Transfer(url, false);
        //this.ClearCurrentRelationShipSelection();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.DefineRelationships_Page_Title);
        this.SelectParentRelationshipLabel.Text = Resources.Resource.SelectParentColumnFor_Label_Text;
        this.TopDeleteImageButton.ButtonText = this.TopDeleteImageButton.ImageToolTip = this.BottomDeleteImageButton.ButtonText = this.BottomDeleteImageButton.ImageToolTip = Resources.Resource.Delete_Button_Text;
        this.InvalidRelationshipsTextLabel.Text = Resources.Resource.InvalidRelationShips_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public string GetTablesDataSource(string schema)
    {
        int basetableid = this.DataViewBO.DataViewManager.BaseTableId;

        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            if (string.IsNullOrEmpty(schema))
            {
                schema = this.DefaultDatabase;
            }

            string result = "YAHOO.RelationshipsNS.LeftPanel.DataSource.liveData = [";
            foreach (TableBO table in this.DataViewBO.DataViewManager.Tables)
            {
                if (table.DataBase == schema && table.ID != Convert.ToInt32(this.ParamCaller)) //CSBR- 153486 Same base table is displayed as an option to select parent table
                    result += "{" + string.Format("tableschema: \"{0}\", tablealias: \"{1}\", tableid: \"{2}\", tablename: \"{3}\", isbasetable: \"{4}\"", table.DataBase, System.Web.HttpUtility.HtmlEncode(table.Alias), table.ID, table.Name, (table.ID == basetableid)) + "},";
            }
            if (result.Length > 0 && !result.EndsWith("["))
                result = result.Remove(result.Length - 1);
            result += @"];
                ";
            this.SchemaDropDownList.SelectedValue = schema;
            this.SchemaDropDownList.DataBind();
            return result;
        }
        return string.Empty;
    }

    public string GetFieldsDataSource(int tableId)
    {
        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            TableBO currentTable = null;
            if (tableId > 0)
                currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableId);
            else if (this.Parent_Relationship > 0)
                currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(this.Parent_Relationship);

            if (currentTable != null)
            {
                string result = "YAHOO.RelationshipsNS.RightPanel.DataSource.liveData = [";

                if (currentTable != null)
                {
                    foreach (FieldBO fld in currentTable.Fields)
                    {
                        if (fld.LookupDisplayFieldId < 0) //Dont want to list a lookup as a parent field for a relationship
                            result += "{" + string.Format("fieldalias: \"{0}\", fieldid: \"{1}\", fieldtype: \"{2}\"", fld.Alias, fld.ID, fld.DataType.ToString()) + "},";
                    }
                }

                if (result.Length > 0 && !result.EndsWith("["))
                    result = result.Remove(result.Length - 1);
                result += @"];
            ";
                return result;
            }
        }

        return string.Empty;
    }

    private List<string> GetSchemas()
    {
        List<string> schemas = new List<string>();
        if (this.DataViewBO.DataViewManager.Tables.Count > 0)
        {
            foreach (TableBO table in this.DataViewBO.DataViewManager.Tables)
            {
                if (!schemas.Contains(table.DataBase))
                    schemas.Add(table.DataBase);
            }
        }
        return schemas;
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
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
        
    /// <summary>
    /// Method returns the base table schema name.
    /// </summary>
    /// <param name="childSchema">Child schema name</param>
    /// <returns>Base Table Schema</returns>
    private string GetTableSchemaName(string childSchema)
    {
        // CBOE-801
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string selectSchema = string.Empty;
        try
        {
            COEDataViewBO theCOEDataViewBO = DataViewBO;
            if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables.Count > 0)
            {
                foreach (TableBO item in theCOEDataViewBO.DataViewManager.Tables)
                {
                    if (theCOEDataViewBO.DataViewManager.BaseTableId == item.ID)
                    {
                        selectSchema = item.DataBase; break;
                    }
                }
            }
        }
        catch
        {
            throw;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return selectSchema;
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
            result = "FilterSchema: " + this.GetTablesDataSource(database);            
        }
        else if (eventArgument.StartsWith("SelectTable: "))
        {
            int tableId = int.Parse(eventArgument.Replace("SelectTable: ", string.Empty));
            result = "SelectTable: " + GetFieldsDataSource(tableId);
        }
        else if (eventArgument.StartsWith("SelectField: "))
        {
            int fieldId = int.Parse(eventArgument.Replace("SelectField: ", string.Empty));
            this.SelectedFieldIDHidden.Value = fieldId.ToString();           
            string errors = this.CanSaveRelationship();
            if (CheckCircularDependency())
                result = "Message: " + "CIRCULARDEPENDENCY";
            else if (string.IsNullOrEmpty(errors))
                result = "SelectField: " + this.ValidRelationships;
            else
                result = "Message: " + errors;
        }
        
    }

    #endregion
}

/// <summary>
/// Contains only collection of relationship
/// Created to resolve circular relationship 
/// CBOE-1185
/// </summary>
class ParentChild
{
    public int Parent
    {
        get;
        set;
    }
    public int Child
    {
        get;
        set;
    }
    public ParentChild(int parent, int child)
    {
        this.Parent = parent;
        this.Child = child;
    }
}

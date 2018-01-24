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
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using System.Collections.Generic;

public partial class TableSummary : System.Web.UI.UserControl
{
    #region Public Events
    public event CommandEventHandler TableDeleted;
    public event CommandEventHandler TableDuplicated;
    public event CommandEventHandler EditTable;
    #endregion

    #region Properties
    /// <summary>
    /// Current COEDataViewBO
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
        }
    }

    /// <summary>
    /// Current selected Node.
    /// </summary>
    public int SelectedTableId
    {
        get
        {
            
            int retVal;
            if (int.TryParse(this.SelectedTableIDHidden.Value, out retVal))
                return retVal;
            else
                return -1;
        }
        set
        {
            this.SelectedTableIDHidden.Value = value.ToString();
        }
    }

    public string SelectedTableIDHiddenClientID
    {
        get { return SelectedTableIDHidden.ClientID; }
    }

    public string AliasTextBoxClientID
    {
        get { return this.AliasTextBox.ClientID; }
    }

    public string NameTextBoxClientID
    {
        get { return this.NameTextBox.ClientID; }
    }
    #endregion

    #region Page Life Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
			//check if SelectedTableId > zero. This will be the case if the session ends while editing dataview.
			//Here if we found SelectedTableID then we can open the table of that id in new window by calling
			//EditTableImageButton_ButtonClicked method. This is similar to clicking on edit table link.
			if (this.SelectedTableId > 0)
            {
                EditTableImageButton_ButtonClicked(null, null);
        }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.DuplicateTableImageButton.ButtonClicked += new EventHandler<EventArgs>(DuplicateTableImageButton_ButtonClicked);
        this.EditTableImageButton.ButtonClicked += new EventHandler<EventArgs>(EditTableImageButton_ButtonClicked);
        this.EditTagsImageButton.ButtonClicked += new EventHandler<EventArgs>(EditTagsImageButton_ButtonClicked);
    }

    #endregion

    #region Event Handlers
    void RemoveTableImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (TableDeleted != null)
        {
            TableDeleted(this, new CommandEventArgs("TableDeleted", this.SelectedTableId));
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DuplicateTableImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (TableDuplicated != null)
        {
            TableDuplicated(this, new CommandEventArgs("TableAliasCreated", this.SelectedTableId));
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void EditTableImageButton_ButtonClicked(object sender, EventArgs e)
    {
        if (EditTable != null)
            EditTable(this, new CommandEventArgs("EditTable", this.SelectedTableId));

        //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
		//DataviewBO can be null if seccion ends in between
        if (this.DataViewBO == null)
            return;

        COEDataViewBO dv = this.DataViewBO;
        //Session["FreshDV"] = dv.Clone();
        Session["FreshTable"] = dv.DataViewManager.Tables.GetTable(this.SelectedTableId).Clone();
        Session["FreshRels"] = dv.DataViewManager.Relationships.GetByChildId(this.SelectedTableId);
        Session["DummyFreshTable"] = (Session["FreshTable"] as TableBO).Clone();
        Session["DummyFreshRels"] = Session["FreshRels"];
        //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
		//Added action, dataview id, selected table id i.e. paramcaller to return string
		//this will help to take proper action if session ends in between.
		String RedirectURL = "~/Forms/DataViewManager/ContentArea/EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + this.SelectedTableId;

        // Edit Existing
        if (this.DataViewBO.COEDataView.DataViewID > 0)
            RedirectURL += "&" + Constants.Action + "=" + Constants.PageStates.Edit_DV + "&"
                + Constants.SelectedDataViewID + "=" + this.DataViewBO.COEDataView.DataViewID;
        else if (Request["IsMaster"] != null && bool.Parse(Request["IsMaster"]))  //Edit Master
            RedirectURL += "&IsMaster=true";
        else
        {
            //Cretae From Existing
            RedirectURL += "&" + Constants.Action + "=" + Constants.PageStates.Clone_DV;
            // Cretae From Master
            if (this.Page.ClientQueryString.Contains(Constants.SelectedDataViewID) &&
                Page.Request.QueryString.Get(Constants.SelectedDataViewID).ToString() == "0")
                RedirectURL += "&" + Constants.SelectedDataViewID + "=" + Page.Request.QueryString.Get(Constants.SelectedDataViewID);
        }
        Response.Redirect(RedirectURL, true);
    }

    void EditTagsImageButton_ButtonClicked(object sender, EventArgs e)
    {
        COEDataViewBO dv = this.DataViewBO;
        Response.Redirect("~/Forms/DataViewManager/ContentArea/EditTags.aspx?" + Constants.ParamCaller + "=" + this.SelectedTableId, true);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set all the controls values.
    /// </summary>
    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NameTitleLabel.Text = Resources.Resource.Name_Label_Text;
        this.AliasTitleLabel.Text = Resources.Resource.Alias_Label_Text;
        this.TableDetailTitle.Text = Resources.Resource.TableSummary_Label_Text;
        this.PrimaryKeysLabel.Text = Resources.Resource.PrimaryKey_Label_Text;
        this.LookupsLabel.Text = Resources.Resource.Lookups_Label_Text;
        this.RelationshipsLabel.Text = Resources.Resource.Relationships_Label_Text;
        this.TagsLabel.Text = Resources.Resource.Tags_Label_Text;
        this.IndexLabel.Text = Resources.Resource.Index_Label_Text;
        this.EditTableImageButton.ButtonText = this.EditTableImageButton.ImageToolTip = Resources.Resource.EditTable_Button_Text;
        this.EditTagsImageButton.ButtonText = this.EditTagsImageButton.ImageToolTip = Resources.Resource.EditTags_Button_Text;
        this.DuplicateTableImageButton.ButtonText = this.DuplicateTableImageButton.ImageToolTip = Resources.Resource.DuplicateTable_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Binds the details table with the given datasource.
    /// </summary>
    /// <param name="dataKey">Datakey of the selected table to display</param>
    public void BindTableDetails(int tableId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SelectedTableId = tableId;

        TableBO selectedTable = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
        if (selectedTable != null)
        {
            if (!string.IsNullOrEmpty(selectedTable.Alias))
                this.AliasTextBox.Text = selectedTable.Alias;
            this.NameTextBox.Text = selectedTable.Name;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public string GetPkDataSource(int tableid)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (tableid > 0 || this.SelectedTableId > 0)
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                if (tableid < 0)
                    tableid = this.SelectedTableId;
                string result = "YAHOO.DataviewBoardNS.TableSummary.PkDataSource.liveData = [";

                TableBO currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableid);

                if (currentTable != null)
                {
                    FieldBO pkField = currentTable.Fields.GetField(currentTable.PrimaryKey);
                    if (pkField != null)
                    {
                        result += "{" + string.Format("fieldid: \"{0}\", fieldalias: \"{1}\", fieldtype: \"{2}\"", pkField.ID, pkField.Alias, pkField.DataType.ToString()) + "},";
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

    public string GetRelDataSource(int tableid)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (tableid > 0 || this.SelectedTableId > 0)
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                if (tableid < 0)
                    tableid = this.SelectedTableId;
                string result = "YAHOO.DataviewBoardNS.TableSummary.RelDataSource.liveData = [";
                TableBO currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableid);
                if (currentTable != null)
                {
                    foreach (RelationshipBO relationship in this.DataViewBO.DataViewManager.Relationships.GetByParentOrChildId(tableid))
                    {
                        TableBO childTbl = this.DataViewBO.DataViewManager.Tables.GetTable(relationship.Child);
                        TableBO parentTbl = this.DataViewBO.DataViewManager.Tables.GetTable(relationship.Parent);
                        if (!relationship.FromMasterSchema && childTbl != null && !childTbl.FromMasterSchema && parentTbl != null && !parentTbl.FromMasterSchema)
                        {
                            string parenttable = string.Format("{0} ({1})", System.Web.HttpUtility.HtmlEncode(parentTbl.Alias), parentTbl.Fields.GetField(relationship.ParentKey).Alias);
                            string childtable = string.Format("{0} ({1})",System.Web.HttpUtility.HtmlEncode(childTbl.Alias), childTbl.Fields.GetField(relationship.ChildKey).Alias);
                            string reltype = relationship.JoinType.ToString();
                            result += "{" + string.Format("parenttable: \"{0}\", childtable: \"{1}\", reltype: \"{2}\"", parenttable, childtable, reltype) + "},";
                        }
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

    public string GetLookupDataSource(int tableid)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (tableid > 0 || this.SelectedTableId > 0)
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                if (tableid < 0)
                    tableid = this.SelectedTableId;
                string result = "YAHOO.DataviewBoardNS.TableSummary.LookupDataSource.liveData = [";

                TableBO currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
                if (currentTable != null)
                {
                    foreach (FieldBO field in currentTable.Fields.GetAllFieldsByLookup())
                    {
                        if (field.LookupFieldId != -1 && field.LookupFieldId != int.MinValue) //Fixed 162170 To add a valid lookup to result, A valid lookup will never have a negative value 
                        {
                            result += "{" + string.Format("lookup: \"{0}\", type: \"{1}\"", field.Alias, field.DataType.ToString()) + "},";
                        }
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

    public string GetTagsDataSource(int tableid)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (tableid > 0 || this.SelectedTableId > 0)
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                if (tableid < 0)
                    tableid = this.SelectedTableId;
                string result = "YAHOO.DataviewBoardNS.TableSummary.TagsDataSource.liveData = [";

                TableBO currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
                if (currentTable != null)
                {
                    // CBOE-895:DVM:Added tags are not getting sorted alphabetically.
                    List<string> lstTags = new List<string>();
                    IEnumerator theIEnumerator = currentTable.Tags.GetEnumerator();
                    if (theIEnumerator != null)
                    {
                        while (theIEnumerator.MoveNext())
                        {
                            lstTags.Add(theIEnumerator.Current.ToString());
                        }

                        lstTags.Sort();
                        foreach (string tag in lstTags)
                        {
                            result += "{" + string.Format("tag: \"{0}\"", tag) + "},";
                        }
                    }
                    // End CBOE-895
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

    /// <summary>
    /// Retrives the table indexes data for specified table id
    /// </summary>
    /// <param name="tableid">Table id for which the index information is required</param>
    /// <returns>Returns the table formation string for YUI</returns>
    public string GetIndexesDataSource(int tableid)
    {
        if (this.DataViewBO == null)
            return string.Empty;

        if (tableid > 0 || this.SelectedTableId > 0)
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                if (tableid < 0)
                    tableid = this.SelectedTableId;
                string result = "YAHOO.DataviewBoardNS.TableSummary.IndexDataSource.liveData = [";

                //retrieve the current selected table
                TableBO currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableid);
                if (currentTable != null)
                {
                    //get the master schema to load the fields which are removed from clone or master table, so that we can display the index details correctly - PP on 06Feb2013
                    COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();
                    if (masterDataViewBO.DataViewManager.Tables.Count > 0)
                        this.DataViewBO.DataViewManager.MergeTable(masterDataViewBO.DataViewManager.Tables, currentTable, masterDataViewBO.DataViewManager.Relationships);

                    DataTable fieldIndexDatatable = DataViewBO.DataViewManager.GetFieldIndexes(currentTable.DataBase, currentTable.Name);

                    if (fieldIndexDatatable != null)
                    {
                        foreach (FieldBO fBo in currentTable.Fields)
                        {
                            DataRow[] indexDataRows = fieldIndexDatatable.Select("column_name = '" + fBo.Name + "'");
                            if (indexDataRows != null && indexDataRows.Length > 0)
                            {
                                fBo.IsIndexed = true;
                                fBo.IndexName = indexDataRows[0]["indexname"].ToString();
                            }
                            else
                            {
                                fBo.IsIndexed = false;
                            }

                            if (fBo.IsIndexed)
                            {
                                result += "{" + string.Format("fieldid: \"{0}\", indexname: \"{1}\"", fBo.Name,fBo.IndexName) + "},";
                            }
                        }
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

    /// <summary>
    /// Checks whether Table alias name is duplicate or not on duplicate Table click
    /// </summary>
    /// <returns></returns>
    public bool IsDuplicateTableAlias(string strDatabase, string strAlias)
    {
        bool isDuplicate = false;
        if (this.DataViewBO != null && this.DataViewBO.DataViewManager != null)
        {
            TableListBO theTableListBO = this.DataViewBO.DataViewManager.Tables;
            if (theTableListBO != null)
            {
                for (int i = 0; i < theTableListBO.Count; i++)
                {
                    if (theTableListBO[i].DataBase == strDatabase && theTableListBO[i].Alias.Trim() == strAlias.Trim())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
        }             
        
        return isDuplicate;
    }

    #endregion
}

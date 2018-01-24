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
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using System.Collections.Generic;


public partial class Forms_DataViewManager_ContentArea_DefineLookup : GUIShellPage, ICallbackEventHandler
{
    #region Variables
    private string _defaultSortOrder = COEDataView.SortDirection.ASCENDING.ToString();
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

    private int ParentTable
    {
        get
        {
            return (int)ViewState[Constants.Parent];
        }
        set
        {
            ViewState[Constants.Parent] = value;
        }
    }

    private int ParentFieldID
    {
        get
        {
            return (int)ViewState[Constants.ParentKey];
        }
        set
        {
            ViewState[Constants.ParentKey] = value;
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

    /// <summary>
    /// Checks if every required element for valid lookup is acomplish.
    /// </summary>
    public string LookupErrors
    {
        get
        {


            int id;
            if (!int.TryParse(this.SelectedFieldIDHidden.Value, out id) || id < 0)
            {
                return Resources.Resource.Lookup_NoLookUpSelected;
            }
            else if (!int.TryParse(this.SelectedDisplayFieldIDHidden.Value, out id) || id < 0)
            {
                return Resources.Resource.Lookup_NoDisplaySelected;
            }
            else if (this.SelectedDisplayFieldIDHidden.Value == this.SelectedFieldIDHidden.Value)
            {
                return Resources.Resource.Lookup_JoinAndDisplaySame;
            }
            else if (!CanSaveLookup())
            {//CBOE-910 Added validation for valid join  ASV 17052013
                return Resources.Resource.Lookup_InvalidJoin;
            }
            if (string.IsNullOrEmpty(this.SortingDropDownList.SelectedValue))
            {
                return Resources.Resource.Lookup_NoSortingSelected;
            }
            return string.Empty;
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
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {

            this.Caller = Request[Constants.Caller];
            this.ParamCaller = Request[Constants.ParamCaller];

            this.ParentTable = int.Parse(this.ParamCaller);
            this.ParentFieldID = this.GetFieldIdFromURL();
            TableBO tbl = this.DataViewBO.DataViewManager.Tables.GetTable(this.ParentTable);
            this.DefaultDatabase = tbl.DataBase;

            FieldBO fld = tbl.Fields.GetField(this.ParentFieldID);
            string sortOrder = _defaultSortOrder;
            

            if (fld.LookupFieldId >= 0)
            {
                TableBO lookupTable = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(fld.LookupFieldId);
                if (lookupTable != null)
                {
                    FieldBO lookupDisplayField = lookupTable.Fields.GetField(fld.LookupDisplayFieldId);
                    this.DefaultDatabase = lookupTable.DataBase;
                    this.SelectedFieldIDHidden.Value = fld.LookupFieldId.ToString();
                    this.SelectedDisplayFieldIDHidden.Value = fld.LookupDisplayFieldId.ToString();
                    this.SelectedTableIDHidden.Value = lookupTable.ID.ToString();

                    this.SelectedToFieldLabel.Text = lookupTable.DataBase + "." + lookupTable.Alias + "." + lookupDisplayField.Alias;
                    sortOrder = fld.LookupSortOrder.ToString();
                }
                else
                {
                    this.SelectedToFieldLabel.Text = "<p style=\"color:red\">Lookup table is not present in dataview</p>";
                }
            }
            this.SelectedFromFieldLabel.Text = tbl.DataBase + "." + tbl.Alias + "." + fld.Alias;
            this.SetControlsAttributtes();
            this.SelectLookupLabel.Text = string.Format(this.SelectLookupLabel.Text, this.SelectedFromFieldLabel.Text, fld.DataType);
            this.AddSortOrderItems(sortOrder);

            this.SchemaDropDownList.DataSource = GetSchemas();
            this.SchemaDropDownList.SelectedValue = this.DefaultDatabase;
            this.SchemaDropDownList.DataBind();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }   

    #endregion

    #region Event Handlers
    void OKImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        if (string.IsNullOrEmpty(this.LookupErrors))
        {
            TableBO tbl = this.DataViewBO.DataViewManager.Tables.GetTable(this.ParentTable);
            FieldBO fld = tbl.Fields.GetField(this.ParentFieldID);
            fld.LookupFieldId = int.Parse(this.SelectedFieldIDHidden.Value);
            fld.LookupDisplayFieldId = int.Parse(this.SelectedDisplayFieldIDHidden.Value);
            if (this.SortingDropDownList.SelectedValue.ToUpperInvariant() == "ASCENDING")
                fld.LookupSortOrder = COEDataView.SortDirection.ASCENDING;
            else
                fld.LookupSortOrder = COEDataView.SortDirection.DESCENDING;

            this.GoToPreviousPage();
        }
        else
            this.DisplayErrorMessage(this.LookupErrors);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        TableBO tbl = this.DataViewBO.DataViewManager.Tables.GetTable(this.ParentTable);
        if (tbl != null)
        {
            FieldBO fld = tbl.Fields.GetField(this.ParentFieldID);
            //Coverity Bug Fix CID :20722 
            if (fld != null)
            {
                FieldBO field = this.DataViewBO.DataViewManager.Tables.GetField(fld.LookupDisplayFieldId);
                if (field != null)
                {
                    fld.IsDefault = false;
                    field.IsDefault = false;
                }

                tbl.Fields.RemoveEntryFromLookUp(fld.LookupFieldId);
                tbl.Fields.RemoveEntryFromLookUp(fld.LookupDisplayFieldId);
                fld.LookupFieldId = -1;
                fld.LookupDisplayFieldId = -1;
            }
        }
        this.GoToPreviousPage();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoToPreviousPage();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.Lookup_Page_Title);
        this.TopDeleteImageButton.ButtonText = this.TopDeleteImageButton.ImageToolTip = this.BottomDeleteImageButton.ButtonText = this.BottomDeleteImageButton.ImageToolTip = Resources.Resource.Delete_Button_Text;
        this.InvalidFieldsLabel.Text = Resources.Resource.InvalidLookUp_Label_Text;
        this.SelectLookupLabel.Text = Resources.Resource.SelectLookupFor_Label_Text;
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

            string result = "YAHOO.LookupsNS.LeftPanel.DataSource.liveData = [";
            foreach (TableBO table in this.DataViewBO.DataViewManager.Tables)
            {
                if (table.DataBase == schema && table.ID != this.ParentTable)                
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

    public string GetFieldsDataSource(int tableId, int numberOfRows, int sortOrder)
    {
        int totalRecords = 0;
        int lookup;
        int.TryParse(this.SelectedFieldIDHidden.Value, out lookup);
        int display;
        int.TryParse(this.SelectedDisplayFieldIDHidden.Value, out display);
        TableBO tbl = this.DataViewBO.DataViewManager.Tables.GetTable(tableId);
        if (tbl != null)
        {
            lookup = tbl.Fields.GetField(lookup) != null ? lookup : -1;
            display = tbl.Fields.GetField(display) != null ? display : -1;
            if (lookup == -1 || display == -1)
            {
                lookup = display = -1;
            }
        }

        if (tableId != -1 || int.TryParse(this.SelectedTableIDHidden.Value, out tableId))
        {
            if (this.DataViewBO.DataViewManager.Tables.Count > 0)
            {
                TableBO currentTable = null;
                if (tableId >= 0)
                    currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableId);
                else if (!string.IsNullOrEmpty(this.SelectedTableIDHidden.Value) && int.Parse(this.SelectedTableIDHidden.Value) >= 0)
                    currentTable = this.DataViewBO.DataViewManager.Tables.GetTable(int.Parse(this.SelectedTableIDHidden.Value));

                if (currentTable != null)
                {
                    string result = "YAHOO.LookupsNS.RightPanelTop.DataSource.liveData = [";

                    List<string> lstFields = new List<string>();
                    if (currentTable != null)
                    {
                        IEnumerator theIEnumerator = currentTable.Fields.GetEnumerator();
                        if (theIEnumerator != null)
                        {
                            while (theIEnumerator.MoveNext())
                            {
                                lstFields.Add((theIEnumerator.Current as FieldBO).Alias);
                            }

                            totalRecords = currentTable.Fields.Count;

                            lstFields.Sort();
                            // sorting field list scending
                            if (sortOrder == -1)
                            {
                                if (this.SortingDropDownList.SelectedValue == COEDataView.SortDirection.ASCENDING.ToString())
                                    sortOrder = 0;
                                else
                                    sortOrder = 1;
                            }

                            int pages = Convert.ToInt32(Math.Ceiling((decimal)totalRecords / numberOfRows));
                            int startIndex = 0;
                            // sorting field list descending  (page wise sorting)                                                         
                            if (sortOrder == 1)
                            {
                                startIndex = 0;

                                for (int j = 0; j < pages; j++)
                                {
                                    if (j == pages - 1)
                                    {
                                        lstFields.Reverse(startIndex, totalRecords - startIndex);
                                    }
                                    else
                                        lstFields.Reverse(startIndex, numberOfRows);
                                    startIndex += numberOfRows;
                                }
                            }

                            foreach (string field in lstFields)
                            {
                                foreach (FieldBO fld in currentTable.Fields)
                                {
                                    if (fld.Alias == field)
                                    {
                                        if (fld.LookupDisplayFieldId < 0) //Dont want to list a lookup as a parent field for a relationship
                                            result += "{" + string.Format("fieldalias: \"{0}\", fieldid: \"{1}\", fieldtype: \"{2}\", islookup: {3}, isdisplay: {4}", fld.Alias.ToString(), fld.ID, fld.DataType.ToString().ToLower(), (lookup >= 0 && lookup == fld.ID).ToString().ToLower(), (display >= 0 && display == fld.ID).ToString().ToLower()).ToString() + "},";

                                        break;
                                    }
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
    private void AddSortOrderItems(string selectedItem)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.SortingDropDownList.Items.Count == 0) //Create dropdown items.
        {
            foreach (string item in Enum.GetNames(typeof(COEDataView.SortDirection)))
            {
                ListItem currentItem = new ListItem(item, item);
                if (item.ToUpper() == selectedItem.ToUpper())
                {
                    currentItem.Selected = true;
                }
                this.SortingDropDownList.Items.Add(currentItem);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Gets the given fieldId reading the URL.
    /// </summary>
    /// <returns></returns>
    private int GetFieldIdFromURL()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        if (Request[Constants.FieldId] != null)
            int.TryParse(Request[Constants.FieldId].ToString(), out retVal);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    private void GoToPreviousPage()
    {
        string url = this.Caller + ".aspx?" + Constants.ParamCaller + "=" + this.ParamCaller;
        Server.Transfer(url, false);
    }

    private void DisplayErrorMessage(string errorMessage)
    {
        this.ErrorAreaUserControl.Text = errorMessage;
        this.ErrorAreaUserControl.Visible = true;
    }

    //CBOE-910 : added method to validate joins on lookup field
    /// <summary>
    /// Validate join of the lookup field.
    /// Join is validated against datatype and index type of both fields.
    /// </summary>
    /// <returns>True if valid join else false</returns>
    private Boolean CanSaveLookup()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            this.ParentFieldID = this.GetFieldIdFromURL();
            FieldBO parentKey = this.DataViewBO.DataViewManager.Tables.GetField(this.ParentFieldID);
            int ChildFieldID = int.Parse(this.SelectedFieldIDHidden.Value);
            FieldBO childKey = this.DataViewBO.DataViewManager.Tables.GetField(ChildFieldID);

            if ((parentKey != null && childKey != null) && (parentKey.DataType == childKey.DataType) &&
                ((parentKey.DataType != COEDataView.AbstractTypes.BLob) && (childKey.DataType != COEDataView.AbstractTypes.BLob) &&
                (parentKey.DataType != COEDataView.AbstractTypes.CLob) && (childKey.DataType != COEDataView.AbstractTypes.CLob))
                )
                return true;
            return false;
        }
        catch
        {
            return false;
        }
        finally
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
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
            int fieldId = -1;
            int displayId = -1;
            int tableId = -1;
            int numberOfRows = 20;
            string sortOrder = COEDataView.SortDirection.ASCENDING.ToString();
            int intSortOrder = 0; // ASC

            string[] arr = eventArgument.Split(',');
            if (arr != null && arr.Length >= 3)
            {

                tableId = int.Parse(arr[0].Replace("SelectTable: ", string.Empty));
                fieldId = int.Parse(arr[1].Replace("FieldID:", string.Empty));
                displayId = int.Parse(arr[2].Replace("DisplayID:", string.Empty));
            }
            this.SelectedFieldIDHidden.Value = fieldId.ToString();
            this.SelectedDisplayFieldIDHidden.Value = displayId.ToString();
            this.SelectedTableIDHidden.Value = tableId.ToString();
            numberOfRows = int.Parse(arr[3].Replace("TotalRows:", string.Empty));
            sortOrder = arr[4].Replace("SortOrder:", string.Empty);
            if (sortOrder == COEDataView.SortDirection.DESCENDING.ToString())
                intSortOrder = 1;
            result = "SelectTable: " + GetFieldsDataSource(tableId, numberOfRows, intSortOrder);
        }
        else if (eventArgument.StartsWith("SelectLookup: "))
        {
            int fieldId = int.Parse(eventArgument.Replace("SelectLookup: ", string.Empty));
            this.SelectedFieldIDHidden.Value = fieldId.ToString();
            result = "SelectLookup: True";

        }
        else if (eventArgument.StartsWith("SelectDisplay: "))
        {
            int fieldId = int.Parse(eventArgument.Replace("SelectDisplay: ", string.Empty));
            this.SelectedDisplayFieldIDHidden.Value = fieldId.ToString();
            result = "SelectDisplay: True";
        }
    }

    #endregion
}


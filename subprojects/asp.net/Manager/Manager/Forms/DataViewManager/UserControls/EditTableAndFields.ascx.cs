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
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Infragistics.WebUI.UltraWebNavigator;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebGrid;
using System.Text;
using Csla;
using System.Linq;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;

public partial class EditTableAndFields : System.Web.UI.UserControl, ICallbackEventHandler
{
    #region Enums
    /// <summary>
    /// List of shown columns in the FieldGrid
    /// </summary>
    private enum ColumnKeys
    {
        Edit,
        ID,
        Radio,
        Name,
        DataType,
        IndexType,
        MimeType,
        PrimaryKey,
        Visible,
        LookUpTable,
        ParentColumn,
        Alias,
        SortOrder,
        Action,
        IsUniqueKey,
        IsDefaultQuery,
        IsDefault
    }

    /// <summary>
    /// List shows error status for primary/unique selection.
    /// </summary>
    private enum PkUkErrorStatus
    {
        /// <summary>
        /// Selected table is base table and no Pk/Uk is selected
        /// </summary>
        SelectPkUkForBaseTable = 1,
        /// <summary>
        /// Selected table is base table and Pk/Uk is selected
        /// </summary>
        InvalidPkUkForBaseTable = 2,
        /// <summary>
        /// Selected base table does not have Primary/Unique key constraint.
        /// </summary>
        SelectedBaseTblHasNoValidPkUk = 3,
        /// <summary>
        /// Table does not have Primary/Unique key constraint.
        /// </summary>
        ChildTableNoPkUkConstraint = 4,
        /// <summary>
        /// Selected Pk/Uk field is valid.
        /// </summary>
        ValidPkUkField = -1
    }

    #endregion

    #region Constants
    private const string _rootNode = "rootNode";
    private const string _defaultAliasWord = "_Alias";
    private const string _parentColumnButtonID = "ParentColumnButton";
    private const string _lookupButtonID = "LookupButton";
    private const string _visibleCheckBoxID = "VisibleCheckBox";
    private const string _isUniqueKeyCheckBoxID = "IsUniqueKeyCheckBox";
    private const string _isDefaultQueryCheckBoxID = "IsDefaultQueryCheckBox";
    private const string _isDefaultCheckBoxID = "IsDefaultCheckBox";
    private const string _actionButtonID = "ActionButton";
    private const string _indexTypeDropDownID = "IndexTypeDropDown";
    private const string _mimeTypeDropDownID = "MimeTypeDropDown";
    #endregion

    private bool _checked = false;
    protected bool Apply_Indexing = false;
    private bool _isPrimaryKeyValid = true;
    private bool _isView = false;		//CBOE-529  Added variable to hold value of IsView property of current selected table. ASV 27032013
    Infragistics.WebUI.WebDataInput.WebTextEdit txtTableAlias = null;

    private ValueList indexTypeValueList = null;
    private string errorMsg = string.Empty;

    #region Properties
    /// <summary>
    /// Current selected Node.
    /// </summary>
    private string SelectedTable
    {
        get
        {
            string retVal = String.Empty;
            if (ViewState[Constants.SelectedTable] != null)
                retVal = ViewState[Constants.SelectedTable].ToString() == _rootNode ? String.Empty : ViewState[Constants.SelectedTable].ToString();
            return retVal;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState[Constants.SelectedTable] = value;
        }
    }

    /// <summary>
    /// Current Selected tableId
    /// </summary>
    private int SelectedTableId
    {
        get
        {
            int retVal = -1;
            if (!string.IsNullOrEmpty(this.SelectedTable))
                int.TryParse(Utilities.GetParamInDataKey(this.SelectedTable, Constants.DataKeysParam.ID), out retVal);
            return retVal;
        }
    }

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

    private string CancelURL
    {
        get
        {
            string retVal = String.Empty;
            if (ViewState["CancelURL"] != null)
                retVal = (string)ViewState["CancelURL"];
            return retVal;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState["CancelURL"] = value;
        }
    }

    private int DeletedFieldCount
    {
        get
        {
            if (ViewState["HasDeletedFields"] == null)
            {
                ViewState["HasDeletedFields"] = 0;
            }

            return (int)ViewState["HasDeletedFields"];
        }
        set
        {
            ViewState["HasDeletedFields"] = value;
        }
    }

    /// <summary>
    /// Show or not master schema (they are comming from a merge) fields.
    /// </summary>
    private bool ShowMasterSchemaFields
    {
        get { return this.ShowMasterSchemaFieldsHidden.Value == "1" ? true : false; }
    }
    private bool isChecked
    {
        get { return _checked; }
        set { _checked = value; }
    }

    private bool isPrimaryKeyValid
    {
        get { return _isPrimaryKeyValid; }
        set { _isPrimaryKeyValid = value; }
    }
    //CBOE-529 added IsView property	ASV 27032013
    private bool IsView
    {
        get { return _isView; }
        set { _isView = value; }
    }

    private ValueList IndexTypeValueList
    {
        get
        {
            if (indexTypeValueList == null)
            {
                var cartridgeBo = ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetValidCartridgeBOEx();
                indexTypeValueList = Utilities.GetIndexTypeValueList(cartridgeBo);
            }

            return indexTypeValueList;
        }
    }

    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.TableFieldsUltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(TableFieldsUltraWebGrid_InitializeLayout);
        this.TableFieldsUltraWebGrid.TemplatedColumnRestored += new TemplatedColumnRestoredEventHandler(TableFieldsUltraWebGrid_TemplatedColumnRestored);
        this.TableFieldsUltraWebGrid.InitializeRow += new InitializeRowEventHandler(TableFieldsUltraWebGrid_InitializeRow);
        this.TableFieldsUltraWebGrid.ItemCommand += new ItemCommandEventHandler(TableFieldsUltraWebGrid_ItemCommand);

        #region Button Events
        this.CreateAliasFieldImageButton.ButtonClicked += new EventHandler<EventArgs>(CreateAliasFieldImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.OkImageButton.ButtonClicked += new EventHandler<EventArgs>(OkImageButton_ButtonClicked);
        this.ToggleButton.Click += new EventHandler(ToggleButton_Click);
        #endregion
        txtTableAlias = this.AliasTextBoxWithPopUp.FindControl("SummaryTextBox") as Infragistics.WebUI.WebDataInput.WebTextEdit;
        if (txtTableAlias != null)
        {
            txtTableAlias.ValueChange += new Infragistics.WebUI.WebDataInput.ValueChangeHandler(txtTableAlias_ValueChange);
            txtTableAlias.MaxLength = 30;
        }
        this.TableFieldsUltraWebGrid.JavaScriptFileName = "/COECommonResources/infragistics/20111CLR20/Scripts/ig_WebGrid.js";
        GetApplyIndexingConfiguration();
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (string.IsNullOrEmpty(this.CancelURL))
        {
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
            //Added condition to check if SelectedTableId is less than zero.
            //This will be the case if the session ends while editing dataview.
            if (this.SelectedTableId <= 0)
                RedirectToBasePage();

            this.CancelURL = Page.ResolveUrl("~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx");
            this.CancelURL += "?" + Constants.ParamCaller + "=" + this.SelectedTableId;

            if (Request.Params["schemaSelected"] != null)
            {
                this.CancelURL += "&schemaSelected=" + Request.Params["schemaSelected"];
            }
        }
        if (!Page.IsPostBack)
        {
            GetPrimaryUniqueKeyConstraintInfo();
            this.SetControlsAttributtes();
            //this.TableFieldsUltraWebGrid.Rows.Sort(true);
        }
        this.AddRowsMoving();

        //CSBR-137648
        //Displaying the Alias text information from session
        if (Session["EditTablealias"] != null)
        {
            AliasTextBoxWithPopUp.Text = Session["EditTablealias"].ToString();
            Session.Remove("EditTablealias");
        }
        //End CSBR-137648
        //CBOE-529
        if (IsView)
            lblVewSelectedError.Text = Resources.Resource.Apply_Index_Error_Message_ForView;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }


    //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
    /// <summary>
    /// This method will redirect to base page as per querystring.
    /// </summary>
    private void RedirectToBasePage()
    {
        //Common url to return
        String ReturnUrl = "~/Forms/DataViewManager/ContentArea/NewDataView.aspx";
        Constants.PageStates Action = Constants.PageStates.Undefined;
        //check for the action parameter in querystring.
        if (Request[Constants.Action] != null)
        {
            if (Enum.IsDefined(typeof(Constants.PageStates), Request[Constants.Action].ToString()))
                Action = (Constants.PageStates)Enum.Parse(typeof(Constants.PageStates), Request[Constants.Action].ToString());
        }
        else if (Request["IsMaster"] != null && bool.Parse(Request["IsMaster"]))
        {
            //condition for 'Edit Master'. Redirect to DataviewBoard.
            ReturnUrl = "~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx?IsMaster=true";
            Response.Redirect(Page.ResolveUrl(ReturnUrl), true);
        }

        String SelectedDataViewID = string.Empty;
        //check for the SelectedDataViewID parameter in querystring.
        if (this.Page.ClientQueryString.Contains(Constants.SelectedDataViewID))
            SelectedDataViewID = Page.Request.QueryString.Get(Constants.SelectedDataViewID);

        switch (Action)
        {
            case Constants.PageStates.Clone_DV:
                if (!string.IsNullOrEmpty(SelectedDataViewID) && SelectedDataViewID == "0")    // Create from Master
                    ReturnUrl = "~/Forms/DataViewManager/ContentArea/EnterNameDescription.aspx";
                break;
            case Constants.PageStates.Edit_DV:      //Edit Existing
                break;

            case Constants.PageStates.Create_DV:
            case Constants.PageStates.Undefined:
            default: Response.Redirect(Constants.PublicContentAreaFolder + "Home.aspx", true);
                break;
        }
        //add action, dataview id, selected table id i.e. paramcaller to return string
        //this will help on NewDataviewPage to take proper action.
        ReturnUrl += "?" + Constants.Action + "=" + Action;
        //check for the SelectedDataViewID parameter in querystring.
        if (this.Page.ClientQueryString.Contains(Constants.ParamCaller))
            ReturnUrl += "&" + Constants.ParamCaller + "=" + Page.Request.QueryString.Get(Constants.ParamCaller);
        //check for the SelectedDataViewID parameter in querystring.
        if (!string.IsNullOrEmpty(SelectedDataViewID))
            ReturnUrl += "&" + Constants.SelectedDataViewID + "=" + SelectedDataViewID;

        Response.Redirect(Page.ResolveUrl(ReturnUrl), true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Jscrip to handle just one selected radio button item.
        StringBuilder jscriptText = new StringBuilder();
        string jscriptKey = "RadioButtonSelection";
        jscriptText.Append(@"<script language='javascript'>");
        jscriptText.Append(@"function activateGridRow(theIndex, fieldID){");
        jscriptText.Append(@"igtbl_getGridById('" + this.TableFieldsUltraWebGrid.ClientID + "').Rows.getRow(theIndex).activate();");
        jscriptText.Append(@"var PrevPk = document.getElementById('" + this.PrevPkFieldIdHidden.ClientID + "').value;");
        jscriptText.Append(@"var PrevLookupDisabled = document.getElementById('" + this.PrevLookupDisabledHidden.ClientID + "').value;");
        jscriptText.Append(@"document.getElementById('" + this.SelectedPKIndexHidden.ClientID + "').value = fieldID;");
        //CSBR 135133
        #region CSBR 135133
        jscriptText.Append(@"var grid = igtbl_getGridById('" + this.TableFieldsUltraWebGrid.ClientID + "');");
        //Loop grid rows
        jscriptText.Append(@"for( var i = 0; i < grid.Rows.length; i++ ){");
        jscriptText.Append(@"var row = grid.Rows.getRow(i);");
        jscriptText.Append(@"var lookupCell = row.getCellFromKey('" + ColumnKeys.LookUpTable.ToString() + "');");
        jscriptText.Append(@"var ParentCell = row.getCellFromKey('" + ColumnKeys.ParentColumn.ToString() + "');");
        jscriptText.Append(@"var IdCell = row.getCellFromKey('" + Constants.ID.ToString() + "');");
        jscriptText.Append(@"var visibleCell = row.getCellFromKey('" + ColumnKeys.Visible.ToString() + "');");

        //checkbox of lookupCell
        jscriptText.Append(@"var _lookupCellCheckBoxElement = lookupCell.Element.getElementsByTagName('input')[0];");
        //checkbox of visibleCell
        jscriptText.Append(@"var _visibleCellCheckBoxElement = visibleCell.Element.getElementsByTagName('input')[0];");

        //Previous Pk field
        jscriptText.Append(@" if(IdCell.getValue() == PrevPk) {");
        jscriptText.Append(@"   if(PrevLookupDisabled == 'false') ");
        jscriptText.Append(@"       _lookupCellCheckBoxElement.disabled = false;");//enable lookup button previous pk selected field lookup if there is no parent
        jscriptText.Append(@"_visibleCellCheckBoxElement.disabled = false;");
        jscriptText.Append(@"visibleCell.Element.innerHTML = visibleCell.Element.innerHTML.replace(/<span disabled=""true"">/i, '<span>');");
        jscriptText.Append(@"visibleCell.Element.innerHTML = visibleCell.Element.innerHTML.replace(/<span disabled="""">/i, '<span>');");
        jscriptText.Append(@"visibleCell.Element.innerHTML = visibleCell.Element.innerHTML.replace(/<span disabled=""disabled"">/i, '<span>');");
        jscriptText.Append(@"visibleCell.Element.innerHTML = visibleCell.Element.innerHTML.replace(/<span disabled>/i, '<span>');");
        jscriptText.Append(@"visibleCell.Element.innerHTML = visibleCell.Element.innerHTML.replace(/<span disabled=disabled>/i, '<span>');");
        jscriptText.Append(@"}");//Close if idcell and pk

        //Current Pk Field
        jscriptText.Append(@" if(IdCell.getValue() == fieldID) {");
        //updating the previous pk filed and its lookup status
        jscriptText.Append(@"document.getElementById('" + this.PrevPkFieldIdHidden.ClientID + "').value = fieldID;");
        jscriptText.Append(@"document.getElementById('" + this.PrevLookupDisabledHidden.ClientID + "').value = _lookupCellCheckBoxElement.disabled;");
        jscriptText.Append(@"document.getElementById('" + this.PrevVisibleFieldHidden.ClientID + "').value = _visibleCellCheckBoxElement.disabled;");
        //disable the lookup button when the the field select as primary key
        jscriptText.Append(@"_lookupCellCheckBoxElement.disabled = true;");
        jscriptText.Append(@"_visibleCellCheckBoxElement.disabled = true;");
        // Visible field should be checked for primary key       
        jscriptText.Append(@"var _CheckBoxElement = visibleCell.Element.getElementsByTagName('input')[0];");
        jscriptText.Append(@"_CheckBoxElement.checked = true;");
        // Checking Visible column header
        jscriptText.Append(@"CheckedChanged('Visible');");

        jscriptText.Append(@"row.activate();");
        jscriptText.Append(@"}");//Close if idcell and fieldid

        jscriptText.Append(@"}");//Close for

        #endregion

        jscriptText.Append(@"}");

        jscriptText.Append(@"function EditCurrent(btnEl){");
        jscriptText.Append(@"var oGrid = igtbl_getGridById(" + this.TableFieldsUltraWebGrid.ClientID + ");");
        jscriptText.Append(@" if(btnEl.value == 'Edit Current Row') {");
        jscriptText.Append(@"oGrid.beginEditTemplate();");
        jscriptText.Append(@"btnEl.value = 'End Row Editing'");

        jscriptText.Append(@"}");
        jscriptText.Append(@"else {");
        jscriptText.Append(@"oGrid.endEditTemplate();");
        jscriptText.Append(@"btnEl.value = 'Edit Current Row';");
        jscriptText.Append(@"}");
        jscriptText.Append(@"}");

        jscriptText.Append(@"function InitializeGrid(gridId) {
            var oGrid = igtbl_getGridById(gridId);
        }
        function CellButtonClick(gridId, cellId) {
            var oGrid = igtbl_getGridById(gridId);
            var oRow = igtbl_getRowById(cellId);
            oRow.editRow();
        }");
        jscriptText.Append(@"</script>");

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(jscriptKey))
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), jscriptKey, jscriptText.ToString());
        //this.TableFieldsUltraWebGrid.Rows.Sort(true);

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.Visible.ToString()))
        {
            // Sets Visible column header checkbox (Checks all Visible fields are checked or not)
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Visible.ToString())).HeaderItem.FindControl("IsAllVisibleKeyCheckBox")).Checked = IsAllFieldValuesChecked("Visible");
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Visible.ToString())).HeaderItem.FindControl("IsAllVisibleKeyCheckBox")).Text = " " + Resources.Resource.Visible_ColHeader_Caption;
        }

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.IsDefault.ToString()))
        {
            // Sets Default column header checkbox (Checks all Default fields are checked or not)
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefault.ToString())).HeaderItem.FindControl("IsAllDefaultKeyCheckBox")).Checked = IsAllFieldValuesChecked("IsDefault");
            //this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefault.ToString()).Header.Caption = Resources.Resource.IsDefault_ColHeader_Caption;
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefault.ToString())).HeaderItem.FindControl("IsAllDefaultKeyCheckBox")).Text = " " + Resources.Resource.IsDefault_ColHeader_Caption;
        }

        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Event Handlers
    void ToggleButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.ToggleButton.Text == Resources.Resource.ShowHiddenFields_Button_Text)
            this.ToggleButton.Text = Resources.Resource.HideHiddenFields_Button_Text;
        else
            this.ToggleButton.Text = Resources.Resource.ShowHiddenFields_Button_Text;
        this.UnBind();
        this.ShowMasterSchemaFieldsHidden.Value = this.ShowMasterSchemaFieldsHidden.Value == "0" ? "1" : "0";
        this.BindTableDetails(this.SelectedTableId);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void OkImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.UnBind();
        string errorMessage = ValidateOk();
        if (string.IsNullOrEmpty(errorMessage) && Session["IsWarningPkUkShown"] == null)
        {
            string urlRedirect = "~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx";

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(Page.ResolveUrl(urlRedirect));
        }
        else if (Session["IsWarningPkUkShown"] != null)
        {
            WarningControl.Visible = true;
            WarningControl.Text = Resources.Resource.WarningInvalidPkUk;
            this.BindTableDetails(this.SelectedTable);
            this.ErrorAreaUserControl.Text = errorMessage;
            this.ErrorAreaUserControl.Visible = false;
        }
        else
        {
            this.BindTableDetails(this.SelectedTable);
            this.ErrorAreaUserControl.Text = errorMessage;//Resources.Resource.Invalid_PrimaryKey;
            this.ErrorAreaUserControl.Visible = true;
        }
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RevertEdition();
        this.Session.Remove("FreshTable");
        this.Session.Remove("FreshRels");
        Session["EditCancel"] = true;
        Server.Transfer(this.CancelURL, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void TableFieldsUltraWebGrid_ItemCommand(object sender, UltraWebGridCommandEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.UnBindFieldsInfo();
        if (e.CommandSource is Button)
        {
            if (((Button)e.CommandSource).ID == _parentColumnButtonID)
                this.GoToParentColumnEdition((Button)e.CommandSource);
            else if (((Button)e.CommandSource).ID == _actionButtonID)
                this.ChangeFieldStatus(((Button)e.CommandSource).Attributes[Constants.FieldId].ToString(), (e.CommandSource as Button).Text);
            else if (((Button)e.CommandSource).ID == _lookupButtonID)
                this.EditLookUp(((Button)e.CommandSource).Attributes[Constants.FieldId].ToString());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CreateAliasFieldImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.CreateAliasField();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to recreate the dropdowns controls after a post back.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TableFieldsUltraWebGrid_TemplatedColumnRestored(object sender, ColumnEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        if (e.Column.Key == ColumnKeys.IndexType.ToString())
            e.Column.ValueList = Utilities.GetIndexTypeValueList();
        else if (e.Column.Key == ColumnKeys.MimeType.ToString())
            e.Column.ValueList = Utilities.GetMimeTypeValueList();
        else if (e.Column.Key == ColumnKeys.ParentColumn.ToString())
        {
            if (((TemplatedColumn)e.Column).CellTemplate == null)
                ((TemplatedColumn)e.Column).CellTemplate = new ParentColumnButton();
        }
        else if (e.Column.Key == ColumnKeys.Action.ToString())
        {
            if (((TemplatedColumn)e.Column).CellTemplate == null)
                ((TemplatedColumn)e.Column).CellTemplate = new ActionButton();
        }
        else if (e.Column.Key == ColumnKeys.LookUpTable.ToString())
        {
            if (((TemplatedColumn)e.Column).CellTemplate == null)
                ((TemplatedColumn)e.Column).CellTemplate = new LookupButton();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Ocurrs one time for element in the datasource. In this case, one time per field.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TableFieldsUltraWebGrid_InitializeRow(object sender, RowEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            if (e.Row.Index > -1)
            {
                //avoid multiple times typecasting. - PP on 30Jan2013
                FieldBO fieldBOObject = (FieldBO)e.Data;
                String strDataTypeLob = string.Empty;

                // CBOE-1722, condition added for index type, if the dataview is migrated from the previous version to 12.6, it shows data type as text with index type as CS_CARTRIAGE for structure fields 
                if (fieldBOObject.DataType == COEDataView.AbstractTypes.BLob || fieldBOObject.DataType == COEDataView.AbstractTypes.CLob || fieldBOObject.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                    strDataTypeLob = fieldBOObject.DataType.ToString();

                if (IndexTypeValueList.ValueListItems.All.Any(item=>item.ToString().Equals(fieldBOObject.IndexType.ToString())))
                {
                    e.Row.Cells.FromKey(ColumnKeys.IndexType.ToString()).Text = fieldBOObject.IndexType.ToString();
                }
                else
                {
                    e.Row.Cells.FromKey(ColumnKeys.IndexType.ToString()).Text = Enum.GetName(typeof(COEDataView.IndexTypes), COEDataView.IndexTypes.UNKNOWN);
                    this.errorMsg += string.Format(Resources.Resource.UnsupportedCartridge_Label_Text, fieldBOObject.IndexType, fieldBOObject.Name);
                }

                e.Row.Cells.FromKey(ColumnKeys.MimeType.ToString()).Text = fieldBOObject.MimeType.ToString();
                ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Visible.ToString()).Column).CellItems[e.Row.Index]).FindControl(_visibleCheckBoxID)).Checked = fieldBOObject.Visible;
                ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsUniqueKey.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isUniqueKeyCheckBoxID)).Checked = fieldBOObject.IsUniqueKey;
                ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isDefaultCheckBoxID)).Checked = fieldBOObject.IsDefault;

                this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Alias.ToString()).Title = Resources.Resource.DisplayName_ToolTip;

                //Bug Fixing : CBOE-242
                CheckBox chkIsUnique = (CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsUniqueKey.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isUniqueKeyCheckBoxID);
                if (String.IsNullOrEmpty(strDataTypeLob))
                    chkIsUnique.Attributes.Add("onclick", "javascript:CheckPkUkConstraint('" + fieldBOObject.Name + "','" + fieldBOObject.ID + "');");
                else
                {
                    ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsUniqueKey.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = "Unique key " + Resources.Resource.LOB_DataType_Error_Text + strDataTypeLob;
                    chkIsUnique.Enabled = false;
                }
                //end of CBOE-242

                //CBOE-529 if selected table is not view then only show color and tooltip. ASV 27032013
                if (!IsView)
                {
                    //JIRA ID : CBOE-482 Changed background color to orange and added tooltip to non-indexed column ASV 19032013
                    ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).Cell.Style.BackColor = System.Drawing.Color.OrangeRed;
                    if (String.IsNullOrEmpty(strDataTypeLob))
                        ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = Resources.Resource.ToolTip_IsDafaultQuery_NonIndexed_Field;
                    else
                        ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = "Index " + Resources.Resource.LOB_DataType_Error_Text + strDataTypeLob;
                }

                CheckBox chkisDefaultQuery = (CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isDefaultQueryCheckBoxID);
                //set IsDefaultQuery check box value based on IsDefault and IsIndexed properties of FieldBO object - PP on 29Jan2013
                if (fieldBOObject.IsDefaultQuery || fieldBOObject.IsIndexed)
                {
                    if (DataViewBO.ID > 0) //check if the view is saved and not a master dataview
                        chkisDefaultQuery.Checked = fieldBOObject.IsDefaultQuery;
                    else
                        chkisDefaultQuery.Checked = true;
                    //JIRA ID : CBOE-482 Changed color from red to green and added tooltip to indexed column ASV 19032013
                    if (fieldBOObject.IsIndexed && !IsView)  //CBOE-529 if selected table is not view then only show color and tooltip. ASV 27032013
                    {
                        ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).Cell.Style.BackColor = System.Drawing.Color.GreenYellow;
                        ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = Resources.Resource.ToolTip_IsDafaultQuery_Indexed_Field;
                    }//end CBOE-482
                }

                //Add onclick event to checkbox based on Apply Indexing and IsIndexed property.
                if (Apply_Indexing && !fieldBOObject.IsIndexed && !IsView && String.IsNullOrEmpty(strDataTypeLob))   //CBOE-529 if selected table is not view then only show popup. ASV 27032013
                    chkisDefaultQuery.Attributes.Add("onclick", "javascript:ShowConfirmationBox('" + fieldBOObject.Name + "','" + fieldBOObject.ID + "');");

                //SortOrder Field
                this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.SortOrder.ToString()).Value = fieldBOObject.SortOrder;
                //If it's a field comming from a master schema we should display it colapsed or disable.
                if (fieldBOObject.FromMasterSchema)
                {
                    if (!this.ShowMasterSchemaFields)
                        e.Row.Hidden = true;
                    else
                    {
                        e.Row.Hidden = false;
                        e.Row.Style.BackColor = System.Drawing.Color.LightGray;
                        foreach (UltraGridCell cell in e.Row.Cells)
                        {
                            cell.AllowEditing = AllowEditing.No;
                        }
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Action.ToString()).Column).CellItems[e.Row.Index]).FindControl(_actionButtonID)).Text = Resources.Resource.Add_Button_Text;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Enabled = false;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Enabled = false;
                        e.Row.Cells.FromKey(ColumnKeys.IndexType.ToString()).AllowEditing = AllowEditing.No;
                        e.Row.Cells.FromKey(ColumnKeys.MimeType.ToString()).AllowEditing = AllowEditing.No;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Visible.ToString()).Column).CellItems[e.Row.Index]).FindControl(_visibleCheckBoxID)).Enabled = false;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsUniqueKey.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isUniqueKeyCheckBoxID)).Enabled = false;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isDefaultQueryCheckBoxID)).Enabled = false;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[e.Row.Index]).FindControl(_isDefaultCheckBoxID)).Enabled = false;
                    }
                }
                else
                {
                    if (fieldBOObject.ID == this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId).PrimaryKey)
                    {
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Action.ToString()).Column).CellItems[e.Row.Index]).FindControl(_actionButtonID)).Visible = false;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Visible.ToString()).Column).CellItems[e.Row.Index]).FindControl(_visibleCheckBoxID)).Attributes.Add("disabled", "true");
                    }
                    else
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Action.ToString()).Column).CellItems[e.Row.Index]).FindControl(_actionButtonID)).Text = Resources.Resource.Remove_Label_Text;
                }
                //We add this attribute in order to identify later the clicked Action Button control.
                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Action.ToString()).Column).CellItems[e.Row.Index]).FindControl(_actionButtonID)).Attributes.Add(Constants.FieldId, fieldBOObject.ID.ToString());
                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.Action.ToString()).Column).CellItems[e.Row.Index]).FindControl(_actionButtonID)).Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                //We add this attribute in order to identify later the clicked LookUp Button control.
                if (String.IsNullOrEmpty(strDataTypeLob))
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Attributes.Add(Constants.FieldId, fieldBOObject.ID.ToString());
                //Paretn Column Stuff
                int fieldKeyId = (int)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ID.ToString()).Value;
                List<RelationshipBO> relationships = this.DataViewBO.DataViewManager.Relationships.GetByChildKey(fieldKeyId);
                //Assume is the first element.
                if (relationships.Count > 0 && !relationships[0].FromMasterSchema)
                {
                    TableBO parentTable = null;
                    FieldBO parentField = null;
                    //TODO: Check is this try/catch can be removed in the future. This code is here because we are traying to keep working on relationships validation and don't stop at this step.
                    try
                    {
                        parentTable = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(relationships[0].ParentKey);
                        parentField = this.DataViewBO.DataViewManager.Tables.GetField(relationships[0].ParentKey);

                        if (parentTable != null && parentField != null)
                        {
                            string text = Utilities.FormatTableText(parentTable.Name, parentTable.Alias);
                            text += ".";
                            text += Utilities.FormatFieldText(parentField.Name, parentField.Alias);
                            int maxLength = 15;
                            if (text.Length > maxLength)
                                text = text.Substring(0, maxLength) + "...";

                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Text = text;
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).ToolTip = ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = Utilities.FormatTableText(parentTable.Name, parentTable.Alias) + " - " + Utilities.FormatFieldText(parentField.Name, parentField.Alias);
                            ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).Cell.TitleMode = CellTitleMode.Always;
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Attributes.Add(Constants.ChildKey.ToString(), fieldKeyId.ToString());
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Attributes.Add(Constants.ParentKey.ToString(), parentField.ID.ToString());
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Enabled = false;
                        }
                        else
                        {
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Text = Resources.Resource.Choose_Label_Text;
                            ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Attributes.Add(Constants.ChildKey.ToString(), fieldKeyId.ToString());
                        }
                    }
                    catch
                    {
                        throw new Exception("The information in the DataView is not valid!. Contact your system admin ( DV Id=" + this.DataViewBO.DataViewManager.DataViewId.ToString() + ")");
                    }
                }
                else
                {
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Text = Resources.Resource.Choose_Label_Text;
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Attributes.Add(Constants.ChildKey.ToString(), fieldKeyId.ToString());
                }
                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).OnClientClick = "SetProgressLayerVisibility(true);";

                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                //LookUps Column
                if (fieldBOObject.LookupFieldId >= 0)
                {
                    TableBO lookupTable = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(fieldBOObject.LookupFieldId);
                    if (lookupTable != null)
                    {
                        FieldBO lookUpField = lookupTable.Fields.GetField(fieldBOObject.LookupFieldId);

                        string schemaText = lookupTable.DataBase;
                        string tableText = string.IsNullOrEmpty(lookupTable.Alias) ? lookupTable.Name : lookupTable.Name + "(" + lookupTable.Alias + ")";
                        string fieldText = string.IsNullOrEmpty(lookUpField.Alias) ? lookUpField.Name : lookUpField.Name + "(" + lookUpField.Alias + ")";
                        string fullText = schemaText + "." + tableText + "." + fieldText;
                        fullText = System.Web.HttpUtility.HtmlDecode(fullText);
                        fullText = System.Web.HttpUtility.HtmlDecode(fullText);
                        string trimmedText = fullText;
                        if (trimmedText.Length > 15)
                            trimmedText = trimmedText.Substring(0, 15) + "...";
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Text = trimmedText;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).ToolTip = ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = fullText;
                        ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).Cell.TitleMode = CellTitleMode.Always;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Enabled = false;
                    }
                    else
                    {
                        fieldBOObject.LookupDisplayFieldId = -1;
                        fieldBOObject.LookupFieldId = -1;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Text = Resources.Resource.Choose_Label_Text;
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Enabled = true;
                    }

                    ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).Cell.TitleMode = CellTitleMode.Always;

                }
                else
                {
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Text = Resources.Resource.Choose_Label_Text;
                    //((Button) ((CellItem) ((TemplatedColumn) this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Enabled = true;
                }
                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).OnClientClick = "SetProgressLayerVisibility(true);";
                ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Style.Add(HtmlTextWriterStyle.TextAlign, "left");

                //Disabling Primarykey,lookup & Relatioship if field is LOB type
                if (!String.IsNullOrEmpty(strDataTypeLob))
                {
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).FindControl(_parentColumnButtonID)).Enabled = false;
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).FindControl(_lookupButtonID)).Attributes.Add("disabled", "true");

                    ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = "Relationship " + Resources.Resource.LOB_DataType_Error_Text + " " + strDataTypeLob;
                    ((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[e.Row.Index].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[e.Row.Index]).Cell.Title = "Lookup " + Resources.Resource.LOB_DataType_Error_Text + " " + strDataTypeLob;
                }
            }
        }
        catch
        {

            throw;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Ocurrs when we want to render the grid (just once in the control life cicle)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TableFieldsUltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.TableFieldsUltraWebGrid.DisplayLayout.ViewType = Infragistics.WebUI.UltraWebGrid.ViewType.Flat;
        this.TableFieldsUltraWebGrid.DisplayLayout.AllowUpdateDefault = AllowUpdate.RowTemplateOnly;
        this.TableFieldsUltraWebGrid.DisplayLayout.AutoGenerateColumns = false;

        //Add the Radio Button Column
        e.Layout.Bands[0].Columns.Insert(0, ColumnKeys.Radio.ToString());
        e.Layout.Bands[0].Columns.FromKey(ColumnKeys.Radio.ToString()).Header.ClickAction = HeaderClickAction.Select;
        e.Layout.Bands[0].Columns.FromKey(ColumnKeys.Radio.ToString()).Header.Caption = Resources.Resource.PrimaryKey_ColHeader_Caption;
        e.Layout.Bands[0].Columns.FromKey(ColumnKeys.Radio.ToString()).Header.Fixed = true;

        //Add the ColumnOrder Col
        e.Layout.Bands[0].SortedColumns.Add(ColumnKeys.SortOrder.ToString());
        e.Layout.SelectTypeRowDefault = SelectType.Single;
        e.Layout.AllowSortingDefault = AllowSorting.OnClient;
        //e.Layout.AllowSortingDefault = AllowSorting.Yes;

        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.MimeType.ToString()))
        {
            UltraGridColumn MimeTypeCol = e.Layout.Bands[0].Columns.FromKey(ColumnKeys.MimeType.ToString());
            MimeTypeCol.ValueList = Utilities.GetMimeTypeValueList();
            MimeTypeCol.CellButtonDisplay = CellButtonDisplay.Always;
        }
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.IndexType.ToString()))
        {
            UltraGridColumn IndexTypeCol = e.Layout.Bands[0].Columns.FromKey(ColumnKeys.IndexType.ToString());
            IndexTypeCol.ValueList = IndexTypeValueList;
            IndexTypeCol.CellButtonDisplay = CellButtonDisplay.Always;
        }
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.ParentColumn.ToString()))
        {
            TemplatedColumn ParentColumnCol = (TemplatedColumn)e.Layout.Bands[0].Columns.FromKey(ColumnKeys.ParentColumn.ToString());
            ParentColumnCol.CellTemplate = new ParentColumnButton();
        }
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.Action.ToString()))
        {
            TemplatedColumn ParentColumnCol = (TemplatedColumn)e.Layout.Bands[0].Columns.FromKey(ColumnKeys.Action.ToString());
            ParentColumnCol.CellTemplate = new ActionButton();
        }
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.LookUpTable.ToString()))
        {
            TemplatedColumn LookupCol = (TemplatedColumn)e.Layout.Bands[0].Columns.FromKey(ColumnKeys.LookUpTable.ToString());
            LookupCol.CellTemplate = new LookupButton();
        }
        //JIRA ID : CBOE-481 Added column header for Query and Display fields. ASV 19032013
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.IsDefaultQuery.ToString()))
        {
            e.Layout.Bands[0].Columns.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Header.Title = Resources.Resource.ToolTip_IsDefaultQuery_Header;
        }
        if (e.Layout.Bands[0].Columns.Exists(ColumnKeys.IsDefault.ToString()))
        {
            e.Layout.Bands[0].Columns.FromKey(ColumnKeys.IsDefault.ToString()).Header.Title = Resources.Resource.ToolTip_IsDefault_Header;
        }
        //END CBOE-481
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Table Alias text box Value Change event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">ValueChangeEventArgs</param>
    void txtTableAlias_ValueChange(object sender, Infragistics.WebUI.WebDataInput.ValueChangeEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            string aliasValue = Convert.ToString(e.Value);
            if (string.IsNullOrEmpty(aliasValue.Trim()))
            {
                this.AliasTextBoxWithPopUp.Text = this.NameTextBoxWithPopUp.Text;
            }
            else if (aliasValue.Contains('"'))
            {
                e.Cancel = true;
            }
        }
        catch
        {
            throw;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods

    private void AddRowsMoving()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.TableFieldsUltraWebGrid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.SortOrder.ToString()).SortIndicator = SortIndicator.None;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Adds/Removes a Field in the current DataView.
    /// </summary>
    /// <param name="fieldId">Id of the field to add</param>
    private void ChangeFieldStatus(string fieldId, string CommandName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        //Find if it is a valid fieldId.
        int fieldIdTemp = int.MinValue;

        if (int.TryParse(fieldId, out fieldIdTemp))
        {
            // If this is remove action, to check the dependency of the field. 
            if (string.Compare(CommandName, "Remove", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                !CheckFieldDependency(fieldIdTemp))
            {
                return;
            }

            FieldBO field = this.DataViewBO.DataViewManager.Tables.GetField(fieldIdTemp);
            if (field != null)
            {
                RemoveInvalidEntryFromRelationship(CommandName, field.ID);
                field.FromMasterSchema = !field.FromMasterSchema;
                if (!field.FromMasterSchema)
                    field.Visible = true; //If the user wants to add that field, show it as visible (most common case).

                ChangeRelationShipsStatus(fieldId);
                this.BindTableDetails(this.DataViewBO.DataViewManager.Tables.GetTableIdByFieldId(field.ID));
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// To check if any other field is depending on this field as lookup or relationship.
    /// </summary>
    private bool CheckFieldDependency(int fieldId)
    {
        // The field path, schema -> table -> field.
        const string FieldPathPattern = "{0} -> {1} -> {2}";

        this.WarningControl.Visible = false;
        this.WarningControl.Text = string.Empty;

        // Check if this field is used for any lookup's join field or display field.
        foreach (var table in this.DataViewBO.DataViewManager.Tables)
        {
            foreach (var field in table.Fields)
            {
                // FromMasterSchema = true means this field is removed from dataview tempery, and it will be real removed when save to database.
                // we can ignore this field.
                if (field.FromMasterSchema)
                {
                    continue;
                }

                // Check if this field is used for any lookup's join field or display field.
                if (field.LookupFieldId == fieldId || field.LookupDisplayFieldId == fieldId)
                {
                    var lookupTable = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(fieldId);
                    var fieldPath = string.Empty;

                    if (lookupTable != null)
                    {
                        // Append the table name.
                        fieldPath = string.Format(FieldPathPattern, lookupTable.DataBase, lookupTable.Alias, field.Alias);
                    }

                    this.WarningControl.Text = string.Format(Resources.Resource.FieldDependencyCheckWarning, fieldPath);
                    this.WarningControl.Visible = true;

                    return false;
                }
            }
        }

        var relationships = this.DataViewBO.DataViewManager.Relationships;

        // Check if this field is used for any relationship's parent field.
        if (relationships != null && relationships.Count > 0)
        {
            foreach (var relation in relationships)
            {
                if (relation.ParentKey == fieldId)
                {
                    var childTable = this.DataViewBO.DataViewManager.Tables.GetTable(relation.Child);
                    if (childTable != null)
                    {
                        var childField = childTable.Fields.GetField(relation.ChildKey);

                        // FromMasterSchema = true means this field is removed from dataview tempery, and it will be real removed when save to database.
                        // we can ignore this field.
                        if (childField != null && childField.FromMasterSchema)
                        {
                            continue;
                        }

                        if (childField != null)
                        {
                            var fieldPath = string.Format(FieldPathPattern, childTable.DataBase, childTable.Alias, childField.Alias);
                            this.WarningControl.Text = string.Format(Resources.Resource.FieldDependencyCheckWarning, fieldPath);
                            this.WarningControl.Visible = true;
                        }
                    }

                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Remove the Entry from relationship
    /// </summary>
    /// <param name="CommandName">Parameter to  check if command is remove command</param>
    /// <param name="fieldID">field Id to delete from relationship list</param>
    private void RemoveInvalidEntryFromRelationship(string CommandName, int fieldID)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            if (fieldID != -1 && CommandName.ToString().Trim().ToLower().Equals("remove"))
            {
                List<RelationshipBO> rel = this.DataViewBO.DataViewManager.Relationships.GetByParentKeyOrChildKeyId(fieldID);
                if (rel != null)
                {
                    foreach (RelationshipBO item in rel)
                    {
                        this.DataViewBO.DataViewManager.Relationships.Remove(item.ParentKey, item.ChildKey);
                    }
                }
            }

        }
        catch
        {
            throw;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ChangeRelationShipsStatus(string fieldId)
    {
        List<RelationshipBO> relations = this.DataViewBO.DataViewManager.Relationships.GetByParentKeyOrChildKeyId(int.Parse(fieldId));
        foreach (RelationshipBO relation in relations)
        {
            relation.FromMasterSchema = !relation.FromMasterSchema;
        }
    }

    /// <summary>
    /// Go to the Define Relationships control and select the parent column
    /// </summary>
    /// <param name="parentCol">Button clicked inside the grid that contains</param>
    private void GoToParentColumnEdition(Button parentCol)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //CSBR-137648
        //Created session for storing the Alias text information
        Session["EditTablealias"] = AliasTextBoxWithPopUp.Text;
        //if (!string.IsNullOrEmpty(AliasTextBoxWithPopUp.Text))
        //    Session["EditTablealias"] = AliasTextBoxWithPopUp.Text;
        //else Bug
        //    Session["EditTablealias"] = NameTextBoxWithPopUp.Text;
        //End CSBR-137648
        string url = String.Empty;
        if (parentCol.Attributes[Constants.ChildKey] != null)
        {
            Session["Isfirst"] = true;
            url = "DefineRelationships.aspx?";
            url += Constants.ChildKey + "=" + parentCol.Attributes[Constants.ChildKey].ToString();
            if (parentCol.Attributes[Constants.ParentKey] != null)
                url += "&" + Constants.ParentKey + "=" + parentCol.Attributes[Constants.ParentKey].ToString();
            url += "&" + Constants.Action + "=EditParentColumn";
            url += "&" + Constants.Caller + "=EditTableAndFields";
            url += "&" + Constants.ParamCaller + "=" + this.SelectedTableId.ToString();
            url += "&schemaSelected=" + Request["schemaSelected"];
            Server.Transfer(url, true);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void EditLookUp(string fieldId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //CSBR-137648
        //Created session for storing the Alias text information
        Session["EditTablealias"] = AliasTextBoxWithPopUp.Text;
        //if (!string.IsNullOrEmpty(AliasTextBoxWithPopUp.Text))
        //    Session["EditTablealias"] = AliasTextBoxWithPopUp.Text;
        //else bug
        //    Session["EditTablealias"] = NameTextBoxWithPopUp.Text;
        //End CSBR-137648

        if (HasRightToLookup())
        {
            string url = String.Empty;
            if (!string.IsNullOrEmpty(fieldId))
            {
                Session["Isfirst"] = true;
                url = "DefineLookup.aspx?" + Constants.FieldId + "=" + fieldId;
                url += "&" + Constants.Caller + "=EditTableAndFields";
                url += "&" + Constants.ParamCaller + "=" + this.SelectedTableId.ToString();
                url += "&schemaSelected=" + Request["schemaSelected"];
                Server.Transfer(url, true);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        else
        {
            this.ErrorAreaUserControl.Text = Resources.Resource.Lookup_Label_Text;
            this.ErrorAreaUserControl.Visible = true;
        }
    }

    private bool HasRightToLookup()
    {
        var instanceSchema = Request["schemaSelected"];
        var instanceName = CambridgeSoft.COE.Framework.Common.Utilities.GetInstanceName(instanceSchema);

        // Gets the parent tableBO.
        var parentTableBO = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);

        foreach (var table in this.DataViewBO.DataViewManager.Tables)
        {
            // If there is lookup field, the leftOutter join will be published on Information link.
            // However information link does not support to self-join, otherwise validation error (identical fault exception) will be thrown.
            // So need to fiter out the tables has the same table name.
            if (table.Name == parentTableBO.Name && table.DataBase == parentTableBO.DataBase)
            {
                // ignore the table which created from same table source.
                continue;
            }

            // If any table come from the same instance, it can set the lookup.
            if (CambridgeSoft.COE.Framework.Common.Utilities.GetInstanceName(table.DataBase) == instanceName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Creates an Alias of a Field (selected in the grid).
    /// </summary>
    private void CreateAliasField()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //UnBind to keep changes in session var
        this.UnBindFieldsInfo();
        //Get selected field in grid
        int fieldId = this.GetSelectedFieldIdOnGrid();
        if (fieldId > int.MinValue)
        {
            TableBO table = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(fieldId);
            FieldBO addedField = table.Fields.CloneAndAddField(fieldId, _defaultAliasWord, ++this.DataViewBO.DataViewManager.Tables.HighestID);
            addedField.SortOrder = this.TableFieldsUltraWebGrid.Rows.Count;
            //Rebind to display new field
            this.BindTableDetails(this.SelectedTable);
            this.TableFieldsUltraWebGrid.Rows.Sort(true);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    private void CheckDefaultField()
    {
        for (int i = 0; i < this.TableFieldsUltraWebGrid.Rows.Count; i++)
        {
            if (((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[i]).FindControl(_isDefaultCheckBoxID)).Checked)
            {
                this.isChecked = true;
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    private int GetSelectedFieldIdOnGrid()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        if (this.TableFieldsUltraWebGrid.DisplayLayout.ActiveRow != null)
            retVal = Convert.ToInt32(this.TableFieldsUltraWebGrid.DisplayLayout.ActiveRow.Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString());
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    /// <summary>
    /// Unbinds the table alias into the given datasource
    /// </summary>
    /// <returns></returns>
    private TableBO UnBindTableInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Unbind table Details.
        TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
        if (table != null)
        {
            table.Alias = this.AliasTextBoxWithPopUp.Text;
            //if(!string.IsNullOrEmpty(this.AliasTextBoxWithPopUp.Text))
            //table.Alias = this.AliasTextBoxWithPopUp.Text;
            //else //bug
            //    table.Alias = this.NameTextBoxWithPopUp.Text;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return table;
    }

    /// <summary>
    /// Unbinds the Fields Grid into the given datasource
    /// </summary>
    private void UnBindFieldsInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
        if (table != null)
        {
            //Get the Selected Radio.
            int tempRowID = int.MinValue;
            if (int.TryParse(this.SelectedPKIndexHidden.Value.ToString(), out tempRowID))
            {
                FieldBO currentField = table.Fields.GetField(tempRowID);
                if (currentField != null)
                    table.PrimaryKey = currentField.ID;
            }
            for (int i = 0; i < this.TableFieldsUltraWebGrid.Rows.Count; i++)
            {
                int tempID = int.MinValue;
                string currentID = this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString();
                if (int.TryParse(currentID, out tempID))
                {
                    FieldBO currentField = table.Fields.GetField(tempID);
                    if (currentField != null && !currentField.FromMasterSchema)
                    {
                        //fields.GetField(tempID).Alias = Utilities.CleanString(this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Alias.ToString()).Value as string);
                        currentField.Alias = System.Web.HttpUtility.HtmlEncode(this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Alias.ToString()).Value as string).Trim();
                        //TODO: Try to discover a shorter way to find the Dropdown in the cell.
                        if (table.PrimaryKey == currentField.ID)
                            currentField.Visible = true;
                        else
                            currentField.Visible = ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Visible.ToString()).Column).CellItems[i]).FindControl(_visibleCheckBoxID)).Checked;
                        currentField.IsUniqueKey = ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsUniqueKey.ToString()).Column).CellItems[i]).FindControl(_isUniqueKeyCheckBoxID)).Checked;
                        currentField.IsDefaultQuery = ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Column).CellItems[i]).FindControl(_isDefaultQueryCheckBoxID)).Checked;
                        currentField.IsDefault = ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[i]).FindControl(_isDefaultCheckBoxID)).Checked;
                        currentField.IndexType = (COEDataView.IndexTypes)Enum.Parse(typeof(COEDataView.IndexTypes), this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IndexType.ToString()).Text);
                        currentField.MimeType = (COEDataView.MimeTypes)Enum.Parse(typeof(COEDataView.MimeTypes), this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.MimeType.ToString()).Text);

                        //SorOrder Hidden column.
                        int sortOrder = i;
                        if (((int)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.SortOrder.ToString()).Value) >= 0)
                            sortOrder = (int)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.SortOrder.ToString()).Value;
                        currentField.SortOrder = sortOrder;
                    }
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Save all the UserControls values in the given Datasource
    /// </summary>
    public void UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Unbind table Details.
        TableBO table = this.UnBindTableInfo();
        this.UnBindFieldsInfo();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Set all the controls values.
    /// </summary>
    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NameTitleLabel.Text = Resources.Resource.Name_Label_Text;
        this.AliasTitleLabel.Text = Resources.Resource.Alias_Label_Text;
        this.TableDetailTitle.Text = Resources.Resource.EditTable_Label_Text + " " + Resources.Resource.Alias_Label_Text;

        //Set all the columns text header.
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.Name.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Name.ToString()).Header.Caption = Resources.Resource.Name_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.Visible.ToString()))
            //this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Visible.ToString()).Header.Caption = Resources.Resource.Visible_ColHeader_Caption;
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Visible.ToString())).HeaderItem.FindControl("IsAllVisibleKeyCheckBox")).Text = " " + Resources.Resource.Visible_ColHeader_Caption;

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.IsUniqueKey.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsUniqueKey.ToString()).Header.Caption = Resources.Resource.IsUniqueKey_ColHeader_Caption;

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.IsDefaultQuery.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefaultQuery.ToString()).Header.Caption = Resources.Resource.IsDefaultQuery_ColHeader_Caption;

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.IsDefault.ToString()))
            //this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefault.ToString()).Header.Caption = Resources.Resource.IsDefault_ColHeader_Caption;
            ((CheckBox)((TemplatedColumn)this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IsDefault.ToString())).HeaderItem.FindControl("IsAllDefaultKeyCheckBox")).Text = " " + Resources.Resource.IsDefault_ColHeader_Caption;

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.Alias.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Alias.ToString()).Header.Caption = Resources.Resource.DisplayName_ColHeader_Caption;

        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.ParentColumn.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.ParentColumn.ToString()).Header.Caption = Resources.Resource.ParentColumn_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.DataType.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.DataType.ToString()).Header.Caption = Resources.Resource.DataType_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.IndexType.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.IndexType.ToString()).Header.Caption = Resources.Resource.IndexType_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.MimeType.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.MimeType.ToString()).Header.Caption = Resources.Resource.MimeType_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.Action.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.Action.ToString()).Header.Caption = Resources.Resource.Action_ColHeader_Caption;
        if (this.TableFieldsUltraWebGrid.Columns.Exists(ColumnKeys.LookUpTable.ToString()))
            this.TableFieldsUltraWebGrid.Columns.FromKey(ColumnKeys.LookUpTable.ToString()).Header.Caption = Resources.Resource.Lookup_ColHeader_Caption;

        this.ShowMasterSchemaFieldsHidden.Value = "0";
        this.CreateAliasFieldImageButton.ButtonText = this.CreateAliasFieldImageButton.ImageToolTip = Resources.Resource.CreateFieldAlias_Button_Text;
        this.CancelImageButton.ButtonText = this.CancelImageButton.ImageToolTip = Resources.Resource.Cancel_Button_Text;
        this.OkImageButton.ButtonText = this.OkImageButton.ImageToolTip = Resources.Resource.OK_Button_Text;
        this.ToggleButton.Text = Resources.Resource.ShowHiddenFields_Button_Text;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Databind the given datasource with the UserControl's controls. 
    /// </summary>
    /// <param name="dataViewBO">Datasource to bind controls</param>
    public void DataBind(COEDataViewBO dataViewBO, string selectedTableID)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int tempTableId = int.MinValue;
        if (dataViewBO != null)
        {
            this.SaveItAsSession(dataViewBO);
            if (int.TryParse(selectedTableID, out tempTableId))
                this.SelectedTable = "ID=" + tempTableId.ToString();

            this.BindTableDetails(this.SelectedTable);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }


    /// <summary>
    /// Binds the details table with the given datasource.
    /// </summary>
    /// <param name="dataKey">Datakey of the selected table to display</param>
    private void BindTableDetails(string dataKey)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(dataKey) && Utilities.IsActionNode(dataKey))
        {
            this.ShowTableDetails(true);
            this.CleanPreviousControlsValues();
            int id = Convert.ToInt32(Utilities.GetParamInDataKey(dataKey, Constants.DataKeysParam.ID));
            COEDataViewBO masterDataViewBO = COEDataViewBO.GetMasterSchema();
            TableBO selectedTable = this.DataViewBO.DataViewManager.Tables.GetTable(id);
            if (masterDataViewBO.DataViewManager.Tables.Count > 0)
                this.DataViewBO.DataViewManager.MergeTable(masterDataViewBO.DataViewManager.Tables, selectedTable, masterDataViewBO.DataViewManager.Relationships);

            if (selectedTable != null)
            {
                //set isView Property
                this.IsView = selectedTable.IsView;

                InitializeDeletedFieldCount(selectedTable);

                this.AliasTextBoxWithPopUp.Text = selectedTable.Alias;

                //if (!string.IsNullOrEmpty(selectedTable.Alias))
                //    this.AliasTextBoxWithPopUp.Text = selectedTable.Alias;
                //else bug
                //    this.AliasTextBoxWithPopUp.Text = this.NameTextBoxWithPopUp.Text;

                this.NameTextBoxWithPopUp.Text = selectedTable.Name;

                this.TableFieldsUltraWebGrid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Name.ToString()).SortIndicator = SortIndicator.Ascending;
                this.TableFieldsUltraWebGrid.DisplayLayout.Bands[0].SortedColumns.Add(ColumnKeys.Name.ToString());

                //Bottom Details table.
                foreach (FieldBO item in selectedTable.Fields)
                {
                    if (item != null)
                    {
                        if (item.LookupDisplayFieldId > 0)
                        {
                            TableBO tbl = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(item.LookupDisplayFieldId);
                            item.IsDefault = false;
                            if (tbl != null)
                            {
                                FieldBO fld = tbl.Fields.GetField(item.LookupDisplayFieldId);
                                if (fld != null)
                                {
                                    fld.IsDefault = true;
                                    item.IsDefault = true;
                                }
                            }
                        }
                    }
                }

                this.TableFieldsUltraWebGrid.DataSource = selectedTable.Fields;
                
                errorMsg = string.Empty;
                this.TableFieldsUltraWebGrid.DataBind();
                
                // display error message if exits error
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    this.ErrorAreaUserControl.Text = errorMsg;
                    this.ErrorAreaUserControl.Visible = true;
                }

                //After databind we create the radio controls for the PK.
                this.InitRadioButtons(selectedTable.PrimaryKey.ToString());
            }
        }
        else
        {
            this.ShowTableDetails(false);
        }
        this.ToggleButton.Enabled = (this.DeletedFieldCount != 0);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Binds the details table with the given datasource.
    /// </summary>
    /// <param name="dataKey">Datakey of the selected table to display</param>
    private void BindTableDetails(int tableId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ShowTableDetails(true);
        this.CleanPreviousControlsValues();
        TableBO selectedTable = this.DataViewBO.DataViewManager.Tables.GetTable(tableId);
        if (selectedTable != null)
        {
            //set isView Property
            this.IsView = selectedTable.IsView;

            InitializeDeletedFieldCount(selectedTable);

            this.AliasTextBoxWithPopUp.Text = selectedTable.Alias;
            //if (!string.IsNullOrEmpty(selectedTable.Alias))
            //    this.AliasTextBoxWithPopUp.Text = selectedTable.Alias;
            //else bug
            //    this.AliasTextBoxWithPopUp.Text = this.NameTextBoxWithPopUp.Text;

            this.NameTextBoxWithPopUp.Text = selectedTable.Name;

            this.TableFieldsUltraWebGrid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Name.ToString()).SortIndicator = SortIndicator.Ascending;
            this.TableFieldsUltraWebGrid.DisplayLayout.Bands[0].SortedColumns.Add(ColumnKeys.Name.ToString());

            //Bottom Details table.

            this.TableFieldsUltraWebGrid.DataSource = selectedTable.Fields;
            this.TableFieldsUltraWebGrid.DataBind();
            //After databind we create the radio controls for the PK.
            this.InitRadioButtons(selectedTable.PrimaryKey.ToString());
        }
        else
        {
            this.ShowTableDetails(false);
        }
        this.ToggleButton.Enabled = (this.DeletedFieldCount != 0);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void InitializeDeletedFieldCount(TableBO selectedTable)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int deletedCount = 0;
        foreach (FieldBO field in selectedTable.Fields)
        {
            if (field.FromMasterSchema)
                deletedCount++;
        }
        this.DeletedFieldCount = deletedCount;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Clear previous alias, etc values (In case one table hasn't this already set)
    /// </summary>
    private void CleanPreviousControlsValues()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AliasTextBoxWithPopUp.Text = this.NameTextBoxWithPopUp.Text = String.Empty;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Insert the Radio Button and mark as selected the given PK
    /// </summary>
    /// <param name="PK">Radio ID to mark as selected</param>
    private void InitRadioButtons(string PK)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.TableFieldsUltraWebGrid.Rows.Count > 0)
        {
            //Add the HTML to create the radio button and connect the onclick event to our jscript
            for (int i = 0; i < this.TableFieldsUltraWebGrid.Rows.Count; i++)
            {
                string showSelected = String.Empty;
                //string currentID = this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey("ID").Value.ToString();

                //CSBR 135133
                #region Parent Checking

                bool HasParent = false;
                //parent column check
                int fieldKeyId = (int)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value;
                List<RelationshipBO> relationships = this.DataViewBO.DataViewManager.Relationships.GetByChildKey(fieldKeyId);
                //Assume is the first element.
                if (relationships.Count > 0 && !relationships[0].FromMasterSchema)
                {
                    TableBO parentTable = null;
                    FieldBO parentField = null;
                    //TODO: Check is this try/catch can be removed in the future. This code is here because we are traying to keep working on relationships validation and don't stop at this step.
                    try
                    {
                        parentTable = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(relationships[0].ParentKey);
                        parentField = this.DataViewBO.DataViewManager.Tables.GetField(relationships[0].ParentKey);

                        if (parentTable != null && parentField != null)
                            HasParent = true;

                    }
                    catch { }
                }

                #endregion
                if (this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString() == PK)
                {
                    showSelected = " checked='true' ";
                    //if (!IsFromRadioEvent)
                    this.SelectedPKIndexHidden.Value = i.ToString();
                    //updating the previous pk field id and its lookup button status
                    this.PrevPkFieldIdHidden.Value = this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString();
                    this.PrevLookupDisabledHidden.Value = HasParent.ToString().ToLower();
                    ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[i]).FindControl(_lookupButtonID)).Enabled = false;

                    // Default field for primary key will be checked only when user required.
                    //((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[i]).FindControl(_isDefaultCheckBoxID)).Checked = true;
                    this.PrevVisibleFieldHidden.Value = this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Visible.ToString()).Value.ToString();
                }
                else
                {
                    if (HasParent)
                    {
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[i]).FindControl(_lookupButtonID)).Enabled = false;
                        ((CheckBox)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.IsDefault.ToString()).Column).CellItems[i]).FindControl(_isDefaultCheckBoxID)).Checked = true;
                    }
                    else
                    {
                        ((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.LookUpTable.ToString()).Column).CellItems[i]).FindControl(_lookupButtonID)).Enabled = true;
                    }
                }
                if ((((Button)((CellItem)((TemplatedColumn)this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ParentColumn.ToString()).Column).CellItems[i]).FindControl(_parentColumnButtonID)).Enabled == false)
                    || (this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.DataType.ToString()).Value.ToString().ToUpper().Contains("LOB")))
                {
                    if (this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.DataType.ToString()).Value.ToString().ToUpper().Contains("LOB"))
                        this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Radio.ToString()).Title = this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Radio.ToString()).Column.Header.Caption
                            + " " + Resources.Resource.LOB_DataType_Error_Text + this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.DataType.ToString()).Value.ToString();

                    //disable the pk radio button, if the field already having the lookup(checking parent disable status)
                    //showSelected = " checked='false' ";
                    this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Radio.ToString()).Value =
                        @"<input type=radio name='TheRadioGroup' onclick='activateGridRow(" +
                        this.TableFieldsUltraWebGrid.Rows[i].Index.ToString() +
                        "," +
                        this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString() +
                        ");' " + showSelected + " disabled='true' >";
                }
                else
                {
                    this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.Radio.ToString()).Value =
                        @"<input type=radio name='TheRadioGroup' onclick='activateGridRow(" +
                        this.TableFieldsUltraWebGrid.Rows[i].Index.ToString() +
                        "," +
                        this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).Value.ToString() +
                        ");' " + showSelected +
                        (this.TableFieldsUltraWebGrid.Rows[i].Cells.FromKey(ColumnKeys.ID.ToString()).AllowEditing == AllowEditing.No ? " disabled='true' >" : ">");
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Shows as visible or not the Details table given a status 
    /// </summary>
    /// <param name="status">Status of the details table</param>
    private void ShowTableDetails(bool status)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ShowDetailsTable.Visible = status;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Saves in Session the given DataViewBO
    /// </summary>
    /// <param name="dataViewBO">Dataviewbo to save as session</param>
    private void SaveItAsSession(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Session[Constants.COEDataViewBO] == null && dataViewBO != null)
            Session[Constants.COEDataViewBO] = dataViewBO;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// This will revert back all the changes made
    /// </summary>
    private void RevertEdition()
    {
        COEDataViewManagerBO Dvm = this.DataViewBO.DataViewManager;
        TableBO table = Dvm.Tables.GetTable(this.SelectedTableId);
        TableBO freshtable = (TableBO)Session["FreshTable"];
        List<RelationshipBO> freshRels = (List<RelationshipBO>)Session["FreshRels"];

        // Jira Id - CBOE-406, Session["FreshTable"] is initialized on Edit table link click only, after pressing back button twice,
        // it becomes null, so  DummyFreshTable seesion is used
        if (freshtable == null && Session["DummyFreshTable"] != null)
        {
            //Coverity Fix : CID 19967
            TableBO tblBO = Session["DummyFreshTable"] as TableBO;
            if (tblBO != null)
                freshtable = tblBO.Clone();
            if (Session["DummyFreshRels"] != null)
                freshRels = (List<RelationshipBO>)Session["DummyFreshRels"];
        }

        if (freshtable != null)
        {
            if (IsDuplicateTableAlias())
                table.Alias = freshtable.Alias;

            int index = Dvm.Tables.IndexOf(table);
            if (freshRels != null && freshRels.Count > 0)
            {
                foreach (RelationshipBO rel in Dvm.Relationships.GetByChildId(this.SelectedTableId))
                {
                    Dvm.Relationships.Remove(rel.Parent, rel.ParentKey, rel.Child, rel.ChildKey);
                }
            }
            Dvm.Tables.Remove(table.ID);

            Dvm.Tables.Insert(index, freshtable); //keep sort
            if (freshRels != null && freshRels.Count > 0)
            {
                foreach (RelationshipBO rel in freshRels)
                {
                    Dvm.Relationships.Add(rel);
                }
            }
        }
    }

    /// <summary>
    /// Gets configuration vlaue that enables or disables the offer to index fields on Default Query field check
    /// </summary>
    private void GetApplyIndexingConfiguration()
    {
        string strApplyIndexing = FrameworkUtils.GetAppConfigSetting(ConfigurationManager.AppSettings["ConfigurationName"], "DVManager", "Apply_Indexing", true);
        if (strApplyIndexing.ToUpperInvariant().Trim() == "ENABLE")
            Apply_Indexing = true;
        else
            Apply_Indexing = false;
    }

    /// <summary>
    /// Gets true if selected primary key is valid
    /// </summary>
    private void CheckPrimaryKeyField()
    {
        this.isPrimaryKeyValid = true;
        COEDataViewBO theCOEDataViewBO = this.DataViewBO;
        if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables != null)
        {
            TableBO table = theCOEDataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
            if (table != null)
            {
                // if there is no priamry key, then there is no need to check whether primary key is valid or not
                if (table.PrimaryKey != 0)
                {
                    List<string> lstInvalidPrimaryKeyFields = theCOEDataViewBO.GetInvalidPrimaryKeyFields(table.DataBase, table.Name);

                    for (int i = 0; i < table.Fields.Count; i++)
                    {
                        FieldBO currentField = table.Fields[i];
                        if (currentField != null && !currentField.FromMasterSchema)
                        {
                            if (table.PrimaryKey == currentField.ID)
                            {
                                // if the content type of primary key is changed from the UI or the primary key is of type date / CLOB/ BLOB
                                if (currentField.MimeType.ToString().ToUpperInvariant() != "NONE" || lstInvalidPrimaryKeyFields.Contains(currentField.Name))
                                {
                                    isPrimaryKeyValid = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Checks whether Table alias name is duplicate or not
    /// </summary>
    /// <returns></returns>
    private bool IsDuplicateTableAlias()
    {
        bool isDuplicate = false;
        COEDataViewBO theCOEDataViewBO = this.DataViewBO;

        if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null)
        {
            TableListBO theTableListBO = theCOEDataViewBO.DataViewManager.Tables;
            TableBO table = theCOEDataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);

            if (theTableListBO != null)
            {
                for (int i = 0; i < theTableListBO.Count; i++)
                {
                    // CBOE-709, Database condition added if Edit Master menu is selected
                    if (theTableListBO[i].ID != this.SelectedTableId && theTableListBO[i].DataBase == table.DataBase && theTableListBO[i].Alias.Trim() == AliasTextBoxWithPopUp.Text.Trim())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
        }

        return isDuplicate;
    }

    /// <summary>
    /// Checks whether all Visible/Default fields are checked or not
    /// </summary>
    /// <returns></returns>
    private bool IsAllFieldValuesChecked(string strColumnKey)
    {
        bool blnAllChecked = true;
        COEDataViewBO theCOEDataViewBO = this.DataViewBO;
        if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables != null)
        {
            TableBO table = theCOEDataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
            if (table != null)
            {
                switch (strColumnKey)
                {
                    case "Visible":
                        for (int i = 0; i < table.Fields.Count; i++)
                        {
                            if (!table.Fields[i].Visible)
                            {
                                blnAllChecked = false;
                                break;
                            }
                        }
                        break;
                    case "IsDefault":
                        for (int i = 0; i < table.Fields.Count; i++)
                        {
                            if (!table.Fields[i].IsDefault)
                            {
                                blnAllChecked = false;
                                break;
                            }
                        }
                        break;
                }
            }
        }
        else
            blnAllChecked = false;
        return blnAllChecked;
    }

    /// <summary>
    /// Validates on sumbitting Ok button 
    /// </summary>
    /// <returns>string: Error message</returns>
    private string ValidateOk()
    {
        //JiraID: CBOE-767 DVM Improvement
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string errorMsg = string.Empty;

        // Check Default column is checked or not
        this.CheckDefaultField();
        if (!isChecked)
            return Resources.Resource.SelectDefaultField_Label_Text;

        //Check duplicate rable alias name 
        if (IsDuplicateTableAlias())
            return Resources.Resource.DuplicateTableAlias;

        // Check valid primary key for selected table
        PkUkErrorStatus thePkUkErrorStatus = this.CheckValidPrimaryKey();
        if (thePkUkErrorStatus != PkUkErrorStatus.ValidPkUkField)
        {
            switch (thePkUkErrorStatus)
            {
                case PkUkErrorStatus.SelectPkUkForBaseTable: errorMsg = Resources.Resource.SelectPkForBaseTable;
                    Session["IsWarningPkUkShown"] = null;
                    WarningControl.Visible = false;
                    WarningControl.Text = string.Empty;
                    break;
                case PkUkErrorStatus.InvalidPkUkForBaseTable: errorMsg = Resources.Resource.Invalid_PrimaryKey;
                    Session["IsWarningPkUkShown"] = null;
                    WarningControl.Visible = false;
                    WarningControl.Text = string.Empty;
                    break;
                case PkUkErrorStatus.SelectedBaseTblHasNoValidPkUk:
                case PkUkErrorStatus.ChildTableNoPkUkConstraint:
                default:
                    // Table voilets the Legit Pk/Uk rule as well as NOTNULL string/Number field not found for selected table.
                    // If this is set then user allowed to save DV by showing warning meesgae.
                    if (string.IsNullOrEmpty(PkUkList.Value))
                    {
                        // Check selected primary key field's datatype is not BLOB/CLOB/DATE/TIME.
                        this.CheckPrimaryKeyField();
                        if (!isPrimaryKeyValid)
                            return Resources.Resource.InvalidPkDataType;

                        // Set the warning message
                        errorMsg = string.Empty;
                        if (Session["IsWarningPkUkShown"] == null)
                            Session["IsWarningPkUkShown"] = true;
                        else
                        {
                            Session["IsWarningPkUkShown"] = null;
                            WarningControl.Visible = false;
                            WarningControl.Text = string.Empty;
                        }
                    }
                    break;
            }
            return errorMsg;
        }

        // Check selected primary key field's datatype is not BLOB/CLOB/DATE/TIME.
        this.CheckPrimaryKeyField();
        if (!isPrimaryKeyValid)
            return Resources.Resource.InvalidPkDataType;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        return string.Empty;
    }
    #endregion

    #region Create Index Methods
    public string CreateIndex(string strFieldID)
    {
        string result = "Index not created";
        try
        {
            //Find if it is a valid fieldId.
            int fieldIdTemp = int.MinValue;
            if (int.TryParse(strFieldID, out fieldIdTemp))
            {
                TableBO table = this.DataViewBO.DataViewManager.Tables.GetTableByFieldId(fieldIdTemp);
                if (table != null)
                {
                    FieldBO field = this.DataViewBO.DataViewManager.Tables.GetField(fieldIdTemp);
                    if (field != null)
                        if (DataViewBO.CreateIndex(table.DataBase, table.Name, field.Name))
                        {
                            field.IsIndexed = true;
                            result = "";
                            this.BindTableDetails(table.ID);
                        }
                }
            }
        }
        catch
        {
            result = "Index not created";
        }
        //set index on selected field
        return result;
    }

    string result = string.Empty;
    public string GetCallbackResult()
    {
        return result;
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument.StartsWith("createindex"))
        {
            result = this.CreateIndex(eventArgument.Replace("createindex:", string.Empty));
        }
    }
    #endregion

    #region Method : Primary Key
    /// <summary>
    ///  Method used to check the valid primary key.    
    /// </summary>
    /// <returns>Returns : 
    ///  1: Select Primary key for base table. 
    ///  2: Invalid primary key selected. 
    ///  3: Selected base table does not have Primary/Unique key constraint.
    ///  4: Child Table does not have Primary/Unique key constraint.
    ///  -1: No Change.Valid Pk/Uk
    /// </returns>
    private PkUkErrorStatus CheckValidPrimaryKey()
    {
        string strPrevPrimaryKey = GetPrevPkUkFromPkID();
        PkUkErrorStatus thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
        try
        {

            string[] NotNullColumnArray = PkUkNotNullList.Value.Split(',');
            string[] PkUkArray = PkUkList.Value.Split(',');
            bool isBaseTable = IsBaseTable();
            if (string.IsNullOrEmpty(strPrevPrimaryKey) && isBaseTable)
            {
                // Select PK/Uk for the table 
                thePkUkErrorStatus = PkUkErrorStatus.SelectPkUkForBaseTable;
            }
            else if (!string.IsNullOrEmpty(strPrevPrimaryKey) && isBaseTable)
            {
                //Check for valid PK/Uk from Legit primary list
                if (PkUkList.Value.Length > 0)
                {
                    foreach (string item in PkUkArray)
                    {
                        if (item.ToUpperInvariant().Equals(strPrevPrimaryKey.ToUpperInvariant()))
                        {
                            thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
                            break;
                        }
                        else
                            thePkUkErrorStatus = PkUkErrorStatus.InvalidPkUkForBaseTable;
                    }
                }
                else if (PkUkNotNullList.Value.Length > 0)  //Check for valid PK/Uk from NotNull string/Number primary list
                {
                    foreach (string item in NotNullColumnArray)
                    {
                        if (item.ToUpperInvariant().Equals(strPrevPrimaryKey.ToUpperInvariant()))
                        {
                            thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
                            break;
                        }
                        else
                            thePkUkErrorStatus = PkUkErrorStatus.InvalidPkUkForBaseTable;
                    }
                }
                else
                    thePkUkErrorStatus = PkUkErrorStatus.SelectedBaseTblHasNoValidPkUk;
            }
            else if (string.IsNullOrEmpty(strPrevPrimaryKey) && !isBaseTable)
            {
                // No message.Accept that child table do not have PK/UK
                thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
            }
            else if (!string.IsNullOrEmpty(strPrevPrimaryKey) && !isBaseTable)
            {
                //Check for valid PK/Uk from Legit primary list
                if (PkUkList.Value.Length > 0)
                {
                    foreach (string item in PkUkArray)
                    {
                        if (item.ToUpperInvariant().Equals(strPrevPrimaryKey.ToUpperInvariant()))
                        {
                            thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
                            break;
                        }
                        else
                            thePkUkErrorStatus = PkUkErrorStatus.InvalidPkUkForBaseTable;
                    }
                }
                else if (PkUkNotNullList.Value.Length > 0)       //Check for valid PK/Uk from NotNull string/Number primary list
                {
                    foreach (string item in NotNullColumnArray)
                    {
                        if (item.ToUpperInvariant().Equals(strPrevPrimaryKey.ToUpperInvariant()))
                        {
                            thePkUkErrorStatus = PkUkErrorStatus.ValidPkUkField;
                            break;
                        }
                        else
                            thePkUkErrorStatus = PkUkErrorStatus.InvalidPkUkForBaseTable;
                    }
                }
                else
                    thePkUkErrorStatus = PkUkErrorStatus.ChildTableNoPkUkConstraint;
            }
        }
        catch
        {
            throw;
        }
        return thePkUkErrorStatus;
    }

    /// <summary>
    ///  Methods to get whether is base table or child table
    /// </summary>
    /// <returns>
    /// True: for base table. False: for child table.</returns>
    private bool IsBaseTable()
    {
        bool isBaseTable = false;
        try
        {
            COEDataViewBO theCOEDataViewBO = this.DataViewBO;
            if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables.Count > 0 && this.SelectedTableId != -1)
            {
                TableBO theTableBO = theCOEDataViewBO.DataViewManager.Tables.GetTable(SelectedTableId);
                if (theTableBO != null)
                {
                    if (theCOEDataViewBO.DataViewManager.BaseTableId != theTableBO.ID)
                        isBaseTable = false;
                    else
                        isBaseTable = true;
                }
            }
        }
        catch
        {
            throw;
        }
        return isBaseTable;
    }

    /// <summary>
    /// Method returns the name of primary/unique key field for hiddenfield PrevPkFieldIdHidden.
    /// </summary>
    /// <returns>Name of Pk/Uk field </returns>
    private string GetPrevPkUkFromPkID()
    {
        string prevPkField = string.Empty;
        COEDataViewBO theCOEDataViewBO = this.DataViewBO;
        if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables.Count > 0 && this.SelectedTableId != -1)
        {
            TableBO theTableBO = theCOEDataViewBO.DataViewManager.Tables.GetTable(SelectedTableId);
            if (theTableBO != null)
            {
                int intPrevPk = PrevPkFieldIdHidden.Value.Length > 0 ? Convert.ToInt32(PrevPkFieldIdHidden.Value) : -1;

                foreach (FieldBO item in theTableBO.Fields)
                {
                    if (item.ID == intPrevPk)
                    {
                        prevPkField = item.Name;
                        break;
                    }
                }
            }
        }
        return prevPkField;
    }

    //Bug Fixing : CBOE-242
    /// <summary>
    /// Method sets the dataview's contraint on hidden value.
    /// </summary>
    internal void GetPrimaryUniqueKeyConstraintInfo()
    {
        try
        {
            COEDataViewBO theCOEDataViewBO = this.DataViewBO;
            if (theCOEDataViewBO != null && theCOEDataViewBO.DataViewManager != null && theCOEDataViewBO.DataViewManager.Tables.Count > 0 && this.SelectedTableId != -1)
            {
                TableBO theTableBO = theCOEDataViewBO.DataViewManager.Tables.GetTable(SelectedTableId);
                if (theTableBO != null)
                {
                    StringBuilder myarray = new StringBuilder();
                    DataTable dt = theCOEDataViewBO.DataViewManager.GetPrimaryUniqueKeyConstraintInfo(theTableBO.DataBase, theTableBO.Name);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            myarray.Append(dRow["Column_Name"].ToString() + ",");
                        }
                        PkUkList.Value = myarray.ToString().Remove(myarray.ToString().Length - 1);
                        dt.Dispose();
                    }
                    //JiraID: CBOE-767 DVM Improvement
                    myarray.Clear();
                    dt = theCOEDataViewBO.DataViewManager.GetPrimaryKeyFieldNotNullCols(theTableBO.DataBase, theTableBO.Name);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            myarray.Append(dRow["Column_Name"].ToString() + ",");
                        }
                        PkUkNotNullList.Value = myarray.ToString().Remove(myarray.ToString().Length - 1);
                        dt.Dispose();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    //end of CBOE-242
    #endregion
}

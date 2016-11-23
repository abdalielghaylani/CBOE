using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;
using System.Data;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.WebCombo;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
using CambridgeSoft.COE.Framework.Caching;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COETableManager", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COETableManager
{
    /// <summary>
    /// A server control that is used to browse, add, edit and delete records of tables, 
    /// which are categorized by application are configurable in terms of table, column, column validation etc.. 
    /// It supports FK-PK translation and ChemDraw. 
    /// </summary>
    [ToolboxData("<{0}:COETableManager runat=server Width=95%></{0}:COETableManager>")]
    public class COETableManager : CompositeControl, INamingContainer
    {
        #region Enums Defined
        /// <summary>
        /// Enumeration for serving the tracking of Currently selected Operation Mode.
        /// </summary>
        public enum ModeOfAction
        {
            Default, Add, Update
        }
        /// <summary>
        /// Enumeration for serving the tracking of Currently showing table Mode.
        /// </summary>
        public enum ModeOfShowTable
        {
            Single,
            Multi
        }
        /// <summary>
        /// Enumeration to validate SQL Query.
        /// </summary>
        public enum ValidateSqlQuery
        {
            VW_PICKLISTDOMAIN
        }

        /// <summary>
        /// Enumeration to disable table based on enums.
        /// Note : Should go in to framework config ??
        /// </summary>
        public enum LockingColumns
        {
            LOCKED,
        }

        /// <summary>
        /// Enumeration to ignore disable columns based on enums.
        /// Note : Should go in to framework config ??
        /// </summary>
        public enum ColumnsToIgnoreLock
        {
            DESCRIPTION, EXT_SQL_SORTORDER,
        }
        #endregion

        #region Constants

        public const string RequiredField = "requiredField";
        public const string PositiveInteger = "positiveInteger";
        public const string MolWeight = "chemicallyValid";
        public const string NumericRange = "numericRange";
        public const string PositiveNumber = "positiveNumber";
        public const string TextLength = "textLength";
        public const string Custom = "custom";
        public const string Validator_Min = "min";
        public const string Validator_Max = "max";
        char[] separator = { '\"', '\'', '@', ' ', ',', '.', '|', '!', '\\', '%', '&', '/', '(', ')', '=', '?', '*', '-', '+', ';', ':', '<', '>', ';' };
        public const string ChemdrawNullValue = @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDExLjAI
ABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABmhkgAAMAnADOTVADMzEcAAQkIAAAA
AAAAAAAAAgkIAAAAhwAAAIcADQgBAAEIBwEAAToEAQABOwQBAABFBAEAATwEAQAA
DAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQA
AADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQI
AgB4AAMIBAAAAHgAIwgBAAUMCAEAAAIIEAAAACQAAAAkAAAAJAAAACQAAAMyAAgA
////////AAAAAAAA//8AAAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//
AAEkAAAAAgADAOQEBQBBcmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGABQAAAAQC
EAAAAAAAAAAAAAAA0AIAABwCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIA
AQAGgAIAAAAAAggAAIBSAADAJwAEAhAAZoZIAADAJwAzk1QAzMxHAAoAAgABAAED
BAADAAAAEAAmAAAAQ2hlbURyYXcgY2FuJ3QgaW50ZXJwcmV0IHRoaXMgbGFiZWwu
AgcCAAEAAAcQAAEAAAAEAAAA8AADAE5VTEwAAAAAAAAAAA==";
        private const string NullOutput = "null";
        private const string Changeinlinejs = "changeInlines";
        private const int TableEditorGridWidth = 950;
        private COETableEditorBO _businessObj;

        #endregion

        #region variables

        bool _includeStructure = false;
        bool _processPaging = false;

        bool _hasValidationError = false;
        bool _renderMaxPageSizeAlert = false;

        bool ClickRefreshInfoVisible = false;

        private struct ChemDrawClientInfo
        {
            public bool isRequiredField;
            public bool isMolWeight;
            public string strRequiredFieldErrorMessage;
            public string strMolWeightErrorMessage;
        }

        int _columnsWidth = 1; // Set to 1 to avoid scrollbars
        #endregion

        #region Public Properties
        /// <summary>
        /// Application Name that has been selected.
        /// </summary>
        public string AppName
        {
            get
            {
                //the appName should come from the ini file or set by the calling applicaton in csla globalcontext["AppName"]
                string appName = COEAppName.Get();
                if (ViewState["AppName"] != null)
                    appName = (string)ViewState["AppName"];
                return appName;
            }

            set
            {
                ViewState["AppName"] = value;
            }
        }
        /// <summary>
        /// Property to keep track of the page size 
        /// </summary>
        public int PageSize
        {
            get
            {
                if (ViewState["PageSize"] == null)
                    return 10;
                else
                    return (int)ViewState["PageSize"];
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        #region the properties of Controls style
        public string UltraWebGridHeaderStyle
        {
            get
            {
                if (ViewState["TableEditorHeaderStyle"] == null)
                    return "TableEditorHeaderStyle";
                else
                    return (string)ViewState["TableEditorHeaderStyle"];
            }
            set
            {
                ViewState["HeaderStyle"] = value;
            }
        }
        public string UltraWebGridFrameStyle
        {
            get
            {
                if (ViewState["FrameStyle"] == null)
                    return "FrameStyle";
                else
                    return (string)ViewState["FrameStyle"];
            }
            set
            {
                ViewState["FrameStyle"] = value;
            }
        }
        public string UltraWebGridRowStyle
        {
            get
            {
                if (ViewState["RowStyle"] == null)
                    return "RowStyle";
                else
                    return (string)ViewState["RowStyle"];
            }
            set
            {
                ViewState["RowStyle"] = value;
            }
        }
        public string PanelEditBtnStyle
        {
            get
            {
                if (ViewState["pnlEditBtnStyle"] == null)
                    return "pnlEditBtnStyle";
                else
                    return (string)ViewState["pnlEditBtnStyle"];
            }
            set
            {
                ViewState["pnlEditBtnStyle"] = value;
            }
        }
        public string PanelEditRecordStyle
        {
            get
            {
                if (ViewState["pnlEditRecordStyle"] == null)
                    return "pnlEditRecordStyle";
                else
                    return (string)ViewState["pnlEditRecordStyle"];
            }
            set
            {
                ViewState["pnlEditRecordStyle"] = value;
            }
        }
        public string PanelSetPageStyle
        {
            get
            {
                if (ViewState["pnlPageStyle"] == null)
                    return "pnlPageStyle";
                else
                    return (string)ViewState["pnlPageStyle"];
            }
            set
            {
                ViewState["pnlPageStyle"] = value;
            }
        }
        public string PanelTableDescriptionStyle
        {
            get
            {
                if (ViewState["pnlTableDescriptionStyle"] == null)
                    return "pnlTableDescriptionStyle";
                else
                    return (string)ViewState["pnlTableDescriptionStyle"];
            }
            set
            {
                ViewState["pnlTableDescriptionStyle"] = value;
            }
        }
        public string PanelViewTableStyle
        {
            get
            {
                if (ViewState["pnlViewTableStyle"] == null)
                    return "pnlViewTableStyle";
                else
                    return (string)ViewState["pnlViewTableStyle"];
            }
            set
            {
                ViewState["pnlViewTableStyle"] = value;
            }
        }

        public string WebGridRowStyle
        {
            get
            {
                if (ViewState["RowStyle"] == null)
                    return "RowStyle";
                else
                    return (string)ViewState["RowStyle"];
            }
            set
            {
                ViewState["RowStyle"] = value;
            }
        }

        public string ButtonStyle
        {
            get
            {
                if (ViewState["ToolbarButton"] == null)
                    return "ToolbarButton";
                else
                    return (string)ViewState["ToolbarButton"];
            }
            set
            {
                ViewState["ToolbarButton"] = value;
            }
        }

        public string GridButtonStyle
        {
            get
            {
                if (ViewState["GridToolbarButton"] == null)
                    return "GridToolbarButton";
                else
                    return (string)ViewState["GridToolbarButton"];
            }
            set
            {
                ViewState["GridToolbarButton"] = value;
            }
        }

        public string ChemdrawStyle
        {
            get
            {
                if (ViewState["COEChemDraw1"] == null)
                    return "COEChemDraw1";
                else
                    return (string)ViewState["COEChemDraw1"];
            }
            set
            {
                ViewState["COEChemDraw1"] = value;
            }
        }

        public string TableEditorListBoxStyle
        {
            get
            {
                if (ViewState["TableEditorListBox"] == null)
                    return "TableEditorListBox";
                else
                    return (string)ViewState["TableEditorListBox"];
            }
            set
            {
                ViewState["TableEditorListBox"] = value;
            }
        }

        public string GridDeleteButtonStyle
        {
            get
            {
                if (ViewState["GridToolbarDeleteButton"] == null)
                    return "GridToolbarDeleteButton";
                else
                    return (string)ViewState["GridToolbarDeleteButton"];
            }
            set
            {
                ViewState["GridToolbarDeleteButton"] = value;
            }
        }

        public string GridEditButtonStyle
        {
            get
            {
                if (ViewState["GridToolbarEditButton"] == null)
                    return "GridToolbarEditButton";
                else
                    return (string)ViewState["GridToolbarEditButton"];
            }
            set
            {
                ViewState["GridToolbarEditButton"] = value;
            }
        }

        public string GridSortField
        {
            get
            {
                if (ViewState["GridSortField"] != null)
                    return ViewState["GridSortField"].ToString();
                else
                    return "";
            }
            set
            {
                ViewState["GridSortField"] = value;
            }
        }

        public SortIndicator GridSortIndicator
        {
            get
            {
                if (ViewState["GridSortIndicator"] != null)
                    return (SortIndicator)ViewState["GridSortIndicator"];
                else
                    return SortIndicator.None;
            }
            set
            {
                ViewState["GridSortIndicator"] = value;
            }
        }

        #endregion



        #endregion

        #region Private Properties
        /// <summary>
        /// Property for displaying child table
        /// </summary>
        public ModeOfShowTable CurrentShowTableMode
        {
            get
            {
                ModeOfShowTable mode;
                if (ViewState["CurrentShowTableMode"] != null)
                    mode = (ModeOfShowTable)ViewState["CurrentShowTableMode"];
                else
                    mode = ModeOfShowTable.Single;
                return mode;
            }
            set
            {
                ViewState["CurrentShowTableMode"] = value;
            }
        }

        /// <summary>
        /// Property to keep track of the Selected Table to interact with the using TableEditor control
        /// </summary>
        public string CurrentTable
        {
            get
            {
                if (ViewState["CurrentTable"] == null)
                    return string.Empty;
                else
                    return ViewState["CurrentTable"].ToString();
            }
            set
            {
                ViewState["CurrentTable"] = value;
            }
        }

        /// <summary>
        /// Property to keep track of the Selected Table to interact with the using TableEditor control
        /// </summary>
        private string CurrentTableDisplayName
        {
            get
            {
                if (ViewState["CurrentTableDisplayName"] == null)
                    return string.Empty;
                else
                    return ViewState["CurrentTableDisplayName"].ToString();
            }
            set
            {
                ViewState["CurrentTableDisplayName"] = value;
            }
        }

        /// <summary>
        /// Property to keep track of the Current Action mode selected
        /// </summary>
        private ModeOfAction CurrentMode
        {
            get
            {
                EnsureChildControls();
                ModeOfAction mode;
                if (ViewState["Mode"] != null)
                    mode = (ModeOfAction)ViewState["Mode"];
                else
                    mode = ModeOfAction.Update;
                return mode;
            }
            set
            {
                EnsureChildControls();
                ViewState["Mode"] = value;
            }
        }

        /// <summary>
        /// Property to keep track of the selected row primary key to interact with the using TableEditor control
        /// </summary>
        private int CurrentRowPK
        {
            get
            {
                if (ViewState["CurrentRowPK"] == null)
                    return 0;
                else
                    return int.Parse(ViewState["CurrentRowPK"].ToString());
            }
            set
            {
                ViewState["CurrentRowPK"] = value;
            }
        }

        /// <summary>
        /// Property to keep track of the return DataTable for the code using 
        /// </summary>
        private DataTable ReturnDataTable
        {
            get
            {
                if (ViewState["DataTable"] == null)
                    return null;
                else
                    return (DataTable)ViewState["DataTable"];
            }
            set
            {
                ViewState["DataTable"] = value;
            }
        }

        /// <summary>
        /// Property for LookupDataSource  
        /// </summary>
        private List<ID_Column> LookupDataSource
        {
            get
            {
                if (ViewState["LookupDataSource"] == null)
                    return null;
                else
                    return (List<ID_Column>)ViewState["LookupDataSource"];
            }
            set
            {
                ViewState["LookupDataSource"] = value;
            }
        }

        private bool HasJustSaved
        {
            get
            {
                if (Page.Session["HasJustSaved"] == null)
                    return false;
                else
                    return (bool)Page.Session["HasJustSaved"];
            }
            set
            {
                Page.Session["HasJustSaved"] = value;
            }
        }
        #endregion

        #region Controls Declarations
        Infragistics.WebUI.Misc.WebPanel pnlViewTables = null;
        Infragistics.WebUI.Misc.WebPanel pnlViewTable = null;
        Infragistics.WebUI.Misc.WebPanel pnlViewChildTable = null;
        Infragistics.WebUI.Misc.WebPanel pnlViewListBox = null;
        Infragistics.WebUI.Misc.WebPanel pnlTableDescription = null;
        Label lblEditTable = null;
        Label lblClickRefreshInfo = null;
        Table lblClickRefreshInfoTable = null;
        System.Web.UI.WebControls.DropDownList ddlTableName;
        Label labTableDescription = null;
        //added on 2008/11/05 to fix 3457 bug
        System.Web.UI.WebControls.Button btnAdd = null;
        System.Web.UI.WebControls.Button btnRefresh = null;
        //end fix

        Infragistics.WebUI.Misc.WebPanel pnlEditBtn = null;
        Infragistics.WebUI.Misc.WebPanel pnlEditRecord = null;
        Infragistics.WebUI.Misc.WebPanel pnlAddRecord = null;
        Label lblTableNameEdtPnl = null;
        Label lblTableNameAddPnl = null;

        HiddenField ActionToDoHiddenField = null;
        HiddenField ConfirmDelHiddenField = null;

        UltraWebGrid uwdList = null;
        UltraWebGrid uwdChildTable = null;
        //added on 2008/11/05 to fix 3457 bug
        System.Web.UI.WebControls.Button btnEdit = null;
        System.Web.UI.WebControls.Button btnCancel = null;
        //end fix

        Label lblPageSize = null;
        TextBox txtPageSize = null;
        Infragistics.WebUI.Misc.WebPanel pnlPage = null;
        //added on 2008/11/05 to fix 3457 bug
        System.Web.UI.WebControls.Button btnSetPageSize = null;
        //end fix

        //for child table
        ListBox leftListBox = null;
        ListBox rightListBox = null;
        #endregion

        #region Bind Controls

        #region CreateChildControls
        /// <summary>
        /// Main Function that draws the Controls. contains the logic to implement the Control 
        /// and binding them with the Events dynamically on the basis of the data that has been 
        /// fetched from the TableEditor Service.
        /// </summary>
        protected override void CreateChildControls()
        {
            //Init Main Page Panel And Controls.
            InitMainPagePanelControls();

            // Init EditButton Panel
            InitEditButtonPanel();

            // Init AddRecord Panel
            InitAddRecordPanel();

            // Init EditRecord Panel.
            InitEditRecordPanel();

            // set controls styles
            SetStyles();

            //Add on 2009/02/06 to fix bug 103982
            isShowMessageLabel();
            //End add

            _includeStructure = false;
            btnEdit.Text = CurrentMode.ToString();
            if (!DesignMode)
            {
                if (CurrentTable != string.Empty)
                {
                    COETableEditorBOList boList = COETableEditorBOList.NewList();
                    //init the DAL object in TableName property define
                    boList.TableName = CurrentTable;
                    _businessObj = null;

                    switch (CurrentMode)
                    {
                        case ModeOfAction.Default:
                            //bind the webcombo control  with tables' names  
                            BindWbcTableName();
                            break;
                        case ModeOfAction.Add:
                            _businessObj = COETableEditorBO.New();//read the xml
                            CreateForm(_businessObj, ref pnlAddRecord, CurrentMode);
                            break;
                        case ModeOfAction.Update:
                            _businessObj = COETableEditorBO.Get(CurrentRowPK);//read the database
                            CreateForm(_businessObj, ref pnlEditRecord, CurrentMode);
                            break;
                    }
                }
                else
                {
                    //bind the webcombo control  with tables' names  
                    BindWbcTableName();
                    HideEditPanel();
                    SetAddButtonVisibility(true);
                    pnlPage.Visible = false;
                    lblEditTable.Visible = true;

                }
            }
            //added on 2008/11/13 to fix 103856 bug
            ddlTableName.SelectedIndexChanged += new EventHandler(ddlTableName_SelectedIndexChanged);
            ddlTableName.AutoPostBack = true;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            //end added
            uwdList.PageIndexChanged += new Infragistics.WebUI.UltraWebGrid.PageIndexChangedEventHandler(uwdList_PageIndexChanged);

            uwdList.ClickCellButton += new ClickCellButtonEventHandler(uwdList_ClickCellButton);
            uwdList.SortColumn += new SortColumnEventHandler(Sorting_ColumnSorted);
            if (this.CurrentShowTableMode == ModeOfShowTable.Multi)
                uwdList.ActiveRowChange += new ActiveRowChangeEventHandler(uwdList_ActiveRowChange);
            // To add button captions. Tao Ran. 2008-07-10
            uwdList.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(uwdList_InitializeRow);
            //added on 2008/11/05 to fix 3457 bug
            if (ddlTableName.SelectedIndex != 0)
            {
                btnRefresh.Visible = true;
            }
            else
            {
                btnRefresh.Visible = false;
            }
            if (btnAdd != null)
            {
                btnAdd.Click += new EventHandler(btnAdd_Click);
            }
            btnEdit.Click += new EventHandler(btnEdit_Click);
            btnCancel.Click += new EventHandler(btnCancle_Click);
            btnCancel.CausesValidation = false;
            btnSetPageSize.Click += new EventHandler(btnSetPageSize_Click);
            //end fix
        }

        /// <summary>
        /// Init Main Page Panel And Controls.
        /// </summary>
        void InitMainPagePanelControls()
        {
            Controls.Clear();

            ConfirmDelHiddenField = new HiddenField();
            ConfirmDelHiddenField.Value = "NotSelDelOption";
            ConfirmDelHiddenField.ID = "ConfirmDelHiddenField";
            this.Controls.Add(ConfirmDelHiddenField);

            //2008-4-16 add by david zhang for init hidden fields
            ActionToDoHiddenField = new HiddenField();
            ActionToDoHiddenField.ID = "ActionToDoHiddenFieldID";
            ActionToDoHiddenField.Value = "REMOVE";
            this.Controls.Add(ActionToDoHiddenField);

            pnlViewTables = new Infragistics.WebUI.Misc.WebPanel();
            pnlViewTables.Header.Visible = false;
            pnlViewTables.ID = "Panel0";
            pnlViewTables.Style.Add("width", "100%");
            pnlViewTables.Style.Add("padding-bottom", "50px");
            lblClickRefreshInfo = new Label();
            lblClickRefreshInfo.Text = "Click on the refresh button to see the updated values.";
            lblClickRefreshInfo.ID = "lblClickRefreshInfo";
            lblClickRefreshInfo.CssClass = "MessagesAreaTextLabel";
            Table lblClickRefreshInfoTable = new Table();
            lblClickRefreshInfoTable.ID = "lblClickRefreshInfoTable";
            lblClickRefreshInfoTable.CssClass = "MessageAreaTable";
            //messageTable.Style.Add(HtmlTextWriterStyle.Width,"100%");
            TableRow messageRow = new TableRow();
            TableCell messageCell = new TableCell();
            messageCell.Attributes.Add("align", "center");
            messageCell.Controls.Add(lblClickRefreshInfo);
            messageRow.Controls.Add(messageCell);
            lblClickRefreshInfoTable.Controls.Add(messageRow);
            lblClickRefreshInfoTable.Visible = true;
            pnlViewTables.Controls.Add(lblClickRefreshInfoTable);
            this.Controls.Add(pnlViewTables);

            if (ClickRefreshInfoVisible == false)
            {
                lblClickRefreshInfo.Visible = false;
                lblClickRefreshInfoTable.Visible = false;
            }

            pnlViewTable = new Infragistics.WebUI.Misc.WebPanel();
            pnlViewTable.Header.Visible = false;
            pnlViewTable.ID = "Panel1";
            this.Controls.Add(pnlViewTable);

            pnlTableDescription = new Infragistics.WebUI.Misc.WebPanel();
            pnlTableDescription.Header.Visible = false;
            pnlTableDescription.ID = "PanelDescription";
            lblEditTable = new Label();
            lblEditTable.Text = "Select a table/view: ";
            lblEditTable.ID = "lblEditTable";
            ddlTableName = new DropDownList();
            ddlTableName.ID = "ddlTableName";

            labTableDescription = new Label();
            Table tbl = new Table();
            tbl.ID = "Table1";
            tbl.Style.Add("width", "100%");
            TableRow row;
            TableCell cell;
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            cell.Controls.Add(lblEditTable);
            cell.Style.Add("text-align", "right");
            cell.Style.Add("width", "50%");
            row.Cells.Add(cell);

            cell = new TableCell();
            //added on 2008/11/13 to fix 103856 bug
            cell.Controls.Add(ddlTableName);
            //end added
            cell.Style.Add("text-align", "left");
            row.Cells.Add(cell);

            cell = new TableCell();
            //added on 2008/11/05 to fix 3457 bug
            if (COETableEditorUtilities.HasAddPrivileges(CurrentTable))
            {
                btnAdd = new Button();
                btnAdd.Text = "Add";
                btnAdd.ControlStyle.CssClass = ButtonStyle;
                //end fix
                cell.Controls.Add(btnAdd);
            }
            else
            {
                if (btnAdd != null)
                {
                    pnlViewTable.Controls.Remove(btnAdd);
                    btnAdd = null;
                }
            }

            row.Cells.Add(cell);

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.ToolTip = "Click to refresh the picklists in registration.";
            btnRefresh.ControlStyle.CssClass = ButtonStyle;
            cell = new TableCell();
            cell.Controls.Add(btnRefresh);
            cell.Style.Add("text-align", "left");
            row.Cells.Add(cell);

            row = new TableRow();
            cell = new TableCell();
            cell.Controls.Add(labTableDescription);
            cell.CssClass = "TableDescription";
            cell.ColumnSpan = 4;
            row.Cells.Add(cell);
            //cell = new TableCell();
            //cell.CssClass = "TableDescription";
            ////added on 2008/11/05 to fix 3457 bug
            //if (COETableEditorUtilities.HasAddPrivileges(CurrentTable))
            //{
            //    btnAdd = new Button();
            //    btnAdd.Text = "Add";
            //    btnAdd.ControlStyle.CssClass = ButtonStyle;
            //    //end fix
            //    cell.Controls.Add(btnAdd);
            //}
            //else
            //{
            //    if (btnAdd != null)
            //    {
            //        pnlViewTable.Controls.Remove(btnAdd);
            //        btnAdd = null;
            //    }
            //}
            //row.Cells.Add(cell);
            tbl.Rows.Add(row);
            pnlTableDescription.Controls.Add(tbl);
            pnlViewTable.Controls.Add(pnlTableDescription);
            uwdList = new UltraWebGrid();
            uwdList.ID = "uwdList";
            //Added on 2009/02/05 to fix 106288 bug.
            uwdList.DisplayLayout.NoDataMessage = "Select a table to edit";

            //End added
            uwdList.DisplayLayout.AutoGenerateColumns = false;
            // To confirm delete at client side. Tao Ran. 2008-07-17
            uwdList.DisplayLayout.ClientSideEvents.ClickCellButtonHandler = "ConfirmDelete";
            uwdList.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            if (!string.IsNullOrEmpty(CurrentTable))
            {
                int maxPageSize = COETableEditorUtilities.GetMaxPageSize(CurrentTable);
                _renderMaxPageSizeAlert = PageSize > maxPageSize;
                if (_renderMaxPageSizeAlert)
                    PageSize = maxPageSize;
            }
            uwdList.DisplayLayout.Pager.PageSize = PageSize;
            uwdList.DisplayLayout.Pager.AllowPaging = true;
            uwdList.DisplayLayout.Pager.Pattern = "[page:first:< ]    [page:prev:<<] | [default] | [page:next:>>] [page:last: >]";
            uwdList.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
            uwdList.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            uwdList.DisplayLayout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.Yes;
            uwdList.DisplayLayout.HeaderClickActionDefault = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.SortSingle;
            uwdList.DisplayLayout.RowStyleDefault.Cursor = Infragistics.WebUI.Shared.Cursors.Default;
            uwdList.DisplayLayout.HeaderStyleDefault.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
            uwdList.DisplayLayout.FooterStyleDefault.Cursor = Infragistics.WebUI.Shared.Cursors.Default;
            uwdList.DisplayLayout.Pager.Style.Cursor = Infragistics.WebUI.Shared.Cursors.Default;
            uwdList.DisplayLayout.FrameStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Default;

            Table tblViewTable = new Table();
            tblViewTable.ID = "TableViewTable";
            TableRow rowViewTable = new TableRow();
            TableCell cellViewTable = new TableCell();
            //Add on 2009/02/06 to fix bug 103982
            messageLabel.Style.Add("width", "100%");
            messageLabel.Style.Add("font-weight", "bold");
            messageLabel.ForeColor = Color.Red;
            messageLabel.Visible = false;
            messageLabel.ID = "MessageLabel";
            cellViewTable.Controls.Add(messageLabel);
            rowViewTable.Cells.Add(cellViewTable);
            tblViewTable.Rows.Add(rowViewTable);
            //End add

            rowViewTable = new TableRow();
            cellViewTable = new TableCell();
            cellViewTable.Controls.Add(uwdList);
            rowViewTable.Cells.Add(cellViewTable);
            tblViewTable.Rows.Add(rowViewTable);

            rowViewTable = new TableRow();

            cellViewTable = new TableCell();

            pnlViewChildTable = new Infragistics.WebUI.Misc.WebPanel();
            pnlViewChildTable.ID = "PanelViewChildTable";
            cellViewTable.Controls.Add(pnlViewChildTable);
            cellViewTable.CssClass = "TableEditorChildTable";
            rowViewTable.Cells.Add(cellViewTable);
            tblViewTable.Rows.Add(rowViewTable);
            pnlViewTable.ID = "PanelViewTable";

            pnlViewTable.Controls.Add(tblViewTable);

            //init different pages's Panel
            pnlPage = new Infragistics.WebUI.Misc.WebPanel();
            pnlPage.ID = "PanelPage";
            //added on 2008/11/13 to fix 103855 bug
            pnlPage.Header.Visible = false;
            //end added
            lblPageSize = new Label();
            lblPageSize.ID = "lblePageSize";
            lblPageSize.Text = "Please input the page size: ";
            Table tblPageSize = new Table();
            tblPageSize.ID = "tblPageSize";
            TableRow rowPageSize = new TableRow(); ;
            TableCell cellPageSize;
            tblPageSize.Rows.Add(rowPageSize);
            cellPageSize = new TableCell();
            cellPageSize.Controls.Add(lblPageSize);
            rowPageSize.Cells.Add(cellPageSize);

            txtPageSize = new TextBox();
            txtPageSize.Width = 20;
            txtPageSize.MaxLength = 3;
            txtPageSize.Text = PageSize.ToString();
            cellPageSize = new TableCell();
            cellPageSize.Controls.Add(txtPageSize);
            cellPageSize.VerticalAlign = VerticalAlign.Middle;
            rowPageSize.Cells.Add(cellPageSize);

            Label lblSpace = new Label();
            lblSpace.ID = "lblSpace";
            lblSpace.Width = 10;
            cellPageSize = new TableCell();
            cellPageSize.Controls.Add(lblSpace);
            cellPageSize.VerticalAlign = VerticalAlign.Middle;
            rowPageSize.Cells.Add(cellPageSize);

            btnSetPageSize = new Button();
            btnSetPageSize.ID = "btnSetPageSize";
            btnSetPageSize.Text = "Set";
            //added on 2008/11/05 to fix 3457 bug
            btnSetPageSize.ControlStyle.CssClass = ButtonStyle;
            //end fix
            cellPageSize = new TableCell();
            cellPageSize.Controls.Add(btnSetPageSize);
            cellPageSize.VerticalAlign = VerticalAlign.Middle;
            rowPageSize.Cells.Add(cellPageSize);
            pnlPage.Controls.Add(tblPageSize);

            Table tblPage = new Table();
            tblPage.ID = "tblPage";
            tblPage.Style.Add("Width", "100%");
            TableRow rowPage;
            TableCell cellPage;
            rowPage = new TableRow();
            tblPage.Rows.Add(rowPage);
            cellPage = new TableCell();
            cellPage.HorizontalAlign = HorizontalAlign.Left;
            cellPage.Controls.Add(pnlPage);
            rowPage.Cells.Add(cellPage);
            pnlViewTable.Controls.Add(tblPage);
            //outside Information Complete
            //if has child table
            if (CurrentShowTableMode == ModeOfShowTable.Multi)
            {
                uwdChildTable = new UltraWebGrid();
                uwdChildTable.ID = "uwdChildTable";
                uwdChildTable.DisplayLayout.AutoGenerateColumns = false;
                uwdChildTable.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
                uwdChildTable.DisplayLayout.Pager.PageSize = 8;
                uwdChildTable.DisplayLayout.Pager.AllowPaging = true;
                //uwdChildTable.DisplayLayout.Pager.StyleMode = PagerStyleMode.PrevNext;
                uwdChildTable.DisplayLayout.Pager.Pattern = "[page:first:< ]    [page:prev:<<] | [default] | [page:next:>>] [page:last: >]";
                uwdChildTable.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
                uwdChildTable.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
                uwdChildTable.DisplayLayout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.OnClient;
                uwdChildTable.DisplayLayout.HeaderClickActionDefault = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.SortSingle;
                //move from CreateChildControl()
                uwdChildTable.PageIndexChanged += new Infragistics.WebUI.UltraWebGrid.PageIndexChangedEventHandler(uwdChildTable_PageIndexChanged);

                pnlViewChildTable.Header.Visible = false;
                pnlViewChildTable.Controls.Add(uwdChildTable);

                pnlViewChildTable.Visible = true;
            }
            else
            {
                pnlViewChildTable.Visible = false;
            }
        }

        /// <summary>
        /// Init EditButton Panel.
        /// </summary>
        void InitEditButtonPanel()
        {
            //added on 2008/11/05 to fix 3457 bug
            btnEdit = new Button();
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnEdit.ControlStyle.CssClass = ButtonStyle;
            btnCancel.ControlStyle.CssClass = ButtonStyle;
            //end fix
            Table tblEditBtn = new Table();
            TableRow rowEditBtn;
            TableCell cellEditBtn;
            rowEditBtn = new TableRow();
            tblEditBtn.Rows.Add(rowEditBtn);
            cellEditBtn = new TableCell();
            cellEditBtn.Controls.Add(btnEdit);
            rowEditBtn.Cells.Add(cellEditBtn);

            cellEditBtn = new TableCell();
            cellEditBtn.Style.Add("Width", "60px");
            rowEditBtn.Cells.Add(cellEditBtn);

            cellEditBtn = new TableCell();
            cellEditBtn.Controls.Add(btnCancel);
            rowEditBtn.Cells.Add(cellEditBtn);
            pnlEditBtn = new Infragistics.WebUI.Misc.WebPanel();
            pnlEditBtn.ID = "PanelEditButton";
            //added on 2008/11/13 to fix 103855 bug
            pnlEditBtn.Header.Visible = false;
            //end added
            pnlEditBtn.Controls.Add(tblEditBtn);

            this.Controls.Add(pnlEditBtn);
        }

        /// <summary>
        /// Init EditRecord Panel.
        /// </summary>
        void InitEditRecordPanel()
        {
            pnlEditRecord = new Infragistics.WebUI.Misc.WebPanel();
            pnlEditRecord.ID = "PanelEditRecord";
            //added on 2008/11/13 to fix 103855 bug
            pnlEditRecord.Header.Visible = false;
            //end added
            //added for showing the table name in the edit panel
            lblTableNameEdtPnl = new Label();
            lblTableNameEdtPnl.Text = CurrentTableDisplayName;
            Table tblTableName = new Table();
            tblTableName.Style.Add("Width", "100%");
            TableRow rowTableName;
            TableCell cellTableName;
            cellTableName = new TableCell();
            cellTableName.HorizontalAlign = HorizontalAlign.Center;
            cellTableName.CssClass = "CellTableName";
            cellTableName.Controls.Add(lblTableNameEdtPnl);
            rowTableName = new TableRow();
            rowTableName.Cells.Add(cellTableName);
            tblTableName.Rows.Add(rowTableName);
            rowTableName = new TableRow();
            tblTableName.Rows.Add(rowTableName);
            pnlEditRecord.Controls.Add(tblTableName);
            //end added
            pnlEditRecord.ID = "PnlEditRecord";

            this.Controls.Add(pnlEditRecord);
        }

        /// <summary>
        /// Init AddRecord Panel.
        /// </summary>
        void InitAddRecordPanel()
        {
            pnlAddRecord = new Infragistics.WebUI.Misc.WebPanel();
            pnlAddRecord.ID = "PanelAddRecord";
            //added on 2008/11/13 to fix 103855 bug
            pnlAddRecord.Header.Visible = false;
            //end added
            pnlAddRecord.ID = "PnlAddRecord";
            //added for showing the table name in the add panel
            lblTableNameAddPnl = new Label();
            lblTableNameAddPnl.Text = CurrentTableDisplayName;
            Table tblTableNameAddPnl = new Table();
            tblTableNameAddPnl.Style.Add("Width", "100%");
            TableRow rowTableNameAddPnl;
            TableCell cellTableNameAddPnl;
            cellTableNameAddPnl = new TableCell();
            cellTableNameAddPnl.HorizontalAlign = HorizontalAlign.Center;
            cellTableNameAddPnl.CssClass = "CellTableName";
            cellTableNameAddPnl.Controls.Add(lblTableNameAddPnl);
            //cellTableNameAddPnl.Style.Add("align", "left");
            rowTableNameAddPnl = new TableRow();
            rowTableNameAddPnl.Cells.Add(cellTableNameAddPnl);
            tblTableNameAddPnl.Rows.Add(rowTableNameAddPnl);
            pnlAddRecord.Controls.Add(tblTableNameAddPnl);
            //end added
            this.Controls.Add(pnlAddRecord);

        }

        /// <summary>
        /// set controls styles
        /// </summary>
        void SetStyles()
        {
            uwdList.DisplayLayout.FilterOptionsDefault.FilterDropDownStyle.CustomRules = "overflow:auto;";
            uwdList.DisplayLayout.FilterOptionsDefault.FilterDropDownStyle.Padding.Left = 5;
            uwdList.DisplayLayout.HeaderStyleDefault.CssClass = UltraWebGridHeaderStyle;
            uwdList.DisplayLayout.FrameStyle.CssClass = UltraWebGridFrameStyle;
            uwdList.DisplayLayout.RowStyleDefault.CssClass = WebGridRowStyle;
            uwdList.DisplayLayout.Grid.Width = (_columnsWidth < TableEditorGridWidth) ? Unit.Pixel(_columnsWidth) : Unit.Pixel(TableEditorGridWidth);
            if (CurrentShowTableMode == ModeOfShowTable.Multi)
            {
                uwdChildTable.DisplayLayout.RowStyleDefault.CssClass = WebGridRowStyle;
                uwdChildTable.DisplayLayout.FilterOptionsDefault.FilterDropDownStyle.CustomRules = "overflow:auto;";
                uwdChildTable.DisplayLayout.FilterOptionsDefault.FilterDropDownStyle.Padding.Left = 5;
                uwdChildTable.DisplayLayout.HeaderStyleDefault.CssClass = UltraWebGridHeaderStyle;
                uwdChildTable.DisplayLayout.FrameStyle.CssClass = UltraWebGridFrameStyle;
            }

            pnlPage.CssClass = PanelSetPageStyle;
            pnlTableDescription.CssClass = PanelTableDescriptionStyle;
            pnlViewTable.CssClass = PanelViewTableStyle;

            pnlAddRecord.CssClass = PanelEditRecordStyle;
            pnlEditRecord.CssClass = PanelEditRecordStyle;

            pnlEditBtn.CssClass = PanelEditBtnStyle;
        }
        #endregion

        public string GetDefaultButtonID()
        {
            if (btnEdit != null && btnEdit.Visible)
                return btnEdit.UniqueID;
            else if (btnAdd != null && btnAdd.Visible)
                return btnAdd.UniqueID;
            else if (btnCancel != null && btnCancel.Visible)
                return btnCancel.UniqueID;
            else
                return null;
        }

        public bool IsColumnToLock(string colName)
        {
            bool retVal = true;
            foreach (var value in Enum.GetValues(typeof(ColumnsToIgnoreLock)))
            {
                if (value.ToString().ToUpper() == colName.ToUpper())
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Get the index of default value in Web Combo List
        /// </summary>
        /// <param name="defaultValue">Value set in FrameWorkConfig.xml</param>
        private int GetDefaultValueIndex(string defaultValue)
        {
            int retVal = 0;
            int iCount = 0;

            List<ID_Column> lookUpDataSOurce = LookupDataSource;
            if (lookUpDataSOurce != null)   //ASV Coverity Fix CID 11763
            {
                foreach (ID_Column col in lookUpDataSOurce)
                {
                    if ((!string.IsNullOrEmpty(col.ID)) && col.ID.ToString() == defaultValue)
                    {
                        retVal = iCount;
                        break;
                    }
                    iCount += 1;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Dynamically creates the Form on the current panel with the required controls according 
        /// to the field type of selected Record.
        /// </summary>
        /// <param name="DataItem"> Object of the TableEditorBO class</param>
        /// <param name="pnl"> Current panel (pnlAddRecord,pnlEditRecord)</param>
        /// <param name="selectedMode"> currently selected Mode <see cref="M190pxodeOfAction"/></param>
        void CreateForm(COETableEditorBO DataItem, ref Infragistics.WebUI.Misc.WebPanel pnl, ModeOfAction selectedMode)
        {
            //Table tblContainer = new Table();
            // Coverity Fix CID - 10544 (from local server)
            using (Table tblContainer = new Table())
            {
                tblContainer.Style.Add("width", "100%");
                tblContainer.Style.Add("border", "1px solid blue");
                if (DataItem != null)
                {
                    // Locking feature
                    bool isTableLocked = false;
                    if (selectedMode == ModeOfAction.Update)
                    {
                        foreach (Column pd in DataItem.Columns)
                        {
                            if (pd.FieldName != LockingColumns.LOCKED.ToString())
                                continue;
                            if (COETableEditorUtilities.getLookupLocation(CurrentTable, pd.FieldName).Length != 0 && COETableEditorUtilities.getLookupLocation(CurrentTable, pd.FieldName).ToLower().Contains("innerxml_"))
                            {
                                isTableLocked = (COETableEditorUtilities.getLookupValueFromInnerXML(CurrentTable, pd.FieldValue.ToString(), pd.FieldName)).ToUpper() == "YES" ? true : false;
                                break;
                            }
                        }
                    }

                    //For each column/Field in side the Current Record.
                    foreach (Column pd in DataItem.Columns)
                    {
                        TableRow tblRowContainer = new TableRow();

                        //added to fix CSBR - 135109
                        if (COETableEditorUtilities.GetHiddenProperty(CurrentTable, pd.FieldName))
                            tblRowContainer.Style.Add(HtmlTextWriterStyle.Display, "none");

                        //addd to fix 104767 bug to mark to identify the mandatory fields
                        TableCell tblCellMark = new TableCell();
                        tblCellMark.Style.Add("width", "7px");
                        tblCellMark.Style.Add("height", "30px");
                        tblCellMark.Style.Add("text-align", "left");


                        TableCell tblCellContainerName = new TableCell();
                        tblCellContainerName.Style.Add("width", "150px");
                        tblCellContainerName.Style.Add("height", "30px");
                        tblCellContainerName.Style.Add("text-align", "left");
                      

                        TableCell tblCellContainerValue = new TableCell();
                        tblCellContainerValue.CssClass = "CellContainerValue";
                        tblCellContainerValue.Style.Add("text-align", "left");
                        

                        TableCell tblCellContainerValidator = new TableCell();
                        // add by david zhang 2008/09/07
                        tblCellContainerValidator.Style.Add("width", "300px");
                        tblCellContainerValidator.Style.Add("height", "30px");
                        tblCellContainerValidator.Style.Add("text-align", "left");

                        // add end
                        WebControl Contr = null;
                        bool isChemDrawControl = false;
                        bool isClientChemDrawControl = false;
                        bool isLookupField = false;
                        //add by Jerry to validate webcombo
                        List<ValidationRule> validationRuleList = COETableEditorUtilities.getValidationRuleList(CurrentTable, pd.FieldName);
                        //Updated on 2009-1-5 for InnerXml node not have lookup field
                        if (COETableEditorUtilities.getLookupLocation(CurrentTable, pd.FieldName).Length != 0)
                        {
                            isLookupField = true;
                            List<ID_Column> tmpAddNullList;
                            ID_Column tmpAddNullID_Colum;
                            Column tmpAddNullColum;
                            bool IsCdx = COETableEditorUtilities.getIsStructureLookupField(CurrentTable, pd.FieldName);
                            if (IsCdx)
                            {
                                tmpAddNullList = COETableEditorBOList.getLookupFieldList(DataItem.ID, pd.FieldName);
                                //To fix 105018 bug to  add string 'null' to webcombo(chemDraw) when the field is not requireField
                                if (!COETableEditorUtilities.isRequied(CurrentTable, pd.FieldName) || tmpAddNullList.Count == 0)
                                {
                                    tmpAddNullColum = new Column("", DbType.String);
                                    if (tmpAddNullList.Count == 0 && COETableEditorUtilities.isRequied(CurrentTable, pd.FieldName))
                                    {
                                        tmpAddNullColum.FieldValue = "";
                                    }
                                    else
                                    {
                                        tmpAddNullColum.FieldValue = ChemdrawNullValue;
                                    }
                                    tmpAddNullID_Colum = new ID_Column(null, tmpAddNullColum);
                                    tmpAddNullList.Insert(0, tmpAddNullID_Colum);
                                }
                                //end add
                                ViewState[pd.FieldName + "dynamicStrc"] = tmpAddNullList;
                            }
                            else
                            {

                                tmpAddNullList = COETableEditorBOList.getLookupFieldList(DataItem.ID, pd.FieldName);
                                //To fix 105018 bug to add string 'null' to webcombo(chemDrawElse) when the field is not requireField 
                                if (!COETableEditorUtilities.isRequied(CurrentTable, pd.FieldName) || tmpAddNullList.Count == 0)
                                {
                                    tmpAddNullColum = new Column("", DbType.String);
                                    if (tmpAddNullList.Count == 0 && COETableEditorUtilities.isRequied(CurrentTable, pd.FieldName))
                                    {
                                        tmpAddNullColum.FieldValue = "";
                                    }
                                    else
                                    {
                                        tmpAddNullColum.FieldValue = "NULL";
                                    }
                                    tmpAddNullID_Colum = new ID_Column(null, tmpAddNullColum);
                                    tmpAddNullList.Insert(0, tmpAddNullID_Colum);
                                }
                                //end add
                                LookupDataSource = tmpAddNullList;
                            }

                            //add to webcombo bind
                            Infragistics.WebUI.WebCombo.WebCombo wbc = new Infragistics.WebUI.WebCombo.WebCombo();
                            isChemDrawControl = BindLookupField(wbc, CurrentTable, pd.FieldName);
                            //end add
                            Contr = wbc;

                            wbc.SelectedIndex = 0;

                            // Assign default value selected in webcombo
                            if (!string.IsNullOrEmpty(COETableEditorUtilities.getDefaultValue(CurrentTable, pd.FieldName)))
                                wbc.SelectedIndex = GetDefaultValueIndex(COETableEditorUtilities.getDefaultValue(CurrentTable, pd.FieldName));

                            wbc.Enabled = (isTableLocked) ? ((IsColumnToLock(pd.FieldName)) ? false : true) : true;
                            if (CurrentMode != ModeOfAction.Add && pd.FieldValue != null)
                            {
                                foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in wbc.Rows)
                                {
                                    if (row.Cells.FromKey("ID").Value != null)
                                    {
                                        //Updated for innerXml_ActiveCase when the value property value is string.
                                        if (row.Cells.FromKey("ID").Value.ToString().Equals(pd.FieldValue.ToString()))
                                        {
                                            wbc.SelectedRow = row;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else  //not exit lookupField
                        {
                            //Check for the CDX field so that ChemDraw can be used.
                            if (COETableEditorUtilities.getIsStructure(CurrentTable, pd.FieldName))
                            {
                                _includeStructure = true;
                                CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed CoeChemControl = new CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed();
                                CoeChemControl.EnableViewState = true;
                                CoeChemControl.ViewOnly = false;
                                //fix 3434 bug
                                CoeChemControl.ControlStyle.CssClass = ChemdrawStyle;
                                //end fix
                                if (CurrentMode != ModeOfAction.Add)
                                    CoeChemControl.InlineData = pd.FieldValue == null ? "" : pd.FieldValue.ToString();
                                CoeChemControl.ViewOnly = (isTableLocked) ? ((IsColumnToLock(pd.FieldName)) ? true : false) : false;
                                CoeChemControl.ID = pd.FieldName;
                                Contr = CoeChemControl;
                                isChemDrawControl = true;
                                isClientChemDrawControl = true;
                            }
                            else
                            {
                                //Selecting the Control according to the type of the of the Column/Field
                                if (pd.FieldType == System.Data.DbType.Boolean)
                                {
                                    CheckBox chk = new CheckBox();
                                    if (CurrentMode != ModeOfAction.Add)
                                        chk.Checked = Convert.ToBoolean(pd.FieldValue);
                                    chk.Enabled = (isTableLocked) ? ((IsColumnToLock(pd.FieldName)) ? false : true) : true;
                                    chk.ID = pd.FieldName;
                                    Contr = chk;
                                }
                                else if (pd.FieldType == System.Data.DbType.DateTime)
                                {
                                    Infragistics.WebUI.WebSchedule.WebDateChooser wb = new Infragistics.WebUI.WebSchedule.WebDateChooser();
                                    if (CurrentMode != ModeOfAction.Add && pd.FieldValue.ToString().Length != 0)
                                        wb.Value = Convert.ToDateTime(pd.FieldValue.ToString());
                                    else
                                    {
                                        wb.Value = System.DateTime.Now;
                                        wb.ReadOnly = true;
                                    }
                                    wb.ID = pd.FieldName;
                                    Contr = wb;
                                }
                                else
                                {
                                    TextBox tbt = new TextBox();
                                    tbt.EnableViewState = false;

                                    if (CurrentMode != ModeOfAction.Add)
                                    {
                                        tbt.Text = pd.FieldValue == null ? "" : Page.Server.HtmlDecode(pd.FieldValue.ToString());
                                    }
                                    tbt.Enabled = (isTableLocked) ? ((IsColumnToLock(pd.FieldName)) ? false : true) : true;
                                    // updated by Jerry for primary key filed unavailable on 2008/04/09 
                                    if (COETableEditorUtilities.getIdFieldName(CurrentTable).ToLower() == pd.FieldName.ToLower())
                                    {
                                        if (CurrentMode == ModeOfAction.Add)
                                            tbt.Visible = false;
                                        else
                                            tbt.Enabled = false;
                                    }

                                    tbt.ID = pd.FieldName;
                                    if (COETableEditorUtilities.GetIsFormula(CurrentTable, pd.FieldName))
                                    {
                                        Contr = new Panel();
                                        Contr.Controls.Add(tbt);
                                        Button formulaButton = new Button();
                                        formulaButton.CssClass = "GetStructureInfoButton";
                                        Contr.Controls.Add(formulaButton);
                                        formulaButton.Text = Resources.GetFormula_Button_Text;
                                        formulaButton.OnClientClick = "this.parentNode.getElementsByTagName('input')[0].value = cd_getFormula(cd_objectArray[0], 0); return false;";
                                    }
                                    else if (COETableEditorUtilities.GetIsMolWeight(CurrentTable, pd.FieldName))
                                    {
                                        Contr = new Panel();
                                        Contr.Controls.Add(tbt);
                                        Button molweightButton = new Button();
                                        molweightButton.CssClass = "GetStructureInfoButton";
                                        Contr.Controls.Add(molweightButton);
                                        molweightButton.Text = Resources.GetMolweight_Button_Text;
                                        molweightButton.OnClientClick = "this.parentNode.getElementsByTagName('input')[0].value = cd_getMolWeight(cd_objectArray[0], 0); return false;";
                                    }
                                    else
                                        Contr = tbt;
                                }
                            }
                        }

                        //added to Validate
                        if (validationRuleList != null)
                        {
                            CustomValidator clientValidator;
                            ChemDrawClientInfo clientChemDrawInfo = new ChemDrawClientInfo();
                            foreach (ValidationRule validationRule in validationRuleList)
                            {
                                //chemdraw Client valid Info
                                if (isClientChemDrawControl)
                                {
                                    if (validationRule.Name.ToLower() == RequiredField.ToLower())
                                    {
                                        clientChemDrawInfo.isRequiredField = true;
                                        clientChemDrawInfo.strRequiredFieldErrorMessage = validationRule.ErrorMessage;
                                    }
                                    else if (validationRule.Name.ToLower() == MolWeight.ToLower())
                                    {
                                        clientChemDrawInfo.isMolWeight = true;
                                        clientChemDrawInfo.strMolWeightErrorMessage = validationRule.ErrorMessage;
                                    }
                                }
                                //To add client validator to webcombo
                                if (isLookupField)
                                {
                                    clientValidator = ValidateLookupFieldClient(validationRule, pnl.ClientID + "_" + Contr.ID.Trim());
                                    if (clientValidator != null)
                                    {
                                        tblCellContainerValidator.Controls.Add(clientValidator);
                                    }
                                }
                                //end add
                                if (Contr is Panel)
                                    CreateValidatorFromRule(validationRule, tblCellContainerValidator, Contr.Controls[0] as WebControl, CurrentTable, pd.FieldName, isChemDrawControl);
                                else
                                    CreateValidatorFromRule(validationRule, tblCellContainerValidator, Contr, CurrentTable, pd.FieldName, isChemDrawControl);

                            }
                            //add Client valid to chemdraw
                            if (isClientChemDrawControl)
                            {
                                clientValidator = ValidateChemDrawClient(clientChemDrawInfo, pnl.ClientID + "_" + Contr.ID.Trim());
                                if (clientValidator != null)
                                {
                                    tblCellContainerValidator.Controls.Add(clientValidator);
                                }
                            }
                        }
                        //end added

                        Label lblp = new Label();
                        string alias = COETableEditorUtilities.GetAlias(CurrentTable, pd.FieldName);
                        lblp.Text = (alias == null ? pd.FieldName : alias);

                        //addd to fix 104767 bug to mark to identify the mandatory fields
                        Label lblMark = new Label();
                        if (COETableEditorUtilities.isRequied(CurrentTable, pd.FieldName) || COETableEditorUtilities.GetHasChemValidValidation(CurrentTable, pd.FieldName))
                        {
                            lblMark.Text = "*";
                            lblMark.Width = new Unit("7px");
                            lblMark.ForeColor = Color.Red;
                        }
                        //end

                        //To hide primary key filed invisible when add.
                        if (Contr.Visible == true)
                        {
                            //addd to fix 104767 bug to mark to identify the mandatory fields
                            tblCellMark.Controls.Add(lblMark);
                            //end add
                            tblCellContainerName.Controls.Add(lblp);
                            tblCellContainerValue.Controls.Add(Contr);

                            tblRowContainer.Cells.Add(tblCellMark);

                            tblRowContainer.Cells.Add(tblCellContainerName);
                            tblRowContainer.Cells.Add(tblCellContainerValue);
                            tblRowContainer.Cells.Add(tblCellContainerValidator);
                            tblContainer.Rows.Add(tblRowContainer);
                        }
                    }

                    Table tblUpdatePnl = new Table();
                    TableRow rowUpdatePnl = new TableRow();
                    TableCell cellUpdatePnl = new TableCell();
                    tblContainer.Width = 0;
                    cellUpdatePnl.Controls.Add(tblContainer);
                    rowUpdatePnl.Cells.Add(cellUpdatePnl);
                    tblUpdatePnl.Rows.Add(rowUpdatePnl);

                    //if has child table 
                    if (CurrentShowTableMode == ModeOfShowTable.Multi)
                    {
                        Table projectEidtTable = CreateChildEditTable();
                        BindListBoxData();
                        pnlViewListBox = new Infragistics.WebUI.Misc.WebPanel();
                        pnlViewListBox.ID = "PanelViewListBox";
                        pnlViewListBox.Header.Visible = false;
                        pnlViewListBox.Controls.Add(projectEidtTable);
                        rowUpdatePnl = new TableRow();
                        cellUpdatePnl = new TableCell();
                        cellUpdatePnl.Style.Add("padding-top", "20px");
                        COEChildTable childTable = COETableEditorUtilities.GetChildTable(CurrentTable, 0);
                        Label lblChildTableName = new Label();
                        lblChildTableName.Text = childTable.DisplayName;
                        cellUpdatePnl.Controls.Add(lblChildTableName);
                        rowUpdatePnl.Cells.Add(cellUpdatePnl);
                        tblUpdatePnl.Rows.Add(rowUpdatePnl);
                        cellUpdatePnl.Controls.Add(pnlViewListBox);
                        rowUpdatePnl.Cells.Add(cellUpdatePnl);
                        tblUpdatePnl.Rows.Add(rowUpdatePnl);
                    }
                    //end childTable

                    //add the Container Table to the current panel.
                    pnl.Controls.Add(tblUpdatePnl);
                }
            }
        }

        #region Create ChildTable's ListBox

        /// <summary>
        /// Create ChildTable's ListBox.
        /// </summary>
        /// <returns></returns>
        private Table CreateChildEditTable()
        {
            Table projectEidtTable = new Table();
            Table operateTable = new Table();
            TableRow row = null;
            TableCell cell = null;

            // Coverity Fix CID : 11839 
            ImageButton moveToRightButton = new ImageButton();
            ImageButton moveToLeftButton = new ImageButton();

            try
            {
                // Create the MoveToRightButton.
                row = new TableRow();
                cell = new TableCell();
                cell.Style.Add("height", "50");
                cell.Style.Add("width", "150px");
                cell.HorizontalAlign = HorizontalAlign.Center;

                moveToRightButton.ImageUrl = Resources.TableEditorMoveToRightArrow;
                moveToRightButton.Width = Unit.Pixel(20);
                moveToRightButton.Height = Unit.Pixel(20);
                moveToRightButton.Click += new ImageClickEventHandler(MoveToRightButton_Click);
                cell.Controls.Add(moveToRightButton);
                row.Cells.Add(cell);
                operateTable.Rows.Add(row);

                //Create the MoveToLeftButton
                row = new TableRow();
                cell = new TableCell();
                cell.Style.Add("height", "50");
                cell.Style.Add("width", "150px");
                cell.Style.Add("padding-top", "10px");
                cell.HorizontalAlign = HorizontalAlign.Center;

                moveToLeftButton.ImageUrl = Resources.TableEditorMoveToLeftArrow;
                moveToLeftButton.Width = Unit.Pixel(20);
                moveToLeftButton.Height = Unit.Pixel(20);
                moveToLeftButton.Click += new ImageClickEventHandler(MoveToLeftButton_Click);
                cell.Controls.Add(moveToLeftButton);
                row.Cells.Add(cell);
                operateTable.Rows.Add(row);

                row = new TableRow();
                cell = new TableCell();
                cell.Style.Add("height", "20");
                cell.Style.Add("width", "150px");
                cell.Text = "Available";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Style.Add("height", "100%");
                cell.Style.Add("width", "150px");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Style.Add("height", "20");
                cell.Style.Add("width", "150px");
                cell.Text = "Current";
                row.Cells.Add(cell);
                projectEidtTable.Rows.Add(row);

                //Create the left ListBox.
                row = new TableRow();
                cell = new TableCell();
                cell.Style.Add("height", "100%");
                cell.Style.Add("width", "150px");
                leftListBox = new ListBox();
                leftListBox.Style.Add("width", "100%");
                leftListBox.CssClass = TableEditorListBoxStyle;
                leftListBox.SelectionMode = ListSelectionMode.Multiple;
                cell.Controls.Add(leftListBox);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Style.Add("height", "100%");
                cell.Style.Add("width", "150px");
                cell.Controls.Add(operateTable);
                row.Cells.Add(cell);

                //Create the right ListBox.
                cell = new TableCell();
                cell.Style.Add("height", "100%");
                cell.Style.Add("width", "150px");
                rightListBox = new ListBox();
                rightListBox.Style.Add("width", "100%");
                rightListBox.SelectionMode = ListSelectionMode.Multiple;
                rightListBox.CssClass = TableEditorListBoxStyle;
                cell.Controls.Add(rightListBox);
                row.Cells.Add(cell);
                projectEidtTable.Rows.Add(row);
                return projectEidtTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cell.Dispose();
                row.Dispose();
                operateTable.Dispose();
                projectEidtTable.Dispose();
            }
        }

        /// <summary>
        /// Bind the data of ChildTable's ListBox.
        /// </summary>
        private void BindListBoxData()
        {
            string id = CurrentRowPK + "";
            if (CurrentMode == ModeOfAction.Add)
                id = null;

            COETableEditorBOList boList = COETableEditorBOList.NewList();
            boList.TableName = CurrentTable;

            // Bind the data of left ListBox.
            leftListBox.DataSource = COETableEditorBOList.getChildDataTable(id, false); ;
            COEChildTable childTable = COETableEditorUtilities.GetChildTable(CurrentTable, 0);
            List<Column> colList = COETableEditorUtilities.GetChildTableColumnList(childTable);
            if (colList.Count != 2)
                return;

            foreach (Column col in colList)
            {
                if (col.FieldName.ToLower() == childTable.PrimaryKey.ToLower())
                    leftListBox.DataValueField = col.FieldName;
                else
                    leftListBox.DataTextField = col.FieldName;

            }
            leftListBox.DataBind();

            // Bind the data of right ListBox.
            rightListBox.DataSource = COETableEditorBOList.getChildDataTable(id, true);
            foreach (Column col in colList)
            {
                if (col.FieldName.ToLower() == childTable.PrimaryKey.ToLower())
                    rightListBox.DataValueField = col.FieldName;
                else
                    rightListBox.DataTextField = col.FieldName;

            }
            rightListBox.DataBind();
        }

        /// <summary>
        /// The click event of MoveToRightButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MoveToRightButton_Click(object sender, EventArgs e)
        {

            ListItem[] itemCollection = new ListItem[leftListBox.Items.Count];
            leftListBox.Items.CopyTo(itemCollection, 0);
            foreach (ListItem item in itemCollection)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                    rightListBox.Items.Insert(0, item);
                    leftListBox.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// The click event of MoveToLeftButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MoveToLeftButton_Click(object sender, EventArgs e)
        {
            ListItem[] itemCollection = new ListItem[rightListBox.Items.Count];
            rightListBox.Items.CopyTo(itemCollection, 0);
            foreach (ListItem item in itemCollection)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                    leftListBox.Items.Insert(0, item);
                    rightListBox.Items.Remove(item);
                }
            }
        }

        #endregion

        #region Methods for creating validators
        /// <summary>
        /// create validator from validation rule info,
        /// and add the validator to related tableCell
        /// </summary>
        /// <param name="validationRule">validation rule info of the control</param>
        /// <param name="tableCell">which tablecell the validator be added to</param>
        /// <param name="controlToAdd">which control the validator be added to</param>
        /// <param name="tableName">the current table name</param>
        /// <param name="fieldName">the current field name</param>
        /// <param name="isChemDrawControl">if the current control is chemdraw control</param>
        private void CreateValidatorFromRule(ValidationRule validationRule, TableCell tableCell, WebControl controlToAdd, string tableName, string fieldName, bool isChemDrawControl)
        {
            BaseValidator validator = this.GetValidator(validationRule, controlToAdd, tableCell, tableName, fieldName, isChemDrawControl);
            if (validator != null)
            {
                tableCell.Controls.Add((Control)validator);
            }
        }

        /// <summary>
        /// get validator from validation rule info
        /// </summary>
        /// <param name="validationRule">validation rule info of the control</param>
        /// <param name="controlToValidate">which control the validator be added to</param>
        /// <param name="tableCell">which tablecell the validator be added to</param>
        /// <param name="tableName">the current table name</param>
        /// <param name="fieldName">the current field name</param>
        /// <param name="isChemDrawControl">if the current control is chemdraw control</param>
        /// <returns>the base validator</returns>
        private BaseValidator GetValidator(ValidationRule validationRule, WebControl controlToValidate, TableCell tableCell, string tableName, string fieldName, bool isChemDrawControl)
        {
            bool isCustomer;
            BaseValidator validator = null;
            if (validationRule != null && controlToValidate != null)  //Coverity Fix CID 11662 ASV
            {
                if (isChemDrawControl && validationRule.Name != Custom)
                {
                    return validator;
                }
                switch (validationRule.Name)
                {
                    case RequiredField:
                        validator = new RequiredFieldValidator();
                        if (controlToValidate != null && controlToValidate is ICOERequireable)
                        {
                            ((ICOERequireable)controlToValidate).Required = true;
                        }
                        break;
                    case NumericRange:
                        validator = new RangeValidator();
                        ((RangeValidator)validator).Type = ValidationDataType.Double;
                        List<Common.Parameter> parameterListRange = COETableEditorUtilities.getParameterList(tableName, fieldName, NumericRange);
                        if (parameterListRange != null)
                        {
                            foreach (Common.Parameter param in parameterListRange)
                            {
                                switch (param.Name.ToLower().Trim())
                                {
                                    case Validator_Min:
                                        ((RangeValidator)validator).MinimumValue = param.Value;
                                        break;
                                    case Validator_Max:
                                        ((RangeValidator)validator).MaximumValue = param.Value;
                                        break;
                                }
                            }
                        }
                        break;
                    case PositiveNumber:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).ValueToCompare = "0";
                        ((CompareValidator)validator).Type = ValidationDataType.Double;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.GreaterThan;
                        break;
                    case PositiveInteger:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).ValueToCompare = "0";
                        ((CompareValidator)validator).Type = ValidationDataType.Integer;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.GreaterThan;
                        break;
                    case TextLength:
                        validator = new RegularExpressionValidator();
                        ((RegularExpressionValidator)validator).ValidationExpression = "(.|\r|\n){@min,@max}";
                        List<Common.Parameter> parameterList = COETableEditorUtilities.getParameterList(tableName, fieldName, TextLength);
                        if (parameterList != null)
                        {
                            foreach (Common.Parameter param in parameterList)
                            {
                                switch (param.Name.ToLower().Trim())
                                {
                                    case Validator_Min:
                                        ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@min", param.Value.Trim());
                                        break;
                                    case Validator_Max:
                                        ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@max", param.Value.Trim());
                                        break;
                                }
                            }
                        }
                        break;
                    case Custom:
                        validator = new CustomValidator();

                        string scriptName = validationRule.Name + "For" + controlToValidate.ID + "clientScript";
                        List<Common.Parameter> parameterListCustom = COETableEditorUtilities.getParameterList(tableName, fieldName, Custom);
                        if (parameterListCustom != null)
                        {
                            foreach (Common.Parameter param in parameterListCustom)
                            {
                                if (param.Name.ToLower().Trim() == "clientscript")
                                {
                                    string scriptBody = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                                         scriptName,
                                                                                         ReplaceControlNames(param.Value));

                                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered(scriptName))
                                        this.Page.ClientScript.RegisterClientScriptBlock(typeof(COETableManager), scriptName, scriptBody);

                                    ((CustomValidator)validator).ClientValidationFunction = scriptName;
                                    ((CustomValidator)validator).ValidateEmptyText = true;
                                }
                            }
                        }
                        break;
                }
                //Coverity Bug Fix CID 11662 
                if (validator != null)
                {
                    validator.ErrorMessage = validationRule.ErrorMessage;
                    validator.Display = ValidatorDisplay.Dynamic;
                    if (!isChemDrawControl)
                    {
                        validator.ControlToValidate = controlToValidate.ID;
                    }
                }
            }
            return validator;
        }

        /// <summary>
        /// delete the string "@" in parameter string
        /// </summary>
        /// <param name="script">the script used in the custom validator</param>
        /// <returns>the replaced string</returns>
        public string ReplaceControlNames(string script)
        {
            string scriptCode = script;
            int startIndex = 0;
            while (startIndex < scriptCode.Length && scriptCode.IndexOf('@', startIndex) > 0)
            {
                startIndex = scriptCode.IndexOf('@') + 1;
                int endIndex = scriptCode.IndexOfAny(separator, startIndex + 1);

                if (endIndex < 0)
                    endIndex = scriptCode.Length - 1;

                int length = endIndex - startIndex;

                if (length > 0)
                {
                    string controlName = scriptCode.Substring(startIndex, endIndex - startIndex);
                    scriptCode = scriptCode.Substring(0, startIndex - 1) + controlName + scriptCode.Substring(endIndex, scriptCode.Length - endIndex);
                }
            }

            return scriptCode;
        }

        /// <summary>
        /// validate ChemDraw in client side
        /// </summary>
        /// <param name="info">ChemDraw validation info getted from coe xml file</param>
        /// <param name="strChemDrawID">ChemDraw client id</param>
        /// <returns>the custom validator</returns>
        private CustomValidator ValidateChemDrawClient(ChemDrawClientInfo info, string strChemDrawID)
        {
            string functionBody = string.Empty;
            // Coverity Fix CID : 11837 
            using (CustomValidator validator = new CustomValidator())
            {

                if (info.isRequiredField && info.isMolWeight)
                {
                    functionBody = @" var num = cd_getMolWeight('" + strChemDrawID + @"');
                                  if(num > 0){
                                    arguments.IsValid = true;    
                                  }
                                  else
                                  {
                                    arguments.IsValid = false;
                                  }";
                    validator.ErrorMessage = info.strMolWeightErrorMessage;
                }
                else if (info.isRequiredField)
                {
                    functionBody = @"var blnStr = cd_isBlankStructure('" + strChemDrawID + @"');
                                  if(blnStr == false){
                                    arguments.IsValid = true;    
                                  }
                                  else
                                  {
                                    arguments.IsValid = false;
                                  }";
                    validator.ErrorMessage = info.strRequiredFieldErrorMessage;
                }
                else if (info.isMolWeight)
                {
                    functionBody = @"var num = cd_getMolWeight('" + strChemDrawID + @"'); 
                                  if(num > 0){
                                    arguments.IsValid = true;    
                                  }
                                  else
                                  {
                                    arguments.IsValid = false;
                                  }";
                    validator.ErrorMessage = info.strMolWeightErrorMessage;
                }
                else
                {
                    return null;
                }
                string scriptName = "ValidClientFor" + strChemDrawID + "Script";
                string scriptBody = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                     scriptName,
                                                                     functionBody);

                if (!this.Page.ClientScript.IsClientScriptBlockRegistered(scriptName))
                    this.Page.ClientScript.RegisterClientScriptBlock(typeof(COETableManager), scriptName, scriptBody);

                validator.ClientValidationFunction = scriptName;
                validator.ValidateEmptyText = true;

                validator.EnableClientScript = true;
                validator.Display = ValidatorDisplay.Dynamic;

                return validator;
            }
        }

        /// <summary>
        /// validate Lookup field in client side
        /// </summary>
        /// <param name="validationRule">Lookup field validation info getted from coe xml file</param>
        /// <param name="strWebComboID">WebCombo client id</param>
        /// <returns>the custom validator</returns>
        private CustomValidator ValidateLookupFieldClient(ValidationRule validationRule, string strWebComboID)
        {
            string functionBody = string.Empty;
            using (CustomValidator validator = new CustomValidator())   //Coverity Fix CID : 11838 
            {
                if (validationRule.Name.ToLower() == RequiredField.ToLower())
                {
                    functionBody = @"var combo = igcmbo_getComboById('" + strWebComboID + @"');
                var selectedIndex = combo.getSelectedIndex();
                var selectedValue = combo.getGrid().Rows.getRow(selectedIndex).getCell(1).getValue();
                if(selectedValue == null)
                    arguments.IsValid = false;    
                else
                    arguments.IsValid = true;";
                    validator.ErrorMessage = validationRule.ErrorMessage;
                }
                else
                {
                    return null;
                }
                string scriptName = "ValidClientFor" + strWebComboID + "Script";
                string scriptBody = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                     scriptName,
                                                                     functionBody);

                if (!this.Page.ClientScript.IsClientScriptBlockRegistered(scriptName))
                    this.Page.ClientScript.RegisterClientScriptBlock(typeof(COETableManager), scriptName, scriptBody);

                validator.ClientValidationFunction = scriptName;
                validator.ValidateEmptyText = true;

                validator.EnableClientScript = true;
                validator.Display = ValidatorDisplay.Dynamic;

                return validator;
            }
        }
        #endregion

        /// <summary>
        ///  bind the dropdownlist control  with tables' names  
        ///  updated on 2008/11/13 to fix 103856 bugs
        /// </summary>
        private void BindWbcTableName()
        {
            Dictionary<string, string> tableList = COETableEditorUtilities.getTables();
            DataTable dt = new DataTable();
            dt.Columns.Add("TableDisplayName", typeof(string));
            dt.Columns.Add("TableName", typeof(string));
            foreach (KeyValuePair<string, string> pair in tableList)
            {
                DataRow dr = dt.NewRow();
                dr["TableName"] = pair.Key;
                dr["TableDisplayName"] = pair.Value;
                dt.Rows.Add(dr);
            }

            //updated on 2008/11/13 to fix 103856 bugs
            ddlTableName.DataSource = dt;
            ddlTableName.DataTextField = "TableDisplayName";
            ddlTableName.DataValueField = "TableName";
            ddlTableName.DataBind();
            ddlTableName.Items.Insert(0, new ListItem("Choose a table"));

            foreach (ListItem listItem in ddlTableName.Items)
            {
                if (listItem.Value.Trim().Equals(CurrentTable.Trim()))
                {
                    listItem.Selected = true;
                    break;
                }
            }
            //end updated
        }

        /// <summary>
        /// bind the ChildTable webgrid control with the ReturnTable 
        /// </summary>
        /// <param name="pareTblPkValue"></param>
        private void BindWebgridChildTable(string pareTblPkValue)
        {
            if (uwdChildTable != null && COETableEditorUtilities.ChildTablesEnabled(CurrentTable))
            {
                COETableEditorBOList bolist = COETableEditorBOList.NewList();
                bolist.TableName = CurrentTable;
                ReturnDataTable = COETableEditorBOList.getChildDataTable(pareTblPkValue, true);

                //Coverity Bug Fix :- CID : 11853  Jira Id :CBOE-194
                uwdChildTable.Columns.Clear();
                //uwdChildTable = new UltraWebGrid();
                foreach (DataColumn dataColumn in ReturnDataTable.Columns)
                {
                    TemplatedColumn tc = new TemplatedColumn();
                    tc.Header.Caption = dataColumn.ColumnName;
                    tc.Key = dataColumn.ColumnName;
                    tc.BaseColumnName = dataColumn.ColumnName;
                    tc.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    uwdChildTable.Columns.Add(tc);
                }
                if (this.CurrentShowTableMode == ModeOfShowTable.Multi && ReturnDataTable.Rows.Count > uwdChildTable.DisplayLayout.Pager.PageSize)
                    uwdChildTable.DisplayLayout.Pager.AllowPaging = true;
                else //if (uwdChildTable != null) //Coverity Bug Fix :- CID : 11853  Jira Id :CBOE-194
                    uwdChildTable.DisplayLayout.Pager.AllowPaging = false;
                uwdChildTable.DataSource = ReturnDataTable;
                uwdChildTable.DataBind();
            }
        }

        /// <summary>
        ///  bind the webgrid control with the ReturnTable  
        /// </summary>
        private void BindWebgrid()
        {
            COETableEditorBOList boList = COETableEditorBOList.NewList();
            boList.TableName = CurrentTable; //init the dal in TableName property define
            labTableDescription.Text = COETableEditorUtilities.getTableDescription(CurrentTable);
            ReturnDataTable = COETableEditorBOList.getTableEditorDataTable(CurrentTable);
            uwdList.Columns.Clear();

            //add delete button
            if (COETableEditorUtilities.HasDeletePrivileges(CurrentTable))
            {
                UltraGridColumn DeletingColumn = new UltraGridColumn("Deleting", "Delete", ColumnType.Button, "Delete");
                DeletingColumn.CellStyle.Padding.Left = 5;
                DeletingColumn.CellStyle.Padding.Right = 5;
                DeletingColumn.CellButtonDisplay = CellButtonDisplay.Always;
                DeletingColumn.CellButtonStyle.CssClass = GridDeleteButtonStyle;
                DeletingColumn.CellButtonStyle.Padding.Left = 5;
                DeletingColumn.CellButtonStyle.Padding.Right = 5;
                //DeletingColumn.CellButtonStyle.Height = 40;
                DeletingColumn.SortIndicator = SortIndicator.Disabled;
                DeletingColumn.Width = 24;
                uwdList.Bands[0].Columns.Add(DeletingColumn);
            }
            //added on 2008/7/7 for looping the TableEditorData node in xml configure file
            int columnIndex = 0;
            List<Column> lstColumn = COETableEditorUtilities.getColumnList(CurrentTable);

            foreach (DataColumn dataColumn in ReturnDataTable.Columns)
            {
                TemplatedColumn tc = new TemplatedColumn();
                string alias = COETableEditorUtilities.GetAlias(CurrentTable, lstColumn[columnIndex].FieldName);
                tc.Header.Caption = (alias == null ? dataColumn.ColumnName : alias);

                tc.Key = dataColumn.ColumnName;
                tc.BaseColumnName = dataColumn.ColumnName;
                //To set the cell content shown in center .Jerry. 2008-08-06
                tc.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                uwdList.Columns.Add(tc);

                //Updated on 2009-1-5 for InnerXml node not have lookup field
                if (COETableEditorUtilities.getLookupLocation(CurrentTable, lstColumn[columnIndex].FieldName).Length != 0)
                {
                    if (COETableEditorUtilities.getIsStructureLookupField(CurrentTable, lstColumn[columnIndex].FieldName))
                    {
                        _includeStructure = true;
                        tc.Key = "CDXValue_" + columnIndex.ToString();
                        tc.Hidden = true;
                        TemplatedColumn tcChemDraw = new TemplatedColumn();
                        tcChemDraw.Header.Caption = alias == null ? dataColumn.ColumnName : alias;
                        tcChemDraw.Key = "ChemDraw_" + columnIndex.ToString();
                        tcChemDraw.IsBound = false;
                        tcChemDraw.CellTemplate = new IndexTypeChemDraw();
                        uwdList.Columns.Add(tcChemDraw);
                    }
                }
                else
                {
                    //Updated on 2008/11/24 to fix the lookup field bug
                    if (COETableEditorUtilities.getIsStructure(CurrentTable, lstColumn[columnIndex].FieldName))
                    {
                        _includeStructure = true;
                        tc.Key = "CDXValue_" + columnIndex.ToString();
                        tc.Hidden = true;
                        TemplatedColumn tcChemDraw = new TemplatedColumn();
                        tcChemDraw.Header.Caption = alias == null ? dataColumn.ColumnName : alias;
                        tcChemDraw.Key = "ChemDraw_" + columnIndex.ToString();
                        tcChemDraw.IsBound = false;
                        tcChemDraw.CellTemplate = new IndexTypeChemDraw();
                        uwdList.Columns.Add(tcChemDraw);
                    }
                }

                columnIndex = columnIndex + 1;
            }

            //add Edit button
            if (COETableEditorUtilities.HasEditPrivileges(CurrentTable))
            {
                UltraGridColumn EditingColumn = new UltraGridColumn("Editing", "Edit", ColumnType.Button, "Edit");
                EditingColumn.CellStyle.Padding.Left = 5;
                EditingColumn.CellStyle.Padding.Right = 5;
                EditingColumn.CellButtonDisplay = CellButtonDisplay.Always;
                EditingColumn.CellButtonStyle.CssClass = GridEditButtonStyle;
                EditingColumn.CellButtonStyle.Padding.Left = 5;
                EditingColumn.CellButtonStyle.Padding.Right = 5;
                //EditingColumn.CellButtonStyle.Height = 40;
                EditingColumn.SortIndicator = SortIndicator.Disabled;
                EditingColumn.Width = 24;
                uwdList.Bands[0].Columns.Add(EditingColumn);
            }

            foreach (UltraGridColumn column in uwdList.Bands[0].Columns)
            {
                _columnsWidth += Convert.ToInt16(column.WidthResolved.Value);
            }
            if (!string.IsNullOrEmpty(GridSortField) && ReturnDataTable.Columns.Contains(GridSortField))
                ReturnDataTable.DefaultView.Sort = GridSortField + (GridSortIndicator == SortIndicator.Ascending ? " ASC " : " DESC");
            uwdList.DataSource = ReturnDataTable;
            uwdList.DataBind();
            if (!string.IsNullOrEmpty(GridSortField) && ReturnDataTable.Columns.Contains(GridSortField))
                uwdList.Columns.FromKey(GridSortField).SortIndicator = GridSortIndicator;
            SetSpecColumnValue();
            RemoveHiddenColumn();
            this.SetStyles();
            if (ReturnDataTable.Rows.Count > uwdList.DisplayLayout.Pager.PageSize)
                uwdList.DisplayLayout.Pager.AllowPaging = true;
            else
                uwdList.DisplayLayout.Pager.AllowPaging = false;
        }

        /// <summary>
        /// remove the hidden columns for fixing the right border problem
        /// </summary>
        private void RemoveHiddenColumn()
        {
            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn col in uwdList.Columns)
            {
                if (col.Key.StartsWith("CDXValue_"))
                {
                    uwdList.Columns.Remove(col);
                }
            }
        }

        /// <summary>
        /// set the specific columns values, chemdraw,lookupfield etc 
        /// </summary>
        private void SetSpecColumnValue()
        {
            COETableEditorBOList boList = COETableEditorBOList.NewList();
            int columnIndex = 0;
            string fieldValue = string.Empty;
            string lookupField = string.Empty;
            string lookupLocation = string.Empty;
            string alias = string.Empty;
            bool isStructureLookupField = false;
            bool isStructure = false;
            List<Column> lstColumn = COETableEditorUtilities.getColumnList(CurrentTable);

            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in uwdList.Rows)
            {
                for (columnIndex = 0; columnIndex < lstColumn.Count; columnIndex++)
                {
                    isStructureLookupField = false;
                    isStructure = false;
                    lookupField = COETableEditorUtilities.getLookupField(CurrentTable, lstColumn[columnIndex].FieldName);
                    lookupLocation = COETableEditorUtilities.getLookupLocation(CurrentTable, lstColumn[columnIndex].FieldName);
                    isStructureLookupField = COETableEditorUtilities.getIsStructureLookupField(CurrentTable, lstColumn[columnIndex].FieldName);
                    isStructure = COETableEditorUtilities.getIsStructure(CurrentTable, lstColumn[columnIndex].FieldName);
                    alias = COETableEditorUtilities.GetAlias(CurrentTable, lstColumn[columnIndex].FieldName);

                    //Updated on 2009-1-5 for InnerXml node not have lookup field 
                    if (lookupLocation.Length != 0 && lookupLocation.ToLower() != "database")
                    {
                        if (isStructureLookupField)
                        {
                            if (row.Cells.FromKey("CDXValue_" + (columnIndex).ToString()).Value != null)
                            {
                                fieldValue = row.Cells.FromKey("CDXValue_" + (columnIndex).ToString()).Value.ToString();
                                if (lookupLocation.ToLower().Contains("innerxml_"))
                                {
                                    //lookup from coeFrameWork.xml
                                    row.Cells.FromKey("CDXValue_" + (columnIndex).ToString()).Value = COETableEditorUtilities.getLookupValueFromInnerXML(CurrentTable, fieldValue, lstColumn[columnIndex].FieldName);
                                    row.Cells.FromKey("ChemDraw_" + (columnIndex).ToString()).Column.Header.Caption = (alias == null ? lstColumn[columnIndex].FieldName : alias);
                                }
                                else
                                {
                                    row.Cells.FromKey("CDXValue_" + (columnIndex).ToString()).Value = COETableEditorUtilities.getLookupValueFromXML(CurrentTable, fieldValue, lstColumn[columnIndex].FieldName);
                                    row.Cells.FromKey("ChemDraw_" + (columnIndex).ToString()).Column.Header.Caption = (alias == null ? lookupField : alias);
                                }
                            }
                        }
                        //lookup field is not structure
                        else
                        {
                            if (row.Cells.FromKey(lstColumn[columnIndex].FieldName).Value != null)
                            {
                                fieldValue = row.Cells.FromKey(lstColumn[columnIndex].FieldName).Value.ToString();
                                if (lookupLocation.ToLower().Contains("innerxml_"))
                                {
                                    //lookup from coeFrameWork.xml
                                    row.Cells.FromKey(lstColumn[columnIndex].FieldName).Value = COETableEditorUtilities.getLookupValueFromInnerXML(CurrentTable, fieldValue, lstColumn[columnIndex].FieldName);
                                    row.Cells.FromKey(lstColumn[columnIndex].FieldName).Column.Header.Caption = (alias == null ? lstColumn[columnIndex].FieldName : alias);
                                }
                                else
                                {
                                    row.Cells.FromKey(lstColumn[columnIndex].FieldName).Value = COETableEditorUtilities.getLookupValueFromXML(CurrentTable, fieldValue, lstColumn[columnIndex].FieldName);
                                    row.Cells.FromKey(lstColumn[columnIndex].FieldName).Column.Header.Caption = (alias == null ? lookupField : alias);
                                }
                            }
                        }
                    }
                    //lookupLocation property is database,and there is structure in the columns of the webgrid 
                    if (isStructure || isStructureLookupField)
                    {
                        TemplatedColumn tc = (TemplatedColumn)row.Cells.FromKey("ChemDraw_" + (columnIndex).ToString()).Column;
                        CellItem ci = (CellItem)tc.CellItems[row.Index];
                        CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed coeChemDraw = (CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed)ci.FindControl("ChemDraw");
                        coeChemDraw.InlineData = row.Cells.FromKey("CDXValue_" + (columnIndex).ToString()).Text;
                        coeChemDraw.ViewOnly = true;
                    }
                    //There is no lookup field or (lookupLocation property is database,and there is no structure in the columns of the webgrid)
                    //the code had processed
                }
            }
        }

        /// <summary>
        /// Ceate the form when Set Page Size
        /// </summary>
        private void SetPageSize()
        {
            PageSize = 10;
            if (!txtPageSize.Text.Equals(string.Empty))
            {
                int tmpSize = 0;

                if (int.TryParse(txtPageSize.Text.Trim(), out tmpSize))
                {
                    PageSize = tmpSize > 0 ? tmpSize : 10;
                }
            }

            uwdList.DisplayLayout.Pager.CurrentPageIndex = 1;
            CurrentMode = ModeOfAction.Default;
            CreateChildControls();
            BindWebgrid();
            //child table for bind childtablewebgrid
            if (CurrentShowTableMode == ModeOfShowTable.Multi)
            {
                string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                string selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                BindWebgridChildTable(selRowPareTblPkValue);
            }
            //end

            pnlViewTable.Visible = true;
            SetAddButtonVisibility(true);
            lblEditTable.Visible = true;
            labTableDescription.Visible = true;
            pnlPage.Visible = true;

            HideEditPanel();

        }

        /// <summary>
        /// bind Lookup field
        /// </summary>
        /// <param name="wbc">WebCombo control needed to be binded</param>
        /// <param name="parCurrentTable">the current table name</param>
        /// <param name="parFieldName">the current field name</param>
        /// <returns>bind Lookup field</returns>
        private bool BindLookupField(Infragistics.WebUI.WebCombo.WebCombo wbc, string parCurrentTable, string parFieldName)
        {
            wbc.DataValueField = "ID";
            wbc.DataTextField = "PColumn";
            bool IsCdx = COETableEditorUtilities.getIsStructureLookupField(parCurrentTable, parFieldName);
            if (IsCdx)
            {
                wbc.DataSource = (List<ID_Column>)ViewState[parFieldName + "dynamicStrc"];
                Infragistics.WebUI.UltraWebGrid.UltraWebGrid uw = wbc.Rows.Band.Grid;
                Infragistics.WebUI.UltraWebGrid.TemplatedColumn tc = new TemplatedColumn();
                tc.Key = "IndexType";
                tc.BaseColumnName = "IndexType";
                tc.IsBound = false;

                tc.AllowUpdate = AllowUpdate.Yes;
                tc.AllowResize = AllowSizing.Fixed;
                uw.Columns.Add(tc);
                uw.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(uw_InitializeLayout);
                wbc.DataTextField = "IndexType";
            }
            else
            {
                wbc.DataSource = LookupDataSource;
            }
            wbc.DataBind();
            wbc.Columns.FromKey("PColumn").Header.Caption = "";
            wbc.ID = parFieldName;
            if (wbc.Rows.Count > 0)
            {
                UltraGridColumn idColumn = wbc.Columns.FromKey("ID");
                int visibleColumnsCount = wbc.Columns.Count;
                if (idColumn != null)
                {
                    visibleColumnsCount--;
                    idColumn.Hidden = true;
                }
                wbc.DropDownLayout.ColWidthDefault = new Unit("150px");
                wbc.DropDownLayout.RowHeightDefault = new Unit("30px");
                wbc.DropDownLayout.DropdownWidth = new Unit((150 * visibleColumnsCount) + 28);
                wbc.DropDownLayout.DropdownHeight = new Unit((30 * wbc.Rows.Count) + 10);

                if (IsCdx)
                {
                    wbc.Columns.FromKey("PColumn").Hidden = true;
                    foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in wbc.Rows)
                    {
                        TemplatedColumn tc = (TemplatedColumn)row.Cells.FromKey("IndexType").Column;
                        CellItem ci = (CellItem)tc.CellItems[row.Index];
                        CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed coeChemDraw = (CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed)ci.FindControl("ChemDraw");
                        coeChemDraw.InlineData = row.Cells.FromKey("PColumn").Value.ToString();
                        coeChemDraw.ViewOnly = true;
                    }
                    wbc.Columns.Remove(wbc.Columns.FromKey("PColumn"));
                }
            }
            return true;
        }

        private void SetAddButtonVisibility(bool visible)
        {
            if (btnAdd != null)
                btnAdd.Visible = visible;
        }
        #endregion

        #region Initialize

        /// <summary>
        /// page on load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //this does not belong here the appName comes from the ini file or the client setting GlobalContext["AppName"]
            Csla.ApplicationContext.GlobalContext["AppName"] = AppName;
            ConfigurationManager.AppSettings.Set("AppName", AppName);
            this.AppName = COEAppName.Get();
            COETableEditorBOList.Reset();
            //2008-4-16 add by david zhang
            _includeStructure = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Clear the chemdraw control when paging, enter paging bug and confirm delete
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!Page.IsClientScriptBlockRegistered("ConfirmDelete"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function ConfirmDelete(gn,cid)
                {                   
                    if (cid.substring(cid.length-2, cid.length)=='_0')
                    {
                        if(!confirm('Are you sure you want to delete?'))
                            document.getElementById('" + ConfirmDelHiddenField.ClientID + @"').value='false'; 
                        else
                            document.getElementById('" + ConfirmDelHiddenField.ClientID + @"').value='true'; 
                    } 
                }
                </script>";
                Page.RegisterClientScriptBlock("ConfirmDelete", strCode);
            }

            //added to fix set page size enter bug
            Page.RegisterStartupScript("EnterTxtPageSize", @"<script>
            function EnterTextBox(button)
            {
                if(event.keyCode == 13)
                {
                     event.keyCode = 9;
                     event.returnValue = false;
                     document.all[button].click();
                }
             }
            </script>");
            txtPageSize.Attributes.Add("onkeydown", "EnterTextBox('" + btnSetPageSize.ClientID + "')");
            //end added
            if (_processPaging)
            {
                //added on 2008/11/05 to fix 3457 bug
                btnEdit.OnClientClick = "setActionToDo('SUBMIT');";
                //end fix
                Page.RegisterStartupScript("OnLoad", @"<script>
            function setActionToDo(val)
            {
                document.getElementById('" + ActionToDoHiddenField.ClientID + @"').value=val;
            }
            </script>");
                if (_includeStructure)
                {
                    Page.ClientScript.RegisterOnSubmitStatement(typeof(string), Changeinlinejs,
                      @"
                if(cd_objectArray!=null && cd_objectArray!='')
                {
                    for(i = 0; i < cd_objectArray.length; i++) {
                
                        if(document.getElementById(cd_objectArray[i] + 'Output') != null){
                            if(document.getElementById('" + ActionToDoHiddenField.ClientID + @"').value=='SUBMIT')
                                document.getElementById(cd_objectArray[i] + 'Output').value = cd_getData(cd_objectArray[i], 'chemical/x-cdx');
                            else
                                ClearOutputValue(i);
                        }
                    }
                }
                function ClearOutputValue(objIndex) 
                {
                    if (!isNaN(objIndex))
                    {
                        if (document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output') != null)
                            document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output').value = '" + NullOutput + @"';
                    }
                }");
                }
                else
                {
                    Page.ClientScript.RegisterOnSubmitStatement(typeof(string), Changeinlinejs, " ");
                }
            }
            if (_renderMaxPageSizeAlert)
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(COETableManager), "MaxPageSizeAlert"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(COETableManager), "MaxPageSizeAlert", string.Format("alert('{0}');", Resources.MaxPageSizeReached), true);
                }
            }

            if (string.IsNullOrEmpty(this.CurrentTable))
                this.uwdList.Visible = false;

            base.OnPreRender(e);

        }

        #endregion

        #region Control Events
        /// <summary>
        /// InitializeLayout event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uw_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("IndexType"))
            {
                TemplatedColumn IndexTypeCol = (TemplatedColumn)e.Layout.Bands[0].Columns.FromKey("IndexType");
                IndexTypeCol.CellTemplate = new IndexTypeChemDraw();
            }
        }

        /// <summary>
        /// To add button captions. Tao Ran. 2008-07-10
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void uwdList_InitializeRow(object sender, RowEventArgs e)
        {
            if (COETableEditorUtilities.HasEditPrivileges(CurrentTable))
                e.Row.Cells.FromKey("Editing").Value = string.Empty;
            if (COETableEditorUtilities.HasDeletePrivileges(CurrentTable))
                e.Row.Cells.FromKey("Deleting").Value = string.Empty;
            // Look for selected row and mark it
            string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
            if (e.Row.Cells.FromKey(pkFieldName) != null && int.Parse(e.Row.Cells.FromKey(pkFieldName).Value.ToString()) == CurrentRowPK)
            {
                this.SetActiveRow(e.Row);
            }
        }

        /// <summary>
        /// dropdownlist ddlTableName Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddlTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDdlValuesandTables();
        }

        private void LoadDdlValuesandTables()
        {
            if (ddlTableName.SelectedIndex != 0)
            {
                CurrentTable = ddlTableName.SelectedItem.Value.Trim();
                CurrentTableDisplayName = ddlTableName.SelectedItem.Text.Trim();
                if (COETableEditorUtilities.GetIsHasChildTable(CurrentTable) && COETableEditorUtilities.ChildTablesEnabled(CurrentTable))
                {
                    CurrentShowTableMode = ModeOfShowTable.Multi;
                    uwdList.DisplayLayout.Pager.CurrentPageIndex = 1;
                    CurrentMode = ModeOfAction.Default;
                    CreateChildControls();
                    BindWebgrid();
                    string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                    if (uwdList.Rows.Count > 0)
                    {
                        string selRowPareTblPkValue = string.Empty;
                        if (CurrentRowPK <= 0)
                        {
                            selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                            this.SetActiveRow(uwdList.Rows[0]);
                        }
                        else
                            selRowPareTblPkValue = CurrentRowPK.ToString();
                        BindWebgridChildTable(selRowPareTblPkValue);
                        pnlViewChildTable.Visible = true;
                    }
                }
                else
                {
                    CurrentShowTableMode = ModeOfShowTable.Single;
                    uwdList.DisplayLayout.Pager.CurrentPageIndex = 1;
                    CurrentMode = ModeOfAction.Default;
                    CreateChildControls();
                    BindWebgrid();
                }
                pnlViewTable.Visible = true;
                SetAddButtonVisibility(true);
                lblEditTable.Visible = true;
                labTableDescription.Visible = true;
                pnlPage.Visible = true;
                btnRefresh.Visible = true;
                HideEditPanel();
            }
            else
            {
                CurrentTable = string.Empty;
                CurrentTableDisplayName = string.Empty;
                CreateChildControls();
                pnlViewChildTable.Visible = false;
                // to fix CSBR-154568: Use the defined method to set visibility of Add button
                SetAddButtonVisibility(false);
                btnRefresh.Visible = false;
                btnCancel.Visible = btnEdit.Visible = false;
            }
        }

        private void SetActiveRow(UltraGridRow ultraGridRow)
        {
            uwdList.DisplayLayout.ActiveRow = ultraGridRow;
            uwdList.DisplayLayout.ActiveRow.Style.BackColor = Color.LightBlue;
            uwdList.DisplayLayout.ActiveRow.Style.ForeColor = Color.Black;
        }

        /// <summary>
        /// hide edit panel 
        /// </summary>
        void HideEditPanel()
        {
            if (pnlAddRecord != null)
            {
                pnlAddRecord.Visible = false;
            }

            if (pnlEditRecord != null)
            {
                pnlEditRecord.Visible = false;
            }

            if (pnlEditBtn != null)
            {
                pnlEditBtn.Visible = false;
            }
        }

        /// <summary>
        /// parent table webgrid page index changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uwdList_PageIndexChanged(object sender, Infragistics.WebUI.UltraWebGrid.PageEventArgs e)
        {
            if (uwdList.Rows.Count > 0)
            {
                _processPaging = true;
            }

            SetStyles();
            uwdList.DisplayLayout.Pager.CurrentPageIndex = e.NewPageIndex;
            BindWebgrid();
            //child table for bind childtablewebgrid
            if (CurrentShowTableMode == ModeOfShowTable.Multi)
            {
                string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                string selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                BindWebgridChildTable(selRowPareTblPkValue);
            }

            pnlViewTable.Visible = true;
            SetAddButtonVisibility(true);
            lblEditTable.Visible = true;
            labTableDescription.Visible = true;
            pnlPage.Visible = true;

            HideEditPanel();
        }

        /// <summary>
        /// child table webgrid page index changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uwdChildTable_PageIndexChanged(object sender, PageEventArgs e)
        {
            BindWebgrid();
            BindWebgridChildTable(CurrentRowPK.ToString());
            SetStyles();
            uwdChildTable.DisplayLayout.Pager.CurrentPageIndex = e.NewPageIndex;
        }

        /// <summary>
        /// parent table webgrid acitve row changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uwdList_ActiveRowChange(object sender, RowEventArgs e)
        {
            string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
            CurrentRowPK = Convert.ToInt32(e.Row.Cells.FromKey(pkFieldName).Text);
            BindWebgrid();
            SetStyles();
            messageLabel.Text = String.Empty;
            if (ConfirmDelHiddenField.Value.Equals("NotSelDelOption"))
            {
                if (CurrentShowTableMode == ModeOfShowTable.Multi)
                {
                    BindWebgridChildTable(CurrentRowPK.ToString());
                }
                SetAddButtonVisibility(true);
                HideEditPanel();
            }
        }

        /// <summary>
        /// Method added to sort the column (This will sort the entire data associated with the grid)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Sorting_ColumnSorted(object sender, SortColumnEventArgs e)
        {
            if (e.ColumnNo > 0)
            {
                GridSortField = uwdList.Columns[e.ColumnNo].Key;
                GridSortIndicator = uwdList.Columns[e.ColumnNo].SortIndicator;
                BindWebgrid();
            }
        }



        //Add on 2009/02/06 to fix bug 103982

        COELabel messageLabel = new COELabel();
        bool isHasMessage = false;
        /// <summary>
        /// If has message then show the message label.
        /// </summary>
        private void isShowMessageLabel()
        {
            if (isHasMessage)
            {
                messageLabel.Visible = true;
                isHasMessage = false;
            }
            else
            {
                messageLabel.Visible = false;
            }
        }
        //End add

        /// <summary>
        /// parent table webgrid cell button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uwdList_ClickCellButton(object sender, CellEventArgs e)
        {
            this.HasJustSaved = false;
            this.CurrentRowPK = int.Parse(e.Cell.Row.Cells.FromKey(COETableEditorUtilities.getIdFieldName(CurrentTable)).Text);
            switch (e.Cell.Key)
            {
                case "Editing"://click edit button
                    {
                        CurrentMode = ModeOfAction.Update;
                        CreateChildControls();
                        pnlViewTable.Visible = false;

                        pnlAddRecord.Visible = false;
                        pnlEditRecord.Visible = true;
                        pnlEditBtn.Visible = true;
                        break;
                    }
                case "Deleting"://click delete button
                    {
                        //Update on 2009/02/06 to fix bug 103982
                        try
                        {
                            COETableEditorBO tablEditor = COETableEditorBO.New();
                            bool isUsedCheck = true;
                            foreach (Column curCol in COETableEditorUtilities.getColumnList(CurrentTable))
                            {
                                string isUsedCheckVal = COETableEditorUtilities.GetIsUsedCheckProperty(CurrentTable, curCol.FieldName);
                                if (!string.IsNullOrEmpty(isUsedCheckVal) && tablEditor.IsUsedCheck(isUsedCheckVal, e.Cell.Row.Cells.FromKey(curCol.FieldName).Text))
                                {
                                    isHasMessage = true;
                                    isUsedCheck = false;
                                    messageLabel.Text = "The entry is being used and therefore should not be deleted.";
                                    break;
                                }
                            }
                            if (isUsedCheck && ConfirmDelHiddenField.Value.ToLower().Equals("true"))
                            {
                                COETableEditorBO.Delete(CurrentRowPK);
                                ClickRefreshInfoVisible = true;
                            }
                        }
                        catch (Csla.DataPortalException ex)
                        {
                            isHasMessage = true;
                            messageLabel.Text = ex.BusinessException.Message;
                        }
                        finally
                        {
                            CreateChildControls();
                            BindWebgrid();
                            //bind child table webgrid
                            if (CurrentShowTableMode == ModeOfShowTable.Multi)
                            {
                                string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                                string selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                                BindWebgridChildTable(selRowPareTblPkValue);
                            }
                            SetAddButtonVisibility(true);
                            HideEditPanel();
                            uwdList.DisplayLayout.Pager.CurrentPageIndex = 1;
                        }
                        //end update
                        break;
                    }
            }
        }


        /// <summary>
        /// Add button in main panel,click it can create add panel
        /// modified on 2008/11/05 to fix 3457 bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAdd_Click(object sender, EventArgs e)
        {
            CurrentMode = ModeOfAction.Add;
            CreateChildControls();
            if (pnlAddRecord != null)
            {
                pnlAddRecord.Visible = true;
            }

            if (pnlEditBtn != null)
            {
                pnlEditBtn.Visible = true;
            }
            if (pnlEditRecord != null)
            {
                pnlEditRecord.Visible = false;
            }

            pnlViewTable.Visible = false;

            this.HasJustSaved = false;
        }

        /// <summary>
        /// Refresh button in main panel,click it to clear the cache.
        /// modified on 2015/07/23 to fix **** bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRefresh_Click(object sender, EventArgs e)
        {
            clearCacheByDomainID();
            ClickRefreshInfoVisible = false;
            LoadDdlValuesandTables();
        }

        /// <summary>
        /// set page size event
        /// modified on 2008/11/05 to fix 3457 bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSetPageSize_Click(object sender, EventArgs e)
        {
            SetPageSize();
        }

        /// <summary>
        /// cancle button click event
        /// modified on 2008/11/05 to fix 3457 bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCancle_Click(object sender, EventArgs e)
        {
            CurrentMode = ModeOfAction.Default;
            CreateChildControls();
            BindWebgrid();
            //bind child table webgrid
            if (CurrentShowTableMode == ModeOfShowTable.Multi)
            {
                string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                string selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                BindWebgridChildTable(selRowPareTblPkValue);
            }
            pnlViewTable.Visible = true;
            SetAddButtonVisibility(true);

            HideEditPanel();
        }

        #region Add,Update Button click events
        /// <summary>
        /// Add/Update Button click events
        /// added on 2008/11/05 to fix 3457 bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEdit_Click(object sender, EventArgs e)
        {
            bool hasChemDrawContent = false;
            ClickRefreshInfoVisible = true;
            string btnText = btnEdit.Text;
            switch (btnText)
            {
                case "Add":
                    hasChemDrawContent = processAdd();
                    break;

                case "Update":
                    hasChemDrawContent = processUpdate();
                    break;
            }

            if (!_hasValidationError)
            {
                CurrentMode = ModeOfAction.Default;
                CreateChildControls();
                BindWebgrid();
                //bind child table webgrid
                if (CurrentShowTableMode == ModeOfShowTable.Multi)
                {
                    string pkFieldName = COETableEditorUtilities.getIdFieldName(CurrentTable);
                    if (uwdList.Rows.Count > 0)
                    {
                        string selRowPareTblPkValue;
                        if (CurrentRowPK == null)
                            selRowPareTblPkValue = uwdList.Rows[0].Cells.FromKey(pkFieldName).Text;
                        else
                            selRowPareTblPkValue = CurrentRowPK.ToString();

                        BindWebgridChildTable(selRowPareTblPkValue);
                    }
                }
                pnlViewTable.Visible = true;
                labTableDescription.Visible = true;
                SetAddButtonVisibility(true);
                HideEditPanel();
            }
        }

        #region Process Functions

        /// <summary>
        /// Update operation on the Current Record.
        /// </summary>
        private bool processUpdate()
        {
            bool hasChemDrawContent = false;
            Infragistics.WebUI.Misc.WebPanel pnlEdit = (Infragistics.WebUI.Misc.WebPanel)this.FindControl("PnlEditRecord");
            _businessObj = COETableEditorBO.Get(CurrentRowPK);
            hasChemDrawContent = process(_businessObj, pnlEdit);
            return hasChemDrawContent;
        }

        /// <summary>
        /// Add operation on the Table.
        /// </summary>
        private bool processAdd()
        {
            bool hasChemDrawContent = false;
            Infragistics.WebUI.Misc.WebPanel pnlAdd = (Infragistics.WebUI.Misc.WebPanel)this.FindControl("PnlAddRecord");
            _businessObj = COETableEditorBO.New();
            hasChemDrawContent = process(_businessObj, pnlAdd);
            return hasChemDrawContent;
        }

        /// <summary>
        /// To clear the cached values by domain ID.
        /// </summary>
        private void clearCacheByDomainID()
        {
            PickListNameValueList.InvalidateCache();
        }

        /// <summary>
        /// Implements the functionality of Add/ Update according to the object 
        /// of TableEditor has been provided.
        /// </summary>
        /// <param name="obj"> Object of COETableEditorBO</param>
        /// <param name="pnl"> Panel on which the Controls would be found to have the values for the Fields </param>
        private bool process(COETableEditorBO obj, Infragistics.WebUI.Misc.WebPanel pnl)
        {
            bool hasChemDrawContent = true;
            object idFieldValue = null;
            string idField = COETableEditorUtilities.getIdFieldName(CurrentTable);
            if (!this.HasJustSaved)
            {
                List<Column> colList = obj.Columns;

                foreach (Column col in colList)
                {
                    //Updated on 2009-1-5 for InnerXml node not have lookup field
                    if (COETableEditorUtilities.getLookupLocation(CurrentTable, col.FieldName).Length == 0)
                    {
                        COETableEditorBOList boList = COETableEditorBOList.NewList();
                        if (COETableEditorUtilities.getIsStructure(CurrentTable, col.FieldName))
                        {
                            string Base64_CDXData = ((CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed)pnl.FindControl(col.FieldName)).OutputData;
                            col.FieldValue = Base64_CDXData;
                        }
                        else
                        {
                            switch (col.FieldType)
                            {
                                case DbType.Double:
                                    //modified by Jerry on 2008/07/24 for column input value is null
                                    try
                                    {
                                        if (!COETableEditorUtilities.getIdFieldName(CurrentTable).ToLower().Equals(col.FieldName.ToLower()))
                                        {
                                            if (!string.IsNullOrEmpty(((TextBox)pnl.FindControl(col.FieldName)).Text.Trim()))
                                            {
                                                double doubleValue;
                                                if (!double.TryParse(((TextBox)pnl.FindControl(col.FieldName)).Text, out doubleValue))
                                                {
                                                    Page.RegisterClientScriptBlock("ValidationError", "<script language='javascript'>alert('" + col.FieldName + " needs a valid number.')</script>");
                                                    _hasValidationError = true;
                                                }
                                                else
                                                {
                                                    col.FieldValue = doubleValue;
                                                }
                                            }
                                            else
                                            {
                                                col.FieldValue = null;
                                            }
                                        }
                                        else
                                        {
                                            if (CurrentMode == ModeOfAction.Add)
                                                col.FieldValue = null;
                                            idFieldValue = col.FieldValue;
                                        }
                                    }
                                    catch
                                    {
                                        Page.RegisterClientScriptBlock("ValidationError", "<script language='javascript'>alert('" + col.FieldName + " needs a valid number.')</script>");
                                        _hasValidationError = true;
                                    }
                                    break;

                                case DbType.AnsiString:
                                    col.FieldValue = Page.Server.HtmlEncode(((TextBox)pnl.FindControl(col.FieldName)).Text);
                                    break;

                                case DbType.Boolean:
                                    col.FieldValue = ((CheckBox)pnl.FindControl(col.FieldName)).Checked;
                                    break;

                                case DbType.DateTime:
                                    col.FieldValue = ((Infragistics.WebUI.WebSchedule.WebDateChooser)pnl.FindControl(col.FieldName)).Value;
                                    break;
                            }
                        }
                    }
                    else//has look up field
                    {
                        //update to fix 105018 bug link problem
                        WebCombo wbcLookupField = (WebCombo)pnl.FindControl(col.FieldName);
                        col.FieldValue = wbcLookupField.DataValue;
                        //end updated
                        wbcLookupField.Rows.Clear();
                        wbcLookupField.Columns.Clear();
                        wbcLookupField.Rows.Band.Grid.Clear();
                    }
                    // Check for default value if field value is null.
                    if (col.FieldValue == null)
                        col.FieldValue = (!string.IsNullOrEmpty(COETableEditorUtilities.getDefaultValue(CurrentTable, col.FieldName))) ? COETableEditorUtilities.getDefaultValue(CurrentTable, col.FieldName) : null;
                }

                obj.Columns = colList;
                if (COETableEditorUtilities.GetIsHasChildTable(CurrentTable) && COETableEditorUtilities.ChildTablesEnabled(CurrentTable))
                {
                    List<int> childTableData = new List<int>();
                    foreach (ListItem item in rightListBox.Items)
                    {
                        childTableData.Add(Int32.Parse(item.Value));
                    }
                    obj.ChildTableData = childTableData;
                }
                idFieldValue = (idFieldValue == null ? string.Empty : idFieldValue);
                foreach (Column colCheck in obj.Columns)
                {
                    if (COETableEditorUtilities.GetIsUniqueProperty(CurrentTable, colCheck.FieldName))
                    {
                        COETableEditorBO tablEditor = COETableEditorBO.New();
                        if (!tablEditor.IsUniqueCheck(CurrentTable, colCheck.FieldName, colCheck.FieldValue.ToString(), idField, idFieldValue.ToString()))
                        {
                            Page.RegisterClientScriptBlock("ValidationError", "<script language='javascript'>alert('" + colCheck.FieldName + " should be unique')</script>");
                            _hasValidationError = true;
                            break;
                        }
                    }
                }
                // Validate if view [VW_PICKLISTDOMAIN] Note : Add more to the list if contains SQL filters for update
                if (CurrentTable.ToLower() == ValidateSqlQuery.VW_PICKLISTDOMAIN.ToString().ToLower())
                {
                    try
                    {
                        bool proceedToValidate = false;
                        for (int i = 0; i < obj.Columns.Count; i++)
                        {
                            Column col = (Column)obj.Columns[i];
                            switch (col.FieldName)
                            {
                                case "EXT_TABLE":
                                case "EXT_ID_COL":
                                case "EXT_DISPLAY_COL":
                                case "EXT_SQL_FILTER":
                                case "EXT_SQL_SORTORDER":
                                    proceedToValidate = (col.FieldValue.ToString().Trim().Length > 0) ? (i = obj.Columns.Count) > 0 : false;
                                    break;
                            }
                        }
                        if (proceedToValidate)
                            COETableEditorBO.Get(CurrentTable, obj.Columns);
                    }
                    catch (Exception ex)
                    {
                        Page.RegisterClientScriptBlock("ValidationError", "<script language='javascript'>alert('" + ex.GetBaseException().Message.ToString() + "')</script>");
                        _hasValidationError = true;
                    }
                }

                if (!_hasValidationError)
                {
                    /*CSBR-153687
                    Modified By DIVYA
                    Checking for duplicate name for picklist domain under manage customizable table.
                    */
                    try
                    {
                        _businessObj = obj.Save();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("ORA-00001"))
                        {
                            Page.RegisterClientScriptBlock("ValidationError", "<script language='javascript'>alert('The value provided already exists')</script>");
                            _hasValidationError = true;
                        }
                        else
                            throw ex;
                    }
                    //End of CSBR-153687
                    CurrentRowPK = _businessObj.ID;
                }
                /* CSBR-158292 Picklistdomain value is not getting added for first time, after getting the validation message for duplicate entry
                    * Changed by jogi 
                    * checking the _hasValidationError and setting the this.HasJustSaved value to fix the problem
                    * As previously this.HasJustSaved was set to true whether _hasValidationError is true or false */
                if (_hasValidationError)
                    this.HasJustSaved = false;
                else
                    this.HasJustSaved = true;
                /* end of CSBR- 158292 */
            }
            return hasChemDrawContent;
        }

        #endregion
        #endregion
        #endregion

        #region ITemplate
        /// <summary>
        /// Index Type ChemDraw
        /// </summary>
        public class IndexTypeChemDraw : ITemplate
        {
            private string _inlineData = null;
            public CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed CoeChemControl = null;
            [NonSerialized]
            static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");
            public string InlineData
            {
                get { return _inlineData; }
                set { _inlineData = value; }
            }
            public IndexTypeChemDraw()
            {
            }
            public IndexTypeChemDraw(string inlineData)
            {
                _inlineData = inlineData;
            }

            #region ITemplate Members

            /// <summary>
            /// init the chemdraw control
            /// </summary>
            /// <param name="container"></param>
            public void InstantiateIn(Control container)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
                CoeChemControl = new CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed();
                CoeChemControl.ID = "ChemDraw";
                CoeChemControl.Height = 80;
                CoeChemControl.Width = 80;
                CoeChemControl.EnableViewState = true;
                CoeChemControl.ViewOnly = false;
                CoeChemControl.InlineData = _inlineData;
                controlItem.Controls.Add(CoeChemControl);
                _coeLog.LogEnd(methodSignature);
            }
            #endregion
        }
        #endregion
    }
}

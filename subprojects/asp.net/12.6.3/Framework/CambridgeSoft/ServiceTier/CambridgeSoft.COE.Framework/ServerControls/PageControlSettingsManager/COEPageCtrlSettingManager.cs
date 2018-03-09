using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using Infragistics.WebUI.Shared;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Web;
using Infragistics.WebUI.UltraWebGrid;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEPageCtrlSettingManager", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEPageCtrlSettingManager
{
    /// <summary>
    /// A user defined control is used to set page's controls.
    /// </summary>
    [ToolboxData("<{0}:COEPageCtrlSettingManager runat=server></{0}:COEPageCtrlSettingManager>")]
    public class COEPageCtrlSettingManager : CompositeControl, INamingContainer
    {
        #region Constant

        //Resources Path
        private const string THEMES_PATH = "CambridgeSoft.COE.Framework.ServerControls.PageControlSettingsManager.Themes.";
        private const string IMAGE_PATH_FILE = THEMES_PATH + "Images.file.gif";
        private const string IMAGE_PATH_FOLDER = THEMES_PATH + "Images.folder.gif";
        private const string IMAGE_PATH_BUTTON = THEMES_PATH + "Images.buttonimage.gif";
        private const string IMAGE_PATH_TOOLBAR_ROW = THEMES_PATH + "Images.backgroundtoolbar.gif";
        private const string IMAGE_PATH_TOOLBAR_TABLE = THEMES_PATH + "Images.watermark.jpg";
        private const string CSS_PATH = THEMES_PATH + "Styles_PageControl.css";

        //Error Message
        private const string MESSAGE_REQUIRED_ID = "ID is required.";
        private const string MESSAGE_EXIST_ID = "This ID has already exist.";
        private const string MESSAGE_MAXLENGTH_ID = "The length must be less than 150.";
        private const string MESSAGE_REQUIRED_PRIVILEGES = "Privileges is required.";
        private const string MESSAGE_REQUIRED_CONTROLS = "Controls is required.";

        #endregion

        #region Enums

        private enum ModeOfAction
        {
            Default, Add, Update
        }

        private enum TypeOfListBox
        {
            Privilege, Control, AppSettings
        }

        private enum ValidationName
        {
            Required, TextLength, Custom
        }

        //CSS Styles
        private enum ControlsStyle
        {
            PageControlContentTable,
            PageControlTree,
            PageControlButton,
            PageControlTitleLabel,
            PageControlRow,
            PageControlBarTable,
            PageControlCell,
            PageControlListBox
        }

        #endregion

        #region Controls Declarations

        Table contentTable = null;
        Panel leftPanel = null;
        Panel rightPanel = null;
        UltraWebTree treeView = null;

        Button btnDone = null;
        Button btnAddOrUpdate = null;
        Button btnDelete = null;
        Button btnCancel = null;
        Button btnBack = null;
        RadioButtonList rblOrAndPrivList = null;
        RadioButtonList rblOrAndAppSettings = null;

        ListBox availablePrivListBox = null;
        ListBox currentPrivListBox = null;
        ListBox availableCtrlListBox = null;
        ListBox currentCtrlListBox = null;

        TextBox idTextBox = null;
        Label titleLabel = null;

        UltraWebGrid appSettingGrid = null;

        //Saving the flag for deleting.
        HiddenField confirmDelHiddenField = null;
        //Save the privileges' id that selected.
        HiddenField privsIDHiddenField = null;
        //Save the controls' id that selected.
        HiddenField ctrlsIDHiddenField = null;
        //Save the controls' id that selected.
        HiddenField appSettingsIDHiddenField = null;
        //Save PrivilegeListID of a page
        HiddenField privilegeListIDs = null;
        //Save PrivilegeListID of PrivilegeListNode selected.
        HiddenField privilegeListID = null;

        #endregion

        #region Variables
        string _appName = string.Empty;
        #endregion

        #region Properties

        /// <summary>
        /// Get the name of current application.
        /// </summary>
        public string AppName
        {
            set { _appName = value; }
            get
            {
                if (ViewState["AppName"] != null)
                    return (string)ViewState["AppName"];
                else if (!string.IsNullOrEmpty(_appName))
                    return _appName;
                return COEAppName.Get();
            }
        }

        /// <summary>
        /// Sets the url for the Back button.
        /// </summary>
        public string PreviousPage
        {
            set { ViewState["PrevPage"] = value; }
            get { return ViewState["PrevPage"] != null ? ViewState["PrevPage"].ToString() : string.Empty; }
        }

        /// <summary>
        /// COEPageControlSettings object
        /// </summary>
        private COEPageControlSettings _pageControlSettings = null;
        private COEPageControlSettings PageControlSettings
        {
            get
            {
                _pageControlSettings = (COEPageControlSettings)ViewState["PageControlSettings"];
                if (ViewState["PageControlSettings"] == null)
                {
                    _pageControlSettings = COEPageControlSettings.GetSettings(AppName);
                }
                return _pageControlSettings;
            }
            set { ViewState["PageControlSettings"] = value; }
        }

        /// <summary>
        /// Current mode
        /// </summary>
        private ModeOfAction CurrentMode
        {
            get
            {
                if (ViewState["CurrentMode"] == null)
                    CurrentMode = ModeOfAction.Default;
                return (ModeOfAction)ViewState["CurrentMode"];
            }
            set { ViewState["CurrentMode"] = value; }
        }

        #endregion

        #region CreateChildControls

        /// <summary>
        /// Main Function that draws the Controls. contains the logic to implement the Control 
        /// and binding them with the Events dynamically on the basis of the data that has been 
        /// fetched from the PageControlSetting Service.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.CreateHiddenFields();
            this.CreateToolBar();
            this.CreatContentTable();
            this.SetControlsStyle();
            this.SetControlsAttributtes();
        }

        /// <summary>
        /// It contains three HiddenFields.
        /// One for Saving the privileges' id which in the control named 'currentPrivListBox'.
        /// One for Saving the controls' id which in the control named 'currentCtrlListBox'.
        /// One for Saving the flag for deleting.
        /// </summary>
        private void CreateHiddenFields()
        {
            confirmDelHiddenField = new HiddenField();
            this.Controls.Add(confirmDelHiddenField);
            privsIDHiddenField = new HiddenField();
            this.Controls.Add(privsIDHiddenField);
            ctrlsIDHiddenField = new HiddenField();
            this.Controls.Add(ctrlsIDHiddenField);
            privilegeListIDs = new HiddenField();
            this.Controls.Add(privilegeListIDs);
            privilegeListID = new HiddenField();
            this.Controls.Add(privilegeListID);
            appSettingsIDHiddenField = new HiddenField();
            this.Controls.Add(appSettingsIDHiddenField);
        }

        /// <summary>
        /// It contains a Button named 'Done', a Button named 'Cancel' 
        /// and a Label for displaying title.
        /// </summary>
        private void CreateToolBar()
        {
            Table table = new Table();
            table.CellSpacing = 0;
            table.CssClass = ControlsStyle.PageControlBarTable.ToString();
            table.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_TOOLBAR_TABLE) + ")");

            TableRow row = new TableRow();
            row.CssClass = ControlsStyle.PageControlRow.ToString();
            row.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_TOOLBAR_ROW) + ")");

            TableCell cell = new TableCell();

            //Back Button
            btnBack = new Button();
            btnBack.CausesValidation = false;
            btnBack.Text = "Back";
            btnBack.Click += new EventHandler(BtnBack_Click);
            cell.Controls.Add(btnBack);

            //Done Button
            btnDone = new Button();
            btnDone.CausesValidation = false;
            btnDone.Text = "Done";
            btnDone.Click += new EventHandler(BtnDone_Click);
            cell.Controls.Add(btnDone);

            //Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.CausesValidation = false;
            btnCancel.Click += new EventHandler(BtnCancel_Click);
            cell.Controls.Add(btnCancel);
            row.Cells.Add(cell);


            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Right;
            //Title Lable
            Label titleLable = new Label();
            titleLable.Text = "Page Control Settings";
            titleLable.ControlStyle.CssClass = ControlsStyle.PageControlTitleLabel.ToString();
            cell.Controls.Add(titleLable);
            row.Cells.Add(cell);
            table.Rows.Add(row);
            this.Controls.Add(table);
        }

        /// <summary>
        /// It contains two Panels.
        /// One is named "leftPanel" for layout of tree.
        /// One is named "rightPanel" for layout of form.
        /// </summary>
        private void CreatContentTable()
        {
            contentTable = new Table();
            TableRow row = new TableRow();

            //Create left panel
            TableCell cell = new TableCell();
            cell.Width = row.Width = new Unit("300px");
            this.CreateLeftPanel();
            cell.Controls.Add(this.leftPanel);
            row.Cells.Add(cell);

            //Create right panel
            cell = new TableCell();
            cell.Width = new Unit("300px");
            this.CreateRightPanel();
            cell.Controls.Add(this.rightPanel);
            row.Cells.Add(cell);

            contentTable.Rows.Add(row);
            this.Controls.Add(contentTable);
        }

        /// <summary>
        /// It contains a tree view type of UltraWebTree.
        /// </summary>
        private void CreateLeftPanel()
        {
            this.leftPanel = new Panel();

            //Create a tree
            this.treeView = PageControlSettings.CreateTree();
            this.treeView.NodeSelectionChanged += new NodeSelectionChangeEventHandler(TreeNodeSelectionChanged);
            this.treeView.Cursor = Infragistics.WebUI.Shared.Cursors.Default;
            this.treeView.WebTreeTarget = WebTreeTarget.ClassicTree;
            this.treeView.ParentNodeImageUrl = this.GetImageUrl(IMAGE_PATH_FOLDER);
            this.treeView.LeafNodeImageUrl = this.GetImageUrl(IMAGE_PATH_FILE);
            this.treeView.Indentation = 20;
            Padding paddings = new Padding();
            paddings.Left = 2;
            paddings.Right = 2;
            paddings.Top = 2;
            paddings.Bottom = 2;
            this.treeView.NodePaddings = paddings;
            this.treeView.HoverNodeStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
            this.treeView.HoverNodeStyle.ForeColor = Color.White;
            this.treeView.HoverNodeStyle.Font.Bold = true;
            this.treeView.HoverNodeStyle.BackColor = Color.MediumSlateBlue;
            this.treeView.Width = new Unit("300px");
            this.leftPanel.Controls.Add(treeView);
        }

        /// <summary>
        /// It contains a table for layout of form.
        /// The form contains PrivilegeListID, PrivilegeListOperator, Privileges, Controls.
        /// </summary>
        private void CreateRightPanel()
        {
            this.rightPanel = new Panel();

            Table table = null;
            TableRow row = null;
            TableCell cell = null;

            table = new Table();

            //PrivilegeList ID Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString();

            //Description of Rule Name
            Label lblIDDescription = new Label();
            lblIDDescription.Text = "Give your Page Control Rule a name";
            lblIDDescription.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(lblIDDescription);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();

            //Label to mark as required
            Label lblMark = new Label();
            lblMark.Text = "*";
            lblMark.ForeColor = Color.Red;

            //Rule Name label
            Label lblID = new Label();
            lblID.Text = "Rule Name:";

            cell.Controls.Add(lblMark);
            cell.Controls.Add(lblID);
            row.Cells.Add(cell);

            cell = new TableCell();
            idTextBox = new TextBox();
            idTextBox.Style.Add("width", "250px");
            idTextBox.ID = "textBox_ID";
            cell.Controls.Add(idTextBox);
            row.Cells.Add(cell);


            //Validation controls for rule name (id)
            cell = new TableCell();
            cell.Style.Add("width", "250px");
            BaseValidator requiredValidator = this.CreateValidator(ValidationName.Required, idTextBox, MESSAGE_REQUIRED_ID, string.Empty);
            cell.Controls.Add(requiredValidator);
            BaseValidator textLengthValidator = this.CreateValidator(ValidationName.TextLength, idTextBox, MESSAGE_MAXLENGTH_ID, string.Empty);
            cell.Controls.Add(textLengthValidator);
            BaseValidator customValidator = this.CreateValidator(ValidationName.Custom, idTextBox, MESSAGE_EXIST_ID, "ValidClientForID");
            cell.Controls.Add(customValidator);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //Description of Control Selection
            row = new TableRow();
            cell = new TableCell();

            LiteralControl sp1 = new LiteralControl("<br />");

            Label lblControlDescription = new Label();
            lblControlDescription.Text = "Select one or more controls you wish the rule to apply to";
            lblControlDescription.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(sp1);
            cell.Controls.Add(lblControlDescription);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //Controls Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString();
            cell.ColumnSpan = 2;

            //Mark as required
            lblMark = new Label();
            lblMark.Text = "*";
            lblMark.ForeColor = Color.Red;

            //Controls label
            Label lblControls = new Label();
            lblControls.Text = "Controls:";
            cell.Controls.Add(lblMark);
            cell.Controls.Add(lblControls);
            row.Cells.Add(cell);
            table.Rows.Add(row);


            row = new TableRow();
            cell = new TableCell();
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Controls.Add(CreateListBoxTable(ref availableCtrlListBox, ref currentCtrlListBox, TypeOfListBox.Control));
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Style.Add("width", "250px");
            BaseValidator validatorControl = this.CreateValidator(ValidationName.Custom, currentCtrlListBox, MESSAGE_REQUIRED_CONTROLS, "ValidClientForControl");
            cell.Controls.Add(validatorControl);
            row.Cells.Add(cell);
            table.Rows.Add(row);


            row = new TableRow();
            cell = new TableCell();

            //Description of Rule Name
            LiteralControl sp2 = new LiteralControl("<br /><br />");

            Label lblCOperatorDescription = new Label();
            lblCOperatorDescription.Text = "Which operator do you want to use between privileges  (priv1 AND priv2, priv1 OR priv2)";
            lblCOperatorDescription.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(sp2);
            cell.Controls.Add(lblCOperatorDescription);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //PrivilegeList Operator Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString();
            cell.Text = "Operator:";
            row.Cells.Add(cell);
            cell = new TableCell();
            this.rblOrAndPrivList = new RadioButtonList();
            CreateRadioButtonGroup(ref cell, ref this.rblOrAndPrivList);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();

            //Description of Privilege selection
            LiteralControl sp3 = new LiteralControl("<br /><br />");

            Label lblPrivDescription = new Label();
            lblPrivDescription.Text = "Select the privileges required to see a control";
            lblPrivDescription.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(sp3);
            cell.Controls.Add(lblPrivDescription);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //Privileges Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString(); ;
            cell.ColumnSpan = 2;
            lblMark = new Label();
            lblMark.Text = "*";
            lblMark.ForeColor = Color.Red;
            Label lblPrivileges = new Label();
            lblPrivileges.Text = "Privileges:";
            cell.Controls.Add(lblMark);
            cell.Controls.Add(lblPrivileges);
            row.Cells.Add(cell);
            table.Rows.Add(row);
            row = new TableRow();
            cell = new TableCell();
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Controls.Add(CreateListBoxTable(ref availablePrivListBox, ref currentPrivListBox, TypeOfListBox.Privilege));
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Style.Add("width", "250px");
            BaseValidator validatorPrivilege = this.CreateValidator(ValidationName.Custom, currentPrivListBox, MESSAGE_REQUIRED_PRIVILEGES, "ValidClientForPrivilege");
            cell.Controls.Add(validatorPrivilege);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();

            //Description of Rule Name
            LiteralControl sp4 = new LiteralControl("<br /><br />");

            Label lblAOperator = new Label();
            lblAOperator.Text = "Which operator do you want to use between settings  (configSetting1 AND sessionVar1, configSetting1 OR sessionVar1)";
            lblAOperator.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(sp4);
            cell.Controls.Add(lblAOperator);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //AppSettings Operator Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString();
            cell.Text = "Operator:";
            row.Cells.Add(cell);
            cell = new TableCell();
            this.rblOrAndAppSettings = new RadioButtonList();
            CreateRadioButtonGroup(ref cell, ref this.rblOrAndAppSettings);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();

            //Description of Rule Name
            LiteralControl sp5 = new LiteralControl("<br /><br />");

            Label lblConfigSettings = new Label();
            lblConfigSettings.Text = "Which config settings do you want to control access to your control";
            lblConfigSettings.Font.Bold = true;

            cell.ColumnSpan = 2;
            cell.Controls.Add(sp5);
            cell.Controls.Add(lblConfigSettings);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //AppSettings Row
            row = new TableRow();
            cell = new TableCell();
            cell.ControlStyle.CssClass = ControlsStyle.PageControlCell.ToString();
            cell.ColumnSpan = 2;

            //required

            // Fix Coverity: CID-28936 Comment never used control
            ////lblMark = new Label();
            ////lblMark.Text = "*";
            ////lblMark.ForeColor = Color.Red;

            // Fix Coverity: CID-28937 Comment never used variable
            //label for app settings
            ////Label lblAppSettings = new Label();
            ////lblAppSettings.Text = "Application Settings:";

            //grid for display of available settings for selection
            this.appSettingGrid = new UltraWebGrid();
            appSettingGrid.DisplayLayout.AutoGenerateColumns = false;
            appSettingGrid.BorderWidth = new Unit(1);
            appSettingGrid.BorderStyle = BorderStyle.Solid;
            appSettingGrid.CellPadding = 3;

            TemplatedColumn tc = new TemplatedColumn();
            tc.Header.Caption = "Key";
            tc.Key = "Key";
            tc.BaseColumnName = "Key";
            tc.Header.Style.Font.Bold = true;
            tc.CellStyle.BorderWidth = new Unit(1);
            tc.CellStyle.BorderStyle = BorderStyle.Solid;
            tc.CellStyle.Padding.Left = new Unit(2);
            tc.CellStyle.Padding.Right = new Unit(2);
            tc.Width = 50;
            tc.CellStyle.HorizontalAlign = HorizontalAlign.Left;

            appSettingGrid.DisplayLayout.AutoGenerateColumns = false;
            appSettingGrid.Bands.Add(new UltraGridBand());
            appSettingGrid.Columns.Clear();
            appSettingGrid.Bands[0].Columns.Add(tc);

            tc = new TemplatedColumn();
            tc.Header.Caption = "Value";
            tc.Key = "Value";
            tc.BaseColumnName = "Value";
            tc.Header.Style.Font.Bold = true;
            tc.CellStyle.BorderWidth = new Unit(1);
            tc.CellStyle.BorderStyle = BorderStyle.Solid;
            tc.CellStyle.Padding.Left = new Unit(2);
            tc.CellStyle.Padding.Right = new Unit(2);
            tc.AllowUpdate = AllowUpdate.Yes;
            tc.Width = 30;
            tc.CellStyle.HorizontalAlign = HorizontalAlign.Left;
            appSettingGrid.Columns.Add(tc);

            tc = new TemplatedColumn();
            tc.Header.Caption = "Type";
            tc.Key = "Type";
            tc.BaseColumnName = "Type";
            tc.Header.Style.Font.Bold = true;
            tc.CellStyle.BorderWidth = new Unit(1);
            tc.CellStyle.BorderStyle = BorderStyle.Solid;
            tc.CellStyle.Padding.Left = new Unit(2);
            tc.CellStyle.Padding.Right = new Unit(2);
            tc.Width = 20;
            tc.CellStyle.HorizontalAlign = HorizontalAlign.Left;
            appSettingGrid.Columns.Add(tc);

            tc = new TemplatedColumn();
            tc.Header.Caption = "Enabled";
            tc.Key = "Enabled";
            tc.BaseColumnName = "Enabled";
            tc.Header.Style.Font.Bold = true;
            tc.CellStyle.BorderWidth = new Unit(1);
            tc.CellStyle.BorderStyle = BorderStyle.Solid;
            tc.CellStyle.Padding.Left = new Unit(2);
            tc.CellStyle.Padding.Right = new Unit(2);
            tc.CellStyle.HorizontalAlign = HorizontalAlign.Left;
            tc.AllowUpdate = AllowUpdate.Yes;
            tc.Width = 20;
            tc.Type = Infragistics.WebUI.UltraWebGrid.ColumnType.CheckBox;
            appSettingGrid.Columns.Add(tc);

            tc = new TemplatedColumn();
            tc.Header.Caption = "ID";
            tc.Key = "ID";
            tc.BaseColumnName = "ID";
            tc.Header.Style.Font.Bold = true;
            tc.CellStyle.BorderWidth = new Unit(1);
            tc.CellStyle.BorderStyle = BorderStyle.Solid;
            tc.CellStyle.Padding.Left = new Unit(2);
            tc.CellStyle.Padding.Right = new Unit(2);
            tc.Hidden = true;
            appSettingGrid.Columns.Add(tc);
            cell.Controls.Add(appSettingGrid);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            //Button Group 
            row = new TableRow();
            cell = new TableCell();
            cell.ColumnSpan = 2;
            cell.HorizontalAlign = HorizontalAlign.Center;
            CreateButtonGroup(ref cell);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            this.rightPanel.Controls.Add(table);
        }

        /// <summary>
        /// It contains two RadioButtons;
        /// One is "OR".
        /// One is "AND"
        /// </summary>
        private void CreateRadioButtonGroup(ref TableCell cell, ref RadioButtonList rbl)
        {
            rbl.Items.Add(new ListItem("OR"));
            rbl.Items.Add(new ListItem("AND"));
            rbl.RepeatDirection = RepeatDirection.Horizontal;
            cell.Controls.Add(rbl);
        }

        /// <summary>
        /// Create Priviledges and Controls' ListBox.
        /// </summary>
        /// <returns></returns> 
        private Table CreateListBoxTable(ref ListBox availableListBox, ref ListBox currentListBox, TypeOfListBox type)
        {
            Table mainTable = new Table();
            Table operateTable = new Table();
            TableRow row = null;
            TableCell cell = null;
            Button moveToRightButton = new Button();
            Button moveToLeftButton = new Button();
            //Coverity Fix CID : 11836 
            try
            {

                //Create the MoveToRightButton.
                row = new TableRow();
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;

                moveToRightButton.Text = "-->";
                moveToRightButton.CausesValidation = false;
                cell.Controls.Add(moveToRightButton);
                row.Cells.Add(cell);
                operateTable.Rows.Add(row);

                //Create the MoveToLeftButton
                row = new TableRow();
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;

                moveToLeftButton.Text = "<--";
                moveToLeftButton.CausesValidation = false;

                cell.Controls.Add(moveToLeftButton);
                row.Cells.Add(cell);
                operateTable.Rows.Add(row);

                row = new TableRow();
                cell = new TableCell();
                cell.Text = "Available";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell = new TableCell();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "Current";
                row.Cells.Add(cell);
                mainTable.Rows.Add(row);

                //Create the left ListBox.
                row = new TableRow();
                cell = new TableCell();
                availableListBox = new ListBox();
                availableListBox.SelectionMode = ListSelectionMode.Multiple;
                availableListBox.Width = new Unit("250px");
                cell.Controls.Add(availableListBox);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Controls.Add(operateTable);
                row.Cells.Add(cell);

                //Create the right ListBox.
                cell = new TableCell();
                cell.VerticalAlign = VerticalAlign.Middle;
                currentListBox = new ListBox();
                currentListBox.ID = type.ToString();
                currentListBox.SelectionMode = ListSelectionMode.Multiple;
                currentListBox.Width = new Unit("250px");
                cell.Controls.Add(currentListBox);
                row.Cells.Add(cell);
                mainTable.Rows.Add(row);

                switch (type)
                {
                    case TypeOfListBox.Privilege:
                        moveToRightButton.Attributes.Add("onclick", "return MoveToRightPrivil();");
                        moveToLeftButton.Attributes.Add("onclick", "return MoveToLeftPrivil();");
                        break;
                    case TypeOfListBox.Control:
                        moveToRightButton.Attributes.Add("onclick", "return MoveToRightControl();");
                        moveToLeftButton.Attributes.Add("onclick", "return MoveToLeftControl();");
                        break;

                    case TypeOfListBox.AppSettings:
                        moveToRightButton.Attributes.Add("onclick", "return MoveToRightAppSetting();");
                        moveToLeftButton.Attributes.Add("onclick", "return MoveToLeftAppSetting();");
                        break;

                }

                return mainTable;

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
                mainTable.Dispose();
                moveToLeftButton.Dispose();
                moveToRightButton.Dispose();
            }
        }

        /// <summary>
        /// It contains two Buttons.
        /// One is named "Update" for adding or updating a PrivilegeList node.
        /// One is named "Delete" for deleting a PrivilegeList node.
        /// </summary>
        /// <param name="cell"></param>
        private void CreateButtonGroup(ref TableCell cell)
        {
            btnAddOrUpdate = new Button();
            btnAddOrUpdate.Text = "Update";
            btnAddOrUpdate.Click += new EventHandler(BtnAddOrUpdate_Click);
            cell.Controls.Add(btnAddOrUpdate);

            btnDelete = new Button();
            btnDelete.CausesValidation = false;
            btnDelete.Attributes.Add("onclick", "return ConfirmDelete();");
            btnDelete.Text = "Delete";
            btnDelete.Click += new EventHandler(BtnDelete_Click);
            cell.Controls.Add(btnDelete);
        }
        /// <summary>
        /// Set child controls' styles
        /// </summary>
        private void SetControlsStyle()
        {
            contentTable.CssClass = ControlsStyle.PageControlContentTable.ToString();
            treeView.CssClass = ControlsStyle.PageControlTree.ToString();

            btnBack.ControlStyle.CssClass = ControlsStyle.PageControlButton.ToString(); ;
            btnBack.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_BUTTON) + ")");
            btnDone.ControlStyle.CssClass = ControlsStyle.PageControlButton.ToString(); ;
            btnDone.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_BUTTON) + ")");
            btnCancel.ControlStyle.CssClass = ControlsStyle.PageControlButton.ToString(); ;
            btnCancel.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_BUTTON) + ")");
            btnAddOrUpdate.ControlStyle.CssClass = ControlsStyle.PageControlButton.ToString(); ;
            btnAddOrUpdate.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_BUTTON) + ")");
            btnDelete.ControlStyle.CssClass = ControlsStyle.PageControlButton.ToString(); ;
            btnDelete.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + this.GetImageUrl(IMAGE_PATH_BUTTON) + ")");

            availablePrivListBox.CssClass = ControlsStyle.PageControlListBox.ToString(); ;
            currentPrivListBox.CssClass = ControlsStyle.PageControlListBox.ToString(); ;
            availableCtrlListBox.CssClass = ControlsStyle.PageControlListBox.ToString(); ;
            currentCtrlListBox.CssClass = ControlsStyle.PageControlListBox.ToString(); ;
        }

        #endregion

        #region Events

        /// <summary>
        /// EventHandler for NodeClicked events. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeNodeSelectionChanged(object sender, WebTreeNodeEventArgs e)
        {
            //Select the Page node.
            if (e.Node.DataKey is COEPageControlSettingsService.Page)
            {
                CurrentMode = ModeOfAction.Add;
                this.BindFormData();
            }
            //Select the Privilege node.
            else if (e.Node.DataKey is ControlSetting)
            {
                CurrentMode = ModeOfAction.Update;
                this.BindFormData();
            }
            else
            {
                CurrentMode = ModeOfAction.Default;
            }

            SetControlsAttributtes();
        }

        /// <summary>
        /// EventHandler for AddOrUpdateButton Click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnAddOrUpdate_Click(object sender, EventArgs e)
        {
            COEPageControlSettingsService.Page currentPage = null;
            ControlSetting currentControlSetting = null;

            switch (CurrentMode)
            {
                case ModeOfAction.Add:
                    currentPage = (COEPageControlSettingsService.Page)treeView.SelectedNode.DataKey;
                    currentControlSetting = new ControlSetting();

                    break;
                case ModeOfAction.Update:
                    currentPage = (COEPageControlSettingsService.Page)treeView.SelectedNode.Parent.DataKey;
                    currentControlSetting = (ControlSetting)treeView.SelectedNode.DataKey;
                    break;
            }

            ControlSetting newControlSetting = new ControlSetting();
            newControlSetting.ID = idTextBox.Text;

            newControlSetting.PrivilegeList = new PrivilegeList();
            newControlSetting.PrivilegeList.OperatorToApply = rblOrAndPrivList.SelectedIndex == 0 ? PrivilegeList.Operators.OR : PrivilegeList.Operators.AND;

            newControlSetting.AppSettingList = new AppSettingList();
            newControlSetting.AppSettingList.OperatorToApply = rblOrAndAppSettings.SelectedIndex == 0 ? AppSettingList.Operators.OR : AppSettingList.Operators.AND;

            newControlSetting.CtrlList = new ControlList();

            string[] privilSelValues = privsIDHiddenField.Value.Split(char.Parse("|"));
            string[] ctrlSelValues = ctrlsIDHiddenField.Value.Split(char.Parse("|"));

            //Add the selected privileges to PrivilegeList object
            foreach (string id in privilSelValues)
            {
                newControlSetting.PrivilegeList.Add(PageControlSettings.FullListOfPrivileges.GetByID(id));
            }
            //Add the selected controls to PrivilegeList object
            foreach (string id in ctrlSelValues)
            {
                //Coverity Bug Fix CID 11660 
                if (currentPage != null)
                    newControlSetting.CtrlList.Add(currentPage.CtrlList.GetByID(id));
            }

            AppSettingList fullAppSettingList = PageControlSettings.FullListOfAppSettings;
            //Add the selected appsettings to AppSettingList object

            if (fullAppSettingList != null)
            {
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSetting appSettingItem = null;
                foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in this.appSettingGrid.Rows)
                {
                    if (Boolean.Parse(row.Cells.FromKey("Enabled").Value.ToString()) == true)
                    {
                        appSettingItem = CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSetting.NewAppSetting(row.Cells.FromKey("ID").Value.ToString(), row.Cells.FromKey("Key").Value.ToString(), row.Cells.FromKey("Value").Value.ToString(), row.Cells.FromKey("Type").Value.ToString());
                        newControlSetting.AppSettingList.Add(appSettingItem);
                    }
                }
            }
            COEPageControlSettingsService.Page page = null;
            //Coverity Bug Fix CID 11660 
            if (currentPage != null)
                page = _pageControlSettings.PageList.GetByID(currentPage.ID);
            if (page != null)
                page.AddORUpdateControlSetting(newControlSetting);

            PageControlSettings = _pageControlSettings;
            if (CurrentMode == ModeOfAction.Update)
            {
                treeView.SelectedNode.Parent.Nodes.Remove(treeView.SelectedNode);
                treeView.SelectedNode.Parent.Nodes.Add(newControlSetting.CreateControlSettingNode());
            }
            else if (CurrentMode == ModeOfAction.Add)
            {
                treeView.SelectedNode.Nodes.Add(newControlSetting.CreateControlSettingNode());
            }

            CurrentMode = ModeOfAction.Default;
            SetControlsAttributtes();
        }

        /// <summary>
        /// EventHandler for BtnDone Click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnDone_Click(object sender, EventArgs e)
        {
            PageControlSettings.UpdateSettings(AppName);
            Page.Response.Write("<script>function window.onload() {alert('The settings were successfully saved');}</script>");
            treeView.SelectedNode = null;
            CurrentMode = ModeOfAction.Default;
            if (HttpContext.Current.Session != null)
            {
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList ctrls = COEPageControlSettings.GetControlListToDisableForCurrentUser(AppName.ToUpper());
                if (ctrls != null) HttpContext.Current.Session[CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.COEPageSettings + AppName.ToUpper()] = ctrls;
            }

            SetControlsAttributtes();
        }

        /// <summary>
        /// EventHandler for BtnDelete Click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnDelete_Click(object sender, EventArgs e)
        {
            if (confirmDelHiddenField.Value.Equals("true"))
            {
                COEPageControlSettingsService.Page currentPage = (COEPageControlSettingsService.Page)treeView.SelectedNode.Parent.DataKey;
                ControlSetting currentControlSetting = (ControlSetting)treeView.SelectedNode.DataKey;
                // Coverity Fix CID - 10905 (from local server)
                if (currentPage != null)
                {
                    COEPageControlSettingsService.Page thePage = _pageControlSettings.PageList.GetByID(currentPage.ID);
                    if(thePage != null)
                        thePage.RemoveControlSetting(currentControlSetting);
                }
                PageControlSettings = _pageControlSettings;
                treeView.SelectedNode.Parent.Nodes.Remove(treeView.SelectedNode);
                CurrentMode = ModeOfAction.Default;
                SetControlsAttributtes();
            }
        }

        /// <summary>
        /// EventHandler for BtnCancel Click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnCancel_Click(object sender, EventArgs e)
        {
            //Page.Response.Redirect(ViewState["UrlReferrer"].ToString());
            treeView.SelectedNode = null;
            CurrentMode = ModeOfAction.Default;
            this.SetControlsAttributtes();
        }

        /// <summary>
        /// EventHandler for BtnCancel Click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnBack_Click(object sender, EventArgs e)
        {
            this.Page.Server.Transfer(this.PreviousPage, false);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Set child controls according to current mode
        /// </summary>
        private void SetControlsAttributtes()
        {
            switch (CurrentMode)
            {
                case ModeOfAction.Default:
                    rightPanel.Visible = false;
                    break;
                case ModeOfAction.Add:
                    rightPanel.Visible = true;
                    btnDelete.Visible = false;
                    btnAddOrUpdate.Text = "Add";
                    rblOrAndPrivList.SelectedIndex = 0;
                    rblOrAndAppSettings.SelectedIndex = 0;
                    break;
                case ModeOfAction.Update:
                    rightPanel.Visible = true;
                    btnDelete.Visible = true;
                    btnAddOrUpdate.Text = "Update";
                    break;
            }
        }

        /// <summary>
        /// Binding the data of right form.
        /// </summary>
        private void BindFormData()
        {
            privsIDHiddenField.Value = string.Empty;
            ctrlsIDHiddenField.Value = string.Empty;
            privilegeListID.Value = string.Empty;
            privilegeListIDs.Value = string.Empty;

            PrivilegeList availablePrivs = new PrivilegeList();
            PrivilegeList currentPrivs = new PrivilegeList();
            ControlList availableCtrls = new ControlList();
            ControlList currentCtrls = new ControlList();
            AppSettingList currentAppSettings = new AppSettingList();

            AppSettingList fullAppSettingList = PageControlSettings.FullListOfAppSettings;

            PrivilegeList fullPrivList = PageControlSettings.FullListOfPrivileges;
            COEPageControlSettingsService.Page currentPage = null;

            if (CurrentMode == ModeOfAction.Add)
            {
                currentPage = (COEPageControlSettingsService.Page)treeView.SelectedNode.DataKey;
                idTextBox.Text = string.Empty;
                availablePrivs = fullPrivList;
                // Coverity Fix CID - 10904 (from local server)
                if(currentPage != null)
                    availableCtrls = currentPage.CtrlList;
            }
            else if (CurrentMode == ModeOfAction.Update)
            {
                currentPage = (COEPageControlSettingsService.Page)treeView.SelectedNode.Parent.DataKey;
                ControlList fullCtrlList = currentPage != null ? currentPage.CtrlList : null;
                ControlSetting selectedControlSetting = (ControlSetting)treeView.SelectedNode.DataKey;
                // Coverity Fix CID - 10904 (from local server)
                if (selectedControlSetting != null)
                {
                    idTextBox.Text = selectedControlSetting.ID;
                    rblOrAndPrivList.SelectedIndex = selectedControlSetting.PrivilegeList.OperatorToApply == PrivilegeList.Operators.AND ? 1 : 0;
                    rblOrAndAppSettings.SelectedIndex = selectedControlSetting.AppSettingList.OperatorToApply == AppSettingList.Operators.AND ? 1 : 0;

                    availablePrivs = selectedControlSetting.GetAvailablePrivList(fullPrivList);
                    currentPrivs = selectedControlSetting.GetCurrentPrivList(fullPrivList);
                    availableCtrls = selectedControlSetting.GetAvailableCtrlList(fullCtrlList);
                    currentCtrls = selectedControlSetting.GetCurrentCtrlList(fullCtrlList);
                    if (fullAppSettingList != null)
                        currentAppSettings = selectedControlSetting.GetCurrentAppSettingList(fullAppSettingList);
                }
                foreach (Privilege priv in currentPrivs)
                {
                    privsIDHiddenField.Value += priv.ID + '|';
                }
                if (privsIDHiddenField.Value.Length > 0)
                    privsIDHiddenField.Value = privsIDHiddenField.Value.Substring(0, privsIDHiddenField.Value.Length - 1);

                foreach (COEPageControlSettingsService.Control ctrl in currentCtrls)
                {
                    ctrlsIDHiddenField.Value += ctrl.ID + '|';
                }
                ctrlsIDHiddenField.Value = ctrlsIDHiddenField.Value.Substring(0, ctrlsIDHiddenField.Value.Length - 1);

                if (currentAppSettings.Count > 0)
                {
                    foreach (COEPageControlSettingsService.AppSetting appSetting in currentAppSettings)
                    {
                        appSettingsIDHiddenField.Value += appSetting.ID + '|';
                    }
                    appSettingsIDHiddenField.Value = appSettingsIDHiddenField.Value.Substring(0, appSettingsIDHiddenField.Value.Length - 1);
                }
                else
                {
                    appSettingsIDHiddenField.Value = String.Empty;
                }

                privilegeListID.Value = treeView.SelectedNode.Text;
            }
            //Coverity Bug  Fix CID 11659 
            if (currentPage != null)
            {
                currentPage = _pageControlSettings.PageList.GetByID(currentPage.ID);
            }
            BindListBoxData(availablePrivs, currentPrivs, availableCtrls, currentCtrls, fullAppSettingList, currentAppSettings);

        }

        /// <summary>
        /// Binding the ListBox data
        /// </summary>
        private void BindListBoxData(PrivilegeList availablePrivs, PrivilegeList currentPrivs, ControlList availableCtrls, ControlList currentCtrls, AppSettingList fullAppSettingsList, AppSettingList currentAppSettings)
        {
            //Init available ListBox of privileges with selected node.
            availablePrivListBox.DataSource = availablePrivs;
            availablePrivListBox.DataTextField = Privilege.PROPERTY_NAME_ID;
            availablePrivListBox.DataValueField = Privilege.PROPERTY_NAME_ID;
            availablePrivListBox.DataBind();
            //set the listbox item title on server side
            SetPrivilLstBoxItemTitle(availablePrivs, availablePrivListBox);

            //Init current ListBox of privileges with selected node.
            currentPrivListBox.DataSource = currentPrivs;
            currentPrivListBox.DataTextField = Privilege.PROPERTY_NAME_ID;
            currentPrivListBox.DataValueField = Privilege.PROPERTY_NAME_ID;
            currentPrivListBox.DataBind();
            //set the listbox item title on server side
            SetPrivilLstBoxItemTitle(currentPrivs, currentPrivListBox);

            //Init available ListBox of controls with selected node.
            availableCtrlListBox.DataSource = availableCtrls;
            availableCtrlListBox.DataTextField = COEPageControlSettingsService.Control.PROPERTY_NAME_ID;
            availableCtrlListBox.DataValueField = COEPageControlSettingsService.Control.PROPERTY_NAME_ID;
            availableCtrlListBox.DataBind();
            //set the listbox item title on server side
            SetCtrlLstBoxItemTitle(availableCtrls, availableCtrlListBox);

            //Init current ListBox of controls with selected node.
            currentCtrlListBox.DataSource = currentCtrls;
            currentCtrlListBox.DataTextField = COEPageControlSettingsService.Control.PROPERTY_NAME_ID;
            currentCtrlListBox.DataValueField = COEPageControlSettingsService.Control.PROPERTY_NAME_ID;
            currentCtrlListBox.DataBind();
            //set the listbox item title on server side
            SetCtrlLstBoxItemTitle(currentCtrls, currentCtrlListBox);

            this.appSettingGrid.DataSource = fullAppSettingsList;
            this.appSettingGrid.DataBind();

            COEPageControlSettingsService.Page currentPage = null;

            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in this.appSettingGrid.Rows)
            {
                if (currentAppSettings.Contains(fullAppSettingsList.GetByID(row.Cells.FromKey("ID").Value.ToString())))
                {
                    row.Cells.FromKey("Enabled").Value = true;
                    row.Cells.FromKey("Value").Value = currentAppSettings.GetByID(row.Cells.FromKey("ID").Value.ToString()).Value;
                }
            }



        }

        /// <summary>
        /// Set privilege listbox item title.
        /// </summary>
        /// <param name="privilegeList">the current PrivilegeList object</param>
        /// <param name="listBox">the current list box</param>
        private void SetPrivilLstBoxItemTitle(PrivilegeList privilegeList, ListBox listBox)
        {
            if (privilegeList.Count > 0)
            {
                for (int index = 0; index < privilegeList.Count; index++)
                {
                    string toolTipContent = "[ID]:" + privilegeList[index].ID + "\n" + "[FriendlyName]:" + privilegeList[index].FriendlyName + "\n" + "[Description]:" + privilegeList[index].Description;
                    listBox.Items[index].Attributes.Add("title", toolTipContent);
                }
            }
        }

        /// <summary>
        /// Set control listbox item title.
        /// </summary>
        /// <param name="ctrlList">the current ControlList object</param>
        /// <param name="listBox">the current list box</param>
        private void SetCtrlLstBoxItemTitle(ControlList ctrlList, ListBox listBox)
        {
            if (ctrlList.Count > 0)
            {
                for (int index = 0; index < ctrlList.Count; index++)
                {
                    string toolTipContent = "[ID]:" + ctrlList[index].ID + "\n" + "[FriendlyName]:" + ctrlList[index].FriendlyName + "\n" + "[Description]:" + ctrlList[index].Description;
                    listBox.Items[index].Attributes.Add("title", toolTipContent);
                }
            }
        }
        /// <summary>
        /// Get Validator
        /// </summary>
        /// <param name="validationName">Custom/Required</param>
        /// <param name="controlToValidate">which control the validator be added to</param>
        /// <param name="scriptName"></param>
        /// <returns>the base validato</returns>
        private BaseValidator CreateValidator(ValidationName validationName, WebControl controlToValidate, string messges, string scriptName)
        {
            BaseValidator validator = null;
            switch (validationName)
            {
                case ValidationName.Required:
                    validator = new RequiredFieldValidator();
                    break;
                case ValidationName.TextLength:
                    validator = new RegularExpressionValidator();
                    ((RegularExpressionValidator)validator).ValidationExpression = "(.|\r|\n){@min,@max}";
                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@min", "1");
                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@max", "150");
                    break;
                case ValidationName.Custom:
                    validator = new CustomValidator();
                    ((CustomValidator)validator).ClientValidationFunction = scriptName;
                    ((CustomValidator)validator).ValidateEmptyText = true;
                    validator.EnableClientScript = true;
                    break;
            }
            validator.ErrorMessage = messges;
            validator.Display = ValidatorDisplay.Dynamic;
            validator.ControlToValidate = controlToValidate.ID;
            return validator;
        }

        /// <summary>
        /// Get the url of given image.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private string GetImageUrl(string imagePath)
        {
            return Page.ClientScript.GetWebResourceUrl(this.GetType(), imagePath);
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Page on load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (Page.Request.UrlReferrer != null)
                    ViewState["UrlReferrer"] = Page.Request.UrlReferrer.ToString();
            }
        }

        /// <summary>
        /// Page on PreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //Confirm delelte
            if (!Page.IsClientScriptBlockRegistered("ConfirmDelete"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function ConfirmDelete()
                {                   
                    if(!confirm('Are you sure you want to delete?'))
                        document.getElementById('" + confirmDelHiddenField.ClientID + @"').value='false'; 
                    else
                        document.getElementById('" + confirmDelHiddenField.ClientID + @"').value='true'; 
                }
                </script>";
                Page.RegisterClientScriptBlock("ConfirmDelete", strCode);
            }

            //Privilege ListBox move to right client event
            if (!Page.IsClientScriptBlockRegistered("MoveToRightPrivil"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function MoveToRightPrivil()
                {
                     var elemAll = document.getElementById('" + availablePrivListBox.ClientID + @"');
                     var elemSel = document.getElementById('" + currentPrivListBox.ClientID + @"');
                     var hid = document.getElementById('" + privsIDHiddenField.ClientID + @"');
                     for(var i=0;i<elemAll.options.length;i++)
                     {
                         if(elemAll.options[i].selected)
                         { 
                             var SelText=elemAll.options[i].innerText;
                             var SelValue = elemAll.options[i].value;
                             var Num = elemSel.options.length;
                             elemSel.options[Num]=new Option(SelText,SelValue);
                             elemSel.options[Num].title = elemAll.options[i].title;
                             elemAll.options.remove(i);
                             i--;
                         }
                     }
                     hid.value='';
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                         hid.value +=elemSel.options[i].value+ '|';
                     }
                     hid.value = hid.value.substr(0,hid.value.length - 1);
                     return false;
               }
                </script>";
                Page.RegisterClientScriptBlock("MoveToRightPrivil", strCode);
            }

            //Privilege ListBox move to left client event
            if (!Page.IsClientScriptBlockRegistered("MoveToLeftPrivil"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function MoveToLeftPrivil()
                {
                     var elemAll = document.getElementById('" + availablePrivListBox.ClientID + @"');
                     var elemSel = document.getElementById('" + currentPrivListBox.ClientID + @"');
                     var hid = document.getElementById('" + privsIDHiddenField.ClientID + @"');
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                        if(elemSel.options[i].selected)
                        {
                            var SelText = elemSel.options[i].innerText;
                            var SelValue = elemSel.options[i].value;
                            var Num = elemAll.options.length;
                            elemAll.options[Num]= new Option(SelText,SelValue);
                            elemAll.options[Num].title = elemSel.options[i].title;
                            elemSel.options.remove(i);
                            i--;
                        }
                     }
                     hid.value='';
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                         hid.value +=elemSel.options[i].value+ '|';
                     }
                     hid.value = hid.value.substr(0,hid.value.length - 1);
                     return false;
               }
                </script>";
                Page.RegisterClientScriptBlock("MoveToLeftPrivil", strCode);
            }

            //Control ListBox move to right client event
            if (!Page.IsClientScriptBlockRegistered("MoveToRightControl"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function MoveToRightControl()
                {
                     var elemAll = document.getElementById('" + availableCtrlListBox.ClientID + @"');
                     var elemSel = document.getElementById('" + currentCtrlListBox.ClientID + @"');
                     var hid = document.getElementById('" + ctrlsIDHiddenField.ClientID + @"');
                     for(var i=0;i<elemAll.options.length;i++)
                     {
                         if(elemAll.options[i].selected)
                         { 
                             var SelText=elemAll.options[i].innerText;
                             var SelValue = elemAll.options[i].value;
                             var Num = elemSel.options.length;
                             elemSel.options[Num]=new Option(SelText,SelValue);
                             elemSel.options[Num].title = elemAll.options[i].title;
                             elemAll.options.remove(i);
                             i--;
                         }
                     }
                     hid.value='';
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                         hid.value +=elemSel.options[i].value+ '|';
                     }
                     hid.value = hid.value.substr(0,hid.value.length - 1);
                     return false;
               }
                </script>";
                Page.RegisterClientScriptBlock("MoveToRightControl", strCode);
            }

            //Control ListBox move to left client event
            if (!Page.IsClientScriptBlockRegistered("MoveToLeftControl"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function MoveToLeftControl()
                {
                     var elemAll = document.getElementById('" + availableCtrlListBox.ClientID + @"');
                     var elemSel = document.getElementById('" + currentCtrlListBox.ClientID + @"');
                     var hid = document.getElementById('" + ctrlsIDHiddenField.ClientID + @"');
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                        if(elemSel.options[i].selected)
                        {
                            var SelText = elemSel.options[i].innerText;
                            var SelValue = elemSel.options[i].value;
                            var Num = elemAll.options.length;
                            elemAll.options[Num]= new Option(SelText,SelValue);
                            elemAll.options[Num].title = elemSel.options[i].title;
                            elemSel.options.remove(i);
                            i--;
                        }
                     }
                     hid.value='';
                     for(var i=0;i<elemSel.options.length;i++)
                     {
                         hid.value +=elemSel.options[i].value+ '|';
                     }
                     hid.value = hid.value.substr(0,hid.value.length - 1);
                     return false;
               }
                </script>";
                Page.RegisterClientScriptBlock("MoveToLeftControl", strCode);
            }

            //Valid current control listbox on client 
            if (!Page.IsClientScriptBlockRegistered("ValidClientForControl"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function ValidClientForControl(source, arguments)
                {
                     var listBox = document.getElementById('" + currentCtrlListBox.ClientID + @"');
                     if(listBox.options.length > 0)
                         arguments.IsValid = true;    
                     else
                         arguments.IsValid = false;
               }
                </script>";
                Page.RegisterClientScriptBlock("ValidClientForControl", strCode);
            }

            //Valid current privilege listbox on client 
            if (!Page.IsClientScriptBlockRegistered("ValidClientForPrivilege"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function ValidClientForPrivilege(source, arguments)
                {
                     var listBox = document.getElementById('" + currentPrivListBox.ClientID + @"');
                     if(listBox.options.length > 0)
                         arguments.IsValid = true;    
                     else
                         arguments.IsValid = false;
               }
                </script>";
                Page.RegisterClientScriptBlock("ValidClientForPrivilege", strCode);
            }

            //Valid Id textbox on client 
            if (!Page.IsClientScriptBlockRegistered("ValidClientForID"))
            {
                string strCode = @"
                <script type='text/javascript' language='javascript'>   
                function ValidClientForID(source, arguments)
                {
                    var privilegeListIDs = document.getElementById('" + privilegeListIDs.ClientID + @"');
                    var privilegeListID = document.getElementById('" + privilegeListID.ClientID + @"');
                    var idTxt = document.getElementById('" + idTextBox.ClientID + @"');
                    if(privilegeListIDs.value != null && privilegeListIDs.value.length > 0)
                    {                    
                        var ids = privilegeListIDs.value.split('|');
                        arguments.IsValid = true;
                        for(var i = 0;i < ids.length;i++)
                        {
                            if(ids[i] == idTxt.value && ids[i] != privilegeListID.value)
                            {
                                arguments.IsValid = false;
                                return;
                            }
                        }
                    }
               }
                </script>";
                Page.RegisterClientScriptBlock("ValidClientForID", strCode);
            }
        }

        #endregion
    }
}
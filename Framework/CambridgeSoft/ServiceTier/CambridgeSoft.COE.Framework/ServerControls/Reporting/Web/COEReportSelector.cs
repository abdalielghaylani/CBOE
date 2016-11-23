using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEReportingService;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Web
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:COEReportSelector runat=server></{0}:COEReportSelector>")]
    public class COEReportSelector : CompositeControl, IReportPicker
    {
        #region Variables

        private const string AUTENTICATIONTICKETPARAMETERNAME = "autenticationTicket";
        private const string REPORTTEMAPLATEIDPARAMETERNAME = "reportTemplateId";

        ReportPickerController _controller = null;

        protected Panel _mainPanel;
        protected Label _messageLabel;
        protected DropDownList _availableReportsDropDown;
        protected LinkButton _createNewLinkButton;
        protected LinkButton _deleteSelectedLinkButton;

        public event EventHandler SelectedReportBuilderMetaChanged;
        public event EventHandler CreateNewReport;
        public event EventHandler DeleteSelectedReport;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or set the dataview id of the reports to be displayed.
        /// </summary>
        public int DataViewId
        {
            get
            {
                return _controller.DataViewId;
            }
            set
            {
                _controller.DataViewId = value;
            }
        }
        public COEReportType? ReportType
        {
            get
            {
                return _controller.ReportType;
            }
            set
            {
                _controller.ReportType = value;
            }
        }

        public string Category
        {
            get
            {
                return _controller.Category;
            }
            set
            {
                _controller.Category = value;
            }
        }

        public bool ShowPrivateReports
        {
            get 
            {
                return _controller.ShowPrivateReports;
            }
            set 
            {
                _controller.ShowPrivateReports = value;
            }
        }

        /// <summary>
        /// Gets the report template builder info of the selected report.
        /// </summary>
        public ReportBuilderMeta SelectedReportBuilderMeta
        {
            get
            {
                return _controller.GetReportBuilder(SelectedReportBuilderId);
            }
        }

        /// <summary>
        /// Gets the report template builder id of the selected report.
        /// </summary>
        private int SelectedReportBuilderId
        {
            get
            {
                int selectedReportId = 0;
                if (this._availableReportsDropDown.SelectedValue != null)
                    int.TryParse(this._availableReportsDropDown.SelectedValue.ToString(), out selectedReportId);
                return selectedReportId;
            }
        }

        public string ApplicationName
        {
            get
            {
                return _controller.ApplicationName;
            }
            set
            {
                _controller.ApplicationName = value;
            }
        }

        public string UserName
        {
            get 
            {
                return _controller.UserName;
            }
            set 
            {
                _controller.UserName = value;
            }
        }

        public bool ShowCreateNewButton
        {
            get
            {
                return ViewState["ShowCreateNewButton"] == null ? false : (bool)ViewState["ShowCreateNewButton"];
            }
            set
            {
                ViewState["ShowCreateNewButton"] = value;
            }
        }

        public bool ShowDeleteButton
        {
            get
            {
                return ViewState["ShowDeleteButton"] == null ? false : (bool)ViewState["ShowDeleteButton"];
            }
            set
            {
                ViewState["ShowDeleteButton"] = value;
            }
        }

        #endregion

        #region Constructors
        public COEReportSelector()
        {
            _controller = new ReportPickerController(this);
        }
        #endregion

        #region Methods
        private void OnSelectedReportBuilderMetaChanged()
        {
            if (this.SelectedReportBuilderMetaChanged != null)
                SelectedReportBuilderMetaChanged(this, new EventArgs());
        }
        private void OnDeleteSelectedReport()
        {
            _controller.DeleteReport(this.SelectedReportBuilderMeta);
            DisplayAvailableReports();

            if (this.DeleteSelectedReport != null)
                DeleteSelectedReport(this, new EventArgs());
        }

        private void OnCreateNewReport()
        {
            if (this.CreateNewReport != null)
                CreateNewReport(this, new EventArgs());

            string autenticationTicket = string.Empty;
            if (Page.Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Page.Request.Cookies["COESSO"].Value))
                autenticationTicket = Page.Request.Cookies["COESSO"].Value;

            this.Page.Response.Redirect(Resources.ReportEUD_URL + "?" + "createNewReport" + "&" + AUTENTICATIONTICKETPARAMETERNAME + "=" + autenticationTicket);
            
        }


        private void BuildReportSelector()
        {
            this._mainPanel = new Panel();
            this._mainPanel.ID = "ReportSelectorPanel";
            this._messageLabel = new Label();
            this._messageLabel.ID = "ReportSelectorMessageLabel";
            this._mainPanel.Width = Unit.Pixel(170);
            this._mainPanel.Height = Unit.Pixel(50);
            this._availableReportsDropDown = new DropDownList();
            this._availableReportsDropDown.ID = "ReportSelectorDropDownList";
            this._availableReportsDropDown.AutoPostBack = true;
            this._messageLabel.Text = Resources.COEReportSelector_Label_Message;
            this._mainPanel.Controls.Add(this._messageLabel);
            this._mainPanel.Controls.Add(this._availableReportsDropDown);

            if (ShowCreateNewButton)
            {
                this._createNewLinkButton = new LinkButton();
                this._createNewLinkButton.ID = "CreateNewLinkButton";
                this._createNewLinkButton.Text = "Create...";
                this._createNewLinkButton.Click += new EventHandler(CreateNewLinkButton_Click);
                this._mainPanel.Controls.Add(this._createNewLinkButton);

                Label separator = new Label();
                separator.Text = "  ";
                this._mainPanel.Controls.Add(separator);
            }

            if (ShowDeleteButton)
            {
                this._deleteSelectedLinkButton = new LinkButton();
                this._deleteSelectedLinkButton.ID = "DeleteSelectedLinkButton";
                this._deleteSelectedLinkButton.Text = "Delete";
                this._deleteSelectedLinkButton.Click += new EventHandler(DeleteSelectedLinkButton_Click);
                this._mainPanel.Controls.Add(this._deleteSelectedLinkButton);
            }

            this.Controls.Add(_mainPanel);
        }

        private void DisplayAvailableReports()
        {
            this._availableReportsDropDown.SelectedIndexChanged += new System.EventHandler(this.AvailableReportsDropDown_SelectedIndexChanged);
            this._availableReportsDropDown.DataTextField = "Name";
            this._availableReportsDropDown.DataValueField = "id";
            this._availableReportsDropDown.DataSource = _controller.ReportBuilders;
            this._availableReportsDropDown.DataBind();
        }

        #endregion

        #region Control Life-Cycle Methods
        protected override void CreateChildControls()
        {
            this.BuildReportSelector();
            this.DisplayAvailableReports();
            base.CreateChildControls();
        }
        #endregion

        #region Event Handlers
        private void AvailableReportsDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedReportBuilderMetaChanged();
        }
        
        void DeleteSelectedLinkButton_Click(object sender, EventArgs e)
        {
            OnDeleteSelectedReport();
        }

        void CreateNewLinkButton_Click(object sender, EventArgs e)
        {
            OnCreateNewReport();   
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Win
{
    public partial class COEReportSelector : UserControl, IReportPicker
    {
        #region events
        public event EventHandler SelectedReportBuilderMetaChanged;
        public event EventHandler CreateNewReport;
        public event EventHandler DeleteSelectedReport;
        #endregion

        #region variables
        ReportPickerController _controller;
        #endregion

        #region Properties
        public int DataViewId
        {
            get
            {
                return _controller.DataViewId;
            }
            set
            {
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.DataViewId = value;
                    this.Invalidate();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
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
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.ReportType = value;
                    this.Invalidate();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
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
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.Category = value;
                    this.Invalidate();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
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
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.ShowPrivateReports = value;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
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
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.ApplicationName = value;
                    this.Invalidate();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
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
                try
                {
                    this.Focus();
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.UserName = value;
                    this.Invalidate();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        public ReportBuilderMeta SelectedReportBuilderMeta
        {
            get
            {
                if (!this.DesignMode)
                    return _controller.GetReportBuilder(SelectedReportBuilderId);

                return null;
            }
        }

        private int SelectedReportBuilderId
        {
            get
            {
                int selectedReportId = 0;
                if (AvailableReportsComboBox.SelectedValue != null)
                    int.TryParse(this.AvailableReportsComboBox.SelectedValue.ToString(), out selectedReportId);
                return selectedReportId;
            }
            set
            {
                this.AvailableReportsComboBox.SelectedValue = value;
            }
        }

        public bool IsAnyReportBuilderMetaSelected
        { 
            get {
                return SelectedReportBuilderId >= 0;
            }
        }

        public bool ShowCreateNewButton
        {
            get
            {
                return this.CreateLinkLabel.Visible;
            }
            set
            {
                this.CreateLinkLabel.Visible = value;
            }
        }

        public bool ShowDeleteButton
        {
            get
            {
                return this.DeleteLinkLabel.Visible;
            }
            set
            {
                this.DeleteLinkLabel.Visible = value;
            }
        }
        #endregion

        #region Constructors
        public COEReportSelector()
        {
            _controller = new ReportPickerController(this);
            InitializeComponent();
        }
        #endregion

        #region Methods
        #region Public Methods
        public void Refresh()
        {
            if (!DesignMode)
            {
                int selectedId = SelectedReportBuilderId;

                _controller.RefreshReportBuilders();

                this.Invalidate();

                if (_controller.GetReportBuilder(selectedId) != null)
                    this.AvailableReportsComboBox.SelectedValue = selectedId;
            }
        }

        public void SelectReport(int reportTemplateId)
        {
            this.SelectedReportBuilderId = reportTemplateId;
        }

        #endregion

        #region Event Handlers
        private void COEReportSelector_Paint(object sender, PaintEventArgs e)
        {
            DisplayAvailableReports();
        }

        private void CreateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnCreateNewReport();
        }

        private void DeleteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnDeleteSelectedReport();
        }

        private void AvailableReportsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedReportBuilderMetaChanged();
        }

        private void COEReportSelector_Layout(object sender, LayoutEventArgs e)
        {
            int rightMargin = 0;

            rightMargin += this.ShowCreateNewButton ? this.CreateLinkLabel.Width + 10 : 0;
            rightMargin += this.ShowDeleteButton ? this.DeleteLinkLabel.Width + 10 : 0;

            this.AvailableReportsComboBox.Width = this.Width - this.AvailableReportsComboBox.Left - rightMargin;
            this.CreateLinkLabel.Left = this.AvailableReportsComboBox.Left + this.AvailableReportsComboBox.Width + 10;
            this.DeleteLinkLabel.Left = this.ShowCreateNewButton ? this.CreateLinkLabel.Left + this.CreateLinkLabel.Width + 10 : this.AvailableReportsComboBox.Left + this.AvailableReportsComboBox.Width;

        }
        #endregion

        #region Private Methods
        private void DisplayAvailableReports()
        {
            if (!DesignMode)
            {
                this.AvailableReportsComboBox.DisplayMember = "Name";
                this.AvailableReportsComboBox.ValueMember = "Id";
                this.AvailableReportsComboBox.DataSource = _controller.ReportBuilders;

                if (_controller.ReportBuilders.Count == 0)
                {
                    this.AvailableReportsComboBox.Text = Resources.NoReportsAvailable;
                    this.AvailableReportsComboBox.Enabled = false;
                }
            }
        }

        private void OnSelectedReportBuilderMetaChanged()
        {
            if (this.SelectedReportBuilderMetaChanged != null)
                SelectedReportBuilderMetaChanged(this, new EventArgs());
        }

        private void OnCreateNewReport()
        {
            if (this.CreateNewReport != null)
                CreateNewReport(this, new EventArgs());

            this.Refresh();
        }

        private void OnDeleteSelectedReport()
        {
            _controller.DeleteReport(this.SelectedReportBuilderMeta);

            this.Refresh();

            if (this.DeleteSelectedReport != null)
                DeleteSelectedReport(this, new EventArgs());
        }
        #endregion
        #endregion
    }
}

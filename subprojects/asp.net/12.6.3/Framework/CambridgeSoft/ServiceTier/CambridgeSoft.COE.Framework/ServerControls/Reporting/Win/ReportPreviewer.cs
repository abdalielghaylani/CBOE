using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using DevExpress.XtraReports.UI;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Win
{
    public partial class COEReportViewer : UserControl, IReportViewer
    {
        #region Variables
        private ReportViewerController _controller = null;
        private bool _showToolbar = true;
        private bool _showMenu = true;
        private XtraReport _report = null;
        CloseCommandHandler _closeCommandHandler;

        public event EventHandler EditReportRequested;
        public event EventHandler ExitRequested;

        #endregion

        #region Properties
        public ReportBuilderMeta ReportBuilderMeta
        {
            get
            {
                return _controller.ReportBuilderMeta;
            }
            set
            {
                _controller.ReportBuilderMeta = value;
                this.Refresh();
            }
        }

        public CambridgeSoft.COE.Framework.COEReportingService.Builders.ReportBuilderBase ReportBuilder
        {
            get
            {
                return _controller.ReportBuilder;
            }
            set
            {
                _controller.ReportBuilder = value;
                Refresh();
            }
        }

        public int HitlistId
        {
            get
            {
                return _controller.HitListId;
            }
            set
            {
                if (value != HitlistId)
                {
                    _controller.HitListId = value;
                    Refresh();
                }
            }
        }

        public object DataSource
        {
            get {
                return _controller.DataSource;
            }
            set 
            {
                if (DataSource != null && !DataSource.Equals(value) || DataSource == null && value != null)
                {
                    _controller.DataSource = value;
                    Refresh();
                }
            }
        }

        public int ReportId
        {
            get
            {
                return _controller.ReportId;
            }
            set
            {
                _controller.ReportId = value;
                Refresh();
            }
        }

        public PagingInfo PagingInfo
        {
            get 
            {
                return _controller.PagingInfo;
            }
            set 
            {
                _controller.PagingInfo = value;
                Refresh();
            }
        }

        public bool ShowToolbar
        {
            get
            {
                return _showToolbar;
            }
            set
            {
                _showToolbar = value;
                this.ReportPreviewToolBar.Visible = value;
            }
        }

        public bool ShowMenu
        {
            get
            {
                return _showMenu;
            }
            set
            {
                _showMenu = value;
                this.ReportPreviewMenu.Visible = value;
            }
        }

        public XtraReport Report
        {
            get {
                return _controller.GetReportInstance();
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public COEReportViewer()
        {
            _controller = new ReportViewerController(this);
            _closeCommandHandler = new CloseCommandHandler(this);
            InitializeComponent();
            
        }
        #endregion

        #region Event Handlers
        private void COEReportViewer_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Cursor previousCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                if (!this.DesignMode)
                    _controller.DisplayReport();

                Cursor.Current = previousCursor;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

    private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OffsetPicker offsetPicker = new OffsetPicker())
            {

                offsetPicker.NumberOfColumns = ((COEXtraReport)this._controller.GetReportInstance()).NumberOfColumns;
                offsetPicker.NumberOfRows = ((COEXtraReport)this._controller.GetReportInstance()).NumberOfRows;
                offsetPicker.SelectedRow = ((COEXtraReport)this._controller.GetReportInstance()).StartingRow;
                offsetPicker.SelectedColumn = ((COEXtraReport)this._controller.GetReportInstance()).StartingColumn;

                if (offsetPicker.ShowDialog() == DialogResult.OK)
                {
                    ((COEXtraReport)this._controller.GetReportInstance()).StartingRow = offsetPicker.SelectedRow;
                    ((COEXtraReport)this._controller.GetReportInstance()).StartingColumn = offsetPicker.SelectedColumn;
                }
            }
            _controller.DisplayReport();
            this.Refresh();
        }

        private void RepeatCountBarEditItem_EditValueChanged(object sender, EventArgs e)
        {
            SheetsBarEditItem.BeginUpdate();
            SheetsBarEditItem.EditValue = "0";
            SheetsBarEditItem.EndUpdate();

            if (int.Parse(RepeatCountBarEditItem.EditValue.ToString()) < 0)
                RepeatCountBarEditItem.EditValue = "0";

            _controller.DisplayReport();
            this.Refresh();
        }


        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Refresh();
        }

        private void SheetsBarEditItem_EditValueChanged(object sender, EventArgs e)
        {
            RepeatCountBarEditItem.BeginUpdate();
            int numberOfSheets = int.Parse(this.SheetsBarEditItem.EditValue.ToString());

            if (numberOfSheets > 0)
                this.RepeatCountBarEditItem.EditValue = numberOfSheets * ((COEXtraReport)this._controller.GetReportInstance()).CopiesPerSheet;
            else
            {
                this.RepeatCountBarEditItem.EditValue = "1";
                this.SheetsBarEditItem.EditValue = "0";
            }
            RepeatCountBarEditItem.EndUpdate();

            _controller.DisplayReport();
            this.Refresh();
        }

        private void EditBarButtonItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnEditReportRequested();
        }
        #endregion

        #region Inteface Methods
        public void ShowReport(XtraReport report, bool readOnly)
        {
            SheetsBarEditItem.Enabled = this.RepeatCountBarEditItem.Enabled = ColumnBarEditItem.Enabled = RowBarEditItem.Enabled = ApplyBarButtonItem.Enabled = false;

            if (report is COEXtraReport)
            {
                COEXtraReport coeXtraReport = (COEXtraReport)report;

                if (coeXtraReport.IsLabelReport)
                {
                    SheetsBarEditItem.Enabled = this.RepeatCountBarEditItem.Enabled = ColumnBarEditItem.Enabled = RowBarEditItem.Enabled = ApplyBarButtonItem.Enabled = true;

                    ((COEXtraReport)report).NumberOfCopies = int.Parse(this.RepeatCountBarEditItem.EditValue.ToString());
                }
            }

            if (report != null)
            {
                this.Enabled = true;
                this.ReportPreviewPrintControl.PrintingSystem = report.PrintingSystem;
                this.ReportPreviewPrintControl.PrintingSystem.AddCommandHandler(_closeCommandHandler);
                report.CreateDocument();
            }
            else
            {
                this.Enabled = false;
                this.ReportPreviewPrintControl.PrintingSystem = null;
                this.ReportPreviewPrintControl.Refresh();
                this.SheetsBarEditItem.EditValue = "0";
                this.RepeatCountBarEditItem.EditValue = "1";
            }

            this.EditBarButtonItem.Enabled = this.ContainsReportToShow();
        }

        public void Refresh()
        {
            this.Invalidate();
        }

        public void Clear()
        {
            _controller.Clear();
        }

        public bool IsReportReadOnly()
        {
            return _controller.IsReportReadOnly;
        }

        public bool ContainsReportToShow()
        {
            return _controller.ContainsReportToShow;
        }

        public COEReport GetReportTemplate()
        {
            return _controller.GetReportDefinition();
        }

        #endregion

        #region Private methods
        private void OnEditReportRequested()
        {
            if (EditReportRequested != null)
                EditReportRequested(this, new EventArgs());
        }

        internal void OnExitRequested()
        {
            if (ExitRequested != null)
                ExitRequested(this, new EventArgs());
        }
        #endregion

        #endregion

        public void RefreshReport()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    #region Commands
    internal class CloseCommandHandler : DevExpress.XtraPrinting.ICommandHandler
    {
        #region Variables
        COEReportViewer _reportViewer;
        #endregion

        #region Constructors
        public CloseCommandHandler(COEReportViewer reportViewer)
        {
            _reportViewer = reportViewer;
        }
        #endregion

        #region ICommandHandler Members

        public bool CanHandleCommand(DevExpress.XtraPrinting.PrintingSystemCommand command, DevExpress.XtraPrinting.IPrintControl printControl)
        {
            return command == DevExpress.XtraPrinting.PrintingSystemCommand.ClosePreview;
        }

        public void HandleCommand(DevExpress.XtraPrinting.PrintingSystemCommand command, object[] args, DevExpress.XtraPrinting.IPrintControl printControl, ref bool handled)
        {
            _reportViewer.OnExitRequested();
            handled = true;
        }

        #endregion
    }
    #endregion
}
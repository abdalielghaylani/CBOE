using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEReportingService;
using DevExpress.Data;
using DevExpress.XtraReports.UserDesigner;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner
{
    public partial class ReportDesignerForm : Form
    {
        #region Variables
        private ReportDesignerController _controller;
        #endregion

        #region Properties
        public COEXtraReport Report
        {
            get 
            {
                return (COEXtraReport)this.DesignPanel.Report;
            }
        }
        #endregion

        #region Methods
        #region Constructors
        internal ReportDesignerForm(ReportDesignerController controller)
        {
            InitializeComponent();

            _controller = controller;

            DesignPanel.DesignerHostLoaded += new DesignerLoadedEventHandler(DesignPanel_DesignerHostLoaded);

            DesignPanel.AddCommandHandler(new SaveCommandHandler(this, controller));
            DesignPanel.AddCommandHandler(new OpenCommandHandler(this, controller));
            DesignPanel.AddCommandHandler(new NewCommandHandler(this, controller));
        }
        #endregion

        #region Interface Methods
        internal void CreateNewReport(string title, object dataSource, string mainReportDataMember)
        {
            COEXtraReport report = new COEXtraReport();

            if (dataSource != null)
                report.DataSource = dataSource;

            if(!string.IsNullOrEmpty(mainReportDataMember))
                report.DataMember = mainReportDataMember;

            this.DesignPanel.OpenReport(report);
            this.ShowReport(title, string.Empty);
        }

        internal void CreateNewReport(string title, string mainReportDataMember)
        {
            CreateNewReport(title, null, mainReportDataMember);
        }

        internal void ShowReport(string name, string reportLayout)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Text = name;
            if (!string.IsNullOrEmpty(reportLayout))
                this.DesignPanel.OpenReport(new MemoryStream(UTF8Encoding.UTF8.GetBytes(reportLayout)));
            Cursor.Current = Cursors.Default;
        }

        internal void DisplayAvailableFields(IDataDictionary availableFields)
        {
            DesignPanel.Report.DataSource = availableFields;

            // Update the Field List.
            IDesignerHost host = (IDesignerHost)DesignPanel.GetService(typeof(IDesignerHost));
            COEFieldListDockPanel.UpdateDataSource(host);
        }
        #endregion

        #region Private Methods
        #endregion
        #endregion

        #region Events Handlers
        void DesignPanel_DesignerHostLoaded(object sender, DesignerLoadedEventArgs e)
        {
            IToolboxService ts = (IToolboxService)e.DesignerHost.GetService(typeof(IToolboxService));

            // Add a custom control.
            ts.AddToolboxItem(new ToolboxItem(typeof(CambridgeSoft.COE.Framework.Controls.Reporting.XRChemDrawEmbed)));
            ts.AddToolboxItem(new ToolboxItem(typeof(CambridgeSoft.COE.Framework.Controls.Reporting.XRTBarCode)));
        }

        private void ViewDataViewBarButtonItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _controller.ShowDataView();
        }

        private void EditResultsCriteriaBarButtonItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _controller.EditResultsCriteria();
        }

        private void ReportDesignerForm_Load(object sender, System.EventArgs e)
        {
            _controller.Initialize();
        }

        private void NewFromDataViewCommandBarItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DesignPanel.ExecCommand(ReportCommand.NewReport, new object[] { e.Item.Name });
        }
        #endregion
    }

    #region Command Pattern Classes
    internal class NewCommandHandler : ICommandHandler
    {
        #region Variables
        ReportDesignerController _controller;
        ReportDesignerForm _form;
        #endregion

        #region Constructors
        public NewCommandHandler(ReportDesignerForm form, ReportDesignerController controller)
        {
            _controller = controller;
            _form = form;
        }
        #endregion

        #region New ICommandHandler Members
        public bool CanHandleCommand(ReportCommand command)
        {
            return command == ReportCommand.NewReport || command == ReportCommand.NewReportWizard;
        }

        public void HandleCommand(ReportCommand command, object[] args, ref bool handled)
        {
            _form.DesignPanel.CloseReport();

            if (_controller.CreateReportFlow(args.Length > 0 && args[0] != null && args[0].ToString().Equals("NewFromDataViewCommandBarItem")))
                _form.DesignPanel.ReportState = ReportState.Opened;

            handled = command == ReportCommand.NewReport;
        }
        #endregion
    }
    internal class OpenCommandHandler : ICommandHandler
    {
        #region variables
        ReportDesignerController _controller;
        ReportDesignerForm _form;
        #endregion

        #region Constructors
        public OpenCommandHandler(ReportDesignerForm form, ReportDesignerController controller)
        {
            _controller = controller;
            _form = form;
        }
        #endregion

        #region Open ICommandHandler Members
        public bool CanHandleCommand(ReportCommand command)
        {
            return command == ReportCommand.OpenFile;
        }

        public void HandleCommand(ReportCommand command, object[] args, ref bool handled)
        {
            if (_controller.OpenReportFlow())
                _form.DesignPanel.ReportState = ReportState.Opened;

            handled = true;
        }
        #endregion
    }
    internal class SaveCommandHandler : ICommandHandler
    {
        #region variables
        ReportDesignerController _controller;
        ReportDesignerForm _form;
        #endregion

        #region Constructors
        public SaveCommandHandler(ReportDesignerForm form, ReportDesignerController controller)
        {
            _controller = controller;
            _form = form;
        }
        #endregion

        #region Save ICommandHandler Members
        public bool CanHandleCommand(ReportCommand command)
        {
            return command == ReportCommand.SaveFile || command == ReportCommand. SaveFileAs;
        }


        public void HandleCommand(ReportCommand command, object[] args, ref bool handled)
        {
            //TODO: Add resultsCriteria filtering for removing unused fields - should this be here.

            MemoryStream outputStream = new MemoryStream();
            _form.DesignPanel.Report.SaveLayout(outputStream);
            if (_controller.SaveReportFlow(UTF8Encoding.UTF8.GetString(outputStream.GetBuffer()), command == ReportCommand.SaveFileAs))
                _form.DesignPanel.ReportState = ReportState.Saved;
            handled = true;
        }
        #endregion
    }
    #endregion

    #region Helper Classes
    internal class COEDescriptiveDataSet : DataSet, IDataDictionary
    {
        #region variables
        string _displayName = string.Empty;
        Dictionary<string, string> _friendlyTablesNames = new Dictionary<string, string>();
        #endregion

        #region Methods
        #region Constructors
        public COEDescriptiveDataSet(string displayname, DataSet dataset) : this(displayname)
        {
            this.CloneDataSet(dataset);
        }

        public COEDescriptiveDataSet(string displayName)
        {
            _displayName = displayName;
        }

        public COEDescriptiveDataSet()
            : base()
        {

        }
        #endregion

        #region Public Methods
        public void SetObjectDisplayName(string name, string friendlyName)
        {
            if (!_friendlyTablesNames.ContainsKey(name))
                _friendlyTablesNames.Add(name, friendlyName);
            else
                _friendlyTablesNames[name] = friendlyName;
        }
        #endregion

        #region Private Methods
        private void CloneDataSet(DataSet dataset)
        {
            MemoryStream stream = new MemoryStream();

            dataset.WriteXml(stream);
            stream.Seek(0, SeekOrigin.Begin);
            this.ReadXml(stream);

            foreach (DataTable currentTable in this.Tables)
            {
                SetObjectDisplayName(currentTable.TableName, currentTable.TableName);
                foreach (DataColumn currentColumn in currentTable.Columns)
                    SetObjectDisplayName(currentColumn.ColumnName, currentColumn.ColumnName);
            }
        }
        #endregion

        #region IDataDictionary Members

        public string GetDataSourceDisplayName()
        {
            return _displayName;
        }

        public string GetObjectDisplayName(string dataMember)
        {
            if (_friendlyTablesNames.ContainsKey(dataMember))
                return _friendlyTablesNames[dataMember];
            else if (dataMember.Contains("."))
                return dataMember.Substring(dataMember.LastIndexOf(".") + 1);

            return string.Empty;
        }

        #endregion
        #endregion
    }
    #endregion
}

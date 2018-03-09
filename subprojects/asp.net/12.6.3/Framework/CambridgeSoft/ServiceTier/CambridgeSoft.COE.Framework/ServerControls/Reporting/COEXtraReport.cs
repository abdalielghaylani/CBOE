using System;
using DevExpress.XtraReports.UI;
using System.Data;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.Reporting.BLL;

namespace CambridgeSoft.COE.Framework.COEReportingService
{
    /// <summary>
    /// COEXtraReport class represents a report instance. This report instance consists of a report definition + data bound to it.
    /// This report instance is capable of being rendered onscreen and exported to several formats as well.
    /// The report can either be a list report, or a label report.
    /// </summary>
    public partial class COEXtraReport : DevExpress.XtraReports.UI.XtraReport
    {
        #region Constants
        private const string _dummyDetailBandName = "COE_Dummy_Details_Band";
        private const string _repeatLabelsReportBandName = "COE_Repeat_Labels_Report_Band";
        #endregion

        #region Variables
        private int _startingRow;
        private int _startingColumn;
        private int _columnsToSkip;
        private int _numberOfCopies = 1;
        private bool _stop;
        private bool _skipLastRecord;
        private COEReportType _reportType;
        private string _category;
        #endregion

        #region Properties
        /// <summary>
        /// This property specifies the initial row for displaying labels, in the case of a label report. If the report type is List this property is ignored
        /// </summary>
        [Browsable(false)]
        public int StartingRow
        {
            get
            {
                return _startingRow;
            }
            set
            {
                if (value >= this.NumberOfRows)
                    value = this.NumberOfRows - 1;

                _startingRow = value;
                _columnsToSkip = GetOffset(_startingRow, _startingColumn);
            }
        }

        /// <summary>
        /// This property specifies the initial column for displaying labels, in the case of a label report. If the report type is List this property is ignored
        /// </summary>
        [Browsable(false)]
        public int StartingColumn
        {
            get
            {
                return _startingColumn;
            }
            set
            {
                if (value >= this.NumberOfColumns)
                    value = this.NumberOfColumns - 1;

                _startingColumn = value;
                _columnsToSkip = GetOffset(_startingRow, _startingColumn);
            }
        }

        /// <summary>
        /// Obtains the number of rows per sheet
        /// </summary>
        [Browsable(false)]
        public int NumberOfRows
        {
            get
            {
                //return (int)(this.PageHeight / this.Detail.HeightF);
                return (int)((this.PageHeight - this.Margins.Top - this.Margins.Bottom) / this.Detail.HeightF);
            }
        }

        /// <summary>
        /// Obtains thenumber of columns per sheet
        /// </summary>
        [Browsable(false)]
        public int NumberOfColumns
        {
            get
            {
                if (this.Detail.MultiColumn.Mode == MultiColumnMode.UseColumnCount)
                    return this.Detail.MultiColumn.ColumnCount;
                else
                    return (int)((this.PageWidth - this.Margins.Right - this.Margins.Left) / this.Detail.MultiColumn.ColumnWidth);

                //return (int)(this.PageWidth / this.Detail.MultiColumn.ColumnWidth);

            }
        }

        /// <summary>
        /// Number of times every label is repeated. If the report type is List, this property is ignored.
        /// </summary>
        [Browsable(false)]
        public int NumberOfCopies
        {
            get
            {
                return _numberOfCopies;
            }
            set
            {
                _numberOfCopies = value;
            }
        }

        /// <summary>
        /// Number of labels that fit in a sheet
        /// </summary>
        [Browsable(false)]
        public int CopiesPerSheet
        {
            get
            {
                return NumberOfColumns * NumberOfRows;
            }
        }
        
        /// <summary>
        /// Returns true if the report type is Label, false otherwise.
        /// </summary>
        [Browsable(false)]
        public bool IsLabelReport
        {
            get
            {
                return this.Detail.MultiColumn.Mode != MultiColumnMode.None;
            }
        }

        /// <summary>
        /// Returns the report type: List or Label.
        /// </summary>
        public COEReportType ReportType
        {
            get 
            {
                return (this.IsLabelReport ? COEReportType.Label : COEReportType.List);
            }
        }

        /// <summary>
        /// This property is only meant to allow the user to edit the category on report designer.
        /// </summary>
        public string Category
        {
            get 
            {
                return _category;
            }

            set 
            {
                _category = value;
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public COEXtraReport()
        {
            InitializeComponent();
        }
        #endregion

        #region internal Methods
        /// <summary>
        /// Calculates the offset (in labels) based on given starting column and starting row. This value is used to skip "offset" label places before rendering.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        internal int GetOffset(int rows, int columns)
        {
            return rows * NumberOfColumns + columns;
        }

        protected override void OnReportInitialize()
        {
            base.OnReportInitialize();
            this.PreProcess();
        }

        /// <summary>
        /// Performs pre-processing steps required for Labels offset functionality (only this for now)
        /// </summary>
        internal void PreProcess()
        {
            if (IsLabelReport)
            {
                if (NumberOfCopies > 1)
                {
                    if (this.Bands.Contains(this.Detail))
                    {
                        this.Bands.Remove(this.Detail);
                        DetailBand dummyBand = new DetailBand();
                        dummyBand.Name = _dummyDetailBandName;
                        this.Bands.Add(dummyBand);
                    }

                    DetailReportBand repeatLabelsReportBand = (DetailReportBand)GetBandByName(this.Bands, _repeatLabelsReportBandName);
                    if (repeatLabelsReportBand == null)
                    {
                        repeatLabelsReportBand = new DetailReportBand();
                        repeatLabelsReportBand.Name = _repeatLabelsReportBandName;
                        this.Bands.Add(repeatLabelsReportBand);
                    }

                    repeatLabelsReportBand.Bands.Add(this.Detail);
                    this.Detail.PrintOnEmptyDataSource = true;
                    this.Detail.RepeatCountOnEmptyDataSource = NumberOfCopies;
                }
                else
                {
                    if (!this.Bands.Contains(this.Detail))
                    {
                        Band dummyBand = GetBandByName(this.Bands, _dummyDetailBandName);
                        if (dummyBand != null)
                            this.Bands.Remove(dummyBand);

                        Band repeatLabelsReportBand = GetBandByName(this.Bands, _repeatLabelsReportBandName);
                        if (repeatLabelsReportBand != null)
                            this.Bands.Remove(repeatLabelsReportBand);

                        this.Bands.Add(this.Detail);
                    }

                    this.Detail.PrintOnEmptyDataSource = false;
                    this.Detail.RepeatCountOnEmptyDataSource = 0;
                }

                _columnsToSkip = GetOffset(_startingRow, _startingColumn);
                if (_columnsToSkip > 0)
                {
                    if (this.DataSource is DataSet && ((DataSet)this.DataSource).Tables.Count > 0 && ((DataSet)this.DataSource).Tables[0].Rows.Count == 1)
                    {
                        ((DataSet)this.DataSource).Tables[0].Rows.Add(((DataSet)this.DataSource).Tables[0].NewRow());
                        _skipLastRecord = true;
                    }

                    _stop = false;

                    this.Detail.BeforePrint -= Detail_BeforePrint;
                    this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(Detail_BeforePrint);

                    foreach (XRControl currentInmediateChild in this.Detail.Controls)
                    {
                        currentInmediateChild.BeforePrint -= currentInmediateChild_BeforePrint;
                        currentInmediateChild.BeforePrint += new System.Drawing.Printing.PrintEventHandler(currentInmediateChild_BeforePrint);
                    }

                    XRPanel panel = new XRPanel();
                    panel.Name = "HiddenBorderPanel";
                    panel.HeightF = this.Detail.HeightF;
                    panel.WidthF = this.Detail.MultiColumn.ColumnWidth;
                    this.Detail.Controls.Add(panel);
                }
            }
        }
        #endregion

        #region Public Methods
        public static Band GetBandByName(BandCollection bandCollection, string name)
        {
            foreach (Band currentBand in bandCollection)
                if (currentBand.Name.Equals(name))
                    return currentBand;

            return null;
        }
        #endregion

        #region Event Handlers
        private void currentInmediateChild_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DevExpress.Data.Browsing.DataBrowser browser = this.fDataContext[this.DataSource, this.DataMember];

            if (!(_skipLastRecord && browser.HasLastPosition))
            {
                if (IsLabelReport && _columnsToSkip >= 0)
                {
                    e.Cancel = true;
                }
            }
            else
                e.Cancel = true;
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (IsLabelReport)
            {
                if (_columnsToSkip > 0)
                {
                    DevExpress.Data.Browsing.DataBrowser browser = this.fDataContext[this.DataSource, this.DataMember];
                    browser.PositionChanged -= Browser_PositionChanged;
                    browser.PositionChanged += new EventHandler(Browser_PositionChanged);
                }
                _columnsToSkip--;
            }
        }
        void Browser_PositionChanged(object sender, EventArgs e)
        {
            if (IsLabelReport)
            {
                ((DevExpress.Data.Browsing.ListBrowser)(sender)).PositionChanged -= Browser_PositionChanged;
                if (!_stop)
                    ((DevExpress.Data.Browsing.ListBrowser)(sender)).Position = 0;
                if (_columnsToSkip < 0)
                {
                    _stop = true;
                }
            }
        }
        #endregion
        #endregion
    }
}

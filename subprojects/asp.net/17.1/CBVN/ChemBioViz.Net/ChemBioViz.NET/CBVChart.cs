using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Web.UI.Design;
using System.Globalization;
using System.Text;

using CambridgeSoft.COE.Framework.Common;
using ChemBioViz.NET;
using FormDBLib;
using CBVUtilities;
using Utilities;

namespace CBVControls
{
    #region ChartControl
    /// <summary>
    /// CBV subclass of MS Chart Control
    /// </summary>
    [Designer(typeof(CBVChartDesigner))]
    public class CBVChartControl : Chart
    {
        #region Variables
        private ResultsCriteria m_rcPlot;
        private DataSet m_dsPlot;
        private int m_nPointHilited = -1;
        private CBVFilterCollection m_filters;
        private List<int> m_selectedRecs;
        private bool m_bInited;
        private bool m_bSelected;
        private String m_xTable, m_yTable, m_xAggreg, m_yAggreg;
        public static bool m_bZoomOnDragX = false, m_bZoomOnDragY = false;

        private MarkerStyle m_hiliteMarkerStyle;
        private Color m_hiliteColor;
        private int m_hiliteSize;
        private Color m_selectedColor;
        private bool m_bAutoLabelAxes;  // if true, set label whenever field is set
        private String m_pkFieldName;   // usually "PK" unless primary key is one of the plotted fields
        #endregion

        #region Constructor
        public CBVChartControl()
        {
            this.MouseUp += new MouseEventHandler(CBVChartControl_MouseUp);
            this.MouseDown += new MouseEventHandler(CBVChartControl_MouseDown);
            this.MouseMove += new MouseEventHandler(CBVChartControl_MouseMove);
            m_rcPlot = null;
            m_dsPlot = null;
            m_filters = new CBVFilterCollection();
            m_selectedRecs = new List<int>();

            ChartAreas.Add(new ChartArea());
            Series.Add(new Series());
            Series[0].ChartType = SeriesChartType.Point;
            SetMarkerDefaults(false);

            m_bInited = false;  // init on first Rebind

            m_hiliteMarkerStyle = MarkerStyle.Circle;
            m_hiliteColor = Color.Lime;
            m_hiliteSize = 8;
            m_selectedColor = Color.Blue;
            Stretchable = false; // no longer defaults true -- CSBR-133936
            m_bAutoLabelAxes = true;
        }
        #endregion

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [BrowsableAttribute(false)]
        public ChemBioVizForm Form
        {
            get
            {
                Control c = this;
                while (c != null)
                {
                    if (c is FormViewControl)
                        return (c as FormViewControl).Form;
                    else if (c.Parent is ChemBioVizForm)
                        return c.Parent as ChemBioVizForm;
                    c = c.Parent;
                }
                return null;
            }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedRecords
        {
            get { return m_selectedRecs; }
        }
        //---------------------------------------------------------------------
        private String FormTableName
        {
            get { return (this.Form == null) ? "" : this.Form.FormDbMgr.TableAlias; }
        }
        //---------------------------------------------------------------------
        public bool IsSubformPlot
        {
            get { return IsSubTablePlot(); }
        }
        //---------------------------------------------------------------------
        public bool Selected
        {
            get { return m_bSelected; }
            set
            {
                m_bSelected = value;
                this.BorderlineColor = Color.Navy;
                this.BorderlineDashStyle = m_bSelected ? ChartDashStyle.Solid : ChartDashStyle.NotSet;
            }
        }
        //---------------------------------------------------------------------
        #region User-Settable Properties
        public MarkerStyle MarkerStyle
        {
            get { return Series[0].MarkerStyle; }
            set { if (Series.Count > 0) Series[0].MarkerStyle = value; }
        }
        //---------------------------------------------------------------------
        public int MarkerSize
        {
            get { return Series[0].MarkerSize; }
            set { if (Series.Count > 0) Series[0].MarkerSize = value; }
        }
        //---------------------------------------------------------------------
        public Color MarkerColor
        {
            get { return Series[0].MarkerColor; }
            set { if (Series.Count > 0) Series[0].MarkerColor = value; }
        }
        //---------------------------------------------------------------------
        public MarkerStyle HiliteMarkerStyle
        {
            get { return m_hiliteMarkerStyle; }
            set { m_hiliteMarkerStyle = value; }
        }
        //---------------------------------------------------------------------
        public int HiliteMarkerSize
        {
            get { return m_hiliteSize; }
            set { m_hiliteSize = value; }
        }
        //---------------------------------------------------------------------
        public Color HiliteMarkerColor
        {
            get { return m_hiliteColor; }
            set { m_hiliteColor = value; }
        }
        //---------------------------------------------------------------------
        public Color SelectedMarkerColor
        {
            get { return m_selectedColor; }
            set { m_selectedColor = value; }
        }
        //---------------------------------------------------------------------
        public SeriesChartType ChartType
        {
            get { return Series[0].ChartType; }
            set { if (Series.Count > 0) Series[0].ChartType = value; }
        }
        //---------------------------------------------------------------------
        public Color BackgroundColor
        {
            get { return this.ChartAreas[0].BackColor; }
            set { if (ChartAreas.Count > 0) ChartAreas[0].BackColor = value; }
        }
        //---------------------------------------------------------------------
        public String MainTitle
        {
            get { return (Titles.Count > 0) ? Titles[0].Text : ""; }
            set
            {
                Titles.Clear();
                if (!String.IsNullOrEmpty(value))
                {
                    Titles.Add(new Title());
                    Titles[0].Text = value;
                }
            }
        }
        //---------------------------------------------------------------------
        public String SeriesTitle
        {
            get { return Series[0].LegendText; }
            set
            {
                Legends.Clear();
                if (!String.IsNullOrEmpty(value))
                    Legends.Add(new Legend());
                if (Series.Count > 0)
                    Series[0].LegendText = value;
            }
        }
        //---------------------------------------------------------------------
        public String PlotTitle
        {
            get { return MainTitle; }
            set { MainTitle = value; }
        }
        //---------------------------------------------------------------------
        public String XAxisLabel
        {
            get
            {
                String xField = String.IsNullOrEmpty(XField) ? CBVConstants.RECNO_NAME : XField;
                return m_bAutoLabelAxes ? xField : ChartAreas[0].AxisX.Title;
            }
            set { if (ChartAreas.Count > 0) ChartAreas[0].AxisX.Title = value; }
        }
        //---------------------------------------------------------------------
        public String YAxisLabel
        {
            get { return m_bAutoLabelAxes ? YField : ChartAreas[0].AxisY.Title; }
            set { if (ChartAreas.Count > 0) ChartAreas[0].AxisY.Title = value; }
        }
        //---------------------------------------------------------------------
        public bool Stretchable
        {
            get { return Anchor == (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right); }
            set
            {
                Anchor = (value == true) ? (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right)
                                            : (AnchorStyles.Top | AnchorStyles.Left);
            }
        }
        //---------------------------------------------------------------------
        public bool AutoLabelAxes
        {
            get { return m_bAutoLabelAxes; }
            set { m_bAutoLabelAxes = value; }
        }
        //---------------------------------------------------------------------
        public bool XGridVisible
        {
            get { return ChartAreas[0].AxisX.MajorGrid.Enabled; }
            set { if (ChartAreas.Count > 0) ChartAreas[0].AxisX.MajorGrid.Enabled = value; }
        }
        //---------------------------------------------------------------------
        public bool YGridVisible
        {
            get { return ChartAreas[0].AxisY.MajorGrid.Enabled; }
            set { if (ChartAreas.Count > 0) ChartAreas[0].AxisY.MajorGrid.Enabled = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        public CBVFilterCollection CBVFilters
        {
            get { return m_filters; }
            set { m_filters = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Data Connection Props
        [TypeConverterAttribute(typeof(XFieldStringConverter))]
        public String XField
        {
            get { return (Series.Count > 0) ? Series[0].XValueMember : ""; }
            set
            {
                if (Series.Count > 0)
                {
                    Series[0].XValueMember = value;
                    if (m_bAutoLabelAxes) XAxisLabel = value;
                }
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(YFieldStringConverter))]
        public String YField
        {
            get { return (Series.Count > 0) ? Series[0].YValueMembers : ""; }
            set
            {
                if (Series.Count > 0)
                {
                    Series[0].YValueMembers = value;
                    if (m_bAutoLabelAxes) YAxisLabel = value;
                }
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(DataSourceConverter))]
        public String XTable
        {
            get { return m_xTable; }
            set
            {
                m_xTable = value;
                if (String.IsNullOrEmpty(m_yTable))
                    m_yTable = value;
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(DataSourceConverter))]
        public String YTable
        {
            get { return m_yTable; }
            set { m_yTable = value; }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(AggregConverter))]
        public String XAggregate
        {
            get { return m_xAggreg; }
            set { m_xAggreg = value; }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(AggregConverter))]
        public String YAggregate
        {
            get { return m_yAggreg; }
            set { m_yAggreg = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ResultsCriteria PlotResultsCriteria
        {
            // return RC to retrieve plot points; create if necessary
            get
            {
                if (m_rcPlot == null)
                    GenerateRC();
                return m_rcPlot;
            }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataSet PlotDataSet
        {
            // return dataset of points; fetch if necessary; create RC if necessary
            get
            {
                if (m_dsPlot == null && PlotResultsCriteria != null && !IsSubformPlot)
                    RetrieveDS();
                return m_dsPlot;
            }
        }
        #endregion

        //---------------------------------------------------------------------
        #region HiddenProperties
        // hidden because greatis fouls up when reading them

        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new SeriesCollection Series
        {
            get { return base.Series; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new LegendCollection Legends
        {
            get { return base.Legends; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ChartAreaCollection ChartAreas
        {
            get { return base.ChartAreas; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TitleCollection Titles
        {
            get { return base.Titles; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        public new bool SuppressExceptions
        {
            get { return base.SuppressExceptions; }
            set { base.SuppressExceptions = value; }
        }
        //---------------------------------------------------------------------
        #endregion
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        private void SetMarkerDefaults(bool bOnlyIfUnset)
        {
            if (bOnlyIfUnset && MarkerStyle != MarkerStyle.None)
                return;
            MarkerColor = Color.CornflowerBlue;
            MarkerStyle = MarkerStyle.Square;
            MarkerSize = 5;
        }
        //---------------------------------------------------------------------
        private void HilitePoint(DataPoint dpt, bool bOn)
        {
            if (bOn)
            {
                dpt.MarkerColor = HiliteMarkerColor;
                dpt.MarkerSize = HiliteMarkerSize;
                dpt.MarkerStyle = HiliteMarkerStyle;
            }
            else
            {
                dpt.MarkerColor = MarkerColor;
                if (IsPointSelected(dpt))
                    dpt.MarkerColor = m_selectedColor;
                dpt.MarkerSize = MarkerSize;
                dpt.MarkerStyle = MarkerStyle;
            }
        }
        //---------------------------------------------------------------------
        public enum PlotType { MainMain, MainSub, Sub1Sub1, Sub1Sub2 }

        public PlotType GetPlotType()
        {
            return GetPlotType(XTable, YTable);
        }
        //---------------------------------------------------------------------
        private PlotType GetPlotType(String xTable, String yTable)
        {
            bool bXIsMain = CBVUtil.Eqstrs(xTable, FormTableName);
            bool bYIsMain = CBVUtil.Eqstrs(yTable, FormTableName);
            if (bXIsMain && bYIsMain) return PlotType.MainMain;
            else if (bXIsMain || bYIsMain) return PlotType.MainSub;
            else if (CBVUtil.Eqstrs(xTable, yTable)) return PlotType.Sub1Sub1;
            else return PlotType.Sub1Sub2;
        }
        //---------------------------------------------------------------------
        public bool IsSubTablePlot()
        {
            // true if nonblank axes are on the same subtable
            return GetPlotType() == PlotType.Sub1Sub1;
        }
        //---------------------------------------------------------------------
        public bool IsMainTablePlot()
        {
            // true if nonblank axes are on the main table
            return GetPlotType() == PlotType.MainMain;
        }
        //---------------------------------------------------------------------
        public bool IsCrossTablePlot()
        {
            // true if one axis is on the main table and the other on a subtable
            return (GetPlotType() == PlotType.MainSub) || IsCrossSubtablePlot();
        }
        //---------------------------------------------------------------------
        private bool IsCrossTablePlot(String xname, String yname)
        {
            // true if one axis is on the main table and the other on a subtable
            return (GetPlotType(xname, yname) == PlotType.MainSub) || IsCrossSubtablePlot(xname, yname);
        }
        //---------------------------------------------------------------------
        public bool IsCrossSubtablePlot()
        {
            // true if both axes are on different subtables
            return GetPlotType() == PlotType.Sub1Sub2;
        }
        //---------------------------------------------------------------------
        private bool IsCrossSubtablePlot(String xname, String yname)
        {
            // true if both axes are on different subtables
            return GetPlotType(xname, yname) == PlotType.Sub1Sub2;
        }
        //---------------------------------------------------------------------
        public bool RequiresAggregSpec(int axisNo /*x=0 y=1*/, String xTable, String yTable)
        {
            if (!IsCrossTablePlot(xTable, yTable))
                return false;   // no aggreg required for main plot or subform plot
            if (IsCrossSubtablePlot(xTable, yTable))
                return true;    // two diff subforms: both axes require aggregates

            bool bXIsMain = CBVUtil.Eqstrs(xTable, FormTableName);
            return (axisNo == 0) != bXIsMain;   // if X is main, Y reqs aggreg, and vice-versa
        }
        //---------------------------------------------------------------------
        public int GetSubTableID()
        {
            String subTableName = String.IsNullOrEmpty(XTable) ? YTable : XTable;   // ??
            return GetSubTableID(subTableName);
        }
        //---------------------------------------------------------------------
        public int GetSubTableID(String subTableName)
        {
            // for subform or cross-table plot: return id of given subtable
            int id = 0;
            if (!String.IsNullOrEmpty(subTableName) && Form.FormDbMgr.SelectedDataView != null)
            {
                COEDataView.DataViewTable dvTable = this.Form.FormDbMgr.FindDVTableByName(subTableName);
                if (dvTable != null)
                    id = dvTable.Id;
            }
            return id;
        }
        //---------------------------------------------------------------------
        private void BindSubformPlot()
        {
            // both columns are either blank or on the same subtable
            // get subtable name from any nonblank column
            int dvtableID = GetSubTableID();
            if (dvtableID == 0) return;
            String dsTableName = String.Concat("Table_", CBVUtil.IntToStr(dvtableID));

            // create new binding source using appropriate relation
            // TO DO: adapt for grandchild subforms
            BindingSource subBS = FormViewControl.GetSubBindingSourceEx(this.Form.BindingSource, dsTableName, null);
            if (subBS == null)
                throw new Exception(String.Format(
                    "Data for subform plot is not available.  Form must include a subform on table '{0}'", XTable));

            if (subBS != null)
            {
                this.DataSource = subBS;
                DataBind();
            }
        }
        //---------------------------------------------------------------------
        public bool GetSelectionLimits(ref double xMin, ref double xMax, ref double yMin, ref double yMax)
        {
            // return coords enclosing selected points
            bool ret = false;
            for (int ip = 0; ip < Series[0].Points.Count; ++ip)
            {
                DataPoint dpt = Series[0].Points[ip];
                if (IsPointSelected(dpt) && !dpt.IsEmpty)
                {
                    double xVal = (dpt.XValue == 0.0) ? ip : dpt.XValue;
                    double yVal = dpt.YValues[0];
                    if (!ret)
                    {
                        xMin = xMax = xVal;
                        yMin = yMax = yVal;
                    }
                    if (xVal < xMin) xMin = xVal;
                    if (xVal > xMax) xMax = xVal;
                    if (yVal < yMin) yMin = yVal;
                    if (yVal > yMax) yMax = yVal;
                    ret = true;
                }
            }
            return ret;
        }
        //---------------------------------------------------------------------
        public void ZoomToRect(double xMin, double xMax, double yMin, double yMax)
        {
            ChartArea area = this.ChartAreas[0];
            area.AxisX.ScaleView.Zoom(xMin, xMax);
            area.AxisY.ScaleView.Zoom(yMin, yMax);
        }
        //---------------------------------------------------------------------
        public void Unzoom()
        {
            ChartArea area = this.ChartAreas[0];
            area.AxisX.ScaleView.ZoomReset(99);
            area.AxisY.ScaleView.ZoomReset(99);
        }
        //---------------------------------------------------------------------
        public void SetZoomOnDragModes(bool bZoomOnDragX, bool bZoomOnDragY)
        {
            // enable/disable zoom-on-drag on one or both axes
            ChartArea area = this.ChartAreas[0];
            area.AxisX.ScaleView.Zoomable = bZoomOnDragX;
            area.AxisY.ScaleView.Zoomable = bZoomOnDragY;

            bool bNeither = !bZoomOnDragX && !bZoomOnDragY;
            area.CursorX.IsUserSelectionEnabled = bZoomOnDragX || bNeither;
            area.CursorY.IsUserSelectionEnabled = bZoomOnDragY || bNeither;

            m_bZoomOnDragX = bZoomOnDragX;
            m_bZoomOnDragY = bZoomOnDragY;
        }
        //---------------------------------------------------------------------
        private void RemoveEmptyPoints()
        {
            // remove points with null Y values from point set
            DataPointCollection dpoints = Series[0].Points;
            for (int i = 0; i < dpoints.Count; ++i)
            {
                if (dpoints[i].IsEmpty)
                {
                    dpoints.RemoveAt(i);
                    --i;
                }
            }
        }
        //---------------------------------------------------------------------
        private void AssignIDsToPoints()
        {
            // tag each point with a record number from the current list
            // there are no points in the list for records having null X values, so skip over those
            if (PlotDataSet == null || PlotDataSet.Tables.Count == 0) return;
            DataTable dataTable = PlotDataSet.Tables[0];
            DataRowCollection dataRows = dataTable.Rows;
            DataPointCollection dpoints = Series[0].Points;

            if (dataRows.Count == dpoints.Count)
            {
                for (int i = 0; i < dpoints.Count; ++i)
                    dpoints[i].Tag = i;
            }
            else if (dataTable.Columns.Contains(this.XField))
            {
                int colNo = dataTable.Columns.IndexOf(this.XField);    // only interested in missing values on X
                int pointIndex = 0;

                // loop rows, assigning non-null row indices to points
                for (int iRow = 0; iRow < dataRows.Count; ++iRow)
                {
                    bool bIsNull = dataRows[iRow].ItemArray[colNo] is System.DBNull;
                    if (!bIsNull)
                        dpoints[pointIndex++].Tag = iRow;
                }
            }
        }
        //---------------------------------------------------------------------
        public void Rebind(bool bHiliteOnly, bool bFullReset)
        {
            // using field names from plot props, retrieve data and bind to plot
            // three cases: (a) one or two fields from main form: make RC of those fields and get new dataset;
            // (b) one or two fields from subform (IsSubformPlot): get data from main RC, making sure subdata is included;
            // (c) one from main, one from sub: make RC with relation, get new dataset
            // if hiliteOnly: fill subform or hilite point on main form; do not retrieve data

            InitOnFirstDisplay();

            // if bFullReset, erase existing rc/ds before rebinding
            if (bFullReset)
                this.ResetPlot();

            try
            {
                // subform plot: data comes from main dataset
                if (IsSubTablePlot())
                {
                    BindSubformPlot();
                    return;
                }
                if (!bHiliteOnly)
                {
                    //if (PlotDataSet != null)
                    this.DataSource = PlotDataSet;
                    if (this.DataSource == null)
                    {
                        String sErr = String.Format("Failed to retrieve data for plot: X={0} Y={1}", XField, YField);
                        MessageBox.Show(sErr, "Plot Data Retrieval", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DataBindings.Clear();
                        return;
                    }
                    DataBind();
                    AssignIDsToPoints();
                    RemoveEmptyPoints();

                    // apply filters
                    if (CBVFilters.Count > 0)
                    {
                        CBVDataPointFilter dpFilter = new CBVDataPointFilter(this);
                        this.DataManipulator.FilterSetEmptyPoints = true;   // otherwise rec <-> point correlation fouls up
                        this.DataManipulator.Filter(dpFilter, Series[0]);
                    }
                }

                // hilite point of current record
                // first unhilite previous selection if any
                if (m_nPointHilited >= 0 && m_nPointHilited < Series[0].Points.Count)
                {
                    HilitePoint(Series[0].Points[m_nPointHilited], false);
                    m_nPointHilited = -1;
                }
                int nRowToHilite = Form.Pager.CurrRow;
                int nPointToHilite = RowToPointIndex(nRowToHilite);
                if (nPointToHilite >= 0 && nPointToHilite < Series[0].Points.Count)
                {
                    HilitePoint(Series[0].Points[nPointToHilite], true);
                    m_nPointHilited = nPointToHilite;
                }
            }
            catch (Exception e)
            {
                CBVUtil.ReportError(e);
                this.DataSource = null;
                this.DataBindings.Clear();
            }
        }
        //---------------------------------------------------------------------
        public int PointIndexToRow(int iPtindex)
        {
            // get the row index of the given point
            DataPointCollection dpc = Series[0].Points;
            if (iPtindex >= 0 && iPtindex < dpc.Count)
            {
                DataPoint dpt = dpc[iPtindex];
                if (dpt.Tag != null)
                    return (int)dpt.Tag;
            }
            return -1;
        }
        //---------------------------------------------------------------------
        public int RowToPointIndex(int iRow)
        {
            // find the index of the point with the given row index as tag
            DataPointCollection dpc = Series[0].Points;
            for (int i = 0; i < dpc.Count; ++i)
            {
                DataPoint dpt = dpc[i];
                if (dpt.Tag != null && (int)dpt.Tag == iRow)
                {
                    if (dpt.IsEmpty)
                        Debug.WriteLine(String.Format("Returning empty point i={0} row={1}", i, iRow));
                    return i;
                }
            }
            return -1;
        }
        //---------------------------------------------------------------------
        public void ResetPlot()
        {
            // toss existing data so we retrieve new next time
            m_rcPlot = null;
            m_dsPlot = null;
        }
        //---------------------------------------------------------------------
        private void InitOnFirstDisplay()
        {
            // initialization requiring parent form; can't do in constructor
            if (!m_bInited)
            {
                this.ContextMenuStrip = Form.PlotContextMenuStrip;

                // defaults for new plot: set to main table on both axes
                if (String.IsNullOrEmpty(this.XTable))
                    this.XTable = Form.FormDbMgr.TableName;
                if (String.IsNullOrEmpty(this.YTable))
                    this.YTable = Form.FormDbMgr.TableName;

                this.XField = this.XField;      // create axis labels if autoLabel is on
                this.YField = this.YField;

                //if (this.IsSubformPlot)       // no longer defaults true -- CSBR-133936
                this.Stretchable = false;   // override constructor default for subform

                SetMarkerDefaults(true);
                SetZoomOnDragModes(false, false);

                this.ChartAreas[0].AxisX.LabelStyle.Format = "0.##";
                this.ChartAreas[0].AxisY.LabelStyle.Format = "0.##";

            }
            m_bInited = true;
        }
        //---------------------------------------------------------------------
        private bool IsMainTable(String tableName)
        {
            // true if tablename matches that connected to main form
            String mainTableName = FormTableName;
            return !String.IsNullOrEmpty(tableName) && CBVUtil.Eqstrs(tableName, mainTableName);
        }
        //---------------------------------------------------------------------
        private bool IsSubTable(String tableName)
        {
            // true if tablename not empty but not same as main table
            String mainTableName = FormTableName;
            return !String.IsNullOrEmpty(tableName) && !CBVUtil.Eqstrs(tableName, mainTableName);
        }
        //---------------------------------------------------------------------
        private bool PrimaryKeyMatchesMainField(int primaryKeyID, COEDataView.DataViewTable t)
        {
            // true if XField or YField has same id as given primary key
            if (IsMainTable(XTable) && !String.IsNullOrEmpty(XField))
            {
                COEDataView.Field f = FormDbMgr.FindDVFieldByName(t, XField);
                if (f != null && f.Id == primaryKeyID)
                {
                    m_pkFieldName = XField;
                    return true;
                }
            }
            if (IsMainTable(YTable) && !String.IsNullOrEmpty(YField))
            {
                COEDataView.Field f = FormDbMgr.FindDVFieldByName(t, YField);
                if (f != null && f.Id == primaryKeyID)
                {
                    m_pkFieldName = YField;
                    return true;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        private void GenerateRC()
        {
            // generate a resultscriteria to retrieve plot fields
            // only for use if at least one of the fields is from the main table
            m_rcPlot = null;
            FormDbMgr formDbMgr = this.Form.FormDbMgr;
            COEDataView.DataViewTable t = formDbMgr.SelectedDataViewTable;
            if (t == null)
                return;

            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            rcTable.Id = t.Id;

            m_rcPlot = new ResultsCriteria();
            m_rcPlot.Tables.Add(rcTable);

            // blank any <record> fieldnames
            String xField = XField, yField = YField;
            bool bHasXField = !String.IsNullOrEmpty(xField) && !xField.Equals(CBVConstants.RECNO_NAME);
            bool bHasYField = !String.IsNullOrEmpty(yField);

            // add primary key field first
            int primaryKeyID = CBVUtil.StrToInt(t.PrimaryKey);
            m_pkFieldName = "PK";
            bool bDoNotAddPK = PrimaryKeyMatchesMainField(primaryKeyID, t); // if true, sets m_pkFieldName
            if (!bDoNotAddPK)
                FormUtil.FieldIDToRC(primaryKeyID, rcTable, "PK");

            // then x and y fields
            if (IsMainTable(XTable) && bHasXField)
                FormUtil.FieldToRC(xField, formDbMgr.SelectedDataView, t, rcTable);     // prevents adding duplicates
            if (IsMainTable(YTable) && bHasYField)
                FormUtil.FieldToRC(yField, formDbMgr.SelectedDataView, t, rcTable);

            // then subform fields
            // NOTE: aggregates are not taken into account in SubFieldToRC -- better be sure they are used elsewhere
            if (IsSubTable(XTable) && bHasXField)
                FormUtil.SubFieldToRC(m_rcPlot, formDbMgr.SelectedDataView, XTable, xField, /*XAggregate,*/ t, rcTable);
            if (IsSubTable(YTable) && bHasYField)
                FormUtil.SubFieldToRC(m_rcPlot, formDbMgr.SelectedDataView, YTable, yField, /*YAggregate,*/ t, rcTable);

            // retrieve a column for each filter field
            List<String> filterFlds = this.CBVFilters.GetFieldList();
            foreach (String sFilter in filterFlds)
                FormUtil.FieldToRC(sFilter, formDbMgr.SelectedDataView, t, rcTable);

            // add sort fields if not already available
            FormUtil.CopySortInfo(formDbMgr.ResultsCriteria, m_rcPlot);
        }
        //---------------------------------------------------------------------
        private void RetrieveDS()
        {
            // get dataset from the server by passing it our RC
            bool bDumpDebugOutput = false;
#if DEBUG
            bDumpDebugOutput = true;
#endif
            Debug.Assert(m_rcPlot != null);
            ChemBioVizForm form1 = this.Form;
            Query q = (form1 == null) ? null : form1.CurrQuery;
            //Coverity Bug Fix local analysis
            if (q != null)
            {
                CBVTimer.StartTimerWithMessage(true, "Retrieving plot data", true);
                COEDataView dv = form1.FormDbMgr.SelectedDataView;
                int maxRecs = ChemBioViz.NET.Properties.Settings.Default.MaxPlotPoints;
                m_dsPlot = Pager.GetNRecords(m_rcPlot, dv, maxRecs, q.HitListID, q.IsSaved);
                if (m_dsPlot != null)
                {
                    TagPlotFields(m_dsPlot);
                    if (bDumpDebugOutput) m_dsPlot.WriteXml("C:\\dset_out_plot.xml", XmlWriteMode.WriteSchema);

                    if (IsCrossTablePlot())
                    {
                        m_dsPlot = CBVUtil.FlattenDataset(m_dsPlot);
                        if (bDumpDebugOutput) m_dsPlot.WriteXml("C:\\dset_out_flat.xml", XmlWriteMode.WriteSchema);
                    }
                }
                CBVTimer.EndTimer();
            }
        }
        //---------------------------------------------------------------------
        private void TagPlotFields(DataSet dataSet, String table, String col, String aggreg)
        {
            // if given table is child, tag col with given aggreg name
            String mainTableName = dataSet.Tables[0].TableName;
            String childTableName = this.Form.FormDbMgr.DVTableNameToDSTableName(table);
            if (childTableName.Equals(mainTableName))
                return;

            DataTable childTable = dataSet.Tables[childTableName];
            if (childTable == null)
                return;

            DataColumn dataCol = childTable.Columns[col];
            if (dataCol == null)
                return;

            dataCol.ExtendedProperties["aggreg"] = aggreg;
        }
        //---------------------------------------------------------------------
        private void TagPlotFields(DataSet dataSet)
        {
            // if x or y axis is on child table, find its relation and add attribute for aggreg
            TagPlotFields(dataSet, XTable, XField, XAggregate);
            TagPlotFields(dataSet, YTable, YField, YAggregate);
        }
        //---------------------------------------------------------------------
        private int RowToPK(int iRow, DataTable dataTable, DataColumn dataCol)
        {
            // retrieve PK value from dataset for given row
            int pk = -1;
            if (iRow >= 0 && iRow < dataTable.Rows.Count && dataTable.Rows[iRow] != null)
            {
                String sPKVal = dataTable.Rows[iRow][dataCol].ToString();
                if (!String.IsNullOrEmpty(sPKVal))
                    pk = CBVUtil.StrToInt(sPKVal);
            }
            return pk;
        }
        //---------------------------------------------------------------------
        public List<int> GetSelectedRecordPKs()
        {
            // convert list of recnos into list of PKS, which had better be in the dataset
            List<int> pks = new List<int>();
            DataTable dataTable = PlotDataSet.Tables[0];
            if (dataTable != null && dataTable.Columns.Contains(m_pkFieldName))
            {
                DataColumn dataCol = dataTable.Columns[m_pkFieldName];
                foreach (int iRow in m_selectedRecs)
                {
                    int ipk = RowToPK(iRow, dataTable, dataCol);
                    if (ipk >= 0)
                        pks.Add(ipk);
                }
            }
            return pks;
        }
        //---------------------------------------------------------------------
        public void UpdatePlotStatLine(int iRow)
        {
            if (iRow < 0) return;
            int ptIndex = RowToPointIndex(iRow);
            DataPoint dpt = (ptIndex < 0) ? null : this.Series[0].Points[ptIndex];

            String sRec = (IsSubformPlot || iRow < 0) ? String.Empty : String.Format("Record {0}:  ", iRow + 1);
            String sMsg = "no plot point";
            if (dpt != null)
            {
                double xVal = (dpt.XValue == 0.0) ? ptIndex + 1 : dpt.XValue;
                sMsg = String.Format("X={0:0.#}  Y={1:0.#}", xVal, dpt.YValues[0]);
            }
            CBVStatMessage.Show(sRec + sMsg);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        void CBVChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            // show point info on status line
            if (!this.Selected) // related to CSBR-128901
                return;

            HitTestResult htr = this.HitTest(e.X, e.Y, ChartElementType.DataPoint);
            if (htr != null && htr.Object != null && htr.PointIndex >= 0)
            {
                int iRow = PointIndexToRow(htr.PointIndex);
                UpdatePlotStatLine(iRow);
            }
        }
        //---------------------------------------------------------------------
        private Point m_mouseDownPoint;
        void CBVChartControl_MouseDown(object sender, MouseEventArgs e)
        {
            m_mouseDownPoint = new Point(e.X, e.Y);

            // on right-press: select this plot, so context menu will apply to it
            if (e.Button == MouseButtons.Right && Form != null && Form.SelectedPlot != this)
                Form.SelectedPlot = this;
        }
        //---------------------------------------------------------------------
        void CBVChartControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            // if drag was for zooming, reset to select mode, do not select anything
            if (m_bZoomOnDragX || m_bZoomOnDragY)
            {
                SetZoomOnDragModes(false, false);
                return;
            }

            // on click, select this plot
            int small = 3;
            ChemBioVizForm form = this.Form;
            if (form == null) return;
            bool bJustAClick = Math.Abs(e.X - m_mouseDownPoint.X) < small && Math.Abs(e.Y - m_mouseDownPoint.Y) < small;
            if (bJustAClick && form != null && form.SelectedPlot != this)
                form.SelectedPlot = this;

            // no point selection allowed in subform plot
            if (this.IsSubformPlot)
                return;

            // get limits of selection rect
            ChartArea area = this.ChartAreas[0];
            double xCursorS = Math.Min(area.CursorX.SelectionStart, area.CursorX.SelectionEnd);
            double xCursorE = Math.Max(area.CursorX.SelectionStart, area.CursorX.SelectionEnd);
            double yCursorS = Math.Min(area.CursorY.SelectionStart, area.CursorY.SelectionEnd);
            double yCursorE = Math.Max(area.CursorY.SelectionStart, area.CursorY.SelectionEnd);
            int nRowToHilite = -1;
            //Coverity bug Fix  : Local Analysis
            Pager pgr = form.Pager;
            if (pgr != null)
            {
                nRowToHilite = pgr.CurrRow;
            }
            int nPointToHilite = RowToPointIndex(nRowToHilite);

            // loop points: color selected, hilite current, uncolor the rest
            bool bShifted = Control.ModifierKeys == Keys.Shift;
            if (!bShifted)
                m_selectedRecs.Clear();
            for (int ip = 0; ip < Series[0].Points.Count; ++ip)
            {
                DataPoint point = Series[0].Points[ip];
                if (point.IsEmpty)
                    continue;

                // determine if point is enclosed by selection rect
                double xVal = (point.XValue == 0.0) ? ip : point.XValue;
                bool bEnclosed = (xVal >= xCursorS && xVal <= xCursorE &&
                                point.YValues[0] >= yCursorS && point.YValues[0] <= yCursorE);
                int iRow = PointIndexToRow(ip);
                bool bIsSelected = m_selectedRecs.Contains(iRow);

                // add to or su btract from selection
                if (bEnclosed && !bIsSelected)
                    m_selectedRecs.Add(iRow);
                else if (bEnclosed && bShifted && bIsSelected)
                    m_selectedRecs.Remove(iRow);

                // color the point
                HilitePoint(point, ip == nPointToHilite);
            }
            // remove selection rect
            area.CursorX.SelectionStart = area.CursorX.SelectionEnd = 0;
            area.CursorY.SelectionStart = area.CursorY.SelectionEnd = 0;

            // if clicked on point, go there
            HitTestResult htr = this.HitTest(e.X, e.Y, ChartElementType.DataPoint);
            if (bJustAClick && htr.Object != null && htr.PointIndex >= 0)
            {
                int iRow = PointIndexToRow(htr.PointIndex);
                if (iRow >= 0)
                {
                    this.Form.DoMove(Pager.MoveType.kmGoto, iRow);
                    UpdatePlotStatLine(iRow);
                    return;
                }
            }

            // put selection count on status bar
            if (m_selectedRecs.Count > 0)
            {
                String sMsg = String.Format("{0} points selected", m_selectedRecs.Count);
                CBVStatMessage.Show(sMsg);
                //} else { 
                //    CBVStatMessage.ShowReadyMsg();
            }
        }
        //---------------------------------------------------------------------
        public bool IsPointSelected(DataPoint dpt)
        {
            int ip = Series[0].Points.IndexOf(dpt);
            int iRow = PointIndexToRow(ip);
            return m_selectedRecs.Contains(iRow);
        }
        //---------------------------------------------------------------------
        public void RedrawSelected()
        {
            // redraw selected points; for use when changing sel color
            foreach (int iRow in m_selectedRecs)
            {
                int ip = RowToPointIndex(iRow);
                DataPoint point = Series[0].Points[ip];
                point.MarkerColor = m_selectedColor;
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion
    //---------------------------------------------------------------------
    #region Designer
    /// <summary>
    /// Custom designer for cbv chart control
    /// See ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/dv_fxdeveloping/html/a6814169-fa7d-4527-808c-637ca5c95f63.htm
    /// </summary>
    public class CBVChartDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        private DesignerActionListCollection actionLists;
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionLists == null)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(new CBVChartActionList(this.Component));
                }
                return actionLists;
            }
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Custom designer action list for cbv chart control
    /// </summary>
    public class CBVChartActionList : DesignerActionList
    {
        public CBVChartActionList(IComponent component)
            : base(component)
        {
            this.m_chartControl = component as CBVChartControl;

            // Cache a reference to DesignerActionUIService, so the actionList can be refreshed.
            this.designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }
        private CBVChartControl m_chartControl;
        private DesignerActionUIService designerActionUISvc = null;

        //---------------------------------------------------------------------
        public CBVChartControl ChartControl
        {
            get { return m_chartControl; }
        }
        //---------------------------------------------------------------------
        // bound designer items: drop-downs for X, Y fields; plot label
        // plus link to plot style dialog

        /* TO DO: see note from doc:
           When a property or method in the class derived from DesignerActionList changes the state of the associated control, 
         * these changes should not be made by direct setter calls to the component's properties. 
         * Instead, such changes should be made through an appropriately created PropertyDescriptor. 
         * This indirect approach ensures that smart-tag undo and UI update actions function correctly. */

        [TypeConverterAttribute(typeof(XFieldStringConverter))]
        public String XField
        {
            get { return String.IsNullOrEmpty(m_chartControl.XField) ? CBVConstants.RECNO_NAME : m_chartControl.XField; }
            set
            {
                m_chartControl.XField = (value as String).Equals(CBVConstants.RECNO_NAME) ? "" : value;
                SetDesignerDirty();
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(YFieldStringConverter))]
        public String YField
        {
            get { return m_chartControl.YField; }
            set { m_chartControl.YField = value; SetDesignerDirty(); }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(DataSourceConverter))]
        public String XTable
        {
            get { return m_chartControl.XTable; }
            set
            {
                bool bChanging = m_chartControl.XTable != null && !m_chartControl.XTable.Equals(value as String);
                if (bChanging)
                    XField = XAggregate = "";
                m_chartControl.XTable = value;
                SetDesignerDirty();
                this.designerActionUISvc.Refresh(this.Component);
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(DataSourceConverter))]
        public String YTable
        {
            get { return m_chartControl.YTable; }
            set
            {
                bool bChanging = m_chartControl.YTable != null && !m_chartControl.YTable.Equals(value as String);
                if (bChanging)
                    YField = YAggregate = "";
                m_chartControl.YTable = value;
                this.designerActionUISvc.Refresh(this.Component);
                SetDesignerDirty();
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(AggregConverter))]
        public String XAggregate
        {
            get { return m_chartControl.XAggregate; }
            set { m_chartControl.XAggregate = value; SetDesignerDirty(); }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(AggregConverter))]
        public String YAggregate
        {
            get { return m_chartControl.YAggregate; }
            set { m_chartControl.YAggregate = value; SetDesignerDirty(); }
        }
        //---------------------------------------------------------------------
        public String PlotTitle
        {
            get { return m_chartControl.MainTitle; }
            set { m_chartControl.MainTitle = value; SetDesignerDirty(); }
        }
        //---------------------------------------------------------------------
        public void ShowPlotProperties()
        {
            // make sure current plot is selected; might not be if we are in form editor
            if (m_chartControl != null && m_chartControl.Form != null)
            {
                m_chartControl.Form.ChartController.SelectedPlot = m_chartControl;
                m_chartControl.Form.ChartController.ModalPlotDialog(false); // do not allow cancel
            }
        }
        //---------------------------------------------------------------------
        private void SetDesignerDirty()
        {
            // tell the designer it's modified
            // better would be: chartControl.Form.TabManager.Control
            FormViewControl fvc = m_chartControl.Parent.Parent.Parent.Parent as FormViewControl;
            if (fvc != null)
            {
                Greatis.FormDesigner.Designer d = fvc.Designer;
                d.SetDirty();
            }
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Data"));
            items.Add(new DesignerActionHeaderItem("Style"));

            items.Add(new DesignerActionPropertyItem("XTable",
                        "X Data Table", "Data", "Main or subtable for horizontal axis"));
            items.Add(new DesignerActionPropertyItem("XField",
                        "X Axis Field", "Data", "Value to be plotted on horizontal axis"));
            items.Add(new DesignerActionPropertyItem("XAggregate",
                        "X Axis Aggregate", "Data", "X Aggregate function if subtable"));

            items.Add(new DesignerActionPropertyItem("YTable",
                        "Y Data Table", "Data", "Main or subtable for vertical axis"));
            items.Add(new DesignerActionPropertyItem("YField",
                        "Y Axis Field", "Data", "Value to be plotted on vertical axis"));
            items.Add(new DesignerActionPropertyItem("YAggregate",
                        "Y Axis Aggregate", "Data", "Y Aggregate function if subtable"));

            items.Add(new DesignerActionPropertyItem("PlotTitle",
                        "Title", "Style", "Plot title"));
            items.Add(new DesignerActionMethodItem(this, "ShowPlotProperties",
                        "Plot Style...", "Style", "Set style and details", true));

            return items;
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// String converter to provide list of fields for plotting
    /// </summary>
    public class PlotFieldsStringConverter : StringConverter
    {
        protected bool m_bIsXAxis;

        public PlotFieldsStringConverter()
        {
            m_bIsXAxis = true;
        }
        //---------------------------------------------------------------------
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.Instance is CBVChartControl || context.Instance is CBVChartActionList;
        }
        //---------------------------------------------------------------------
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // means no values are allowed except those we offer
        }
        //---------------------------------------------------------------------
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Coverity Bug Fix CID 12923 
            CBVChartControl chartControl = null;
            //CBVChartControl chartControl = (context != null && (context.Instance is CBVChartControl)) ? context.Instance as CBVChartControl :
            //    (context != null && (context.Instance is CBVChartActionList)) ? (context.Instance as CBVChartActionList).ChartControl : null;
            if (context != null)
            {
                if (context.Instance is CBVChartControl)
                    chartControl = context.Instance as CBVChartControl;
                if (context.Instance is CBVChartActionList)
                    //Coverity fix - CID 12923
                    chartControl = ((CBVChartActionList)context.Instance).ChartControl;
            }
            List<String> valuesList = new List<String>();
            if (chartControl != null)
            {
                String subTableName = m_bIsXAxis ? chartControl.XTable : chartControl.YTable;
                int subTableID = chartControl.IsMainTablePlot() ? 0 : chartControl.GetSubTableID(subTableName);
                valuesList = chartControl.Form.FormDbMgr.GetFieldList(false, false, true, subTableID, true);    // no blank
                if (m_bIsXAxis)
                    valuesList.Add(CBVConstants.RECNO_NAME);
            }
            StandardValuesCollection vals = new StandardValuesCollection(valuesList);
            return vals;
        }
    }
    //---------------------------------------------------------------------
    public class XFieldStringConverter : PlotFieldsStringConverter
    {
        public XFieldStringConverter()
        {
            m_bIsXAxis = true;
        }
    }
    //---------------------------------------------------------------------
    public class YFieldStringConverter : PlotFieldsStringConverter
    {
        public YFieldStringConverter()
        {
            m_bIsXAxis = false;
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// String converter to provide list of aggregate functions
    /// </summary>
    public class AggregConverter : PlotFieldsStringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<String> valuesList = new List<String>();
            valuesList.Add("");
            valuesList.Add("AVG");
            valuesList.Add("MAX");
            valuesList.Add("MIN");
            StandardValuesCollection vals = new StandardValuesCollection(valuesList);
            return vals;
        }
    }
    //---------------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------------
    #region PointFilter
    public class CBVDataPointFilter : IDataPointFilter
    {
        private CBVChartControl m_chartControl;

        public CBVDataPointFilter(CBVChartControl chartControl)
        {
            m_chartControl = chartControl;
        }
        public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
        {
            return m_chartControl.CBVFilters.FilterDataPoint(m_chartControl, point, pointIndex);
        }
    }
    #endregion
    //---------------------------------------------------------------------
    #region ObjectConverter
    public class UntitledExpandableConverter : ExpandableObjectConverter
    {
        // return blank for string; avoids showing class names in prop grid for expandable items
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                       object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String))
                return "";
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    #endregion
    //---------------------------------------------------------------------
    #region Filters
    /// <summary>
    /// Data object to filter a single field
    /// </summary>
    [SerializableAttribute]
    public class CBVPlotFilter
    {
        #region Variables
        private String m_fieldName;
        private double m_dFilterMin, m_dFilterMax;
        private double m_dDataMin, m_dDataMax;
        #endregion

        #region Constructor
        public CBVPlotFilter()
        {
            Clear();
        }
        public void Clear()
        {
            m_dFilterMin = m_dDataMin = CBVChartPanel.DefaultMin;
            m_dFilterMax = m_dDataMax = CBVChartPanel.DefaultMax;
            m_fieldName = String.Empty;
        }
        #endregion

        #region Properties
        public String FieldName
        {
            get { return m_fieldName; }
            set { m_fieldName = value; }
        }
        public double FilterMin
        {
            get { return m_dFilterMin; }
            set { m_dFilterMin = value; }
        }
        public double FilterMax
        {
            get { return m_dFilterMax; }
            set { m_dFilterMax = value; }
        }
        public double DataMin
        {
            get { return m_dDataMin; }
            set { m_dDataMin = value; }
        }
        public double DataMax
        {
            get { return m_dDataMax; }
            set { m_dDataMax = value; }
        }
        #endregion
    }
    #endregion
    //---------------------------------------------------------------------
    #region FilterCollection
    /// <summary>
    /// Collection of filter objects belonging to a plot
    /// </summary>
    [SerializableAttribute]
    public class CBVFilterCollection : List<CBVPlotFilter>
    {
        #region Constructor
        public CBVFilterCollection()
        {
        }
        #endregion

        #region Methods
        public List<String> GetFieldList()
        {
            List<String> strs = new List<String>();
            foreach (CBVPlotFilter filter in this)
                if (!String.IsNullOrEmpty(filter.FieldName))
                    strs.Add(filter.FieldName);
            return strs;
        }
        //---------------------------------------------------------------------
        public bool FilterDataPoint(CBVChartControl chartControl, DataPoint point, int pointIndex)
        {
            // return true if point is to be filtered out
            foreach (CBVPlotFilter filter in this)
            {
                if (!String.IsNullOrEmpty(filter.FieldName))
                {
                    DataSet plotDS = chartControl.PlotDataSet;
                    if (plotDS.Tables.Count > 0)
                    {
                        DataTable table = plotDS.Tables[0];
                        DataColumn col = table.Columns[filter.FieldName];
                        int rowIndex = chartControl.PointIndexToRow(pointIndex);
                        if (col != null && rowIndex >= 0 && rowIndex < table.Rows.Count)
                        {
                            String sFilterValue = table.Rows[rowIndex][col].ToString();
                            if (String.IsNullOrEmpty(sFilterValue))
                                return true;    // never happens

                            double dFilterValue = CBVUtil.StrToDbl(sFilterValue);
                            if (dFilterValue < filter.FilterMin)
                                return true;
                            if (dFilterValue > filter.FilterMax)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
    #endregion
    //---------------------------------------------------------------------
    #region ChartController
    /// <summary>
    /// Controller communicating between selected plot and dialog/pane UI devices
    /// </summary>
    public class CBVChartController
    {
        private ChemBioVizForm m_form;
        private CBVChartControl m_selectedPlot;
        private CBVChartPanel m_plotControls;

        #region Constructor
        public CBVChartController(ChemBioVizForm form)
        {
            m_form = form;
            m_plotControls = new CBVChartPanel(m_form);
            m_selectedPlot = null;
        }
        #endregion

        #region Properties
        public CBVChartControl SelectedPlot
        {
            get { return m_selectedPlot; }
            set
            {
                // deselect previous if different
                if (m_selectedPlot != value && m_selectedPlot != null)
                    m_selectedPlot.Selected = false;

                // select the given plot
                m_selectedPlot = value;
                if (m_selectedPlot != null)
                {
                    m_selectedPlot.Selected = true;
                    m_selectedPlot.UpdatePlotStatLine(this.m_form.Pager.CurrRow);   // related to CSBR-128901
                }

                // pass plot's info to plot panel
                m_plotControls.Bind(m_form, m_selectedPlot);
            }
        }
        //---------------------------------------------------------------------
        public CBVChartPanel PlotControls
        {
            get { return m_plotControls; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        public void ModalPlotDialog(bool bAllowCancel)
        {
            CBVChartDialog plotDialog = new CBVChartDialog(m_form, bAllowCancel);
            plotDialog.BindToChartControl(m_selectedPlot);
            plotDialog.Modified = false;
            plotDialog.ShowDialog();
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion
    //---------------------------------------------------------------------
}

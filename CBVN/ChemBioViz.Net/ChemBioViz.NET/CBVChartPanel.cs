using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

using CBVControls;
using CBVUtilities;
using Utilities;
using FormDBLib;

namespace ChemBioViz.NET
{
    #region ChartPanel
    /// <summary>
    /// Control panel for plotting, shown in explorer bar
    /// </summary>
    public partial class CBVChartPanel : Form
    {
        #region Variables
        private CBVChartControl m_chartControl;
        private ChemBioVizForm m_form;
        private bool m_bHasUncommittedChanges;
        #endregion

        #region Constructor
        public CBVChartPanel(ChemBioVizForm form)
        {
            m_form = form;
            InitializeComponent();
            this.TopLevel = false;
            m_bHasUncommittedChanges = false;
        }
        #endregion

        #region Properties
        public CBVChartControl ChartControl
        {
            get { return this.m_chartControl; }
        }
        //---------------------------------------------------------------------
        private bool Modified
        {
            set { if (m_chartControl != null && m_chartControl.Form != null)
                m_chartControl.Form.Modified = value; }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Total occupied height: bottom edge of bottom-most control
        /// </summary>
        public int MaxY
        {
            get {
                int maxY = 0;
                foreach (Control c in this.Controls)
                    if (c.Bottom > maxY) maxY = c.Bottom;
                return maxY;
            }
        }
        //---------------------------------------------------------------------
        static public double DefaultMin
        {
            get { return 0.0; }
        }
        //---------------------------------------------------------------------
        static public double DefaultMax
        {
            get { return 100.0; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        void cbvDualSliderControl1_ValueChanged(object sender, SliderControlEventArgs e)
        {
            CBVFilterControl fc = (sender as CBVDualSliderControl).Parent.Parent as CBVFilterControl;
            if (fc == null)
                return;

            if (e.m_bLeft)
                fc.PlotFilter.FilterMin = e.m_dPos;
            else
                fc.PlotFilter.FilterMax = e.m_dPos;
            m_chartControl.Rebind(false, false);
            Modified = true;
        }
        //---------------------------------------------------------------------
        private void linkLabelFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_chartControl != null)
                AddFilterControl();
        }
        //---------------------------------------------------------------------
        private void linkLabelProps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_form.ChartController.ModalPlotDialog(true);
        }
        //---------------------------------------------------------------------
        private void linkLabelApply_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DlgToChart())
            {
                m_chartControl.Rebind(false, true); // retrieve new data
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void linkLabelRevert_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_bHasUncommittedChanges = false;   // prevent warning
            ChartToDlg();
        }
        //---------------------------------------------------------------------
        private void comboBoxXFld_DropDown(object sender, EventArgs e)
        {
            FillFieldCombo(comboBoxX.SelectedItem as String, comboBoxXFld, true);
        }
        //---------------------------------------------------------------------
        private void comboBoxYFld_DropDown(object sender, EventArgs e)
        {
            FillFieldCombo(comboBoxY.SelectedItem as String, comboBoxYFld, false);
        }
        //---------------------------------------------------------------------
        private void any_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_bHasUncommittedChanges = true;
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void comboBoxX_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFieldCombo(comboBoxX.SelectedItem as String, comboBoxXFld, true);
            this.comboBoxXAgg.SelectedIndex = -1;
            any_SelectedIndexChanged(sender, e);
        }
        //---------------------------------------------------------------------
        private void comboBoxY_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFieldCombo(comboBoxY.SelectedItem as String, comboBoxYFld, false);
            this.comboBoxYAgg.SelectedIndex = -1;
            any_SelectedIndexChanged(sender, e);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods

        private void CheckAxisDimmings(int axisNo, ComboBox comboAgg)
        {
            String xTable = comboBoxX.SelectedItem as String, yTable = comboBoxY.SelectedItem as String;
            bool bNeedsAgg = m_chartControl != null && m_chartControl.Form != null &&
                            m_chartControl.RequiresAggregSpec(axisNo, xTable, yTable);
            if (!bNeedsAgg)
                comboAgg.SelectedIndex = -1;
        }
        //---------------------------------------------------------------------
        private void CheckButtons()
        {
            bool bShowLinks = m_bHasUncommittedChanges;
            CheckAxisDimmings(0, comboBoxXAgg);
            CheckAxisDimmings(1, comboBoxYAgg);

            this.linkLabelApply.Visible = bShowLinks;
            this.linkLabelRevert.Visible = bShowLinks;
            m_bHasUncommittedChanges = bShowLinks;
        }
        //---------------------------------------------------------------------
        private void ChartToDlg()
        {
            if (m_chartControl != null && m_bHasUncommittedChanges && OkToApplyChanges())
            {
                DlgToChart();
                m_chartControl.Rebind(false, true); // retrieve new data
                Modified = true;
            }
            comboBoxXFld.SelectedItem = comboBoxYFld.SelectedItem = comboBoxX.SelectedItem = null;
            comboBoxY.SelectedItem = comboBoxXAgg.SelectedItem = comboBoxYAgg.SelectedItem = null;
            ClearCombos();
            this.RemoveFilterControls();
            if (m_chartControl != null)
            {
                FillTableCombos();
                FillFieldCombo(m_chartControl.XTable, comboBoxXFld, true);
                FillFieldCombo(m_chartControl.YTable, comboBoxYFld, false);

                comboBoxXFld.SelectedItem = String.IsNullOrEmpty(m_chartControl.XField) ? CBVConstants.RECNO_NAME : m_chartControl.XField;
                comboBoxYFld.SelectedItem = m_chartControl.YField;
                comboBoxX.SelectedItem = m_chartControl.XTable;
                comboBoxY.SelectedItem = m_chartControl.YTable;
                comboBoxXAgg.SelectedItem = m_chartControl.XAggregate;
                comboBoxYAgg.SelectedItem = m_chartControl.YAggregate;

                this.AddFilterControls();
            }
            m_bHasUncommittedChanges = false;
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private bool OkToApplyChanges()
        {
            String message = "You have made changes in the plot control.  Do you want to apply them to the plot?";
            return MessageBox.Show(message, "Data Source Modified", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        //---------------------------------------------------------------------
        public static bool ValidateChartControlData(CBVChartControl chartControl)
        {
            return ValidateData(chartControl,
                chartControl.XField, chartControl.YField, chartControl.XTable, chartControl.YTable,
                chartControl.XAggregate, chartControl.YAggregate, true);
        }
        //---------------------------------------------------------------------
        private bool ValidateDlgData()
        {
            // make sure we have all the data we need; if not, alert user and return false
            String sXField = comboBoxXFld.SelectedItem as String;
            String sYField = comboBoxYFld.SelectedItem as String;
            String sXTable = comboBoxX.SelectedItem as String;
            String sYTable = comboBoxY.SelectedItem as String;
            String sXAggregate = comboBoxXAgg.SelectedItem as String;
            String sYAggregate = comboBoxYAgg.SelectedItem as String;
            return ValidateData(m_chartControl, sXField, sYField, sXTable, sYTable, sXAggregate, sYAggregate, false);
        }
        //---------------------------------------------------------------------
        private static bool ValidateData(CBVChartControl chartControl, 
            String sXField, String sYField, String sXTable, String sYTable, String sXAggregate, String sYAggregate,
            bool EmptyXFieldOK)
        {
            // make sure fields are non-blank
            String sMessage = String.Empty;
            if (String.IsNullOrEmpty(sXTable))
                sMessage = String.Format("Select a table for the X axis.");
            else if (String.IsNullOrEmpty(sYTable))
                sMessage = String.Format("Select a table for the Y axis.");
            else if (!EmptyXFieldOK && String.IsNullOrEmpty(sXField))
                sMessage = String.Format("Select a field for the X axis in table '{0}'.", sXTable);
            else if (String.IsNullOrEmpty(sYField))
                sMessage = String.Format("Select a field for the Y axis in table '{0}'.", sYTable);

            // check that cross-table child has aggreg specified
            if (chartControl != null && String.IsNullOrEmpty(sMessage))
            {
                bool bNeedsAggX = chartControl.RequiresAggregSpec(0, sXTable, sYTable);
                bool bNeedsAggY = chartControl.RequiresAggregSpec(1, sXTable, sYTable);
                if (bNeedsAggX && String.IsNullOrEmpty(sXAggregate))
                    sMessage = String.Format("Select an aggregate function for the X axis on field '{0}'.", sXField);
                else if (bNeedsAggY && String.IsNullOrEmpty(sYAggregate))
                    sMessage = String.Format("Select an aggregate function for the Y axis on field '{0}'.", sYField);
            }

            // TO DO: if subform plot, make sure form contains subform with selected fields


            // alert user if errors
            if (!String.IsNullOrEmpty(sMessage))
            {
                MessageBox.Show(sMessage, "Invalid Plot Definition", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        //---------------------------------------------------------------------
        private bool DlgToChart()
        {
            if (!ValidateDlgData())
                return false;

            if (m_chartControl != null)
            {
                String selX = comboBoxXFld.SelectedItem as String;
                m_chartControl.XField = selX.Equals(CBVConstants.RECNO_NAME) ? "" : selX;
                m_chartControl.YField = comboBoxYFld.SelectedItem as String;
                m_chartControl.XTable = comboBoxX.SelectedItem as String;
                m_chartControl.YTable = comboBoxY.SelectedItem as String;
                m_chartControl.XAggregate = comboBoxXAgg.SelectedItem as String;
                m_chartControl.YAggregate = comboBoxYAgg.SelectedItem as String;
            }
            m_bHasUncommittedChanges = false;
            CheckButtons();
            return true;
        }
        //---------------------------------------------------------------------
        public void Bind(ChemBioVizForm form, CBVChartControl chartControl)
        {
            // connect to given plot control; if null, disconnect
            m_form = form;
            m_chartControl = chartControl;
            ChartToDlg();
        }
        //---------------------------------------------------------------------
        private void FillFieldCombo(String tableName, ComboBox combo, bool bAddRecordItem)
        {
            Object curSelection = combo.SelectedItem;
            bool bFoundSelection = false;
            combo.Items.Clear();
            if (m_chartControl != null && !String.IsNullOrEmpty(tableName))
            {
                COEDataView.DataViewTable dvTable = m_chartControl.Form.FormDbMgr.FindDVTableByName(tableName);
                int tableID = (dvTable == null) ? 0 : dvTable.Id;

                BindingSource fullBS = m_chartControl.Form.FullBindingSource; // to ensure full source is built
                List<String> fieldList = m_chartControl.Form.FormDbMgr.GetFieldList(false, false, true, tableID, true);
                foreach (String fldName in fieldList)
                {
                    combo.Items.Add(fldName);
                    if (curSelection != null && fldName.Equals(curSelection))
                        bFoundSelection = true;
                }
            }
            if (bAddRecordItem)
            {
                combo.Items.Add(CBVConstants.RECNO_NAME);
                if (curSelection != null && curSelection.Equals(CBVConstants.RECNO_NAME))
                    bFoundSelection = true;
            }
            combo.SelectedItem = bFoundSelection? curSelection : null;
        }
        //---------------------------------------------------------------------
        private void ClearCombos()
        {
            comboBoxX.Items.Clear();
            comboBoxY.Items.Clear();
            comboBoxXFld.Items.Clear();
            comboBoxYFld.Items.Clear();
        }
        //---------------------------------------------------------------------
        private void FillTableCombos()
        {
            FillTableCombo(comboBoxX);
            FillTableCombo(comboBoxY);
        }
        //---------------------------------------------------------------------
        private void FillTableCombo(ComboBox combo)
        {
            combo.Items.Clear();
            if (m_chartControl != null)
            {
                List<String> tableList = m_chartControl.Form.FormDbMgr.GetTableList();
                foreach (String tblName in tableList)
                    combo.Items.Add(tblName);
            }
        }
        #endregion

        #region FilterControls
        //---------------------------------------------------------------------
        public void RemoveFilterControls()
        {
            for (int i = 0; i < Controls.Count; ++i) {
                if (Controls[i] is CBVFilterControl) {
                    Controls.RemoveAt(i);
                    --i;
                }
            }
        }
        //---------------------------------------------------------------------
        public void AddFilterControls()
        {
            if (m_chartControl == null) return;
            foreach (CBVPlotFilter pf in m_chartControl.CBVFilters)
            {
                CBVFilterControl newFC = CreateFilterControl();
                newFC.PlotFilter = pf;
                this.Controls.Add(newFC);
                newFC.LayoutComponents(this.Size.Width);
                newFC.SetValuesFromPlotFilter();
            }
        }
        //---------------------------------------------------------------------
        public CBVFilterControl CreateFilterControl()
        {
            Rectangle rPanel = this.ClientRectangle;
            CBVFilterControl newFC = new CBVFilterControl(rPanel.Width, this.MaxY, m_chartControl);
            newFC.FilterRemoved += new CBVFilterControl.FilterRemoveEventHandler(FilterRemoved);
            newFC.FilterFieldChanged += new CBVFilterControl.FilterFieldChangedEventHandler(FilterFieldChanged);
            newFC.Slider.ValueChanged += new CBVDualSliderControl.ValueChangedEventHandler(cbvDualSliderControl1_ValueChanged);
            return newFC;
        }
        //---------------------------------------------------------------------
        public void AddFilterControl()
        {
            if (m_chartControl != null)
            {
                CBVFilterControl newFC = CreateFilterControl();
                newFC.PlotFilter = new CBVPlotFilter();
                m_chartControl.CBVFilters.Add(newFC.PlotFilter);
                this.Controls.Add(newFC);
                newFC.LayoutComponents(this.Size.Width);
                newFC.SetValuesFromPlotFilter();
            }
        }
        //---------------------------------------------------------------------
        private void UpdateDsetIfNeeded()
        {
            // if current RC does not contain all required fields, rebuild RC and DS
            bool bNeedsUpdate = false;
            foreach (CBVPlotFilter filter in this.m_chartControl.CBVFilters)
            {
                String fieldName = filter.FieldName;
                if (String.IsNullOrEmpty(fieldName)) continue;
                if (!FormUtil.RCHasField(fieldName, this.m_chartControl.Form.FormDbMgr.SelectedDataViewTable,
                                                    this.m_chartControl.PlotResultsCriteria.Tables[0]))
                    bNeedsUpdate = true;
            }
            if (bNeedsUpdate)
            {
                this.m_chartControl.ResetPlot();
                DataSet dsNew = this.m_chartControl.PlotDataSet;    // retrieve now instead of on first drag
            }
        }
        //---------------------------------------------------------------------
        static public bool GetDataFieldMinMax(DataSet dataSet, String fieldName, ref double dMin, ref double dMax)
        {
            // general routine to loop through dataset finding min/max of given column
            double dMin0 = dMin, dMax0 = dMax;
            dMin = 99999.9; dMax = -99999.9;
            if (!String.IsNullOrEmpty(fieldName) &&
                dataSet.Tables.Count > 0 && dataSet.Tables[0].Columns.Contains(fieldName))
            {
                DataTable table = dataSet.Tables[0];    // assume filtering on base table
                DataColumn dataCol = dataSet.Tables[0].Columns[fieldName];
                Debug.Assert(dataCol != null);

                foreach (DataRow row in table.Rows)
                {
                    String sValue = row[dataCol].ToString();
                    if (!String.IsNullOrEmpty(sValue))
                    {
                        double dValue = CBVUtil.StrToDbl(sValue);
                        if (dValue < dMin) dMin = dValue;
                        if (dValue > dMax) dMax = dValue;
                    }
                }
            }
            if (dMin >= dMax)   // i.e., bogus or no results
            {
                dMin = dMin0;
                dMax = dMax0;
                return false;
            }
            return true;
        }
        //---------------------------------------------------------------------
        private void FilterFieldChanged(object sender, CBVUtil.StringEventArgs e)
        {
            // retrieve min, max for selected field
            // inform fc->slider and/or filter obj attached to fc

            CBVFilterControl fc = sender as CBVFilterControl;
            String newFieldName = e.String, oldFieldName = fc.PlotFilter.FieldName;

            // retrieve a new dataset if we added a field
            fc.PlotFilter.FieldName = newFieldName;
            UpdateDsetIfNeeded();

            // get the min and max of the filtering field
            double dMin = DefaultMin, dMax = DefaultMax;
            bool bValidFilter = GetDataFieldMinMax(this.m_chartControl.PlotDataSet, newFieldName, ref dMin, ref dMax);
            if (!bValidFilter)
            {
                // if we failed to get min/max, alert and go back to empty filter
                if (!String.IsNullOrEmpty(newFieldName))
                    MessageBox.Show(String.Concat("Field '", newFieldName, "' is empty or invalid, cannot be used for filtering."),
                        "Invalid Filter Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fc.ClearFilter();
            }

            fc.Slider.ValueMin = dMin;  // this should all be a method of fc
            fc.Slider.ValueMax = dMax;
            fc.PlotFilter.DataMin = dMin;
            fc.PlotFilter.DataMax = dMax;
            fc.PlotFilter.FilterMin = fc.PlotFilter.DataMin;
            fc.PlotFilter.FilterMax = fc.PlotFilter.DataMax;

            fc.Slider.Refresh();
            m_chartControl.Rebind(false, false);
            Modified = true;
        }
        //---------------------------------------------------------------------
        void FilterRemoved(object sender, FilterRemoveControlArgs e)
        {
            CBVFilterControl fc = e.m_filterControl;

            this.m_chartControl.CBVFilters.Remove(fc.PlotFilter);

            int yTop = fc.Location.Y;
            foreach (Control c in this.Controls)
            {
                if (c.Location.Y > yTop)
                    c.Location = new Point(c.Location.X, c.Location.Y - fc.Size.Height);
            }
            this.Controls.Remove(fc);
            this.m_chartControl.Rebind(false, false);
            Modified = true;
        }
        #endregion
    }
    #endregion
}
//---------------------------------------------------------------------
namespace CBVControls
{
    #region Slider
    public class SliderControlEventArgs : EventArgs
    {
        //---------------------------------------------------------------------
        public bool m_bLeft;
        public double m_dPos;
        public SliderControlEventArgs(bool bLeft, double dPos)
        {
            m_bLeft = bLeft;
            m_dPos = dPos;
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Slider control with two thumbs, for specifying range
    /// </summary>
    public class CBVDualSliderControl : UserControl
    {
        #region Variables
        private double m_dLeftPosition, m_dRightPosition;   // slider positions in user coords
        private double m_dMin, m_dMax;                      // scale extrema in user coords
        private bool m_bDraggingLeft, m_bDraggingRight;     // true during drag

        private int m_textWid, m_textHgt, m_horizMargin;    // unchanging dists; see constructor
        private int m_shortTick, m_mediumTick, m_longTick;

        private int m_xScaleLeft, m_xScaleRight;            // ends of scale within client rect
        private int m_yCenter;                              // position of red-blue bar, a little above center
        private int m_yTextTop;                             // top of text labels below thumbs
        private int m_yTickBase;                            // position of tick base line
        private Image m_thumbImage;                         // thumb bitmap; load once

        public delegate void ValueChangedEventHandler(Object sender, SliderControlEventArgs e);
        public event ValueChangedEventHandler ValueChanged;
        private enum ThumbPos { None = 0, Left = 1, Right = 2 }
        #endregion

        #region Constructor
        public CBVDualSliderControl()
        {
            m_bDraggingLeft = m_bDraggingRight = false;
            m_thumbImage = null;

            m_textWid = 30;         // max width of label (below thumb)
            m_textHgt = 8;          // label height in pixels
            m_horizMargin = 12;     // dist between client edge and start/end of slider scale
            m_shortTick = 4;
            m_mediumTick = 8;
            m_longTick = 20;

            m_dLeftPosition = m_dMin = ChemBioViz.NET.CBVChartPanel.DefaultMin;
            m_dRightPosition = m_dMax = ChemBioViz.NET.CBVChartPanel.DefaultMax;

            this.MouseDown += new MouseEventHandler(CBVDualSliderControl_MouseDown);
            this.MouseMove += new MouseEventHandler(CBVDualSliderControl_MouseMove);
            this.MouseUp += new MouseEventHandler(CBVDualSliderControl_MouseUp);
            this.SizeChanged += new System.EventHandler(CBVDualSliderControl_SizeChanged);
        }
        #endregion

        #region Properties
        public double LeftPosition
        {
            get { return m_dLeftPosition; }
            set { m_dLeftPosition = value; }
        }
        public double RightPosition
        {
            get { return m_dRightPosition; }
            set { m_dRightPosition = value; }
        }
        public double ValueMin
        {
            get { return m_dMin; }
            set { m_dMin = m_dLeftPosition = value; }
        }
        public double ValueMax
        {
            get { return m_dMax; }
            set { m_dMax = m_dRightPosition = value; }
        }
        #endregion

        #region Events
        void CBVDualSliderControl_SizeChanged(object sender, EventArgs e)
        {
            RecalcDisplayParams();
        }
        //---------------------------------------------------------------------
        void CBVDualSliderControl_MouseDown(object sender, MouseEventArgs e)
        {
            // if touching thumb, begin drag
            Point p = new Point(e.X, e.Y);
            ThumbPos iTouch = IsTouchingThumb(p);
            if (iTouch == ThumbPos.Left) m_bDraggingLeft = true;
            else if (iTouch == ThumbPos.Right) m_bDraggingRight = true;
        }
        //---------------------------------------------------------------------
        void CBVDualSliderControl_MouseUp(object sender, MouseEventArgs e)
        {
            // end drag
            m_bDraggingLeft = m_bDraggingRight = false;
            Invalidate();
        }
        //---------------------------------------------------------------------
        void CBVDualSliderControl_MouseMove(object sender, MouseEventArgs e)
        {
            // if dragging, change position and redraw plot
            if (m_bDraggingLeft || m_bDraggingRight)
            {
                // do nothing if attempting to drag beyond scale end or onto other thumb
                double dNewVal = PercentToPlotValue(ClientToPercent(e.X));
                bool bOutOfRange = (m_bDraggingLeft && dNewVal < m_dMin) || (m_bDraggingRight && dNewVal > m_dMax);
                if ((m_bDraggingLeft && dNewVal >= m_dRightPosition) || (m_bDraggingRight && dNewVal <= m_dLeftPosition))
                    bOutOfRange = true;

                if (!bOutOfRange)
                {
                    // invalidate before and after changing the position
                    ThumbPos which = m_bDraggingLeft ? ThumbPos.Left : ThumbPos.Right;
                    Invalidate(GetThumbRect(which));
                    Invalidate(GetLabelRect(which));
                    if (which == ThumbPos.Left) m_dLeftPosition = dNewVal;
                    else m_dRightPosition = dNewVal;
                    Invalidate(GetThumbRect(which));
                    Invalidate(GetLabelRect(which));

                    // send event to tell plot to repaint (lands in cbvDualSliderControl1_ValueChanged)
                    ValueChanged.Invoke(this, new SliderControlEventArgs(which == ThumbPos.Left, dNewVal));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set internal variables based on current client rect; call on window resize
        /// </summary>
        void RecalcDisplayParams()
        {
            if (m_thumbImage == null)
                m_thumbImage = ChemBioViz.NET.Properties.Resources.Slider_Thumb;

            Rectangle rClient = this.ClientRectangle;
            rClient.Inflate(-1, -1);

            int yCenterlineOffset = -3;
            m_yCenter = yCenterlineOffset + ((rClient.Top + rClient.Bottom) / 2);
            m_yTickBase = rClient.Bottom - 1;
            m_xScaleLeft = rClient.Left + m_horizMargin;
            m_xScaleRight = rClient.Right - m_horizMargin;
            m_yTextTop = m_yCenter + (m_thumbImage.Size.Height / 2) + 2;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Conversions between user and client coords; use percent of dist from left
        /// </summary>
        private double PlotValueToPercent(double dPlotVal)
        {
            return (m_dMax == m_dMin) ? 0 : 100.0 * (dPlotVal - m_dMin) / (m_dMax - m_dMin);
        }
        //---------------------------------------------------------------------
        private double PercentToPlotValue(double dPercent)
        {
            return m_dMin + (dPercent / 100.0) * (m_dMax - m_dMin);
        }
        //---------------------------------------------------------------------
        private int PercentToClient(double dPercent)
        {
            return m_xScaleLeft + (int)(dPercent * (double)(m_xScaleRight - m_xScaleLeft) / 100.0);
        }
        //---------------------------------------------------------------------
        private double ClientToPercent(int vClient)
        {
            return (int)((double)(vClient - m_xScaleLeft) * 100.0 / (double)(m_xScaleRight - m_xScaleLeft));
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Rectangles containing thumbs and labels
        /// </summary>
        private Rectangle GetLabelRect(ThumbPos which)
        {
            int xPos = (which == ThumbPos.Left) ? PercentToClient(PlotValueToPercent(m_dLeftPosition)) :
                                                    PercentToClient(PlotValueToPercent(m_dRightPosition));
            return new Rectangle(xPos - m_textWid / 2, m_yTextTop, m_textWid, m_textHgt);
        }
        //---------------------------------------------------------------------
        private Rectangle GetThumbRect(ThumbPos which)
        {
            Size imageSize = m_thumbImage.Size;
            int xPos = (which == ThumbPos.Left) ? PercentToClient(PlotValueToPercent(m_dLeftPosition)) :
                                                    PercentToClient(PlotValueToPercent(m_dRightPosition));
            return new Rectangle(xPos - imageSize.Width / 2, m_yCenter - imageSize.Height / 2,
                                                    imageSize.Width, imageSize.Height);
        }
        //---------------------------------------------------------------------
        private ThumbPos IsTouchingThumb(Point p)
        {
            if (GetThumbRect(ThumbPos.Left).Contains(p)) return ThumbPos.Left;
            else if (GetThumbRect(ThumbPos.Right).Contains(p)) return ThumbPos.Right;
            return ThumbPos.None;
        }
        //---------------------------------------------------------------------
        public void SetValuesFromPlotFilter(CBVPlotFilter plotFilter)
        {
            m_dLeftPosition = plotFilter.FilterMin;
            m_dRightPosition = plotFilter.FilterMax;
            m_dMin = plotFilter.DataMin;
            m_dMax = plotFilter.DataMax;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Paint
        /// <summary>
        /// Draw given label underneath given thumb, centered in fixed-width space
        /// </summary>
        private void DrawLabel(Graphics graphics, ThumbPos which, String sLabel)
        {
            using (Font font1 = new Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Point))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                //Coverity Bug Fix CBOE-186
                using (Brush b = new SolidBrush(Color.Black))
                {
                    graphics.DrawString(sLabel, font1, b, GetLabelRect(which), stringFormat);
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Draw base line and tick marks dividing space into tenths
        /// </summary>
        private void DrawTicks(Graphics graphics, int xLeft, int xRight, int yPos)
        {
            Pen grayPen = new Pen(Color.Gray, 1);
            for (int i = 0, xCur = xLeft; i < 11; ++i)
            {
                int tickHt = (i == 0 || i == 10) ? m_longTick : (i == 5) ? m_mediumTick : m_shortTick;
                graphics.DrawLine(grayPen, xCur, yPos, xCur, yPos - tickHt);
                xCur = xLeft + (i + 1) * (xRight - xLeft) / 10;
            }
            graphics.DrawLine(grayPen, xLeft, yPos, xRight, yPos);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Draw the slider control with current parameters
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            System.Drawing.Graphics graphics = this.CreateGraphics();
            Pen bluePen = new Pen(Color.Blue, 2);
            Pen redPen = new Pen(Color.Red, 2);

            // draw tickmarks
            DrawTicks(graphics, m_xScaleLeft, m_xScaleRight, m_yTickBase);

            // draw the two thumbs
            Rectangle rLeft = GetThumbRect(ThumbPos.Left), rRight = GetThumbRect(ThumbPos.Right);
            graphics.DrawImage(m_thumbImage, new Point(rLeft.Left, rLeft.Top));
            graphics.DrawImage(m_thumbImage, new Point(rRight.Left, rRight.Top));

            // draw slider bar: red between thumbs, blue outside
            int xLeft = PercentToClient(PlotValueToPercent(m_dLeftPosition));
            int xRight = PercentToClient(PlotValueToPercent(m_dRightPosition));
            int halfImageWid = m_thumbImage.Size.Width / 2;

            graphics.DrawLine(bluePen, new Point(xLeft + halfImageWid, m_yCenter), new Point(xRight - halfImageWid, m_yCenter));
            if (m_xScaleLeft < (xLeft - halfImageWid))
                graphics.DrawLine(redPen, new Point(m_xScaleLeft, m_yCenter), new Point(xLeft - halfImageWid, m_yCenter));
            if (m_xScaleRight > (xRight + halfImageWid))
                graphics.DrawLine(redPen, new Point(xRight + halfImageWid, m_yCenter), new Point(m_xScaleRight, m_yCenter));

            // draw labels below thumbs
            // for some reason we need to avoid drawing A while dragging B, otherwise A looks bold and funky
            if (!m_bDraggingRight)
                DrawLabel(graphics, ThumbPos.Left, String.Format("{0:F1}", m_dLeftPosition));
            if (!m_bDraggingLeft)
                DrawLabel(graphics, ThumbPos.Right, String.Format("{0:F1}", m_dRightPosition));

            base.OnPaint(e);
        }
        #endregion
    }
    #endregion

    #region FilterControl
    public class FilterRemoveControlArgs : EventArgs
    {
        public CBVFilterControl m_filterControl;
        public FilterRemoveControlArgs(CBVFilterControl filterControl)
        {
            m_filterControl = filterControl;
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Control for operating a single filter; includes field combo and slider control
    /// </summary>
    public class CBVFilterControl : UserControl
    {
        #region Variables
        private ComboBox m_fieldCombo;
        private CBVDualSliderControl m_slider;
        private GroupBox m_groupBox;
        private int m_yTop, m_parentWidth;
        private int xMargin, yTopMargin, yGap, comboHgt, sliderHgt, xInset, yBotMargin;
        private String m_fieldName;
        private CBVChartControl m_chartControl;
        private CBVPlotFilter m_plotFilter;

        public delegate void FilterRemoveEventHandler(Object sender, FilterRemoveControlArgs e);
        public event FilterRemoveEventHandler FilterRemoved;
        public delegate void FilterFieldChangedEventHandler(Object sender, CBVUtil.StringEventArgs e);
        public event FilterFieldChangedEventHandler FilterFieldChanged;
        #endregion

        #region Constructor
        public CBVFilterControl(int parentWidth, int yTop, CBVChartControl chartControl)
        {
            m_chartControl = chartControl;
            m_fieldCombo = new ComboBox();
            m_fieldCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            m_slider = new CBVDualSliderControl();
            m_groupBox = new GroupBox();
            m_groupBox.Controls.Add(m_fieldCombo);
            m_groupBox.Controls.Add(m_slider);
            this.Controls.Add(m_groupBox);

            m_groupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_fieldCombo.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_slider.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_fieldName = "<none>";
            m_fieldCombo.DataBindings.Add(new Binding("SelectedIndex", this, "FieldIndex", true, DataSourceUpdateMode.OnPropertyChanged));

            m_fieldCombo.Items.Add("<none>");
            BindingSource fullBS = chartControl.Form.FullBindingSource; // to ensure full source is built  
            // Displays the fields to the Add Filter Combo Box from the tables present in RC. 
            ResultsCriteria rc = chartControl.Form.FormDbMgr.ResultsCriteria;
            for (int i = 0; i <= rc.Tables.Count - 1; i++)
            {
                int ID = rc.Tables[i].Id;
                COEDataView.DataViewTable dvTable = chartControl.Form.FormDbMgr.FindDVTableByID(ID);
                string baseTableName = chartControl.Form.FormDbMgr.SelectedDataView.BaseTableName;
                List<String> fieldList = chartControl.Form.FormDbMgr.GetFieldList(false, false, true, rc.Tables[i].Id, true);
                foreach (String fldName in fieldList)
                {
                    if (dvTable.Name == baseTableName)
                        m_fieldCombo.Items.Add(fldName);
                    else
                        m_fieldCombo.Items.Add(dvTable.Alias + ":" + fldName);
                }
            }
            m_fieldCombo.Items.Add("<remove this filter>");

            xMargin = 12;       // dist from group box left to combo left
            yTopMargin = 16;    // from group box top to combo top
            yBotMargin = 8;     // from slider bottom to group box bottom
            yGap = 0;           // from combo bottom to slider top
            comboHgt = 21;      // height of combo box
            sliderHgt = 46;     // height of slider
            xInset = 8;         // dist from client l/r to groupbox l/r

            m_yTop = yTop;
            m_parentWidth = parentWidth;
            this.SizeChanged += new EventHandler(CBVFilterControl_SizeChanged);
            this.Location = new Point(0, m_yTop);
            this.Size = new Size(parentWidth, TotalHeight);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Properties
        public CBVPlotFilter PlotFilter
        {
            get { return m_plotFilter; }
            set { m_plotFilter = value; }
        }
        //---------------------------------------------------------------------
        public ComboBox Combo
        {
            get { return m_fieldCombo; }
        }
        //---------------------------------------------------------------------
        public int FieldIndex
        {
            get { return m_fieldCombo.FindStringExact(m_fieldName); }
            set
            {
                String newFieldName = this.m_fieldCombo.Items[value].ToString();               
                if (newFieldName.Contains(":"))
                {
                    char delimiter = ':';
                    int delimiterpos = newFieldName.LastIndexOf(delimiter);
                    newFieldName = newFieldName.Substring(delimiterpos + 1);
                }
                if (CBVUtil.StartsWith(newFieldName, "<remove"))
                {
                    FilterRemoved.Invoke(this, new FilterRemoveControlArgs(this));
                }
                else if (!CBVUtil.Eqstrs(m_fieldName, newFieldName))
                {
                    m_fieldName = newFieldName;
                    if (CBVUtil.StartsWith(newFieldName, "<none")) m_fieldName = String.Empty;
                    m_chartControl.Rebind(false, true);     // retrieve new data
                    FilterFieldChanged.Invoke(this, new CBVUtil.StringEventArgs(m_fieldName));
                }
            }
        }
        //---------------------------------------------------------------------
        public String FieldName
        {
            get { return m_fieldName; }
            set { m_fieldName = value; }
        }
        //---------------------------------------------------------------------
        public int TotalHeight
        {
            get { return yTopMargin + comboHgt + yGap + sliderHgt + yBotMargin; }
        }
        //---------------------------------------------------------------------
        public CBVDualSliderControl Slider
        {
            get { return m_slider; }
        }
        #endregion

        #region Events
        void CBVFilterControl_SizeChanged(object sender, EventArgs e)
        {
            Rectangle rClient = this.ClientRectangle;
            LayoutComponents(rClient.Width);
            m_slider.Refresh();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        public void ClearFilter()
        {
            // reset to empty filter
            m_fieldName = "<none>";
            PlotFilter.Clear();
            m_fieldCombo.SelectedIndex = 0;
        }
        //---------------------------------------------------------------------
        public void LayoutComponents(int parentWidth)
        {
            int small = 2;
            Rectangle rGroup = new Rectangle(xInset, 0, parentWidth - 2 * xInset, TotalHeight);
            Rectangle rCombo = new Rectangle(xMargin, yTopMargin, rGroup.Width - 2 * xMargin, comboHgt);
            Rectangle rSlider = new Rectangle(small, yTopMargin + comboHgt + yGap, rGroup.Width - 2 * small, sliderHgt);

            m_groupBox.Location = new Point(rGroup.Left, rGroup.Top);
            m_groupBox.Size = new Size(rGroup.Width, rGroup.Height);

            m_fieldCombo.Location = new Point(rCombo.Left, rCombo.Top);
            m_fieldCombo.Size = new Size(rCombo.Width, rCombo.Height);

            m_slider.Location = new Point(rSlider.Left, rSlider.Top);
            m_slider.Size = new Size(rSlider.Width, rSlider.Height);
        }
        //---------------------------------------------------------------------
        public void SetValuesFromPlotFilter()
        {
            // update control display to match values of plotfilter, as after form load
            this.Slider.SetValuesFromPlotFilter(this.PlotFilter);
            this.FieldName = this.PlotFilter.FieldName;
            if (String.IsNullOrEmpty(FieldName))
                FieldName = "<none>";   // TO DO: make this a const
            this.m_fieldCombo.SelectedIndex = m_fieldCombo.FindStringExact(FieldName);
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion
}


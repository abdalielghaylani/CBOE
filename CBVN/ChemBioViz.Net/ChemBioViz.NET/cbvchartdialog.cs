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

using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using CBVControls;
using FormDBLib;

namespace ChemBioViz.NET
{
    public partial class CBVChartDialog : Form
    {
        private CBVChartControl m_chartControl;
        private ChemBioVizForm m_form;
        private CBVChartControl m_chartControlOrig;
        private static Point m_dialogLocation = Point.Empty;
        private bool m_modified, m_bAllowCancel;

        #region Constructor
        public CBVChartDialog(ChemBioVizForm form, bool bAllowCancel)
        {
            m_form = form;
            m_chartControl = null;
            m_chartControlOrig = null;
            m_modified = false;
            m_bAllowCancel = bAllowCancel;

            this.Load += new EventHandler(CBVChartDialog_Load);
            this.FormClosing += new FormClosingEventHandler(CBVChartDialog_FormClosing);
            InitializeComponent();
        }
        #endregion

        #region Properties
        public bool Modified
        {
            get { return m_modified; }
            set { m_modified = value; }
        }
        #endregion

        #region Methods
        public void PrepForUndo(CBVChartControl chartControl)
        {
            // make a clone of the given control
            m_chartControlOrig = (chartControl == null)? null :
                    (CBVChartControl)ControlFactory.CloneCtrl(chartControl, false);
            if (m_chartControlOrig != null)
                m_chartControlOrig.Enabled = false; // prevent user clicking the clone
        }
        //---------------------------------------------------------------------
        public void Undo()
        {
            // swap the clone in place of the current control
            if (m_chartControlOrig != null && m_chartControl != null && m_chartControl.Parent != null)
            {
                Control cParent = m_chartControl.Parent;
                cParent.Controls.Remove(m_chartControl);

                m_chartControl = m_chartControlOrig;
                m_chartControlOrig = null;

                cParent.Controls.Add(m_chartControl);
                m_chartControl.Enabled = true;
                Application.DoEvents();
                m_chartControl.Rebind(false, false);

                // tell the controller to select the current plot
                if (m_chartControl.Form != null && m_chartControl.Form.SelectedPlot != m_chartControl)
                    m_chartControl.Form.SelectedPlot = m_chartControl;

                m_modified = false;
                Application.DoEvents();
            }
        }
        //---------------------------------------------------------------------
        public void BindToChartControl(CBVChartControl chartControl)
        {
            // put control data into dialog and bind variables to dialog controls
            // called only from controller.Select when changing selection
            if (chartControl != m_chartControl && m_bAllowCancel)
                PrepForUndo(chartControl);

            m_chartControl = chartControl;
            this.cBVChartControlBindingSource.DataSource = m_chartControl;

            BindControl(comboBoxPointStyle, "SelectedIndex", "MarkerStyle");
            BindControl(numericUpDownPointSize, "Value", "MarkerSize");
            BindControl(ultraColorPickerPoint, "Color", "MarkerColor");
            BindControl(comboBoxPlotStyle, "SelectedIndex", "ChartType");
            BindControl(ultraColorPickerBkgr, "Color", "BackgroundColor");
            BindControl(textBoxMainTitle, "Text", "MainTitle");
            BindControl(textBoxXAxis, "Text", "XAxisLabel");
            BindControl(textBoxYAxis, "Text", "YAxisLabel");

            BindControl(ultraColorPickerSelected, "Color", "SelectedMarkerColor");
            BindControl(checkBoxStretch, "Checked", "Stretchable");
            BindControl(comboBoxHiliteStyle, "SelectedIndex", "HiliteMarkerStyle");
            BindControl(numericUpDownHiliteSize, "Value", "HiliteMarkerSize");
            BindControl(ultraColorPickerHilite, "Color", "HiliteMarkerColor");
            BindControl(checkBoxAutoLabel, "Checked", "AutoLabelAxes");
            BindControl(textBoxLegend, "Text", "SeriesTitle");

            BindControl(checkBoxXGrid, "Checked", "XGridVisible");
            BindControl(checkBoxYGrid, "Checked", "YGridVisible");

            Refresh();
        }
        //---------------------------------------------------------------------
        private void BindControl(Control c, String propertyName, String dataMember)
        {
            c.DataBindings.Clear();
            if (m_chartControl != null)
                c.DataBindings.Add(new Binding(propertyName, m_chartControl, dataMember, true, DataSourceUpdateMode.Never));
            c.Enabled = (m_chartControl != null);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        void CBVChartDialog_Load(object sender, EventArgs e)
        {
            if (m_dialogLocation.IsEmpty)
                this.CenterToParent();
            else
                this.Location = m_dialogLocation;

            m_modified = false;

            this.ultraButtonCancel.Enabled = m_bAllowCancel;
        }
        //---------------------------------------------------------------------
        void CBVChartDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dialogLocation = this.Location;
        }
        //---------------------------------------------------------------------
        private void ultraButtonOK_Click(object sender, EventArgs e)
        {
            if (m_modified && m_chartControl != null && m_chartControl.Form != null)
            {
                m_chartControl.Form.Modified = true;
                m_chartControl.Rebind(false, true);
            }
            Close();
        }
        //---------------------------------------------------------------------
        private void ultraButtonCancel_Click(object sender, EventArgs e)
        {
            if (m_modified && m_bAllowCancel)
                Undo();
            Close();
        }
        //---------------------------------------------------------------------
        private void OnAnyChange()
        {
            m_modified = true;
        }
        //---------------------------------------------------------------------
        private void ultraColorPickerBkgr_ColorChanged(object sender, EventArgs e)
        {
            m_chartControl.BackgroundColor = (sender as UltraColorPicker).Color;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void textBoxMainTitle_TextChanged(object sender, EventArgs e)
        {
            m_chartControl.MainTitle = (sender as TextBox).Text;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void textBoxMainXAxis_TextChanged(object sender, EventArgs e)
        {
            m_chartControl.XAxisLabel = (sender as TextBox).Text;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void textBoxMainYAxis_TextChanged(object sender, EventArgs e)
        {
            m_chartControl.YAxisLabel = (sender as TextBox).Text;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void comboBoxPlotStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_chartControl.ChartType = (SeriesChartType)this.comboBoxPlotStyle.SelectedIndex;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void ultraColorPicker1_ColorChanged(object sender, EventArgs e)
        {
            m_chartControl.MarkerColor = this.ultraColorPickerPoint.Color;
            m_chartControl.Rebind(true, false);
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            m_chartControl.MarkerSize = (int)this.numericUpDownPointSize.Value;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_chartControl.MarkerStyle = (MarkerStyle)this.comboBoxPointStyle.SelectedIndex;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void ultraColorPickerSelected_ColorChanged(object sender, EventArgs e)
        {
            m_chartControl.SelectedMarkerColor = this.ultraColorPickerSelected.Color;
            m_chartControl.RedrawSelected();
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void ultraColorPickerHilite_ColorChanged(object sender, EventArgs e)
        {
            m_chartControl.HiliteMarkerColor = this.ultraColorPickerHilite.Color;
            m_chartControl.Rebind(true, false);
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void comboBoxHiliteStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_chartControl.HiliteMarkerStyle = (MarkerStyle)this.comboBoxHiliteStyle.SelectedIndex;
            m_chartControl.Rebind(true, false);
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void numericUpDownHiliteSize_ValueChanged(object sender, EventArgs e)
        {
            m_chartControl.HiliteMarkerSize = (int)this.numericUpDownHiliteSize.Value;
            m_chartControl.Rebind(true, false);
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void checkBoxStretch_CheckedChanged(object sender, EventArgs e)
        {
            m_chartControl.Stretchable = this.checkBoxStretch.Checked;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void textBoxLegend_TextChanged(object sender, EventArgs e)
        {
            m_chartControl.SeriesTitle = (sender as TextBox).Text;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void checkBoxAutoLabel_CheckedChanged(object sender, EventArgs e)
        {
            m_chartControl.AutoLabelAxes = checkBoxAutoLabel.Checked;

            this.textBoxXAxis.Enabled = !checkBoxAutoLabel.Checked;
            this.textBoxYAxis.Enabled = !checkBoxAutoLabel.Checked;
            if (checkBoxAutoLabel.Checked)
            {
                m_chartControl.XField = m_chartControl.XField;  // resets label
                m_chartControl.YField = m_chartControl.YField;
            }
            else
            {
                m_chartControl.XAxisLabel = textBoxXAxis.Text;
                m_chartControl.YAxisLabel = textBoxYAxis.Text;
            }
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void checkBoxXGrid_CheckedChanged(object sender, EventArgs e)
        {
            m_chartControl.XGridVisible = this.checkBoxXGrid.Checked;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        private void checkBoxYGrid_CheckedChanged(object sender, EventArgs e)
        {
            m_chartControl.YGridVisible = this.checkBoxYGrid.Checked;
            OnAnyChange();
        }
        //---------------------------------------------------------------------
        #endregion
    }
}

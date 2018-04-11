using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;

using ChemBioViz.NET;
using FormDBLib;
using CBVControls;
using Utilities;
using CBVUtilities;
using ChemControls;

namespace ChemBioViz.NET
{
    public partial class QueryTextPropsDialog : Form
    {
        #region Variables
        private CBVQueryTextBox m_queryTextBox;
        private CBVUnitsManager m_unitsMgr;

        #endregion

        #region Constructor
        public QueryTextPropsDialog(CBVQueryTextBox queryTextBox)
        {
            m_queryTextBox = queryTextBox;
            InitializeComponent();

            this.boundFieldTextBox.Text = m_queryTextBox.BoundField;
            this.fieldTypeTextBox.Text = m_queryTextBox.BoundFieldType.ToString();
            this.listInputCheckBox.Checked = m_queryTextBox.AllowListInput;
            this.tooltipTextBox.Text = m_queryTextBox.TooltipText;

            String sXml = ChemBioVizForm.GetResourceXmlString();
            m_unitsMgr = new CBVUnitsManager(sXml);

            FillUnitsCombo();
            this.useUnitsCheckBox.Checked = !String.IsNullOrEmpty(m_queryTextBox.Units);
            if (this.useUnitsCheckBox.Checked)
                this.unitsComboBox.SelectedIndex = m_unitsMgr.FindInComboList(m_queryTextBox.Units);

            FillOperatorsCombo();
            this.operatorComboBox.SelectedItem = m_queryTextBox.Operator.ToString();

            FillAggregCombo();
            this.aggregComboBox.SelectedItem = m_queryTextBox.Aggregate;

            CenterToParent();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void FillUnitsCombo()
        {
            // taken from UnitsStringConverter.GetStandardValues (chembiovizform.cs)
            String sXml = ChemBioVizForm.GetResourceXmlString();
            List<String> valuesList = m_unitsMgr.GetComboList();
            foreach (String s in valuesList)
                this.unitsComboBox.Items.Add(s);
        }
        //---------------------------------------------------------------------
        private void FillAggregCombo()
        {
            // taken from StdAggregConverter.GetStandardValues (cbvcontrols.cs)
            List<String> valuesList = CBVTextBox.StdAggregConverter.GetAggregNames();
            valuesList.Insert(0, "");
            foreach (String s in valuesList)
                this.aggregComboBox.Items.Add(s);
        }
        //---------------------------------------------------------------------
        private void FillOperatorsCombo()
        {
            foreach (SearchCriteria.COEOperators op in Enum.GetValues(typeof(SearchCriteria.COEOperators)))
                this.operatorComboBox.Items.Add(op.ToString());
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        private void OKbutton_Click(object sender, EventArgs e)
        {
            m_queryTextBox.Operator = (SearchCriteria.COEOperators)Enum.Parse(
                typeof(SearchCriteria.COEOperators), operatorComboBox.SelectedItem.ToString());
            m_queryTextBox.Units = (this.useUnitsCheckBox.Checked && this.unitsComboBox.SelectedItem != null) ? 
                this.unitsComboBox.SelectedItem.ToString() : String.Empty;
            m_queryTextBox.AllowListInput = this.listInputCheckBox.Checked;
            m_queryTextBox.TooltipText = this.tooltipTextBox.Text;
            m_queryTextBox.Aggregate = this.aggregComboBox.SelectedItem.ToString();

            //if (m_bModified)
            //    SetDesignerDirty(); // CSBR-128735
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        //---------------------------------------------------------------------
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //---------------------------------------------------------------------
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FormDBLib;
using CambridgeSoft.COE.Framework.Common;

namespace ChemBioViz.NET
{
    public partial class AdvancedSearchDialog : Form
    {
        #region Constructor
        public AdvancedSearchDialog()
        {
            // to do: save current search options here in case of cancel
            InitializeComponent();
            this.CenterToParent();
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method serializes current state of the controls in the .settings file
        /// </summary>
        private void SerializeSettings()
        {
            SearchOptionsSettings.Default.AnyTetStereo = anyTetStereoButton.Checked;
            SearchOptionsSettings.Default.EitherTetStereo = eitherTetStereoButton.Checked;
            SearchOptionsSettings.Default.SameTetStereo = sameTetStereoButton.Checked;
            SearchOptionsSettings.Default.AnyDoubleBondStereo = anyDoubleBondStereoButton.Checked;
            SearchOptionsSettings.Default.SameDoubleBondStereo = sameDoubleBondStereoButton.Checked;
        }

        private void EnableControlsToMatchStereochemistry(bool match)
        {
            sameTetStereoButton.Enabled = match;
            eitherTetStereoButton.Enabled = match;
            anyTetStereoButton.Enabled = match;
            tetrahedralStereoGroupBox.Enabled = match;

            sameDoubleBondStereoButton.Enabled = match;
            anyDoubleBondStereoButton.Enabled = match;
            doubleBondGroupBox.Enabled = match;
        }
        #endregion

        #region Events
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            SerializeSettings();
            this.Close();
        }

        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            // TO DO: restore settings saved in constructor
            this.Close();
        }

        private void matchStereochemistryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.EnableControlsToMatchStereochemistry(matchStereochemistryCheckBox.Checked);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpotfireIntegration
{
    public partial class SpotfirePropsDialog : Form
    {
        public SpotfirePropsDialog()
        {
            InitializeComponent();
            this.AutoRefreshCheckBox.Checked = Properties.settings.Default.AutoRefresh;
            this.WarnRefreshCheckBox.Checked = Properties.settings.Default.WarnOnRefresh;
            this.WarnRebuildCheckBox.Checked = Properties.settings.Default.WarnOnRebuild;
            this.maxRowsUpDown.Value = Properties.settings.Default.MaxRows;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Properties.settings.Default.AutoRefresh = this.AutoRefreshCheckBox.Checked;
            Properties.settings.Default.WarnOnRefresh = this.WarnRefreshCheckBox.Checked;
            Properties.settings.Default.WarnOnRebuild = this.WarnRebuildCheckBox.Checked;
            Properties.settings.Default.MaxRows = (int)this.maxRowsUpDown.Value;
            DialogResult = DialogResult.OK;
        }
    }
}

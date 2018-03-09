using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharedLib;

namespace SpotfireIntegration
{
    public partial class RefreshWarningDialog : CheckBoxDialog
    {
        public RefreshWarningDialog()
        {
            this.textBox1.Text = "Do you wish to refresh the Spotfire data table to match the current ChemBioViz hitlist?";
            this.CheckboxCaption = "Always refresh without asking";
        }

        protected override void YesButton_Click(object sender, EventArgs e)
        {
            Properties.settings.Default.WarnOnRefresh = !this.DontAskCheckBox.Checked;
        }

        protected override void NoButton_Click(object sender, EventArgs e)
        {
            Properties.settings.Default.WarnOnRefresh = !this.DontAskCheckBox.Checked;
        }
    }

    public partial class RebuildWarningDialog : CheckBoxDialog
    {
        public RebuildWarningDialog()
        {
            this.textBox1.Text = "This operation will replace the current Spotfire analysis.  Do you wish to continue?";
            this.CheckboxCaption = "Always replace without asking";
        }

        protected override void YesButton_Click(object sender, EventArgs e)
        {
            Properties.settings.Default.WarnOnRebuild = !this.DontAskCheckBox.Checked;
        }

        protected override void NoButton_Click(object sender, EventArgs e)
        {
            Properties.settings.Default.WarnOnRebuild = !this.DontAskCheckBox.Checked;
        }
    }

    public class SizeExceededDialog
    {
        private int m_curSize, m_maxSize;
        public SizeExceededDialog(int curSize, int maxSize)
        {
            this.m_curSize = curSize;
            this.m_maxSize = maxSize;
        }

        public bool Show()
        {
            String msgInfo = String.Format("Hitlist size [{0}] exceeds row limit [{1}].", m_curSize, m_maxSize); // CSBR-156732 Fixed
            String msg1 = "Spotfire cannot load from the current hitlist.";
            String msg2 = String.Format("Start with a smaller hitlist, or increase the limit in Spotfire Preferences.");
            String msg = String.Format("{0}\n\n{1}\n{2}", msgInfo, msg1, msg2);

            MessageBox.Show(msg);
            return false;
        }
    }
}

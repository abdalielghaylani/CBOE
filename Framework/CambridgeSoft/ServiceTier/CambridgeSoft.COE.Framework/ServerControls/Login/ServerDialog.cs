using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    public partial class ServerDialog : Form
    {
        public string ServerName
        {
            get { return NameTextBox.Text; }
        }

        public bool Tier2
        {
            get { return TwoTierRadioButton.Checked; }
        }
        public bool Tier3
        {
            get { return ThreeTierRadioButton.Checked; }
        }
        public bool SSL
        {
            get { return SSLCheckBox.Checked; }
        }

        public ServerDialog()
        {
            InitializeComponent();
            InitializeEvents();
        }

        #region "Events"
        private void InitializeEvents()
        {
            TwoTierRadioButton.CheckedChanged+=new EventHandler(rbo2Tier_CheckedChanged);
            ThreeTierRadioButton.CheckedChanged += new EventHandler(rbo3Tier_CheckedChanged);
            NameTextBox.KeyPress+=new KeyPressEventHandler(txtInput_KeyPress);
            NameTextBox.Enter+=new EventHandler(txtInput_Enter);
            OKButton.Click+=new EventHandler(btnOK_Click);
            CancelButton.Click+=new EventHandler(btnCancel_Click);
            SSLCheckBox.CheckedChanged+=new EventHandler(cbxSSL_CheckedChanged);
            this.Load+=new EventHandler(MRUServer_Load);         
        }
        
        private void rbo2Tier_CheckedChanged(object sender, EventArgs e)
        {
            SSLCheckBox.Enabled = false;
            lblHeader.Text = "Enter Oracle service name";
            NameTextBox.Text = "<Oracle service name>";
            this.NameTextBox.Focus();
            SelectTextboxText(this.NameTextBox);
        }

        private void rbo3Tier_CheckedChanged(object sender, EventArgs e)
        {
            SSLCheckBox.Enabled = true;
            lblHeader.Text = "Enter middle tier server name";
            NameTextBox.Text = "<Middle-tier server name>";
            this.NameTextBox.Focus();
            SelectTextboxText(this.NameTextBox);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.btnOK_Click(sender, e);
        }

        private void txtInput_Enter(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.SelectionStart = 0;
                NameTextBox.SelectionLength = NameTextBox.Text.Length;
            }

        }
        private void cbxSSL_CheckedChanged(object sender, EventArgs e)
        {
            NameTextBox.Focus();
        }

        private void MRUServer_Load(object sender, EventArgs e)
        {
            this.NameTextBox.Focus();
        }
        #endregion "Events"

        #region "Methods"
        private void SelectTextboxText(TextBox txtBox)
        {
            txtBox.SelectionStart = 0;
            txtBox.SelectionLength = NameTextBox.Text.Length;
        }
        #endregion "Methods"


    }
}
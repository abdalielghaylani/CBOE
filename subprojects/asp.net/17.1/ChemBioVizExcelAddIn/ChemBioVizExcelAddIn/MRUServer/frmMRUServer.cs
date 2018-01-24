using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChemBioVizExcelAddIn
{
    public partial class MRUServer : Form
    {
        public string ServerName
        {
            get { return txtInput.Text; }
        }

        public bool Tier2
        {
            get { return rbo2Tier.Checked; }
        }
        public bool Tier3
        {
            get { return rbo3Tier.Checked; }
        }
        public bool SSL
        {
            get { return cbxSSL.Checked; }
        }

        public MRUServer()
        {
            InitializeComponent();
            InitializeEvents();
        }

        #region "Events"
        private void InitializeEvents()
        {
            rbo2Tier.CheckedChanged+=new EventHandler(rbo2Tier_CheckedChanged);
            rbo3Tier.CheckedChanged += new EventHandler(rbo3Tier_CheckedChanged);
            txtInput.KeyPress+=new KeyPressEventHandler(txtInput_KeyPress);
            txtInput.Enter+=new EventHandler(txtInput_Enter);
            btnOK.Click+=new EventHandler(btnOK_Click);
            btnCancel.Click+=new EventHandler(btnCancel_Click);
            cbxSSL.CheckedChanged+=new EventHandler(cbxSSL_CheckedChanged);
            this.Load+=new EventHandler(MRUServer_Load);         
        }
        
        private void rbo2Tier_CheckedChanged(object sender, EventArgs e)
        {
            cbxSSL.Enabled = false;
            lblHeader.Text = "Enter Oracle service name";
            txtInput.Text = "<Oracle service name>";
            this.txtInput.Focus();
            SelectTextboxText(this.txtInput);
        }

        private void rbo3Tier_CheckedChanged(object sender, EventArgs e)
        {
            cbxSSL.Enabled = true;
            lblHeader.Text = "Enter middle tier server name";
            txtInput.Text = "<Middle-tier server name>";
            this.txtInput.Focus();
            SelectTextboxText(this.txtInput);
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
            if (!String.IsNullOrEmpty(txtInput.Text))
            {
                txtInput.SelectionStart = 0;
                txtInput.SelectionLength = txtInput.Text.Length;
            }

        }
        private void cbxSSL_CheckedChanged(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        private void MRUServer_Load(object sender, EventArgs e)
        {
            this.txtInput.Focus();
        }
        #endregion "Events"

        #region "Methods"
        private void SelectTextboxText(TextBox txtBox)
        {
            txtBox.SelectionStart = 0;
            txtBox.SelectionLength = txtInput.Text.Length;
        }
        #endregion "Methods"


    }
}
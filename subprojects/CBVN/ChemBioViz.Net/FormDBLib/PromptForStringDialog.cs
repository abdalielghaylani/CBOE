using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FormDBLib
{
    public partial class PromptForStringDialog : Form
    {
        #region Variables
        private String m_insertedString = String.Empty;
        #endregion

        #region Properties
        public String InsertedString
        {
            get { return m_insertedString; }
            set { m_insertedString = value; }
        }
        #endregion

        #region Constructors
        public PromptForStringDialog(String dialogLabel, String initialValue)
        {
            InitializeComponent();
            this.CenterToParent();
            messageLabel.Text = dialogLabel;
            userStringTextBox.Text = initialValue;
        }
        #endregion

        #region Events
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            m_insertedString = userStringTextBox.Text;
            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private void userStringTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                m_insertedString = userStringTextBox.Text;
                DialogResult = DialogResult.OK;
            }
        }
        #endregion
        





    }
}
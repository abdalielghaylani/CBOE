using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormDBLib
{
    public partial class PromptWithRadio : Form
    {
        #region Variables
        private int m_radioChoice = 0;
        private String m_insertedString = String.Empty;
        private String m_dialogLabel1 = String.Empty;
        private String m_dialogLabel2 = String.Empty;
        private String m_initialValue1 = String.Empty;
        private String m_initialValue2 = String.Empty;
        private String m_checkboxLabel = String.Empty;
        private bool m_bCheckValue = false;
        #endregion

        #region Properties
        public String InsertedString
        {
            get { return m_insertedString; }
            set { m_insertedString = value; }
        }
        public int RadioChoice
        {
            get { return m_radioChoice; }
            set { m_radioChoice = value; }
        }
        public bool Checked
        {
            get { return m_bCheckValue; }
            set { m_bCheckValue = value; }
        }
        #endregion

        #region Methods
        public PromptWithRadio(String dialogLabel, String initialValue,
                    String radio1, String radio2, int radioVal, String label2, String initValue2,
                    String checkboxLabel, bool bCheckVal)
        {
            InitializeComponent();
            CenterToParent();

            m_dialogLabel1 = dialogLabel;
            m_dialogLabel2 = String.IsNullOrEmpty(label2) ? m_dialogLabel1 : label2;
            m_initialValue1 = initialValue;
            m_initialValue2 = String.IsNullOrEmpty(initValue2) ? m_initialValue1 : initValue2;

            m_radioChoice = radioVal;
            radioButton1.Text = radio1;
            radioButton2.Text = radio2;
            radioButton1.Checked = m_radioChoice == 0;
            radioButton2.Checked = m_radioChoice == 1;
            messageLabel.Text = (m_radioChoice == 0) ? m_dialogLabel1 : m_dialogLabel2;
            userStringTextBox.Text = (m_radioChoice == 0) ? m_initialValue1 : m_initialValue2;

            if (String.IsNullOrEmpty(checkboxLabel)) {
                this.checkBox1.Visible = false;
            } else {
                checkBox1.Text = checkboxLabel;
                checkBox1.Checked = bCheckVal;
            }
        }
        #endregion

        #region Events
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            m_insertedString = userStringTextBox.Text;
            m_radioChoice = radioButton1.Checked ? 0 : 1;
            m_bCheckValue = checkBox1.Checked;
            DialogResult = DialogResult.OK;
        }

        private void RadioButton1_Click(object sender, EventArgs e)
        {
            userStringTextBox.Text = m_initialValue1;
            messageLabel.Text = m_dialogLabel1;
        }

        private void RadioButton2_Click(object sender, EventArgs e)
        {
            userStringTextBox.Text = m_initialValue2;
            messageLabel.Text = m_dialogLabel2;
        }

        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void userStringTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKUltraButton_Click(sender, e);
        }
        #endregion
   }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharedLib
{
    public partial class CheckBoxDialog : Form
    {
        // originally intended to be an all-purpose "do not ask/tell again" message box
        // evolved into a bare-bones yes/no dialog with no mechanism for saving state
        #region Variables
        protected bool m_bIsChecked = false;
        #endregion

        #region Constructor
        public CheckBoxDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public bool IsChecked
        {
            get { return m_bIsChecked; }
            set { m_bIsChecked = value; }
        }
        public String CheckboxCaption
        {
            get { return this.DontAskCheckBox.Text; }
            set { this.DontAskCheckBox.Text = value; }
        }
        #endregion

        #region Methods
        protected virtual void YesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        protected virtual void NoButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
        #endregion
    }
}


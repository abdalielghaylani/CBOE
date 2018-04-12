using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.DataLoaderGUI.Properties;

namespace CambridgeSoft.DataLoaderGUI.Forms
{
    public partial class SplitForm : Form
    {
        bool ok = false;

        public bool OK
        {
            get { return this.ok; }
        }

        public string GetNumber
        {
            get
            {
                return _SplitTextBox.Text;
            }
        }

        public SplitForm()
        {
            InitializeComponent();
        }

        private void _OKButton_Click(object sender, EventArgs e)
        {
            int splitNo;
            if (_SplitTextBox.Text.Trim().Length > 0)
            {
                if (int.TryParse(_SplitTextBox.Text.Trim(), out splitNo))
                {
                    if (splitNo <= 0)
                    {
                        MessageBox.Show("The split number above 0.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _SplitTextBox.Focus();
                        _SplitTextBox.SelectAll();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("The split number must be a number", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _SplitTextBox.Focus();
                    _SplitTextBox.SelectAll();
                    return;
                }
            }

            if (string.IsNullOrEmpty(_SplitTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input the split number.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _SplitTextBox.Focus();
                return;
            }
            ok = true;
            this.Close();
        }

        private void _CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
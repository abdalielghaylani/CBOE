using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.NCDS_DataLoader.Forms
{
    public partial class BatchMarkForm : Form
    {
        int maxValue;
        bool ok = false;
        public int MaxValue
        {
            set { this.maxValue = value; }
        }
        public bool OK
        {
            get { return this.ok; }
        }

        public int[] GetNumber
        {
            get 
            { 
                int[] number = new int[2];
                number[0] = Convert.ToInt32(_FromTextBox.Text.Trim());
                number[1] = Convert.ToInt32(_ToTextBox.Text.Trim());
                return number;
            }
        }

        public BatchMarkForm()
        {
            InitializeComponent();
        }

        private void _OKButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_FromTextBox.Text.Trim()))
            {
                return;
            }

            int fromNo;
            if (int.TryParse(_FromTextBox.Text.Trim(), out fromNo))
            {
                if (fromNo <= 0)
                {
                    MessageBox.Show("The begin number must above 0.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _FromTextBox.Focus();
                    _FromTextBox.SelectAll();
                    return;
                }
                if (fromNo > maxValue)
                {
                    MessageBox.Show(string.Format("The begin number can not greater than {0}.", maxValue), "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _FromTextBox.Focus();
                    _FromTextBox.SelectAll();
                    return;
                }
            }
            else
            {
                MessageBox.Show("The begin number must be a number", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _FromTextBox.Focus();
                _FromTextBox.SelectAll();
                return;
            }

            if (string.IsNullOrEmpty(_ToTextBox.Text.Trim()))
            {
                return;
            }

            int toNo;
            if (int.TryParse(_ToTextBox.Text.Trim(), out toNo))
            {
                if (toNo <= 0)
                {
                    MessageBox.Show("The end number must above 0.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _ToTextBox.Focus();
                    _ToTextBox.SelectAll();
                    return;
                }
                if (toNo > maxValue)
                {
                    MessageBox.Show(string.Format("The end number can not greater than {0}.", maxValue), "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _ToTextBox.Focus();
                    _ToTextBox.SelectAll();
                    return;
                }

            }
            else
            {
                MessageBox.Show("The end number must be a number", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _ToTextBox.Focus();
                _ToTextBox.SelectAll();
                return;
            }


            if (string.IsNullOrEmpty(_FromTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(_ToTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input the begin number and the end number.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _FromTextBox.Focus();
                return;
            }
            if(Convert.ToInt32(_FromTextBox.Text.Trim()) > 
                Convert.ToInt32(_ToTextBox.Text.Trim()))
            {
                MessageBox.Show("The begin number can not bigger than the end number.");
                _FromTextBox.Focus();
                return;
            }
            ok = true;
            this.Close();
        }

        private void _CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _FromTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void _ToTextBox_Leave(object sender, EventArgs e)
        {
        }
    }
}
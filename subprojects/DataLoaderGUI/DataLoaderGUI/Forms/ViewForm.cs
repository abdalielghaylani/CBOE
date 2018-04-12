using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.DataLoaderGUI.Controls;
using System.Diagnostics;
using CambridgeSoft.DataLoaderGUI.Properties;

namespace CambridgeSoft.DataLoaderGUI.Forms
{
    public partial class ViewForm : Form
    {
        private DisplayInputData _DisplayInputData;        

        public ViewForm(DisplayInputData displayInputData)
        {
            InitializeComponent();
            _DisplayInputData = displayInputData;
            _DisplayInputData.Cancel += new EventHandler(DisplayInputData_Cancel);
            _DisplayInputData.Accept += new EventHandler(DisplayInputData_Accept);
            _DisplayInputData.Dock = DockStyle.Top;
            this.Controls.Add(_DisplayInputData);
        }

        /************************************************************************
        ** DisplayInputData
        **************************************************************************/
        private void DisplayInputData_Accept(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;

                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "DisplayInputData_Accept_Exception");
                Trace.Flush();
                MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            return;
        } // DisplayInputData_Accept()

        private void DisplayInputData_Cancel(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        } // DisplayInputData_Cancel()
    }
}

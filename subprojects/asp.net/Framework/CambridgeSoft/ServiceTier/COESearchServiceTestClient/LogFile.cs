using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace COESearchServiceTest
{
    public partial class LogFile : Form
    {
        private string _logFilePath;
        private DataTable _dt;
        public LogFile(DataTable dt)
        {
            _dt = dt;
            InitializeComponent();
        }

        public LogFile(string logFilePath)
        {
            _logFilePath = logFilePath;
            InitializeComponent();
        }

        private void LogFile_Load(object sender, EventArgs e)
        {
            if(_dt != null)
            {
                //DataGrid grid = new DataGrid();
                dataGridView1.DataSource = _dt;
                dataGridView1.Show();
            }
        }

    }
}
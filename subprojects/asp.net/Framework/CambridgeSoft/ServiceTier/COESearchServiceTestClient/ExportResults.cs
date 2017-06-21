using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COESearchServiceTest
{
    public partial class ExportResults : Form
    {
        private string _exportPath;
        public ExportResults(string exportPath)
        {   _exportPath = exportPath;
            InitializeComponent();
        }

        private void ExportResults_Load(object sender, EventArgs e)
        {
            this.ExportTextResults.Text = File.ReadAllText(_exportPath);
        } 
    }
}
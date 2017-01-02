using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChemBioVizExcelAddIn
{
    public partial class frmProcess : Form
    {
        public frmProcess()
        {
            InitializeComponent();
        }

        public frmProcess(string msgTitle)
        {
            InitializeComponent();
            this.Text = msgTitle.Trim();
        }
       
    }
}
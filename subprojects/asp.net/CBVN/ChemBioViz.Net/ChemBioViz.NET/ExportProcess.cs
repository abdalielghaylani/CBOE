using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChemBioViz.NET
{
    public partial class ExportProcess : Form
    {
        
        public ExportProcess()
        {
            InitializeComponent();
        }
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { progresBar.Value = value; if (value == 100) { System.Threading.Thread.Sleep(50); this.Close(); } }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set { msgLbl.Text = value; }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (this.Owner != null && this.Owner is ChemBioVizForm)
                (this.Owner as ChemBioVizForm).CancelBackgroundWorker = true;
            this.Close();
        }
    }
}

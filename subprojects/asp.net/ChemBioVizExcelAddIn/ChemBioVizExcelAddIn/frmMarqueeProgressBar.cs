using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChemBioVizExcelAddIn
{
    public partial class frmMarqueeProgressBar : Form
    {
        private int maxValue=0;
        private string progressStyle = string.Empty;
        private int incrValue = 0;
        private bool isDisplayOnly = false;

        private string ProgressStyle { get { return progressStyle; } }
        private bool IsDisplayOnly { get { return isDisplayOnly; } set { isDisplayOnly = value; } }       

        public frmMarqueeProgressBar()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmProcessProgress_Load);
        }
        
        public frmMarqueeProgressBar(string msgTitle,  string progressStyle,bool isDispOnly)
        {
            InitializeComponent();
            this.label1.Text = msgTitle;
            IsDisplayOnly = isDispOnly;
            this.progressStyle = progressStyle;
        }       

        public frmMarqueeProgressBar(int maxValue, string progressStyle)
        {
            InitializeComponent();
            this.maxValue = maxValue;
            this.progressStyle = progressStyle;
        }

        private void frmProcessProgress_Load(object sender, EventArgs e)
        {
            label1.Visible = true;
            if (IsDisplayOnly)
            {
                OnLoad();
            }
            else if (!IsDisplayOnly)
            {
                OnLoad();
                backgroundWorker.RunWorkerAsync();
            } 
        }

        private void OnLoad()
        { 
            if (progressStyle.Equals(StringEnum.GetStringValue(Global.ProgressBarStyle.CONTINUOUS),StringComparison.OrdinalIgnoreCase))
            {
                 progressBar1.Style = ProgressBarStyle.Blocks;
                 progressBar1.Minimum = 0;
                 progressBar1.Maximum = maxValue;
            }
            else if (progressStyle.Equals(StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE), StringComparison.OrdinalIgnoreCase))
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 100;
                
                progressBar1.Minimum = 0;
                progressBar1.Maximum = maxValue;
            }           
          
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= maxValue; i++)
            {
                incrValue = i;
                backgroundWorker.ReportProgress(i);
                System.Threading.Thread.Sleep(100 * Global.NoOfRecToProcessOnPBar);                
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int processFrom = CBVExcel.rowCount + 1;
            int processColNo= CBVExcel.colCount+ 1;
            int processTo = CBVExcel.rowCount + Global.NoOfRecToProcessOnPBar;

            if (processTo > maxValue)
                processTo = maxValue;

            progressBar1.Value = e.ProgressPercentage;

            //label1.Text = "Processing " + processFrom.ToString() + " to " + processTo + " of " + maxValue.ToString();
            label1.Text = "Processing column " + processColNo + " results......";
            
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {   
            
        }
       
       
    }
}
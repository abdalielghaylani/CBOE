using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Data;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// Display the job information collected so far
    /// </summary>
    public partial class JobSummary : UIBase
    {
        private Job _Job;
        private Button _OpenButton;
        //JED: private LogMessageView _LogMessageView;
        private LogSummaryView _LogMessageView;

        /// <summary>
        /// Set / get Job
        /// </summary>
        public Job Job
        {
            protected get
            {
                return _Job;
            }
            set
            {
                _Job = value;
                _LogMessageView.LoaderJob = Job;
                return;
            }
        }

        /// <summary>
        /// Allow initial path to be set
        /// </summary>
        public string LogMessagePath
        {
            set
            {
                _LogMessageView.LogMessagePath = value;
                return;
            }
        }

        /// <summary>
        /// Construct the JobSummary
        /// </summary>
        public JobSummary()
        {
            StatusText = "Job summary";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();

            //JED: _LogMessageView = new LogMessageView();
            //JED: _LogMessageView.HeightChanged += new LogMessageView.HeightChangedEventHandler(LogMessageView_HeightChanged);
            _LogMessageView = new LogSummaryView();
            _LogMessageView.AutoSize = true;
            _LogMessageView.Dock = DockStyle.Fill;

            Controls.Add(_LogMessageView);
            // _OpenButton
            _OpenButton = UIBase.GetButton(ButtonType.Open);
            _OpenButton.Click += new EventHandler(OpenButton_Click);
            Controls.Add(_OpenButton);
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            CancelButton.Enabled = false;
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(JobSummary_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // JobSummary()

        #region event handlers

        private void LogMessageView_HeightChanged(object sender, EventArgs e)
        {
            LogMessageView logMessageView = sender as LogMessageView;
            OnLayout(new LayoutEventArgs(this, "Bounds"));
            return;
        }

        private void LogSummaryView_HeightChanged(object sender, EventArgs e)
        {
            OnLayout(new LayoutEventArgs(this, "Bounds"));
            return;
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        } // AcceptButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        private void JobSummary_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _LogMessageView.MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height - AcceptButton.Height);
                _LogMessageView.Height = _LogMessageView.MaximumSize.Height;
                _LogMessageView.Top = y;
                y += _LogMessageView.Height;
                _OpenButton.Top = y;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _LogMessageView.Left = x;
                _OpenButton.Left = x;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                //
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            }
            return;
        } // JobSummary_Layout()

        private void OpenButton_Click(object sender, EventArgs e)
        {
            string strFilename = OpenLogFileDialog();
            if (strFilename != string.Empty)
            {
                _LogMessageView.LogMessagePath = strFilename;
            }
            return;
        } // OpenButton_Click()

        #endregion

        /// <summary>
        /// OpenFileDialog for opening a log file
        /// </summary>
        /// <returns></returns>
        static public string OpenLogFileDialog()
        {
            string strRet = string.Empty;
            OpenFileDialog dlgOpenFileDialog = new OpenFileDialog();
            dlgOpenFileDialog.Filter = "log file (*.log)|*.log";
            dlgOpenFileDialog.CheckFileExists = true;
            string _ApplicationFolder = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + Application.ProductName + @"\";
            dlgOpenFileDialog.InitialDirectory = _ApplicationFolder;
            dlgOpenFileDialog.Multiselect = false;
            dlgOpenFileDialog.ReadOnlyChecked = true;
            dlgOpenFileDialog.RestoreDirectory = true;
            dlgOpenFileDialog.ShowReadOnly = false;
            dlgOpenFileDialog.Title = Application.ProductName + " - Job log file";
            if (dlgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                strRet = dlgOpenFileDialog.FileName.Trim();
            }
            return strRet;
        }
 
    }
}

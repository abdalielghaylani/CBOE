using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using CambridgeSoft.DataLoaderGUI.Properties;

namespace CambridgeSoft.DataLoaderGUI.Forms
{
    public partial class ShowProgressForm : Form
    {
        public JobParameters _job;
        private string _logFilePath = string.Empty;
        private TargetActionType tagetActionType;
        public volatile int totalRecordToProcess = 0;
        private volatile int noOfRecordsPorcessed = 0;
        private volatile int recordsimported = 0;
        private volatile int recordsTempImported = 0;
        private volatile int recordsPermImported = 0;
        private volatile int percentageCompleted = 0;
        public volatile int invalidRecords = 0;
        public volatile int dupRecords = 0;
        public volatile int uniqueRecords = 0;
        private volatile int noActionRecords = 0;
        private double recordsPorcessRate = 0;
        Stopwatch ProgressStopWatch = new Stopwatch();
        JobExecutor jobexe = new JobExecutor();

        public ShowProgressForm()
        {
            InitializeComponent();
        }

        delegate void JobProcessDelegate();
        private void ShowProgress_Load(object sender, EventArgs e)
        {
            OkButton.Enabled = false;
            CopyLogButton.Visible = false;
            tagetActionType = _job.TargetActionType;
            if (tagetActionType == TargetActionType.FindDuplicates)
            {
                ScanPanel.Visible = true;
                ImportPanel.Visible = false;
            }
            else if (tagetActionType.ToString().Contains("Import"))
            {
                ScanPanel.Visible = false;
                ImportPanel.Visible = true;
            }
            JobProcessDelegate jobProcess = new JobProcessDelegate(ExecuteJob);
            jobProcess.BeginInvoke(new AsyncCallback(ResultsReturned), jobProcess);
                      
        }

        // call back method to capture results
        private void ResultsReturned(IAsyncResult iar)
        {
            // cast the state object back to the delegate type
            JobProcessDelegate del = (JobProcessDelegate)iar.AsyncState;
            // call EndInvoke on the delegate to get the results
            del.EndInvoke(iar);

            if (iar.IsCompleted)
            {
                if (noOfRecordsPorcessed != totalRecordToProcess)
                {                    
                    if (jobexe.JobResult.TotalRecordsCount != jobexe.JobResult.NoOfRecordsProcessed)
                    {
                        //MessageBox.Show(jobexe.JobResult.NoOfRecordsProcessed.ToString());
                    }
                    else
                    {
                        noOfRecordsPorcessed = jobexe.JobResult.NoOfRecordsProcessed;
                        recordsTempImported = jobexe.JobResult.TemporalRecordsCount;
                        recordsPermImported = jobexe.JobResult.PermanentRecordsCount;
                        invalidRecords = jobexe.JobResult.InvalidRecordsCount;
                        dupRecords = jobexe.JobResult.DuplicateRecordsCount;
                        uniqueRecords = jobexe.JobResult.UniqueRecordsCount;
                        noActionRecords = jobexe.JobResult.NoActionRecordsCount;
                        recordsPorcessRate = jobexe.JobResult.ProcessRate;
                        recordsimported = recordsTempImported + recordsPermImported;
                        ShowProgress();
                    }
                }
                if (CopyLogButton.InvokeRequired)
                {
                    CopyLogButton.Invoke(new MethodInvoker(delegate { CopyLogButton.Visible = true; }));
                }
                if (OkButton.InvokeRequired)
                {
                    OkButton.Invoke(new MethodInvoker(delegate { OkButton.Enabled = true; }));                    
                }
                if (WaitLabel.InvokeRequired)
                {
                    WaitLabel.Invoke(new MethodInvoker(delegate { WaitLabel.Visible = false; }));
                }
                if (CancelButton.InvokeRequired)
                {
                    WaitLabel.Invoke(new MethodInvoker(delegate { CancelButton.Visible = false; }));                    
                }
            }
        }

        delegate void ShowProgressDelegate(string pi, int totalRecords, int recordsSoFar);
        private void ShowProgressBar(string resultMessage, int totalRecords, int recordsSoFar)
        {
            // Make sure we're on the right thread
            if (lblProgress.InvokeRequired == false)
            {
                lblProgress.Text = resultMessage;

                UniqueRecordsLabel.Text = uniqueRecords.ToString();
                DuplicateRecordsLabel.Text = dupRecords.ToString();
                InvalidRecordsScanLabel.Text = invalidRecords.ToString();

                TempRecordsLabel.Text = recordsTempImported.ToString();
                PermRecordsLabel.Text = recordsPermImported.ToString();
                NoActionRecordsLabel.Text = noActionRecords.ToString();
                InvalidRecordsImportLabel.Text = invalidRecords.ToString();

                if (totalRecords >= recordsSoFar)
                {
                    ResultProgressBar.Maximum = totalRecords;
                    ResultProgressBar.Value = recordsSoFar;
                }                
            }
            else
            {
                // Show progress asynchronously
                ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgressBar);
                BeginInvoke(showProgress, new object[] { resultMessage, totalRecords, recordsSoFar });
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        private void ExecuteJob()
        {
            JobExecutor.JobCancelled = false;
            //JobExecutor jobexe = new JobExecutor();
            jobexe.JobResult.BeforeRecordsChunkProcess += BeforeRecordsChunkProcess;
            jobexe.JobResult.AfterRecordsChunkProcess += AfterRecordsChunkProcess;
            jobexe.JobResult.RecordsValidated += RecordsValidated;
            jobexe.JobResult.RecordsDupChecked += RecordsDupChecked;
            jobexe.JobResult.RecordsUniqueChecked += RecordsUniqueChecked;
            jobexe.JobResult.RecordsTotal += TotalRecordsToProcess;
            jobexe.JobResult.RecordsImported += RecordsImported;
            jobexe.JobResult.RecordsTempImported += RecordsTempImported;
            jobexe.JobResult.RecordsPermImported += RecordsPermImported;
            jobexe.JobResult.RecordsInvalid += RecordsInValidated;
            jobexe.JobResult.RecordsNoAction += RecordsWithNoAction;
            jobexe.JobResult.RecordsProcessed += RecordsProcessed;
            jobexe.JobResult.RecordsProcessRate += RecordsProcessRate;

            timer1.Start();
            ProgressStopWatch.Start();
            jobexe.DoUnattendedJob(_job);            
            do
            {
                if (JobExecutor.JobCancelled)
                    break;
            } while (!jobexe.JobResult.JobComplete);
            if (jobexe.JobResult.JobComplete || JobExecutor.JobCancelled)
            {
                ProgressStopWatch.Stop();
                timer1.Stop();
                _logFilePath = jobexe.GetLogFile();
                if (jobexe.JobInterupped)
                {
                    MessageBox.Show("Unable to process. Please see log for more details", CambridgeSoft.DataLoaderGUI.Properties.Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                }
            }
        }

        private void CopyLogButton_Click(object sender, EventArgs e)
        {
            //read log file and copy the content to clip board.
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                //using (StreamReader sr = new StreamReader(_logFilePath))
                //{
                //    String line;
                //    StringBuilder logContent = new StringBuilder();
                //    // Read and display lines from the file until the end of
                //    // the file is reached.
                //    while ((line = sr.ReadLine()) != null)
                //    {
                //        logContent.Append(line + System.Environment.NewLine);
                //    }
                //    Clipboard.SetText(logContent.ToString());
                //}
                Clipboard.SetText(_logFilePath);
            }
            catch (Exception ex)
            {
                // Let the user know what went wrong.
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(ex.Message);
            }
            
        }

        private void ShowProgress()
        {
            StringBuilder ResultMessage = new StringBuilder();
            percentageCompleted = (totalRecordToProcess == 0) ? totalRecordToProcess : (noOfRecordsPorcessed * 100) / totalRecordToProcess;

            if (tagetActionType == TargetActionType.FindDuplicates)
            {
                ResultMessage.Append(string.Format(Resources.ScaningRecords, percentageCompleted, noOfRecordsPorcessed, totalRecordToProcess));
                ResultMessage.Append(System.Environment.NewLine);
                ResultMessage.Append(string.Format(Resources.ScanRate, Math.Round(recordsPorcessRate, 2))); 
            }
            if (tagetActionType.ToString().Contains("Import"))
            {                
                ResultMessage.Append(string.Format(Resources.LoadingRecords, percentageCompleted, noOfRecordsPorcessed, totalRecordToProcess));
                ResultMessage.Append(System.Environment.NewLine);
                ResultMessage.Append(string.Format(Resources.LoadRate, Math.Round(recordsPorcessRate, 2)));
            }
            ShowProgressBar(ResultMessage.ToString(), totalRecordToProcess, noOfRecordsPorcessed);
        }
        public void BeforeRecordsChunkProcess(object sender, EventArgs e)
        {
            IndexRange ir = (IndexRange)sender;
            // We are not displaying any chunk processing info here
        }

        public void AfterRecordsChunkProcess(object sender, EventArgs e)
        {
            IndexRange ir = (IndexRange)sender;
        }

        private void RecordsValidated(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            ShowProgress();
        }

        private void RecordsProcessed(object sender, EventArgs e)
        {
            noOfRecordsPorcessed = (int)sender;
            ShowProgress();            
        }  

        private void RecordsProcessRate(object sender, EventArgs e)
        {
            recordsPorcessRate = (double)sender;
            ShowProgress();
        }        

        private void RecordsInValidated(object sender, EventArgs e)
        {
            invalidRecords = (int)sender;
            ShowProgress();
        }

        private void TotalRecordsToProcess(object sender, EventArgs e)
        {
            totalRecordToProcess = (int)sender;
            ShowProgress();
        }

        private void RecordsWithNoAction(object sender, EventArgs e)
        {
            noActionRecords = (int)sender; ;
            ShowProgress();
        }
        private void RecordsDupChecked(object sender, EventArgs e)
        {
            dupRecords = (int)sender; 
            ShowProgress();
        }

        private void RecordsUniqueChecked(object sender, EventArgs e)
        {
            uniqueRecords = (int)sender;
            ShowProgress();
        }

        private void RecordsImported(object sender, EventArgs e)
        {
            recordsimported = (int)sender;
            ShowProgress();
        }

        private void RecordsTempImported(object sender, EventArgs e)
        {
            recordsTempImported = (int)sender;
            ShowProgress();
        }

        private void RecordsPermImported(object sender, EventArgs e)
        {
            recordsPermImported = (int)sender;
            ShowProgress();
        }

        private void RecordsExtracted(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            ShowProgress();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan objTimeSpan = TimeSpan.FromMilliseconds(ProgressStopWatch.ElapsedMilliseconds);
            StopWatchLabel.Text = String.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", objTimeSpan.Hours, objTimeSpan.Minutes, objTimeSpan.Seconds);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            JobExecutor.JobCancelled = true;
            CancelButton.Enabled = false;
            WaitLabel.Visible = true;
        }
       
    }
}

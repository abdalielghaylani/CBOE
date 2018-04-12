using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;

namespace DataPreviewer
{
    /// <summary>
    /// This form shows the following progression:
    /// (1) Mapping a list of objects (of a single type) into a DataTable based on that
    ///     type's available properties
    /// (2) 
    /// </summary>
    public partial class DataPreviewer : Form
    {
        private bool _bindObjects;
        private string _myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string _file = string.Empty;
        private int _maxIndex = 50000;

        /// <summary>
        /// Represents a combined path for the selected folder and the currently-selected
        /// file in that folder.
        /// </summary>
        private string SelectedFile
        {
            get
            {
                return System.IO.Path.Combine(_myDocuments, _file);
            }
        }

        CurrencyManager cm;
        private BindingList<ISourceRecord> sourceRecords;
        private DataTable sourceTable;

        /// <summary>
        /// Silly demo form.
        /// </summary>
        public DataPreviewer()
        {
            InitializeComponent();
            InitializeFileSelector();

            this.dgvParsedData.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvParsedData_CellFormatting);

            InitializeBackgroundWorker();
        }

        private void InitializeFileSelector()
        {
            this.tsCbo_Files.Items.Clear();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_myDocuments);
            System.IO.FileInfo[] sdfiles = di.GetFiles("*.sdf");
            this.tsCbo_Files.Items.AddRange(sdfiles);

            this.tsCbo_Files.SelectedIndex = -1;
            this.tsCbo_Files.Text = string.Empty;

            this.tsCbo_Files.SelectedIndexChanged -= this.tsCbo_Files_SelectedIndexChanged;
            this.tsCbo_Files.SelectedIndexChanged += this.tsCbo_Files_SelectedIndexChanged;
        }

        /// <summary>
        /// This is a standard, same-thread event-listener for 'record parsed'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Contains the record that was parsed</param>
        public void reader_RecordParsed(object sender, RecordParsedEventArgs e)
        {
            ISourceRecord c = e.SourceRecord as ISourceRecord;
        }

        /// <summary>
        /// Hooks up event-handlers for the BackgroundWorker instance to handle non-UI thread processing.
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        /// <summary>
        /// Creates form bindings
        /// </summary>
        private void BindParsedData()
        {
            if (sourceRecords.Count == 0)
            {
                ClearFormBindings();
                return;
            }

            ISourceRecord src = null;

            if (_bindObjects)
            {
                this.dgvParsedData.DataSource = sourceRecords;
                this.statusLabel.Text =
                    "File loaded! " +
                    sourceRecords.Count.ToString() +
                    " records parsed into objects and mapped to a DataTable";

                cm = (CurrencyManager)this.BindingContext[sourceRecords];

                BindingSource bs = new BindingSource();
                src = (ISourceRecord)cm.Current;
                this.rtbParsedStructure.DataBindings.Add("text", src.FieldSet["sd_molecule"], "");

                cm.CurrentChanged += new EventHandler(cm_CurrentChanged);
                cm_CurrentChanged(cm, new EventArgs());
            }
            else
            {
                this.dgvParsedData.DataSource = sourceTable;
                this.statusLabel.Text =
                    "File loaded! " +
                    (sourceTable.Rows.Count).ToString() +
                    " records parsed into objects and mapped to a DataTable";
                this.rtbParsedStructure.DataBindings.Add("text", sourceTable, "sd_molecule");
                cm = (CurrencyManager)this.BindingContext[sourceTable];
                cm.CurrentChanged += new EventHandler(cm_CurrentChanged);
                cm_CurrentChanged(cm, new EventArgs());
            }

            //show the Mol with word-wrap
            this.dgvParsedData.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        /// <summary>
        /// Re-initializes form bindings and data-sources
        /// </summary>
        private void ClearFormBindings()
        {
            this.dgvParsedData.DataBindings.Clear();
            this.rtbParsedStructure.DataBindings.Clear();
            this.axChemDrawCtl2.Objects.Clear();

            this.rtbParsedStructure.Text = null;
            this.cm = null;
        }

        private int[] GetRecordIndices()
        {
            string records = this.tsText_Indices.Text;
            string[] recordIndices = records.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int[] indices = new int[recordIndices.Length];
            for (int i = 0; i < recordIndices.Length; i++)
                indices[i] = Convert.ToInt32(recordIndices[i]);
            return indices;
        }

        private void LoadRecordsBySourceIndices(int[] indices)
        {
            Array.Sort(indices);
            DateTime begin = DateTime.Now;
            List<ISourceRecord> records = new List<ISourceRecord>();
            SDFileReader reader = new SDFileReader(this.SelectedFile);

            ISourceRecord extractedRecord;
            int recordsToParse = indices.Length;

            foreach (int i in indices)
            {
                reader.Seek(i);

                extractedRecord = reader.GetNext();
                if (extractedRecord != null)
                {
                    records.Add(extractedRecord);
                    int pctg = (int)(((double)records.Count / (recordsToParse)) * 100);
                }
                else
                    break;
            }

            //close the stream
            reader.Close();

            TimeSpan ts = DateTime.Now - begin;
            double elapsed = ts.TotalMilliseconds;

            this.statusLabel.Text =
                string.Format("Parsed {0} records in {1} seconds", records.Count.ToString(), (elapsed / 1000).ToString());

            sourceRecords = new BindingList<ISourceRecord>(records);
            DataTable results = ConvertRecordsToTable(records, null, null);

            ClearFormBindings();
            BindParsedData();
        }

        private void ExportRecordsBySourceIndices(int[] indices)
        {
            Array.Sort(indices);
            DateTime begin = DateTime.Now;
            List<ISourceRecord> records = new List<ISourceRecord>();
            SDFileReader reader = new SDFileReader(this.SelectedFile);

            ISourceRecord extractedRecord;
            int recordsToParse = indices.Length;

            foreach (int i in indices)
            {
                reader.Seek(i);

                extractedRecord = reader.GetNext();
                if (extractedRecord != null)
                {
                    records.Add(extractedRecord);
                    int pctg = (int)(((double)records.Count / (recordsToParse)) * 100);
                }
                else
                    break;
            }

            //close the stream
            reader.Close();            

            StringBuilder sdOutputBuilder = new StringBuilder();
            foreach (ISourceRecord rec in records)
            {
                SDSourceRecord sdRec = rec as SDSourceRecord;
                if (sdRec != null)  // Coverity fix : CBOE-1946
                    sdOutputBuilder.AppendLine(sdRec.ComposeRecord());
            }
            string sdOutput = sdOutputBuilder.ToString();
            this.rtbParsedStructure.Text = sdOutput;

            TimeSpan ts = DateTime.Now - begin;
            double elapsed = ts.TotalMilliseconds;

            this.statusLabel.Text =
                string.Format("Parsed and 'exported' {0} records in {1} seconds", records.Count.ToString(), (elapsed / 1000).ToString());
        }

        private void ExportRecordsByRecordList(BindingList<ISourceRecord> records)
        {
            StringBuilder sdOutputBuilder = new StringBuilder();
            foreach (ISourceRecord rec in records)
            {
                SDSourceRecord sdRec = rec as SDSourceRecord;
                if (sdRec != null)  // Coverity fix : CBOE-1946
                    sdOutputBuilder.AppendLine(sdRec.ComposeRecord());
            }
            string sdOutput = sdOutputBuilder.ToString();

            System.IO.FileInfo fi = new System.IO.FileInfo(this.SelectedFile + ".out");
            System.IO.StreamWriter sw = fi.CreateText();
            sw.Write(sdOutput);
            sw.Flush();
            sw.Close();

            MessageBox.Show(fi.FullName, "Record export complete", MessageBoxButtons.OK);
        }

        #region >Form event-handlers<

        /// <summary>
        /// Where the potentially time-consuming work is done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Ensure the worker thread can be monitored and controlled from here
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            // Assign the Result property of the DoWorkEventArgs object.
            // This is will be available to the RunWorkerCompleted eventhandler.
            e.Result = this.MapFromSDFile(worker, e);
        }

        /// <summary>
        /// Primary working method
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private DataTable MapFromSDFile(BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<ISourceRecord> records = ReadRecordsBackground(worker, e);

            //Intervening step to test the AutoMapper...whoa
            //MappingService mapSvc = new MappingService();
            //IList<object> sinks = mapSvc.GetSinkObjectsFromSourceObjects(records, typeof(SDFileRecord));

            DataTable results = ConvertRecordsToTable(records, worker, e);
            return results;
        }

        private List<ISourceRecord> ReadRecordsBackground(BackgroundWorker worker, DoWorkEventArgs e)
        {
            DateTime then = DateTime.Now;
            List<ISourceRecord> records = new List<ISourceRecord>();
            SDFileReader reader = new SDFileReader(this.SelectedFile);
            ISourceRecord extractedRecord;

            int percentage = 0;
            int newpercentage = 0;
            int totalCount = reader.RecordCount;

            reader.Rewind();
            reader.Seek(0);
            reader.RecordParsed += new EventHandler<RecordParsedEventArgs>(reader_RecordParsed);

            do
            {
                extractedRecord = reader.GetNext();

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    reader.Close();
                    break;
                }

                if (extractedRecord != null)
                {
                    records.Add(extractedRecord as ISourceRecord);
                    int parsedCount = reader.ParsedRecordCount;
                    newpercentage = (int)(((double)parsedCount / (totalCount)) * 100);
                    if (newpercentage > percentage)
                    {
                        percentage = newpercentage;
                        worker.ReportProgress(percentage, extractedRecord);
                    }
                }
            } while (extractedRecord != null && records.Count <= (_maxIndex));

            TimeSpan ts = DateTime.Now - then;
            double elapsed = ts.TotalMilliseconds;
            reader.Close();
            reader.RecordParsed -= new EventHandler<RecordParsedEventArgs>(reader_RecordParsed);

            return records;
        }

        private DataTable ConvertRecordsToTable(List<ISourceRecord> records, BackgroundWorker worker, DoWorkEventArgs e)
        {
            sourceRecords = new BindingList<ISourceRecord>(records);
            sourceTable = Utils.GenerateTable(records, SourceFieldTypes.TypeDefinitions);
            return sourceTable;
        }

        /// <summary>
        /// Handles the outcome/results of the background operation. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle exceptions.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where operation was canceled.
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                this.statusLabel.Text = "Canceled";
                ClearFormBindings();
            }
            else
            {
                // Finally, handle the case where the operation succeeded.
                // In this case, this is superfluous since we're using form-scope variables
                // for the parsed record 'lists'
                DataTable dt = (DataTable)e.Result;

                BindParsedData();
            }

            // Always reset the porogress bar
            this.statusProgress.Value = 0;
        }

        /// <summary>
        /// This event handler updates the progress bar. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //SDFileRecord record = e.UserState as SDFileRecord;
            int pct = e.ProgressPercentage;
            if (pct > 0)
                this.statusProgress.PerformStep();
        }

        /// <summary>
        /// Force cell presentation to use 'ToString' for all cell data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvParsedData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
                e.Value = e.Value.ToString();
        }

        /// <summary>
        /// This method displays the chemical structure corresponding to the current
        /// binding context's 'Mol' property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_CurrentChanged(object sender, EventArgs e)
        {
            CurrencyManager curm = sender as CurrencyManager;
            if (this._bindObjects)
            {
                ISourceRecord rec = curm.Current as ISourceRecord;
                if (rec != null)    // Coverity Fix: CBOE-1946
                    this.axChemDrawCtl2.set_Data("chemical/x-mdl-molfile", rec.FieldSet["sd_molecule"]);
            }
            else
            {
                DataRowView drv = curm.Current as DataRowView;
                if (drv != null)        // Coverity Fix: CBOE-1946
                    this.axChemDrawCtl2.set_Data("chemical/x-mdl-molfile", drv["sd_molecule"]);
            }
        }

        // Select the source file folder
        private void tsBtn_SetFolder_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog f = new FolderBrowserDialog();
            if (f.ShowDialog() == DialogResult.OK)
                _myDocuments = f.SelectedPath;
            this.InitializeFileSelector();
        }

        // Repopulate the file-chooser dropdown
        private void tsCbo_Files_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox cbo = (ToolStripComboBox)sender;
            System.IO.FileInfo fi = (System.IO.FileInfo)cbo.Items[cbo.SelectedIndex];
            _file = fi.Name;
        }

        // Paves the way for backend-thread file processing
        private void tsBtn_Start_Click(object sender, EventArgs e)
        {
            ClearFormBindings();

            this.statusLabel.Text = "Loading SDFile...";
            this.statusProgress.Style = ProgressBarStyle.Blocks;
            this.statusProgress.Maximum = (100);
            this.statusProgress.Step = 1;
            this.backgroundWorker1.RunWorkerAsync();
        }

        // Interrupts backend-thread file processing
        private void tsBtn_Cancel_Click(object sender, EventArgs e)
        {
            // Cancel the asynchronous operation.
            if (this.backgroundWorker1.WorkerSupportsCancellation)
                this.backgroundWorker1.CancelAsync();
        }

        // Export the parsed data to a file
        private void tsBtn_Export_Click(object sender, EventArgs e)
        {
            this.ExportRecordsByRecordList(this.sourceRecords);
        }

        private void showRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bindObjects = false;
            ClearFormBindings();
            if (this.sourceTable != null) BindParsedData();
        }

        private void showObjectDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bindObjects = true;
            ClearFormBindings();
            if (this.sourceRecords != null) BindParsedData();
        }

        private void recreateSDRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bindObjects)
            {
                if (cm != null && cm.Current != null)   // Coverity Fix: CBOE-1946
                {
                    SDSourceRecord rec = cm.Current as SDSourceRecord;
                    //this.rtbParsedStructure.Text = rec.ComposeRecord();
                    if (rec != null)
                        this.rtbParsedStructure.Text = rec.RawRecord;
                }
            }
            else
            {
                MessageBox.Show("Switch to 'object' view to do this");
            }
        }

        //COUNT all records
        private void tsBtn_CountAll_Click(object sender, EventArgs e)
        {
            SDFileReader reader = new SDFileReader(this.SelectedFile);

            DateTime begin = DateTime.Now;
            int count = reader.CountAll();

            //close the stream
            reader.Close();

            TimeSpan ts = DateTime.Now - begin;
            double elapsed = ts.TotalMilliseconds;
            this.statusLabel.Text = string.Format("File has {0} records (counted in {1} seconds)", count.ToString(), (elapsed / 1000).ToString());
        }

        private void tsBtn_ParseSpecific_Click(object sender, EventArgs e)
        {
            int[] indices = GetRecordIndices();
            LoadRecordsBySourceIndices(indices);
        }

        private void tsBtn_ExportSpecific_Click(object sender, EventArgs e)
        {
            int[] indices = GetRecordIndices();
            ExportRecordsBySourceIndices(indices);
        }

        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Data;
using CambridgeSoft.COE.DataLoader.Windows.Common;
using CambridgeSoft.COE.DataLoader.Windows.Forms;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// This control fetches and displays a job's summary information and its
    /// transactional messages.
    /// </summary>
    public partial class LogSummaryView : UserControl
    {
        #region >Properties and Private Data<

        private const string UNKNOWN = "UNKNOWN";

        private Job _loaderJob;
        private string _logMessagePath = string.Empty;
        private string _logMessageFileName = string.Empty;
        private DataTable _summaryTable;
        private DataTable _messagesTable;
        private DataView _messagesView;
        private BindingList<KVP> _summaryInfo;
        private Dictionary<string, string> _summaryDictionary;

        /// <summary>
        /// The loader job
        /// </summary>
        public Job LoaderJob
        {
            protected get { return _loaderJob; }
            set { _loaderJob = value; }
        }

        /// <summary>
        /// The path at which the loader job's summary log is found
        /// </summary>
        public string LogMessagePath
        {
            get{ return _logMessagePath; }
            set
            {
                _logMessagePath = value;
                LoadCompletedJobLogFile();
            }
        }

        /// <summary>
        /// Provides a means by which a natural Dictionary<string, object> can be converted
        /// into a BindingList: each keyed value in the dictionary is used to instantiate a new
        /// KVP object.
        /// </summary>
        private class KVP
        {
            string _key;
            public string Statistic{ get{return _key ;} set {_key = value;} }

            object _value;
            public object Value { get { return _value; } set { _value = value; } }

            public KVP(string key, object value)
            {
                _key = key;
                _value = value;
            }
        }

        #endregion
        
        /// <summary>
        /// Constructor.
        /// Event-handlers added dynamically here.
        /// </summary>
        public LogSummaryView()
        {
            InitializeComponent();
            this.chkError.CheckedChanged += chkError_CheckedChanged;
            this.dgvMessages.ShowCellErrors = true;
            this.dgvMessages.CellDoubleClick += dgvMessages_CellDoubleClick;
            //JED: these event-handlers are retired due to the combined use of dgvMessages_CellDoubleClick
            //this.dgvMessages.CellContentDoubleClick += dgvMessages_CellContentDoubleClick;
            //this.dgvMessages.RowHeaderMouseDoubleClick += dgvMessages_RowHeaderMouseDoubleClick;
        }

        /// <summary>
        /// Reads the log file, parses accordingly and binds data to control
        /// </summary>
        private void LoadCompletedJobLogFile()
        {
            //reset the GUI
            chkError.Checked = false;

            //read the log file into a complete dataset - in two steps
            FileStream oFileStream;
            DataTable summaryData;
            DataTable messageData;
            int[] summarySeverity = new int[3];
            int[] messageSeverity = new int[1];

            _logMessageFileName = Path.GetFileName(_logMessagePath);
            oFileStream = File.OpenRead(_logMessagePath);
            summaryData = LogMessageList.MakeDataSet(
                oFileStream
                , true
                , new bool[] { false, false, false }
                , ref summarySeverity
            ).Tables[0];
            _summaryTable = summaryData;

            messageData = LogMessageList.MakeDataSet(
                oFileStream
                , false
                , new bool[] { true, true, true }
                , ref messageSeverity
            ).Tables[0];
            oFileStream.Close();
            _messagesTable = messageData;

            foreach (DataRow dr in _messagesTable.Rows)
            {
                if (
                    dr["Outcome"].ToString() == "E"
                    || dr["Outcome"].ToString() == "C"
                    || dr["Outcome"].ToString() == "W"
                )
                {
                    dr.RowError = dr["Message"].ToString();
                    //dr.SetColumnError(_messagesTable.Columns["Outcome"], dr["Message"].ToString());
                }
            }

            ExtractSummaryDictionary();

            BindLogDataToControls();
        }

        /// <summary>
        /// Extracts the data from the summary table, using especially defensive coding due to
        /// the unusual format of the summary information.
        /// </summary>
        private void ExtractSummaryDictionary()
        {
            BindingList<KVP> sumInfo = new BindingList<KVP>();
            _summaryDictionary = new Dictionary<string, string>();

            string spec = string.Empty;

            foreach (DataRow oDataRow in _summaryTable.Rows)
            {
                string[] strMessage = oDataRow[3].ToString().Split(new char[] { '\t' }, 2);
                string key = strMessage[0];
                object value = strMessage[1];
                sumInfo.Add(new KVP(key, value));
                _summaryDictionary[key] = value.ToString();
            }
            _summaryInfo = sumInfo;

            if (_summaryDictionary.ContainsKey("Specification") && LoaderJob != null)
                LoaderJob.Load(_summaryDictionary["Specification"]);
        }

        /// <summary>
        /// Once the summary information and the log messages have been extracted from the log,
        /// assign their data to controls for viewing.
        /// </summary>
        private void BindLogDataToControls()
        {
            textBox1.Lines = GenerateJobSummaryMessages();

            _messagesView = _messagesTable.DefaultView;
            _messagesView.Sort = "Transaction ASC";

            dgvMessages.DataSource = _messagesView;

            //clear the instructional place-holder text
            this.lblInstruction.Text = string.Empty;
            Boolean _xmlColThatFailed= false;
            if (_messagesTable.HasErrors)
            {
                this.lblInstruction.Text = "For errors, double-click the icon to view the details.";
                _xmlColThatFailed = true; // Make True If Job Contains Any Exceptions.
            }

            //configure messages grid
            this.dgvMessages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (DataGridViewColumn col in this.dgvMessages.Columns)
            {
                switch (col.Name.ToLower())
                {
                    case "transaction":
                        {
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            col.FillWeight = 10;
                            col.Width = 10;
                            col.HeaderText = "Index";
                            col.ToolTipText = "The index of the original record from the source data file.";
                        }
                        break;
                    case "outcome":
                        {
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            col.FillWeight = 20;
                            col.Width = 30;
                            col.Visible = false;
                        }
                        break;
                    case "message":
                        {
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            col.FillWeight = 100;
                            col.ToolTipText =
                                "The result of the import, or the error encountered if the record could not be imported";
                        }
                        break;
                    case "source":
                        {
                            col.Visible = false;
                        }
                        break;
                    case "xml":
                        {
                            col.Visible = _xmlColThatFailed; // Make True If Job Contains Any Exceptions.
                        }
                        break;
                    default:
                        {
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            col.FillWeight = 10;
                            col.Width = 10;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Displays a popup containing either the "Xml" value or the "Message" value from the cell's row,
        /// depending on where the row is double-clicked. If double-clicked anywhere other than the row-selector,
        /// the "Xml" valus is displayed, formatted using an instance of System.Xml.XmlTextWriter.
        /// </summary>
        /// <see cref="FormatXmlString"/>
        /// <param name="sender">a DataGridView instance</param>
        /// <param name="e">DataGridViewCellEventArgs object</param>
        private void dgvMessages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (e.RowIndex != -1)
            {
                string index = dgv.Rows[e.RowIndex].Cells["Transaction"].Value.ToString();
                string column = string.Empty;
                string label = string.Empty;
                
                if (e.ColumnIndex != -1)
                {
                    //any part of the row EXCEPT the row-selector
                    column = "Xml";
                    label = "Object XML";
                }
                else
                {
                    //row-selector
                    column = "Message";
                    label = "Message";
                }

                MessageBoxRichText oMessageBoxRichText = new MessageBoxRichText(string.Format("{0} for Index {1}", label, index));
                if (!string.IsNullOrEmpty(dgv.Rows[e.RowIndex].ErrorText))
                {
                    oMessageBoxRichText.MessageText = Utilities.FormatXmlString(dgv.Rows[e.RowIndex].Cells[column].Value.ToString());
                    oMessageBoxRichText.StartPosition = FormStartPosition.CenterParent;
                    oMessageBoxRichText.ShowDialog();
                }

            }

        }

        /// <summary>
        /// Displays a popup containing the "Xml" value from the cell's row,
        /// formatted using an instance of System.Xml.XmlTextWriter.
        /// </summary>
        /// <see cref="FormatXmlString"/>
        /// <param name="sender">a DataGridView instance</param>
        /// <param name="e">DataGridViewCellEventArgs object</param>
        private void dgvMessages_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex != -1)
            {
                string index = dgv.Rows[e.RowIndex].Cells["Transaction"].Value.ToString();
                MessageBoxRichText oMessageBoxRichText = new MessageBoxRichText(string.Format("Object XML for Index {0}", index));
                if (!string.IsNullOrEmpty(dgv.Rows[e.RowIndex].ErrorText))
                {
                    oMessageBoxRichText.MessageText = Utilities.FormatXmlString(dgv.Rows[e.RowIndex].Cells["Xml"].Value.ToString());
                    oMessageBoxRichText.StartPosition = FormStartPosition.CenterParent;
                    oMessageBoxRichText.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Displays a popup containing the "Message" value from the cell's row.
        /// Occurs when the user double-clicks the row-selector area of the row.
        /// </summary>
        /// <param name="sender">a DataGridView instance</param>
        /// <param name="e">DataGridViewCellMouseEventArgs object</param>
        private void dgvMessages_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridViewRow dgrv = dgv.Rows[e.RowIndex];

            if (e.RowIndex != -1)
            {
                string index = dgv.Rows[e.RowIndex].Cells["Transaction"].Value.ToString();
                MessageBoxRichText oMessageBoxRichText = new MessageBoxRichText(string.Format("Message for Index {0}", index));
                if (!string.IsNullOrEmpty(dgv.Rows[e.RowIndex].ErrorText))
                {
                    oMessageBoxRichText.MessageText = Utilities.FormatXmlString(dgv.Rows[e.RowIndex].Cells["Message"].Value.ToString());
                    oMessageBoxRichText.StartPosition = FormStartPosition.CenterParent;
                    oMessageBoxRichText.ShowDialog();
                }
            }

        }

        /// <summary>
        /// Provides a filtering mechanism so that the user can choose to view only the errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkError_CheckedChanged(object sender, EventArgs e)
        {
            if (_messagesTable != null && _messagesTable.Rows.Count > 0)
            {
                if (chkError.Checked)
                {
                    DataView dvFltered =
                        new DataView(_messagesTable, "Outcome='E' OR Outcome='C' OR Outcome='W'", "Outcome", DataViewRowState.CurrentRows);
                    dgvMessages.DataSource = dvFltered;
                }
                else
                {
                    dgvMessages.DataSource = _messagesView;
                }
            }
        }

        /// <summary>
        /// Create an array of text messages used to display a Loader job summary overview.
        /// </summary>
        /// <returns>Messages describing the job, including its elapsed time.</returns>
        private string[] GenerateJobSummaryMessages()
        {
            string titleTemplate = "Viewing log file '{0}'";
            string timingsTemplate = "Began [{0}] and ended [{1}] with an elapsed time of {2}";
            string processedTemplate = "With {0} records processed successfully out of {1} total.";

            string began = "UNKNOWN";
            this._summaryDictionary.TryGetValue("Start", out began);
            string ended = "UNKNOWN";
            this._summaryDictionary.TryGetValue("End", out ended);
            string elapsed = "UNKNOWN";
            this._summaryDictionary.TryGetValue("Elapsed", out elapsed);
            if (!string.IsNullOrEmpty(elapsed) && elapsed != "UNKNOWN")
            {
                if (elapsed.Contains(".") || elapsed.Contains(","))
                    elapsed = elapsed.Split(".,".ToCharArray())[0];
                int iElapsed = int.Parse(elapsed);
                TimeSpan ts = new TimeSpan(0, 0, iElapsed);
                if (iElapsed <= 60)
                    elapsed = ts.TotalSeconds.ToString("N1") + " seconds";
                else if (iElapsed <= 3600)
                    elapsed = ts.TotalMinutes.ToString("N1") + " minutes";
                else
                    elapsed = ts.TotalHours.ToString("N2") + " hours";
            }

            string processed = UNKNOWN;
            string success = UNKNOWN;
            this._summaryDictionary.TryGetValue("Processed", out processed);
            string errors = UNKNOWN;
            this._summaryDictionary.TryGetValue("Errors", out errors);
            if (!string.IsNullOrEmpty(processed)
                && processed != UNKNOWN
                && !string.IsNullOrEmpty(errors)
                && errors != UNKNOWN
                )
            {
                int iErrors = Int32.Parse(errors);
                int iProcessed = Int32.Parse(processed);
                int iSuccess = (iProcessed - iErrors);
                success = iSuccess.ToString();
            }

            string[] messages = new string[] {
                string.Format(titleTemplate, _logMessageFileName)
                , string.Format(timingsTemplate, began, ended, elapsed)
                , string.Format(processedTemplate, success, processed)
            };

            return messages;
        }
    }
}

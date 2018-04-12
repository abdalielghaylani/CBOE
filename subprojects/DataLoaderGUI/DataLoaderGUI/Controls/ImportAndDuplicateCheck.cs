using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Core;
using System.Diagnostics;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.DataLoaderGUI.Common;
using CambridgeSoft.DataLoaderGUI.Properties;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.DataLoaderGUI.Forms;

namespace CambridgeSoft.DataLoaderGUI.Controls
{
    public partial class ImportAndDuplicateCheck : UIBase
    {
        private string _Action;
        private JobParameters _job;
        private string _xmlPath;
        private string _userName;
        private string _password;
        private TargetActionType _targetActionType;
        private string _FullFilePath;
        private string _strFilePath;
        private bool _backToFront = false;
        private bool _skipflag = false;
        private string[] _xml;
        private bool _noStructureDupCheck = false;
        private bool _HaveStructure;
        private Dictionary<ISourceRecord, string> _records = new Dictionary<ISourceRecord, string>();
        private Dictionary<ISourceRecord, string> _uniqueRecords = new Dictionary<ISourceRecord, string>();
        private Dictionary<ISourceRecord, string> _structureDupRecords = new Dictionary<ISourceRecord, string>();
        int uniqueCount = 0;
        int duplicateCount = 0;
        int invalidCount = 0;
        int totalCount = 0;
        private string _selectedFilePath = string.Empty;

        ShowProgressForm showProgressForm = null;

        public string Action
        {
            get { return _Action; }
        }
        public JobParameters JOB
        {
            set { this._job = value; }
        }
        public string UserName
        {
            set { this._userName = value; }
        }
        public string Password
        {
            set { this._password = value; }
        }
        public string FullFilePath
        {
            set { this._FullFilePath = value; }
        }
        public string StrFilePath
        {
            set { this._strFilePath = value; }
        }
        public string XmlPath
        {
            set { this._xmlPath = value; }
        }

        public bool OptionPanelEnabled
        {
            set { this.ImportExportGroupBox.Enabled = value; }
        }

        public bool BackToFront
        {
            get { return _backToFront; }
        }

        public bool Skipflag
        {
            set { _skipflag = value; }
        }

        public string[] _Xml
        {
            set 
            { 
                _xml = value;
                ScanButton.Enabled = true;
            }
        }

        public bool NoStructureDupCheck
        {
            set { _noStructureDupCheck = value; }
        }

        public bool HaveStructure
        {
            set
            {
                this._HaveStructure = value;
            }
            get
            {
                return _HaveStructure;
            }
        }

        public Dictionary<ISourceRecord, string> UNIQUERECORDS
        {
            set { this._uniqueRecords = value; }
            get { return _uniqueRecords; }
        }
        public Dictionary<ISourceRecord, string> STRUCTUREDUPRECORDS
        {
            set { this._structureDupRecords = value; }
            get { return _structureDupRecords; }
        }
        public Dictionary<ISourceRecord, string> RECORDS
        {
            set { this._records = value; }
            get { return _records; }
        }

        public void AuthorizeUser()
        {
            //these two permisions dictate access to the temporary Registry
            bool hasRegTempPermission = (
                Csla.ApplicationContext.User.IsInRole("ADD_COMPOUND_TEMP")
                || Csla.ApplicationContext.User.IsInRole("REGISTER_TEMP")
            );

            ImportTempRadioButton.Enabled = hasRegTempPermission;
            ImportTempRadioButton.Checked = hasRegTempPermission;
            //these two permisions dictate access to the permanent Registry
            bool hasRegPermPermission = (
                Csla.ApplicationContext.User.IsInRole("ADD_COMPONENT")
                || Csla.ApplicationContext.User.IsInRole("EDIT_COMPOUND_REG")
                || Csla.ApplicationContext.User.IsInRole("REGISTER_DIRECT")
            );

            ImportRegDupNoneRadioButton.Enabled = hasRegPermPermission;
            ImportRegDupAsTempRadioButton.Enabled = hasRegPermPermission;
            ImportRegDupAsCreateNewRadioButton.Enabled = hasRegPermPermission;
            ImportRegDupAsCreateNewBatchRadioButton.Enabled = hasRegPermPermission;

            if (hasRegTempPermission == false && hasRegPermPermission == false)
            {
                ImportButton.Enabled = false;
                //AcceptButton.Text = "Exit";
                //ResultTextBox.Visible = true;
                //ResultTextBox.Text = "Insufficient privileges for requested operation.";
            }
            else if (hasRegTempPermission == false && hasRegPermPermission == true)
            {
                ImportRegDupAsTempRadioButton.Checked = true;
            }
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            if (AcceptButton.Text == "Exit")
            {
                //Delete invalid and duplicate and unique files
                _Action = string.Empty;
                DeleteTempFiles();
                OnAccept();
            }
        }

        private void DeleteTempFiles()
        {
            string uniqueFileName = string.Empty;
            string duplicateFileName = string.Empty;
            string invalidFilePath = string.Empty;
            _job.DataSourceInformation.FullFilePath = _FullFilePath;
            uniqueFileName = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
            duplicateFileName = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);
            invalidFilePath = JobUtility.GetPurposedFilePath("invalid", _job.DataSourceInformation.DerivedFileInfo);
            try
            {
                if (File.Exists(uniqueFileName))
                    File.Delete(uniqueFileName);
                if (File.Exists(duplicateFileName))
                    File.Delete(duplicateFileName);
                if (File.Exists(invalidFilePath))
                    File.Delete(invalidFilePath);
                if (File.Exists(_xmlPath))
                    File.Delete(_xmlPath);
            }
            catch { }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        public ImportAndDuplicateCheck()
        {
            InitializeComponent();
            Controls.Add(AcceptButton);
            Controls.Add(CancelButton);
            AcceptButton.Top = SaveMappingFileButton.Top;
            CancelButton.Top = AcceptButton.Top;
            int MaxWidth = ImportExportGroupBox.Left + ImportExportGroupBox.Width;            
            AcceptButton.Left = MaxWidth - CancelButton.Width;
            CancelButton.Left = AcceptButton.Left - (CancelButton.Width + 10);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            AllRadioButton.Checked = true;
            //ImportTempRadioButton.Checked = true;
            SaveMappingFileButton.Height = AcceptButton.Height;
            BeginImportButton.Height = AcceptButton.Height;
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {           
            string exportedFilePath = string.Empty;
            string structuredupexportedFilePath = string.Empty;
            string invalidFilePath = string.Empty;

            List<string> list_Regnum = new List<string>();

            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();
                XmlDocument doc = new XmlDocument();
                _records = new Dictionary<ISourceRecord, string>();
                _uniqueRecords = new Dictionary<ISourceRecord, string>();
                _structureDupRecords = new Dictionary<ISourceRecord, string>();

                //doc.LoadXml(_xml[1]);
                _job.DataSourceInformation.FullFilePath = _FullFilePath;
                exportedFilePath = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
                structuredupexportedFilePath = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);
                invalidFilePath = JobUtility.GetPurposedFilePath("invalid", _job.DataSourceInformation.DerivedFileInfo);

                try
                {                    
                    if (File.Exists(exportedFilePath))               
                        File.Delete(exportedFilePath);
                    if (File.Exists(structuredupexportedFilePath))
                        File.Delete(structuredupexportedFilePath);
                    if (File.Exists(invalidFilePath))
                        File.Delete(invalidFilePath);
                }
                catch 
                {
                    using (StreamWriter sw = new StreamWriter(exportedFilePath, false))
                    {
                        sw.Close();
                        sw.Dispose();
                        File.Delete(exportedFilePath);
                    }
                    using (StreamWriter sw = new StreamWriter(structuredupexportedFilePath, false))
                    {
                        sw.Close();
                        sw.Dispose();
                        File.Delete(structuredupexportedFilePath);
                    }
                    using (StreamWriter sw = new StreamWriter(invalidFilePath, false))
                    {
                        sw.Close();
                        sw.Dispose();
                        File.Delete(invalidFilePath);
                    }
                }

                //duplicateCheck();
                _targetActionType = TargetActionType.FindDuplicates;
                ExecuteCOEDataLoader();
                
                //JobArgumentInfo jobArgs = GetJobArgumentInfo();
                //_job = JobArgumentInfo.ProcessArguments(jobArgs);
                //if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalUrl"]))
                //{
                //    _job.UserName = jobArgs.UserName;
                //    _job.Password = jobArgs.Password;
                //}
                //doc.LoadXml(_xml[0]);
                //_job.Mappings = Mappings.GetMappings(doc.OuterXml);

                //string message = ReadInputFile.DoReadInputFile(_job);

                this.Refresh();

                // unique file
                if (uniqueCount > 0)
                    UniqueRadioButton.Enabled = true;
                else
                    UniqueRadioButton.Enabled = false;

                // structuredup file
                if (duplicateCount > 0)
                    DuplicateRadioButton.Enabled = true;
                else
                    DuplicateRadioButton.Enabled = false;

                //invalid
                if (invalidCount > 0)
                    InvalidRecordsRadioButton.Enabled = true;
                else
                    InvalidRecordsRadioButton.Enabled = false;

                TotalCountLabel.Text = totalCount.ToString();
                InvalidCountLabel.Text = invalidCount.ToString();
                DuplicateCountLabel.Text = duplicateCount.ToString();
                UniqueCountLabel.Text = uniqueCount.ToString();

                if (duplicateCount == 0 && uniqueCount == 0)
                {
                    CancelButton.Enabled = true;
                    ImportExportGroupBox.Enabled = false;
                }
                else
                {
                    ScanButton.Enabled = false;
                }

                if (_job.SourceRecords != null)
                {
                    _job.SourceRecords.Clear();
                }
                if (_job.DataSourceInformation.FileType == SourceFileType.MSExcel)
                {
                    _job.DataSourceInformation.FileType = SourceFileType.MSExcel;
                    _job.DataSourceInformation.FieldDelimiters = null;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;

                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "ImportAndDuplicate_Accept_Exception");
                if (_xml.Length > 0)
                {
                    Trace.WriteLine(_xml[0], "Duplicate Check Mappings ");
                }
                if (_xml.Length == 2)
                {
                    Trace.WriteLine(_xml[1], "Mappings ");
                }
                Trace.WriteLine((_records != null) ? _records.Count : 0, "Recrods ");
                Trace.WriteLine((_uniqueRecords != null) ? _uniqueRecords.Count : 0, "Unique Recrods ");
                Trace.WriteLine((_structureDupRecords != null) ? _structureDupRecords.Count : 0, "StructureDup Recrods ");
                Trace.Flush();
                MessageBox.Show(ex.Message, "Duplicate Check Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!InvalidRecordsRadioButton.Checked)
                {
                    base.Cursor = Cursors.WaitCursor;

                    string strFile = _FullFilePath;
                    if (!System.IO.File.Exists(strFile))
                    {
                        MessageBox.Show("Input file error.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!System.IO.File.Exists(_xmlPath))
                    {
                        MessageBox.Show("Mapping file error.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (ImportTempRadioButton.Checked == true)
                    {
                        _targetActionType = TargetActionType.ImportTemp;
                    }
                    else if (ImportRegDupNoneRadioButton.Checked == true)
                    {
                        _targetActionType = TargetActionType.ImportRegDupNone;
                    }
                    else if (ImportRegDupAsTempRadioButton.Checked == true)
                    {
                        _targetActionType = TargetActionType.ImportRegDupAsTemp;
                    }
                    else if (ImportRegDupAsCreateNewRadioButton.Checked == true)
                    {
                        _targetActionType = TargetActionType.ImportRegDupAsCreateNew;
                    }
                    else if (ImportRegDupAsCreateNewBatchRadioButton.Checked == true)
                    {
                        _targetActionType = TargetActionType.ImportRegDupAsNewBatch;
                    }
                    else
                    {
                        MessageBox.Show("Please select import option");
                        return;
                    }
                    this.ImportExportGroupBox.Enabled = false;
                    ExecuteCOEDataLoader();
                    ImportExportGroupBox.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Import should not be done for invalid records", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "ImportOption");
                Trace.Flush();
                MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                base.Cursor = Cursors.Default;
            }
        }

        private void ExecuteCOEDataLoader()
        {
            if ((JobUtility.USE_THREADING && JobUtility.NUMBER_OF_THREADS > 0) || !JobUtility.USE_THREADING)
            {
                JobArgumentInfo jobArgsTemp = GetJobArgumentInfo();
                JobParameters job = JobArgumentInfo.ProcessArguments(jobArgsTemp);
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalUrl"]))
                {
                    job.UserName = jobArgsTemp.UserName;
                    job.Password = jobArgsTemp.Password;
                }
                showProgressForm = new ShowProgressForm();
                showProgressForm._job = job;
                showProgressForm.StartPosition = FormStartPosition.CenterParent;
                showProgressForm.ControlBox = false;
                if (jobArgsTemp.ActionType == TargetActionType.FindDuplicates)
                {
                    showProgressForm.Text = "Duplicate Check Scan Progress";
                }
                else if (jobArgsTemp.ActionType.ToString().Contains("Import"))
                {
                    showProgressForm.Text = "Import Progress";
                }

                showProgressForm.ShowDialog();
                totalCount = showProgressForm.totalRecordToProcess;
                uniqueCount = showProgressForm.uniqueRecords;
                duplicateCount = showProgressForm.dupRecords;
                invalidCount = showProgressForm.invalidRecords;
            }
            else
            {
                MessageBox.Show("Thread count should be more than zero", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Cursor = Cursors.WaitCursor;
        }

        private JobArgumentInfo GetJobArgumentInfo()
        {
            _job.DataSourceInformation.FullFilePath = _FullFilePath;

            string _duplicateFileName = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);
            string _uniqueFileName = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
            string _selectedFileName = _selectedFilePath;
            string _invalidFileName = JobUtility.GetPurposedFilePath("invalid", _job.DataSourceInformation.DerivedFileInfo);
            JobArgumentInfo jobArgsTemp = new JobArgumentInfo();
            jobArgsTemp.ActionType = _targetActionType;
            if (jobArgsTemp.ActionType != TargetActionType.FindDuplicates)
            {
                if (AllRadioButton.Checked)
                    jobArgsTemp.DataFile = _FullFilePath;
                if (UniqueRadioButton.Checked)
                    jobArgsTemp.DataFile = _uniqueFileName;
                if (DuplicateRadioButton.Checked)
                    jobArgsTemp.DataFile = _duplicateFileName;
                if (SelectedRecordsRadioButton.Checked)
                    jobArgsTemp.DataFile = _selectedFileName;
                if (InvalidRecordsRadioButton.Checked)
                    jobArgsTemp.DataFile = _invalidFileName;
            }
            else
            {
                jobArgsTemp.DataFile = _FullFilePath;
            }
            jobArgsTemp.MappingFile = _xmlPath;

            if (_job.DataSourceInformation.FullFilePath.ToLower().EndsWith(".xls") ||
                 _job.DataSourceInformation.FullFilePath.ToLower().EndsWith(".xlsx"))
            {
                _job.DataSourceInformation.FileType = SourceFileType.MSExcel;
            }
            if (jobArgsTemp.DataFile.ToLower().EndsWith(".xls") ||
                 jobArgsTemp.DataFile.ToLower().EndsWith(".xlsx"))
            {
                jobArgsTemp.TableOrWorksheet = _job.DataSourceInformation.TableName;
                jobArgsTemp.HasHeader = _job.DataSourceInformation.HasHeaderRow;
                jobArgsTemp.FileType = SourceFileType.MSExcel;
            }
            else if (jobArgsTemp.DataFile.ToLower().EndsWith(".txt") || jobArgsTemp.DataFile.ToLower().EndsWith(".csv"))
            {
                jobArgsTemp.HasHeader = _job.DataSourceInformation.HasHeaderRow;
                if (_job.DataSourceInformation.FileType == SourceFileType.MSExcel)
                {
                    jobArgsTemp.TableOrWorksheet = _job.DataSourceInformation.TableName;
                    jobArgsTemp.Delimiters = new string[] { "\\t" };
                    jobArgsTemp.FileType = SourceFileType.CSV;
                }
                else
                {
                    jobArgsTemp.Delimiters = (_job.DataSourceInformation.FieldDelimiters[0].ToString().Equals("\t") ? new string[] { "\\t" } :
                    new string[] { _job.DataSourceInformation.FieldDelimiters[0].ToString() });
                    jobArgsTemp.FileType = SourceFileType.CSV;
                }
            }
            else
            {
                jobArgsTemp.FileType = _job.DataSourceInformation.FileType;
            }
            jobArgsTemp.UserName = _userName;
            jobArgsTemp.Password = _password;
            if (jobArgsTemp.DataFile == _FullFilePath)
            {
                jobArgsTemp.RangeBegin = _job.ActionRanges[0].RangeBegin + 1;
                jobArgsTemp.RangeEnd = _job.ActionRanges[0].RangeEnd + 1;
            }
            return jobArgsTemp;
        }

        private void BeginImportButton_Click(object sender, EventArgs e)
        {
            _backToFront = true;
            DeleteTempFiles();
            OnAccept();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            _job.DataSourceInformation.FullFilePath = _FullFilePath;
            string sourceFile = string.Empty;
            
            if (AllRadioButton.Checked == true)
            {
                MessageBox.Show("It's the same as the input file!", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (DuplicateRadioButton.Checked == true)
            {
                string duplicateFileName = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);
                sourceFile = duplicateFileName;
            }
            else if (UniqueRadioButton.Checked == true)
            {
                string uniqueFileName = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
                sourceFile = uniqueFileName;
            }
            else if (SelectedRecordsRadioButton.Checked == true)
            {
                string selectedFileName = JobUtility.GetPurposedFilePath("selected", _job.DataSourceInformation.DerivedFileInfo);
                sourceFile = selectedFileName;
            }
            else if (InvalidRecordsRadioButton.Checked == true)
            {
                string invalidFileName = JobUtility.GetPurposedFilePath("invalid", _job.DataSourceInformation.DerivedFileInfo);
                sourceFile = invalidFileName;
            }

            ExportedRecords(sourceFile);
        }

        private void ReviewButton_Click(object sender, EventArgs e)
        {
            _targetActionType = TargetActionType.Unknown;
            string message = string.Empty;
            //SourceFieldTypes.TypeDefinitions.Clear();
            //_job.DataSourceInformation.FullFilePath = _FullFilePath;
            //message = ReadInputFile.DoReadInputFile(_job);
            JobArgumentInfo jobArgs = GetJobArgumentInfo();
            _job = JobArgumentInfo.ProcessArguments(jobArgs);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalUrl"]))
            {
                _job.UserName = jobArgs.UserName;
                _job.Password = jobArgs.Password;
            }            
            message = ReadInputFile.DoReadInputFile(_job);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_xml[0]);
            _job.Mappings = Mappings.GetMappings(doc.OuterXml);
            _job.DataSourceInformation.TableName = jobArgs.TableOrWorksheet;
            
            if (string.IsNullOrEmpty(message))
            {
                IFileReader readerr = _job.FileReader;
                readerr.Rewind();
                this.Refresh();
                IndexRange originalRange = null;
                if (jobArgs.RangeEnd == int.MaxValue)
                    jobArgs.RangeEnd = readerr.CountAll();
                originalRange = new IndexRange(jobArgs.RangeBegin - 1, jobArgs.RangeEnd - 1);
                _job.SourceRecords = readerr.ExtractToRecords(originalRange);
                _records.Clear();
                foreach (ISourceRecord iSR in _job.SourceRecords)
                {
                    _records.Add(iSR, string.Empty);
                }

                this.Refresh();
                readerr.Close();

                //Display Input
                DisplayInputData _DisplayInputData = new DisplayInputData();
                _DisplayInputData.JOB = _job;
                DataSet ds = new DataSet();
                DataTable results = new DataTable();
                Utils getTable = new Utils();
                results = getTable.GenerateDisplayTable(_records, SourceFieldTypes.TypeDefinitions);
                ds.Tables.Add(results);
                _DisplayInputData.InputData = _records;
                _DisplayInputData.Data = ds;
                _DisplayInputData.AcceptButton.Text = "Use Selected";
                _DisplayInputData.AcceptButton.Image = null;
                _DisplayInputData.AcceptButton.TextAlign = ContentAlignment.MiddleCenter;
                _DisplayInputData.CancelButton.Text = "Cancel";
                _DisplayInputData.CancelButton.Image = null;
                _DisplayInputData.CancelButton.TextAlign = ContentAlignment.MiddleCenter;
                ViewForm viewForm = new ViewForm(_DisplayInputData);
                viewForm.StartPosition = FormStartPosition.CenterParent;
                viewForm.ControlBox = false;
                DialogResult dr = viewForm.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedRecordsRadioButton.Enabled = true;
                    SelectedRecordsRadioButton.Checked = true;
                    _selectedFilePath = _DisplayInputData._fName;
                }
            }
            else
            {
                MessageBox.Show(message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveMappingFileButton_Click(object sender, EventArgs e)
        {
            string sourceFile = _xmlPath;
            SaveMappingFile(sourceFile);
        }

        private void ExportedRecords(string sourceFile)
        {
            if (File.Exists(sourceFile))
            {

                string filter = string.Empty;
                switch (_job.DataSourceInformation.FileType)
                {
                    case SourceFileType.CSV:
                    case SourceFileType.MSExcel:
                        filter = "Text files (*.txt)|*.txt";
                        break;
                    case SourceFileType.SDFile:
                        filter = "SD files (*.sdf)|*.sdf";
                        break;
                    //case SourceFileType.MSExcel:
                    //    filter = "Microsoft Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                    //    break;
                    default:
                        break;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = filter;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fName = saveFileDialog.FileName;
                    try
                    {
                        if (File.Exists(fName))
                        {
                            File.Delete(fName);
                        }
                        File.Copy(sourceFile, fName);
                        MessageBox.Show("Export completed!", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Source file not found!", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveMappingFile(string sourceFile)
        {
            if (File.Exists(sourceFile))
            {
                string filter = string.Empty;
                filter = "XML files (*.xml)|*.xml";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = filter;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fName = saveFileDialog.FileName;
                    try
                    {
                        if (File.Exists(fName))
                        {
                            File.Delete(fName);
                        }
                        File.Copy(sourceFile, fName);
                        MessageBox.Show("Mapping file Saved!", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Source file not found!", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

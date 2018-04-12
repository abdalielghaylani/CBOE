using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.NCDS_DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using System.IO;
using System.Diagnostics;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.NCDS_DataLoader.Forms;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using System.Xml;
namespace CambridgeSoft.NCDS_DataLoader.Controls
{
    public partial class ModeSelection : UIBase
    {
        private string _Action;
        private string _fName;
        private JobParameters _job;
        private Dictionary<ISourceRecord, string> _records;
        private Dictionary<ISourceRecord, string> _uniqueRecords;
        private Dictionary<ISourceRecord, string> _structureDupRecords;
        private List<string> list_Regnum;
        private bool _HaveStructure;
        private bool _noStructureDupCheck = false;
        private string _duplicateFileName;
        private string _uniqueFileName;
        private string _inputFileName;
        private IndividualArgumentsCommandInfo parsedArgs;
        private string[] _xml;
        private SourceFileType filetype;
        private string[] _splitFiles;
        private string _userName;
        private string _password;

        public string[] SplitFiles
        {
            set { _splitFiles = value; }
        }
        public string DuplicateFileName
        {
            set { _duplicateFileName = value; }
            get
            {
                return _duplicateFileName;
            }
        }

        public string UserName
        {
            set { this._userName = value; }
        }
        public string Password
        {
            set { this._password = value; }
        }

        public string InputFileName
        {
            set { _inputFileName = value; }
            get
            {
                return _inputFileName;
            }
        }
        public SourceFileType FileType
        {
            set { filetype = value; }
        }
        public string[] _Xml
        {
            set { _xml = value; }
        }
        public IndividualArgumentsCommandInfo ParsedArgs
        {
            set
            {
                this.parsedArgs = value;
            }
            get
            {
                return parsedArgs;
            }
        }
        public string UniqueFileName
        {
            set { _uniqueFileName = value; }
            get
            {
                return _uniqueFileName;
            }
        }

        public string FileName
        {
            get { return _fName; }
        }

        public bool NoStructureDupCheck
        {
            set { _noStructureDupCheck = value; }
        }

        public Dictionary<ISourceRecord, string> RECORDS
        {
            set { this._records = value; }
            get { return _records; }
        }
        public List<string> List_Regnum
        {
            set { this.list_Regnum = value; }
            get { return list_Regnum; }
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

        public JobParameters JOB
        {
            set { this._job = value; }
        }

        public bool HaveStructure
        {
            set
            {
                this._HaveStructure = value;
                if (_HaveStructure || 
                    (_HaveStructure == false && _noStructureDupCheck == true))
                {
                    this._AllRadioButton.Enabled = true;
                    this._DuplicateRadioButton.Enabled = true;
                    this._UniqueRadioButton.Enabled = true;
                    this._ExportButton.Enabled = true;
                }
                else
                {
                    this._AllRadioButton.Checked = true;
                    this._AllRadioButton.Enabled = false;
                    this._DuplicateRadioButton.Enabled = false;
                    this._UniqueRadioButton.Enabled = false;
                    this._ExportButton.Enabled = false;
                }
                                    
            }
            get
            {
                return _HaveStructure;
            }
        }

        public string Action
        {
            get { return _Action; }
        }

        public string AllCount
        {
            //set { _AllCountLabel.Text = string.Format("{0} records checked.", value); }
            set
            {
                string[] text = value.Split(',');
                if (text.Length > 1)
                {
                    _AllCountLabel.Text = string.Format("{0} record(s) checked. ", text[0]) +
                        string.Format("{0} record(s) invalid. ", text[1]);
                }
                else if (text.Length == 1)
                {
                    _AllCountLabel.Text = string.Format("{0} record(s) checked.", value);
                }
            }
        }

        public string DuplicateCount
        {
            set { _DuplicateCountLabel.Text = value.ToString();}
        }

        public string UniqueCount
        {
            set { _UniqueCountLabel.Text = value.ToString();}
        }

        public ModeSelection()
        {
            InitializeComponent();
            Controls.Add(AcceptButton);
            Controls.Add(CancelButton);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            string MaxValue = string.Empty;
            try
            {
                MaxValue = System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"].ToString();
                MaxValue = (Convert.ToInt32(MaxValue) > 400) ? "400" : MaxValue;

            }
            catch
            {
                MessageBox.Show("Can't find the MaxDisplayValue configuration in app.config.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _CommentLabel.Text = 
                string.Format("Exceed the maximum number of shows {0} and only the first {0} records will be displayed.", MaxValue);
        }

        private void _MergerFile()
        {
            if (_splitFiles != null)
            {
                IndividualArgumentsCommandInfo parsedArgsTemp = new IndividualArgumentsCommandInfo();
                string uniqueCont = string.Empty;
                string duplicateCont = string.Empty;
                string exportedFilePaths = string.Empty;
                string structuredupexportedFilePaths = string.Empty;

                string exportedFilePathfull = string.Empty;
                string structuredupexportedFilePathfull = string.Empty;
                exportedFilePathfull = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
                structuredupexportedFilePathfull = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);

                if (File.Exists(exportedFilePathfull))
                    File.Delete(exportedFilePathfull);
                if (File.Exists(structuredupexportedFilePathfull))
                    File.Delete(structuredupexportedFilePathfull);

                for (int i = 2; i < _splitFiles.Length - 1; i++)
                {
                    parsedArgsTemp.DataFile = _splitFiles[i].ToString();
                    JobParameters jobTemp = new JobParameters();
                    jobTemp = NCDSBaseForm.ProcessArguments(parsedArgsTemp);

                    //XmlDocument docs = new XmlDocument();
                    exportedFilePaths = JobUtility.GetPurposedFilePath("unique", jobTemp.DataSourceInformation.DerivedFileInfo);
                    structuredupexportedFilePaths = JobUtility.GetPurposedFilePath("structuredup", jobTemp.DataSourceInformation.DerivedFileInfo);

                    if (File.Exists(exportedFilePaths))
                    {
                        string cont = System.IO.File.ReadAllText(exportedFilePaths);
                        if (i != 2 && exportedFilePaths.ToLower().EndsWith(".txt"))
                        {
                            string[] strText = System.IO.File.ReadAllLines(exportedFilePaths);
                            string headerText = strText[0].ToString();
                            cont = cont.Remove(0, headerText.Length);
                        }
                        uniqueCont = uniqueCont + cont;
                    }

                    if (File.Exists(structuredupexportedFilePaths))
                    {
                        string cont = System.IO.File.ReadAllText(structuredupexportedFilePaths);
                        if (i != 2 && structuredupexportedFilePaths.ToLower().EndsWith(".txt"))
                        {
                            string[] strText = System.IO.File.ReadAllLines(structuredupexportedFilePaths);
                            string headerText = strText[0].ToString();
                            cont = cont.Remove(0, headerText.Length);
                        }
                        duplicateCont = duplicateCont + cont;

                    }
                }
                if (uniqueCont != "")
                {
                    FileStream fsas = new FileStream(exportedFilePathfull, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter swa = new StreamWriter(fsas);

                    swa.WriteLine(uniqueCont);
                    swa.Close();
                    fsas.Close();
                }

                if (duplicateCont != "")
                {
                    FileStream fsas1 = new FileStream(structuredupexportedFilePathfull, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter swa1 = new StreamWriter(fsas1);

                    swa1.WriteLine(duplicateCont);
                    swa1.Close();
                    fsas1.Close();
                }
                
            }
        }
        private void _ReviewButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Loading the file need to take a few minutes! ", "whether to continue?", MessageBoxButtons.YesNo);
            if(dr==DialogResult.Yes)
            {
              
                reviewlabel.Visible = true;
                XmlDocument doc = new XmlDocument();
                int maxValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"].ToString());
                maxValue = (maxValue > 400) ? 400 : maxValue;
                int uniqueCount = 0;
                int duplicateCount = 0;

                string exportedFilePath = string.Empty;
                string structuredupexportedFilePath = string.Empty;
                string inputFileNamePath = string.Empty;
                if (_splitFiles != null)
                {
                    _MergerFile();
                    exportedFilePath = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
                    structuredupexportedFilePath = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);

                }
                else
                {
                    exportedFilePath = UniqueFileName;
                    structuredupexportedFilePath = DuplicateFileName;
                }
                inputFileNamePath = InputFileName;

                parsedArgs.RangeBegin = 1;
                parsedArgs.RangeEnd = 400;
                parsedArgs.ActionType = TargetActionType.FindDuplicates;
                //parsedArgs.MappingFile = _xmlPath;
                _job = NCDSBaseForm.ProcessArguments(parsedArgs);
                doc.LoadXml(_xml[0]);
                _job.Mappings = Mappings.GetMappings(doc.OuterXml);

                if (_HaveStructure == true ||
                    (_HaveStructure == false && _noStructureDupCheck == true))
                {

                    if (_DuplicateRadioButton.Checked == true)
                    {
                        if (File.Exists(structuredupexportedFilePath))
                        {
                            parsedArgs.DataFile = structuredupexportedFilePath;
                            parsedArgs.RangeEnd = 400;
                            _job.DataSourceInformation.FullFilePath = structuredupexportedFilePath;
                            // 0916 Jeff Edit
                            if (filetype == SourceFileType.MSExcel)
                            {
                                _job.DataSourceInformation.FileType = SourceFileType.CSV;
                                _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };

                            }

                            ReadInputFile.DoReadInputFile(_job);
                            IFileReader readerr = _job.FileReader;
                            readerr.Rewind();
                            duplicateCount = readerr.CountAll();
                            this.Refresh();
                            _job.ActionRanges[0].RangeEnd = 400;
                            if (_HaveStructure == true)
                            {
                                _structureDupRecords = DuplicateCheck.DoDuplicateCheck(_job, true, maxValue, ref uniqueCount, ref duplicateCount);
                            }
                            else
                            {
                                if (_splitFiles != null)
                                {
                                    string logFullPathBefore = this.FindLastFile();
                                    this.duplicateCheck(structuredupexportedFilePath);
                                    //_structureDupRecords = DuplicateCheck.DoDuplicateCheck(_job, false, maxValue, ref uniqueCount, ref duplicateCount);
                                    string logFullPath = this.FindLastFile();
                                    if (File.Exists(logFullPath) && logFullPathBefore != logFullPath)
                                    {
                                        StringBuilder result = new StringBuilder();
                                        StreamReader textStr = new StreamReader(logFullPath);
                                        string strLine = string.Empty;
                                        bool startRead = false;
                                        int lineCount = 0;
                                        while (!textStr.EndOfStream)
                                        {
                                            strLine = textStr.ReadLine();
                                            if (lineCount > 0)
                                            {
                                                string[] strResult = strLine.Split(',');
                                                if (strResult.Length > 2)
                                                {
                                                    if (!string.IsNullOrEmpty(strResult[1]))
                                                    {
                                                        if (strResult[1].Contains("matched"))
                                                        {
                                                            string[] strResultRegNum = strLine.Split(':');
                                                            if (strResultRegNum.Length > 1)
                                                            {
                                                                list_Regnum.Add(strResultRegNum[1].Replace("\"", string.Empty));
                                                            }
                                                        }
                                                        else if (strResult[1].Contains("unique"))
                                                        {
                                                            list_Regnum.Add(string.Empty);
                                                        }
                                                    }
                                                }

                                            }
                                            lineCount++;
                                        }
                                        textStr.Close();
                                    }
                                }


                                RecordsExtractionService extractionService;
                                extractionService = new RecordsExtractionService(_job);

                                _job.SourceRecords = extractionService.DoJob().ResponseContext["SourceRecords"] as List<ISourceRecord>;

                                int listIndex = 0;
                                _structureDupRecords.Clear();
                                foreach (ISourceRecord kvp in _job.SourceRecords)
                                {
                                    for (int ii = listIndex; ii < list_Regnum.Count; ii++)
                                    {
                                        if (!string.IsNullOrEmpty(list_Regnum[ii]))
                                        {
                                            _structureDupRecords.Add(kvp, list_Regnum[ii]);
                                            listIndex = ii + 1;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                        if (_structureDupRecords.Count == 0)
                        {
                            reviewlabel.Visible = false;
                            MessageBox.Show("No duplicate data!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        _job.SourceRecords.Clear();
                        foreach (KeyValuePair<ISourceRecord, string> kvp in _structureDupRecords)
                        {
                            _job.SourceRecords.Add(kvp.Key);
                        }

                        _Action = "0";
                    }
                    else if (_UniqueRadioButton.Checked == true)
                    {
                        if (File.Exists(exportedFilePath))
                        {
                            parsedArgs.DataFile = exportedFilePath;
                            parsedArgs.RangeEnd = 400;
                            _job.DataSourceInformation.FullFilePath = exportedFilePath;
                            // 0916 Jeff Edit
                            if (filetype == SourceFileType.MSExcel)
                            {
                                _job.DataSourceInformation.FileType = SourceFileType.CSV;
                                _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };

                            }

                            ReadInputFile.DoReadInputFile(_job);
                            IFileReader readerr = _job.FileReader;
                            readerr.Rewind();
                            uniqueCount = readerr.CountAll();
                            this.Refresh();
                            _job.ActionRanges[0].RangeEnd = 400;

                            if (_HaveStructure == true)
                            {
                                _uniqueRecords = DuplicateCheck.DoDuplicateCheck(_job, true, maxValue, ref uniqueCount, ref duplicateCount);
                            }

                            else
                            {
                                RecordsExtractionService extractionService;
                                extractionService = new RecordsExtractionService(_job);

                                _job.SourceRecords = extractionService.DoJob().ResponseContext["SourceRecords"] as List<ISourceRecord>;
                                _uniqueRecords.Clear();
                                foreach (ISourceRecord kvp in _job.SourceRecords)
                                {
                                    _uniqueRecords.Add(kvp, string.Empty);
                                }
                            }
                        }
                        if (_uniqueRecords.Count == 0)
                        {
                            reviewlabel.Visible = false;
                            MessageBox.Show("No unique data", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        _job.SourceRecords.Clear();

                        foreach (KeyValuePair<ISourceRecord, string> kvp in _uniqueRecords)
                        {
                            _job.SourceRecords.Add(kvp.Key);
                        }

                        _Action = "1";
                    }
                    else
                    {
                        parsedArgs.DataFile = inputFileNamePath;
                        parsedArgs.RangeEnd = 400;
                        _job.DataSourceInformation.FullFilePath = inputFileNamePath;

                        ReadInputFile.DoReadInputFile(_job);
                        IFileReader readerr = _job.FileReader;
                        readerr.Rewind();
                        this.Refresh();
                        _job.ActionRanges[0].RangeEnd = 400;

                        if (_HaveStructure == true)
                        {

                            _records = DuplicateCheck.DoDuplicateCheck(_job, true, maxValue, ref uniqueCount, ref duplicateCount);
                        }
                        else
                        {

                            if (_splitFiles != null)
                            {
                                string logFullPathBefore = this.FindLastFile();
                                this.duplicateCheck(inputFileNamePath);
                                //_structureDupRecords = DuplicateCheck.DoDuplicateCheck(_job, false, maxValue, ref uniqueCount, ref duplicateCount);
                                string logFullPath = this.FindLastFile();
                                if (File.Exists(logFullPath) && logFullPathBefore != logFullPath)
                                {
                                    StringBuilder result = new StringBuilder();
                                    StreamReader textStr = new StreamReader(logFullPath);
                                    string strLine = string.Empty;
                                    bool startRead = false;
                                    int lineCount = 0;
                                    while (!textStr.EndOfStream)
                                    {
                                        strLine = textStr.ReadLine();
                                        if (lineCount > 0)
                                        {
                                            string[] strResult = strLine.Split(',');
                                            if (strResult.Length > 2)
                                            {
                                                if (!string.IsNullOrEmpty(strResult[1]))
                                                {
                                                    if (strResult[1].Contains("matched"))
                                                    {
                                                        string[] strResultRegNum = strLine.Split(':');
                                                        if (strResultRegNum.Length > 1)
                                                        {
                                                            list_Regnum.Add(strResultRegNum[1].Replace("\"", string.Empty));
                                                        }
                                                    }
                                                    else if (strResult[1].Contains("unique"))
                                                    {
                                                        list_Regnum.Add(string.Empty);
                                                    }
                                                }
                                            }

                                        }
                                        lineCount++;
                                    }
                                    textStr.Close();
                                }
                            }

                            RecordsExtractionService extractionService;
                            extractionService = new RecordsExtractionService(_job);

                            _job.SourceRecords = extractionService.DoJob().ResponseContext["SourceRecords"] as List<ISourceRecord>;

                            int listIndex = 0;
                            _records.Clear();
                            if (list_Regnum.Count == _job.SourceRecords.Count)
                            {
                                foreach (ISourceRecord kvp in _job.SourceRecords)
                                {
                                    _records.Add(kvp, list_Regnum[listIndex]);
                                    listIndex++;
                                }
                            }

                        }

                        _job.SourceRecords.Clear();
                        foreach (KeyValuePair<ISourceRecord, string> kvp in _records)
                        {
                            _job.SourceRecords.Add(kvp.Key);
                        }



                        _Action = "2";
                    }

                    _Action = _Action + "," + "Review";
                }
                else
                {
                    _Action = "2,Review";
                }
                OnAccept(); // The parent will check to make certain the file exists
                reviewlabel.Visible = false;
            }
        }

        private void duplicateCheck(string strFile)
        {
            string _dataUploaderPath;

            string strFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            _dataUploaderPath = strFolder + "DataLoader2\\COEDataLoader.exe";
            //}
            if (!System.IO.File.Exists(_dataUploaderPath))
            {
                MessageBox.Show("Can't find COEDataLoader.exe. Please confim the path.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //string strFile = _InputFileChooser.InputFileName;
            if (!System.IO.File.Exists(strFile))
            {
                MessageBox.Show("Input file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
             string _xmlPath;
            _xmlPath = _job.DataSourceInformation.FullFilePath;
            int index = 1;

            while (File.Exists(_xmlPath + ".xml"))
            {
                _xmlPath = _job.DataSourceInformation.FullFilePath + index.ToString();
                index++;
            }

            _xmlPath = _xmlPath + ".xml";

            FileStream fs = new FileStream(_xmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(_xml[1]);
            sw.Close();
            fs.Close();
            //Add Mapping File Path to Delete List
            //if (!_files.Contains(_xmlPath))
            //{
            //    _files.Add(_xmlPath);
            //}


            if (!System.IO.File.Exists(_xmlPath))
            {
                MessageBox.Show("Mapping file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string _ImportOption = "findduplicates";

            int maxValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"].ToString());
            maxValue = (maxValue > 400) ? 400 : maxValue;

            ProcessStartInfo start = new ProcessStartInfo(_dataUploaderPath);

            string strArg = "/act:" + _ImportOption + "\n\r" +
                              "/data:" + strFile + "\n\r" +
                //" /type:" + _job.DataSourceInformation.FileType +
                              "/mapping:" + _xmlPath + "\n\r" +
                              "/begin:1" + "\n\r" +
                              "/end:" + maxValue + "\n\r";

            if (strFile.ToLower().EndsWith(".xls") ||
                strFile.ToLower().EndsWith(".xlsx"))
            {
                strArg = strArg + "/tbl:" + _job.DataSourceInformation.TableName + "\n\r" +
                         "/header:" + ((_job.DataSourceInformation.HasHeaderRow) ? "+" : "-") + "\n\r" +
                         "/type:" + "MSExcel";
            }
            else if (strFile.ToLower().EndsWith(".txt"))
            {
                strArg = strArg + "/header:" + ((_job.DataSourceInformation.HasHeaderRow) ? "+" : "-");
                if (_job.DataSourceInformation.FileType == SourceFileType.MSExcel)
                {
                    strArg = strArg + "\n\r" + "/delimiter:\\t" + "\n\r" +
                    "/type:" + "CSV";
                }
                else
                {
                    strArg += "\n\r" + "/delimiter:" + (_job.DataSourceInformation.FieldDelimiters[0].ToString().Equals("\t") ? "\\t" :
                    _job.DataSourceInformation.FieldDelimiters[0].ToString()) + "\n\r" +
                    "/type:" + "CSV";
                }
            }
            else
            {
                strArg = strArg + "/type:" + _job.DataSourceInformation.FileType;
            }

            // Export Command File
            string _CommandPath = "C:\\" + Path.GetFileName(strFile) + ".Command" + ".txt";
            index = 1;


            while (File.Exists(_CommandPath))
            {
                _CommandPath = "C:\\" + Path.GetFileName(strFile) + ".Command" + index.ToString() + ".txt";
                index++;
            }
            FileStream fss = new FileStream(_CommandPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sww = new StreamWriter(fss);
            sww.WriteLine(strArg);
            sww.Close();

            start.Arguments = " /command:" + _CommandPath + " /username:" + _userName +
                  " /password:" + _password;
            start.CreateNoWindow = true;
            //**************************************************************************

            //start.RedirectStandardOutput = true;//
            //start.RedirectStandardInput = true;//
            //**************************************************************************
            start.UseShellExecute = false;


            Process p = new Process();
            p.StartInfo = start;
            p.Start();

            p.WaitForExit();
            p.Close();

        }

        public string FindLastFile()
        {
            string logFullPath = CambridgeSoft.COE.DataLoader.Core.Workflow.JobUtility.GetLogFilePath();
            int index = logFullPath.LastIndexOf("\\");
            string logPath = Path.GetDirectoryName(logFullPath);
            DirectoryInfo d = new DirectoryInfo(logPath);
            FileInfo[] list = d.GetFiles();

            Array.Sort<FileInfo>(list, new FIleLastTimeComparer());

            if (list.Length > 0)
            {
                logFullPath = list[list.Length - 1].FullName;
            }
            else
            {
                logFullPath = string.Empty;
            }
            return logFullPath;
        }
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_HaveStructure == true ||
                (_HaveStructure == false && _noStructureDupCheck == true))
                {

                    if (!CheckExport())
                        return;

                    if (_Action == "0" || _Action == "1")
                    {
                        if (_DuplicateRadioButton.Checked == true)
                        {
                            _fName = _duplicateFileName;
                        }
                        else if (_UniqueRadioButton.Checked == true)
                        {
                            _fName = _uniqueFileName;
                        }
                        else if (_AllRadioButton.Checked == true)
                        {
                            _fName = _inputFileName;
                        }
                    }
                    else if (_Action == "2")
                    {
                        _fName = _job.DataSourceInformation.FullFilePath;
                    }

                    _Action = _Action + "," + "Import";
                }
                else
                {
                    _fName = _job.DataSourceInformation.FullFilePath;
                    _Action = "2,Import";
                }
                OnAccept();
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "ModeSelection");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        private Dictionary<ISourceRecord, string> export()
        {
            Dictionary<ISourceRecord, string> exportRecords = new Dictionary<ISourceRecord, string>();
            if (_DuplicateRadioButton.Checked == true)
            {
                _Action = "0";

                if (_structureDupRecords.Count == 0)
                {
                    MessageBox.Show("No duplicate data!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    exportRecords = _structureDupRecords;
                }
            }
            else if (_UniqueRadioButton.Checked == true)
            {
                _Action = "1";

                if (_uniqueRecords.Count == 0)
                {
                    MessageBox.Show("No unique data", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    exportRecords = _uniqueRecords;
                }
            }
            else
            {
                _Action = "2";
            }
            return exportRecords;
        }

        private void _ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Dictionary<ISourceRecord, string> exportRecords = export();
                                
                if (!CheckExport())
                    return;

                string sourceFile = string.Empty;
                if (_AllRadioButton.Checked == true)
                {
                    MessageBox.Show("It's the same as the input file!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }else if (_DuplicateRadioButton.Checked == true)
                {
                    sourceFile = _duplicateFileName;
                }
                else
                {
                    sourceFile = _uniqueFileName;
                }

                //if (exportRecords == null || exportRecords.Count == 0)
                //{
                //    return;
                //}
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
                        MessageBox.Show("Export completed!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    //ExportFile.doExportFile(_job, exportRecords, fName);

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "ModeSelection_ExportButton_Click");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private bool CheckExport()
        {
            if (_DuplicateRadioButton.Checked == true)
            {
                _Action = "0";

                if (Convert.ToInt32(_DuplicateCountLabel.Text) == 0)
                {
                    MessageBox.Show("No duplicate data!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (_UniqueRadioButton.Checked == true)
            {
                _Action = "1";

                if (Convert.ToInt32(_UniqueCountLabel.Text) == 0)
                {
                    MessageBox.Show("No unique data", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                _Action = "2";
            }
            return true;
        }
    }
}

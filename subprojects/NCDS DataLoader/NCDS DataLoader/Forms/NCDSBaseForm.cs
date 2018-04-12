using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.NCDS_DataLoader.Controls;
using CambridgeSoft.NCDS_DataLoader.Common;
using System.Diagnostics;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Threading;
using System.Data.OleDb;

namespace CambridgeSoft.NCDS_DataLoader.Forms
{
    public partial class NCDSBaseForm : Form
    {
        private Control _CurrentControl;
        private Control _PreControl;
        private InputFileChooser _InputFileChooser;
        private DisplayInputData _DisplayInputData;
        private InputOutputMapper _InputOutputMapper;
        private ModeSelection _ModeSelection;
        private ImportOption _ImportOption;
        private static bool _authenticated = false;
        private JobParameters _job;
        private string _InputField;
        private string[] _xml;
        private string _xmlPath;
        private delegate bool IncreaseHandle(int nValue);
        private string _traceFilePath;
        private List<string> _files = new List<string>();
        private Dictionary<ISourceRecord, string> _records = new Dictionary<ISourceRecord, string>();
        private Dictionary<ISourceRecord, string> _uniqueRecords = new Dictionary<ISourceRecord, string>();
        private Dictionary<ISourceRecord, string> _structureDupRecords = new Dictionary<ISourceRecord, string>();
        private int _recordsCount = 0;
        IndividualArgumentsCommandInfo parsedArgs;
        private string[] splitfiles = null;


        //public Process[] ProDuplicate = null;
        // constructor
        /// <summary>
        /// <c>NCDSBaseForm</c> is the constructor for the main form.
        /// </summary>
        public NCDSBaseForm()
        {
            InitializeComponent();
            _job = new JobParameters();
            _InputFileChooser = new InputFileChooser();
            _DisplayInputData = new DisplayInputData();
            _InputOutputMapper = new InputOutputMapper();
            _ModeSelection = new ModeSelection();
            _ImportOption = new ImportOption();
            _InputFileChooser.Accept += new EventHandler(InputFileChooser_Accept);
            _InputFileChooser.Cancel += new EventHandler(InputFileChooser_Cancel);
            _DisplayInputData.Cancel += new EventHandler(DisplayInputData_Cancel);
            _DisplayInputData.Accept += new EventHandler(DisplayInputData_Accept);
            _InputOutputMapper.Cancel += new EventHandler(InputOutputMapper_Cancel);
            _InputOutputMapper.Accept += new EventHandler(InputOutputMapper_Accept);
            _ModeSelection.Cancel += new EventHandler(ModeSelection_Cancel);
            _ModeSelection.Accept += new EventHandler(ModeSelection_Accept);
            _ImportOption.Cancel += new EventHandler(ImportOption_Cancel);
            _ImportOption.Accept += new EventHandler(ImportOption_Accept);

            string appFolderLocation = COEConfigurationBO.ConfigurationBaseFilePath;
            string logOutPutPath = Path.Combine(appFolderLocation, @"TraceLogOutput\" + "NCDS DataLoader");
            Directory.CreateDirectory(logOutPutPath);//create directory if not exists
            string fileName = string.Format("{0:yy-MM-ddTHH-mm-ss}", DateTime.Now);
            _traceFilePath = CambridgeSoft.COE.DataLoader.Core.Workflow.JobUtility.GetPurposedFilePath("Log", new FileInfo(Path.Combine(logOutPutPath, fileName)));

            TextWriter m_tracer;
            m_tracer = File.CreateText(_traceFilePath);
            System.Diagnostics.TextWriterTraceListener objTraceListener = new TextWriterTraceListener(m_tracer);
            Trace.Listeners.Add(objTraceListener); 
        }

        #region EVENT
        /************************************************************************
        ** InputFileChooser
        **************************************************************************/
        private void InputFileChooser_Accept(object sender, EventArgs e)
        {
            string strExt;
            string strInputFile = _InputFileChooser.InputFileName;
            parsedArgs = new IndividualArgumentsCommandInfo();
            Accept(_InputFileChooser, _InputOutputMapper, delegate()
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Make sure it at least exists
                    if (strInputFile.Trim().Length == 0)
                    {
                        MessageBox.Show("Please select the input file. ", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    // Make sure it at least exists
                    if (File.Exists(strInputFile) == false)
                    {
                        MessageBox.Show("File does not exist: '" + strInputFile + "'", "NCDS DataLoader", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    // Any Input objects
                    List<string> listMatchingFilters = _InputFileChooser.MatchingFilters;
                    strExt = Path.GetExtension(strInputFile).ToLower();
                    if (listMatchingFilters.Count == 0)
                    {
                        MessageBox.Show("Unknown filename extension: " + strExt, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    // Too many Input objects?
                    if (listMatchingFilters.Count > 1)
                    {
                        MessageBox.Show("Use the Browse button and choose the desired entry under file of type", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }


                    if (string.IsNullOrEmpty(_InputFileChooser.UserName) && _InputFileChooser.Split == false)
                    {
                        MessageBox.Show("Please input the user name.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }


                    if (string.IsNullOrEmpty(_InputFileChooser.Password) && _InputFileChooser.Split == false)
                    {
                        MessageBox.Show("Please input the password.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }

                    bool bIsAuthenticated = false;

                    if (_InputFileChooser.Split == false)
                    {
                        bIsAuthenticated = AuthenticateUser(_InputFileChooser.UserName,
                                             _InputFileChooser.Password);
                    }
                    
                    if (!bIsAuthenticated && _InputFileChooser.Split == false)
                    {
                        return true;
                    }

                    if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                    {
                        if (string.IsNullOrEmpty(_InputFileChooser.WorkSheet))
                        {
                            MessageBox.Show("Please select a worksheet.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }

                        if (string.IsNullOrEmpty(_InputFileChooser.Header))
                        {
                            MessageBox.Show("Please select the Header.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                        parsedArgs.TableOrWorksheet = _InputFileChooser.WorkSheet;
                        parsedArgs.HasHeader = _InputFileChooser.IsHeader;
                    }
                    else if (_InputFileChooser.GetFileType == SourceFileType.CSV)
                    {
                        if (string.IsNullOrEmpty(_InputFileChooser.Header))
                        {
                            MessageBox.Show("Please select the Header.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }

                        if (_InputFileChooser.Delimiter.Length == 0)
                        {
                            MessageBox.Show("Please input the Delimiter.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                        parsedArgs.HasHeader = _InputFileChooser.IsHeader;
                        parsedArgs.Delimiters = new string[] { _InputFileChooser.Delimiter };
                    }

                    parsedArgs.DataFile = _InputFileChooser.InputFileName;
                    parsedArgs.FileType = _InputFileChooser.GetFileType;
                    parsedArgs.UserName = _InputFileChooser.UserName;
                    parsedArgs.Password = _InputFileChooser.Password;
                    parsedArgs.RangeBegin = 1;
                    parsedArgs.RangeEnd = 2147483647;

                    if (_InputFileChooser.Split == true)
                    {
                        using (SplitForm form = new SplitForm())
                        {
                            form.ShowDialog();
                            if (form.OK)
                            {
                                parsedArgs.RangeEnd = Convert.ToInt32(form.GetNumber);
                                parsedArgs.ActionType = TargetActionType.SplitFile;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }

                    Clear();

                    _job = new JobParameters();
                    SourceFieldTypes.TypeDefinitions.Clear();
                    _job = ProcessArguments(parsedArgs);

                    string message = ReadInputFile.DoReadInputFile(_job);

                    if (!string.IsNullOrEmpty(message) && _InputFileChooser.Split != true)
                    {
                        MessageBox.Show(message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    else if (!string.IsNullOrEmpty(message) && _InputFileChooser.Split == true)
                    {
                        MessageBox.Show(message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }

                    //IFileReader reader = ReadInputFile.DoReadInputFile(_job);
                    IFileReader reader = _job.FileReader;
                    List<ISourceRecord> records = new List<ISourceRecord>();

                    reader.Rewind();

                    _InputField = string.Empty;
                    if (reader.CountAll() > 0)
                    {
                        _InputField = InputFieldSpec(SourceFieldTypes.TypeDefinitions);
                    }
                    int intMaxInputRecordValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxInputRecordValue"]);

                    if (reader.CountAll() > intMaxInputRecordValue)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                    {
                        MessageBox.Show(string.Format("Please use a file with less than {0} records.", intMaxInputRecordValue), "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    if (reader.CountAll() > 15000)
                    {
                        MessageBox.Show("Can't load a sample file within more than 10,000 records.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }   
                    
                    _recordsCount = reader.CountAll();
                    _InputOutputMapper.HaveStructure = _InputFileChooser.HaveStructure;
                    _InputOutputMapper.InputFieldSpec = _InputField;
                    _InputOutputMapper.Invalid = 0;
                    _InputOutputMapper.ParsedArgs = parsedArgs;
                    _InputOutputMapper.SetForm();

                    this._TitleLabel.Text = "Field Mapping  ";
                    this._CommentLabel.Text = "Please select the mapping fields.";
                    return false;
                }
                catch (Exception ex)
                {
                    string message = ex.Message + "\n" + ex.StackTrace;
                    Trace.WriteLine(DateTime.Now, "Time ");
                    Trace.WriteLine(message, "InputFileChooser_Accept_Exception");
                    Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.UserName) ? string.Empty : _InputFileChooser.UserName), "User Name ");
                    Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.InputFileName) ? string.Empty : _InputFileChooser.InputFileName), "File Name ");
                    Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.WorkSheet) ? string.Empty : _InputFileChooser.WorkSheet), "WorkSheet ");
                    Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.Delimiter) ? string.Empty : _InputFileChooser.Delimiter), "Delimiter ");
                    Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.IsHeader.ToString()) ? string.Empty : _InputFileChooser.IsHeader.ToString()), "Header ");
                    Trace.Flush();
                    MessageBox.Show("Unable to open the file.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            });
            return;
        } // InputFileChooser_Accept()

        private void InputFileChooser_Cancel(object sender, EventArgs e)
        {
            // Transition Login to Exit
            {
                SuspendLayout();
                Controls.Remove(_InputFileChooser); _InputFileChooser = null;
                ResumeLayout(false);
                PerformLayout();
            }

            Close();
            return;
        } // InputFileChooser_Cancel()

        private void TduplicateCheck(string splitFilePath)
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


            //2012
            string strFile = _InputFileChooser.InputFileName;
            string strFileSplit = splitFilePath;
            bool boolflag =_job.DataSourceInformation.HasHeaderRow;

            if (strFile.ToLower().EndsWith(".txt"))
            {
                if (boolflag)
                {
                    string[] strText = System.IO.File.ReadAllLines(strFile);
                    string headerText = strText[0].ToString();

                    //string strFileSplit = splitFilePath;
                    string[] strTextSplit = System.IO.File.ReadAllLines(strFileSplit);

                    //string headerText = strText[0].ToString();
                    if (strTextSplit[0].ToString() != strText[0].ToString())
                    {
                        string cont = System.IO.File.ReadAllText(strFileSplit);
                        string headerTexts = headerText + "\r" + cont;
                        System.IO.File.Delete(strFileSplit);
                        
                        FileStream fsas = new FileStream(strFileSplit, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                        
                            StreamWriter swa = new StreamWriter(fsas);

                            swa.WriteLine(headerTexts);
                            swa.Close();
                            fsas.Close();
                            swa.Dispose();
                            fsas.Dispose();
                        
                        
                    }
                }
                //System.IO.File.Delete(strFileSplit);
            }

            if (strFile.ToLower().EndsWith(".xls") ||
                strFile.ToLower().EndsWith(".xlsx"))
            {
                if (boolflag)
                {
                    string strHead = "NO";
                    if (_job.DataSourceInformation.HasHeaderRow)
                    { strHead = "NO"; }
                    else
                    {
                        strHead = "YES";
                    }
                    string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFile + ";Extended Properties=\"Excel 8.0;HDR=" + strHead + ";IMEX=1\"";
                    OleDbConnection myConn = new OleDbConnection(strCon);
                    string sheetname = _job.DataSourceInformation.TableName;
                    //string strCom = " SELECT * FROM [Sheet1]";
                    string strCom = " SELECT * FROM [" + sheetname + "$]";
                    myConn.Open();
                    //DataTable sheetNames = myConn.GetOleDbSchemaTable
                    //(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    //DataRow dr = sheetNames.Rows[0];

                    OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
                    DataSet ds = new DataSet();
                    myCommand.Fill(ds);
                    myConn.Close();
                    string headerText = "";

                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        headerText += ds.Tables[0].Rows[0][j].ToString() + "\t";
                    }



                    string[] strTextSplit = System.IO.File.ReadAllLines(strFileSplit);

                    if (strTextSplit[0].ToString() != headerText)
                    {
                        headerText = headerText.Trim();
                        string cont = System.IO.File.ReadAllText(strFileSplit);
                        string headerTexts = headerText + "\r" + cont;
                        System.IO.File.Delete(strFileSplit);


                        FileStream fsas = new FileStream(strFileSplit, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter swa = new StreamWriter(fsas);

                        swa.WriteLine(headerTexts);
                        swa.Close();
                        fsas.Close();
                    }
                }
            }

            if (!System.IO.File.Exists(strFileSplit))
            {
                MessageBox.Show("Input file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //2012
            //_xmlPath = _job.DataSourceInformation.FullFilePath;
            _xmlPath = splitFilePath;
            int index = 1;

            while (File.Exists(_xmlPath + ".xml"))
            {
                //2012
                //_xmlPath = _job.DataSourceInformation.FullFilePath + index.ToString();
                _xmlPath = splitFilePath + index.ToString();
                index++;
            }

            _xmlPath = _xmlPath + ".xml";

            FileStream fs = new FileStream(_xmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(_xml[1]);
            sw.Close();
            fs.Close();
            //Add Mapping File Path to Delete List
            if (!_files.Contains(_xmlPath))
            {
                _files.Add(_xmlPath);
            }


            if (!System.IO.File.Exists(_xmlPath))
            {
                MessageBox.Show("Mapping file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string _ImportOption = "findduplicates";

            //ExecuteCOEDataLoader();

            ProcessStartInfo start = new ProcessStartInfo(_dataUploaderPath);

            string strArg = "/act:" + _ImportOption + "\n\r" +
                              "/data:" + strFileSplit + "\n\r" +
                //" /type:" + _job.DataSourceInformation.FileType +
                              "/mapping:" + _xmlPath + "\n\r" +
                              "/begin:1" + "\n\r" +
                              "/end:2147483647" + "\n\r";

            if (strFileSplit.ToLower().EndsWith(".xls") ||
                strFileSplit.ToLower().EndsWith(".xlsx"))
            {
                strArg = strArg + "/tbl:" + _job.DataSourceInformation.TableName + "\n\r" +
                         "/header:" + ((_job.DataSourceInformation.HasHeaderRow) ? "+" : "-") + "\n\r" +
                         "/type:" + "MSExcel";
            }
            else if (strFileSplit.ToLower().EndsWith(".txt"))
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
            string _CommandPath = "C:\\" + Path.GetFileName(strFileSplit) + ".Command" + ".txt";
            index = 1;


            while (File.Exists(_CommandPath))
            {
                _CommandPath = "C:\\" + Path.GetFileName(strFileSplit) + ".Command" + index.ToString() + ".txt";
                index++;
            }
            FileStream fss = new FileStream(_CommandPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sww = new StreamWriter(fss);
            sww.WriteLine(strArg);
            sww.Close();
            fss.Close();
            if (!_files.Contains(_CommandPath))
            {
                _files.Add(_CommandPath);
            }

            start.Arguments = " /command:" + _CommandPath + " /username:" + _InputFileChooser.UserName +
                              " /password:" + _InputFileChooser.Password;

            start.CreateNoWindow = false;
            //**************************************************************************

            //start.RedirectStandardOutput = true;//
            //start.RedirectStandardInput = true;//
            //**************************************************************************
            start.UseShellExecute = false;


            Process p = new Process();
            p.StartInfo = start;
            p.Start();

            //**************************************************************************
            //StringBuilder strResult1 = new StringBuilder();
            //StreamReader reader = p.StandardOutput;
            //string line = reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    strResult1.Append(line + "\n");
            //    line = reader.ReadLine();
            //}
            //**************************************************************************
            //p.WaitForExit();
            p.Close();

        }

        private void duplicateCheck()
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
           
            string strFile = _InputFileChooser.InputFileName;
            if (!System.IO.File.Exists(strFile))
            {
                MessageBox.Show("Input file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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
            if (!_files.Contains(_xmlPath))
            {
                _files.Add(_xmlPath);
            }


            if (!System.IO.File.Exists(_xmlPath))
            {
                MessageBox.Show("Mapping file error.", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string _ImportOption = "findduplicates";


            ProcessStartInfo start = new ProcessStartInfo(_dataUploaderPath);

            string strArg = "/act:" + _ImportOption + "\n\r" +
                              "/data:" + strFile + "\n\r" +
                              "/mapping:" + _xmlPath + "\n\r" +
                              "/begin:1" + "\n\r" +
                              "/end:2147483647" + "\n\r";

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
            fss.Close();
            if (!_files.Contains(_CommandPath))
            {
                _files.Add(_CommandPath);
            }

            start.Arguments = " /command:" + _CommandPath + " /username:" + _InputFileChooser.UserName +
                              " /password:" + _InputFileChooser.Password;

            start.CreateNoWindow = false;
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

        /************************************************************************
        ** InputOutputMapper
        **************************************************************************/
        private void InputOutputMapper_Accept(object sender, EventArgs e)
        {
            string diaplayValue = System.Configuration.ConfigurationManager.AppSettings["SplitValue"].ToString();
            string messagesplit = "";
            int sourceNum = 0;
            if (splitfiles!=null)
            {
            splitfiles.Initialize();}
            IFileReader readerSplit = _job.FileReader;
            //sourceNum = readerSplit.CountAll();
            sourceNum = readerSplit.RecordCount;
            if (sourceNum > System.Convert.ToInt32(diaplayValue))
            {
                parsedArgs.RangeEnd = Convert.ToInt32(diaplayValue);
                parsedArgs.ActionType = TargetActionType.SplitFile;
                _job = ProcessArguments(parsedArgs);

                messagesplit = ReadInputFile.DoReadInputFile(_job);
                splitfiles = messagesplit.Split('\r');

                for (int s = 0; s < splitfiles.Length; s++)
                {
                    splitfiles[s] = splitfiles[s].TrimEnd();

                    if (!_files.Contains(splitfiles[s].ToString()))
                    {
                        _files.Add(splitfiles[s].ToString());
                    }
                }
            }



            bool skipcheck = _InputOutputMapper.SkipCheck;
            if (skipcheck)
            {
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
                _xml = _InputOutputMapper.Mapping;
                sw.WriteLine(_xml[1]);
                sw.Close();
                fs.Close();

                if (!_files.Contains(_xmlPath))
                {
                    _files.Add(_xmlPath);
                }

                Accept(_InputOutputMapper, _ImportOption, delegate()
    {
        _ImportOption.JOB = _job;
        _ImportOption.UserName = _InputFileChooser.UserName;
        _ImportOption.Password = _InputFileChooser.Password;
        _ImportOption.FullFilePath = _InputFileChooser.InputFileName;
        _ImportOption.StrFilePath = _InputFileChooser.InputFileName;
        _ImportOption.SplitFiles = splitfiles;
        _ImportOption.XmlPath = _xmlPath;
        _ImportOption.OptionPanelEnabled = true;
        _ImportOption.AcceptButton.Text = "Next";
        _ImportOption.ResultVisable = false;
        _ImportOption.Skipflag = skipcheck;
        _ImportOption.AuthorizeUser();
        this._TitleLabel.Text = "Import Option ";
        this._CommentLabel.Text = "Please select one option to be applied.";
        this._PreControl = _InputOutputMapper;

        return false;
    });
            }
            else{
                ToModeSelection(splitfiles);
            };
            return;
        } // InputOutputMapper_Accept()
        //delegate void ToModeSelectionDelegate(string[] splitfiles);
        public void ToModeSelection(string[] splitfiles)
        {
            int uniqueCount = 0;
            int duplicateCount = 0;
            string exportedFilePath = string.Empty;
            string structuredupexportedFilePath = string.Empty;

            List<string> list_Regnum = new List<string>();
            Accept(_InputOutputMapper, _ModeSelection, delegate()
    {
        try
        {
            if (splitfiles != null)
            {
                for (int j = 2; j < splitfiles.Length - 1; j++)
                {
                    this.Cursor = Cursors.WaitCursor;

                    this.Refresh();

                    XmlDocument doc = new XmlDocument();

                    _xml = _InputOutputMapper.Mapping;
                    _records = new Dictionary<ISourceRecord, string>();
                    _uniqueRecords = new Dictionary<ISourceRecord, string>();
                    _structureDupRecords = new Dictionary<ISourceRecord, string>();
                    bool noStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;

                    int maxValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"].ToString());
                    maxValue = (maxValue > 400) ? 400 : maxValue;

                    {

                        {
                            doc.LoadXml(_xml[1]);

                            Dictionary<ISourceRecord, string> currentRecrods;

                            int intSplit = 400;
                            int remainCount = 0;
                            //2012

                            IndividualArgumentsCommandInfo parsedArgsTemp = new IndividualArgumentsCommandInfo();
                            parsedArgsTemp.DataFile = splitfiles[j].ToString();
                            JobParameters jobTemp = new JobParameters();
                            jobTemp = ProcessArguments(parsedArgsTemp);

                            exportedFilePath = JobUtility.GetPurposedFilePath("unique", jobTemp.DataSourceInformation.DerivedFileInfo);
                            structuredupexportedFilePath = JobUtility.GetPurposedFilePath("structuredup", jobTemp.DataSourceInformation.DerivedFileInfo);

                            if (!_files.Contains(exportedFilePath))
                            {
                                _files.Add(exportedFilePath);
                            }
                            if (!_files.Contains(structuredupexportedFilePath))
                            {
                                _files.Add(structuredupexportedFilePath);
                            }
                            try
                            {
                                if (File.Exists(exportedFilePath))
                                    File.Delete(exportedFilePath);
                                if (File.Exists(structuredupexportedFilePath))
                                    File.Delete(structuredupexportedFilePath);
                            }
                            catch
                            {
                            }
                            string logFullPathBefore = _ImportOption.FindLastFile();

                            TduplicateCheck(splitfiles[j].ToString());

                            Thread.Sleep(10000);
                        }



                    }

                    if (_job.SourceRecords != null)
                    {
                        _job.SourceRecords.Clear();
                    }
                }
                t.SynchronizingObject = this;
                t.Elapsed += new System.Timers.ElapsedEventHandler(timerProcess);
                t.AutoReset = true;
                t.Enabled = true;

                _ModeSelection.AcceptButton.Enabled = false;
                _ModeSelection.CancelButton.Enabled = false;
                _ModeSelection.panel1.Enabled = false;
                _ModeSelection._AllCountLabel.Visible=false;
                _ModeSelection._ExportButton.Enabled=false;
                _ModeSelection._ImportButton.Enabled=false;
                _ModeSelection._ReviewButton.Enabled = false;
                _ModeSelection.reviewlabel.Visible = true;
                _ModeSelection.reviewlabel.Text = "Duplicate checking,please wait...";
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;

                this.Refresh();

                XmlDocument doc = new XmlDocument();

                _xml = _InputOutputMapper.Mapping;
                _records = new Dictionary<ISourceRecord, string>();
                _uniqueRecords = new Dictionary<ISourceRecord, string>();
                _structureDupRecords = new Dictionary<ISourceRecord, string>();
                bool noStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;

                int maxValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"].ToString());
                maxValue = (maxValue > 400) ? 400 : maxValue;

                {

                    {
                        doc.LoadXml(_xml[1]);

                        Dictionary<ISourceRecord, string> currentRecrods;

                        int intSplit = 400;
                        int remainCount = 0;

                        exportedFilePath = JobUtility.GetPurposedFilePath("unique", _job.DataSourceInformation.DerivedFileInfo);
                        structuredupexportedFilePath = JobUtility.GetPurposedFilePath("structuredup", _job.DataSourceInformation.DerivedFileInfo);
                        if (!_files.Contains(exportedFilePath))
                        {
                            _files.Add(exportedFilePath);
                        }
                        if (!_files.Contains(structuredupexportedFilePath))
                        {
                            _files.Add(structuredupexportedFilePath);
                        }
                        try
                        {
                            if (File.Exists(exportedFilePath))
                                File.Delete(exportedFilePath);
                            if (File.Exists(structuredupexportedFilePath))
                                File.Delete(structuredupexportedFilePath);
                        }
                        catch
                        {
                        }
                        string logFullPathBefore = _ImportOption.FindLastFile();

                        duplicateCheck();
                        try
                        {
                            if (_InputOutputMapper.HaveStructure != true)
                            {

                                string logFullPath = _ImportOption.FindLastFile();
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
                        }
                        catch
                        {
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            parsedArgs.RangeBegin = 1;
                            parsedArgs.RangeEnd = 400;
                            parsedArgs.DataFile = _InputFileChooser.InputFileName;
                            parsedArgs.ActionType = TargetActionType.FindDuplicates;
                            parsedArgs.MappingFile = _xmlPath;
                            _job = ProcessArguments(parsedArgs);
                            doc.LoadXml(_xml[0]);
                            _job.Mappings = Mappings.GetMappings(doc.OuterXml);
                            string message = ReadInputFile.DoReadInputFile(_job);

                            this.Refresh();

                            // unique file

                            if (File.Exists(exportedFilePath))
                            {
                                parsedArgs.DataFile = exportedFilePath;
                                parsedArgs.RangeEnd = 99999999;
                                _job.DataSourceInformation.FullFilePath = exportedFilePath;
                                // 0916 Jeff Edit
                                if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                                {
                                    _job.DataSourceInformation.FileType = SourceFileType.CSV;
                                    _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };
                                }

                                ReadInputFile.DoReadInputFile(_job);

                                IFileReader reader = _job.FileReader;
                                reader.Rewind();
                                uniqueCount = reader.CountAll();

                                this.Refresh();
                                _job.ActionRanges[0].RangeEnd = 400;

                                reader.Close();

                            }
                            // structuredup file
                            if (File.Exists(structuredupexportedFilePath))
                            {
                                parsedArgs.DataFile = structuredupexportedFilePath;
                                parsedArgs.RangeEnd = 99999999;
                                _job.DataSourceInformation.FullFilePath = structuredupexportedFilePath;
                                // 0916 Jeff Edit
                                if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                                {
                                    _job.DataSourceInformation.FileType = SourceFileType.CSV;
                                    _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };

                                }

                                message = ReadInputFile.DoReadInputFile(_job);
                                IFileReader readerr = _job.FileReader;
                                readerr.Rewind();
                                duplicateCount = readerr.CountAll();
                                this.Refresh();
                                _job.ActionRanges[0].RangeEnd = 400;

                                readerr.Close();

                            }
                        }
                    }



                }

                if (_job.SourceRecords != null)
                {
                    _job.SourceRecords.Clear();
                }
                if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                {
                    _job.DataSourceInformation.FileType = SourceFileType.MSExcel;
                    _job.DataSourceInformation.FieldDelimiters = null;
                }

                // Export Mapping File
                if ((uniqueCount + duplicateCount) == _recordsCount)
                {
                    _ModeSelection.AllCount = Convert.ToString((uniqueCount + duplicateCount));
                }
                else
                {
                    _ModeSelection.AllCount = (uniqueCount + duplicateCount) + "," + (_recordsCount - (uniqueCount + duplicateCount)).ToString();
                }
                _job.DataSourceInformation.FullFilePath = _InputFileChooser.InputFileName;

                _ModeSelection.DuplicateCount = duplicateCount.ToString();
                _ModeSelection.UniqueCount = uniqueCount.ToString();
                _ModeSelection.JOB = _job;
                _ModeSelection.RECORDS = _records;
                _ModeSelection.UNIQUERECORDS = _uniqueRecords;
                _ModeSelection.STRUCTUREDUPRECORDS = _structureDupRecords;
                _ModeSelection.NoStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;
                _ModeSelection.HaveStructure = _InputOutputMapper.HaveStructure;
                _ModeSelection.DuplicateFileName = structuredupexportedFilePath;
                _ModeSelection.InputFileName = _InputFileChooser.InputFileName;
                _ModeSelection.UniqueFileName = exportedFilePath;
                _ModeSelection.ParsedArgs = _InputOutputMapper.ParsedArgs;
                _ModeSelection._Xml = _xml;
                _ModeSelection.List_Regnum = list_Regnum;
                _ModeSelection.FileType = _InputFileChooser.GetFileType;
                _ModeSelection.UserName = _InputFileChooser.UserName;
                _ModeSelection.Password = _InputFileChooser.Password;

                if (duplicateCount == 0 && uniqueCount == 0)
                {
                    _ModeSelection.AcceptButton.Enabled = false;
                    _ModeSelection.CancelButton.Enabled = true;
                    _ModeSelection.panel1.Enabled = false;
                    _ModeSelection._ExportButton.Enabled = false;
                    _ModeSelection._ImportButton.Enabled = false;
                    _ModeSelection._ReviewButton.Enabled = false;
                    _ModeSelection._AllCountLabel.Visible = true;
                    _ModeSelection.reviewlabel.Visible = false;
                }

                this._TitleLabel.Text = string.Empty;
                this._CommentLabel.Text = string.Empty;
            }

            return false;
        }
        catch (Exception ex)
        {
            string message = ex.Message + "\n" + ex.StackTrace;

            Trace.WriteLine(DateTime.Now, "Time ");
            Trace.WriteLine(message, "InputOutputMapper_Accept_Exception");
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
            return true;
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    });
        }
        //2012
        System.Timers.Timer t = new System.Timers.Timer(10000);
        public void timerProcess(object source, System.Timers.ElapsedEventArgs e)
        {
            int uniqueCount = 0;
            int duplicateCount = 0;
            List<string> list_Regnum = new List<string>();

            string name = "COEDataLoader";
            Process[] prc = Process.GetProcesses();
            foreach (Process pr in prc)
            {
                if (name == pr.ProcessName)
                {
                    return;
                }
            }

            t.Enabled = false;
            string exportedFilePath = string.Empty;
            string structuredupexportedFilePath = string.Empty;
            IndividualArgumentsCommandInfo parsedArgsTemp = new IndividualArgumentsCommandInfo();


            for (int i = 2; i < splitfiles.Length - 1; i++)
            {
                            
                    parsedArgsTemp.DataFile = splitfiles[i].ToString();
                    XmlDocument doc = new XmlDocument();
                    JobParameters jobTemp = new JobParameters();
                    
                    jobTemp = ProcessArguments(parsedArgsTemp);

                    exportedFilePath = JobUtility.GetPurposedFilePath("unique", jobTemp.DataSourceInformation.DerivedFileInfo);
                    structuredupexportedFilePath = JobUtility.GetPurposedFilePath("structuredup", jobTemp.DataSourceInformation.DerivedFileInfo);

                parsedArgs.RangeBegin = 1;
                parsedArgs.RangeEnd = 400;
                parsedArgs.DataFile = splitfiles[i];
                parsedArgs.ActionType = TargetActionType.FindDuplicates;
                parsedArgs.MappingFile = _xmlPath;
                _job = ProcessArguments(parsedArgs);
                doc.LoadXml(_xml[0]);
                _job.Mappings = Mappings.GetMappings(doc.OuterXml);
                if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                {
                    _job.DataSourceInformation.FileType = SourceFileType.CSV;
                    _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };
                }
                string message = ReadInputFile.DoReadInputFile(_job);

                this.Refresh();

                // unique file

                if (File.Exists(exportedFilePath))
                {
                    parsedArgs.DataFile = exportedFilePath;
                    parsedArgs.RangeEnd = 99999999;
                    _job.DataSourceInformation.FullFilePath = exportedFilePath;
                    // 0916 Jeff Edit
                    if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                    {
                        _job.DataSourceInformation.FileType = SourceFileType.CSV;
                        _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };
                    }

                    ReadInputFile.DoReadInputFile(_job);

                    IFileReader reader = _job.FileReader;
                    reader.Rewind();
                    uniqueCount += reader.CountAll();

                    this.Refresh();
                    _job.ActionRanges[0].RangeEnd = 400;

                    reader.Close();

                }
                // structuredup file
                if (File.Exists(structuredupexportedFilePath))
                {
                    parsedArgs.DataFile = structuredupexportedFilePath;
                    parsedArgs.RangeEnd = 99999999;
                    _job.DataSourceInformation.FullFilePath = structuredupexportedFilePath;
                    // 0916 Jeff Edit
                    if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
                    {
                        _job.DataSourceInformation.FileType = SourceFileType.CSV;
                        _job.DataSourceInformation.FieldDelimiters = new string[] { "\t" };

                    }

                    message = ReadInputFile.DoReadInputFile(_job);
                    IFileReader readerr = _job.FileReader;
                    readerr.Rewind();
                    duplicateCount += readerr.CountAll();
                    this.Refresh();
                    _job.ActionRanges[0].RangeEnd = 400;

                    readerr.Close();

                }
        
            }

            //// 0916 Jeff Edit
            //if (_InputFileChooser.GetFileType == SourceFileType.MSExcel)
            //{
            //    _job.DataSourceInformation.FileType = SourceFileType.MSExcel;
            //    _job.DataSourceInformation.FieldDelimiters = null;
            //}

            // Export Mapping File
            if ((uniqueCount + duplicateCount) == _recordsCount)
            {
                _ModeSelection.AllCount = Convert.ToString((uniqueCount + duplicateCount));
            }
            else
            {
                _ModeSelection.AllCount = (uniqueCount + duplicateCount) + "," + (_recordsCount - (uniqueCount + duplicateCount)).ToString();
            }
            _job.DataSourceInformation.FullFilePath = _InputFileChooser.InputFileName;

            _ModeSelection.DuplicateCount = duplicateCount.ToString();
            _ModeSelection.UniqueCount = uniqueCount.ToString();
            _ModeSelection.JOB = _job;
            _ModeSelection.RECORDS = _records;
            _ModeSelection.UNIQUERECORDS = _uniqueRecords;
            _ModeSelection.STRUCTUREDUPRECORDS = _structureDupRecords;
            _ModeSelection.NoStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;
            _ModeSelection.HaveStructure = _InputOutputMapper.HaveStructure;
            _ModeSelection.DuplicateFileName = structuredupexportedFilePath;
            _ModeSelection.InputFileName = _InputFileChooser.InputFileName;
            _ModeSelection.UniqueFileName = exportedFilePath;
            _ModeSelection.ParsedArgs = _InputOutputMapper.ParsedArgs;
            _ModeSelection.UserName = _InputFileChooser.UserName;
            _ModeSelection.Password = _InputFileChooser.Password;
            _ModeSelection._Xml = _xml;
            _ModeSelection.List_Regnum = list_Regnum;
            _ModeSelection.FileType = _InputFileChooser.GetFileType;
            _ImportOption.SplitFiles = splitfiles;
            _ModeSelection.SplitFiles = splitfiles;
            if (duplicateCount == 0 && uniqueCount == 0) {
                _ModeSelection.AcceptButton.Enabled = false;
                _ModeSelection.CancelButton.Enabled = true;
                _ModeSelection.panel1.Enabled = false;
                _ModeSelection._ExportButton.Enabled = false;
                _ModeSelection._ImportButton.Enabled = false;
                _ModeSelection._ReviewButton.Enabled = false;
                _ModeSelection._AllCountLabel.Visible = false;
                _ModeSelection.reviewlabel.Visible = false;
            }
            else
            {
                _ModeSelection.AcceptButton.Enabled = true;
                _ModeSelection.CancelButton.Enabled = true;
                _ModeSelection.panel1.Enabled = true;
                _ModeSelection._ExportButton.Enabled = true;
                _ModeSelection._ImportButton.Enabled = true;
                _ModeSelection._ReviewButton.Enabled = true;
                _ModeSelection._AllCountLabel.Visible = true;
                _ModeSelection.reviewlabel.Visible = false;
            }

            _ModeSelection.reviewlabel.Text = " Loading the file,please waiting...";
           

            this._TitleLabel.Text = string.Empty;
            this._CommentLabel.Text = string.Empty;

        }

        private string getFileName(string checkType)
        {
            int index = 0;
            string fullPath = _job.DataSourceInformation.DerivedFileInfo.FullName;
            int indexDot = fullPath.LastIndexOf('.');

            string fName = JobUtility.GetPurposedFilePath(checkType + index.ToString(), _job.DataSourceInformation.DerivedFileInfo);
            while (File.Exists(fName))
            {
                fName = JobUtility.GetPurposedFilePath(checkType + index.ToString(), _job.DataSourceInformation.DerivedFileInfo);
                index++;
            }
            return fName;
        }


        private void InputOutputMapper_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputOutputMapper, _InputFileChooser, delegate()
            {
                this._TitleLabel.Text = "Select Imported File ";
                this._CommentLabel.Text = "Please select the sample file to be imported.";
                return;
            });
            return;
        } // InputOutputMapper_Cancel()

        /************************************************************************
        ** ModeSelection
        **************************************************************************/
        private void ModeSelection_Accept(object sender, EventArgs e)
        {
            try
            {
                _structureDupRecords = _ModeSelection.STRUCTUREDUPRECORDS;
                _uniqueRecords = _ModeSelection.UNIQUERECORDS;
                _records = _ModeSelection.RECORDS;
                string[] action = _ModeSelection.Action.Split(',');

                if (action[1] == "Review")
                {
                    Accept(_ModeSelection, _DisplayInputData, delegate()
                    {
                        _DisplayInputData.JOB = _job;

                        DataTable results = new DataTable();

                        if (action[0] == "0")
                        {
                            if (_structureDupRecords.Count != 0)
                            {
                                Utils getTable = new Utils();
                                results = getTable.GenerateDisplayTable(_structureDupRecords, SourceFieldTypes.TypeDefinitions);
                                _DisplayInputData.InputData = _structureDupRecords;
                            }
                        }
                        else if (action[0] == "1")
                        {
                            if (_uniqueRecords.Count != 0)
                            {
                                Utils getTable = new Utils();
                                results = getTable.GenerateDisplayTable(_uniqueRecords, SourceFieldTypes.TypeDefinitions);
                                _DisplayInputData.InputData = _uniqueRecords;
                            }
                        }
                        else if (action[0] == "2")
                        {
                            if (_records.Count != 0)
                            {
                                Utils getTable = new Utils();
                                results = getTable.GenerateDisplayTable(_records, SourceFieldTypes.TypeDefinitions);
                                _DisplayInputData.InputData = _records;
                                                            }
                        }
                        if (results == null || results.Rows.Count == 0)
                        {
                            return true;
                        }
                        _DisplayInputData.Data = results;
                        this._TitleLabel.Text = "Duplicate Check Report ";
                        this._CommentLabel.Text = string.Empty;


                        return false;

                    });
                }
                else
                {
                    Accept(_ModeSelection, _ImportOption, delegate()
                    {
                        _ImportOption.JOB = _job;
                        _ImportOption.UserName = _InputFileChooser.UserName;
                        _ImportOption.Password = _InputFileChooser.Password;
                        _ImportOption.FullFilePath = _ModeSelection.FileName;
                        _ImportOption.XmlPath = _xmlPath;
                        _ImportOption.OptionPanelEnabled = true;
                        _ImportOption.AcceptButton.Text = "Next";
                        _ImportOption.ResultVisable = false;
                        _ImportOption.AuthorizeUser();
                        this._TitleLabel.Text = "Import Option ";
                        this._CommentLabel.Text = "Please select one option to be applied.";
                        this._PreControl = _ModeSelection;

                        if (!_files.Contains(_ModeSelection.FileName) &&
                        _ModeSelection.FileName != _job.DataSourceInformation.FullFilePath)
                        {
                            _files.Add(_ModeSelection.FileName);
                        }

                        return false;
                    });
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;

                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "ModeSelection_Accept_Exception");
                Trace.WriteLine(_ModeSelection.Action, "Action ");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            return;
        } // ModeSelection_Accept()

        private void ModeSelection_Cancel(object sender, EventArgs e)
        {
            Cancel(_ModeSelection, _InputOutputMapper, delegate()
            {
                this._TitleLabel.Text = "Field Mapping  ";
                this._CommentLabel.Text = "Please select the mapping fields.";

                return;
            });
            return;
        } // ModeSelection_Cancel()

        /************************************************************************
        ** DisplayInputData
        **************************************************************************/
        private void DisplayInputData_Accept(object sender, EventArgs e)
        {
            try
            {
                Accept(_DisplayInputData, _ImportOption, delegate()
                {
                    _ImportOption.JOB = _job;
                    _ImportOption.UserName = _InputFileChooser.UserName;
                    _ImportOption.Password = _InputFileChooser.Password;
                    _ImportOption.FullFilePath = _DisplayInputData.FileName;
                    _ImportOption.XmlPath = _xmlPath;
                    _ImportOption.OptionPanelEnabled = true;
                    _ImportOption.AcceptButton.Text = "Next";
                    _ImportOption.ResultVisable = false;
                    _ImportOption.AuthorizeUser();
                    this._TitleLabel.Text = "Import Option ";
                    this._CommentLabel.Text = "Please select one option to be applied.";
                    this._PreControl = _DisplayInputData;
                    if (!_files.Contains(_DisplayInputData.FileName) &&
                        _DisplayInputData.FileName != _job.DataSourceInformation.FullFilePath)
                    {
                        _files.Add(_DisplayInputData.FileName);
                    }

                    return false;
                });
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;

                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "DisplayInputData_Accept_Exception");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            return;
        } // DisplayInputData_Accept()

        private void DisplayInputData_Cancel(object sender, EventArgs e)
        {
            Cancel(_DisplayInputData, _ModeSelection, delegate()
            {
                this._TitleLabel.Text = string.Empty;
                this._CommentLabel.Text = string.Empty;
                return;
            });
            return;
        } // DisplayInputData_Cancel()

        /************************************************************************
        ** ImportOption
        **************************************************************************/
        private void ImportOption_Accept(object sender, EventArgs e)
        {
            if (_ImportOption.BackToFront == false)
            {
                try
                {
                    {
                        //Add Mapping File Path to Delete List
                        if (!_files.Contains(_ImportOption.CommandPath))
                        {
                            _files.Add(_ImportOption.CommandPath);
                        }
                        SuspendLayout();
                        Controls.Remove(_ImportOption); _ImportOption = null;
                        ResumeLayout(false);
                        PerformLayout();
                    }
                    Close();
                }
                catch (Exception ex)
                {
                    string message = ex.Message + "\n" + ex.StackTrace;

                    Trace.WriteLine(DateTime.Now, "Time ");
                    Trace.WriteLine(message, "ImportOption_Accept_Exception");
                    Trace.Flush();

                    MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_ImportOption.CommandPath))
                {
                    //Add Mapping File Path to Delete List
                    if (!_files.Contains(_ImportOption.CommandPath))
                    {
                        _files.Add(_ImportOption.CommandPath);
                    }
                }
                Cancel(_ImportOption, _InputFileChooser, delegate()
                {
                    this._TitleLabel.Text = "Select Imported File ";
                    this._CommentLabel.Text = "Please select the sample file to be imported.";
                    _InputOutputMapper = new InputOutputMapper();
                    _InputOutputMapper.Cancel += new EventHandler(InputOutputMapper_Cancel);
                    _InputOutputMapper.Accept += new EventHandler(InputOutputMapper_Accept);

                    return;
                });
            }

            return;
        } // ImportOption_Accept()

        private void ImportOption_Cancel(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_ImportOption.CommandPath))
            {
                //Add Mapping File Path to Delete List
                if (!_files.Contains(_ImportOption.CommandPath))
                {
                    _files.Add(_ImportOption.CommandPath);
                }
            }

            if (this._PreControl == _ModeSelection)
            {
                Cancel(_ImportOption, _ModeSelection, delegate()
                {
                    this._TitleLabel.Text = string.Empty;
                    this._CommentLabel.Text = string.Empty;
                    return;
                });
            }
            else if (this._PreControl == _InputOutputMapper)
            {
                Cancel(_ImportOption, _InputOutputMapper, delegate()
                {
                    this._TitleLabel.Text = "Field Mapping  ";
                    this._CommentLabel.Text = "Please select the mapping fields.";
                    return;
                });
            }
            else
            {
                Cancel(_ImportOption, _DisplayInputData, delegate()
                {
                    this._TitleLabel.Text = "Duplicate Check Report ";
                    this._CommentLabel.Text = string.Empty;
                    return;
                });
            }
            return;
        } // ImportOption_Cancel()

        private void NCDSBaseForm_Load(object sender, EventArgs e)
        {
            Layout += new LayoutEventHandler(DataLoader_Layout);

            this.Text = "NCDS DataLoader v" + Application.ProductVersion;

            Accept(null, _InputFileChooser, delegate()
            {
                return false;
            });
        } // NCDSBaseForm_Load()

        public static void SetWorkingSet(int maxWorkingSet)
        {
            System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
        }


        private void DataLoader_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if (_CurrentControl != null &&
                _CurrentControl is InputOutputMapper)
            {
                _CurrentControl.Size = _CurrentControl.MaximumSize = new Size(800, 503);
                _CurrentControl.Left = 10;
                _CurrentControl.Top = 80;
            }
            return;
        } // DataLoader_Layout()

        private void NCDSBaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string fileName = string.Empty;
            try
            {
                if (_ImportOption != null &&
                    !string.IsNullOrEmpty(_ImportOption.CommandPath))
                {
                    //Add Mapping File Path to Delete List
                    if (!_files.Contains(_ImportOption.CommandPath))
                    {
                        _files.Add(_ImportOption.CommandPath);
                    }
                }

                if (_files.Count > 0)
                {
                    for (int i = 0; i < _files.Count; i++)
                    {
                        fileName = _files[i];
                        if (File.Exists(_files[i]))
                        {
                            try
                            {
                                File.Delete(_files[i]);
                            }
                            catch (Exception ex)
                            {}
                        }
                    }
                }
                Trace.Close();

                if (File.Exists(_traceFilePath) &&
                    File.ReadAllBytes(_traceFilePath).Length == 0)
                {
                    File.Delete(_traceFilePath);
                }

                Clear();
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "InputFileChooser_Cancel_Exception");
                Trace.WriteLine(fileName, "Error File Name ");
                Trace.Flush();
                return;
            }
        } // NCDSBaseForm_FormClosed()

        #endregion


        #region METHOD
        /// <summary>
        /// Get the imput field xml.
        /// </summary>
        /// <param name="typeDefinitions">Stores field names and data-types for the current</param>
        ///<returns>imput field xml</returns>
        private string InputFieldSpec(Dictionary<string, Type> typeDefinitions)
        {
            COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();

            oCOEXmlTextWriter.WriteStartElement("fieldlist");

            foreach (KeyValuePair<string, Type> kvp in typeDefinitions)
            {
                //Exclude mapping options for fields with no discovered values
                oCOEXmlTextWriter.WriteStartElement("field");
                oCOEXmlTextWriter.WriteAttributeString("dbname", kvp.Key);

                if (kvp.Value == null)
                {
                    oCOEXmlTextWriter.WriteAttributeString("dbtype", string.Empty);
                }
                else
                {
                    oCOEXmlTextWriter.WriteAttributeString("dbtype", kvp.Value.ToString());
                }
                oCOEXmlTextWriter.WriteEndElement();
            } //
            oCOEXmlTextWriter.WriteEndElement();
            string InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
            oCOEXmlTextWriter.Close();

            InputFieldSpec = EditXml(InputFieldSpec);

            return InputFieldSpec;
        }

        /// <summary>
        /// Get the imput field xml.
        /// </summary>
        /// <param name="typeDefinitions">xml</param>
        ///<returns>imput field xml</returns>
        private string EditXml(string strXml)
        {
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(strXml);
            XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;
            COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
            oCOEXmlTextWriter.WriteStartElement("fieldlist");
            foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
            {
                // dbname
                string strDbname = oXmlNodeField.Attributes["dbname"].Value;
                string strDbtypePrefix = oXmlNodeField.Attributes["dbtype"].Value;
                bool bDbtypeReadonly = (oXmlNodeField.Attributes["dbtypereadonly"] != null) ? oXmlNodeField.Attributes["dbtypereadonly"].Value.ToLower() == "true" : false;
                string strDbtypeSuffix = string.Empty;
                {
                    int indexOf = strDbtypePrefix.IndexOf('[');
                    if ((indexOf > 0) && (strDbtypePrefix.Substring(indexOf, 2) != "[]"))
                    {
                        strDbtypeSuffix = strDbtypePrefix.Substring(indexOf);
                        strDbtypePrefix = strDbtypePrefix.Remove(indexOf);
                    }
                }
                string strName;
                {
                    XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["name"];
                    strName = (oXmlAttribute != null) ? oXmlAttribute.Value : strDbname;
                }
                string strTypePrefix = strDbtypePrefix;
                string strTypeSuffix = strDbtypeSuffix;
                {
                    XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["type"];
                    if (oXmlAttribute != null)
                    {
                        strTypePrefix = oXmlAttribute.Value;
                        {
                            int indexOf = strTypePrefix.IndexOf('[');
                            if ((indexOf > 0) && (strTypePrefix.Substring(indexOf, 2) != "[]"))
                            {
                                strTypeSuffix = strTypePrefix.Substring(indexOf);
                                strTypePrefix = strTypePrefix.Remove(indexOf);
                            }
                        }
                    }
                    else
                    {
                        MappingTypeDb eMappingType = new MappingTypeDb(strDbtypePrefix);
                        strTypePrefix = eMappingType.BasicTypeName;
                    }
                }

                oCOEXmlTextWriter.WriteStartElement("field");
                oCOEXmlTextWriter.WriteAttributeString("dbname", strDbname);
                if (strDbname == SDSourceRecord.MOLECULE_FIELD)
                {
                    strDbtypePrefix = "System.Byte[]";
                    strTypePrefix = "Binary";
                }

                oCOEXmlTextWriter.WriteAttributeString("dbtype", strDbtypePrefix);
                oCOEXmlTextWriter.WriteAttributeString("name", strDbname);
                oCOEXmlTextWriter.WriteAttributeString("type", strTypePrefix);
                oCOEXmlTextWriter.WriteEndElement();
            }
            oCOEXmlTextWriter.WriteEndElement();
            string InputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
            oCOEXmlTextWriter.Close();

            return InputFieldSpec;
        }

        private void Clear()
        {
            if (_job != null)
                _job = null;
            if (_records != null)
                _records = null;
            if (_structureDupRecords != null)
                _structureDupRecords = null;
            if (_uniqueRecords != null)
                _uniqueRecords = null;
            if (_PreControl != null)
                _PreControl = null;

            _job = new JobParameters();
            _DisplayInputData = new DisplayInputData();
            _ModeSelection = new ModeSelection();
            _ImportOption = new ImportOption();

            _DisplayInputData.Cancel += new EventHandler(DisplayInputData_Cancel);
            _DisplayInputData.Accept += new EventHandler(DisplayInputData_Accept);
            _ModeSelection.Cancel += new EventHandler(ModeSelection_Cancel);
            _ModeSelection.Accept += new EventHandler(ModeSelection_Accept);
            _ImportOption.Cancel += new EventHandler(ImportOption_Cancel);
            _ImportOption.Accept += new EventHandler(ImportOption_Accept);

        }

        /// <summary>
        /// Create a new JobParameters object.
        /// </summary>
        /// <param name="parsedArgs">arguments</param>
        /// <returns>an initialized JobParameters object to allow further processing</returns>
        public static JobParameters ProcessArguments(IndividualArgumentsCommandInfo parsedArgs)
        {
            // convert to internal object
            JobParameters job = JobCommandInfoConverter.ConvertToJobParameters(parsedArgs);
            return job;
        }

        /// <summary>
        /// <para>Code to execute when advancing to a UI step</para>
        /// <para>For use with <see cref="Accept"/></para>
        /// </summary>
        private delegate bool Accept_PrepareToActivate();

        /// <summary>
        /// Used to advance to the next UI step
        /// </summary>
        /// <param name="ControlBeingAccepted">Is the control for the step being accepted</param>
        /// <param name="ControlToActivate">Is the control for the step being activated</param>
        /// <param name="PrepareToActivate">Ecapsulates the transition code. See <see cref="Accept_PrepareToActivate"/></param>
        private void Accept(CambridgeSoft.NCDS_DataLoader.Controls.UIBase ControlBeingAccepted, CambridgeSoft.NCDS_DataLoader.Controls.UIBase ControlToActivate, Accept_PrepareToActivate PrepareToActivate)
        {
            if (_CurrentControl != ControlBeingAccepted)
            {
                throw new Exception("Developer error in UI step order");
            }
            if (PrepareToActivate() == false)
            {
                {
                    SuspendLayout();
                    if (_CurrentControl != null) Controls.Remove(_CurrentControl);
                    Controls.Add(ControlToActivate);
                    _CurrentControl = ControlToActivate;
                    _CurrentControl.BackColor = BackColor;
                    ResumeLayout(false);
                    PerformLayout();
                }
                if (ControlToActivate.Visible)
                {
                    Visible = true;
                    ControlToActivate.Select();
                    ControlToActivate.Focus();
                    AcceptButton = ControlToActivate.AcceptButton;
                    CancelButton = ControlToActivate.CancelButton;
                }
                else
                {
                    ControlToActivate.OnAccept();
                }
            }
            return;
        } // Accept()

        /// <summary>
        /// Used to cancel a UI step
        /// </summary>
        /// <param name="ControlBeingCancelled">Is the control for the step being cancelled</param>
        /// <param name="ControlToActivate">Is the control for the step being activated</param>
        /// <param name="ThingsToUndo">Ecapsulates the undo code. See <see cref="Cancel_ThingsToDo"/></param>
        /// <example>
        /// Example of Cancel usage:
        /// <code>
        /// Cancel(delegate()
        /// Cancel(_OutputConfiguration, _OutputTypeChooser, delegate()
        /// {
        ///     _JobSpec.OutputTypeSet(string.Empty);
        ///     return;
        /// });
        /// </code>
        /// </example>
        private void Cancel(UIBase ControlBeingCancelled, UIBase ControlToActivate, Cancel_ThingsToDo ThingsToUndo)
        {
            if (_CurrentControl != ControlBeingCancelled)
            {
                throw new Exception("Developer error in UI step order");
            }
            {
                SuspendLayout();
                Controls.Remove(ControlBeingCancelled); ControlBeingCancelled = null;
                Controls.Add(ControlToActivate);
                _CurrentControl = ControlToActivate;
                ResumeLayout(false);
                PerformLayout();
            }
            ThingsToUndo();
            if (ControlToActivate.Visible)
            {
                ControlToActivate.Select();
                ControlToActivate.Focus();
                AcceptButton = ControlToActivate.AcceptButton;
                CancelButton = ControlToActivate.CancelButton;
            }
            else
            {
                ControlToActivate.OnCancel();
            }
            return;
        } // Cancel()

        /// <summary>
        /// <para>Code to execute when cancelling a UI step</para>
        /// <para>For use with <see cref="Cancel"/></para>
        /// </summary>
        private delegate void Cancel_ThingsToDo();

        /// <summary>
        /// Authenticates the console user via COE security services.
        /// </summary>
        /// <param name="user">the 'user' or 'login' argument</param>
        /// <param name="password">the 'pwd' or 'password' argument</param>
        private static bool AuthenticateUser(string user, string password)
        {
            string msg = string.Empty;

            try
            {
                _authenticated = COEPrincipal.Login(user, password);
            }
            catch (Csla.DataPortalException cex)
            {
                msg = cex.BusinessException.GetBaseException().Message;
                MessageBox.Show(msg, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                msg = ex.GetBaseException().Message;
                MessageBox.Show(msg, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return _authenticated;
        }
        #endregion

    }
}
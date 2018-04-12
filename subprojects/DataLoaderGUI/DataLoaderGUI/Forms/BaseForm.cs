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
using CambridgeSoft.DataLoaderGUI.Controls;
using CambridgeSoft.DataLoaderGUI.Common;
using System.Diagnostics;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Threading;
using System.Data.OleDb;
using CambridgeSoft.DataLoaderGUI.Properties;

namespace CambridgeSoft.DataLoaderGUI.Forms
{
    public partial class BaseForm : Form
    {
        private Control _CurrentControl;
        private Control _PreControl;
        private InputFileChooser _InputFileChooser;
        private DisplayInputData _DisplayInputData;
        private InputOutputMapper _InputOutputMapper;
        //private ModeSelection _ModeSelection;
        //private ImportOption _ImportOption;
        private ImportAndDuplicateCheck _ImportAndDuplicateCheck;
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
        JobArgumentInfo jobArgs;
        //private string[] splitfiles = null;        

        //public Process[] ProDuplicate = null;
        // constructor

        /// <summary>
        /// <c>BaseForm</c> is the constructor for the main form.
        /// </summary>
        public BaseForm()
        {
            InitializeComponent();
            _job = new JobParameters();
            _InputFileChooser = new InputFileChooser();
            _DisplayInputData = new DisplayInputData();
            _InputOutputMapper = new InputOutputMapper();
            //_ModeSelection = new ModeSelection();
            //_ImportOption = new ImportOption();
            _ImportAndDuplicateCheck = new ImportAndDuplicateCheck();
            _InputFileChooser.Accept += new EventHandler(InputFileChooser_Accept);
            _InputFileChooser.Cancel += new EventHandler(InputFileChooser_Cancel);
            _DisplayInputData.Cancel += new EventHandler(DisplayInputData_Cancel);
            _DisplayInputData.Accept += new EventHandler(DisplayInputData_Accept);
            _InputOutputMapper.Cancel += new EventHandler(InputOutputMapper_Cancel);
            _InputOutputMapper.Accept += new EventHandler(InputOutputMapper_Accept);
            //_ModeSelection.Cancel += new EventHandler(ModeSelection_Cancel);
            //_ModeSelection.Accept += new EventHandler(ModeSelection_Accept);
            //_ImportOption.Cancel += new EventHandler(ImportOption_Cancel);
            //_ImportOption.Accept += new EventHandler(ImportOption_Accept);
            _ImportAndDuplicateCheck.Cancel += new EventHandler(ImportAndDuplicateCheck_Cancel);
            _ImportAndDuplicateCheck.Accept += new EventHandler(ImportAndDuplicateCheck_Accept);

            string appFolderLocation = COEConfigurationBO.ConfigurationBaseFilePath;
            string logOutPutPath = Path.Combine(appFolderLocation, @"TraceLogOutput\" + Resources.Message_Title);
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
            jobArgs = new JobArgumentInfo();
            Accept(_InputFileChooser, _InputOutputMapper, delegate()
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Make sure it at least exists
                    if (strInputFile.Trim().Length == 0)
                    {
                        MessageBox.Show("Please select the input file. ", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);        
                        return true;  // ERROR
                    }
                    // Make sure it at least exists
                    if (File.Exists(strInputFile) == false)
                    {
                        MessageBox.Show("File does not exist: '" + strInputFile + "'", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    // Any Input objects
                    List<string> listMatchingFilters = _InputFileChooser.MatchingFilters;
                    strExt = Path.GetExtension(strInputFile).ToLower();
                    if (listMatchingFilters.Count == 0)
                    {
                        MessageBox.Show("Unknown filename extension: " + strExt, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }
                    // Too many Input objects?
                    if (listMatchingFilters.Count > 1)
                    {
                        MessageBox.Show("Use the Browse button and choose the desired entry under file of type", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;  // ERROR
                    }

                    if (string.IsNullOrEmpty(_InputFileChooser.UserName) && _InputFileChooser.Split == false)
                    {
                        MessageBox.Show("Please input the user name.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }

                    if (string.IsNullOrEmpty(_InputFileChooser.Password) && _InputFileChooser.Split == false)
                    {
                        MessageBox.Show("Please input the password.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    if (_InputFileChooser.StartAt > _InputFileChooser.EndAt)
                    {
                        MessageBox.Show("Start record should not greater than end record.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            MessageBox.Show("Please select a worksheet.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }

                        if (string.IsNullOrEmpty(_InputFileChooser.Header))
                        {
                            MessageBox.Show("Please select the Header.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                        jobArgs.TableOrWorksheet = _InputFileChooser.WorkSheet;
                        jobArgs.HasHeader = _InputFileChooser.IsHeader;
                    }
                    else if (_InputFileChooser.GetFileType == SourceFileType.CSV)
                    {
                        if (string.IsNullOrEmpty(_InputFileChooser.Header))
                        {
                            MessageBox.Show("Please select the Header.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }

                        if (_InputFileChooser.Delimiter.Trim().Length == 0)
                        {
                            MessageBox.Show("Please input the Delimiter.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                        jobArgs.HasHeader = _InputFileChooser.IsHeader;
                        jobArgs.Delimiters = new string[] { _InputFileChooser.Delimiter };
                    }

                    jobArgs.DataFile = _InputFileChooser.InputFileName;
                    jobArgs.FileType = _InputFileChooser.GetFileType;
                    jobArgs.UserName = _InputFileChooser.UserName;
                    jobArgs.Password = _InputFileChooser.Password;
                    jobArgs.KnownRegNum = _InputFileChooser.KnownRegNum;                    

                    if (_InputFileChooser.Split == true)
                    {
                        using (SplitForm form = new SplitForm())
                        {
                            form.ShowDialog();
                            if (form.OK)
                            {
                                jobArgs.RangeEnd = Convert.ToInt32(form.GetNumber);
                                jobArgs.ActionType = TargetActionType.SplitFile;
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
                    if (!_InputFileChooser.Split && _InputFileChooser.StartAt > 0 && _InputFileChooser.EndAt >= _InputFileChooser.StartAt)
                    {
                        jobArgs.RangeBegin = _InputFileChooser.StartAt;
                        jobArgs.RangeEnd = _InputFileChooser.EndAt;
                    }
                    _job = JobArgumentInfo.ProcessArguments(jobArgs);
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalUrl"]))
                    {
                        _job.UserName = jobArgs.UserName;
                        _job.Password = jobArgs.Password;
                    }
                    string message = ReadInputFile.DoReadInputFile(_job);

                    if (!string.IsNullOrEmpty(message) && _InputFileChooser.Split != true)
                    {
                        MessageBox.Show(message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    else if (!string.IsNullOrEmpty(message) && _InputFileChooser.Split == true)
                    {
                        MessageBox.Show(message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }

                    IFileReader reader = _job.FileReader;
                    List<ISourceRecord> records = new List<ISourceRecord>();

                    reader.Rewind();

                    _InputField = string.Empty;
                    if (reader.CountAll() > 0)
                    {
                        _InputField = InputFieldSpec(SourceFieldTypes.TypeDefinitions);
                    }
                    //int intMaxInputRecordValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxInputRecordValue"]);

                    //if (reader.CountAll() > intMaxInputRecordValue)
                    //{
                    //    MessageBox.Show(string.Format("Please use a file with less than {0} records.", intMaxInputRecordValue), Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return true;  // ERROR
                    //}
                    //if (reader.CountAll() > 15000)
                    //{
                    //    MessageBox.Show("Can't load a sample file within more than 10,000 records.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return true;  // ERROR
                    //}

                    _recordsCount = reader.CountAll();
                    _InputOutputMapper.HaveStructure = _InputFileChooser.HaveStructure;
                    _InputOutputMapper.InputFieldSpec = _InputField;                    
                    _InputOutputMapper.Invalid = 0;
                    _InputOutputMapper.ParsedArgs = jobArgs;
                    _InputOutputMapper.SetForm();

                    this._TitleLabel.Text = "Field Mapping  ";
                    this._CommentLabel.Text = "Please select the mapping fields.";
                    return false;
                }
                catch (Exception ex)
                {
                    if (SourceFieldTypes.TypeDefinitions.Count == 0)
                    {
                        MessageBox.Show("Invalid delimiter.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string message = ex.Message + "\n" + ex.StackTrace;
                        Trace.WriteLine(DateTime.Now, "Time ");
                        Trace.WriteLine(message, "InputFileChooser_Accept_Exception");
                        Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.UserName) ? string.Empty : _InputFileChooser.UserName), "User Name ");
                        Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.InputFileName) ? string.Empty : _InputFileChooser.InputFileName), "File Name ");
                        Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.WorkSheet) ? string.Empty : _InputFileChooser.WorkSheet), "WorkSheet ");
                        Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.Delimiter) ? string.Empty : _InputFileChooser.Delimiter), "Delimiter ");
                        Trace.WriteLine((string.IsNullOrEmpty(_InputFileChooser.IsHeader.ToString()) ? string.Empty : _InputFileChooser.IsHeader.ToString()), "Header ");
                        Trace.WriteLine(_InputFileChooser.KnownRegNum, "Known RegNum ");
                        Trace.Flush();
                        MessageBox.Show("Unable to open the file.", Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            SuspendLayout();
            Controls.Remove(_InputFileChooser); _InputFileChooser = null;
            ResumeLayout(false);
            PerformLayout();
            Close();
            return;
        } // InputFileChooser_Cancel()
        
        /// <summary>
        /// Open the file, split its contents, and forward the arguments
        /// </summary>
        /// <param name="controlFilePath">path to the control file, containing the required parameters</param>
        /// <returns>an array of string arguments</returns>
        private static string[] UnpackControlFile(string controlFilePath)
        {
            string[] arguments = new string[] { };
            if (System.IO.File.Exists(controlFilePath))
                arguments = System.IO.File.ReadAllLines(controlFilePath);
            return arguments;
        }
        
        /************************************************************************
        ** InputOutputMapper
        **************************************************************************/
        private void InputOutputMapper_Accept(object sender, EventArgs e)
        {
            _xmlPath = _job.DataSourceInformation.FullFilePath + ".xml"; //JobUtility.getMappingFileName(_job.DataSourceInformation.FullFilePath);           

            //FileStream fs = new FileStream(_xmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(_xmlPath, false);
            _xml = _InputOutputMapper.Mapping;
            sw.WriteLine(_xml[1]);
            sw.Close();
            //fs.Close();

            if (!_files.Contains(_xmlPath))
            {
                _files.Add(_xmlPath);
            }

            Accept(_InputOutputMapper, _ImportAndDuplicateCheck, delegate()
            {
                _ImportAndDuplicateCheck.JOB = _job;
                _ImportAndDuplicateCheck.UserName = _InputFileChooser.UserName;
                _ImportAndDuplicateCheck.Password = _InputFileChooser.Password;
                _ImportAndDuplicateCheck.FullFilePath = _InputFileChooser.InputFileName;
                _ImportAndDuplicateCheck.StrFilePath = _InputFileChooser.InputFileName;
                _ImportAndDuplicateCheck.XmlPath = _xmlPath;
                _ImportAndDuplicateCheck._Xml = _InputOutputMapper.Mapping;
                _ImportAndDuplicateCheck.NoStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;
                _ImportAndDuplicateCheck.OptionPanelEnabled = true;
                _ImportAndDuplicateCheck.AcceptButton.Text = "Exit";
                _ImportAndDuplicateCheck.AcceptButton.Image = null;
                _ImportAndDuplicateCheck.AcceptButton.TextAlign = ContentAlignment.MiddleCenter;
                _ImportAndDuplicateCheck.Skipflag = true;
                _ImportAndDuplicateCheck.HaveStructure = _InputOutputMapper.HaveStructure;
                _ImportAndDuplicateCheck.AuthorizeUser();
                this._TitleLabel.Text = "Import and Duplicate Check";
                this._CommentLabel.Text = string.Empty;
                this._PreControl = _InputOutputMapper;

                return false;
            });
            return;
        } // InputOutputMapper_Accept() 

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
            //try
            //{
                //_structureDupRecords = _ModeSelection.STRUCTUREDUPRECORDS;
                //_uniqueRecords = _ModeSelection.UNIQUERECORDS;
                //_records = _ModeSelection.RECORDS;
                //string[] action = _ModeSelection.Action.Split(',');

                //if (action[1] == "Review")
                //{
                //    Accept(_ModeSelection, _DisplayInputData, delegate()
                //    {
                //        _DisplayInputData.JOB = _job;

                //        DataTable results = new DataTable();

                //        if (action[0] == "0")
                //        {
                //            if (_structureDupRecords.Count != 0)
                //            {
                //                Utils getTable = new Utils();
                //                results = getTable.GenerateDisplayTable(_structureDupRecords, SourceFieldTypes.TypeDefinitions);
                //                _DisplayInputData.InputData = _structureDupRecords;
                //            }
                //        }
                //        else if (action[0] == "1")
                //        {
                //            if (_uniqueRecords.Count != 0)
                //            {
                //                Utils getTable = new Utils();
                //                results = getTable.GenerateDisplayTable(_uniqueRecords, SourceFieldTypes.TypeDefinitions);
                //                _DisplayInputData.InputData = _uniqueRecords;
                //            }
                //        }
                //        else if (action[0] == "2")
                //        {
                //            if (_records.Count != 0)
                //            {
                //                Utils getTable = new Utils();
                //                results = getTable.GenerateDisplayTable(_records, SourceFieldTypes.TypeDefinitions);
                //                _DisplayInputData.InputData = _records;
                //            }
                //        }
                //        if (results == null || results.Rows.Count == 0)
                //        {
                //            return true;
                //        }
                //        _DisplayInputData.Data = results;
                //        this._TitleLabel.Text = "Duplicate Check Report ";
                //        this._CommentLabel.Text = string.Empty;


                //        return false;

                //    });
                //}
                //else
                //{
                //    Accept(_ModeSelection, _ImportOption, delegate()
                //    {
                //        _ImportOption.JOB = _job;
                //        _ImportOption.UserName = _InputFileChooser.UserName;
                //        _ImportOption.Password = _InputFileChooser.Password;
                //        _ImportOption.FullFilePath = _ModeSelection.FileName;
                //        _ImportOption.XmlPath = _xmlPath;
                //        _ImportOption.OptionPanelEnabled = true;
                //        _ImportOption.AcceptButton.Text = "Next";
                //        _ImportOption.ResultVisable = false;
                //        _ImportOption.AuthorizeUser();
                //        this._TitleLabel.Text = "Import Option ";
                //        this._CommentLabel.Text = "Please select one option to be applied.";
                //        this._PreControl = _ModeSelection;

                //        if (!_files.Contains(_ModeSelection.FileName) &&
                //        _ModeSelection.FileName != _job.DataSourceInformation.FullFilePath)
                //        {
                //            _files.Add(_ModeSelection.FileName);
                //        }

                //        return false;
                //    });
                //}
            //}
            //catch (Exception ex)
            //{
            //    string message = ex.Message + "\n" + ex.StackTrace;

            //    Trace.WriteLine(DateTime.Now, "Time ");
            //    Trace.WriteLine(message, "ModeSelection_Accept_Exception");
            //    Trace.WriteLine(_ModeSelection.Action, "Action ");
            //    Trace.Flush();
            //    MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            return;
        } // ModeSelection_Accept()

        private void ModeSelection_Cancel(object sender, EventArgs e)
        {
            //Cancel(_ModeSelection, _InputOutputMapper, delegate()
            //{
            //    this._TitleLabel.Text = "Field Mapping  ";
            //    this._CommentLabel.Text = "Please select the mapping fields.";

            //    return;
            //});
            return;
        } // ModeSelection_Cancel()

        /************************************************************************
        ** DisplayInputData
        **************************************************************************/
        private void DisplayInputData_Accept(object sender, EventArgs e)
        {
            try
            {
                Accept(_DisplayInputData, _ImportAndDuplicateCheck, delegate()
                {
                    _ImportAndDuplicateCheck.JOB = _job;
                    _ImportAndDuplicateCheck.UserName = _InputFileChooser.UserName;
                    _ImportAndDuplicateCheck.Password = _InputFileChooser.Password;
                    _ImportAndDuplicateCheck.FullFilePath = _InputFileChooser.InputFileName;
                    _ImportAndDuplicateCheck.StrFilePath = _InputFileChooser.InputFileName;
                    _ImportAndDuplicateCheck.XmlPath = _xmlPath;
                    _ImportAndDuplicateCheck._Xml = _InputOutputMapper.Mapping;
                    _ImportAndDuplicateCheck.NoStructureDupCheck = _InputOutputMapper.NoStructureDupCheck;
                    _ImportAndDuplicateCheck.OptionPanelEnabled = true;
                    _ImportAndDuplicateCheck.AcceptButton.Text = "Exit";
                    _ImportAndDuplicateCheck.AcceptButton.Image = null;
                    _ImportAndDuplicateCheck.AcceptButton.TextAlign = ContentAlignment.MiddleCenter;
                    _ImportAndDuplicateCheck.Skipflag = true;
                    _ImportAndDuplicateCheck.HaveStructure = _InputOutputMapper.HaveStructure;
                    _ImportAndDuplicateCheck.SelectedRecordsRadioButton.Enabled = true;
                    _ImportAndDuplicateCheck.SelectedRecordsRadioButton.Checked = true;
                    _ImportAndDuplicateCheck.AuthorizeUser();
                    this._TitleLabel.Text = "Import and Duplicate Check";
                    this._CommentLabel.Text = string.Empty;
                    this._PreControl = _InputOutputMapper;

                    return false;
                });
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;

                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "DisplayInputData_Accept_Exception");
                Trace.Flush();
                MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            return;
        } // DisplayInputData_Accept()

        private void DisplayInputData_Cancel(object sender, EventArgs e)
        {
            Cancel(_DisplayInputData, _ImportAndDuplicateCheck, delegate()
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
            //if (_ImportOption.BackToFront == false)
            //{
            //    try
            //    {
            //        SuspendLayout();
            //        Controls.Remove(_ImportOption); _ImportOption = null;
            //        ResumeLayout(false);
            //        PerformLayout();
            //        Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        string message = ex.Message + "\n" + ex.StackTrace;

            //        Trace.WriteLine(DateTime.Now, "Time ");
            //        Trace.WriteLine(message, "ImportOption_Accept_Exception");
            //        Trace.Flush();

            //        MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //}
            //else
            //{
            //    Cancel(_ImportOption, _InputFileChooser, delegate()
            //    {
            //        this._TitleLabel.Text = "Select Imported File ";
            //        this._CommentLabel.Text = "Please select the sample file to be imported.";
            //        _InputOutputMapper = new InputOutputMapper();
            //        _InputOutputMapper.Cancel += new EventHandler(InputOutputMapper_Cancel);
            //        _InputOutputMapper.Accept += new EventHandler(InputOutputMapper_Accept);

            //        return;
            //    });
            //}

            return;
        } // ImportOption_Accept()

        private void ImportOption_Cancel(object sender, EventArgs e)
        {
            //if (this._PreControl == _ModeSelection)
            //{
            //    Cancel(_ImportOption, _ModeSelection, delegate()
            //    {
            //        this._TitleLabel.Text = string.Empty;
            //        this._CommentLabel.Text = string.Empty;
            //        return;
            //    });
            //}
            //else if (this._PreControl == _InputOutputMapper)
            //{
            //    Cancel(_ImportOption, _InputOutputMapper, delegate()
            //    {
            //        this._TitleLabel.Text = "Field Mapping  ";
            //        this._CommentLabel.Text = "Please select the mapping fields.";
            //        return;
            //    });
            //}
            //else
            //{
            //    Cancel(_ImportOption, _DisplayInputData, delegate()
            //    {
            //        this._TitleLabel.Text = "Duplicate Check Report ";
            //        this._CommentLabel.Text = string.Empty;
            //        return;
            //    });
            //}
            return;
        } // ImportOption_Cancel()

        /************************************************************************
        ** Import and duplicate check
        **************************************************************************/
        private void ImportAndDuplicateCheck_Accept(object sender, EventArgs e)
        {
            if (_ImportAndDuplicateCheck.BackToFront == false)
            {
                try
                {
                    SuspendLayout();
                    Controls.Remove(_ImportAndDuplicateCheck); _ImportAndDuplicateCheck = null;
                    ResumeLayout(false);
                    PerformLayout();
                    Close();
                }
                catch (Exception ex)
                {
                    string message = ex.Message + "\n" + ex.StackTrace;

                    Trace.WriteLine(DateTime.Now, "Time ");
                    Trace.WriteLine(message, "_ImportAndDuplicateCheck_Accept_Exception");
                    Trace.Flush();

                    MessageBox.Show(ex.Message, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                Cancel(_ImportAndDuplicateCheck, _InputFileChooser, delegate()
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
        } // ImportAndDuplicateCheck_Accept()

        private void ImportAndDuplicateCheck_Cancel(object sender, EventArgs e)
        {
            if (this._PreControl == _InputOutputMapper)
            {
                Cancel(_ImportAndDuplicateCheck, _InputOutputMapper, delegate()
                {
                    this._TitleLabel.Text = "Field Mapping  ";
                    this._CommentLabel.Text = "Please select the mapping fields.";
                    return;
                });
            }
            return;
        } // ImportAndDuplicateCheck_Cancel()

        private void BaseForm_Load(object sender, EventArgs e)
        {
            Layout += new LayoutEventHandler(DataLoader_Layout);

            this.Text = Resources.Message_Title + " v" + Application.ProductVersion;

            Accept(null, _InputFileChooser, delegate()
            {
                return false;
            });
        } // BaseForm_Load()

        public static void SetWorkingSet(int maxWorkingSet)
        {
            System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
        }

        private void DataLoader_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if (_CurrentControl != null &&
                _CurrentControl is InputOutputMapper)
            {
                _CurrentControl.Size = _CurrentControl.MaximumSize = new Size(800, 400);
                _CurrentControl.Left = 10;
                _CurrentControl.Top = 60;
            }
            return;
        } // DataLoader_Layout()

        private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string fileName = string.Empty;
            try
            {
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
        } // BaseForm_FormClosed()

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
                List<string> structureHeaders = new List<string> { "STRUCTURE", "STRUCTURES", "SD_MOLECULE", "COMPOUND_MOLECULE" };
                // Similar list : ..\ChemOfficeEnterprise\DataLoaderGUI\DataLoaderGUI\Common\Utils.cs -- GenerateDisplayTable method
                // structure field header should be 'Structure' to view the chemical structure in view form.
                //Here we are supporting the other headers for automatic mapping.
                if (structureHeaders.Contains(strDbname.ToUpper()))                
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
            //_ModeSelection = new ModeSelection();
            //_ImportOption = new ImportOption();
            _ImportAndDuplicateCheck = new ImportAndDuplicateCheck();
            _DisplayInputData.Cancel += new EventHandler(DisplayInputData_Cancel);
            _DisplayInputData.Accept += new EventHandler(DisplayInputData_Accept);
            //_ModeSelection.Cancel += new EventHandler(ModeSelection_Cancel);
            //_ModeSelection.Accept += new EventHandler(ModeSelection_Accept);
            //_ImportOption.Cancel += new EventHandler(ImportOption_Cancel);
            //_ImportOption.Accept += new EventHandler(ImportOption_Accept);
            _ImportAndDuplicateCheck.Cancel += new EventHandler(ImportAndDuplicateCheck_Cancel);
            _ImportAndDuplicateCheck.Accept += new EventHandler(ImportAndDuplicateCheck_Accept);
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
        private void Accept(CambridgeSoft.DataLoaderGUI.Controls.UIBase ControlBeingAccepted, CambridgeSoft.DataLoaderGUI.Controls.UIBase ControlToActivate, Accept_PrepareToActivate PrepareToActivate)
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
            else
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
                MessageBox.Show(msg, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                msg = ex.GetBaseException().Message;
                MessageBox.Show(msg, Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return _authenticated;
        }
        #endregion
    }
}
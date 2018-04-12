using System;
using System.Data;
using System.IO;    // Logging
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms; // Logging
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Data.InputObjects;
using CambridgeSoft.COE.DataLoader.Data.OutputObjects;
using CambridgeSoft.COE.DataLoader.Windows.Common;

namespace CambridgeSoft.COE.DataLoader.Data
{
    /// <summary>
    /// Repository for the job information and responsible for job execution
    /// </summary>
    public class Job
    {
        #region data

        private Int32 _nJobStart;
        private Int32 _nJobCount;

        private readonly string _ApplicationFolder = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + Application.ProductName + @"\";
        private InputObject _oInput;
        private string _LogMessagePath;
        private string _xmlMappings;
        private OutputObject _oOutput;
        private string _strOutputType;
        private COEProgressHelper _oPh = new COEProgressHelper();
        private string _strUser;

        #endregion

        #region properties

        public string SavedJobFolder
        {
            get { return this._ApplicationFolder; }
        }

        /// <summary>
        /// Get the DataSet for preview and sort if necessary
        /// </summary>
        public DataSet DataSetForPreview
        {
            get
            {
                DataSet oDataSet = null;
                if (Input != null)
                {
                    if (Input.ExternalSortRequired)
                    {
                        DataSet oDataSetUnsorted = Input.DataSetForPreview;
                        string strOrderBy = Input.InputFieldSortSql;
                        DataRow[] oDataRows = oDataSetUnsorted.Tables[0].Select("true", strOrderBy);
                        oDataSet = oDataSetUnsorted.Clone();
                        foreach (DataRow oDataRow in oDataRows)
                        {
                            oDataSet.Tables[0].ImportRow(oDataRow);
                        }
                        oDataSetUnsorted.Clear();
                    }
                    else
                    {
                        oDataSet = Input.DataSetForPreview;
                    }
                }
                return oDataSet;
            }
        } // DataSetForPreview

        /// <summary>
        /// Get/set the input object
        /// </summary>
        public InputObject Input
        {
            private get
            {
                return _oInput;
            }
            set
            {
                _oInput = value;
                if (_oInput != null) _oInput.Ph = Ph;
                return;
            }
        } // Input

        /// <summary>
        /// Get/set the input configuration
        /// </summary>
        public string InputConfiguration
        {
            get
            {
                return (Input != null) ? Input.Configuration : null;
            }
            set
            {
                if (Input != null)
                {
                    string strInputConfiguration = InputConfiguration;
                    if (Input.Configuration != value)
                    {
                        Input.Configuration = value;
                        OnChanges(new JobChangesEventArgs("InputConfiguration"));
                    }
                    if (InputConfiguration == string.Empty) Input.Configuration = strInputConfiguration;   // Silently restore iif string.Empty
                }
                return;
            }
        } // InputConfiguration


        /// <summary>
        /// Get/set the input database
        /// </summary>
        public string InputDb
        {
            get { return Input.Db; }
            set
            {
                Input.Db = value;
                OnChanges(new JobChangesEventArgs("InputDb"));
            }
        } // InputDb

        /// <summary>
        /// Get the input message list (if any)
        /// </summary>
        public LogMessageList InputMessageList
        {
            get
            {
                return (Input != null) ? Input.MessageList : new LogMessageList();
            }
        } // InputMessageList

        /// <summary>
        /// Get/set the input field sort specification
        /// </summary>
        public string InputFieldSort
        {
            get
            {
                return (Input != null) ? Input.InputFieldSort : string.Empty;
            }
            set
            {
                if (Input != null)
                {
                    if (Input.InputFieldSort != value)
                    {
                        Input.InputFieldSort = value;
                        OnChanges(new JobChangesEventArgs("InputFieldSort"));
                    }
                }
                return;
            }
        } // InputFieldSort

        /// <summary>
        /// Get/set the input field specification
        /// </summary>
        public string InputFieldSpec
        {
            get
            {
                return (Input != null) ? Input.InputFieldSpec : string.Empty;
            }
            set
            {
                if (Input != null)
                {
                    if (Input.InputFieldSpec != value)
                    {
                        Input.InputFieldSpec = value;
                        OnChanges(new JobChangesEventArgs("InputFieldSpec"));
                    }
                }
                if (Output != null)
                {
                    Output.InputFieldSpec = value;
                }
                return;
            }
        } // InputFieldSpec

        /// <summary>
        /// Get/set the input filter last used to invoke the input object
        /// </summary>
        public string InputFilter
        {
            get
            {
                return (Input != null) ? Input.InputFilter : string.Empty;
            }
            set
            {
                if (Input != null)
                {
                    Input.InputFilter = value;
                }
                return;
            }
        } // InputFilter

        /// <summary>
        /// Get the number of input records
        /// </summary>
        public int InputRecords
        {
            get
            {
                return (Input != null) ? Input.Records : 0;
            }
        } // InputRecords

        /// <summary>
        /// Get whether the number of input records is approximate
        /// </summary>
        public bool InputRecordsApproximate
        {
            get
            {
                return (Input != null) ? Input.RecordsApproximate : true;
            }
        } // InputRecordsApproximate

        /// <summary>
        /// Get whether the number of input records is unknown
        /// </summary>
        public bool InputRecordsUnknown
        {
            get
            {
                return (Input != null) ? Input.RecordsUnknown : true;
            }
        } // InputRecordsUnknown

        /// <summary>
        /// Get/set the input table
        /// </summary>
        public string InputTable
        {
            get
            {
                return (Input != null) ? Input.Table : string.Empty;
            }
            set
            {
                if (Input != null)
                {
                    if (Input.Table != value)
                    {
                        Input.Table = value;
                    }
                    OnChanges(new JobChangesEventArgs("InputTable"));   // May be out of step due to internal processing
                }
                return;
            }
        } // InputTable

        /// <summary>
        /// Get the list og available input tables
        /// </summary>
        public List<string> InputTableList
        {
            get
            {
                return (Input != null) ? Input.TableList : null;
            }
        } // InputTableList

        /// <summary>
        /// Get/set the count of the number of records to be processed
        /// </summary>
        public Int32 JobCount
        {
            get
            {
                return _nJobCount;
            }
            set
            {
                if (_nJobCount != value)
                {
                    _nJobCount = value;
                    OnChanges(new JobChangesEventArgs("JobCount"));
                }
                return;
            }
        } // JobCount

        private Nullable<int> _jobChunkSize;
        /// <summary>
        /// The number of records to collect for batching the job activity.
        /// </summary>
        public Nullable<int> JobChunkSize
        {
            get
            {
                if (_jobChunkSize == null)
                {
                    string chunkSize = System.Configuration.ConfigurationManager.AppSettings["jobChunkSize"];
                    if (chunkSize == null)
                        _jobChunkSize = 5;
                    else
                        _jobChunkSize = Convert.ToInt32(chunkSize);
                }
                return _jobChunkSize;
            }
        }

        /// <summary>
        /// Get/set the starting record number for processing (1-relative)
        /// </summary>
        public Int32 JobStart
        {
            get
            {
                return _nJobStart;
            }
            set
            {
                if (_nJobStart != value)
                {
                    _nJobStart = value;
                    OnChanges(new JobChangesEventArgs("JobStart"));
                }
                return;
            }
        } // JobStart

        /// <summary>
        /// Get/set the path to the Job execution log file
        /// </summary>
        public string LogMessagePath
        {
            get
            {
                return _LogMessagePath;
            }
            protected set
            {
                _LogMessagePath = value;
                return;
            }
        } // LogMessagePath

        /// <summary>
        /// Get/set the output field mappings
        /// </summary>
        public string Mappings
        {
            get
            {
                return _xmlMappings;
            }
            set
            {
                if (_xmlMappings != value)
                {
                    _xmlMappings = value;
                    if (Output != null) Output.Mappings = Mappings;
                    if (Input != null) Input.Mappings = Mappings;
                    OnChanges(new JobChangesEventArgs("Mappings"));
                }
                return;
            }
        } // Mappings

        /// <summary>
        /// Get/set the output object
        /// </summary>
        public OutputObject Output
        {
            private get
            {
                return _oOutput;
            }
            set
            {
                _oOutput = value;
                if (Output != null)
                {
                    Output.Ph = Ph;
                    OutputType = Output.OutputType;
                }
                return;
            }
        } // Output

        /// <summary>
        /// Get/set the output configuration
        /// </summary>
        public string OutputConfiguration
        {
            get
            {
                return (Output != null) ? Output.Configuration : null;
            }
            set
            {
                if (Output != null)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        Output.Configuration = value;
                        OnChanges(new JobChangesEventArgs("OutputConfiguration"));
                    }
                }
                return;
            }
        } // OutputConfiguration

        /// <summary>
        /// Get/set the output database
        /// </summary>
        public string OutputDb
        {
            get
            {
                return (Output != null) ? Output.Db : null;
            }
            set
            {
                if (Output != null)
                {
                    if (Output.Db != value)
                    {
                        Output.Db = value;
                        OnChanges(new JobChangesEventArgs("OutputDb"));
                    }
                }
                return;
            }
        } // OutputDb

        /// <summary>
        /// Get the output field specification
        /// </summary>
        public string OutputFieldSpec
        {
            get
            {
                return (Output != null) ? Output.OutputFieldSpec : string.Empty;
            }
        } // OutputFieldSpec

        /// <summary>
        /// Get the output file filter
        /// </summary>
        public string OutputFilter
        {
            get
            {
                return (Output != null) ? Output.Filter : string.Empty;
            }
        } // OutputFilter

        /// <summary>
        /// Get the output fullname
        /// </summary>
        public string OutputToString
        {
            get
            {
                return (Output != null) ? Output.ToString() : string.Empty;
            }
        } // OutputToString

        /// <summary>
        /// Get/set the output type selected
        /// </summary>
        public string OutputType    // WJC needs work
        {
            get
            {
                return _strOutputType;
            }
            set
            {
                if (_strOutputType != value)
                {
                    _strOutputType = value;
                    OnChanges(new JobChangesEventArgs("OutputType"));   // Not persisted since f(Output)
                }
                return;
            }
        } // OutputType

        /// <summary>
        /// Get/set the ProgressHelper
        /// </summary>
        public COEProgressHelper Ph
        {
            get
            {
                return _oPh;
            }
            set
            {
                _oPh = value;
                return;
            }
        }

        /// <summary>
        /// Get/set the logged in user
        /// </summary>
        public string User  // WJC we need to persist the Password too (encrypted)
        {
            get
            {
                return _strUser;
            }
            set
            {
                if (_strUser != value)
                {
                    _strUser = value;
                    OnChanges(new JobChangesEventArgs("User"));
                }
                return;
            }
        }

        #endregion

        #region event declaration/definition
        /// <summary>
        /// Information about property changes for use by <see cref="Changes"/>
        /// </summary>
        public class JobChangesEventArgs : EventArgs
        {
            /// <summary>
            /// Semicolon separated list of properties that have changed
            /// </summary>
            public string properties;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="strProperties"></param>
            public JobChangesEventArgs(string strProperties)
            {
                properties = strProperties;
                return;
            }
        } // class JobChangesEventArgs

        /// <summary>
        /// Delegate for Changes event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void JobChangesEventHandler(object sender, JobChangesEventArgs e);

        /// <summary>
        /// The Changes event is raised when Job properties change
        /// </summary>
        public event JobChangesEventHandler Changes;
        private void OnChanges(JobChangesEventArgs e)
        {
            if (Changes != null)
            {
                Changes(this, e);
            }
            return;
        } // OnChanges()
        #endregion

        #region LogMessageList
        private LogMessageList _LogMessageList = new LogMessageList();
        /// <summary>
        /// Append a message to the message list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogMessage"></param>
        protected void AddMessage(LogMessage.LogSeverity veLogSeverity, LogMessage.LogSource veLogSource, int vnTransaction, string vstrLogMessage)
        {
            _LogMessageList.Add(veLogSeverity, veLogSource, vnTransaction, vstrLogMessage);
            return;
        } // AddMessage()

        /// <summary>
        /// Append a list of errors to the message list
        /// </summary>
        /// <param name="vlistLogMessageList"></param>
        protected void AddMessages(LogMessageList vlistLogMessageList)
        {
            _LogMessageList.Add(vlistLogMessageList);
            return;
        } // AddMessage()

        /// <summary>
        /// Clear the message list
        /// </summary>
        protected void ClearMessages()
        {
            _LogMessageList.Clear();
            return;
        } // ClearMessages()

        /// <summary>
        /// Close the log
        /// </summary>
        protected void CloseMessageLog()
        {
            _LogMessageList.Close();
            return;
        } // CloseMessageLog()

        /// <summary>
        /// Return true if there is anything in the message list
        /// </summary>
        public bool HasMessages
        {
            get
            {
                return (_LogMessageList.Count > 0);
            }
        } // HasMessages

        /// <summary>
        /// Get the message list (if any)
        /// </summary>
        public LogMessageList MessageList
        {
            get
            {
                return _LogMessageList;
            }
        } // MessageList

        /// <summary>
        /// Write accumulated errors to the log
        /// </summary>
        /// <param name="voStreamWriter"></param>
        protected void WriteMessageLog(StreamWriter voStreamWriter)
        {
            _LogMessageList.Save(voStreamWriter);
            return;
        } // WriteMessageLog()

        #endregion

        #region input methods
        /// <summary>
        /// Close the input database
        /// </summary>
        public void CloseInputDb()
        {
            if (Input != null)
            {
                Input.CloseDb();
                InputDb = Input.Db;
            }
            return;
        } // CloseInputDb()

        /// <summary>
        /// Load a Job that was previously Saved
        /// </summary>
        /// <param name="vXml"></param>
        public bool Load(string vXml)
        {
            ClearMessages();
            do
            { // while (false)
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(vXml);
                XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                {
                    XmlNode oXmlNodeOutput = oXmlNodeRoot.SelectSingleNode("Output");
                    string strProperty = oXmlNodeOutput.Attributes["property"].Value;
                    string strObject = oXmlNodeOutput.Attributes["object"].Value;
                    Type t = System.Type.GetType(strObject);
                    if (t == null)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Job, -1, "The output object is not available: " + strObject);
                        break;
                    }
                    {
                        Object obj = t.InvokeMember(
                            null,
                            System.Reflection.BindingFlags.DeclaredOnly |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance,
                            null,
                            null,
                            new Object[0]
                        );
                        Output = (OutputObject)obj;
                    }

                    if (Output.IsValid == false)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Job, -1, "The output object is not valid.");
                        AddMessages(Output.MessageList);
                        break;
                    }

                    {
                        XmlNode oXmlNodeOutputDb = oXmlNodeOutput.SelectSingleNode("OutputDb");
                        string strOutputDb = oXmlNodeOutputDb.InnerText.Trim();
                        OutputDb = strOutputDb;
                    }
                    {
                        XmlNode oXmlNodeOutputConfiguration = oXmlNodeOutput.SelectSingleNode("OutputConfiguration");
                        string strOutputConfiguration = oXmlNodeOutputConfiguration.InnerXml.Trim();
                        OutputConfiguration = strOutputConfiguration;
                    }
                    {
                        XmlNode oXmlNodeOutputFieldSpec = oXmlNodeOutput.SelectSingleNode("OutputFieldSpec");
                        if (oXmlNodeOutputFieldSpec != null)
                        {
                            string strOutputFieldSpec = oXmlNodeOutputFieldSpec.InnerXml.Trim();
                        }
                        //OutputFieldSpec = strOutputFieldSpec;
                    }
                } // Output
                {
                    XmlNode oXmlNodeInput = oXmlNodeRoot.SelectSingleNode("Input");
                    string strProperty = oXmlNodeInput.Attributes["property"].Value;
                    string strObject = oXmlNodeInput.Attributes["object"].Value;
                    Type t = System.Type.GetType(strObject);
                    if (t == null)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Job, -1, "The input object is not available: " + strObject);
                        break;
                    }
                    {
                        Object obj = t.InvokeMember(
                            null,
                            System.Reflection.BindingFlags.DeclaredOnly |
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance,
                            null,
                            null,
                            new Object[0]
                        );
                        Input = (InputObject)obj;
                    }
                    if (Input.IsValid == false)
                    {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Job, -1, "The input object is not valid.");
                        AddMessages(Input.MessageList);
                        break;
                    }

                    {
                        XmlNode oXmlNodeInputDb = oXmlNodeInput.SelectSingleNode("InputDb");
                        string strInputDb = oXmlNodeInputDb.InnerText.Trim();
                        InputDb = strInputDb;
                    }
                    {
                        XmlNode oXmlNodeInputConfiguration = oXmlNodeInput.SelectSingleNode("InputConfiguration");
                        string strInputConfiguration = oXmlNodeInputConfiguration.InnerXml.Trim();
                        InputConfiguration = strInputConfiguration;
                    }
                    {
                        XmlNode oXmlNodeInputTable = oXmlNodeInput.SelectSingleNode("InputTable");
                        string strInputTable = oXmlNodeInputTable.InnerText.Trim();
                        InputTable = strInputTable;
                    }
                    {
                        XmlNode oXmlNodeInputFieldSpec = oXmlNodeInput.SelectSingleNode("InputFieldSpec");
                        string strInputFieldSpec = oXmlNodeInputFieldSpec.InnerXml.Trim();
                        InputFieldSpec = strInputFieldSpec;
                    }
                } // Input
                {
                    XmlNode oXmlNodeInputOutput = oXmlNodeRoot.SelectSingleNode("inputoutput");
                    {
                        XmlNode oXmlNodeMappings = oXmlNodeInputOutput.SelectSingleNode("Mappings");
                        string strMappings = oXmlNodeMappings.InnerXml.Trim();
                        Mappings = strMappings;
                    }
                } // InputOutput
                {
                    XmlAttribute oXmlAttribute = oXmlNodeRoot.Attributes["JobStart"];
                    int nJobStart = (oXmlAttribute != null) ? Int32.Parse(oXmlAttribute.Value) : -1;
                    oXmlAttribute = oXmlNodeRoot.Attributes["JobCount"];
                    int nJobCount = (oXmlAttribute != null) ? Int32.Parse(oXmlAttribute.Value) : -1;
                    JobStart = nJobStart;
                    JobCount = nJobCount;
                } // Configuration
            } while (false);
            return HasMessages;
        } // Load()

        /// <summary>
        /// Open the input database
        /// </summary>
        /// <returns></returns>
        public bool OpenInputDb()
        {
            return (Input != null) ? Input.OpenDb() : true;
        } // OpenInputDb()

        /// <summary>
        /// Close an input table within the input database
        /// </summary>
        /// <returns></returns>
        public bool CloseInputTable()
        {
            bool bRet = true;
            if (Input != null)
            {
                bRet = Input.CloseTable();
                InputTable = Input.Table;
            }
            return bRet;
        } // CloseInputTable()

        /// <summary>
        /// Open an input table within the input database
        /// </summary>
        /// <returns></returns>
        public bool OpenInputTable()
        {
            bool bRet = true;
            if (Input != null)
            {
                Input.Table = InputTable;
                bRet = Input.OpenTable();
            }
            return bRet;
        } // OpenInputTable()
        #endregion

        #region job methods
        /// <summary>
        /// Execute the task as specified
        /// </summary>
        /// <returns></returns>
        public bool Execute()   // true on failure
        {
            ClearMessages();
            if (Directory.Exists(_ApplicationFolder) == false)
            {
                DirectoryInfo di = Directory.CreateDirectory(_ApplicationFolder);
            }
            StreamWriter oStreamWriter;
            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "Specification\t" + Save());
            DateTime dtStart = DateTime.Now;
            LogMessagePath = _ApplicationFolder + dtStart.ToString("yyyyMMddHHmmss") + ".log";
            oStreamWriter = File.CreateText(_LogMessagePath);
            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "Start\t" + dtStart.ToString("yyyy-MM-dd HH:mm:ss"));
            WriteMessageLog(oStreamWriter);
            Input.OpenDb();
            Input.OpenTable();
            Input.Mappings = Mappings; // Make sure they match in the case of a reexecute
            Output.Mappings = Mappings;
            int cErrors = 0;
            int cProcessed = 0;
            do
            {
                int cLimit = JobCount;  // int.MaxValue
                if (Input.OpenDataSet(JobStart - 1, cLimit))
                {
                    AddMessages(Input.MessageList);
                }
                // WJC TODO check for cancelled or Critical error
                int cChunk = (int)this.JobChunkSize;
                bool bFirstChunk = true;
                Ph.Minimum = Input.Minimum;
                Ph.Maximum = Input.Maximum;
                Ph.CancelConfirmation = "If you stop this operation not all records will be processed";
                Ph.ProgressSection(delegate() /* Job::Execute (Records?, Cancel) */
                {
                    while (cProcessed < cLimit)
                    {
                        if (Ph.IsRunning)
                        {
                            if (Ph.CancellationPending) break;
                            Ph.Value = Input.Value;
                            Ph.StatusText = "Processing..." + cProcessed.ToString() + ((cLimit != int.MaxValue) ? " of " + cLimit.ToString() : string.Empty) + " processed";
                        }
                        DataSet oDataSet = null;
                        if (Input.ExternalSortRequired) // Probably should be chunked on output side for progress/stop
                        {
                            cChunk = cLimit;
                            DataSet oDataSetUnsorted = null;
                            if (Input.ReadDataSet(cChunk, ref oDataSetUnsorted))
                            {
                                AddMessages(Input.MessageList);
                            }
                            if (oDataSetUnsorted == null)
                            {
                                AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Input, -1, "Error reading the input dataset");
                                break;
                            }
                            string strOrderBy = Input.InputFieldSortSql;
                            DataRow[] oDataRows = oDataSetUnsorted.Tables[0].Select("true", strOrderBy);
                            oDataSet = oDataSetUnsorted.Clone();
                            foreach (DataRow oDataRow in oDataRows)
                            {
                                oDataSet.Tables[0].ImportRow(oDataRow);
                            }
                            oDataSetUnsorted.Clear();
                        }
                        else
                        {
                            if (cChunk > (cLimit - cProcessed))
                            {
                                cChunk = (cLimit - cProcessed); // Partial last chunk
                            }
                            if (Input.ReadDataSet(cChunk, ref oDataSet))
                            {
                                AddMessages(Input.MessageList);
                            }
                            if (oDataSet == null)
                            {
                                AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Input, -1, "Error reading the input dataset");
                                break;
                            }
                        }
                        int cThis = oDataSet.Tables[0].Rows.Count;
                        if (cThis == 0)
                        {
                            break;  // OK. End of data
                        }
                        if (bFirstChunk)
                        {
                            bFirstChunk = false;
                            if (Output.StartWrite(oDataSet))
                            {
                                AddMessages(Output.MessageList);
                                break;
                            }
                        }
                        if (Output.DataSetWrite(oDataSet, (JobStart - 1) + cProcessed))
                        {
                            AddMessages(Output.MessageList);
                        }
                        if (HasMessages)
                        {
                            cErrors += MessageList.ErrorTransactions;
                            WriteMessageLog(oStreamWriter);
                        }
                        cProcessed += cThis;
                    } // while (cProcessed < cLimit)
                    Ph.Value = Input.Value;
                });
                Output.EndWrite();
                Input.CloseDataSet();
            } while (false);
            //CloseInputTable(); // No, we might reexecute
            //CloseInputDb(); // No, we might reexecute
            {
                DateTime dtEnd = DateTime.Now;
                AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "End\t" + dtEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                TimeSpan tsElapsed = (dtEnd - dtStart);
                AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "Elapsed\t" + tsElapsed.TotalSeconds.ToString());
            }
            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "Processed\t" + cProcessed.ToString());
            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Job, -1, "Errors\t" + cErrors.ToString());
            WriteMessageLog(oStreamWriter);
            oStreamWriter.Close();
            CloseMessageLog();
            return HasMessages;
        } // Execute()

        /// <summary>
        /// Save a job specification
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string xmlSave = string.Empty;
            if ((Input != null) && (Output != null))  // WJC should also check for valid
            {
                COEXmlTextWriter oCOEXmlTextWriter = oCOEXmlTextWriter = new COEXmlTextWriter();
                // open job
                oCOEXmlTextWriter.WriteStartElement("job");
                // job attributes
                oCOEXmlTextWriter.WriteStartAttribute("JobStart"); // JobStart
                oCOEXmlTextWriter.WriteString(JobStart.ToString());
                oCOEXmlTextWriter.WriteEndAttribute();
                oCOEXmlTextWriter.WriteStartAttribute("JobCount"); // JobCount
                oCOEXmlTextWriter.WriteString(JobCount.ToString());
                oCOEXmlTextWriter.WriteEndAttribute();
                {
                    // open principal
                    oCOEXmlTextWriter.WriteStartElement("User");
                    oCOEXmlTextWriter.WriteStartAttribute("property"); // User
                    oCOEXmlTextWriter.WriteString("User");
                    oCOEXmlTextWriter.WriteEndAttribute();
                    oCOEXmlTextWriter.WriteString(User);
                    // close principal
                    oCOEXmlTextWriter.WriteEndElement();

                    // open input
                    oCOEXmlTextWriter.WriteStartElement("Input");
                    oCOEXmlTextWriter.WriteStartAttribute("property"); // Input
                    oCOEXmlTextWriter.WriteString("Input");
                    oCOEXmlTextWriter.WriteEndAttribute();
                    oCOEXmlTextWriter.WriteStartAttribute("object");
                    oCOEXmlTextWriter.WriteString(Input.ToString());
                    oCOEXmlTextWriter.WriteEndAttribute();
                    {
                        // db
                        {
                            oCOEXmlTextWriter.WriteStartElement("InputDb");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // InputDb
                            oCOEXmlTextWriter.WriteString("InputDb");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteString(InputDb);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // configuration
                        {
                            oCOEXmlTextWriter.WriteStartElement("InputConfiguration");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // InputConfiguration
                            oCOEXmlTextWriter.WriteString("InputConfiguration");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(InputConfiguration);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // table
                        {
                            oCOEXmlTextWriter.WriteStartElement("InputTable");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // InputTable
                            oCOEXmlTextWriter.WriteString("InputTable");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteString(InputTable);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // field spec
                        {
                            oCOEXmlTextWriter.WriteStartElement("InputFieldSpec");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // InputFieldSpec
                            oCOEXmlTextWriter.WriteRaw("InputFieldSpec");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteStartAttribute("content");
                            oCOEXmlTextWriter.WriteString("xml");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(InputFieldSpec);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // field sort spec
                        {
                            oCOEXmlTextWriter.WriteStartElement("InputFieldSort");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // InputFieldSort
                            oCOEXmlTextWriter.WriteRaw("InputFieldSort");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteStartAttribute("content");
                            oCOEXmlTextWriter.WriteString("xml");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(InputFieldSort);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                    }
                    // close input
                    oCOEXmlTextWriter.WriteEndElement();

                    // open output
                    oCOEXmlTextWriter.WriteStartElement("Output");
                    oCOEXmlTextWriter.WriteStartAttribute("property"); // Output
                    oCOEXmlTextWriter.WriteString("Output");
                    oCOEXmlTextWriter.WriteEndAttribute();
                    oCOEXmlTextWriter.WriteStartAttribute("object");
                    oCOEXmlTextWriter.WriteString(Output.ToString());
                    oCOEXmlTextWriter.WriteEndAttribute();
                    {
                        // db
                        {
                            oCOEXmlTextWriter.WriteStartElement("OutputDb");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // OutputDb
                            oCOEXmlTextWriter.WriteString("OutputDb");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteString(OutputDb);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // configuration
                        {
                            oCOEXmlTextWriter.WriteStartElement("OutputConfiguration");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // OutputOutputConfiguration
                            oCOEXmlTextWriter.WriteRaw("OutputConfiguration");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteStartAttribute("content");
                            oCOEXmlTextWriter.WriteString("xml");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(OutputConfiguration);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        // field spec
                        {
                            oCOEXmlTextWriter.WriteStartElement("OutputFieldSpec");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // OutputFieldSpec
                            oCOEXmlTextWriter.WriteRaw("OutputFieldSpec");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteStartAttribute("content");
                            oCOEXmlTextWriter.WriteString("xml");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(OutputFieldSpec);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                    }
                    // close output
                    oCOEXmlTextWriter.WriteEndElement();

                    // open inputoutput
                    oCOEXmlTextWriter.WriteStartElement("inputoutput");
                    {
                        // mappings
                        {
                            oCOEXmlTextWriter.WriteStartElement("Mappings");
                            oCOEXmlTextWriter.WriteStartAttribute("property"); // Mappings
                            oCOEXmlTextWriter.WriteRaw("Mappings");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteStartAttribute("content");
                            oCOEXmlTextWriter.WriteString("xml");
                            oCOEXmlTextWriter.WriteEndAttribute();
                            oCOEXmlTextWriter.WriteRaw(Mappings);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                    }
                    // close inputoutput
                    oCOEXmlTextWriter.WriteEndElement();
                }
                // close job
                oCOEXmlTextWriter.WriteEndElement();

                string xmlUgly = oCOEXmlTextWriter.XmlString;
                oCOEXmlTextWriter.Close();
                xmlSave = COEXmlTextWriter.Pretty(xmlUgly); // OK Job::Save()
            } // if ((oInputObject != null) && (Output != null))
            return xmlSave;
        } // Save()
        #endregion

    } // class Job
}

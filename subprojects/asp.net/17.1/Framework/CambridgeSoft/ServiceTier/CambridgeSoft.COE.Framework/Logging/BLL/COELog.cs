using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Logging.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections;

namespace CambridgeSoft.COE.Framework.COELoggingService
{
    /// <summary>
    /// This is the main class for the logging service.
    /// To log within an application or framework. You  create a coelog object
    /// and then call it's Log methods where you want to log.  Logfiles are output to //ChemOfficeEnterprise[AssemblyVersion]/LogOutput/&lt;clientIdentifier&gt;/&lt;UserName/UserID&gt;/&lt;Session&gt;/coelog.log
    /// Example:
    /// COELog coeLog = COELog.GetSingleton("serviceIdentifier"));
    /// coeLog.LogStart() - sets the start of the logging with a message as an optional parameter
    /// coeLog.Log() - general purpose logging within start and end. Message is optional
    /// coeLog.LogEnd()- sets the start of the logging with a message as an optional parameter
    /// coeLog.GetLogFile - get the logfile for the client and bring it locally
    /// coeLog.GetLogFileAsDataTable - get the logfile that is converted to a datatable for dipslay in a grid
    /// coeLog.ClearLogFile - delete log files (only from username/userid downward
    /// </summary>
    /// 
    [Serializable]
    public class COELog
    {
        #region Variables
        [NonSerialized]
        COECallingClientLoggingConfig _clientLoggingConfig = null;

        //the coeLoggingSettings object using COELoggingConfigruation to populate a LoggingConfig which is serializalbe
        //try to pass loggingSettings directly cause serialation issues when running in 3 tier mode.
        [NonSerialized]
        COELoggingSettings coeLoggingSettings = new COELoggingSettings();

        string _category = null;
        private bool _enabled;
        private string _serverLogPath = string.Empty;
        string _specialFolder = string.Empty;
        string _userDirDirectory = string.Empty;
        #endregion

        #region Properties
        private static Stack _timerStack = null;
        private static MethodTimer _mainTimer = null;

        public static Stack TimerStack
        {
            get
            {
                if (_timerStack == null)
                    _timerStack = new Stack();

                return _timerStack;
            }
        }

        public bool Enabled
        {
            get { return _clientLoggingConfig == null ? false : _clientLoggingConfig.Enabled; }
            set
            {
                if (_clientLoggingConfig != null)
                    _clientLoggingConfig.Enabled = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        ///  Constructor for the COELog service. Checks to see if logging is enabled by checking the calling
        ///  clients app or web.config file.
        ///  here we will check the state of the csla global context variable. If it is empty we will look in the local configuration file
        ///  the app should have the following. The enterpriseLibrary.ConfigurationSource says to point to the framework
        ///  config file. the coeLoggingConfiguration says to enable and what categories and priorities to log
        ///  <configSections>
        ///      <section name="enterpriseLibrary.ConfigurationSource" type="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ConfigurationSourceSection, Microsoft.Practices.EnterpriseLibrary.Common, Version=2.0.0.0, Culture=neutral, PublicKeyToken=6579c5fdfc419d72" />
        ///      <section name="coeLoggingConfiguration" type="CambridgeSoft.COE.Framework.Common.COELoggingConfiguration, CambridgeSoft.COE.Framework, Version=12.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf" />
        ///  </configSections>
        ///  <enterpriseLibrary.ConfigurationSource selectedSource="File Configuration Source">
        ///      <sources>
        ///          <add name="File Configuration Source" type="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.FileConfigurationSource, Microsoft.Practices.EnterpriseLibrary.Common, Version=2.0.0.0, Culture=neutral, PublicKeyToken=6579c5fdfc419d72"
        ///              filePath="C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x\COEFrameworkConfig.xml" />
        ///      </sources>
        ///  </enterpriseLibrary.ConfigurationSource>
        ///  <coeLoggingConfiguration enableLogging="false" categories="ALL" level="10"/>
        /// the actually logging configuration is specified in the COEFrameworkConfig.xml on the application server
        /// </summary>
        private COELog(string serviceName)
        {
            _category = serviceName;
            // GetSettings(); //this files _clientLoggingConfig (done outside constructor to avoid recursion)


        }
        #endregion

        #region Singleton Pattern
        [NonSerialized]
        private static Dictionary<string, COELog> _coeLogs = null;

        public static COELog GetSingleton(string serviceName)
        {
            if (_coeLogs == null)
                _coeLogs = new Dictionary<string, COELog>();
            if (!_coeLogs.ContainsKey(serviceName))
            {
                _coeLogs.Add(serviceName, new COELog(serviceName));
            }
            if (_coeLogs[serviceName]._clientLoggingConfig == null)
            {
                _coeLogs[serviceName].GetSettings();
            }
            return _coeLogs[serviceName];
        }
        #endregion

        #region Methods

        private void GetSettings()
        {
            _clientLoggingConfig = coeLoggingSettings.GetSettings();
            _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath;
            _serverLogPath = _specialFolder + @"LogOutput\" + _clientLoggingConfig.LogEntryIdentifier;

        }

        /// <summary>
        /// This is main log method for the COELog service.  All other methods call this method.  
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message, int priority, SourceLevels severity)
        {
            if (Enabled)
            {
                //this will get the stack trace information to send through the portal. If you try to pass it any other way you end up needed the client on 
                //the applicatio server.
                IDictionary<string, object> contextInfo = (new Dictionary<string, object>());
                DebugInformationProvider provider = new DebugInformationProvider();
                provider.PopulateDictionary(contextInfo);
                StringBuilder msgBuilder = new StringBuilder(message);

                try
                {
                    object stackPeek = TimerStack.Peek();
                    if (stackPeek != null)
                    {
                        MethodTimer timer = (MethodTimer)stackPeek;
                        msgBuilder.Append("||| Time :: " + timer.GetTime());
                        msgBuilder.Append("||| TotalTime:: " + _mainTimer.GetTime());
                        msgBuilder.Append("||| Depth::" + TimerStack.Count.ToString());
                        msgBuilder.Append("||| Severity::" + severity.ToString());
                    }
                    else
                        msgBuilder.Append("||| Severity::" + severity.ToString());

                    string loggableMessage = msgBuilder.ToString();

                    WriteCommand result = DataPortal.Execute<WriteCommand>(
                        new WriteCommand(
                            _serverLogPath
                            , loggableMessage
                            , contextInfo
                            , _clientLoggingConfig
                            , _category
                            , priority
                            , severity.ToString()
                        )
                    );
                }
                catch
                {
                    /*JED: So, if we fail to be able to contact the server, we just ignore the whole thing? */
                    throw;
                }
            }
        }

        /// <summary>
        /// log method overload
        /// </summary>
        public void Log(string message, int priority)
        {
            this.Log(message, priority, SourceLevels.All);
        }

        /// <summary>
        /// log method overload
        /// </summary>
        public void Log(string message)
        {
            this.Log(message, 0, SourceLevels.All);
        }

        /// <summary>
        /// Prefix log with "START" and use message paramter
        /// </summary>
        public void LogStart(string message, int priority, SourceLevels severity)
        {
            MethodTimer timer = new MethodTimer();

            if (TimerStack.Count == 0)
            {
                _mainTimer = timer;
            }

            TimerStack.Push(timer);
            timer.Start();

            this.Log("START: " + message, priority, severity);
        }

        /// <summary>
        /// Prefix log with "START" and use message paramter
        /// </summary>
        public void LogStart(string message, int priority)
        {

            this.LogStart(message, priority, SourceLevels.All);
        }

        /// <summary>
        /// Prefix log with "START" and use message paramter
        /// </summary>
        public void LogStart(string message)
        {
            this.LogStart(message, 0, SourceLevels.All);
        }

        /// <summary>
        /// Prefix log with "END" and use message paramter
        /// </summary>
        public void LogEnd(string message, int priority, SourceLevels severity)
        {
            this.Log("END: " + message, priority, severity);

            MethodTimer timer = (MethodTimer)TimerStack.Pop();
            timer.Stop();
        }

        /// <summary>
        /// Prefix log with "END" and use message paramter
        /// </summary>
        public void LogEnd(string message, int priority)
        {

            this.LogEnd(message, priority, SourceLevels.All);
        }

        /// <summary>
        /// Prefix log with "END" and use message paramter
        /// </summary>
        public void LogEnd(string message)
        {
            this.LogEnd(message, 0, SourceLevels.All);
        }

        /// <summary>
        /// Clear the user/session log files
        /// </summary>
        public void ClearLogFile()
        {

            ClearLogFileCommand result;
            result = DataPortal.Execute<ClearLogFileCommand>(new ClearLogFileCommand(_serverLogPath));
            ;
        }

        /// <summary>
        /// Gets the log file for the user/session
        /// </summary>
        /// <param name="outputpath">path for outputting the local log file</param>
        public void GetLogFile(string outputpath)
        {
            if (Enabled)
            {

                try
                {

                    GetLogFileCommand result;
                    result = DataPortal.Execute<GetLogFileCommand>(new GetLogFileCommand(_serverLogPath));
                    if (outputpath == string.Empty)
                    {
                        outputpath = @"C:\Temp\" + _clientLoggingConfig.LogEntryIdentifier + "Framework.log";
                    }
                    string path = outputpath;
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    if (!File.Exists(path))
                    {

                        // Create a file to write to.
                        using (StreamWriter sw = File.CreateText(path))
                        {

                            sw.Write(result.ClientLog);


                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
        /// <summary>
        /// Gets the log file for the user/session as a DataTable
        /// </summary>
        public DataTable GetLogFileAsDataTable()
        {
            try
            {
                if (Enabled == true)
                {
                    GetLogFileAsDataTableCommand result;
                    result = DataPortal.Execute<GetLogFileAsDataTableCommand>(new GetLogFileAsDataTableCommand(_serverLogPath, false));
                    return result.ClientLogDataTable;
                }
                else
                {
                    return new DataTable("Empty");
                }
            }
            catch (Exception)
            {
                return new DataTable("Empty");
            }

        }
        /// <summary>
        /// Gets the log file for the user/session as a DataTable
        /// </summary>
        public static DataTable GetLogFileAsDataTable(string filePath)
        {
            GetLogFileAsDataTableCommand result;
            result = DataPortal.Execute<GetLogFileAsDataTableCommand>(new GetLogFileAsDataTableCommand(filePath, true));
            return result.ClientLogDataTable;
        }

        #endregion


        /// <summary>
        /// This is the writecommand that does all the work. It is run on the applicaiton server side
        /// </summary>
        [Serializable]
        private class WriteCommand : CommandBase
        {
            #region variables
            private string _category = "ALL";
            private string _severity = "ALL";
            private string _message;
            private string _elapsedTime;
            private int _priority = 0;
            private string _serverLogPath = String.Empty;
            private IDictionary<string, object> _contextInfo;
            private COECallingClientLoggingConfig _clientLoggingConfig;
            #endregion

            #region methods
            public WriteCommand(string serverLogPath, string message, IDictionary<string, object> contextInfo, COECallingClientLoggingConfig clientLoggingConfig, string category, int priority, string severity)
            {
                _message = message;

                /*string elapsedConstant = "Elapsed Time = ";
                if(message.Contains(elapsedConstant))
                {
                    int startIndex = message.IndexOf(elapsedConstant) + elapsedConstant.Length;
                    int endIndex = message.IndexOf(":", startIndex);

                    _elapsedTime = message.Substring(startIndex, endIndex - startIndex);
                }*/
                _contextInfo = contextInfo;
                _clientLoggingConfig = clientLoggingConfig;
                _category = category;
                _priority = priority;
                _serverLogPath = serverLogPath;
                _severity = severity;

            }

            protected override void DataPortal_Execute()
            {
                try
                {

                    //create a log entry with the passed in information
                    LogEntry logEntry = new LogEntry();
                    logEntry.Message = _message;
                    //log extended informatin when priority is 10
                    //this additionally adds an end log entry for parsing for creating
                    //a datatable. Otherwise parsing the contextInfo is very tedious since you have
                    //this may not be the best way to deal with this, but for now is
                    //serves the purpose
                    if (_clientLoggingConfig.Priority == 10)
                    {
                        _contextInfo.Add("|END", "LOGENTRY|");
                        logEntry.ExtendedProperties = _contextInfo;
                    }
                    else
                    {
                        //recreate the dictionary so that the stack informatio is wiped out since we are not asking for it
                        IDictionary<string, object> _contextInfo = new Dictionary<string, object>();
                        _contextInfo.Add("|END", "LOGENTRY|");
                        logEntry.ExtendedProperties = _contextInfo;
                    }

                    //only log categories and priorites that client specifies
                    if (_priority <= _clientLoggingConfig.Priority)
                    {
                        if (_clientLoggingConfig.Categories.IndexOf(",") > 0)
                        {
                            char[] delimiterChars = { ',' };
                            string[] catArray = _clientLoggingConfig.Categories.Split(delimiterChars);


                            for (int i = 0; i < catArray.Length; i++)
                            {
                                string catItem = catArray[i];
                                if (catItem == _category)
                                {
                                    logEntry.Categories.Add(_category);
                                    COELogWriter.WriteLogEntry(logEntry);
                                }
                            }
                        }
                        else
                        {
                            if ((_clientLoggingConfig.Categories == _category) || (_clientLoggingConfig.Categories.ToUpper() == "ALL"))
                            {
                                logEntry.Categories.Add(_category);
                                COELogWriter.WriteLogEntry(logEntry);
                            }
                        }
                    }
                }

                catch (Exception ex)
                {

                    throw;
                }
            }
            #endregion
        }

        /// <summary>
        /// DataPortal command for getting user/session log file 
        /// </summary>
        [Serializable]
        private class GetLogFileCommand : CommandBase
        {
            #region variables
            private string _clientLog = string.Empty;
            private string _serverLogPath = string.Empty;
            #endregion

            public GetLogFileCommand(string serverLogPath)
            {
                _serverLogPath = serverLogPath;

            }


            public string ClientLog
            {
                get { return _clientLog; }
                set { _clientLog = value; }
            }

            protected override void DataPortal_Execute()
            {
                try
                {


                    string sessionLogPath = _serverLogPath + @"\UserName_" + COEUser.Name.ToString() + "_UserID_" + COEUser.ID.ToString() + @"\SessionID_" + COEUser.SessionID.ToString() + @"\coelog.log";


                    if (File.Exists(sessionLogPath))
                    {
                        File.Copy(sessionLogPath, sessionLogPath + "Temp");
                        _clientLog = File.ReadAllText(sessionLogPath + "Temp");
                        File.Delete(sessionLogPath + "Temp");
                    }
                    else
                    {
                        _clientLog = "no entries found";
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

        }

        /// <summary>
        /// DataPortal command for getting user/session log file as a datatable
        /// </summary>
        [Serializable]
        private class GetLogFileAsDataTableCommand : CommandBase
        {
            private DataTable _clientLogDataTable = null;
            private string _serverLogPath = string.Empty;
            private string _filePath = string.Empty;

            public GetLogFileAsDataTableCommand(string serverLogPath)
            {
                _serverLogPath = serverLogPath;

            }

            public GetLogFileAsDataTableCommand(string filePath, bool isFullPath)
            {
                if (isFullPath == false)
                {
                    _serverLogPath = filePath;
                }
                else
                {
                    _filePath = filePath;
                }


            }


            public DataTable ClientLogDataTable
            {
                get { return _clientLogDataTable; }
                set { _clientLogDataTable = value; }
            }

            protected override void DataPortal_Execute()
            {
                StreamReader myReader = null;
                string sessionLogPath = string.Empty;
                try
                {
                    sessionLogPath = _filePath;

                    _clientLogDataTable = new DataTable("LogResults");
                    int recordIndex;
                    int fieldIndex;
                    string[] records;
                    string[] fields;
                    string[] recordSeparator = new string[] { "|END - LOGENTRY|" };
                    string[] fieldSeparator = new string[] { "|||" };
                    string[] fieldItemSeparator = new string[] { "::" };
                    if (File.Exists(sessionLogPath))
                    {

                        if (File.Exists(sessionLogPath + ".Temp"))
                        {
                            File.Delete(sessionLogPath + ".Temp");
                        }
                        //Open file and read all text.
                        File.Copy(sessionLogPath, sessionLogPath + ".Temp");
                        myReader = File.OpenText(sessionLogPath + ".Temp");
                        //Coverity Bug Fix 11563 
                        if (myReader != null)
                        {
                            records = myReader.ReadToEnd().Replace("\r\n", "").Split(recordSeparator, StringSplitOptions.RemoveEmptyEntries);
                            //Create data columns accordingly
                            if (records != null)
                            {
                                for (recordIndex = 0; recordIndex < records.Length; recordIndex++)
                                {
                                    DataRow myRow = _clientLogDataTable.NewRow();
                                    fields = records[recordIndex].ToString().Split(fieldSeparator, StringSplitOptions.None);
                                    //first get the column names:
                                    for (fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
                                    {
                                        string[] fieldItem = fields[fieldIndex].ToString().Split(fieldItemSeparator, StringSplitOptions.None);
                                        if (recordIndex == 0 && !string.IsNullOrEmpty(fieldItem[0]))
                                        {
                                            _clientLogDataTable.Columns.Add(new DataColumn(fieldItem[0].Trim(), typeof(System.String)));
                                        }

                                        if (fieldItem.Length >= 2 && !string.IsNullOrEmpty(fieldItem[1]))
                                        {
                                            string columnName = fieldItem[0].Trim();
                                            if (myRow.Table.Columns.Contains(columnName))
                                                myRow[columnName] = fieldItem[1].TrimEnd();
                                        }

                                    }
                                    _clientLogDataTable.Rows.Add(myRow);

                                }
                            }
                        }

                    }
                }
                finally
                {
                    if (myReader != null)
                        myReader.Close();
                }



            }
        }

        /// <summary>
        /// DataPortal command for clearing user/session log file 
        /// </summary>
        [Serializable]
        private class ClearLogFileCommand : CommandBase
        {

            private string[] _messages;
            private string _clientLog = string.Empty;
            private string _serverLogPath = string.Empty;


            public ClearLogFileCommand(string serverLogPath)
            {
                _serverLogPath = serverLogPath;

            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    string sessionLogPath = _serverLogPath + @"\UserName_" + COEUser.Name.ToString() + "_UserID_" + COEUser.ID.ToString() + @"\SessionID_" + COEUser.SessionID.ToString() + @"\";
                    string sessionDirDirectory = _serverLogPath + @"\UserName_" + COEUser.Name.ToString() + "_UserID_" + COEUser.ID.ToString() + @"\SessionID_" + COEUser.SessionID.ToString();
                    string userDirDirectory = _serverLogPath + @"\UserName_" + COEUser.Name.ToString() + "_UserID_" + COEUser.ID.ToString();
                    if (Directory.Exists(sessionLogPath))
                    {
                        //ljb: at time the delete isn't working in 2-tier mode. The files seem to be locked. I'll work this out.
                        string[] fileList = Directory.GetFiles(sessionLogPath, "*.*");
                        foreach (string fileName in fileList)
                        {
                            File.Delete(sessionLogPath + fileName);
                        }

                        Directory.Delete(sessionDirDirectory, true);
                        Directory.Delete(userDirDirectory, true);
                    }
                }


                catch (Exception ex)
                {

                    //  throw; //I don't see the need to throw an excetpion
                }
            }

        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;



namespace CambridgeSoft.COE.Framework.COELoggingService
{
    [Serializable]
    internal static class COELogWriter
    {
        private static LogWriter writer;              // Instance is created in static ctor which is thread safe
        const string EventLogSource = "Code Source";  // Event Log Source name
        const string ErrorCategory = "Errors";        // We have only one message category: Errors 
        
        static COELogWriter()
        {
            writer = CreateLogWriterFromCode();  // static ctor is thread safe according to ECMA standard section 9.5.3 
        }

        private static LogWriter CreateLogWriterFromCode()
        {
            // This is our message template for for the flatfile sink
            TextFormatter formatter = new TextFormatter("Timestamp:: {timestamp} |||" +
                                                        "Message:: {message} |||" +
                                                        "Category:: {category} |||" +
                                                        "Win32 Thread Id:: {win32ThreadId}|||" +
                                                        "Process Id:: {processId} |||" +
                                                        "Process Name:: {processName}|||" +
                                                        "Applicaton Server Machine:: {machine} |||" +
                                                        "Application Server Domain:: {appDomain} |||" +
                                                        "Extended Properties:: {dictionary({key} - {value})}");

            LogSource emptyTraceSource = new LogSource("none");
            //default sink for categories
            LogSource categoryTraceSource = new LogSource("Client", System.Diagnostics.SourceLevels.All);
            //default sink for errors
            LogSource errorsTraceSource = new LogSource(ErrorCategory, System.Diagnostics.SourceLevels.All);
            //string path = "C:\\TEMP\\coelog.log";
            string path = COEConfigurationBO.ConfigurationBaseFilePath + 
                    @"LogOutput\" + COEUser.ClientIdentifier + @"\UserName_" + COEUser.Name.ToString() + "_UserID_" + COEUser.ID.ToString() + @"\SessionID_" + COEUser.SessionID.ToString() +
                    @"\coelog" + 
                    "_" + DateTime.Now.Month  +
                    "_" + DateTime.Now.Day +
                    "_" + DateTime.Now.Year +
                    /*"_" + DateTime.Now.Hour +
                    "_" + DateTime.Now.Minute +
                    "_" + DateTime.Now.Second +*/
                    ".log";

            //bind category sources to flatfiletrace listiner
            categoryTraceSource.Listeners.Add(new FlatFileTraceListener(path, formatter));


            IDictionary<string, LogSource> traceSources = new Dictionary<string, LogSource>();

            //add all possible categories
            traceSources.Add("COESearch", categoryTraceSource);
            traceSources.Add("COEDatabasePublishing", categoryTraceSource);
            traceSources.Add("COEDataView", categoryTraceSource);
            traceSources.Add("COEExport", categoryTraceSource);
            traceSources.Add("COEForm", categoryTraceSource);
            traceSources.Add("COEGenericObjectStorage", categoryTraceSource);
            traceSources.Add("COEHitList", categoryTraceSource);
            traceSources.Add("COELogging", categoryTraceSource);
            traceSources.Add("COEPickListPicker", categoryTraceSource);
            traceSources.Add("COESearchCriteria", categoryTraceSource);
            traceSources.Add("COESecurity", categoryTraceSource);
            traceSources.Add("COETableEditor", categoryTraceSource);
            traceSources.Add("Errors", errorsTraceSource);

            //create logwriter will all information from above
            return new LogWriter(new ILogFilter[0], // ICollection<ILogFilter> filters
                               traceSources,        // IDictionary<string, LogSource> traceSources
                               categoryTraceSource, // LogSource allEventsTraceSource
                               categoryTraceSource,    // LogSource notProcessedTraceSource
                               errorsTraceSource,    // LogSource errorsTraceSource
                               "Client",        // string defaultCategory
                               true,                // bool tracingEnabled
                               false);                // bool logWarningsWhenNoCategoriesMatch
        }


        /// <summary>
        /// Write a log entry to a file with an message template specifed in code
        /// </summary>
        /// <param name="message">Error message to log</param>
        public static  void WriteLogEntry(LogEntry logEntry)
        {
            
             writer.Write(logEntry);
            
          
        }
    }
}

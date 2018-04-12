using System;
using System.IO;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Properties;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.Common;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;


namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class JobExecutor : MarshalByRefObject
    {
        //Fix for CSBR-161737-Log message needs to be changed for the action "importregdupascreatenew"
        public static JobParameters jobRecordsImported = null;
        public ProcessResult JobResult = new ProcessResult();
        public int ThreadIndex = 0;
        public static bool JobCancelled = false;
        public bool JobInterupped = false;

        /// <summary>
        /// For unattended loader session, we just walk through all the steps for an import action. But we can stop at any step,
        /// based on target action type.
        /// </summary>
        /// <param name="jobParameters">The job parameters instructing this unattended loader session.</param>
        public void DoUnattendedJob(JobParameters jobParameters)
        {
            try
            {
                //put this is
                // _consoleWriter = new ConsoleWriter();
                JobCancelled = false;
                JobInterupped = false;
                BeforeDataLoading(jobParameters);
                JobResult.ResetDefault();
                JobResult.ProcessComplete += AfterDataLoadedComplete;
                // ListTables action doesn't require job parameters normalization.
                if (jobParameters.TargetActionType == TargetActionType.ListTables)
                {
                    DoListTables(jobParameters);
                    JobResult.JobComplete = true;
                    return;
                }

                if (DoNormalizeJobParameters(jobParameters))
                {
                    // _consoleWriter.WriteLine(Resources.CmdVali_Valid);

                }
                else
                {
                    JobResult.JobComplete = true;
                    return;
                }

                if (jobParameters.TargetActionType == TargetActionType.ValidateMapping)
                {
                    JobResult.JobComplete = true;
                    return;
                }

                // The reader should have already been initialized during the normalization process.
                IFileReader reader = jobParameters.FileReader;

                DoCountRecords(jobParameters);
                if (jobParameters.TargetActionType == TargetActionType.CountRecords)
                {
                    JobResult.JobComplete = true;
                    return;
                }
                if (jobParameters.TargetActionType == TargetActionType.SplitFile)
                {
                    DoSplitFile(jobParameters);
                    JobResult.JobComplete = true;
                    return;
                }

                //_consoleWriter.WriteLine(String.Format("Determining field names and data-types for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name));
                //_consoleWriter.WriteLine();
                if (jobParameters.TargetActionType == TargetActionType.ListFields)
                {
                    // The field and type scan service has already been called in normalization service,
                    // so here we simply display the global type definition.
                    DisplayFields(SourceFieldTypes.TypeDefinitions);
                    JobResult.JobComplete = true;
                    return;
                }

                // Step 1: We split the source file into multiple chunks. Each output chunk doesn't contain the source records directly,
                // but instead contain the indices for those records. Then we process each chunk, one after another. After each
                // chunk finishes its processing, we dispose all memory allocated for that chunk and start over from Step 2.
                IndexRange originalRange = null;
                if (!jobParameters.ActionRanges.TryGetValue(0, out originalRange))
                    throw new Exception("Programmatic error: the data-range requested could not be extracted.");

                // If user didn't specify range, use 1 - <record count> by default.
                originalRange.RangeEnd = Math.Min(reader.RecordCount - 1, originalRange.RangeEnd);
                JobResult.TotalRecordsCount = (originalRange.RangeEnd - originalRange.RangeBegin) + 1;
                if (JobUtility.USE_THREADING)
                {

                    int threadsCount = Math.Min(JobUtility.NUMBER_OF_THREADS, reader.RecordCount);
                    JobResult.ThreadCount = threadsCount;
                    JobResult.ThreadMode = true;
                    IndexList originalList = originalRange.ToIndexList(originalRange.RangeBegin, originalRange.RangeEnd);
                    int recordProcessCount = originalList.ToIndexRanges(threadsCount).Count;
                    IndexRanges threadRanges = originalList.ToIndexRanges(recordProcessCount);
                    if (threadRanges.Count < threadsCount)
                        JobResult.ThreadCount = threadRanges.Count;
                    Thread[] thProcessJob = new Thread[threadRanges.Count];
                    for (int i = 0; i < threadRanges.Count; i++)
                    {
                        #region " Procesess Executor "

                        JobParameters jobParam = new JobParameters(jobParameters.DataSourceInformation, jobParameters.Mappings, jobParameters.ActionRanges);
                        jobParam.Mappings = jobParameters.Mappings;
                        jobParam.TargetActionType = jobParameters.TargetActionType;
                        jobParam.UserName = jobParameters.UserName;
                        jobParam.Password = jobParameters.Password;
                        IndexRange currentIndexRange = new IndexRange(threadRanges[i].RangeBegin, threadRanges[i].RangeEnd);
                        var domain = AppDomain.CreateDomain("OtherDomain" + i.ToString());
                        var obj = domain.CreateInstanceAndUnwrap(typeof(JobExecutor).Assembly.FullName, typeof(JobExecutor).FullName);
                        var jobexe = (JobExecutor)obj;
                        //JobExecutor jobexe = new JobExecutor();
                        jobexe.ThreadIndex = i;
                        string tracefielname = "";
                        string logfile = "";
                        if (!string.IsNullOrEmpty(TraceLog))
                            tracefielname = TraceLog.Replace(".trace", "_" + i.ToString() + ".trace");
                        if (Log != null)
                            logfile = Log.LogFilePath;
                        thProcessJob[i] = new Thread(unused => jobexe.ProcessJobExecutor(jobParam, currentIndexRange, JobResult, logfile, tracefielname));
                        thProcessJob[i].Start();

                        #endregion " Procesess Executor "
                    }
                    try
                    {
                        for (int th = 0; th < thProcessJob.Length; th++)
                        {
                            while (thProcessJob[th] != null && thProcessJob[th].IsAlive)
                            {
                                if (JobExecutor.JobCancelled)
                                    thProcessJob[th].Abort();
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    ProcessJobExecutor(jobParameters, originalRange, JobResult);
                    JobResult.JobComplete = true;
                }

                if (jobParameters.FileReader != null)
                    jobParameters.FileReader.Close();
                // AfterDataLoaded();
            }
            catch (Exception ex)
            {
                CambridgeSoft.COE.Framework.ExceptionHandling.COEExceptionDispatcher.HandleUIException(ex);
                Log.WriteToLogFile(ex.Message, true);
                JobInterupped = true;
                JobResult.JobComplete = true;
            }
        }

        #region fields and utility methods

        private static LogTable _log;
        private static string _traceLog;

        /// <summary>
        /// a LogTable object used to store information to be written in log file.
        /// </summary>
        public static LogTable Log
        {
            get { return _log; }
            set { _log = value; }
        }

        public static string TraceLog
        {
            get { return _traceLog; }
            set { _traceLog = value; }
        }

        private static void WriteTab()
        {
            // _consoleWriter.Write("\t");
        }

        #endregion

        #region top level workflow

        /// <summary>
        /// get original index range and table name,write log header.
        /// </summary>
        /// <param name="parameters"></param>
        private static void BeforeDataLoading(JobParameters parameters)
        {
            string logFilePath = JobUtility.GetLogFilePath();
            TargetActionType targetActionType = parameters.TargetActionType;
            if (targetActionType == TargetActionType.ValidateMapping ||
                targetActionType == TargetActionType.ValidateData ||
                targetActionType == TargetActionType.FindDuplicates ||
                targetActionType == TargetActionType.ImportRegDupAsCreateNew ||
                targetActionType == TargetActionType.ImportRegDupAsNewBatch ||
                targetActionType == TargetActionType.ImportRegDupAsTemp ||
                targetActionType == TargetActionType.ImportRegDupNone ||
                targetActionType == TargetActionType.ImportTemp)    //Log only for these target action type
            {
                Log = new LogTable(parameters, logFilePath);
                Log.WriteHeader();
            }

            //Trace
            Trace.Listeners.Clear();

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["EnableTrace"]) &&
                System.Configuration.ConfigurationManager.AppSettings["EnableTrace"].ToLower() == "true")
            {
                string logFileName = Path.GetFileName(logFilePath);
                int firstDot = logFileName.IndexOf('.');
                int lastDot = logFileName.LastIndexOf('.');
                string traceLogFileName = logFileName.Remove(firstDot, lastDot - firstDot).Insert(firstDot, ".trace");
                string traceLogFilePath = Path.Combine("PerformanceTrace", traceLogFileName);
                TraceLog = traceLogFilePath;
                Directory.CreateDirectory(Path.GetDirectoryName(traceLogFilePath));//create directory if not exists
                StreamWriter trace = new StreamWriter(traceLogFilePath, false);
                Trace.Listeners.Add(new TextWriterTraceListener(trace));
            }
        }

        public void SetTraceLog(string tracelogFile)
        {
            //if (Trace.Listeners.Count == 0)
            {
                StreamWriter trace = new StreamWriter(tracelogFile, false);
                Trace.Listeners.Add(new TextWriterTraceListener(trace));
            }
        }

        public string GetLogFile()
        {
            return Log.LogFilePath;
        }

        /// <summary>
        /// Write log summary
        /// </summary>
        private static void AfterDataLoaded()
        {
            if (Log == null) return;
            Log.WriteSummary();

            //open the log file using default application

            //NOTE: 'Silent' execution allows better *unattended* process control
            //System.Diagnostics.Process.Start(Log.LogFilePath);
            //_consoleWriter.WriteLine(string.Format("Please find log output at: '{0}'", Log.LogFilePath));

            //Trace.Close();
        }

        private void AfterDataLoadedComplete(object sender, EventArgs e)
        {
            if (Log == null) return;
            // Log.WriteSummary();
            Log.WriteToLogFile(Resources.Log_Seperator, true);
            Log.WriteToLogFile(JobResult.LogSummary, true);
            JobResult.OnShowOutput(JobResult.LogSummary);
            //open the log file using default application

            //NOTE: 'Silent' execution allows better *unattended* process control
            //System.Diagnostics.Process.Start(Log.LogFilePath);
            //_consoleWriter.WriteLine(string.Format("Please find log output at: '{0}'", Log.LogFilePath));
            JobResult.OnShowOutput(string.Format("Please find log output at: '{0}'", Log.LogFilePath));
            Trace.Close();
        }


        /// <summary>
        /// console output,create new log entry.
        /// </summary>
        /// <param name="range"></param>
        private static void BeforeRecordsChunkProcessing(IndexRange range)
        {
            string message = string.Format(Resources.BeforeRecordsChunkProcessing, range.RangeBegin + 1, range.RangeEnd + 1);
            //_consoleWriter.WriteLine(message);
            //_consoleWriter.WriteLine();
            // if (!JobUtility.USE_THREADING)
            {
                if (Log == null) return;
                Log.Clear();//clear the log entries that has been written to log file in previous chunk processing.
                for (int index = range.RangeBegin + 1; index <= range.RangeEnd + 1; index++)
                {
                    LogEntry entry = new LogEntry();
                    entry.Status = RecordStatus.Ready;
                    Log.Add(index, entry);
                }
            }
        }

        /// <summary>
        /// console output,write log information of the current chunk
        /// </summary>
        /// <param name="range"></param>
        private static void AfterRecordsChunkProcessed(IndexRange range)
        {
            /// _consoleWriter.WriteLine();
            string message = string.Format(Resources.AfterRecordsChunkProcessing, range.RangeBegin + 1, range.RangeEnd + 1);
            // _consoleWriter.WriteLine(message);

            if (Log == null) return;
            //update status before written to log
            foreach (LogEntry entry in Log.Values)
            {
                RecordStatus defaultStatus = Log.DefaultStatus;
                if (defaultStatus == RecordStatus.UnImported)//supposed to import data into database
                {
                    switch (entry.Status)
                    {
                        case RecordStatus.UnMapped:
                            entry.Status = defaultStatus;
                            entry.Reason = string.Format(Resources.Log_NotMappedReason, entry.Reason);
                            break;
                        case RecordStatus.Invalid:
                            entry.Status = defaultStatus;
                            entry.Reason = string.Format(Resources.Log_InvalidReason, entry.Reason);
                            break;
                        case RecordStatus.Matched:
                            entry.Status = defaultStatus;
                            break;
                    }
                }
                else if (entry.Status == RecordStatus.Ready)
                {
                    entry.Status = defaultStatus;
                }
            }
            Log.WriteDetails();
        }

        #endregion

        #region event handlers for each job service

        /// <summary>Console output</summary>
        /// <remarks>
        /// before parsing a source record
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordParsing(object sender, RecordProcessingEventArgs e)
        {
            //_consoleWriter.CursorLeft(0);
            WriteTab();
            // _consoleWriter.Write(Resources.RecordParsing, e.SourceRecord.SourceIndex);
        }
        /// <summary>console output, and optionally stores unextracted records' information for log </summary>
        /// <remarks>
        /// A list source records are extracted from source file.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsExtracted(object sender, EventArgs e)
        {
            RecordsProcessedEventArgs args = e as RecordsProcessedEventArgs;
            ///_consoleWriter.WriteLine();
            WriteTab();
            // _consoleWriter.Write(Resources.RecordsExtracted, args.Count);
            // _consoleWriter.WriteLine();

            if (args.SourceRecords != null && args.SourceRecords.Count > 0)
                Log.AddFailedRecords(RecordStatus.UnExtracted, args.SourceRecords);
        }

        /// <summary>Console output</summary>
        /// <remarks>
        /// before mapping a source record
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordMapping(object sender, RecordProcessingEventArgs e)
        {
            //_consoleWriter.CursorLeft(0);
            WriteTab();
            //_consoleWriter.Write(Resources.RecordMapping, e.SourceRecord.SourceIndex);
        }

        ///<summary>console output,and optionally stores unmapped records' iformation for log</summary>
        /// <remarks>
        /// a list of source records are mapped
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsMapped(object sender, EventArgs e)
        {
            ////_consoleWriter.WriteLine();
            RecordsProcessedEventArgs args = (RecordsProcessedEventArgs)e;
            WriteTab();
            // _consoleWriter.WriteLine(Resources.RecordsMapped, args.Count);
            // _consoleWriter.WriteLine();

            if (args.SourceRecords != null && args.SourceRecords.Count > 0)
                Log.AddFailedRecords(RecordStatus.UnMapped, args.SourceRecords);
        }

        /// <summary>Console output</summary>
        /// <remarks>
        /// before validating a destination record
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordValidating(object sender, RecordProcessingEventArgs e)
        {
            // _consoleWriter.CursorLeft(0);
            WriteTab();
            // _consoleWriter.Write(Resources.RecordValidating, e.SourceRecord.SourceIndex);
        }

        /// <summary>console output, and optionally stores invalid records' information for log</summary>
        /// <remarks>
        /// a list destination records are validated
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsValidated(object sender, EventArgs e)
        {
            RecordsProcessedEventArgs args = (RecordsProcessedEventArgs)e;
            //_consoleWriter.WriteLine();
            WriteTab();

            //_consoleWriter.WriteLine(Resources.RecordsValidated, args.Count - args.SourceRecords.Count, args.SourceRecords.Count);
            //_consoleWriter.WriteLine();

            if (args.SourceRecords != null && args.SourceRecords.Count > 0)
                Log.AddFailedRecords(RecordStatus.Invalid, args.SourceRecords);
        }

        /// <summary>console output, and optionally stores duplicated records' information for log</summary>
        /// <remarks>
        /// a list destination records has been duplicate checked
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsDupChecked(object sender, EventArgs e)
        {
            RecordsProcessedEventArgs args = (RecordsProcessedEventArgs)e;
            WriteTab();
            //// _consoleWriter.WriteLine(Resources.RecordsDupChecked, args.Count - args.SourceRecords.Count, args.SourceRecords.Count);
            // _consoleWriter.WriteLine();

            if (args.SourceRecords != null && args.SourceRecords.Count > 0)
                Log.AddFailedRecords(RecordStatus.Matched, args.SourceRecords);
        }

        /// <summary>Console output</summary>
        /// <remarks>
        /// before importing a destination record into database
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordImporting(object sender, RecordProcessingEventArgs e)
        {
            //_consoleWriter.CursorLeft(0);
            WriteTab();
            //_consoleWriter.Write(Resources.RecordImporting, e.SourceRecord.SourceIndex);
        }

        /// <summary>console output,and optionally stores unimported records' information for log</summary>
        /// <remarks>
        /// a list of destination record has been imported into database
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsImported(object sender, EventArgs e)
        {
            // _consoleWriter.WriteLine();
            RecordsImportedEventArgs args = (RecordsImportedEventArgs)e;
            WriteTab();
            //if (!JobUtility.USE_THREADING)
            {
                //Fix for CSBR-161737-Log message needs to be changed for the action "importregdupascreatenew"
                JobParameters jobImportReg = null;
                jobImportReg = (JobParameters)jobRecordsImported;
                //update log entries
                //temporal, permanent, as well as not imported records
                Dictionary<ISourceRecord, string> notImportedRecords = new Dictionary<ISourceRecord, string>();
                foreach (KeyValuePair<ISourceRecord, RegRecordSummaryInfo> info in args.RecordsSummaryInfo)
                {
                    int index = info.Key.SourceIndex;
                    string regNum = info.Value.RegNum;
                    string tempNum = info.Value.TempNum;
                    
                    if (string.IsNullOrEmpty(regNum) == false &&
                        regNum != "null" && regNum != "0")//permanent records
                    {
                        Log[index].Status = RecordStatus.Registered;
                        //Fix for CSBR-161737-Log message needs to be changed for the action "importregdupascreatenew"
                        if (jobImportReg.TargetActionType.ToString() == "ImportRegDupAsCreateNew")
                            Log[index].Reason = string.Format(Resources.Log_RegisterdRecordNew, regNum, info.Value.BatchCount);
                        else
                            Log[index].Reason = string.Format(Resources.Log_RegisterdRecord, regNum, info.Value.BatchCount);
                        Log.PermanentRecordsCount++;
                    }
                    else if (string.IsNullOrEmpty(tempNum) == false &&
                            tempNum != "0")//temporal records
                    {
                        Log[index].Status = RecordStatus.AddedToPendingReview;
                        Log[index].Reason = string.Format(Resources.Log_TemporalRecord, tempNum);
                        Log.TemporalRecordsCount++;
                    }
                    else//cannot be imported because duplicates found
                    {
                        //TODO:update the message as RegNum of matched records
                        notImportedRecords.Add(info.Key, info.Value.Message);
                        Log.AddFailedRecords(RecordStatus.Matched, notImportedRecords);
                    }

                    if (info.Value.IsRedBoxWarningExists)   // Jira ID - CBOE-1158
                        Log[index].Reason += Environment.NewLine + info.Value.RedBoxWarningMessage;
                   
                }

                //_consoleWriter.WriteLine(Resources.RecordsImported, Log.PermanentRecordsCount + Log.TemporalRecordsCount);
            }


        }

        #endregion

        #region Sub classes

        /// <summary>
        /// Stores all information needs to by log file.
        /// </summary>
        /// <remarks>
        /// Each item in the dictionary stores information of a source record,of which the source record index is stored as key,
        /// other information are stored in corresponding LogEntry.
        /// It also maintain the information needed to write the summary in log file.
        /// </remarks>
        [Serializable]
        public class LogTable : Dictionary<int, LogEntry>
        {
            private Dictionary<RecordStatus, IndexList> _failedRecords;
            public IndexRange _originalRange = null;
            private TargetActionType _targetActionType;
            private RecordStatus _defaultStatus = RecordStatus.Ready;
            private string _tableName = null;
            private string _logFilePath = null;
            private int _temporalRecordsCount;
            private int _permanentRecordsCount;
            public static string LOG_HEADER = string.Format("{0},{1},{2}", Resources.LogCol_SourceRecordIndex, Resources.LogCol_Status, Resources.LogCol_Reason);

            /// <summary>
            /// Get original IndexRange,and table name if there is.
            /// </summary>
            /// <param name="parameters"></param>
            public LogTable(JobParameters parameters, string logFilePath)
            {
                parameters.ActionRanges.TryGetValue(0, out _originalRange);
                _targetActionType = parameters.TargetActionType;
                _tableName = parameters.DataSourceInformation.TableName;
                _logFilePath = logFilePath;
            }

            public int TemporalRecordsCount
            {
                get { return _temporalRecordsCount; }
                set { _temporalRecordsCount = value; }
            }

            public int PermanentRecordsCount
            {
                get { return _permanentRecordsCount; }
                set { _permanentRecordsCount = value; }
            }


            /// <summary>
            /// Keep a list source record index for each kind of failure process(unextracted,unmapped,invalid,duplciated,unimported),
            /// this will be used in summary.
            /// </summary>
            public Dictionary<RecordStatus, IndexList> FailedRecords
            {
                get
                {
                    if (_failedRecords == null)
                        _failedRecords = new Dictionary<RecordStatus, IndexList>();
                    return _failedRecords;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public RecordStatus DefaultStatus
            {
                get
                {
                    if (_defaultStatus == RecordStatus.Ready)
                    {
                        switch (_targetActionType)//choose default Status value for different target action.
                        {
                            case TargetActionType.ValidateMapping:
                                _defaultStatus = RecordStatus.Mapped;
                                break;
                            case TargetActionType.ValidateData:
                                _defaultStatus = RecordStatus.Valid;
                                break;
                            case TargetActionType.FindDuplicates:
                                _defaultStatus = RecordStatus.Unique;
                                break;
                            default:
                                _defaultStatus = RecordStatus.UnImported;
                                break;
                        }
                    }
                    return _defaultStatus;
                }
            }

            /// <summary>
            /// Add a list of failed source records and the corresponding failure type(invalid,etc.) and reason(why invalid,etc.)
            /// </summary>
            /// <remarks>
            /// It not only add items to <see cref="FailedRecords"/>, but also update the corresponding log entry for each failed source record.
            /// </remarks>
            /// <param name="failureType">At which step the records are suggested as failed.</param>
            /// <param name="sourceRecords">list of failed source records and the corresponding failure reason</param>
            public void AddFailedRecords(RecordStatus failureType, Dictionary<ISourceRecord, string> sourceRecords)
            {
                List<int> indices = new List<int>();
                List<int> InvalidIndices = new List<int>();
                foreach (KeyValuePair<ISourceRecord, string> failedRecord in sourceRecords)
                {
                    int index = failedRecord.Key.SourceIndex;
                    if (this.ContainsKey(index))
                    {
                        if (failureType == RecordStatus.Matched && failedRecord.Value != null && failedRecord.Value.Contains("Error"))
                        {
                            this[index].Status = RecordStatus.Invalid;
                            InvalidIndices.Add(index);
                        }
                        else
                        {
                            this[index].Status = failureType;
                            indices.Add(index);
                        }
                        this[index].Reason = failedRecord.Value;
                    }
                    else
                        indices.Add(index);
                }
                if (FailedRecords.ContainsKey(failureType) == false)
                    FailedRecords[failureType] = new IndexList();
                FailedRecords[failureType].AddValues(indices);
                if (InvalidIndices.Count > 0)
                {
                    if (FailedRecords.ContainsKey(RecordStatus.Invalid) == false)
                        FailedRecords[RecordStatus.Invalid] = new IndexList();
                    FailedRecords[RecordStatus.Invalid].AddValues(InvalidIndices);
                }
            }

            /// <summary>
            /// Get string representation of a IndexList, combined with neccessary tab keys.
            /// </summary>
            /// <remarks>
            /// <para>If you have a IndexList {2,3,4,8} and tabLevel is 0,you will get:</para>
            /// <para>from 2 to 4</para>
            /// <para>8</para>
            /// </remarks>
            /// <param name="indices">An IndexList object from which to obtain the string representation</param>
            /// <param name="tabLevel">indent level</param>
            /// <returns></returns>
            private string GetIndexRangesString(IndexList indices, int tabLevel)
            {
                IndexRanges ranges = indices.ToIndexRanges();
                StringBuilder builder = new StringBuilder();
                string buff = string.Empty;
                foreach (IndexRange range in ranges.Values)
                {
                    buff = "".PadLeft(tabLevel, '\t');
                    if (range.RangeBegin == range.RangeEnd)
                    {
                        buff += range.RangeBegin.ToString();
                    }
                    else
                    {
                        buff += string.Format(Resources.LogSum_IndexRanges, range.RangeBegin, range.RangeEnd);
                    }
                    builder.AppendLine(buff);
                }
                return builder.ToString();
            }

            public void WriteHeader()
            {
                //using (StreamWriter sw = new StreamWriter(_logFilePath, false))
                //    sw.WriteLine(LOG_HEADER);
                WriteToLogFile(LOG_HEADER, true);
            }

            public void WriteDetails()
            {

                try
                {
                    //using (StreamWriter sw = new StreamWriter(_logFilePath))
                    //{
                    //    sw.Write(Log.DetailsLog);
                    //    sw.Close();
                    //    sw.Dispose();
                    //}
                    WriteToLogFile(Log.DetailsLog, false);
                }
                catch { }

            }

            public void WriteSummary()
            {
                //using (StreamWriter sw = new StreamWriter(_logFilePath, true))
                //{
                //    sw.WriteLine(Resources.Log_Seperator);
                //    sw.Write("\"{0}\"", Log.Summary);
                //    sw.Close();
                //    sw.Dispose();
                //}
                WriteToLogFile(Resources.Log_Seperator, true);
                WriteToLogFile(Log.Summary, true);
            }

            public void WriteToLogFile(string msg, bool bAddNewLine)
            {
                Mutex LogFileMutex = new Mutex(false, "LogFileWrite");
                //Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(msg);
                LogFileMutex.WaitOne();
                using (StreamWriter sw = new StreamWriter(_logFilePath, true))
                {
                    //
                    if (bAddNewLine)
                        sw.WriteLine(msg);
                    else
                        sw.Write(msg);
                    sw.Close();
                    sw.Dispose();
                }
                LogFileMutex.ReleaseMutex();
            }

            public string LogFilePath
            {
                get { return _logFilePath; }
            }

            /// <summary>
            /// 
            /// </summary>
            public string DetailsLog
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    string message;
                    foreach (KeyValuePair<int, LogEntry> logEntry in Log)
                    {
                        string status = Log[logEntry.Key].Status.ToString().ToLower();
                        switch (Log[logEntry.Key].Status)
                        {
                            case RecordStatus.AddedToPendingReview:
                                status = Resources.Log_AddedToPendingReview;
                                break;
                            case RecordStatus.UnImported:
                                status = Resources.Log_UnImported;
                                break;
                        }
                        message = string.Format("{0},\"{1}\",\"{2}\"", logEntry.Key, status, Log[logEntry.Key].Reason);
                        builder.AppendLine(message);
                    }
                    return builder.ToString();
                }
            }

            /// <summary>
            /// Collect summary information for log file.
            /// </summary>
            public string Summary
            {
                get
                {
                    int totalCount = 0;
                    int unextractedCount = 0;
                    int unmappedCount = 0;
                    int invalidCount = 0;
                    int duplicatedCount = 0;
                    int unimportedCount = 0;
                    int expectedCount = 0;
                    string targetAction = string.Empty;
                    string expectedResult = string.Empty;
                    StringBuilder summary = new StringBuilder();
                    bool noActionTaken = false;

                    //retrieve target action and expected results
                    switch (_targetActionType)
                    {
                        case TargetActionType.ValidateMapping:
                            targetAction = Resources.LogSum_TargetAction_ValidateMapping;
                            expectedResult = Resources.LogSum_ValidateMapping_ExpectedResult;
                            break;
                        case TargetActionType.ValidateData:
                            targetAction = Resources.LogSum_TargetAction_Validate;
                            expectedResult = Resources.LogSum_Validate_ExpectedResult;
                            break;
                        case TargetActionType.FindDuplicates:
                            targetAction = Resources.LogSum_TargetAction_FindDuplicates;
                            expectedResult = Resources.LogSum_FindDuplicates_ExpectedResult;
                            break;
                        default:
                            targetAction = Resources.LogSum_TargetAction_Load;
                            expectedResult = string.Format(Resources.LogSum_Load_ExpectedResult, TemporalRecordsCount, PermanentRecordsCount);
                            if (TemporalRecordsCount + PermanentRecordsCount == 0)
                                noActionTaken = true;
                            break;
                    }

                    //original IndexRange and table name
                    if (_originalRange != null)
                    {
                        totalCount = _originalRange.RangeEnd - _originalRange.RangeBegin + 1;
                        summary.AppendFormat(Resources.LogSum_ExpectedRange, targetAction, _originalRange.RangeBegin + 1, _originalRange.RangeEnd + 1, totalCount);
                        if (string.IsNullOrEmpty(_tableName) == false)
                        {
                            summary.AppendFormat(Resources.LogSum_TableName, _tableName);
                        }
                        summary.AppendLine();
                    }

                    //list different types of failed records.(unextracted,unmapped,invalid,duplciated,unimported)
                    IndexList failedRecords = null;
                    if (FailedRecords.TryGetValue(RecordStatus.UnExtracted, out failedRecords))
                    {
                        //unextracted records count.
                        summary.AppendFormat(Resources.LogSum_UnExtracted, unextractedCount = failedRecords.Count);
                        summary.AppendLine();
                        //list of unextracted records' index
                        summary.AppendLine(GetIndexRangesString(FailedRecords[RecordStatus.UnExtracted], 1));
                    }
                    if (FailedRecords.TryGetValue(RecordStatus.UnMapped, out failedRecords))
                    {
                        //unmapped records count.
                        summary.AppendFormat(Resources.LogSum_UnMapped, unmappedCount = failedRecords.Count);
                        summary.AppendLine();
                        //list of unmapped records' index
                        summary.AppendLine(GetIndexRangesString(FailedRecords[RecordStatus.UnMapped], 1));
                    }
                    if (FailedRecords.TryGetValue(RecordStatus.Invalid, out failedRecords))
                    {
                        //invalid records count.
                        summary.AppendFormat(Resources.LogSum_Invalid, invalidCount = failedRecords.Count);
                        summary.AppendLine();
                        //list of invalid records' index
                        summary.AppendLine(GetIndexRangesString(FailedRecords[RecordStatus.Invalid], 1));
                    }
                    if (FailedRecords.TryGetValue(RecordStatus.Matched, out failedRecords))
                    {
                        //duplicated records count.
                        summary.AppendFormat(Resources.LogSum_Duplicated, duplicatedCount = failedRecords.Count);
                        summary.AppendLine();
                        //list of duplicated records' index
                        summary.AppendLine(GetIndexRangesString(FailedRecords[RecordStatus.Matched], 1));
                    }
                    if (FailedRecords.TryGetValue(RecordStatus.UnImported, out failedRecords))
                    {
                        //unimported records count.
                        summary.AppendFormat(Resources.LogSum_UnImported, unimportedCount = failedRecords.Count);
                        summary.AppendLine();
                        //list of unimported records' index
                        summary.AppendLine(GetIndexRangesString(FailedRecords[RecordStatus.UnImported], 1));
                    }

                    expectedCount = totalCount - unextractedCount - unmappedCount - invalidCount - duplicatedCount - unimportedCount;

                    if (noActionTaken)
                        summary.Append(Resources.LogSum_NoActionTaken);
                    else
                        summary.AppendFormat(Resources.LogSum_SuccesfullyProcessed, expectedCount, expectedResult);
                    return summary.ToString();
                }
            }
        }

        /// <summary>
        /// All possible status of a source record or its corresponding destination record when try to load it into database.
        /// </summary>
        public enum RecordStatus
        {
            Ready,
            UnExtracted,
            UnMapped,
            Mapped,
            Valid,
            Invalid,
            Unique,
            Matched,
            UnImported,
            Registered,
            AddedToPendingReview,
        }

        /// <summary>
        /// Stores information of a source record or its corresponding destination record.
        /// </summary>
        public class LogEntry
        {
            private RecordStatus _status;

            public RecordStatus Status
            {
                get { return _status; }
                set { _status = value; }
            }
            private string _reason;

            public string Reason
            {
                get { return _reason; }
                set { _reason = value; }
            }
        }

        #endregion

        private static void DoListTables(JobParameters jobParameters)
        {
            //_consoleWriter.WriteLine("Determining tables/worksheets for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name);
            //_consoleWriter.WriteLine();

            TablesDiscoveryService tblSvc = new TablesDiscoveryService(jobParameters);
            JobResponse response = tblSvc.DoJob();
            List<string> tableNames =
                response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST] as List<string>;

            DisplayTableNames(tableNames);
        }

        private static bool DoNormalizeJobParameters(JobParameters jobParameters)
        {
            NormalizeJobParametersService normalizeJobParametersService = new NormalizeJobParametersService(jobParameters);
            JobResponse normalizeJobParametersResponse = normalizeJobParametersService.DoJob();
            Dictionary<string, object> allResultsDic = normalizeJobParametersResponse.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;
            if (allResultsDic != null)
            {
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_TABLE_NAME))
                {
                    List<string> tableNames = allResultsDic[NormalizeJobParametersService.INVALID_TABLE_NAME] as List<string>;
                    // _consoleWriter.WriteLine(string.Format(Resources.CmdVali_InvalidTableName, jobParameters.DataSourceInformation.TableName));
                    // _consoleWriter.WriteLine(Resources.CmdVali_ValidTableNames);
                    tableNames.ForEach(delegate(string tableName)
                    {
                        // _consoleWriter.WriteLine(tableName);
                    });
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_RANGE_BEGIN))
                {
                    int recordCount = (int)allResultsDic[NormalizeJobParametersService.INVALID_RANGE_BEGIN];
                    //_consoleWriter.WriteLine(Properties.Resources.RecordsCount, recordCount.ToString());
                    //_consoleWriter.WriteLine(Properties.Resources.RangeBeginCount,
                    //    (jobParameters.ActionRanges[0].RangeBegin + 1).ToString());
                    //_consoleWriter.WriteLine(Properties.Resources.CmdVali_RangeBeginValidation);
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE))
                {
                    List<string> invalidDerivedArgValues = allResultsDic[NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE] as List<string>;
                    // _consoleWriter.WriteLine(Resources.CmdVali_ArgumentsNotFoundMessage);
                    invalidDerivedArgValues.ForEach(delegate(string invalidDerivedArgValue)
                    {
                        // _consoleWriter.WriteLine(invalidDerivedArgValue);
                    });
                    // _consoleWriter.WriteLine(Resources.CmdVali_ValidArgumentValues);
                    foreach (string sourceArg in SourceFieldTypes.TypeDefinitions.Keys)
                    {
                        //_consoleWriter.WriteLine(sourceArg);
                    }
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_PICKLIST_CODE))
                {
                    List<string> invalidPicklistCodes = allResultsDic[NormalizeJobParametersService.INVALID_PICKLIST_CODE] as List<string>;
                    //_consoleWriter.WriteLine(Resources.CmdVali_InvalidPicklistCodes);
                    invalidPicklistCodes.ForEach(delegate(string invalidPicklistCode)
                    {
                        //_consoleWriter.WriteLine(invalidPicklistCode);
                    });
                    return false;
                }
            }

            return true;
        }

        private void DoCountRecords(JobParameters jobParameters)
        {
            StringBuilder sbMsg = new StringBuilder("");
            sbMsg.AppendLine(string.Format("Counting records for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name));
            sbMsg.AppendLine(string.Format("Records count: {0}", jobParameters.FileReader.RecordCount));
            sbMsg.AppendLine();
            JobResult.OnShowOutput(sbMsg.ToString());

        }

        private void DoSplitFile(JobParameters jobParameters)
        {
            //_consoleWriter.WriteLine("Splitting file '{0}' into subset files", jobParameters.DataSourceInformation.DerivedFileInfo.Name);
            //_consoleWriter.WriteLine();

            SplitFileService svc = new SplitFileService(jobParameters);
            JobResponse response = svc.DoJob();
            List<string> newFileNames =
                response.ResponseContext[SplitFileService.SPLIT_FILE_RESPONSE] as List<string>;

            //_consoleWriter.WriteLine();
            JobResult.DisplayFileNames(newFileNames);
        }

        private void DisplayFields(Dictionary<string, Type> fieldDefinitions)
        {
            char underscore = '_';
            StringBuilder sbMsg = new StringBuilder("");
            sbMsg.Append("Field".PadRight(30) + "Type".PadRight(30));
            sbMsg.AppendLine(underscore.ToString().PadRight(30, underscore) + underscore.ToString().PadRight(30, underscore));
            //_consoleWriter.WriteLine("Field".PadRight(30) + "Type".PadRight(30));
            //_consoleWriter.WriteLine(
            //    underscore.ToString().PadRight(30, underscore) + underscore.ToString().PadRight(30, underscore)
            //    );

            if (fieldDefinitions == null)
                sbMsg.AppendLine("No fields found.");
            else
                foreach (KeyValuePair<string, Type> kvp in fieldDefinitions)
                {
                    string type = (kvp.Value == null) ? "undetermined (no data)" : kvp.Value.ToString().PadRight(30);
                    sbMsg.AppendLine(kvp.Key.PadRight(30) + type);
                }
            JobResult.OnShowOutput(sbMsg.ToString());
            //_consoleWriter.WriteLine();
        }

        private static void DisplayTableNames(List<string> tableNames)
        {
            //char underscore = '_';

            //_consoleWriter.WriteLine("Tables / Worksheets".PadRight(30));
            //_consoleWriter.WriteLine(underscore.ToString().PadRight(30, underscore));
            //if (tableNames == null || tableNames.Count == 0)
            //    _consoleWriter.WriteLine("No tables or worksheets found.");
            //else
            //    foreach (string tableName in tableNames)
            //        _consoleWriter.WriteLine(tableName.PadRight(30));

            //_consoleWriter.WriteLine();
        }

        public void ProcessJobExecutor(JobParameters jobParameters, IndexRange processRange, ProcessResult psR, string LogFilePath, string traceLogPath)
        {
            if (Log == null)
            {
                Log = new LogTable(jobParameters, LogFilePath);
                Log._originalRange = processRange;
            }
            SetTraceLog(traceLogPath);
            ProcessJobExecutor(jobParameters, processRange, psR);
            Thread.CurrentThread.Abort();
        }

        public void ProcessJobExecutor(JobParameters jobParameters, IndexRange processRange, ProcessResult processOutput)
        {
            try
            {
                //Fix for CSBR-161737-Log message needs to be changed for the action "importregdupascreatenew"
                jobRecordsImported = (JobParameters)jobParameters;
                processOutput.TargetAction = jobParameters.TargetActionType;
                //if (_consoleWriter == null)
                //    _consoleWriter = new ConsoleWriter();

                //Honor the requested index range
                // 1 - ensure we are not surpassing the length of small files
                if (!string.IsNullOrEmpty(jobParameters.UserName) && !string.IsNullOrEmpty(jobParameters.Password))
                    COEPrincipal.Login(jobParameters.UserName, jobParameters.Password);
                int chunkSize = Math.Min(JobUtility.CHUNK_SIZE, processRange.ToIndexList().Count);
                // 2- ensure we are not surpassing the size of a small request range
                chunkSize = Math.Min(chunkSize, processRange.ToIndexList().Count);
                //Break up the range(s) appropriately
                IndexList originalList = processRange.ToIndexList(processRange.RangeBegin, processRange.RangeEnd);
                IndexRanges splitRanges = originalList.ToIndexRanges(chunkSize);
                IndexRanges workerRanges = null;

                //if (JobResult.RecordParsing == null)

                for (int i = 0; i < splitRanges.Count; i++)
                {
                    if (JobExecutor.JobCancelled)
                    {
                        processOutput.JobComplete = true;
                        return;
                    }

                    DateTime before = DateTime.Now;
                    Trace.Indent();
                    BeforeRecordsChunkProcessing(splitRanges[i]);
                    processOutput.OnBeforeRecordsChunkProcess(splitRanges[i]);
                    // Step 2: As a start for each chunk, we fetch a list of source records whose indices fall into
                    // the current chunk.
                    workerRanges = new IndexRanges();
                    workerRanges.Add(0, splitRanges[i]);
                    jobParameters.ActionRanges = workerRanges;

                    RecordsExtractionService extractionService = new RecordsExtractionService(jobParameters);
                    extractionService.RecordProcessing += RecordParsing;
                    extractionService.JobComplete += RecordsExtracted;
                    jobParameters.SourceRecords = extractionService.DoJob().ResponseContext[CambridgeSoft.COE.DataLoader.Core.Workflow.RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;
                    int SourceRecordCount = jobParameters.SourceRecords.Count;
                    //processOutput.TotalRecordsCount += SourceRecordCount;
                    // Step 3: Then we map the source records to desired destination records, using the mapping information.
                    // We will export those source records failing the mapping.
                    MappingService mappingService = new MappingService(jobParameters);
                    mappingService.RecordProcessing += RecordMapping;
                    mappingService.JobComplete += RecordsMapped;
                    jobParameters.DestinationRecords = mappingService.DoJob().ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;
                    if (jobParameters.TargetActionType == TargetActionType.ValidateMapping)
                    {
                        AfterRecordsChunkProcessed(splitRanges[i]);
                        processOutput.OnAfterRecordsChunkProcess(splitRanges[i]);
                        continue;
                    }

                    // Step 4: After the mapping is done, we validate each destination record and export source records corresponding
                    // to the invalid destination records.
                    ValidateRecordService validateRecordService = new ValidateRecordService(jobParameters);
                    validateRecordService.RecordProcessing += RecordValidating;
                    validateRecordService.JobComplete += RecordsValidated;
                    validateRecordService.DoJob();
                    processOutput.ValidRecordsCount += jobParameters.DestinationRecords.Count;
                    processOutput.InvalidRecordsCount += (SourceRecordCount - jobParameters.DestinationRecords.Count);
                    // TODO: INTERNAL POSSIBLE STOP POINT HERE!!!
                    if (jobParameters.TargetActionType == TargetActionType.ValidateData)
                    {
                        // Dispose destination records
                        // TODO: Do we need to dispose the source record list as well?
                        JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                        AfterRecordsChunkProcessed(splitRanges[i]);
                        processOutput.ValidateDataProgress = processOutput.ValidRecordsCount + processOutput.InvalidRecordsCount;
                        continue;
                    }

                    // POSSIBLE STOP POINT HERE!!!
                    if (jobParameters.TargetActionType == TargetActionType.FindDuplicates)
                    {
                        // Step 5 - a): Based on destination record type, we will optionally perform duplicate check against destination records.
                        // Again, we will export source records corresponding to destination records which have duplicates.
                        List<DuplicateCheckResponse> findDupResult;
                        if (jobParameters.Mappings.DestinationRecordType == Mappings.DestinationRecordTypeEnum.RegistryRecord)
                        {
                            FindDuplicateService findDuplicateService = new FindDuplicateService(jobParameters);
                            findDuplicateService.JobComplete += RecordsDupChecked;
                            // TODO: To be removed. For performance testing only.
                            //DateTime findDuplicatesStart = DateTime.Now;
                            findDupResult = findDuplicateService.DoJob().ResponseContext[FindDuplicateService.DUPLICATE_CHECK_RESPONSE] as List<DuplicateCheckResponse>;
                            //_consoleWriter.WriteLine(DateTime.Now.Subtract(findDuplicatesStart).TotalSeconds.ToString());       

                            //int dupCount = 0;
                            //int uniqueCount = 0;
                            if (findDupResult != null)
                            {
                                foreach (DuplicateCheckResponse dupCheckResp in findDupResult)
                                {
                                    if (dupCheckResp.MatchedRegistrations.Count > 0)
                                        processOutput.IncreaseDuplicateRecordCount();
                                    else if (dupCheckResp.UniqueRegistrations == -1)
                                        processOutput.IncreaseInvalidRecordCount();
                                    else
                                        processOutput.IncreaseUniqueRecordCount();
                                    //Application.DoEvents();
                                }
                                //if (dupCount > 0)
                                //    processOutput.DuplicateRecordsCount += dupCount;
                                //if (uniqueCount > 0)
                                //    processOutput.UniqueRecordsCount += uniqueCount;
                            }
                        }

                        // Dispose destination records
                        // TODO: Do we need to dispose the source record list as well?
                        JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                        jobParameters.DestinationRecords = null;
                        AfterRecordsChunkProcessed(splitRanges[i]);

                        continue;
                    }
                    else
                    {
                        List<RegRecordSummaryInfo> importResult = null;
                        // Step 5 - b): Finally we import all the remaining destination records to the database.
                        ImportRecordService importRecordService = new ImportRecordService(jobParameters);
                        importRecordService.RecordProcessing += RecordImporting;
                        importRecordService.JobComplete += RecordsImported;
                        // TODO: To be removed. For performance testing only.
                        DateTime importStart = DateTime.Now;
                        importResult = importRecordService.DoJob().ResponseContext[ImportRecordService.IMPORT_RESULT] as List<RegRecordSummaryInfo>;
                        //_consoleWriter.WriteLine(DateTime.Now.Subtract(importStart).TotalSeconds.ToString());
                        if (importResult != null)       // Coverity Fix: CBOE-1946
                        {
                            foreach (RegRecordSummaryInfo regSummary in importResult)
                            {
                                if (!(regSummary.RegNum == "null" || string.IsNullOrEmpty(regSummary.RegNum) || (regSummary != null && regSummary.RegNum == "0")))
                                {
                                    //processOutput.PermanentRecordsCount += 1;
                                    processOutput.IncreasePermRecordCount();
                                }
                                else if (regSummary.TempNum != null && !string.IsNullOrEmpty(regSummary.TempNum.Replace("0", "")))
                                {
                                    //processOutput.TemporalRecordsCount += 1;
                                    processOutput.IncreaseTempRecordCount();
                                }
                                else
                                {
                                    //processOutput.NoActionRecordsCount += 1;
                                    processOutput.IncreaseNoActionRecordCount();
                                }
                                //Application.DoEvents();
                            }
                        }

                        // Dispose destination records
                        // TODO: Do we need to dispose the source record list as well?
                        JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                    }

                    AfterRecordsChunkProcessed(splitRanges[i]);
                    Trace.Unindent();
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine(string.Format("{0} seconds for this chunk.", DateTime.Now.Subtract(before).TotalSeconds));
                    Trace.WriteLine(string.Empty);
                }
                
                processOutput.SetLogSummary(ThreadIndex, Log.Summary);
                processOutput.ThreadComplete = processOutput.ThreadComplete + 1;
                //processOutput.ValidRecordsCount = processOutput.TemporalRecordsCount + processOutput.PermanentRecordsCount;
                //processOutput.LogSummary = Log.Summary;
                
                //AfterDataLoaded();                
            }
            catch (Exception ex)
            {
                CambridgeSoft.COE.Framework.ExceptionHandling.COEExceptionDispatcher.HandleUIException(ex);
                Log.WriteToLogFile(ex.Message, true);
                //Log.WriteToLogFile(ex.StackTrace, true);
                JobInterupped = true;
            }
            //finally
            //{
            //    processOutput.SetLogSummary(ThreadIndex, Log.Summary);
            //    processOutput.ThreadComplete = processOutput.ThreadComplete + 1;
            //}
        }
    }

    public class ProcessResult : MarshalByRefObject
    {
        #region Properties
        private volatile TargetActionType _targetActionType;
        private volatile int _totalRecordsCount = 0;
        private volatile int _temporalRecordsCount = 0;
        private volatile int _permanentRecordsCount = 0;
        private volatile int _validRecordsCount = 0;
        private volatile int _invalidRecordsCount = 0;
        private volatile int _duplicateRecordsCount = 0;
        private volatile int _uniqueRecordsCount = 0;
        private volatile int _noActionRecords = 0;
        private volatile bool threadMode = false;
        private volatile int threadCount = 0;
        private volatile int threadComplete = 0;
        private volatile bool jobComplete = false;
        private volatile int currentRecordParsing = 0;
        private volatile int validateDataProgress = 0;
        private volatile int _noOfRecordsProcessed = 0;
        private static double _processRate = 0;
        private volatile StringBuilder _logSummary;
        private volatile List<string> _logOut;
        Stopwatch recordProcessStopWatch = new Stopwatch();

        public event EventHandler<EventArgs> ProcessComplete;
        public event EventHandler<EventArgs> BeforeRecordsChunkProcess;
        public event EventHandler<EventArgs> AfterRecordsChunkProcess;
        public event EventHandler<EventArgs> RecordParsing;
        public event EventHandler<EventArgs> RecordsExtracted;
        public event EventHandler<EventArgs> RecordsValidated;
        public event EventHandler<EventArgs> RecordsDupChecked;
        public event EventHandler<EventArgs> RecordsUniqueChecked;
        public event EventHandler<EventArgs> RecordsImported;
        public event EventHandler<EventArgs> RecordsTotal;
        public event EventHandler<EventArgs> RecordsInvalid;
        public event EventHandler<EventArgs> RecordsNoAction;
        public event EventHandler<EventArgs> RecordsValidateData;
        public event EventHandler<EventArgs> ShowOutput;
        public event EventHandler<EventArgs> RecordsTempImported;
        public event EventHandler<EventArgs> RecordsPermImported;
        public event EventHandler<EventArgs> RecordsProcessed;
        public event EventHandler<EventArgs> RecordsProcessRate;

        public void ResetDefault()
        {
            _totalRecordsCount = 0;
            _temporalRecordsCount = 0;
            _permanentRecordsCount = 0;
            _validRecordsCount = 0;
            _invalidRecordsCount = 0;
            _duplicateRecordsCount = 0;
            _uniqueRecordsCount = 0;
            threadMode = false;
            threadCount = 0;
            threadComplete = 0;
            jobComplete = false;
            currentRecordParsing = 0;
            _noActionRecords = 0;
            validateDataProgress = 0;
            _noOfRecordsProcessed = 0;
            _processRate = 0;
            recordProcessStopWatch.Start();
            _logSummary = new StringBuilder("");
            _logOut = new List<string>();
        }

        public TargetActionType TargetAction
        {
            get { return _targetActionType; }
            set { _targetActionType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalRecordsCount
        {
            get { return _totalRecordsCount; }
            set
            {
                _totalRecordsCount = value;
                if (RecordsTotal != null)
                    RecordsTotal(_totalRecordsCount, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TemporalRecordsCount
        {
            get { return _temporalRecordsCount; }
            //set
            //{
            //_temporalRecordsCount = value;
            //if (RecordsImported != null)
            //    RecordsImported(_permanentRecordsCount + _temporalRecordsCount, EventArgs.Empty);
            //if (RecordsTempImported != null)
            //    RecordsTempImported(_temporalRecordsCount, EventArgs.Empty);
            //NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public int PermanentRecordsCount
        {
            get { return _permanentRecordsCount; }
            //set
            //{
            //    _permanentRecordsCount = value;
            //    if (RecordsImported != null)
            //        RecordsImported(_permanentRecordsCount + _temporalRecordsCount, EventArgs.Empty);
            //    if (RecordsPermImported != null)
            //        RecordsPermImported(_permanentRecordsCount, EventArgs.Empty);
            //    NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public int NoActionRecordsCount
        {
            get { return _noActionRecords; }
            //set
            //{
            //    _noActionRecords = value;
            //    if (RecordsNoAction != null)
            //        RecordsNoAction(_noActionRecords, EventArgs.Empty);
            //    NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ValidRecordsCount
        {
            get { return _validRecordsCount; }
            set
            {
                _validRecordsCount = value;
                if (RecordsValidated != null)
                    RecordsValidated(_validRecordsCount, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int InvalidRecordsCount
        {
            get { return _invalidRecordsCount; }
            set
            {
                _invalidRecordsCount = value;
                if (RecordsInvalid != null)
                    RecordsInvalid(_invalidRecordsCount, EventArgs.Empty);
                if (_targetActionType == TargetActionType.FindDuplicates)
                    NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
                else if (_targetActionType.ToString().ToLower().Contains("import"))
                    NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DuplicateRecordsCount
        {
            get { return _duplicateRecordsCount; }
            //set
            //{
            //    _duplicateRecordsCount = value;
            //    if (RecordsDupChecked != null)
            //        RecordsDupChecked(_duplicateRecordsCount, EventArgs.Empty);
            //    NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public int UniqueRecordsCount
        {
            get { return _uniqueRecordsCount; }
            //set
            //{
            //    _uniqueRecordsCount = value;
            //    if (RecordsUniqueChecked != null)
            //        RecordsUniqueChecked(_uniqueRecordsCount, EventArgs.Empty);
            //    NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public int NoOfRecordsProcessed
        {
            get { return _noOfRecordsProcessed; }
            set
            {
                _noOfRecordsProcessed = value;
                if (RecordsProcessed != null)
                    RecordsProcessed(_noOfRecordsProcessed, EventArgs.Empty);
                ProcessRate = (double)_validRecordsCount / ((double)recordProcessStopWatch.ElapsedMilliseconds / 1000);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ProcessRate
        {
            get { return _processRate; }
            set
            {
                _processRate = value;
                if (RecordsProcessRate != null)
                    RecordsProcessRate(_processRate, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentRecordParsing
        {
            get { return currentRecordParsing; }
            set
            {
                currentRecordParsing = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ValidateDataProgress
        {
            get { return validateDataProgress; }
            set
            {
                validateDataProgress = value;
                if (RecordsValidateData != null)
                    RecordsValidateData(validateDataProgress, EventArgs.Empty);
            }
        }

        public bool ThreadMode
        {
            set { threadMode = value; }
            get { return threadMode; }
        }

        public int ThreadCount
        {
            set
            {
                threadCount = value;

                for (int i = 0; i < threadCount; i++)
                {
                    _logOut.Add("");
                }
            }
            get { return threadCount; }
        }

        public int ThreadComplete
        {
            set
            {
                threadComplete = value;
                if (threadMode && threadComplete >= threadCount)
                {
                    if (RecordsProcessed != null)
                        RecordsProcessed(_noOfRecordsProcessed, EventArgs.Empty);
                    if (RecordsProcessRate != null)
                        RecordsProcessRate(_processRate, EventArgs.Empty);
                    JobComplete = true;
                }
            }
            get { return threadComplete; }
        }

        public bool JobComplete
        {
            set
            {
                jobComplete = value;
                if (jobComplete)
                    OnProcessComplete();
            }
            get { return jobComplete; }
        }

        public string LogSummary
        {
            get
            {
                _logSummary.Clear();
                foreach (string strLog in _logOut)
                {
                    _logSummary.AppendLine(strLog);
                }
                return _logSummary.ToString();
            }
            set
            {
                _logSummary.AppendLine(value);
            }
        }

        public void SetLogSummary(int threadIndex, string summary)
        {
            if (_logOut.Count > threadIndex)
            {
                StringBuilder sbFormat = new StringBuilder("Thread " + (threadIndex + 1) + " summary");
                sbFormat.AppendLine("");
                sbFormat.AppendLine(new string('-', 20));
                sbFormat.AppendLine(summary);
                _logOut[threadIndex] = sbFormat.ToString();
            }
            else
                _logOut.Add(summary);
        }
        protected void OnProcessComplete()
        {
            if (ProcessComplete != null)
            {
                ProcessComplete(this, EventArgs.Empty);
                recordProcessStopWatch.Stop();
            }
        }

        public void OnBeforeRecordsChunkProcess(IndexRange indexRange)
        {
            if (BeforeRecordsChunkProcess != null)
                BeforeRecordsChunkProcess(indexRange, EventArgs.Empty);
        }

        public void OnAfterRecordsChunkProcess(IndexRange indexRange)
        {
            if (AfterRecordsChunkProcess != null)
                AfterRecordsChunkProcess(indexRange, EventArgs.Empty);
        }

        public void OnShowOutput(object sender)
        {
            if (ShowOutput != null)
                ShowOutput(sender, EventArgs.Empty);
        }

        public void DisplayFileNames(List<string> fileNames)
        {
            char underscore = '_';
            StringBuilder sbMsg = new StringBuilder("New files");
            sbMsg.AppendLine();
            sbMsg.Append(underscore.ToString().PadRight(30, underscore));

            if (fileNames == null || fileNames.Count == 0)
                sbMsg.AppendLine("No subset files created.");
            else
                foreach (string fileName in fileNames)
                    sbMsg.AppendLine(fileName.PadRight(30));
            if (ShowOutput != null)
                ShowOutput(sbMsg.ToString(), EventArgs.Empty);
        }

        #endregion

        #region Methods

        public void IncreaseTempRecordCount()
        {
            _temporalRecordsCount++;
            if (RecordsImported != null)
                RecordsImported(_permanentRecordsCount + _temporalRecordsCount, EventArgs.Empty);
            if (RecordsTempImported != null)
                RecordsTempImported(_temporalRecordsCount, EventArgs.Empty);
            NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
        }

        public void IncreasePermRecordCount()
        {
            _permanentRecordsCount++;
            if (RecordsImported != null)
                RecordsImported(_permanentRecordsCount + _temporalRecordsCount, EventArgs.Empty);
            if (RecordsPermImported != null)
                RecordsPermImported(_permanentRecordsCount, EventArgs.Empty);
            NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
        }

        public void IncreaseNoActionRecordCount()
        {
            _noActionRecords++;
            if (RecordsNoAction != null)
                RecordsNoAction(_noActionRecords, EventArgs.Empty);
            NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
        }

        public void IncreaseUniqueRecordCount()
        {
            _uniqueRecordsCount++;
            if (RecordsUniqueChecked != null)
                RecordsUniqueChecked(_uniqueRecordsCount, EventArgs.Empty);
            NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
        }

        public void IncreaseDuplicateRecordCount()
        {
            _duplicateRecordsCount++;
            if (RecordsDupChecked != null)
                RecordsDupChecked(_duplicateRecordsCount, EventArgs.Empty);
            NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
        }

        public void IncreaseInvalidRecordCount()
        {
            _invalidRecordsCount++;
            if (RecordsInvalid != null)
                RecordsInvalid(_invalidRecordsCount, EventArgs.Empty);
            if (_targetActionType == TargetActionType.FindDuplicates)
                NoOfRecordsProcessed = _uniqueRecordsCount + _duplicateRecordsCount + _invalidRecordsCount;
            else if (_targetActionType.ToString().ToLower().Contains("import"))
                NoOfRecordsProcessed = _temporalRecordsCount + _permanentRecordsCount + _noActionRecords + _invalidRecordsCount;
        }

        #endregion

    }



}

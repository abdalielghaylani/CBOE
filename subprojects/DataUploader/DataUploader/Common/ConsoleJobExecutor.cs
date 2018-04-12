using System;
using System.IO;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Properties;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.DataLoader.Common
{
    public static class ConsoleJobExecutor
    {
        private static ConsoleWriter _consoleWriter = null;
        


        /// <summary>
        /// For unattended loader session, we just walk through all the steps for an import action. But we can stop at any step,
        /// based on target action type.
        /// </summary>
        /// <param name="jobParameters">The job parameters instructing this unattended loader session.</param>
        public static void DoUnattendedJob(ConsoleWriter consoleWriter, JobParameters jobParameters)
        {   //put this is
            _consoleWriter = consoleWriter;
            BeforeDataLoading(jobParameters);

            // ListTables action doesn't require job parameters normalization.
            if (jobParameters.TargetActionType == TargetActionType.ListTables)
            {
                DoListTables(jobParameters);
                return;
            }
            
            if (DoNormalizeJobParameters(jobParameters))
            {
                _consoleWriter.WriteLine(Resources.CmdVali_Valid);
            }
            else
                return;

            if (jobParameters.TargetActionType == TargetActionType.ValidateMapping)
                return;

            // The reader should have already been initialized during the normalization process.
            IFileReader reader = jobParameters.FileReader;

            DoCountRecords(jobParameters);
            if (jobParameters.TargetActionType == TargetActionType.CountRecords)
                return;

            if (jobParameters.TargetActionType == TargetActionType.SplitFile)
            {
                DoSplitFile(jobParameters);
                return;
            }

            _consoleWriter.WriteLine(String.Format("Determining field names and data-types for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name));
            _consoleWriter.WriteLine();
            if (jobParameters.TargetActionType == TargetActionType.ListFields)
            {
                // The field and type scan service has already been called in normalization service,
                // so here we simply display the global type definition.
                DisplayFields(SourceFieldTypes.TypeDefinitions);
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

            //Honor the requested index range
            // 1 - ensure we are not surpassing the length of small files
            int chunkSize = Math.Min(JobUtility.CHUNK_SIZE, reader.RecordCount);
            // 2- ensure we are not surpassing the size of a small request range
            chunkSize = Math.Min(chunkSize, originalRange.ToIndexList().Count);
            //Break up the range(s) appropriately
            IndexList originalList = originalRange.ToIndexList(originalRange.RangeBegin, originalRange.RangeEnd);
            IndexRanges splitRanges = originalList.ToIndexRanges(chunkSize);
            IndexRanges workerRanges = null;

            for (int i = 0; i < splitRanges.Count; i++)
            {
                DateTime before = DateTime.Now;
                Trace.Indent();
                BeforeRecordsChunkProcessing(splitRanges[i]);

                // Step 2: As a start for each chunk, we fetch a list of source records whose indices fall into
                // the current chunk.
                workerRanges = new IndexRanges();
                workerRanges.Add(0, splitRanges[i]);
                jobParameters.ActionRanges = workerRanges;

                RecordsExtractionService extractionService = new RecordsExtractionService(jobParameters);
                extractionService.RecordProcessing += RecordParsing;
                extractionService.JobComplete += RecordsExtracted;
                jobParameters.SourceRecords = extractionService.DoJob().ResponseContext[CambridgeSoft.COE.DataLoader.Core.Workflow.RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;

                // Step 3: Then we map the source records to desired destination records, using the mapping information.
                // We will export those source records failing the mapping.
                MappingService mappingService = new MappingService(jobParameters);
                mappingService.RecordProcessing += RecordMapping;
                mappingService.JobComplete += RecordsMapped;
                jobParameters.DestinationRecords = mappingService.DoJob().ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;
                if (jobParameters.TargetActionType == TargetActionType.ValidateMapping)
                {
                    AfterRecordsChunkProcessed(splitRanges[i]);
                    continue;
                }

                // Step 4: After the mapping is done, we validate each destination record and export source records corresponding
                // to the invalid destination records.
                ValidateRecordService validateRecordService = new ValidateRecordService(jobParameters);
                validateRecordService.RecordProcessing += RecordValidating;
                validateRecordService.JobComplete += RecordsValidated;
                validateRecordService.DoJob();

                // TODO: INTERNAL POSSIBLE STOP POINT HERE!!!
                if (jobParameters.TargetActionType == TargetActionType.ValidateData)
                {
                    // Dispose destination records
                    // TODO: Do we need to dispose the source record list as well?
                    JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                    AfterRecordsChunkProcessed(splitRanges[i]);
                    continue;
                }

                // POSSIBLE STOP POINT HERE!!!
                if (jobParameters.TargetActionType == TargetActionType.FindDuplicates)
                {
                    // Step 5 - a): Based on destination record type, we will optionally perform duplicate check against destination records.
                    // Again, we will export source records corresponding to destination records which have duplicates.
                    if (jobParameters.Mappings.DestinationRecordType == Mappings.DestinationRecordTypeEnum.RegistryRecord)
                    {
                        FindDuplicateService findDuplicateService = new FindDuplicateService(jobParameters);
                        findDuplicateService.JobComplete += RecordsDupChecked;
                        // TODO: To be removed. For performance testing only.
                        DateTime findDuplicatesStart = DateTime.Now;
                        findDuplicateService.DoJob();
                        _consoleWriter.WriteLine(DateTime.Now.Subtract(findDuplicatesStart).TotalSeconds.ToString());
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
                    // Step 5 - b): Finally we import all the remaining destination records to the database.
                    ImportRecordService importRecordService = new ImportRecordService(jobParameters);
                    importRecordService.RecordProcessing += RecordImporting;
                    importRecordService.JobComplete += RecordsImported;
                    // TODO: To be removed. For performance testing only.
                    DateTime importStart = DateTime.Now;
                    importRecordService.DoJob();
                    _consoleWriter.WriteLine(DateTime.Now.Subtract(importStart).TotalSeconds.ToString());

                    // Dispose destination records
                    // TODO: Do we need to dispose the source record list as well?
                    JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                }
                AfterRecordsChunkProcessed(splitRanges[i]);
                Trace.Unindent();
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Format("{0} seconds for this chunk." , DateTime.Now.Subtract(before).TotalSeconds));
                Trace.WriteLine(string.Empty);
            }

            if (jobParameters.FileReader != null)
                jobParameters.FileReader.Close();

            AfterDataLoaded();
        }

        #region fields and utility methods

        private static LogTable _log;

        /// <summary>
        /// a LogTable object used to store information to be written in log file.
        /// </summary>
        private static LogTable Log
        {
            get { return _log; }
            set { _log = value; }
        }

        private static void WriteTab()
        {
            _consoleWriter.Write("\t");
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
            TargetActionType targetActionType=parameters.TargetActionType;
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
                Directory.CreateDirectory(Path.GetDirectoryName(traceLogFilePath));//create directory if not exists
                StreamWriter trace = new StreamWriter(traceLogFilePath, false);
                Trace.Listeners.Add(new TextWriterTraceListener(trace));
            }
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
            _consoleWriter.WriteLine(string.Format("Please find log output at: '{0}'", Log.LogFilePath));

            Trace.Close();
        }

        /// <summary>
        /// console output,create new log entry.
        /// </summary>
        /// <param name="range"></param>
        private static void BeforeRecordsChunkProcessing(IndexRange range)
        {
            string message = string.Format(Resources.BeforeRecordsChunkProcessing, range.RangeBegin + 1, range.RangeEnd + 1);
            _consoleWriter.WriteLine(message);
            _consoleWriter.WriteLine();

            if (Log == null) return;
            Log.Clear();//clear the log entries that has been written to log file in previous chunk processing.
            for (int index = range.RangeBegin + 1; index <= range.RangeEnd + 1; index++)
            {
                LogEntry entry = new LogEntry();
                entry.Status = RecordStatus.Ready;
                Log.Add(index, entry);
            }
        }

        /// <summary>
        /// console output,write log information of the current chunk
        /// </summary>
        /// <param name="range"></param>
        private static void AfterRecordsChunkProcessed(IndexRange range)
        {
            _consoleWriter.WriteLine();
            string message = string.Format(Resources.AfterRecordsChunkProcessing, range.RangeBegin + 1, range.RangeEnd + 1);
            _consoleWriter.WriteLine(message);

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
                else if(entry.Status==RecordStatus.Ready)
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
            _consoleWriter.CursorLeft(0);
            WriteTab();
            _consoleWriter.Write(Resources.RecordParsing, e.SourceRecord.SourceIndex);
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
            _consoleWriter.WriteLine();
            WriteTab();
            _consoleWriter.Write(Resources.RecordsExtracted, args.Count);
            _consoleWriter.WriteLine();

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
            _consoleWriter.CursorLeft(0);
            WriteTab();
            _consoleWriter.Write(Resources.RecordMapping, e.SourceRecord.SourceIndex);
        }

        ///<summary>console output,and optionally stores unmapped records' iformation for log</summary>
        /// <remarks>
        /// a list of source records are mapped
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsMapped(object sender, EventArgs e)
        {
            _consoleWriter.WriteLine();
            RecordsProcessedEventArgs args = (RecordsProcessedEventArgs)e;
            WriteTab();
            _consoleWriter.WriteLine(Resources.RecordsMapped, args.Count);
            _consoleWriter.WriteLine();

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
            _consoleWriter.CursorLeft(0);
            WriteTab();
            _consoleWriter.Write(Resources.RecordValidating, e.SourceRecord.SourceIndex);
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
            _consoleWriter.WriteLine();
            WriteTab();
          
            _consoleWriter.WriteLine(Resources.RecordsValidated, args.Count - args.SourceRecords.Count, args.SourceRecords.Count);
            _consoleWriter.WriteLine();

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
            _consoleWriter.WriteLine(Resources.RecordsDupChecked, args.Count - args.SourceRecords.Count, args.SourceRecords.Count);
            _consoleWriter.WriteLine();

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
            _consoleWriter.CursorLeft(0);
            WriteTab();
            _consoleWriter.Write(Resources.RecordImporting, e.SourceRecord.SourceIndex);
        }

        /// <summary>console output,and optionally stores unimported records' information for log</summary>
        /// <remarks>
        /// a list of destination record has been imported into database
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RecordsImported(object sender, EventArgs e)
        {
            _consoleWriter.WriteLine();
            RecordsImportedEventArgs args = (RecordsImportedEventArgs)e;
            WriteTab();

            //update log entries

            //temporal, permanent, as well as not imported records
            Dictionary<ISourceRecord, string> notImportedRecords = new Dictionary<ISourceRecord, string>();
            foreach (KeyValuePair<ISourceRecord, RegRecordSummaryInfo> info in args.RecordsSummaryInfo)
            {
                int index=info.Key.SourceIndex;
                string regNum = info.Value.RegNum;
                string tempNum = info.Value.TempNum;
                if (string.IsNullOrEmpty(regNum) == false &&
                    regNum != "null")//permanent records
                {
                    Log[index].Status = RecordStatus.Registered;
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
            }

            _consoleWriter.WriteLine(Resources.RecordsImported, Log.PermanentRecordsCount+Log.TemporalRecordsCount);
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
        public class LogTable : Dictionary<int, LogEntry>
        {
            private Dictionary<RecordStatus, IndexList> _failedRecords;
            private IndexRange _originalRange = null;
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
            public LogTable(JobParameters parameters,string logFilePath)
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
                foreach (KeyValuePair<ISourceRecord, string> failedRecord in sourceRecords)
                {
                    int index = failedRecord.Key.SourceIndex;
                    indices.Add(index);
                    if (this.ContainsKey(index))
                    {
                        this[index].Status = failureType;
                        this[index].Reason = failedRecord.Value;
                    }
                }
                if (FailedRecords.ContainsKey(failureType) == false)
                    FailedRecords[failureType] = new IndexList();
                FailedRecords[failureType].AddValues(indices);
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
                using (StreamWriter sw = new StreamWriter(_logFilePath, false))
                    sw.WriteLine(LOG_HEADER);
            }

            public void WriteDetails()
            {
                using (StreamWriter sw = new StreamWriter(_logFilePath, true))
                    sw.Write(Log.DetailsLog);
            }

            public void WriteSummary()
            {
                using (StreamWriter sw = new StreamWriter(_logFilePath, true))
                {
                    sw.WriteLine(Resources.Log_Seperator);
                    sw.Write("\"{0}\"", Log.Summary);
                }
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
                            if( TemporalRecordsCount + PermanentRecordsCount == 0 )
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
            AddedToPendingReview
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
            _consoleWriter.WriteLine("Determining tables/worksheets for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name);
            _consoleWriter.WriteLine();

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
                    _consoleWriter.WriteLine(string.Format(Resources.CmdVali_InvalidTableName, jobParameters.DataSourceInformation.TableName));
                    _consoleWriter.WriteLine(Resources.CmdVali_ValidTableNames);
                    tableNames.ForEach(delegate(string tableName)
                    {
                        _consoleWriter.WriteLine(tableName);
                    });
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_RANGE_BEGIN))
                {
                    int recordCount = (int)allResultsDic[NormalizeJobParametersService.INVALID_RANGE_BEGIN];
                    _consoleWriter.WriteLine(Properties.Resources.RecordsCount, recordCount.ToString());
                    _consoleWriter.WriteLine(Properties.Resources.RangeBeginCount,
                        (jobParameters.ActionRanges[0].RangeBegin + 1).ToString());
                    _consoleWriter.WriteLine(Properties.Resources.CmdVali_RangeBeginValidation);
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE))
                {
                    List<string> invalidDerivedArgValues = allResultsDic[NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE] as List<string>;
                    _consoleWriter.WriteLine(Resources.CmdVali_ArgumentsNotFoundMessage);
                    invalidDerivedArgValues.ForEach(delegate(string invalidDerivedArgValue)
                    {
                        _consoleWriter.WriteLine(invalidDerivedArgValue);
                    });
                    _consoleWriter.WriteLine(Resources.CmdVali_ValidArgumentValues);
                    foreach (string sourceArg in SourceFieldTypes.TypeDefinitions.Keys)
                    {
                        _consoleWriter.WriteLine(sourceArg);
                    }
                    return false;
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_PICKLIST_CODE))
                {
                    List<string> invalidPicklistCodes = allResultsDic[NormalizeJobParametersService.INVALID_PICKLIST_CODE] as List<string>;
                    _consoleWriter.WriteLine(Resources.CmdVali_InvalidPicklistCodes);   // Coverity Fix: CBOE-1946
                    if (invalidPicklistCodes != null)
                    {
                        invalidPicklistCodes.ForEach(delegate(string invalidPicklistCode)
                        {
                            _consoleWriter.WriteLine(invalidPicklistCode);
                        });
                    }
                    return false;
                }
            }

            return true;
        }

        private static void DoCountRecords(JobParameters jobParameters)
        {
            _consoleWriter.WriteLine("Counting records for file '{0}'", jobParameters.DataSourceInformation.DerivedFileInfo.Name);
            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine("Records count: {0}", jobParameters.FileReader.RecordCount);
            _consoleWriter.WriteLine();
        }

        private static void DoSplitFile(JobParameters jobParameters)
        {
            _consoleWriter.WriteLine("Splitting file '{0}' into subset files", jobParameters.DataSourceInformation.DerivedFileInfo.Name);
            _consoleWriter.WriteLine();

            SplitFileService svc = new SplitFileService(jobParameters);
            JobResponse response = svc.DoJob();
            List<string> newFileNames =
                response.ResponseContext[SplitFileService.SPLIT_FILE_RESPONSE] as List<string>;

            _consoleWriter.WriteLine();
            DisplayFileNames(newFileNames);
        }

        private static void DisplayFileNames(List<string> fileNames)
        {
            char underscore = '_';

            _consoleWriter.WriteLine("New files");
            _consoleWriter.WriteLine(underscore.ToString().PadRight(30, underscore));
            if (fileNames == null || fileNames.Count == 0)
                _consoleWriter.WriteLine("No subset files created.");
            else
                foreach (string fileName in fileNames)
                    _consoleWriter.WriteLine(fileName.PadRight(30));

            _consoleWriter.WriteLine();
        }

        private static void DisplayFields(Dictionary<string, Type> fieldDefinitions)
        {
            char underscore = '_';

            _consoleWriter.WriteLine("Field".PadRight(30) + "Type".PadRight(30));
            _consoleWriter.WriteLine(
                underscore.ToString().PadRight(30, underscore) + underscore.ToString().PadRight(30, underscore)
                );

            if (fieldDefinitions == null)
                _consoleWriter.WriteLine("No fields found.");
            else
                foreach (KeyValuePair<string, Type> kvp in fieldDefinitions)
                {
                    string type = (kvp.Value == null) ? "undetermined (no data)" : kvp.Value.ToString().PadRight(30);
                    _consoleWriter.WriteLine(kvp.Key.PadRight(30) + type);
                }

            _consoleWriter.WriteLine();
        }

        private static void DisplayTableNames(List<string> tableNames)
        {
            char underscore = '_';

            _consoleWriter.WriteLine("Tables / Worksheets".PadRight(30));
            _consoleWriter.WriteLine(underscore.ToString().PadRight(30, underscore));
            if (tableNames == null || tableNames.Count == 0)
                _consoleWriter.WriteLine("No tables or worksheets found.");
            else
                foreach (string tableName in tableNames)
                    _consoleWriter.WriteLine(tableName.PadRight(30));

            _consoleWriter.WriteLine();
        }

       
    }

}

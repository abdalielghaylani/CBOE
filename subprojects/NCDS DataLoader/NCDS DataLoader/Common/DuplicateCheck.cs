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

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    /// <summary>
    /// Read records and split records before duplicate check.
    /// </summary>
    public class DuplicateCheck
    {
        /// <summary>
        /// Read records and split records before duplicate check.
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.DestinationRecords is required
        /// <param name="havaStructure">
        /// is the input file include structure
        /// JobParameters.DestinationRecords is required
        /// </param>
        public static Dictionary<ISourceRecord, string> DoDuplicateCheck(JobParameters jobParameters, bool havaStructure,
            int maxValue, ref int uniqueCount, ref int duplicateCount)
        {
            Dictionary<ISourceRecord, string> matchedRecords = new Dictionary<ISourceRecord, string>();
            string DUPLICATE_CHECK_RESPONSE = "Duplicate_Check_Response";
            JobResponse jobResponse;
            IFileReader reader = jobParameters.FileReader;
            //reader.Rewind();
            IndexRange originalRange = null;
            if (!jobParameters.ActionRanges.TryGetValue(0, out originalRange))
                throw new Exception("Programmatic error: the data-range requested could not be extracted.");
            IndexRanges indexRanges = new IndexRanges();
            //if (havaStructure == false)
            {
                indexRanges = jobParameters.ActionRanges;
            }
            // If user didn't specify range, use 1 - <record count> by default.
            originalRange.RangeEnd = Math.Min(reader.RecordCount - 1, originalRange.RangeEnd);

            //Honor the requested index range
            // 1 - ensure we are not surpassing the length of small files
            string strChunkSize = System.Configuration.ConfigurationManager.AppSettings["ChunkSize"].ToString();
            int chunkSize = Convert.ToInt32(strChunkSize); ;
            //Break up the range(s) appropriately
            IndexList originalList = originalRange.ToIndexList(originalRange.RangeBegin, originalRange.RangeEnd);
            IndexRanges splitRanges = originalList.ToIndexRanges(chunkSize);
            string info = string.Empty;
            IndexRanges workerRanges = new IndexRanges();
            List<ISourceRecord> propertyDupRecords = new List<ISourceRecord>();
            List<ISourceRecord> identifierDupRecords = new List<ISourceRecord>();
            //RecordsExtraction extractionService = new RecordsExtraction();
            //MappingServiceClass mappingService = new MappingServiceClass();
            //ValidateRecord validateRecordService = new ValidateRecord();
            RecordsExtractionService extractionService;
            MappingService mappingService;
            ValidateRecordService validateRecordService;

            FindDuplicate findDuplicateService = new FindDuplicate();
            List<RegRecordSummaryInfo> summaryInfoList;
            ISourceRecord matchedRecord;



            for (int i = 0; i < splitRanges.Count; i++)
            {
                workerRanges = new IndexRanges();

                workerRanges.Add(0, splitRanges[i]);
                jobParameters.ActionRanges = workerRanges;
                extractionService = new RecordsExtractionService(jobParameters);

                jobParameters.SourceRecords = extractionService.DoJob().ResponseContext["SourceRecords"] as List<ISourceRecord>;

                //extractionService.Job = jobParameters;
                //jobParameters.SourceRecords = extractionService.DoJob().ResponseContext[CambridgeSoft.COE.DataLoader.Core.Workflow.RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;
                // No structrue and No duplicate check property
                if (havaStructure == false)
                {
                    foreach (ISourceRecord iSR in jobParameters.SourceRecords)
                    {
                        matchedRecords.Add(iSR, string.Empty);
                    }
                }
                else
                {
                    mappingService = new MappingService(jobParameters);
                    jobParameters.DestinationRecords = mappingService.DoJob().ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

                    //mappingService.Job = jobParameters;
                    //jobParameters.DestinationRecords = mappingService.DoJob().ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;
                    //validateRecordService.Job = jobParameters;
                    ////validateRecordService = new ValidateRecordService(jobParameters);
                    //validateRecordService.DoJob();
                    validateRecordService = new ValidateRecordService(jobParameters);
                    validateRecordService.DoJob();

                    // duplicate check
                    jobResponse = findDuplicateService.DoJobInternal(jobParameters);

                    // Dispose destination records
                    if (jobParameters.DestinationRecords != null)
                    {
                        JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                        jobParameters.DestinationRecords = null;
                    }

                    // duplicate result
                    summaryInfoList = null;
                    summaryInfoList = RegistrationLoadList.ConvertFromDupCheckResponses((List<DuplicateCheckResponse>)jobResponse.ResponseContext[DUPLICATE_CHECK_RESPONSE]);

                    {
                        //List<ISourceRecord> uniqueRecords = new List<ISourceRecord>();
                        //List<ISourceRecord> structureDupRecords = new List<ISourceRecord>();

                        for (int j = 0; j < summaryInfoList.Count; j++)
                        {
                            //matched records found
                            if (summaryInfoList[j].DiagnosticLevel == EventLogEntryType.Warning)
                            {
                                matchedRecord = jobParameters.SourceRecords[j];
                                int start = summaryInfoList[j].Message.IndexOf(':') + 1;
                                string strRegNum = summaryInfoList[j].Message.Substring(start, summaryInfoList[j].Message.Length - start);

                                //duplicateCount++;
                                //structureDupRecords.Add(matchedRecord);
                                //if (duplicateCount <= maxValue)
                                //{
                                matchedRecords.Add(matchedRecord, strRegNum);
                                //}

                            }
                            else
                            {
                                //uniqueCount++;

                                //if (uniqueCount <= maxValue)
                                //{
                                    //no matched records
                                    matchedRecords.Add(jobParameters.SourceRecords[j], string.Empty);
                                //}
                                //uniqueRecords.Add(jobParameters.SourceRecords[j]);


                            }
                        }

                        //JobUtility.ExportSourceRecords(structureDupRecords, jobParameters, duplicateFileName, false);
                        //JobUtility.ExportSourceRecords(uniqueRecords, jobParameters, uniqueFileName, false);

                    }
                }
                //// Dispose destination records
                //if (jobParameters.DestinationRecords != null)
                //{
                //    JobUtility.DisposeDestinationRecords(jobParameters.DestinationRecords);
                //    jobParameters.DestinationRecords = null;
                //}

                //int sleepCount = 0;
                //Math.DivRem(i, 5, out sleepCount);
                //if (i >= 0 && sleepCount == 0)
                //{
                //    GC.Collect();
                    
                //    System.Threading.Thread.Sleep(60000);
                //}
            }
            jobParameters.ActionRanges = indexRanges;

            if (jobParameters.FileReader != null)
                jobParameters.FileReader = null;
            return matchedRecords;
        }
    }
}

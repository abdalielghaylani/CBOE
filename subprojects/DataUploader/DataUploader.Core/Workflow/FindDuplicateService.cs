using System;
using System.Diagnostics;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class FindDuplicateService : JobService
    {
        public const string DUPLICATE_CHECK_RESPONSE = "Duplicate_Check_Response";

        /// <summary>
        /// Find duplicated record of the destination records
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.DestinationRecords is required
        /// </param>
        public FindDuplicateService(JobParameters jobParameters) : base(jobParameters) { }

        /// <summary>
        /// // This service only applies when the destination object type is RegistryRecord
        /// </summary>
        /// <returns></returns>
        protected override JobResponse DoJobInternal()
        {
            JobResponse jobResponse = new JobResponse(JobParameters);

            List<IDestinationRecord> destinationRecords = JobParameters.DestinationRecords;
            RegistrationLoadList registrationLoadList = RegistrationLoadList.NewRegistrationLoadList();
            RegistryRecord registryRecord = null;

            for (int j = 0; j < destinationRecords.Count; j++)
            {
                registryRecord = destinationRecords[j] as RegistryRecord;

                if (registryRecord != null)
                {
                    registrationLoadList.AddRegistration(registryRecord);
                }
            }
            
            // TODO: Determine what should be returned from this service.
            //jobResponse.ResponseContext[DUPLICATE_CHECK_RESPONSE] = registrationLoadList.CheckDuplicates();

            List<DuplicateCheckResponse> responses = registrationLoadList.CheckDuplicates();
            jobResponse.ResponseContext[DUPLICATE_CHECK_RESPONSE] = responses;
            //collect information for log file, and categorize source records to be exported
            List<RegRecordSummaryInfo> summaryInfoList = RegistrationLoadList.ConvertFromDupCheckResponses(responses);
            Dictionary<ISourceRecord, string> matchedRecords = new Dictionary<ISourceRecord, string>();
            List<ISourceRecord> uniqueRecords = new List<ISourceRecord>();
            List<ISourceRecord> structureDupRecords = new List<ISourceRecord>();
            List<ISourceRecord> propertyDupRecords = new List<ISourceRecord>();
            List<ISourceRecord> identifierDupRecords = new List<ISourceRecord>();
            List<ISourceRecord> invalidSourceRecordList = new List<ISourceRecord>();
            List<IDestinationRecord> invalidDestinationRecordList = new List<IDestinationRecord>();
            Dictionary<ISourceRecord, string> invalidSourceRecordsDic = new Dictionary<ISourceRecord, string>();

            for (int i = 0; i < summaryInfoList.Count; i++)
            {
                if (summaryInfoList[i].DiagnosticLevel == EventLogEntryType.Warning)//matched records found
                {
                    ISourceRecord matchedRecord=JobParameters.SourceRecords[i];
                    matchedRecords.Add(matchedRecord, summaryInfoList[i].Message);
                    switch (responses[i].Mechanism)
                    {
                        case PreloadDupCheckMechanism.Structure:
                            structureDupRecords.Add(matchedRecord);
                            break;
                        case PreloadDupCheckMechanism.ComponentProperty:
                            propertyDupRecords.Add(matchedRecord);
                            break;
                        case PreloadDupCheckMechanism.ComponentIdentifier:
                            identifierDupRecords.Add(matchedRecord);
                            break;
                        case PreloadDupCheckMechanism.None:
                            if(summaryInfoList[i].Message.Contains("Error"))                            
                            {
                                invalidSourceRecordsDic.Add(JobParameters.SourceRecords[i], JobParameters.DestinationRecords[i].ValidationErrorMessage);
                                invalidDestinationRecordList.Add(JobParameters.DestinationRecords[i]);
                            }
                            break;
                    }
                }
                else
                    uniqueRecords.Add(JobParameters.SourceRecords[i]);
            }
            //export‘unique’,‘structuredup’,‘propertydup’and "identifierdup" records to different files 
            string exportedFilePath;
            if(uniqueRecords.Count!=0)
            {
                exportedFilePath=JobUtility.GetPurposedFilePath("unique",JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(uniqueRecords, JobParameters, exportedFilePath, false);
            }
            if (structureDupRecords.Count != 0)
            {
                exportedFilePath = JobUtility.GetPurposedFilePath("structuredup", JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(structureDupRecords, JobParameters, exportedFilePath, false);
            }
            if (propertyDupRecords.Count != 0)
            {
                exportedFilePath = JobUtility.GetPurposedFilePath("propertydup", JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(propertyDupRecords, JobParameters, exportedFilePath, false);
            }
            if (identifierDupRecords.Count != 0)
            {
                exportedFilePath = JobUtility.GetPurposedFilePath("identifierdup", JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(identifierDupRecords, JobParameters, exportedFilePath, false);
            }
            invalidSourceRecordList = new List<ISourceRecord>(invalidSourceRecordsDic.Keys);
            if (invalidSourceRecordList.Count > 0)
            {
                string jobFileName = JobUtility.GetPurposedFilePath("invalid", JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(invalidSourceRecordList, JobParameters, jobFileName, false);
                JobUtility.HandleInvalidRecords(invalidSourceRecordList, invalidDestinationRecordList, JobParameters);
            }
            OnRecordsDupChecked(matchedRecords ,destinationRecords.Count);
            return jobResponse;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.DestinationRecords != null;
        }

        private void OnRecordsDupChecked(Dictionary<ISourceRecord,string> sourceRecords, int count)
        {
            base.OnJobComplete(this,new RecordsProcessedEventArgs(sourceRecords,count));
        }
    }
}

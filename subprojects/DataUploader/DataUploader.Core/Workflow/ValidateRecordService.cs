using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Framework.Services;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class ValidateRecordService : JobService
    {
        public const string INVALID_RECORDS = "Invalid Records";
        public event EventHandler<RecordProcessingEventArgs> RecordProcessing;

        /// <summary>
        /// Validate destination records
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.SourceRecords and JobParameters.DestinationRecords are required
        /// </param>
        public ValidateRecordService(JobParameters jobParameters) : base(jobParameters) { }

        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);

            List<ISourceRecord> invalidSourceRecordList = new List<ISourceRecord>();
            List<ISourceRecord> validSourceRecordList=new List<ISourceRecord> ();
            List<IDestinationRecord> invalidDestinationRecordList = new List<IDestinationRecord>();
            Dictionary<ISourceRecord, string> invalidSourceRecordsDic = new Dictionary<ISourceRecord, string>();

            bool exportValidatedRecords = JobParameters.TargetActionType == TargetActionType.ValidateData ? true : false;
            int originalDstRecordsCount=JobParameters.DestinationRecords.Count;
            for (int i = 0; i < originalDstRecordsCount; i++)
            {
                OnRecordProcessing(JobParameters.SourceRecords[i]);
                if (!JobParameters.DestinationRecords[i].IsValid)
                {
                    invalidSourceRecordsDic.Add(JobParameters.SourceRecords[i], JobParameters.DestinationRecords[i].ValidationErrorMessage);
                    invalidDestinationRecordList.Add(JobParameters.DestinationRecords[i]);
                }
                else if (exportValidatedRecords)
                    validSourceRecordList.Add(JobParameters.SourceRecords[i]);
            }

            invalidSourceRecordList = new List<ISourceRecord>(invalidSourceRecordsDic.Keys);
            if (invalidSourceRecordList.Count > 0)
            {
                if (JobParameters.TargetActionType == TargetActionType.FindDuplicates)
                {
                    string jobFileName = JobUtility.GetPurposedFilePath("invalid", JobParameters.DataSourceInformation.DerivedFileInfo);
                    JobUtility.ExportSourceRecords(invalidSourceRecordList, JobParameters, jobFileName, false);
                }
                JobUtility.HandleInvalidRecords(invalidSourceRecordList, invalidDestinationRecordList, JobParameters);
            }
            if (validSourceRecordList.Count > 0)
            {
                string validFileName = JobUtility.GetPurposedFilePath("validated", JobParameters.DataSourceInformation.DerivedFileInfo);
                JobUtility.ExportSourceRecords(validSourceRecordList, JobParameters, validFileName, false);
            }

            response.ResponseContext[INVALID_RECORDS] = invalidSourceRecordList;

            if (JobParameters.SourceRecords.Count != JobParameters.DestinationRecords.Count)
                throw new Exception("Programmatic error while handling invalid source records");

            OnRecordsValidated(invalidSourceRecordsDic, originalDstRecordsCount);
            return response;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.SourceRecords != null &&
                JobParameters.DestinationRecords != null;
        }

        /// <summary>
        /// Rise RecordValidating event
        /// </summary>
        /// <param name="sourceRecordIndex">The source record index corresponding to the destination record</param>
        private void OnRecordProcessing(ISourceRecord sourceRecord)
        {
            if (RecordProcessing != null)
                RecordProcessing(this, new RecordProcessingEventArgs(sourceRecord));
        }

        /// <summary>
        /// Rise JobComplete event
        /// </summary>
        /// <param name="invalidCount">number of invalid destination records that are validated</param>
        /// <param name="validCount">number of valid destination records that are validated</param>
        private void OnRecordsValidated(Dictionary<ISourceRecord,string> invalidRecords, int count)
        {
            base.OnJobComplete(this, new RecordsProcessedEventArgs(invalidRecords, count));
        }
    }
}

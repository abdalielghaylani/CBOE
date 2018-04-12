using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class ValidationRecordService : JobService
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
        public ValidationRecordService(JobParameters jobParameters) : base(jobParameters) { }

        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);

            List<ISourceRecord> invalidRecordsList = new List<ISourceRecord>();
            Dictionary<ISourceRecord, string> invalidRecordsDic = new Dictionary<ISourceRecord, string>();

            for (int i = 0; i < JobParameters.DestinationRecords.Count; i++)
            {
                OnRecordProcessing(JobParameters.SourceRecords[i]);
                if (!JobParameters.DestinationRecords[i].IsValid)
                {
                    //TODO:Get the destination records' error message
                    invalidRecordsDic.Add(JobParameters.SourceRecords[i],string.Empty);
                }
            }
            invalidRecordsList = new List<ISourceRecord>(invalidRecordsDic.Keys);
            if (invalidRecordsList.Count > 0)
            {
                string jobFileName = JobUtility.GetPurposedFilePath("invalid", JobParameters.DataSourceInformation.DerivedFileInfo);

                JobUtility.ExportSourceRecords(invalidRecordsList, JobParameters, jobFileName, false);
            }
            response.ResponseContext[INVALID_RECORDS] = invalidRecordsList;

            OnRecordsValidated(invalidRecordsDic, JobParameters.DestinationRecords.Count);
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

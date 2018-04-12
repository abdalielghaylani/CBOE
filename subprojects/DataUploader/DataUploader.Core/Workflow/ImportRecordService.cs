using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.Properties;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class ImportRecordService : JobService
    {
        public const string IMPORT_FAILED_RECORDS = "Import failed records";
        public const string IMPORT_RESULT = "ImportResult";
        public event EventHandler<RecordProcessingEventArgs> RecordProcessing;

        /// <summary>
        /// Import the destination records to the database.
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.DestinationRecords is required.
        /// </param>
        public ImportRecordService(JobParameters jobParameters) : base(jobParameters) { }

        protected override JobResponse DoJobInternal()
        {
            JobResponse jobResponse = new JobResponse(JobParameters);

            List<IDestinationRecord> destinationRecords = JobParameters.DestinationRecords;
            RegistrationLoadList registrationLoadList = RegistrationLoadList.NewRegistrationLoadList();
            RegistryRecord registryRecord = null;

            for (int i = 0; i < destinationRecords.Count; i++)
            {
                OnRecordProcessing(JobParameters.SourceRecords[i]);                
                registryRecord = destinationRecords[i] as RegistryRecord;
                if (registryRecord != null)
                {
                    //JiraID: CBOE-1158
                    registryRecord.ModuleName = CambridgeSoft.COE.Registration.Services.Types.ChemDrawWarningChecker.ModuleName.DATALOADER2;
                    registrationLoadList.AddRegistration(registryRecord);
                }
            }

      
            List<RegRecordSummaryInfo> importResult = null;

            switch (JobParameters.TargetActionType)
            {
                case TargetActionType.ImportRegDupAsCreateNew:
                    importResult = registrationLoadList.Register(DuplicateAction.Duplicate);

                    break;
                case TargetActionType.ImportRegDupAsNewBatch:
                    importResult = registrationLoadList.Register(DuplicateAction.Batch);

                    break;
                case TargetActionType.ImportRegDupNone:
                    importResult = registrationLoadList.Register(DuplicateAction.None);

                    break;
                case TargetActionType.ImportRegDupAsTemp:
                    importResult = registrationLoadList.Register(DuplicateAction.Temporary);                     
                    break;
                case TargetActionType.ImportTemp:
                    importResult = registrationLoadList.Submit();
                    break;
                default:
                    throw new ArgumentException("Invalid target action");
            }

            // TODO: Determine what should be returned from this service.
            //jobResponse.ResponseContext[DUPLICATE_CHECK_RESPONSE] = importResult;

            //Fetch log information
            Dictionary<ISourceRecord, RegRecordSummaryInfo> recordsSummaryInfo = new Dictionary<ISourceRecord, RegRecordSummaryInfo>();
            for (int i = 0; i < JobParameters.SourceRecords.Count; i++)
            {
                //update message of RegRecordSummaryInfo to contain matching Registrations                
                if (string.IsNullOrEmpty(importResult[i].Message) && //RegRecordSummaryInfo.Message is not null means an exception when registering
                    !string.IsNullOrEmpty(importResult[i].DuplicateMessage))
                {
                    importResult[i].Message = importResult[i].DuplicateMessage;
                }
                else if (!string.IsNullOrEmpty(importResult[i].Message))
                {
                    if (importResult[i].Message.Contains("ORA-12899"))
                    {
                        int requiredMessageLoc = importResult[i].Message.IndexOf(":", importResult[i].Message.IndexOf("ORA-12899")) + 1;
                        importResult[i].Message = importResult[i].Message.Substring(requiredMessageLoc, importResult[i].Message.IndexOf("ORA-", requiredMessageLoc) - requiredMessageLoc);
                    }
                    else if (importResult[i].Message.Contains("ORA-00001:") && importResult[i].Message.ToUpper().Contains("UNIQUE CONSTRAINT") && importResult[i].Message.ToUpper().Contains("REGDB.UNQ_REGNUM_REG_NUMBER"))
                    {
                        importResult[i].Message = Resources.KnownRegNumMessage;
                    }
                }              
                recordsSummaryInfo.Add(JobParameters.SourceRecords[i], importResult[i]);
            }
            jobResponse.ResponseContext[IMPORT_RESULT] = importResult;
            OnRecordsImported(recordsSummaryInfo);

            return jobResponse;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.DestinationRecords != null;
        }

        /// <summary>
        /// Rise RecordImporting event
        /// </summary>
        /// <param name="sourceRecordIndex">The source record index corresponding to the destination record being imported.</param>
        private void OnRecordProcessing(Contracts.ISourceRecord sourceRecord)
        {
            if (RecordProcessing != null)
                RecordProcessing(this, new RecordProcessingEventArgs(sourceRecord));
        }

        /// <summary>
        /// Rise RecordsChunkImported event
        /// </summary>
        /// <param name="chunkSize">The count of destination imported records</param>
        private void OnRecordsImported(Dictionary<ISourceRecord, RegRecordSummaryInfo> recordsSummaryInfo)
        {
            base.OnJobComplete(this, new RecordsImportedEventArgs(recordsSummaryInfo));
        }
    }
}

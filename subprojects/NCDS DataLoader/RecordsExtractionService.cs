using System;
using System.Collections.Generic;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// Abstracts the retrieval of ISourceRecords from the underlying data-file using an
    /// IFileReader instance (either provided by the constructor or created from the DataSourceInformation
    /// property of the JobParameters).
    /// </summary>
    public class RecordsExtractionService:JobService
    {
        public event EventHandler<RecordProcessingEventArgs> RecordProcessing;
        public const string SOURCERECORDS = "SourceRecords";

        private IFileReader _reader = null;

        /// <summary>
        /// Constructor. Accepts working parameters in the the form of a <seealso cref="JobParameters"/>
        /// instance.
        /// </summary>
        /// <param name="jobParameters">
        /// pertinent information includes data-file details and the range(s) of records to retrieve
        /// </param>
        public RecordsExtractionService(JobParameters jobParameters)
            : base(jobParameters)
        {}

        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);
            List<ISourceRecord> extractedRecords = new List<ISourceRecord>();

            //create the file reader only as necessary
            _reader = JobParameters.FileReader;
            if (_reader == null)
                _reader = FileReaderFactory.FetchReader(JobParameters.DataSourceInformation);

            foreach (IndexRange range in JobParameters.ActionRanges.Values)
            {
                //rewind the reader only if it is currently past the desired index range
                if (_reader.CurrentRecordIndex > range.RangeBegin)
                    _reader.Rewind();

                _reader.Seek(range.RangeBegin);

                IndexList indices = range.ToIndexList();

                // Loop over the indexes...
                // ...they're contiguous by definition becuase they came from a single 'range'
                foreach (int i in indices)
                {
                    ISourceRecord record = _reader.GetNext();
                    if (record != null)
                    {
                        OnRecordProcessing(record);
                        extractedRecords.Add(record);
                    }
                    else
                    {
                        // TODO: Add reactions to source file parsing failure.
                        break;
                    }
                }
            }
            //TODO:determin how to collect unextracted records
            OnRecordsExtracted(extractedRecords.Count);
            response.ResponseContext[RecordsExtractionService.SOURCERECORDS] = extractedRecords;
            return response;
        }

        /// <summary>
        /// JobParameters not null, and should know table/worksheet name for Access and Excel source file. 
        /// </summary>
        /// <returns></returns>
        protected override bool AreJobParametersValid()
        {
            if (JobParameters != null)
            {
                if (JobParameters.FileReader!=null)
                    return true;
                else if (JobParameters.DataSourceInformation != null &&
                JobParameters.DataSourceInformation.DerivedFileInfo.Exists)
                {
                    if (JobParameters.DataSourceInformation.FileType == SourceFileType.MSAccess ||
                        JobParameters.DataSourceInformation.FileType == SourceFileType.MSExcel)
                    {
                        return string.IsNullOrEmpty(JobParameters.DataSourceInformation.TableName) ? false : true;
                    }
                    else return true;
                }
                else return false;
            }
            else
                return false;
        }

        private void OnRecordProcessing(ISourceRecord sourceRecord)
        {
            if (RecordProcessing != null)
                RecordProcessing(this, new RecordProcessingEventArgs(sourceRecord));
        }

        private void OnRecordsExtracted(int count)
        {
            OnJobComplete(this, new RecordsProcessedEventArgs(null, count));
        }
    }
}

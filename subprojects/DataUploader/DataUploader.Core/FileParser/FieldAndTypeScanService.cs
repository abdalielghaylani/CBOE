using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class FieldAndTypeScanService : JobService
    {
        public const string TYPE_DEFINITIONS = "Type Definitions";

        public FieldAndTypeScanService(JobParameters jobParameters) : base(jobParameters) { }

        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);
            IndexRange range = JobParameters.ActionRanges[0];
            IFileReader reader = null;

            try
            {
                reader = JobParameters.FileReader;
                //Coverity fix - CID 19192 
                if (reader == null)
                {
                    if (FileParser.FileReaderFactory.FetchReader(JobParameters.DataSourceInformation) != null)
                    {
                        reader = FileParser.FileReaderFactory.FetchReader(JobParameters.DataSourceInformation);
                    }
                }
                if (reader != null)
                {
                    reader.Seek(range.RangeBegin);
                    do
                    {
                        reader.GetNext();
                    } while (reader.Current != null && reader.CurrentRecordIndex <= range.RangeEnd);
                }
                // Return the global type definitions
                response.ResponseContext[TYPE_DEFINITIONS] = SourceFieldTypes.TypeDefinitions;
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("invalid delimiter"))
                    throw exception;
                COEExceptionDispatcher.HandleUIException(exception);
            }

            return response;
        }

        protected override bool AreJobParametersValid()
        {
            if (JobParameters.ActionRanges == null
                || JobParameters.ActionRanges.Count == 0
                || JobParameters.DataSourceInformation == null)
                return false;

            return true;
        }
    }
}

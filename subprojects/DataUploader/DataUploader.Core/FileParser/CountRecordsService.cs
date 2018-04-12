using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class CountRecordsService : JobService
    {
        public const string RECORD_COUNT = "RecordCount";

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameters object.
        /// JobParameters.DataSourceInformation is required, and should contains FullFilePath,FileType
        /// </param>
        public CountRecordsService(JobParameters jobParameters) : base(jobParameters) { }

        /// <summary>
        /// Count the amount of records contained in a specific file to import.
        /// </summary>
        /// <returns>The count of total records.</returns>
        protected override CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse DoJobInternal()
        {
            IFileReader reader = JobParameters.FileReader;
            int count = reader.CountAll();

            JobResponse jobResponse = new JobResponse(JobParameters);
            jobResponse.ResponseContext[RECORD_COUNT] = count;

            return jobResponse;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.FileReader != null;
        }
    }
}

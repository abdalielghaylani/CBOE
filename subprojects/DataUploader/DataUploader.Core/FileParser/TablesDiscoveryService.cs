using System;
using System.Collections.Generic;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Access;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Excel;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// A job service that lists the names of all the tables in an Access file, 
    /// or worksheets in an Excel file.
    /// </summary>
    public class TablesDiscoveryService:JobService
    {
        public const string RESPONSE_TABLENAMESLIST = "TableNamesList";

        /// <summary>
        /// Instantiate the job service using the JobParameter
        /// </summary>
        /// <param name="jobParameters">
        /// A JobParameter object.
        /// JobParameter.DataSourceInformation is required.
        /// </param>
        public TablesDiscoveryService(JobParameters jobParameters):base(jobParameters)
        {}

        protected override bool AreJobParametersValid()
        {
            return JobParameters.DataSourceInformation != null &&
                JobParameters.DataSourceInformation.DerivedFileInfo.Exists;
        }

        protected override Workflow.JobResponse DoJobInternal()
        {
            Workflow.JobResponse response = new Workflow.JobResponse(JobParameters);
            List<string> tableNames=null;
            SourceFileInfo fileInformation = JobParameters.DataSourceInformation;

            switch (fileInformation.FileType)
            {
                case SourceFileType.MSAccess:
                    {

                        tableNames = AccessOleDbReader.FetchTableNames(fileInformation);
                        break;
                    }
                case SourceFileType.MSExcel:
                    {
                        tableNames = ExcelOleDbReader.FetchTableNames(fileInformation);
                        break;
                    }
            }
            response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST] = tableNames;
            return response;
        }


    }
}

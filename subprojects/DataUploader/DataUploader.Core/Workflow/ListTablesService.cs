using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.FileParser.Access;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Excel;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class ListTablesService : JobService
    {
        public const string TABLE_NAMES = "Table Names";

        public ListTablesService(JobParameters jobParameters) : base(jobParameters) { }

        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);

            SourceFileInfo fileInformation = JobParameters.DataSourceInformation;
            List<string> tableNames = new List<string>();

            switch (fileInformation.FileType)
            {
                case SourceFileType.MSAccess:
                    {
                        AccessOleDbReader reader = new AccessOleDbReader(
                            fileInformation.FullFilePath
                            , null
                            , MSOfficeVersion.Unknown
                        );
                        tableNames = reader.GetTableAndViewList();
                        break;
                    }
                case SourceFileType.MSExcel:
                    {
                        ExcelOleDbReader reader = new ExcelOleDbReader(
                            fileInformation.FullFilePath
                            , null
                            , MSOfficeVersion.Unknown
                            , false
                        );
                        tableNames = reader.GetWorksheetsList();
                        break;
                    }
                case SourceFileType.SDFile:
                //{
                //    tableNames.Add("SDTable"); break;
                //}
                case SourceFileType.CSV:
                //{
                //    tableNames.Add("CSVTable"); break;
                //}
                case SourceFileType.Unknown:
                    {
                        break;
                    }
                default: break;
            }

            response.ResponseContext[TABLE_NAMES] = tableNames;

            return response;
        }

        protected override bool AreJobParametersValid()
        {
            return JobParameters.DataSourceInformation != null;
        }
    }
}

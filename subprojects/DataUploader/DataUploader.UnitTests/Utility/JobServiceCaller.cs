using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader
{
    public static class JobServiceCaller
    {
        public static JobResponse CountRecords(JobParameters jobParameters)
        {
            CountRecordsService service = new CountRecordsService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse ListFields(JobParameters jobParameters)
        {
            FieldAndTypeScanService service = new FieldAndTypeScanService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse ListTables(JobParameters jobParameters)
        {
            TablesDiscoveryService service = new TablesDiscoveryService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse SplitFile(JobParameters jobParameters)
        {
            SplitFileService service = new SplitFileService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse ExtractRecords(JobParameters jobParameters)
        {
            RecordsExtractionService service = new RecordsExtractionService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse MapRecords(JobParameters jobParameters)
        {
            MappingService service = new MappingService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse ValidateRecords(JobParameters jobParameters)
        {
            JobService mapJobService = new MappingService(jobParameters);
            return mapJobService.DoJob();
        }

        public static JobResponse FindDuplicate(JobParameters jobParameters)
        {
            FindDuplicateService service = new FindDuplicateService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse ImportRecord(JobParameters jobParameters)
        {
            ImportRecordService service = new ImportRecordService(jobParameters);
            return service.DoJob();
        }

        public static JobResponse NormalizeJobParameters(JobParameters jobParamters)
        {
            NormalizeJobParametersService service = new NormalizeJobParametersService(jobParamters);
            return service.DoJob();
        }
    }
}

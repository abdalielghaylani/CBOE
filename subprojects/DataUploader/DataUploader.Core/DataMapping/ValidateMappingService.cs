using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using System.Collections;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

namespace CambridgeSoft.COE.DataLoader.Core.DataMapping
{
    /*
     * This service basically does the following things:
     * 1. Validate all specified derived argument values exist in source field definition, in a case-insensitive
     * manner. Normalized the value, if needed.
     * 2. Validate if pickListCode in mapping file exists in database, in a case-insensitive manner.
     * Normalized the value, if needed.
     * 3. Validate if table name or worksheet name exists in source file, in a case-insensitive manner.
     * Normalized the value, if needed.
     * 4. Validate the input range's Begin value is less than or equal to the record count of source file.
     * 
     */
    public class NormalizeJobParametersService : JobService
    {
        public const string HAS_PASSED_VALIDATION = "HasPassedValition";
        public const string INVALID_DERIVED_ARG_LIST = "InvalidDerivedArgList";

        //private const string TEMPLATE_INVALID_TABLE_NAME = "Table name '{0}' doesn't exist in source file";
        //private const string TEMPLATE_INVALID_RANGE_BEGIN = "Specified range's start is larger than the record count in source file";
        //private const string TEMPLATE_INVALID_DERIVED_ARG_VALUE = "Derived argument value '{0}' doesn't exist in source file";
        //private const string TEMPLATE_INVALID_PICKLIST_CODE = "Picklist with code '{0}' doesn't exist";

        public const string DIC_ALL_RESULTS = "AllResultsDic";
        public const string INVALID_TABLE_NAME = "InvalidTableName";
        public const string INVALID_RANGE_BEGIN = "InvalidRangeBegin";
        public const string INVALID_DERIVED_ARG_VALUE = "InvalidDerivedArgValue";
        public const string INVALID_PICKLIST_CODE = "InvalidPicklistCode";

        Dictionary<string, object> allResultsDic = null;
        
        public NormalizeJobParametersService(JobParameters jobParameters) : base(jobParameters) { }

        #region Override members

        protected override JobResponse DoJobInternal()
        {
            allResultsDic = new Dictionary<string,object>();
            JobResponse response = new JobResponse(JobParameters);
            response.ResponseContext[DIC_ALL_RESULTS] = allResultsDic;
            // Ideally, we should define an xml schema to represent the result from this service. But for now,
            // we simply return lists of messages for each subprocess respectively.

            NormalizeTableName();
            if (allResultsDic.ContainsKey(INVALID_TABLE_NAME))
                return response;

            // Initialize the file reader, which needs to be preserved
            // and kept open throughout the whole session lifecycle.
            JobParameters.FileReader = FileReaderFactory.FetchReader(JobParameters.DataSourceInformation);

            FieldAndTypeScanService ftSvc = new FieldAndTypeScanService(JobParameters);
            ftSvc.DoJob();

            CountRecordsService countRecordsService = new CountRecordsService(JobParameters);
            countRecordsService.DoJob();

            ValidateRangeBeginValue();
            if (allResultsDic.ContainsKey(INVALID_RANGE_BEGIN))
                return response;

            if (JobParameters.TargetActionType == TargetActionType.FindDuplicates ||
                JobParameters.TargetActionType == TargetActionType.ImportRegDupAsCreateNew ||
                JobParameters.TargetActionType == TargetActionType.ImportRegDupAsNewBatch ||
                JobParameters.TargetActionType == TargetActionType.ImportRegDupAsTemp ||
                JobParameters.TargetActionType == TargetActionType.ImportRegDupNone ||
                JobParameters.TargetActionType == TargetActionType.ImportTemp ||
                JobParameters.TargetActionType == TargetActionType.ValidateData ||
                JobParameters.TargetActionType == TargetActionType.ValidateMapping)
            {
                NormalizeDerivedArgValues();
                NormalizePickListCodes();
            }

            return response;
        }

        protected override bool AreJobParametersValid()
        {
            return true;
        }

        #endregion

        #region Normalize derived argument value

        private void NormalizeDerivedArgValues()
        {
            List<string> invalidDerivedArgValues = new List<string>();

            List<Mappings.Arg> args = GetArgsWithDerivedValue();
            string normalizedArgValue = string.Empty;
            
            foreach (Mappings.Arg arg in args)
            {
                normalizedArgValue = this.NormalizeDerivedArgValue(arg.Value);

                if (string.IsNullOrEmpty(normalizedArgValue))
                    invalidDerivedArgValues.Add(arg.Value);
                else
                    arg.Value = normalizedArgValue;
            }

            if (invalidDerivedArgValues.Count > 0)
                allResultsDic.Add(INVALID_DERIVED_ARG_VALUE, invalidDerivedArgValues);
        }

        private List<Mappings.Arg> GetArgsWithDerivedValue()
        {
            List<Mappings.Arg> args = new List<Mappings.Arg>();

            foreach (Mappings.Mapping mapping in JobParameters.Mappings.MappingCollection)
            {
                if (mapping.Enabled)
                {
                    foreach (Mappings.Arg arg in mapping.MemberInformation.Args)
                    {
                        if (arg.Input == Mappings.InputEnum.Derived)
                            args.Add(arg);
                    }
                }
            }

            return args;
        }

        /// <summary>
        /// Normalizes the picklistCode to its case-insensitive equivalent.
        /// </summary>
        private string NormalizeDerivedArgValue(string argValue)
        {
            if (string.IsNullOrEmpty(argValue))
                throw new ArgumentNullException("picklistCode");

            string normalizedDerivedArgValue = string.Empty;

            foreach (KeyValuePair<string, Type> kvp in SourceFieldTypes.TypeDefinitions)
            {
                if (string.Compare(kvp.Key, argValue, true) == 0)
                {
                    normalizedDerivedArgValue = kvp.Key;
                    break;
                }
            }

            return normalizedDerivedArgValue;
        }

        #endregion

        #region Normalize PicklistCode

        private void NormalizePickListCodes()
        {
            List<string> invalidPicklistCodes = new List<string>();

            List<Mappings.Arg> args = GetArgsWithCode();
            string normalizedPicklistCode = string.Empty;
            
            foreach (Mappings.Arg arg in args)
            {
                normalizedPicklistCode = JobUtility.NormalizePicklistCode(arg.PickListCode);

                if (string.IsNullOrEmpty(normalizedPicklistCode))
                    invalidPicklistCodes.Add(arg.PickListCode);
                else
                    arg.PickListCode = normalizedPicklistCode;
            }

            if (invalidPicklistCodes.Count > 0)
                allResultsDic.Add(INVALID_PICKLIST_CODE, invalidPicklistCodes);
        }

        /// <summary>
        /// Retrieves a list of arguments that have picklistCode value. We get the list of Arg instead of
        /// list of picklistCode for the purpose of further picklistCode normalization process.
        /// </summary>
        /// <returns>A list of arguments that have picklistCode value.</returns>
        private List<Mappings.Arg> GetArgsWithCode()
        {
            List<Mappings.Arg> args = new List<Mappings.Arg>();

            foreach (Mappings.Mapping mapping in JobParameters.Mappings.MappingCollection)
            {
                if (mapping.Enabled)
                {
                    foreach (Mappings.Arg arg in mapping.MemberInformation.Args)
                    {
                        if (!string.IsNullOrEmpty(arg.PickListCode))
                            args.Add(arg);
                    }
                }
            }
            
            return args;
        }

        #endregion

        #region Normalize table or worksheet name

        private void NormalizeTableName()
        {
            if (!string.IsNullOrEmpty(JobParameters.DataSourceInformation.TableName) &&
                (JobParameters.DataSourceInformation.FileType == SourceFileType.MSAccess ||
                JobParameters.DataSourceInformation.FileType == SourceFileType.MSExcel))
            {
                TablesDiscoveryService tblSvc = new TablesDiscoveryService(JobParameters);
                JobResponse response = tblSvc.DoJob();
                List<string> tableNames =
                    response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST] as List<string>;

                if (tableNames != null && tableNames.Count > 0)
                {
                    foreach (string tableName in tableNames)
                    {
                        if (string.Compare(tableName, JobParameters.DataSourceInformation.TableName, true) == 0)
                        {
                            JobParameters.DataSourceInformation.TableName = tableName;
                            return;
                        }
                    }
                }

                allResultsDic.Add(INVALID_TABLE_NAME, tableNames);
            }
        }

        #endregion

        #region Validate range's begin value

        private void ValidateRangeBeginValue()
        {
            if (JobParameters.ActionRanges[0].RangeBegin >= JobParameters.FileReader.RecordCount)
                allResultsDic.Add(INVALID_RANGE_BEGIN, JobParameters.FileReader.RecordCount);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.NCDS_DataLoader.Properties;
using System.Diagnostics;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    /// <summary>
    /// Read input file and get property types from input file .
    /// </summary>
    /// <param name="jobParameters">The job parameters instructing this unattended loader session.</param>
    public class ReadInputFile
    {
        public static string DoReadInputFile(JobParameters jobParameters)
        {
            string message = DoNormalizeJobParameters(jobParameters);
            if (!string.IsNullOrEmpty(message))
            {
                return message;
            }

            if (jobParameters.TargetActionType == TargetActionType.SplitFile)
            {
                return DoSplitFile(jobParameters);
            }

            return string.Empty;
        }
        private static string DoNormalizeJobParameters(JobParameters jobParameters)
        {
            StringBuilder message = new StringBuilder();
            NormalizeJobParametersService normalizeJobParametersService = new NormalizeJobParametersService(jobParameters);
            JobResponse normalizeJobParametersResponse = normalizeJobParametersService.DoJob();
            Dictionary<string, object> allResultsDic = normalizeJobParametersResponse.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;
            if (allResultsDic != null)
            {
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_TABLE_NAME))
                {
                    List<string> tableNames = allResultsDic[NormalizeJobParametersService.INVALID_TABLE_NAME] as List<string>;
                    message.Append(string.Format(Resources.CmdVali_InvalidTableName, jobParameters.DataSourceInformation.TableName));
                    message.Append("\n");
                    message.Append(Resources.CmdVali_ValidTableNames);
                    message.Append("\n");
                    tableNames.ForEach(delegate(string tableName)
                    {
                        message.Append(tableName);
                        message.Append("\n");
                    });
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_RANGE_BEGIN))
                {
                    message.Append("INVALID_RANGE_BEGIN");
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE))
                {
                    List<string> invalidDerivedArgValues = allResultsDic[NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE] as List<string>;
                    message.Append(Resources.CmdVali_ArgumentsNotFoundMessage);
                    message.Append("\n");
                    invalidDerivedArgValues.ForEach(delegate(string invalidDerivedArgValue)
                    {
                        message.Append(invalidDerivedArgValue);
                        message.Append("\n");
                    });
                    message.Append(Resources.CmdVali_ValidArgumentValues);
                    message.Append("\n");
                    foreach (string sourceArg in SourceFieldTypes.TypeDefinitions.Keys)
                    {
                        message.Append(sourceArg);
                        message.Append("\n");
                    }
                }
                if (allResultsDic.ContainsKey(NormalizeJobParametersService.INVALID_PICKLIST_CODE))
                {
                    List<string> invalidPicklistCodes = allResultsDic[NormalizeJobParametersService.INVALID_PICKLIST_CODE] as List<string>;
                    message.Append(Resources.CmdVali_InvalidPicklistCodes);
                    message.Append("\n");
                    invalidPicklistCodes.ForEach(delegate(string invalidPicklistCode)
                    {
                        message.Append(invalidPicklistCode);
                        message.Append("\n");
                    });
                }
            }
            return message.ToString();
        }

        private static string DoSplitFile(JobParameters jobParameters)
        {
            SplitFileService svc = new SplitFileService(jobParameters);
            JobResponse response = svc.DoJob();
            List<string> newFileNames =
                response.ResponseContext[SplitFileService.SPLIT_FILE_RESPONSE] as List<string>;

            return DisplayFileNames(newFileNames, jobParameters);
        }

        private static string DisplayFileNames(List<string> fileNames, JobParameters jobParameters)
        {
            StringBuilder splitMessage = new StringBuilder();
            splitMessage.Append(string.Format("Records count:{0} \r", jobParameters.FileReader.RecordCount));
            splitMessage.Append("New files: \r");

            if (fileNames == null || fileNames.Count == 0)
                splitMessage.Append("No subset files created.");
            else
                foreach (string fileName in fileNames)
                    splitMessage.Append(fileName.PadRight(30) + "\r");

            return splitMessage.ToString();
        }
    }
}

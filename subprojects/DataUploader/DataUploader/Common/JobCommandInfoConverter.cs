using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.Workflow;

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// Helper utility to transform command-line information into functional job parameters.
    /// </summary>
    public class JobCommandInfoConverter
    {
        /// <summary>
        /// Converts the parsed, validated command-line information into a JobParameters instance.
        /// </summary>
        /// <param name="cmdInfo">the parsed command-line arguments</param>
        /// <returns>an initialized JobParameters instance ready for execution</returns>
        public static JobParameters ConvertToJobParameters(IndividualArgumentsCommandInfo cmdInfo)
        {
            SourceFileInfo source = ConvertToSourceFileData(cmdInfo);
            IndexRanges ranges = ConvertToIndexRanges(cmdInfo);
            Mappings maps = null;

            //get mapping file required TargetActionType collection
            List<TargetActionType> mappingFileRequiredTypes = new List<TargetActionType>();
            mappingFileRequiredTypes.Add(TargetActionType.FindDuplicates);
            mappingFileRequiredTypes.Add(TargetActionType.ImportRegDupAsCreateNew);
            mappingFileRequiredTypes.Add(TargetActionType.ImportRegDupAsNewBatch);
            mappingFileRequiredTypes.Add(TargetActionType.ImportRegDupAsTemp);
            mappingFileRequiredTypes.Add(TargetActionType.ImportRegDupNone);
            mappingFileRequiredTypes.Add(TargetActionType.ImportTemp);
            mappingFileRequiredTypes.Add(TargetActionType.ValidateData);
            mappingFileRequiredTypes.Add(TargetActionType.ValidateMapping);

            if (!string.IsNullOrEmpty(cmdInfo.MappingFile) &&
                mappingFileRequiredTypes.Contains(cmdInfo.ActionType))
            {
                maps = Mappings.GetMappingsFromFile(cmdInfo.MappingFile);
            }

            JobParameters jobParameters = new JobParameters(source, maps, ranges);
            jobParameters.TargetActionType = cmdInfo.ActionType;

            return jobParameters;
        }

        /// <summary>
        /// Converts the parsed, validated command-line information into an IndexRanges instance.
        /// </summary>
        /// <param name="cmdInfo">the parsed command-line arguments</param>
        /// <returns>a dictionary of one or more index-ranges</returns>
        public static IndexRanges ConvertToIndexRanges(IndividualArgumentsCommandInfo cmdInfo)
        {
            IndexRanges ranges = new IndexRanges();
            ranges.Add(0, new IndexRange(cmdInfo.RangeBegin - 1, cmdInfo.RangeEnd - 1));
            return ranges;
        }

        /// <summary>
        /// Converts the parsed, validated command-line information into a SourceFileInfo instance.
        /// </summary>
        /// <param name="cmdInfo">the parsed command-line arguments</param>
        /// <returns>an object with sufficient information to parse a data-file</returns>
        public static SourceFileInfo ConvertToSourceFileData(IndividualArgumentsCommandInfo cmdInfo)
        {
            SourceFileInfo source = new SourceFileInfo();
            source.FileType = cmdInfo.FileType;
            source.FullFilePath = cmdInfo.DataFile;

            switch (cmdInfo.FileType)
            {
                case SourceFileType.CSV:
                    {
                        source.FieldDelimiters = DeriveFieldDelimiter(cmdInfo.Delimiters);
                        source.HasHeaderRow = cmdInfo.HasHeader;
                        break;
                    }
                case SourceFileType.MSAccess:
                    {
                        source.TableName = cmdInfo.TableOrWorksheet;
                        break;
                    }
                case SourceFileType.MSExcel:
                    {
                        source.HasHeaderRow = cmdInfo.HasHeader;
                        source.TableName = cmdInfo.TableOrWorksheet;
                        break;
                    }
            }
            return source;
        }

        /// <summary>
        /// Converts command-line friendly field delimiter names (such as "tab" or "pipe") into
        /// their actual values appleid to the CSV-parser.
        /// </summary>
        /// <param name="delimiterNames">the command-line friendly names of CSV field delimiters</param>
        /// <returns>an array of strings (with one member) containing the translated field delimiter</returns>
        private static string[] DeriveFieldDelimiter(string[] delimiterNames)
        {
            string delimiter = delimiterNames[0];

            if (delimiterNames.Length > 0)
            {
                switch (delimiter.Trim().ToUpper())
                {
                    case "COMMA":
                    case "C":
                    case ",":
                        {
                            delimiter = ","; break;
                        }
                    case "TAB":
                    case "T":
                    case "\\T":
                    case "\\\\T":
                        {
                            delimiter = "\t"; break;
                        }
                    case "PIPE":
                    case "P":
                    case "|":
                        {
                            delimiter = "|"; break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return new string[] { delimiter };
        }

        #region validation methods

        public static bool ValidateCMDArguments(IndividualArgumentsCommandInfo cmdInfo, out List<string> errorMessageList)
        {
            string tempErrorMessage;
            errorMessageList = new List<string>();

            if (!DataFileExists(cmdInfo, out tempErrorMessage))
            {
                errorMessageList.Add(tempErrorMessage);
            }
            if (!ValidateRange(cmdInfo, out tempErrorMessage))
            {
                errorMessageList.Add(tempErrorMessage);
            }
            if (!ValidateByFileType(cmdInfo, out tempErrorMessage))
            {
                errorMessageList.Add(tempErrorMessage);
            }
            if (!ValidateByActionType(cmdInfo, out tempErrorMessage))
            {
                errorMessageList.Add(tempErrorMessage);
            }

            if (errorMessageList.Count > 0)
            {
                return false;
            }
            else
                return true;
        }

        private static bool ValidateByFileType(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            switch (cmdInfo.FileType)
            {
                case SourceFileType.MSAccess:
                case SourceFileType.MSExcel:
                    // For ListTables action, no need to do this check.
                    if (cmdInfo.ActionType == TargetActionType.ListTables)
                    {
                        errorMessage = string.Empty;
                        return true;
                    }
                    if (TableOrWorkSheetExists(cmdInfo, out errorMessage))
                        return true;
                    else
                        return false;

                default:
                    errorMessage = string.Empty;
                    return true;
            }
        }

        private static bool ValidateByActionType(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            switch (cmdInfo.ActionType)
            {
                case TargetActionType.FindDuplicates:
                case TargetActionType.ImportRegDupAsCreateNew:
                case TargetActionType.ImportRegDupAsNewBatch:
                case TargetActionType.ImportRegDupAsTemp:
                case TargetActionType.ImportRegDupNone:
                case TargetActionType.ImportTemp:
                case TargetActionType.ValidateData:
                case TargetActionType.ValidateMapping:
                    if (ValidateMappingFile(cmdInfo,out errorMessage))
                        return true;
                    else
                        return false;

                default:
                    errorMessage = string.Empty;
                    return true;
            }
        }


        private static bool DataFileExists(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            if (System.IO.File.Exists(cmdInfo.DataFile))
            {
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                errorMessage = Properties.Resources.CmdVali_DataFileExists;
                return false;
            }
        }

        //haven't called by any methods
        private static bool UsernameAndPasswordExists(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            if (!string.IsNullOrEmpty(cmdInfo.UserName) &&
                !string.IsNullOrEmpty(cmdInfo.Password))
            {
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                errorMessage = Properties.Resources.CmdVali_UserAndPwdExists;
                return false;
            }
        }

        private static bool ValidateMappingFile(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            //Including mapping file exists validation and mapping xml schema validation.
            if (System.IO.File.Exists(cmdInfo.MappingFile))
            {
                errorMessage = JobUtility.ValidateMappingFile(cmdInfo.MappingFile);
                if (string.IsNullOrEmpty(errorMessage))
                    return true;
                else
                    return false;
            }
            else
            {
                errorMessage = Properties.Resources.CmdVali_MappingFileExists;
                return false;
            }
        }

        private static bool ValidateRange(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            if (cmdInfo.RangeBegin >= 1 &&
                cmdInfo.RangeBegin <= cmdInfo.RangeEnd)
            {
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                errorMessage = Properties.Resources.CmdVali_RangeValidation;
                return false;
            }
        }

        private static bool TableOrWorkSheetExists(IndividualArgumentsCommandInfo cmdInfo, out string errorMessage)
        {
            if (!string.IsNullOrEmpty(cmdInfo.TableOrWorksheet))
            {
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                errorMessage = Properties.Resources.CmdVali_TableOrWorkSheetExists;
                return false;
            }
        }

        #endregion

    }
}

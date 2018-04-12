using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;

namespace CambridgeSoft.DataLoaderGUI.Common
{
    public class  JobArgumentInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JobArgumentInfo()
        {
            _rangeBegin = 1;
            _rangeEnd = int.MaxValue;
        }

        #region Properties

        private string _dataFile;
        public string DataFile 
        {
            get { return _dataFile; }
            set { _dataFile = value; }
        }

        private SourceFileType _fileType;
        public SourceFileType FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

        private TargetActionType _actionType;
        public TargetActionType ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }

        private string _mappingFile;
        public string MappingFile
        {
            get { return _mappingFile; }
            set { _mappingFile = value; }
        }

        private int _rangeBegin;
        public int RangeBegin
        {
            get { return _rangeBegin; }
            set { _rangeBegin = value; }
        }

        private int _rangeEnd;
        public int RangeEnd
        {
            get { return _rangeEnd; }
            set { _rangeEnd = value; }
        }

        private bool _hasHeader;
        public bool HasHeader
        {
            get { return _hasHeader; }
            set { _hasHeader = value; }
        }

        private bool _knownRegNum;
        public bool KnownRegNum
        {
            get { return _knownRegNum; }
            set { _knownRegNum = value; }
        }

        private string _tableOrWorksheet;
        public string TableOrWorksheet
        {
            get { return _tableOrWorksheet; }
            set { _tableOrWorksheet = value; }
        }

        private string[] _delimiters;
        public string[] Delimiters
        {
            get { return _delimiters; }
            set { _delimiters = value; }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the parsed, validated command-line information into a JobParameters instance.
        /// </summary>
        /// <param name="jobArgsInfo">the parsed command-line arguments</param>
        /// <returns>an initialized JobParameters instance ready for execution</returns>
        public static JobParameters ConvertToJobParameters(JobArgumentInfo jobArgsInfo)
        {
            SourceFileInfo source = ConvertToSourceFileData(jobArgsInfo);
            IndexRanges ranges = ConvertToIndexRanges(jobArgsInfo);
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

            if (!string.IsNullOrEmpty(jobArgsInfo.MappingFile) &&
                mappingFileRequiredTypes.Contains(jobArgsInfo.ActionType))
            {
                maps = Mappings.GetMappingsFromFile(jobArgsInfo.MappingFile);
            }

            JobParameters jobParameters = new JobParameters(source, maps, ranges);
            jobParameters.TargetActionType = jobArgsInfo.ActionType;

            return jobParameters;
        }

        /// <summary>
        /// Converts the parsed, validated command-line information into a SourceFileInfo instance.
        /// </summary>
        /// <param name="jobArgsInfo">the parsed command-line arguments</param>
        /// <returns>an object with sufficient information to parse a data-file</returns>
        public static SourceFileInfo ConvertToSourceFileData(JobArgumentInfo jobArgsInfo)
        {
            SourceFileInfo source = new SourceFileInfo();
            source.FileType = jobArgsInfo.FileType;
            source.FullFilePath = jobArgsInfo.DataFile;

            switch (jobArgsInfo.FileType)
            {
                case SourceFileType.CSV:
                    {
                        source.FieldDelimiters = DeriveFieldDelimiter(jobArgsInfo.Delimiters);
                        source.HasHeaderRow = jobArgsInfo.HasHeader;
                        break;
                    }
                case SourceFileType.MSAccess:
                    {
                        source.TableName = jobArgsInfo.TableOrWorksheet;
                        break;
                    }
                case SourceFileType.MSExcel:
                    {
                        source.HasHeaderRow = jobArgsInfo.HasHeader;
                        source.TableName = jobArgsInfo.TableOrWorksheet;
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

        /// <summary>
        /// Converts the parsed, validated command-line information into an IndexRanges instance.
        /// </summary>
        /// <param name="jobArgsInfo">the parsed command-line arguments</param>
        /// <returns>a dictionary of one or more index-ranges</returns>
        public static IndexRanges ConvertToIndexRanges(JobArgumentInfo jobArgsInfo)
        {
            IndexRanges ranges = new IndexRanges();
            ranges.Add(0, new IndexRange(jobArgsInfo.RangeBegin - 1, jobArgsInfo.RangeEnd - 1));
            return ranges;
        }

        /// <summary>
        /// Create a new JobParameters object.
        /// </summary>
        /// <param name="jobArgs">arguments</param>
        /// <returns>an initialized JobParameters object to allow further processing</returns>
        public static JobParameters ProcessArguments(JobArgumentInfo jobArgs)
        {
            // convert to internal object
            JobParameters job = JobArgumentInfo.ConvertToJobParameters(jobArgs);
            return job;
        }

        #endregion
    }
}

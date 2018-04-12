using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;

using CommandLine;

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// Represents the options available at the command-line.
    /// </summary>
    public class IndividualArgumentsCommandInfo
    {
        /// <summary>
        /// file source of data
        /// </summary>
        [ArgumentAttribute(ArgumentType.Required, ShortName = "data", LongName = "datafile", HelpText = "The input data-file.")]
        public string DataFile;

        /// <summary>
        /// classification (via an enumeration) of the data source file
        /// </summary>
        [ArgumentAttribute(ArgumentType.Required, ShortName = "type", LongName = "filetype", HelpText = "The type (format) of the input data-file.")]
        public SourceFileType FileType;

        /// <summary>
        /// classification (via an enumeration) of the action to perform
        /// </summary>
        [ArgumentAttribute(ArgumentType.Required, ShortName = "act", LongName = "action", HelpText = "The desired action/job to perform.")]
        public TargetActionType ActionType;

        /// <summary>
        /// the file containing mapping specification
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "mapping", LongName = "mappingfile", HelpText = "The mapping file.")]
        public string MappingFile;

        /// <summary>
        /// the beginning of the range of data-records in the source file to be processed
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, DefaultValue = 1, ShortName = "b", LongName = "begin", HelpText = "Starting record index.")]
        public int RangeBegin;

        /// <summary>
        /// the ending of the range of data-records in the source file to be processed
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, DefaultValue = int.MaxValue, ShortName = "e", LongName = "end", HelpText = "Ending record index.")]
        public int RangeEnd;

        /// <summary>
        /// indicator of the CSV or MS Excel worksheet having a column names in the first row
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "hdr", LongName = "header", HelpText = "The CSV or MS Excel worksheet includes a header row.")]
        public bool HasHeader;

        /// <summary>
        /// for MS Excel or MS Access files, the names of the worksheet, table or view containing
        /// the target data
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "tbl", LongName = "table", HelpText = "The table/view (MS Access) or worksheet (MS Excel) containing data.")]
        public string TableOrWorksheet;

        /// <summary>
        /// for CSV files, the field-delimiter used to separate column values
        /// </summary>
        [ArgumentAttribute(ArgumentType.MultipleUnique, DefaultValue = new string[] { "," }, ShortName = "del", LongName = "delimiter", HelpText = "The field delimiter.")]
        public string[] Delimiters;

        /// <summary>
        /// for authentication purposes
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "user", LongName = "username", HelpText = "A valid username for the CBOE Registration System.")]
        public string UserName;

        /// <summary>
        /// for authentication purposes
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "pwd", LongName = "password", HelpText = "The password for the user/login.")]
        public string Password;

        [ArgumentAttribute(ArgumentType.AtMostOnce, DefaultValue=false, ShortName = "s", LongName = "silent", HelpText = "Run unattended without writing to console window.")]
        public bool RunSilent;
    
    }
}

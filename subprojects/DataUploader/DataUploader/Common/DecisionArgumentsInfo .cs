using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// This class is intended to be 'silently' considered and used to determine which
    /// subsequent CommandInfo class to use for public notifications and job executions.
    /// 
    /// </summary>
    public class DecisionArgumentsInfo
    {
        /// <summary>
        /// file source of command-line arguments
        /// </summary>
        [ArgumentAttribute(
            ArgumentType.AtMostOnce
            , ShortName = "cmd"
            , LongName = "command"
            , HelpText = "Reference to a control file. Each argument must be on a new line."
            )
        ]
        public string CommandFilePath;
    }
}

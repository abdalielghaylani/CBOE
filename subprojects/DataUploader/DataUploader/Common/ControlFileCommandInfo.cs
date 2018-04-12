using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;

using CommandLine;

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// Represents the options available at the command-line; specific to using a control file
    /// to house the individual parameters.
    /// </summary>
    public class ControlFileCommandInfo
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

        /// <summary>
        /// for authentication purposes
        /// </summary>
        [ArgumentAttribute(
            ArgumentType.AtMostOnce
            , ShortName = "user"
            , LongName = "username"
            , HelpText = "A valid username for the CBOE Registration System."
            )
        ]
        public string UserName;

        /// <summary>
        /// for authentication purposes
        /// </summary>
        [ArgumentAttribute(
           ArgumentType.AtMostOnce
            , ShortName = "pwd"
            , LongName = "password"
            , HelpText = "The password for the user/login."
            )
        ]
        public string Password;

    }
}

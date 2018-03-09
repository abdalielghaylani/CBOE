// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace COEConfigurationPublishTool
{
    using System;
    using System.IO;

    /// <summary>
    /// The class to process log
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The log single instance
        /// </summary>
        private static Logger singleInstance;

        /// <summary>
        /// The log file name
        /// </summary>
        private string logFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="logFileName">The log file path.</param>
        private Logger(string logFileName)
        {
            this.logFileName = logFileName;
        }

        /// <summary>
        /// Gets the single instance of Logger.
        /// </summary>
        public static Logger SingleInstance
        {
            get
            {
                if (singleInstance == null)
                {
                    throw new Exception("Logger not initilized, please call the Init method before using it.");
                }

                return singleInstance;
            }
        }

        /// <summary>
        /// Initialize the Logger using the log file name.
        /// </summary>
        /// <param name="logFileName">The log file name</param>
        public static void Init(string logFileName)
        {
            singleInstance = new Logger(logFileName);
        }

        /// <summary>
        /// Print and log the message with red color.
        /// </summary>
        /// <param name="errorMsg">The error message.</param>
        /// <param name="newLine">If switch new line or not.</param>
        public void PrintAndLogError(string errorMsg, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            this.PrintAndLogMsg(errorMsg, newLine);
            Console.ResetColor();
        }

        /// <summary>
        /// Print and log the message with white color.
        /// </summary>
        /// <param name="infoMsg">The information message.</param>
        /// <param name="newLine">If switch new line or not.</param>
        public void PrintAndLogInfo(string infoMsg, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.White;
            this.PrintAndLogMsg(infoMsg, newLine);
            Console.ResetColor();
        }

        /// <summary>
        /// Print and log message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="newLine">Need enter or not</param>
        public void PrintAndLogMsg(string message, bool newLine = false)
        {
            Console.WriteLine(message);
            if (newLine)
            {
                Console.WriteLine();
            }

            this.LogOnly(message);
        }

        /// <summary>
        /// Only log the message on log file.
        /// </summary>
        /// <param name="message">The message log in file.</param>
        public void LogOnly(string message)
        {
            using (var fileStream = File.Open(this.logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString() + " : " + message);
                    streamWriter.Flush();
                }
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandProcessor.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.ExternalProcess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A processor of commandHandlers from StdIn to StdOut/StdErr
    /// </summary>
    internal class CommandProcessor
    {
        #region Constants and Fields

        /// <summary>
        /// The available commandHandlers.
        /// </summary>
        private readonly List<CommandHandler> commandHandlers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor"/> class.
        /// </summary>
        public CommandProcessor()
        {
            this.commandHandlers = new List<CommandHandler>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the command processor.
        /// </summary>
        /// <param name="stdIn">The reader for StdIn.</param>
        /// <param name="stdOut">The writer for StdOut.</param>
        /// <param name="stdErr">The writer for StdErr.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "[fblom, 2011-10-05]: The exception is 'carried over' onto the host process")]
        public void Run(TextReader stdIn, TextWriter stdOut, TextWriter stdErr)
        {
            bool exit = false;
            while (!exit)
            {
                try
                {
                    string input = stdIn.ReadLine();
                    if (input == null)
                    {
                        exit = true;
                    }
                    else
                    {
                        string commandResult = string.Empty;
                        string errorText = string.Empty;

                        try
                        {
                            commandResult = this.ProcessCommand(input);
                        }
                        catch (Exception e)
                        {
                            errorText = e.Message + e.StackTrace;
                        }

                        stdOut.WriteLine(ProcessHost.CommuncationPreamble + commandResult);
                        stdOut.Flush();

                        string base64ErrorText = Convert.ToBase64String(Encoding.UTF8.GetBytes(errorText));
                        stdErr.WriteLine(ProcessHost.CommuncationPreamble + base64ErrorText);
                        stdErr.Flush();
                    }
                }
                catch
                {
                    exit = true;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="commandHandler">The command.</param>
        protected void AddCommand(CommandHandler commandHandler)
        {
            this.commandHandlers.Add(commandHandler);
        }

        /// <summary>
        /// Handles an unknown command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The text to be returned from the unknown command.</returns>
        protected virtual string HandleUnknownCommand(string commandText)
        {
            return string.Empty;
        }

        /// <summary>
        /// Processes the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The result of the command.</returns>
        private string ProcessCommand(string commandText)
        {
            string[] parameters = commandText.Split(new[] { ' ' });
            foreach (CommandHandler commandHandler in this.commandHandlers)
            {
                if (commandHandler.IsMatch(parameters))
                {
                    string result = commandHandler.Execute(parameters);
                    return result;
                }
            }

            // An unknown command.
            return this.HandleUnknownCommand(commandText);
        }

        #endregion
    }
}
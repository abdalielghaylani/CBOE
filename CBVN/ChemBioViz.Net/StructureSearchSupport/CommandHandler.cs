// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StructureSearchSupport
{
    using System;
    using System.Text;

    /// <summary>
    /// A single command handler that is processed through an <see cref="CommandProcessor"/>.
    /// </summary>
    public abstract class CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The expected number of additional parameters.
        /// </summary>
        private readonly int _expectedNumberOfAdditionalParameters;

        /// <summary>
        /// The name of the command.
        /// </summary>
        private readonly string _name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="expectedNumberOfAdditionalParameters">The expected number of additional parameters.</param>
        protected CommandHandler(string name, int expectedNumberOfAdditionalParameters)
        {
            _name = name;
            _expectedNumberOfAdditionalParameters = expectedNumberOfAdditionalParameters;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the command using the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of the command.</returns>
        public string Execute(string[] parameters)
        {
            return ExecuteCore(parameters);
        }

        /// <summary>
        /// Determines whether the specified parameters is a match for the command.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     <c>true</c> if the specified parameters is a match for the command; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(string[] parameters)
        {
            if (!VerifyExactNumberOfParameters(parameters, _expectedNumberOfAdditionalParameters + 1))
            {
                return false;
            }

            var commandName = parameters[0].Trim();
            return VerifyCommandNameMatches(commandName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the specified text to a base64 encoded text.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>The encoded text.</returns>
        protected static string ConvertToBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var bytes = Encoding.UTF8.GetBytes(text);
            var result = Convert.ToBase64String(bytes);
            return result;
        }

        /// <summary>
        /// Converts the specified base64 text to a decoded text.
        /// </summary>
        /// <param name="encodedText">The encoded text.</param>
        /// <returns>The decoded text.</returns>
        protected static string FromBase64(string encodedText)
        {
            var bytes = Convert.FromBase64String(encodedText);
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary>
        /// Verifies that the command has the exact number of parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedNumberOfParameters">The number of parameters.</param>
        /// <returns><c>true</c> if the number of parameters matches the expected number of parameters; otherwise, <c>false</c>.</returns>
        protected static bool VerifyExactNumberOfParameters(string[] parameters, int expectedNumberOfParameters)
        {
            return parameters.Length == expectedNumberOfParameters;
        }

        /// <summary>
        /// Overridable method that executes the command. 
        /// </summary>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The result of the command.</returns>
        protected abstract string ExecuteCore(string[] parameters);

        /// <summary>
        /// Verifies that the command name matches.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns><c>true</c> if the command name matches the current command name; otherwise, <c>false</c>.</returns>
        protected bool VerifyCommandNameMatches(string commandName)
        {
            return string.Compare(_name, commandName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
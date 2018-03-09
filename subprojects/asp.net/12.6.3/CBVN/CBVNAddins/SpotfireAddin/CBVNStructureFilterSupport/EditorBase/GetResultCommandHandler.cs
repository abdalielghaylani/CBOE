// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetResultCommandHandler.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilterSupport.EditorBase
{
    using System;
    using System.Collections.Generic;
    using CBVNStructureFilterSupport.ExternalProcess;

    /// <summary>
    /// A command handler for handling "GetResult" commands.
    /// </summary>
    internal class GetResultCommandHandler : CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The command that this handler reacts to.
        /// </summary>
        public const string Command = "GetResult";

        /// <summary>
        /// The dictionary that holds the results.
        /// </summary>
        private readonly IDictionary<string, string> results;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResultCommandHandler"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public GetResultCommandHandler(IDictionary<string, string> results)
            : base(Command, 1)
        {
            this.results = results;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridable method that executes the command.
        /// </summary>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The result of the command.</returns>
        protected override string ExecuteCore(string[] parameters)
        {
            string key = FromBase64(parameters[1]);
            if (this.results.ContainsKey(key))
            {
                string result = this.results[key];
                string base64 = ConvertToBase64(result);
                return base64;
            }

            throw new ArgumentException("Unknown result parameter", key);
        }

        #endregion
    }
}
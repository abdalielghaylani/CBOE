// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetResultCommandHandler.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;


    /// <summary>
    /// A command handler for handling "GetResult" commands.
    /// </summary>
    public class GetResultCommandHandler : CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The command that this handler reacts to.
        /// </summary>
        public const string Command = "GetResult";

        /// <summary>
        /// The dictionary that holds the results.
        /// </summary>
        private readonly IDictionary<string, string> _results;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResultCommandHandler"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public GetResultCommandHandler(IDictionary<string, string> results)
            : base(Command, 1)
        {
            _results = results;
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
            var key = FromBase64(parameters[1]);
            if (_results.ContainsKey(key))
            {
                var result = _results[key];
                var base64 = ConvertToBase64(result);
                return base64;
            }

            throw new ArgumentException("Err_UnknownResultParam", key);
        }

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetParameterCommandHandler.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;
    using CBVNStructureFilterSupport.ExternalProcess;

    /// <summary>
    /// A command handler for the "SetParameter" command.
    /// </summary>
    internal class SetParameterCommandHandler : CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The command that this handler reacts to.
        /// </summary>
        public const string Command = "SetParameter";

        /// <summary>
        /// The dictionary where we collect the parameters.
        /// </summary>
        private readonly IDictionary<string, string> namedParameters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SetParameterCommandHandler"/> class.
        /// </summary>
        /// <param name="namedParameters">The named parameters dictionary where we collect the parameters.</param>
        public SetParameterCommandHandler(IDictionary<string, string> namedParameters)
            : base(Command, 2)
        {
            this.namedParameters = namedParameters;
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
            // The first parameter is the command name ("setProperty")
            // It is ignored.
            // The second parameter is the name of the parameter to set.
            string parameterName = FromBase64(parameters[1]).ToUpperInvariant();

            // The third parameter is the value.
            string value = FromBase64(parameters[2]);
            this.namedParameters[parameterName] = value;

            return string.Empty;
        }

        #endregion
    }
}
// <copyright file="GetSupportedStructureFormatsCommandHandler.cs" company="PerkinElmer Inc.">
// Copyright © 2011 PerkinElmer Inc.,
// 100 CambridgePark Drive, Cambridge, MA 02140.
// All rights reserved.
// This software is the confidential and proprietary information
// of PerkinElmer Inc. ("Confidential Information"). You shall not
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>

namespace CBVNStructureFilterSupport.EditorBase
{
    using System.Collections.Generic;

    using CBVNStructureFilterSupport.ExternalProcess;

    /// <summary>
    /// A command handler for the "GetSupportedFormats" command.
    /// </summary>
    internal class GetSupportedStructureFormatsCommandHandler : CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The actual command.
        /// </summary>
        public const string Command = "GetSupportedFormats";

        /// <summary>
        /// The list of supported formats, converted to a Base64, and then separated by spaces.
        /// </summary>
        private readonly string supportedFormats;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSupportedStructureFormatsCommandHandler"/> class.
        /// </summary>
        /// <param name="supportedFormats">The supported formats.</param>
        public GetSupportedStructureFormatsCommandHandler(params string[] supportedFormats)
            : base(Command, 0)
        {
            List<string> convertedFormats = new List<string>(supportedFormats.Length);
            foreach (string supportedFormat in supportedFormats)
            {
                convertedFormats.Add(ConvertToBase64(supportedFormat));
            }

            this.supportedFormats = string.Join(" ", convertedFormats.ToArray());
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
            return this.supportedFormats;
        }

        #endregion
    }
}
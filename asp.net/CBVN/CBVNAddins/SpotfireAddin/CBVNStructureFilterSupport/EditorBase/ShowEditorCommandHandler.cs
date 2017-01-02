// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowEditorCommandHandler.cs" company="PerkinElmer Inc.">
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

using CBVNStructureFilterSupport.ExternalProcess;
namespace CBVNStructureFilterSupport.EditorBase
{    

    /// <summary>
    /// A command handler to handle the "ShowEditor" command.
    /// </summary>
    internal class ShowEditorCommandHandler : CommandHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The command that this handler reacts to.
        /// </summary>
        public const string Command = "ShowEditor";

        /// <summary>
        /// The action to perform when an editor should be shown.
        /// </summary>
        private readonly Action action;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowEditorCommandHandler"/> class.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        public ShowEditorCommandHandler(Action action)
            : base(Command, 0)
        {
            this.action = action;
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
            this.action();
            return string.Empty;
        }

        #endregion
    }
}
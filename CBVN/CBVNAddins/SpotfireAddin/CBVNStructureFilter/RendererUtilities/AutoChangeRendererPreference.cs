// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoChangeRendererPreference.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.Persistence;
    using Spotfire.Dxp.Framework.Preferences;

    /// <summary>
    /// the preference for auto change structure renderer prompt
    /// </summary>
    [PersistenceVersion(9, 0)]
    internal class AutoChangeRendererPreference : CustomPreference
    {
        #region Enumerations

        /// <summary>
        /// the dialog result from auto change structure renderer prompt dialog
        /// </summary>
        [Serializable]
        [PersistenceVersion(9, 0)]
        public enum AutoChangeRendererDialogResult
        {
            None,   // None result, not start validate the renderer
            Yes,    // accept to change to default renderer
            No      // not accept to change the renderer
        }

        #endregion

        #region Constants and Fields

        /// <summary>
        /// the value to store if user can skip the auto change structure renderer prompt
        /// </summary>
        private readonly PreferenceProperty<bool> skipPrompt;

        /// <summary>
        /// last prompt dialog result user choosed
        /// if it is none value, LD should show the prompt
        /// otherwise, LD will not show prompt
        /// and use the result user choosed
        /// </summary>
        private readonly PreferenceProperty<AutoChangeRendererDialogResult> lastPromptResult;

        #endregion

        #region Constructors and Destructors

        public AutoChangeRendererPreference()
        {
            // add this two preferences as local preference value
            this.skipPrompt =
                this.AddPreference(
                    new PreferenceProperty<bool>(
                        "SkipPrompt",
                        "1.0",
                        PreferencePersistenceScope.Local,
                        PreferenceUsage.SingleUser,
                        false));

            this.lastPromptResult =
                this.AddPreference(
                    new PreferenceProperty<AutoChangeRendererDialogResult>(
                        "LastPromptResult",
                        "1.0",
                        PreferencePersistenceScope.Local,
                        PreferenceUsage.SingleUser,
                        AutoChangeRendererDialogResult.None));
        }

        #endregion

        #region Properties

        /// <summary>
        /// get and set if skip to prompt dialog
        /// </summary>
        public bool SkipPrompt
        {
            get
            {
                return this.skipPrompt.Value;
            }
            set
            {
                this.skipPrompt.Value = value;
            }
        }

        /// <summary>
        /// get and set last prompt dialog result
        /// </summary>
        public AutoChangeRendererDialogResult LastPromptResult
        {
            get
            {
                return this.lastPromptResult.Value;
            }
            set
            {
                this.lastPromptResult.Value = value;
            }
        }

        /// <summary>
        /// Implementations of this property are to return the name of
        /// the category to which the preference belongs. The category name must
        /// not be <c>null</c> or an empty string for the preference to be
        /// properly persisted.
        /// </summary>
        public override string Category
        {
            get { return "PromptPreference"; }
        }

        /// <summary>
        /// Implementations of this property are to return the name of
        /// the subcategory to which the preference belongs. The subcategory
        /// must not be <c>null</c> or an empty string for the preference to
        /// be properly persisted.
        /// </summary>
        public override string SubCategory
        {
            get { return "AutoChangeRenderer"; }
        }

        #endregion

    }
}

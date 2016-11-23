// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterWorkerTypeIdentifiers.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter.FilterThread
{
    using Spotfire.Dxp.Application.Extension;

    /// <summary>
    /// Defines the identifiers used for the panels in this project.
    /// </summary>
    public sealed class StructureFilterWorkerTypeIdentifiers : CustomTypeIdentifiers
    {
        #region Constants and Fields

        /// <summary>
        /// Type identifier for.
        /// </summary>
        public static readonly CustomTypeIdentifier ExampleWorkerTypeId =
            CreateTypeIdentifier(
                "CBVNStructureFilter.SimpleWorker.StructureFilterWorker",
                InvariantResources.ThreadDisplayName, 
                InvariantResources.ThreadDescription);

        #endregion
    }
}
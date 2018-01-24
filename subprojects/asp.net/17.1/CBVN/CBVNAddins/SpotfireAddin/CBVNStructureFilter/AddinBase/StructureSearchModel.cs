// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureSearchModel.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;

    using Spotfire.Dxp.Data.Import;
    using CBVNStructureFilter.Properties;

    /// <summary>
    /// The base model object for structure search parameters.
    /// </summary>
    internal class StructureSearchModel
    {
        #region Public Methods

        /// <summary>
        /// Gets the result list description.
        /// </summary>
        /// <param name="searchLink">The search link.</param>
        /// <param name="retrieveLink">The retrieve link.</param>
        /// <returns>The description for the information links used.</returns>
        public virtual string GetResultListDescription(
            InformationLinkDescriptor searchLink, InformationLinkDescriptor retrieveLink)
        {
            return string.Format(Resources.DefaultListDescription, searchLink.Path, retrieveLink.Path);
        }

        /// <summary>
        /// Populates the information link parameters.
        /// </summary>
        /// <param name="searchLink">The search link.</param>
        /// <param name="informationLinkParameters">The information link parameters.</param>
        public virtual void PopulateInformationLinkParameters(
            InformationLinkDescriptor searchLink, ICollection<InformationLinkParameter> informationLinkParameters)
        {
            // The base class does not do anything.
        }

        #endregion
    }
}

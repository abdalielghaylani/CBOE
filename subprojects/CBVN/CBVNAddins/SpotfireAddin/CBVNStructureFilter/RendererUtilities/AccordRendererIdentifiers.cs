// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccordRendererIdentifiers.cs" company="PerkinElmer Inc.">
//   Copyright � 2012 PerkinElmer Inc. 
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
    using Spotfire.Dxp.Application.Extension;
    using CBVNStructureFilter.Properties;

    internal class AccordRendererIdentifiers : CustomTypeIdentifiers
    {
        #region Constants and Fields

        public static readonly CustomTypeIdentifier AccordRenderer = CreateTypeIdentifier(
            "AccordRenderer", InvariantResources.Accord, string.Format(Resources.RendererDescription, InvariantResources.Accord));

        #endregion
    }
}

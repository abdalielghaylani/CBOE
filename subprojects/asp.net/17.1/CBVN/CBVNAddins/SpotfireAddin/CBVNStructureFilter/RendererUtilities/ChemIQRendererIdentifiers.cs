// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChemIQRendererIdentifiers.cs" company="PerkinElmer Inc.">
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
    using Spotfire.Dxp.Application.Extension;
    using CBVNStructureFilter.Properties;

    internal class ChemIQRendererIdentifiers : CustomTypeIdentifiers
    {
        #region Constants and Fields

        public static readonly CustomTypeIdentifier ChemIQRenderer = CreateTypeIdentifier(
            "ChemIQRenderer", InvariantResources.ChemIQ, string.Format(Resources.RendererDescription, InvariantResources.ChemIQ));

        #endregion
    }
}

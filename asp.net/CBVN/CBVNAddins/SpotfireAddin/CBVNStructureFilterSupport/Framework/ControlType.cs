// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlType.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilterSupport.Framework
{
    /// <summary>
    /// Supported control types.
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        None = -1,

        /// <summary>
        /// Accord renderer.
        /// </summary>
        Accord = 2,

        /// <summary>
        /// ChemDraw renderer.
        /// </summary>
        ChemDraw = 1,

        /// <summary>
        /// Marvin renderer.
        /// </summary>
        Marvin = 3,

        /// <summary>
        /// Symyx/MDL renderer.
        /// </summary>
        MdlDraw = 0,

        /// <summary>
        /// ChemIQ renderer.
        /// </summary>
        ChemIQ = 4
    }
}

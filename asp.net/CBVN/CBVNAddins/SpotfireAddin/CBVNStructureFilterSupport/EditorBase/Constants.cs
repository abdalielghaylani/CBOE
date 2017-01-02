// <copyright file="Constants.cs" company="PerkinElmer Inc.">
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
    /// <summary>
    /// This class holds the names of the parameters that we are sending to and receiving from an editor.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The structure format for the structure.
        /// </summary>
        public const string StructureFormat = "StructureFormat";

        /// <summary>
        /// The actual structure.
        /// </summary>
        public const string Structure = "Structure";

        /// <summary>
        /// The window handle (can be used to make a subprocess modal).
        /// </summary>
        public const string Handle = "Handle";

        /// <summary>
        /// Gets the dialog result.
        /// </summary>
        public const string DialogResult = "DialogResult";
    }
}

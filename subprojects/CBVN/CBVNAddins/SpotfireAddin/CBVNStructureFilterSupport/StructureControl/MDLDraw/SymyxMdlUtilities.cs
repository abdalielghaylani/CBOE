// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymyxMdlUtilities.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilterSupport.MDLDraw
{
    using System;
    using System.IO;
    using CBVNStructureFilterSupport.Framework;

    /// <summary>
    /// Utilities for handling Symyx/MDL Draw in all supported versions
    /// </summary>
    internal class SymyxMdlUtilities
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether Symyx or MDL Draw is installed
        /// </summary>
        /// <value>
        /// <c>true</c> if Symyx or MDL Draw is installed; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsSymyxOrMdlInstalled
        {
            get
            {
                bool result = false;
                if (Symyx32IsInstalled)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Symyx Draw 3.2 is installed.
        /// </summary>
        /// <value><c>true</c> if Symyx Draw 3.2 is installed; otherwise, <c>false</c>.</value>
        private static bool Symyx32IsInstalled
        {
            get
            {
                try
                {
                    // Using this type for the first time causes its dependent assemblies to be loaded
                    // and that may fail
                    return StructureControl.IsInstalled;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (FileLoadException)
                {
                    return false;
                }
                catch (BadImageFormatException)
                {
                    return false;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the control instance. Assumes that the caller has respected
        /// the <see cref="IsSymyxOrMdlInstalled"/> property value.
        /// </summary>
        /// <returns>A control instance.</returns>
        internal static StructureControlBase CreateControl(bool viewOnly)
        {
            StructureControlBase result = null;
            if (Symyx32IsInstalled)
            {
                result = new StructureControl(viewOnly);
            }

            return result;
        }

        #endregion
    }
}

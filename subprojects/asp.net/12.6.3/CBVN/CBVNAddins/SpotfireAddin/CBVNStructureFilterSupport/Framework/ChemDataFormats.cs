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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace CBVNStructureFilterSupport.Framework
{
    /// <summary>
    /// Constants for use throughout the product.
    /// </summary>
    public class ChemDataFormats
    {
        #region Contructor
        static ChemDataFormats()
        {
            EnhancedMetafile = DataFormats.EnhancedMetafile;
        }
        #endregion
        #region CONSTANTS
        /// <summary>
        /// Smiles string type
        /// </summary>
        public static readonly string Smiles = "SMILES";
        /// <summary>
        /// MDLCT type
        /// </summary>
        public static readonly string MDLCT = "MDLCT";
        /// <summary>
        /// MDLSK type
        /// </summary>
        public static readonly string MDLSK = "MDLSK";
        /// <summary>
        /// CDX type
        /// </summary>
        public static readonly string CDX = "ChemDraw Interchange Format";
        /// <summary>
        /// CDXML type
        /// </summary>
        public static readonly string CDXML = "ChemDraw Text Styles";
        /// <summary>
        /// EMF
        /// </summary>
        public static readonly string EnhancedMetafile;
        #endregion
        #region Method
        /// <summary>
        /// Get format
        /// </summary>
        /// <param name="FormatString">string</param>
        /// <returns>format</returns>
        public static DataFormats.Format GetFormat(string FormatString)
        {
            return DataFormats.GetFormat(FormatString);
        }
        #endregion

    }
}

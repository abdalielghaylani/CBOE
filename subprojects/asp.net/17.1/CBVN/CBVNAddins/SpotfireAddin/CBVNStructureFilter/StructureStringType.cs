// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureStringType.cs" company="PerkinElmer Inc.">
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
    using System.Diagnostics.CodeAnalysis;

    using Spotfire.Dxp.Framework.Persistence;

    /// <summary>
    /// Structure payload hint value.
    /// </summary>
    /// <remarks>Must be public since it's used in the XmlSerialized IdList class</remarks>
    [Serializable]
    [PersistenceVersion(9, 0)]
    [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "FBM: CodeCleanup for legacy code.")]
    public enum StructureStringType
    {
        /// <summary>
        /// Unknown payload type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Molfile clob payload type.
        /// </summary>
        Molfile,

        /// <summary>
        /// CHIME string payload type.
        /// </summary>
        CHIME,

        /// <summary>
        /// SMILES string payload type.
        /// </summary>
        SMILES,

        /// <summary>
        /// CDX binary payload type. 
        /// </summary>
        CDX
    }

    internal class StructureStringTypeHelper
    {
        // CDX
        internal const string MIME_TYPE_XCDX = "chemical/x-cdx";//preferred
        internal const string MIME_TYPE_CDX = "chemical/cdx";
        internal const string MIME_TYPE_CHEMDRAW = "chemical/x-chemdraw";
        // CDXML
        internal const string MIME_TYPE_XML = "text/xml";
        internal const string MIME_TYPE_XCDXML = "chemical/x-cdxml";
        internal const string MIME_TYPE_CDXML = "chemical/cdxml";
        // MOL
        internal const string MIME_TYPE_XMDLMOLFILE = "chemical/x-mdl-molfile";//preferred
        internal const string MIME_TYPE_MDLMOLFILE = "chemical/mdl-molfile";
        internal const string MIME_TYPE_XMSIMOLFILE = "chemical/x-msi-molfile";
        internal const string MIME_TYPE_MSIMOLFILE = "chemical/msi-molfile";
        // Molfile v3000 reserved
        internal const string MIME_TYPE_MOLFILEV3000 = "chemical/x-mdl-molfile-v3000";
        internal const string MIME_TYPE_MOLFILEV3000a = "chemical/mdl-molfile-v3000";
        // SMILES
        internal const string MIME_TYPE_XDAYSMILES = "chemical/x-daylight-smiles";//preferred
        internal const string MIME_TYPE_DAYSMILES = "chemical/daylight-smiles";
        internal const string MIME_TYPE_XSMILES = "chemical/x-smiles";
        internal const string MIME_TYPE_SMILES = "chemical/smiles";
        // CHIME
        internal const string MIME_TYPE_XMDLCHIME = "chemical/x-mdl-chime";

        internal static StructureStringType MimeTypeToStructureStringType(string mimeType)
        {
            // note these are taken from the current supported types in the CCCLR assembly (Utils.h, class MimeTypes)
            switch (mimeType)
            {
                // CDX
                case MIME_TYPE_XCDX:
                case MIME_TYPE_CDX:
                case MIME_TYPE_CHEMDRAW:
                    return StructureStringType.CDX;
                // CDXML - not yet included
                case MIME_TYPE_XML:
                case MIME_TYPE_XCDXML:
                case MIME_TYPE_CDXML:
                    return StructureStringType.Unknown;
                // MOL
                case MIME_TYPE_XMDLMOLFILE:
                case MIME_TYPE_MDLMOLFILE:
                case MIME_TYPE_XMSIMOLFILE:
                case MIME_TYPE_MSIMOLFILE:
                    return StructureStringType.Molfile;
                // Molfile v3000 reserved
                //        "chemical/x-mdl-molfile-v3000";
                // "chemical/mdl-molfile-v3000";
                // SMILES
                case MIME_TYPE_XDAYSMILES:
                case MIME_TYPE_DAYSMILES:
                case MIME_TYPE_XSMILES:
                case MIME_TYPE_SMILES:
                    return StructureStringType.SMILES;
                case MIME_TYPE_XMDLCHIME:
                    return StructureStringType.CHIME;
            }
            return StructureStringType.Unknown;
        }

        internal static string StructureStringTypeToMimeType(StructureStringType structureStringType)
        {
            switch (structureStringType)
            {
                case StructureStringType.CDX:
                    return MIME_TYPE_XCDX;
                case StructureStringType.CHIME:
                    return MIME_TYPE_XMDLCHIME;
                case StructureStringType.Molfile:
                    return MIME_TYPE_XMDLMOLFILE;
                case StructureStringType.SMILES:
                    return MIME_TYPE_XDAYSMILES;
            }
            return null;
        }
    }
}

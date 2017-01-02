// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterUtilities.cs" company="PerkinElmer Inc.">
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

using Spotfire.Dxp.Data;
using CBVNStructureFilterSupport.ChemDraw;
using CBVNStructureFilterSupport.Framework;

namespace CBVNStructureFilter
{
    internal class FilterUtilities
    {
        internal const int PreviousSearchesMax = 10;

        //TODO The methods in this class should be moved to a more generic assembly that deals with structure types, once structure type code is consolidated within Lead Discovery
        
        internal static bool IsStructureColumn(DataColumn col)
        {
            var contentType = col.Properties.ContentType;
            return contentType == Identifiers.MolfileContentType || contentType == Identifiers.SmilesContentType ||
                   contentType == Identifiers.CDXContentType || contentType == Identifiers.ChimeContentType ||
                   contentType == Identifiers.CDXMLContentType;
        }

        internal static StructureStringType GetDataColumnStringType(DataColumn col)
        {
            if (col != null)
            {
                string contentType = col.Properties.ContentType;

                switch (contentType)
                {
                    case Identifiers.MolfileContentType:
                        return StructureStringType.Molfile;
                    case Identifiers.SmilesContentType:
                        return StructureStringType.SMILES;
                    case Identifiers.ChimeContentType:
                        return StructureStringType.CHIME;
                    case Identifiers.CDXContentType:
                    case Identifiers.CDXMLContentType:
                        return StructureStringType.CDX;
                }
            }
            return StructureStringType.Unknown;
        }

        public static string ConvertCdxToMol(string cdx)
        {
            return (string)(ChemDrawInvoker.ConvertStructure(cdx ?? string.Empty,
                                new[] { Identifiers.MolfileContentType })
                      [Identifiers.MolfileContentType]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.Utility
{
	/// <summary>
	/// This class manages conversions from Framework Messaging Types Enums to strings and vice-versa. This is required for serialization.
	/// </summary>
	public class COEConvert
	{
		/// <summary>
		/// Converts a given string to a SearchCriteria.Positions. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">The string to be converted. Posible values: left, right, both, none</param>
		/// <returns>The corresponding position</returns>
		public static SearchCriteria.Positions ToPositions(string value) {
			switch(value.Trim().ToUpper()) {
				case "LEFT":
					return SearchCriteria.Positions.Left;
				case "RIGHT":
					return SearchCriteria.Positions.Right;
				case "BOTH":
					return SearchCriteria.Positions.Both;
				case "":
				case "NONE":
					return SearchCriteria.Positions.None;
                default:
                    return SearchCriteria.Positions.None;
			}
		}
		/// <summary>
		/// Converts the given SearchCriteria.Positions to string (uppercase)
		/// </summary>
		/// <param name="value">The SearchCriteria.Positions to be converted to string</param>
		/// <returns>The String representation of the value. Possible values: LEFT, RIGHT, BOTH, NONE</returns>
		public static string ToString(SearchCriteria.Positions value) {
			switch(value) {
				case SearchCriteria.Positions.Left:
					return "LEFT";
				case SearchCriteria.Positions.Right:
					return "RIGHT";
				case SearchCriteria.Positions.Both:
					return "BOTH";
				default:
					return "NONE";
			}
		}
		/// <summary>
		///  Converts a given string to a SearchCriteria.TetrahedralStereoMatching. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">The string to be converted. Possible values: yes, no, any, either, same</param>
		/// <returns>The corresponding SearchCriteria.TetrahedralStereoMatching</returns>
		public static SearchCriteria.TetrahedralStereoMatching ToTetrahedralStereoMatching(string value) {
			switch(value.Trim().ToUpper()) {
				case "YES":
					return SearchCriteria.TetrahedralStereoMatching.Yes;
				case "NO":
					return SearchCriteria.TetrahedralStereoMatching.No;
				case "":
				case "ANY":
					return SearchCriteria.TetrahedralStereoMatching.Any;
				case "EITHER":
					return SearchCriteria.TetrahedralStereoMatching.Either;
				case "SAME":
					return SearchCriteria.TetrahedralStereoMatching.Same;
                default:
                    return SearchCriteria.TetrahedralStereoMatching.Any;
			}
		}

		/// <summary>
		/// Converts the given SearchCriteria.TetrahedralStereoMatching to string (uppercase)
		/// </summary>
		/// <param name="value">The SearchCriteria.TetrahedralStereoMatching to be converted to string</param>
		/// <returns>The string representation of the value. Possible values: YES, NO, ANY, EITHER, SAME</returns>
		public static string ToString(SearchCriteria.TetrahedralStereoMatching value) {
			switch(value) {
				case SearchCriteria.TetrahedralStereoMatching.Yes:
					return "YES";
				case SearchCriteria.TetrahedralStereoMatching.No:
					return "NO";
				case SearchCriteria.TetrahedralStereoMatching.Any:
					return "ANY";
				case SearchCriteria.TetrahedralStereoMatching.Either:
					return "EITHER";
				default:
					return "SAME";
			}
		}

		/// <summary>
		///  Converts a given string to a SearchCriteria.COEBoolean. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">the string to be converted. Possible values: yes, no</param>
		/// <returns>The corresponding SearchCriteria.COEBoolean</returns>
		public static SearchCriteria.COEBoolean ToCOEBoolean(string value) {
			switch(value.Trim().ToUpper()) { 
				case "YES":
					return SearchCriteria.COEBoolean.Yes;
				case "NO":
					return SearchCriteria.COEBoolean.No;
                case "TRUE":
                    return SearchCriteria.COEBoolean.Yes;
                case "FALSE":
                    return SearchCriteria.COEBoolean.No;
                default:
                    return SearchCriteria.COEBoolean.No;
			}
		}

		/// <summary>
		/// Converts the given SearchCriteria.COEBoolean to string (uppercase)
		/// </summary>
		/// <param name="value">The SearchCriteria.COEBoolean to be converted to string</param>
		/// <returns>The string representation of the value. Possible values: YES, NO</returns>
		public static string ToString(SearchCriteria.COEBoolean value)
		{
			switch(value)
			{
				case SearchCriteria.COEBoolean.Yes:
					return "YES";
				default:
					return "NO";
			}
		}

		/// <summary>
		///  Converts a given string to a SearchCriteria.COEOperators. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">The string to be converted. Possible values: GT, GTE, LT, LTE, EQUAL, NOTEQUAL, IN, LIKE</param>
		/// <returns>The corresponding SearchCriteria.COEOperators</returns>
		public static SearchCriteria.COEOperators ToCOEOperator(string value) {
			switch(value.Trim().ToUpper()) { 
				case "GT":
					return SearchCriteria.COEOperators.GT;
				case "GTE":
					return SearchCriteria.COEOperators.GTE;
				case "LT":
					return SearchCriteria.COEOperators.LT;
				case "LTE":
					return SearchCriteria.COEOperators.LTE;
				case "EQUAL":
					return SearchCriteria.COEOperators.EQUAL;
				case "NOTEQUAL":
					return SearchCriteria.COEOperators.NOTEQUAL;
				case "IN":
					return SearchCriteria.COEOperators.IN;
				case "LIKE":
                    return SearchCriteria.COEOperators.LIKE;
                case "NOTIN":
                    return SearchCriteria.COEOperators.NOTIN;
                case "STARTSWITH":
                    return SearchCriteria.COEOperators.STARTSWITH;
                case "CONTAINS":
                    return SearchCriteria.COEOperators.CONTAINS;
                case "NOTCONTAINS":
                    return SearchCriteria.COEOperators.NOTCONTAINS;
                case "NOTLIKE":
                    return SearchCriteria.COEOperators.NOTLIKE;
                case "ENDWITH":
                    return SearchCriteria.COEOperators.ENDWITH;                
                default:
                    return SearchCriteria.COEOperators.LTE;
			}
		}

		/// <summary>
		/// Converts the given SearchCriteria.COEOperators to string (uppercase)
		/// </summary>
		/// <param name="value">The SearchCriteria.COEOperators to be converted to string</param>
		/// <returns>The string representation of the value. Possible values: GT, GTE, LT, LTE, EQUAL, NOTEQUAL, IN</returns>
		public static string ToString(SearchCriteria.COEOperators value) {
			switch(value) {
				case SearchCriteria.COEOperators.GT:
					return "GT";
				case SearchCriteria.COEOperators.GTE:
					return "GTE";
				case SearchCriteria.COEOperators.LT:
					return "LT";
				case SearchCriteria.COEOperators.LTE:
					return "LTE";
				case SearchCriteria.COEOperators.EQUAL:
					return "EQUAL";
				case SearchCriteria.COEOperators.NOTEQUAL:
					return "NOTEQUAL";
				case SearchCriteria.COEOperators.IN:
					return "IN";
				default:
					return "LIKE";
			}
		}

		/// <summary>
		///  Converts a given string to a COEDataView.AbstractTypes. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">the string to be converted. Possible values: Text, Integer, Real, Date, Boolean, Lob</param>
		/// <returns>The Corresponding COEDataView.AbstractTypes</returns>
		public static COEDataView.AbstractTypes ToAbstractType(string value) {
			switch(value.Trim().ToUpper()) { 
				case "TEXT":
					return COEDataView.AbstractTypes.Text;
				case "INTEGER":
					return COEDataView.AbstractTypes.Integer;
				case "REAL":
					return COEDataView.AbstractTypes.Real;
				case "DATE":
					return COEDataView.AbstractTypes.Date;
				case "BOOLEAN":
					return COEDataView.AbstractTypes.Boolean;
                case "BLOB":
                    return COEDataView.AbstractTypes.BLob;
                case "CLOB":
                    return COEDataView.AbstractTypes.CLob;
                default:
                    return COEDataView.AbstractTypes.Text;
			}
		}

		/// <summary>
		/// Converts the given COEDataView.AbstractTypes to string (uppercase)
		/// </summary>
		/// <param name="value">The COEDataView.AbstractTypes to be converted to string</param>
        /// <returns>The string representation of the value. Possible values: TEXT, INTEGER, REAL, DATE, BOOLEAN, Lob</returns>
		public static string ToString(COEDataView.AbstractTypes value) {
			switch(value) {
				case COEDataView.AbstractTypes.Text:
					return "TEXT";
				case COEDataView.AbstractTypes.Integer:
					return "INTEGER";
				case COEDataView.AbstractTypes.Real:
					return "REAL";
				case COEDataView.AbstractTypes.Date:
					return "DATE";
                case COEDataView.AbstractTypes.BLob:
                    return "BLOB";
                case COEDataView.AbstractTypes.CLob:
                    return "CLOB";
				default:
					return "BOOLEAN";
			}
		}
		/// <summary>
		///  Converts a given string to a COEDataView.JoinTypes. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">the string to be converted. Possible values: INNER, OUTER</param>
		/// <returns>The Corresponding COEDataView.JoinTypes</returns>
		public static COEDataView.JoinTypes ToJoinType(string value) {
			switch(value.Trim().ToUpper()) {
				case "INNER":
					return COEDataView.JoinTypes.INNER;
				case "OUTER":
					return COEDataView.JoinTypes.OUTER;
                default:
                    return COEDataView.JoinTypes.INNER;
			}
		}

		/// <summary>
		/// Converts the given COEDataView.JoinTypes to string (uppercase)
		/// </summary>
		/// <param name="value">The COEDataView.JoinTypes to be converted to string</param>
		/// <returns>The string representation of the value. Possible values: INNER, OUTER</returns>
		public static string ToString(COEDataView.JoinTypes value) {
			switch(value) {
				case COEDataView.JoinTypes.INNER :
					return "INNER";
				default:
					return "OUTER";
			}
		}

		/// <summary>
		///  Converts a given string to a ResultsCriteria.SortDirection. Doesn't take into account casing.
		/// </summary>
		/// <param name="value">the string to be converted. Possible values: ASC, DESC</param>
		/// <returns>The Corresponding ResultsCriteria.SortDirection</returns>
		public static ResultsCriteria.SortDirection ToSortDirection(string value) {
			switch(value.Trim().ToUpper()) {
				case "ASC":
					return ResultsCriteria.SortDirection.ASC;
				case "DESC":
					return ResultsCriteria.SortDirection.DESC;
                default:
                    return ResultsCriteria.SortDirection.ASC;
			}
		}

		/// <summary>
		/// Converts the given ResultsCriteria.SortDirection to string (uppercase)
		/// </summary>
		/// <param name="value">The ResultsCriteria.SortDirection to be converted to string</param>
		/// <returns>The string representation of the value. Possible values: asc, desc</returns>
		public static string ToString(ResultsCriteria.SortDirection value) {
			switch(value) {
				case ResultsCriteria.SortDirection.ASC:
					return "asc";
                case ResultsCriteria.SortDirection.DESC:
                    return "desc";
				default:
					return "asc";
			}
		}

        /// <summary>
        ///  Converts a given string to a COEDataView.IndexTypes. Doesn't take into account casing.
        /// </summary>
        /// <param name="value">the string to be converted. </param>
        /// <returns>The Corresponding COEDataView.IndexTypes</returns>
        public static COEDataView.IndexTypes ToIndexType(string value)
        {
            switch (value.Trim().ToUpper())
            {
                case "DIRECT_CARTRIDGE":
                    return COEDataView.IndexTypes.DIRECT_CARTRIDGE;
                case "JCHEM_CARTRIDGE":
                    return COEDataView.IndexTypes.JCHEM_CARTRIDGE;
                case "CS_CARTRIDGE":
                    return COEDataView.IndexTypes.CS_CARTRIDGE;
                case "FULL_TEXT":
                    return COEDataView.IndexTypes.FULL_TEXT;
                case "NONE":
                    return COEDataView.IndexTypes.NONE;
                case "UNKNOWN":
                    return COEDataView.IndexTypes.UNKNOWN;
                default:
                    return COEDataView.IndexTypes.UNKNOWN;
            }
        }

        /// <summary>
        /// Converts the given COEDataView.IndexTypes to string (uppercase)
        /// </summary>
        /// <param name="value">The COEDataView.IndexTypes to be converted to string</param>
        /// <returns>The string representation of the value.</returns>
        public static string ToString(COEDataView.IndexTypes value)
        {
            switch (value)
            {
                case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                    return "DIRECT_CARTRIDGE";
                case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                    return "JCHEM_CARTRIDGE";
                case COEDataView.IndexTypes.CS_CARTRIDGE:
                    return "CS_CARTRIDGE";
                case COEDataView.IndexTypes.FULL_TEXT:
                    return "FULL_TEXT";
                case COEDataView.IndexTypes.NONE:
                    return "NONE";
                case COEDataView.IndexTypes.UNKNOWN:
                    return "UNKNOWN";
                default:
                    return "NONE";
            }
        }
        
        /// <summary>
        ///  Converts a given string to a COEDataView.MimeTypes. Doesn't take into account casing.
        /// </summary>
        /// <param name="value">the string to be converted. </param>
        /// <returns>The Corresponding COEDataView.MimeTypes</returns>
        public static COEDataView.MimeTypes ToMimeType(string value)
        {
            switch (value.Trim().ToUpper())
            {
                case "NONE":
                    return COEDataView.MimeTypes.NONE;
                case "UNKNOWN":
                    return COEDataView.MimeTypes.UNKNOWN;
                case "TEXT_XML":
                    return COEDataView.MimeTypes.TEXT_XML;
                case "TEXT_RAW":
                    return COEDataView.MimeTypes.TEXT_RAW;
                case "TEXT_PLAIN":
                    return COEDataView.MimeTypes.TEXT_PLAIN;
                case "TEXT_HTML":
                    return COEDataView.MimeTypes.TEXT_HTML;
                case "IMAGE_X_WMF":
                    return COEDataView.MimeTypes.IMAGE_X_WMF;
                case "IMAGE_PNG":
                    return COEDataView.MimeTypes.IMAGE_PNG;
                case "IMAGE_JPEG":
                    return COEDataView.MimeTypes.IMAGE_JPEG;
                case "IMAGE_GIF":
                    return COEDataView.MimeTypes.IMAGE_GIF;
                case "CHEMICAL_X_SMILES":
                    return COEDataView.MimeTypes.CHEMICAL_X_SMILES;
                case "CHEMICAL_X_MDLMOLFILE":
                    return COEDataView.MimeTypes.CHEMICAL_X_MDLMOLFILE;
                case "CHEMICAL_X_CDX":
                    return COEDataView.MimeTypes.CHEMICAL_X_CDX;
                case "CHEMICAL_X_MDL_CHIME":
                    return COEDataView.MimeTypes.CHEMICAL_X_MDL_CHIME;
                case "CHEMICAL_X_DATADIRECT_CTAB":
                    return COEDataView.MimeTypes.CHEMICAL_X_DATADIRECT_CTAB;
                case "CHEMICAL_X_CML":
                    return COEDataView.MimeTypes.CHEMICAL_X_CML;
                case "APPLICATION_PDF":
                    return COEDataView.MimeTypes.APPLICATION_PDF;
                case "APPLICATION_MS_MSWORD":
                    return COEDataView.MimeTypes.APPLICATION_MS_MSWORD;
                case "APPLICATION_MS_EXCEL":
                    return COEDataView.MimeTypes.APPLICATION_MS_EXCEL;
                default:
                    return COEDataView.MimeTypes.UNKNOWN;
            }
        }

        /// <summary>
        /// Converts the given COEDataView.MimeTypes to string (uppercase)
        /// </summary>
        /// <param name="value">The COEDataView.MimeTypes to be converted to string</param>
        /// <returns>The string representation of the value. </returns>
        public static string ToString(COEDataView.MimeTypes value)
        {
            switch (value)
            {
                case COEDataView.MimeTypes.NONE:
                    return "NONE";
                case COEDataView.MimeTypes.UNKNOWN:
                    return "UNKNOWN";
                case COEDataView.MimeTypes.TEXT_XML:
                    return "TEXT_XML";
                case COEDataView.MimeTypes.TEXT_RAW:
                    return "TEXT_RAW";
                case COEDataView.MimeTypes.TEXT_PLAIN:
                    return "TEXT_PLAIN";
                case COEDataView.MimeTypes.TEXT_HTML:
                    return "TEXT_HTML";
                case COEDataView.MimeTypes.IMAGE_X_WMF:
                    return "IMAGE_X_WMF";
                case COEDataView.MimeTypes.IMAGE_PNG:
                    return "IMAGE_PNG";
                case COEDataView.MimeTypes.IMAGE_JPEG:
                    return "IMAGE_JPEG";
                case COEDataView.MimeTypes.IMAGE_GIF:
                    return "IMAGE_GIF";
                case COEDataView.MimeTypes.CHEMICAL_X_SMILES:
                    return "CHEMICAL_X_SMILES";
                case COEDataView.MimeTypes.CHEMICAL_X_MDLMOLFILE:
                    return "CHEMICAL_X_MDLMOLFILE";
                case COEDataView.MimeTypes.CHEMICAL_X_CDX:
                    return "CHEMICAL_X_CDX";
                case COEDataView.MimeTypes.CHEMICAL_X_MDL_CHIME:
                    return "CHEMICAL_X_CHIME";
                case COEDataView.MimeTypes.CHEMICAL_X_CML:
                    return "CHEMICAL_X_CML";
                case COEDataView.MimeTypes.CHEMICAL_X_DATADIRECT_CTAB:
                    return "CHEMICAL_DIRECT_BLOB_MOLECULE";
                case COEDataView.MimeTypes.APPLICATION_PDF:
                    return "APPLICATION_PDF";
                case COEDataView.MimeTypes.APPLICATION_MS_MSWORD:
                    return "APPLICATION_MS_MSWORD";
                case COEDataView.MimeTypes.APPLICATION_MS_EXCEL:
                    return "APPLICATION_MS_EXCEL";              
                default:
                    return "NONE";
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SearchCriteria.COELogicalOperators ToCOELogicalOperators(string value)
        {
            switch (value.Trim().ToUpper())
            {
                case "OR":
                    return SearchCriteria.COELogicalOperators.Or;
                case "AND":
                    return SearchCriteria.COELogicalOperators.And;
                default:
                    return SearchCriteria.COELogicalOperators.And;
            }
        }

        /// <summary>
        /// convert the search type  string to StructureSearchType enum.
        /// </summary>
        /// <param name="value">The search type string.</param>
        /// <returns>
        /// The structure search type enum.
        /// </returns>
        public static SearchCriteria.JChemStructureSearchType ToStructureSearchType(string value)
        {
            switch (value.Trim().ToUpper())
            {
                case "F":
                    return SearchCriteria.JChemStructureSearchType.Full;
                case "S":
                    return SearchCriteria.JChemStructureSearchType.SubStructure;
                case "T":
                    return SearchCriteria.JChemStructureSearchType.Similar;
                default:
                    return SearchCriteria.JChemStructureSearchType.Full;
            }
        }

        /// <summary>
        /// Convert the search type enum to string.
        /// </summary>
        /// <param name="searchType">The structure search type.</param>
        /// <returns>The structure search type string.</returns>
        public static string ToString(SearchCriteria.JChemStructureSearchType searchType)
        {
            switch (searchType)
            {
                case SearchCriteria.JChemStructureSearchType.Full:
                    return "F";
                case SearchCriteria.JChemStructureSearchType.SubStructure:
                    return "S";
                case SearchCriteria.JChemStructureSearchType.Similar:
                    return "T";
                default:
                    return "F";
            } 
        }        
	}
}

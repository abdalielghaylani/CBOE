//===========================================================================
// CambridgeSoft Corp. Copyright © 2004-2007, All rights reserved.
// 
// Enumerations.cs
//
// Description:
//      
//
// Created On: 8/12/2006 10:17:34 AM
// Created By: Sunil Gupta <mailto:sgupta@camsoft.com> 
//===========================================================================
using System;

namespace CambridgeSoft.COE.Framework.Common.MolSrvWrapper
{
	/// <summary>
	/// Types of searches available against MolServer.
	/// </summary>
	public enum CSStrucSearchType 
	{ 
		/// <summary>
		/// An exact search.
		/// </summary>
		Exact,
		/// <summary>
		/// A search by substructure.
		/// </summary>
		Substructure,
		/// <summary>
		/// A search by similarity.
		/// </summary>
		Similarity,
		/// <summary>
		/// None.
		/// </summary>
		None
	};

	/// <summary>
	/// Supported FieldTypes
	/// </summary>
	public enum CSFieldType	
	{
		/// <summary>
		/// Formula in IUPAC standard notation.
		/// </summary>
		CSFDFormula,
		/// <summary>
		/// Molecular weight.
		/// </summary>
		CSFDMolWt,
		/// <summary>
		/// Simple structure notation.
		/// </summary>
		CSFDSmiles,
		/// <summary>
		/// Full structure.
		/// </summary>
		CSFDStructure,
		/// <summary>
		/// A file with a Structure.
		/// </summary>
		CSFDStructFile,
		/// <summary>
		/// A cdx coded in Base64.
		/// </summary>
		CSFDBase64Cdx,
		/// <summary>
		/// A realtional field.
		/// </summary>
		CSFDRelational,
		/// <summary>
		/// Others.
		/// </summary>
		CSFDUnknown
	};
}

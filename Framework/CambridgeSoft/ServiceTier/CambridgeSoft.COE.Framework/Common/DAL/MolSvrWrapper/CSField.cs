//===========================================================================
// CambridgeSoft Corp. Copyright © 2004-2007, All rights reserved.
// 
// CSField.cs
//
// Description:
//      
//
// Created On: 8/12/2006 10:15:48 AM
// Created By: Sunil Gupta <mailto:sgupta@camsoft.com> 
//===========================================================================
using System;

namespace CambridgeSoft.COE.Framework.Common.MolSrvWrapper
{
	/// <summary>
	/// Summary description for CSField.
	/// </summary>
	public class CSField
	{
		#region Variables
		private CSFieldType type;
		private CSStrucSearchType strucSearchType;
		private string val;
		#endregion

		#region Constructors
		public CSField(CSFieldType fldType, 
			CSStrucSearchType fldStrucSearchType, string fldValue)
		{
			type = fldType;
			val = fldValue;
			strucSearchType = fldStrucSearchType;
		}
		#endregion

		#region Properties
		public CSFieldType Type
		{
			get
			{
				return type;
			}
		}

		public CSStrucSearchType StrucSearchType
		{
			get
			{
				return strucSearchType;
			}
		}

		public string Value
		{
			get
			{
				return val;
			}
		}
		#endregion
	}
}

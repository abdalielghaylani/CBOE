using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils
{
	/// <summary>
	/// Provides translation for different special characters between diferent RDBMS
	/// </summary>
	public class SpecialCharacters
	{
		#region Constants
		private const string DefaultParamSeparator = ":";
		private const string OracleParamSeparator = ":";
		private const string SQLServerParamSeparator = "?";
		private const string MSAccessParamSeparator = "?";
		#endregion

		#region Methods
		/// <summary>
		/// Returns the character used by a database for parametrized queries.
		/// </summary>
		/// <param name="dataBaseType">The underlying database.</param>
		/// <returns>The charachter that is used for parameters.</returns>
		public static string GetParameterSeparatorChar(DBMSType dataBaseType) {
			switch(dataBaseType) {
				case DBMSType.ORACLE:
					return OracleParamSeparator;
				case DBMSType.SQLSERVER:
					return MSAccessParamSeparator;
				default:
					return DefaultParamSeparator;
			}
		}
		#endregion
	}
}

using System;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils
{
	/// <summary>
	/// Provides static methods to manipulate data types.
	/// </summary>
	public class TypesConversor
	{
		/// <summary>
		/// Get a DbType from a string.
		/// </summary>
		/// <param name="type">The type as string.</param>
		/// <returns>The type as DbType.</returns>
		public static DbType GetType(string type) {
			switch (type.Trim().ToLower()) {
				case "text":
                case "clob":
                case "blob":
					return DbType.String;
				case "integer":
					return DbType.Int32;
				case "real":
					return DbType.Decimal;
				case "date":
					return DbType.DateTime;
                case "boolean":
                    return DbType.Boolean;
				default:
                    throw new UnsupportedDataTypeException(Resources.UnsupportedDataType + " " + type);
			}
		}

		/// <summary>
		/// Utility method to know if a DBType is numeric or not.
		/// </summary>
		/// <param name="type">The DbType.</param>
		/// <returns>True if numeric, false otherwise.</returns>
		public static bool IsNumeric(DbType type) {
			switch (type) {
				case DbType.Decimal:
				case DbType.Int32:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Utility method to know if a DBType is Text or not.
		/// </summary>
		/// <param name="type">The DbType.</param>
		/// <returns>True if Text, false otherwise.</returns>
		public static bool IsText(DbType type) {
			switch (type) {
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
                case DbType.StringFixedLength:
				case DbType.Xml:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Utility method to know if a DBType is Boolean or not.
		/// </summary>
		/// <param name="type">The DbType.</param>
		/// <returns>True if Boolean, false otherwise.</returns>
		public static bool IsBoolean(DbType type) {
			switch (type) {
				case DbType.Boolean:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Utility method to know if a DBType is Date or not.
		/// </summary>
		/// <param name="type">The DbType.</param>
		/// <returns>True if Date, false otherwise.</returns>
		public static bool IsDate(DbType type) {
			switch (type) {
				case DbType.DateTime:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the equivalent abstract type, given a DbType.
		/// </summary>
		/// <param name="concreteType">The DbType.</param>
		/// <returns>The abstract type.</returns>
        public static COEDataView.AbstractTypes GetAbstractType(DbType concreteType) {
			switch (concreteType) {
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.Object:
                    return COEDataView.AbstractTypes.Text;
				case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return COEDataView.AbstractTypes.Integer;
				case DbType.Decimal:
                case DbType.Double:
                case DbType.VarNumeric:
                    return COEDataView.AbstractTypes.Real;
				case DbType.Boolean:
                    return COEDataView.AbstractTypes.Boolean;
                case DbType.Date:
                case DbType.Time:
                case DbType.DateTime:
                    return COEDataView.AbstractTypes.Date;
				default:
                    throw new UnsupportedDataTypeException(Resources.UnsupportedDataType + concreteType.ToString());
			}
		}
	}
}

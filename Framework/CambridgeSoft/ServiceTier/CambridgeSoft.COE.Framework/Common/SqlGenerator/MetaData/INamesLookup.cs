using System;
using System.Data;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData
{
	/// <summary>
	/// Iterface that expose methods for lookups.
	/// </summary>
	public interface INamesLookup
	{
		/// <summary>
		/// Gets the field name from its id.
		/// </summary>
		/// <param name="fieldIndex">The id in the xml.</param>
		/// <returns>The name as string.</returns>
		string GetFieldName(int fieldIndex);
		
        /// <summary>
		/// Gets the field type from its id.
		/// </summary>
		/// <param name="fieldIndex">The id in the xml.</param>
		/// <returns>The SqlDbType.</returns>
		DbType GetFieldType(int fieldIndex);
	
        /// <summary>
		/// Gets the Parent Table of a field from the field id.
		/// </summary>
		/// <param name="fieldIndex">The field id in the xml.</param>
		/// <returns>The Table instance.</returns>
		Table GetParentTable(int fieldIndex);
		
        /// <summary>
		/// Gets a table from its id.
		/// </summary>
		/// <param name="tableIndex">The table id in the xml.</param>
		/// <returns>The Table instance.</returns>
		Table GetTable(int tableIndex);

        /// <summary>
        /// Gets an IColumn from its id. This can be a lookup or a regular field.
        /// </summary>
        /// <param name="fieldIndex">The field id in the xml.</param>
        /// <returns>The IColumn instance.</returns>
        IColumn GetColumn(int fieldIndex);

        /// <summary>
        /// Gets the alias of a column from its id.
        /// </summary>
        /// <param name="fieldId">The field id in  the xml.</param>
        /// <returns>The alias.</returns>
        string GetColumnAlias(int fieldId);

        /// <summary>
        /// Gets the base table's primary key field
        /// </summary>
        /// <returns>The base table's primary key field</returns>
        Field GetBaseTablePK();
    }
}

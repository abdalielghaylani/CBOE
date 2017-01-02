using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries
{
	/// <summary>
	/// Insert non query statement.
	/// </summary>
	public class Truncate : NonQuery
	{
		#region Variables
		private Query selectStatement;
		#endregion

		#region Properties
		/// <summary>
		/// If the values to be inserted are to be extracted from a subquery, the query is specified in this property.
		/// </summary>
		public Query SelectStatement
		{
			get {
				return selectStatement;
			}
			set {
				this.selectStatement = value;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Truncate() : base() {
		}

		/// <summary>
		/// Returns the INSERT itself as a string, and the list of parameters.
		/// </summary>
		/// <param name="databaseType">The DBMSType.</param>
		/// <returns>The string of the insert statement.</returns>
		public override string GetDependantString(DBMSType databaseType) {
			if(MainTable == null)
				throw new SQLGeneratorException(Resources.MainTableNotSet);
            
			StringBuilder builder = new StringBuilder(string.Empty);
            // Edited by Q3 technologies(Chitrank,Gaurav)

            builder.Append("Truncate table "); 
			if(this.MainTable.Database.Trim() != "")
				builder.AppendFormat("{0}.", this.MainTable.Database);
			builder.Append(MainTable.TableName);
		

			return builder.ToString();
		}
		#endregion

	}
}

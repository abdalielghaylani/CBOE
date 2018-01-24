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
	public class Insert : NonQuery
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
		public Insert() : base() {
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

			builder.Append("INSERT INTO "); 
			if(this.MainTable.Database.Trim() != "")
				builder.AppendFormat("{0}.", this.MainTable.Database);
			builder.Append(MainTable.TableName);
			if(Fields.Count > 0) {
				builder.Append(" (");
				for(int index = 0; index < Fields.Count; index++) {
					builder.Append(Fields[index].FieldName);
					if(index < Fields.Count - 1)
						builder.Append(", ");
				}
				builder.Append(") ");
			}

			if(selectStatement == null) {
				builder.Append("VALUES (");
				for(int index = 0; index < this.ParamValues.Count; index++ ) {
					builder.Append(this.ParameterHolder);
					
					if(this.UseParametersByName)
						builder.Append(index);

					if(index < this.ParamValues.Count - 1)
						builder.Append(", ");
					//ParamValues.Add(currentValue);
				}
				builder.Append(")");
			} else {
				selectStatement.DataBaseType = databaseType;
				builder.AppendFormat(" ({0})", selectStatement.ToString());

				this.ParamValues.Clear();
				foreach(Value currentValue in selectStatement.ParamValues)
					this.ParamValues.Add(currentValue);
			}

			return builder.ToString();
		}
		#endregion

	}
}

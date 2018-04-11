using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a less than comparison operator, "&gt;" or “&gt;=”, in a  SQL where clause.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseLessThan class requires the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item><b>Field dataField:</b> Represents the field of the database that is being compared. Its name and type are required.</item>
	/// <item><b>Value Val:</b> represents the value against wich the dataField is being compared. Its value and type are required. The type of the field can differ from the type of the value.</item>
	/// <item><b>bool GreaterEqual:</b> flag that indicates whether it’s &gt; or &gt;=.</item>
	/// <item><b>bool CaseSensitive:</b> flag that indicates whether take into account casing or not.</item>
	/// <item><b>Position TrimPosition:</b> flag that indicates how to perform the trim of value. Posible Values: Left, Right, Both, None.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// The WhereClause decides whether it’s TextCriteria, NumericalCriteria, etc. based upon the Val Value field.
	/// This Where Clause can take into account casing and can perform trim of the input value. 
	/// In the special case of a numericalCriteria that is applied into a text database field, the trim operation trims leading and or trailing zeroes (instead of just spaces).
	/// In the case of a TextCriteria, this implementation compares the ascii codes of the strings.
	/// </para>
	/// <para>
	/// The SQL generation aims prepared statements rather than simple queries: parameters are not hardcoded into the query text, but marked with a parameter holder character. Parameters are returned in a separate list, ordered by position as they appear in the query.
	/// Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
	/// </para>
	/// <para>
	/// <b>Example:</b>
	/// </para>
	/// <b>Programatically:</b>
	/// <code lang="C#">
	/// WhereClauseGreaterThan target = new WhereClauseGreaterThan();
	/// target.DataField = new Field();
	/// target.DataField.FieldName = "MolWeight";
	/// target.DataField.FieldType = SqlDbType.Float;
	/// target.Val.Val = "00012.0000";
	/// target.Val.Type = SqlDbType.Float;
	/// target.GreaterEqual = true;
	/// target.TrimPosition = Positions.Both;
	/// Query.AddWhereClauseItem(clause);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;structureCriteriaItem id=XX fieldid=19 modifier=’’ tableid=3&gt;
	/// 	&lt;numericalCriteria
	/// 		operator='GTE' trim='BOTH'&gt;00012.0000
	/// 	&lt;/numericalCriteria&gt;
	/// &lt;/structureCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// [AND] (MolWeight &gt;= :0);
	/// </para>
	/// <para>
	/// And will add the following parameters to the query parameter list:
	/// </para>
	/// <list type="bullet">
	/// <item>Value(12, DataFieldtype.Float)</item>
	/// </list>
	/// </summary>
	public class WhereClauseGreaterThan : WhereClauseBinaryOperation
	{
		#region Properties
		/// <summary>
		/// Determines if it will be a greater or a greater equal operation.
		/// </summary>
		public bool GreaterEqual {
			get {
				return this.greaterEqual;
			}
			set {
				this.greaterEqual = value;
			}
		}
		/// <summary>
		/// Determines if the value must be triimed, and where.
		/// </summary>
		public SearchCriteria.Positions TrimPosition {
			get {
				return this.trimPosition;
			}
			set {
				this.trimPosition = value;
			}
		}

        /// <summary>
        /// Gets or sets if it is necessary to normalize the name.
        /// </summary>
		public bool NormalizeChemicalName {
			get { return normalizeChemicalName; }
			set { normalizeChemicalName = value; }
		}
		#endregion

		#region Variables
		private bool greaterEqual;
		private SearchCriteria.Positions trimPosition;
		private bool normalizeChemicalName;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes its members to its default values.
		/// </summary>
		public WhereClauseGreaterThan() : base() {
			this.dataField = new Field();
			this.val = new Value();
			this.greaterEqual = false;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Encapsulates different sintaxis for the underlying RDBMS. If the equal will be case insensitive,
		/// then a lower is applied to the field and the value is lowered too.
		/// </summary>
		/// <exception cref="System.Exception">Thrown when the data type is not Text</exception>
		/// <returns>A string for the underlying RDBMS, in the form of: FieldName > FieldValue</returns>
		protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
		{
			StringBuilder builder = new StringBuilder(string.Empty);

			this.val.Val = NormalizationUtils.TrimValue(this.val.Val, this.val.Type, this.TrimPosition);

			switch(TypesConversor.GetAbstractType(this.dataField.FieldType)) {
                case COEDataView.AbstractTypes.Text:
					builder.Append("(");
					builder.Append(this.GetStartingTrimStatement(dataBaseType));
					builder.Append(base.GetFullName(dataField));
					builder.Append(this.GetEndingTrimStatement(dataBaseType));
                    
					builder.Append(" " + (greaterEqual?">=":">") + " ");

                    if(this.NormalizeChemicalName) {
                        builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                    }
					builder.Append(this.ParameterHolder);
					if(this.UseParametersByName)
						builder.Append(values.Count);

                    if(this.NormalizeChemicalName) {
                        builder.Append(")");
                    }

					builder.Append(")");
					values.Add(val);
					break;
                case COEDataView.AbstractTypes.Boolean:
                case COEDataView.AbstractTypes.Real:
                case COEDataView.AbstractTypes.Integer:
					builder.Append("(");
					builder.Append(base.GetFullName(dataField));
					builder.Append(" " + (greaterEqual ? ">=" : ">") + " ");
					builder.Append(this.ParameterHolder);
					if(this.UseParametersByName)
						builder.Append(values.Count);
					builder.Append(")");
					values.Add(val);
					break;
                case COEDataView.AbstractTypes.Date:
					builder.Append("(");
					builder.Append(base.GetFullName(dataField));
					builder.Append(" " + (greaterEqual ? ">=" : ">") + " ");
					switch(dataBaseType) {
						case DBMSType.ORACLE:
                            //TO_DATE('09/22/2006 11:02:26','MM/DD/YYYY HH:MI:SS') (Oracle)
							builder.Append("TO_DATE(");
							builder.Append(this.ParameterHolder);
							if(this.UseParametersByName)
								builder.Append(values.Count);
                            builder.Append(",'" + Resources.DefaultDateFormat.Replace("HH", "hh24").Replace(":mm:", ":mi:") + "')");
							values.Add(val);
							break;
						case DBMSType.SQLSERVER:
                            //CONVERT(DATETIME, '09/13/2006 11:02:26' , 101)
							builder.Append("CONVERT(DATETIME,");
							builder.Append(this.ParameterHolder);
							if(this.UseParametersByName)
								builder.Append(values.Count);
							builder.Append(",101)");
							values.Add(val);
							break;
						case DBMSType.MSACCESS:
                            //FORMAT('09/13/2006 11:02:26', 'MM/DD/YYYY HH:MI:SS')
							builder.Append("FORMAT(");
							builder.Append(this.ParameterHolder);
							if(this.UseParametersByName)
								builder.Append(values.Count);
                            builder.Append(",'" + Resources.DefaultDateFormat + "')");
							values.Add(val);
							break;
					}
					builder.Append(")");
					break;
			}
			return builder.ToString();
		}

		private string GetStartingTrimStatement(DBMSType dataBaseType) {
			switch(this.trimPosition) {
				case SearchCriteria.Positions.Left:
					return "LTRIM(";
				case SearchCriteria.Positions.Right:
					return "RTRIM(";
				case SearchCriteria.Positions.Both:
					return "TRIM(";
			}
			return "";
		}

		private string GetEndingTrimStatement(DBMSType dataBaseType) {
			if(this.trimPosition != SearchCriteria.Positions.None)
				return ")";
			return "";
		}
		#endregion
	}
}

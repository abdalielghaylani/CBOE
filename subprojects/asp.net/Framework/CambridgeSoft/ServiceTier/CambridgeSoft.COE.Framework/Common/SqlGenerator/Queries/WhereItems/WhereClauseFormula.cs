using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a FormulaContains operator, which is an Oracle depandant query. The query is of type "CartridgeSchema.FormulaContains(fld, formula, options)=1”.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseFormula class required the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item>Field dataField: Represents the field of the database that is being compared. Its name and type are required.</item>
	/// <item>Value Val: represents the formula against which the dataField is being compared. Its value and type are required.</item>
    /// <item>bool Full: flag that indicates whether it’s full or not. This is passed to options SQL parameter. If full match is specified, then no other kinds of atoms are allowed in the formula</item>
	/// <item>String  CartridgeSchema: This should be the name of the schema for the Cs Cartridge. By default it is set to CsCartridge.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// This implementation will work on Oracle database only and will throw an exception if other database is used.
	///The SQL generation aims prepared statements rather than simple queries: parameters are not hardcoded into the query text, but marked with a parameter holder character. Parameters are returned in a separate list, ordered by position as they appear in the query.
	///Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
	/// </para>
	/// <para>
	/// <b>Example:</b>
	/// </para>
	/// <b>Programatically:</b>
	/// <code lang="C#">
	/// WhereClauseFormula clause = new WhereClauseFormula();
	/// clause.DataField.FieldId = 20;
	/// clause.DataField.FieldName = "Base64_cdx";
	/// clause.DataField.FieldType = System.Data.DbType.VarBinary;
	/// clause.Val.Val = "c1ccccc1";
	/// clause.CartridgeSchema = "CSCARTRIDGE";
	/// Query.AddWhereClauseItem(clause);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;searchCriteriaItem id=XX fieldid=20 modifier=’’ tableid=3&gt;
	/// 	&lt;FormulaCriteria&gt;
	/// 		&lt;CsCartridgeFormulaCriteria Full=’NO’&gt;c1ccccc1
	/// 		&lt;/CsCartridgeFormulaCriteria&gt;
	/// 	&lt;/FormulaCriteria&gt;
	/// &lt;searchCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// [AND] (CSCARTRIDGE.FormulaContains(:1, :2, :3)=1);
	/// </para>
	/// <para>
	/// And will add the following parameters to the query parameter list:
	/// </para>
	/// <list type="bullet">
	///		<item>Value(Base64_cdx, DataFieldtype.Text),</item>
	///		<item>Value(c1ccccc1, DataFieldtype.Text),</item>
	///		<item>Value(FULL=YES, DataFieldtype.Text)</item>
	/// </list>
	/// <para>
	/// <b>To Be Done:</b> There is no way to define the CartridgeSchema in the xml. We should extend the xml to allow for it.
	/// </para>
	/// </summary>
	public class WhereClauseFormula : WhereClauseBinaryOperation
	{

		#region Properties
        /// <summary>
        /// Gets or sets if it is a full word search.
        /// </summary>
		public bool Full {
			get {
				return this.full;
			}
			set {
				this.full = value;
			}
		}

        /// <summary>
        /// Gets or sets the cartridge schema name.
        /// </summary>
		public string CartridgeSchema {
			get {
				return this.cartridgeSchema;
			}
			set {
				this.cartridgeSchema = value;
			}
		}
		#endregion

		#region Variables
		private bool full;
		private string cartridgeSchema;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
		public WhereClauseFormula() : base() {
			this.dataField = new Field();
			this.val = new Value();
			this.full = false;
            //this.cartridgeSchema = "CsCartridge";
		}
		#endregion

		#region Methods
        /// <summary>
        /// Gets the string representation of the Where clause formula for the given database and adds
        /// its param values to the list of param values of the query that cames in "values".
        /// </summary>
        /// <param name="databaseType">The database te get the string for.</param>
        /// <param name="values">The existing param values of a query.</param>
        /// <returns>A string of the form: (CartridgeSchema.FormulaContains(:1, :2, :3)=1).</returns>
		protected override string GetDependantString(DBMSType databaseType, List<Value> values) {
			// The xml snippet that is represented here looks like:
			// <CsCartridgeFormulaCriteria full="NO">C5-10Br2-4</CsCartridgeFormulaCriteria>
			if (databaseType != DBMSType.ORACLE) {
				throw new Exception("This clause only works in Oracle implementations");
			}
            //LJB 2/2/2009 fixe hardcoding of cartridge schema name
            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(databaseType);

			string options = this.full ? "FULL=YES" : "FULL=NO";
			StringBuilder builder = new StringBuilder(cartridgeSchema);
			
			// CsCartridge.FormulaContains(fieldid, 'C5-10Br2-4', '')=1
			builder.Append(".FormulaContains(");
			builder.Append(base.GetFullName(dataField));

			builder.Append(", ");
			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);

			values.Add(val);
			
			builder.Append(", ");
			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);

			values.Add(new Value(options, System.Data.DbType.String));

			builder.Append(")=1");

			return builder.ToString();
		}
		#endregion
	}
}

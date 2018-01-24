using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a MolWeightContains operator, which is an Oracle depandant query. The query is of type "CartridgeSchema.MolWeightContains(fld, massMin, massMax, options)=1”.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseMolWeight class required the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item>Field dataField: Represents the field of the database that is being compared. Its name and type are required.</item>
	/// <item>Value Val: This value is currently discarded. There is no use in filling it.</item>
	/// <item>double MinMass: The lower mass to compare to.</item>
	/// <item>double MaxMass: The higher mass to compare to.</item>
	/// <item>String  CartridgeSchema: This should be the name of the schema for the Cs Cartridge. By default it is set to CsCartridge.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// This implementation will work on Oracle database only and will throw an exception if other database is used.
	/// The Val property is discarded.
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
	/// WhereClauseMolWeight clause = new WhereClauseMolWeight();
	/// clause.DataField.FieldId = 20;
	/// clause.DataField.FieldName = "Base64_cdx";
	/// clause.DataField.FieldType = System.Data.SqlDbType.VarBinary;
	/// clause.MinMass = 100.5;
	/// clause.MaxMass = 200;
	/// clause.CartridgeSchema = "CSCARTRIDGE";
	/// Query.AddWhereClauseItem(clause);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;searchCriteriaItem id=XX fieldid=20 modifier=’’ tableid=3&gt;
	/// 	&lt;MolWeightCriteria&gt;
	/// 		&lt;CsCartridgeMolWeightCriteria min=’100.5’ max=’200’ /&gt;
	///		&lt;/MolWeightCriteria>
	/// &lt;searchCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// [AND] (CSCARTRIDGE.MolWeightContains(:1, :2, :3, :4)=1);
	/// </para>
	/// <para>
	/// And will add the following parameters to the query parameter list:
	/// </para>
	/// <list type="bullet">
	///		<item>Value(Base64_cdx, DataFieldtype.Text),</item>
	///		<item>Value(100.5, DataFieldtype.Real),</item>
	///		<item>Value(200, DataFieldtype.Real),</item>
	///		<item>Value(‘’, DataFieldtype.Text)</item>
	/// </list>
	/// <para>
	/// <b>To Be Done:</b> There is no way to define the CartridgeSchema in the xml. We should extend the xml to allow for it.
	/// </para>
	/// 
	/// </summary>
	public class WhereClauseMolWeight : WhereClauseBinaryOperation
	{

		#region Properties
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

        /// <summary>
        /// Gets or sets the min mass.
        /// </summary>
		public double MinMass {
			get {
				return this.minMass;
			}
			set {
				this.minMass = value;
			}
		}

        /// <summary>
        /// Gets or sets the max mass.
        /// </summary>
		public double MaxMass {
			get {
				return this.maxMass;
			}
			set {
				this.maxMass = value;
			}
		}
		#endregion

		#region Variables
		private string cartridgeSchema;
		private double minMass;
		private double maxMass;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
		public WhereClauseMolWeight() : base(){
			this.dataField = new Field();
			this.val = new Value();// This value is discarded
			this.minMass = 0.0;
			this.maxMass = 0.0;
            //this.cartridgeSchema = "CsCartridge";
		}
		#endregion

		#region Methods
        /// <summary>
        /// Gets the string representation of the molweight clause for the underlying database, and adds
        /// its param values to the already existing list of values of a query.
        /// </summary>
        /// <param name="databaseType">The database.</param>
        /// <param name="values">The already existing param values of a query.</param>
        /// <returns>The string representation.</returns>
		protected override string GetDependantString(DBMSType databaseType, List<Value> values) {
			
			// CsCartridge.MolWeightContains(fld, massMin, massMax, ‘’)=1
            //LJB 2/2/2009 fixe hardcoding of cartridge schema name
            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(databaseType);

			if (databaseType != DBMSType.ORACLE) {
				throw new Exception("This clause only works in Oracle implementations");
			}

			StringBuilder builder = new StringBuilder(cartridgeSchema);
			builder.Append(".MolWeightContains(");
			builder.Append(base.GetFullName(dataField));
			builder.Append(", ");

			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);
			values.Add(new Value(minMass.ToString(), System.Data.DbType.Double));
			
			builder.Append(", ");

			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);
            values.Add(new Value(maxMass.ToString(), System.Data.DbType.Double));

			builder.Append(", ");

			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);
			values.Add(new Value("", System.Data.DbType.String));
			
			builder.Append(")=1");
			
			return builder.ToString();
		}
		#endregion
	}
}

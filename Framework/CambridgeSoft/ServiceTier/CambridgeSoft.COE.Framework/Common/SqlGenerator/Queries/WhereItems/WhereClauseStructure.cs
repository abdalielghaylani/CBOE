using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.COEConfigurationService;
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a MolWeightContains operator, which is an Oracle depandant query. The query is of type "CsCartridge.MoleculeContains(fld, 'SELECT query FROM CsCartridge.Queries WHERE id = value', '', options)=1”.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseStructure class required the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item>Field dataField: Represents the field of the database that is being compared. Its name is required.</item>
	/// <item>Value Val: The value should contain the id of the already inserted query molecule</item>
	/// <item>String  CartridgeSchema: This should be the name of the schema for the CsCartridge. By default it is set to CsCartridge.</item>
	/// <item>bool absoluteHitsRel: Default is false.</item>
	/// <item>bool relativeTetStereo: Default is false.</item>
	/// <item>TetrahedralStereoMatching tetrahedralStereo: Default is Yes. Possible values are: [YES|NO|SAME|EITHER|ANY]</item>
	/// <item>int simThreshold: To set the similarity percentage when SIMILAR=YES is set as well. Otherwise disregarded. Default value (used if it is omitted) is 90 percent.</item>
	/// <item>bool reactionCenter: To specify if reaction search can match only reaction centers. Default is YES.</item>
	/// <item>bool fullSearch: To specify full structure search. Default is substructure search (NO).</item>
	/// <item>bool tautometer: Default is NO.</item>
	/// <item>bool fragmentsOverlap: Default is NO.</item>
	/// <item>bool permitExtraneousFragmentsIfRXN: Default is NO.</item>
	/// <item>bool permitExtraneousFragments: Default is NO.</item>
	/// <item>bool doubleBondStereo: Default is YES.</item>
	/// <item>bool hitAnyChargeHetero: Default is YES.</item>
	/// <item>bool identity: Default is NO.</item>
	/// <item>bool hitAnyChargeCarbon: Default is YES.</item>
	/// <item>bool similar: Similarity search. Default is NO.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// This implementation will work on Oracle database only and will throw an exception if other database is used.
	/// It is expected that the query molecule was already inserted into Queries table.
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
	/// WhereClauseStructure clause = new WhereClauseStructure();
	/// clause.DataField.FieldId = 20;
	/// clause.DataField.FieldName = "Base64_cdx";
	/// clause.DataField.FieldType = System.Data.SqlDbType.VarBinary;
	/// clause.absoluteHitsRel = false;
	/// clause.relativeTetStereo = false;
	/// clause.tetrahedralStereo = true;
	/// clause.simThreshold = 100;
	/// clause.reactionCenter = true;
	/// clause.fullSearch = false;
	/// clause.tautometer = false;
	/// clause.fragmentsOverlap = false;
	/// clause.permitExtraneousFragmentsIfRXN = false;
	/// clause.permitExtraneousFragments = false;
	/// clause.doubleBondStereo = false;
	/// clause.hitAnyChargeHetero = true;
	/// clause.identity = false;
	/// clause.hitAnyChargeCarbo = true;
	/// clause.similar = false;
	/// clause.CartridgeSchema = "CSCARTRIDGE";
	/// Query.AddWhereClauseItem(clause);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;structureCriteriaItem id=XX fieldid=20 modifier=’’ tableid=3&gt;
	/// 	&lt;CSCartridgeStructureCriteria
	/// 		absolutehitsrel=" NO" 
	/// 		relativeTetStereo=" NO" 
	/// 		tetrahedralStereo="YES" 
	/// 		simThreshold="100" 
	/// 		reactionCenter="YES" 
	/// 		full="NO" 
	/// 		tautomer="NO" 
	/// 		fragmentsoverlap="NO" 
	/// 		permitExtraneousFragmentsIfRXN="NO" 
	/// 		permitExtraneousFragments="NO" 
	/// 		doublebondstereo="NO" 
	/// 		hitAnyChargeHetero="YES" 
	/// 		identity="NO" 
	/// 		hitAnyChargeCarbon="YES" 
	/// 		similar="NO"
	/// 		format=”BASE64CDX”&gt;value
	/// 	&lt;/CSCartridgeStructureCriteria&gt;
	/// &lt;/structureCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// [AND] (CSCARTRIDGE.MoleculeContains(:0, :1, :2, :3)=1);
	/// </para>
	/// <para>
	/// And will add the following parameters to the query parameter list:
	/// </para>
	/// <list type="bullet">
	/// <item>Value("Base64_cdx", System.Data.DbType.String)),</item>
	/// <item>Value("SELECT query FROM CSCARTRIDGE.Queries WHERE id = 15", System.Data.DbType.String),</item>
	/// <item>Value("", System.Data.DbType.String),</item>
	/// <item>Value("absoluteHitsRel=NO, relativeTetStereo=NO, tetrahedralStereo=Yes, simThreshold=90, reactionCenter=YES, fullSearch=NO, tautometer=NO, fragmentsOverlap=NO, permitExtraneousFragmentsIfRXN=NO, permitExtraneousFragments=NO, doubleBondStereo=YES, hitAnyChargeHetero=YES, identity=NO, hitAnyChargeCarbon=YES, similar=NO, ignoreImplicitH=NO", System.Data.DbType.String)</item>
	/// </list>
	/// <para>
	/// <b>To Be Done:</b> There is no way to define the CartridgeSchema in the xml. We should extend the xml to allow for it.
	/// </para>
    /// <para>
    /// Implement the following parameters:
    /// </para>
    /// <list type="table">
    /// <item>COST=&lt;number&gt; - to communicate with the Oracle query optimizer. See the chapter Query optimization.</item>
    /// <item>FUNCTIONCOST=&lt;number&gt; - to communicate with the Oracle query optimizer. See the chapter Query optimization.</item>
    /// </list>
    /// <para>
	/// Review whether the following parameters should be added:
	/// </para>
	/// <list type="table">
    /// <item>COMPLETE=&lt;number&gt; - to limit the number of hits tested against atom by atom matching.</item>
    /// <item>MASSMAX=&lt;number&gt; - to prescribe maximum molecular mass to matching records.</item>
    /// <item>MASSMIN=&lt;number&gt; - to prescribe minimum molecular mass to matching records.</item>
	/// <item>USE_THREADS=[YES|NO] - Default value is read from the Globals table.</item>
    /// <item>SELECTIVITY=&lt;number&gt; - to communicate with the Oracle query optimizer. See the chapter Query optimization.</item>
    /// <item>COST=&lt;number&gt; - to communicate with the Oracle query optimizer. See the chapter Query optimization.</item>
    /// <item>FUNCTIONCOST=&lt;number&gt; - to communicate with the Oracle query optimizer. See the chapter Query optimization.</item>
	/// </list>
	/// </summary>
	public class WhereClauseStructure : WhereClauseBinaryOperation
	{

		#region Properties
        /// <summary>
        /// Gets or sets the cartridge schema.
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
        /// Gets or sets if absolute hits or not. Default is NO.
        /// </summary>
		public bool AbsoluteHitsRel{
			get {
				return this.absoluteHitsRel;
			}
			set {
				this.absoluteHitsRel = value;
			}
		}

        /// <summary>
        /// Gets or sets if relative tet stereo. Default is NO.
        /// </summary>
		public bool RelativeTetStereo {
			get {
				return this.relativeTetStereo;
			}
			set {
				this.relativeTetStereo = value;
			}
		}

        /// <summary>
        /// Gets or sets the tetrahedral stereo matching. Default is YES.
        /// </summary>
		public SearchCriteria.TetrahedralStereoMatching TetrahedralStereo {
			get {
				return this.tetrahedralStereo;
			}
			set {
				this.tetrahedralStereo = value;
			}
		}

        /// <summary>
        /// Gets or sets the similarity percentage. It is taken into account when SIMILAR=YES is set as well, 
        /// otherwise disregarded. Default value (used if it is omitted) is 90 percent.
        /// </summary>
		public int SimThreshold {
			get {
				return this.simThreshold;
			}
			set {
				this.simThreshold = value;
			}
		}

        /// <summary>
        /// Gets or sets if reaction search can match only reaction centers. Default is YES.
        /// </summary>
		public bool ReactionCenter {
			get {
				return this.reactionCenter;
			}
			set {
				this.reactionCenter = value;
			}
		}

        /// <summary>
        /// Gets or sets if it will be a full structure search. Default is substructure search.
        /// </summary>
		public bool FullSearch {
			get {
				return this.fullSearch;
			}
			set {
				this.fullSearch = value;
			}
		}

        /// <summary>
        /// Gets or sets if tautometer. Default is NO.
        /// </summary>
		public bool Tautometer {
			get {
				return this.tautometer;
			}
			set {
				this.tautometer = value;
			}
		}

        /// <summary>
        /// Gets or sets if fragments can overlap. Default is NO.
        /// </summary>
		public bool FragmentsOverlap {
			get {
				return this.fragmentsOverlap;
			}
			set {
				this.fragmentsOverlap = value;
			}
		}

        /// <summary>
        /// Gets or sets if permit extraneous fragments. Default is NO.
        /// </summary>
		public bool PermitExtraneousFragmentsIfRXN {
			get {
				return this.permitExtraneousFragmentsIfRXN;
			}
			set {
				this.permitExtraneousFragmentsIfRXN = value;
			}
		}

        /// <summary>
        /// Gets or sets if permit extraneous fragments. Default is NO.
        /// </summary>
		public bool PermitExtraneousFragments {
			get {
				return this.permitExtraneousFragments;
			}
			set {
				this.permitExtraneousFragments = value;
			}
		}

        /// <summary>
        /// Gets or sets if double bond stereo matching. Default is YES.
        /// </summary>
		public bool DoubleBondStereo {
			get {
				return this.doubleBondStereo;
			}
			set {
				this.doubleBondStereo = value;
			}
		}

        /// <summary>
        /// Gets or sets if hit any charged hetero atom. Default is YES.
        /// </summary>
		public bool HitAnyChargeHetero {
			get {
				return this.hitAnyChargeHetero;
			}
			set {
				this.hitAnyChargeHetero = value;
			}
		}

        /// <summary>
        /// Gets or sets if identity matching. Default is NO.
        /// </summary>
		public bool Identity {
			get {
				return this.identity;
			}
			set {
				this.identity = value;
			}
		}

        /// <summary>
        /// Gets or sets if hit any charged carbon. Default is YES.
        /// </summary>
		public bool HitAnyChargeCarbon {
			get {
				return this.hitAnyChargeCarbon;
			}
			set {
				this.hitAnyChargeCarbon = value;
			}
		}

        /// <summary>
        /// Gets or sets if similarity search. Default is NO.
        /// </summary>
		public bool Similar {
			get {
				return this.similar;
			}
			set {
				this.similar = value;
			}
		}

        /// <summary>
        /// Gets or sets if highlights should be determined. Default is NO.
        /// </summary>
        public bool Highlight
        {
            get
            {
                return this.highlight;
            }
            set
            {
                this.highlight = value;
            }
        }

        /// <summary>
        /// Gets or sets if implicit hydrogens should be ignored. Default is NO.
        /// </summary>
        public bool IgnoreImplicitHydrogens {
            get {
                return this.ignoreImplicitHydrogens;
            }
            set {
                this.ignoreImplicitHydrogens = value;
            }
        }

        /// <summary>
        /// Gets or sets the input format.
        /// </summary>
		public string Format {
			get {
				return this.format;
			}
			set {
				this.format = value;
			}
		}

		/// <summary>
		/// Query4000 is the first 4000 characters of the query string. The query string can be an encoded CDX document, a SMILES string, or a MolFile. For more information about these formats, please see DDL Considerations. The Oracle server limits the size of string literals to 4000 characters. 
		/// </summary>
		public string Query4000 {
			get {
				return this.Val.Val;
			}
			set {
				this.Val.Val = value;
			}
		}

		public string CartridgeParams
		{
			get
			{
				return this.cartridgeParams;
			}
			set
			{
				this.cartridgeParams = value;
			}
		}

		#endregion

		#region Variables
		private string cartridgeSchema;
		private bool absoluteHitsRel;
		private bool relativeTetStereo;
		private SearchCriteria.TetrahedralStereoMatching tetrahedralStereo;
		private int simThreshold;
		private bool reactionCenter;
		private bool fullSearch;
		private bool tautometer;
		private bool fragmentsOverlap;
		private bool permitExtraneousFragmentsIfRXN;
		private bool permitExtraneousFragments;
		private bool doubleBondStereo;
		private bool hitAnyChargeHetero;
		private bool identity;
		private bool hitAnyChargeCarbon;
		private bool similar;
        private bool highlight;
        private bool ignoreImplicitHydrogens;
		private string format;
		private string cartridgeParams;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
		public WhereClauseStructure() : base(){
			base.dataField = new Field();
			base.val = new Value();
            //this.cartridgeSchema = "CsCartridge";

            //TODO: Implement the following parameters
            /*
                COST=<number>-to communicate with the Oracle query optimizer. See the chapter Query optimization.
				FUNCTIONCOST=<number>-to communicate with the Oracle query optimizer. See the chapter Query optimization.
             */
            //TODO: Review the following parameters, that are not supported in xml but supported in cartridge. Also check if format is required
			/*
				COMPLETE=<number> - to limit the number of hits tested against atom by atom matching 
				MASSMAX=<number>- to prescribe maximum molecular mass to matching records 
				MASSMIN=<number> - to prescribe minimum molecular mass to matching records 
				USE_THREADS=[YES|NO] - Default value is read from the Globals table 
				SELECTIVITY=<number>-to communicate with the Oracle query optimizer. See the chapter Query optimization. 
			 */
			this.absoluteHitsRel = false;
			this.relativeTetStereo = false;
			this.tetrahedralStereo = SearchCriteria.TetrahedralStereoMatching.Yes;
			this.simThreshold = 90;
			this.reactionCenter = true;
			this.fullSearch = false;
			this.tautometer = false;
			this.fragmentsOverlap = false;
			this.permitExtraneousFragmentsIfRXN = false;
			this.permitExtraneousFragments = false;
			this.doubleBondStereo = true;
			this.hitAnyChargeHetero = true;
			this.hitAnyChargeCarbon = true;
			this.identity = false;
			this.similar = false;
            this.ignoreImplicitHydrogens = false;
			this.format = "BASE64CDX";
			this.cartridgeParams = string.Empty;
            this.highlight = false;
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Gets the string representation of the where clause structure for the given database and
        /// adds its parameter values to the values list.
		/// </summary>
		/// <param name="databaseType">The database to get the string for.</param>
		/// <param name="values">Already existing parameter values or a query.</param>
		/// <returns>The string representation.</returns>
		protected override string GetDependantString(DBMSType databaseType, List<Value> values) {
			/*
			<structureCriteria>
				<CSCartridgeStructureCriteria
					absolutehitsrel=" NO" 
					relativeTetStereo=" NO" 
					tetrahedralStereo="YES" 
					simThreshold="100" 
					reactionCenter="YES" 
					full="NO" 
					tautomer="NO" 
					fragmentsoverlap="NO" 
					permitExtraneousFragmentsIfRXN="NO" 
					permitExtraneousFragments="NO" 
					doublebondstereo="NO" 
					hitAnyChargeHetero="YES" 
					identity="NO" 
					hitAnyChargeCarbon="YES" 
					similar="NO"
					format=”BASE64CDX”
			        cartridgeParams="">query4000
				</CSCartridgeStructureCriteria>
			</structureCriteria>
			*/
			if (databaseType != DBMSType.ORACLE) {
				throw new Exception("This clause only works in Oracle implementations");
			}
            //LJB 2/2/2009 fixe hardcoding of cartridge schema name
            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(databaseType);

			// CsCartridge.MoleculeContains(mol, 'SELECT query FROM CsCartridge.Queries WHERE id = 1234', '', '')=1;
			//string query = "SELECT query FROM " + cartridgeSchema + ".tempQueries WHERE id = " + this.val.Val;
			string options = string.Empty;

			if ((cartridgeParams != null) && (cartridgeParams != string.Empty))
				options = cartridgeParams;
			else
			{
				options = BuildOptions();
			}
			StringBuilder builder = new StringBuilder(cartridgeSchema);
			builder.Append(".MoleculeContains(");
			builder.Append(base.GetFullName(dataField));
			builder.Append(", ");
			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);
			values.Add(new Value(this.Query4000, System.Data.DbType.String));
			builder.Append(", ");
            builder.Append("''");
			builder.Append(", ");
			builder.Append(this.ParameterHolder);
			if(this.UseParametersByName)
				builder.Append(values.Count);
			values.Add(new Value(options, System.Data.DbType.String));

            if (highlight)
            {
                builder.Append(",1");
            }

			builder.Append(")=1");

			return builder.ToString();
		}

		private string BuildOptions() {
			StringBuilder builder = new StringBuilder(string.Empty);
			builder.Append("absoluteHitsRel=" + GetStringFromBool(absoluteHitsRel));
			builder.Append(", relativeTetStereo=" + GetStringFromBool(relativeTetStereo));
			builder.Append(", tetrahedralStereo=" + tetrahedralStereo);
			builder.Append(", simThreshold=" + simThreshold);
			builder.Append(", reactionCenter=" + GetStringFromBool(reactionCenter));
			builder.Append(", full=" + GetStringFromBool(fullSearch));
            builder.Append(", tautomer=" + GetStringFromBool(tautometer));
			builder.Append(", fragmentsOverlap=" + GetStringFromBool(fragmentsOverlap));
			builder.Append(", permitExtraneousFragmentsIfRXN=" + GetStringFromBool(permitExtraneousFragmentsIfRXN));
			builder.Append(", permitExtraneousFragments=" + GetStringFromBool(permitExtraneousFragments));
			builder.Append(", doubleBondStereo=" + GetStringFromBool(doubleBondStereo));
			builder.Append(", hitAnyChargeHetero=" + GetStringFromBool(hitAnyChargeHetero));
			builder.Append(", identity=" + GetStringFromBool(identity));
			builder.Append(", hitAnyChargeCarbon=" + GetStringFromBool(hitAnyChargeCarbon));
			builder.Append(", similar=" + GetStringFromBool(similar));
            
            //JHS 10/25/2010 - I realize this does not match the above, 
            //but I am testing to only pass when we actually want the highlighting
            if (highlight)
            {
                builder.Append(", highlight=" + GetStringFromBool(highlight));
            }

            //builder.Append(", ignoreImplicitH=" + GetStringFromBool(ignoreImplicitHydrogens));
			return builder.ToString();
		}

		private string GetStringFromBool(bool value) {
			if (value)
				return "YES";
			else
				return "NO";
		}
		#endregion


    }
}

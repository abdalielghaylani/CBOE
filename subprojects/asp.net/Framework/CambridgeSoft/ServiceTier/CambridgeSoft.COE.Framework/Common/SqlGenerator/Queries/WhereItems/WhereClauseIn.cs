using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a IN operator in a  SQL where clause. This Class is designed to handle lots of input, that’s why a temporary table strategy is used, if possible (in access this is not possible). Also, an approximated row count (cardinality) is calculated and inserted into the clause, for purposes of DBMS Query optimization.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseIn class requires the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item><b>Field dataField</b> Represents the field of the database that is being compared. Its name and type are required.</item>
	/// <item><b>Value [] Values:</b> represents the array values against wich the dataField is being compared.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	///The SQL generation aims prepared statements rather than simple queries: parameters are not hardcoded into the query text, but marked with a parameter holder character. Parameters are returned in a separate list, ordered by position as they appear in the query.
	///Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
	/// </para>
	/// <para>
	/// <b>Example:</b>
	/// </para>
	/// <b>Programatically:</b>
	/// <code lang="C#">
	///WhereClauseIn target = new WhereClauseIn();
	///	target.DataField = new Field();
	///	target.DataField.FieldName = "MolId";
	///	target.DataField.FieldType = DbType.Int32;
	///	target.Values = new Value[3] { 
	///	new Value("12", DbType.Int32), 
	///	new Value("13", DbType.Int32), 
	///	new Value("24", DbType.Int32) };
	///	Query.AddWhereClauseItem(clause)
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	///&lt;searchCriteriaItem id="1" fieldid="27" modifier="" tableid="6"&gt;
	///	&lt;numericalCriteria operator="IN"&gt;12, 13, 14&lt;/numericalCriteria&gt;
	///&lt;/searchCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// MolId IN(
	///		SELECT /*+ cardinality(t 10)*/ * 
	///		FROM TABLE(CAST(STR2TBL(:1) as COEDB.MYTABLETYPE)) t 
	///		WHERE ROWNUM &gt;= 0)
	/// </para>
	/// <para>
	/// And will add the following parameters to the query parameter list:
	/// </para>
	/// <list type="bullet">
	/// <item>Value("12", DbType.Int32)</item>
	///	 <item>Value("13", DbType.Int32)</item>
	///	 <item>Value("24", DbType.Int32)</item>
	/// </list>
	/// </summary>
    public class WhereClauseIn : WhereClauseNAryOperation
    {
		#region Properties
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

        /// <summary>
        /// Indicates if the search must be case sensitive. True by default.
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }
		#endregion

		#region Variables
		private SearchCriteria.Positions trimPosition;
		private bool normalizeChemicalName;
        private bool caseSensitive;
		#endregion

		#region Constants
		private const string functionString = "fn_ParseText2Table";
		private const string TXT_VALUE = "TXT_VALUE";
		private const string INT_VALUE = "INT_VALUE";
		private const string NUM_VALUE = "NUM_VALUE";
		private int cardinality;
		#endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseIn() : base() {
            this.dataField = new Field();
			this.values = new Value[0];
            this.caseSensitive = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resolves the differences between databases to generate the string of Execute method..
        /// </summary>
		/// <remarks>Missing implementation for Access.</remarks>
        /// <returns>A string of the form: IN('value0', 'value1', ... , 'valueN') for the specific
        /// database.</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> queryValues) {
			cardinality = CalculateCardinality(this.values.Length);
            System.Text.StringBuilder builder = new System.Text.StringBuilder(string.Empty);
			System.Text.StringBuilder valuesBuilder = new System.Text.StringBuilder(string.Empty);
			builder.Append(this.GetLeftOperator(dataBaseType));
			builder.Append(" IN(");
			switch(dataBaseType){
				case DBMSType.SQLSERVER:
					builder.Append("SELECT /*+ cardinality(t ");
					builder.Append(cardinality);
					builder.Append(")*/ ");
					switch(TypesConversor.GetAbstractType(dataField.FieldType)) {
                        case COEDataView.AbstractTypes.Text:
							builder.Append(TXT_VALUE);
							break;
                        case COEDataView.AbstractTypes.Integer:
							builder.Append(INT_VALUE);
							break;
                        case COEDataView.AbstractTypes.Real:
							builder.Append(NUM_VALUE);
							break;
					}
					break;
				case DBMSType.ORACLE:
					builder.Append("SELECT /*+ cardinality(t ");
					builder.Append(cardinality);
					builder.Append(")*/ ");
					builder.Append("*");
					break;
			}
			/*
			 * ORACLE Sintaxis:
			 * FROM TABLE(CAST(STR2TBL(:1) as COEDB.MYTABLETYPE)) t where rownum >= 0
			 * 
			 * SQLServer Sintaxis:
			 * FROM fn_ParseText2Table('?1', ', '))
			 */
			
			switch(dataBaseType) {
					//TODO: Add a better support for MS Access.
				case DBMSType.MSACCESS:
					builder.Append(this.ParameterHolder);
					if(this.UseParametersByName)
						builder.Append(queryValues.Count);
					builder.Append(")");
					break;
				case DBMSType.SQLSERVER:
					builder.Append(" FROM ");
					builder.Append(functionString);
					builder.Append("(");
					builder.Append(this.ParameterHolder);
					if(this.UseParametersByName)
						builder.Append(queryValues.Count);
					builder.Append(")");
					break;
				case DBMSType.ORACLE:
					builder.Append(" FROM ");
                    builder.Append("TABLE(CAST(" + Resources.CentralizedStorageDB + ".COEDBLibrary.ClobToTable(");
					builder.Append(this.ParameterHolder);
					if(this.UseParametersByName)
						builder.Append(queryValues.Count);
                    builder.Append(") as " + Resources.CentralizedStorageDB + ".MYTABLETYPE)) t WHERE ROWNUM >= 0)");
					break;
			}
            for(int i = 0; i < this.values.Length; i++) {
				this.values[i].Val = NormalizationUtils.TrimValue(this.values[i].Val, this.values[i].Type, this.trimPosition);

                //if(TypesConversor.IsText(this.dataField.FieldType))
                //    if(this.NormalizeChemicalName) {
                //        string value = this.values[i].Val;
                //        this.values[i].Val = NormalizationUtils.CleanTheSyns(ref value);
                //    }

				valuesBuilder.Append(this.values[i].Val);

				if(i + 1 < this.values.Length) {
					valuesBuilder.Append(",");
				}
            }
			Value valueToAdd = new Value();
			valueToAdd.Val = valuesBuilder.ToString();
            valueToAdd.Type = System.Data.DbType.AnsiString;
			queryValues.Add(valueToAdd);
            return builder.ToString();
        }

        private string GetLeftOperator(DBMSType dataBaseType)
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            builder.Append(this.GetStartingCaseStatement(dataBaseType));
            builder.Append(this.GetStartingTrimStatement(dataBaseType));
            builder.Append(base.GetFullName(dataField));
            builder.Append(this.GetEndingTrimStatement(dataBaseType));
            builder.Append(this.GetEndingCaseStatement(dataBaseType));
            return builder.ToString();
        }

        private string GetStartingCaseStatement(DBMSType dataBaseType)
        {
            if(!this.caseSensitive)
            {
                switch(dataBaseType)
                {
                    case DBMSType.ORACLE:
                    case DBMSType.SQLSERVER:
                        return "LOWER(";
                    case DBMSType.MSACCESS:
                        return "LCASE(";
                }
            }
            return string.Empty;
        }

        private string GetEndingCaseStatement(DBMSType dataBaseType)
        {
            if(!this.caseSensitive)
                return ")";
            return string.Empty;
        }

        private string GetStartingTrimStatement(DBMSType dataBaseType)
        {
            switch(this.trimPosition)
            {
                case SearchCriteria.Positions.Left:
                    return "LTRIM(";
                case SearchCriteria.Positions.Right:
                    return "RTRIM(";
                case SearchCriteria.Positions.Both:
                    return "TRIM(";
            }
            return string.Empty;
        }

        private string GetEndingTrimStatement(DBMSType dataBaseType)
        {
            if(this.trimPosition != SearchCriteria.Positions.None)
                return ")";
            return string.Empty;
        }

		private int CalculateCardinality(int count) {
			string countString = count.ToString();
			StringBuilder temp = new StringBuilder("1");
			temp.Append(new string('0', countString.Length));
			return int.Parse(temp.ToString());
		}
        #endregion
    }
}
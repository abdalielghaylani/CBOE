using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    /// <summary>
    /// <para>
    /// This class implements an equal comparison operator, "=", in a  SQL where clause.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The WhereClauseEqual class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field dataField:</b> Represents the field of the database that is being compared. Its name and type are required.</item>
    /// <item><b>Value Val:</b> represents the value against wich the dataField is being compared. Its value and type are required. The type of the field can differ from the type of the value. In the special case the value is 'NULL', a 'where field IS NULL' is generated. Similarly, if the value is 'NOT NULL', a 'where field IS NOT NULL' is generated</item>
    /// <item><b>bool CaseSensitive:</b> flag that indicates whether take into account casing or not.</item>
    /// <item><b>Position TrimPosition:</b> flag that indicates how to perform the trim of value. Posible Values: Left, Right, Both, None.</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// The WhereClause decides whether it’s TextCriteria, NumericalCriteria, etc. based upon the Val Value field.
    /// This WhereClause can take into account casing and can perform trim of the input value. 
    /// In the special case of a numericalCriteria that is applied into a text database field, the trim operation trims leading and or trailing zeroes (instead of just spaces).
    /// In the case of a TextCriteria, this implementation compares the ascii codes of the strings.
    /// In the special case the value is 'NULL', a 'where field IS NULL' is generated. Similarly, if the value is 'NOT NULL', a 'where field IS NOT NULL' is generated.
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
    /// WhereClauseEqual clause = new WhereClauseEqual();
    /// clause.TrimPosition = Positions.Both;
    /// clause.DataField = new Field();
    /// clause.DataField.FieldName = "MolWeight";
    /// clause.DataField.FieldType = System.Data.DbType.String;
    /// clause.Val.Type = System.Data.DbType.Decimal;
    /// clause.Val.Val = "	00018.00000	";
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;structureCriteriaItem id=XX fieldid=19 modifier='' tableid=3&gt;
    /// 	&lt;numericalCriteria operator='EQUAL' trim='BOTH'&gt;	00018.00000	
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
    /// [AND] (MolWeight = :0)
    /// </para>
    /// <para>
    /// And will add the following parameters to the query parameter list:
    /// </para>
    /// <list type="bullet">
    /// <item>Value(18, DataFieldtype.Text)</item>
    /// </list>
    /// </summary>
    public class WhereClauseEqual : WhereClauseBinaryOperation {
        #region Properties
        /// <summary>
        /// Determines if the comparation is case sensitive.
        /// <remarks>If the field type is not Text this property is discarded.</remarks>
        /// </summary>
        public bool CaseSensitive {
            get {
                return this.caseSensitive;
            }
            set {
                this.caseSensitive = value;
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
        private bool caseSensitive;
        private SearchCriteria.Positions trimPosition;
        private bool normalizeChemicalName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseEqual()
            : base() {
            this.dataField = new Field();
            this.val = new Value();
            caseSensitive = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Encapsulates different sintaxis for the underlying RDBMS. If the equal will be case insensitive,
        /// then a lower is applied to the field and the value is lowered too.
        /// </summary>
        /// <exception cref="System.Exception">Thrown when the data type is not Text</exception>
        /// <returns>A string for the underlying RDBMS, in the form of: FieldName = FieldValue</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values) {
            StringBuilder builder = new StringBuilder(string.Empty);
            /* 
            * It is necessary to check which is 
            * the DBType and which is the fieldtype
            */
            if(this.val.Val.ToLower().Trim() == "null")
                return string.Format("({0} IS NULL)", base.GetFullName(dataField));

            if(this.val.Val.ToLower().Trim() == "not null")
                return string.Format("({0} IS NOT NULL)", base.GetFullName(dataField));

            this.val.Val = NormalizationUtils.TrimValue(this.val.Val, this.val.Type, this.trimPosition);

            switch (TypesConversor.GetAbstractType(this.val.Type))
            {
                case COEDataView.AbstractTypes.Text:
                    builder.Append("(");
                    if(!caseSensitive)
                        val.Val = val.Val.ToLower();

                    builder.Append(this.GetStartingCaseStatement(dataBaseType));
                    builder.Append(this.GetStartingTrimStatement(dataBaseType));
                    builder.Append(base.GetFullName(dataField));
                    builder.Append(this.GetEndingTrimStatement(dataBaseType));
                    builder.Append(this.GetEndingCaseStatement(dataBaseType));

                    builder.Append(" = ");
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

                    val.Type = dataField.FieldType;
                    values.Add(val);
                    break;
                case COEDataView.AbstractTypes.Real:
                    /*
                     * TODO: Test the values generated when the field is a double, to check that the port from VB is working fine.
                     */
                    double highVal = 0;
                    double lowVal = 0;
                    NormalizationUtils.GetSearchRange(NormalizationUtils.TrimValue(this.val.Val, 
                                                                                   DbType.Decimal,
                                                                                   SearchCriteria.Positions.Both),
                                                      ref lowVal,
                                                      ref highVal);
                    builder.Append("(");
                    builder.Append(base.GetFullName(dataField));
                    builder.Append(" BETWEEN ");
                    builder.Append(this.ParameterHolder);
                    if(this.UseParametersByName)
                        builder.Append(values.Count);
                    values.Add(new Value(lowVal.ToString(), DbType.Decimal));

                    builder.Append(" AND ");
                    builder.Append(this.ParameterHolder);
                    if(this.UseParametersByName)
                        builder.Append(values.Count);
                    values.Add(new Value(highVal.ToString(), DbType.Decimal));
                    builder.Append(")");
                    break;
                case COEDataView.AbstractTypes.Boolean:
                case COEDataView.AbstractTypes.Integer:
                    builder.Append("(");
                    builder.Append(base.GetFullName(dataField));
                    builder.Append(" = ");
                    builder.Append(this.ParameterHolder);
                    if(this.UseParametersByName)
                        builder.Append(values.Count);
                    builder.Append(")");
                    if (string.IsNullOrEmpty(this.AggregateFunctionName))
                        val.Type = dataField.FieldType;
                    else
                        val.Type = DbType.Decimal;
                    values.Add(val);
                    break;
                case COEDataView.AbstractTypes.Date:
                    builder.Append("(");
                    builder.Append(base.GetFullName(dataField));
                    builder.Append(" = ");
                    switch(dataBaseType) {
                        case DBMSType.ORACLE:
                            //TO_DATE('09/22/2006 11:02:26','MM/DD/YYYY HH:MI:SS') (Oracle)
                            builder.Append("TO_DATE(");
                            builder.Append(this.ParameterHolder);
                            if(this.UseParametersByName)
                                builder.Append(values.Count);
                            builder.Append(",'" + Resources.DefaultDateFormat.Replace("HH", "hh24").Replace(":mm:", ":mi:") + "')");
                            //val.Type = dataField.FieldType;
                            values.Add(val);
                            break;
                        case DBMSType.SQLSERVER:
                            //CONVERT(DATETIME, '09/13/2006 11:02:26' , 101)
                            builder.Append("CONVERT(DATETIME,");
                            builder.Append(this.UseParametersByName);
                            if(this.UseParametersByName)
                                builder.Append(values.Count);
                            builder.Append(",101)");

                            val.Type = dataField.FieldType;
                            values.Add(val);
                            break;
                        case DBMSType.MSACCESS:
                            //FORMAT('09/22/2006 11:02:26','MM/DD/YYYY HH:MI:SS')
                            builder.Append("FORMAT(");
                            builder.Append(this.ParameterHolder);
                            if(this.UseParametersByName)
                                builder.Append(values.Count);
                            builder.Append(",'" + Resources.DefaultDateFormat + "')");
                            val.Type = dataField.FieldType;
                            values.Add(val);
                            break;
                    }
                    builder.Append(")");
                    break;
            }
            return builder.ToString();
        }

        private string GetStartingCaseStatement(DBMSType dataBaseType) {
            if(!this.caseSensitive) {
                switch(dataBaseType) {
                    case DBMSType.ORACLE:
                    case DBMSType.SQLSERVER:
                        return "LOWER(";
                    case DBMSType.MSACCESS:
                        return "LCASE(";
                }
            }
            return "";
        }

        private string GetEndingCaseStatement(DBMSType dataBaseType) {
            if(!this.caseSensitive)
                return ")";
            return "";
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

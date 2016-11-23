using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// <para>
    /// This class implements a BETWEEN operator in a  SQL where clause. The WhereClauseBetween is a EQUAL XML operator for a NumericalCriteria that parse into two values when a dash separator is encountered into the criteria. For example is the criteria “10 – 15” it should be interpreted as greater than 10 and less than 15.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The WhereClauseBetween class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field dataField:</b> Represents the field of the database that is being compared. Its name and type are required.</item>
    /// <item><b>Value [] Values:</b> represents a pair of values against wich the dataField is being compared.</item>
    /// <item><b>Position TrimPosition:</b> flag that indicates how to perform the trim of value. Posible Values: Left, Right, Both, None.</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// The spaces in the criteria should be allowed so that “10 -15” and “10 – 15” shouldn’t lead to same result. Other more complex cases that should be supported:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Negative ranges “-10 – -15”:</b> lt negative 10 and gt negative 15.</item>
    /// <item><b>Mixed ranges “10 – -5”:</b> lt 10 and gt negative 5.</item>
    /// <item><b>Spaces before each dash may or may not be present i.e. “-10 – -4":</b> lt negative 4 and gt negative 10.</item>
    /// </list>
    /// <para>
    /// Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// WhereClauseBetween target = new WhereClauseBetween();
    /// target.TrimPosition = Positions.Both;
    ///	target.DataField = new Field();
    ///	target.DataField.FieldName = "MolId";
    ///	target.DataField.FieldType = DbType.Int32;
    ///	target.Values = new Value[2] { 
    ///	new Value("12", DbType.Int32), 
    ///	new Value("14", DbType.Int32) };
    ///	Query.AddWhereClauseItem(target)
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;searchCriteriaItem id="1" fieldid="27" modifier="" tableid="6"&gt;
    ///	 &lt;numericalCriteria operator="EQUAL" trim="BOTH"&gt;12 - 14&lt;/numericalCriteria&gt;
    /// &lt;/searchCriteriaItem&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    ///This code will produce the following where clause statement:
    /// </para>
    /// <para>
    /// [AND] MolId BETWEEN :0 AND :1
    /// </para>
    /// <para>
    /// And will add the following parameters to the query parameter list:
    /// </para>
    /// <list type="bullet">
    /// <item>Value("12", DbType.Int32)</item>
    ///	<item>Value("14", DbType.Int32)</item>
    /// </list>
    /// </summary>
    public class WhereClauseBetween : WhereClauseNAryOperation
    {
        #region Properties
        /// <summary>
        /// Determines if the value must be trimed, and where.
        /// </summary>
        public SearchCriteria.Positions TrimPosition
        {
            get
            {
                return this.trimPosition;
            }
            set
            {
                this.trimPosition = value;
            }
        }
        #endregion

        #region Variables
        private SearchCriteria.Positions trimPosition;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseBetween()
            : base()
        {
            this.dataField = new Field();
            this.values = new Value[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resolves the differences between databases to generate the string of Execute method..
        /// </summary>
        /// <remarks>Missing implementation for Access.</remarks>
        /// <returns>A string of the form: BETWEEN value0 AND value1 for the specific database.</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> queryValues)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder(string.Empty);

            builder.Append(base.GetFullName(dataField));
            builder.Append(" BETWEEN ");
            int i = 1;
            foreach (Value val in this.values)
            {
                val.Val = NormalizationUtils.TrimValue(val.Val, val.Type, this.trimPosition);

                switch (dataBaseType)
                {
                    case DBMSType.SQLSERVER:
                        switch (TypesConversor.GetAbstractType(this.dataField.FieldType))
                        {
                            case COEDataView.AbstractTypes.Real:
                            case COEDataView.AbstractTypes.Integer:
                                builder.Append(this.UseParametersByName);
                                if (this.UseParametersByName)
                                    builder.Append(queryValues.Count);
                                queryValues.Add(new Value(val.Val.ToString(), dataField.FieldType));
                                break;
                            case COEDataView.AbstractTypes.Date:
                                //CONVERT(DATETIME, '09/13/2006 11:02:26' , 101)
                                builder.Append("CONVERT(DATETIME,");
                                builder.Append(this.UseParametersByName);
                                if (this.UseParametersByName)
                                    builder.Append(queryValues.Count);
                                builder.Append(",101)");
                                queryValues.Add(new Value(val.Val.ToString(), val.Type));
                                break;
                        }
                        break;
                    case DBMSType.ORACLE:
                        switch (TypesConversor.GetAbstractType(this.dataField.FieldType))
                        {
                            case COEDataView.AbstractTypes.Real:
                            case COEDataView.AbstractTypes.Integer:
                                builder.Append(this.ParameterHolder);
                                if (this.UseParametersByName)
                                    builder.Append(queryValues.Count);
                                queryValues.Add(new Value(val.Val.ToString(), dataField.FieldType));
                                break;
                            case COEDataView.AbstractTypes.Date:
                                //TO_DATE('09/22/2006 11:02:26','MM/DD/YYYY HH:MI:SS') (Oracle)
                                builder.Append("TO_DATE(");
                                builder.Append(this.ParameterHolder);
                                if (this.UseParametersByName)
                                    builder.Append(queryValues.Count);
                                builder.Append(",'" + Resources.DefaultDateFormat.Replace("HH", "hh24").Replace(":mm:", ":mi:") + "')");
                                queryValues.Add(new Value(val.Val.ToString(), val.Type));
                                break;
                        }
                        break;
                }

                if (i < this.values.Length)
                {
                    builder.Append(" AND ");
                    i++;
                }
            }
            return builder.ToString();
        }
        #endregion
    }
}

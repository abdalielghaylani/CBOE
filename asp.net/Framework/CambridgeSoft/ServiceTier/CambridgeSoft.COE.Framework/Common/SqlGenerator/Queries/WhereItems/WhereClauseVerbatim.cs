using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    /// <summary>
    /// <para>
    /// This class permits the user to insert its SQL clause textually, while sticking to the prepared statement overall design.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The WhereClauseIn class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>string Verbatim:</b> Represents Literal (or Verbatim) SQL text that the user wants to add to the query. Parameters must be incated by using the parameter marker</item>
    /// <item><b>List&lt;Value&gt; Parameters:</b>: represents the array values that will populate the query when executing, at a later stage (and a different subsystem).</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// The Field and the Values array are ignored in this implementation.
    /// This class is not guaranteed to be present in future implementations, as it poses high security problems (SQL Injection, etc.)
    /// The SQL generation aims prepared statements rather than simple queries: parameters are not hardcoded into the query text, but marked with a parameter holder character. Parameters are returned in a separate list, ordered by position as they appear in the query.
    /// Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// WhereClauseVerbatim target = new WhereClauseVerbatim();
    /// target.Verbatim = "myTable.MyField = ? AND myTable.MyOtherField = ?";
    /// target.Parameters = new List&lt;Value&gt;()
    /// target.Parameters.Add(new Value("12", System.Data.DbType.Int32));
    /// target.Parameters.Add(new Value("lelele", System.Data.DbType.String));
    /// Query.AddWhereClauseItem(clause)
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;searchCriteriaItem id="1" fieldid="1" tableid="1" modifier=""&gt;
    /// &lt;verbatimCriteria&gt;
    /// &lt;verbatim&gt;myTable.MyField = ? AND myTable.MyOtherField = ?&lt;/verbatim&gt;
    /// &lt;parameter&gt;12&lt;/parameter&gt;
    /// &lt;parameter&gt;lelele&lt;/parameter&gt;
    /// &lt;/verbatimCriteria&gt;
    /// &lt;/searchCriteriaItem&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following where clause statement:
    /// </para>
    /// <para>
    /// myTable.MyField = :0 AND myTable.MyOtherField = :1
    /// </para>
    /// <para>
    /// And will add the following parameters to the query parameter list:
    /// </para>
    /// <list type="bullet">
    /// <item>Value("12", DbType.Int32)</item>
    /// <item>Value("lelele", System.Data.DbType.String)</item>
    /// </list>
    /// </summary>
    public class WhereClauseVerbatim : WhereClauseNAryOperation {
        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public WhereClauseVerbatim()
            : base() {
            this.verbatim = string.Empty;
            this.parameters = new List<Value>();
        }
        #endregion

        #region Variables
        private string verbatim = "";
        private List<Value> parameters;
        private string parameterHolderCharacter = "?";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the verbatim value.
        /// </summary>
        public string Verbatim {
            get {
                return this.verbatim;
            }
            set {
                this.verbatim = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of parameters.
        /// </summary>
        public List<Value> Parameters {
            get {
                return this.parameters;
            }
            set {
                this.parameters = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the string representation of the verbatim clause for the given database, and adds the param values 
        /// to the values list.
        /// </summary>
        /// <param name="databaseType">The database to get the string for.</param>
        /// <param name="values">Already existent values of a query.</param>
        /// <returns>The string representation of the clause.</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values) {
            string replacedVerbatim = this.Verbatim;

            if(SqlInjection.FindSqlInjection(replacedVerbatim))
                throw new SQLGeneratorException(string.Format("{0} - Attempt SQL Injection: {1}", "VerbatimCriteria", replacedVerbatim));

            int position = 0;

            foreach(Value currentValue in this.Parameters) {
                if(SqlInjection.FindSqlInjection(currentValue.Val))
                    throw new SQLGeneratorException(string.Format("{0} - Attempt SQL Injection: {1}", "VerbatimCriteria", currentValue.Val));

                if((position = this.ReplaceNextParameterHolder(ref replacedVerbatim, position, values.Count, databaseType)) == -1)
                    throw new SQLGeneratorException(Resources.CannotReplaceParamInLiteral.Replace("&currentVal", currentValue.Val));
                values.Add(currentValue);
            }
                        
            return replacedVerbatim;
        }

        private int ReplaceNextParameterHolder(ref string verbatim, int currentPosition, int nextParam, DBMSType databaseType) {
            int nextHolderPosition = verbatim.IndexOf(this.parameterHolderCharacter, currentPosition);

            if(nextHolderPosition >= 0) {
                string originalVerbatim = verbatim;

                verbatim = originalVerbatim.Substring(0, nextHolderPosition);
                verbatim = verbatim + this.ParameterHolder;
                if(this.UseParametersByName) {
                    verbatim += nextParam.ToString();
                    verbatim += (nextHolderPosition + 1 < originalVerbatim.Length ?
                                            originalVerbatim.Substring(nextHolderPosition + 1, originalVerbatim.Length - nextHolderPosition - 1) :
                                            string.Empty);
                } else
                    verbatim += (nextHolderPosition < originalVerbatim.Length ?
                                            originalVerbatim.Substring(nextHolderPosition, originalVerbatim.Length - nextHolderPosition - 1) :
                                            string.Empty);
            }
            return nextHolderPosition;
        }
        #endregion
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;
using System.Text.RegularExpressions;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// <para>
    /// This class implements a LIKE comparison operator in a  SQL where clause.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The WhereClauseLike class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field dataField:</b> Represents the field of the database that is being compared. Its name and type are required.</item>
    /// <item><b>Value Val:</b> represents the value against wich the dataField is being compared. Its value and type are required. The type of the field can differ from the type of the value.</item>
    /// <item><b>bool CaseSensitive:</b> flag that indicates whether take into account casing or not.</item>
    /// <item><b>Position TrimPosition:</b> flag that indicates how to perform the trim of value. Posible Values: Left, Right, Both, None.</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// Like operator is only supported on TextCriterias.
    /// This Where Clause can take into account casing and can perform trim of the input value. 
    /// This implementation compares the ascii codes of the strings.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// WhereClauseLike target = new WhereClauseLike();
    /// target.DataField = new Field();
    /// target.DataField.FieldName = "Structure";
    /// target.DataField.FieldType = DbType.String;
    /// target.Val = new Value("	C1CCCCC1  ", DbType.String);
    /// target.CaseSensitive = false;
    /// target.TrimPosition = Positions.Both;
    /// Query.AddWhereClauseItem(clause);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;structureCriteriaItem id=XX fieldid=20 modifier='' tableid=3&gt;
    /// 	&lt;textCriteria operator='LIKE' trim='BOTH' caseSensitive='NO'&gt;	C1CCCCC1  
    /// 	&lt;/textCriteria&gt;
    /// &lt;/structureCriteriaItem&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following where clause statement:
    /// </para>
    /// <para>
    /// [AND] (LOWER(Structure) LIKE :0);
    /// </para>
    /// <para>
    /// And will add the following parameters to the query parameter list:
    /// </para>
    /// <list type="bullet">
    /// <item>Value(c1ccccc1, DataFieldtype.Text</item>
    /// </list>
    /// </summary>
    public class WhereClauseLike : WhereClauseBinaryOperation
    {
        #region Properties
        /// <summary>
        /// Where will be applied the wildcard. For instance if left is selected the generated like will
        /// be in the form: FieldName Like('%FieldValue')
        /// </summary>
        public virtual SearchCriteria.Positions WildCardPosition
        {
            get
            {
                return this.wildCardPosition;
            }
            set
            {
                this.wildCardPosition = value;
            }
        }

        /// <summary>
        /// Determines if the comparation is case sensitive.
        /// <remarks>If the field type is not Text this property is discarded.</remarks>
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return this.caseSensitive;
            }
            set
            {
                this.caseSensitive = value;
            }
        }

        /// <summary>
        /// Determines if the value must be triimed, and where.
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

        /// <summary>
        /// Determines if the like must be a Full word search.
        /// </summary>
        public virtual bool FullWordSearch
        {
            get
            {
                return this.fullWordSearch;
            }
            set
            {
                this.fullWordSearch = value;
            }
        }

        /// <summary>
        /// Gets or sets if it is necessary to normalize the name.
        /// </summary>
        public virtual bool NormalizeChemicalName
        {
            get { return normalizeChemicalName; }
            set { normalizeChemicalName = value; }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Where will be applied the wildcard. For instance if left is selected the generated like will
        /// be in the form: FieldName Like('%FieldValue')
        /// </summary>
        private SearchCriteria.Positions wildCardPosition;
        private bool caseSensitive;
        private SearchCriteria.Positions trimPosition;
        private bool fullWordSearch;
        private bool normalizeChemicalName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseLike()
            : base()
        {
            this.dataField = new Field();
            this.val = new Value();
            wildCardPosition = SearchCriteria.Positions.Both;
            trimPosition = SearchCriteria.Positions.Both;
            caseSensitive = true;
            this.fullWordSearch = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Encapsulates different sintaxis for the underlying RDBMS. If the like operation will be case insensitive,
        /// then a lower is applied to the field and the value is lowered too.
        /// </summary>
        /// <exception cref="System.Exception">Thrown when the data type is not Text</exception>
        /// <returns>A string for the underlying RDBMS, in the form of: FieldName Like('%FieldValue%')</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            //SetWildCardPositionsFromValue();
            //TrimValues();

            this.Val.Val = NormalizationUtils.TrimValue(this.Val.Val, this.Val.Type, this.TrimPosition);
            bool useEscapeCharForPercent = this.val.Val.Contains("¬");
            if (!caseSensitive)
                val.Val = val.Val.ToLower();

            if (fullWordSearch)
            {
                builder.Append("(");
                string leftOperator = GetLeftOperator(dataBaseType);

                builder.Append(leftOperator + " LIKE '% ' || ");
                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if(this.UseParametersByName)
                    builder.Append(values.Count);

                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));

                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }

                if(this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Right ||
                   this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Both)
                    builder.Append(" || '%'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(" OR " + leftOperator + " LIKE ");

                if(this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Left ||
                   this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Both)
                    builder.Append(" '%' || ");

                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if (this.UseParametersByName)
                    builder.Append(values.Count);

                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));

                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }

                builder.Append(" || ' %'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(" OR " + leftOperator + " LIKE ");

                
                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if (this.UseParametersByName)
                    builder.Append(values.Count);
                
                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));
                
                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }

                builder.Append(" || '.%'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(" OR " + leftOperator + " LIKE '% ' || ");

                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if (this.UseParametersByName)
                    builder.Append(values.Count);
                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));

                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }

                builder.Append(" || ' %'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(" OR " + leftOperator + " LIKE '% ' || ");

                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if (this.UseParametersByName)
                    builder.Append(values.Count);
                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));

                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }
                builder.Append(" || '.%'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(" OR " + leftOperator + " LIKE ");


                if(this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Left ||
                    this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Both)
                    builder.Append(" '%' || ");

                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }

                builder.Append(this.ParameterHolder);
                if (this.UseParametersByName)
                    builder.Append(values.Count);

                values.Add(new Value(this.GetWildCardLessString(val.Val), val.Type));

                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }

                if(this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Right ||
                    this.GetWildCardPositionsFromValue(this.val.Val) == SearchCriteria.Positions.Both)
                    builder.Append(" || '%'");

                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));

                builder.Append(")");
            }
            else
            {
                builder.Append("(");
                builder.Append(GetLeftOperator(dataBaseType));
                builder.Append(" LIKE ");

                switch(this.GetWildCardPositionsFromValue(GetWildCardDependantString(val.Val))) {
                    case SearchCriteria.Positions.Left:
                    case SearchCriteria.Positions.Both:
                        builder.Append("'%' || ");
                        break;
                }
                
                if(this.NormalizeChemicalName) {
                    builder.Append(Resources.CentralizedStorageDB + ".Normalize(");
                }
                
                builder.Append(this.ParameterHolder);
                if(this.UseParametersByName)
                    builder.Append(values.Count);
                if(this.NormalizeChemicalName) {
                    builder.Append(")");
                }
                
                switch(this.GetWildCardPositionsFromValue(GetWildCardDependantString(val.Val))) {
                    case SearchCriteria.Positions.Right:
                    case SearchCriteria.Positions.Both:
                        builder.Append(" || '%'");
                        break;
                }

                values.Add(new Value(GetWildCardLessString(val.Val), val.Type));
                if(useEscapeCharForPercent)
                    builder.Append(GetEscapeStatement(dataBaseType));
                
                builder.Append(")");
            }
            
            return builder.ToString();
        }

        private string GetEscapeStatement(DBMSType dataBaseType) {
            switch(dataBaseType) {
                case DBMSType.ORACLE:
                    return " ESCAPE '¬'";
                case DBMSType.MSACCESS:
                case DBMSType.SQLSERVER:
                default:
                    return string.Empty;
            }
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
            if (!this.caseSensitive)
            {
                switch (dataBaseType)
                {
                    case DBMSType.ORACLE:
                    case DBMSType.SQLSERVER:
                        return "LOWER(";
                    case DBMSType.MSACCESS:
                        return "LCASE(";
                }
            }
            return "";
        }

        private string GetEndingCaseStatement(DBMSType dataBaseType)
        {
            if (!this.caseSensitive)
                return ")";
            return "";
        }

        private string GetStartingTrimStatement(DBMSType dataBaseType)
        {
            switch (this.trimPosition)
            {
                case SearchCriteria.Positions.Left:
                    return "LTRIM(";
                case SearchCriteria.Positions.Right:
                    return "RTRIM(";
                case SearchCriteria.Positions.Both:
                    return "TRIM(";
            }
            return "";
        }

        private string GetEndingTrimStatement(DBMSType dataBaseType)
        {
            if (this.trimPosition != SearchCriteria.Positions.None)
                return ")";
            return "";
        }

        private SearchCriteria.Positions GetWildCardPositionsFromValue(string value)
        {
            if (value.StartsWith("*") || value.StartsWith("%"))
            {
                if (value.EndsWith("*") || value.EndsWith("%"))
                {
                    return SearchCriteria.Positions.Both;
                }
                else
                {
                    return SearchCriteria.Positions.Left;
                }
            }
            else if (value.EndsWith("*") || value.EndsWith("%"))
            {
                return SearchCriteria.Positions.Right;
            }
            else
            {
                return SearchCriteria.Positions.None;
            }
        }

        private string GetWildCardLessString(string value)
        {
            string lessString = value;
            Regex multiWildCard = new Regex(@"(?<!(\\))");
            lessString = multiWildCard.Replace(lessString, "");
            if(lessString.StartsWith("*"))
                lessString = lessString.TrimStart('*');
            if(lessString.EndsWith("*") && !lessString.EndsWith("\\*"))
                lessString = lessString.TrimEnd('*');
            lessString = lessString.Replace(@"\*", "*");
            lessString = GetSingleWildCardDependantString(lessString);
            return lessString;
        }

        private string GetWildCardDependantString(string value)
        {
            string dependantString = value;
            dependantString = GetMultiWildCardDependantString(dependantString);
            dependantString = GetSingleWildCardDependantString(dependantString);

            if(!dependantString.Contains("%")) {
                switch(WildCardPosition) {
                    case SearchCriteria.Positions.Left:
                        dependantString = dependantString.Insert(0, "%");
                        break;
                    case SearchCriteria.Positions.Right:
                        dependantString += "%";
                        break;
                    case SearchCriteria.Positions.Both:
                        dependantString = dependantString.Insert(0, "%");
                        dependantString += "%";
                        break;
                }
            }

            return dependantString;
        }

        private string GetMultiWildCardDependantString(string value)
        {
            string multiWildCardString = value;
            if (multiWildCardString.EndsWith("*") && !multiWildCardString.EndsWith("\\*"))
                multiWildCardString = multiWildCardString.TrimEnd('*') + "%";
            if (multiWildCardString.StartsWith("*"))
                multiWildCardString = "%" + multiWildCardString.TrimStart('*');
            multiWildCardString = value.Replace(@"\*", "*");
            return multiWildCardString;
        }

        private string GetSingleWildCardDependantString(string value)
        {
            Regex singleWildCard = new Regex(@"(?<!(\\))\?");
            return singleWildCard.Replace(value, "_").Replace(@"\?", "?");
        }
        #endregion
    }
}

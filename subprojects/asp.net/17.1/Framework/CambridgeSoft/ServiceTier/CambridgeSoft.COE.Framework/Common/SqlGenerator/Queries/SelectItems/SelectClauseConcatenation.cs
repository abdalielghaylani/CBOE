using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class allows the user to concatenate several SelectClauseItems on a single column.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseConcatenation class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>List&lt;SelectClauseItem&gt; clauses:</b> The SelectClauseItems to be concatenated.</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// This class will use the “+” or “&amp;” as the concatenation operator depending on the underlying dbms.
    /// SelectClauses are implemented in two differentiable parts: The SelectClause itself, in charge of generating the string and a Parser whose responsibility is to extract the information needed for building the clause from an xmlnode.
    /// All SelectClause are mapped in mappings.xml, wich indicates given a SelectClause name, wich class to use for parsing and obtaining the generated string. There is a set of predefined SelectClauseFields, but the user can expand with his own. By implementing SelectClauseItem and ISelectClauseParser and adding the corresponding entry in this file.
    /// Along with SelectClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType) method as the “Execute Method”
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseField item = new SelectClauseField();
    /// item.DataField.Table = emp;
    /// item.DataField.FieldName = "name";
    /// SelectClauseAvg itemAvg = new SelectClauseAvg();
    /// itemAvg.DataField.Table = emp;
    /// itemAvg.DataField.FieldName = "id";
    /// SelectClauseLiteral itemLiteral = new SelectClauseLiteral();
    /// itemLiteral.Literal = "Literal 123";
    /// SelectClauseConcatenation temConcatenation = new SelectClauseConcatenation();
    /// itemConcatenation.Clauses.Add(itemLiteral);
    /// itemConcatenation.Clauses.Add(item);
    /// itemConcatenation.Alias = "FullName";
    /// query.AddSelectItem(itemConcatenation);query.AddSelectItem(literal);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;concatenation&gt;
    ///		&lt;avg id="1"/&gt;
    ///		&lt;literal&gt;Literal 123&lt;/literal&gt;
    /// &lt;/concatenation>
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// <b>In Oracle:</b>
    /// (AVG(e.id) || 'Literal 123') AS FullName
    /// </para>
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (AVG(e.id) + 'Literal 123') AS FullName
    /// </para>
    /// <para>
    /// <b>To Be Done:</b> Review the SelectClauses Aliases (In this case there is no place to put it on the xml).
    /// </para>
    /// </summary>
    public class SelectClauseConcatenation : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// The Select Clauses to be concatenated.
        /// </summary>
        public List<SelectClauseItem> Clauses
        {
            get
            {
                return this.clauses;
            }
            set
            {
                this.clauses = value;
            }
        }

        public override string Name
        {
            get { return this.Alias; }
        }
        #endregion

        #region Variables
        /// <summary>
        /// The Select Clauses to be concatenated.
        /// </summary>
        protected List<SelectClauseItem> clauses;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SelectClauseConcatenation()
        {
            this.clauses = new List<SelectClauseItem>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the string representation of the Concatenation, to be inserted into a select clause.
        /// </summary>
        /// <param name="dataBaseType">The target database for wich the concatenation will be generated</param>
        /// <returns>a string containing the concatenation</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder result = new StringBuilder();

            string concatenationOperator;
            switch (dataBaseType)
            {
                case DBMSType.ORACLE:
                    concatenationOperator = " || ";
                    break;
                default:
                    concatenationOperator = " + ";
                    break;
            }

            result.Append("(");
            for (int currentIndex = 0; currentIndex < this.clauses.Count; currentIndex++)
            {
                result.Append(this.clauses[currentIndex].Execute(dataBaseType, values));

                if (currentIndex != this.clauses.Count - 1)
                    result.Append(concatenationOperator);
            }

            result.Append(")");

            return result.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Creates a new Instance based upon a xml represntation of the clause. 
        /// </summary>
        /// <param name="resultsXmlNode">The xml snippet containing the xml representation of the clause</param>
        /// <param name="dvnLookup">DataView Naming Lookup object used for translating identifiers to objects</param>
        /// <returns></returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, INamesLookup dvnLookup)
        {
            SelectClauseConcatenation item = new SelectClauseConcatenation();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            foreach (XmlNode currentChild in resultsXmlNode.ChildNodes)
            {
                if (currentChild.NodeType != XmlNodeType.Comment)
                {
                    SelectClauseItem childItem = SelectClauseFactory.CreateSelectClauseItem(currentChild, dvnLookup);
                    item.clauses.Add(childItem);
                }
            }

            return item;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class allows the user to retrieve an Analytical RowNum over several SelectClauseItems on a single column.
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
    /// SelectClauses are implemented in two differentiable parts: The SelectClause itself, in charge of generating the string and a Parser whose responsibility is to extract the information needed for building the clause from an xmlnode.
    /// All SelectClause are mapped in mappings.xml, wich indicates given a SelectClause name, wich class to use for parsing and obtaining the generated string. There is a set of predefined SelectClauseFields, but the user can expand with his own. By implementing SelectClauseItem and ISelectClauseParser and adding the corresponding entry in this file.
    /// Along with SelectClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType) method as the “Execute Method”
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table compounds = new Table();
    /// compounds.TableName = "compounds";
    /// SelectClauseField item1 = new SelectClauseField();
    /// item1.DataField.Table = compounds;
    /// item1.DataField.FieldName = "compound_id";
    /// SelectClauseAvg item2 = new SelectClauseField();
    /// item2.DataField.Table = compounds;
    /// item2.DataField.FieldName = "substance_name";
    /// SelectClauseAnalyticalRowNum itemRowNum = new SelectClauseAnalyticalRowNum();
    /// itemRowNum.Clauses.Add(item1);
    /// itemRowNum.Clauses.Add(item2);
    /// itemRowNum.Alias = "SortOrder";
    /// query.AddSelectItem(itemRowNum);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;analyticalRowNum alias="SortOrder"&gt;
    ///		&lt;field id="1"/&gt;
    ///		&lt;field id="2"/&gt;
    /// &lt;/analyticalRowNum>
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// <b>In Oracle:</b>
    /// ROW_NUM() over(order by compound_id, substance_name) as SortOrder
    /// </para>
    /// </summary>
    /// <remarks><para>This class works only in Oracle DBMS</para></remarks>
    public class SelectClauseAnalyticalRowNum : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// The "order by" clauses to be ordered previous to generate the RowNum.
        /// </summary>
        public List<OrderByClauseItem> Clauses
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

        public override IColumn DataField
        {
            get
            {
                if (this.clauses != null && this.clauses.Count > 0)
                    return this.clauses[0].Item.DataField;
                else
                    return base.DataField;
            }
            set
            {
                base.DataField = value;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// The "order by" clauses to be ordered previous to generate the RowNum.
        /// </summary>
        protected List<OrderByClauseItem> clauses;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SelectClauseAnalyticalRowNum()
        {
            this.clauses = new List<OrderByClauseItem>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the string representation of the Analytical row num, to be inserted into a select clause.
        /// </summary>
        /// <param name="dataBaseType">The target database for which the RowNum will be generated</param>
        /// <returns>A string containing the RowNum</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder result = new StringBuilder();

            if (dataBaseType != DBMSType.ORACLE)
                throw new SQLGeneratorException("The RowNum analytical select clause can only be used on Oracle DBMSType");

            // ROW_NUM() over(order by compound_id, substance_name) as SortOrder

            if (clauses.Count > 0)
            {
                result.Append(@"ROW_NUMBER() over(order by ");
                for (int i = 0; i < clauses.Count; i++)
                {
                    result.Append(clauses[i].Item.Execute(dataBaseType, values));
                    result.Append(" ");
                    result.Append(clauses[i].Direction);
                    if (i != clauses.Count - 1)
                        result.Append(",");
                }
                result.Append(@")");
            }

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
            SelectClauseAnalyticalRowNum item = new SelectClauseAnalyticalRowNum();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            foreach (XmlNode currentChild in resultsXmlNode.ChildNodes)
            {
                if (currentChild.NodeType != XmlNodeType.Comment)
                {
                    SelectClauseItem childItem = SelectClauseFactory.CreateSelectClauseItem(currentChild, dvnLookup);
                    OrderByClauseItem orderByItem = new OrderByClauseItem(childItem);
                    if (resultsXmlNode.Attributes["SortDirection"] != null && resultsXmlNode.Attributes["SortDirection"].Value.ToLower() == "desc")
                        orderByItem.Direction = ResultsCriteria.SortDirection.DESC;
                    else
                        orderByItem.Direction = ResultsCriteria.SortDirection.ASC;

                    item.clauses.Add(orderByItem);
                }
            }

            return item;
        }

        #endregion
    }
}

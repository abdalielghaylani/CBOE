using System;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class represents a literal text the user wants to add to the select clause.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseLiteral class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>string  Literal:</b> The text that is going to be added to the columns of the query.</item>
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
    /// SelectClauseLiteral literal = new SelectClauseLiteral();
    /// item.Literal = "literal";
    /// item.Alias = "Information";
    /// query.AddSelectItem(literal);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;literal alias="Information"&gt;literal&lt;/literal&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// 'literal' Information
    /// </para>
    /// </summary>
    public class SelectClauseLiteral : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// String containing the literal to be included in the SELECT clause.
        /// </summary>
        public string Literal
        {
            get
            {
                return this.literal;
            }
            set
            {
                this.literal = value;
            }
        }
        public override string Name
        {
            get { return this.Alias; }
        }
        public List<Value> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new List<Value>();

                return _parameters;
            }
            set { _parameters = value; }
        }

        #endregion

        #region Variables
        /// <summary>
        /// String containing the literal to be included in the SELECT clause.
        /// </summary>
        protected string literal;
        private List<Value> _parameters;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseLiteral()
        {
            this.literal = string.Empty;
        }
        public SelectClauseLiteral(string literal)
        {
            this.literal = literal;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case just return the desired string as literal.
        /// </summary>
        /// <returns>A string containing the select part corresponding to this field (i.e. the field name)</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            foreach (Value currentValue in Parameters)
                values.Add(currentValue);

            return literal;
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseLiteral, which 
        /// is initialized with the inner text of the literal node.
        /// </summary>
        /// <param name="literalNode">The literal node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseLiteral.</returns>
        public SelectClauseItem CreateInstance(XmlNode literalNode, INamesLookup dvnLookup)
        {
            SelectClauseLiteral item = new SelectClauseLiteral();
            if (literalNode.Attributes["alias"] != null)
                item.Alias = literalNode.Attributes["alias"].Value.ToString();

            if (literalNode.Attributes["visible"] != null && literalNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(literalNode.Attributes["visible"].Value);

            item.literal = literalNode.InnerText.Trim();
            return item;
        }
        #endregion

    }
}

using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class allows the user to perform a Switch (CASE or DECODE) SQL Statement.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseCase class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>List&lt;SelectClauseItem&gt; Clauses:</b> The SelectClauseItems to be concatenated.</item>
    /// <item><b>SelectClauseItem Clause:</b> The SelectClauseItem used for the swich condition.</item>
    /// <item><b>SortedDictionary&lt;string, SelectClauseItem&gt; Cases:</b> A Dictionary of switch possible cases, where the string key is to be used as the case and the SelectClauseItem is the result.</item>
    /// <item><b>SelectClauseItem Default:</b> The default result.</item>
    /// </list>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// This class doesn’t support Access yet, and will throw an exception if used on that “dbms”.
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
    /// Field nameField = new Field();
    /// nameField.Table = emp;
    /// nameField.FieldName = "NAME";
    /// Field lastNameField = new Field();
    /// lastNameField.Table = emp;
    /// lastNameField.FieldName = "LASTNAME";
    /// SelectClauseField clause = new SelectClauseField();
    /// clause.DataField = nameField;
    /// SelectClauseLiteral caseLiteral = new SelectClauseLiteral();
    /// caseLiteral.Literal = "'Literal Name'";
    /// SelectClauseField caseField = new SelectClauseField();
    /// caseField.DataField = lastNameField;
    /// SelectClauseCase target = new SelectClauseCase();
    /// target.Clause = clause;
    /// target.Cases.Add("'JOHN'", caseLiteral);
    /// target.Cases.Add("'SMITH'", caseField);
    /// target.Default = caseField;			
    /// query.AddSelectItem(itemConcatenation);query.AddSelectItem(literal);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;switch inputType="text"&gt;
    ///		&lt;clause&gt;
    ///			&lt;field id ="2"/&gt;
    ///		&lt;/clause&gt;
    ///		&lt;condition value="123"&gt;
    ///			&lt;field id ="2"/&gt;
    ///		&lt;/condition&gt;
    ///		&lt;condition value="aBb"&gt;
    ///			&lt;literal&gt;12323&lt;/literal&gt;
    ///		&lt;/condition&gt;
    ///		&lt;condition value="C"&gt;
    ///			&lt;literal&gt;'12323'&lt;/literal&gt;
    ///		&lt;/condition&gt;
    ///		&lt;condition default=""&gt;
    ///			&lt;field id="1"/&gt;
    ///		&lt;/condition&gt;
    /// &lt;/switch&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statements:
    /// </para>
    /// <para>
    /// <b>In Oracle:</b>
    /// DECODE (e.NAME, 'JOHN', Literal Name, 'SMITH', e.LASTNAME, e.LASTNAME)
    /// </para>
    /// <para>
    /// <b>In MS-SQLSERVER:</b>
    /// case e.NAME
    ///		WHEN 'JOHN' THEN 'Literal Name' 
    ///		WHEN 'SMITH' THEN e.LASTNAME 
    ///		ELSE e.LASTNAME 
    /// END
    /// </para>
    /// <para>
    /// <b>To Be Done:</b> Review the SelectClauses Aliases (In this case there is no place to put it on the xml). And Implement this in Ms-Asccess.
    /// </para>
    /// </summary>
    public class SelectClauseCase : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// Clause describing the column on wich the switch-case is being performed
        /// </summary>
        public SelectClauseItem Clause
        {
            get
            {
                return this.clause;
            }
            set
            {
                this.clause = value;
            }
        }

        /// <summary>
        /// Dictionary containing all the posible cases of the switch-case statement.
        /// </summary>
        public SortedDictionary<string, SelectClauseItem> Cases
        {
            get
            {
                return this.cases;
            }
            set
            {
                this.cases = value;
            }
        }

        /// <summary>
        /// Default response of the switch-case statement, if none of the "cases" is fullfilled.
        /// </summary>
        public SelectClauseItem Default
        {
            get
            {
                return this.defaultItem;
            }
            set
            {
                this.defaultItem = value;
            }
        }

        /// <summary>
        /// Type of the column on wich the case is being performed. TODO: Review this method of handling the type (we preferred this instead of adding a type to selectClauses)
        /// </summary>
        public DbType InputType
        {
            get
            {
                return inputType;
            }
            set
            {
                inputType = value;
            }
        }

        public override string Name
        {
            get
            {
                if (this.Alias != null && this.Alias.Trim() != string.Empty)
                    return this.Alias;

                return this.clause.Name;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Field describing the column on wich the switch-case is being performed
        /// </summary>
        protected SelectClauseItem clause;

        /// <summary>
        /// Dictionary containing all the posible cases of the switch-case statement.
        /// </summary>
        protected SortedDictionary<string, SelectClauseItem> cases;

        /// <summary>
        /// Type of the column on wich the case is being performed. TODO: Review this method of handling the type (we preferred this instead of adding a type to selectClauses)
        /// </summary>
        protected DbType inputType;

        /// <summary>
        /// Default response of the switch-case statement, if none of the "cases" is fullfilled.
        /// </summary>
        protected SelectClauseItem defaultItem;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize its values to its default values.
        /// </summary>
        public SelectClauseCase()
        {
            this.clause = null;
            //this.column.FieldName = "*";
            this.cases = new SortedDictionary<string, SelectClauseItem>();
            this.defaultItem = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Write something here.
        /// </summary>
        /// <param name="dataBaseType">Write something here.</param>
        /// <returns>Write something here.</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            string quotes = "";

            switch (this.inputType)
            {
                case DbType.String:
                    quotes = "'";
                    break;
            }

            switch (dataBaseType)
            {
                case DBMSType.SQLSERVER:
                    if (this.cases.Count > 0)
                    {
                        builder.Append("case ");
                        builder.Append(this.clause.Execute(dataBaseType, values));
                        foreach (KeyValuePair<string, SelectClauseItem> currentItem in this.cases)
                        {
                            builder.Append(" WHEN ");
                            builder.Append(quotes);
                            builder.Append(currentItem.Key);
                            builder.Append(quotes);
                            builder.Append(" THEN ");
                            builder.Append(currentItem.Value.Execute(dataBaseType, values));
                        }
                        if (this.defaultItem != null)
                        {
                            builder.Append(" ELSE ");
                            builder.Append(defaultItem.Execute(dataBaseType, values));
                        }
                        builder.Append(" END");
                    }
                    break;
                case DBMSType.ORACLE:
                    if (this.cases.Count > 0)
                    {
                        builder.Append("DECODE (");
                        builder.Append(this.clause.Execute(dataBaseType, values));
                        foreach (KeyValuePair<string, SelectClauseItem> currentItem in this.cases)
                        {
                            builder.Append(", ");
                            builder.Append(quotes);
                            builder.Append(currentItem.Key);
                            builder.Append(quotes);
                            builder.Append(", ");
                            builder.Append(currentItem.Value.Execute(dataBaseType, values));
                        }
                        if (this.defaultItem != null)
                        {
                            builder.Append(", ");
                            builder.Append(defaultItem.Execute(dataBaseType, values));
                        }
                        builder.Append(")");
                    }
                    break;
                case DBMSType.MSACCESS:
                    throw new Exception("SelectClauseCase is not yet implemented for MS-Access");
            }
            return builder.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Write something here.
        /// </summary>
        /// <param name="resultsXmlNode">Write something here.</param>
        /// <param name="dvnLookup">Write something here.</param>
        /// <returns>Write something here.</returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, INamesLookup dvnLookup)
        {
            SelectClauseCase item = new SelectClauseCase();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            if (resultsXmlNode.Attributes["inputType"] != null && !string.IsNullOrEmpty(resultsXmlNode.Attributes["inputType"].Value))
                item.inputType = Utils.TypesConversor.GetType(resultsXmlNode.Attributes["inputType"].Value.Trim());
            else
                throw new Exception(string.Format("Required attribute {0} is missing: {1}", "inputType", resultsXmlNode.OuterXml));


            foreach (XmlNode currentNode in resultsXmlNode.ChildNodes)
            {
                if (currentNode.Name != "conditions" && item.clause == null)
                {
                    item.clause = SelectClauseFactory.CreateSelectClauseItem(currentNode, dvnLookup);
                    break;
                }
            }

            if (item.clause == null)
                throw new SQLGeneratorException(Resources.SwitchColumnNotSpecified);

            XmlNamespaceManager manager = new XmlNamespaceManager(resultsXmlNode.OwnerDocument.NameTable);
            manager.AddNamespace("COE", resultsXmlNode.NamespaceURI);

            XmlNode cases = resultsXmlNode.SelectSingleNode("COE:conditions", manager);
            if (cases != null)
            {
                foreach (XmlNode currentNode in cases.ChildNodes)
                {
                    if (currentNode.Attributes["default"] != null && currentNode.Attributes["default"].Value.ToLower() == "true")
                        item.defaultItem = SelectClauseFactory.CreateSelectClauseItem(currentNode.ChildNodes[0], dvnLookup);
                    else
                    {
                        string caseLabel = currentNode.Attributes["value"] != null ? currentNode.Attributes["value"].Value.Trim() : "unnamed";
                        SelectClauseItem childItem = SelectClauseFactory.CreateSelectClauseItem(currentNode.ChildNodes[0], dvnLookup);
                        item.cases.Add(caseLabel, childItem);
                    }
                }
            }
            else
                throw new Exception(string.Format("Required element {0} is missing: {1}", "conditions", resultsXmlNode.OuterXml));

            return item;
        }
        #endregion

    }
}

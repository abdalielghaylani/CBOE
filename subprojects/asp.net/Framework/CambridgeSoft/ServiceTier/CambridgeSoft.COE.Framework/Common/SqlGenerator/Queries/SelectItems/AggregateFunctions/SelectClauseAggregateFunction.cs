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
    /// This class allows the user to use a function to work on several SelectClauseItems on a single field.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseAggregate Function low level class called by all functions that inherit from it. The aggregate function can be single parameter in the simplest case or multiple
    /// parameter in more complex cases.  This class takes a list of selectclauseitmes as an ordered parametr list and concatenates the returned getdependentstring
    /// from each with parentese with the function name.  
    /// examples of simple, one parameter aggregate functions in oracle are max, min, count, std and sum. Similar names are used for sqlserver and msaccess.
    /// The class can be used directly in xml or programatically or the higher level convenience methods provided can be used. These in turn call this base class to actually
    /// build the string.  Using the low level functin directly is useful for custom functions that we do not provide
    ///
     /// <b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseField item = new SelectClauseField();
    /// item.DataField.Table = emp;
    /// item.DataField.FieldName = "yearsEmployed";
    /// SelectClauseFunction itemFunction = new SelectClauseFunction();
    /// itemFunction.Name = "AVG";
    /// itemFunction.Parameters.Add(item);
    /// itemFunction.Alias = "Average Years";
    /// query.AddSelectItem(itemFunction);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;AggregateFunction name="AVG" &gt;
    ///     &lt;Parameters&gt;
    ///		    &lt;field fieldid="1"/&gt;
    ///     &lt;/Parameters&gt;
    /// &lt;/function>
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// <b>In Oracle:</b>
    /// (AVG(e.id)) AS "Average Years Employed"
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (AVG(e.id)) AS "Average Years Employed"
    /// </para>
    /// <para>
    /// <b>To Be Done:</b> Review the SelectClauses Aliases (In this case there is no place to put it on the xml).
    /// </para>
    /// </summary>
    public class SelectClauseAggregateFunction : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// The Select Clauses to be parameters for the named function.
        /// </summary>
        public List<SelectClauseItem> Parameters
        {
            get
            {
                return this.parameters;
            }
            set
            {
                this.parameters = value;
            }
        }

        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                    return "AggFor" + this.DataField.FieldId;
                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

        public override string Name
        {
            get
            {
                return this.Alias;
            }
        }
        public string FunctionName
        {
            get
            {
                return this.functionName;
            }
            set
            {
                this.functionName = value;
            }
        }

        /// <summary>
        /// Overriden. The field that the function is being applied to.
        /// </summary>
        public override IColumn DataField
        {
            get
            {
                if (this.parameters != null && this.parameters.Count > 0)
                    return this.parameters[0].DataField;
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
        /// The Select Clauses to be used by the named function.
        /// </summary>
        protected List<SelectClauseItem> parameters;
        protected string functionName;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SelectClauseAggregateFunction()
        {
            this.parameters = new List<SelectClauseItem>();
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


            result.Append(this.FunctionName.ToUpper() + "(");
            if (this.parameters.Count > 1)
            {
                for (int currentIndex = 0; currentIndex < this.parameters.Count; currentIndex++)
                {
                    result.Append(this.parameters[currentIndex].Execute(dataBaseType, values));
                    if (currentIndex != this.parameters.Count-1)
                    {
                        result.Append(",");
                    }
                }
            }
            else
            {
                result.Append(this.parameters[0].Execute(dataBaseType, values));
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
            SelectClauseAggregateFunction item = new SelectClauseAggregateFunction();
            item.functionName = resultsXmlNode.Attributes["functionName"].Value.ToString().ToUpper();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            foreach (XmlNode currentChild in resultsXmlNode.ChildNodes)
            {
                if (currentChild.NodeType != XmlNodeType.Comment)
                {
                    SelectClauseItem childItem = SelectClauseFactory.CreateSelectClauseItem(currentChild, dvnLookup);
                    item.Parameters.Add(childItem);
                }
            }

            return item;
        }

        #endregion
    }
}

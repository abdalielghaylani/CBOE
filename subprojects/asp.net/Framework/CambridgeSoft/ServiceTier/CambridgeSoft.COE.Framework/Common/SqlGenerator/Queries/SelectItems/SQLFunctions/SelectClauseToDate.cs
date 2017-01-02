using System;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// SelectClauseToDate is a convience function that calls the lower level SelectClauseFunction to add a ToData(field, datemask) itme to a select clasue
    /// 
    ///<b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseToDate itemFunction = new SelectClauseToDate();
    /// itemFunction.FieldID = "1";
    /// query.AddSelectItem(itemFunction);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;ToDate fieldID="1" /&gt;
    ///     
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// <b>In Oracle:</b>
    /// (ToDate(e.id,"MM:DD:YYYY")) AS "First Name"
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (LCase(e.id)) AS "First Name" //TODO need to look this up
    /// </para>
    /// </summary>
    public class SelectClauseToDate : SelectClauseSQLFunction, ISelectClauseParser
    {

        #region Properties
        /// <summary>
        /// Name of the DataBase Field to be included in the SELECT clause.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        public Field DataField
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
        public override string Name
        {
            get
            {
                if (this.Alias.Trim() != "")
                    return this.Alias;
                return this.DataField.GetNameString();
            }
        }

        public string DateMask
        {
            get
            {
                return this.dateMask;
            }
            set
            {
                this.dateMask = value;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Name of the DataBase Field on which the TOData function is to be applied.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Field dataField;
        protected string dateMask;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseToDate()
        {
            this.dataField = new Field();
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method calls the selectclauseaggretate function getDependString to do the actual job. 
        /// First you prepare the parameters and add them to the base.Parameters collection
        /// then you call base.GetDependentString to do the jog
        /// The return will be:
        /// In this case returns the to date function applied to the specified field.
        /// TO_DATE(FieldName, dataMask)
        /// </summary>
        /// <param name="dataBaseType">The underlying Database.</param>
        /// <returns>Returns a string with the To_Date  function, to be added to the SELECT clause</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            base.Parameters.Clear();

            Field functionField = new Field();
            functionField.FieldName = this.dataField.FieldName;
            functionField.FieldId = this.dataField.FieldId;
            SelectClauseField selectClauseField = new SelectClauseField(functionField);
            selectClauseField.FieldId = this.dataField.FieldId;

            string singleQuotes = "'";
            //create the a selectclausefield as the single paramter
            SelectClauseLiteral selectClauseLiteral = new SelectClauseLiteral();
            selectClauseLiteral.Literal = singleQuotes + this.DateMask + singleQuotes;



            string functionName = string.Empty;
            switch (dataBaseType)
            {
                case DBMSType.ORACLE:
                    functionName = "TO_DATE";
                    break;
                //case DBMSType.SQLSERVER:
                //    functionName = "AVG";
                //    break;
                //case DBMSType.MSACCESS:
                //    functionName = "AVG";
                //    break;
            }
            base.FunctionName = functionName;
            base.Alias = this.Alias;
            base.Parameters.Add(selectClauseField);
            base.Parameters.Add(selectClauseLiteral);

            return base.GetDependantString(dataBaseType, values);
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Creates a new Instance based upon a xml represntation of the clause. 
        /// </summary>
        /// <param name="resultsXmlNode">The xml snippet containing the xml representation of the clause</param>
        /// <param name="dvnLookup">DataView Naming Lookup object used for translating identifiers to objects</param>
        /// <returns>a SelectClauseLowerCase item</returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseToDate item = new SelectClauseToDate();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            item.DataField.FieldId = int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim());
            item.DataField.FieldName = dvnLookup.GetFieldName(item.DataField.FieldId);
            item.DataField.FieldType = dvnLookup.GetFieldType(item.DataField.FieldId);
            item.DataField.Table = dvnLookup.GetParentTable(item.DataField.FieldId);
            item.DateMask = resultsXmlNode.Attributes["dateMask"].Value.Trim();

            return item;
        }

        #endregion
    }
}

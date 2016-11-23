using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// SelectCluaseMax is a convience function that calls the lower level SelectCluaseAggregateFunction to add a Max(field) itme to a select clasue
    /// 
    ///<b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseMax itemFunction = new SelectClauseMax();
    /// itemFunction.FieldID = "1";
    /// query.AddSelectItem(itemFunction);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;MAX fieldID="1" /&gt;
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
    /// (MAX(e.id)) AS "Max Years Employed"
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (MAX(e.id)) AS "Max Years Employed"
    /// </para>
    /// </summary>
    public class SelectClauseMax : SelectClauseAggregateFunction, ISelectClauseParser
    {
        #region Properties

        public override string Name
        {
            get
            {
                if (this.Alias.Trim() != "")
                    return this.Alias;
                return this.DataField.GetNameString();
            }
        }

        #endregion

        #region Variables
        /// <summary>
        /// Name of the DataBase Field to be included in the SELECT clause.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Field fieldName;

        /// <summary>
        /// Id of the DataBase Field.
        /// </summary>
        protected int fieldId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseMax()
        {
            this.fieldName = new Field();
            this.fieldName.FieldName = "*";

            this.dataField = new Field();
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case just return the name of the field.
        /// </summary>
        /// <returns>A string containing the select part corresponding to this field (i.e. the field name)</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            base.Parameters.Clear();
            //Coverity Bug Fix :- CID : 11477  Jira Id :CBOE-194
            Field functionField = this.dataField as Field;
            SelectClauseField selectClauseField = new SelectClauseField(functionField);
            selectClauseField.FieldId = this.dataField.FieldId;
            string functionName = string.Empty;

            switch (dataBaseType)
            {
                case DBMSType.ORACLE:
                    functionName = "MAX";
                    break;
                case DBMSType.SQLSERVER:
                    functionName = "MAX";
                    break;
                case DBMSType.MSACCESS:
                    functionName = "MAX";
                    break;
            }
            base.FunctionName = functionName;
            base.Alias = this.Alias;
            base.Parameters.Add(selectClauseField);

            string result = base.GetDependantString(dataBaseType, values);
            if (functionField != null)
            {
                switch (functionField.FieldType)
                {
                    case System.Data.DbType.Decimal:
                    case System.Data.DbType.Double:
                        result = "ROUND(" + result + ", 10)";
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a instance of the SelectClauseMax item
        /// </summary>
        /// <param name="fieldNode">xml representing the xml node</param>
        /// <returns>SelectClauseMax item,</returns>
        public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup)
        {
            SelectClauseMax item = new SelectClauseMax();

            if (fieldNode.Attributes["alias"] != null)
                item.Alias = fieldNode.Attributes["alias"].Value.ToString();

            item.DataField = new Field(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()), dvnLookup.GetFieldName(item.DataField.FieldId), dvnLookup.GetFieldType(item.DataField.FieldId));
            item.DataField.Table = dvnLookup.GetParentTable(item.DataField.FieldId);

            return item;
        }
        #endregion

    }
}

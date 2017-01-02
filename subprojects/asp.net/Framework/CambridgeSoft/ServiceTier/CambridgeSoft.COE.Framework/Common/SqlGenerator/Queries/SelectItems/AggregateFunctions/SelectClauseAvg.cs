using System;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// SelectCluaseAvg is a convience function that calls the lower level SelectCluaseAggregateFunction to add a Max(field) itme to a select clasue
    /// 
    ///<b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseAverage itemFunction = new SelectClauseAverage();
    /// itemFunction.FieldID = "1";
    /// query.AddSelectItem(itemFunction);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;AVG fieldID="1" /&gt;
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
    /// (AVG(e.id)) AS "Average Years Employed"
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (AVG(e.id)) AS "Average Years Employed"
    /// </para>
    /// </summary>
    public class SelectClauseAvg : SelectClauseAggregateFunction, ISelectClauseParser
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

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseAvg()
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
        /// In this case returns the SQL AVG aggregate function applied to the specified field.
        /// AVG(FieldName)
        /// </summary>
        /// <param name="dataBaseType">The underlying Database.</param>
        /// <returns>Returns a string with the AVG aggregate function, to be added to the SELECT clause</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {

            base.Parameters.Clear();
            ////Coverity Bug Fix :- CID : 11476  Jira Id :CBOE-194
            Field fld = this.DataField as Field;
            SelectClauseField selectClauseField = new SelectClauseField(this.dataField);

            string functionName = string.Empty;
            switch (dataBaseType)
            {
                case DBMSType.ORACLE:
                    functionName = "AVG";
                    break;
                case DBMSType.SQLSERVER:
                    functionName = "AVG";
                    break;
                case DBMSType.MSACCESS:
                    functionName = "AVG";
                    break;
            }
            base.FunctionName = functionName;
            base.Alias = this.Alias;
            base.Parameters.Add(selectClauseField);
            string result = base.GetDependantString(dataBaseType, values);
            if (fld != null)
            {
                switch (fld.FieldType)
                {
                    case System.Data.DbType.Decimal:
                    case System.Data.DbType.Double:
                        result = "ROUND(" + result + ", 10)";
                        break;
                }
            }
            return result;
        }
        #endregion


        #region ISelectClauseParser Members
        /// <summary>
        /// Creates a new Instance based upon a xml represntation of the clause. 
        /// </summary>
        /// <param name="resultsXmlNode">The xml snippet containing the xml representation of the clause</param>
        /// <param name="dvnLookup">DataView Naming Lookup object used for translating identifiers to objects</param>
        /// <returns>a SelectClauseAvg item</returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseAvg item = new SelectClauseAvg();
            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            item.DataField = new Field(int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim()), dvnLookup.GetFieldName(item.DataField.FieldId), dvnLookup.GetFieldType(item.DataField.FieldId));
            item.DataField.Table = dvnLookup.GetParentTable(item.DataField.FieldId);
            return item;
        }

        #endregion
    }
}

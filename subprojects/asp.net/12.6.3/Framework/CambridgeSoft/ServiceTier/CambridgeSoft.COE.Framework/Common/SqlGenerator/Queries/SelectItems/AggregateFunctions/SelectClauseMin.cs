using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// SelectCluaseMin is a convience function that calls the lower level SelectCluaseAggregateFunction to add a Max(field) itme to a select clasue
    /// 
    ///<b>Example:</b>
    /// </para>
    /// <b>Programatically:</b>
    /// <code lang="C#">
    /// Table emp = new Table();
    /// emp.TableName = "emp";
    /// emp.Alias = "e";
    /// SelectClauseMin itemFunction = new SelectClauseMin();
    /// itemFunction.FieldID = "1";
    /// query.AddSelectItem(itemFunction);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;Min fieldID="1" /&gt;
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
    /// (Min(e.id)) AS "Min Years Employed"
    /// <para>
    /// <b>In MS-SQLSERVER &amp; MS-Accesss:</b>
    /// (Min(e.id)) AS "Min Years Employed"
    /// </para>
    /// </summary>
	public class SelectClauseMin : SelectClauseAggregateFunction, ISelectClauseParser
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

       
        #endregion

        #region Variables
        /// <summary>
        /// Name of the DataBase Field on which the TOData function is to be applied.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Field dataField;
        #endregion

		#region Constructors
		/// <summary>
		/// Initializes its values to its default values.
		/// </summary>
		public SelectClauseMin() {
            this.dataField = new Field();
			
		}
		#endregion

		#region Methods
		/// <summary>
		/// This method does the actual job.
		/// In this case just return the name of the field.
		/// </summary>
		/// <returns>A string containing the select part corresponding to this field (i.e. the field name)</returns>
		protected override string GetDependantString(DBMSType dataBaseType, List<Value> values) {

            base.Parameters.Clear();

            Field functionField = new Field();
            functionField.FieldName = this.dataField.FieldName;
            functionField.FieldId = this.dataField.FieldId;
            SelectClauseField selectClauseField = new SelectClauseField(functionField);
            selectClauseField.FieldId = this.dataField.FieldId;

            string functionName = string.Empty;
            switch (dataBaseType)
            {
                case DBMSType.ORACLE:
                    functionName = "MIN";
                    break;
                case DBMSType.SQLSERVER:
                    functionName = "MIN";
                    break;
                case DBMSType.MSACCESS:
                    functionName = "MIN";
                    break;
            }
            base.FunctionName = functionName;
            base.Alias = this.Alias;
            base.Parameters.Add(selectClauseField);

            string result = base.GetDependantString(dataBaseType, values);
            switch(this.dataField.FieldType)
            {
                case System.Data.DbType.Decimal:
                case System.Data.DbType.Double:
                    result = "ROUND(" + result + ", 10)";
                    break;
            }
            return result;
		}

        /// <summary>
        /// Creates a new Instance based upon a xml represntation of the clause. 
        /// </summary>
        /// <param name="resultsXmlNode">The xml snippet containing the xml representation of the clause</param>
        /// <param name="dvnLookup">DataView Naming Lookup object used for translating identifiers to objects</param>
        /// <returns>a SelectClauseMin item</returns>
		public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup) {
			SelectClauseMin item = new SelectClauseMin();
			
			if(fieldNode.Attributes["alias"] != null)
				item.Alias = fieldNode.Attributes["alias"].Value.ToString();

            item.DataField.FieldId = int.Parse(fieldNode.Attributes["fieldId"].Value.Trim());
            item.DataField.FieldName = dvnLookup.GetFieldName(item.DataField.FieldId);
            item.DataField.Table = dvnLookup.GetParentTable(item.DataField.FieldId);
			
			return item;
		}
		#endregion

	}
}

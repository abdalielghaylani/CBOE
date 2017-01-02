using System;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
	/// <summary>
	/// <para>
	/// This class represents a simple field. 
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The SelectClauseField class requires the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item><b>Field dataField:</b> Represents the field itself that is going to be added to the columns of the query.</item>
	/// <item><b>int FieldId:</b> The Id of the field DataField. This can be obtained directly from the dataField, and is left only for backward compatibility.</item>
	/// <item><b>string FieldName:</b> The Name of the field DataField. This can be obtained directly from the dataField, and is left only for backward compatibility.</item>
	/// <item><b>string TableName:</b> The Table owner of the field DataField. This can be obtained directly from the dataField, and is left only for backward compatibility.</item>
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
	/// Table emp = new Table();
	/// emp.TableName = "emp";
	/// emp.Alias = "e";
	/// SelectClauseField field  = new SelectClauseField();
	/// field.DataField.Table = emp;
	/// field.DataField.FieldName = "name";
    /// field.Alias = "EmployeName";
	/// query.AddSelectItem(field);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
    /// &lt;field fieldId="2" alias"EmployeName"/&gt;
	/// </code>
    /// <i>where the field 2 aliased "EmployeName" is called "name" and belongs to "emp" table (whose alias is e) in the dataview defined for this query.</i>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following select clause statement:
	/// </para>
	/// <para>
	/// e.name
	/// </para>
	/// </summary>
    public class SelectClauseField : SelectClauseItem, ISelectClauseParser
    {

        #region Properties
        /// <summary>
		/// Gets or sets the id of the DataBase Field.
		/// </summary>
		public int FieldId {
			get {
				return this.fieldId;
			}
			set {
				this.fieldId = value;
			}
		}

		public override string Name {
			get {
                //if(this.Alias != null && this.Alias.Trim() != "")
                //    return this.Alias;

				return this.DataField.GetNameString();
			}
		}
        #endregion

        

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseField() {
            this.dataField = new Field();
        }
        public SelectClauseField(IColumn column)
        {
            this.dataField = column;
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
            var field = dataField as Field;
            if (field != null && field.FieldType == System.Data.DbType.DateTime)
            {
                return string.Format("TO_DATE(TO_CHAR({0}, 'YYYY-MON-DD HH24:MI:SS'), 'YYYY-MON-DD HH24:MI:SS')", this.dataField.GetFullyQualifiedNameString());
            }

            return this.dataField.GetFullyQualifiedNameString();
        }

        #endregion

		#region ISelectClauseParser Members
		/// <summary>
		/// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseField, which at this point
		/// has only set its id.
		/// </summary>
		/// <param name="fieldNode">The field node of the search results xml definition.</param>
		/// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
		/// <returns>An instance of SelectClauseField.</returns>
		public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup) {
			SelectClauseField item = new SelectClauseField();
            if(fieldNode.Attributes["alias"] != null && fieldNode.Attributes["alias"].Value != string.Empty) {
                item.Alias = fieldNode.Attributes["alias"].Value.ToString();
            } else {
                item.Alias = dvnLookup.GetColumnAlias(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            }

            if (fieldNode.Attributes["visible"] != null && fieldNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(fieldNode.Attributes["visible"].Value);

            //CBOE-779/CBOE-1026
            //(REG: Records cannot be searched in CBOE Build 12.6.0.2505)
            IColumn dtField=dvnLookup.GetColumn(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            if(dtField!=null)
            {
                item.DataField = dtField;
            }   

			return item;
		}
		#endregion
	}
}

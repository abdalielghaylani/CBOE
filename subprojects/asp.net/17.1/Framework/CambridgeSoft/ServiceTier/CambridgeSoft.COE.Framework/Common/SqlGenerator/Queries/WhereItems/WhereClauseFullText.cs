using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.COEConfigurationService;
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
	/// <summary>
	/// <para>
	/// This class implements a Full Text operator, for Oracle this produces "Contains(fld, 'where clause'). It assumes fld is indexted by Oracle CTXSys.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The WhereClauseStructure class required the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item>Field dataField: Represents the field of the database that is being compared. Its name is required.</item>
	/// <item>Value Val: The value should contain the id of the already inserted query molecule</item>
    /// <item>String  Full Text Cartridge: This should be the name of the schema for the text searching cartridge. By default for oracle it is set to ctxsys.context</item>

	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// At this time the implementation will work on Oracle database
	/// </para>
	/// <para>
	/// The SQL generation aims prepared statements rather than simple queries: parameters are not hardcoded into the query text, but marked with a parameter holder character. Parameters are returned in a separate list, ordered by position as they appear in the query.
	/// Along with WhereClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType, Values array) method as the “Execute Method”
	/// </para>
	/// <para>
	/// <b>Example:</b>
	/// </para>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;searchCriteriaItem id=XX fieldid=20 modifier=’’ tableid=3&gt;
	/// 	&lt;FullTextCriteria
	/// 		
    /// 	&lt;/FullTextCriteria&gt;
    /// &lt;/searchCriteriaItem&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following where clause statement:
	/// </para>
	/// <para>
	/// [AND] Contains (:0, :1,);
	/// </para>
	/// <para>

	/// </list>
	/// </summary>
	public class WhereClauseFullText : WhereClauseBinaryOperation
	{

		#region Properties
        


		#endregion

		#region Variables
	
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public WhereClauseFullText()
            : base()
        {
			base.dataField = new Field();
			base.val = new Value();
          
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Gets the string representation of the where clause structure for the given database and
        /// adds its parameter values to the values list.
		/// </summary>
		/// <param name="databaseType">The database to get the string for.</param>
		/// <param name="values">Already existing parameter values or a query.</param>
		/// <returns>The string representation.</returns>
		protected override string GetDependantString(DBMSType databaseType, List<Value> values) {
			
            
            StringBuilder builder = new StringBuilder();

            switch (databaseType)
            {
                case DBMSType.ORACLE:
                    builder.Append("CONTAINS(");
                    builder.Append(base.GetFullName(dataField));
                    builder.Append(",");
                    builder.Append(this.ParameterHolder);
                    if (this.UseParametersByName)
                        builder.Append(values.Count);
                    val.Val = this.val.Val.Replace('*','%');
                    values.Add(this.val);
                    builder.Append(")>0");
                    break;
                case DBMSType.SQLSERVER:
                    throw new Exception("not implement for sqlserver");
                case DBMSType.MSACCESS:
                    throw new Exception("not implement for access");
                default:
                    break;
            }
			
			return builder.ToString();
		}

		
		#endregion
	}
}

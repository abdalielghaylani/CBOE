using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors
{
	/// <summary>
	/// This class is responsible for handling Structure like queries. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class StructureProcessor : SearchProcessor
	{
		#region Variables
		private string structure;
		private int primaryKey;
		private string tempTableName;
        private string schemaName;
        private SearchCriteria.StructureCriteria _structureCriteria;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");

		#endregion

		#region Properties
		/// <summary>
		/// Temporary table name to be used for inserting molecules in the preprocess step, in the case of using CSCartridge.
		/// </summary>
        internal string TempTableName
        {
            get
            {
                return this.tempTableName;
            }
            set
            {
                this.tempTableName = value;
            }
        }

        /// <summary>
        /// Gets or sets the cartridge schema name.
        /// </summary>
        internal string SchemaName
        {
            get
            {
                return this.schemaName;
            }
            set
            {
                this.schemaName = value;
            }
        }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of Structure Processor with a given SearchCriteria Item xml representation
		/// </summary>
		/// <param name="item">The SearchCriteriaItem xml represntation that is to be handled by this Processor</param>
        internal StructureProcessor(XmlNode xmlNode) : base(xmlNode)
        {
            this.schemaName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
            this.tempTableName = "TEMPQUERIES";
        }

		/// <summary>
		/// Creates a new instance of Structure Processor with a given SearchCriteria Item
		/// </summary>
		/// <param name="item">The SearchCriteriaItem that is to be handled by this Processor</param>
        internal StructureProcessor(SearchCriteria.SearchCriteriaItem item)
            : base(item)
        {
            _structureCriteria = (SearchCriteria.StructureCriteria)item.Criterium;
            this.structure = _structureCriteria.Structure;

            this.schemaName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
            this.tempTableName = "TEMPQUERIES";
        }

		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this processor
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview)
        {
			this.primaryKey = 1;

			/*string tempTableName = ((SearchCriteria.StructureCriteria)this.item.Criterium).CartridgeSchema;
			if(tempTableName.Trim() != "")
				tempTableName += ".";
			tempTableName += "TempQueries";
			*/

			Insert insert = new Insert();
            var mainTable = new Table() { Database = this.schemaName, TableName = this.tempTableName };

            insert.MainTable = mainTable;
            insert.Fields.Add(new Field("QUERY", DbType.String, mainTable));
            insert.Fields.Add(new Field("ID", DbType.Int32, mainTable));

			insert.ParamValues.Add(new Value(this.structure, DbType.String));
			insert.ParamValues.Add(new Value(primaryKey.ToString(), DbType.Int32));

#if DEBUG
            System.Diagnostics.Debug.WriteLine("StructureProcessor: Query Inseriton: ");
            insert.UseParameters = false;
            System.Diagnostics.Debug.WriteLine(insert);
            System.Diagnostics.Debug.WriteLine(" ");
            insert.UseParameters = true;
#endif
		    try
		    {
		        searchDAL.ExecuteNonQuery(insert);
		    }
		    catch (Exception ex)
		    {
		        // ORA-0001: unique constraint violated
                var oraEx = ex as Oracle.DataAccess.Client.OracleException;
                if (oraEx!=null && oraEx.Number == 1)
		        {
                    // According to design, we are allow to only insert 1 row in TEMPQUERIES table in one transaction.
                    // The problem is that in some case, such as DAT-1121, in GetData function it will create more than one 
                    // SturctureProcessor instances one is for parent table query the other is for child table query. In these
                    // cases, an exception will be raised when process SturctureProcessor second time.
                    // We almost believe that the SturctureProcessor instance for parent table query is not necessary. But in
                    // this time, we don't want to refactor the code. So we ignored the second insert operation.
		        }
                else
                {
                    // Other errors
                    throw new Exception("Error when trying to insert temporary molecule - StructureProcessor. \r\n" + ex.Message);
                }
		    }

		    Query tempSelect = new Query();
            tempSelect.SetMainTable(mainTable);

            SelectClauseField queryColumn = new SelectClauseField();
            queryColumn.DataField = new Field("QUERY", DbType.String, mainTable);

            WhereClauseEqual primaryKeyConstraint = new WhereClauseEqual();
            primaryKeyConstraint.DataField = new Field("ID", DbType.Int32, mainTable);
            primaryKeyConstraint.Val = new Value(this.primaryKey.ToString(), DbType.Int32);

            tempSelect.AddSelectItem(queryColumn);
            tempSelect.AddWhereItem(primaryKeyConstraint);

            tempSelect.EncloseInParenthesis = false;
            tempSelect.UseParameters = false;
            string tempSelectString = tempSelect.ToString(); //.Replace(":0", this.primaryKey.ToString());

            ((SearchCriteria.StructureCriteria)_searchCriteriaItem.Criterium).Query4000 = tempSelectString;
		}

		/// <summary>
		/// Template Method Process - No need to update the query
		/// </summary>
		/// <returns>a new searchCriteriaItem that is used to rebuild the searchCriteria xml</returns>
        public override void Process(Query query) { }

		/// <summary>
		/// Template Method PostProcess - Restores the original structure.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL)
        {
            _structureCriteria.Structure = this.structure;
		}
		#endregion
	}
}

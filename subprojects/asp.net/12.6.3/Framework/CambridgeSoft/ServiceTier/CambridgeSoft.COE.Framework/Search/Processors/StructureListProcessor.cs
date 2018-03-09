using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.IO;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors
{
	/// <summary>
	/// This class is responsible for handling Structure list like queries. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class StructureListProcessor : SearchProcessor
	{
		#region Variables
        private const string CANONICAL_TABLE_ALIAS = "CanonicalTBL";
        private const string TEMP_TABLE_ALIAS = "TempTBL";
        private string[] structureList;
		private int maxID;
		private string tempTableName;
        private SearchCriteria.StructureListCriteria _structureListCriteria;
        private int _originalTableId = -1;
        private int _originalFieldId = -1;
        private string schemaName;
        private Relation _query2LookupRel = null;
        private Relation _lookup2CanonicalRel = null;

        private string _canonicalTableName = string.Empty;
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
        internal StructureListProcessor(XmlNode xmlNode) : base(xmlNode)
        {
            this.SchemaName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
            this.tempTableName = this.SchemaName + ".TEMPQUERIES";
            _canonicalTableName = string.Empty;
        }

		/// <summary>
		/// Creates a new instance of Structure Processor with a given SearchCriteria Item
		/// </summary>
		/// <param name="item">The SearchCriteriaItem that is to be handled by this Processor</param>
        internal StructureListProcessor(SearchCriteria.SearchCriteriaItem item) : base(item)
        {
            _structureListCriteria = (SearchCriteria.StructureListCriteria) item.Criterium;
            this.structureList = ParsingUtilities.ParseList(new MemoryStream(UnicodeEncoding.ASCII.GetBytes(_structureListCriteria.StructureList)), "sdf").ToArray();
            this.SchemaName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
            this.tempTableName = this.SchemaName + ".TEMPQUERIES";
            _canonicalTableName = string.Empty;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Fetches the canonical table, inserts the canonical of the given structures into temp.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL, COEDataView dataview)
        {
            this.maxID = structureList.Length;
            this.SchemaName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
#if DEBUG
            string inserts = string.Empty;

#endif
            //fetch the canonical tablename
            /*
            explain plan SET STATEMENT_ID = '123' for
            select * from regdb.vw_MIXTURE_structure where cscartridge.moleculecontains(structure, 'c1cccc1','','full=no')=1;

            SELECT cscartridge.aux.cnTabName('REGDB', (select OBJECT_NAME from sys.plan_table$ where STATEMENT_ID = '123' and operation='DOMAIN INDEX' AND ROWNUM < 2)) FROM DUAL;
            */
            COEDataView.DataViewTable table = dataview.Tables.getById(_searchCriteriaItem.TableId);
            //Coverity fix - CID 11738
            if (table != null)
            {
                COEDataView.Field fld = dataview.Tables[table.Name].Fields.getById(_searchCriteriaItem.FieldId);
                if (fld != null)
                {
                    if (fld.LookupDisplayFieldId < 0)
                    {
                        if (fld.IndexType != COEDataView.IndexTypes.CS_CARTRIDGE)
                            throw new Exception(Resources.SDFSearchWithoutIndex);
                    }
                    else
                    {
                        // Need to remember table and field info for the query2lookup relationsip
                        // *before* we overwrite the fld with the lookup field info
                        string pfldName = fld.Name;
                        string ptblName = string.Format(@"{0}.{1}", table.Database, table.Name);
                        string ptblAlias = table.Alias;
                        string lookupFieldName = dataview.GetFieldById(fld.LookupFieldId).Name;

                        // Overwrite the fld with the lookup field
                        fld = dataview.GetFieldById(fld.LookupDisplayFieldId);
                        table = dataview.Tables.getById(fld.ParentTableId);

                        // Check the lookup table is cartridge indexed 
                        if (fld.IndexType != COEDataView.IndexTypes.CS_CARTRIDGE)
                            throw new Exception(Resources.SDFSearchOverInvalidField);

                        // Rememeber the original table/field
                        _originalFieldId = _searchCriteriaItem.FieldId;
                        _originalTableId = _searchCriteriaItem.TableId;

                        // overwrite table/field with lookup info
                        _searchCriteriaItem.TableId = table.Id;
                        _searchCriteriaItem.FieldId = fld.Id;


                        // Setup relationships that will be used to ajust the query in the processor
                        // This relationship will be used to add the lookup table to the query
                        // Note empty field names in relationship means that table is added to FROM clause 
                        // without a corresponding join clause
                        _lookup2CanonicalRel = new Relation(string.Format("{0}.{1}", table.Database, table.Name),
                                                            table.Alias,
                                                            string.Empty,
                                                            _canonicalTableName,
                                                            CANONICAL_TABLE_ALIAS,
                                                            string.Empty);

                        // query2LookupRel
                        // will be used in processor to add the join between query and lookup table
                        _query2LookupRel = new Relation(ptblName,
                                                        ptblAlias,
                                                        pfldName,
                                                        string.Format(@"{0}.{1}", table.Database, table.Name),
                                                        table.Alias,
                                                        lookupFieldName);
                    }
                }
                string explainSQL = @"EXPLAIN PLAN SET STATEMENT_ID = '" + _searchCriteriaItem.FieldId.ToString() + @"' INTO COEDB.COE_PLAN_TABLE for 
            SELECT 1 FROM " + table.Database + "." + table.Name + " WHERE " + this.SchemaName + ".MOLECULECONTAINS(" + table.Name + "." + fld.Name + @", 'C1CCCC1','','IDENTITY=YES')=1";

                string fetchCanonicalTBL = "SELECT " + this.SchemaName + ".AUX.CNTABNAME('" + table.Database + @"', (SELECT OBJECT_NAME FROM COEDB.COE_PLAN_TABLE WHERE STATEMENT_ID = '" + _searchCriteriaItem.FieldId.ToString() + @"' AND OPERATION='DOMAIN INDEX' AND ROWNUM < 2)) FROM DUAL";

                DbCommand cmd = searchDAL.DALManager.Database.GetSqlStringCommand(explainSQL);
                try
                {
                    searchDAL.DALManager.ExecuteNonQuery(cmd);
                    cmd = searchDAL.DALManager.Database.GetSqlStringCommand(fetchCanonicalTBL);
                    object obj = searchDAL.DALManager.ExecuteScalar(cmd);
                    if (obj != null && !Convert.IsDBNull(obj))
                        _canonicalTableName = obj.ToString().ToUpper();
                }
                catch (Exception e)
                {                   
                    throw new Exception(Resources.SDFSearchWithoutIndex);
                }                

                if (_canonicalTableName.ToLower().EndsWith(table.Database.ToLower() + "_")) //No Index Found...
                    throw new Exception(Resources.SDFSearchWithoutIndex);






                for (int i = 0; i < maxID; i++)
                {
                    /*
                    insert into cscartridge.TEMPQUERIES(ID, QUERY) VALUES (0,CSCARTRIDGE.CONVERTCDX.CDXTOCANONICAL(base64_cdx));
                    */
                    string insert = "INSERT INTO " + this.SchemaName + ".TEMPQUERIES(ID, QUERY)" +
                                    "VALUES (" + searchDAL.DALManager.BuildSqlStringParameterName("ix") + "," + this.SchemaName + ".CONVERTCDX.CDXTOCANONICAL(" + searchDAL.DALManager.BuildSqlStringParameterName("structure") + "))";

                    DbCommand command = searchDAL.DALManager.Database.GetSqlStringCommand(insert);

                    Oracle.DataAccess.Client.OracleParameter paramIndex = new Oracle.DataAccess.Client.OracleParameter(searchDAL.DALManager.BuildSqlStringParameterName("ix"), Oracle.DataAccess.Client.OracleDbType.Int32, ParameterDirection.Input);
                    paramIndex.Value = i;
                    command.Parameters.Add(paramIndex);

                    Oracle.DataAccess.Client.OracleParameter paramStructure = new Oracle.DataAccess.Client.OracleParameter(searchDAL.DALManager.BuildSqlStringParameterName("structure"), Oracle.DataAccess.Client.OracleDbType.Clob, ParameterDirection.Input);
                    Oracle.DataAccess.Types.OracleClob clob = new Oracle.DataAccess.Types.OracleClob(searchDAL.DALManager.DbConnection as Oracle.DataAccess.Client.OracleConnection);
                    char[] charAray = this.structureList[i].ToCharArray();
                    clob.Write(charAray, 0, charAray.Length);
                    paramStructure.Value = clob;
                    command.Parameters.Add(paramStructure);
#if DEBUG
                    inserts += insert.Replace(":ix", i.ToString()).Replace(":structure", "'" + this.structureList[i] + "'") + ";\n";
#endif
                    try
                    {
                        if (searchDAL.DALManager.ExecuteNonQuery(command) < 1)
                            throw new Exception("Error when trying to insert temporary molecule - StructureListProcessor");
                    }
                    catch (Exception ex)
                    {
                        // We don't want to stop the loop if there is an error in a single insertion, but we would need to log the error somehow
                        //TODO: Log the error.
                    }
                }

#if DEBUG
                System.Diagnostics.Debug.WriteLine(inserts);
#endif
            }
        }
		/// <summary>
		/// Template Method Process - No modification to the query is needed
		/// </summary>
		/// <returns>a new searchCriteriaItem that is used to rebuild the searchCriteria xml</returns>
        public override void Process(Query query) 
        {
            if (!this.TempTableName.Contains("."))
                this.TempTableName = string.Format("{0}.{1}", this.SchemaName, this.TempTableName);
            // Note empty field names in relationship means that table is added to FROM clause 
            // without a corresponding join clause
            Relation rel = new Relation(_canonicalTableName,
                                        CANONICAL_TABLE_ALIAS,
                                        string.Empty,
                                        this.TempTableName,
                                        TEMP_TABLE_ALIAS,
                                        string.Empty);
            query.AddJoinRelation(rel);
            
            // Add relationships created by preprocessor for lookup case
            if (_lookup2CanonicalRel != null)
            {
                query.AddJoinRelation(_lookup2CanonicalRel);
                query.AddJoinRelation(_query2LookupRel);
            }

        }

		/// <summary>
		/// Template Method PostProcess - Performs the required steps for cleaning up the command execution.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) 
        {
            if (_originalFieldId > 0)
                _searchCriteriaItem.FieldId = _originalFieldId;
            if (_originalTableId > 0)
                _searchCriteriaItem.TableId = _originalTableId;
        }
		#endregion
	}
}
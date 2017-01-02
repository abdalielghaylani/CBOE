using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common;
using FormWizard;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;
using SpotfireIntegration.Common;
using CambridgeSoft.COE.Framework.COESearchService;
using System;

namespace SpotfireIntegration.SpotfireAddin
{
    public sealed class ResearchAssayHistoryTool : CustomTool<AnalysisApplication>
    {
        public ResearchAssayHistoryTool()
            : base("Load Research Assay History...")
        {
        }

        protected override void ExecuteCore(AnalysisApplication context)
        {
            //RunResearchAssayHistoryForm(context);
        }

        /*
        static internal void RunResearchAssayHistoryForm(AnalysisApplication context)
        {
            COEService service = context.GetService<COEService>();
            if (!service.IsAuthenticated)
            {
                if (!service.Login())
                {
                    MessageBox.Show("Unable to authenticate with the ChemOffice Enterprise server.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            ResearchAssayHistoryForm.ResearchAssayHistoryForm rahForm = new ResearchAssayHistoryForm.ResearchAssayHistoryForm();
            rahForm.Projects = GetProjects(service);

            if (rahForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            COEDataView dataView = service.GetDataView(666);
            HitListInfo hitListInfo = null;
            COEHitListBO hitListBO = null;

            service.ExecuteUsingCOEPrincipal(delegate() {
                hitListInfo = GetHitList(rahForm, dataView);
                hitListBO = COEHitListBO.Get(hitListInfo.HitListType, hitListInfo.HitListID);
            });
            
            // Now prompt for a results criteria.
            SelectDataForm selectDataForm = new SelectDataForm();
            selectDataForm.availableDataViews = service.GetDataViews();
            selectDataForm.tableFilter = new HitListTableFilter(service, hitListBO);

            if (selectDataForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            COEDataViewBO selectedDataView = selectDataForm.dataViewBO;
            ResultsCriteria resultsCriteria = selectDataForm.resultsCriteria;

            HitListInfo refinedHitListInfo = RefineHitList(service, hitListInfo, selectedDataView, resultsCriteria);

            // Load the DataView.
            int dataViewID = selectedDataView.ID;
            int hitListID = refinedHitListInfo.HitListID;
            HitListType hitListType = refinedHitListInfo.HitListType;
            int numHits = refinedHitListInfo.RecordCount;
            COEHitList hitList = new COEHitList(hitListID, hitListType, numHits, dataViewID, resultsCriteria);

            // Now just load the data.
            string baseTableAlias = selectedDataView.COEDataView.Tables[selectedDataView.BaseTable].Alias;
            SpotfireCOETableLoader.LoadTables(context, hitList, baseTableAlias);
        }

        #region Search Methods

        static private COEDataView.Field GetField(COEDataView.DataViewTable table, string name)
        {
            foreach (COEDataView.Field field in table.Fields)
            {
                if (name.Equals(field.Name))
                {
                    return field;
                }
            }
            throw new Exception(string.Format("No \"{0}\" field found", name));
        }

        static private SearchCriteria GetSearchCriteria(ResearchAssayHistoryForm.ResearchAssayHistoryForm rahForm, COEDataView dataView)
        {
            SearchCriteria searchCriteria = new SearchCriteria();

            switch (rahForm.SelectedSearchType)
            {
                case ResearchAssayHistoryForm.ResearchAssayHistoryForm.SearchType.ARNumber:
                    COEDataView.Field regNumberField = GetField(dataView.Tables.getById(dataView.Basetable), "REG_BATCH");
                    SearchCriteria.LogicalCriteria arNumberDisjunction = new SearchCriteria.LogicalCriteria();
                    arNumberDisjunction.LogicalOperator = SearchCriteria.COELogicalOperators.Or;
                    searchCriteria.Items.Add(arNumberDisjunction);

                    foreach (string arNumber in rahForm.ArNumbers)
                    {
                        SearchCriteria.TextCriteria arNumberCriteria = new SearchCriteria.TextCriteria();
                        arNumberCriteria.CaseSensitive = SearchCriteria.COEBoolean.No;
                        arNumberCriteria.Operator = SearchCriteria.COEOperators.STARTSWITH;
                        arNumberCriteria.Trim = SearchCriteria.Positions.Both;
                        arNumberCriteria.Value = arNumber;
                        SearchCriteria.SearchCriteriaItem arNumberItem = new SearchCriteria.SearchCriteriaItem();
                        arNumberItem.TableId = dataView.Basetable;
                        arNumberItem.FieldId = regNumberField.Id;
                        arNumberItem.Criterium = arNumberCriteria;
                        arNumberDisjunction.Items.Add(arNumberItem);
                    }
                    break;

                case ResearchAssayHistoryForm.ResearchAssayHistoryForm.SearchType.Project:
                    COEDataView.Field projectField = GetField(dataView.Tables.getById(dataView.Basetable), "BATCH_PROJECT_ID");
                    SearchCriteria.TextCriteria projectCriteria = new SearchCriteria.TextCriteria();
                    projectCriteria.CaseSensitive = SearchCriteria.COEBoolean.Yes;
                    projectCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                    projectCriteria.Trim = SearchCriteria.Positions.None;
                    projectCriteria.Value = rahForm.SelectedProject;
                    SearchCriteria.SearchCriteriaItem projectItem = new SearchCriteria.SearchCriteriaItem();
                    projectItem.TableId = dataView.Basetable;
                    projectItem.FieldId = projectField.Id;
                    projectItem.Criterium = projectCriteria;
                    projectItem.SearchLookupByID = false;
                    searchCriteria.Items.Add(projectItem);
                    break;

                case ResearchAssayHistoryForm.ResearchAssayHistoryForm.SearchType.Structure:
                    COEDataView.Field molIDField = GetField(dataView.Tables.getById(dataView.Basetable), "MOL_ID");
                    int structureFieldId = molIDField.LookupDisplayFieldId;
                    COEDataView.Field structureField = dataView.GetFieldById(structureFieldId);
                    SearchCriteria.StructureCriteria structureCriteria = new SearchCriteria.StructureCriteria();
                    switch (rahForm.SelectedStructureSearchType)
                    {
                        case ResearchAssayHistoryForm.ResearchAssayHistoryForm.StructureSearchType.ExactStructure:
                            structureCriteria.Identity = SearchCriteria.COEBoolean.Yes;
                            break;
                        case ResearchAssayHistoryForm.ResearchAssayHistoryForm.StructureSearchType.FullStructure:
                            structureCriteria.FullSearch = SearchCriteria.COEBoolean.Yes;
                            break;
                        case ResearchAssayHistoryForm.ResearchAssayHistoryForm.StructureSearchType.Substructure:
                            structureCriteria.FullSearch = SearchCriteria.COEBoolean.No;
                            break;
                        case ResearchAssayHistoryForm.ResearchAssayHistoryForm.StructureSearchType.TanimotoSimilarity:
                            structureCriteria.Similar = SearchCriteria.COEBoolean.Yes;
                            structureCriteria.SimThreshold = 90;
                            break;
                    }
                    //structureCriteria.Format = "cdxb64";
                    structureCriteria.Implementation = "cscartridge";
                    structureCriteria.Structure = rahForm.StructureBase64;
                    SearchCriteria.SearchCriteriaItem structureItem = new SearchCriteria.SearchCriteriaItem();
                    structureItem.TableId = dataView.Basetable;
                    structureItem.FieldId = molIDField.Id;
                    //structureItem.TableId = structureField.ParentTableId;
                    //structureItem.FieldId = structureFieldId;
                    structureItem.Criterium = structureCriteria;
                    structureItem.SearchLookupByID = false;
                    searchCriteria.Items.Add(structureItem);
                    break;
            }

            COEDataView.DataViewTable summaryTable = dataView.Tables["RESULTS_SUMMARY"];
            if (summaryTable != null)
            {
                COEDataView.Field numResultsField = GetField(summaryTable, "NUM_RESULTS");
                if (numResultsField != null)
                {
                    SearchCriteria.NumericalCriteria numResultsCriteria = new SearchCriteria.NumericalCriteria();
                    numResultsCriteria.Operator = SearchCriteria.COEOperators.GT;
                    numResultsCriteria.Value = "0";
                    SearchCriteria.SearchCriteriaItem numResultsItem = new SearchCriteria.SearchCriteriaItem();
                    numResultsItem.TableId = summaryTable.Id;
                    numResultsItem.FieldId = numResultsField.Id;
                    numResultsItem.Criterium = numResultsCriteria;
                    searchCriteria.Items.Add(numResultsItem);
                }
            }

            return searchCriteria;
        }

        static private HitListInfo GetHitList(ResearchAssayHistoryForm.ResearchAssayHistoryForm rahForm, COEDataView dataView)
        {
            SearchCriteria searchCriteria = GetSearchCriteria(rahForm, dataView);
            COESearch search = new COESearch(dataView.DataViewID);
            return search.GetHitList(searchCriteria, dataView.DataViewID);
        }

        static private IList<string> GetProjects(COEService service)
        {
            string connectionString = string.Format("Data Source={0};User Id={1};Password={2}", service.DataSourceName, "CSSADMIN", "CSSADMIN");
            OracleDatabase database = new OracleDatabase(connectionString);
            DbConnection connection = database.CreateConnection();
            connection.Open();

            const string sql = "select project_name from regdb.batch_projects where active = 1 order by upper(project_name)";

            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            DbDataReader reader = command.ExecuteReader();

            IList<string> projects = new List<string>();
            while (reader.Read())
            {
                string projectName = reader.GetString(0);
                projects.Add(projectName);
            }

            return projects;
        }

        static private HitListInfo RefineHitList(COEService service, HitListInfo hitListInfo, COEDataViewBO dataViewBO, ResultsCriteria resultsCriteria)
        {
            SearchCriteria.LogicalCriteria assayDisjunction = new SearchCriteria.LogicalCriteria();
            assayDisjunction.LogicalOperator = SearchCriteria.COELogicalOperators.Or;

            // Create a search criteria to refine for compounds that have at least one assay result in the results criteria.
            foreach (ResultsCriteria.ResultsCriteriaTable rcTable in resultsCriteria.Tables)
            {
                if (rcTable.Id == dataViewBO.COEDataView.Basetable)
                {
                    continue;
                }

                TableBO dvTable = dataViewBO.DataViewManager.Tables.GetTable(rcTable.Id);

                SearchCriteria.TextCriteria pkCriteria = new SearchCriteria.TextCriteria();
                pkCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                pkCriteria.Value = "NOT NULL";

                SearchCriteria.SearchCriteriaItem pkItem = new SearchCriteria.SearchCriteriaItem();
                pkItem.TableId = dvTable.ID;
                pkItem.FieldId = dvTable.PrimaryKey;
                pkItem.Criterium = pkCriteria;
                assayDisjunction.Items.Add(pkItem);
            }

            if (assayDisjunction.Items.Count > 0)
            {
                SearchCriteria searchCriteria = new SearchCriteria();
                searchCriteria.Items.Add(assayDisjunction);

                COESearch search = new COESearch(dataViewBO.ID);
                HitListInfo refinedHitListInfo = null;
                service.ExecuteUsingCOEPrincipal(delegate { refinedHitListInfo = search.GetHitList(searchCriteria, dataViewBO.COEDataView, hitListInfo); });
                return refinedHitListInfo;
            }
            else
            {
                return hitListInfo;
            }
        }

        #endregion
    }

    class HitListTableFilter : ITableFilter
    {
        private COEService _service;
        private COEHitListBO _hitListBO;
        private IDictionary<string, HitsCompoundsPair> _cache;

        internal HitListTableFilter(COEService service, COEHitListBO hitListBO)
        {
            this._service = service;
            this._hitListBO = hitListBO;
        }

        #region ITableFilter Members

        public bool IncludeTable(SelectDataForm form, TableBO tableBO)
        {
            if (tableBO.ID == form.dataViewBO.DataViewManager.BaseTableId)
            {
                return true;
            }
            if (this._cache == null)
            {
                GetTableHits();
            }

            string tableName = (tableBO.DataBase + "." + tableBO.Name).ToUpper();

            HitsCompoundsPair pair;
            this._cache.TryGetValue(tableName, out pair);
            return pair.hits > 0;
        }

        public string TableDisplayName(SelectDataForm form, TableBO tableBO)
        {
            if (tableBO.ID == form.dataViewBO.DataViewManager.BaseTableId)
            {
                return tableBO.Alias;
            }
            if (this._cache == null)
            {
                GetTableHits();
            }

            string tableName = (tableBO.DataBase + "." + tableBO.Name).ToUpper();

            HitsCompoundsPair pair;
            this._cache.TryGetValue(tableName, out pair);
            return string.Format("{0} ({1} / {2})", tableBO.Alias, pair.hits, pair.compounds);
        }

        #endregion

        private void GetTableHits()
        {
            this._cache = new Dictionary<string, HitsCompoundsPair>();

            string connectionString = string.Format("Data Source={0};User Id={1};Password={2}", this._service.DataSourceName, "CSSADMIN", "CSSADMIN");
            OracleDatabase database = new OracleDatabase(connectionString);
            DbConnection connection = database.CreateConnection();
            connection.Open();

            string hitListTable = null;
            switch (this._hitListBO.HitListType)
            {
                case HitListType.TEMP:
                    hitListTable = "COETEMPHITLIST";
                    break;
                case HitListType.SAVED:
                    hitListTable = "COESAVEDHITLIST";
                    break;
            }

            string sql = string.Format(
                "select rs.table_name, sum(rs.num_results), count(distinct rs.ar_number) " +
                "from biodm.results_summary rs, biodm.reg_batch_view rbv, coedb.{0} h " +
                "where rs.ar_number = rbv.reg_batch and rbv.cpd_internal_id = h.id and h.hitlistid = {1} " +
                "group by rs.table_name", hitListTable, this._hitListBO.HitListID);

            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string tableName = reader.GetString(0).ToUpper();
                decimal totalResults = reader.GetDecimal(1);
                decimal distinctCompounds = reader.GetDecimal(2);
                HitsCompoundsPair pair = new HitsCompoundsPair();
                pair.hits = (int)totalResults;
                pair.compounds = (int)distinctCompounds;
                this._cache[tableName] = pair;
            }
        }
        */

        /*
        /// <summary>
        /// Builds a results criteria that selects the number of hitlist rows in every table.
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        private ResultsCriteria BuildResultsCriteria(COEDataViewManagerBO dataViewManager)
        {
            ResultsCriteria resultsCriteria = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable(dataViewManager.BaseTableId);
            TableBO baseTable = dataViewManager.Tables.GetTable(dataViewManager.BaseTableId);

            rcTable.Criterias.Add(BuildHitCount(dataViewManager.BaseTableId, baseTable.PrimaryKey));

            foreach (RelationshipBO relationship in dataViewManager.Relationships)
            {
                if (relationship.Parent == dataViewManager.BaseTableId)
                {
                    rcTable.Criterias.Add(BuildHitCount(relationship.Child, relationship.ChildKey));
                }
            }

            resultsCriteria.Tables.Add(rcTable);
            return resultsCriteria;
        }

        private ResultsCriteria.AggregateFunction BuildHitCount(int table, int tableKey)
        {
            ResultsCriteria.AggregateFunction agg = new ResultsCriteria.AggregateFunction();
            agg.Alias = table.ToString();
            agg.FunctionName = "COUNT";
            agg.Parameters.Add(new ResultsCriteria.Field(tableKey));
            return agg;
        }
        */
    }

    struct HitsCompoundsPair
    {
        public int hits;
        public int compounds;
    }
}

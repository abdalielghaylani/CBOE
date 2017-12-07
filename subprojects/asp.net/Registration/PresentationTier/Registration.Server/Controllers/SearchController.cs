using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class SearchController : RegControllerBase
    {      
        private static FormGroup GetFormGroup(bool? temp)
        {
            var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            configRegRecord.COEFormHelper.Load(formGroupType);
            return configRegRecord.FormGroup;
        }

        private static GenericBO GetGenericBO(bool? temp)
        {
            var formGroup = GetFormGroup(temp);
            GenericBO bo = GenericBO.GetGenericBO(Consts.CHEMBIOVIZAPLPICATIONNAME, formGroup);
            return bo;
        }

        private static QueryData GetHitlistQueryInternal(int id, bool? temp)
        {
            var hitlistBO = GetHitlistBO(id);
            if (hitlistBO.SearchCriteriaID == 0)
                throw new RegistrationException("The hit-list has no query associated with it");
            COESearchCriteriaBO searchCriteriaBO = COESearchCriteriaBO.Get(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
            if (searchCriteriaBO.SearchCriteria == null)
                throw new RegistrationException("No search criteria is associated with this hit-list");
            var formGroup = GetFormGroup(temp);
            return new QueryData(searchCriteriaBO.DataViewId != formGroup.Id, searchCriteriaBO.SearchCriteria.ToString());
        }

        private JObject GetHitlistRecordsInternal(int id, bool? temp, int? skip = null, int? count = null, string sort = null)
        {
            var hitlistBO = GetHitlistBO(id);
            return GetRegistryRecordsListView(temp, skip, count, sort, hitlistBO.HitListInfo);
        }

        private JObject SearchRecordsInternal(QueryData queryData, bool? temp, int? skip, int? count, string sort)
        {
            var searchCriteria = new SearchCriteria();
            string structureName = string.Empty;            
            try
            {
                searchCriteria.GetFromXML(queryData.SearchCriteria);
                SearchCriteria.SearchExpression itemToDelete = null;
                foreach (var item in searchCriteria.Items)
                {
                    if (item is SearchCriteria.SearchCriteriaItem)
                    {
                        var searchCriteriaItem = (SearchCriteria.SearchCriteriaItem)item;
                        var structureCriteria = searchCriteriaItem.Criterium as SearchCriteria.StructureCriteria;
                        if (structureCriteria != null)
                        {
                            var query = structureCriteria.Query4000;
                            if (!string.IsNullOrEmpty(query))
                            {
                                if (query.StartsWith("<"))
                                {
                                    var cdxData = ChemistryHelper.ConvertToCdxAndName(query, ref structureName, true);
                                    if (string.IsNullOrEmpty(cdxData)) itemToDelete = item;
                                    structureCriteria.Structure = cdxData;
                                }
                                else if (query.StartsWith("VmpD"))
                                {
                                    var cdxmlData = ChemistryHelper.ConvertToCdxmlAndName(query, ref structureName, true);
                                    if (string.IsNullOrEmpty(cdxmlData)) itemToDelete = item;
                                }
                            }
                        }
                    }
                }
                if (itemToDelete != null) searchCriteria.Items.Remove(itemToDelete);
            }
            catch (Exception ex)
            {
                throw new RegistrationException("The search criteria is invalid", ex);
            }

            try
            {
                return GetRegistryRecordsListView(temp, skip, count, sort, null, searchCriteria, queryData.HighlightSubStructures);
            }
            catch (Exception exc)
            {
                throw new RegistrationException("The search could not be performed", exc);
            }
        }

        private int CreateTempHitlist(QueryData queryData, bool? temp) 
        {
            var coeSearch = new COESearch();
            var dataViewId = int.Parse(temp != null && temp.Value ? ControlIdChangeUtility.TEMPSEARCHGROUPID : ControlIdChangeUtility.PERMSEARCHGROUPID);
            var dataView = SearchFormGroupAdapter.GetDataView(dataViewId);
            var searchCriteria = new SearchCriteria();
            string structureName = string.Empty;
            try
            {
                searchCriteria.GetFromXML(queryData.SearchCriteria);
                SearchCriteria.SearchExpression itemToDelete = null;
                foreach (var item in searchCriteria.Items)
                {
                    if (item is SearchCriteria.SearchCriteriaItem)
                    {
                        var searchCriteriaItem = (SearchCriteria.SearchCriteriaItem)item;
                        var structureCriteria = searchCriteriaItem.Criterium as SearchCriteria.StructureCriteria;
                        if (structureCriteria != null)
                        {
                            var query = structureCriteria.Query4000;
                            if (!string.IsNullOrEmpty(query))
                            {
                                if (query.StartsWith("<"))
                                {
                                    var cdxData = ChemistryHelper.ConvertToCdxAndName(query, ref structureName, true);
                                    if (string.IsNullOrEmpty(cdxData)) itemToDelete = item;
                                    structureCriteria.Structure = cdxData;
                                }
                                else if (query.StartsWith("VmpD"))
                                {
                                    var cdxmlData = ChemistryHelper.ConvertToCdxmlAndName(query, ref structureName, true);
                                    if (string.IsNullOrEmpty(cdxmlData)) itemToDelete = item;
                                }
                            }
                        }
                    }
                }
                if (itemToDelete != null) searchCriteria.Items.Remove(itemToDelete);
            }
            catch (Exception ex)
            {
                throw new RegistrationException("The search criteria is invalid", ex);
            }
            
            var hitlistInfo = coeSearch.GetHitList(searchCriteria, dataView);
            var hitlistBO = GetHitlistBO(hitlistInfo.HitListID);
            hitlistBO.SearchCriteriaID = searchCriteria.SearchCriteriaID;
            if (!string.IsNullOrEmpty(structureName))
            {
                var additionalCriteriaCount = searchCriteria.Items.Count - 1;
                var more = additionalCriteriaCount > 0 ? string.Format(" +{0}", additionalCriteriaCount) : string.Empty;
                var moreDesc = additionalCriteriaCount > 0 ? string.Format(" with {0} more criteria", additionalCriteriaCount) : string.Empty;
                hitlistBO.Name = string.Format("{0}{1}", structureName, more);
                hitlistBO.Description = string.Format("Search for {0}{1}", structureName, moreDesc);
            }
            hitlistBO.Update();

            return hitlistInfo.HitListID;
        }

        private COEDataView AddMolWt(COEDataView dataView)
        {
            int basetableid = dataView.Basetable;
            COEDataView resultDataview = new COEDataView();
            resultDataview = GetDataViewClone(dataView);
            resultDataview.Tables = new COEDataView.DataViewTableList();
            COEDataView basetableDataview = new COEDataView();
            COEDataView childtableDataview = new COEDataView();
            foreach (COEDataView.DataViewTable coeTable in dataView.Tables)
            {
                if (coeTable.Id == basetableid)
                {
                    basetableDataview.Tables.Add(GetTableNode(coeTable));
                }
                else
                {
                    childtableDataview.Tables.Add(GetTableNode(coeTable));
                }
            }
            resultDataview.Tables.AddRange(basetableDataview.Tables);
            resultDataview.Tables.AddRange(childtableDataview.Tables);

            return resultDataview;
        }

        private COEDataView GetDataViewClone(COEDataView dataview)
        {
            COEDataView newDataview = new COEDataView();
            newDataview.Application = dataview.Application;
            newDataview.Basetable = dataview.Basetable;

            newDataview.Database = dataview.Database;
            newDataview.DataViewHandling = dataview.DataViewHandling;
            newDataview.DataViewID = dataview.DataViewID;
            newDataview.Description = dataview.Description;
            newDataview.Name = dataview.Name;
            newDataview.Relationships = dataview.Relationships;
            newDataview.Tables = dataview.Tables;
            newDataview.XmlNs = dataview.XmlNs;
            return newDataview;
        }

        private COEDataView.DataViewTable GetTableNode(COEDataView.DataViewTable coeTable)
        {
            COEDataView.DataViewTable newCoeTable = new COEDataView.DataViewTable();
            newCoeTable = GetDVTableNode(coeTable);
            newCoeTable.Fields = new COEDataView.DataViewFieldList();
            for (int i = 0; i < coeTable.Fields.Count; i++)
            {
                COEDataView.Field xnode = new COEDataView.Field();
                xnode = coeTable.Fields[i];
                newCoeTable.Fields.Add(xnode);

                // only add the mw and mf virtual columns if we have reason to believe that this is a structure column
                if ((coeTable.Fields[i].IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) ||
                        (coeTable.Fields[i].MimeType == COEDataView.MimeTypes.CHEMICAL_X_CDX))
                {
                    newCoeTable.Fields.Add(GetFieldNode(xnode, "Mol Wt"));
                    newCoeTable.Fields.Add(GetFieldNode(xnode, "Mol Formula"));
                }
            }

            return newCoeTable;
        }

        private COEDataView.Field GetFieldNode(COEDataView.Field node, string alias)
        {
            COEDataView.Field newnode = new COEDataView.Field();
            newnode.Alias = alias;
            newnode.DataType = node.DataType;
            newnode.Id = node.Id;
            newnode.IndexType = node.IndexType;
            newnode.LookupDisplayFieldId = node.LookupDisplayFieldId;
            newnode.LookupFieldId = node.LookupFieldId;
            newnode.LookupSortOrder = node.LookupSortOrder;
            newnode.MimeType = node.MimeType;
            newnode.Name = node.Name;
            newnode.ParentTableId = node.ParentTableId;
            newnode.SortOrder = node.SortOrder;
            newnode.Visible = node.Visible;

            return newnode;
        }

        private COEDataView.DataViewTable GetDVTableNode(COEDataView.DataViewTable node)
        {
            COEDataView.DataViewTable newnode = new COEDataView.DataViewTable();
            newnode.Alias = node.Alias;
            newnode.Database = node.Database;
            newnode.Fields = node.Fields;
            newnode.Id = node.Id;
            newnode.IsView = node.IsView;
            newnode.Name = node.Name;
            newnode.PrimaryKey = node.PrimaryKey;

            return newnode;
        }

        private JArray GetExportTemplates(int dataViewId)
        {
            var exportTemplate = new JArray();
            exportTemplate.Add(new JObject(
                new JProperty("ID", 0),
                new JProperty("Name", "Default Template")));

            var templateList = COEExportTemplateBOList.GetUserTemplatesByDataViewId(dataViewId, COEUser.Name, true);
            foreach (var template in templateList)
            {
                exportTemplate.Add(new JObject(
                new JProperty("ID", template.ID),
                new JProperty("Name", template.Name)));
            }
            return exportTemplate;
        }

        /// <summary>
        /// Returns all hit-lists.
        /// </summary>
        /// <response code="200">Successful</response>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>A list of hit-list objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists")]
        [SwaggerOperation("GetHitlists")]
        [SwaggerResponse(200, type: typeof(List<HitlistData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetHitlists(bool? temp = null)
        {
            return await CallMethod(() =>
            {
                var result = new List<HitlistData>();
                var formGroup = GetFormGroup(temp);
                var tempHitLists = COEHitListBOList.GetRecentHitLists(Consts.REGDB, COEUser.Name, formGroup.Id, 10);
                foreach (var h in tempHitLists)
                {
                    result.Add(new HitlistData(h));
                }
                var savedHitLists = COEHitListBOList.GetSavedHitListList(Consts.REGDB, COEUser.Name, formGroup.Id);
                foreach (var h in savedHitLists)
                {
                    result.Add(new HitlistData(h));
                }
                return result;
            });
        }

        /// <summary>
        /// Deletes a hit-list
        /// </summary>
        /// <remarks>Deletes a hit-list by its ID</remarks>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="id">The ID of hit-list to delete</param>
        /// <returns>The <see cref="ResponseData"/> object containing the ID of deleted hit-list</returns>
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        [HttpDelete]
        [Route(Consts.apiPrefix + "hitlists/{id}")]
        [SwaggerOperation("DeleteHitlist")]
        public async Task<IHttpActionResult> DeleteHitlist(int id)
        {
            return await CallMethod(() =>
            {
                COEHitListBO.Delete(HitListType.TEMP, id);
                COEHitListBO.Delete(HitListType.SAVED, id);
                return new ResponseData(id: id);
            });
        }

        /// <summary>
        /// Update a hit-list
        /// </summary>
        /// <remarks>Update a hit-list by its ID
        /// If the given hit-list is found only in the temporary list but the type is specified as saved,
        /// it is considered as a request to save the temporary hit-list as a saved hit-list.
        /// </remarks>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="id">The ID of hit-list to update</param>
        /// <param name="hitlistData">The hit list data</param>
        /// <returns>The <see cref="ResponseData"/> object containing the ID of updated hit-list</returns>
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        [HttpPut]
        [Route(Consts.apiPrefix + "hitlists/{id}")]
        [SwaggerOperation("UpdateHitlist")]
        public async Task<IHttpActionResult> UpdateHitlist(int id, [FromBody] HitlistData hitlistData)
        {
            return await CallMethod(() =>
            {
                var hitlistType = hitlistData.HitlistType;
                var hitlistBO = COEHitListBO.Get(hitlistType, id);
                if (hitlistBO == null)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the hit-list for ID, {0}", id));
                bool saveHitlist = false;
                if (hitlistType == HitListType.SAVED && hitlistBO.HitListID == 0)
                {
                    // Saving temporary hit-list
                    saveHitlist = true;
                    hitlistBO = COEHitListBO.Get(HitListType.TEMP, id);
                }
                if (hitlistBO.HitListID > 0)
                {
                    hitlistBO.Name = hitlistData.Name;
                    hitlistBO.Description = hitlistData.Description;
                    hitlistBO.IsPublic = hitlistData.IsPublic.HasValue ? hitlistData.IsPublic.Value : false;
                    if (hitlistBO.SearchCriteriaID > 0)
                    {
                        var searchCriteriaBO = COESearchCriteriaBO.Get(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
                        searchCriteriaBO.Name = hitlistBO.Name;
                        searchCriteriaBO.Description = hitlistBO.Description;
                        searchCriteriaBO.IsPublic = hitlistBO.IsPublic;
                        searchCriteriaBO = saveHitlist ? searchCriteriaBO.Save() : searchCriteriaBO.Update();
                        if (saveHitlist)
                        {
                            hitlistBO.SearchCriteriaID = searchCriteriaBO.ID;
                            hitlistBO.SearchCriteriaType = searchCriteriaBO.SearchCriteriaType;
                        }
                    }
                    if (saveHitlist)
                    {
                        var idToDelete = hitlistBO.ID;
                        hitlistBO = hitlistBO.Save();
                        COEHitListBO.Delete(HitListType.TEMP, idToDelete);
                    }
                    else
                    {
                        hitlistBO = hitlistBO.Update();
                    }
                }
                return new ResponseData(id: hitlistBO.ID);
            });
        }

        /// <summary>
        /// Create a hit-list
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>A <see cref="ResponseData" /> object containing the ID of the created hit-list/></returns>
        [HttpPost]
        [Route(Consts.apiPrefix + "hitlists")]
        [SwaggerOperation("CreateHitlist")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateHitlist(bool? temp = null)
        {
            return await CallMethod(() =>
            {
                var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
                var formGroup = GetFormGroup(temp);
                var genericBO = GenericBO.GetGenericBO("Registration", formGroup.Id);
                string name = hitlistData["Name"].ToString();
                bool isPublic = Convert.ToBoolean(hitlistData["IsPublic"].ToString());
                string dscription = hitlistData["Description"].ToString();
                string userID = COEUser.Name;
                int numHits = Convert.ToInt32(hitlistData["NumHits"].ToString());
                string databaseName = "REGDB";
                int hitListID = Convert.ToInt32(hitlistData["HitListID"].ToString());
                int dataViewID = formGroup.DataViewId;
                HitListType hitlistType = (HitListType)(int)hitlistData["HitlistType"];
                int searchcriteriaId = Convert.ToInt32(hitlistData["SearchcriteriaId"].ToString());
                string searchcriteriaType = hitlistData["SearchcriteriaType"].ToString();
                CambridgeSoft.COE.Framework.COEHitListService.DAL dal = null;
                var id = dal.CreateNewTempHitList(name, isPublic, dscription, userID, numHits, databaseName, hitListID, dataViewID, hitlistType, searchcriteriaId, searchcriteriaType);
                return new ResponseData(id: id);
            });
        }

        /// <summary>
        /// Returns the list of registry records for a hit-list
        /// </summary>
        /// <remarks>Returns the list of registry records for a hit-list by its ID</remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="id">The hit-list ID</param>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <param name="refresh">The flag to refresh the query results by re-running the search query</param>
        /// <param name="skip">The number of items to skip</param>
        /// <param name="count">The maximum number of items to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The promise to return a JSON object containing an array of registration records</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id}/records")]
        [SwaggerOperation("GetHitlistRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetHitlistRecords(int id, bool? temp = null, bool? refresh = null, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                if (refresh != null && refresh.Value)
                {
                    var queryData = GetHitlistQueryInternal(id, temp);
                    return SearchRecordsInternal(queryData, temp, skip, count, sort);
                }
                return GetHitlistRecordsInternal(id, temp, skip, count, sort);
            });
        }

        /// <summary>
        /// Returns the list of registry records for a hit-list
        /// </summary>
        /// <remarks>Returns the list of registry records for a hit-list by its ID</remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="id">The hit-list ID</param>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>The object containing the search criteria as XML string</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id}/query")]
        [SwaggerOperation("GetHitlistQuery")]
        [SwaggerResponse(200, type: typeof(QueryData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetHitlistQuery(int id, bool? temp = null)
        {
            return await CallMethod(() =>
            {
                var queryData = GetHitlistQueryInternal(id, temp);
                var searchCriteria = new SearchCriteria();
                searchCriteria.GetFromXML(queryData.SearchCriteria);
                foreach (var item in searchCriteria.Items)
                {
                    if (!(item is SearchCriteria.SearchCriteriaItem)) continue;
                    var searchCriteriaItem = (SearchCriteria.SearchCriteriaItem)item;
                    var structureCriteria = searchCriteriaItem.Criterium as SearchCriteria.StructureCriteria;
                    if (structureCriteria == null) continue;
                    var query = structureCriteria.Query4000;
                    if (!string.IsNullOrEmpty(query) && query.StartsWith("VmpD"))
                    {
                        structureCriteria.Structure = SecurityElement.Escape(ChemistryHelper.ConvertToCdxml(query, true));
                    }
                }
                queryData.SearchCriteria = searchCriteria.ToString();
                return queryData;
            });
        }

        /// <summary>
        /// Returns a hit-list by its ID
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="id1">The ID of the hit-list to use as the base</param>
        /// <param name="op">The name of operation to apply</param>
        /// <param name="id2">The ID of the hit-list to apply the operation</param>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <param name="skip">The number of records to skip</param>
        /// <param name="count">The maximum number of records to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The list of hit-list objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id1}/{op}/{id2}/records")]
        [SwaggerOperation("GetAdvHitlistRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetAdvHitlistRecords(int id1, string op, int id2, bool? temp = null, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                JObject data = new JObject();
                var formGroup = GetFormGroup(temp);
                int dataViewId = formGroup.DataViewId;
                var hitlistBO1 = GetHitlistBO(id1);
                if (hitlistBO1 == null)
                    throw new RegistrationException(string.Format("The hit-list identified by ID {0} is invalid", id1));
                var hitlistBO2 = GetHitlistBO(id2);
                if (hitlistBO2 == null)
                    throw new RegistrationException(string.Format("The hit-list identified by ID {0} is invalid", id2));
                char symbol;
                string join;
                COEHitListBO newHitlist;
                switch (op)
                {
                    case "intersect":
                        symbol = 'x';
                        join = "intersecting with";
                        newHitlist = COEHitListOperationManager.IntersectHitList(hitlistBO1.HitListInfo, hitlistBO2.HitListInfo, dataViewId);
                        break;
                    case "subtract":
                        symbol = '-';
                        join = "subtracted by";
                        newHitlist = COEHitListOperationManager.SubtractHitLists(hitlistBO1.HitListInfo, hitlistBO2.HitListInfo, dataViewId);
                        break;
                    default: // union
                        join = "combining with";
                        symbol = '+';
                        newHitlist = COEHitListOperationManager.UnionHitLists(hitlistBO1.HitListInfo, hitlistBO2.HitListInfo, dataViewId);
                        break;
                }
                string hitListName = string.Format("({0}) {1} ({2})", hitlistBO1.Name, symbol, hitlistBO2.Name);
                string hitListDescription = string.Format("{0} {1} {2}", hitlistBO1.Description, join, hitlistBO2.Description);
                newHitlist.Name = hitListName.Substring(0, Math.Min(hitListName.Length, 50));
                newHitlist.Description = hitListDescription.Substring(0, Math.Min(hitListDescription.Length, 250));
                newHitlist.Update();
                return GetHitlistRecordsInternal(newHitlist.HitListID, temp);
            });
        }

        /// <summary>
        /// Save a hit-list
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>The count of marked hits</returns>
        [HttpPost]
        [Route(Consts.apiPrefix + "markedhits")]
        [SwaggerOperation("MarkedHitsSave")]
        public int MarkedHitsSave(bool? temp = null)
        {
            CheckAuthentication();
            var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
            var formGroup = GetFormGroup(temp);
            var genericBO = GenericBO.GetGenericBO("Registration", formGroup.Id);
            var hitlistBO = genericBO.MarkedHitList;
            hitlistBO.Name = hitlistData["Name"].ToString();
            hitlistBO.Description = hitlistData["Description"].ToString();
            hitlistBO.HitListType = HitListType.SAVED;
            hitlistBO.Save();
            int markedCount = genericBO.GetMarkedCount();
            return markedCount;
        }

        /// <summary>
        /// Returns a hit-list by its ID
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="id">The ID of the hit-list that needs to be fetched</param>
        /// <returns>The matching hit-list</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id}")]
        [SwaggerOperation("GetHitlist")]
        [SwaggerResponse(200, type: typeof(HitlistData))]
        public async Task<IHttpActionResult> GetHitlist(int id)
        {
            return await CallMethod(() =>
            {
                var hitlistBO = GetHitlistBO(id);
                return new HitlistData(hitlistBO);
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "search/records")]
        [SwaggerOperation("SearchRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> SearchRecords([FromBody] QueryData queryData, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                return CreateTempHitlist(queryData, false);
                // return SearchRecordsInternal(queryData, null, skip, count, sort);
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "search/temp-records")]
        [SwaggerOperation("SearchTempRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> SearchTempRecords([FromBody] QueryData queryData, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                return CreateTempHitlist(queryData, true);
            });
        }

        /// <summary>
        /// Returns the list of registry records for a hit-list
        /// </summary>
        /// <remarks>Returns the list of registry records for a hit-list by its ID</remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="id">The hit-list ID</param>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <param name="skip">The number of items to skip</param>
        /// <param name="count">The maximum number of items to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The promise to return a JSON object containing an array of registration records</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id}/export")]
        [SwaggerOperation("ExportHitlistRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> ExportHitlistRecords(int id, bool? temp = null, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                return GetHitlistRecordsInternal(id, temp, skip, count, sort);
            });
        }

        /// <summary>
        /// Returns the exported file according to the specified file type for a hit-list
        /// </summary>
        /// <remarks>Returns the exported file according to the specified file type</remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="id">The ID of the hit-list to be exported to the SD File</param>
        /// <param name="exportType">The export type expected</param>
        /// <param name="resultsCriteriaTableData">The selected fields to be included in the export</param>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>The exported file for the matching hit-list</returns>
        [HttpPost]
        [Route(Consts.apiPrefix + "hitlists/{id}/export/{exportType}")]
        [SwaggerOperation("ExportHitlist")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public HttpResponseMessage ExportHitlist(int id, string exportType, List<ResultsCriteriaTableData> resultsCriteriaTableData, bool? temp = null)
        {
            CheckAuthentication();
            var formGroup = GetFormGroup(temp);
            var hitlistBO = GetHitlistBO(id);

            var coex = new COEExport();

            var pagingInfo = new PagingInfo()
            {
                HitListID = hitlistBO.HitListID,
                RecordCount = hitlistBO.HitListInfo.RecordCount
            };

            var resultsCriteria = new ResultsCriteria();
            foreach (var criteriaTableData in resultsCriteriaTableData)
            {
                var isStructureIndex = !string.IsNullOrEmpty(criteriaTableData.IndexType) && criteriaTableData.IndexType.ToUpper() == "CS_CARTRIDGE";
                var isStructureMimeType = !string.IsNullOrEmpty(criteriaTableData.MimeType) && criteriaTableData.MimeType.ToUpper() == "CHEMICAL_X_CDX";
                var isStructureColumn = isStructureIndex || isStructureMimeType;

                var resultsCriteriaTable = resultsCriteria.Tables.Find(t => t.Id == criteriaTableData.TableId);
                if (resultsCriteriaTable == null)
                {
                    resultsCriteriaTable = new ResultsCriteria.ResultsCriteriaTable()
                    {
                        Id = criteriaTableData.TableId,
                        Criterias = new List<ResultsCriteria.IResultsCriteriaBase>()
                    };
                    resultsCriteria.Tables.Add(resultsCriteriaTable);
                }

                if (!criteriaTableData.Alias.ToLower().Contains("mol wt") && !criteriaTableData.Alias.ToLower().Contains("mol formula"))
                {
                    var field = new ResultsCriteria.Field
                    {
                        Id = criteriaTableData.FieldId,
                        Alias = criteriaTableData.Alias,
                        Visible = criteriaTableData.Visible
                    };

                    resultsCriteriaTable.Criterias.Add(field);
                }

                if (isStructureColumn && criteriaTableData.Alias.ToLower().Contains("mol wt"))
                {
                    ResultsCriteria.MolWeight molWeightCriteria = new ResultsCriteria.MolWeight();
                    molWeightCriteria.Alias = criteriaTableData.Alias;
                    molWeightCriteria.Id = criteriaTableData.FieldId;
                    molWeightCriteria.Visible = criteriaTableData.Visible;
                    resultsCriteriaTable.Criterias.Add(molWeightCriteria);
                }

                if (isStructureColumn && criteriaTableData.Alias.ToLower().Contains("mol formula"))
                {
                    ResultsCriteria.Formula formulaCriteria = new ResultsCriteria.Formula();
                    formulaCriteria.Alias = criteriaTableData.Alias;
                    formulaCriteria.Id = criteriaTableData.FieldId;
                    formulaCriteria.Visible = criteriaTableData.Visible;
                    resultsCriteriaTable.Criterias.Add(formulaCriteria);
                }
            }

            string exportedData = coex.GetData(resultsCriteria, pagingInfo, formGroup.Id, exportType);
            // CSBR-138818 Replacing <sub> with null while export.
            if (exportedData != null)
                exportedData = exportedData.Replace("<sub>", string.Empty).Replace("</sub>", string.Empty);

            try
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(exportedData);
                    writer.Flush();
                    stream.Position = 0;

                    var httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(stream.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", string.Format("Exported{0}HitsFromDV{1}.sdf", hitlistBO.HitListInfo.RecordCount, formGroup.Id));
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("chemical/x-mdl-sdfile");
                    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = "ExportedSDFfile.sdf";
                    httpResponseMessage.StatusCode = System.Net.HttpStatusCode.OK;

                    return httpResponseMessage;
                }
            }
            catch (IOException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Return the results criteria for search
        /// </summary>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <param name="templateId">The selected template</param>
        /// <returns>The promise to return a JSON object containing an array of results criteria</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/resultsCriteria")]
        [SwaggerOperation("GetResultsCriteria")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetResultsCriteria(bool? temp = null, int? templateId = null)
        {
            return await CallMethod(() =>
            {
                GenericBO bo = GetGenericBO(temp);
                COEDataView dataView = new COEDataView();
                dataView.GetFromXML(AddMolWt(bo.DataView).ToString());
                dataView.RemoveNonRelationalTables();

                XmlDocument templateCriterias = new XmlDocument();
                COEExportTemplateBO exportTemplateBo = templateId.HasValue ? COEExportTemplateBO.Get(templateId.Value) : null;
                if (exportTemplateBo != null && exportTemplateBo.ResultCriteria != null)
                {
                    templateCriterias.Load(new StringReader(exportTemplateBo.ResultCriteria.ToString()));
                }

                var resultsCriteria = new JArray();
                foreach (var dataViewTable in dataView.Tables)
                {
                    foreach (var dataViewField in dataViewTable.Fields)
                    {
                        var visible = false;
                        var fieldAlias = dataViewField.Alias;
                        string xmlNamespace = "COE";
                        string xmlNSres = "COE.ResultsCriteria";
                        XmlNamespaceManager managerRes = new XmlNamespaceManager(new NameTable());
                        managerRes.AddNamespace(xmlNamespace, xmlNSres);
                        XmlNode templateFeildNode = templateCriterias.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:field[@fieldId='" + dataViewField.Id + "']", managerRes);

                        var isStructureIndex = dataViewField.IndexType.ToString().ToUpper() == "CS_CARTRIDGE";
                        var isStructureMimeType = dataViewField.MimeType.ToString().ToUpper() == "CHEMICAL_X_CDX";
                        var isStructureColumn = isStructureIndex || isStructureMimeType;

                        if (isStructureColumn && templateId == null)
                        {
                            visible = true; // if is default template, structure columns should be selected by default
                        }

                        if (templateFeildNode != null)
                        {
                            visible = true;
                            if (!isStructureColumn)
                                fieldAlias = templateFeildNode.Attributes["alias"].Value;
                        }

                        var key = string.Format("{0}{1}", dataViewField.Id, Regex.Replace(fieldAlias, @"\s", string.Empty)).ToLower();
                        resultsCriteria.Add(new JObject(
                            new JProperty("key", key),
                            new JProperty("tableId", dataViewTable.Id),
                            new JProperty("tableName", dataViewTable.Alias),
                            new JProperty("fieldId", dataViewField.Id),
                            new JProperty("fieldName", fieldAlias),
                            new JProperty("visible", visible),
                            new JProperty("indexType", dataViewField.IndexType.ToString()),
                            new JProperty("mimeType", dataViewField.MimeType.ToString())));
                    }
                }

                return resultsCriteria;
            });
        }

        /// <summary>
        /// Return the export templates for the current user
        /// </summary>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected error</response>
        /// <param name="temp">The flag indicating whether or not it is for temporary records (default: false)</param>
        /// <returns>The promise to return a JSON object containing an array of export templates</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "exportTemplates")]
        [SwaggerOperation("GetExportTemplates")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetExportTemplates(bool? temp = null)
        {
            return await CallMethod(() =>
            {
                GenericBO bo = GetGenericBO(temp);
                return GetExportTemplates(bo.DataView.DataViewID);
            });
        }
    }
}

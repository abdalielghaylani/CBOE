using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json;
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

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class SearchController : RegControllerBase
    {
        private const string databaseName = "REGDB";

        private static COEHitListBO GetHitlistBO(int id)
        {
            var hitlistBO = COEHitListBO.Get(HitListType.TEMP, id);
            if (hitlistBO == null || hitlistBO.HitListID == 0)
                hitlistBO = COEHitListBO.Get(HitListType.SAVED, id);
            if (hitlistBO == null || hitlistBO.HitListID == 0)
                throw new IndexOutOfRangeException(string.Format("Cannot find the hit-list for ID, {0}", id));
            return hitlistBO;
        }

        private static QueryData GetHitlistQueryInternal(int id, bool? temp)
        {
            var hitlistBO = GetHitlistBO(id);
            if (hitlistBO.SearchCriteriaID == 0)
                throw new RegistrationException("The hit-list has no query associated with it");
            COESearchCriteriaBO searchCriteriaBO = COESearchCriteriaBO.Get(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
            if (searchCriteriaBO.SearchCriteria == null)
                throw new RegistrationException("No search criteria is associated with this hit-list");
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
            configRegRecord.COEFormHelper.Load(formGroupType);
            var formGroup = configRegRecord.FormGroup;
            return new QueryData(searchCriteriaBO.DataViewId != formGroup.Id, searchCriteriaBO.SearchCriteria.ToString());
        }

        private JObject GetHitlistRecordsInternal(int id, int? skip = null, int? count = null, string sort = null)
        {
            var hitlistType = HitListType.TEMP;
            var hitlistBO = COEHitListBO.Get(hitlistType, id);
            if (hitlistBO == null)
                throw new RegistrationException("Cannot instantiate the hit-list object");
            if (hitlistBO.HitListID == 0) hitlistType = HitListType.SAVED;
            var tableName = "vw_mixture_regnumber";
            var whereClause = string.Format(" WHERE mixtureid in (SELECT ID FROM COEDB.{0} WHERE hitlistId=:hitlistId)",
                hitlistType == HitListType.TEMP ? "coetemphitlist" : "coesavedhitlist");
            var query = GetQuery(tableName + whereClause, RecordColumns, sort, "modified", "regid");
            var args = new Dictionary<string, object>();
            args.Add(":hitlistId", id);
            return new JObject(
                new JProperty("temporary", false),
                new JProperty("hitlistId", id),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName + whereClause, args))),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("rows", ExtractData(query, args, skip, count))
            );
        }

        private JObject SearchRecordsInternal(QueryData queryData, int? skip, int? count, string sort)
        {
            var coeSearch = new COESearch();
            var dataView = SearchFormGroupAdapter.GetDataView(int.Parse(ControlIdChangeUtility.PERMSEARCHGROUPID));
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
            var hitlistInfo = coeSearch.GetPartialHitList(searchCriteria, dataView);
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
            return GetHitlistRecordsInternal(hitlistBO.HitListID, skip, count, sort);
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
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
                configRegRecord.COEFormHelper.Load(formGroupType);
                var formGroup = configRegRecord.FormGroup;
                var tempHitLists = COEHitListBOList.GetRecentHitLists(databaseName, COEUser.Name, formGroup.Id, 10);
                foreach (var h in tempHitLists)
                {
                    result.Add(new HitlistData(h));
                }
                var savedHitLists = COEHitListBOList.GetSavedHitListList(databaseName, COEUser.Name, formGroup.Id);
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
                CambridgeSoft.COE.Framework.COEHitListService.DAL objDAL = null;
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
                configRegRecord.COEFormHelper.Load(formGroupType);
                var formGroup = configRegRecord.FormGroup;
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
                var id = objDAL.CreateNewTempHitList(name, isPublic, dscription, userID, numHits, databaseName, hitListID, dataViewID, hitlistType, searchcriteriaId, searchcriteriaType);
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
                    return SearchRecordsInternal(queryData, skip, count, sort);
                }
                return GetHitlistRecordsInternal(id, skip, count, sort);
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
                return GetHitlistQueryInternal(id, temp);
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
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
                configRegRecord.COEFormHelper.Load(formGroupType);
                var formGroup = configRegRecord.FormGroup;
                int dataViewId = formGroup.Id;
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
                newHitlist.Name = string.Format("({0}) {1} ({2})", hitlistBO1.Name, symbol, hitlistBO2.Name);
                newHitlist.Description = string.Format("{0} {1} {2}", hitlistBO1.Description, join, hitlistBO2.Description);
                newHitlist.Update();
                return GetHitlistRecordsInternal(newHitlist.HitListID);
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
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            var formGroupType = temp != null && temp.Value ? COEFormHelper.COEFormGroups.SearchTemporary : COEFormHelper.COEFormGroups.SearchPermanent;
            configRegRecord.COEFormHelper.Load(formGroupType);
            var formGroup = configRegRecord.FormGroup;
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
                return SearchRecordsInternal(queryData, skip, count, sort);
            });
        }
    }
}

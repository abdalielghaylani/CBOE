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

        private JObject GetHitlistRecordsInternal(int id, int? skip = null, int? count = null, string sort = null)
        {
            var hitlistType = HitListType.TEMP;
            var hitlistBO = COEHitListBO.Get(hitlistType, id);
            // This is an error condition.
            // it might be better to throw an exception here.
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

        private static COEHitListBO GetHitlistBO(int id)
        {
            var hitlistBO = COEHitListBO.Get(HitListType.TEMP, id);
            if (hitlistBO == null)
                hitlistBO = COEHitListBO.Get(HitListType.SAVED, id);
            if (hitlistBO == null)
                throw new IndexOutOfRangeException(string.Format("Cannot find the hit-list for ID, {0}", id));
            return hitlistBO;
        }

        /// <summary>
        /// Returns all hit-lists.
        /// </summary>
        /// <response code="200">Successful</response>
        /// <returns>A list of hit-list objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists")]
        [SwaggerOperation("GetHitlists")]
        [SwaggerResponse(200, type: typeof(List<Hitlist>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetHitlists()
        {
            return await CallMethod(() =>
            {
                var result = new List<Hitlist>();
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
                var formGroup = configRegRecord.FormGroup;
                var tempHitLists = COEHitListBOList.GetRecentHitLists(databaseName, COEUser.Name, formGroup.Id, 10);
                foreach (var h in tempHitLists)
                {
                    result.Add(new Hitlist(h.ID, h.HitListID, h.HitListType, h.NumHits, h.IsPublic, h.SearchCriteriaID, h.SearchCriteriaType, h.Name, h.Description, h.MarkedHitListIDs, h.DateCreated));
                }
                var savedHitLists = COEHitListBOList.GetSavedHitListList(databaseName, COEUser.Name, formGroup.Id);
                foreach (var h in savedHitLists)
                {
                    result.Add(new Hitlist(h.ID, h.HitListID, h.HitListType, h.NumHits, h.IsPublic, h.SearchCriteriaID, h.SearchCriteriaType, h.Name, h.Description, h.MarkedHitListIDs, h.DateCreated));
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
        /// <returns>The <see cref="ResponseData"/> object containing the ID of updated hit-list</returns>
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        [HttpPut]
        [Route(Consts.apiPrefix + "hitlists/{id}")]
        [SwaggerOperation("UpdateHitlist")]
        public async Task<IHttpActionResult> UpdateHitlist(int id)
        {
            return await CallMethod(() =>
            {
                var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
                var hitlistType = (HitListType)(int)hitlistData["HitlistType"];
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
                    hitlistBO.Name = hitlistData["Name"].ToString();
                    hitlistBO.Description = hitlistData["Description"].ToString();
                    hitlistBO.IsPublic = (bool)hitlistData["IsPublic"];
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
                    hitlistBO = saveHitlist ? hitlistBO.Save() : hitlistBO.Update();
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
        /// <returns>A <see cref="ResponseData" /> object containing the ID of the created hit-list/></returns>
        [HttpPost]
        [Route(Consts.apiPrefix + "hitlists")]
        [SwaggerOperation("CreateHitlist")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateHitlist()
        {
            return await CallMethod(() =>
            {
                var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
                CambridgeSoft.COE.Framework.COEHitListService.DAL objDAL = null;
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
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
        public async Task<IHttpActionResult> GetHitlistRecords(int id, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
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
        /// <returns>The promise to return a JSON object containing an array of registration records</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id}/query")]
        [SwaggerOperation("GetHitlistQuery")]
        [SwaggerResponse(200, type: typeof(SimpleData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetHitlistQuery(int id)
        {
            return await CallMethod(() =>
            {
                var hitlistBO = GetHitlistBO(id);
                if (hitlistBO.SearchCriteriaID == 0)
                    throw new RegistrationException("The hit-list has no query associated with it");
                COESearchCriteriaBO searchCriteriaBO = COESearchCriteriaBO.Get(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
                var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
                var formGroup = configRegRecord.FormGroup;
                SearchCriteria searchCriteria = SearchFormGroupAdapter.GetSearchCriteria(formGroup.QueryForms[0]);
                foreach (var item in searchCriteriaBO.SearchCriteria.Items)
                {
                    if (item is SearchCriteria.SearchCriteriaItem)
                    {
                        var searchCriteriaItem = (SearchCriteria.SearchCriteriaItem)item;
                        if (searchCriteriaItem.ID >= 0)
                            SearchFormGroupAdapter.PopulateSearchCriteria(searchCriteria, searchCriteriaItem);
                    }
                }
                return new SimpleData(searchCriteria.ToString());
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
        /// <param name="skip">The number of records to skip</param>
        /// <param name="count">The maximum number of records to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The list of hit-list objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "hitlists/{id1}/{op}/{id2}/records")]
        [SwaggerOperation("GetAdvHitlistRecords")]
        public JObject GetAdvHitlistRecords(int id1, int op, int id2, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            JObject data = new JObject();
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
            var formGroup = configRegRecord.FormGroup;
            int dataViewID = formGroup.Id;
            switch (op)
            {
                case 1: // intersect      
                   // resultHitListID = objDAL.IntersectHitLists(id1, hitListID1Type, id2, hitListID2Type, dbName, dataViewID);
                   // objJArray = ExtractData("select vw_mixture_regnumber.regid as id, vw_mixture_regnumber.name," +
                   // "vw_mixture_regnumber.created, vw_mixture_regnumber.modified, vw_mixture_regnumber.personcreated as creator, 'record/' || vw_mixture_regnumber.regid || '?' || to_char(vw_mixture_regnumber.modified, 'YYYYMMDDHH24MISS') as structure, vw_mixture_regnumber.regnumber, vw_mixture_regnumber.statusid as status, vw_mixture_regnumber.approved FROM regdb.vw_mixture_regnumber,regdb.vw_batch vw_batch " +
                   // "where vw_mixture_regnumber.mixtureid in (select id from coedb.coetemphitlist s where s.hitlistid=" + resultHitListID + ") and vw_batch.regid = vw_mixture_regnumber.regid");
                    break;
                case 2: // subtract
                   // resultHitListID = objDAL.SubtractHitLists(id1, hitListID1Type, id2, hitListID2Type, dbName, dataViewID);
                   // objJArray = ExtractData("select vw_mixture_regnumber.regid as id, vw_mixture_regnumber.name," +
                   // "vw_mixture_regnumber.created, vw_mixture_regnumber.modified, vw_mixture_regnumber.personcreated as creator, 'record/' || vw_mixture_regnumber.regid || '?' || to_char(vw_mixture_regnumber.modified, 'YYYYMMDDHH24MISS') as structure, vw_mixture_regnumber.regnumber, vw_mixture_regnumber.statusid as status, vw_mixture_regnumber.approved FROM regdb.vw_mixture_regnumber,regdb.vw_batch vw_batch " +
                   // "where vw_mixture_regnumber.mixtureid in (select id from coedb.coetemphitlist s where s.hitlistid=" + resultHitListID + ") and vw_batch.regid = vw_mixture_regnumber.regid");
                    break;
                case 3: // union
                   // resultHitListID = objDAL.SubtractHitLists(id1, hitListID1Type, id2, hitListID2Type, dbName, dataViewID);
                   // objJArray = ExtractData("select vw_mixture_regnumber.regid as id, vw_mixture_regnumber.name," +
                   // "vw_mixture_regnumber.created, vw_mixture_regnumber.modified, vw_mixture_regnumber.personcreated as creator, 'record/' || vw_mixture_regnumber.regid || '?' || to_char(vw_mixture_regnumber.modified, 'YYYYMMDDHH24MISS') as structure, vw_mixture_regnumber.regnumber, vw_mixture_regnumber.statusid as status, vw_mixture_regnumber.approved FROM regdb.vw_mixture_regnumber,regdb.vw_batch vw_batch " +
                   // "where vw_mixture_regnumber.mixtureid in (select id from coedb.coetemphitlist s where s.hitlistid=" + resultHitListID + ") and vw_batch.regid = vw_mixture_regnumber.regid");
                    break;
                default: // Subtract from entire list
                    data = GetHitlistRecordsInternal(id1);
                    break;
            }

            return data;
        }

        /// <summary>
        /// Save a hit-list
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        /// <returns>The count of marked hits</returns>
        [HttpPost]
        [Route(Consts.apiPrefix + "markedhits")]
        [SwaggerOperation("MarkedHitsSave")]
        public int MarkedHitsSave()
        {
            CheckAuthentication();
            var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
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
        [SwaggerOperation("SearchHitlistsIdGet")]
        [SwaggerResponse(200, type: typeof(Hitlist))]
        public async Task<IHttpActionResult> SearchHitlistsIdGet(int id)
        {
            return await CallMethod(() =>
            {
                var hitlistBO = GetHitlistBO(id);
                return new Hitlist(hitlistBO);
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "search/records")]
        [SwaggerOperation("SearchRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> SearchRecords([FromBody] JObject criteria, int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                var coeSearch = new COESearch();
                var dataView = SearchFormGroupAdapter.GetDataView(int.Parse(ControlIdChangeUtility.PERMSEARCHGROUPID));
                var searchCriteria = new SearchCriteria();
                try
                {
                    searchCriteria.GetFromXML((string)criteria["searchCriteria"]);
                }
                catch (Exception ex)
                {
                    throw new RegistrationException("The search criteria is invalid", ex);
                }
                var hitlistInfo = coeSearch.GetPartialHitList(searchCriteria, dataView);
                var hitlistBO = GetHitlistBO(hitlistInfo.HitListID);
                hitlistBO.SearchCriteriaID = searchCriteria.SearchCriteriaID;
                hitlistBO.Update();
                return GetHitlistRecordsInternal(hitlistBO.HitListID, skip, count, sort);
            });
        }

    }
}

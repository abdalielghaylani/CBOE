using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Registration.Services;
using PerkinElmer.COE.Registration.Server.Code;
using Microsoft.Web.Http;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.Registration;
using System.Net;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class BatchController : RegControllerBase
    {
        /// <summary>
        /// Returns the list of all batches
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="0">Unexpected error</response>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}/records")]
        [SwaggerOperation("GetBatchs")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetBatchs(int? id = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "batches";
            var whereClause = " WHERE FULLREGNUMBER=:fullregNumber";
            var query = GetQuery(tableName + whereClause, RecordColumns, sort, "TEMPBATCHID", "BATCH_INTERNAL_ID");
            var args = new Dictionary<string, object>();
            args.Add(":fullregNumber", id);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("batch", false),
                new JProperty("fullregNumber", id),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName + whereClause, args))),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("rows", ExtractData(query, args, skip, count))
            ));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        // TODO: Add comments for summary, remarks, and response codes.
        // Response code should include
        // - 404 for id not found error
        // - 400 for bad request
        // etc.
        // TODO: Return type should be properly defined in Models directory.
        // Refer to Hitlist model as an example.
        // Swagger code-gen can be used to generate model objects.
        // The model does not have to be complete up-front.
        // Add just enough members such as ID to get started.
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("GetBatch")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public JArray GetBatch(int id)
        {
            CheckAuthentication();
            JArray data = new JArray();
            string fullregNumber = CambridgeSoft.COE.Registration.Services.Types.Batch.GetBatch(id).RegNumber.ToString();
            return ExtractData("SELECT BATCH_INTERNAL_ID, TEMPBATCHID, BATCH_NUMBER, FULLREGNUMBER, DATECREATED, ENTRY_PERSON_ID, BATCH_REG_PERSON_ID, PERSONAPPROVED, LAST_MOD_DATE, STATUS_ID FROM REGDB.batches WHERE FULLREGNUMBER=" + fullregNumber + "");
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("CreateBatch")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public void CreateBatch(JObject batch)
        {
            CheckAuthentication();
            var hitlistData = Request.Content.ReadAsAsync<JObject>().Result;
            Batch batchNew = new Batch();
            batchNew.ID = (int)hitlistData["id"];
            batchNew.TempBatchID = (int)hitlistData["tempBatchID"];
            batchNew.BatchNumber = (int)hitlistData["batchNumber"];
            batchNew.FullRegNumber = (string)hitlistData["fullRegNumber"];
            batchNew.DateCreated = (DateTime)hitlistData["dateCreated"];
            batchNew.PersonCreated = (int)hitlistData["personCreated"];
            batchNew.PersonRegistered = (int)hitlistData["personRegistered"];
            batchNew.PersonApproved = (int)hitlistData["personApproved"];
            batchNew.DateLastModified = (DateTime)hitlistData["dateLastModified"];
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("UpdateBatch")]
        public void UpdateBatch(int id)
        {
            CheckAuthentication();
            //return Ok(string.Format("The batch #{0} was updated successfully!", id));
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batchs/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(string))]
        public void DeleteBatch(int id)
        {
            CheckAuthentication();
            CambridgeSoft.COE.Registration.Services.Types.Batch.DeleteBatch(id);
        }
    }
}

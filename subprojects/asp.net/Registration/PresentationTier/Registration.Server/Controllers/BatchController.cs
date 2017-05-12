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
using System.Xml;
using Newtonsoft.Json;
using CambridgeSoft.COE.Registration.Access;
using System.Data.Common;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class BatchController : RegControllerBase
    {
        private RegistrationOracleDAL regoraDal = null;

        private RegistrationOracleDAL RegOraDal
        {
            get
            {
                if (regoraDal == null)
                    DalUtils.GetRegistrationDAL(ref regoraDal, CambridgeSoft.COE.Registration.Constants.SERVICENAME);
                return regoraDal;
            }
        }

        /// <summary>
        /// Returns the list of all batches
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="0">Unexpected error</response>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("GetAllBatches")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public JArray GetAllBatches()
        {
            CheckAuthentication();
            JArray data = new JArray();
            return ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH");
        }

        /// <summary>
        /// Returns the list of all batches by RegID
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="0">Unexpected error</response>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}/records")]
        [SwaggerOperation("GetBatches")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetBatches(int? id = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "VW_BATCH";
            var whereClause = " WHERE REGID=:regId";
            var query = GetQuery(tableName + whereClause, BatchColumns, sort, "BATCHID", "TEMPBATCHID");
            var args = new Dictionary<string, object>();
            args.Add(":regId", id);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("batch", false),
                new JProperty("regId", id),
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
            return ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH WHERE BATCHID=" + id + "");
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("CreateBatch")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public void CreateBatch(JObject data)
        {
            CheckAuthentication();
            XmlDocument datatoXml = JsonConvert.DeserializeXmlNode(data.ToString());
            RegOraDal.InsertRegistryRecordTemporary(datatoXml.InnerXml);
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("UpdateBatch")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public void UpdateBatch(String regType, JObject data)
        {
            CheckAuthentication();
            string dalresponseXml = string.Empty;
            XmlDocument datatoXml = JsonConvert.DeserializeXmlNode(data.ToString());
            if (regType == "Temp")
            {
                RegOraDal.UpdateRegistryRecordTemporary(datatoXml.InnerXml, out dalresponseXml);
            }
            else if (regType == "Perm")
            {
                RegOraDal.UpdateRegistryRecord(datatoXml.InnerXml, CambridgeSoft.COE.Registration.DuplicateCheck.CompoundCheck, out dalresponseXml);
            }
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(string))]
        public void DeleteBatch(int id)
        {
            CheckAuthentication();
            CambridgeSoft.COE.Registration.Services.Types.Batch.DeleteBatch(id);
        }
    }
}

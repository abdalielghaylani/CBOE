using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class BatchController : RegControllerBase
    {
        protected static RecordColumn[] BatchRecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn { Definitions = "BATCHID", Sortable = true },
                    new RecordColumn { Definitions = "TEMPBATCHID", Sortable = true }, 
                    new RecordColumn { Definitions = "BATCHNUMBER", Sortable = true }, 
                    new RecordColumn { Definitions = "FULLREGNUMBER", Sortable = true }, 
                    new RecordColumn { Definitions = "DATECREATED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONCREATED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONREGISTERED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONAPPROVED", Sortable = true }, 
                    new RecordColumn { Definitions = "DATELASTMODIFIED", Sortable = true }
                };
            }
        }

        /// <summary>
        /// Returns the list of all batches
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <returns>A promise to return an <see cref="IHttpActionResult"/> object</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("GetAllBatches")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetAllBatches()
        {
            return await CallMethod(() =>
           {             
               var data = ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH");
               if (data.Count() == 0)
                   throw new IndexOutOfRangeException(string.Format("Cannot find the Batches."));
               return data;
           });
        }

        /// <summary>
        /// Returns the list of all batches for the specified registration ID
        /// </summary>
        /// <response code="200">Successful operation</response>
        /// <param name="id">The ID of the registration record</param>
        /// <param name="skip">The number of items to skip</param>
        /// <param name="count">The maximum number of items to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The promise to return an <see cref="IHttpActionResult"/> object</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}/records")]
        [SwaggerOperation("GetBatches")]
        [SwaggerResponse(200, type: typeof(List<JObject>))]
        public async Task<IHttpActionResult> GetBatches(int? id = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "VW_BATCH";
            var whereClause = " WHERE REGID=:regId";
            var query = GetQuery(tableName + whereClause, BatchRecordColumns, sort, "BATCHID", "TEMPBATCHID");
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

        /// <summary>
        /// Returns the list of all batches by BatchID
        /// </summary>
        /// <param name="id">Batch ID</param>
        /// <returns>The array of batch objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("GetBatch")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(404, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetBatch(int id)
        {
            return await CallMethod(() =>
           {
               var args = new Dictionary<string, object>();
               args.Add(":Id", id);
               var data = ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH WHERE BATCHID=:Id", args);
               if (data.Count() == 0)
                   throw new IndexOutOfRangeException(string.Format("Cannot find the batch number, {0}", id));
               return data;
           });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("CreateBatch")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateBatch(JObject data)
        {
            return await CallServiceMethod((service) =>
            {
                CheckAuthentication();
                XmlDocument datatoXml = JsonConvert.DeserializeXmlNode(data.ToString());
                var resultString = RegDal.InsertRegistryRecordTemporary(datatoXml.InnerXml);
                return new ResponseData(Convert.ToInt32(resultString));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("UpdateBatch")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        public async Task<IHttpActionResult> UpdateBatch(string regType, JObject data)
        {
            return await CallMethod(() =>
            {              
                string message = string.Empty;
                XmlDocument datatoXml = JsonConvert.DeserializeXmlNode(data.ToString(Newtonsoft.Json.Formatting.None));
                if (regType == "Temp")
                {
                    RegDal.UpdateRegistryRecordTemporary(datatoXml.InnerXml, out message);
                }
                else if (regType == "Perm")
                {
                    RegDal.UpdateRegistryRecord(datatoXml.InnerXml, CambridgeSoft.COE.Registration.DuplicateCheck.CompoundCheck, out message);
                }

                return new ResponseData(null, null, message, null);
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteBatch(int id)
        {
            CheckAuthentication();
            CambridgeSoft.COE.Registration.Services.Types.Batch.DeleteBatch(id);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, string.Format("The component #{0} was deleted successfully!", id));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}

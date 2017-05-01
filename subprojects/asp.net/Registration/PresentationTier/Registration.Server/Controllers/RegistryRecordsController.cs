using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services;
using PerkinElmer.COE.Registration.Server.Code;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;
using System.Net;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class RegistryRecordsController : RegControllerBase
    {
        #region Permanent Records
        [HttpGet]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("GetRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetRecords(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "vw_mixture_regnumber";
            var query = GetQuery(tableName, RecordColumns, sort, "modified", "regid");
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("temporary", false),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                new JProperty("rows", ExtractData(query, null, skip, count))
            ));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "records/{id}")]
        public dynamic GetRecord(int id)
        {
            return null;
        }
        #endregion // Permanent Records

        #region Temporary Records
        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("GetTemp")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetTemp(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "vw_temporarycompound";
            var query = GetQuery(tableName, TempRecordColumns, sort, "datelastmodified", "tempcompoundid");
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("temporary", true),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                new JProperty("rows", ExtractData(query, null, skip, count))
            ));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        public dynamic GetTemp(int id)
        {
            using (var service = new COERegistrationServices())
            {
                service.Credentials.AuthenticationTicket = GetSessionToken();
                return service.RetrieveTemporaryRegistryRecord(id);
            }
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("DeleteTemp")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteTemp(int id)
        {
            using (var service = new COERegistrationServices())
            {
                service.Credentials.AuthenticationTicket = GetSessionToken();
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK, service.DeleteTemporaryRegistryRecord(id));
                return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
            }
        }
        #endregion // Tempoary Records
    }
}

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
using System.Xml;
using Newtonsoft.Json;

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
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IHttpActionResult> GetRecords(int? skip = null, int? count = null, string sort = null)
        {
            return await CallGetMethod(() =>
            {
                var tableName = "vw_mixture_regnumber";
                var query = GetQuery(tableName, RecordColumns, sort, "modified", "regid");
                return new JObject(
                    new JProperty("temporary", false),
                    new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                    new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                    new JProperty("rows", ExtractData(query, null, skip, count))
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("GetRecord")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(404, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetRecord(int id)
        {
            return await CallServiceMethod((service) =>
            {
                string record;
                if (id < 0)
                {
                    record = service.RetrieveNewRegistryRecord();
                }
                else
                {
                    var args = new Dictionary<string, object>();
                    args.Add(":id", id);
                    var regNum = (string)ExtractValue("SELECT regnumber FROM vw_mixture_regnumber WHERE regid=:id", args);
                    if (string.IsNullOrEmpty(regNum))
                        throw new IndexOutOfRangeException();
                    record = service.RetrieveRegistryRecord(regNum);
                }
                var recordXml = new XmlDocument();
                recordXml.LoadXml(record);
                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("CreateRecord")]
        [SwaggerResponse(201, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateRecord(JObject record)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)record["data"]);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var resultString = service.CreateRegistryRecord(recordString, "N").Replace("ReturnList", "data");
                doc.LoadXml(resultString);
                var errorNode = doc.SelectSingleNode("//ErrorMessage");
                if (errorNode != null)
                    throw new RegistrationException(errorNode.InnerText);
                return JObject.Parse(JsonConvert.SerializeXmlNode(doc));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("UpdateRecord")]
        public void UpdateRecord(int id)
        {
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("DeleteRecord")]
        [SwaggerResponse(200, type: typeof(string))]
        public void DeleteRecord(int id)
        {
        }

        #endregion // Permanent Records

        #region Temporary Records
        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("GetTempRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(string))]
        [SwaggerResponse(500, type: typeof(string))]
        public async Task<IHttpActionResult> GetTempRecords(int? skip = null, int? count = null, string sort = null)
        {
            return await CallGetMethod(() =>
            {
                var tableName = "vw_temporarycompound";
                var query = GetQuery(tableName, TempRecordColumns, sort, "datelastmodified", "tempcompoundid");
                return new JObject(
                    new JProperty("temporary", true),
                    new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                    new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                    new JProperty("rows", ExtractData(query, null, skip, count))
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("GetTempRecord")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(404, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetTempRecord(int id)
        {
            return await CallServiceMethod((service) =>
            {
                var args = new Dictionary<string, object>();
                args.Add(":id", id);
                var count = Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) FROM vw_temporarycompound WHERE tempcompoundid=:id", args));
                if (count == 0)
                    throw new IndexOutOfRangeException();
                var record = service.RetrieveTemporaryRegistryRecord(id);
                var recordXml = new XmlDocument();
                recordXml.LoadXml(record);
                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("CreateTempRecord")]
        [SwaggerResponse(201, type: typeof(string))]
        public async Task<IHttpActionResult> CreateTempRecord(JObject record)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)record["data"]);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var resultString = service.CreateTemporaryRegistryRecord(recordString);
                return new JObject(new JProperty("data", new JObject(new JProperty("id", Convert.ToInt32(resultString)))));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("UpdateTempRecord")]
        public void UpdateTempRecord(int id)
        {
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("DeleteTempRecord")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteTempRecord(int id)
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

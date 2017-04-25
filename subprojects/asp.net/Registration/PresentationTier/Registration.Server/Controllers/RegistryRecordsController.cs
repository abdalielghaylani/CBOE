using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegistryRecordsController : RegControllerBase
    {
        #region Permanent Records
        [Route(Consts.apiPrefix + "records")]
        public JObject Get(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "vw_mixture_regnumber";
            var query = GetQuery(tableName, RecordColumns, sort, "modified", "regid");
            return new JObject(
                new JProperty("temporary", false),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                new JProperty("rows", ExtractData(query, null, skip, count))
            );
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "records/{id}")]
        public dynamic Get(int id)
        {
            return null;
        }
        #endregion // Permanent Records

        #region Temporary Records
        [Route(Consts.apiPrefix + "temp-records")]
        public JObject GetTemp(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "vw_temporarycompound";
            var query = GetQuery(tableName, TempRecordColumns, sort, "datelastmodified", "tempcompoundid");
            return new JObject(
                new JProperty("temporary", true),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                new JProperty("rows", ExtractData(query, null, skip, count))
            );
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
        public Task<HttpResponseMessage> DeleteTemp(int id)
        {
            using (var service = new COERegistrationServices())
            {
                service.Credentials.AuthenticationTicket = GetSessionToken();
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(service.DeleteTemporaryRegistryRecord(id));
                return Task.FromResult(response);
            }
        }
        #endregion // Tempoary Records
    }
}

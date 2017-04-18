using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegistryRecordsController : RegControllerBase
    {
        #region Permanent Records
        [Route("api/RegistryRecords")]
        public JObject Get(int? skip = null, int? count = null, string sort = null)
        {
            var records = new JArray();
            // Default sorting order is descending order by modified date
            if (string.IsNullOrEmpty(sort)) sort = "modified DESC";
            // Make the sorting unique
            if (!sort.ToLower().Contains("regid")) sort += ", regid";
            string query = string.Format(
                "SELECT regid id, name, created, modified, personcreated as creator, " +
                    "'record/' || regid || '?' || to_char(modified, 'YYYYMMDDHH24MISS') as structure, " +
                    "regnumber, statusid as status, approved " +
                "FROM vw_mixture_regnumber a " +
                "ORDER BY {0}", sort);
            return new JObject(
                new JProperty("temporary", false),
                new JProperty("rows", ExtractData(query, null, skip, count)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM vw_mixture_regnumber")))
            );
        }

        public dynamic Get(int id)
        {
            return null;
        }
        #endregion // Permanent Records

        #region Temporary Records
        [Route("api/RegistryRecords/Temp")]
        public JObject GetTemp(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var query = "SELECT tempcompoundid id, tempbatchid batchid, formulaweight MW, molecularformula MF, datecreated created, datelastmodified modified, personcreated as creator, 'temprecord/' || tempcompoundid || '?' || to_char(datelastmodified, 'YYYYMMDDHH24MISS') as structure FROM vw_temporarycompound ORDER BY tempbatchid DESC";
            return new JObject(
                new JProperty("temporary", true),
                new JProperty("rows", ExtractData(query, null, skip, count)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM vw_temporarycompound")))
            );
        }

        [HttpGet]
        [Route("api/RegistryRecords/Temp/{id}")]
        public dynamic GetTemp(int id)
        {
            using (var service = new COERegistrationServices())
            {
                service.Credentials.AuthenticationTicket = GetSessionToken();
                return service.RetrieveTemporaryRegistryRecord(id);
            }
        }

        [HttpDelete]
        [Route("api/RegistryRecords/Temp/{id}")]
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

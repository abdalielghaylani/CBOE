using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegistryRecordsController : RegControllerBase
    {
        private class DataColumn
        {
            public string definition;
            public string label;
            public bool sortable;
        }

        private static string CleanupSortTerm(DataColumn[] columns, string sortTerm)
        {
            const string sortDesc = " desc";
            const string sortAsc = " asc";
            var desc = sortTerm.EndsWith(sortDesc);
            var t = sortTerm.Replace(sortDesc, string.Empty).Replace(sortAsc, string.Empty).Trim();
            var dc = columns.FirstOrDefault(c => c.definition.Equals(t));
            if (dc == null) dc = columns.FirstOrDefault(c => c.label != null && c.label.Equals(t));
            t = dc == null ? string.Empty : dc.definition;
            if (desc) t += sortDesc;
            return t;
        }

        #region Permanent Records
        [Route("api/records")]
        public JObject Get(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var columns = new DataColumn[]
            {
                new DataColumn{ definition = "regid", label = "id", sortable = true },
                new DataColumn{ definition = "name", sortable = true },
                new DataColumn{ definition = "created", sortable = true },
                new DataColumn{ definition = "modified", sortable = true },
                new DataColumn{ definition = "personcreated", label ="creator", sortable = true },
                new DataColumn{ definition = "'record/' || regid || '?' || to_char(modified, 'YYYYMMDDHH24MISS')", label = "structure", sortable = false },
                new DataColumn{ definition = "regnumber", sortable = true },
                new DataColumn{ definition = "statusid", label = "status", sortable = true },
                new DataColumn{ definition = "approved", sortable = true }
            };
            var selectTerms = String.Join(", ", columns.Select(c => c.definition + (c.label != null ? " " + c.label : string.Empty)));
            // Default sorting order is descending order by modified date
            if (string.IsNullOrEmpty(sort)) sort = string.Empty;
            var sortTerms = String.Join(", ", sort.ToLower().Split(new char[] { ',' }).Select(t => t.Trim())
                .Select(t => CleanupSortTerm(columns, t)).Where(t => !string.IsNullOrEmpty(t)));
            if (string.IsNullOrEmpty(sortTerms)) sortTerms = "modified desc";
            // Make the sorting unique
            if (!sortTerms.Contains("regid")) sort += ", regid";
            string query = string.Format("SELECT {0} FROM vw_mixture_regnumber a ORDER BY {1}", selectTerms, sortTerms);
            return new JObject(
                new JProperty("temporary", false),
                new JProperty("rows", ExtractData(query, null, skip, count)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM vw_mixture_regnumber")))
            );
        }

        [HttpGet]
        [Route("api/records/{id}")]
        public dynamic Get(int id)
        {
            return null;
        }
        #endregion // Permanent Records

        #region Temporary Records
        [Route("api/temp-records")]
        public JObject GetTemp(int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var columns = new DataColumn[]
            {
                new DataColumn{ definition = "tempcompoundid", label = "id", sortable = true },
                new DataColumn{ definition = "tempbatchid", label = "batchid", sortable = true },
                new DataColumn{ definition = "formulaweight", label = "mw", sortable = true },
                new DataColumn{ definition = "molecularformula", label = "mf", sortable = true },
                new DataColumn{ definition = "datecreated", label = "created", sortable = true },
                new DataColumn{ definition = "datelastmodified", label = "modified", sortable = true },
                new DataColumn{ definition = "personcreated", label ="creator", sortable = true },
                new DataColumn{ definition = "'temprecord/' || tempcompoundid || '?' || to_char(datelastmodified, 'YYYYMMDDHH24MISS')", label = "structure", sortable = false }
            };
            var selectTerms = String.Join(", ", columns.Select(c => c.definition + (c.label != null ? " " + c.label : string.Empty)));
            // Default sorting order is descending order by modified date
            if (string.IsNullOrEmpty(sort)) sort = string.Empty;
            var sortTerms = String.Join(", ", sort.ToLower().Split(new char[] { ',' }).Select(t => t.Trim())
                .Select(t => CleanupSortTerm(columns, t)).Where(t => !string.IsNullOrEmpty(t)));
            if (string.IsNullOrEmpty(sortTerms)) sortTerms = "datelastmodified desc";
            // Make the sorting unique
            if (!sortTerms.Contains("tempcompoundid")) sort += ", tempcompoundid";
            string query = string.Format("SELECT {0} FROM vw_temporarycompound a ORDER BY {1}", selectTerms, sortTerms);
            return new JObject(
                new JProperty("temporary", true),
                new JProperty("rows", ExtractData(query, null, skip, count)),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM vw_temporarycompound")))
            );
        }

        [HttpGet]
        [Route("api/temp-records/{id}")]
        public dynamic GetTemp(int id)
        {
            using (var service = new COERegistrationServices())
            {
                service.Credentials.AuthenticationTicket = GetSessionToken();
                return service.RetrieveTemporaryRegistryRecord(id);
            }
        }

        [HttpDelete]
        [Route("api/temp-records/{id}")]
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

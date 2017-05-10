using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Csla.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COETableEditorService;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration.Services;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegControllerBase : ApiController
    {
        private const string sortDesc = " desc";
        private const string sortAsc = " asc";
        private RegistrationOracleDAL regDal = null;

        private RegistrationOracleDAL RegDal
        {
            get
            {
                if (regDal == null)
                    DalUtils.GetRegistrationDAL(ref regDal, CambridgeSoft.COE.Registration.Constants.SERVICENAME);
                return regDal;
            }
        }

        protected SafeDataReader GetReader(string sql, Dictionary<string, object> args = null)
        {
            if (args == null) args = new Dictionary<string, object>();
            return RegDal.GetPicklist(sql, args);
        }

        protected JArray ExtractData(SafeDataReader reader)
        {
            var data = new JArray();
            var fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                var row = new JObject();
                for (int i = 0; i < fieldCount; ++i)
                {
                    var fieldName = reader.GetName(i);
                    // Skip row_num field if any
                    if (fieldName.Equals("row_num", StringComparison.OrdinalIgnoreCase))
                        continue;
                    var fieldType = reader.GetFieldType(i);
                    object fieldData;
                    switch (fieldType.Name.ToLower())
                    {
                        case "int16":
                        case "int32":
                            fieldData = reader.GetInt32(i);
                            break;
                        case "datetime":
                            fieldData = reader.GetDateTime(i);
                            break;
                        case "decimal":
                            fieldData = (double)reader.GetDecimal(i);
                            break;
                        default:
                            fieldData = reader.GetString(i);
                            break;
                    }
                    row.Add(new JProperty(fieldName, fieldData));
                }
                data.Add(row);
            }
            return data;
        }

        protected object ExtractValue(string sql, Dictionary<string, object> args = null)
        {
            using (var reader = GetReader(sql, args))
            {
                object value = null;
                while (reader.Read())
                {
                    var fieldType = reader.GetFieldType(0);
                    switch (fieldType.Name.ToLower())
                    {
                        case "int16":
                        case "int32":
                            value = reader.GetInt32(0);
                            break;
                        case "datetime":
                            value = reader.GetDateTime(0);
                            break;
                        case "decimal":
                            value = reader.GetDecimal(0);
                            break;
                        default:
                            value = reader.GetString(0);
                            break;
                    }
                }
                return value;
            }
        }

        protected JArray ExtractData(string sql, Dictionary<string, object> args = null, int? skip = null, int? count = null)
        {
            if ((skip != null && skip.Value > 0) || count != null)
            {
                int lowerLimit = Math.Max(skip == null ? 0 : skip.Value, 0);
                int upperLimit = Math.Max(count == null ? lowerLimit + 1 : lowerLimit + count.Value, lowerLimit + 1);
                sql = string.Format("SELECT ROWNUM row_num, q1.* FROM ({0}) q1 WHERE ROWNUM <= :upperLimt", sql);
                if (args == null) args = new Dictionary<string, object>();
                args.Add(":upperLimit", upperLimit);
                if (lowerLimit > 0)
                {
                    sql = string.Format("SELECT q2.* FROM ({0}) q2 WHERE row_num > :lowerLimit", sql);
                    args.Add(":lowerLimit", lowerLimit);
                }
            }
            using (var reader = GetReader(sql, args))
            {
                return ExtractData(reader);
            }
        }

        protected string GetSessionToken()
        {
            CookieHeaderValue cookie = Request.Headers.GetCookies(Consts.ssoCookieName).FirstOrDefault();
            return cookie != null ? cookie[Consts.ssoCookieName].Value : string.Empty;
        }

        protected void CheckAuthentication()
        {
            string sessionToken = GetSessionToken();
            if (string.IsNullOrEmpty(sessionToken) || !COEPrincipal.Login(sessionToken, true))
                throw new AuthenticationException();
        }

        protected async Task<IHttpActionResult> CallGetMethod(Func<object> method)
        {
            HttpResponseMessage responseMessage;
            try
            {
                CheckAuthentication();
                var tableList = new JArray();
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, method());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateErrorResponse(
                    ex is AuthenticationException ?
                    HttpStatusCode.Unauthorized :
                    HttpStatusCode.InternalServerError, ex);
            }
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        protected async Task<IHttpActionResult> CallServiceMethod(Func<COERegistrationServices, JObject> method)
        {
            HttpResponseMessage responseMessage;
            try
            {
                using (var service = new COERegistrationServices())
                {
                    service.Credentials.AuthenticationTicket = GetSessionToken();
                    responseMessage = Request.CreateResponse(HttpStatusCode.OK, method(service));
                }
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateErrorResponse(
                    ex is AuthenticationException ?
                    HttpStatusCode.Unauthorized :
                    ex is RegistrationException ?
                    HttpStatusCode.BadRequest :
                    ex is IndexOutOfRangeException ?
                    HttpStatusCode.NotFound :
                    HttpStatusCode.InternalServerError, ex);
            }
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        public static string GetAbsoluteUrl(string relativeUrl, bool globalScope = false)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (!globalScope)
            {
                if (relativeUrl.StartsWith("/"))
                    relativeUrl = relativeUrl.Insert(0, "~");
                if (!relativeUrl.StartsWith("~/"))
                    relativeUrl = relativeUrl.Insert(0, "~/");
                relativeUrl = VirtualPathUtility.ToAbsolute(relativeUrl);
            }

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, relativeUrl);
        }

        protected class RecordColumn
        {
            public string definition;
            public string label;
            public bool sortable;
        }

        protected static string CleanupSortTerm(RecordColumn[] columns, string sortTerm)
        {
            var desc = sortTerm.EndsWith(sortDesc);
            var t = sortTerm.Replace(sortDesc, string.Empty).Replace(sortAsc, string.Empty).Trim();
            var dc = columns.FirstOrDefault(c => c.definition.Equals(t));
            if (dc == null) dc = columns.FirstOrDefault(c => c.label != null && c.label.Equals(t));
            t = dc == null ? string.Empty : dc.definition;
            if (desc) t += sortDesc;
            return t;
        }

        protected static RecordColumn[] RecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn{ definition = "regid", label = "id", sortable = true },
                    new RecordColumn{ definition = "name", sortable = true },
                    new RecordColumn{ definition = "created", sortable = true },
                    new RecordColumn{ definition = "modified", sortable = true },
                    new RecordColumn{ definition = "personcreated", label ="creator", sortable = true },
                    new RecordColumn{ definition = "'record/' || regid || '?' || to_char(modified, 'YYYYMMDDHH24MISS')", label = "structure", sortable = false },
                    new RecordColumn{ definition = "regnumber", sortable = true },
                    new RecordColumn{ definition = "statusid", label = "status", sortable = true },
                    new RecordColumn{ definition = "approved", sortable = true }
                };
            }
        }

        protected static RecordColumn[] TempRecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn{ definition = "tempcompoundid", label = "id", sortable = true },
                    new RecordColumn{ definition = "tempbatchid", label = "batchid", sortable = true },
                    new RecordColumn{ definition = "formulaweight", label = "mw", sortable = true },
                    new RecordColumn{ definition = "molecularformula", label = "mf", sortable = true },
                    new RecordColumn{ definition = "datecreated", label = "created", sortable = true },
                    new RecordColumn{ definition = "datelastmodified", label = "modified", sortable = true },
                    new RecordColumn{ definition = "personcreated", label ="creator", sortable = true },
                    new RecordColumn{ definition = "'temprecord/' || tempcompoundid || '?' || to_char(datelastmodified, 'YYYYMMDDHH24MISS')", label = "structure", sortable = false }
                };
            }
        }

        protected static RecordColumn[] BatchColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn{ definition = "batchid", label = "id", sortable = true },
                    new RecordColumn{ definition = "tempbatchid", label = "tempbatchid", sortable = true },
                    new RecordColumn{ definition = "batchnumber", label = "batchnumber", sortable = true },
                    new RecordColumn{ definition = "fullregnumber", label = "fullregnumber", sortable = true },
                    new RecordColumn{ definition = "datecreated", label = "created", sortable = true },
                    new RecordColumn{ definition = "personcreated", label = "personcreated", sortable = true },
                    new RecordColumn{ definition = "personregistered", label = "personregistered", sortable = true },
                    new RecordColumn{ definition = "personapproved", label = "personapproved", sortable = true },
                    new RecordColumn{ definition = "datelastmodified", label = "modified", sortable = true }
                };
            }
        }

        protected static string GetSelectTerms(RecordColumn[] columns)
        {
            return String.Join(", ", columns.Select(c => c.definition + (c.label != null ? " " + c.label : string.Empty)));
        }

        protected static string GetSortTerms(RecordColumn[] columns, string sortTerms, string defaultColumn, string uniqueColumn)
        {
            // Default sorting order is descending order by modified date
            if (string.IsNullOrEmpty(sortTerms)) sortTerms = string.Empty;
            sortTerms = String.Join(", ", sortTerms.ToLower().Split(new char[] { ',' }).Select(t => t.Trim())
                .Select(t => CleanupSortTerm(columns, t)).Where(t => !string.IsNullOrEmpty(t)));
            if (string.IsNullOrEmpty(sortTerms)) sortTerms = defaultColumn + sortDesc;
            // Make the sorting unique
            if (!sortTerms.Contains(uniqueColumn)) sortTerms += ", " + uniqueColumn;
            return sortTerms;
        }

        protected static string GetQuery(string tableName, RecordColumn[] columns, string sortTerms, string defaultColumn, string uniqueColumn)
        {
            return string.Format("SELECT {0} FROM {1} ORDER BY {2}", GetSelectTerms(columns), tableName, GetSortTerms(columns, sortTerms, defaultColumn, uniqueColumn));
        }
    }
}
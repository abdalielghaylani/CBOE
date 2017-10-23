using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using Csla.Data;
using Newtonsoft.Json.Linq;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegControllerBase : ApiController
    {
        private const string sortDesc = " desc";
        private const string sortAsc = " asc";
        private RegistrationOracleDAL regDal = null;

        private HttpResponseMessage CreateErrorResponse(Exception ex)
        {
            var message = ex is Csla.DataPortalException ? ((Csla.DataPortalException)ex).BusinessException.Message : ex.Message;
            var statusCode = ex is AuthenticationException || ex is PrivilegeNotHeldException || ex is UnauthorizedAccessException ?
                HttpStatusCode.Unauthorized :
                ex is RegistrationException || ex is Csla.DataPortalException ?
                HttpStatusCode.BadRequest :
                ex is IndexOutOfRangeException ?
                HttpStatusCode.NotFound :
                HttpStatusCode.InternalServerError;
            return string.IsNullOrEmpty(message) ? Request.CreateErrorResponse(statusCode, ex) : Request.CreateErrorResponse(statusCode, message, ex);
        }

        protected RegistrationOracleDAL RegDal
        {
            get
            {
                if (regDal == null)
                    DalUtils.GetRegistrationDAL(ref regDal, CambridgeSoft.COE.Registration.Constants.SERVICENAME);
                return regDal;
            }
        }

        protected COEIdentity UserIdentity
        {
            get
            {
                var sessionToken = GetSessionToken();
                var args = new object[] { sessionToken };
                var identity = (COEIdentity)typeof(COEIdentity).GetMethod(
                    "GetIdentity",
                    BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(string) },
                    null
                ).Invoke(null, args);
                return identity;
            }
        }

        protected Dictionary<string, List<string>> UserPrivileges
        {
            get
            {
                return typeof(COEIdentity).GetField("_appRolePrivileges", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UserIdentity) as Dictionary<string, List<string>>;
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

        protected JArray ExtractPickListDomain()
        {
            var args = new Dictionary<string, object>();
            args.Add(":loggedInUser", UserIdentity.ID);
            var pickListDomains = ExtractData("SELECT * FROM VW_PICKLISTDOMAIN");
            foreach (var pickListDomain in pickListDomains)
            {
                var picklistNameValueList = PickListNameValueList.GetPickListNameValueList(Convert.ToInt32(pickListDomain["ID"]), true, null);

                var list = new JArray();
                foreach (string key in picklistNameValueList.KeyValueList.Keys)
                {
                    var item = new JObject(new JProperty("key", Convert.ToInt32(key)), new JProperty("value", picklistNameValueList.KeyValueList[key]));
                    list.Add(item);
                }
                pickListDomain["data"] = list;
            }
            return pickListDomains;
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
                throw new AuthenticationException(string.IsNullOrEmpty(sessionToken) ? "Empty token" : "Invalid token: " + sessionToken);
        }

        protected void CheckAuthorizations(string[] permissions)
        {
            bool isAuthorized = false;
            foreach (string permission in permissions)
            {
                isAuthorized = Csla.ApplicationContext.User.IsInRole(permission);
                if (!isAuthorized)
                    break;
            }

            if (!isAuthorized)
                throw new PrivilegeNotHeldException("Not allowed to execute " + string.Join(",", permissions));
        }

        protected async Task<IHttpActionResult> CallMethod(Func<object> method, string[] permissions = null, [CallerMemberName] string memberName = "", CacheControlHeaderValue cacheControl = null)
        {
            HttpResponseMessage responseMessage;
            try
            {
                try
                {
                    CheckAuthentication();
                }
                catch
                {
                    // Sometimes authentication fails due to multi-threading issue.
                    // This is a hack to overcome the short-comings of the current Registration codebase
                    // that does not handle multiple simultaneous calls well.
                    CheckAuthentication();
                }
                if (permissions != null) CheckAuthorizations(permissions);
                var statusCode = Request.Method == HttpMethod.Post && memberName.StartsWith("Create") ? HttpStatusCode.Created : HttpStatusCode.OK;
                responseMessage = Request.CreateResponse(statusCode, method());
                if (cacheControl != null)
                    responseMessage.Headers.CacheControl = cacheControl;

            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
                Logger.Error(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        protected async Task<IHttpActionResult> CallServiceMethod(Func<COERegistrationServices, object> method)
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
                Logger.Error(ex);
                responseMessage = CreateErrorResponse(ex);
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
            var port = url.Port != 80 ? (":" + url.Port) : string.Empty;

            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, relativeUrl);
        }

        protected class RecordColumn
        {
            public string Definitions { get; set; }

            public string Label { get; set; }

            public bool Sortable { get; set; }
        }

        protected static string CleanupSortTerm(RecordColumn[] columns, string sortTerm)
        {
            var desc = sortTerm.EndsWith(sortDesc);
            var t = sortTerm.Replace(sortDesc, string.Empty).Replace(sortAsc, string.Empty).Trim();
            var dc = columns.FirstOrDefault(c => c.Definitions.Equals(t));
            if (dc == null) dc = columns.FirstOrDefault(c => c.Label != null && c.Label.Equals(t));
            t = dc == null ? string.Empty : dc.Definitions;
            if (desc) t += sortDesc;
            return t;
        }

        protected static RecordColumn[] RecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn { Definitions = "regid", Label = "id", Sortable = true },
                    new RecordColumn { Definitions = "name", Sortable = true },
                    new RecordColumn { Definitions = "created", Sortable = true },
                    new RecordColumn { Definitions = "modified", Sortable = true },
                    new RecordColumn { Definitions = "personcreated", Label = "creator", Sortable = true },
                    new RecordColumn { Definitions = "'record/' || regid || '?' || to_char(modified, 'YYYYMMDDHH24MISS')", Label = "structure", Sortable = false },
                    new RecordColumn { Definitions = "regnumber", Sortable = true },
                    new RecordColumn { Definitions = "statusid", Label = "status", Sortable = true },
                    new RecordColumn { Definitions = "approved", Sortable = true }
                };
            }
        }

        protected static RecordColumn[] TempRecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn { Definitions = "tempcompoundid", Label = "id", Sortable = true },
                    new RecordColumn { Definitions = "c.tempbatchid", Label = "batchid", Sortable = true },
                    new RecordColumn { Definitions = "formulaweight", Label = "mw", Sortable = true },
                    new RecordColumn { Definitions = "molecularformula", Label = "mf", Sortable = true },
                    new RecordColumn { Definitions = "c.datecreated", Label = "created", Sortable = true },
                    new RecordColumn { Definitions = "c.datelastmodified", Label = "modified", Sortable = true },
                    new RecordColumn { Definitions = "c.personcreated", Label = "creator", Sortable = true },
                    new RecordColumn { Definitions = "'temprecord/' || tempcompoundid || '?' || to_char(c.datelastmodified, 'YYYYMMDDHH24MISS')", Label = "structure", Sortable = false },
                    new RecordColumn { Definitions = "b.statusid", Label = "statusid", Sortable = false }
                };
            }
        }

        protected static string GetSelectTerms(RecordColumn[] columns)
        {
            return string.Join(", ", columns.Select(c => c.Definitions + (c.Label != null ? " " + c.Label : string.Empty)));
        }

        protected static string GetSortTerms(RecordColumn[] columns, string sortTerms, string defaultColumn, string uniqueColumn)
        {
            // Default sorting order is descending order by modified date
            if (string.IsNullOrEmpty(sortTerms)) sortTerms = string.Empty;
            sortTerms = string.Join(", ", sortTerms.ToLower().Split(new char[] { ',' }).Select(t => t.Trim())
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

        protected static string GetPropertyTypeLabel(ConfigurationRegistryRecord.PropertyListType propertyType)
        {
            return propertyType == ConfigurationRegistryRecord.PropertyListType.AddIns ? "Add-in" :
                propertyType == ConfigurationRegistryRecord.PropertyListType.Batch ? "Batch" :
                propertyType == ConfigurationRegistryRecord.PropertyListType.BatchComponent ? "Batch Component" :
                propertyType == ConfigurationRegistryRecord.PropertyListType.Compound ? "Compound" :
                propertyType == ConfigurationRegistryRecord.PropertyListType.PropertyList ? "Registry" :
                propertyType == ConfigurationRegistryRecord.PropertyListType.Structure ? "Base Fragment" : "Extra";
        }
    }
}
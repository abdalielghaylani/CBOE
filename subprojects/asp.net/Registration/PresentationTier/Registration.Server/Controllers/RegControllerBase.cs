using CambridgeSoft.COE.Framework.COETableEditorService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Access;
using Csla.Data;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net.Http.Headers;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegControllerBase : ApiController
    {
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
            CookieHeaderValue cookie = Request.Headers.GetCookies("COESSO").FirstOrDefault();
            return cookie != null ? cookie["COESSO"].Value : string.Empty;
        }

        protected void CheckAuthentication()
        {
            string sessionToken = GetSessionToken();
            if (string.IsNullOrEmpty(sessionToken) || !COEPrincipal.Login(sessionToken, true))
                throw new InvalidOperationException("Authentication failed");
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
    }
}
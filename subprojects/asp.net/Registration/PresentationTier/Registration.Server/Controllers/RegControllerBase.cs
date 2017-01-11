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

        protected JArray ExtractData(string sql, Dictionary<string, object> args = null)
        {
            using (var reader = GetReader(sql, args))
            {
                return ExtractData(reader);
            }
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
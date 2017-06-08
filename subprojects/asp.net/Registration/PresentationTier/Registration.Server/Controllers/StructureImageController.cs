using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Microsoft.Web.Http;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class StructureImageController : RegControllerBase
    {
        private string GetStructureRelativePath(string type, int compoundId, int height, int width, int resolution)
        {
            var f = type.Equals("record", System.StringComparison.OrdinalIgnoreCase) ?
                new string[] { "structureaggregation", "vw_mixture_regnumber", "regid" } :
                type.Equals("temprecord", System.StringComparison.OrdinalIgnoreCase) ?
                new string[] { "normalizedstructure", "vw_temporarycompound", "tempcompoundid" } :
                new string[] { "structure", "vw_fragment", "fragmentid" };

            var queryParams = new Dictionary<string, object>();
            queryParams.Add(":id", compoundId);

            string structureData = string.Empty;
            if (type.Equals("template"))
                structureData = GetStructureData(compoundId);
            else
                structureData = (string)ExtractData(string.Format("SELECT {0} data FROM {1} WHERE {2}=:id", f[0], f[1], f[2]), queryParams)[0]["DATA"];

            var args = new object[] { structureData, "image/png", (double)height, (double)width, resolution.ToString() };
            return (string)typeof(COEChemDrawConverterUtils).GetMethod(
                "GetStructureResource",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new System.Type[] { typeof(string), typeof(string), typeof(double), typeof(double), typeof(string) },
                null
            ).Invoke(null, args);
        }

        [Route(Consts.apiPrefix + "StructureImage/{type}/{compoundId}/{height:int?}/{width:int?}/{resolution:int?}")]
        public HttpResponseMessage GetStructureImage(string type, int compoundId, int height = 150, int width = 200, int resolution = 300)
        {
            CheckAuthentication();
            var relPath = GetStructureRelativePath(type, compoundId, height, width, resolution);
            var fullPath = HttpContext.Current.Server.MapPath(relPath);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(File.ReadAllBytes(fullPath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var cacheControl = new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(1) };
            response.Headers.CacheControl = cacheControl;
            return response;
        }

        [Route(Consts.apiPrefix + "StructureUrl/{type}/{compoundId}/{height:int?}/{width:int?}/{resolution:int?}")]
        public HttpResponseMessage GetStructureUrl(string type, int compoundId, int height = 150, int width = 200, int resolution = 300)
        {
            CheckAuthentication();
            var relPath = GetStructureRelativePath(type, compoundId, height, width, resolution);
            var response = Request.CreateResponse(HttpStatusCode.OK, relPath);
            var cacheControl = new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(1) };
            response.Headers.CacheControl = cacheControl;
            return response;
        }

        private string GetStructureData(int templateId)
        {
            COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(templateId);
            if (genericStorageBO == null)
                throw new RegistrationException(string.Format("no template found for the template id '{0}'", templateId));

            RegistryRecord record = RegistryRecord.NewRegistryRecord();
            record.InitializeFromXml(genericStorageBO.COEGenericObject, true, false);
            return record.StructureAggregation;
        }
    }
}

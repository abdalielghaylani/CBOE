using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System;
using System.Web;
using System.IO;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
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
            var structureData = (string)ExtractData(string.Format("SELECT {0} data FROM {1} WHERE {2}=:id", f[0], f[1], f[2]), queryParams)[0]["DATA"];
            var args = new object[] { structureData, "image/png", (double)height, (double)width, resolution.ToString() };
            return (string)typeof(COEChemDrawConverterUtils).GetMethod("GetStructureResource", BindingFlags.Static | BindingFlags.NonPublic, null,
                new System.Type[] { typeof(string), typeof(string), typeof(double), typeof(double), typeof(string) }, null).Invoke(null, args);
        }

        [Route("api/StructureImage/{type}/{compoundId}/{height:int?}/{width:int?}/{resolution:int?}")]
        public HttpResponseMessage GetStructureImage(string type, int compoundId, int height = 150, int width = 200, int resolution = 300)
        {
            var relPath = GetStructureRelativePath(type, compoundId, height, width, resolution);
            var fullPath = HttpContext.Current.Server.MapPath(relPath);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(File.ReadAllBytes(fullPath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var cacheControl = new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(1) };
            response.Headers.CacheControl = cacheControl;
            return response;
        }

        [Route("api/StructureUrl/{type}/{compoundId}/{height:int?}/{width:int?}/{resolution:int?}")]
        public HttpResponseMessage GetStructureUrl(string type, int compoundId, int height = 150, int width = 200, int resolution = 300)
        {
            var relPath = GetStructureRelativePath(type, compoundId, height, width, resolution);
            var response = Request.CreateResponse(HttpStatusCode.OK, relPath);
            var cacheControl = new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(1) };
            response.Headers.CacheControl = cacheControl;
            return response;
        }
    }
}

using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Reflection;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class DataConversionController : RegControllerBase
    {
        private JObject Convert(string fromType, string toType, string fromData)
        {
            // CacheableChemdrawControl may dispose automatically when timeout elapses.
            // In other words, it does not need to be disposed of explicitly in this code.
            // In order to support multiple versions of ChemDrawCtl, all calls to ChemDrawCtl are done dynamically.
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            dynamic cacheableChemDrawCtl = (object)CambridgeSoft.COE.Framework.Caching.CacheableChemdrawControl.GetCachedChemdrawControl(assemblyName);
            dynamic chemDrawCtl =  cacheableChemDrawCtl.Control;
            dynamic cdObjects = chemDrawCtl.Objects;
            cdObjects.Clear();
            chemDrawCtl.DataEncoded = true;
            chemDrawCtl.set_Data(fromType, fromData);
            var toData = new JObject(
                new JProperty("data", chemDrawCtl.get_Data(toType))
            );
            return toData;
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "DataConversion/ToCdxml")]
        public JObject ToCdxml()
        {
            var fromData = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return Convert("chemical/x-cdx", "text/xml", fromData);
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "DataConversion/FromCdxml")]
        public JObject FromCdxml()
        {
            var fromData = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return Convert("text/xml", "chemical/x-cdx", fromData);
        }
    }
}

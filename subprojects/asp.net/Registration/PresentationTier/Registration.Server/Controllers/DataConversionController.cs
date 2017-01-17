using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using ChemDrawControl14;
using System.Runtime.InteropServices;
using System.Text;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class DataConversionController : RegControllerBase
    {
        private JObject Convert(string fromType, string toType, string fromData)
        {
            ChemDrawCtl ctrl = null;
            try
            {
                ctrl = new ChemDrawCtl();
                ctrl.Objects.Clear();
                ctrl.DataEncoded = true;
                ctrl.Objects.set_Data(fromType, null, null, null, fromData);
                var toData = new JObject(
                    new JProperty("data", ctrl.get_Data(toType))
                );
                return toData;
            }
            finally
            {
                if (ctrl != null)
                    Marshal.ReleaseComObject(ctrl);
                GC.Collect();
            }
        }

        [HttpPost]
        [Route("api/DataConversion/ToCdxml")]
        public JObject ToCdxml()
        {
            var fromData = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return Convert("chemical/x-cdx", "text/xml", fromData);
        }

        [HttpPost]
        [Route("api/DataConversion/FromCdxml")]
        public JObject FromCdxml()
        {
            var fromData = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return Convert("text/xml", "chemical/x-cdx", fromData);
        }
    }
}

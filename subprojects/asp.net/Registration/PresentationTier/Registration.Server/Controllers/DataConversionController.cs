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
        [HttpPost]
        [Route("api/DataConversion/ToCdxml")]
        public JObject ToCdxml()
        {
            var fromData = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            ChemDrawCtl ctrl = null;
            try
            {
                ctrl = new ChemDrawCtl();
                ctrl.Objects.Clear();
                ctrl.DataEncoded = true;
                ctrl.Objects.set_Data("chemical/x-cdx", null, null, null, fromData);
                var toData = new JObject(
                    new JProperty("data", ctrl.get_Data("text/xml"))
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
    }
}

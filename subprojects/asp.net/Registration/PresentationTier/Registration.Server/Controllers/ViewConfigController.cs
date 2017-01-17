using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class ViewConfigController : RegControllerBase
    {
        [Route("api/ViewConfig/Lookups")]
        public JObject GetLookups()
        {
            return new JObject(
                new JProperty("users", ExtractData("SELECT * FROM VW_PEOPLE")),
                new JProperty("fragments", ExtractData("SELECT * FROM VW_FRAGMENT")),
                new JProperty("fragmentTypes", ExtractData("SELECT * FROM VW_FRAGMENTTYPE")),
                new JProperty("identifierTypes", ExtractData("SELECT * FROM VW_IDENTIFIERTYPE")),
                new JProperty("notebooks", ExtractData("SELECT * FROM VW_NOTEBOOK")),
                new JProperty("pickList", ExtractData("SELECT * FROM VW_PICKLIST")),
                new JProperty("pickListDomains", ExtractData("SELECT * FROM VW_PICKLISTDOMAIN")),
                new JProperty("projects", ExtractData("SELECT * FROM VW_PROJECT")),
                new JProperty("sequences", ExtractData("SELECT * FROM VW_SEQUENCE")),
                new JProperty("sites", ExtractData("SELECT * FROM VW_SITES"))
            );
        }
    }
}

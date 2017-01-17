using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegistryRecordsController : RegControllerBase
    {
        #region Permanent Records
        public JArray Get()
        {
            return ExtractData("SELECT regid id, name, created, modified, personcreated as creator, 'record.' || regid as structure, regnumber, statusid as status, approved FROM vw_mixture_regnumber ORDER BY regnumber DESC");
        }

        public dynamic Get(int id)
        {
            return null;
        }
        #endregion // Permanent Records

        #region Temporary Records
        [Route("api/RegistryRecords/Temp")]
        public JArray GetTemp()
        {
            return ExtractData("SELECT tempcompoundid id, tempbatchid batchid, formulaweight MW, molecularformula MF, datecreated created, datelastmodified modified, personcreated as creator, 'temprecord.' || tempcompoundid as structure FROM vw_temporarycompound ORDER BY tempbatchid DESC");
        }

        [Route("api/RegistryRecords/Temp/{id}")]
        public dynamic GetTemp(int id)
        {
            return null;
        }
        #endregion // Tempoary Records
    }
}

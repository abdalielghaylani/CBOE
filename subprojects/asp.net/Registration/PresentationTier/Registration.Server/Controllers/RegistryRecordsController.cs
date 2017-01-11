using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class RegistryRecordsController : RegControllerBase
    {
        #region Permanent Records
        public JArray Get()
        {
            return ExtractData("SELECT regid id, name, created, modified, personcreated as creator, 'api/RegistryRecords/StructureImage/' || regid || '/300/300/300' as structureuri, regnumber, statusid as status, approved FROM vw_mixture_regnumber");
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
            return ExtractData("SELECT tempcompoundid id, tempbatchid batchid, formulaweight MW, molecularformula MF, datecreated created, datelastmodified modified, personcreated as creator, 'api/RegistryRecords/StructureImage/' || tempcompoundid || '/300/300/300' as structureUri FROM vw_temporarycompound");
        }

        [Route("api/RegistryRecords/Temp/{id}")]
        public dynamic GetTemp(int id)
        {
            return null;
        }
        #endregion // Tempoary Records

        [Route("api/RegistryRecords/StructureImage/{compoundId}/{height:int?}/{width:int?}/{resolution:int?}")]
        public static object GetStructureImage(int compoundId, int height = 300, int width = 300, int resolution = 300)
        {
            object[] args = { string.Empty, "image/png", 300d, 300d, "300" };
            return typeof(COEChemDrawConverterUtils).GetMethod("ConvertStructure", System.Reflection.BindingFlags.Static).Invoke(null, args);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;

namespace PerkinElmer.COE.ChemBioViz.Server.Controllers
{
    public class RegistrationSearchController : RegSearchControllerBase
    {
        #region SearchRecords
        [Route("api/SearchRecords/RetrieveAll")]
        public JArray RetrieveAll()
        {
            CheckAuthentication();
            return null;
            //return ExtractData("SELECT tempcompoundid id, tempbatchid batchid, formulaweight MW, molecularformula MF, datecreated created, datelastmodified modified, personcreated as creator, 'temprecord/' || tempcompoundid || '?' || to_char(datelastmodified, 'YYYYMMDDHH24MISS') as structure FROM vw_temporarycompound ORDER BY tempbatchid DESC");
        }
        #endregion SearchRecords
    }
}
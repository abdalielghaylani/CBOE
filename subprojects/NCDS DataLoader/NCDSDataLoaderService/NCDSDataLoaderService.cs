using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.NCDS_DataLoader.Web
{
    [WebService(Namespace = "http://localhost/NCDSDataLoaderService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class NCDSDataLoaderService : System.Web.Services.WebService
    {
        public NCDSDataLoaderService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public string[] GetNonStructuralDuplicateCheckSettings()
        {
            KeyValuePair<PreloadDupCheckMechanism, string> _dupCheckMechanism;
            _dupCheckMechanism = RegSvcUtilities.GetNonStructuralDuplicateCheckSettings();
            string[] strValue = new string[2];
            strValue[0] = _dupCheckMechanism.Key.ToString();
            strValue[1] = _dupCheckMechanism.Value.ToString();
            return strValue;
        }
    }
}

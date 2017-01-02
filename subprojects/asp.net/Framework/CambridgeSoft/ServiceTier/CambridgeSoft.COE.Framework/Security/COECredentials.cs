using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services.Protocols;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    [Serializable()]
    /// <summary>
    /// Credentials to be used to authenticate users within the webservice.
    /// </summary>
    public class COECredentials : SoapHeader {
        public string AuthenticationTicket;
        public string UserName;
        public string Password;
    }

}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Controls.WebParts;
using CambridgeSoft.COE.Framework.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Manager
{
    public class NumberOfUsers: CustomItem
    {

        public override string  GetCustomItem()
        {
            string styleForNumber = base.configData.Get("styleForNumber").Value;
            string styleForText = base.configData.Get("styleForText").Value;

            return "<p style='" + styleForNumber + "'> 20 <span style='" + styleForText + "'> SomeText</span></p>";
        }
     
    }
}

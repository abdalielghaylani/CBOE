using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace CambridgeSoft.COE.Security.Services
{
	public class SSOConfigurationExemptUser : ConfigurationElement
	{
		[ConfigurationProperty("userName", IsRequired = true)]
		public string userName
		{
			get
			{
				return this["userName"] as string;
			}
			set
			{
				this["userName"] = value;
			}
		}

		[ConfigurationProperty("ssoProvider", IsRequired = false)]
		public string ssoProvider
		{
			get
			{
				return this["ssoProvider"] as string;
			}
			set
			{
				this["ssoProvider"] = value;
			}
		}
	}
}

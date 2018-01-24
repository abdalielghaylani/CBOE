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

	public class SSOConfigurationReturnInfo : ConfigurationElement
	{
		[ConfigurationProperty("ldapCode", IsRequired = true)]
		public string ldapCode
		{
			get
			{
				return this["ldapCode"] as string;
			}
		}

		[ConfigurationProperty("displayName", IsRequired = false)]
		public string displayName
		{
			get
			{
				return this["displayName"] as string;
			}
		}
		[ConfigurationProperty("mapTo", IsRequired = false)]
		public string mapTo
		{
			get
			{
				return this["mapTo"] as string;
			}
		}

		[ConfigurationProperty("nodeName", IsRequired = false)]
		public string nodeName
		{
			get
			{
				return this["nodeName"] as string;
			}
		}
	}

	
}

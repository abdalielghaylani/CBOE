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
	public class SSOConfigurationLDAPSetting : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsRequired = true)]
		public string Name
		{
			get
			{
				return this["Name"] as string;
			}
			set
			{
				this["Name"] = value;
			}
		}

		[ConfigurationProperty("Value", IsRequired = true)]
		public string Value
		{
			get
			{
				return this["Value"] as string;
			}
			set
			{
				this["Value"] = value;
			}
		}
	}
}

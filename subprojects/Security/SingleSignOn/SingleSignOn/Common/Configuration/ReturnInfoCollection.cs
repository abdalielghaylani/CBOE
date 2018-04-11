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

	public class SSOConfigurationReturnInfoCollection : ConfigurationElementCollection
	{
		public SSOConfigurationReturnInfo this[int index]
		{
			get
			{
				return base.BaseGet(index) as SSOConfigurationReturnInfo;
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}
		new public SSOConfigurationReturnInfo this[string Name]
		{
			get
			{
				return (SSOConfigurationReturnInfo)BaseGet(Name);
			}
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new SSOConfigurationReturnInfo();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SSOConfigurationReturnInfo)element).ldapCode;
		}
	}

}

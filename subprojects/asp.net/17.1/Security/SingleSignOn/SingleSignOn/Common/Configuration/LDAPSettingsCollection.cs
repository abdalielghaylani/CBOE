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
	public class SSOConfigurationLDAPSettingsCollection : ConfigurationElementCollection
	{
		public SSOConfigurationLDAPSetting this[int index]
		{
			get
			{
				return base.BaseGet(index) as SSOConfigurationLDAPSetting;
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
		new public SSOConfigurationLDAPSetting this[string Name]
		{
			get
			{
				return (SSOConfigurationLDAPSetting)BaseGet(Name);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SSOConfigurationLDAPSetting();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SSOConfigurationLDAPSetting)element).Name;
		}

		public int IndexOf(SSOConfigurationLDAPSetting Name)
		{
			return BaseIndexOf(Name);
		}

	}
}

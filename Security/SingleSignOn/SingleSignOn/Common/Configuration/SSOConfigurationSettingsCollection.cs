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
	public class SSOConfigurationSettingsCollection : ConfigurationElementCollection
	{
		public SSOConfigurationSetting this[int index]
		{
			get
			{
				return base.BaseGet(index) as SSOConfigurationSetting;
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
		new public SSOConfigurationSetting this[string Name]
		{
			get
			{
				return (SSOConfigurationSetting)BaseGet(Name);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SSOConfigurationSetting();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SSOConfigurationSetting)element).Name;
		}

		public int IndexOf(SSOConfigurationSetting Name)
		{
			return BaseIndexOf(Name);
		}

	}
}

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
	public class SSOConfigurationExemptUsersCollection : ConfigurationElementCollection
	{
		public SSOConfigurationExemptUser this[int index]
		{
			get
			{
				return base.BaseGet(index) as SSOConfigurationExemptUser;
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
		new public SSOConfigurationExemptUser this[string Name]
		{
			get
			{
				return (SSOConfigurationExemptUser)BaseGet(Name);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SSOConfigurationExemptUser();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SSOConfigurationExemptUser)element).userName;
		}

		public int IndexOf(SSOConfigurationExemptUser userName)
		{
			return BaseIndexOf(userName);
		}
		public void Add(SSOConfigurationExemptUser exemptUser)
		{
			BaseAdd(exemptUser);
		} 

	}
}

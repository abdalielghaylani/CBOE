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
	public class SSOConfigurationCSSConnectionsCollection : ConfigurationElementCollection
	{
		public SSOConfigurationCSSConnection this[int index]
		{
			get
			{
				return base.BaseGet(index) as SSOConfigurationCSSConnection;
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
		new public SSOConfigurationCSSConnection this[string Name]
		{
			get
			{
				return (SSOConfigurationCSSConnection)BaseGet(Name);
			}
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new SSOConfigurationCSSConnection();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SSOConfigurationCSSConnection)element).connectionName;
		}
	}
}

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
using System.IO;
using System.Xml.XPath;
using System.Resources;
using SingleSignOn.Properties;
namespace CambridgeSoft.COE.Security.Services
{
	public class SSOConfigurationCSSConnection : ConfigurationElement
	{

		[ConfigurationProperty("connectionName", IsRequired = true)]
		public string connectionName
		{
			get
			{
				return this["connectionName"] as string;
			}
		}

		[ConfigurationProperty("dataSource", IsRequired = false)]
		public string dataSource
		{
			get
			{
				string ds = string.Empty;

				ds = this["dataSource"] as string;

				if (ds == "")
				{
					ds = GetCOEOracleDatasource();
				}

				return ds;
			}
		}

        [ConfigurationProperty("schemaName", IsRequired = false)]
        public string schemaName
        {
            get
            {
                string schema = string.Empty;

                schema = this["schemaName"] as string;

                if (schema == "")
                {
                    schema = "COEDB";
                }

                return schema;
            }
        }


        [ConfigurationProperty("password", IsRequired = false)]
        public string password
        {
            get
            {
                string pw = string.Empty;

                pw = this["password"] as string;

                if (pw == "")
                {
                    pw = GetSchemaPassword(this.schemaName);
                }

                if (pw == "")
                {
                    pw = "ORACLE";
                }

                return pw;
            }
        }

        private string GetCOEOracleDatasource()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //Load the file into the XmlDocument

            try
            {
                //xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ChemOfficeEnterprise" + Resources.COEVersion + "\\" + Resources.SSOConfigFileName);
                //use the path as defined in SSOConfiguration which is now public
                xmlDoc.Load(SSOConfig.configPath);

                XmlNode xnode = xmlDoc.SelectSingleNode("configuration/coeConfiguration/dbmsTypes");

                string rval = string.Empty;

                for (int i = 0; i < xnode.ChildNodes.Count; i++)
                {
                    if (xnode.ChildNodes[i].Attributes["name"].Value.ToUpper() == "ORACLE")
                    {
                        if (!string.IsNullOrEmpty(xnode.ChildNodes[i].Attributes["dataSource"].Value.ToUpper()))
                        {
                            rval = xnode.ChildNodes[i].Attributes["dataSource"].Value.ToUpper();
                            break;
                        }                        
                    }
                }

                return rval;
            }
            catch
            {

                return "";
            }
        }

        private string GetSchemaPassword(string schemaName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //Load the file into the XmlDocument

            try
            {
                //xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ChemOfficeEnterprise" + Resources.COEVersion + "\\" + Resources.SSOConfigFileName);
                //use the path as defined in SSOConfiguration which is now public
                xmlDoc.Load(SSOConfig.configPath);

                XmlNode xnode = xmlDoc.SelectSingleNode("configuration/coeConfiguration/databases");

                string rval = string.Empty;

                for (int i = 0; i < xnode.ChildNodes.Count; i++)
                {
                    if (xnode.ChildNodes[i].Attributes["name"].Value.ToUpper() == schemaName)
                    {
                        if (!string.IsNullOrEmpty(xnode.ChildNodes[i].Attributes["password"].Value.ToUpper()))
                        {
                            rval = xnode.ChildNodes[i].Attributes["password"].Value;//.ToUpper();
                            break;
                        }
                    }
                }

                return rval;
            }
            catch
            {

                return "";
            }
        }
	}
}

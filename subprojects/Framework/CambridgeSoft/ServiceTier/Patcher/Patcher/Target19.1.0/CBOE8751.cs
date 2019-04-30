using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE8751:BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlAttribute defaultAttr = null;
            XmlAttribute providerAttr = null;

            XmlNode defaultFips = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applicationDefaults");
            XmlNode providerFips = frameworkConfig.SelectSingleNode("/configuration/SSOConfiguration/ProviderConfiguration/Settings/add[@Name='FIPS_ENABLED']");
            XmlNode providerSettings = frameworkConfig.SelectSingleNode("/configuration/SSOConfiguration/ProviderConfiguration/Settings");
            if (defaultFips != null)
            {
                if (defaultFips.Attributes["fipsEnabled"] == null)
                {
                    defaultAttr = defaultFips.OwnerDocument.CreateAttribute("fipsEnabled");
                    defaultFips.Attributes.Append(defaultAttr);
                    defaultFips.Attributes["fipsEnabled"].Value = "false";
                    
                    messages.Add("fipsEnabled Attribute created Successfully");
                }
                else
                {
                    messages.Add("fipsEnabled attribute already exists");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("applicationDefaults tag is not available in FrameworkConfig XML");
            }

            if (providerSettings != null)
            {
                if (providerFips == null)
                {
                    providerFips = providerSettings.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                    createNewAttribute("Name", "FIPS_ENABLED", ref providerFips);
                    createNewAttribute("Value", "FALSE", ref providerFips);
                    providerSettings.AppendChild(providerFips);
                    messages.Add("FIPS_ENABLED node create successfully");
                }
                else
                {
                    messages.Add("FIPS_ENABLED node already exists");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Settings Tag is not available in FrameworkConfig XML");
            }


            if (!errorsInPatch && messages.Count != 0)
            {
                messages.Add("process succeed!");
            }
            else
            {
                messages.Add("failed to patch.");
            }

            return messages;
        }

        #region Private Method
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        #endregion
	}
}

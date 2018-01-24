using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Add a new lookup entry for Structure level Identifier type.
    /// These lookup entries are stored in COEFrameworkConfig, 
    /// and are used for UI display when manage customizable table "IDENTIFIERS TYPES"
    /// </summary>
	class StructureIdentifierType_LookupEntry:BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlNode identifierTypes = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='IDENTIFIERTYPES']/innerXmlData");
            if (identifierTypes != null)
            {
                if (identifierTypes.SelectSingleNode("//add[@name='structureValue']")!=null)
                    messages.Add("Lookup entry for Structure level Identifier type already exists");
                else
                {
                    //new lookup entry for Structure level Identifier type
                    XmlElement newEntry = frameworkConfig.CreateElement("add");

                    XmlAttribute attribute = frameworkConfig.CreateAttribute("name");
                    attribute.Value = "structureValue";
                    newEntry.SetAttributeNode(attribute);

                    attribute = frameworkConfig.CreateAttribute("value");
                    attribute.Value = "S";
                    newEntry.SetAttributeNode(attribute);

                    attribute = frameworkConfig.CreateAttribute("display");
                    attribute.Value = "Base Fragment";
                    newEntry.SetAttributeNode(attribute);

                    identifierTypes.AppendChild(newEntry);
                }
            }
            else
            {
                messages.Add("No IDENTIFIERTYPES found in CoeFrameworkConfig");
                errorsInPatch = true;
            }

            if (!errorsInPatch)
                messages.Add("Structure level Identifier type patched successfully");
            else
                messages.Add("Failed to add new lookup entry for Structure level Identifier type");
            return messages;
        }
	}
}

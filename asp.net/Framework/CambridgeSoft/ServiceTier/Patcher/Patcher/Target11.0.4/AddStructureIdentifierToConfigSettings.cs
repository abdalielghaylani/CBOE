using System.Xml;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Add a new system setting 'Structure_Identifiers' to the Advanced tab of System Settings page.
    /// </summary>
	class AddStructureIdentifierToConfigSettings : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlDocument regConfig = null;
            foreach (XmlDocument config in configurations)
            {
                if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                {
                    regConfig = config;
                    break;
                }
            }

            //Coverity fix - CID 19421
            if (regConfig != null)
            {
                XmlNode miscSettingsNode = regConfig.SelectSingleNode(
                    "/Registration/applicationSettings/groups/add[@name='MISC']/settings");
                XmlNode structureIdentifiersNode = regConfig.SelectSingleNode(
                    "/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='Structure_Identifiers']");

                if (miscSettingsNode != null && structureIdentifiersNode == null)
                {
                    structureIdentifiersNode = regConfig.CreateElement("add");

                    XmlAttribute nameAttr = regConfig.CreateAttribute("name");
                    nameAttr.Value = "Structure_Identifiers";
                    structureIdentifiersNode.Attributes.Append(nameAttr);

                    XmlAttribute valueAttr = regConfig.CreateAttribute("value");
                    valueAttr.Value = "False";
                    structureIdentifiersNode.Attributes.Append(valueAttr);

                    XmlAttribute controlTypeAttr = regConfig.CreateAttribute("controlType");
                    controlTypeAttr.Value = "PICKLIST";
                    structureIdentifiersNode.Attributes.Append(controlTypeAttr);

                    XmlAttribute descriptionAttr = regConfig.CreateAttribute("description");
                    descriptionAttr.Value = "Enables/Disables Identifiers grid at Structure level";
                    structureIdentifiersNode.Attributes.Append(descriptionAttr);

                    XmlAttribute allowedValuesAttr = regConfig.CreateAttribute("allowedValues");
                    allowedValuesAttr.Value = "True|False";
                    structureIdentifiersNode.Attributes.Append(allowedValuesAttr);

                    XmlAttribute isAdminAttr = regConfig.CreateAttribute("isAdmin");
                    isAdminAttr.Value = "False";
                    structureIdentifiersNode.Attributes.Append(isAdminAttr);

                    XmlAttribute processorClassAttr = regConfig.CreateAttribute("processorClass");
                    processorClassAttr.Value = "";
                    structureIdentifiersNode.Attributes.Append(processorClassAttr);

                    miscSettingsNode.AppendChild(structureIdentifiersNode);
                }
                else
                {
                    errorsInPatch = true;

                    if (miscSettingsNode == null)
                        messages.Add("Unable to find the MISC node");
                    if (structureIdentifiersNode != null)
                        messages.Add("This patch may have already been applied.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Unable to find the Reg App Settings node");
            }

            if (!errorsInPatch)
                messages.Add("NoStructureDuplicateCheckAppSetting was successfully patched");
            else
                messages.Add("NoStructureDuplicateCheckAppSetting was patched with errors");

            return messages;
        }
    }
}

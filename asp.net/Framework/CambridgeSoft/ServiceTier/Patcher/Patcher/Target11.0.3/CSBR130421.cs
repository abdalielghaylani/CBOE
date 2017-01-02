using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Supports the padding of the BatchNumber for FullRegNumber as a system setting.
    /// </summary>
    /// <remarks>
    /// The default configuration does not have an allowedValues attribute so this code
    /// includes the provision of one if none is found.
    /// </remarks>
    public class CSBR130421 : BugFixBaseCommand
	{
        public override List<string> Fix(
            List<XmlDocument> forms, List<XmlDocument> dataviews
            , List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlDocument regConfig = null;
            XmlNode nodeToModify = null;
            foreach (XmlDocument config in configurations)
            {
                if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                {
                    regConfig = config;
                    break;
                }
            }

            //Coverity fix- CID 19416
            if (regConfig != null)
            {
                nodeToModify = regConfig.SelectSingleNode(
                    "/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='BatchNumberLength']");
            }
            string controlType = "PICKLIST";
            string allowedValues = "2|3|4|5|6";

/*
<add
  name="BatchNumberLength"
  value="2"
  controlType="PICKLIST"
  description="This setting determines the length of the batch number to be used."
  allowedValues="2|3|4|5|6"
  isAdmin="False"
/>
*/
            if (nodeToModify != null)
            {
                // controlType attribute
                {
                    XmlAttribute controlTypeAttribute = nodeToModify.Attributes["controlType"];
                    if (controlTypeAttribute == null)
                    {
                        controlTypeAttribute = regConfig.CreateAttribute("controlType");
                        nodeToModify.Attributes.Append(controlTypeAttribute);
                    }
                    controlTypeAttribute.Value = controlType;
                }
                // allowedValues attribute
                {
                    XmlAttribute allowedValuesAttribute = nodeToModify.Attributes["allowedValues"];
                    if (allowedValuesAttribute == null)
                    {
                        allowedValuesAttribute = regConfig.CreateAttribute("allowedValues");
                        nodeToModify.Attributes.Append(allowedValuesAttribute);
                    }
                    allowedValuesAttribute.Value = allowedValues;
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Unable to find the 'BatchNumberLength' node to modify!");
            }

            if (!errorsInPatch)
                messages.Add("Modifications required by CSBR130421 were successfully patched");
            else
                messages.Add("CSBR130421 was patched with errors");
            return messages;

        }
	}
}

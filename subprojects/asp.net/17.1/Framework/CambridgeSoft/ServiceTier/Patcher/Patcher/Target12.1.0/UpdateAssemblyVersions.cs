using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class UpdateAssemblyVersions : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNodeList nodeList = frameworkConfig.DocumentElement.SelectNodes("//*[contains(@dalProviderAssemblyNameFull,'Version=11.0.1.0')] | //*[contains(@type,'Version=11.0.1.0')] | //*[contains(@assembly,'Version=11.0.1.0')]");

            if (nodeList.Count == 0)
            {
                errorsInPatch = true;
                messages.Add("There was no type referencing framework 11.0.1.0");
            }
            else
            {
                foreach (XmlNode node in nodeList)
                {
                    string name = node.Name;
                    if (node.Attributes["name"] != null && !string.IsNullOrEmpty(node.Attributes["name"].Value))
                        name = node.Attributes["name"].Value;

                    frameworkConfig.InnerXml = frameworkConfig.InnerXml.Replace("Version=11.0.1.0", "Version=12.1.0.0");

                    messages.Add("Node " + name + " was updated with the new assembly version");
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("Assembly versions were successfully updated in framework config.");
            }
            else
            {
                messages.Add("No assembly version was updated in framework config.");
            }
            
            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
   public class EN3588 : BugFixBaseCommand
	{
        // Fix for EN-3588 No Show Hit Lists option in Registry Compound Selector
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            #region FrameworkConfig changes:
            XmlNode searchRegisteryLeftpan = frameworkConfig.SelectSingleNode("//formBehaviour/form[@formId='4006']/leftPanelOptions");
            if (searchRegisteryLeftpan == null)
            {
                errorInPatch = true;
                messages.Add("There was no leftpan option configured for ELN searching permanent records");
            }
            else
            {
                XmlNode queryManagementNode = searchRegisteryLeftpan.SelectSingleNode("./queryManagement");
                if (queryManagementNode == null)
                {
                    queryManagementNode = frameworkConfig.CreateElement("queryManagementNode");
                    queryManagementNode.Attributes.Append(frameworkConfig.CreateAttribute("visible"));
                    queryManagementNode.Attributes.Append(frameworkConfig.CreateAttribute("enabled"));
                    searchRegisteryLeftpan.AppendChild(queryManagementNode);
                }

                queryManagementNode.Attributes["visible"].Value = "YES";
                queryManagementNode.Attributes["enabled"].Value = "YES";
                messages.Add("QueryManagement enabled in ELN search registry form.");
            }
            #endregion

            if (!errorInPatch)
                messages.Add("QueryManagement successfully enabled in ELN search registry form.");
            else
                messages.Add("Error  while enabling QueryManagement in ELN search registry form.");

            return messages;
        }
	}
}

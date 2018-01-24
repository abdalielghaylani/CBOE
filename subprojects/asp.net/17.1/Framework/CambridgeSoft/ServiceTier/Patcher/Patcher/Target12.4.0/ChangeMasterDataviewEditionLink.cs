using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Issue: SelectMasterDVTables.aspx is deprecated and DataviewBoard.aspx should be used instead.
    /// </summary>
	public class ChangeMasterDataviewEditionLink : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// - Open COEFrameworkConfig.xml file and look for EditMaster links, there should be 2 of them
        /// - Change the url of those 2 links to be ~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx?IsMaster=true
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            XmlNodeList oldConfig = frameworkConfig.SelectNodes("//add[@name='EditMaster']");
            if (oldConfig.Count == 0)
            {
                messages.Add("There were no links for Editing the Master Dataview.");
                errorInPatch = true;
            }
            else
            {
                foreach (XmlNode node in oldConfig)
                {
                    if (node.Attributes["url"].Value == "~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx?IsMaster=true")
                    {
                        messages.Add("Master dataview edition already has the right URL for " + node.ParentNode.ParentNode.Attributes["name"].Value);
                    }
                    else
                    {
                        node.Attributes["url"].Value = "~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx?IsMaster=true";
                        messages.Add("Modified master dataview edition for " + node.ParentNode.ParentNode.Attributes["name"].Value);
                    }
                }
            }

            if (!errorInPatch)
                messages.Add("New url for editing master dataviews was successfully set");
            else
                messages.Add("New url for editing master dataviews was not set");

            return messages;
        }
    }
}

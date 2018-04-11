using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// For group security a new item in the home needs to be placed.
    /// </summary>
	class AddManageGroupsHomeSetting : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to solve:
        /// 
        /// Open COEFrameworkConfig.xml look for coeHomeSettings and then for the entry for COE (add name="COE").
        /// Insert at the end the following node to the links list:
        /// 
        /// &lt;add name="ManageGroups" display="Manage Groups" tip="Administer groups" url="~/Forms/SecurityManager/ContentArea/ManageGroups.aspx?appName=All CS Applications" privilege="HIDEME" linkIconSize="small" linkIconBasePath="Icon_Library/Windows_Collection/windows_PNG/PNG" linkIconFileName="manage_users.png" /&gt;
        /// 
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
            bool errorsInPatch = false;

            XmlNode node = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links");

            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("Home settings for COE application where not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlNode manageGroupsNode = node.SelectSingleNode("./add[@name='ManageGroups']");
                if (manageGroupsNode == null)
                {
                    //<add name="ManageGroups" display="Manage Groups" tip="Administer groups" url="~/Forms/SecurityManager/ContentArea/ManageGroups.aspx?appName=All CS Applications" 
                    //privilege="HIDEME" linkIconSize="small" linkIconBasePath="Icon_Library/Windows_Collection/windows_PNG/PNG" linkIconFileName="manage_users.png" />
                    manageGroupsNode = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "ManageGroups";
                    manageGroupsNode.Attributes.Append(name);
                    XmlAttribute display = frameworkConfig.CreateAttribute("display");
                    display.Value = "Manage Groups";
                    manageGroupsNode.Attributes.Append(display);
                    XmlAttribute tip = frameworkConfig.CreateAttribute("tip");
                    tip.Value = "Administer groups";
                    manageGroupsNode.Attributes.Append(tip);
                    XmlAttribute url = frameworkConfig.CreateAttribute("url");
                    url.Value = "~/Forms/SecurityManager/ContentArea/ManageGroups.aspx?appName=All CS Applications";
                    manageGroupsNode.Attributes.Append(url);
                    XmlAttribute privilege = frameworkConfig.CreateAttribute("privilege");
                    privilege.Value = "HIDEME";
                    manageGroupsNode.Attributes.Append(privilege);
                    XmlAttribute linkIconSize = frameworkConfig.CreateAttribute("linkIconSize");
                    linkIconSize.Value = "small";
                    manageGroupsNode.Attributes.Append(linkIconSize);
                    XmlAttribute linkIconBasePath = frameworkConfig.CreateAttribute("linkIconBasePath");
                    linkIconBasePath.Value = "Icon_Library/Windows_Collection/windows_PNG/PNG";
                    manageGroupsNode.Attributes.Append(linkIconBasePath);
                    XmlAttribute linkIconFileName = frameworkConfig.CreateAttribute("linkIconFileName");
                    linkIconFileName.Value = "manage_users.png";
                    manageGroupsNode.Attributes.Append(linkIconFileName);

                    node.AppendChild(manageGroupsNode);
                    messages.Add("ManageGroups node succesfully added in FW config");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("ManageGroups node was already present in the links list for COE application (FW Config)");
                }
            }
            if (!errorsInPatch)
                messages.Add("AddManageGroupsHomeSetting was successfully patched");
            else
                messages.Add("AddManageGroupsHomeSetting was patched with errors");
            return messages;
        }
    }
}

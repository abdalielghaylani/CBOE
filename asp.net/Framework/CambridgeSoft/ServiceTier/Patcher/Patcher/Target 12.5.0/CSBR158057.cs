using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CSBR158057 : BugFixBaseCommand
    {
        /// <summary>
        ///Patcher to update COEFrameworkConfig.xml. Fix to CSBR-158057 issues  to add the manage Group logo on the home and security manager pages.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        List<string> messages = new List<string>();

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            # region Add Manage Groups under Security on COE Home page

            XmlNode groupNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links/add[@name='ManageGroups']");
            XmlNode parentNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links");
            if (groupNode != null)
            {
                parentNode.RemoveChild(groupNode);
            }
            AddManageGrouplink(parentNode);

            #endregion

            # region Add Manage Groups link on Security home page
            try
            {
                groupNode = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='MANAGER']/applicationHome/groups/add[@name='SecurityManagerPanelContents']/links/add[@name='ManageGroups']");
                parentNode = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='MANAGER']/applicationHome/groups/add[@name='SecurityManagerPanelContents']/links");
                if (groupNode != null)
                {
                    parentNode.RemoveChild(groupNode);
                }
            }
            catch (Exception ex)
            {
                messages.Add(ex.Message);
            }
            AddManageGrouplink(parentNode);
            #endregion

            return messages;
        }

        private void AddManageGrouplink(XmlNode parentNode)
        {
            try
            {
                XmlNode newChild = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                createNewAttribute("name", "ManageGroups", ref newChild);
                createNewAttribute("display", "Manage Groups", ref newChild);
                createNewAttribute("tip", "Administer groups", ref newChild);
                createNewAttribute("url", "~/Forms/SecurityManager/ContentArea/ManageGroups.aspx?appName=All CS Applications", ref newChild);
                createNewAttribute("privilege", "CSS_MANAGE_GROUPS", ref newChild);
                createNewAttribute("linkIconSize", "small", ref newChild);
                createNewAttribute("linkIconBasePath", "Icon_Library/Windows_Collection/windows_PNG/PNG", ref newChild);
                createNewAttribute("linkIconFileName", "manage_users.png", ref newChild);

                parentNode.AppendChild(newChild);
                messages.Add("Manage Groups link was added succesfully.");
            }
            catch (Exception ex)
            {
                messages.Add("Manage Groups link was not added due to following error : " + ex.Message);
            }
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class InventoryIntegration : BugFixBaseCommand
    {


        /// <summary>
        /// patcher for adding InventoryIntegration system setting for registration.
        /// </summary>
        public override List<string> Fix(
            List<XmlDocument> forms
            , List<XmlDocument> dataviews
            , List<XmlDocument> configurations
            , XmlDocument objectConfig
            , XmlDocument frameworkConfig
            )
        {

            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlDocument regConfig = null;

            try
            {
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                    {
                        regConfig = new XmlDocument();
                        regConfig.LoadXml(config.OuterXml);
                        string lockingEnabledPatch = "//add[@name='INVENTORY']/settings/add[@name='InventoryIntegration']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);

                        if (xpathRootNode == null)
                        {
                            XmlNode settingsNode = regConfig.SelectSingleNode("//add[@name='INVENTORY']/settings");
                            XmlNode InsertBeforeNode = regConfig.SelectSingleNode("//add[@name='INVENTORY']/settings/add[@name='EnableFragments']");
                            XmlNode InsertAfterNode = regConfig.SelectSingleNode("//add[@name='INVENTORY']/settings/add[@name='EnableSubmissionTemplates']");

                            xpathRootNode = settingsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                            createNewAttribute("name", "InventoryIntegration", ref xpathRootNode);
                            createNewAttribute("value", "Disabled", ref xpathRootNode);
                            createNewAttribute("controlType", "PICKLIST", ref xpathRootNode);
                            createNewAttribute("allowedValues", "Enabled|Disabled", ref xpathRootNode);
                            createNewAttribute("description", "Enables duplicate checking to occur when records are submitted and requires suggested action from submitter.", ref xpathRootNode);
                            createNewAttribute("isAdmin", "False", ref xpathRootNode);
                            if (InsertBeforeNode != null)
                            {
                                settingsNode.InsertBefore(xpathRootNode, InsertBeforeNode);
                            }
                            else if (InsertAfterNode != null)
                            {
                                settingsNode.InsertAfter(xpathRootNode, InsertAfterNode);
                            }
                            else
                            {
                                settingsNode.AppendChild(xpathRootNode);
                            }
                            configurations[configurations.IndexOf(config)] = regConfig;
                            messages.Add("InventoryIntegration node was added succesfully.");
                        }
                        else
                            messages.Add("RegSettings already contains InventoryIntegration node.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }
            finally
            {
                if (!errorsInPatch)
                {
                    messages.Add("InventoryIntegration was successfully patched");
                }
                else
                    messages.Add("InventoryIntegration patch was aborted due to errors");
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

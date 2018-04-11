using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class BatchHighlightingField : BugFixBaseCommand
    {


        /// <summary>
        /// patcher for adding BatchHighlightingField system setting for registration.
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
                        string lockingEnabledPatch = "//add[@name='REGADMIN']/settings/add[@name='BatchHighlightingField']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);
                        
                        if (xpathRootNode == null)
                        {
                            XmlNode settingsNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings");
                            XmlNode InsertBeforeNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings/add[@name='EnableUseBatchPrefixes']");
                            XmlNode InsertAfterNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings/add[@name='BatchHighlightingValue']");

                            xpathRootNode = settingsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                            createNewAttribute("name", "BatchHighlightingField", ref xpathRootNode);
                            createNewAttribute("value", "", ref xpathRootNode);
                            createNewAttribute("controlType", "TEXT", ref xpathRootNode);
                            createNewAttribute("allowedValues", "", ref xpathRootNode);
                            createNewAttribute("description", "In combination with BachHighlightingValue configures a rule to highlight those batches that has a specific value for a given field", ref xpathRootNode);
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
                            messages.Add("BatchHighlightingField node was added succesfully.");
                        }
                        else
                            messages.Add("RegSettings already contains BatchHighlightingField node.");
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
                    messages.Add("BatchHighlightingField was successfully patched");
                }
                else
                    messages.Add("BatchHighlightingField patch was aborted due to errors");
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

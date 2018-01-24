using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class BatchHighlightingTooltip : BugFixBaseCommand
    {


        /// <summary>
        /// patcher for adding BatchHighlightingTooltip system setting for registration.
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
                        string lockingEnabledPatch = "//add[@name='REGADMIN']/settings/add[@name='BatchHighlightingTooltip']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);

                        if (xpathRootNode == null)
                        {
                            XmlNode settingsNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings");
                            XmlNode InsertBeforeNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings/add[@name='BatchHighlightingValue']");
                            XmlNode InsertAfterNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings/add[@name='EnableAuditing']");

                            xpathRootNode = settingsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                            createNewAttribute("name", "BatchHighlightingTooltip", ref xpathRootNode);
                            createNewAttribute("value", "", ref xpathRootNode);
                            createNewAttribute("controlType", "TEXT", ref xpathRootNode);
                            createNewAttribute("allowedValues", "", ref xpathRootNode);
                            createNewAttribute("description", "If a batch is highlithed this tooltip would tell the user why it was highlighted", ref xpathRootNode);
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
                            messages.Add("BatchHighlightingTooltip node was added succesfully.");
                        }
                        else
                            messages.Add("RegSettings already contains BatchHighlightingTooltip node.");
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
                    messages.Add("BatchHighlightingTooltip was successfully patched");
                }
                else
                    messages.Add("BatchHighlightingTooltip patch was aborted due to errors");
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

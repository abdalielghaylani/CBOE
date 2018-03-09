using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class EN4243 : BugFixBaseCommand
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
                        string lockingEnabledPatch = "//add[@name='REGADMIN']/settings/add[@name='UserPreferenceHideBatchProperties']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);
                        
                        if (xpathRootNode == null)
                        {
                            XmlNode settingsNode = regConfig.SelectSingleNode("//add[@name='REGADMIN']/settings");
                            xpathRootNode = settingsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);
                            createNewAttribute("name", "UserPreferenceHideBatchProperties", ref xpathRootNode);
                            createNewAttribute("value", "CREATION_DATE|NOTEBOOK_TEXT|AMOUNT|AMOUNT_UNITS|PURITY|APPEARANCE|PURITY_COMMENTS|SAMPLEID|SOLUBILITY", ref xpathRootNode);
                            createNewAttribute("controlType", "TEXT", ref xpathRootNode);
                            createNewAttribute("allowedValues", "", ref xpathRootNode);
                            createNewAttribute("description", "Properites listed here will be hidden on Batch section in user preference page", ref xpathRootNode);
                            createNewAttribute("isAdmin", "True", ref xpathRootNode);
                            settingsNode.AppendChild(xpathRootNode);
                            configurations[configurations.IndexOf(config)] = regConfig;
                            messages.Add("UserPreferenceHideBatchProperties node was added successfully.");
                        }
                        else
                            messages.Add("RegSettings already contains UserPreferenceHideBatchProperties node.");
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
                    messages.Add("UserPreferenceHideBatchProperties was successfully patched");
                }
                else
                    messages.Add("UserPreferenceHideBatchProperties patch was aborted due to errors");
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

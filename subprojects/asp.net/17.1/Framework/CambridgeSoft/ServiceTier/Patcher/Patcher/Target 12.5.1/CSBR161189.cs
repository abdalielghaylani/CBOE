using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to add the OnUpdatingPermHandler to RegistrationAddins.FindDuplicatesAddIn
    /// </summary>
    class CSBR161189 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeAddInPath = string.Empty;
            _coeAddInPath = "MultiCompoundRegistryRecord/AddIns"; // Path to check the Rootnode before patcher update.

            XmlNode rootNode;
            XmlNode insertAfterNode;

            #region AddIns
            rootNode = objectConfig.SelectSingleNode(_coeAddInPath);
            XmlNode sFindDupAddInNode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.FindDuplicatesAddIn'][@friendlyName='Find Custom Field Duplicates']");
            if (sFindDupAddInNode != null)
            {
                XmlNode eventNode = sFindDupAddInNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Event", null);
                createNewAttribute("eventName", "UpdatingPerm", ref eventNode);
                createNewAttribute("eventHandler", "OnUpdatingPermHandler", ref eventNode);

                if (sFindDupAddInNode.SelectNodes("Event").Count > 0)
                {
                    if (sFindDupAddInNode.SelectSingleNode("Event[@eventName='UpdatingPerm']") == null)
                    {
                        insertAfterNode = sFindDupAddInNode.SelectSingleNode("Event[@eventName='Registering']");
                        if (insertAfterNode != null)
                        {
                            sFindDupAddInNode.InsertAfter(eventNode, insertAfterNode);
                        }
                        else
                        {
                            sFindDupAddInNode.AppendChild(eventNode);
                        }
                        messages.Add("UpdatingPerm event added to RegistrationAddins.FindDuplicates AddIn successfully.");
                    }
                }
                else
                {
                    sFindDupAddInNode.AppendChild(eventNode);
                    messages.Add("UpdatingPerm event added to RegistrationAddins.FindDuplicates AddIn successfully.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("RegistrationAddins.FindDuplicates AddIn does not exist.");
            }


            #endregion

            if (!errorsInPatch)
            {
                messages.Add("COEObjectConfig was updated successfully.");
            }
            else
                messages.Add("COEObjectConfig was not patched due to errors.");

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

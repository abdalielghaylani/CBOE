using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  Updating the difference Between 1210 to 1250 DEV line 
    /// </summary>
    class COEObjectConfig1251 : BugFixBaseCommand
    {

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coePropertyPath = string.Empty;
            string _coeAddInPath = string.Empty;
            _coePropertyPath = "MultiCompoundRegistryRecord/BatchList/Batch/PropertyList"; // Path to check the Rootnode before patcher update.
            _coeAddInPath = "MultiCompoundRegistryRecord/AddIns"; // Path to check the Rootnode before patcher update.

            XmlNode rootNode;
            XmlNode InsertAfterNode;

            #region AddIns
            rootNode = objectConfig.SelectSingleNode(_coeAddInPath);
            XmlNode sAggrnode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.StructureAggregationAddIn'][@friendlyName='Aggregate Structures']");

           

            if (sAggrnode != null)
            {
                if (sAggrnode.SelectNodes("Event").Count > 0)
                {
                    if (sAggrnode.SelectSingleNode("Event[@eventName='Saving']") == null)
                    {
                        InsertAfterNode = sAggrnode.SelectSingleNode("Event[@eventName='Updating']");
                        XmlNode eventNode = sAggrnode.OwnerDocument.CreateNode(XmlNodeType.Element, "Event", null);
                        createNewAttribute("eventName", "Saving", ref eventNode);
                        createNewAttribute("eventHandler", "OnSavingHandler", ref eventNode);
                        sAggrnode.AppendChild(eventNode);
                        if (InsertAfterNode != null)
                        {
                            sAggrnode.InsertAfter(eventNode, InsertAfterNode);
                        }
                        else
                        {
                            sAggrnode.AppendChild(eventNode);
                        }
                        messages.Add("Aggregate Structures node was updated succesfully.");
                    }
                }
            }

            #endregion

            if (!errorsInPatch)
            {
                messages.Add("COEObjectConfig was updated succesfully");
            }
            else
                messages.Add("COEObjectConfig  was patched with errors");

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

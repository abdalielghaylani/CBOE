using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  Updating the difference Between 1210 to 1250 DEV line 
    /// </summary>
    class COEObjectConfig1250 : BugFixBaseCommand
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
            XmlNode InsertBeforeNode;
            XmlNode InsertAfterNode;
                        
            #region AddIns
            rootNode = objectConfig.SelectSingleNode(_coeAddInPath);
          

            XmlNode SNormNode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn'][@friendlyName='Structure Normalization']");
            XmlNode saltStripnode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.SaltStripAddIn'][@friendlyName='Salt Stripping']");
            XmlNode sAggrnode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.StructureAggregationAddIn'][@friendlyName='Aggregate Structures']");

            if (SNormNode != null)
            {
                if (SNormNode.SelectNodes("Event").Count > 0)
                {
                    SNormNode.SelectNodes("Event")[0].Attributes["eventName"].Value = "Inserting";
                    SNormNode.SelectNodes("Event")[0].Attributes["eventHandler"].Value = "OnInsertHandler";
                    SNormNode.SelectNodes("Event")[1].Attributes["eventName"].Value = "Updating";
                    SNormNode.SelectNodes("Event")[1].Attributes["eventHandler"].Value = "OnUpdateHandler";
                    messages.Add("Structure Normalization node was updated succesfully.");
                }
            }

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

            if (saltStripnode == null)
            {

                InsertBeforeNode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.StructureAggregationAddIn'][@friendlyName='Aggregate Structures']");
                InsertAfterNode = rootNode.SelectSingleNode("AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn'][@friendlyName='Structure Normalization']");

                saltStripnode = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AddIn", null);
                createNewAttribute("assembly", "CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc", ref saltStripnode);
                createNewAttribute("class", "CambridgeSoft.COE.Registration.Services.RegistrationAddins.SaltStripAddIn", ref saltStripnode);
                createNewAttribute("friendlyName", "Salt Stripping", ref saltStripnode);
                createNewAttribute("required", "no", ref saltStripnode);
                createNewAttribute("enabled", "no", ref saltStripnode);

                XmlNode eventNode = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Event", null);
                createNewAttribute("eventName", "Inserting", ref eventNode);
                createNewAttribute("eventHandler", "OnEventHandler", ref eventNode);
                saltStripnode.AppendChild(eventNode);
                saltStripnode.AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AddInConfiguration", null));
                saltStripnode.SelectSingleNode("AddInConfiguration").AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "EditChemDrawing", null));
                saltStripnode.SelectSingleNode("AddInConfiguration/EditChemDrawing").InnerText = "true";
                if (InsertAfterNode != null)
                {
                    rootNode.InsertAfter(saltStripnode, InsertAfterNode);
                }
                else
                {
                    rootNode.AppendChild(saltStripnode);
                }
                messages.Add("Salt Stripping node was added succesfully.");
            }
            #endregion
            
            //#region Batch_Prefix SaltandBatchSuffix

            //_coePropertyPath = "MultiCompoundRegistryRecord/BatchList/Batch/PropertyList"; // Path to check the Rootnode before patcher update.
            //rootNode = objectConfig.SelectSingleNode(_coePropertyPath);
            //InsertBeforeNode = rootNode.SelectSingleNode("Property[@name='SCIENTIST_ID']");

            //XmlNode batchPrefix_Node = rootNode.SelectSingleNode("Property[@name='BATCH_PREFIX']");
            //XmlNode saltAndBacthSuffix_Node = rootNode.SelectSingleNode("Property[@name='SALTANDBATCHSUFFIX']");

            //if (batchPrefix_Node == null)
            //{
            //    batchPrefix_Node = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Property", null);
            //    createNewAttribute("name", "BATCH_PREFIX", ref batchPrefix_Node);
            //    createNewAttribute("friendlyName", "BATCH_PREFIX", ref batchPrefix_Node);
            //    createNewAttribute("type", "PICKLISTDOMAIN", ref batchPrefix_Node);
            //    createNewAttribute("precision", "", ref batchPrefix_Node);
            //    createNewAttribute("pickListDomainID", "4", ref batchPrefix_Node);
            //    createNewAttribute("sortOrder", "0", ref batchPrefix_Node);
            //    batchPrefix_Node.AppendChild(batchPrefix_Node.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", null));
            //    if (InsertBeforeNode != null)
            //    {
            //        rootNode.InsertBefore(batchPrefix_Node, InsertBeforeNode);
            //    }
            //    else
            //    {
            //        rootNode.AppendChild(batchPrefix_Node);
            //    }
            //    messages.Add("BATCH_PREFIX node was updated succesfully in PropertyList.");
            //}
            //InsertAfterNode = rootNode.SelectSingleNode("Property[@name='BATCH_PREFIX']");
            //if (saltAndBacthSuffix_Node == null)
            //{
            //    saltAndBacthSuffix_Node = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Property", null);
            //    createNewAttribute("name", "SALTANDBATCHSUFFIX", ref saltAndBacthSuffix_Node);
            //    createNewAttribute("friendlyName", "SALTANDBATCHSUFFIX", ref saltAndBacthSuffix_Node);
            //    createNewAttribute("type", "TEXT", ref saltAndBacthSuffix_Node);
            //    createNewAttribute("precision", "100", ref saltAndBacthSuffix_Node);
            //    createNewAttribute("sortOrder", "1", ref saltAndBacthSuffix_Node);
            //    saltAndBacthSuffix_Node.AppendChild(saltAndBacthSuffix_Node.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", null));
            //    if (InsertAfterNode != null)
            //    {
            //        rootNode.InsertAfter(saltAndBacthSuffix_Node, InsertAfterNode);
            //    }
            //    else
            //    {
            //        rootNode.AppendChild(saltAndBacthSuffix_Node);
            //    }
            //    messages.Add("SALTANDBATCHSUFFIX node was updated succesfully in PropertyList.");
            //}

            //int iCount = 0;
            //foreach (XmlNode childNode in rootNode.ChildNodes)
            //{
            //    if (childNode.Attributes["sortOrder"] != null)
            //    {
            //        childNode.Attributes["sortOrder"].Value = Convert.ToString(iCount);
            //    }

            //    iCount++;
            //}
            //#endregion

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

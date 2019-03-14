using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE8231:BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;
            string coeDataViewPath = string.Empty;
            #region DataView
            foreach (XmlDocument dataviewDoc in dataviews)
            {
                string id = dataviewDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataviewDoc.DocumentElement.Attributes["dataviewid"].Value;

                if (id == "4002")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='1']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {                        
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SOURCE_V10']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", "1562", ref fields);
                            createNewAttribute("name", "SOURCE_V10", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("alias", "SOURCE_V10", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "true", ref fields);
                            createNewAttribute("isUniqueKey", "false", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[4002]:This field node with id=1562 added succesfully.");
                        }
                        else
                        {
                            messages.Add("Dataview[4002]:This field node with id=1562 already exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[4002]:VW_TEMPORARYBATCH was not found on dataview 4002.");
                    }
                }
            }
            
            #endregion

            #region COEObjectConfig

            XmlNode rootNode;
            string _coePropertyPath = string.Empty;
            _coePropertyPath = "MultiCompoundRegistryRecord/BatchList/Batch/PropertyList"; // Path to check the Rootnode before patcher update.
            rootNode = objectConfig.SelectSingleNode(_coePropertyPath);
            XmlNode lastModPersonID_Node = rootNode.SelectSingleNode("Property[@name='LAST_MOD_PERSON_ID']");
            if (lastModPersonID_Node == null)
            {
                lastModPersonID_Node = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Property", null);
                createNewAttribute("name", "LAST_MOD_PERSON_ID", ref lastModPersonID_Node);
                createNewAttribute("friendlyName", "LAST_MOD_PERSON_ID", ref lastModPersonID_Node);
                createNewAttribute("type", "NUMBER", ref lastModPersonID_Node);
                createNewAttribute("precision", "4.0", ref lastModPersonID_Node);
                createNewAttribute("sortOrder", "0", ref lastModPersonID_Node);
                lastModPersonID_Node.AppendChild(lastModPersonID_Node.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", null));

                rootNode.AppendChild(lastModPersonID_Node);
                messages.Add("LAST_MOD_PERSON_ID node was Addded succesfully in PropertyList.");
            }
            else
            {
                messages.Add("LAST_MOD_PERSON_ID node is already available");
            }

            if (!errorsInPatch)
                messages.Add("Patch was successfully added.");
            else
                messages.Add("Patch was not successfully added");
            return messages;

            #endregion

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8231 : BugFixBaseCommand
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

            string lastModPersonIDPropertyXml = @"<Property name=""LAST_MOD_PERSON_ID"" friendlyName=""LAST_MOD_PERSON_ID"" type=""NUMBER"" precision=""8.0"" sortOrder=""69"">
               <validationRuleList>
                  <validationRule validationRuleName=""integer"" errorMessage=""This property value must be an integer number"">
                     <params>
                        <param name=""integerPart"" value=""8"" />
                        <param name=""decimalPart"" value=""0"" />
                     </params>
                  </validationRule>
                  <validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 8 characters"">
                     <params>
                        <param name=""min"" value=""0"" />
                        <param name=""max"" value=""8"" />
                     </params>
                  </validationRule>
               </validationRuleList>
            </Property>";

            if (rootNode != null)
            {
                XmlNode lastModPersonID_Node = rootNode.SelectSingleNode("Property[@name='LAST_MOD_PERSON_ID']");
                if (lastModPersonID_Node == null)
                {
                    rootNode.InnerXml += lastModPersonIDPropertyXml;
                    messages.Add("LAST_MOD_PERSON_ID node was Addded succesfully in Batch PropertyList.");
                }
                else
                {
                    messages.Add("LAST_MOD_PERSON_ID node is already available in Batch PropertyList.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Batch PropertyList node is not available in objectconfig xml");
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE2642 : BugFixBaseCommand
	{

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            #region Dataview changes
            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4004"))
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    XmlNodeList tableTemporarycompund = currentDataview.SelectNodes("//COE:table[@id='2']/COE:fields[@id]", manager);                  

                    XmlNode tableTemporarycompundNode = currentDataview.SelectSingleNode("//COE:table[@id='2']", manager);
                    XmlNode fieldTEMPBATCHID = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='201'][@name='TEMPCOMPOUNDID']", manager);
                    XmlNode fieldSTRUCTUREAGGREGATION = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='202'][@name='BASE64_CDX']", manager);
                    XmlNode fieldPERSONCREATED = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='203'][@name='PERSONCREATED']", manager);
                   
                    // For Table 2 in 4004 dataview xml
                    if (tableTemporarycompundNode != null)
                    {
                        if (tableTemporarycompundNode.Attributes["name"].Value == "VW_TEMPORARYCOMPOUND")
                        {
                            tableTemporarycompundNode.Attributes["name"].Value = "VW_TEMPORARYBATCH";
                            tableTemporarycompundNode.Attributes["alias"].Value = "VW_TEMPORARYBATCH";
                            messages.Add("4004 Dataview Table 2: '" + tableTemporarycompundNode.Attributes["name"].Value + "' Attribute Updated Successfully ");
                        }
                        else
                        {
                            messages.Add("4004 Dataview Table 2: '" + tableTemporarycompundNode.Attributes["name"].Value + "' Attribute was already Updated");
                        }
                        if (tableTemporarycompundNode.Attributes["name"].Value == "VW_TEMPORARYBATCH")
                        {
                            #region iteration
                            foreach (XmlNode fieldNode in tableTemporarycompund)
                            {

                                XmlAttribute visibleAttribute = null;

                                if (fieldTEMPBATCHID != null && fieldNode.Attributes["id"].Value == "201" && fieldNode.Attributes["name"].Value == "TEMPCOMPOUNDID")
                                {
                                    fieldTEMPBATCHID.Attributes["name"].Value = "TEMPBATCHID";
                                    messages.Add("4004 Dataview Table 2: '" + fieldNode.Attributes["name"].Value + "' name attribute is Updated Successfully for field 201");
                                }
                                else if(fieldNode.Attributes["name"].Value == "TEMPBATCHID")
                                {
                                    messages.Add("4004 Dataview Table 2: '" + fieldNode.Attributes["name"].Value + "' name attribute is already Updated for field 201 ");
                                }

                                else if (fieldSTRUCTUREAGGREGATION != null && fieldNode.Attributes["id"].Value == "202" && fieldNode.Attributes["name"].Value == "BASE64_CDX")
                                {
                                    fieldSTRUCTUREAGGREGATION.Attributes["name"].Value = "STRUCTUREAGGREGATION";
                                    messages.Add("4004 Dataview Table 2: '" + fieldNode.Attributes["name"].Value + "' name attribute is already Updated for field 202");
                                }
                                else if (fieldNode.Attributes["name"].Value == "STRUCTUREAGGREGATION")
                                {
                                    messages.Add("4004 Dataview Table 2: '" + fieldNode.Attributes["name"].Value + "' name attribute is already Updated for field 202 ");
                                }
                                else if (fieldPERSONCREATED != null && fieldNode.Attributes["id"].Value == "203" && fieldNode.Attributes["name"].Value == "PERSONCREATED")
                                {
                                    XmlAttribute lookupFieldId = fieldNode.Attributes["lookupFieldId"];
                                    XmlAttribute lookupDisplayFieldId = fieldNode.Attributes["lookupDisplayFieldId"];
                                    if (lookupFieldId == null)
                                    {
                                        visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupFieldId");
                                        fieldNode.Attributes.Append(visibleAttribute);
                                        fieldNode.Attributes["lookupFieldId"].Value = "300";
                                        messages.Add("4004 Dataview Table 2: lookupFieldId Attribute is successfully added to field 203");
                                    }
                                    else
                                    {
                                        messages.Add("4004 Dataview Table 2: lookupFieldId Attribute is already Updated in field 203");
                                    }
                                    if (lookupDisplayFieldId == null)
                                    {
                                        visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupDisplayFieldId");
                                        fieldNode.Attributes.Append(visibleAttribute);
                                        fieldNode.Attributes["lookupDisplayFieldId"].Value = "301";
                                        messages.Add("4004 Dataview Table 2: lookupDisplayFieldId Attribute is Successfully added to field 203");
                                    }
                                    else
                                    {
                                        messages.Add("4004 Dataview Table 2: lookupDisplayFieldId Attribute is already Updated in field 203");
                                    }
                                }

                            }

                            #endregion
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add(" Error: Table attribute VW_TEMPORARYCOMPOUND|VW_TEMPORARYBATCH  was not found in 4004 Dataview ");
                        }
                    }
                    else
                    {
                        errorInPatch = true;
                        messages.Add("Error: Table attribute VW_TEMPORARYCOMPOUND|VW_TEMPORARYBATCH  was not found in 4004 Dataview ");

                    }
               }
             }

            #endregion

            if (!errorInPatch)
                messages.Add("4004 Dataview was successfully updated for Upgrade machine.");
            else
                messages.Add("Error: 4004 Dataview was not successfully updated for Upgrade machine.");

            return messages;

        }
        #region Private Method
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode refnode)
        {
            XmlAttribute attributes = refnode.OwnerDocument.CreateAttribute(attributeName);
            refnode.Attributes.Append(attributes);
            refnode.Attributes[attributeName].Value = attributeValue;
        }

        #endregion

	}
}

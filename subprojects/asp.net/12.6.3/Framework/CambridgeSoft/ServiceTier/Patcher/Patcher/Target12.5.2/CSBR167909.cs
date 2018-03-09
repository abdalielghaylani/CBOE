using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class CSBR167909 : BugFixBaseCommand
	{
        /// <summary>
        /// Fix CSBR167909 for Upgradation machine(CBOE125vm1) only
        /// </summary>
      
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
                    XmlNodeList allTablesLog = currentDataview.SelectNodes("//COE:table[@id]", manager);
                   
                    // Selecting each table Fields list to iterate
                    XmlNodeList tableLogBulkRegistrationID = currentDataview.SelectNodes("//COE:table[@id='193']/COE:fields[@id]", manager);
                    XmlNodeList tableLogBulkRegistration = currentDataview.SelectNodes("//COE:table[@id='209']/COE:fields[@id]", manager);
                    XmlNodeList tableMixtureRegnumber = currentDataview.SelectNodes("//COE:table[@id='1']/COE:fields[@id]", manager);
                    XmlNodeList tableTemporarycompund = currentDataview.SelectNodes("//COE:table[@id='2']/COE:fields[@id]", manager);
                    //

                    XmlNode fieldTemp_ID = currentDataview.SelectSingleNode("//COE:table[@id='209']/COE:fields[@id='220'][@name='TEMP_ID']", manager);
                    XmlNode tableTemporarycompundNode = currentDataview.SelectSingleNode("//COE:table[@id='2']", manager);
                    XmlNode fieldTEMPBATCHID = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='201'][@name='TEMPCOMPOUNDID']", manager);
                    XmlNode fieldSTRUCTUREAGGREGATION = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='202'][@name='BASE64_CDX']", manager);
                    XmlNode fieldPERSONCREATED = currentDataview.SelectSingleNode("//COE:table[@id='2']/COE:fields[@id='203'][@name='PERSONCREATED']", manager);
                    XmlNode selectTablePeople = currentDataview.SelectSingleNode("//COE:table[@id='3']", manager);
                    XmlNode selectPeopleUser_CodeField = currentDataview.SelectSingleNode("//COE:table[@id='3']/COE:fields[@id='303']", manager);

                    // For Table 193 in 4004 dataview xml
                    foreach (XmlNode fieldNode in tableLogBulkRegistrationID)
                    {
                        XmlAttribute visibleAttribute =fieldNode.Attributes["visible"];
                        if (visibleAttribute == null)
                        {
                            visibleAttribute=fieldNode.OwnerDocument.CreateAttribute("visible");
                            fieldNode.Attributes.Append(visibleAttribute);
                            fieldNode.Attributes["visible"].Value = "false";
                            messages.Add("Successfully visible Field Attribute Added");
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add("Fields attribute was found Not Added");
                            
                        }
                    }

                    // For Table 209 in 4004 dataview xml
                    foreach (XmlNode fieldNode in tableLogBulkRegistration)
                    {
                        XmlAttribute visibleAttribute = fieldNode.Attributes["visible"];
                        if (visibleAttribute == null)
                        {
                            if (fieldTemp_ID != null && fieldNode.Attributes["id"].Value == "220")
                            {
                                fieldTemp_ID.Attributes["lookupFieldId"].Value = "201";
                                fieldTemp_ID.Attributes["lookupDisplayFieldId"].Value = "203";
                                fieldTemp_ID.Attributes["mimeType"].Value = "NONE";
                                messages.Add("Successfully lookupFieldId, lookupDisplayFieldId and mimeType Attributes was Updated for Table 209");
                            }
                            visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("visible");
                            fieldNode.Attributes.Append(visibleAttribute);
                            fieldNode.Attributes["visible"].Value = "false";
                            messages.Add("Successfully visible Fields Attribute Added");
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add("Fields attribute was found Not added");

                        }
                    }

                    // For Table 1 in 4004 dataview xml
                    foreach (XmlNode fieldNode in tableMixtureRegnumber)
                    {
                        XmlAttribute visibleAttribute = fieldNode.Attributes["visible"];
                        if (visibleAttribute == null)
                        {
                           
                            visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("visible");
                            fieldNode.Attributes.Append(visibleAttribute);
                            fieldNode.Attributes["visible"].Value = "false";
                            messages.Add("Successfully visible Field Attribute Added");
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add("Fields attribute was found Not added");

                        }
                    }

                    // For Table 2 in 4004 dataview xml
                    if (tableTemporarycompundNode != null)
                    {
                        if (tableTemporarycompundNode.Attributes["name"].Value != "VW_TEMPORARYBATCH" && tableTemporarycompundNode.Attributes["alias"].Value != "VW_TEMPORARYBATCH")
                        {
                            tableTemporarycompundNode.Attributes["name"].Value = "VW_TEMPORARYBATCH";
                            tableTemporarycompundNode.Attributes["alias"].Value = "VW_TEMPORARYBATCH";
                            messages.Add("Successfully name and alias Attributes were Updated for Table 2");
                        }
                        else
                            messages.Add("Name and Alias Attributes were not Updated for Table 2");
                    }
                    foreach (XmlNode fieldNode in tableTemporarycompund)
                    {

                        XmlAttribute visibleAttribute = fieldNode.Attributes["visible"];
                        if (visibleAttribute == null)
                        {
                            if (fieldTEMPBATCHID != null && fieldNode.Attributes["id"].Value == "201")
                            {
                                fieldTEMPBATCHID.Attributes["name"].Value = "TEMPBATCHID";
                                messages.Add("Successfully name Attribute was Updated for field 201");
                            }

                            if (fieldSTRUCTUREAGGREGATION != null && fieldNode.Attributes["id"].Value == "202")
                            {
                                fieldSTRUCTUREAGGREGATION.Attributes["name"].Value = "STRUCTUREAGGREGATION";
                                messages.Add("Successfully name Attribute was Updated for Field 202");
                            }
                            if (fieldPERSONCREATED != null && fieldNode.Attributes["id"].Value == "203")
                            {
                                visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupFieldId");
                                fieldNode.Attributes.Append(visibleAttribute);
                                fieldNode.Attributes["lookupFieldId"].Value = "300";
                                visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupDisplayFieldId");
                                fieldNode.Attributes.Append(visibleAttribute);
                                fieldNode.Attributes["lookupDisplayFieldId"].Value = "301";
                                messages.Add("Successfully lookupFieldId and lookupDisplayFieldId Attribute were Added");
                            }
                                
                            visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("visible");
                            fieldNode.Attributes.Append(visibleAttribute);
                            fieldNode.Attributes["visible"].Value = "false";
                            messages.Add("Successfully visible Field Attribute was Added");
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add("Field attribute was found Not Added");

                        }
                    }

                    // For Table 3 in 4004 dataview xml
                    XmlNodeList tablePeople = currentDataview.SelectNodes("//COE:table[@id='3']/COE:fields[@id]", manager);
                    foreach (XmlNode fieldNode in tablePeople)
                    {
                        XmlAttribute visibleAttribute = fieldNode.Attributes["visible"];
                        if (visibleAttribute == null)
                        {
                            visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("visible");
                            fieldNode.Attributes.Append(visibleAttribute);
                            fieldNode.Attributes["visible"].Value = "false";
                            messages.Add("Successfully visible Field Attribute was Added");
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add("Fields attribute was found Not Added");
                        }
                    }

                    if (selectPeopleUser_CodeField == null)
                    {
                        XmlNode user_codeNode = selectTablePeople.OwnerDocument.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                        createNewAttribute("id", "303", ref user_codeNode);
                        createNewAttribute("name", "USER_CODE", ref user_codeNode);
                        createNewAttribute("dataType", "TEXT", ref user_codeNode);
                        createNewAttribute("alias", "USER_CODE", ref user_codeNode);
                        createNewAttribute("indexType", "NONE", ref user_codeNode);
                        createNewAttribute("mimeType", "NONE", ref user_codeNode);
                        createNewAttribute("visible", "false", ref user_codeNode);
                        selectTablePeople.AppendChild(user_codeNode);
                        messages.Add("Successfully USER_CODE Node Added for Table 3.");
                    }
                 
               }
            }

            #endregion

            if (!errorInPatch)
                messages.Add("4004 Dataview was successfully updated for Upgradation fix.");
            else
                messages.Add("4004 Dataview was not successfully updated for Upgradation fix.");

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

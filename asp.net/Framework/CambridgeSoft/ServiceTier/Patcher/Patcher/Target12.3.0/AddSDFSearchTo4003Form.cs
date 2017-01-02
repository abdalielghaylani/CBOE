using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding new element 'SDF Search' to 4003 form
    /// </summary>
    public class AddSDFSearchTo4003Form : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4003")
                {
                    XmlNode selectedNode = doc.SelectSingleNode("//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", manager);
                    XmlNode SDFNode = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='SDF_upload']", manager);
                    XmlNode InsertAfterNode = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='IdentifierValue']", manager);
                    XmlNode InsertBeforeNode = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='REG_COMMENTS']", manager);

                    if (selectedNode != null)
                    {
                        // add node if SDF Node doesn't exists
                        if (SDFNode == null)
                        {
                            SDFNode = doc.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");

                            // adding attribute
                            XmlAttribute NameAttribute = SDFNode.OwnerDocument.CreateAttribute("name");
                            SDFNode.Attributes.Append(NameAttribute);
                            SDFNode.Attributes["name"].Value = "SDF_upload";

                            // adding child tags
                            XmlNode labelNode = doc.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                            labelNode.InnerText = "SDF Search";
                            XmlNode showHelpNode = doc.CreateNode(XmlNodeType.Element, "showHelp", "COE.FormGroup");
                            showHelpNode.InnerText = "false";
                            XmlNode isFileUploadNode = doc.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                            isFileUploadNode.InnerText = "false";
                            XmlNode pageComunicationProviderNode = doc.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                            XmlNode fileUploadBindingExpressionNode = doc.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                            XmlNode helpTextNode = doc.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                            XmlNode defaultValueNode = doc.CreateNode(XmlNodeType.Element, "defaultValue", "COE.FormGroup");
                            XmlNode bindingExpressionNode = doc.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                            bindingExpressionNode.InnerText = "SearchCriteria[50].Criterium.Value"; // To be dyanamic later
                            XmlNode searchCriteriaItemNode = doc.CreateNode(XmlNodeType.Element, "searchCriteriaItem", "COE.FormGroup");
                            XmlAttribute idAttribute = searchCriteriaItemNode.OwnerDocument.CreateAttribute("id");
                            searchCriteriaItemNode.Attributes.Append(idAttribute);
                            searchCriteriaItemNode.Attributes["id"].Value = "50";
                            XmlAttribute searchLookupByID = searchCriteriaItemNode.OwnerDocument.CreateAttribute("searchLookupByID");
                            searchCriteriaItemNode.Attributes.Append(searchLookupByID);
                            searchCriteriaItemNode.Attributes["searchLookupByID"].Value = "true";
                            XmlAttribute aggregateFunctionName = searchCriteriaItemNode.OwnerDocument.CreateAttribute("aggregateFunctionName");
                            searchCriteriaItemNode.Attributes.Append(aggregateFunctionName);
                            searchCriteriaItemNode.Attributes["aggregateFunctionName"].Value = "";
                            XmlAttribute fieldidAttribute = searchCriteriaItemNode.OwnerDocument.CreateAttribute("fieldid");
                            searchCriteriaItemNode.Attributes.Append(fieldidAttribute);
                            searchCriteriaItemNode.Attributes["fieldid"].Value = "1701";
                            XmlAttribute tableidAttribute = searchCriteriaItemNode.OwnerDocument.CreateAttribute("tableid");
                            searchCriteriaItemNode.Attributes.Append(tableidAttribute);
                            searchCriteriaItemNode.Attributes["tableid"].Value = "17";
                            searchCriteriaItemNode.InnerXml = "<structureListCriteria negate=\"NO\" xmlns=\"COE.FormGroup\" />";

                            XmlNode idNode = doc.CreateNode(XmlNodeType.Element, "Id", "COE.FormGroup");
                            idNode.InnerText = "SDFFileInput";
                            XmlNode configInfoNode = doc.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                            configInfoNode.InnerXml = "<fieldConfig xmlns=\"COE.FormGroup\"><Width >240px</Width><CSSLabelClass xmlns=\"COE.FormGroup\">FELabel</CSSLabelClass></fieldConfig>";
                            XmlNode displayInfoNode = doc.CreateNode(XmlNodeType.Element, "displayInfo", "COE.FormGroup");
                            displayInfoNode.InnerXml = "<cssClass xmlns=\"COE.FormGroup\">SDF42x40</cssClass><type xmlns=\"COE.FormGroup\">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEListUpload</type><visible xmlns=\"COE.FormGroup\">true</visible>";

                            XmlNode validationRuleList = doc.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                            XmlNode serverEvents = doc.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                            XmlNode clientEvents = doc.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");

                            XmlNode dataSource = doc.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                            XmlNode dataSourceId = doc.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                            XmlNode requiredStyle = doc.CreateNode(XmlNodeType.Element, "requiredStyle", "COE.FormGroup");
                            XmlNode displayData = doc.CreateNode(XmlNodeType.Element, "displayData", "COE.FormGroup");


                            SDFNode.AppendChild(labelNode);
                            SDFNode.AppendChild(showHelpNode);
                            SDFNode.AppendChild(isFileUploadNode);
                            SDFNode.AppendChild(pageComunicationProviderNode);
                            SDFNode.AppendChild(fileUploadBindingExpressionNode);
                            SDFNode.AppendChild(helpTextNode);
                            SDFNode.AppendChild(defaultValueNode);
                            SDFNode.AppendChild(bindingExpressionNode);
                            SDFNode.AppendChild(idNode);
                            SDFNode.AppendChild(displayInfoNode);
                            SDFNode.AppendChild(validationRuleList);
                            SDFNode.AppendChild(serverEvents);
                            SDFNode.AppendChild(clientEvents);
                            SDFNode.AppendChild(configInfoNode);
                            SDFNode.AppendChild(dataSource);
                            SDFNode.AppendChild(dataSourceId);
                            SDFNode.AppendChild(requiredStyle);
                            SDFNode.AppendChild(searchCriteriaItemNode);
                            SDFNode.AppendChild(displayData);
                            if (InsertAfterNode != null)
                            {
                                selectedNode.InsertAfter(SDFNode, InsertAfterNode);
                            }
                            else if (InsertBeforeNode != null)
                            {
                                selectedNode.InsertBefore(SDFNode, InsertBeforeNode);
                            }
                            else
                            {
                                selectedNode.AppendChild(SDFNode);
                            }

                            messages.Add("SDF field added.");
                        }
                        else
                        {
                            messages.Add("SDF field already exists.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Patcher was not added.");
                    }
                    break;
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("SDF was successfully patched");
            }
            else
                messages.Add("SDF was patched with errors");

            return messages;
        }
    }
}

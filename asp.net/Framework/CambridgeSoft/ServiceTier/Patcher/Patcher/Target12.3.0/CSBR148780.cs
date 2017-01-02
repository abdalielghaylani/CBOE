using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patcher For CSBR-148780: Adds formElement[@name="STRUCT_COMMENTS"] node to coeForm[4011.xml]-viewMode.
    /// </summary>
	class CSBR148780 : BugFixBaseCommand
	{
        #region Abstract Method
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region 4011.xml
                if (id == "4011")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:detailsForms[@defaultForm='0']/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (rootSelectedNode != null)
                    {
                        messages.Add(createNewFormElement(coeFormPath, manager, ref rootSelectedNode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:viewMode was not found in form 4011 to add formElement[@name='STRUCT_COMMENTS'].");
                    }
                    break;
                }
                #endregion
            }
            #endregion

            if (!errorsInPatch)
                messages.Add("CSBR148780 was successfully fixed.");
            else
                messages.Add("CSBR148780 was fixed with errors.");
            return messages;
        }
        #endregion

        #region Private Function
        private string createNewFormElement(string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            String formMode = "viewMode";
            String formElementName = "STRUCT_COMMENTS";
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            XmlNode insertAfterNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='Identifiers']", manager);
            XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='STRUCT_NAME']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", null);
                label.InnerText = "Structure Comments";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", null);
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", null);
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", null);
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", null);
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", null);
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", null);
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", null);
                bindingExpression.InnerText = "Compound.BaseFragment.Structure.PropertyList[@Name='STRUCT_COMMENTS'| Value]";
                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", null);
                id.InnerText = "STRUCT_COMMENTSProperty";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", null);
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", null));
                displayInfo.SelectSingleNode("cssClass", manager).InnerText = "Std100x80";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", null));
                displayInfo.SelectSingleNode("type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", null));
                displayInfo.SelectSingleNode("visible", manager).InnerText = "true";
                //--------------------
                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", null);
                XmlNode serverEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "serverEvents", null);
                XmlNode clientEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "clientEvents", null);
                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSLabelClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextAreaViewMode";
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", null);
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", null);
                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", null);

                formElementNode.AppendChild(label);
                formElementNode.AppendChild(showHelp);
                formElementNode.AppendChild(isFileUpload);
                formElementNode.AppendChild(pageComunicationProvider);
                formElementNode.AppendChild(fileUploadBindingExpression);
                formElementNode.AppendChild(helpText);
                formElementNode.AppendChild(defaultValue);
                formElementNode.AppendChild(bindingExpression);
                formElementNode.AppendChild(id);
                formElementNode.AppendChild(displayInfo);
                formElementNode.AppendChild(validationRuleList);
                formElementNode.AppendChild(serverEvents);
                formElementNode.AppendChild(clientEvents);
                formElementNode.AppendChild(configInfo);
                formElementNode.AppendChild(dataSource);
                formElementNode.AppendChild(dataSourceId);
                formElementNode.AppendChild(displayData);

                // attach new node at specified position or append
                if (insertAfterNode != null)
                {
                    rootSelectedNode.InsertAfter(formElementNode, insertAfterNode);
                }
                else if (insertBeforeNode != null)
                {
                    rootSelectedNode.InsertBefore(formElementNode, insertBeforeNode);
                }
                else
                {
                    rootSelectedNode.AppendChild(formElementNode);
                }
                return "Form[4011:" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' added succesfully.";
            }
            else
            {
                return "Form[4011:" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' already exists.";
            }
        }
        #endregion

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

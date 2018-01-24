using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  changelist # 440437 and 440442
    /// </summary>
    class CSBR118435 : BugFixBaseCommand
    {

        #region Variable
        int _loopCount = 0;
        #endregion

        #region Private Property
        private int LoopCount
        {
            get
            {
                return _loopCount;
            }
            set
            {
                _loopCount = _loopCount + value;
            }
        }
        #endregion

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
                                
                #region 4002.xml  4003.xml
                if (id == "4002" || id=="4003")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region EditMode
                    coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        messages.Add(createNewFormElement(id, coeFormPath, manager, ref selectedNode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:layoutInfo was not found in form " + id + " to create formelement.");
                    }
                    #endregion

                    
                }
                #endregion

                #region Validate Loopcount
                if (LoopCount == 2)
                {
                    LoopCount = -1;
                    break;
                }
                #endregion
            }
            #endregion

            if (!errorsInPatch)
                messages.Add("CSBR-118435 Workflow was successfully fixed.");
            else
                messages.Add("CSBR-118435 Workflow  was fixed with partial update.");
            return messages;
        }

        #endregion

        #region Private Function
        private string createNewFormElement(string formId, string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            string formElementName = "COMPONENTNUMBER";
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            XmlNode insertAfterNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='Status']", manager);
            XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='PERSONCREATED']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                label.InnerText = "Number of Components";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", "COE.FormGroup");
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", "COE.FormGroup");
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", "COE.FormGroup");
                id.InnerText = "ComponentNumberStateControl";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", "COE.FormGroup");
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:cssClass", manager).InnerText = "Std25x40";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl";

                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:visible", manager).InnerText = "true";
                //--------------------

                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                XmlNode serverEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "serverEvents", "COE.FormGroup");
                XmlNode clientEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "clientEvents", "COE.FormGroup");

                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "DisplayType", "COE.FormGroup"));
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSLabelClass", "COE.FormGroup"));
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSClass", "COE.FormGroup"));
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ItemCSSClass", "COE.FormGroup"));

                // States
                //------------------------
                XmlNode States = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "States", "COE.FormGroup");
                for (int i = 0; i<=2; i++)
                {
                    XmlNode State = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "State", "COE.FormGroup");
                    createNewAttribute("text", "", ref State);
                    createNewAttribute("value", "", ref State);
                    States.AppendChild(State);
                }
                States.ChildNodes[0].Attributes["text"].Value = "Any";
                States.ChildNodes[0].Attributes["value"].Value = "";

                States.ChildNodes[1].Attributes["text"].Value = "Single Component";
                States.ChildNodes[1].Attributes["value"].Value = "1";

                States.ChildNodes[2].Attributes["text"].Value = "More than one component";
                States.ChildNodes[2].Attributes["value"].Value = "1";
                //------------------------
                fieldConfig.AppendChild(States);
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "DefaultSelectedValue", "COE.FormGroup"));

                fieldConfig.SelectSingleNode("COE:DisplayType", manager).InnerText = "DropDown";
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FEDropDownList";
                fieldConfig.SelectSingleNode("COE:ItemCSSClass", manager).InnerText = "ImageButton";
                
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                XmlNode searchCriteriaItem = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "searchCriteriaItem", "COE.FormGroup");
                createNewAttribute("fieldid","",ref  searchCriteriaItem);
                createNewAttribute("id", "",ref searchCriteriaItem);
                createNewAttribute("tableid", "", ref searchCriteriaItem);
                createNewAttribute("aggregateFunctionName", "", ref searchCriteriaItem);
                XmlNode numericalCriteria = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "numericalCriteria", "COE.FormGroup");
                createNewAttribute("negate", "", ref numericalCriteria);
                createNewAttribute("trim", "", ref numericalCriteria);
                createNewAttribute("operator", "", ref numericalCriteria);
                searchCriteriaItem.AppendChild(numericalCriteria);
                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", "COE.FormGroup");

                switch (formId)
                {
                    case "4002":
                        int searchCriteriaId = 0; // Code block to find the search criteria id dynamically fix for cboe-2067
                        XmlNodeList searchCriteriaIds = rootSelectedNode.SelectNodes("//COE:searchCriteriaItem/@id", manager);
                        foreach(XmlNode idAttrib in searchCriteriaIds)
                        {
                            if (int.Parse(idAttrib.Value) >= searchCriteriaId)
                                searchCriteriaId = int.Parse(idAttrib.Value) + 1;
                        }
                        bindingExpression.InnerText = "SearchCriteria[" + searchCriteriaId.ToString() + "].Criterium.Value";
                        searchCriteriaItem.Attributes["fieldid"].Value = "200";
                        searchCriteriaItem.Attributes["id"].Value = searchCriteriaId.ToString();
                        searchCriteriaItem.Attributes["tableid"].Value = "2";
                        searchCriteriaItem.Attributes["aggregateFunctionName"].Value = "count";
                        numericalCriteria.Attributes["negate"].Value = "NO";
                        numericalCriteria.Attributes["trim"].Value = "NONE";
                        numericalCriteria.Attributes["operator"].Value = "EQUAL";
                        break;
                    case "4003":
                         int searchCriteriaIdCompound = 0;// Code block to find the search criteria id dynamically fix for cboe-2067
                         XmlNodeList searchCriteriaCompound = rootSelectedNode.SelectNodes("//COE:searchCriteriaItem/@id", manager);
                         foreach (XmlNode idAttrib in searchCriteriaCompound)
                        {
                            if (int.Parse(idAttrib.Value) >= searchCriteriaIdCompound)
                                searchCriteriaIdCompound = int.Parse(idAttrib.Value) + 1;
                        }
                         bindingExpression.InnerText = "SearchCriteria[" + searchCriteriaIdCompound.ToString() + "].Criterium.Value";
                        searchCriteriaItem.Attributes["fieldid"].Value = "402";
                        searchCriteriaItem.Attributes["id"].Value = searchCriteriaIdCompound.ToString();
                        searchCriteriaItem.Attributes["tableid"].Value = "4";
                        searchCriteriaItem.Attributes["aggregateFunctionName"].Value = "count";
                        numericalCriteria.Attributes["negate"].Value = "NO";
                        numericalCriteria.Attributes["trim"].Value = "NONE";
                        numericalCriteria.Attributes["operator"].Value = "EQUAL";
                        break;
                }

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
                formElementNode.AppendChild(searchCriteriaItem);
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
                return "Form["+formId+"]:The formelement node with name='" + formElementName + "' added succesfully.";
            }
            else
            {
                return "Form[" + formId + "]:The formelement node with name='" + formElementName + "' already exits.";
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

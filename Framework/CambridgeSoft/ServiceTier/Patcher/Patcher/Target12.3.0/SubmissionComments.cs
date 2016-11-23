using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  changelist # 415189 and 416120 
    /// </summary>
    class SubmissionComments : BugFixBaseCommand
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
            string coeDataViewPath = string.Empty;
            int fieldId = 0;

            #region Dataview Changes:
            foreach (XmlDocument dataviewDoc in dataviews)
            {
                string id = dataviewDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataviewDoc.DocumentElement.Attributes["dataviewid"].Value;

                if (id == "4002")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='1']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        fieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        fieldId = fieldId + 1;

                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SUBMISSIONCOMMENTS']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", fieldId.ToString(), ref fields);
                            createNewAttribute("name", "SUBMISSIONCOMMENTS", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("alias", "SUBMISSIONCOMMENTS", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[4002]:This field node with id='" + fieldId + "' added succesfully.");
                        }
                        else
                        {
                            fieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[4002]:This field node with id='" + fieldId + "' already exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[4002]:VW_TEMPORARYBATCH was not found on dataview 4002.");
                    }
                }

                #region Validate Loopcount
                if (LoopCount == 1)
                {
                    LoopCount = -1;
                    break;
                }
                #endregion
            }
            #endregion

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region 4002.xml
                if (id == "4002")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region From Element
                    coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table [@name='Table_1']/COE:Columns/COE:Column [@name='Review Record']"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    string formElementName = "SubmitterComments";
                    if (rootSelectedNode != null)
                    {
                        XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
                        XmlNode insertAfterNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='TEMPBATCHID']", manager);
                        if (formElementNode == null)
                        {
                            formElementNode = formDoc.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                            createNewAttribute("name", formElementName, ref formElementNode);

                            // create child nodes
                            XmlNode configInfo = formDoc.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                            XmlNode fieldConfig = formDoc.CreateNode(XmlNodeType.Element, "fieldConfig", "COE.FormGroup");
                            XmlNode cssClass = formDoc.CreateNode(XmlNodeType.Element, "CSSClass", "COE.FormGroup");

                            // append child nodes
                            cssClass.InnerText = "FETableItem2";
                            fieldConfig.AppendChild(cssClass);
                            configInfo.AppendChild(fieldConfig);
                            formElementNode.AppendChild(configInfo);

                            // attach new rootnode at specified place
                            if (insertAfterNode != null)
                            {
                                rootSelectedNode.InsertAfter(formElementNode, insertAfterNode);
                            }
                            else
                            {
                                rootSelectedNode.AppendChild(formElementNode);
                            }
                            messages.Add("Form[4002]:The formelement node with name='SubmitterComments' added succesfully.");
                        }
                        else
                        {
                            messages.Add("Form[4002]:The formelement node with name='SubmitterComments' already exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4002]:Column reviewrecord was not found in form 4002 to append formelement.");
                    }
                    #endregion

                    #region Result Criteria
                    coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COER:resultsCriteria/COER:tables/COER:table[@id='1']";// Path to check the Rootnode before patcher update.
                    manager.AddNamespace("COER", "COE.ResultsCriteria");
                    XmlNode tableSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    
                    if (tableSelectedNode != null)
                    {
                        XmlNode switchElementNode = tableSelectedNode.SelectSingleNode(coeFormPath + "/COER:switch [@alias='SubmitterComments']", manager);
                        XmlNode insertAfterFieldNode = tableSelectedNode.SelectSingleNode(coeFormPath + "/COER:field[@fieldId='1503']", manager);
                        if (switchElementNode == null)
                        {
                            switchElementNode = formDoc.CreateNode(XmlNodeType.Element, "switch", "COE.ResultsCriteria");
                            createNewAttribute("alias", "SubmitterComments", ref switchElementNode);
                            createNewAttribute("inputType", "text", ref switchElementNode);

                            // create child nodes
                            XmlNode sqlFunction = formDoc.CreateNode(XmlNodeType.Element, "SQLFunction", "COE.ResultsCriteria");
                            createNewAttribute("functionName", "NVL", ref sqlFunction);
                            //  sub child nodes
                            //--------------------
                            XmlNode field = formDoc.CreateNode(XmlNodeType.Element, "field", "COE.ResultsCriteria");
                            createNewAttribute("fieldId", fieldId.ToString(), ref field);
                            sqlFunction.AppendChild(field);
                            sqlFunction.AppendChild(formDoc.CreateNode(XmlNodeType.Element, "literal", "COE.ResultsCriteria"));
                            sqlFunction.SelectSingleNode("COER:literal", manager).InnerText = "'nullitem'";
                            //--------------------

                            XmlNode conditions = formDoc.CreateNode(XmlNodeType.Element, "conditions", "COE.ResultsCriteria");
                            //  sub child nodes
                            //--------------------
                            XmlNode condition1 = formDoc.CreateNode(XmlNodeType.Element, "condition", "COE.ResultsCriteria");
                            createNewAttribute("value", "nullitem", ref  condition1);
                            condition1.AppendChild(formDoc.CreateNode(XmlNodeType.Element, "literal", "COE.ResultsCriteria"));
                            condition1.SelectSingleNode("COER:literal", manager).InnerText = "''";
                            XmlNode condition2 = formDoc.CreateNode(XmlNodeType.Element, "condition", "COE.ResultsCriteria");
                            createNewAttribute("default", "true", ref  condition2);
                            condition2.AppendChild(formDoc.CreateNode(XmlNodeType.Element, "literal", "COE.ResultsCriteria"));
                            condition2.SelectSingleNode("COER:literal", manager).InnerText = "'Dups Found'";
                            conditions.AppendChild(condition1);
                            conditions.AppendChild(condition2);
                            //--------------------

                            switchElementNode.AppendChild(sqlFunction);
                            switchElementNode.AppendChild(conditions);


                            // attach new rootnode at specified place
                            if (insertAfterFieldNode != null)
                            {
                                tableSelectedNode.InsertAfter(switchElementNode, insertAfterFieldNode);
                            }
                            else
                            {
                                tableSelectedNode.AppendChild(switchElementNode);
                            }
                            messages.Add("Form[4002]:The switch node with alias='SubmitterComments' added succesfully.");
                        }
                        else
                        {
                            messages.Add("Form[4002]:The switch node with alias='SubmitterComments' already exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4002]:table with id='1' was not found in form 4002 to append switchelement.");
                    }
                    #endregion


                }
                #endregion

                #region 4011.xml
                if (id == "4011")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region EditMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_editMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_editMode != null)
                    {
                        messages.Add(createNewFormElement("editMode", coeFormPath, manager, ref selectedNode_editMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region ViewMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_viewMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_viewMode != null)
                    {
                        messages.Add(createNewFormElement("viewMode", coeFormPath, manager, ref selectedNode_viewMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
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
                messages.Add("Submission Workflow was successfully fixed.");
            else
                messages.Add("Submission Workflow  was fixed with errors.");
            return messages;
        }

        #endregion

        #region Private Function
        private string createNewFormElement(string formMode, string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            string formElementName = "SubmissionComments";
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            XmlNode insertAfterNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='Status']", manager);
            XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='StructureAggregation']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                label.InnerText = "Submission Comments";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", "COE.FormGroup");
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", "COE.FormGroup");
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", null);
                bindingExpression.InnerText = "SubmissionComments";
                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", "COE.FormGroup");
                id.InnerText = "SubmissionCommentsID";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", "COE.FormGroup");
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "style", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:style", manager).InnerText = "margin-top:5px;";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:cssClass", manager).InnerText = "Std100x80";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", "COE.FormGroup"));
                switch (formMode.ToLower())
                {
                    case "editmode":
                        displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea";
                        break;
                    case "viewmode":
                        displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly";
                        break;
                }
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:visible", manager).InnerText = "true";
                //--------------------

                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", null);
                // sub child
                //--------------------
                switch (formMode.ToLower())
                {
                    case "editmode":
                        XmlNode validationRule = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", null);
                        createNewAttribute("validationRuleName", "textLength", ref validationRule);
                        createNewAttribute("errorMessage", "The property value can have between 0 and 200 characters", ref validationRule);
                        createNewAttribute("displayPosition", "Top_Left", ref validationRule);

                        XmlNode param1 = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "param", "COE.FormGroup");
                        createNewAttribute("name", "min", ref  param1);
                        createNewAttribute("value", "0", ref param1);

                        XmlNode param2 = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "param", "COE.FormGroup");
                        createNewAttribute("name", "max", ref param2);
                        createNewAttribute("value", "200", ref param2);

                        validationRule.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "params", "COE.FormGroup"));
                        validationRule.SelectSingleNode("COE:params", manager).AppendChild(param1);
                        validationRule.SelectSingleNode("COE:params", manager).AppendChild(param2);
                        validationRuleList.AppendChild(validationRule);
                        break;
                }
                //--------------------
                XmlNode serverEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "serverEvents", "COE.FormGroup");
                XmlNode clientEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "clientEvents", "COE.FormGroup");

                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSLabelClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSClass", "COE.FormGroup"));
                switch (formMode.ToLower())
                {
                    case "editmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextArea";
                        break;
                    case "viewmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextAreaViewMode";
                        break;
                }
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                XmlNode requiredStyle = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "requiredStyle", "COE.FormGroup");
                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", "COE.FormGroup");

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
                formElementNode.AppendChild(requiredStyle);
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
                return "Form[4011:" + formMode.ToUpper() + "]:The formelement node with name='SubmitterComments' added succesfully.";
            }
            else
            {
                return "Form[4011:" + formMode.ToUpper() + "]:The formelement node with name='SubmissionComments' already exits.";
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

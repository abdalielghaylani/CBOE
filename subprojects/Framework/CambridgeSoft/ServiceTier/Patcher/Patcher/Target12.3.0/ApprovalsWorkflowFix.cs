using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class ApprovalsWorkflowFix : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            #region FrameworkConfig changes:

            XmlNode regReviewRegLink = frameworkConfig.SelectSingleNode("//add[@name='RegistrationPanelContents']/links/add[@name='ReviewRegister']");
            if (regReviewRegLink == null)
            {
                errorInPatch = true;
                messages.Add("Registration main panel has no review register link, and thus no action is required");
            }
            else if(regReviewRegLink.Attributes["url"] != null && !string.IsNullOrEmpty(regReviewRegLink.Attributes["url"].Value))
            {
                regReviewRegLink.Attributes["url"].Value = regReviewRegLink.Attributes["url"].Value.Replace("Caller=RR", "Caller=ALL");
                messages.Add("Url for review register link in registration main panel was successfully updated");
            }

            XmlNode managerRegRRLink = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Registration']/links/add[@name='ReviewRegister']");
            
            if (managerRegRRLink == null)
            {
                errorInPatch = true;
                messages.Add("ReviewRegister link was not found on COEManager home settings , and thus no action is required");
            }
            else if(managerRegRRLink.Attributes["url"] != null && !string.IsNullOrEmpty(managerRegRRLink.Attributes["url"].Value))
            {
                managerRegRRLink.Attributes["url"].Value = managerRegRRLink.Attributes["url"].Value.Replace("Caller=RR", "Caller=ALL");
                messages.Add("Url for review register link in manager main panel was successfully updated");
            }

            XmlNode regDash = frameworkConfig.SelectSingleNode("//add[@name='RegistrationDash1']/customItems");
            if (regDash == null)
            {
                errorInPatch = true;
                messages.Add("Registration dashboard configuration was not found in framework config.");
            }
            else
            {
                if (regDash.SelectNodes("./add[@name='ApprovedTempRegistries']").Count > 0)
                {
                    messages.Add("ApprovedTempRegistries is already present in registration dashboard");
                }
                else
                {
                    //Records to register
                    XmlNode approvedTempRegNode = frameworkConfig.CreateElement("add");
                    approvedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("name")).Value = "ApprovedTempRegistries";
                    approvedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("display"));
                    approvedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("privilege")).Value = "REGISTER_TEMP";
                    approvedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("className")).Value = "CambridgeSoft.COE.Registration.Services.RegSystem.GetApprovedTempRegistriesCount";
                    approvedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("assembly")).Value = "CambridgeSoft.COE.Registration.Services";
                    approvedTempRegNode.InnerXml = @"<itemConfig>
                    <add name=""cssForText"" value=""DashBoardText"" />
                    <add name=""text"" value=""&lt;a href='/COERegistration/Forms/ReviewRegister/ContentArea/ReviewRegisterSearch.aspx?Caller=AR'&gt;{0} records to register&lt;/a&gt;"" />
                  </itemConfig>";
                    regDash.InsertBefore(approvedTempRegNode, regDash.FirstChild);
                    messages.Add("Approved temp regiesties link added to Registration's dashboard");
                }

                if (regDash.SelectNodes("./add[@name='SubmittedTempRegistries']").Count > 0)
                {
                    messages.Add("SubmittedTempRegistries is already present in registration dashboard");
                }
                else
                {
                    //Records to approve
                    XmlNode submittedTempRegNode = frameworkConfig.CreateElement("add");
                    submittedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("name")).Value = "SubmittedTempRegistries";
                    submittedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("display"));
                    submittedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("privilege")).Value = "SET_APPROVED_FLAG";
                    submittedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("className")).Value = "CambridgeSoft.COE.Registration.Services.RegSystem.GetTempRegistriesCount";
                    submittedTempRegNode.Attributes.Append(frameworkConfig.CreateAttribute("assembly")).Value = "CambridgeSoft.COE.Registration.Services";
                    submittedTempRegNode.InnerXml = @"<itemConfig>
                        <add name=""cssForText"" value=""DashBoardText""/>
                        <add name=""text"" value=""&lt;a href='/COERegistration/Forms/ReviewRegister/ContentArea/ReviewRegisterSearch.aspx?Caller=RR'&gt;{0} records to approve&lt;/a&gt;""/>
                      </itemConfig>";
                    regDash.InsertBefore(submittedTempRegNode, regDash.FirstChild);
                    messages.Add("Approved temp regiesties link added to Registration's dashboard");
                }
                
                XmlNode approvedPermRegNode = regDash.SelectSingleNode("./add[@name='ApprovedPermRegistries']");
                if (approvedPermRegNode != null)
                {
                    regDash.RemoveChild(approvedPermRegNode);
                    messages.Add("Old approvals link removed from registration dashboard");
                }
                else
                {
                    messages.Add("ApprovedPermRegistries dasboard item was already removed");
                }

                XmlNode tempRegistriesNode = regDash.SelectSingleNode("./add[@name='TempRegistries']/add[@name='text']");
                if (tempRegistriesNode == null)
                {
                    errorInPatch = true;
                    messages.Add("TempRegistries dashboard item was not found on reg config.");
                }
                else if(tempRegistriesNode.Attributes["value"] != null && !string.IsNullOrEmpty(tempRegistriesNode.Attributes["value"].Value))
                {
                    tempRegistriesNode.Attributes["value"].Value = tempRegistriesNode.Attributes["value"].Value.Replace("Caller=RR", "Caller=ALL");
                    messages.Add("Successfully updated the url for TempRegistries dashboard item");
                }
                
            }
            
            XmlNode tempActionLinks = frameworkConfig.SelectSingleNode("//formBehaviour/form[@formId='4002']/actionLinks");
            if (tempActionLinks == null)
            {
                errorInPatch = true;
                messages.Add("There was no actionLinks configured for searching temporary records");
            }
            else
            {
                if (tempActionLinks.SelectNodes("./link[@id='ApproveMarkedLink']").Count > 0)
                {
                    messages.Add("ApproveMarkedLink was already added in framework config");
                }
                else
                {
                    XmlNode approvedMarkedLink = frameworkConfig.CreateElement("link");
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("id")).Value = "ApproveMarkedLink";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("href")).Value = "/COERegistration/Forms/BulkRegisterMarked/ContentArea/ApproveMarked.aspx?COEHitListID={0}";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("text")).Value = "Approve Marked";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("target")).Value = "_parent";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("tooltip")).Value = "Registration - Approve";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("cssClass")).Value = "MenuItemLink";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("enabled")).Value = "true";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("privileges")).Value = "SET_APPROVED_FLAG";
                    approvedMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("confirmationMessage")).Value = "Are you sure you want to approve the marked registries?";
                    tempActionLinks.AppendChild(approvedMarkedLink);
                    messages.Add("Approved temporary records actionLink was successfully added");
                }
            }

            #endregion


            #region Dataview changes:
            int fieldId = 1503;
            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4002"))
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    XmlNode tableTemporaryBatch = currentDataview.SelectSingleNode("//COE:table[@id='1']", manager);

                    if (tableTemporaryBatch == null)
                    {
                        errorInPatch = true;
                        messages.Add("VW_TEMPORARYBATCH was not found on dataview 4002");
                    }
                    else
                    {
                        if (tableTemporaryBatch.SelectNodes("./COE:fields[@name='STATUSID']", manager).Count > 0)
                        {
                            messages.Add("Dataview 4002 already containes a definition for StatusID at batch level");
                        }
                        else
                        {
                            XmlNode maxField = tableTemporaryBatch.SelectSingleNode("COE:fields[not(@id <= preceding-sibling::COE:fields/@id) and not(@id <= following-sibling::COE:fields/@id)]/@id", manager);
                            fieldId = int.Parse(maxField.Value) + 1;
                            XmlNode statusIdFld = currentDataview.CreateElement("fields", "COE.COEDataView");
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("id")).Value = fieldId.ToString();
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("name")).Value = "STATUSID";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("alias")).Value = "STATUSID";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("dataType")).Value = "INTEGER";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("indexType")).Value = "NONE";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("mimeType")).Value = "NONE";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("visible")).Value = "1";
                            statusIdFld.Attributes.Append(currentDataview.CreateAttribute("lookupSortOrder")).Value = "ASCENDING";
                            tableTemporaryBatch.AppendChild(statusIdFld);
                            messages.Add("StatusID field was successfully added to VW_TEMPORARYBATCH table in the dataview 4002");

                        }
                    }
                }
            }
            #endregion

            #region Form changes:

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;

                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                
                if (id.Equals("4002"))
                {
                    int searchCriteriaId = 0;
                    XmlNodeList searchCriteriaIds = doc.SelectNodes("//COE:searchCriteriaItem/@id", manager);
                    foreach(XmlNode idAttrib in searchCriteriaIds)
                    {
                        if (int.Parse(idAttrib.Value) >= searchCriteriaId)
                            searchCriteriaId = int.Parse(idAttrib.Value) + 1;
                    }

                    XmlNode queryLayoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']//COE:coeForm[@id='0']/COE:layoutInfo", manager);
                    if (queryLayoutInfo == null)
                    {
                        errorInPatch = true;
                        messages.Add("Query mode LayoutInfo was not found on form 4002");
                    }
                    else
                    {
                        if (queryLayoutInfo.SelectNodes("./COE:formElement[@name='Status']", manager).Count > 0)
                        {
                            messages.Add("Status formElement was already present in query mode");
                        }
                        else
                        {
                            XmlNode personCreated = queryLayoutInfo.SelectSingleNode("./COE:formElement[@name='PERSONCREATED']", manager);
                            XmlNode statusQueryFE = doc.CreateElement("formElement", "COE.FormGroup");
                            statusQueryFE.Attributes.Append(doc.CreateAttribute("name")).Value = "Status";
                            statusQueryFE.InnerXml = @"<label xmlns=""COE.FormGroup"">Status</label>
						  <showHelp xmlns=""COE.FormGroup"">false</showHelp>
						  <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
						  <pageComunicationProvider xmlns=""COE.FormGroup""/>
						  <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
						  <helpText xmlns=""COE.FormGroup""/>
						  <defaultValue xmlns=""COE.FormGroup""/>
						  <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[" + searchCriteriaId + @"].Criterium.Value</bindingExpression>
						  <Id xmlns=""COE.FormGroup"">StatusStateControl</Id>
						  <displayInfo xmlns=""COE.FormGroup"">
							<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
							<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
							<visible xmlns=""COE.FormGroup"">true</visible>
						  </displayInfo>
						  <validationRuleList xmlns=""COE.FormGroup""/>
						  <serverEvents xmlns=""COE.FormGroup""/>
						  <clientEvents xmlns=""COE.FormGroup""/>
						  <configInfo xmlns=""COE.FormGroup"">
							<fieldConfig xmlns=""COE.FormGroup"">
							  <DisplayType xmlns=""COE.FormGroup"">DropDown</DisplayType>
							  <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
							  <CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
							  <ItemCSSClass xmlns=""COE.FormGroup"">StatusButton</ItemCSSClass>
							  <States xmlns=""COE.FormGroup"">
								<State text=""Choose..."" value="""" url=""""/>
								<State text=""Approved"" value=""2"" url=""/COERegistration/App_Themes/Common/Images/ThumbsUp.png""/>
								<State text=""Submitted"" value=""1"" url=""/COERegistration/App_Themes/Common/Images/ThumbsDown.png""/>
							  </States>
							  <DefaultSelectedValue xmlns=""COE.FormGroup"">0</DefaultSelectedValue>
							</fieldConfig>
						  </configInfo>
						  <dataSource xmlns=""COE.FormGroup""/>
						  <dataSourceId xmlns=""COE.FormGroup""/>
						  <searchCriteriaItem fieldid=""" + fieldId + @""" id=""" + searchCriteriaId + @""" tableid=""1"" xmlns=""COE.FormGroup"">
							<numericalCriteria operator=""EQUAL"" xmlns=""COE.FormGroup"" />
						  </searchCriteriaItem>
						  <displayData xmlns=""COE.FormGroup""/>";

                            if (personCreated != null)
                                queryLayoutInfo.InsertBefore(statusQueryFE, personCreated);
                            else
                                queryLayoutInfo.InsertBefore(statusQueryFE, queryLayoutInfo.ChildNodes[5]);

                            messages.Add("Form element for filtering by statusid was successfully added in form 4002");
                        }
                    }

                    XmlNode table1Cols = doc.SelectSingleNode("//COE:listForm[@id='0']//COE:coeForm[@id='0']/COE:layoutInfo//COE:table[@name='Table_1']/COE:Columns", manager);
                    if (table1Cols == null)
                    {
                        errorInPatch = true;
                        messages.Add("There are no columns in the list form configured for form 4002");
                    }
                    else
                    {
                        if (table1Cols.SelectNodes("./COE:Column[@name='STATUSCOLUMN']", manager).Count > 0)
                        {
                            messages.Add("Status column was already present in list view");
                        }
                        else
                        {
                            XmlNode structureColumn = table1Cols.SelectSingleNode("./COE:Column[@name='Structure']", manager);
                            XmlNode statusColumn = doc.CreateElement("Column", "COE.FormGroup");
                            statusColumn.Attributes.Append(doc.CreateAttribute("name")).Value = "STATUSCOLUMN";
                            statusColumn.InnerXml = @"<headerText xmlns=""COE.FormGroup"">Approved</headerText>
                              <width xmlns=""COE.FormGroup"">70px</width>
                              <formElement name=""STATUSID"" xmlns=""COE.FormGroup"">
                                <displayInfo xmlns=""COE.FormGroup"">
                                  <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
                                </displayInfo>
                                <Id xmlns=""COE.FormGroup"">ListStatusStateControl</Id>
                                <configInfo xmlns=""COE.FormGroup"">
                                  <fieldConfig xmlns=""COE.FormGroup"">
                                    <CSSClass xmlns=""COE.FormGroup"">state_control</CSSClass>
                                    <ItemCSSClass xmlns=""COE.FormGroup"">StatusButton</ItemCSSClass>
                                    <DisplayType xmlns=""COE.FormGroup"">ImageButton</DisplayType>
                                    <ReadOnly xmlns=""COE.FormGroup"">true</ReadOnly>
                                    <States xmlns=""COE.FormGroup"">
                                      <State text="""" value=""1"" url=""/COERegistration/App_Themes/Common/Images/ThumbsDown.png""/>
                                      <State text="""" value=""2"" url=""/COERegistration/App_Themes/Common/Images/ThumbsUp.png""/>
                                    </States>
                                  </fieldConfig>
                                </configInfo>
                              </formElement>";
                            if (structureColumn != null)
                                table1Cols.InsertAfter(statusColumn, structureColumn);
                            else
                                table1Cols.InsertAfter(statusColumn, table1Cols.ChildNodes[2]);

                            messages.Add("Status column was succesfully added to the list view of form 4002");
                        }

                        manager.AddNamespace("COE1", "COE.ResultsCriteria");
                        XmlNode table1ResultCriteria = doc.SelectSingleNode("//COE:listForm[@id='0']//COE1:resultsCriteria/COE1:tables/COE1:table[@id='1']", manager);
                        if (table1ResultCriteria == null)
                        {
                            errorInPatch = true;
                            messages.Add("Table_1 was not found in the results criteria for form 4002");
                        }
                        else
                        {
                            if (table1ResultCriteria.SelectNodes("./COE1:field[@alias='STATUSID']", manager).Count > 0)
                            {
                                messages.Add("STATUSID was already present in the results criteria");
                            }
                            else
                            {
                                XmlNode newRC = doc.CreateElement("field", "COE.ResultsCriteria");

                                newRC.Attributes.Append(doc.CreateAttribute("visible")).Value = "true";
                                newRC.Attributes.Append(doc.CreateAttribute("alias")).Value = "STATUSID";
                                newRC.Attributes.Append(doc.CreateAttribute("orderById")).Value = "0";
                                newRC.Attributes.Append(doc.CreateAttribute("direction")).Value = "asc";
                                newRC.Attributes.Append(doc.CreateAttribute("fieldId")).Value = fieldId.ToString();

                                table1ResultCriteria.AppendChild(newRC);
                                messages.Add("StatusID result criteria was succesfully added to form 4002");
                            }
                        }
                    }
                }
                else if (id.Equals("4011"))
                {
                    XmlNode mixtureViewModeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']//COE:coeForm[@id='0']/COE:viewMode", manager);
                    if (mixtureViewModeForm == null)
                    {
                        errorInPatch = true;
                        messages.Add("mixture form was not found for form 4011");
                    }
                    else
                    {
                        if (mixtureViewModeForm.SelectNodes("./COE:formElement[@name='Status']", manager).Count > 0)
                        {
                            messages.Add("Status control for approvals was already present in form 4011");
                        }
                        else
                        {
                            XmlNode idNode = mixtureViewModeForm.SelectSingleNode("./COE:formElement[@name='ID']", manager);
                            XmlNode statusFormElement = doc.CreateElement("formElement", "COE.FormGroup");
                            statusFormElement.Attributes.Append(doc.CreateAttribute("name")).Value = "Status";
                            statusFormElement.InnerXml = @"<showHelp xmlns=""COE.FormGroup"">false</showHelp>
                      <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                      <pageComunicationProvider xmlns=""COE.FormGroup""/>
                      <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                      <helpText xmlns=""COE.FormGroup""/>
                      <defaultValue xmlns=""COE.FormGroup""/>
                      <bindingExpression xmlns=""COE.FormGroup"">Status</bindingExpression>
                      <Id xmlns=""COE.FormGroup"">StatusProperty</Id>
                      <displayInfo xmlns=""COE.FormGroup"">
                        <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Registration.RegStatusControl</type>
				        <assembly xmlns=""COE.FormGroup"">CambridgeSoft.COE.RegistrationWebApp</assembly>
				        <style xmlns=""COE.FormGroup"">margin-left:450px</style>
                        <visible xmlns=""COE.FormGroup"">true</visible>
                      </displayInfo>
                      <serverEvents xmlns=""COE.FormGroup""/>
                      <clientEvents xmlns=""COE.FormGroup""/>
                      <configInfo xmlns=""COE.FormGroup"">
                        <fieldConfig xmlns=""COE.FormGroup"">
                          <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                          <CSSClass xmlns=""COE.FormGroup"">state_control</CSSClass>
                          <DisplayType xmlns=""COE.FormGroup"">DropDown</DisplayType>
                          <States xmlns=""COE.FormGroup"">
                            <State text=""Approved"" value=""Approved"" url=""~/App_Themes/Common/Images/ThumbsUp.png""/>
                            <State text=""Not Approved"" value=""Submitted"" url=""~/App_Themes/Common/Images/ThumbsDown.png""/>
                          </States>
                        </fieldConfig>
                      </configInfo>
                      <dataSource xmlns=""COE.FormGroup""/>
                      <dataSourceId xmlns=""COE.FormGroup""/>
                      <requiredStyle/>
                      <displayData xmlns=""COE.FormGroup""/>";

                            if (idNode != null)
                                mixtureViewModeForm.InsertAfter(statusFormElement, idNode);
                            else
                                mixtureViewModeForm.InsertAfter(statusFormElement, mixtureViewModeForm.ChildNodes[1]);

                            messages.Add("Status formElement was successfully added to form 4011");
                        }
                    }


                    XmlNode mixtureEditModeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']//COE:coeForm[@id='0']/COE:editMode", manager);
                    if (mixtureEditModeForm == null)
                    {
                        errorInPatch = true;
                        messages.Add("mixture form was not found for form 4011");
                    }
                    else
                    {
                        if (mixtureEditModeForm.SelectNodes("./COE:formElement[@name='Status']", manager).Count > 0)
                        {
                            messages.Add("Status control for approvals was already present in form 4011");
                        }
                        else
                        {
                            XmlNode statusFormElementEditMode = doc.CreateElement("formElement", "COE.FormGroup");
                            statusFormElementEditMode.Attributes.Append(doc.CreateAttribute("name")).Value = "Status";
                            statusFormElementEditMode.InnerXml = @"<showHelp xmlns=""COE.FormGroup"">false</showHelp>
                      <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                      <pageComunicationProvider xmlns=""COE.FormGroup""/>
                      <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
                      <helpText xmlns=""COE.FormGroup""/>
                      <defaultValue xmlns=""COE.FormGroup""/>
                      <bindingExpression xmlns=""COE.FormGroup"">Status</bindingExpression>
                      <Id xmlns=""COE.FormGroup"">StatusProperty</Id>
                      <displayInfo xmlns=""COE.FormGroup"">
                        <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Registration.RegStatusControl</type>
				        <assembly xmlns=""COE.FormGroup"">CambridgeSoft.COE.RegistrationWebApp</assembly>
				        <style xmlns=""COE.FormGroup"">margin-left:570px</style>
                        <visible xmlns=""COE.FormGroup"">true</visible>
                      </displayInfo>
                      <serverEvents xmlns=""COE.FormGroup""/>
                      <clientEvents xmlns=""COE.FormGroup""/>
                      <configInfo xmlns=""COE.FormGroup"">
                        <fieldConfig xmlns=""COE.FormGroup"">
                          <CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
                          <CSSClass xmlns=""COE.FormGroup"">state_control</CSSClass>
                          <DisplayType xmlns=""COE.FormGroup"">DropDown</DisplayType>
                          <States xmlns=""COE.FormGroup"">
                            <State text=""Approved"" value=""Approved"" url=""~/App_Themes/Common/Images/ThumbsUp.png""/>
                            <State text=""Not Approved"" value=""Submitted"" url=""~/App_Themes/Common/Images/ThumbsDown.png""/>
                          </States>
                        </fieldConfig>
                      </configInfo>
                      <dataSource xmlns=""COE.FormGroup""/>
                      <dataSourceId xmlns=""COE.FormGroup""/>
                      <requiredStyle/>
                      <displayData xmlns=""COE.FormGroup""/>";

                            mixtureEditModeForm.InsertBefore(statusFormElementEditMode, mixtureEditModeForm.FirstChild);

                            messages.Add("Status formElement was successfully added to form 4011");
                        }
                    }
                }
            }
            #endregion

            if (!errorInPatch)
                messages.Add("Approvals Workflow was successfully fixed.");
            else
                messages.Add("Approvals Workflow  was fixed with partial update.");

            return messages;
        }
    }
}

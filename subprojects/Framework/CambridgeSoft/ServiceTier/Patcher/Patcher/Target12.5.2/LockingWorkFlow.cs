using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patcher for Locking work flow to modify formbo 4003,4012 and frameworkconfig
    /// </summary>
    class LockingWorkFlow : BugFixBaseCommand
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

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region frameworkconfig fix

            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string actionLinkNodepath = "//coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4003']/actionLinks/link[@id='LockMarkedLink']";

            #region Validate
            XmlNode actionLinkNode = frameworkConfig.SelectSingleNode(actionLinkNodepath);

            if (actionLinkNode == null)
            {
                errorsInPatch = true;
                messages.Add("LockMarkedLink is not available to update.");
            }
            else if (actionLinkNode.Attributes["href"] == null)
            {
                errorsInPatch = true;
                messages.Add("href attribute is not available in LockMarkedLink .");
            }
            else
            {
                actionLinkNode.Attributes["href"].Value = "/COERegistration/Forms/BulkRegisterMarked/ContentArea/LockMarked.aspx?COEHitListID={0}";
                messages.Add("LockingWorkFlow actionlink is successfully updated.");
            }

            #endregion

            #endregion

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region   4003.xml
                if (id == "4003")
                {
                    LoopCount = 1;
                    //Removing the status search criteria from Search Registry page.
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region EditMode
                    string coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        XmlNode selectedStatusNode = selectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='STATUS']", manager);
                        if (selectedStatusNode != null)
                        {
                            //selectedNode.RemoveChild(selectedStatusNode);
                            selectedStatusNode.InnerXml = @"<label>Status</label>
              <showHelp>false</showHelp>
              <isFileUpload>false</isFileUpload>
              <pageComunicationProvider />
              <fileUploadBindingExpression />
              <helpText />
              <defaultValue />
              <bindingExpression>SearchCriteria[12].Criterium.Value</bindingExpression>
              <Id>LockedStatusStateControl</Id>
              <displayInfo>
                <cssClass>Std25x40</cssClass>
                <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
                <visible>true</visible>
              </displayInfo>
              <validationRuleList />
              <serverEvents />
              <clientEvents />
              <configInfo>
                <fieldConfig>
                  <DisplayType>DropDown</DisplayType>
                  <CSSLabelClass>FELabel</CSSLabelClass>
                  <CSSClass>FEDropDownList</CSSClass>
                  <ItemCSSClass>ImageButton</ItemCSSClass>
                  <States>
                    <State text=""Choose..."" value="""" url=""/COERegistration/App_Themes/Common/Images/DrawStructure.png"" />
                    <State text=""Unlocked"" value=""3"" url=""/COERegistration/App_Themes/Common/Images/NoStructure.png"" />
                    <State text=""Locked"" value=""4"" url=""/COERegistration/App_Themes/Common/Images/UnknownStructure.png"" />
                  </States>
                  <DefaultSelectedValue>0</DefaultSelectedValue>
                </fieldConfig>
              </configInfo>
              <dataSource />
              <dataSourceId />
              <searchCriteriaItem fieldid=""110"" id=""12"" tableid=""1"">
                <textCriteria negate=""NO"" normalizedChemicalName=""NO"" hillFormula=""NO"" fullWordSearch=""NO"" caseSensitive=""YES"" trim=""NONE"" operator=""LIKE"" hint="""" defaultWildCardPosition=""NONE"" />
              </searchCriteriaItem>
              <displayData />";

                            messages.Add("Status node updated in form " + id + ".");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Status node was not found in  form " + id + ".");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:layoutInfo was not found in form " + id + " to create formelement.");
                    }
                    #endregion

                    #region StatusColmn
                    string coeBaseFormPath = "//COE:listForms/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement/COE:configInfo/COE:fieldConfig/COE:tables"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedTableNode = formDoc.SelectSingleNode(coeBaseFormPath, manager);
                    XmlNode selectedStatusListNode = null;
                    if (selectedTableNode != null)
                        selectedStatusListNode = selectedTableNode.SelectSingleNode("COE:table[@name='Table_1']/COE:Columns/COE:Column[@name='LOCKEDSTATUSCOLUMN']", manager);
                    if (selectedStatusListNode != null)
                    {
                        if (selectedStatusListNode.Attributes["name"] != null)
                        {
                            selectedStatusListNode.Attributes["name"].Value = "STATUSID";
                        }
                        selectedStatusListNode.InnerXml = @"<headerText>Locked</headerText>
                          <width>70px</width>
                          <formElement name=""STATUSID"">
                            <displayInfo>
                              <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
                            </displayInfo>
                            <Id>ListLockedStatusStateControl</Id>
                            <configInfo>
                              <fieldConfig>
                                <CSSClass>state_control</CSSClass>
                                <ItemCSSClass>StatusButton</ItemCSSClass>
                                <DisplayType>ImageButton</DisplayType>
                                <ReadOnly>true</ReadOnly>
                                <States>
                                  <State text="""" value=""3"" url=""/COERegistration/App_Themes/Common/Images/ThumbsDown.png"" />
                                  <State text="""" value=""4"" url=""/COERegistration/App_Themes/Common/Images/ThumbsUp.png"" />
                                </States>
                              </fieldConfig>
                            </configInfo>
                          </formElement>";
                        messages.Add("Status column updated in form " + id + ".");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Status column was not found in  form " + id + ".");
                    }
                   
                    #endregion
                }
                #endregion

                #region   4006.xml
                if (id == "4006")  // Fix for ELN Search Registry APPROVE field (CSBR-164837)
                {
                    LoopCount = 1;
                    XmlNamespaceManager managerSearchELN = new XmlNamespaceManager(formDoc.NameTable);
                    managerSearchELN.AddNamespace("COE", "COE.FormGroup");

                    #region EditMode
                    string coeFormPathSearchELN = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNodeSearchELN = formDoc.SelectSingleNode(coeFormPathSearchELN, managerSearchELN);
                    if (selectedNodeSearchELN != null)
                    {
                        XmlNode selectedStatusNodeELN = selectedNodeSearchELN.SelectSingleNode(coeFormPathSearchELN + "/COE:formElement[@name='APPROVED']", managerSearchELN);
                        if (selectedStatusNodeELN != null)
                        {
                            selectedStatusNodeELN.Attributes["name"].Value = "STATUS";
                            selectedStatusNodeELN.InnerXml = @"<label>Status</label>
              <showHelp>false</showHelp>
              <isFileUpload>false</isFileUpload>
              <pageComunicationProvider />
              <fileUploadBindingExpression />
              <helpText />
              <defaultValue />
              <bindingExpression>SearchCriteria[12].Criterium.Value</bindingExpression>
              <Id>LockedStatusStateControl</Id>
              <displayInfo>
                <cssClass>Std25x40</cssClass>
                <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl</type>
                <visible>true</visible>
              </displayInfo>
              <validationRuleList />
              <serverEvents />
              <clientEvents />
              <configInfo>
                <fieldConfig>
                  <DisplayType>DropDown</DisplayType>
                  <CSSLabelClass>FELabel</CSSLabelClass>
                  <CSSClass>FEDropDownList</CSSClass>
                  <ItemCSSClass>ImageButton</ItemCSSClass>
                  <States>
                    <State text=""Choose..."" value="""" url=""/COERegistration/App_Themes/Common/Images/DrawStructure.png"" />
                    <State text=""Unlocked"" value=""3"" url=""/COERegistration/App_Themes/Common/Images/NoStructure.png"" />
                    <State text=""Locked"" value=""4"" url=""/COERegistration/App_Themes/Common/Images/UnknownStructure.png"" />
                  </States>
                  <DefaultSelectedValue>0</DefaultSelectedValue>
                </fieldConfig>
              </configInfo>
              <dataSource />
              <dataSourceId />
              <searchCriteriaItem fieldid=""110"" id=""12"" tableid=""1"">
                <textCriteria negate=""NO"" normalizedChemicalName=""NO"" hillFormula=""NO"" fullWordSearch=""NO"" caseSensitive=""YES"" trim=""NONE"" operator=""LIKE"" hint="""" defaultWildCardPosition=""NONE"" />
              </searchCriteriaItem>
              <displayData />";

                            messages.Add("Status node updated in form " + id + ".");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Status node was not found in  form " + id + ".");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:layoutInfo was not found in form " + id + " to create formelement.");
                    }
                    #endregion

                }
                #endregion
                #region 4012.xml
                if (id == "4012")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region EditMode
                    string _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode";
                    XmlNode selectedRootNode = formDoc.SelectSingleNode(_coeFormPath, manager);
                    if (selectedRootNode != null)
                    {
                        XmlNode selectedStatusNode = selectedRootNode.SelectSingleNode(_coeFormPath + "/COE:formElement[@name='STATUS']", manager);
                        if (selectedStatusNode != null)
                        {
                            selectedStatusNode.InnerXml = @"<showHelp>false</showHelp>
              <isFileUpload>false</isFileUpload>
              <pageComunicationProvider />
              <fileUploadBindingExpression />
              <helpText />
              <defaultValue />
              <bindingExpression>Status</bindingExpression>
              <Id>LockStatusProperty</Id>
              <displayInfo>
                <type>CambridgeSoft.COE.Registration.RegStatusControl</type>
                <assembly>CambridgeSoft.COE.RegistrationWebApp</assembly>
                <style>margin-left:630px</style>
                <visible>true</visible>
              </displayInfo>
              <serverEvents />
              <clientEvents />
              <configInfo>
                <fieldConfig>
                  <CSSLabelClass>FELabel</CSSLabelClass>
                  <CSSClass>state_control</CSSClass>
                  <DisplayType>DropDown</DisplayType>
                  <States>
                    <State text=""Locked"" value=""Locked"" url=""~/App_Themes/Common/Images/ThumbsUp.png""/>
                    <State text=""Unlocked"" value=""Registered"" url=""~/App_Themes/Common/Images/ThumbsDown.png""/>
                  </States>
                </fieldConfig>
              </configInfo>
              <dataSource />
              <dataSourceId />
              <requiredStyle />
              <displayData />";
                            messages.Add("Status node updated in form " + id + ".");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Status node was not found in  form " + id + ".");
                        }

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:Status node was not found in form " + id + ".");
                    }
                    #endregion
                }
                #endregion

                #region Validate Loopcount
                if (LoopCount == 5)
                {
                    LoopCount = -2;
                    break;
                }
                #endregion
            }
            #endregion

            #region result

            if (errorsInPatch)
            {
                messages.Add("LockingWorkFlow was fixed with errors.");
            }
            else
            {
                messages.Add("LockingWorkFlow was successfully patched.");
            }

            #endregion

            return messages;
        }
    }
}

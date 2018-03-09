using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class LockingWorkflowFix : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            #region FrameworkConfig changes:
            XmlNode permActionLinks = frameworkConfig.SelectSingleNode("//formBehaviour/form[@formId='4003']/actionLinks");
            if (permActionLinks == null)
            {
                errorInPatch = true;
                messages.Add("There was no actionLinks configured for searching permanent records");
            }
            else
            {
                XmlNode lockMarkedLink = permActionLinks.SelectSingleNode("./link[@id='ApproveMarkedLink']");
                if (lockMarkedLink == null)
                {
                    lockMarkedLink = frameworkConfig.CreateElement("link");
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("id"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("href"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("text"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("target"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("tooltip"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("cssClass"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("enabled"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("privileges"));
                    lockMarkedLink.Attributes.Append(frameworkConfig.CreateAttribute("confirmationMessage"));
                    permActionLinks.AppendChild(lockMarkedLink);
                }

                lockMarkedLink.Attributes["id"].Value = "LockMarkedLink";
                lockMarkedLink.Attributes["href"].Value = "/COERegistration/Forms/BulkRegisterMarked/ContentArea/ApproveMarked.aspx?COEHitListID={0}";
                lockMarkedLink.Attributes["text"].Value = "Lock Marked";
                lockMarkedLink.Attributes["target"].Value = "_parent";
                lockMarkedLink.Attributes["tooltip"].Value = "Registration - Lock";
                lockMarkedLink.Attributes["cssClass"].Value = "MenuItemLink";
                lockMarkedLink.Attributes["enabled"].Value = "true";
                lockMarkedLink.Attributes["privileges"].Value = "SET_LOCKED_FLAG";
                lockMarkedLink.Attributes["confirmationMessage"].Value = "Are you sure you want to lock the marked registries?";

                messages.Add("Locking approved actionLink successfully fixed");
            }
            #endregion

            #region Dataview changes:
            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4003"))
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    XmlNode approvedNode = currentDataview.SelectSingleNode("//COE:table[@id='1']/COE:fields[@name='APPROVED']", manager);

                    if (approvedNode == null)
                    {
                        errorInPatch = true;
                        messages.Add("APPROVED field was not found on dataview 4003");
                    }
                    else
                    {
                        approvedNode.Attributes["name"].Value = "STATUSID";
                        approvedNode.Attributes["dataType"].Value = "INTEGER";
                        approvedNode.Attributes["alias"].Value = "STATUSID";
                        messages.Add("Successfully renamed APPROVED to STATUSID in dataview 4003");
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

                if (id.Equals("4003"))
                {
                    XmlNode queryApprovedStatus = doc.SelectSingleNode("//COE:queryForm[@id='0']//COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='APPROVED']", manager);
                    if (queryApprovedStatus == null)
                    {
                        errorInPatch = true;
                        messages.Add("APPROVED formElement was not found on query mode at form 4003");
                    }
                    else
                    {
                        queryApprovedStatus.Attributes["name"].Value = "STATUS";
                        queryApprovedStatus["Id"].InnerText = "LockedStatusStateControl";
                        queryApprovedStatus.SelectSingleNode(".//COE:State[@value='Approved']/@value", manager).Value = "4";
                        queryApprovedStatus.SelectSingleNode(".//COE:State[@value='Rejected']/@value", manager).Value = "3";
                        messages.Add("APPROVED formElement was succesfully changed to STATUSID in query mode at form 4003");
                    }

                    XmlNode listApprovedStatus = doc.SelectSingleNode("//COE:listForm[@id='0']//COE:coeForm[@id='0']/COE:layoutInfo//COE:table[@name='Table_1']/COE:Columns/COE:Column[@name='APPROVEDCOLUMN']", manager);

                    if (listApprovedStatus == null)
                    {
                        errorInPatch = true;
                        messages.Add("APPROVEDCOLUMN was not found on the list form at form 4003");
                    }
                    else
                    {
                        listApprovedStatus.Attributes["name"].Value = "LOCKEDSTATUSCOLUMN";
                        listApprovedStatus["headerText"].InnerText = "Locked";
                        listApprovedStatus.SelectSingleNode("./COE:formElement/@name", manager).Value = "STATUSID";
                        listApprovedStatus.SelectSingleNode("./COE:formElement", manager).AppendChild(doc.CreateElement("Id", "COE.FormGroup")).InnerText = "ListLockedStatusStateControl";
                        listApprovedStatus.SelectSingleNode("./COE:formElement//COE:ItemCSSClass", manager).InnerText = "StatusButton";
                        listApprovedStatus.SelectSingleNode("./COE:formElement//COE:State[@value='Rejected']/@value", manager).Value = "3";
                        listApprovedStatus.SelectSingleNode("./COE:formElement//COE:State[@value='Approved']/@value", manager).Value = "4";
                        listApprovedStatus.SelectSingleNode("./COE:formElement//COE:fieldConfig", manager).AppendChild(doc.CreateElement("DefaultSelectedValu", "COE.FormGroup")).InnerText = "0";
                        messages.Add("APPROVEDCOLUMN was successfully renamed to LOCKEDSTATUSCOLUMN at form 4003");
                    }

                    manager.AddNamespace("COE1", "COE.ResultsCriteria");
                    XmlNode approvedRC = doc.SelectSingleNode("//COE:listForm[@id='0']//COE1:resultsCriteria/COE1:tables/COE1:table[@id='1']/COE1:field[@alias='APPROVED']", manager);
                    if (approvedRC == null)
                    {
                        errorInPatch = true;
                        messages.Add("APPROVED result criteria was not found on form 4003");
                    }
                    else
                    {
                        approvedRC.Attributes["alias"].Value = "STATUSID";
                    }
                }
                else if (id.Equals("4012"))
                {
                    XmlNode mixtureEditModeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']//COE:coeForm[@id='0']/COE:editMode", manager);
                    if (mixtureEditModeForm == null)
                    {
                        errorInPatch = true;
                        messages.Add("Edit view was not found on mixture form (id=0) at from 4012");
                    }
                    else
                    {
                        XmlNode approvedEditNode = mixtureEditModeForm.SelectSingleNode("./COE:formElement[@name='APPROVED']", manager);
                        if (approvedEditNode == null)
                        {
                            messages.Add("Approved form element is not present on edit mode at form 4012");
                        }
                        else
                        {
                            mixtureEditModeForm.RemoveChild(approvedEditNode);
                            messages.Add("Sucessfully removed approved form element from edit mode at form 4012");
                        }
                    }

                    XmlNode viewModeFormElement = doc.SelectSingleNode("//COE:detailsForm[@id='0']//COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='APPROVED']", manager);
                    if (viewModeFormElement == null)
                    {
                        errorInPatch = true;
                        messages.Add("APPROVED formElement was not found on view mode at form 4012");
                    }
                    else
                    {
                        viewModeFormElement.Attributes["name"].Value = "STATUS";
                        viewModeFormElement["Id"].InnerText = "LockedStatusProperty";
						viewModeFormElement["bindingExpression"].InnerText = "Status";
                        XmlNode displayInfo = viewModeFormElement["displayInfo"];
                        displayInfo["type"].InnerText = "CambridgeSoft.COE.Registration.RegStatusControl";
                        displayInfo.AppendChild(doc.CreateElement("assembly", "COE.FormGroup")).InnerText = "CambridgeSoft.COE.RegistrationWebApp";
                        displayInfo.AppendChild(doc.CreateElement("style", "COE.FormGroup")).InnerText = "margin-left:450px";
                        viewModeFormElement.RemoveChild(viewModeFormElement["validationRuleList"]);
                        viewModeFormElement.SelectSingleNode(".//COE:State[@value='NotSet']/@value", manager).Value = string.Empty;
                        viewModeFormElement.SelectSingleNode(".//COE:State[@value='Rejected']/@value", manager).Value = "3";
                        viewModeFormElement.SelectSingleNode(".//COE:State[@value='Approved']/@value", manager).Value = "4";
                        messages.Add("Succesfully modified APPROVED column in view mode at form 4012");
                    }
                }
            }
            #endregion

            if (!errorInPatch)
                messages.Add("Locking Workflow was successfully fixed.");
            else
                messages.Add("Locking Workflow  was fixed with errors.");

            return messages;
        }
    }
}

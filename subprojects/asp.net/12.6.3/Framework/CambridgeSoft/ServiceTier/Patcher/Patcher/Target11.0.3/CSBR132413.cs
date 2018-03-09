using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 132413 
    /// On the duplicate resoluton page, On the batch information section, there are 3 issues suggested to be fixed.
    /// </summary>
    class CSBR132413 : BugFixBaseCommand
    {
        /// <summary>
        /// This were the manual steps to fix:
        /// In FormGroup 4013:
        /// 1. Search for coeForm id="2"
        /// 2. Replace the coeForm datasourceId with "CurrentBatchCslaDataSource"
        /// 3. Replace  the bindingExpressions of the following formElements:        
        /// formelement name            bindingexpression
        /// "Projects"                  "this.Projects"
        /// "Identifiers"               "this.IdentifierList"
        /// "Batch ID"                  "this.ID"
        /// "Date Created"              "this.DateCreated"
        /// "Last Modification Date"    "this.DateLastModified"
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4013")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode coeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']", manager);
                    if (coeForm == null)
                    {
                        messages.Add("The expected coeForm id='2' was not found");
                        errorsInPatch = true;
                    }
                    else
                    {
                        #region Modifying coeForm id='2' datasourceId
                        XmlAttribute datasourceid = coeForm.Attributes["dataSourceId"];
                        if (datasourceid == null || datasourceid.Value != "DuplicatesCslaDataSource")
                        {
                            messages.Add("The coeForm id='2' did not have the expected datasourceid and was not changed.");
                            errorsInPatch = true;
                        }
                        else
                        {
                            datasourceid.Value = "CurrentBatchCslaDataSource";
                            messages.Add("The coeForm id='2' datasourceId was successfully changed");
                        }
                        #endregion

                        #region Setting Up fromElements to modify
                        Dictionary<string, string> bindingExpsToModifyByFormElement = new Dictionary<string, string>();
                        bindingExpsToModifyByFormElement.Add("Projects", "this.ProjectList");
                        bindingExpsToModifyByFormElement.Add("Identifiers", "this.IdentifierList");
                        bindingExpsToModifyByFormElement.Add("Batch ID", "this.ID");
                        bindingExpsToModifyByFormElement.Add("Date Created", "this.DateCreated");
                        bindingExpsToModifyByFormElement.Add("Last Modification Date", "this.DateLastModified");
                        #endregion

                        #region Modifying formElements bindingExpressions

                        foreach (KeyValuePair<string, string> formElement in bindingExpsToModifyByFormElement)
                        {
                            this.ChangeFormElementBindingExpression(ref coeForm, formElement, ref errorsInPatch, ref messages, manager);
                        }
                        #endregion

                        #region Make Projects Grid Read-Only
                        XmlNode projectRONode = coeForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:ReadOnly", manager);
                        if (projectRONode != null && projectRONode.InnerText.ToLower() == "false")
                        {
                            projectRONode.InnerText = "true";
                            messages.Add("Successfully made projects form element read only");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Project grid didn't have the expected ReadOnly value and was not modified");
                        }
                        #endregion
                    }
                }
            }

            if (!errorsInPatch)
                messages.Add("CSBR132413 was successfully patched");
            else
                messages.Add("CSBR132413 was patched with errors");
            return messages;
        }

        /// <summary>
        /// Modifies XmlNode coeForm at especific formElement bindingExpressions
        /// </summary>
        /// <param name="coeForm">The XmlNode of the coeForm to modify</param>
        /// <param name="formElement">The formElement name and the bindigExpression value</param>
        /// <param name="errors">Indicates whether there are errors</param>
        /// <param name="messages">the error messages</param>
        private void ChangeFormElementBindingExpression(ref XmlNode coeForm, KeyValuePair<string, string> formElement, ref bool errors, ref List<string> messages, XmlNamespaceManager manager)
        {
            XmlNode fEBindingExp = coeForm.SelectSingleNode("./COE:viewMode/COE:formElement[@name='" + formElement.Key + "']/COE:bindingExpression",manager);

            if (fEBindingExp != null && fEBindingExp.InnerText != formElement.Value)
            {
                fEBindingExp.InnerText = formElement.Value;
                messages.Add("The formElement name='" + formElement.Key + "' bindingExpression was successfully changed.");
            }
            else
            {
                messages.Add("The formElement name='" + formElement.Key + "' has previously been modified.");
                errors = false;
            }
        }

    }
}

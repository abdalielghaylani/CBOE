using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 132100:Suggest to change the label font color of registry/batch project to red
    /// </summary>
    class CSBR132100 : BugFixBaseCommand
    {
        /// <summary>
        /// Steps to fix manually:
        /// 1 - Open COEFormGroups 4010, 4011, 4012
        /// 2 - Replace Registry and Batch Projects form elements labels csslabelclass value with 'RLSRegLabel' and 'RLSBatchLabel' respectively
        /// </summary>        
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string oldLabelCSS = "FELabel";
            string newRegProjLabelCSS = "FELabel RLSRegLabel";
            string newBatchProjLabelCSS = "FELabel RLSBatchLabel";

            List<string> formsIds = new List<string>();
            formsIds.Add("0");
            formsIds.Add("4");

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4010" || id == "4011" || id == "4012")
                {
                    if (id == "4010") 
                    {
                        XmlNode ProjEditMode4010 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='ProjectList']", manager);

                        if (ProjEditMode4010 != null)
                        {
                            ProjEditMode4010.Attributes["name"].Value = "Projects";
                            messages.Add("The ProjectList form element name attribute at editMode on COEFormGroup id='" + id + "', form id='0', was changed with Projects");
                            
                        }
                        
                    }
                    foreach (string formId in formsIds)
                    {
                        XmlNode ProjLabelCSSEditMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:editMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:CSSLabelClass", manager);

                        if (ProjLabelCSSEditMode == null)
                        {
                            XmlNode fieldConfigNode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:editMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig", manager);
                            fieldConfigNode.InsertBefore(doc.CreateNode(XmlNodeType.Element, "CSSLabelClass", fieldConfigNode.NamespaceURI), fieldConfigNode.FirstChild);
                            ProjLabelCSSEditMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:editMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:CSSLabelClass", manager);
                            ProjLabelCSSEditMode.InnerText = oldLabelCSS;
                        }

                        if (ProjLabelCSSEditMode.InnerText.Trim().ToUpper() != oldLabelCSS.Trim().ToUpper())
                        {
                            messages.Add("The Projects form element CSSLabelClass at editMode on COEFormGroup id='" + id + "', form id='" + formId + "', doesn´t have the expected value and was not changed");
                            errorsInPatch = true;
                        }
                        else
                        {
                            ProjLabelCSSEditMode.InnerText = formId == "0" ? newRegProjLabelCSS : newBatchProjLabelCSS;

                            messages.Add("The Projects form element CSSLabelClass value at editMode on COEFormGroup id='" + id + "', form id='" + formId + "', was successfully updated");
                        }

                        if (id == "4010")
                        {
                            XmlNode ProjLabelCSSAddMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:addMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:CSSLabelClass", manager);

                            if (ProjLabelCSSAddMode == null)
                            {
                                XmlNode fieldConfigNode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:addMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig", manager);
                                fieldConfigNode.InsertBefore(doc.CreateNode(XmlNodeType.Element, "CSSLabelClass", fieldConfigNode.NamespaceURI), fieldConfigNode.FirstChild);
                                ProjLabelCSSAddMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + formId + "']/COE:addMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:CSSLabelClass", manager);
                                ProjLabelCSSAddMode.InnerText = oldLabelCSS;
                            }

                            if (ProjLabelCSSAddMode.InnerText.Trim().ToUpper() != oldLabelCSS.Trim().ToUpper())
                            {
                                messages.Add("The Projects form element CSSLabelClass at addMode on COEFormGroup id='" + id + "', form id='" + formId + "', doesn´t have the expected value and was not changed");
                                errorsInPatch = true;

                            }
                            else
                            {
                                ProjLabelCSSAddMode.InnerText = formId == "0" ? newRegProjLabelCSS : newBatchProjLabelCSS;

                                messages.Add("The Projects form element CSSLabelClass value at addMode on COEFormGroup id='" + id + "', form id='" + formId + "', was successfully updated");
                            }

                        }
                    }

                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR - 132100 was successfully patched");
            }
            else
                messages.Add("CSBR - 132100 was patched with errors");
            return messages;
        }
    }
}

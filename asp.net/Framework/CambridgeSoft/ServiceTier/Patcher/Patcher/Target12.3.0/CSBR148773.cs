using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patcher For CSBR-148773: Updates coeForm[4011.xml] with three changes type,validationrule,cssclass.
    /// </summary>
    class CSBR148773 : BugFixBaseCommand
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

                #region 4010.xml
                if (id == "4010")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:detailsForms[@defaultForm='0']/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='STRUCT_COMMENTS']"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    
                    if (rootSelectedNode != null)
                    {
                        XmlNode typeNode = rootSelectedNode.SelectSingleNode("COE:displayInfo/COE:type", manager);
                        XmlNode vRuleListNode = rootSelectedNode.SelectSingleNode("COE:validationRuleList", manager);
                        XmlNode cssClassNode = rootSelectedNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:CSSClass", manager);

                        #region Type
                        if (typeNode != null)
                        {

                            if (string.IsNullOrEmpty(typeNode.InnerText))
                            {
                                typeNode.InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly";
                                messages.Add("Form[" + id + "]:The type node was updated succesfully.");
                            }
                            else if (typeNode.InnerText.ToLower() != "cambridgesoft.coe.framework.controls.coeformgenerator.coetextareareadonly")
                            {
                                typeNode.InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly";
                                messages.Add("Form[" + id + "]:The type node was updated succesfully.");
                            }
                            else
                            {
                                messages.Add("Form[" + id + "]:The type node contains latest value.");
                            }
                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:The type node was not found.");
                        }
                        #endregion
                        
                        #region Rule List
                        if (vRuleListNode != null)
                        {
                            if (vRuleListNode.HasChildNodes)
                            {
                                XmlNode vRuleNode = vRuleListNode.SelectSingleNode("COE:validationRule", manager);
                                if (vRuleListNode != null)
                                {
                                    vRuleListNode.RemoveChild(vRuleNode);
                                    messages.Add("Form[" + id + "]:The validationRule node was removed succesfully.");
                                }
                                else
                                {
                                    messages.Add("Form[" + id + "]:The validationRule node was not found.");
                                }
                            }
                            else
                            {
                                messages.Add("Form[" + id + "]:The validationRuleList does not contain child nodes.");
                            }

                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:The validationRuleList node was not found.");
                        }
                        #endregion

                        #region Css Class
                        if (cssClassNode != null)
                        {

                            if (string.IsNullOrEmpty(cssClassNode.InnerText))
                            {
                                cssClassNode.InnerText = "FETextAreaViewMode";
                                messages.Add("Form[" + id + "]:The cssClassNode node was updated succesfully.");
                            }
                            else if (cssClassNode.InnerText.ToLower() != "fetextareaviewmode")
                            {
                                cssClassNode.InnerText = "FETextAreaViewMode";
                                messages.Add("Form[" + id + "]:The cssClassNode node was updated succesfully.");
                            }
                            else
                            {
                                messages.Add("Form[" + id + "]:The cssClassNode node contains latest value.");
                            }
                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:The cssClassNode node was not found.");
                        }
                        #endregion
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:The formElement[@name='STRUCT_COMMENTS'] " + id + " does not exist");
                    }
                    break;
                }
                #endregion

            }
            #endregion
            if (!errorsInPatch)
                messages.Add("CSBR148773 was successfully fixed.");
            else
                messages.Add("CSBR148773 was fixed with errors.");
            return messages;
        }
        #endregion

    }
}

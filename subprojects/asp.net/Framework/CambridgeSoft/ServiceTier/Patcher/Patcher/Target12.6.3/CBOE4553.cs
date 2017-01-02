using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE4553 : BugFixBaseCommand
    {
        #region variable

        string _formElement = "STRUCT_NAME";
        string _paramName = "max";
        List<string> messages = new List<string>();
        bool errorsInPatch = false;
        #endregion

        #region property

        private string formElement
        {
            set { _formElement = value; }
            get { return _formElement; }
        }

        private string paramName
        {
            set { _paramName = value; }
            get { return _paramName; }
        }

        #endregion

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {                       
            string coeColumnPath = string.Empty;
            XmlNode rootNode;
            #region Root Node
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                if (id == "4013")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");                   
                    coeColumnPath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode";
                    rootNode = formDoc.SelectSingleNode(coeColumnPath, manager);
                    UpdateForm(rootNode, manager, "addMode");
                    coeColumnPath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode";
                    rootNode = formDoc.SelectSingleNode(coeColumnPath, manager);
                    UpdateForm(rootNode, manager, "editMode");
                    coeColumnPath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode";
                    rootNode = formDoc.SelectSingleNode(coeColumnPath, manager);
                    UpdateForm(rootNode, manager, "viewMode");                    
                }
            }
            #endregion

            if (!errorsInPatch)
                messages.Add("CBOE4553 was successfully fixed.");
            else
                messages.Add("CBOE4553  was fixed with partial update.");
            return messages;
        }
        #region update method
        private void UpdateForm(XmlNode rootSelectedNode, XmlNamespaceManager manager, string mode)
        {
            
                    if (rootSelectedNode != null)
                    {
                        if (rootSelectedNode.SelectSingleNode("COE:formElement[@name='" + formElement + "']", manager) != null)
                        {
                            XmlNode currentNode = rootSelectedNode.SelectSingleNode("COE:formElement[@name='" + formElement + "']", manager);
                            if (currentNode.SelectSingleNode("COE:validationRuleList", manager) != null)
                            {
                                XmlNode validationNode = currentNode.SelectSingleNode("COE:validationRuleList", manager);
                                if (validationNode.SelectSingleNode("COE:validationRule", manager) != null)
                                {
                                    XmlNode validationRuleNode = validationNode.SelectSingleNode("COE:validationRule", manager);
                                    if (validationRuleNode.SelectSingleNode("COE:params", manager) != null)
                                    {
                                        XmlNode paramsNode = validationRuleNode.SelectSingleNode("COE:params", manager);
                                        if (paramsNode.SelectSingleNode("COE:param[@name='" + paramName + "']", manager) != null)
                                        {
                                            XmlNode maxNode = paramsNode.SelectSingleNode("COE:param[@name='" + paramName + "']", manager);
                                            if (maxNode.Attributes["value"] != null)
                                            {
                                                if (maxNode.Attributes[1].Value.Equals("500"))
                                                {
                                                    messages.Add(" Value already patched in 4013 xml" + mode);
                                                }
                                                else
                                                {
                                                    maxNode.Attributes[1].Value = "500";
                                                }
                                            }
                                            else
                                            {
                                                messages.Add(" ERROR in FORM 4013: Max attribute not found");
                                                errorsInPatch = true;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        messages.Add(" ERROR in FORM 4013: params attribute not found");
                                        errorsInPatch = true;
                                    }
                                }
                                else
                                {
                                    messages.Add(" ERROR in FORM 4013: validationRule attribute not found");
                                    errorsInPatch = true;
                                }                                
                            }

                            else
                            {
                                messages.Add(" ERROR in FORM 4013: validationRuleList attribute not found");
                                errorsInPatch = true;
                            }
                        }
                        else
                        {
                            messages.Add(" ERROR - FORM 4013:" + formElement + " not found");
                            errorsInPatch = true;
                        }                       
                    }
                    else
                    {
                        messages.Add("  ERROR - FORM 4013: COEFORM ID=1 not found!");
                        errorsInPatch = true;
                    }                    
                    #endregion
        }
	}
}

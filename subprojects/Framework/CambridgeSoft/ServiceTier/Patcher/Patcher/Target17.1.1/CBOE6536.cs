using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE6536 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;
            _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='PURITY']/COE:validationRuleList"; // Path to check the Rootnode before patcher update.

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4011")
                {
                    XmlNode rootNode = doc.SelectSingleNode(_coeFormPath, manager);

                    if (rootNode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("ValidationRuleList column is not available to update patch for form [" + id + "].");
                        break;
                    }
                    else
                    {
                        XmlNode validationRule = rootNode.SelectSingleNode("COE:validationRule[@validationRuleName='numericRange']", manager);
                        if (validationRule == null)
                        {
                            validationRule = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", "COE.FormGroup");
                            createNewAttribute("validationRuleName", "numericRange", ref validationRule);
                            createNewAttribute("errorMessage", "Purity must be between 0 and 100", ref validationRule);
                            createNewAttribute("displayPosition", "Top_Left", ref validationRule);
                            
                            XmlNode param1 = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "param", "COE.FormGroup");
                            createNewAttribute("name", "min", ref  param1);
                            createNewAttribute("value", "0", ref param1);

                            XmlNode param2 = rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "param", "COE.FormGroup");
                            createNewAttribute("name", "max", ref param2);
                            createNewAttribute("value", "100", ref param2);

                            validationRule.AppendChild(rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "params", "COE.FormGroup"));
                            validationRule.SelectSingleNode("COE:params", manager).AppendChild(param1);
                            validationRule.SelectSingleNode("COE:params", manager).AppendChild(param2);
                            rootNode.AppendChild(validationRule);
                        }
                        else { messages.Add("The Numeric Range Validation rule already present in form [" + id + "] "); }
                    }

                }
            }
            if (!errorsInPatch)
                messages.Add("CBOE6536 was successfully patched");
            else
                messages.Add("CBOE6536 was patched with errors");
            return messages;
        }
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }
    }
}

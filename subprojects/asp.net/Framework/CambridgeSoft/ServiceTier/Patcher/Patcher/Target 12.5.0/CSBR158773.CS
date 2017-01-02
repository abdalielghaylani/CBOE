using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Hide repeted columns in search temp but dont delete them. 
    /// </summary>
    public class CSBR158773 : BugFixBaseCommand
    {


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;

            _coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_2']/COE:Columns/COE:Column[@name='{0}']"; // Path to check the Rootnode before patcher update.

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4002")
                {
                    string[] columnNames = { "COMPONENTSTRUCTURE", "FORMULAWEIGHT", "MOLECULARFORMULA", "CMP_COMMENTS", "STRUCTURE_COMMENTS_TXT"};
                    for (int i = 0; i < columnNames.Length - 1 ; i++)
                    {
                        XmlNode rootNode = doc.SelectSingleNode(string.Format(_coeFormPath, columnNames[i]), manager);

                        #region Column Update
                        if (rootNode == null)
                        {
                            errorsInPatch = true;
                            messages.Add(columnNames[i] + "type column is not available to update patch for form [" + id + "].");
                            break;
                        }
                        else
                        {
                            if (rootNode.Attributes["hidden"] != null)
                            {

                                rootNode.Attributes["hidden"].Value = "True";
                            }
                            else
                                createNewAttribute("hidden", "True", ref rootNode);
                        }
                        # endregion

                    }
                    break;
                }

            }
            if (!errorsInPatch)
                messages.Add("CSBR158773 was successfully patched");
            else
                messages.Add("CSBR158773 was patched with partial update");
            return messages;
        }
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

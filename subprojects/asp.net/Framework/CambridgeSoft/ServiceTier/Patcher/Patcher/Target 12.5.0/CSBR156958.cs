using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  changelist # 440832 Sorting by AL Number column is not working as expected . General fix.
    /// </summary>
    class CSBR156958 : BugFixBaseCommand
    {
        

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeColumnPath = string.Empty;

            #region Column Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                if (id == "4003")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region Column
                    coeColumnPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table [@name='Table_1']/COE:Columns/COE:Column [@name='REGNUMBER']"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeColumnPath, manager);
                    if (rootSelectedNode != null)
                    {
                        if (rootSelectedNode.Attributes["sortField"] == null)
                        {
                            createNewAttribute("sortField", "REGID", ref rootSelectedNode);
                            messages.Add("Form[4003]:The attribute sortfield was added succesfully for column REGNUMBER .");
                        }
                        else
                        {
                            messages.Add("Form[4003]:The column REGNUMBER already contains attribute sortfiled.");
                        }
                    }
                    break;
                    #endregion
                }
            }
            #endregion

            if (!errorsInPatch)
                messages.Add("CSBR156958 was successfully fixed.");
            else
                messages.Add("CSBR156958  was fixed with partial update.");
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

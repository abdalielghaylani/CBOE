using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-148975: To solve component grid overlapping - remove style [height,width] in node.
    /// </summary>
    class CSBR148975 : BugFixBaseCommand
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

                #region 4012.xml
                if (id == "4012")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:detailsForms[@defaultForm='0']/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:viewMode/COE:formElement[@name='Identifiers']/COE:displayInfo/COE:style"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (rootSelectedNode != null)
                    {
                        rootSelectedNode.InnerText = "";
                        messages.Add("Form[" + id + "]:The style node was updated successfully.");
                    }
                    else
                    {
                        errorsInPatch = false;
                        messages.Add("Form[" + id + "]:The style node was not found");
                    }
                    break;
                }
                #endregion  

            }
            #endregion
            if (!errorsInPatch)
                messages.Add("CSBR148975 was successfully fixed.");
            else
                messages.Add("CSBR148975 was fixed with errors.");
            return messages;
        }
        #endregion

    }
}

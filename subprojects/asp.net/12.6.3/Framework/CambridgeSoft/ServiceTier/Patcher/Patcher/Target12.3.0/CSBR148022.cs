using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Changing the width of struture box from 24 to 32
    /// </summary>
    public class CSBR148022 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "5002")
                {
                    XmlNode selectedNode = doc.SelectSingleNode("//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:displayInfo/COE:style", manager);

                    if (selectedNode != null)
                    {
                        selectedNode.InnerText = "width:32%;";
                        messages.Add("Structure box Adjusted.");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Style could not be found in Structure box.");
                    }
                    break;
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("Structure box Adjustment was successfully patched");
            }
            else
                messages.Add("Structure box Adjustment was patched with errors");

            return messages;
        }
    }
}

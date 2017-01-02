using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for 159710.
    /// Manual steps : Remove all child nodes under Details Form in 4016 COEForm.
    /// </summary>
    public class CSBR159710 : BugFixBaseCommand
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

                if (id == "4016")
                {
                    XmlNode selectedNode = doc.SelectSingleNode("//COE:detailsForms", manager);

                    if (selectedNode != null)
                    {
                        selectedNode.RemoveAll();
                        messages.Add("Unused forms deleted.");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Patcher was not added.");
                    }
                                        
                    break;
                }
            }
            
            if (!errorsInPatch)
                messages.Add("CSBR159710 was successfully patched");
            else
                messages.Add("CSBR159710 was patched with errors");
            return messages;
        }
    }
}

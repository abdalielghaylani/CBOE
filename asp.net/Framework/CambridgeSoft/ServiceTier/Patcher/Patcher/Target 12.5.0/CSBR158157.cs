using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace CambridgeSoft.COE.Patcher
{
	public class CSBR158157 : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;
                      

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4010" || id == "4012")
                {
                     _coeFormPath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:clientScripts/COE:script[@eventName='OnSubmit']"; // Path to check the Rootnode before patcher update.

                     XmlNode selectedNode = doc.SelectSingleNode(_coeFormPath, manager);
                    if(selectedNode != null)
                    {
                        if (selectedNode.Attributes["eventName"].Value == "OnSubmit" && !(selectedNode.InnerText.Contains("return false;")))
                        {
                            string clientEvent = selectedNode.InnerText.Trim();
                            int lastIndex = clientEvent.LastIndexOf(";");
                            if (lastIndex == clientEvent.Trim().Length - 1)
                            {
                                clientEvent = "if(!(" + clientEvent.Substring(0, lastIndex) + ")){return false;}";
                                selectedNode.InnerText = clientEvent;
                            }
                            messages.Add("ClientEvent OnSubmit was successfully changed for COEFormGroup Id=" + id);
                        }
                        else
                        {
                            messages.Add("ClientEvent OnSubmit is already changed for COEFormGroup Id=" + id);
                        }
                    }
                    else
                    {
                        messages.Add("ClientEvent OnSubmit doesn't exist for COEFormGroup id=" + id);
                        errorsInPatch = true;
                    }
                }                         
            }
            if (!errorsInPatch)
                messages.Add("CSBR158157 was successfully patched");
            else
                messages.Add("CSBR158157 was patched with errors");
            return messages;
        }
    }
}

	
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for 164488
    /// Remove duplicate form elements.
    /// </summary>
    public class CSBR164488 : BugFixBaseCommand
    {

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;
            string formElementName = "Projects";
            string removeNodeName = "COE:CSSLabelClass";
            _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='" + formElementName + "']"; // Path to check the Rootnode before patcher update.

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4012")
                {
                    XmlNode selectedNode = doc.SelectSingleNode(_coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        XmlNode tempNode = selectedNode.SelectSingleNode("COE:configInfo", manager);
                        if (tempNode != null)
                        {
                            XmlNode removeNode = selectedNode.SelectSingleNode("COE:configInfo/" + removeNodeName, manager);
                            if (removeNode != null)
                            {
                                tempNode.RemoveChild(removeNode);
                                errorsInPatch = false;
                                messages.Add("The node \"" + removeNodeName + "\" is removed from the formelement \"" + formElementName + "\" in the form " + id + " successfully.");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("The node \"" + removeNodeName + "\" does not exist in the formelement \"" + formElementName + "\" to be removed in the form " + id + ".");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("\"" + formElementName + "\" formelement was not found in form " + id + ".");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("\"" + formElementName + "\" formelement was not found in form " + id + ".");
                    }
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR164488 was successfully patched");
            else
                messages.Add("CSBR164488 was not patched due to errors");
            return messages;
        }
    }
}

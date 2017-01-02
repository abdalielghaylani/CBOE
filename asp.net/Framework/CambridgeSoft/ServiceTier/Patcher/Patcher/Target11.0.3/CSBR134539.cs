using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 134539:Component level Identifier values are not updated for the registered records.
    /// </summary>
    class CSBR134539 : BugFixBaseCommand
    {
        /// <summary>
        /// Manual steps:
        /// 1-On COEForms 4011 and 4012 remove all Defaut rows of the identifiers formElements.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4011" || id == "4012")
                {
                    string[] modes = { "addMode", "editMode" };

                    foreach (string mode in modes)
                    {
                        XmlNode defaultRowsNode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:" + mode + "/COE:formElement[@name='Identifiers']/COE:configInfo/COE:fieldConfig/COE:DefaultRows", manager);
                        if (defaultRowsNode != null)
                        {
                            if (defaultRowsNode.InnerText != string.Empty)
                            {
                                defaultRowsNode.InnerText = string.Empty;
                                messages.Add("The default rows was removed from formElement name ='Identifiers' at " + mode + " of COEFormGroup id='" + id + "' coeForm id='1'");
                            }
                            else
                            {
                                messages.Add("WARNING: The default rows was already removed from formElement name ='Identifiers' at " + mode + " of COEFormGroup id='" + id + "' coeForm id='1'");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("The DefaultRows node was not found at " + mode + " on COEFormGroup id='" + id + "' coeForm id='1'");
                        }
                    }
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("CSBR-133565 was successfully patched");
            }
            else
                messages.Add("CSBR-133565 was patched with errors");

            return messages;
        }
    }
}

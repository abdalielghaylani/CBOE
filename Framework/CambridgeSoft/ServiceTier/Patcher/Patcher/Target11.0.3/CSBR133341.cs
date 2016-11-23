using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 133341:When RLS=OFF, two 'project name' fields are in Search Registry page
    /// </summary>
    class CSBR133341 : BugFixBaseCommand
    {
        /// <summary>
        /// Steps to fix manually:
        /// 1 - Open form groups 4002 and 4003.
        /// 2 - Replace REGISTRY_PROJECT form element label for 'Registry Project Name' in both from groups.
        /// 3 - Replace BATCH_PROJECT form element label for 'Batch Project Name' in both from groups.
        /// </summary>        
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            string oldLabel = "Project Name";
            string newRegProjLabel = "Registry Project Name";
            string newBatchProjLabel = "Batch Project Name";
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {

                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                if (id == "4002" || id == "4003")
                {
                    XmlNode registryProjectsDDLabel = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='REGISTRY_PROJECT']/COE:label", manager);

                    if(registryProjectsDDLabel.InnerText.Trim().ToUpper().Equals(newRegProjLabel.Trim().ToUpper()))
                    {
                        messages.Add("Warning: The REGISTRY_PROJECT form element label on COEFormGroup id='" + id + "' was already patched.");
                    }
                    else if (registryProjectsDDLabel.InnerText.Trim().ToUpper() != oldLabel.Trim().ToUpper())
                    {
                        messages.Add("Error: The REGISTRY_PROJECT form element label on COEFormGroup id='" + id + "' doesn´t have the expected value and was not changed");
                        errorsInPatch = true;
                    }
                    else
                    {
                        registryProjectsDDLabel.InnerText = newRegProjLabel;
                        messages.Add("The REGISTRY_PROJECT form element label on COEFormGroup id='" + id + "' was successfully updated");
                    }

                    XmlNode batchProjectsDDLabel;

                    string coeFormId = id == "4002" ? "0" : "2";

                    batchProjectsDDLabel = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='" + coeFormId + "']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']/COE:label", manager);


                    if (batchProjectsDDLabel.InnerText.Trim().ToUpper().Equals(newBatchProjLabel.ToUpper().Trim()))
                    {
                        messages.Add("Warning: The BATCH_PROJECT form element label on COEFormGroup id='" + id + "' was already patched");
                    }
                    else if (batchProjectsDDLabel.InnerText.Trim().ToUpper() != oldLabel.Trim().ToUpper())
                    {
                        messages.Add("Error: The BATCH_PROJECT form element label on COEFormGroup id='" + id + "' doesn´t have the expected value and was not changed");
                        errorsInPatch = true;
                    }
                    else
                    {
                        batchProjectsDDLabel.InnerText = newBatchProjLabel;
                        messages.Add("The BATCH_PROJECT form element label on COEFormGroup id='" + id + "' was successfully updated");
                    }
                }

            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR133141 was successfully patched");
            }
            else
                messages.Add("CSBR133141 was patched with errors");
            return messages;
        }
    }
}

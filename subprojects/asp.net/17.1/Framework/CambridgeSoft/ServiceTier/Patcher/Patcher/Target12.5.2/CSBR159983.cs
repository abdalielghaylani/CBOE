using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for 159983	[CBOE 12.1.2 Migration] Unable to EDIT registered record (perm only).
    /// Remove duplicate form elements.
    /// </summary>
    public class CSBR159983 : BugFixBaseCommand
    {

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;
            _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/{0}"; // Path to check the Rootnode before patcher update.

            string[] formModes = new string[] { "COE:addMode", "COE:editMode", "COE:viewMode" };

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4012")
                {
                    foreach (string mode in formModes)
                    {
                        string _formPath = string.Format(_coeFormPath , mode) + "/COE:formElement[@name='Identifiers']";
                        XmlNode rootNode = doc.SelectSingleNode(string.Format(_coeFormPath , mode), manager);
                        XmlNodeList formNodeList = doc.SelectNodes(_formPath, manager);

                        #region Formxml Validations
                        if (rootNode == null)
                        {
                            errorsInPatch = true;
                            messages.Add(" mode " + mode + " is not available to update patch.");
                            continue;
                        }
                        else if (formNodeList == null)
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers property is not available to update patch in form [" + id + "] for mode " + mode + ".");
                            continue;

                        }
                        # endregion

                        #region Formxml changes
                        if (formNodeList.Count > 1)
                        {
                            rootNode.RemoveChild(formNodeList.Item(1));
                            messages.Add("Duplicate Identifiers formelement property is removed successfully in form [" + id + "] for mode " + mode + ".");
                        }
                        #endregion

                    }
                    break;
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR159983 was successfully patched");
            else
                messages.Add("CSBR159983 was patched with errors");
            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Structure id text box editable in 4012 make it read only. 
    /// </summary>
    class CSBR158761 : BugFixBaseCommand
    {


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;

            _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Structure ID']/COE:displayInfo/COE:type"; // Path to check the Rootnode before patcher update.

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4012")
                {
                    XmlNode rootNode = doc.SelectSingleNode(_coeFormPath, manager);

                    #region Form 4012 
                    if (rootNode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("type node is not available to update patch for form [" + id + "].");
                        break;
                    }
                    else
                    {
                        rootNode.InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly";
                    }
                    # endregion
                }

            }
            if (!errorsInPatch)
                messages.Add("CSBR158761 was successfully patched");
            else
                messages.Add("CSBR158761 was patched with partial update");
            return messages;
        }
    }
}

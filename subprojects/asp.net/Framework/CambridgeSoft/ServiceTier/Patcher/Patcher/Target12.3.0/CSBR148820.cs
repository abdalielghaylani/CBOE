using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-148820: To remove formelement with name='APPROVED' in [coeForms:4014,4019].xml.
    /// </summary>
	class CSBR148820 : BugFixBaseCommand
	{
        #region Variable
        int _loopCount = 0;
        #endregion

        #region Private Property
        private int LoopCount
        {
            get
            {
                return _loopCount;
            }
            set
            {
                _loopCount = _loopCount + value;
            }
        }
        #endregion

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

                #region 4014.xml
                if (id == "4014")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:detailsForms[@defaultForm='0']/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    string formElementName = "APPROVED";
                    if (rootSelectedNode != null)
                    {
                        XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
                        if (formElementNode != null)
                        {
                            rootSelectedNode.RemoveChild(formElementNode);
                            messages.Add("Form[" + id + "]:The formelement node with name='" + formElementName + "' removed succesfully.");
                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:The formelement node with name='" + formElementName + "' does not exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:Viewmode was not found in form " + id + " to remove formelement");
                    }
                }
                #endregion

                #region 4019.xml
                if (id == "4019")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    string formElementName = "APPROVED";
                    if (rootSelectedNode != null)
                    {
                        XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
                        if (formElementNode != null)
                        {
                            rootSelectedNode.RemoveChild(formElementNode);
                            messages.Add("Form[" + id + "]:The formelement node with name='" + formElementName + "' removed succesfully.");
                        }
                        else
                        {
                            messages.Add("Form[" + id + "]:The formelement node with name='" + formElementName + "' does not exits.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:Viewmode was not found in form " + id + " to remove formelement");
                    }
                }
                #endregion

                #region Validate Loopcount
                if (LoopCount == 2)
                {
                    LoopCount = -1;
                    break;
                }
                #endregion
                    
            }
            #endregion
            if (!errorsInPatch)
                messages.Add("CSBR148820 was successfully fixed.");
            else
                messages.Add("CSBR148820 was fixed with errors.");
            return messages;
        }
        #endregion

	}
}

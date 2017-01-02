using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  CSBR 163957
    /// </summary>
    class CSBR163957 : BugFixBaseCommand
    {
        #region Abstract Method

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;
            string id = string.Empty;

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;
                                
                #region 4002.xml and 4003.xml
                if (id == "4002" || id == "4003")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    string formElementName = "COMPONENTNUMBER";
                    coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='" + formElementName + "']"; // Path to check the Component Number node before the patcher update.
                    XmlNode selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        XmlNode tempNode = selectedNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:States/COE:State[@text='More than one component']", manager);
                        if (tempNode != null)
                        {
                            //Coverity fix - CID 19429
                            XmlNode searchNode = tempNode.Attributes.Item(1);
                            if (searchNode != null && searchNode.InnerText == "1")
                            {
                                searchNode.InnerText = ">1";
                                messages.Add("The value for \"More than one component\" search is set to >1 in form " + id + " successfully.");
                                errorsInPatch = false;
                            }
                        }
                        else
                        {
                            messages.Add("State node doesn't exist in the formelement \"" + formElementName + "\" in form " + id + ".");
                            errorsInPatch = true;
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("\"" + formElementName + "\" formelement was not found in form " + id + ".");
                    }
                }
                #endregion

            }
            #endregion

            if (!errorsInPatch)
                messages.Add("CSBR-163957 was successfully fixed in form " + id + ".");
            else
                messages.Add("CSBR-163957 was not was fixed in form " + id + " due to errors.");
            return messages;
        }

        #endregion
    }
}

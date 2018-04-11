using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch for CSBR148626
    /// Adding new 'DefaultDate' Node with Innertext as 'TODAY'
    /// Added Path of 4010.xml-COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='CREATION_DATE']/COE:configInfo/...
    /// </summary>
    public class CSBR148626 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4010")
                {
                    XmlNode selectedDefaultDateDoc = doc.SelectSingleNode("//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='CREATION_DATE']/COE:configInfo/COE:fieldConfig", manager);
                    XmlNode selectedSection508CompliantNode = doc.SelectSingleNode("//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='CREATION_DATE']/COE:configInfo/COE:fieldConfig/COE:Section508Compliant", manager);
                    XmlNode coeFontSizeNode = doc.SelectSingleNode("//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='CREATION_DATE']/COE:configInfo/COE:fieldConfig/COE:FontSize", manager);


                    XmlNode defaultDateNode = selectedDefaultDateDoc.SelectSingleNode("COE:DefaultDate", manager);
                    if (defaultDateNode == null)
                    {
                        defaultDateNode = doc.CreateNode(XmlNodeType.Element, "DefaultDate", "COE.FormGroup");
                        defaultDateNode.InnerText = "TODAY";
                        if (selectedSection508CompliantNode != null)
                        {
                            selectedDefaultDateDoc.InsertAfter(defaultDateNode, selectedSection508CompliantNode);
                        }
                        else if (coeFontSizeNode != null)
                        {
                            selectedDefaultDateDoc.InsertBefore(defaultDateNode, coeFontSizeNode);
                        }
                        else
                        {
                            selectedDefaultDateDoc.AppendChild(defaultDateNode);
                        }
                        messages.Add("DefaultDate is Added.");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("DefaultDate is already present - skipping...");
                    }
                    if (!errorsInPatch)
                    {
                        messages.Add("CSBR148626 was successfully patched");
                    }
                    else
                        messages.Add("CSBR148626 was Not successfully patched");

                }
            }
            return messages;
        }
    }
}


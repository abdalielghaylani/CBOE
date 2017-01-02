using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    //Fix for CSBR 153502(Part one)
    //Part one of CSBR 153502.
    //This fixes when we attach a SD file in Search registry of Registration and click on Search button.
    //It fixes bug like 'Object refeence is not set to Instance of the Object"
    public class CSBR153502 : BugFixBaseCommand
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

                if (id == "4003")
                {
                    XmlNode selectedNode = doc.SelectSingleNode("//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", manager);
                    XmlNode SDFNode = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='SDF_upload']", manager);
                    if (selectedNode != null)
                    {

                        if (SDFNode != null)
                        {
                            XmlNode searchCriteriaItemNode = SDFNode.SelectSingleNode("COE:searchCriteriaItem", manager);
                            if (searchCriteriaItemNode != null)
                            {
                                XmlAttribute fieldidAttrib = searchCriteriaItemNode.Attributes["fieldid"];
                                XmlAttribute tableidAttrib = searchCriteriaItemNode.Attributes["tableid"];
                                if (fieldidAttrib != null)
                                {
                                    fieldidAttrib.Value = "107";
                                }
                                else
                                {
                                    errorsInPatch = true;
                                    messages.Add("SDF field is not updated.");
                                    break;
                                }

                                if (tableidAttrib != null)
                                {
                                    tableidAttrib.Value = "1";
                                }
                                else
                                {
                                    errorsInPatch = true;
                                    messages.Add("SDF field is not updated.");
                                    break;
                                }

                            }

                            messages.Add("SDF field is updated.");
                        }
                        else
                        {
                            messages.Add("SDF field Not Updated.");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Patcher was not Updated.");
                    }
                    break;
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("SDF was uploaded successfully patched");
            }
            else
                messages.Add("SDF was uploaded patched with errors");

            return messages;
        }

    }
}


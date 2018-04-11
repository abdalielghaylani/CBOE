using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;


namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-132237: Batch information title was available in twice in Submit new record form
    /// </summary>
    class CSBR132237 : BugFixBaseCommand
    {
        /// <summary>
        /// Manuals steps to fix:
        ///1 - Open customize forms in registration and edit forms 4010, go to detailsForms, select hole coeform id="4", cut it and paste                   after coeForm id="1002".
        ///2 - Delete coeForm id="4" title. 
        /// </summary>     
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4010" || id == "4012" || id == "4011")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode coeFormId4 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']", manager);
                    XmlNode coeFormId1002 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']", manager);
                    if (coeFormId4 == null || coeFormId1002 == null)
                    {
                        messages.Add("The form id='4' or form id='1002' was not found on COEFormGroup id='" + id + "')");
                        errorsInPatch = true;
                    }
                    else
                    {
                        if (coeFormId4.SelectSingleNode("./COE:title", manager).InnerText.Trim() != "Batch Information")
                        {
                            messages.Add("The coeForm id ='4' doesn´t have the expected title on COEFormGroup id='" + id + "')");
                            errorsInPatch = true;
                        }
                        else
                        {
                            //changing form title
                            coeFormId4.SelectSingleNode("./COE:title", manager).InnerText = string.Empty;

                            //changing form position
                            doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms", manager).RemoveChild(coeFormId4);
                            doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms", manager).InsertAfter(coeFormId4, coeFormId1002);


                            if (coeFormId1002.SelectSingleNode("./COE:formDisplay", manager) != null)
                            {
                                if (coeFormId1002.SelectSingleNode("./COE:formDisplay/COE:style", manager) == null)
                                {
                                    //changing form style
                                    XmlNode styleNode = doc.CreateNode(XmlNodeType.Element, "style", doc.DocumentElement.NamespaceURI);
                                    styleNode.InnerText = "margin-bottom:0px;";
                                    coeFormId1002.SelectSingleNode("./COE:formDisplay", manager).AppendChild(styleNode);
                                }
                                else
                                {
                                    messages.Add("The coeForm id='1002' formDisplay doesn´t have the expected configuration on COEFormGroup id='" + id + "')");
                                    errorsInPatch = true;
                                }
                            }
                            else
                            {
                                messages.Add("The coeForm id='1002' doesn´t have formDisplay node on COEFormGroup id='" + id + "')");
                                errorsInPatch = true;
                            }
                        }

                        //Coverity fix - CID 19417
                        if (id == "4011")
                        {
                            XmlNode fEProjectEditMode = coeFormId1002.SelectSingleNode("./COE:editMode/COE:formElement[@name='Projects']", manager);
                            XmlNode fEIdentifiersEditMode = coeFormId1002.SelectSingleNode("./COE:editMode/COE:formElement[@name='Identifiers']", manager);
                            XmlNode fEProjectViewMode = coeFormId1002.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Projects']", manager);
                            XmlNode fEIdentifiersViewMode = coeFormId1002.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Identifiers']", manager);

                            if (fEIdentifiersEditMode != null)
                                coeFormId4.SelectSingleNode("./COE:editMode", manager).AppendChild(fEProjectEditMode);
                            else
                            {
                                messages.Add("The Projects formElement doesn´t exist on coeForm id='1002' at editMode on COEFormGroup id='" + id + "')");
                                errorsInPatch = true;
                            }
                            if (fEIdentifiersEditMode != null)
                                coeFormId4.SelectSingleNode("./COE:editMode", manager).AppendChild(fEIdentifiersEditMode);
                            else
                            {
                                messages.Add("The Identifiers formElement doesn´t exist on coeForm id='1002' at editMode on COEFormGroup id='" + id + "')");
                                errorsInPatch = true;
                            }
                            if (fEIdentifiersViewMode != null)
                                coeFormId4.SelectSingleNode("./COE:viewMode", manager).AppendChild(fEProjectViewMode);
                            else
                            {
                                messages.Add("The Projects formElement doesn´t exist on coeForm id='1002' at viewMode on COEFormGroup id='" + id + "')");
                                errorsInPatch = true;
                            }
                            if (fEIdentifiersViewMode != null)
                                coeFormId4.SelectSingleNode("./COE:viewMode", manager).AppendChild(fEIdentifiersViewMode);
                            else
                            {
                                messages.Add("The Identifiers formElement doesn´t exist on coeForm id='1002' at viewMode on COEFormGroup id='" + id + "')");
                                errorsInPatch = true;
                            }
                        }
                    }
                }

            }
            if (!errorsInPatch)
                messages.Add("CSBR132237 was successfully patched");
            else
                messages.Add("CSBR132237 was patched with errors");
            return messages;
        }
    }
}

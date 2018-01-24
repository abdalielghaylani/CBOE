using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-160172: Structure options become dislocated while adding third component to the mixture
    /// </summary>
    class CSBR160172 : BugFixBaseCommand
    {

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                if (id == "4012")
                {
                    XmlNode rootSelectedNode = formDoc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='BaseFragmentStructure']", manager);
                    XmlNode structureNode = formDoc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='StructureID']", manager);
                    if (rootSelectedNode != null)
                    {
                        // Remove width for CHEMDRAW object
                        XmlNode styleWidthNode = rootSelectedNode.SelectSingleNode("COE:displayInfo/COE:style", manager);
                        if (styleWidthNode != null)
                        {
                            rootSelectedNode.SelectSingleNode("COE:displayInfo", manager).RemoveChild(styleWidthNode);
                            messages.Add("Form[" + id + "]: The style node was removed successfully for COE:formElement[@name='BaseFragmentStructure']");
                        }


                        XmlNode fieldConfigNode = rootSelectedNode.SelectSingleNode("COE:configInfo/COE:fieldConfig", manager);
                        if (fieldConfigNode != null)
                        {
                            // CSS Class
                            XmlNode cssClass = fieldConfigNode.SelectSingleNode("COE:CSSClass", manager);
                            if (cssClass == null)
                            {
                                cssClass = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSClass", "COE.FormGroup");
                                fieldConfigNode.AppendChild(cssClass);
                            }
                            cssClass.InnerText = "FEStructure";                            

                            // Height
                            XmlNode Height = fieldConfigNode.SelectSingleNode("COE:Height", manager);
                            if (Height == null)
                            {
                                Height = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Height", "COE.FormGroup");
                                fieldConfigNode.AppendChild(Height);
                            }
                            Height.InnerText = "300px";
                            

                            // Width
                            XmlNode width = fieldConfigNode.SelectSingleNode("COE:width", manager);
                            if (width == null)
                            {
                                width = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "width", "COE.FormGroup");
                                fieldConfigNode.AppendChild(width);                                
                            }
                            width.InnerText = "350px";
                           
                            messages.Add("Form[" + id + "]: The fieldConfig node is updated successfully for COE:formElement[@name='BaseFragmentStructure']");

                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Form[" + id + "]:The fieldConfig node is not available to apply patch");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:The formElement[@name='BaseFragmentStructure'] " + id + " does not exist");
                    }
                    if (structureNode != null)
                    {
                        XmlNode displayNode = structureNode.SelectSingleNode("COE:displayInfo", manager);

                        if (displayNode != null)
                        {
                            XmlNode styleNode = displayNode.SelectSingleNode("COE:style", manager);
                            if (styleNode == null)
                            {
                                styleNode = structureNode.OwnerDocument.CreateNode(XmlNodeType.Element, "style", "COE.FormGroup");
                                displayNode.AppendChild(styleNode);
                            }                           
                            styleNode.InnerText = "width:88%;";
                            messages.Add("Form[" + id + "]: The style node is updated successfully for COE:formElement[@name='StructureID']");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Form[" + id + "]:The displayNode does not exist");
                        }
                        
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:The formElement[@name='StructureID'] " + id + " does not exist");
                    }
                    break;
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR160172 was successfully patched.");
            else
                messages.Add("CSBR160172 was not patched due to errors.");
            return messages;
        }

    }
}

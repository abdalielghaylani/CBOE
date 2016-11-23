using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary> 
    /// </summary>
    public class CSBR158036 : BugFixBaseCommand
    {
        /// <summary>
        /// Manual steps to fix:     
        /// Open coeframeworkconfig.xml and look for the generalOptions belonging to the parent application REGISTRATION in the ChemBioViz's application section.               
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlNode ParentNode = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour");                        
            XmlNode node = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4019']");
            XmlNode InsertAfterNode = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4006']");
            if (ParentNode != null)
            {
                if (node == null)
                {
                    node = frameworkConfig.CreateNode(XmlNodeType.Element, "form", null);
                    XmlAttribute formId = node.OwnerDocument.CreateAttribute("formId");
                    node.Attributes.Append(formId);
                    node.Attributes["formId"].Value = "4019";
                    node.InnerXml = @"<chemDrawOptions chemDrawPluginPolicy=""Available""></chemDrawOptions>
                        <leftPanelOptions>
                            <queryManagement visible=""NO"" enabled=""NO""/>
						    <exportManagement visible=""NO"" enabled=""NO""/>
						    <searchPreferences visible=""NO"" enabled=""NO""/>
						</leftPanelOptions>
						<menuOptions>
						    <restoreMenu visible=""NO""/>
						    <refineMenu visible=""YES""/>
						    <queryMenu visible=""NO"" enabled=""NO""/>
						    <markedMenu visible=""NO""/>
						    <exportMenu visible=""NO"" enabled=""NO""/>
						    <printMenu visible=""YES"" enabled=""YES""/>
						    <resultsPerPageMenu visible=""YES""/>
						</menuOptions>";            
                    if (InsertAfterNode != null)
                    {
                        ParentNode.InsertAfter(node, InsertAfterNode);
                        messages.Add("Form 4019 added to coeframeworkconfig.xml");
                    }
                    else
                    {
                        ParentNode.AppendChild(node);
                        messages.Add("Form 4019 added to coeframeworkconfig.xml");
                    }
                }
                else if(node != null)
                {
                    XmlNode tempNode = frameworkConfig.SelectSingleNode("//formBehaviour/form[@formId='4019']/menuOptions/printMenu");
                    if (tempNode != null)
                    {
                        if (tempNode.Attributes["visible"].Value == "NO" || tempNode.Attributes["enabled"].Value == "NO")
                        {
                            tempNode.Attributes["visible"].Value = "YES";
                            tempNode.Attributes["enabled"].Value = "YES";
                            messages.Add("Print Menu was made visible/enabled for form4019 in ChemBioViz");
                        }
                    }
                    else
                    {
                        messages.Add("For Form 4019 in ChemBioViz Print node doesn't Exist");
                        errorsInPatch = true;
                    }
                }                                                   
                else
                {
                    messages.Add("Form 4019 already exists in coeframeworkconfig.xml");
                }            
            }
            else
            {
                messages.Add("Parent node for Form 4019 was not found");
                errorsInPatch = true;
            }

            if (!errorsInPatch)
                messages.Add("Form 4019 was successfully added to coeframeworkconfig.xml");
            else
                messages.Add("Form 4019 was added to coeframeworkconfig.xml with errors");
            return messages;
        }
    }
}

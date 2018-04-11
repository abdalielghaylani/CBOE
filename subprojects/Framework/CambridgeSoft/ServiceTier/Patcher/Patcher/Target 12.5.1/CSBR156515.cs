using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary> 
    /// </summary>
    public class CSBR156515 : BugFixBaseCommand
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
            XmlNode node = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4004']");
            XmlNode InsertAfterNode = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4003']");
            if (ParentNode != null)
            {
                if (node == null)
                {
                    node = frameworkConfig.CreateNode(XmlNodeType.Element, "form", null);
                    XmlAttribute formId = node.OwnerDocument.CreateAttribute("formId");
                    node.Attributes.Append(formId);
                    node.Attributes["formId"].Value = "4004";
                    node.InnerXml = @"<chemDrawOptions chemDrawPluginPolicy=""Available""></chemDrawOptions>
                        <leftPanelOptions>
                            <queryManagement visible=""NO"" enabled=""NO""/>
						    <exportManagement visible=""NO"" enabled=""NO""/>
						    <searchPreferences visible=""YES"" enabled=""YES""/>
						</leftPanelOptions>
						<menuOptions>
						    <restoreMenu visible=""YES""/>
						    <refineMenu visible=""YES""/>
						    <queryMenu visible=""YES"" enabled=""YES""/>
						    <markedMenu visible=""NO""/>
						    <exportMenu visible=""NO"" enabled=""NO""/>
						    <printMenu visible=""YES"" enabled=""YES""/>
						    <resultsPerPageMenu visible=""YES""/>
						</menuOptions>";
                    if (InsertAfterNode != null)
                    {
                        ParentNode.InsertAfter(node, InsertAfterNode);
                        messages.Add("Export control is disabled for Form id 4004 in coeframeworkconfig.xml");
                    }
                    else
                    {
                        ParentNode.AppendChild(node);
                        messages.Add("Export control is disabled for Form id 4004 in coeframeworkconfig.xml");
                    }
                }               
                else
                {
                    messages.Add("Export control is already disabled for Form id 4004 in coeframeworkconfig.xml");
                }
            }
            else
            {
                messages.Add("Parent node for Form id 4004 was not found in coeframeworkconfig.xml");
                errorsInPatch = true;
            }
            if (!errorsInPatch)
                messages.Add("CSBR-156515 was successfully patched");
            else
                messages.Add("CSBR-156515 was patched with errors");
            return messages;
        }
    }
}

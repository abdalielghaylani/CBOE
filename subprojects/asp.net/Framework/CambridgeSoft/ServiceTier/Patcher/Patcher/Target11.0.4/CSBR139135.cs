using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-139135: Feature Request: Link to COE Excel on CBOE Home page
    /// The suggestion is to add a link to installer of excel in Manage home page in Application and Utilities section 
    /// 
    ///Expected Result:
    ///As per CSBR-139135 “ChemBioViz Excel” link should be added at home page to access ChemBioViz excel addin service directly
    ///
    ///Actual Result:
    ///There is such link currently
    ///
    /// </summary>


    class CSBR139135 : BugFixBaseCommand
    {
        /// <summary>
        /// -Manual steps to add the link
        /// Open COEFrameworkConfig.xml file
        /// Under coeHomeSettings - Utility section, below 'CBVClient' node
        /// add the following node
        ///    <add name="CBVExcel" display="ChemBioViz Excel" tip="Launch the Installer for ChemBioViz Excel" 
        ///     url="/cfserverasp/Clients/ChemBioVizExcel Addin/ChemBioVizExcelAddin1104.exe" privilege="" linkIconSize="small" 
        ///     linkIconBasePath="Icon_Library/windows_collection/Windows_PNG/PNG" linkIconFileName="search.png"/>
        /// create 'ChemBioVizExcel Addin' folder if does not exists
        /// make sure you have ChemBioVizExcelAddin1104.exe file in the specified location.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            XmlNode ExcelLinkPanelParentNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Utilities']/links");
            XmlNode ExcelLinkNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Utilities']/links/add[@name='CBVExcel']");
            XmlNode CBVClientNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Utilities']/links/add[@name='CBVClient']");

            if (ExcelLinkPanelParentNode != null)
            {
                // Adding node if does not exists
                if (ExcelLinkNode == null)
                {
                    ExcelLinkNode = frameworkConfig.CreateNode(XmlNodeType.Element, "add", null);

                    // Adding Attributes

                    XmlAttribute NameAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("name");
                    ExcelLinkNode.Attributes.Append(NameAttribute);
                    ExcelLinkNode.Attributes["name"].Value = "CBVExcel";

                    XmlAttribute DisplayAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("display");
                    ExcelLinkNode.Attributes.Append(DisplayAttribute);
                    ExcelLinkNode.Attributes["display"].Value = "ChemBioViz Excel";

                    XmlAttribute TipAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("tip");
                    ExcelLinkNode.Attributes.Append(TipAttribute);
                    ExcelLinkNode.Attributes["tip"].Value = "Launch the Installer for ChemBioViz Excel";

                    XmlAttribute URLAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("url");
                    ExcelLinkNode.Attributes.Append(URLAttribute);
                    ExcelLinkNode.Attributes["url"].Value = "/cfserverasp/Clients/ChemBioVizExcel Addin/ChemBioVizExcelAddin1104.exe";

                    XmlAttribute PrivilegeAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("privilege");
                    ExcelLinkNode.Attributes.Append(PrivilegeAttribute);
                    ExcelLinkNode.Attributes["privilege"].Value = "";

                    XmlAttribute LinkIconSizeAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("linkIconSize");
                    ExcelLinkNode.Attributes.Append(LinkIconSizeAttribute);
                    ExcelLinkNode.Attributes["linkIconSize"].Value = "small";

                    XmlAttribute LinkIconBasePathAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("linkIconBasePath");
                    ExcelLinkNode.Attributes.Append(LinkIconBasePathAttribute);
                    ExcelLinkNode.Attributes["linkIconBasePath"].Value = "Icon_Library/windows_collection/Windows_PNG/PNG";

                    XmlAttribute LinkIconFileNameAttribute = ExcelLinkNode.OwnerDocument.CreateAttribute("linkIconFileName");
                    ExcelLinkNode.Attributes.Append(LinkIconFileNameAttribute);
                    ExcelLinkNode.Attributes["linkIconFileName"].Value = "search.png";

                    // Inserts CBVExcel Node after CBVClient if it exists else appends at last
                    if (CBVClientNode != null)
                        ExcelLinkPanelParentNode.InsertAfter(ExcelLinkNode, CBVClientNode);
                    else
                        ExcelLinkPanelParentNode.AppendChild(ExcelLinkNode);

                    messages.Add("ChemBio Excel link added at coeHomeSetting-->groups-->utilities section.");
                }
                else
                {
                    messages.Add("ChemBio Excel link already exists.");
                }
            }
            else
            {
                errorInPatch = true;
                messages.Add("ChemBio Excel link was not added.");
            }

            if (!errorInPatch)
            {
                messages.Add("CSBR-139135 was successfully patched");
            }
            else
                messages.Add("CSBR-139135 was patched with errors");

            return messages;
        }
    }
}

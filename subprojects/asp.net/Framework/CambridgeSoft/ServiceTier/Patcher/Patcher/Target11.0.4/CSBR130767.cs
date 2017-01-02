using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-130767: Untitled page is available as title for the Table Editor form
    /// Note#9: Hemadevi Ramireddy marked this bug as Not Fixed (N:o) because
    /// Right part of the form is not able to view. and it does not allow the user to maximize the form. 
    /// 
    ///Expected Result:
    ///As per CSBR-130767 “TableEditor” form width should be more to visible entire content - Security Managet section
    ///
    ///Actual Result:
    ///“TableEditor” form has less width and it is hiding the right side content.
    ///
    /// </summary>
    class CSBR130767 : BugFixBaseCommand
    {
        /// <summary>
        /// - Manual steps to fix:
        /// - Open COEFrameworkConfig.xml file. 
        /// - Search coeConfiguration section -> applications -> MANAGER entry -> applicationhome --> groups section -> SecurityManagerPanelContents entry -> links section --> TableEditor entry. 
        /// - set the value as "width=980" to newWindow attribute of 'TableEditor' entry.
        /// - Search coeHomeSettings section -> groups -> COE entry -> links section --> TableEditor entry. 
        /// - set the value as "width=980" to newWindow attribute of 'TableEditor' entry..
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

            XmlNode TableEditorPanelNode = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='MANAGER']/applicationHome/groups/add[@name='SecurityManagerPanelContents']/links/add[@name='TableEditor']");
            XmlNode TableEditorNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links/add[@name='TableEditor']");

            if (TableEditorPanelNode != null)
            {
                if (TableEditorPanelNode.Attributes["newWindow"] != null)
                {
                    TableEditorPanelNode.Attributes["newWindow"].Value = "width=980";
                    messages.Add("TableEditor window width increased at coeConfiguration-->SecurityManagerPanelContents section.");
                }
                else
                {
                    XmlAttribute NewWindowAttribute = TableEditorPanelNode.OwnerDocument.CreateAttribute("newWindow");
                    NewWindowAttribute.Value = "width=980";
                    TableEditorPanelNode.Attributes.Append(NewWindowAttribute);
                    messages.Add("TableEditor window width increased at coeConfiguration-->SecurityManagerPanelContents section.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("TableEditor link was not present at coeConfiguration-->SecurityManagerPanelContents section - skipping.");
            }

            if (TableEditorNode != null)
            {
                if (TableEditorNode.Attributes["newWindow"] != null)
                {
                    TableEditorNode.Attributes["newWindow"].Value = "width=980";
                    messages.Add("TableEditor window width increased at coeHomeSettings-->COE section.");
                }
                else
                {
                    XmlAttribute NewWindowAttribute = TableEditorNode.OwnerDocument.CreateAttribute("newWindow");
                    NewWindowAttribute.Value = "width=980";
                    TableEditorNode.Attributes.Append(NewWindowAttribute);
                    messages.Add("TableEditor window width increased at coeHomeSettings-->COE section.");
                }
            }
            else
            {
                if (!errorsInPatch)
                    errorsInPatch = true;
                messages.Add("TableEditor link was not present at coeHomeSettings-->COE section - skipping.");
            }


            if (!errorsInPatch)
            {
                messages.Add("CSBR-130767 was successfully patched");
            }
            else
                messages.Add("CSBR-130767 was patched with errors");

            return messages;
        }
    }
}

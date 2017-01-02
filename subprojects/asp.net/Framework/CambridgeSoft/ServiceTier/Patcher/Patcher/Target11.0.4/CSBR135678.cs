using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-135678: Main page link for Table editor in DV Manager leads to Security Manager
    /// (no version): Jordan Shatsoff suggested me that 
    /// The contents of the home page are controlled by COEFrameworkConfig.xml.  If you want to remove the link then you need to follow these steps
    /// 1) Remove it from the xml on your system and test.
    /// 2) If that works then check the change into here //depot/ENotebook/Degas_Development/ChemOfficeEnterprise/Framework/CambridgeSoft/ChemOfficeEnterprise11/COEFrameworkConfig.xml <text>
    /// 3) Write a patcher that will remove that link.
    /// BUT....I have looked at this bug more closely, I don't think that is really the correct solution to remove the link (well it might be short term, but read below).
    /// Lets say that we did want to add a table that was for Dataview manager to TableEditor.  In that scenario we would want a link to it.  BUT when we click Main from that link we would want it to take us to the Dataview Manager Main and NOT the Security Manager Main.  This might mean we have to use 2 different aspx pages.
    /// SO....This is utlimately what I think.
    /// If we are going to need a new page for table editor specific to Dataview Manager then we should remove the link according to the provided steps. (This is probably the case)
    /// If they can somehow share the same page then the table editor page will need some kind of context so it knows which main page to go back to (Security vs. DV Manager).
    /// Krishna Comments :
    /// The issue is not only with menu link , because it is it is using the Security Manager master page all the links and headings are coming from the Security Manager master page , so that I think we need to use the new page with DataView Manager master.  Hence as per your suggestion if we need new page then we need to remove the link according to the provided steps.
    ///     
    ///Expected Result:
    ///As per CSBR-135678 “TableEditor” link should not be shown at home page - DataView Manager section
    ///
    ///Actual Result:
    ///“TableEditor” is available in Home page - DataView Manager section with Security Manager master.
    ///
    /// </summary>
    class CSBR135678 : BugFixBaseCommand
    {
        /// <summary>
        /// - Manual steps to fix:
        /// - Open COEFrameworkConfig.xml file. 
        /// - Search coeConfiguration section -> applications -> MANAGER entry -> applicationhome --> groups section -> DataviewManagerPanelContents entry -> links section. 
        /// - Remove entry for 'TableEditor'.
        /// - Search coeHomeSettings section -> groups -> COEMANAGER_DV entry -> links section. 
        /// - Remove entry for 'TableEditor'.
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

            XmlNode TableEditorPanelNode = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='MANAGER']/applicationHome/groups/add[@name='DataviewManagerPanelContents']/links/add[@name='TableEditor']");
            XmlNode TableEditorNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COEMANAGER_DV']/links/add[@name='TableEditor']");

            if (TableEditorPanelNode != null)
            {
                TableEditorPanelNode.ParentNode.RemoveChild(TableEditorPanelNode);
                messages.Add("TableEditor link removed from coeConfiguration-->DataviewManagerPanelContents section.");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("TableEditor link was not present at coeConfiguration-->DataviewManagerPanelContents section - skipping.");
            }

            if (TableEditorNode != null)
            {
                TableEditorNode.ParentNode.RemoveChild(TableEditorNode);
                messages.Add("TableEditor link removed from coeHomeSettings-->COEMANAGER_DV section.");
            }
            else
            {
                if (!errorsInPatch)
                    errorsInPatch = true;
                messages.Add("TableEditor link was not present at coeHomeSettings-->COEMANAGER_DV section - skipping.");
            }


            if (!errorsInPatch)
            {
                messages.Add("CSBR-135678 was successfully patched");
            }
            else
                messages.Add("CSBR-135678 was patched with errors");

            return messages;
        }
    }
}

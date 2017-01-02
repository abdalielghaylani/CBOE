using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding new DocManager elements and modification of existed links regarding DocManager
    /// </summary>
    class AddDocManagerElements : BugFixBaseCommand
	{        
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            // modification of URLs
            XmlNode MainPageNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='MainPage']");
            XmlNode SearchDocumentsNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='SearchDocuments']");
            XmlNode SubmitDocumentsNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='SubmitDocuments']");
            XmlNode RecentActivitiesNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='RecentActivities']");

            if (MainPageNode != null)
            {
                MainPageNode.Attributes["url"].Value = "/DocManager/Forms/Public/ContentArea/Home.aspx";                
            }
            if (SearchDocumentsNode != null)
            {
                SearchDocumentsNode.Attributes["url"].Value = "/DocManager/Forms/Public/ContentArea/SearchDocument.aspx";
            }
            if (SubmitDocumentsNode != null)
            {
                SubmitDocumentsNode.Attributes["url"].Value = "/DocManager/Forms/Public/ContentArea/SubmitNewDocument.aspx";
            }
            if (RecentActivitiesNode != null)
            {
                RecentActivitiesNode.Attributes["url"].Value = "/DocManager/Forms/Public/ContentArea/RecentActivities.aspx";
            }

            // adding new DocManager Element
            XmlNode ParentNode = frameworkConfig.SelectSingleNode("//applications");
            XmlNode DocManagerNode = frameworkConfig.SelectSingleNode("//applications/add[@name='DOCMANAGER']");

            // adds or updates docmanager if the nodes exists without child nodes
            if (DocManagerNode != null && !(DocManagerNode.HasChildNodes))
            {
                DocManagerNode = frameworkConfig.CreateNode(XmlNodeType.Element, "add", null);
                createNewAttribute("name", "DOCMANAGER", DocManagerNode);
                createNewAttribute("database", "DOCMGR", DocManagerNode);

                AddNodes(DocManagerNode);

                ParentNode.AppendChild(DocManagerNode);

                messages.Add("Element has been added successfully");
            }
            else if (DocManagerNode == null)
            {
                AddNodes(DocManagerNode);
                messages.Add("Element has been updated successfully");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("This element might have been existed already");
            }

            if (!errorsInPatch)
            {
                messages.Add("DocManager was successfully patched");
            }
            else
                messages.Add("DocManager was patched with errors");

            return messages;
        }

        private void createNewAttribute(string attributeName, string attributeValue, XmlNode nodeName)
        {
            XmlAttribute attributes = nodeName.OwnerDocument.CreateAttribute(attributeName);
            nodeName.Attributes.Append(attributes);
            nodeName.Attributes[attributeName].Value = attributeValue;
        }

        private void AddNodes(XmlNode DocManagerNode)
        {
            DocManagerNode.InnerXml = "<applicationHome>" +
                  "<groups>" +
                  "<add name=\"DocManagerPanelContents\" enabled=\"true\" coeIdentifier=\"DocManager\" pageSectionTarget=\"panel\" display=\"DocManager System\" newWindow=\"false\" color=\"blue\" helpText=\"Click on the links below to enter to any of the sections\">" +
                  "<links>" +
                  "<add name=\"SearchDocuments\" display=\"Search documents\" tip=\"Search DocManager documents\" url=\"/DocManager/Forms/Public/ContentArea/SearchDocument.aspx\" privilege=\"SEARCH_DOCS\" linkIconSize=\"small\" linkIconBasePath=\"Icon_Library/Windows_Collection/Windows_png/PNG\" linkIconFileName=\"search_folder.png\" />" +
                  "<add name=\"SubmitDocuments\" display=\"Submit documents\" tip=\"Submit a document to DocManager\" url=\"/DocManager/Forms/Public/ContentArea/SubmitNewDocument.aspx\" privilege=\"SUBMIT_DOCS\" linkIconSize=\"small\" linkIconBasePath=\"Icon_Library/network_Collection/network_aqua/PNG\" linkIconFileName=\"attach.png\"  />" +
                  "<add name=\"RecentActivities\" display=\"Recent Activities\" tip=\"Recent history of DocManager activities\" url=\"/DocManager/Forms/Public/ContentArea/RecentActivities.aspx\" privilege=\"VIEW_HISTORY\" linkIconSize=\"small\" linkIconBasePath=\"Icon_Library/windows_Collection/windows_png/PNG\" linkIconFileName=\"history.png\"/>" +
                  "</links>" +
                  "</add>" +
                  "</groups>" +
                  "</applicationHome>";
        }
    }
}

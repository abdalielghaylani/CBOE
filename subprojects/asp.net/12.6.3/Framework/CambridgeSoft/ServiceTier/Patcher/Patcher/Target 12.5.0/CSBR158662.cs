using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary> 
    /// </summary>
    public class CSBR158662 : BugFixBaseCommand
    {
        #region private
        List<string> messages = new List<string>();
        #endregion

        /// <summary>
        /// Manual steps to fix:     
        /// Open coeframeworkconfig.xml. Search for 4006 in the file.  Find the menu options node.  Add/Replace with a new SendTo node: <sendToMenu visible="NO" enabled="NO" />           
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            bool errorsInPatch = false;
            XmlNode parentNode = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4006']/menuOptions");
            XmlNode node = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4006']/menuOptions/sendToMenu");

            if (parentNode != null)
            {

                //If it is there we really don't care what the configuration is.  Just remove it.
                if (node != null)
                {
                    parentNode.RemoveChild(node);
                }

                //Now we can add it back in with the correct settings.
                AddSendToConfig(parentNode);

            }
            else
            {
                messages.Add("Parent node for Form 4006 was not found.  Default configuration will be used.");
                errorsInPatch = true;
            }

            if (!errorsInPatch)
                messages.Add("Form 4006 was successfully modified to hide the sendToMenu.");
            else
                messages.Add("Form 4006 was modified in coeframeworkconfig.xml with errors.");
            return messages;

        }



        private void AddSendToConfig(XmlNode parentNode)
        {
            try
            {
                XmlNode newChild = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "sendToMenu", null);
                createNewAttribute("visible", "NO", ref newChild);
                createNewAttribute("enabled", "NO", ref newChild);

                parentNode.AppendChild(newChild);
                messages.Add("SendTo menu has been hidden successfully.");
            }
            catch (Exception ex)
            {
                messages.Add("SendTo menu has not been hidden with the following errors : " + ex.Message);
            }
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Originally teh link said Send to Inventory. Since Alcon project we prefer to use Create Container(s)
    /// </summary>
	public class SendToInventoryTextChange : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix.
        /// 
        /// Open COEFrameworkConfig.xml file and look for the text "SendToInventory".
        /// Modify the text attribute to be "Create Container(s)" and the tooltip attribute to be "Create Invetory Containers"
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
            bool errorInPatch = false;
            XmlNode sendToInventoryNode = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='CHEMBIOVIZ']/formBehaviour/form[@formId='4003']/actionLinks/link[@id='SendToInventoryLink']");

            if (sendToInventoryNode != null)
            {
                if (sendToInventoryNode.Attributes["text"].Value == "Send To Inventory")
                {
                    sendToInventoryNode.Attributes["text"].Value = "Create container(s)";
                    messages.Add("SendToInventory text was changed to \"Create container(s)\"");
                }
                else
                    messages.Add("The text configured for Send to inventory link was not the one expected. No change applied");

                if (sendToInventoryNode.Attributes["tooltip"].Value == "Inventory - Send to")
                {
                    sendToInventoryNode.Attributes["tooltip"].Value = "Create Inventory Containers";
                    messages.Add("SendToInventory tooltip was changed to \"Create Inventory Containers\"");
                }
                else
                    messages.Add("The tooltip configured for Send to inventory link was not the one expected. No change applied");
            }
            else
            {
                errorInPatch = true;
                messages.Add("Send to Inventory link under form 4003 was not found");
            }

            if (!errorInPatch)
                messages.Add("Send To Inventory Text succesfully updated");
            else
                messages.Add("Send To Inventory Text was not updated");

            return messages;
        }
    }
}

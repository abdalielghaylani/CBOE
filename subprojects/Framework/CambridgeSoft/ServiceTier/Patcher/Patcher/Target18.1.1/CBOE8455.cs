using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8455 : BugFixBaseCommand
	{
        /// <summary>
        /// patcher for changing InvContainersDataviewID system setting for registration.
        /// </summary>
        public override List<string> Fix(
            List<XmlDocument> forms
            , List<XmlDocument> dataviews
            , List<XmlDocument> configurations
            , XmlDocument objectConfig
            , XmlDocument frameworkConfig
            )
        {

            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            XmlDocument regConfig = null;

            try
            {
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']/groups") != null)
                    {
                        regConfig = new XmlDocument();
                        regConfig.LoadXml(config.OuterXml);
                        string lockingEnabledPatch = "//add[@name='INVENTORY']/settings/add[@name='InvContainersDataviewID']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);

                        if (xpathRootNode != null)
                        {
                            if (xpathRootNode.Attributes["value"].Value == "8005")
                            {
                                xpathRootNode.Attributes["value"].Value = "3001";
                                configurations[configurations.IndexOf(config)] = regConfig;
                                messages.Add("Replaced InvContainersDataviewID with correct ID Value");
                            }
                            else
                                messages.Add("RegSettings already contains InvContainersDataviewID with correct ID.");
                        }
                        else
                            messages.Add("RegSettings doesnot contain InvContainersDataviewID.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }
            finally
            {
                if (!errorsInPatch)
                {
                    messages.Add("InvContainersDataviewID was successfully patched");
                }
                else
                    messages.Add("InvContainersDataviewID patch was aborted due to errors");
            }
            return messages;
        }      
	}
}

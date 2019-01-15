using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE8447:BugFixBaseCommand
	{
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
                        string lockingEnabledPatch = "//add[@name='ApprovalsEnabled']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(lockingEnabledPatch);

                        if (xpathRootNode != null)
                        {
                            if (xpathRootNode.Attributes["isAdmin"].Value == "True")
                            {
                                xpathRootNode.Attributes["isAdmin"].Value = "False";
                                configurations[configurations.IndexOf(config)] = regConfig;
                                messages.Add("Updated isAdmin setting.");
                            }
                            else
                                messages.Add("Already the isAdmin Attribute is updated.");
                        }
                        else
                            messages.Add("ApprovalsEnabled Tag is missing.");
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
                    messages.Add("CBOE8447 was successfully patched");
                }
                else
                    messages.Add("CBOE8447 patch was aborted due to errors");
            }
            return messages;
        }      
	}
}

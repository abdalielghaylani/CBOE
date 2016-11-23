using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Fix for CSBR-133765: BatchesAsTemp setting is unused and should be eliminated.
    /// </summary>
    class CSBR133765 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            try
            {
                XmlDocument regConfig = null;
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                    {
                        regConfig = config;
                        break;
                    }
                }
                //Coverity fix - CID 19422
                if (regConfig != null)
                {
                    XmlNode batchesToTempNode = regConfig.SelectSingleNode(
                        "/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='BatchesToTemp']");

                    if (batchesToTempNode != null)
                    {
                        batchesToTempNode.ParentNode.RemoveChild(batchesToTempNode);
                        messages.Add("'BatchesToTemp' setting removed.");
                    }
                    else
                    {
                        messages.Add("'BatchesToTemp' setting was not found.");
                    }
                }
                else
                {
                    messages.Add("Reg App Settings node not found.");
                }
            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }

            return messages;
        }
	}
}

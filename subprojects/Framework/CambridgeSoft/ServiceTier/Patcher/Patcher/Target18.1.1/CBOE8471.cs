using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE8471:BugFixBaseCommand
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
                        string ProjectsUsedPatch = "//add[@name='MISC']/settings/add[@name='ProjectsUsed']";
                        string BatchProjectsUsedPatch = "//add[@name='MISC']/settings/add[@name='BatchProjectsUsed']";
                        XmlNode xpathRootNode = regConfig.SelectSingleNode(ProjectsUsedPatch);
                        XmlNode ypathRootNode = regConfig.SelectSingleNode(BatchProjectsUsedPatch);

                        if (xpathRootNode != null || ypathRootNode != null)
                        {
                            if (xpathRootNode.Attributes["isAdmin"].Value == "True")
                            {
                                xpathRootNode.Attributes["isAdmin"].Value = "False";                                
                                messages.Add("Replaced  ProjectsUsed with correct isAdmin Value");
                            }
                            else
                                messages.Add("RegSettings already contains ProjectsUsed with correct Value.");

                            if (ypathRootNode.Attributes["isAdmin"].Value == "True")
                            {
                                ypathRootNode.Attributes["isAdmin"].Value = "False";                              
                                messages.Add("Replaced BatchProjectsUsed with correct isAdmin Value");
                            }
                            else
                                messages.Add("RegSettings already contains BatchProjectsUsed with correct Value.");
                                configurations[configurations.IndexOf(config)] = regConfig;
                        }
                        else
                            messages.Add("RegSettings doesnot contain ProjectsUsed and BatchProjectsUsed.");
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
                    messages.Add("CBOE8471 was successfully patched");
                }
                else
                    messages.Add("CBOE-8471 patch was aborted due to errors");
            }
            return messages;
        }  
	}
}

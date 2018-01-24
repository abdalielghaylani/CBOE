using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-133562: IE6: Batch Submission link under DocManager Enterprise cannot be selected.
    /// </summary>
    class CSBR133562 : BugFixBaseCommand
    {
        /// <summary>
        /// Steps to manually fix:
        /// 1 - Open COEFrameworkConfig.xml file.
        /// 2 - Go to XmlPath: coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='BatchSubmission'].
        /// 3 - Remove or comment the node.
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
            bool errorsInPatch = false;

            XmlNode batchSubmission = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='BatchSubmission']");

            if (batchSubmission != null)
            {
                frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='DocManager']/links").RemoveChild(batchSubmission);
                messages.Add("SUCCESS: coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='BatchSubmission'] was removed");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("ERROR: coeHomeSettings/groups/add[@name='DocManager']/links/add[@name='BatchSubmission'] was not found");
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-133562 was successfully patched");
            }
            else
                messages.Add("CSBR-133562 was patched with errors");

            return messages;
        }

    }
}

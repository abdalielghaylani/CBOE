using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class SeqProjectsForProjects : BugFixBaseCommand
	{

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            /* //add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT'] */
            /* sequenceName="SEQ_PROJECTS" */
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT']");
            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("The node VW_PROJECT was not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlAttribute seqName = node.Attributes["sequenceName"];
                if (seqName != null)
                {
                    errorsInPatch = true;
                    messages.Add("The node VW_PROJECT was already configured to have a sequence. No change was made");
                }
                else
                {
                    seqName = frameworkConfig.CreateAttribute("sequenceName");
                    seqName.Value = "SEQ_PROJECTS";
                    node.Attributes.Append(seqName);
                    messages.Add("Sequence name was added successfully to VW_PROJECT");
                }
            }
            if (!errorsInPatch)
                messages.Add("SeqProjectsForProjects was successfully patched");
            else
                messages.Add("SeqProjectsForProjects was patched with errors");
            return messages;
        }
    }
}

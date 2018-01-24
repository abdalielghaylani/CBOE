using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Bug Description:
    /// 
    /// Table editor: projects shows user list when RLS is  not enabled
    /// 
    /// 1. go to the table editor>projects view
    /// 2. create a new project
    /// 
    /// In the create project page, there are list boxes for users to be added to projects.  Since I didn't enable row level security, I shouldn't see this list box.  I am allowed to add users to the list for access to the project.  When I return to the project I added by choosing edit, the user list is empty again.
    /// </summary>
	public class CSBR115952 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open coeframeworkconfig.xml and search for &lt;add name="VW_PROJECT" under registration section and append to that node the following Attribute: disableChildTables="True"
        ///
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

            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='REGISTRATION']/tableEditor/add[@name='VW_PROJECT']");
            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("The node VW_PROJECT was not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlAttribute disableChildTables = node.Attributes["disableChildTables"];
                if (disableChildTables == null)
                {
                    disableChildTables = frameworkConfig.CreateAttribute("disableChildTables");
                    disableChildTables.Value = "True";
                    node.Attributes.Append(disableChildTables);
                    messages.Add("disableChildTables succesfully added");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("disableChildTables was already present in VW_PROJECT configuration and was not modified");
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR115952 was successfully patched");
            else
                messages.Add("CSBR115952 was patched with errors");
            return messages;
        }
    }
}

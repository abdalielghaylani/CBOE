using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary> 
    /// Adding correct Icon path for Manage Groups link
    /// </summary>
    public class CSBR162684 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode ManageGroupsNode = frameworkConfig.SelectSingleNode("/configuration/coeHomeSettings/groups/add[@name='COE']/links/add[@name='ManageGroups']");


            if (ManageGroupsNode == null || ManageGroupsNode.Attributes["linkIconBasePath"].Value == null)
            {
                errorsInPatch = true;
            }

            if (errorsInPatch)
            {
                messages.Add("CSBR162684 had  errors.");
            }
            else
            {
                ManageGroupsNode.Attributes["linkIconBasePath"].Value = "Icon_Library/custom_Collection/PNG";
                messages.Add("CSBR162684 was successfully fixed.");
            }
            return messages;

        }
    }
}

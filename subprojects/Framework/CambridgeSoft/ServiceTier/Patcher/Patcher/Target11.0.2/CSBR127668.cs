using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Bug Description:
    /// 
    /// Manage configuration: User cannot resize the window and there are not scrollbars
    /// 
    /// 1- Log into COE Manager as a DV Manager user. e.g: CSSADMIN/CSSADMIN
    /// 2- Go to DV Manager section and click on Manage Configuration icon
    /// 
    /// Expected result: The Manage configuration window should be opened and user should be able to see all the contents
    /// 
    /// Obtained result: The opened window cannot be re-sized an the contents are not completely shown. There are no scrollbars to move the page.
    /// See the attached screenshot for further details
    /// 
    /// </summary>
	public class CSBR127668 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to Fix:
        /// 
        /// Open coeframeworkconfig.xml and look for the string add name="ConfigControlSettings" under &lt;add name="COE"
        /// Replace its attribute
        /// newWindow="true" 
        /// 
        /// with
        /// 
        /// newWindow="location=yes,status=yes,scrollbars=yes,toolbar=yes,resizable=yes,width=1000,height=550,top=0,left=0,menubar=yes,dialog=no,minimizable=yes"
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

            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='COE']/links/add[@name='ConfigControlSettings']");
            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("The item ConfigControlSettings was not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlAttribute newWindow = node.Attributes["newWindow"];
                if(newWindow == null)
                {
                    newWindow = frameworkConfig.CreateAttribute("newWindow");
                    node.Attributes.Append(newWindow);
                }
                if (newWindow.Value.ToLower() != "true")
                {
                    errorsInPatch = true;
                    messages.Add("newWindow attribute seems to be already modified and was not overriden");
                }
                else
                {
                    newWindow.Value = "location=yes,status=yes,scrollbars=yes,toolbar=yes,resizable=yes,width=1000,height=550,top=0,left=0,menubar=yes,dialog=no,minimizable=yes";
                    messages.Add("newWindow was modified with the new parameter");
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR127668 was successfully patched");
            else
                messages.Add("CSBR127668 was patched with errors");
            return messages;
        }
    }
}

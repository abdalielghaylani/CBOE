using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-144633: Global Search giving ORA-00942 error for users that don't have any INV role
    /// </summary>
    class CSBR144633 : BugFixBaseCommand
	{
        /// <summary>
        /// Steps to manually fix:
        /// 1 - Open COEFrameworkConfig.xml file.
        /// 2 - Go to XmlPath: coeHomeSettings/groups/add[@name='Inventory']/links/add[@name='GlobalSearch'].
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

            XmlNode GlobalSearch = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Inventory']/links/add[@name='GlobalSearch']");

            if (GlobalSearch != null)
            {
                GlobalSearch.Attributes["privilege"].Value = "INV_BROWSE_ALL";
                messages.Add("SUCCESS: Changed the privilege from SEARCH_REG to INV_BROWSE_ALL for Global Search");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("ERROR: coeHomeSettings/groups/add[@name='Inventory']/links/add[@name='GlobalSearch'] was not found");
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-144633 was successfully patched");
            }
            else
                messages.Add("CSBR-144633 was patched with errors");

            return messages;
        }

    }
}

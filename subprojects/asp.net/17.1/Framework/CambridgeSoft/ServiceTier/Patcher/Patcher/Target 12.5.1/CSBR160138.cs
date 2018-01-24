using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Update delete privilege to picklist table editor
    /// </summary>
    public class CSBR160138 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string defaultPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_PICKLIST']";

            // modification of picklist delete privilege
            XmlNode MainPageNode = frameworkConfig.SelectSingleNode(defaultPath);

            #region Validate

            if (MainPageNode.Attributes["deletePriv"] != null)
            {
                MainPageNode.Attributes["deletePriv"].Value = "HIDME";
                errorsInPatch = false;
                messages.Add("The deletePriv was updated succesfully.");
            }

          
            #endregion

            #region Update
            if (errorsInPatch)
            {
                messages.Add("CSBR160138 was fixed with partial update.");
            }
            else
            {
                messages.Add("CSBR160138 was successfully fixed.");
            }
            #endregion

            return messages;
            #endregion
        }
    }
}

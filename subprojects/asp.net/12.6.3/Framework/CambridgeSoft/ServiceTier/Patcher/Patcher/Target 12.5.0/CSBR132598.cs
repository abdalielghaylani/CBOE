using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding SEARCH_TEMP privilege to search temporary and review register links for system information
    /// </summary>
  public  class CSBR132598 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig 
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string defaultPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/applicationHome/groups/add[@name='RegistrationDash1']";
            // modification of Registration Dash Board
            XmlNode MainPageNode = frameworkConfig.SelectSingleNode(defaultPath);

            #region Validate
            XmlNode SearchTemporaryNode = frameworkConfig.SelectSingleNode(defaultPath + "/customItems/add[@name='TempRegistries']");
            XmlNode SearchAllTemporaryNode = frameworkConfig.SelectSingleNode(defaultPath + "/customItems/add[@name='AllTempRegistries']");
            if (MainPageNode == null)
            {
                errorsInPatch = true;
            }
            if (SearchTemporaryNode == null || SearchAllTemporaryNode == null)
            {
                errorsInPatch = true;
            }
            //Coverity fix - CID 19415
            else if (SearchTemporaryNode.Attributes["privilege"] == null || SearchAllTemporaryNode.Attributes["privilege"] == null)
            {
                errorsInPatch = true;
            }
            #endregion

            #region Update
            if (errorsInPatch)
            {
                messages.Add("CSBR132598 was fixed with partial update.");
            }
            else
            { 
                SearchTemporaryNode.Attributes["privilege"].Value = SearchAllTemporaryNode.Attributes["privilege"].Value = "SEARCH_TEMP";
                messages.Add("CSBR132598 was successfully fixed.");
            }
            #endregion 

            return messages;
            #endregion
        }
    }
}

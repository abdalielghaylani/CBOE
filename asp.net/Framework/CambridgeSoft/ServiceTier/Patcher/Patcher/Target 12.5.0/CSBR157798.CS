using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding ADD_COMPONENT privilege for submitmixture links in home page.
    /// </summary>
    public class CSBR157798 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string panelContentPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/applicationHome/groups/add[@name='RegistrationPanelContents']";
            string linksContentPath = "//coeHomeSettings/groups/add[@name='Registration']";


            #region Validate
            XmlNode panelContentNode = frameworkConfig.SelectSingleNode(panelContentPath + "/links/add[@name='SubmitMixture']");
            XmlNode linksContentNode = frameworkConfig.SelectSingleNode(linksContentPath + "/links/add[@name='SubmitMixture']");


            if (panelContentNode == null || linksContentNode == null)
            {
                errorsInPatch = true;
                messages.Add("Links are not available to update privilege.");
            }
            else if (panelContentNode.Attributes["privilege"] == null || linksContentNode.Attributes["privilege"] == null)
            {
                errorsInPatch = true;
                messages.Add("privilege attribute is not available to update.");
            }
           
            #endregion

            #region Update
            if (errorsInPatch)
            {
                messages.Add("CSBR157798 was fixed with partial update.");
            }
            else
            {
                panelContentNode.Attributes["privilege"].Value = "ADD_COMPONENT";
                linksContentNode.Attributes["privilege"].Value = "HIDEME";// Upgrade server failing to show Submit Mixture link oh Registration Home page.CSBR -158516.
                messages.Add("CSBR157798 was successfully fixed.");
            }
            #endregion

            return messages;
            #endregion
        }
    }
}

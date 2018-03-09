using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding SEARCH_TEMP privilege to search temporary and review register links for system information
    /// </summary>
    public class CSBR158516_1252 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig

            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            #region Validate
            XmlNode SubmitMixtureNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='COE']/links/add[@name='SubmitMixture']");


            if (SubmitMixtureNode == null || SubmitMixtureNode.Attributes["privilege"] == null)
            {
                errorsInPatch = true;
            }

            #endregion

            #region Update
            if (errorsInPatch)
            {
                messages.Add("CSBR158516 had  errors.");
            }
            else
            {
                SubmitMixtureNode.Attributes["privilege"].Value = "HIDEME";
                messages.Add("CSBR158516 was successfully fixed.");
            }
            #endregion

            return messages;
            #endregion
        }
    }
}

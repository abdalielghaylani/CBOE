using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Ading HIDEME privilege to SubmitMixture link 
    /// </summary>
    public class CSBR158516 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig

            List<string> messages = new List<string>();

            #region Validate
            XmlNode SubmitMixtureNode = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='Registration']/links/add[@name='SubmitMixture']");


            if (SubmitMixtureNode != null)
            {
                if (SubmitMixtureNode.Attributes["privilege"] == null)
                    createNewAttribute("privilege", "HIDEME", ref  SubmitMixtureNode);
                SubmitMixtureNode.Attributes["privilege"].Value = "HIDEME";
                 messages.Add("CSBR158516 was successfully fixed.");
            }
            else
            {
                messages.Add("SubmitMixture link is not available in framework config to update the privilege attribute.");
            }
            #endregion
            
            #endregion

            return messages;
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }
    }
}

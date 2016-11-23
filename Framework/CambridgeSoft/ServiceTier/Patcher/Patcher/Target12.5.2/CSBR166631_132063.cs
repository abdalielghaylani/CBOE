using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	/// <summary>
	/// This patcher class is to update the privileges for sequence table editor entries on coeframeworkconfig and 
    /// also update isUnique fileds for some of the table editor details.
	/// </summary>
    class CSBR166631_132063 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region frameworkconfig fix

            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            #region Upate sequence table editor

            string sequenceTableLink = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']";
            XmlNode sequenceTablekNode = frameworkConfig.SelectSingleNode(sequenceTableLink);

            if (sequenceTablekNode == null)
            {
                errorsInPatch = true;
                messages.Add("Sequence table editor node is not available to update.");
            }
            else
            {
                if (sequenceTablekNode.Attributes["editPriv"] == null)
                {
                    createNewAttribute("editPriv", "EDIT_SEQUENCES_TABLE", ref sequenceTablekNode);
                    messages.Add("editPriv attribute is added  in Sequence .");
                }
                if (sequenceTablekNode.Attributes["addPriv"] == null)
                {
                    createNewAttribute("addPriv", "ADD_SEQUENCES_TABLE", ref sequenceTablekNode);
                    messages.Add("addPriv attribute is added  in Sequence .");
                }
                if (sequenceTablekNode.Attributes["deletePriv"] == null)
                {
                    createNewAttribute("deletePriv", "DELETE_SEQUENCES_TABLE", ref sequenceTablekNode);
                    messages.Add("deletePriv attribute is added  in Sequence .");
                }
                
            }

            #endregion

            #region Update IsUnique flag

            string tableEditorPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/";

            addUniquNewAttribute(tableEditorPath + "add[@name='VW_PROJECT']/tableEditorData/add[@name='NAME']", ref frameworkConfig);

            addUniquNewAttribute(tableEditorPath + "add[@name='VW_NOTEBOOKS']/tableEditorData/add[@name='NAME']", ref frameworkConfig);

            addUniquNewAttribute(tableEditorPath + "add[@name='VW_FRAGMENTTYPE']/tableEditorData/add[@name='DESCRIPTION']", ref frameworkConfig);

            addUniquNewAttribute(tableEditorPath + "add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIX']", ref frameworkConfig);

            addUniquNewAttribute(tableEditorPath + "add[@name='VW_IDENTIFIERTYPE']/tableEditorData/add[@name='NAME']", ref frameworkConfig);

            #endregion

            #endregion

            #region result

            if (errorsInPatch)
            {
                messages.Add("Table editor updated with with errors.");
            }
            else
            {
                messages.Add("Table editor updated with isUnique key successfully.");
            }

            #endregion

            return messages;
        }

        #region Private Method

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        private void addUniquNewAttribute(string xpathTocheck, ref XmlDocument frameworkConfigDoc)
        {
            XmlNode nodeToUpdate = frameworkConfigDoc.SelectSingleNode(xpathTocheck);

            if (nodeToUpdate != null)
            {
                createNewAttribute("isUnique", "true", ref nodeToUpdate);
            }
        }


        #endregion
	}
}

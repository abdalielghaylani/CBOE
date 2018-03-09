using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adding ADD_COMPONENT privilege for submitmixture links in home page.
    /// </summary>
    public class CSBR133966 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {

            #region Frameworkconfig

            List<string> messages = new List<string>(); 
            bool errorsInPatch = false;
            string viewPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']";
            string parentPath = viewPath + "/tableEditorData";
            string columnToAdd = "AUTOSELCOMPDUPCHK";
            string columnToAddAfter = "TYPE";


            #region Validate

            XmlNode viewContentNode = frameworkConfig.SelectSingleNode(viewPath);
            XmlNode parentContentNode = frameworkConfig.SelectSingleNode(parentPath);
            XmlNode columnContentNode = frameworkConfig.SelectSingleNode(parentPath + "/add[@name='AUTOSELCOMPDUPCHK']");


            if (viewContentNode == null || parentContentNode == null)
            {
                errorsInPatch = true;
                messages.Add("Parent table editor is not available to perform the update");
            }
            else if (columnContentNode != null)
            {
                errorsInPatch = true;
                messages.Add(columnToAdd + "is already available in VW_Sequence.");
            }

            #endregion

            #region Update
            if (errorsInPatch)
            {
                messages.Add("NewClassName was fixed with partial update.");
            }
            else
            {
                #region PROTOTYPE
                //  <add name="AUTOSELCOMPDUPCHK" dataType="string" lookupLocation="innerXml_ACTIVECASES" defaultValue="F" alias="Automatically select first component for mixture duplicate checking?">
                //<validationRule>
                //  <add name="requiredField" errorMessage="This field is required"/>
                //</validationRule>
              //</add>
                #endregion

                #region ADD
                XmlNode InsertAfterNode = parentContentNode.SelectSingleNode("add[@name='" + columnToAddAfter + "']");
                columnContentNode = parentContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "add", null);

                createNewAttribute("name", columnToAdd, ref columnContentNode);
                createNewAttribute("dataType", "string", ref columnContentNode);
                createNewAttribute("lookupLocation", "innerXml_ACTIVECASES", ref columnContentNode);
                createNewAttribute("defaultValue", "F", ref columnContentNode);
                createNewAttribute("alias", "Automatically select first component for mixture duplicate checking?", ref columnContentNode);

                XmlNode validationRule = columnContentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", null);
                validationRule.InnerXml = @"<add name=""requiredField"" errorMessage=""This field is required""/>";
                columnContentNode.AppendChild(validationRule);

                #endregion
                

                if (InsertAfterNode != null)
                {
                    parentContentNode.InsertAfter(columnContentNode, InsertAfterNode);
                }
                else
                {
                    parentContentNode.AppendChild(columnContentNode);
                }
                messages.Add(columnToAdd + " node was added succesfully.");

            }
            #endregion

            return messages;
            #endregion
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }
    }
}

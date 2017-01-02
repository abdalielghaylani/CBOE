using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Data;


namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// New Feature request for PICKLISTDOMAIN.
    /// Chages to formBO's, Frameworkconfig.xml
    /// </summary>
    public class SqlFilter_SqlSortOrder_FrameworkConfig : BugFixBaseCommand
    {


        List<string> _messages = new List<string>();
        bool _errorsInPatch = false;
        Dictionary<string, string> PicklistDomain = new Dictionary<string, string>();

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {

           
            #region Framework change
            UpdateFrameworkConfig(frameworkConfig);
            #endregion

            if (!_errorsInPatch)
                _messages.Add("PicklistFeatureRequest was successfully patched");
            else
                _messages.Add("PicklistFeatureRequest was patched with errors");
            return _messages;
        }// Method

        private void UpdateFrameworkConfig(XmlDocument frameworkConfig)
        {
            string viewPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='{0}']";
            string parentPath = viewPath + "/childTable";
            string columnToAddPath = parentPath + "/add[@name='{1}']";
            string currentView = string.Empty;
            string columnToEdit = string.Empty;

            XmlNode viewContentNode;
            XmlNode parentContentNode;
            XmlNode columnContentNode;

            #region VW_PROJECT -->  COEDB.PEOPLE
            currentView = "VW_PROJECT";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {
                columnToEdit = "COEDB.PEOPLE";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToEdit));
                if (columnContentNode != null)
                {
                    #region sqlFilter
                    if (columnContentNode.Attributes["sqlFilter"] == null)
                    {
                        createNewAttribute("sqlFilter", "PEOPLE.ACTIVE = 1", ref columnContentNode);
                    }
                    columnContentNode.Attributes["sqlFilter"].Value = "PEOPLE.ACTIVE = 1";
                    #endregion

                    #region sqlSortOrder
                    if (columnContentNode.Attributes["sqlSortOrder"] == null)
                    {
                        createNewAttribute("sqlSortOrder", "PEOPLE.USER_ID ASC", ref columnContentNode);
                    }
                    columnContentNode.Attributes["sqlSortOrder"].Value = "PEOPLE.USER_ID ASC";
                    #endregion
                }
            }
            else
            {
                _errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion

            #region VW_SEQUENCE -->  COEDB.PEOPLE
            currentView = "VW_SEQUENCE";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {
                columnToEdit = "COEDB.PEOPLE";
                columnContentNode = frameworkConfig.SelectSingleNode(string.Format(columnToAddPath, currentView, columnToEdit));
                if (columnContentNode != null)
                {
                    #region sqlFilter
                    if (columnContentNode.Attributes["sqlFilter"] == null)
                    {
                        createNewAttribute("sqlFilter", "PEOPLE.ACTIVE = 1", ref columnContentNode);
                    }
                    columnContentNode.Attributes["sqlFilter"].Value = "PEOPLE.ACTIVE = 1";
                    #endregion

                    #region sqlSortOrder
                    if (columnContentNode.Attributes["sqlSortOrder"] == null)
                    {
                        createNewAttribute("sqlSortOrder", "PEOPLE.USER_ID ASC", ref columnContentNode);
                    }
                    columnContentNode.Attributes["sqlSortOrder"].Value = "PEOPLE.USER_ID ASC";
                    #endregion
                }
            }
            else
            {
                _errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }

            #endregion
        }
              
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

    }// Class

}// Name Space


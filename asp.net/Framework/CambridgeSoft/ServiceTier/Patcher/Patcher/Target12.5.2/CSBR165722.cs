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
    /// Chages for Frameworkconfig.xml to remove childtable for sequence tableeditor
    /// </summary>
    public class CSBR165722 : BugFixBaseCommand
    {


        List<string> _messages = new List<string>();
        Dictionary<string, string> PicklistDomain = new Dictionary<string, string>();

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {

            bool errorsInPatch = false;
            #region Framework change
            UpdateFrameworkConfig(frameworkConfig);
            #endregion

            if (!errorsInPatch)
                _messages.Add("CSBR-165722 was successfully patched");
            else
                _messages.Add("CSBR-165722 was patched with errors");
            return _messages;
        }// Method
        
        private void UpdateFrameworkConfig(XmlDocument frameworkConfig)
        {
            bool errorsInPatch = false;
            string viewPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='{0}']";
            string parentPath = viewPath + "/childTable";
            string currentView = string.Empty;

            XmlNode viewContentNode;
            XmlNode parentContentNode;

            #region VW_SEQUENCE
            currentView = "VW_SEQUENCE";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));
            if (viewContentNode != null)
            {

                #region Delete  node childTable
                if (parentContentNode != null)
                {
                    #region DELETE
                    viewContentNode.RemoveChild(parentContentNode);
                    #endregion
                    _messages.Add("childTable [List of users available in sequences] node was deleted succesfully.");
                    errorsInPatch = false;
                }
                else
                {
                    errorsInPatch = true;
                    _messages.Add("childTable [List of users available in sequences] is already deleted in " + currentView + " .");
                }
                #endregion
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the delete operation");
            }

            #endregion
        }

    }// Class

}// Name Space


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
    /// To remove delete option for identifiers picklist.
    /// </summary>
    public class CSBR167678 : BugFixBaseCommand
    {

        List<string> _messages = new List<string>();

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            bool errorsInPatch = false;
                     
            #region Framework change
            errorsInPatch = UpdateFrameworkConfig(frameworkConfig);
            #endregion

            if (!errorsInPatch)
                _messages.Add("CSBR-167678 was successfully patched");
            else
                _messages.Add("CSBR-167678 was patched with errors");
            return _messages;
        }
        
        private bool UpdateFrameworkConfig(XmlDocument frameworkConfig)
        {
            bool errorsInPatch = false;
            string viewPath = "//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='{0}']";
            string parentPath = viewPath + "/tableEditorData";
            string columnToAddPath = parentPath + "/add[@name='{1}']";
            string columnToAddAfterPath = parentPath + "/add[@name='{1}']";
            string currentView = string.Empty;
            XmlNode viewContentNode;
            XmlNode parentContentNode;

            #region VW_IDENTIFIERTYPE
            currentView = "VW_IDENTIFIERTYPE";
            viewContentNode = frameworkConfig.SelectSingleNode(string.Format(viewPath, currentView));
            parentContentNode = frameworkConfig.SelectSingleNode(string.Format(parentPath, currentView));

            if (viewContentNode != null)
            {
                #region Update delete privilege
                if (viewContentNode.Attributes["deletePriv"] != null)
                {
                    viewContentNode.Attributes["deletePriv"].Value = "HIDEME";
                }
                #endregion
                errorsInPatch = false;
            }
            else
            {
                errorsInPatch = true;
                _messages.Add("Parent table editor " + currentView + " is not available to perform the update");
            }
            #endregion

            return errorsInPatch;
        }

    }// Class

}// Name Space


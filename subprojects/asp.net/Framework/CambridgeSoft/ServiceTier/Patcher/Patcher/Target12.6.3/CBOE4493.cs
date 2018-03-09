using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE4493 : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeUpdateInPath = string.Empty;
            _coeUpdateInPath = "MultiCompoundRegistryRecord/AddIns/AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn']/AddInConfiguration/ScriptFile"; // Path to check the Rootnode before patcher update.
            string updatePath = "C:\\Program Files (x86)\\PerkinElmer\\ChemOfficeEnterprise\\Registration\\PythonScripts\\parentscript.py";
            XmlNode rootNode;

            #region AddIns
            rootNode = objectConfig.SelectSingleNode(_coeUpdateInPath);
            if (rootNode != null && rootNode.InnerText.Equals(updatePath))
            {
                messages.Add("Patch is already applied");
            }
            else if (rootNode != null && !rootNode.InnerText.Equals(updatePath))
            {
                rootNode.InnerText = updatePath;
            }
            else
                errorsInPatch = true;
            #endregion
            if (!errorsInPatch)
            {
                messages.Add("COEObjectConfig was updated succesfully");
            }
            else
                messages.Add("COEObjectConfig  was patched with errors");

            return messages;
        }        
	}
}

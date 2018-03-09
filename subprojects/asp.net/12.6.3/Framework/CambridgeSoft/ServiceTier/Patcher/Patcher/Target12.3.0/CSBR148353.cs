using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-148353: Can't export from Registration because of sequencenumber in dataview
    /// </summary>
	public class CSBR148353 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix
        /// 
        /// a) Edit dataview 4003.
        ///     - Look for ROOTNUMBER field under VW_REGISTRYNUMBER table and remove it
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4003"))
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    
                    XmlNode regNumberTbl = currentDataview.SelectSingleNode("//COE:table[@name='VW_REGISTRYNUMBER']", manager);
                    if (regNumberTbl == null)
                    {
                        errorInPatch = true;
                        messages.Add("VW_MIXTURE_REGNUMBER was not found on dataview 4003");
                    }
                    else
                    {
                        XmlNode rootNumFld = regNumberTbl.SelectSingleNode(".//COE:fields[@name='ROOTNUMBER']", manager);
                        if (rootNumFld == null)
                            messages.Add("The field ROOTNUMBER was already removed from dataview 4003, no action performed");
                        else
                        {
                            regNumberTbl.RemoveChild(rootNumFld);
                            messages.Add("The field ROOTNUMBER was successfully removed from dataview 4003");
                        }
                    }
                }
            }

            if (!errorInPatch)
                messages.Add("CSBR148353 was successfully fixed.");
            else
                messages.Add("CSBR148353 was fixed with errors.");

            return messages;
        }
    }
}

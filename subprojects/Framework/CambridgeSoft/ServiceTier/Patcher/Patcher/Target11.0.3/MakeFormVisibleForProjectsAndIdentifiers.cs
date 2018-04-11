using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// In submit form the coeform="4" was being hidden. That form contained the identifiers and projects grids for batch level.
    /// </summary>
	public class MakeFormVisibleForProjectsAndIdentifiers : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Goto customize registration and edit form 4010 (submit).
        /// Look for the form 4 and make it visible by changing &lt;visible&gt;false&lt;/visible&gt; to &lt;visible&gt;true&lt;/visible&gt; under formDisplay node
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
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                if (id == "4010")
                {
                    XmlNode visible = doc.SelectSingleNode("//COE:coeForm[@id='4']/COE:formDisplay/COE:visible", manager);
                    if (visible != null && visible.InnerText.ToLower() == "false")
                    {
                        visible.InnerText = "true";
                        messages.Add("Visible attributte successfully changed");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Visible attribute did not have the expected value and was not changed");
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("MakeFormVisibleForProjectsAndIdentifiers was successfully patched");
            }
            else
                messages.Add("MakeFormVisibleForProjectsAndIdentifiers was patched with errors");

            return messages;
        }
    }
}

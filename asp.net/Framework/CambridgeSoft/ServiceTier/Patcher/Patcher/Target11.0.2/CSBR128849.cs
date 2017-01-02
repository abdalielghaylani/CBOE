using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-128849:
    /// The Mol Weight of Component shows as "Invalid mask component" for a Registered record:
    /// </summary>
    public class CSBR128849 : BugFixBaseCommand
    {
        /// <summary>
        /// These were the manual steps to fix:
        /// On form 4014:
        /// 1.	Search for the string “<Mask>{0:#.###}</Mask>”.
        /// 2.	Replace the previous string with:
        /// <Mask>#.###</Mask>
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
            foreach(XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if(id == "4014")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode mask = doc.SelectSingleNode("//COE:Mask[text()='{0:#.###}']", manager);
                    if(mask != null)
                    {
                        mask.InnerText = "#.###";
                        messages.Add("Mask was successfully updated");
                    }
                    else
                    {
                        messages.Add("The expected mask {0:#.###} was not found");
                        errorsInPatch = true;
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR128849 was successfully patched");
            else
                messages.Add("CSBR128849 was patched with errors");
            return messages;
        }
    }
}

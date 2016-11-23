using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR – 128177:
    /// Identifiers set at registry level are used at compound level too:
    /// (The issue is caused by some formElements containing redundant <DataSource> and <DataSourceID> tags in their configuration.  The fix  is to delete the <DataSourceID> tag where a <DataSource> tag is already in use.)
    /// </summary>
    public class CSBR128177 : BugFixBaseCommand
    {
        /// <summary>
        /// These were the manual steps to fix:
        /// In FormGroup 4010:
        /// 1.	Search in the xml text for: &lt;DataSource&gt;SELECT
        /// 2.	If the following line is:
        /// &lt;DataSourceID&gt;IdentifiersCslaDataSource&lt;/DataSourceID&gt;
        /// Then delete that line.  This situation should occur three times within that formgroup xml.
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
            int nodesCommented = 0;
            foreach(XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if(id == "4010")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNodeList fieldConfigList = doc.SelectNodes("//COE:DataSource[string-length(text())>0]/..", manager);
                    if (fieldConfigList.Count == 0)
                    {
                        messages.Add("There was no datasource specified in any formElement of FormGroup 4010.");
                        errorsInPatch = true;
                    }
                    else
                    {
                        foreach (XmlNode fieldConfig in fieldConfigList)
                        {
                            XmlNode datasourceId = fieldConfig.SelectSingleNode("./COE:DataSourceID", manager);
                            if (datasourceId != null)
                            {
                                fieldConfig.ReplaceChild(doc.CreateComment(datasourceId.OuterXml), datasourceId);
                                messages.Add("DataSourceID node was commented out to honor existing DataSource node");
                                nodesCommented++;
                            }
                        }
                        if (nodesCommented == 0)
                        {
                            errorsInPatch = true;
                            messages.Add("There were no formElement with BOTH DataSource AND DataSourceID nodes present. No change required");
                        }
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR128177 was successfully patched");
            else
                messages.Add("CSBR128177 was patched with errors");
            return messages;
        }
    }
}

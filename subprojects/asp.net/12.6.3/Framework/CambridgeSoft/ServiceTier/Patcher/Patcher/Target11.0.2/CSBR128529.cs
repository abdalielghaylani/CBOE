using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-128529:
    /// Identifier fields are missing in the data loader while mapping the output fields.
    /// </summary>
    public class CSBR128529 : BugFixBaseCommand
    {
        /// <summary>
        /// These were the manual steps to fix:
        /// On FormGroup 4015:
        /// 1.	Search in the xml text for: <DataSource>SELECT
        /// 2.	If the following line is:
        /// <DataSourceID>IdentifiersCslaDataSource</DataSourceID>
        /// 3.	Then delete that line.  This situation should occur three times within that formgroup xml.
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
                if(id == "4015")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNodeList fieldConfigList = doc.SelectNodes("//COE:DataSource[string-length(text())>0]/..", manager);
                    if (fieldConfigList.Count == 0)
                    {
                        messages.Add("There was no datasource specified in any formElement of FormGroup 4015.");
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
                messages.Add("CSBR128529 was successfully patched");
            else
                messages.Add("CSBR128529 was patched with errors");
            return messages;
        }
    }
}

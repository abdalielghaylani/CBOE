using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR – 128322:
    /// 
    /// Error appears in the Custom property field while registering duplicates from Submit record page.
    /// </summary>
    public class CSBR128322 : BugFixBaseCommand
    {
        /// <summary>
        /// These were the manual steps to fix:
        /// In FormGroup 4013:
        /// 1.	Search for coeForm id="1001"
        /// 2.	Replace  the current dataSourceId value ("DuplicatesCslaDataSource") with "DuplicateCompoundsCslaDataSource"
        /// 3.	Search for coeForm id="1002"
        /// 4.	Replace  the current dataSourceId value ("DuplicatesCslaDataSource") with "DuplicateBatchesCslaDataSource"
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
                if(id == "4013")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode coeForm1001 = doc.SelectSingleNode("//COE:coeForm[@id='1001']", manager);
                    if(coeForm1001.Attributes["dataSourceId"] != null && coeForm1001.Attributes["dataSourceId"].Value == "DuplicatesCslaDataSource")
                    {
                        coeForm1001.Attributes["dataSourceId"].Value = "DuplicateCompoundsCslaDataSource";
                        messages.Add("DatasourceId of the form 1001 was successfully changed to DuplicateCompoundsCslaDataSource");
                    }
                    else
                    {
                        messages.Add("DataSourceId in form 1001 was not the one expected and thus was not modified");
                        errorsInPatch = true;
                    }

                    XmlNode coeForm1002 = doc.SelectSingleNode("//COE:coeForm[@id='1002']", manager);
                    if(coeForm1002.Attributes["dataSourceId"] != null && coeForm1002.Attributes["dataSourceId"].Value == "DuplicatesCslaDataSource")
                    {
                        coeForm1002.Attributes["dataSourceId"].Value = "DuplicateBatchesCslaDataSource";
                        messages.Add("DatasourceId of the form 1002 was successfully changed to DuplicateBatchesCslaDataSource");
                    }
                    else
                    {
                        messages.Add("DataSourceId in form 1002 was not the one expected and thus was not modified");
                        errorsInPatch = true;
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR128322 was successfully patched");
            else
                messages.Add("CSBR128322 was patched with errors");
            return messages;
        }
    }
}

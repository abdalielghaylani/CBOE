using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR – 127570:
    /// 
    /// Error appears in the Custom property field while registering duplicates from Submit record page
    /// </summary>
    public class CSBR127570 : BugFixBaseCommand
    {
        /// <summary>
        /// This were the manual steps to fix:
        /// In FormGroup 4013:
        /// 1.	Search for coeForm id="1000"
        /// 2.	Replace  the current dataSourceId value ("DuplicatesCslaDataSource") with “CurrentDuplicateCslaDataSource”
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
                    XmlNode coeForm = doc.SelectSingleNode("//COE:coeForm[@id='1000']", manager);
                    if(coeForm == null)
                    {
                        messages.Add("There was no Form with ID 1000 for FormGroup 4013");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlAttribute datasourceid = coeForm.Attributes["dataSourceId"];
                        if(datasourceid == null || datasourceid.Value != "DuplicatesCslaDataSource")
                        {
                            messages.Add("Form with id 1000 did not have the expected datasourceid and was not changed.");
                            errorsInPatch = true;
                        }
                        else
                        {
                            datasourceid.Value = "CurrentDuplicateCslaDataSource";
                            messages.Add("DatasourceId was successfullu changed");
                        }
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR127570 was successfully patched");
            else
                messages.Add("CSBR127570 was patched with errors");
            return messages;
        }
    }
}

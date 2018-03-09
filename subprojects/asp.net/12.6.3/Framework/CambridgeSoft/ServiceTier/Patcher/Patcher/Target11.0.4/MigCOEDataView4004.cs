using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEDataView 4004
    /// </summary>
	class MigCOEDataView4004:BugFixBaseCommand
	{
        /// <summary>
        /// 1. add application attribute for dataview
        /// 2. correct a relationship with wrong child table
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument dataview in dataviews)
            {
                string dataviewid = dataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataview.DocumentElement.Attributes["dataviewid"].Value;
                if (dataviewid == "4004")
                {
                    string originalDataview = dataview.OuterXml;
                    try
                    {
                        XmlNamespaceManager manager = new XmlNamespaceManager(dataview.NameTable);
                        string xmlns = "COE.COEDataView";
                        string prefix = "COE";
                        manager.AddNamespace(prefix, xmlns);

                        //1. add application attribute for dataview
                        XmlAttribute applicaton = dataview.DocumentElement.Attributes["application"];
                        if (applicaton == null)
                        {
                            applicaton = dataview.CreateAttribute("application");
                            applicaton.Value = "REGISTRATION";
                            dataview.DocumentElement.Attributes.Append(applicaton);
                            messages.Add("Add application setting");
                        }
                        else
                            messages.Add("application already set");

                        //2. correct a relationship between VW_LOG_BULKREGISTRATION and VW_TEMPORARYCOMPOUND, incorrect child talbe id
                        XmlNode VLB_VT_Relationship = dataview.SelectSingleNode("/COE:COEDataView/COE:relationships/COE:relationship[@parent='209' and @child='1' and @parentkey='212' and @childkey='201']", manager);
                        if (VLB_VT_Relationship != null)
                        {
                            VLB_VT_Relationship.Attributes["child"].Value = "2";
                            messages.Add("correct a relationship between VW_LOG_BULKREGISTRATION and VW_TEMPORARYCOMPOUND, incorrect child talbe id");
                        }
                        else
                            messages.Add("Relationship between VW_LOG_BULKREGISTRATION and VW_TEMPORARYCOMPOUND already corrected");

                    }
                    catch (Exception e)
                    {
                        errorsInPatch = true;
                        messages.Add("Exception occurs : " + e.Message);
                    }

                    if (errorsInPatch)
                        dataview.LoadXml(originalDataview);
                }
            }

            if (errorsInPatch)
                messages.Add("Fail to patch COEDataView 4004");
            else
                messages.Add("Succeed to patch COEDataView 4004");

            return messages;
        }
	}
}

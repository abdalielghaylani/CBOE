using System;
using System.Collections.Generic;

using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEDataView 4002
    /// </summary>
	public class MigCOEDataView4002 : BugFixBaseCommand
	{
        /// <summary>
        /// Add application attribute to the dataview
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
                string dataviewId = dataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataview.DocumentElement.Attributes["dataviewid"].Value;
                if (dataviewId == "4002")
                {
                    string originalDataView = dataview.OuterXml;
                    try
                    {
                        if (dataview.DocumentElement.Attributes["application"] == null)
                        {
                            XmlAttribute application = dataview.CreateAttribute("application");
                            application.Value = "REGISTRATION";
                            dataview.DocumentElement.Attributes.Append(application);
                            messages.Add("Add applicatoin setting");
                        }
                        else
                            messages.Add("application already set");
                    }
                    catch (Exception e)
                    {
                        errorsInPatch = true;
                        messages.Add("Exception occurs :" + e.Message);
                    }

                    if (errorsInPatch)
                        dataview.LoadXml(originalDataView);
                }
            }

            if (errorsInPatch)
                messages.Add("Fail to patch COEDataView 4002");
            else
                messages.Add("Succeed to patch COEDataView 4002");

            return messages;
        }
    }
}

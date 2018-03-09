using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Patcher.Utilities;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// 1. Add application="REGISTRATION" attribute in root COEDataView node.
    /// </summary>
	public class MigCOEDataView4019 : BugFixBaseCommand
	{
        List<string> messages = new List<string>();
        bool errorsInPatch = false;

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            XmlDocument coeDataView4019 = PatcherUtility.GetCOEDataViewById(dataviews, "4019");

            if (coeDataView4019 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find COEDataView 4019");
            }
            else
            {
                if (coeDataView4019.DocumentElement.Attributes["application"] == null)
                {
                    XmlAttribute application = coeDataView4019.CreateAttribute("application");
                    application.Value = "REGISTRATION";
                    coeDataView4019.DocumentElement.Attributes.Append(application);
                    messages.Add("SUCCEED: Add applicatoin setting");
                }
                else
                    messages.Add("WARNING: application already set");
            }

            return messages;
        }
    }
}

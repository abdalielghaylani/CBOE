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
	public class MigCOEDataView4016 : BugFixBaseCommand
	{
        List<string> messages = new List<string>();
        bool errorsInPatch = false;

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            XmlDocument coeDataView4016 = PatcherUtility.GetCOEDataViewById(dataviews, "4016");

            if (coeDataView4016 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find COEDataView 4016");
            }
            else
            {
                if (coeDataView4016.DocumentElement.Attributes["application"] == null)
                {
                    XmlAttribute application = coeDataView4016.CreateAttribute("application");
                    application.Value = "REGISTRATION";
                    coeDataView4016.DocumentElement.Attributes.Append(application);
                    messages.Add("SUCCEED: Add applicatoin setting");
                }
                else
                    messages.Add("WARNING: application already set");
            }

            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-134552: "2IS027 - Date format should be changed to be understandable by French people"
    /// 
    /// Actual Behaviour: The dates configured in date picker are formatted as the CultureInfo specified, Whether in long or short format, but
    /// there is no way to override that format.
    /// 
    /// Expected Result: There should be a way to configure the desired date format. There should be a generic config for applications and a specific
    /// configuration for datepickers. So it is simple to configure them all at once, but still can be override ir required.
    /// </summary>
	public class CSBR134552 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// - Remove the following config from all the 40xx forms: <DateFormat>Short</DateFormat>
        /// - Add the following attribute to ApplicationDefaults node: dateFormat=""
        /// 
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
            
            string formList = "4002, 4003, 4005, 4006, 4010, 4011, 4012, 4014, 4015, 4019"; //Default configuration delivers these forms with DateFormat as Short
            
            foreach (XmlDocument doc in forms)
            {
                if (doc.InnerXml.Contains("<DateFormat>Short</DateFormat>"))
                {
                    doc.InnerXml = doc.InnerXml.Replace("<DateFormat>Short</DateFormat>", string.Empty);
                }
                else if (doc.InnerXml.Contains("<COE:DateFormat>Short</COE:DateFormat>"))
                {
                    doc.InnerXml = doc.InnerXml.Replace("<COE:DateFormat>Short</COE:DateFormat>", string.Empty);
                }
                else
                {
                    string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                    if (formList.Contains(id))
                    {
                        errorsInPatch = true;
                        messages.Add("FormID=" + id + " did not have the expected DateFormat and was not removed");
                    }
                }

            }
            if (!errorsInPatch)
                messages.Add("Successfully removed Short DateFormat config from all FormElements");

            XmlNode appDefaults = frameworkConfig.SelectSingleNode("//applicationDefaults");
            if (appDefaults.Attributes["dateFormat"] == null)
            {
                appDefaults.Attributes.Append(frameworkConfig.CreateAttribute("dateFormat")).Value = string.Empty;
                messages.Add("Successfully added dateFormat attribute to FrameworkConfig.xml file");
            }
            else
                messages.Add("FrameworkConfig.xml file already contains a dateFormat attribute");

            if (!errorsInPatch)
            {
                messages.Add("CSBR-134552 was successfully patched");
            }
            else
                messages.Add("CSBR-134552 was patched with errors");

            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-132305: Wrong version number shows on the title bar of Registration
    /// Bug:The version shown on the title bar, is 11.0.1
    /// Expect result : It should be version 11.0.3
    /// </summary>
	public class CSBR132305:BugFixBaseCommand
	{
        /// <summary>
        /// No manual fix provided
        /// </summary>
        /// <remarks>
        /// Update default page title of Registration, which includes the version number.
        /// This fix update the version number from 11.0.1 to 11.0.3
        /// </remarks>
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
            try
            {
                foreach (XmlDocument doc in configurations)
                {
                    if (doc.DocumentElement.Name.ToUpper() == "REGISTRATION")//configuration for Registration
                    {
                        XmlNode appPageTitle = doc.SelectSingleNode("//Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='AppPageTitle']");
                        if (appPageTitle != null && appPageTitle.Attributes["value"] != null)
                        {
                            string originalTitle = appPageTitle.Attributes["value"].Value;
                            appPageTitle.Attributes["value"].Value = originalTitle.Replace("11.0.1", "11.0.3");
                            messages.Add("Successfully update AppPageTitle (default page title) for Registration");
                        }
                        else
                        {
                            messages.Add("Can not find AppPageTitle (default page title) in Registration configuration");
                            errorsInPatch = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                messages.Add(e.Message);
                errorsInPatch = true;
            }
            if (!errorsInPatch)
                messages.Add("CSBR132305 was successfully patched");
            else
                messages.Add("CSBR132305 was patched with errors");
            return messages;
        }
	}
}

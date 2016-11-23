using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 131872: The values for Registry_Identifiers and Component_Identifiers are set to False in the Registration system settings
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application with reg privilliges 
    /// 2. Go to Customize registration 
    /// 3. Go to System settings 
    /// 4. Go to Advanced tab
    /// 5. View the Values of Registry_Identifiers and Component_Identifiers
    /// 
    /// Bug: The values for Registry_Identifiers and Component_Identifiers are set to False
    /// 
    /// Expected result: The values for Registry_Identifiers and Component_Identifiers should be  set to True by default.
    /// </summary>
	public class CSBR131872 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Go to customize registration, go to system settings and under advanced tab change the values for Registry_Identifiers and Component_Identifiers to True
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
            foreach (XmlDocument doc in configurations)
            {
                if (doc.DocumentElement.Name.ToLower() == "registration")
                {
                    XmlNode registryIdentifiers = doc.SelectSingleNode("/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='Registry_Identifiers']");

                    if (registryIdentifiers == null)
                    {
                        messages.Add("Registry_Identifiers configuration item under misc settings was not found");
                        errorsInPatch = true;
                    }
                    else
                    {
                        if (registryIdentifiers.Attributes["value"].Value == "True")
                        {
                            messages.Add("WARNING: Registry_Identifiers already have the proper value set");
                        }
                        else
                        {
                            registryIdentifiers.Attributes["value"].Value = "True";
                            messages.Add("Registry_Identifiers succesfully set to True");
                        }
                    }

                    XmlNode componentIdentifiers = doc.SelectSingleNode("/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='Component_Identifiers']");

                    if (componentIdentifiers == null)
                    {
                        messages.Add("Component_Identifiers configuration item under misc settings was not found");
                        errorsInPatch = true;
                    }
                    else
                    {
                        if (componentIdentifiers.Attributes["value"].Value == "True")
                        {
                            messages.Add("WARNING: Component_Identifiers already have the proper value set");
                        }
                        else
                        {
                            componentIdentifiers.Value = "True";
                            messages.Add("Component_Identifiers succesfully set to True");
                        }
                    }
                }
            }
            if (!errorsInPatch)
                messages.Add("CSBR131872 was successfully patched");
            else
                messages.Add("CSBR131872 was patched with errors");
            return messages;
        }
	}
}

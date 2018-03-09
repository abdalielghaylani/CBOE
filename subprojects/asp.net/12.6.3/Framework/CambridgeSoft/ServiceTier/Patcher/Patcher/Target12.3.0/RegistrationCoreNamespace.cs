using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// In 12.3 COERegistration projects were refactored. And thus the DAL for both Registration.Services and RegistrationAdmin.Services where
    /// included in Registration.Core.
    /// </summary>
	public class RegistrationCoreNamespace : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix.
        /// 
        /// Open COEFrameworkConfig.xml file and to to the services section.
        /// Find COERegistration and COERegistrationAdmin entries beneath it and configure the following two atttibutes as follows:
        /// 
        /// dalProviderAssemblyNameShort="Registration.Core"
        /// dalProviderAssemblyNameFull="Registration.Core, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc"
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
            bool errorInPatch = false;
            XmlNode coeRegistrationService = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/services/add[@name='COERegistration']");
            XmlNode coeRegistrationAdminService = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/services/add[@name='COERegistrationAdmin']");
            string targetFullAssembly = "Registration.Core, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc";
            string targetShortAssembly = "Registration.Core";
            if (coeRegistrationService != null)
            {
                if (coeRegistrationService.Attributes["dalProviderAssemblyNameShort"].Value != targetShortAssembly)
                {
                    coeRegistrationService.Attributes["dalProviderAssemblyNameShort"].Value = targetShortAssembly;
                    messages.Add("COERegistration service short assembly name updated succesfully");
                }
                else
                    messages.Add("COERegistration service short assembly name already has the right value");

                if (coeRegistrationService.Attributes["dalProviderAssemblyNameFull"].Value != targetFullAssembly)
                {
                    coeRegistrationService.Attributes["dalProviderAssemblyNameFull"].Value = targetFullAssembly;
                    messages.Add("COERegistration service full assembly name updated succesfully");
                }
                else
                    messages.Add("COERegistration service full assembly name already has the right value");
            }
            else
            {
                errorInPatch = true;
                messages.Add("No COERegistration service found on Framework config");
            }


            if (coeRegistrationAdminService != null)
            {
                if (coeRegistrationAdminService.Attributes["dalProviderAssemblyNameShort"].Value != targetShortAssembly)
                {
                    coeRegistrationAdminService.Attributes["dalProviderAssemblyNameShort"].Value = targetShortAssembly;
                    messages.Add("COERegistrationAdmin service short assembly name updated succesfully");
                }
                else
                    messages.Add("COERegistrationAdmin service short assembly name already has the right value");

                if (coeRegistrationAdminService.Attributes["dalProviderAssemblyNameFull"].Value != targetFullAssembly)
                {
                    coeRegistrationAdminService.Attributes["dalProviderAssemblyNameFull"].Value = targetFullAssembly;
                    messages.Add("COERegistrationAdmin service full assembly name updated succesfully");
                }
                else
                    messages.Add("COERegistrationAdmin service full assembly name already has the right value");
            }
            else
            {
                errorInPatch = true;
                messages.Add("No COERegistrationAdmin service found on Framework config");
            }

            if (!errorInPatch)
                messages.Add("RegistrationCoreNamespace succesfully patched");
            else
                messages.Add("RegistrationCoreNamespace patched with errors");

            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE8456:BugFixBaseCommand
	{

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode COERegistration = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/services/add[@name='COERegistration']");
            XmlNode COERegistrationAdmin = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/services/add[@name='COERegistrationAdmin']");

            if (COERegistration != null)
            {
                if (COERegistration.Attributes["dalProviderAssemblyNameFull"].Value == "Registration.Core, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc")
                {
                    COERegistration.Attributes["dalProviderAssemblyNameFull"].Value = "Registration.Core, Version=18.1.1.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc";
                    messages.Add("Registration.Core Assembly Version updated successfully for COERegistration Tag");
                }
                else
                {
                    messages.Add("Registration.Core Assembly Version is already updated for COERegistration Tag");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("COERegistration tag is not available in FramewrokConfig XML");
            }

            if (COERegistrationAdmin != null)
            {
                if (COERegistrationAdmin.Attributes["dalProviderAssemblyNameFull"].Value == "Registration.Core, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc")
                {
                    COERegistrationAdmin.Attributes["dalProviderAssemblyNameFull"].Value = "Registration.Core, Version=18.1.1.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc";
                    messages.Add("Registration.Core Assembly Version updated successfully for COERegistrationAdmin Tag");
                }
                else
                {
                    messages.Add("Registration.Core Assembly Version is already updated for COERegistrationAdmin Tag");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("COERegistrationAdmin tag is not available in FramewrokConfig XML");
            }


            if (!errorsInPatch && messages.Count != 0)
            {
                messages.Add("process succeed!");
            }
            else
            {
                messages.Add("failed to patch.");
            }

            return messages;
        }
	}
}

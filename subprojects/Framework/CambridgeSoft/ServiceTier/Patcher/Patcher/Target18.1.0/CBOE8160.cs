using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8160 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode databaseAttribute = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='ChemACX']/links/add[@name='DatabaseWizard']");

            if (databaseAttribute != null)
            {
                if (databaseAttribute.Attributes["display"].Value == "Database Wizard")
                {
                    databaseAttribute.RemoveAll();
                    messages.Add("Database Wizard tag removed successfully");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Database Wizard is not present");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Database Wizard node not available on ACX");
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


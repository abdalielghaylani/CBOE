using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8129 : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode privilegeAttribute = frameworkConfig.SelectSingleNode("//coeHomeSettings/groups/add[@name='ChemACX']/links/add[@name='Search']");

            if (privilegeAttribute != null)
            {
                if (string.IsNullOrEmpty(privilegeAttribute.Attributes["privilege"].Value))
                {
                    privilegeAttribute.Attributes["privilege"].Value = "BROWSE_ACX||BUY_ACX";
                    messages.Add("Privileges added for search tag");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Privileges already present for search");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Search node not available on ACX");
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


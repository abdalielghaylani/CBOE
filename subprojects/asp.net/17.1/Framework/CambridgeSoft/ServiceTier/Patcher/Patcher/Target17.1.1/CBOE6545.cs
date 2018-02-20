using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE6545:BugFixBaseCommand
	{

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
           
            # region Prefix delimiter Custom validation
            XmlNode prefixdelimAlias = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIXDELIMITER']");

            if (prefixdelimAlias != null)
            {
                if (prefixdelimAlias.Attributes["alias"] == null)
                {
                    XmlAttribute name = frameworkConfig.CreateAttribute("alias");
                    name.Value = "PREFIX DELIMITER";
                    prefixdelimAlias.Attributes.Append(name);
                    messages.Add("Alias added for Prefix delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Alias already present for Prefix delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Prefix delimiter node not available on sequence table editor");
            }

            #endregion

            # region suffix delimiter Custom validation
            XmlNode suffixdelimAlias = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='SUFFIXDELIMITER']");

            if (suffixdelimAlias != null)
            {
                if (suffixdelimAlias.Attributes["alias"] == null)
                {
                    XmlAttribute name = frameworkConfig.CreateAttribute("alias");
                    name.Value = "SUFFIX DELIMITER";
                    suffixdelimAlias.Attributes.Append(name);
                    messages.Add("Alias added for suffix delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Alias already present for suffix delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("suffix delimiter node not available on sequence table editor");
            }

            #endregion

            # region batch delimiter Custom validation
            XmlNode batchdelimAlias = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='BATCHDELIMITER']");

            if (batchdelimAlias != null)
            {
                if (batchdelimAlias.Attributes["alias"] == null)
                {
                    XmlAttribute name = frameworkConfig.CreateAttribute("alias");
                    name.Value = "BATCH DELIMITER";
                    batchdelimAlias.Attributes.Append(name);
                    messages.Add("Alias added for batch delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Alias already present for batch delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Batch Delimiter node not available on sequence table editor");
            }

            #endregion


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

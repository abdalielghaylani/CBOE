using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-139582: duplicate settings exist for 'MWROUNDDIGIT ' and 'AllowHitlistManagement '.
    /// NOTE: These exist because some previous developer was not careful enough about trimming
    /// the attribute value, and left one character of whitespace at the end, hence the duplication.
    /// </summary>
    class CSBR139582 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            try
            {
                XmlDocument regConfig = null;
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                    {
                        regConfig = config;
                        break;
                    }
                }

                if (regConfig != null)
                {
                    //'MWROUNDDIGIT ' and 'AllowHitlistmanagement ' settings removal
                    //NOTE the trailing space on each string as the cause of the apparent duplication
                    List<string> settingsToEliminate = 
                        new List<string>(new string[2] { "MWROUNDDIGIT ", "AllowHitlistManagement " });
                    foreach (string setting in settingsToEliminate)
                    {
                        XmlNodeList nodes = regConfig.SelectNodes(
                            string.Format("/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='{0}']", setting)
                            );
                        if (nodes != null)
                        {
                            if (nodes.Count > 1)
                            {
                                for (int i = 1;i<nodes.Count;i++)
                                {
                                    nodes[i].ParentNode.RemoveChild(nodes[i]);
                                }
                                messages.Add(string.Format("(Duplicated) '{0}' setting removed.", setting));
                            }
                        }
                        else
                            messages.Add(string.Format("'{0}' setting was not found.", setting));
                    }
                }
            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }

            return messages;
        }
	}
}

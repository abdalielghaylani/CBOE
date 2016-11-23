using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE3870 : BugFixBaseCommand
	{
        List<string> messages = new List<string>();
        bool errorsInPatch = false;
        // set the root path
        string viewpath = "//Registration/applicationSettings[@name='Reg App Settings']/groups";
       
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            // set the path to child node
            string nodetoRemove = viewpath + "/add[@name='DOCMGR']";
            foreach (XmlDocument config in configurations)
            {
                string name = config.DocumentElement.Name == null ? string.Empty : config.DocumentElement.Name;
                if(name == "Registration") 
                 {
                    XmlNode rootNode = config.SelectSingleNode(nodetoRemove);
                    if (rootNode == null)
                    {
                        messages.Add(" ERROR - DOCMGR Element not available || Already patched : ");
                        errorsInPatch = true;
                    }
                    else
                    {
                     rootNode.RemoveAll();                                          
                    }
                 }
                else if (name == string.Empty)
                {
                    messages.Add(" ERROR - Document name not available-- Unable to patch : ");
                    errorsInPatch = true;
                }
            }
            if (!errorsInPatch)
                messages.Add(" CBOE3870 was successfully patched ");
            else
                messages.Add(" CBOE3870 was not patched due to errors ");
            return messages; 
        }       
	}
}

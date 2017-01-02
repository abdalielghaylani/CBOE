using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-125232:
    /// 
    /// System Settings: The 'pipe' character is replaced by "%7c" on the edit mode of the value of the parameter CustomPropertyStyle on the Advanced tab
    /// </summary>
    public class CSBR125232 : BugFixBaseCommand
    {
        /// <summary>
        /// This were the manual steps to fix:
        /// 1.	Select “Advanced” tab.
        /// 2.	Locate the row with the name column equal to CustomPropertyStyle and set the value column to: Std25x40|Std50x40|Std50x80|Std75x40|Std100x40|Std100x80
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
            foreach(XmlDocument doc in configurations)
            {
                if(doc.DocumentElement.Name.ToLower() == "registration")
                {
                    XmlAttribute customPropertyStyles = doc.SelectSingleNode("/Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='CustomPropertyStyles']").Attributes["value"];
                    if(customPropertyStyles == null)
                    {
                        messages.Add("CustomPropertyStyles configuration item under misc settings was not found");
                        errorsInPatch = true;
                    }
                    else
                    {
                        if (customPropertyStyles.Value.Contains("Std25x40|Std50x40|Std50x80|Std75x40|Std100x40|Std100x80"))
                        {
                            errorsInPatch = true;
                            messages.Add("CustomPropertyStyles already have the proper value set");
                        }
                        else
                        {
                            customPropertyStyles.Value = "Std25x40|Std50x40|Std50x80|Std75x40|Std100x40|Std100x80";
                            messages.Add("Succesfully replaced CustomPropertyStyles");
                        }
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR125232 was successfully patched");
            else
                messages.Add("CSBR125232 was patched with errors");
            return messages;
        }
    }
}

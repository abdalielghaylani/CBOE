using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Prior to 12.3 SaveQueryHistory was defaulted to NO
    /// </summary>
	public class ChangeDefaultSaveQueryHistoryToYES : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open COEFrameworkConfig.xml file and look for SaveQueryHistory attribute, set its value to YES.
        /// Notice that there might be several entries for this attribute.
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
            XmlNodeList saveQueryHistoryXmlNodeList = frameworkConfig.SelectNodes("//*[@saveQueryHistory]");
            if (saveQueryHistoryXmlNodeList.Count == 0)
            {
                errorInPatch = true;
                messages.Add("SaveQueryHistory configuration item was not found");
            }
            foreach (XmlNode node in saveQueryHistoryXmlNodeList)
            {
                node.Attributes["saveQueryHistory"].Value = "YES";
            }


            XmlNode appDefaultsSaveQueryHistory = frameworkConfig.SelectSingleNode("//applicationDefaults/searchService/add[@name='saveQueryHistory']");
            if (appDefaultsSaveQueryHistory != null && appDefaultsSaveQueryHistory.Attributes["value"] != null && appDefaultsSaveQueryHistory.Attributes["value"].Value != "YES")
                appDefaultsSaveQueryHistory.Attributes["value"].Value = "YES";

            if (!errorInPatch)
                messages.Add("SaveQueryHistory default values have been succesfully updated");
            else
                messages.Add("SaveQueryHistory default values were not updated");

            return messages;
        }
    }
}

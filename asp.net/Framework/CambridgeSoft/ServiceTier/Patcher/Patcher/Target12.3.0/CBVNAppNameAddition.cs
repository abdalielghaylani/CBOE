using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Since 12.3 ChemBioViz.Net has its own config in coeframeworkconfig.xml
    /// </summary>
	public class CBVNAppNameAddition : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix.
        /// 
        /// Open COEFrameworkConfig.xml file and look for applications node.
        /// Add the following entry under the applications node:
        /// 
        /// &lt;add name="CBVN" database="SAMPLE"&gt;&lt;/add&gt;
        /// 
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

            XmlNode CBVNApplication = frameworkConfig.SelectSingleNode("//applications/add[@name='CBVN']");

            if (CBVNApplication != null)
            {
                errorInPatch = true;
                messages.Add("CBVN application was already present in config file");
            }
            else
            {
                //<add name="CBVN" database="SAMPLE"></add>
                CBVNApplication = frameworkConfig.CreateElement("add");
                CBVNApplication.Attributes.Append(frameworkConfig.CreateAttribute("name"));
                CBVNApplication.Attributes.Append(frameworkConfig.CreateAttribute("database"));
                CBVNApplication.Attributes["name"].Value = "CBVN";
                CBVNApplication.Attributes["database"].Value = "SAMPLE";
                frameworkConfig.SelectSingleNode("//applications").AppendChild(CBVNApplication);
                messages.Add("CBVN application added the the list of applications");
            }

            if (!errorInPatch)
                messages.Add("CBVN application was successfully added");
            else
                messages.Add("CBVN application was not added");

            return messages;
        }
    }
}

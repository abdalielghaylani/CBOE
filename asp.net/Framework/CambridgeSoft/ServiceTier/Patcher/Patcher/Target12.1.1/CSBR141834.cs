using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class CSBR141834 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// - Open Form 4013.
        ///     * For the coeForm with id 9, in viewMode, replace RegistryProjectsCslaDataSource with AllProjectsCslaDataSource
        ///     * For the coeForm with id 2, in viewMode, replace BatchProjectsCslaDataSource with AllProjectsCslaDataSource
        /// 
        /// - Open Form 4012.
        ///     * For the coeForm with id 0, in viewMode, replace RegistryProjectsCslaDataSource with AllProjectsCslaDataSource
        ///     * For the coeForm with id 4, in viewMode, replace BatchProjectsCslaDataSource with AllProjectsCslaDataSource
        /// 
        /// - Open Form 4011.
        ///     * For the coeForm with id 0, in viewMode, replace RegistryProjectsCslaDataSource with AllProjectsCslaDataSource
        ///     * For the coeForm with id 4, in viewMode, replace BatchProjectsCslaDataSource with AllProjectsCslaDataSource
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
            
            string formList = "4011, 4012, 4013";
            foreach(XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                if (formList.Contains(id))
                {
                    string registryFormID = (id == "4013") ? "9" : "0";
                    XmlNode regProjectNode = doc.SelectSingleNode("//COE:coeForm[@id='" + registryFormID + "']/COE:viewMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns", manager);
                    if (regProjectNode != null)
                    {
                        if (regProjectNode.InnerXml.Contains("RegistryProjectsCslaDataSource"))
                        {
                            regProjectNode.InnerXml = regProjectNode.InnerXml.Replace("RegistryProjectsCslaDataSource", "AllProjectsCslaDataSource");
                            messages.Add("Successfully replaced 'RegistryProjectsCslaDataSource' with AllProjectsCslaDataSource in FormGroup " + id);
                        }
                        else
                            messages.Add("'RegistryProjectsCslaDataSource' was not found in FormGroup " + id);
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form " + registryFormID + " was not found in FormGroup " + id);
                    }

                    string batchFormID = (id == "4013") ? "2" : "4";
                    XmlNode batchProjectNode = doc.SelectSingleNode("//COE:coeForm[@id='" + batchFormID + "']/COE:viewMode/COE:formElement[@name='Projects']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns", manager);
                    if (batchProjectNode != null)
                    {
                        if (batchProjectNode.InnerXml.Contains("BatchProjectsCslaDataSource"))
                        {
                            batchProjectNode.InnerXml = batchProjectNode.InnerXml.Replace("BatchProjectsCslaDataSource", "AllProjectsCslaDataSource");
                            messages.Add("Successfully replaced 'BatchProjectsCslaDataSource' with AllProjectsCslaDataSource in FormGroup " + id);
                        }
                        else
                            messages.Add("'BatchProjectsCslaDataSource' was not found in FormGroup " + id);
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form " + batchFormID + " was not found in FormGroup " + id);
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-141834 was successfully patched");
            }
            else
                messages.Add("CSBR-141834 was patched with errors");

            return messages;
        }
    }
}

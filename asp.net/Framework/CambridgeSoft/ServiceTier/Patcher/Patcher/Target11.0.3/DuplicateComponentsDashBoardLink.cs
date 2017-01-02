using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Requirement description:
    /// RLS 4.3: Users with the Manage_System_Duplicates privilege can see the count of duplicates across all projects regardless of RLS restrictions, but are limited to accessing only the projects that have privileges for. 
    /// </summary>
	public class DuplicateComponentsDashBoardLink : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open coeframeworkconfig.xml and search for a node named RegistrationDash1 (&lt;add name="RegistrationDash1") inside the customItems node add the following node:
        /// &lt;add name="DuplicateComponents" display="" privilege="" className="CambridgeSoft.COE.Registration.Services.RegSystem.GetDuplicateComponentsCount" assembly="CambridgeSoft.COE.Registration.Services"&gt;
        /// 	&lt;itemConfig&gt;
        ///     	&lt;add name="cssForText" value="DashBoardText"/&gt;
        /// 		&lt;add name="text" value="&lt;a href='/COERegistration/Forms/ComponentDuplicates/ContentArea/ComponentDuplicatesSearch.aspx?Caller=RD'&gt;{0} duplicates entries&lt;/a&gt;"/&gt;
        ///     &lt;/itemConfig&gt;
        /// &lt;/add&gt;
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
            string xmlAsString = @"<itemConfig>
										<add name=""cssForText"" value=""DashBoardText""/>
										<add name=""text"" value=""&lt;a href='/COERegistration/Forms/ComponentDuplicates/ContentArea/ComponentDuplicatesSearch.aspx?Caller=RD'&gt;{0} duplicates entries&lt;/a&gt;""/>
									</itemConfig>";
            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='RegistrationDash1']/customItems/add[@name='DuplicateComponents']");
            if (node == null)
            {
                XmlNode holder = frameworkConfig.SelectSingleNode("//add[@name='RegistrationDash1']/customItems");
                if (holder == null)
                {
                    messages.Add("RegistrationDash1 was not found, DuplicateComponents was not added");
                    errorsInPatch = true;
                }
                else
                {
                    XmlElement duplicateComponents = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "DuplicateComponents";
                    duplicateComponents.Attributes.Append(name);
					XmlAttribute display = frameworkConfig.CreateAttribute("display");
                    display.Value = string.Empty;
                    duplicateComponents.Attributes.Append(display);
                    XmlAttribute className = frameworkConfig.CreateAttribute("className");
                    className.Value = "CambridgeSoft.COE.Registration.Services.RegSystem.GetDuplicateComponentsCount";
                    duplicateComponents.Attributes.Append(className);
                    XmlAttribute assembly = frameworkConfig.CreateAttribute("assembly");
                    assembly.Value = "CambridgeSoft.COE.Registration.Services";
                    duplicateComponents.Attributes.Append(assembly);
                    duplicateComponents.InnerXml = xmlAsString;
                    holder.AppendChild(duplicateComponents);
                    messages.Add("DuplicateComponents added to RegistrationDash1 in coeframeworkconfig.xml");
                }
            }
            else
            {
                messages.Add("DuplicateComponents was already added");
                errorsInPatch = true;
            }
            if (!errorsInPatch)
                messages.Add("DuplicateComponentsDashBoardLink was successfully patched");
            else
                messages.Add("DuplicateComponentsDashBoardLink was patched with errors");
            return messages;
        }
    }
}

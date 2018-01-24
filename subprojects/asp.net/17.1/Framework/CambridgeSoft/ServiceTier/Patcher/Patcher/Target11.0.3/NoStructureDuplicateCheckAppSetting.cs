using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Adds Registration system settings which support, when activated, duplicate-checking
    /// using a component property or component identifier during bulk-loading.
    /// </summary>
    public class NoStructureDuplicateCheckAppSetting : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// <para>
        /// In coedb.coeconfiguration, the Registration settings XML needs to be edited. Add the following
        /// xml immediately AFTER to the '&lt;add nem="DUPLICATE_CHECKING"...&gt;' node:
        /// </para>
        /// <para>
        /// <![CDATA[
        /// <remove/>
        /// <add name="ENHANCED_DUPLICATE_SCAN" title="Extended Duplicate-checking" description="Configure property-based duplicate-checking in the absence of chemical structure data.">
        ///   <settings>
        ///     <add name="Level" value="Component" controlType="PICKLIST" description="The nesting level where the 'duplication' indicator is defined." allowedValues="Component" isAdmin="True" />
        ///     <add name="Type" value="Property" controlType="PICKLIST" description="The type of indicator used as the basis for 'property-based' duplicate-checking." allowedValues="Property|Identifier" isAdmin="True" />
        ///     <add name="Value" value="" controlType="TEXT" description="The name of the indicator that, when a value is present, determines logical uniqueness." isAdmin="True" />
        ///   </settings>
        /// </add>
        /// ]]>
        /// </para>
        /// <para>
        /// To activate the feature immediately, ensure the 'Type' is either Component or Identifier,
        /// and provide either a component custom property OR an identifier name for the 'Value'. Blank
        /// out the 'Value' to disable the feature.
        /// </para>
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(
            List<XmlDocument> forms, List<XmlDocument> dataviews
            , List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlDocument regConfig = null;
            foreach (XmlDocument config in configurations)
            {
                if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                {
                    regConfig = config;
                    break;
                }
            }

            //Coverity fix - CID 19420
            if (regConfig != null)
            {
                XmlNode insertAfterNode = regConfig.SelectSingleNode(
                    "/Registration/applicationSettings/groups/add[@name='DUPLICATE_CHECKING']");
                XmlNode shouldNotExistYet = regConfig.SelectSingleNode(
                    "/Registration/applicationSettings/groups/add[@name='ENHANCED_DUPLICATE_SCAN']");

                if (insertAfterNode != null && shouldNotExistYet == null)
                {
                    // generate the 'remove' node
                    XmlNode removeNode = regConfig.CreateElement("remove");
                    messages.Add("Created new 'remove' node.");

                    insertAfterNode.ParentNode.InsertAfter(removeNode, insertAfterNode);
                    messages.Add("Inserted 'remove' node into document at desired location.");

                    // generate the 'add' node
                    XmlElement settings = regConfig.CreateElement("add");
                    {
                        // name attribute
                        XmlAttribute nameNode = regConfig.CreateAttribute("name");
                        nameNode.Value = "ENHANCED_DUPLICATE_SCAN";
                        settings.Attributes.Append(nameNode);
                    }
                    {
                        // title attribute
                        XmlAttribute titleNode = regConfig.CreateAttribute("title");
                        titleNode.Value = "Extended Duplicate-checking";
                        settings.Attributes.Append(titleNode);
                    }
                    {
                        // description node
                        XmlAttribute descNode = regConfig.CreateAttribute("description");
                        descNode.Value = "Configure property-based duplicate-checking in the absence of chemical structure data.";
                        settings.Attributes.Append(descNode);
                    }

                    string settingsXml =
                    @"<settings>
                  <add name=""Level"" value=""Component"" controlType=""PICKLIST"" description=""The nesting level where the 'duplication' indicator is defined."" allowedValues=""Component"" isAdmin=""False""/>
                  <add name=""Type"" value=""Property"" controlType=""PICKLIST"" description=""The type of indicator used as the basis for 'property-based' duplicate-checking."" allowedValues=""Property|Identifier"" isAdmin=""False"" />
                  <add name=""Value"" value="""" controlType=""TEXT"" description=""The name of the indicator that, when a value is present, determines logical uniqueness."" isAdmin=""False"" />
                  </settings>";

                    settings.InnerXml = settingsXml;
                    messages.Add("Created new 'add' node, corresponding to the new 'remove' node.");

                    insertAfterNode.ParentNode.InsertAfter(settings, removeNode);
                    messages.Add("Inserted 'add' node into document at desired location.");
                }
                else
                {
                    errorsInPatch = true;

                    if (insertAfterNode == null)
                        messages.Add("Unable to find the node after which the new settings would have been inserted");
                    if (shouldNotExistYet != null)
                        messages.Add("This patch may have already been applied.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Unable to find the Reg App Settings node.");
            }

            if (!errorsInPatch)
                messages.Add("NoStructureDuplicateCheckAppSetting was successfully patched");
            else
                messages.Add("NoStructureDuplicateCheckAppSetting was patched with errors");
            return messages;

        }
	}
}

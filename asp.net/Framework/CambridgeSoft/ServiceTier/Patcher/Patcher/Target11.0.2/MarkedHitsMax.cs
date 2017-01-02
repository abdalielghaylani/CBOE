using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Requirement description:
    /// Limit the amount of hits that can be marked.
    /// </summary>
	public class MarkedHitsMax : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open coeframeworkconfig.xml and look for the generalOptions belonging to the parent application REGISTRATION in the ChemBioViz's application section.
        /// 
        /// Add the following node as child:
        ///     &lt;add name="MarkedHitsMax" value="500" /&gt;
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
            //<add name="MarkedHitsMax" value="500" />
            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='CHEMBIOVIZ']/parentApplication/add[@name='REGISTRATION']/applicationBehaviour/generalOptions");
            if (node != null)
            {
                XmlNode markedHitsMax = node.SelectSingleNode("./add[@name='MarkedHitsMax']");
                if (markedHitsMax != null)
                {
                    messages.Add("MarkedHitsMax was already there and was not modified");
                    errorsInPatch = true;
                }
                else
                {
                    markedHitsMax = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "MarkedHitsMax";
                    markedHitsMax.Attributes.Append(name);
                    XmlAttribute value = frameworkConfig.CreateAttribute("value");
                    value.Value = "500";
                    markedHitsMax.Attributes.Append(value);
                    node.AppendChild(markedHitsMax);
                    messages.Add("MarkedHitsMax added to coeframeworkconfig.xml");
                }
            }
            else
            {
                messages.Add("Parent node for MarkedHitsMax was not found");
                errorsInPatch = true;
            }

            if (!errorsInPatch)
                messages.Add("MarkedHitsMax was successfully patched");
            else
                messages.Add("MarkedHitsMax was patched with errors");
            return messages;
        }
    }
}

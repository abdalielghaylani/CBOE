using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Added support for HighlightedStructure select clause
    /// </summary>
	class AddHighlightedStructureMapping : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to solve:
        /// 
        /// Open the coeframeworkconfig.xml file and search for the select clause mappings. The node name is selectClauses.
        /// Add as the last child of that node the following one:
        /// 
        /// &lt;selectClause name="highlightedstructure" parserClassName="CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseStructureHighlight"/&gt;
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
            bool errorsInPatch = false;
            
            XmlNamespaceManager manager = new XmlNamespaceManager(frameworkConfig.NameTable);
            manager.AddNamespace("COE", "COE.Mappings");

            XmlNode node = frameworkConfig.SelectSingleNode("//COE:mappings/COE:selectClauses", manager);
            
            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("SelectClauses mappings where not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlNode highlightedstructureNode = node.SelectSingleNode("./COE:selectClause[@name='highlightedstructure']", manager);
                if (highlightedstructureNode == null)
                {
                    //<selectClause name="highlightedstructure" parserClassName="CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseStructureHighlight"/>
                    highlightedstructureNode = frameworkConfig.CreateElement("selectClause", "COE.Mappings");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "highlightedstructure";
                    highlightedstructureNode.Attributes.Append(name);
                    XmlAttribute parserClassName = frameworkConfig.CreateAttribute("parserClassName");
                    parserClassName.Value = "CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems.SelectClauseStructureHighlight";
                    highlightedstructureNode.Attributes.Append(parserClassName);
                    node.AppendChild(highlightedstructureNode);
                    messages.Add("HighlightedStructure mapping succesfully added for FW config");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Highlighted structure was already present in select clause mappings (FW Config)");
                }
            }
            if (!errorsInPatch)
                messages.Add("AddHighlightedStructureMapping was successfully patched");
            else
                messages.Add("AddHighlightedStructureMapping was patched with errors");
            return messages;
        }
    }
}

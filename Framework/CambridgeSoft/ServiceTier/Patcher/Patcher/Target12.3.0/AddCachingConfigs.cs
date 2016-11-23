using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class AddCachingConfigs : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;

            XmlNodeList oldConfig = frameworkConfig.SelectNodes("//*[@cacheDataview]");
            if (oldConfig.Count == 0)
                messages.Add("There were no old config for caching dataviews to be removed");

            foreach (XmlNode node in oldConfig)
            {
                node.Attributes.Remove(node.Attributes["cacheDataview"]);
            }
            string dataviewConfigNode = @"<dataview>
		  <add name=""cache"" value=""ServerAndClientCache"" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name=""absoluteExpiration"" value="""" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name=""slidingExpiration"" value=""20"" /> <!-- int representing minutes -->
		  <add name=""defaultPriority"" value=""Normal"" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</dataview>";
            string searchCriteriaConfigNode = @"<searchCriteria>
		  <add name=""cache"" value=""ServerAndClientCache"" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name=""absoluteExpiration"" value="""" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name=""slidingExpiration"" value=""20"" /> <!-- int representing minutes -->
		  <add name=""defaultPriority"" value=""Normal"" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</searchCriteria>";
            string formConfigNode = @"<form>
		  <add name=""cache"" value=""ServerAndClientCache"" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name=""absoluteExpiration"" value="""" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name=""slidingExpiration"" value=""20"" /> <!-- int representing minutes -->
		  <add name=""defaultPriority"" value=""Normal"" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</form>";

            XmlNode applicationDefaultsNode = frameworkConfig.SelectSingleNode("//applicationDefaults");
            XmlNode cachingNode = applicationDefaultsNode.SelectSingleNode("./caching");
            if (cachingNode == null)
            {
                cachingNode = frameworkConfig.CreateElement("caching");
                applicationDefaultsNode.AppendChild(cachingNode);
            }

            if (cachingNode.SelectSingleNode("dataview") == null)
            {
                cachingNode.InnerXml += dataviewConfigNode;
            }
            else
            {
                errorInPatch = true;
                messages.Add("dataview caching was already configured");
            }

            if (cachingNode.SelectSingleNode("searchCriteria") == null)
            {
                cachingNode.InnerXml += searchCriteriaConfigNode;
            }
            else
            {
                errorInPatch = true;
                messages.Add("searchCriteria caching was already configured");
            }

            if (cachingNode.SelectSingleNode("form") == null)
            {
                cachingNode.InnerXml += formConfigNode;
            }
            else
            {
                errorInPatch = true;
                messages.Add("form caching was already configured");
            }
            /*<caching>
		<dataview>
		  <add name="cache" value="ServerAndClientCache" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name="absoluteExpiration" value="" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name="slidingExpiration" value="20" /> <!-- int representing minutes -->
		  <add name="defaultPriority" value="Normal" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</dataview>
		<searchCriteria>
		  <add name="cache" value="ServerAndClientCache" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name="absoluteExpiration" value="" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name="slidingExpiration" value="20" /> <!-- int representing minutes -->
		  <add name="defaultPriority" value="Normal" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</searchCriteria>
		<form>
		  <add name="cache" value="ServerAndClientCache" /> <!-- allowable values are: Disabled | ClientCache | ServerCache | ServerAndClientCache -->
		  <add name="absoluteExpiration" value="" /> <!-- mm/dd/aaaa HH:mm:ss -->
		  <add name="slidingExpiration" value="20" /> <!-- int representing minutes -->
		  <add name="defaultPriority" value="Normal" /> <!-- allowable values are: AboveNormal | BelowNormal | Default | High | Low | Normal | NotRemovable -->
		</form>
	  </caching>*/
            if (!errorInPatch)
                messages.Add("Default caching configurations were successfully added");
            else
                messages.Add("Default caching configurations were added with errors");

            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class MigCOEObjectConfig : BugFixBaseCommand
    {
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            string structcommentsPropertyXml = @"<Property name=""STRUCT_COMMENTS"" friendlyName=""STRUCT_COMMENTS"" type=""TEXT"" precision=""200"" sortOrder=""0"">
                <validationRuleList>
                  <validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters"">
                    <params>
                      <param name=""min"" value=""0""/>
                      <param name=""max"" value=""200""/>
                    </params>
                  </validationRule>
                </validationRuleList>
              </Property>
";
            string structnamePropertyXml = @"<Property name=""STRUCT_NAME"" friendlyName=""STRUCT_NAME"" type=""TEXT"" precision=""2000"" sortOrder=""1"">
                <validationRuleList>
                  <validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 2000 characters"">
                    <params>
                      <param name=""min"" value=""0""/>
                      <param name=""max"" value=""2000""/>
                    </params>
                  </validationRule>
                </validationRuleList>
              </Property>
";

            XmlNode structurePropertyListNode = objectConfig.SelectSingleNode(
                "/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList"
                );

            if (structurePropertyListNode != null)
            {
                XmlNode structcommentsPropertyNode = structurePropertyListNode.SelectSingleNode(
                    "Property[@name='STRUCT_COMMENTS']"
                );
                if (structcommentsPropertyNode == null)
                    structurePropertyListNode.InnerXml += structcommentsPropertyXml;

                XmlNode structNamePropertyNode = structurePropertyListNode.SelectSingleNode(
                    "Property[@name='STRUCT_NAME']"
                );
                if (structNamePropertyNode == null)
                    structurePropertyListNode.InnerXml += structnamePropertyXml;

                messages.Add("Adding default Structure property succeeded.");
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Cannot find Structure's PropertyList node");
            }

            if (!errorsInPatch && messages.Count != 0)
            {
                messages.Add("Add default structure custom property process succeed!");
            }
            else
            {
                messages.Add("AddDefaultStructureCustomPropertyToCoeObjectConfig failed to patch.");
            }

            return messages;
        }
    }
}

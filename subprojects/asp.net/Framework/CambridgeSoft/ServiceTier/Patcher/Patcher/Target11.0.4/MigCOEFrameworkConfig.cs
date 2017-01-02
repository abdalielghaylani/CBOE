using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// 1. Modify SEQUENCE table editor
    ///     a. Change REGNUMBERLENGTH to use innerXml lookup, which ranges from 4 - 9;
    /// </summary>
    public class MigCOEFrameworkConfig : BugFixBaseCommand
    {
        public const string REGNUMBERLENGTH_XPath =
        "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='REGNUMBERLENGTH']";
        public const String REGISTRATIONTableEditorData_XPath =
        "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData";

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            if (frameworkConfig != null)
            {
                // Reserve the original xml to recover in case of exception during update.
                string original_OuterXML = frameworkConfig.OuterXml;

                try
                {
                    UpdateREGNUMBERLENGTHNode(frameworkConfig, messages);


                }
                catch (Exception ex)
                {
                    errorsInPatch = true;
                    messages.Add("Processing with error: " + ex.Message);
                }

                if (errorsInPatch)
                {
                    messages.Add("Failed to change framework config, reload with the original one.");
                    frameworkConfig.LoadXml(original_OuterXML);
                }
                else
                {
                    messages.Add("All changes were made.");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Error: Can't get framework config info.");
            }

            return messages;
        }

        private void UpdateREGNUMBERLENGTHNode(XmlDocument frameworkConfig, List<string> messages)
        {
            XmlNode REGNUMBERLENGTHNode = frameworkConfig.SelectSingleNode(REGNUMBERLENGTH_XPath);
            // Add lookupLocation and alias attributes

            XmlAttribute lookupLocation = frameworkConfig.CreateAttribute("lookupLocation");
            lookupLocation.Value = "innerXml_REGNUMBERLENGTHS";
            REGNUMBERLENGTHNode.Attributes.Append(lookupLocation);

            XmlAttribute alias = frameworkConfig.CreateAttribute("alias");
            alias.Value = "Reg Number Length";
            REGNUMBERLENGTHNode.Attributes.Append(alias);

            // Preserve only requiredField validation rule
            REGNUMBERLENGTHNode.InnerXml =
                @"<validationRule><add name=""requiredField"" errorMessage=""'Reg Number Length' is required""/></validationRule>";

            messages.Add("Succeed to update REGNUMBERLENGTH node.");

            // Add REGNUMBERLENGTH lookup
            if (frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='REGNUMBERLENGTHS']") == null)
            {
                XmlNode InnerXml = frameworkConfig.SelectSingleNode(
                    "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml");
                XmlNode InnerXml_REGNUMBERLENGTHS = frameworkConfig.CreateElement("add");
                XmlAttribute name = frameworkConfig.CreateAttribute("name");
                name.Value = "REGNUMBERLENGTHS";
                InnerXml_REGNUMBERLENGTHS.Attributes.Append(name);
                InnerXml_REGNUMBERLENGTHS.InnerXml = @"<innerXmlData>
              <add name=""None"" value=""-1"" display=""No padding""/>
              <add name=""four"" value=""4"" display=""4 digits""/>
              <add name=""five"" value=""5"" display=""5 digits""/>
              <add name=""six"" value=""6"" display=""6 digits""/>
              <add name=""seven"" value=""7"" display=""7 digits""/>
              <add name=""eight"" value=""8"" display=""8 digits""/>
              <add name=""nine"" value=""9"" display=""9 digits""/>
            </innerXmlData>";

                InnerXml.AppendChild(InnerXml_REGNUMBERLENGTHS);

                messages.Add("Succeed to add REGNUMBERLENGTHS inner xml.");
            }
            else
                messages.Add("REGNUMBERLENGTHS inner xml was already present.");
        }
    }
}

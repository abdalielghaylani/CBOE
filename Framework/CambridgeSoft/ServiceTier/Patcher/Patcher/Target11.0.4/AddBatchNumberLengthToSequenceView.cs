using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-133562: IE6: Batch Submission link under DocManager Enterprise cannot be selected.
    /// </summary>
    class AddBatchNumberLengthToSequenceView : BugFixBaseCommand
    {

        public const string BATCHNUMBERLENGTH_XPath =
        "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='BATCHNUMLENGTH']";

        public const String REGISTRATIONTableEditorData_XPath =
        "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData";

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            if (frameworkConfig != null)
            {
                string original_OuterXML=frameworkConfig.OuterXml;

                try
                {
                    if (frameworkConfig.SelectSingleNode(BATCHNUMBERLENGTH_XPath) == null)
                    {
                        XmlNode REGISTRATIONTableEditorData = frameworkConfig.SelectSingleNode(REGISTRATIONTableEditorData_XPath);
                        XmlNode BATCHNUMBERLENGTH = frameworkConfig.CreateElement("add");

                        XmlAttribute name = frameworkConfig.CreateAttribute("name");
                        name.Value = "BATCHNUMLENGTH";
                        BATCHNUMBERLENGTH.Attributes.Append(name);

                        XmlAttribute dataType = frameworkConfig.CreateAttribute("dataType");
                        dataType.Value = "number";
                        BATCHNUMBERLENGTH.Attributes.Append(dataType);

                        XmlAttribute lookupLocation = frameworkConfig.CreateAttribute("lookupLocation");
                        lookupLocation.Value = "innerXml_BATCHNUMBERLENGTHS";
                        BATCHNUMBERLENGTH.Attributes.Append(lookupLocation);

                        XmlAttribute alias = frameworkConfig.CreateAttribute("alias");
                        alias.Value = "Batch Number Length";
                        BATCHNUMBERLENGTH.Attributes.Append(alias);

                        BATCHNUMBERLENGTH.InnerXml = @"<validationRule><add name=""requiredField"" errorMessage=""'Batch Number Length' is required""/></validationRule>";

                        REGISTRATIONTableEditorData.InsertAfter(BATCHNUMBERLENGTH, REGISTRATIONTableEditorData.FirstChild);
                        messages.Add("Succeed to add new node.");
                    }

                    XmlNode InnerXml_SALTSUFFIXGENERATORS = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='SALTSUFFIXGENERATORS']");
                    if (InnerXml_SALTSUFFIXGENERATORS!=null&&
                        InnerXml_SALTSUFFIXGENERATORS.InnerXml!=XmlDepository.SALTSUFFIXGENERATORS_InnerXMl)
                    {
                        InnerXml_SALTSUFFIXGENERATORS.InnerXml = XmlDepository.SALTSUFFIXGENERATORS_InnerXMl;

                        messages.Add("Succeed to modifiy node.");
                    }

                    string InnerXml_BATCHNUMBERLENGTHS_XPath = "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml/add[@name='BATCHNUMBERLENGTHS']";
                    if (frameworkConfig.SelectSingleNode(InnerXml_BATCHNUMBERLENGTHS_XPath) == null)
                    {
                        XmlNode InnerXml = frameworkConfig.SelectSingleNode("/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/innerXml");
                        XmlNode InnerXml_BATCHNUMBERLENGTHS = frameworkConfig.CreateElement("add");
                        XmlAttribute name = frameworkConfig.CreateAttribute("name");
                        name.Value = "BATCHNUMBERLENGTHS";
                        InnerXml_BATCHNUMBERLENGTHS.Attributes.Append(name);
                        InnerXml_BATCHNUMBERLENGTHS.InnerXml = XmlDepository.BATCHNUMBERLENGTHS_InnerXMl;

                        InnerXml.InsertAfter(InnerXml_BATCHNUMBERLENGTHS, InnerXml_SALTSUFFIXGENERATORS);

                        messages.Add("Succeed to add new node.");
                    }
                }
                catch(Exception ex)
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
                messages.Add("Error: Can't get framework config info.");
            }

           
            return messages;
        }

    }

    class XmlDepository
    {
        public const string SALTSUFFIXGENERATORS_InnerXMl = @"<innerXmlData>
              <add name=""Code"" value=""code"" display=""Codes""/>
              <add name=""Formula"" value=""formula"" display=""Formulae""/>
              <add name=""ID"" value=""id"" display=""IDs""/>
              <add name=""Descriptions"" value=""desc"" display=""Descriptions""/>
            </innerXmlData>";

        public const string BATCHNUMBERLENGTHS_InnerXMl = @"<innerXmlData>
              <add name=""None"" value=""-1"" display=""No padding""/>
              <add name=""two"" value=""2"" display=""2 digits""/>
              <add name=""three"" value=""3"" display=""3 digits""/>
              <add name=""four"" value=""4"" display=""4 digits""/>
              <add name=""five"" value=""5"" display=""5 digits""/>
              <add name=""six"" value=""6"" display=""6 digits""/>
            </innerXmlData>";
    }
}
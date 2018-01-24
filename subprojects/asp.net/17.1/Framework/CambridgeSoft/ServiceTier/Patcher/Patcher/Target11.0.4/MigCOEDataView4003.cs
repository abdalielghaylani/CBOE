using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Migrate COEDataView 4003
    /// </summary>
    public class MigCOEDataView4003 : BugFixBaseCommand
    {
        /// <summary>
        /// 1. Add application attribute to dataview
        /// 2. Add STRUCT_COMMENTS field to VW_MIXTURE_STRUCTURE table
        /// 3. Add VW_STRUCTURE_IDENTIFIER table
        /// 4. Add relationship between VW_MIXTURE_STRUCTURE and VW_STRUCTURE_IDENTIFIER
        /// 5. Add VW_BATCHIDENTIFIER table
        /// 6. Add relationship between VW_BATCH and VW_BATCHIDENTIFIER
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument dataview in dataviews)
            {
                string dataviewid = dataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataview.DocumentElement.Attributes["dataviewid"].Value;
                if (dataviewid == "4003")
                {
                    string originalDataview = dataview.OuterXml;
                    try
                    {
                        XmlNamespaceManager manager = new XmlNamespaceManager(dataview.NameTable);
                        string xmlns = "COE.COEDataView";
                        string prefix = "COE";
                        manager.AddNamespace(prefix, xmlns);

                        //1. Add application attribute to dataview
                        if (dataview.DocumentElement.Attributes["application"] == null)
                        {
                            XmlAttribute application = dataview.CreateAttribute("application");
                            application.Value = "REGISTRATION";
                            dataview.DocumentElement.Attributes.Append(application);
                            messages.Add("application already set");
                        }
                        else
                            messages.Add("Application setting already exists");

                        //2. Add STRUCT_COMMENTS field to VW_MIXTURE_STRUCTURE table
                        XmlNode tables = dataview.SelectSingleNode("/COE:COEDataView/COE:tables", manager);
                        XmlNode VW_MIXTURE_STRUCTURE_Table = tables.SelectSingleNode("COE:table[@name='VW_MIXTURE_STRUCTURE']", manager);
                        if (VW_MIXTURE_STRUCTURE_Table != null)
                        {
                            XmlNode STRUCT_COMMENTS_Field = VW_MIXTURE_STRUCTURE_Table.SelectSingleNode("COE:fields[@name='STRUCT_COMMENTS']", manager);
                            if (STRUCT_COMMENTS_Field == null)
                            {
                                STRUCT_COMMENTS_Field = dataview.CreateElement("fields", xmlns);
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("id"));
                                STRUCT_COMMENTS_Field.Attributes["id"].Value = "452";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("name"));
                                STRUCT_COMMENTS_Field.Attributes["name"].Value = "STRUCT_COMMENTS";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("dataType"));
                                STRUCT_COMMENTS_Field.Attributes["dataType"].Value = "TEXT";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("alias"));
                                STRUCT_COMMENTS_Field.Attributes["alias"].Value = "STRUCT_COMMENTS";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("indexType"));
                                STRUCT_COMMENTS_Field.Attributes["indexType"].Value = "NONE";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("mimeType"));
                                STRUCT_COMMENTS_Field.Attributes["mimeType"].Value = "NONE";
                                STRUCT_COMMENTS_Field.Attributes.Append(dataview.CreateAttribute("visible"));
                                STRUCT_COMMENTS_Field.Attributes["visible"].Value = "1";

                                VW_MIXTURE_STRUCTURE_Table.AppendChild(STRUCT_COMMENTS_Field);
                                messages.Add("Add STRUCT_COMMENTS field to VW_MIXTURE_STRUCTURE table");
                            }
                            else
                                messages.Add("VW_MIXTURE_STRUCTURE table already contains a STRUCT_COMMENTS field");
                        }

                        //3. Add VW_STRUCTURE_IDENTIFIER table
                        XmlNode VW_STRUCTURE_IDENTIFIER_Table = tables.SelectSingleNode("COE:table[@name='VW_STRUCTURE_IDENTIFIER']", manager);
                        if (VW_STRUCTURE_IDENTIFIER_Table == null)
                        {
                            VW_STRUCTURE_IDENTIFIER_Table = dataview.CreateElement("table", xmlns);
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("id"));
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes["id"].Value = "15";
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("name"));
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes["name"].Value = "VW_STRUCTURE_IDENTIFIER";
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("alias"));
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes["alias"].Value = "VW_STRUCTURE_IDENTIFIER";
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("database"));
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes["database"].Value = "REGDB";
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("primaryKey"));
                            VW_STRUCTURE_IDENTIFIER_Table.Attributes["primaryKey"].Value = "1500";

                            //the order of each table should not matter, but mimicing changes in DataView xml file is the first option
                            XmlNode VW_COMPOUND_IDENTIFIER_Table = tables.SelectSingleNode("COE:table[@id='6' and @name='VW_COMPOUND_IDENTIFIER']", manager);
                            if (VW_COMPOUND_IDENTIFIER_Table != null)
                                tables.InsertAfter(VW_STRUCTURE_IDENTIFIER_Table, VW_COMPOUND_IDENTIFIER_Table);
                            else
                                tables.AppendChild(VW_STRUCTURE_IDENTIFIER_Table);
                            VW_STRUCTURE_IDENTIFIER_Table.InnerXml = @"<fields id=""1500"" name=""ID"" dataType=""INTEGER"" alias=""ID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
      <fields id=""1501"" name=""STRUCTUREID"" dataType=""INTEGER"" alias=""STRUCTUREID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
      <fields id=""1502"" name=""VALUE"" dataType=""TEXT"" alias=""VALUE"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
      <fields id=""1503"" name=""TYPE"" dataType=""INTEGER"" lookupFieldId=""700"" lookupDisplayFieldId=""701"" alias=""TYPE"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>";

                            messages.Add("Add VW_STRUCTURE_IDENTIFIER table");
                        }
                        else
                            messages.Add("VW_STRUCTURE_IDENTIFIER table already exists");

                        //4. Add relationship between VW_MIXTURE_STRUCTURE and VW_STRUCTURE_IDENTIFIER
                        XmlNode relationships = dataview.SelectSingleNode("/COE:COEDataView/COE:relationships", manager);
                        if (relationships != null)
                        {
                            XmlNode VMS_VSI_Relationship = relationships.SelectSingleNode("COE:relationship[@parent='4' and @child='15']", manager);
                            if (VMS_VSI_Relationship == null)
                            {
                                VMS_VSI_Relationship = dataview.CreateElement("relationship", xmlns);
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("child"));
                                VMS_VSI_Relationship.Attributes["child"].Value = "15";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("parent"));
                                VMS_VSI_Relationship.Attributes["parent"].Value = "4";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("childkey"));
                                VMS_VSI_Relationship.Attributes["childkey"].Value = "1501";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("parentkey"));
                                VMS_VSI_Relationship.Attributes["parentkey"].Value = "404";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("jointype"));
                                VMS_VSI_Relationship.Attributes["jointype"].Value = "INNER";

                                //Once again, the order of each relationship should not matter, but mimicing changes in DataView xml file as first option
                                XmlNode VMS_VCI_Relationship = relationships.SelectSingleNode("COE:relationship[@parent='4' and @child='6']", manager);
                                if (VMS_VCI_Relationship != null)
                                    relationships.InsertAfter(VMS_VSI_Relationship, VMS_VCI_Relationship);
                                else
                                    relationships.AppendChild(VMS_VSI_Relationship);

                                messages.Add("Create relationship between VW_MIXTURE_STRUCTURE and VW_STRUCTURE_IDENTIFIER tables");
                            }
                            else
                                messages.Add("Relationship between VW_MIXTURE_STRUCTURE and VW_STRUCTURE_IDENTIFIER already exists");
                        }

                        //5. Add VW_BATCHIDENTIFIER table
                        XmlNode VW_BATCHIDENTIFIER_Table = tables.SelectSingleNode("COE:table[@name='VW_BATCHIDENTIFIER']", manager);
                        if (VW_BATCHIDENTIFIER_Table == null)
                        {
                            VW_BATCHIDENTIFIER_Table = dataview.CreateElement("table", xmlns);
                            VW_BATCHIDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("id"));
                            VW_BATCHIDENTIFIER_Table.Attributes["id"].Value = "14";
                            VW_BATCHIDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("name"));
                            VW_BATCHIDENTIFIER_Table.Attributes["name"].Value = "VW_BATCHIDENTIFIER";
                            VW_BATCHIDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("alias"));
                            VW_BATCHIDENTIFIER_Table.Attributes["alias"].Value = "VW_BATCHIDENTIFIER";
                            VW_BATCHIDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("database"));
                            VW_BATCHIDENTIFIER_Table.Attributes["database"].Value = "REGDB";
                            VW_BATCHIDENTIFIER_Table.Attributes.Append(dataview.CreateAttribute("primaryKey"));
                            VW_BATCHIDENTIFIER_Table.Attributes["primaryKey"].Value = "1400";

                            tables.AppendChild(VW_BATCHIDENTIFIER_Table);
                            VW_BATCHIDENTIFIER_Table.InnerXml = @"
<fields id=""1400"" name=""ID"" dataType=""INTEGER"" alias=""ID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""1401"" name=""BATCHID"" dataType=""INTEGER"" alias=""STRUCTUREID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""1402"" name=""VALUE"" dataType=""TEXT"" alias=""VALUE"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""1403"" name=""TYPE"" dataType=""INTEGER"" lookupFieldId=""700"" lookupDisplayFieldId=""701"" alias=""TYPE"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
";

                            messages.Add("VW_BATCHIDENTIFIER table added successfully");
                        }
                        else
                            messages.Add("VW_BATCHIDENTIFIER table already exists");

                        //6. Add relationship between VW_BATCH and VW_BATCHIDENTIFIER
                        if (relationships != null)
                        {
                            XmlNode VMS_VSI_Relationship = relationships.SelectSingleNode("COE:relationship[@parent='9' and @child='14']", manager);
                            if (VMS_VSI_Relationship == null)
                            {
                                VMS_VSI_Relationship = dataview.CreateElement("relationship", xmlns);
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("child"));
                                VMS_VSI_Relationship.Attributes["child"].Value = "14";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("parent"));
                                VMS_VSI_Relationship.Attributes["parent"].Value = "9";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("childkey"));
                                VMS_VSI_Relationship.Attributes["childkey"].Value = "1401";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("parentkey"));
                                VMS_VSI_Relationship.Attributes["parentkey"].Value = "900";
                                VMS_VSI_Relationship.Attributes.Append(dataview.CreateAttribute("jointype"));
                                VMS_VSI_Relationship.Attributes["jointype"].Value = "INNER";

                                relationships.AppendChild(VMS_VSI_Relationship);

                                messages.Add("Create relationship between VW_BATCH and VW_BATCHIDENTIFIER tables");
                            }
                            else
                                messages.Add("Relationship between VW_BATCH and VW_BATCHIDENTIFIER already exists");
                        }
                    }
                    catch (Exception e)
                    {
                        errorsInPatch = true;
                        messages.Add("Exception occurs: " + e.Message);
                    }

                    if (errorsInPatch)
                        dataview.LoadXml(originalDataview);
                }
            }

            if (errorsInPatch)
                messages.Add("Fail to patch COEDataView 4003");
            else
                messages.Add("Succeed to patch COEDataView 4003");

            return messages;
        }

    }
}
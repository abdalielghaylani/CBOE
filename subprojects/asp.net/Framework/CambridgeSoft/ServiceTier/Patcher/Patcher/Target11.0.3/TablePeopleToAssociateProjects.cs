using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	public class TablePeopleToAssociateProjects : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            /*<add name="COEDB.PEOPLE" description="People" primaryKeyField="PERSON_ID" editPriv="MANAGE_PEOPLE_PROJECT" addPriv="hidden" deletePriv="hidden" 
             * displayName="PEOPLE" disableChildTables="False" disableTable="False"> */
            /* //add[@name='REGISTRATION']/tableEditor/ */
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='REGISTRATION']/tableEditor");
            if (node == null)
            {
                errorsInPatch = true;
                messages.Add("REGISTRATION's tableEditor was not found on coeframeworkconfig.xml");
            }
            else
            {
                XmlNode peopleTable = node.SelectSingleNode("./add[@name='COEDB.PEOPLE']");
                if (peopleTable != null)
                {
                    errorsInPatch = true;
                    messages.Add("People table was already added to registration's tableEditor. No change was made");
                }
                else
                {
                    peopleTable = frameworkConfig.CreateElement("add");
                    peopleTable.InnerXml = @"
                        <tableEditorData>
						  <add name=""PERSON_ID"" dataType=""number"" />
						  <add name=""USER_ID"" dataType=""string"" />
						</tableEditorData>
						<childTable>
						  <add name=""VW_PROJECT"" primaryKeyField=""PROJECTID"" parentPK=""VW_PEOPLEPROJECT.PERSONID"" childPK=""VW_PEOPLEPROJECT.PROJECTID"" displayName=""PROJECTS"">
							<childTableData>
							  <add name=""PROJECTID"" dataType=""number"" />
							  <add name=""NAME"" dataType=""string"" />
							  <add name=""DESCRIPTION"" dataType=""string"" />
							  <add name=""ACTIVE"" dataType=""string"" />
							  <add name=""TYPE"" dataType=""string"" />
							  <add name=""ISPUBLIC"" dataType=""string"" />
								</childTableData>
							</add>
						</childTable>";
                    node.AppendChild(peopleTable);
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "COEDB.PEOPLE";
                    peopleTable.Attributes.Append(name);
                    XmlAttribute description = frameworkConfig.CreateAttribute("description");
                    description.Value = "People";
                    peopleTable.Attributes.Append(description);
                    XmlAttribute primaryKeyField = frameworkConfig.CreateAttribute("primaryKeyField");
                    primaryKeyField.Value = "PERSON_ID";
                    peopleTable.Attributes.Append(primaryKeyField);
                    XmlAttribute editPriv = frameworkConfig.CreateAttribute("editPriv");
                    editPriv.Value = "MANAGE_PEOPLE_PROJECT";
                    peopleTable.Attributes.Append(editPriv);
                    XmlAttribute addPriv = frameworkConfig.CreateAttribute("addPriv");
                    addPriv.Value = "hidden";
                    peopleTable.Attributes.Append(addPriv);
                    XmlAttribute deletePriv = frameworkConfig.CreateAttribute("deletePriv");
                    deletePriv.Value = "hidden";
                    peopleTable.Attributes.Append(deletePriv);
                    XmlAttribute displayName = frameworkConfig.CreateAttribute("displayName");
                    displayName.Value = "PEOPLE";
                    peopleTable.Attributes.Append(displayName);
                    XmlAttribute disableChildTables = frameworkConfig.CreateAttribute("disableChildTables");
                    disableChildTables.Value = "False";
                    peopleTable.Attributes.Append(disableChildTables);
                    XmlAttribute disableTable = frameworkConfig.CreateAttribute("disableTable");
                    disableTable.Value = "True";
                    peopleTable.Attributes.Append(disableTable);

                    messages.Add("People table was successfully added to registration's tableEditor configuration");
                }
            }
            if (!errorsInPatch)
                messages.Add("SeqProjectsForProjects was successfully patched");
            else
                messages.Add("SeqProjectsForProjects was patched with errors");
            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// For hiding projects and identifiers grids easily with page control settings we want to avoid having different ids for each mode.
    /// Currently a letter for the mode is being added as in AMix_ProjectsUltraGrid for add mode.
    /// 
    /// This is necessary in the Support for configuring the desired level at which projects and identifiers would be shown.
    /// </summary>
	public class UnifyingWebGridsIdsForPageControlSettings : BugFixBaseCommand
	{
        /// <summary>
        /// No manual fix is provided.
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

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                switch (id)
                {
                    #region Form 4010
                    case "4010":
                        XmlNode addModeProjects = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Projects']", manager);
                        if (addModeProjects != null)
                        {
                            if (addModeProjects.InnerXml.Contains("AMix_"))
                            {
                                addModeProjects.InnerXml = addModeProjects.InnerXml.Replace("AMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in project form element on add mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on add mode for form 0 in coeform 4010");
                            }

                            if (addModeProjects.InnerXml.Contains(">ProjectsCslaDataSource") || addModeProjects.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                addModeProjects.InnerXml = addModeProjects.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in project form element on add mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                if (addModeProjects.InnerXml.Contains(">RegistryProjectsCslaDataSource") || addModeProjects.InnerXml.Contains("\"RegistryProjectsCslaDataSource"))
                                    messages.Add("ProjectsCslaDataSource was already changed in project form element on add mode for form 0 in coeform 4010");
                                else
                                {
                                    errorsInPatch = true;
                                    messages.Add("ProjectsCslaDataSource was not found in project form element on add mode for form 0 in coeform 4010");
                                }
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on add mode for form 0 in coeform 4010"); 
                        }

                        XmlNode addModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        if (addModeIdentifiers != null)
                        {
                            if (addModeIdentifiers.InnerXml.Contains("AMix_"))
                            {
                                addModeIdentifiers.InnerXml = addModeIdentifiers.InnerXml.Replace("AMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on add mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 0 in coeform 4010");
                        }

                        //-----------
                        XmlNode editModeProjectList = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='ProjectList']", manager);
                        if (editModeProjectList != null)
                        {
                            if (editModeProjectList.InnerXml.Contains("EMix_"))
                            {
                                editModeProjectList.InnerXml = editModeProjectList.InnerXml.Replace("EMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in ProjectList form element on edit mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in ProjectList form element on edit mode for form 0 in coeform 4010");
                            }

                            if (editModeProjectList.InnerXml.Contains(">ProjectsCslaDataSource") || editModeProjectList.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                editModeProjectList.InnerXml = editModeProjectList.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in ProjectList form element on edit mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in ProjectList form element on edit mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("ProjectList form element was not found on edit mode for form 0 in coeform 4010");
                        }

                        XmlNode editModeIdentifierList = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='IdentifierList']", manager);
                        if (editModeIdentifierList != null)
                        {
                            if (editModeIdentifierList.InnerXml.Contains("<Id>IdentifiersUltraGrid</Id>"))
                            {
                                editModeIdentifierList.InnerXml = editModeIdentifierList.InnerXml.Replace("<Id>IdentifiersUltraGrid</Id>", "<Id>Mix_IdentifiersUltraGrid</Id>");
                                messages.Add("Sucessfully updated ids in identifier list form element on edit mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in IdentifierList form element on edit mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("IdentifierList form element was not found on edit mode for form 0 in coeform 4010");
                        }

                        //-----------
                        XmlNode viewModeProjects = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Projects']", manager);
                        if (viewModeProjects != null)
                        {
                            if (viewModeProjects.InnerXml.Contains("VMix_"))
                            {
                                viewModeProjects.InnerXml = viewModeProjects.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in projects form element on view mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on view mode for form 0 in coeform 4010");
                            }

                            if (viewModeProjects.InnerXml.Contains(">ProjectsCslaDataSource") || viewModeProjects.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                viewModeProjects.InnerXml = viewModeProjects.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in projects form element on view mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project form element on view mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on view mode for form 0 in coeform 4010");
                        }

                        XmlNode viewModeIdentifiers = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiers != null)
                        {
                            if (viewModeIdentifiers.InnerXml.Contains("VMix_"))
                            {
                                viewModeIdentifiers.InnerXml = viewModeIdentifiers.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 0 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 0 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4010");
                        }

                        //-----------
                        XmlNode addModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        if (addModeIdentifiersF1 != null)
                        {
                            if (addModeIdentifiersF1.InnerXml.Contains("VCompound_"))
                            {
                                addModeIdentifiersF1.InnerXml = addModeIdentifiersF1.InnerXml.Replace("VCompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on add mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4010");
                        }

                        //-----------
                        XmlNode editModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                        if (editModeIdentifiersF1 != null)
                        {
                            if (editModeIdentifiersF1.InnerXml.Contains("VCompound_"))
                            {
                                editModeIdentifiersF1.InnerXml = editModeIdentifiersF1.InnerXml.Replace("VCompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on edit mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on edit mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4010");
                        }

                        //-----------
                        XmlNode viewModeIdentifiersF1 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifier']", manager);
                        if (viewModeIdentifiersF1 != null)
                        {
                            if (viewModeIdentifiersF1.InnerXml.Contains("VCompound_"))
                            {
                                viewModeIdentifiersF1.InnerXml = viewModeIdentifiersF1.InnerXml.Replace("VCompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 1 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 1 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4010");
                        }

                        //-----------
                        XmlNode addModeProjectsF4 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:addMode/COE:formElement[@name='Projects']", manager);
                        if (addModeProjectsF4 != null)
                        {
                            if (addModeProjectsF4.InnerXml.Contains("AMix_"))
                            {
                                addModeProjectsF4.InnerXml = addModeProjectsF4.InnerXml.Replace("AMix_", "Batch_");
                                messages.Add("Sucessfully updated ids in project form element on add mode for form 4 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on add mode for form 4 in coeform 4010");
                            }

                            if (addModeProjectsF4.InnerXml.Contains(">ProjectsCslaDataSource") || addModeProjectsF4.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                addModeProjectsF4.InnerXml = addModeProjectsF4.InnerXml.Replace(">ProjectsCslaDataSource", ">BatchProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"BatchProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in project form element on add mode for form 4 in coeform 4010");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project form element on add mode for form 4 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on add mode for form 4 in coeform 4010");
                        }

                        XmlNode addModeIdentifiersF4 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        if (addModeIdentifiersF4 != null)
                        {
                            if (addModeIdentifiersF4.InnerXml.Contains("AMix_"))
                            {
                                addModeIdentifiersF4.InnerXml = addModeIdentifiersF4.InnerXml.Replace("AMix_", "Batch_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 4 in coeform 4010");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on add mode for form 4 in coeform 4010");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 4 in coeform 4010");
                        }
                        break;
                    #endregion
                    #region Form 4011
                    case "4011":
                        XmlNode addModeIdentifiersF14011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        if (addModeIdentifiersF14011 != null)
                        {
                            if (addModeIdentifiersF14011.InnerXml.Contains("ACompound"))
                            {
                                addModeIdentifiersF14011.InnerXml = addModeIdentifiersF14011.InnerXml.Replace("ACompound", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on add mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4011");
                        }

                        //-----------
                        XmlNode editModeIdentifiersF14011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                        if (editModeIdentifiersF14011 != null)
                        {
                            if (editModeIdentifiersF14011.InnerXml.Contains("ACompound"))
                            {
                                editModeIdentifiersF14011.InnerXml = editModeIdentifiersF14011.InnerXml.Replace("ACompound", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on edit mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on edit mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4011");
                        }

                        //-----------
                        XmlNode viewModeIdentifiersF14011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF14011 != null)
                        {
                            if (viewModeIdentifiersF14011.InnerXml.Contains("ACompound"))
                            {
                                viewModeIdentifiersF14011.InnerXml = viewModeIdentifiersF14011.InnerXml.Replace("ACompound", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 1 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 1 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 1 in coeform 4011");
                        }

                        //-----------
                        XmlNode viewModeIdentifiersF24011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF24011 != null)
                        {
                            if (viewModeIdentifiersF24011.InnerXml.Contains("ACompound"))
                            {
                                viewModeIdentifiersF24011.InnerXml = viewModeIdentifiersF24011.InnerXml.Replace("ACompound", "Compound");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 2 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 2 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 2 in coeform 4011");
                        }

                        //-----------
                        XmlNode editModeProjectsF04011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Projects']", manager);
                        if (editModeProjectsF04011 != null)
                        {
                            if (editModeProjectsF04011.InnerXml.Contains("EMix"))
                            {
                                editModeProjectsF04011.InnerXml = editModeProjectsF04011.InnerXml.Replace("EMix", "Mix_");
                                messages.Add("Sucessfully updated ids in Projects form element on edit mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in Projects form element on edit mode for form 0 in coeform 4011");
                            }

                            if (editModeProjectsF04011.InnerXml.Contains(">ProjectsCslaDataSource") || editModeProjectsF04011.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                editModeProjectsF04011.InnerXml = editModeProjectsF04011.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in Projects form element on edit mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in Projects form element on edit mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on edit mode for form 0 in coeform 4011");
                        }

                        XmlNode editModeIdentifiersF04011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                        if (editModeIdentifiersF04011 != null)
                        {
                            if (editModeIdentifiersF04011.InnerXml.Contains("<Id>EMixIdentifiersUltraGrid</Id>"))
                            {
                                editModeIdentifiersF04011.InnerXml = editModeIdentifiersF04011.InnerXml.Replace("<Id>EMixIdentifiersUltraGrid</Id>", "<Id>Mix_IdentifiersUltraGrid</Id>");
                                messages.Add("Sucessfully updated ids in identifier list form element on edit mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in IdentifierList form element on edit mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("IdentifierList form element was not found on edit mode for form 0 in coeform 4011");
                        }

                        //-----------
                        XmlNode viewModeProjectsF04011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Projects']", manager);
                        if (viewModeProjectsF04011 != null)
                        {
                            if (viewModeProjectsF04011.InnerXml.Contains("VMix"))
                            {
                                viewModeProjectsF04011.InnerXml = viewModeProjectsF04011.InnerXml.Replace("VMix", "Mix");
                                messages.Add("Sucessfully updated ids in projects form element on view mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on view mode for form 0 in coeform 4011");
                            }

                            if (viewModeProjectsF04011.InnerXml.Contains(">ProjectsCslaDataSource") || viewModeProjectsF04011.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                viewModeProjectsF04011.InnerXml = viewModeProjectsF04011.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in projects form element on view mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project form element on view mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on view mode for form 0 in coeform 4011");
                        }

                        XmlNode viewModeIdentifiersF04011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF04011 != null)
                        {
                            if (viewModeIdentifiersF04011.InnerXml.Contains("VMix_"))
                            {
                                viewModeIdentifiersF04011.InnerXml = viewModeIdentifiersF04011.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 0 in coeform 4011");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 0 in coeform 4011");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4011");
                        }

                        break;
                    #endregion
                    #region Form 4012
                    case "4012":
                        XmlNode addModeIdentifiersF14012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']", manager);
                        if (addModeIdentifiersF14012 != null)
                        {
                            if (addModeIdentifiersF14012.InnerXml.Contains("ACompound_"))
                            {
                                addModeIdentifiersF14012.InnerXml = addModeIdentifiersF14012.InnerXml.Replace("ACompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on add mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on add mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on add mode for form 1 in coeform 4012");
                        }

                        //-----------
                        XmlNode editModeIdentifiersF14012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']", manager);
                        if (editModeIdentifiersF14012 != null)
                        {
                            if (editModeIdentifiersF14012.InnerXml.Contains("ECompound_"))
                            {
                                editModeIdentifiersF14012.InnerXml = editModeIdentifiersF14012.InnerXml.Replace("ECompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on edit mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on edit mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on edit mode for form 1 in coeform 4012");
                        }

                        //-----------
                        XmlNode viewModeIdentifiersF14012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF14012 != null)
                        {
                            if (viewModeIdentifiersF14012.InnerXml.Contains("VCompound_"))
                            {
                                viewModeIdentifiersF14012.InnerXml = viewModeIdentifiersF14012.InnerXml.Replace("VCompound_", "Compound_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 1 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 1 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 1 in coeform 4012");
                        }

                        //-----------
                        XmlNode editModeProjectsF04012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Projects']", manager);
                        if (editModeProjectsF04012 != null)
                        {
                            if (editModeProjectsF04012.InnerXml.Contains("EMix_"))
                            {
                                editModeProjectsF04012.InnerXml = editModeProjectsF04012.InnerXml.Replace("EMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in Projects form element on edit mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in Projects form element on edit mode for form 0 in coeform 4012");
                            }

                            if (editModeProjectsF04012.InnerXml.Contains(">ProjectsCslaDataSource") || editModeProjectsF04012.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                editModeProjectsF04012.InnerXml = editModeProjectsF04012.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in Projects form element on edit mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in Projects form element on edit mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on edit mode for form 0 in coeform 4012");
                        }

                        XmlNode editModeIdentifiersF04012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifier']", manager);
                        if (editModeIdentifiersF04012 != null)
                        {
                            if (editModeIdentifiersF04012.InnerXml.Contains("EMix_"))
                            {
                                editModeIdentifiersF04012.InnerXml = editModeIdentifiersF04012.InnerXml.Replace("EMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifier list form element on edit mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in IdentifierList form element on edit mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("IdentifierList form element was not found on edit mode for form 0 in coeform 4012");
                        }

                        //-----------
                        XmlNode viewModeProjectsF04012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Projects']", manager);
                        if (viewModeProjectsF04012 != null)
                        {
                            if (viewModeProjectsF04012.InnerXml.Contains("VMix_"))
                            {
                                viewModeProjectsF04012.InnerXml = viewModeProjectsF04012.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in projects form element on view mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on view mode for form 0 in coeform 4012");
                            }

                            if (viewModeProjectsF04012.InnerXml.Contains(">ProjectsCslaDataSource") || viewModeProjectsF04012.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                viewModeProjectsF04012.InnerXml = viewModeProjectsF04012.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in projects form element on view mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project form element on view mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on view mode for form 0 in coeform 4012");
                        }

                        XmlNode viewModeIdentifiersF04012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF04012 != null)
                        {
                            if (viewModeIdentifiersF04012.InnerXml.Contains("VMix_"))
                            {
                                viewModeIdentifiersF04012.InnerXml = viewModeIdentifiersF04012.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 0 in coeform 4012");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 0 in coeform 4012");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 0 in coeform 4012");
                        }

                        //-----------
                        XmlNode viewModeF44012 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:viewMode", manager);
                        if (viewModeF44012 != null && string.IsNullOrEmpty(viewModeF44012.InnerXml))
                        {
                            string viewModeInner = @"<formElement name=""Projects"" xmlns=""COE.FormGroup"">
							<label xmlns=""COE.FormGroup"">Projects</label>
							<showHelp xmlns=""COE.FormGroup"">false</showHelp>
							<isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
							<pageComunicationProvider xmlns=""COE.FormGroup"" />
							<fileUploadBindingExpression xmlns=""COE.FormGroup"" />
							<helpText xmlns=""COE.FormGroup"" />
							<defaultValue xmlns=""COE.FormGroup"" />
							<bindingExpression xmlns=""COE.FormGroup"">ProjectList</bindingExpression>
							<Id xmlns=""COE.FormGroup"">Batch_ProjectsUltraGrid</Id>
							<displayInfo xmlns=""COE.FormGroup"">
							    <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
							    <visible xmlns=""COE.FormGroup"">true</visible>
							</displayInfo>
							<validationRuleList xmlns=""COE.FormGroup"" />
							<serverEvents xmlns=""COE.FormGroup"" />
							<clientEvents xmlns=""COE.FormGroup"" />
							<configInfo xmlns=""COE.FormGroup"">
							    <HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
							    <HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
							    <AddButtonCSS>AddButtonCSS</AddButtonCSS>
							    <RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
							    <RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
							    <RowStyleCSS>RowStyleCSS</RowStyleCSS>
							    <SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
							    <fieldConfig>
							      <CSSLabelClass>FELabel</CSSLabelClass>
							      <AddRowTitle>Add Project</AddRowTitle>
							      <RemoveRowTitle>Remove Project</RemoveRowTitle>
							      <ReadOnly>true</ReadOnly>
							      <DefaultEmptyRows>1</DefaultEmptyRows>
							      <tables>
								    <table>
								      <Columns>
									    <Column name=""ID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" />
									    <Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""BatchProjectsCslaDataSource"">
									      <formElement xmlns=""COE.FormGroup"">
										    <Id xmlns=""COE.FormGroup"">Batch_ProjectDropDownList</Id>
										    <label xmlns=""COE.FormGroup"">Project</label>
										    <bindingExpression xmlns=""COE.FormGroup"">ProjectID</bindingExpression>
										    <configInfo xmlns=""COE.FormGroup"">
										      <fieldConfig>
											    <CSSClass>FEDropDownListGrid</CSSClass>
											    <DataSourceID>BatchProjectsCslaDataSource</DataSourceID>
											    <DataTextField>Name</DataTextField>
											    <DataValueField>ProjectID</DataValueField>
											    <ID>Inner_Batch_ProjectDropDownList</ID>
											    <Columns>
											      <Column key=""ProjectID"" title=""Project ID"" visible=""false"" />
											      <Column key=""Name"" title=""Name"" />
											      <Column key=""Description"" title=""Description"" />
											    </Columns>
										      </fieldConfig>
										    </configInfo>
										    <displayInfo xmlns=""COE.FormGroup"">
										      <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
										    </displayInfo>
									      </formElement>
									    </Column>
									    <Column name=""Name"" />
								      </Columns>
								    </table>
							      </tables>
							      <ClientSideEvents>
								    <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(ProjectID)}</Event>
							      </ClientSideEvents>
							    </fieldConfig>
							</configInfo>
							<dataSource xmlns=""COE.FormGroup"" />
							<dataSourceId xmlns=""COE.FormGroup"" />
							<displayData xmlns=""COE.FormGroup"" />
						</formElement>";
                            viewModeF44012.InnerXml = viewModeInner;
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("viewMode tag of the form 4 in coeform 4012 was not empty as expected. No change was made.");
                        }

                        break;
                    #endregion
                    #region Form 4013
                    case "4013":
                        //-----------
                        XmlNode viewModeProjectsF94013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='9']/COE:viewMode/COE:formElement[@name='Projects']", manager);
                        if (viewModeProjectsF94013 != null)
                        {
                            if (viewModeProjectsF94013.InnerXml.Contains("VMix_"))
                            {
                                viewModeProjectsF94013.InnerXml = viewModeProjectsF94013.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in projects form element on view mode for form 9 in coeform 4013");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in project form element on view mode for form 9 in coeform 4013");
                            }

                            if (viewModeProjectsF94013.InnerXml.Contains(">ProjectsCslaDataSource") || viewModeProjectsF94013.InnerXml.Contains("\"ProjectsCslaDataSource"))
                            {
                                viewModeProjectsF94013.InnerXml = viewModeProjectsF94013.InnerXml.Replace(">ProjectsCslaDataSource", ">RegistryProjectsCslaDataSource").Replace("\"ProjectsCslaDataSource", "\"RegistryProjectsCslaDataSource");
                                messages.Add("Sucessfully updated datasources in projects form element on view mode for form 9 in coeform 4013");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project form element on view mode for form 9 in coeform 4013");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was not found on view mode for form 9 in coeform 4013");
                        }

                        XmlNode viewModeIdentifiersF94013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='9']/COE:viewMode/COE:formElement[@name='Identifiers']", manager);
                        if (viewModeIdentifiersF94013 != null)
                        {
                            if (viewModeIdentifiersF94013.InnerXml.Contains("VMix_"))
                            {
                                viewModeIdentifiersF94013.InnerXml = viewModeIdentifiersF94013.InnerXml.Replace("VMix_", "Mix_");
                                messages.Add("Sucessfully updated ids in identifiers form element on view mode for form 9 in coeform 4013");
                            }
                            else
                            {
                                messages.Add("WARNING: Ids where already changed in identifiers form element on view mode for form 9 in coeform 4013");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Identifiers form element was not found on view mode for form 9 in coeform 4013");
                        }

                        XmlNode viewModeF24013 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode", manager);
                        if (viewModeF24013.SelectSingleNode("./COE:formElement[@name='Projects']", manager) == null)
                        {
                            XmlNode projectsFormElement = doc.CreateElement("COE", "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute name = doc.CreateAttribute("name");
                            name.Value = "Projects";
                            projectsFormElement.Attributes.Append(name);
                            projectsFormElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Projects</label>
                                <showHelp xmlns=""COE.FormGroup"">false</showHelp>
                                <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
                                <pageComunicationProvider xmlns=""COE.FormGroup"" />
                                <fileUploadBindingExpression xmlns=""COE.FormGroup"" />
                                <helpText xmlns=""COE.FormGroup"" />
                                <defaultValue xmlns=""COE.FormGroup"" />
                                <bindingExpression xmlns=""COE.FormGroup"">this.Duplicates.Current.BatchList[0].ProjectList</bindingExpression>
                                <Id xmlns=""COE.FormGroup"">Batch_ProjectsUltraGrid</Id>
                                <displayInfo xmlns=""COE.FormGroup"">
                                    <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGridUltra</type>
                                    <visible xmlns=""COE.FormGroup"">true</visible>
                                </displayInfo>
                                <validationRuleList xmlns=""COE.FormGroup"" />
                                <serverEvents xmlns=""COE.FormGroup"" />
                                <clientEvents xmlns=""COE.FormGroup"" />
                                <configInfo xmlns=""COE.FormGroup"">
                                    <HeaderStyleCSS>HeaderStyleCSS</HeaderStyleCSS>
                                    <HeaderHorizontalAlign>Center</HeaderHorizontalAlign>
                                    <AddButtonCSS>AddButtonCSS</AddButtonCSS>
                                    <RemoveButtonCSS>RemoveButtonCSS</RemoveButtonCSS>
                                    <RowAlternateStyleCSS>RowAlternateStyleCSS</RowAlternateStyleCSS>
                                    <RowStyleCSS>RowStyleCSS</RowStyleCSS>
                                    <SelectedRowStyleCSS>RowSelectedStyleCSS</SelectedRowStyleCSS>
                                    <fieldConfig>
                                      <AddRowTitle>Add Project</AddRowTitle>
                                      <RemoveRowTitle>Remove Project</RemoveRowTitle>
                                      <ReadOnly>false</ReadOnly>
                                      <DefaultEmptyRows>1</DefaultEmptyRows>
                                      <tables>
                                        <table>
                                          <Columns>
                                            <Column name=""ProjectID"" visible=""false"" columnType=""Custom"" defaultValue=""0"" />
                                            <Column name=""Project"" dataTextValueField=""Name"" dataSourceID=""BatchProjectsCslaDataSource"">
                                              <formElement xmlns=""COE.FormGroup"">
                                                <Id xmlns=""COE.FormGroup"">Batch_ProjectDropDownList</Id>
                                                <label xmlns=""COE.FormGroup"">Project</label>
                                                <bindingExpression xmlns=""COE.FormGroup"">ProjectID</bindingExpression>
                                                <configInfo xmlns=""COE.FormGroup"">
                                                  <fieldConfig>
                                                    <CSSClass>FEDropDownListGrid</CSSClass>
                                                    <CSSLabelClass>FELabel</CSSLabelClass>
                                                    <DataSourceID>BatchProjectsCslaDataSource</DataSourceID>
                                                    <DataTextField>Name</DataTextField>
                                                    <DataValueField>ProjectID</DataValueField>
                                                    <ID>Inner_B_ProjectDropDownList</ID>
                                                    <Columns>
                                                      <Column key=""ProjectID"" title=""Project ID"" visible=""false"" />
                                                      <Column key=""Name"" title=""Name"" />
                                                      <Column key=""Description"" title=""Description"" />
                                                    </Columns>
                                                  </fieldConfig>
                                                </configInfo>
                                                <displayInfo xmlns=""COE.FormGroup"">
                                                  <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra</type>
                                                </displayInfo>
                                              </formElement>
                                            </Column>
                                            <Column name=""Name"" />
                                          </Columns>
                                        </table>
                                      </tables>
                                      <ClientSideEvents>
                                        <Event name=""BeforeEnterEdit"">{CustomJS_FilterByUnique(ProjectID)}</Event>
                                      </ClientSideEvents>
                                    </fieldConfig>
                                </configInfo>
                                <dataSource xmlns=""COE.FormGroup"" />
                                <dataSourceId xmlns=""COE.FormGroup"" />
                                <requiredStyle xmlns=""COE.FormGroup"" />
                                <displayData xmlns=""COE.FormGroup"" />";
                            viewModeF24013.InsertBefore(projectsFormElement, viewModeF24013.FirstChild);
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Projects form element was already present in form 2 of coeform 4013 in viewMode");
                        }
                        break;
                    #endregion
                    #region Form 4014
                    case "4014":

                        XmlNode viewModeProjectColumnF24014 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']//COE:Column[@name='Project']", manager);
                        if (viewModeProjectColumnF24014 != null)
                        {
                            if (viewModeProjectColumnF24014.InnerXml.Contains("<DataSourceID>ProjectsCslaDataSource</DataSourceID>"))
                            {
                                viewModeProjectColumnF24014.InnerXml = viewModeProjectColumnF24014.InnerXml.Replace("<DataSourceID>ProjectsCslaDataSource</DataSourceID>", "<DataSourceID>BatchProjectsCslaDataSource</DataSourceID>");
                                messages.Add("Sucessfully updated datasources in project Column on add mode for form 2 in coeform 4014");
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ProjectsCslaDataSource was already changed in project Column on add mode for form 2 in coeform 4014");
                            }
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Project Column was not found on add mode for form 2 in coeform 4014");
                        }

                        XmlNode compoundIdentifiersIDF04014 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:viewMode//COE:Id[text()='VCompound_IdentifiersUltraGrid']", manager);
                        if (compoundIdentifiersIDF04014 != null)
                        {
                            compoundIdentifiersIDF04014.InnerText = "Compound_IdentifiersUltraGrid";
                            messages.Add("Compound identifier webgrid's id successfully changed in form 2 for coeform 4014");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Compound identifier webgrid's id was already changed in form 2 for coeform 4014");
                        }
                        break;
                    #endregion
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("UnifyingWebGridsIdsForPageControlSettings was successfully patched");
            }
            else
                messages.Add("UnifyingWebGridsIdsForPageControlSettings was patched with errors");
            return messages;
        }
	}
}

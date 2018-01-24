using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;

using Csla;
using Csla.Data;
using Csla.Validation;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    [System.ComponentModel.TypeConverter(typeof(ProjectListConverter))]
    public class ProjectList :
        RegistrationBusinessListBase<ProjectList, Project>, IKeyValueListHolder
    {
        private const string PROJECTLIST_MANAGERNAME = "Default Cache Manager";
        private const string PROJECTLIST_CACHEKEY = "ProjectsByType.{0}";
        private const CacheItemPriority CACHE_PRIORITY = CacheItemPriority.Normal;
        private static SlidingTime CacheExpiry = new SlidingTime(TimeSpan.FromSeconds(60));

        #region Factory Methods

        internal static ProjectList NewProjectList()
        {
            return new ProjectList();
        }

        [COEUserActionDescription("CreateProjectList")]
        public static ProjectList NewProjectList(string xml, bool isClean, bool isNew)
        {
            try
            {
                return new ProjectList(xml, isClean,isNew);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
        
        private ProjectList()
        {
            //MarkAsChild();
        }

        private ProjectList(string xml, bool isClean, bool isNew) 
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("ProjectList/Project");
            if(xIterator.MoveNext()) 
            {
                do {
                    this.Add(Project.NewProject(xIterator.Current.OuterXml, isClean,isNew));
                } while(xIterator.Current.MoveToNext());
            }
        }

        [COEUserActionDescription("GetProjectList")]
        public static ProjectList GetProjectList()
        {
            try
            {
                //if (!CanGetObject())
                //{
                //    throw new System.Security.SecurityException("User not authorized to view a ProjectList");
                //}
                return DataPortal.Fetch<ProjectList>(new Criteria());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetActiveProjectList")]
        public static ProjectList GetActiveProjectListByPersonID(int personID)
        {
            try
            {
                return DataPortal.Fetch<ProjectList>(new ActiveCriteria(personID));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetActiveProjectListByType")]
        public static ProjectList GetActiveProjectListByPersonIDAndType(int personID, ProjectTypeEnum type)
        {
            try
            {
                return GetActiveProjectListByPersonIDAndType(personID, type, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetActiveProjectListByType")]
        public static ProjectList GetActiveProjectListByPersonIDAndType(int personId, ProjectTypeEnum type, bool useCache)
        {
            ProjectList projectList = null;

            try
            {
                if (useCache)
                {
                    string cacheKey = string.Empty;
                    CacheManager manager = CacheFactory.GetCacheManager(PROJECTLIST_MANAGERNAME);

                    cacheKey = string.Format(PROJECTLIST_CACHEKEY, type.ToString());
                    projectList = manager.GetData(cacheKey) as ProjectList;
                    if (projectList == null)
                    {
                        projectList = DataPortal.Fetch<ProjectList>(new ActiveCriteriaByType(personId, type));
                        manager.Add(cacheKey, projectList, CACHE_PRIORITY, null, CacheExpiry);
                    }
                }
                else
                {
                    projectList = DataPortal.Fetch<ProjectList>(new ActiveCriteriaByType(personId, type));
                }

                return projectList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetActiveProjectListByType")]
        public static ProjectList GetActiveProjectListByPersonIDAndType(int personId, string type, bool useCache)
        {

            ProjectList projectList = null;
            try
            {
                ProjectTypeEnum translatedType = (ProjectTypeEnum)Enum.Parse(typeof(ProjectTypeEnum), type);
                projectList = GetActiveProjectListByPersonIDAndType(personId, translatedType, useCache);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
            return projectList;
        }

        #endregion

        [Serializable()]
        protected class Criteria
        {
            public Criteria() { }
        }

        [Serializable()]
        protected class ActiveCriteria
        {
            private int _id;
            public int PersonID { get { return _id; } }

            public ActiveCriteria(int personID) { _id = personID;}
        }

        [Serializable()]
        protected class ActiveCriteriaByType
        {
            private int _id;
            public int PersonID { get { return _id; } }

            private ProjectTypeEnum _type;
            public ProjectTypeEnum Type { get { return _type; } }

            public ActiveCriteriaByType(int personID,ProjectTypeEnum type) 
            {
                _id = personID;
                _type = type;
            }
        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
            using (SafeDataReader dr = this.RegDal.GetProjectList())
            {
                this.Fetch(dr);
            }
        }

        protected void DataPortal_Fetch(ActiveCriteria criteria)
        {
            using (SafeDataReader dr = this.RegDal.GetActiveProjectListByPerson(criteria.PersonID))
            {
                this.Fetch(dr);
            }
        }

        protected void DataPortal_Fetch(ActiveCriteriaByType criteria)
        {
            using (SafeDataReader dr = this.RegDal.GetActiveProjectListByPersonAndType(criteria.PersonID,Char.Parse(criteria.Type.ToString())))
            {
                this.Fetch(dr);
            }
        }

        private void Fetch(SafeDataReader reader)
        {
            while (reader.Read())
            {
                this.Add(Project.NewProject(0, reader.GetInt32("PROJECTID"),
                                                reader.GetString("NAME"),
                                                reader.GetString("ACTIVE") == "T" ? true : false,
                                                reader.GetString("DESCRIPTION"),
                                                reader.GetString("TYPE")
                                                ));
            }
        }

        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the ProjectList node</param>
        internal void UpdateFromXml(XmlNode parentNode)
        {
            if (parentNode != null)
            {
                XmlNodeList nodes = parentNode.SelectNodes("Project");
                foreach (XmlNode node in nodes)
                {
                    XmlNode idNode = node.SelectSingleNode("ID");
                    if (idNode == null || idNode.InnerText == "0" || idNode.InnerText == string.Empty)
                    {
                        //if there's no ID yet, we're new

                        this.Add(Project.NewProject(node.OuterXml, false, true));
                    }
                    else
                    {
                        //if there's an ID, we will check for update
                        //loop over the existing projects
                        foreach (Project p in this)
                        {
                            XmlNode matchingChild =
                                parentNode.SelectSingleNode(string.Format("Project[ID='{0}']", p.ID));

                            //if a node matches this ID, we should update the matching object
                            if (matchingChild != null)
                                p.UpdateFromXml(matchingChild);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the ProjectList node</param>
        internal void UpdateUserPreference(XmlNode parentNode,ProjectTypeEnum projectType)
        {
            if (parentNode != null && this.Count == 1 && this[0].ProjectID==0)
            {
                XmlNodeList nodes = parentNode.SelectNodes("Project");
                ProjectList availableProjects = ProjectList.GetActiveProjectListByPersonIDAndType(COEUser.ID, projectType);
                foreach (XmlNode node in nodes)
                {
                    
                    XmlNode idNode = node.SelectSingleNode("ProjectID");
                    if (idNode != null && !string.IsNullOrEmpty(idNode.InnerText))
                    {
                        bool bProjectExits = false;
                        string projectid = idNode.InnerText; // Null check already done 
                        foreach (Project currentProject in availableProjects)
                        {
                            if (currentProject.ProjectID.ToString() == projectid) // Need to check with ID and not Unique id
                            {
                                bProjectExits = true;
                                break;
                            }
                        }
                        if (bProjectExits)
                            this.Add(Project.NewProject(node.OuterXml, false, true));
                        else
                        {
                            throw new ValidationException("You have an inactive project selected in your user preferences. Please update your preferences and try again.");
                        }
                    }
                }
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            bool showUpdateTag = false;

            foreach(Project currentProject in this)
                if(currentProject.IsDirty && !currentProject.IsNew && !currentProject.IsDeleted)
                    showUpdateTag = true;

            StringBuilder builder = new StringBuilder(string.Empty);
            builder.Append(string.Format("<ProjectList{0}>", addCRUDattributes && this.IsDirty && showUpdateTag ? " update=\"yes\"" : string.Empty));

            foreach(Project currentProject in this)
                builder.Append(currentProject.UpdateSelf(addCRUDattributes));

            foreach (Project currentDeletedProject in this.DeletedList)
            {
                //CSBR:143968 for the case where the record has never been committed to the database, but has identifiers that are
                //deleted such as loading a template, simply remove them from the xml
                if (!currentDeletedProject.IsNew)
                {
                    builder.Append(currentDeletedProject.UpdateSelf(true));
                }
            }
            builder.Append("</ProjectList>");

            return builder.ToString();
        }

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="component">Component to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetProjectIndex")]
        public int GetIndex(Project project)
        {
            int retVal = 0;
            try
            {
                if (this.Contains(project))
                {
                    foreach (Project currentProject in this)
                    {
                        retVal++;
                        if (currentProject.UniqueID == project.UniqueID)
                            break;
                    }
                }
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return retVal;
            }
        }

        public enum ProjectTypeEnum 
        {
            /// <summary>
            /// All project type
            /// </summary>
            A,
            /// <summary>
            /// Registry project type
            /// </summary>
            R,
            /// <summary>
            /// Batch project type
            /// </summary>
            B
        }

        #region IKeyValueListHolder Members

        public System.Collections.IDictionary KeyValueList
        {
            [COEUserActionDescription("GetProjectDataPairList")]
            get
            {
                try
                {
                    System.Collections.Specialized.HybridDictionary dictionary = new System.Collections.Specialized.HybridDictionary(this.Count);
                    foreach (Project project in this)
                        dictionary.Add(project.ProjectID, project.Name);
                    return dictionary;
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return null;
                }
            }
        }

        #endregion

    }
}

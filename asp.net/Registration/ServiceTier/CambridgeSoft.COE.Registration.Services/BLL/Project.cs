using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class Project : BusinessBase<Project>
    {
        private List<string> _changedProperties = new List<string>();

        private int _id;
        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                if(_id != value) {
                    _id = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _projectID;
        public int ProjectID
        {
            get
            {
                CanReadProperty(true);
                return _projectID;
            }
            set
            {
                if (_projectID != value)
                {
                    _projectID = value;
                    PropertyHasChanged();
                }
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                CanReadProperty(true);
                return _description;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (_description != value)
                {
                    _description = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                CanReadProperty(true);
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (_name != value)
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        private bool _active = true;
        public bool Active
        {
            get 
            { 
                CanReadProperty(true);
                return _active; 
            }
            set 
            {
                CanWriteProperty(true);
                if (_active != value)
                {
                    _active = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _type = string.Empty;
        /// <summary>
        /// Gets or sets the type of the project.
        /// </summary>
        /// <value>The type.</value>
        /// <example>B means to be related with Batches and R with Registry</example>
        public string Type
        {
            get
            {
                CanReadProperty(true);
                return _type;
            }
            set
            {
                CanWriteProperty(true);
                _type = value;
                PropertyHasChanged();
            }
        }

        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public int OrderIndex
        {
            get
            {
                CanReadProperty(true);
                return ((ProjectList)base.Parent).GetIndex(this);
            }
        }

        /// <summary>
        /// Identifier for a batchcomponent object. 
        /// The ID is not enough when you are creating a new component and the ID is null.
        /// </summary>
        [Browsable(false)]
        public string UniqueID
        {
            get
            {
                return _id.ToString() + "|" + _name.ToString();
            }
        }

        public override bool IsValid
        {
            get { return base.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty; }
        }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 500));

            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 250));
        }

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Project", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
        }

        public static bool CanEditObject()
        {
            return true;
        }

        #region Factory Methods

        [COEUserActionDescription("CreateProject")]
        public static Project NewProject(string xml, bool isClean, bool isNew)
        {
            try
            {
                return new Project(xml, isClean,isNew);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Creates a new project object
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="projectID">The project ID.</param>
        /// <param name="name">The name.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The type of project to be applied to (E.g. b means Batches)</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("CreateProject")]
        public static Project NewProject(int id, int projectID, string name, bool active, string description, string type)
        {
            try
            {
                return new Project(id, projectID, name, active, description, type); 
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateProject")]
        public static Project NewProject()
        {
            try
            {
                return new Project();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private Project()
        {
            MarkAsChild(); 
        }

        private Project(string xml, bool isClean, bool isNew) : this() 
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            
            XPathNodeIterator xIterator = xNavigator.Select("Project/ProjectID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _projectID = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Project/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Project/ProjectID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Name", string.Empty)))
                    _name = xIterator.Current.GetAttribute("Name", string.Empty);

            xIterator = xNavigator.Select("Project/ProjectID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Active", string.Empty)))
                    _active = xIterator.Current.GetAttribute("Active", string.Empty).ToUpper() == "T" ? true : false;

            xIterator = xNavigator.Select("Project/ProjectID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Description", string.Empty)))
                    _description = xIterator.Current.GetAttribute("Description", string.Empty);

            if(isClean)
                MarkClean();
            if (!isNew)
                MarkOld();
        }

        private Project(int id, int projectID, string name, bool active, string description, string type) : this() 
        {
            _id = id;
            _name = name;
            _active = active;
            _description = description;
            _projectID = projectID;
            _type = type;
        }

        #endregion

        /// <summary>
        /// Allows updating from an Xml Node
        /// </summary>
        /// <param name="parentNode">the Project node</param>
        internal void UpdateFromXml(XmlNode parentNode)
        {
            //TODO: Verify this method
            Project p = Project.NewProject(parentNode.OuterXml, true, true);
            //this.ProjectID = p.ID;
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            if (this._projectID > 0)
            {
                // Project
                builder.Append("<Project");
                if (addCRUDattributes && this.IsNew)
                    builder.Append(" insert=\"yes\"");
                if (addCRUDattributes && this.IsDeleted)
                    builder.Append(" delete=\"yes\"");
                builder.Append(">");

                // ID
                if (this.ID > 0)
                    builder.Append("<ID>" + this.ID + "</ID>");

                // ProjectID
                builder.Append("<ProjectID");
                if (addCRUDattributes && this.IsDirty && !this.IsNew && !this.IsDeleted)
                    builder.Append(" update=\"yes\"");
                builder.Append(">");
                builder.Append(this._projectID + "</ProjectID>");

                // OrderIndex
                builder.Append("<OrderIndex>" + this.OrderIndex.ToString() + "</OrderIndex>");

                builder.Append("</Project>");
            }
            return builder.ToString();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Registration;
using Csla;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    [Serializable()]
    public class AddIn : BusinessBase<AddIn>
    {
        #region Variables

        private int _id;
        private EventList _eventList;
        private string _addInConfiguration;
        private string _assembly;
        private string _className;
        private string _classNameSpace;
        //private string _library;
        private bool _enable;
        private bool _required;
        private bool _confIsChange;
        private string _friendlyName;

        #endregion

        #region Properties

        public string ClassNameSpace
        {
            get
            {
                return _classNameSpace;
            }
        }

        public int Id
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {

                CanWriteProperty(true);
                _id = value;
                PropertyHasChanged();

            }
        }

        public EventList EventList
        {
            get
            {
                CanReadProperty(true);
                return _eventList;
            }
            set
            {
                if (value == null)
                {
                    CanWriteProperty(true);
                    _eventList = value;
                    PropertyHasChanged();
                }
            }
        }

        public string AddInConfiguration
        {
            get
            {
                CanReadProperty(true);
                return _addInConfiguration;
            }

            set
            {
                if (_addInConfiguration != value)
                {
                    CanWriteProperty(true);
                    _addInConfiguration = value;
                    PropertyHasChanged();
                    _confIsChange = true;
                }
            }
        }

        public string Assembly
        {
            get
            {
                CanReadProperty(true);
                return _assembly;
            }
            set
            {
                if (_assembly != value)
                {
                    CanWriteProperty(true);
                    _assembly = value;
                    PropertyHasChanged();
                }
            }
        }

        public string ClassName
        {
            get
            {
                CanReadProperty(true);
                return _className;
            }
            set
            {
                if (_className != value)
                {
                    CanWriteProperty(true);
                    _className = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsRequired
        {
            get
            {
                CanReadProperty(true);
                return _required;
            }
        }

        public bool IsEnable
        {
            get
            {
                CanReadProperty(true);
                return _enable;
            }

            set
            {
                CanWriteProperty(true);
                _enable = value;
                PropertyHasChanged();
            }
        }

        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || _eventList.IsDirty;
            }
        }

        public string FriendlyName
        {
            get
            {
                CanReadProperty(true);
                return _friendlyName;
            }
        }

        #endregion

        #region Constructors

        private AddIn(XmlNode xmlNode, bool isNew)
        {
            _assembly = xmlNode.Attributes["assembly"].Value;
            _className = xmlNode.Attributes["class"].Value;

            if (xmlNode.Attributes["friendlyName"] != null)
                _friendlyName = xmlNode.Attributes["friendlyName"].Value;
            else
                _friendlyName = string.Empty;

            if (xmlNode.Attributes["enabled"] != null && xmlNode.Attributes["enabled"].Value == "no")
                _enable = false;
            else
                _enable = true;

            if (xmlNode.Attributes["required"] != null && xmlNode.Attributes["required"].Value == "no")
                _required = false;
            else
                _required = true;

            _eventList = EventList.NewEventList(xmlNode.SelectNodes("./Event"));
            if (xmlNode.SelectSingleNode("./AddInConfiguration") != null)
                _addInConfiguration = xmlNode.SelectSingleNode("./AddInConfiguration").OuterXml.ToString();
            else
                _addInConfiguration = string.Empty;

            if (isNew)
            {
                MarkNew();
            }
            else
            {
                MarkOld();
            }
            MarkClean();
            _confIsChange = false;

        }

        private AddIn(string assembly, string className, string frienlyName, EventList eventList, string configuration, string nameSpace, bool enabled, bool required)
        {
            _assembly = assembly;
            _className = className;
            _friendlyName = frienlyName;
            _classNameSpace = nameSpace;
            _eventList = eventList;
            _addInConfiguration = configuration;
            _enable = enabled;
            _required = required;
            _confIsChange = false;
            this.MarkNew();
            this.MarkDirty();

        }

        private AddIn()
        {
            _confIsChange = false;
        }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("CreateAddIn")]
        public static AddIn NewAddIn(XmlNode xmlNode, bool isNew)
        {
            try
            {
                return new AddIn(xmlNode, isNew);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("CreateAddIn")]
        public static AddIn NewAddIn(string assembly, string className, string friendlyName, EventList eventList, string configuration, string nameSpace, bool enable, bool required)
        {
            try
            {
                return new AddIn(assembly, className, friendlyName, eventList, configuration, nameSpace, enable, required);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        public static AddIn NewAddIn()
        {
            return new AddIn();
        }

        #endregion

        #region Business Method

        protected override object GetIdValue()
        {
            return _id;
        }

        [COEUserActionDescription("GetAddInXml")]
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<AddIn ");

            if (!IsDeleted && IsNew && _classNameSpace != null)                           
                builder.Append(" assembly=\"" + _assembly + "\" class=\"" + _classNameSpace + "." + _className + "\"");            
            else
                builder.Append(" assembly=\"" + _assembly + "\" class=\"" + _className + "\"");

            builder.Append(" friendlyName=\"" + _friendlyName + "\"");

            if (this.IsRequired)
                builder.Append(" required=\"yes\" ");
            else
                builder.Append(" required=\"no\" ");

            if (this.IsEnable)
                builder.Append(" enabled=\"yes\" ");
            else
                builder.Append(" enabled=\"no\" ");

            if (!IsDeleted && IsNew)
                builder.Append(" insert=\"yes\"");
            else if (IsDirty && !IsDeleted && !IsNew)
                builder.Append(" update=\"yes\"");
            else if (IsDeleted)
            {
                builder.Append(" delete=\"yes\"");
                builder.Append(">");
                builder.Append("</AddIn>");
                return builder.ToString();
            }

            builder.Append(">");
            builder.Append(this._eventList.UpdateSelf(IsNew));
            XmlDocument addInConfiguration = new XmlDocument();
            try
            {
                addInConfiguration.LoadXml((string.IsNullOrEmpty(_addInConfiguration)) ? "<AddInConfiguration />" : _addInConfiguration);
                if (_confIsChange)
                {
                    addInConfiguration.SelectSingleNode("/AddInConfiguration").Attributes.Append(addInConfiguration.CreateAttribute("update"));
                    addInConfiguration.SelectSingleNode("/AddInConfiguration").Attributes["update"].Value = "yes";
                }

                builder.Append(addInConfiguration.InnerXml.ToString());
            }
            catch(Exception ex) 
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            
            //builder.Append("<AddInConfiguration");
            //if(_confIsChange)
            //    builder.Append(" update=\"yes\" >");
            //else
            //    builder.Append(">");            
            //builder.Append(this.AddInConfiguration);
            //builder.Append("</AddInConfiguration>");
            builder.Append("</AddIn>");
            return builder.ToString();
        }

        public void MarkAsNew() 
        {
            this.MarkNew();
        }

        #endregion

    }
}

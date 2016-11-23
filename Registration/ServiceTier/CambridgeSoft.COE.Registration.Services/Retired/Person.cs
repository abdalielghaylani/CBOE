//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Xml;
//using System.Xml.XPath;
//using System.Data;
//using CambridgeSoft.COE.Registration;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Framework.COEConfigurationService;
//using Csla;
//using Csla.Data;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Services.Types
//{
//    [Serializable()]
//    public class Person : BusinessBase<Person>
//    {
//        #region Business Methods

//        private int _id;
//        private string _firstName;
//        private string _middleName;
//        private string _lastName;
//        private int _supervisor;
//        private string _code;
//        private string _title;
//        private string _department;
//        private int _site;
//        private string _address;
//        private string _phone;
//        private string _email;
//        private bool _active;


//        [System.ComponentModel.DataObjectField(true, true)]
//        public int ID
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _id;
//            }
//            set
//            {
//                _id = value;
//            }
//        }

//        protected override object GetIdValue()
//        {
//            return _id;
//        }

        
//        public override bool IsValid
//        {
//            //get { return base.IsValid && _resources.IsValid; }
//            get { return base.IsValid; }
//        }

//        public override bool IsDirty
//        {
//            get { return base.IsDirty; }
//            //get { return base.IsDirty || _resources.IsDirty; }
//        }
//        #endregion

//        #region Validation Rules

//        protected override void AddBusinessRules()
//        {
//            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
//            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
//        }

//        #endregion

//        #region Authorization Rules

//        protected override void AddAuthorizationRules()
//        {
//            AuthorizationRules.AllowWrite(
//              "Person", "ADD_IDENTIFIER");
//        }

//        public static bool CanAddObject()
//        {
//            //return Csla.ApplicationContext.User.IsInRole("person");
//            return true;
//        }

//        public static bool CanGetObject()
//        {
//            return true;
//        }

//        public static bool CanDeleteObject()
//        {
//            return true;
//            //return Csla.ApplicationContext.User.IsInRole("person");
//        }

//        public static bool CanEditObject()
//        {
//            return true;
//            //return Csla.ApplicationContext.User.IsInRole("person");
//        }

//        #endregion

//        #region Factory Methods

//        public static Person NewPerson()
//        {
//            Person person = new Person();

//            //person.Description = null;
//            //person.Value = null;

//            return person;
//        }

//        [COEUserActionDescription("CreatePerson")]
//        public static Person NewPerson(string xml, bool isClean)
//        {
//            try
//            {
//                return new Person(xml, isClean);
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
        
//        private Person()
//        {
//            _middleName = string.Empty;
//            _lastName = string.Empty;
//            _supervisor = 0;
//            _code = string.Empty;
//            _title = string.Empty;
//            _department = string.Empty;
//            _site = 0;
//            _address = string.Empty;
//            _phone = string.Empty;
//            _email = string.Empty;
//            _active = false;
//        }

//        private Person(string xml, bool isClean) {
//            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
//            XPathNavigator xNavigator = xDocument.CreateNavigator();

//            XPathNodeIterator xIterator = xNavigator.Select("Person/FirstName");
//            xIterator.MoveNext();
//            _firstName = xIterator.Current.Value;

//            if(isClean)
//                MarkClean();
//        }

//        [COEUserActionDescription("SavePerson")]
//        public override Person Save()
//        {
//            try
//            {
//                if (IsDeleted && !CanDeleteObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to remove a Person");
//                }
//                else if (IsNew && !CanAddObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to add a Person");
//                }
//                else if (!CanEditObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to update a Person");
//                }
//                return base.Save();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
//        #endregion

//        #region Xml

//        internal string UpdateSelf()
//        {
//            StringBuilder builder = new StringBuilder("");

//            builder.Append("<Person>");
//            builder.Append("<ID>" + this._id + "</ID>");
//            builder.Append("<FirstName>" + this._firstName + "</FirstName>");
//            builder.Append("<MiddleName>" + this._middleName + "</MiddleName>");
//            builder.Append("<LastName>" + this._lastName + "</LastName>");
//            builder.Append("<Supervisor>" + this._supervisor + "</Supervisor>");
//            builder.Append("<Code>" + this._code + "</Code>");
//            builder.Append("<Title>" + this._title + "</Title>");
//            builder.Append("<Department>" + this._department + "</Department>");
//            builder.Append("<Site>" + this._site + "</Site>");
//            builder.Append("<Address>" + this._address + "</Address>");
//            builder.Append("<Phone>" + this._phone + "</Phone>");
//            builder.Append("<Email>" + this._email + "</Email>");
//            builder.Append("<Active>" + this._active + "</Active>");
//            builder.Append("<PrivilegeList></PrivilegeList>");

//            builder.Append("</Person>");

//            return builder.ToString();
//        }

//        #endregion
    
//    }
//}


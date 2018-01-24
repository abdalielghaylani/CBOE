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
//    public class Notebook : BusinessBase<Notebook>
//    {
//        #region Business Methods

//        private int _id;
//        private string _name = string.Empty;
//        private string _description = string.Empty;
//        private bool _active = true;

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

//        public string Description
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _description;
//            }
//            set
//            {
//                CanWriteProperty(true);
//                if (value == null) value = string.Empty;
//                if (_description != value)
//                {
//                    _description = value;
//                    PropertyHasChanged();
//                }
//            }
//        }

//        public string Name
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _name;
//            }
//            set
//            {
//                CanWriteProperty(true);
//                _name = value;
//                PropertyHasChanged();
//            }
//        }

//        public bool Active
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _active;
//            }
//            set
//            {
//                CanWriteProperty(true);
//                _active = value;
//                PropertyHasChanged();
//            }
//        }

 
//        #endregion

//        #region Validation Rules

//        protected override void AddBusinessRules()
//        {
//            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
//            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 500));

//            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
//            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 250));
//        }

//        #endregion

//        #region Authorization Rules

//        protected override void AddAuthorizationRules()
//        {
//            AuthorizationRules.AllowWrite("Notebook", "ADD_IDENTIFIER");
//        }

//        public static bool CanAddObject()
//        {
//            return true;
//        }

//        public static bool CanGetObject()
//        {
//            return true;
//        }

//        public static bool CanDeleteObject()
//        {
//            return true;
//        }

//        public static bool CanEditObject()
//        {
//            return true;
//        }

//        #endregion

//        #region Factory Methods

//        [COEUserActionDescription("CreateNoteBook")]
//        public static Notebook NewNotebook(string xml, bool isClean)
//        {
//            try
//            {
//                return new Notebook(xml, isClean);
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        [COEUserActionDescription("CreateNoteBook")]
//        public static Notebook NewNotebook(int id, string name, bool active, string description)
//        {
//            try
//            {
//                return new Notebook(id, name, active, description); 
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        private Notebook()
//        {
//            MarkAsChild(); 
//        }

//        private Notebook(string xml, bool isClean)
//            : this() 
//        {
//            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
//            XPathNavigator xNavigator = xDocument.CreateNavigator();
            
//            XPathNodeIterator xIterator = xNavigator.Select("Notebook/NotebookID");
//            if (xIterator.MoveNext())
//                if (!string.IsNullOrEmpty(xIterator.Current.Value))
//                    _id = Convert.ToInt32(xIterator.Current.Value);

//            xIterator = xNavigator.Select("Notebook/NotebookID");
//            if (xIterator.MoveNext())
//                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Name", string.Empty)))
//                    _name = xIterator.Current.GetAttribute("Name", string.Empty);

//            xIterator = xNavigator.Select("Notebook/NotebookID");
//            if (xIterator.MoveNext())
//                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Active", string.Empty)))
//                    _active = xIterator.Current.GetAttribute("Active", string.Empty).ToUpper() == "T" ? true : false;

//            xIterator = xNavigator.Select("Notebook/NotebookID");
//            if (xIterator.MoveNext())
//                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Description", string.Empty)))
//                    _description = xIterator.Current.GetAttribute("Description", string.Empty);

//            if(isClean)
//                MarkClean();
//        }

//        private Notebook(int id, string name, bool active, string description)
//            : this() 
//        {
//            _id = id;
//            _name = name;
//            _active = active;
//            _description = description;
//        }

//        #endregion

//        #region Xml

//        internal string UpdateSelf(bool addCRUDattributes)
//        {
//            StringBuilder builder = new StringBuilder("");
//            if (this._id > 0)
//            {
//                builder.Append("<Notebook>");
//                builder.Append("<NotebookID>" + this._id + "</NotebookID>");
//                builder.Append("</Notebook>");
//            }
//            return builder.ToString();
//        }

//        #endregion
//    }
//}

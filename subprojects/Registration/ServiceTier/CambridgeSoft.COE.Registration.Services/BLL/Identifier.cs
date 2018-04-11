using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class Identifier : RegistrationBusinessBase<Identifier>
    {
        private List<string> _changedProperties = new List<string>();

        private int _identifierID;
        /// <summary>
        /// Identifier 
        /// </summary>
        public int IdentifierID
        {
            get
            {
                CanReadProperty(true);
                return _identifierID;
            }
            set
            {
                _identifierID = value;
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
                return this.ID.ToString() + "|" + _name.ToString();
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
                return ((IdentifierList)base.Parent).GetIndex(this);
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
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

        private string _inputText = string.Empty;
        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                CanWriteProperty(true);
                if (value == null) value = string.Empty;
                if (_inputText != value)
                {
                    _inputText = value;
                    PropertyHasChanged();
                }
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
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
            AuthorizationRules.AllowWrite("Identifier", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("xxx");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("xxx");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("xxx");
        }

        #region Factory Methods

        [COEUserActionDescription("CreateIdentifier")]
        public static Identifier NewIdentifier(string xml, bool isNew, bool isClean)
        {
            try
            {
                return new Identifier(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateIdentifier")]
        public static Identifier NewIdentifier(int id, int identifierID,string name, string description, bool active)
        {
            try
            {
                return new Identifier(id, identifierID, name, description, active);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateIdentifier")]
        public static Identifier NewIdentifier()
        {
            try
            {
                return new Identifier();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private Identifier()
        {
            MarkAsChild();
        }

        private Identifier(string xml, bool isNew, bool isClean) : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Identifier/IdentifierID");
            if(xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    int.TryParse(xIterator.Current.Value, out _identifierID);

            xIterator = xNavigator.Select("Identifier/ID");
            int id;
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    int.TryParse(xIterator.Current.Value, out id);
                    this.ID = id;
                }

            xIterator = xNavigator.Select("Identifier/InputText");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _inputText = xIterator.Current.Value;

            xIterator = xNavigator.Select("Identifier/IdentifierID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Name", string.Empty)))
                    _name = xIterator.Current.GetAttribute("Name", string.Empty);

            xIterator = xNavigator.Select("Identifier/IdentifierID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Active", string.Empty)))
                    _active = xIterator.Current.GetAttribute("Active", string.Empty).ToUpper() == "T" ? true : false;

            xIterator = xNavigator.Select("Identifier/IdentifierID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.GetAttribute("Description", string.Empty)))
                    _description = xIterator.Current.GetAttribute("Description", string.Empty);

            if (!isNew)
                MarkOld();

            if(isClean)
                MarkClean();
        }

        private Identifier(int id, int identifierID, string name, string description, bool active) : this()
        {
            this.ID = id;
            _identifierID = identifierID;
            _name = name;
            _description = description;
            _active = active;
        }

        #endregion

        internal string UpdateSelf(bool addInsertUpdateAttributes)
        {
            StringBuilder builder = new StringBuilder("");
            if (this._identifierID > 0)
            {
                string attrib = string.Empty;

                if (addInsertUpdateAttributes && this.IsNew)
                    attrib = " insert=\"yes\"";
                else if (addInsertUpdateAttributes && this.IsDeleted)
                    attrib = " delete=\"yes\"";
                else if (addInsertUpdateAttributes && this.IsDirty)
                    attrib = " update=\"yes\"";

                builder.Append(string.Format("<Identifier{0}>", attrib));

                if(this.ID > 0)
                    builder.Append("<ID>" + this.ID + "</ID>");
                builder.Append("<IdentifierID");
                if (addInsertUpdateAttributes && this.IsDirty && !this.IsNew && !this.IsDeleted)
                    builder.Append(" update=\"yes\"");
                builder.Append(">");
                builder.Append(this._identifierID + "</IdentifierID>");
                builder.Append("<InputText");
                if (addInsertUpdateAttributes && this.IsDirty && !this.IsNew && !this.IsDeleted)
                    builder.Append(" update=\"yes\"");
                builder.Append(">");
                builder.Append(_inputText + "</InputText>");
                builder.Append("<OrderIndex>" + this.OrderIndex.ToString() + "</OrderIndex>");
                builder.Append("</Identifier>");
            }
            return builder.ToString();
        }
    }
}

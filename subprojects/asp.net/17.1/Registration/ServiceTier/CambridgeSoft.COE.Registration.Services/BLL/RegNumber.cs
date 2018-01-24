using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;

using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class RegNumber : RegistrationBusinessBase<RegNumber>
    {
        //private string _rootNumber;
        private int _sequenceNumber;

        private string _value;
        public string RegNum
        {
            get
            {
                CanReadProperty(true);
                return _value;
            }
            //  Must be setable for legacy data loading
            set
            {
                _value = value;
                return;
            }
        }

        private Sequence _sequence;
        public Sequence Sequence
        {
            get
            {
                CanReadProperty(true);
                return _sequence;
            }
            set
            {
                CanWriteProperty(true);
                if (!_sequence.Equals(value))
                {
                    _sequence = value;
                    PropertyHasChanged();
                }
            }
        }

        public override bool IsValid { get { return base.IsValid; } }

        public override bool IsDirty { get { return base.IsDirty; } }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "RegNumber", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("regNumber");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("regNumber");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("regNumber");
        }

        [COEUserActionDescription("CreateRegNumber")]
        public static RegNumber NewRegNumber()
        {
            try
            {
                using (RegistryRecord registryRecord = RegistryRecord.NewRegistryRecord())
                {
                    RegNumber regNumber = registryRecord.RegNumber;

                    regNumber.MarkClean();
                    regNumber.MarkNew();

                    return regNumber;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateRegNumber")]
        public static RegNumber NewRegNumber(bool IsRegistryRecord)
        {
            try
            {
                if (IsRegistryRecord)
                {
                    using (RegistryRecord registryRecord = RegistryRecord.NewRegistryRecord())
                    {
                        RegNumber regNumber = registryRecord.RegNumber;

                        regNumber.MarkClean();
                        regNumber.MarkNew();

                        return regNumber;
                    }
                }
                else
                {
                    Component newComponent = Component.NewComponent();

                    RegNumber regNumber = newComponent.Compound.RegNumber;

                    regNumber.MarkClean();
                    regNumber.MarkNew();

                    return regNumber;

                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateRegNumber")]
        public static RegNumber NewRegNumber(string xml, bool isClean)
        {
            try
            {
                return new RegNumber(xml, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private RegNumber()
        {
            ValidationRules.CheckRules();
        }

        private RegNumber(string xml, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            /*XPathNodeIterator xIterator = xNavigator.Select("RegNumber/RootNumber");
            if(xIterator.MoveNext())
                _rootNumber = xIterator.Current.Value;*/

            XPathNodeIterator xIterator = xNavigator.Select("RegNumber/RegID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this.ID = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("RegNumber/SequenceNumber");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _sequenceNumber = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("RegNumber/RegNumber");
            if (xIterator.MoveNext())
                _value = xIterator.Current.Value;

            xIterator = xNavigator.Select("RegNumber/SequenceID");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _sequence = Sequence.NewSequence(xIterator.Current.ValueAsInt);
                else
                {
                    if (this.ID < 0)
                        _sequence = Sequence.NewSequence(2);
                    else
                        _sequence = Sequence.NewSequence(1);
                }
                //Harcoded until DB changes (it's returning this value)
            }

            if (isClean)
                MarkClean();

            //ValidationRules.CheckRules();
        }

        [COEUserActionDescription("SaveRegNumber")]
        public override RegNumber Save()
        {
            try
            {
                if (IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a RegNumber");
                }
                else if (IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a RegNumber");
                }
                else if (!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a RegNumber");
                }
                return base.Save();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<RegNumber");
            if (addCRUDattributes && this.IsDirty)
                builder.Append(" update=\"yes\"");
            builder.Append(">");
            builder.Append("<RegID>" + this.ID + "</RegID>");
            //builder.Append(string.Format("<RootNumber>{0}</RootNumber>", this._rootNumber != null ? this._rootNumber : string.Empty));
            builder.Append("<SequenceNumber>" + this._sequenceNumber + "</SequenceNumber>");

            if (_sequence != null)
                builder.Append(_sequence.UpdateSelf(addCRUDattributes));

            builder.Append(string.Format("<RegNumber>{0}</RegNumber>", this._value != null ? this._value : string.Empty));
            builder.Append("</RegNumber>");

            string retval = builder.ToString();
            return retval;
        }

    }
}


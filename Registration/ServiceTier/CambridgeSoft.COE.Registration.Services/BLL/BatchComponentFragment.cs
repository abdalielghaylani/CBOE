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
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// BatchComponentFragment
    /// </summary>
    [Serializable()]
    public class BatchComponentFragment : RegistrationBusinessBase<BatchComponentFragment>
    {
        private List<string> _changedProperties = new List<string>();
        private Dictionary<string, object> _oldValues = new Dictionary<string, object>();

        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [Browsable(true)]
        public int OrderIndex
        {
            get
            {
                CanReadProperty(true);
                return ((BatchComponentFragmentList)base.Parent).GetIndex(this);
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
                return this.ID.ToString() + "|" + _equivalents.ToString() + "|" + _fragmentId;
            }
        }

        private float _equivalents;
        public float Equivalents
        {
            get
            {
                CanReadProperty(true);
                return _equivalents;
            }
            set
            {
                CanWriteProperty(true);
                if (_equivalents != value)
                {
                    this.SetOldValue("Equivalents", _equivalents);
                    _equivalents = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _fragmentId;
        public int FragmentID
        {
            get
            {
                CanReadProperty(true);
                return _fragmentId;
            }
            set
            {
                CanWriteProperty(true);
                if (_fragmentId != value)
                {
                    this.SetOldValue("FragmentID", _fragmentId);
                    _fragmentId = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _compoundFragmentId;
        public int CompoundFragmentID
        {
            get
            {
                CanReadProperty(true);
                return _compoundFragmentId;
            }
        }

        private string _mw = string.Empty;
        public string MW
        {
            get
            {
                CanReadProperty(true);
                return _mw;
            }
            set
            {
                CanWriteProperty(true);
                if (_mw != value)
                {
                    this.SetOldValue("MW", _mw);
                    _mw = value;

                    //MW is not written in UpdateSelf() XML snippet, so we don't take it into account for IsDirty status.
                    //PropertyHasChanged();
                }
            }
        }

        private string _formula = string.Empty;
        public string Formula
        {
            get
            {
                CanReadProperty(true);
                return _formula;
            }
            set
            {
                CanWriteProperty(true);
                if (_formula != value)
                {
                    this.SetOldValue("Formula", _formula);
                    _formula = value;

                    ////Formula is not written in UpdateSelf() XML snippet, so we don't take it into account for IsDirty status.
                    //PropertyHasChanged();
                }
            }
        }

        public override bool IsValid
        {
            //get { return base.IsValid && _resources.IsValid; }
            get { return base.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
        }

        private void SetOldValue(string propName, object propValue)
        {
            if (_oldValues.ContainsKey(propName))
                _oldValues[propName] = propValue;
            else
                _oldValues.Add(propName, propValue);
        }

        public object GetOldValue(string propName)
        {
            if (_oldValues.ContainsKey(propName))
                return _oldValues[propName];
            else
                return null;
        }

        public int[] CheckForDuplicates()
        {
            throw new System.NotImplementedException();
        }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Fragment", "ADD_IDENTIFIER");
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

        [COEUserActionDescription("CreateBatchComponentFragment")]
        public static BatchComponentFragment NewBatchComponentFragment()
        {
            try
            {
                return new BatchComponentFragment();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateBatchComponentFragment")]
        public static BatchComponentFragment NewBatchComponentFragment(string xml, bool isNew, bool isClean)
        {
            try
            {
                return new BatchComponentFragment(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private BatchComponentFragment()
        {
            MarkAsChild();
        }

        [COEUserActionDescription("CreateBatchComponentFragment")]
        public static BatchComponentFragment NewBatchComponentFragment(int fragmentID, float equivalent)
        {
            try
            {
                return new BatchComponentFragment(fragmentID, equivalent);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateBatchComponentFragment")]
        public static BatchComponentFragment NewBatchComponentFragment(Fragment fragment, float equivalent)
        {
            try
            {
                BatchComponentFragment bcf = new BatchComponentFragment();
                bcf.Equivalents = equivalent;
                bcf.Formula = fragment.Formula;
                bcf.FragmentID = fragment.FragmentID;
                bcf.MW = fragment.MW;
                return bcf;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private BatchComponentFragment(int fragmentID, float equivalent)
            : this()
        {
            _fragmentId = fragmentID;
            _equivalents = equivalent;
        }

        private BatchComponentFragment(string xml, bool isNew, bool isClean) : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("BatchComponentFragment/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this.ID = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("BatchComponentFragment/FragmentID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _fragmentId = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("BatchComponentFragment/CompoundFragmentID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _compoundFragmentId = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("BatchComponentFragment/Equivalents");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _equivalents = Convert.ToSingle(xIterator.Current.Value);

            if (string.IsNullOrEmpty(MW) && string.IsNullOrEmpty(Formula))
            {
                List<int> framIdList = new List<int>();
                framIdList.Add(_fragmentId);
                FragmentList frg = FragmentList.GetFragmentList(framIdList);
                if (frg.Count > 0)
                {
                    _mw = frg[0].MW;
                    _formula = frg[0].Formula;
                }
            }
            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();
        }

        [COEUserActionDescription("SaveBatchComponentFragment")]
        public override BatchComponentFragment Save()
        {
            try
            {
                if (IsDeleted && !CanDeleteObject())
                {
                    throw new System.Security.SecurityException("User not authorized to remove a BatchComponentFragment");
                }
                else if (IsNew && !CanAddObject())
                {
                    throw new System.Security.SecurityException("User not authorized to add a BatchComponentFragment");
                }
                else if (!CanEditObject())
                {
                    throw new System.Security.SecurityException("User not authorized to update a BatchComponentFragment");
                }
                return base.Save();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        internal new void MarkNew()
        {
            base.MarkNew();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            if (FragmentID > 0)
            {
                builder.Append(string.Format("<BatchComponentFragment{0}>", (addCRUDattributes && this.IsNew ? " insert=\"yes\"" : (addCRUDattributes && this.IsDeleted ? " delete=\"yes\"" : string.Empty))));

                builder.Append("<ID>" + this.ID + "</ID>");

                builder.Append("<FragmentID");
                if (addCRUDattributes && !this.IsNew && _changedProperties.Contains("FragmentID"))
                    builder.Append(" update=\"yes\"");
                builder.Append(">" + this._fragmentId + "</FragmentID>");

                builder.Append("<Equivalents");
                if (addCRUDattributes && !this.IsNew && _changedProperties.Contains("Equivalents"))
                    builder.Append(" update=\"yes\"");

                builder.Append(">" + this._equivalents + "</Equivalents>");
                builder.Append("<OrderIndex>" + this.OrderIndex.ToString() + "</OrderIndex>");
                builder.Append("</BatchComponentFragment>");
            }
            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            int fetchfragmentId = (parentNode.SelectSingleNode("FragmentID")!=null?int.Parse(parentNode.SelectSingleNode("FragmentID").InnerText):-1);
            float fetchEquivalents = (parentNode.SelectSingleNode("Equivalents") != null ? float.Parse(parentNode.SelectSingleNode("Equivalents").InnerText) : -1);
            if (fetchfragmentId == this.FragmentID)
                this.Equivalents = fetchEquivalents;
        }
    }
}



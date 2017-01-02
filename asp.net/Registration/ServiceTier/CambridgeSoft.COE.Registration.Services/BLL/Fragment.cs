using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Registration.Services.Common;

/*TODO: Provide a better abstaraction for Component.FragmentList, or eliminate it
 * altogether. We should have a CompoundFragment separate from a Fragment, which has
 * simply the CompoundFragmentId and a valid Fragment object.
 */

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// This class is supposed to represent the editing class for pure fragment data,
    /// but is confused about its usage! Why does it have a FragmentComponentId?
    /// </summary>
    /// <remarks>
    /// Is this data managed by the Table Editor?
    /// </remarks>
    [Serializable()]
    public class Fragment : BusinessBase<Fragment>
    {

        #region [ Factory Methods ]

        public static Fragment NewFragment(bool isNew, bool isClean)
        {
            return new Fragment(isNew, isClean);
        }
        private Fragment(bool isNew, bool isClean)
        {
            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();

            MarkAsChild();
        }

        public static Fragment NewFragment(string xml, bool isNew, bool isClean)
        {
            Fragment fragment = new Fragment(xml, isNew, isClean);
            return fragment;
        }
        private Fragment(string xml, bool isNew, bool isClean)
        {

            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Fragment/FragmentID");

            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _fragmentId = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Fragment/CompoundFragmentID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _compoundFragmentId = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Fragment/Description");
            if (xIterator.MoveNext())
                _description = xIterator.Current.Value;

            xIterator = xNavigator.Select("Fragment/DateCreated");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _dateCreated = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Fragment/DateLastModified");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _dateLastModified = DateTime.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Fragment/Code");
            if (xIterator.MoveNext())
                _code = xIterator.Current.Value;

            xIterator = xNavigator.Select("Fragment/Structure");
            if (xIterator.MoveNext())
                _structure = Structure.NewStructure(xIterator.Current.OuterXml, isNew, isClean);

            if (_structure != null)
            {
                _mw = _structure.MolWeight.ToString();
                _formula = _structure.Formula;
            }

            xIterator = xNavigator.Select("Fragment/FragmentTypeID");
            if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.Value))
                _fragmenttTypeId = string.IsNullOrEmpty(xIterator.Current.Value) ? -1 : Convert.ToInt32(xIterator.Current.Value);

            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();

            MarkAsChild();
        }

        internal static Fragment NewFragment(int fragmentId, string code, string description, int fragmentTypeId, string structureValue, double mw, string formula)
        {
            return new Fragment(fragmentId, code, description, fragmentTypeId, structureValue, mw, formula);
        }
        private Fragment(int fragmentId, string code, string description, int fragmentTypeId, string structureValue, double mw, string formula)
            : this()
        {
            _fragmentId = fragmentId;
            _code = code;
            _description = description;
            _fragmenttTypeId = fragmentTypeId;
            _mw = mw.ToString();
            _formula = formula;
            _structure = Structure.NewStructure(structureValue, mw, formula);
        }

        private Fragment()
        {
            MarkAsChild();
            MarkClean();
            if (this.FragmentID > 0)
                this.MarkOld();
        }

        #endregion

        #region [ Authorization and Validation Rules ]

        /// <summary>
        /// Permits editability only by users with the ADD_IDENTIFIER privilege.
        /// </summary>
        /// <remarks>
        /// A fragment is an identifier somehow? This seems kind of sloppy...
        /// </remarks>
        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite("Fragment", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject() { return true; }

        public static bool CanGetObject() { return true; }

        public static bool CanDeleteObject() { return true; }

        public static bool CanEditObject() { return true; }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        #endregion

        #region [ Properties and members ]

        private List<string> _changedProperties = new List<string>();

        private int _fragmentId;
        [System.ComponentModel.DataObjectField(true, true)]
        public int FragmentID
        {
            get
            {
                CanReadProperty(true);
                return _fragmentId;
            }
            set
            {
                if (_fragmentId != value)
                {
                    _fragmentId = value;
                    if (!_changedProperties.Contains("FragmentID"))
                        _changedProperties.Add("FragmentID");
                    PropertyHasChanged();
                }
            }
        }

        private int _fragmenttTypeId;
        public int FragmentTypeId
        {
            get
            {
                CanReadProperty(true);
                return _fragmenttTypeId;
            }
            set
            {
                _fragmenttTypeId = value;
            }
        }

        private int _compoundFragmentId;
        public int CompoundFragmentId
        {
            get
            {
                CanReadProperty(true);
                return _compoundFragmentId;
            }
            set
            {
                _compoundFragmentId = value;
            }
        }

        private string  _code;
        public string Code
        {
            get
            {
                CanReadProperty(true);
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        private string _mw;
        public string MW
        {
            get
            {
                CanReadProperty(true);
                return _mw;
            }
            set
            {
                _mw = value;
            }
        }
        
        private string _formula;
        public string Formula
        {
            get
            {
                CanReadProperty(true);
                return _formula;
            }
            set
            {
                _formula = value;
            }
        }
        
        private Structure _structure;
        /// <summary>
        /// Structure associated with the current Fragment
        /// </summary>
        public Structure Structure
        {
            get
            {
                CanReadProperty(true);
                return _structure;
            }
            internal set
            {
                _structure = value;
            }
        }
       
        private string _description;
        /// <summary>
        /// Description text
        /// </summary>
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
                if (_description != value)
                {
                    _description = value;
                    PropertyHasChanged();
                }
            }
        }

        private DateTime _dateCreated;

        private DateTime _dateLastModified;
        
        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string OrderIndex
        {
            get
            {
                CanReadProperty(true);
                return string.Format("{0:00}", ((FragmentList)base.Parent).IndexOf(this) + 1);
            }
        }

        public override bool IsValid
        {
            get { return base.IsValid; }
        }

        public override bool IsDirty
        {
            get 
            {
                if (this.Structure != null)
                    return base.IsDirty || Structure.IsDirty;
                else
                    return base.IsDirty;
            }
        }

        #endregion

        /// <summary>
        /// Csla requirement for the current version of that framework.
        /// </summary>
        /// <returns>the local ID value</returns>
        protected override object GetIdValue() { return _fragmentId; }

        /// <summary>
        /// Custom xml-based 'serialization' of this object's current state
        /// </summary>
        /// <param name="addCRUDattributes">true to record state-changes</param>
        /// <returns>an xml string representing this object and its children</returns>
        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            if (this._fragmentId > 0)
            {
                string attrib = string.Empty;

                if (addCRUDattributes && this.IsNew)
                    attrib = " insert=\"yes\"";
                else if (addCRUDattributes && this.IsDeleted)
                    attrib = " delete=\"yes\"";
                else if (addCRUDattributes && this.IsDirty)
                    attrib = " update=\"yes\"";

                builder.Append(string.Format("<Fragment{0}>", attrib));

                builder.Append("<CompoundFragmentID>" + this.CompoundFragmentId + "</CompoundFragmentID>");
                
                builder.Append("<FragmentID");
                if (addCRUDattributes && this.IsDirty && !this.IsNew && !this.IsDeleted)
                    builder.Append(" update=\"yes\"");
                builder.Append(">" + this.FragmentID + "</FragmentID>");

                if (_structure != null)
                    builder.Append(this._structure.UpdateSelf(addCRUDattributes));
                builder.Append("</Fragment>");
            }
            return builder.ToString();
        }

    }
}


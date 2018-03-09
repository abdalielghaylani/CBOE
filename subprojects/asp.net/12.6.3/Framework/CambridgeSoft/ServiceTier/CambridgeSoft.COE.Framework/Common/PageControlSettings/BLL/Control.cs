using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using Csla.Validation;
using System.Xml.XPath;
using System.IO;
using Infragistics.WebUI.UltraWebNavigator;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class Control : BusinessBase<Control>
    {
        #region Constant

        public const string PROPERTY_NAME_ID = "ID";
        public const string PROPERTY_NAME_FRIENDLYNAME = "FriendlyName";

        #endregion

        #region Variables

        private string _id = string.Empty;
        private string _friendlyName = string.Empty;
        private string _description = string.Empty;
        private string _parentControlId = string.Empty;
        private ControlType _controlType = ControlType.Control;
        private string _placeHolderID = string.Empty;
        private int _coeformID = -1;
        private string _pageID = string.Empty;
        private Actions _action = Actions.Hide;

        public enum ControlType
        {
            COEForm = 0,
            Control = 1,
            Page = 2,
            COEGenerableControl = 3,
            CompositeControl = 4,
            COEWebGridColumn = 5,
            COETableManagerChildTable = 6
        }

        [DefaultValue(Actions.Hide)]
        public enum Actions 
        {
            Hide,
            Disable
        }

        #endregion

        #region Properties

        /// <summary>
        /// Control Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public string ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }

        /// <summary>
        /// A nicer name for the page
        /// </summary>
        public string FriendlyName
        {
            get
            {
                CanReadProperty(true);
                return _friendlyName;
            }
        }

        /// <summary>
        /// A brief description of the current page
        /// </summary>
        public string Description
        {
            get
            {
                CanReadProperty(true);
                return _description;
            }
        }

        /// <summary>
        /// In CompoundControls the reference to the grand parent control
        /// </summary>
        public string ParentControlId
        {
            get 
            {
                CanReadProperty(true);
                return _parentControlId; 
            }
            set 
            { 
                _parentControlId = value; 
            }
        }

        /// <summary>
        /// Type of control
        /// </summary>
        /// <remarks>This is required is order to understand how to disable the control
        /// E.g the way to disable a page (yes, it's a control) it's very different from a COEForm. 
        ///</remarks>
        public ControlType TypeOfControl
        {
            get { return _controlType; }
        }

        /// <summary>
        /// Holder of the control in the page.
        /// </summary>
        /// <remarks>Required for COEForms, not important at all for pages and simple controls</remarks>
        public string PlaceHolderID
        {
            get { return _placeHolderID; }
        }

        /// <summary>
        /// Page id of where the control belongs to.
        /// </summary>
        /// <remarks> Required for an easier identification when disabling controls</remarks>
        public string PageID
        {
            get { return _pageID.ToUpper(); }
        }

        /// <summary>
        /// COEFormGroupID where the formelement is inside.
        /// </summary>
        public int COEFormID
        {
            get
            {
                return _coeformID;
            }
        }

        /// <summary>
        /// Defines if the control must be hide or disabled.
        /// </summary>
        public Actions Action 
        {
            get { return _action; }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Create a Control by given xml.
        /// </summary>
        /// <param name="xml"></param>
        private Control(string xml, string pageId)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Control/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                     _id = ControlIdChangeUtility.ChangeControlID(xIterator.Current.Value);//checking the id if eligible for change

            xIterator = xNavigator.Select("Control/FriendlyName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _friendlyName = xIterator.Current.Value;

            xIterator = xNavigator.Select("Control/Description");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _description = xIterator.Current.Value;

            xIterator = xNavigator.Select("Control/ParentControlID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _parentControlId = xIterator.Current.Value;

            xIterator = xNavigator.Select("Control/PlaceHolderID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _placeHolderID = xIterator.Current.Value;

            xIterator = xNavigator.Select("Control/TypeOfControl");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    if (Enum.IsDefined(typeof(ControlType), xIterator.Current.Value))
                        _controlType = (ControlType)Enum.Parse(typeof(ControlType), xIterator.Current.Value);
                }
            }

            if (!string.IsNullOrEmpty(pageId))
                _pageID = pageId;

            xIterator = xNavigator.Select("Control/COEFormID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    int.TryParse(xIterator.Current.Value, out _coeformID);

            xIterator = xNavigator.Select("Control/Action");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    if (Enum.IsDefined(typeof(Actions), xIterator.Current.Value))
                        _action = (Actions)Enum.Parse(typeof(Actions), xIterator.Current.Value);
                }
            }
        }

        #endregion

        #region Factory methods

        public static Control NewControl(string xml, string pageId)
        {
            return new Control(xml, pageId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build this into the custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Control>");
            builder.Append("<ID>" + this._id + "</ID>");
            if (type == COEPageControlSettings.Type.Master)
            {
                builder.Append("<FriendlyName>" + this._friendlyName + "</FriendlyName>");
                builder.Append("<Description>" + this._friendlyName + "</Description>");
            }
            builder.Append("<TypeOfControl>" + this._controlType.ToString() + "</TypeOfControl>");
            builder.Append("<PlaceHolderID>" + this._placeHolderID + "</PlaceHolderID>");
            builder.Append("<COEFormID>" + this._coeformID + "</COEFormID>");
            builder.Append("<Action>" + this._action.ToString() + "</Action>");

            builder.Append("</Control>");
            return builder.ToString();
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion
    }
}

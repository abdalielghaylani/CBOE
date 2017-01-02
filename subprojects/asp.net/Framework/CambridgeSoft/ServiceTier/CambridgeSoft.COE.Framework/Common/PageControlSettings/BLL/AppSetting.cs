using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using Csla.Validation;
using System.Xml.XPath;
using System.IO;
using Infragistics.WebUI.UltraWebNavigator;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class AppSetting : BusinessBase<AppSetting>
    {
        #region Constant

        public const string PROPERTY_NAME_ID = "ID";
        public const string PROPERTY_NAME_KEY = "KEY";

        #endregion

        #region Variables

        private string _id = string.Empty;
        private string _key = string.Empty;
        private string _value = string.Empty;
        private string _type = string.Empty;
        private string _friendlyName = string.Empty;
        private string _description = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// The variable name 
        /// </summary>
        public string ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }
        
        /// <summary>
        /// The variable name 
        /// </summary>
        public string Key
        {
            get
            {
                CanReadProperty(true);
                return _key;
            }
        }

        /// <summary>
        /// The value that the variable should hold
        /// </summary>
        public string Value
        {
            get
            {
                CanReadProperty(true);
                return _value;
            }
            set {
                _value = value;
            }
        }

        /// <summary>
        /// The source for the variable value
        /// </summary>
        public string Type
        {
            get
            {
                CanReadProperty(true);
                return _type;
            }
        }

        /// <summary>
        /// A nicer name for the appSetting
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
        /// A description for the appSetting
        /// </summary>
        public string Description
        {
            get
            {
                CanReadProperty(true);
                return _description;
            }
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Create an AppSetting by given xml.
        /// </summary>
        /// <param name="xml">AppSetting content in xml format</param>
        /// <returns>AppSetting Object created from the given xml</returns>
        public static AppSetting NewAppSetting(string xml)
        {
            return new AppSetting(xml);
        }

        /// <summary>
        /// Create an AppSetting with the given parameters
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns>AppSetting Object created from the given parameters</returns>
        public static AppSetting NewAppSetting(string ID, string key, string value, string type)
        {
            return new AppSetting(ID, key, value, type);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml">AppSetting content in xml format</param>
        private AppSetting(string xml)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("AppSetting/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSetting/Key");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _key = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSetting/Value");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _value = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSetting/Type");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _type = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSetting/FriendlyName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _friendlyName = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSetting/Description");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _description = xIterator.Current.Value;
        }

        /// <summary>
        /// Constructor with values in parameters
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        private AppSetting(string ID, string key, string value, string type)
        {
            _id = ID;
            _key = key;
            _value = value;
            _type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Format this into xml.
        /// </summary>
        /// <returns>string with the xml content</returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<AppSetting>");
            builder.Append("<ID>" + this._id + "</ID>");
            builder.Append("<Key>" + this._key + "</Key>");
            builder.Append("<Value>" + this._value + "</Value>");
            builder.Append("<Type>" + this._type + "</Type>");
            builder.Append("</AppSetting>");
            return builder.ToString();
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion
    }
}

/// <summary>
/// Enumerate allowed Application Setting types for COEPAGECONTROL elements. Allowed values are:
/// <list type="bullet">
///   <item>All: Read the application settings from all the sources</item>
///   <item>Session: Read the application settings from Session variables</item>
///   <item>COEConfiguration: Read the application settings form COEDB.COECONFIGURATION table</item>
///   <item>Application: Read the application settings form Application variables</item>
/// </list>
/// </summary>
public enum AppSettingTypes
{
    ///<summary>Read the application settings from all the sources</summary>
    All,
    ///<summary>Read the application settings from Session variables</summary>
    Session,
    ///<summary>Read the application settings form COEDB.COECONFIGURATION table</summary>
    COEConfiguration,
    /// <summary>Read the application settings from Application variables</summary>
    Application
}
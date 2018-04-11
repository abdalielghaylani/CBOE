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
    public class Privilege : BusinessBase<Privilege>
    {
        #region Constant

        public const string PROPERTY_NAME_ID = "ID";
        public const string PROPERTY_NAME_FRIENDLYNAME = "FriendlyName";

        #endregion

        #region Variables

        private string _id = string.Empty;
        private string _friendlyName = string.Empty;
        private string _description = string.Empty;

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
        /// A nicer name for the privilege
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
        /// A brief description of the current privilege
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
        /// Create a Privilege by given xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Privilege NewPrivilege(string xml)
        {
            return new Privilege(xml, string.Empty);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        private Privilege(string xml, string pageID)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Privilege/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("Privilege/FriendlyName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _friendlyName = xIterator.Current.Value;

            xIterator = xNavigator.Select("Privilege/Description");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _description = xIterator.Current.Value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Format this into xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Privilege>");
            builder.Append("<ID>" + this._id + "</ID>");
            builder.Append("</Privilege>");
            return builder.ToString();
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion
    }
}

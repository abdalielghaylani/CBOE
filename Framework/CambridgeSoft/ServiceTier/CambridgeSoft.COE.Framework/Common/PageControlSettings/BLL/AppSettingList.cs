using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Infragistics.WebUI.UltraWebNavigator;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class AppSettingList : BusinessListBase<AppSettingList, AppSetting>
    {
        #region Variables

        private Operators _operator = Operators.OR;

        [NonSerialized]
        private FilteredBindingList<AppSetting> _filteredList;
        private string _id = string.Empty;
        private string _oldID = string.Empty;

        public enum Operators
        {
            OR = 0,
            AND = 1,
        }

        #endregion

        #region Properties

        /// <summary>
        /// AppSettingList Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// This property is used to save old ID When update ID.
        /// </summary>
        public string OldID
        {
            get { return _oldID; }
            set { _oldID = value; }
        }

        public Operators OperatorToApply
        {
            get { return _operator; }
            set { _operator = value; }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Return an AppSetting list created given the input xml.
        /// </summary>
        /// <param name="xml">XML snippet that represets the object</param>
        /// <returns>A list of pages</returns>
        public static AppSettingList NewAppSettingList(string xml, string pageId)
        {
            return new AppSettingList(xml, pageId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build this object into custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<AppSettings>");
            if (this.Count > 0)
            {
                builder.Append("<Operator>" + this._operator + "</Operator>");
                builder.Append("<ID>" + this._id + "</ID>");
                //Update AppSetting collection
                for (int i = 0; i < this.Count; i++)
                    builder.Append(this[i].UpdateSelf(type));
            }
            builder.Append("</AppSettings>");
            return builder.ToString();
        }

        /// <summary>
        /// Get an AppSetting by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AppSetting GetByID(string Id)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<AppSetting>(this);
            _filteredList.RemoveFilter();//Just in case a previous filter has been applied
            _filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByID);
            _filteredList.ApplyFilter(String.Empty, Id);
            if (_filteredList.Count > 0)
            {
                if (_filteredList[0] != null)
                    return _filteredList[0];
            }
            return null;
        }

        /// <summary>
        /// Returns a list of appsettings that is available to select 
        /// </summary>
        /// <returns></returns>
        public AppSettingList GetAvailableAppSettingList(AppSettingList fullAppSettingList)
        {
            AppSettingList availableAppSettingList = new AppSettingList();
            foreach (AppSetting appSetting in fullAppSettingList)
            {
                if (!this.Contains(appSetting))
                    availableAppSettingList.Add(appSetting);
            }
            return availableAppSettingList;
        }


        /// <summary>
        /// Returns a list of privileges list that already exists in given AppSettingList.
        /// </summary>
        /// <returns></returns>
        public AppSettingList GetCurrentAppSettingList(AppSettingList fullAppSettingList)
        {
            AppSettingList currentAppSettingList = new AppSettingList();
            foreach (AppSetting appSetting in fullAppSettingList)
            {
                if (this.Contains(appSetting))
                    currentAppSettingList.Add(this.GetByID(appSetting.ID));
            }
            return currentAppSettingList;
        }

        #endregion

        #region Constructors

        public AppSettingList()
        {
            if (_filteredList == null)
                _filteredList = new FilteredBindingList<AppSetting>(this);
        }

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        private AppSettingList(string xml, string pageId)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("AppSettings/Operator");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    if (Enum.IsDefined(typeof(Operators), xIterator.Current.Value))
                        _operator = (Operators)Enum.Parse(typeof(Operators), xIterator.Current.Value);
                }
            }

            xIterator = xNavigator.Select("AppSettings/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("AppSettings/AppSetting");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(AppSetting.NewAppSetting(xIterator.Current.OuterXml));
                } while (xIterator.Current.MoveToNext() && xIterator.Current.Name == "AppSetting");
            }
        }

        #endregion

        #region Filter Classes

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                string id = string.Empty;
                AppSetting appSetting = null;
                //Filter is a int
                if (filter is string)
                    id = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item is AppSetting)
                {
                    appSetting = ((AppSetting)item);
                    if (appSetting != null && appSetting.ID == id) //For coverity fix on 11Jan2013
                        return true;
                }
                return false;
            }
        }

        #endregion
    }
}
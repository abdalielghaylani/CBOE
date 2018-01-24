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
    public class ControlSettingList : BusinessListBase<ControlSettingList, ControlSetting>
    {
        #region Variables

        [NonSerialized]
        private FilteredBindingList<ControlSetting> _filteredList;
        private string _id = string.Empty;
        private string _oldID = string.Empty;
        private string _xml = string.Empty;
        //private PrivilegeList _privilegeList;
        //private ControlList _ctrlList;
        //private AppSettingList _appSettingList;

        public enum Operators
        {
            OR = 0,
            AND = 1,
        }

        #endregion

        #region Properties

        /// <summary>
        /// ControlSettingList Identifier
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


        #endregion

        #region Factory Methods

        /// <summary>
        /// Return a ControlSetting list created given the input xml.
        /// </summary>
        /// <param name="xml">XML snippet that represets the object</param>
        /// <returns>A list of pages</returns>
        public static ControlSettingList NewControlSettingList(string xml, string pageId)
        {
            return new ControlSettingList(xml, pageId);
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
            builder.Append("<ControlSettings>");
            //Update ControlSetting
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf(type));
            builder.Append("</ControlSettings>");
            return builder.ToString();
        }

        /// <summary>
        /// Get a privilege by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ControlSetting GetByID(string Id)
        {
            //TODO; Check why is happening this.
            if (_filteredList == null) _filteredList = new FilteredBindingList<ControlSetting>(this);
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
        /// Returns a list privileges that is available to select with given ControlSettingList.
        /// </summary>
        /// <returns></returns>
        public ControlSettingList GetAvailablePrivList(ControlSettingList fullPrivList)
        {
            ControlSettingList availablePrivList = new ControlSettingList();
            foreach (ControlSetting priv in fullPrivList)
            {
                if (!this.Contains(priv))
                    availablePrivList.Add(priv);
            }
            return availablePrivList;
        }

        /// <summary>
        /// Returns a list privileges that has already existed in given ControlSettingList.
        /// </summary>
        /// <returns></returns>
        public ControlSettingList GetCurrentPrivList(ControlSettingList fullPrivList)
        {
            ControlSettingList currentPrivList = new ControlSettingList();
            foreach (ControlSetting priv in fullPrivList)
            {
                if (this.Contains(priv))
                    currentPrivList.Add(priv);
            }
            return currentPrivList;
        }


        #endregion

        #region Constructors

        public ControlSettingList()
        {
            if (_filteredList == null)
                _filteredList = new FilteredBindingList<ControlSetting>(this);
        }

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        private ControlSettingList(string xml, string pageId)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("ControlSettings/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("ControlSettings/ControlSetting");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(ControlSetting.NewControlSetting(xIterator.Current.OuterXml, pageId));
                } while (xIterator.Current.MoveToNext() && xIterator.Current.Name == "ControlSetting");
            }



        }

        #endregion

        #region Filter Classes

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                string id = string.Empty;
                ControlSetting priv = null;
                //Filter is a int
                //Coverity Bug Fix CID 11467 
                if (filter != null && filter is string)
                    id = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is ControlSetting)
                    priv = ((ControlSetting)item);
                if (priv != null && priv.ID == id)
                    return true;
                return false;
            }
        }

        #endregion
    }
}

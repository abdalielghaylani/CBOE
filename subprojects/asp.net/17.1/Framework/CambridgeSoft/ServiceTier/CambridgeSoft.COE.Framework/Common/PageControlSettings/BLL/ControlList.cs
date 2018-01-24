using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class ControlList : BusinessListBase<ControlList, Control>
    {
        #region Variables

        [NonSerialized]
        private FilteredBindingList<Control> _filteredList;
        private string _id = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// ControlList Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public string ID
        {
            get { return _id; }
        }

        public FilteredBindingList<Control> Controls
        {
            get { return _filteredList; }
        }


        #endregion

        #region Factory Methods

        /// <summary>
        /// Return a Control list created given the input xml.
        /// </summary>
        /// <param name="xml">XML snippet that represets the object</param>
        /// <returns>A list of pages</returns>
        public static ControlList NewControlList(string xml, string pageID)
        {
            return new ControlList(xml, pageID);
        }

        /// <summary>
        /// Return a Control list created given the input xml.
        /// </summary>
        /// <returns>A list of pages</returns>
        public static ControlList NewControlList()
        {
            return new ControlList();
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
            builder.Append("<Controls>");
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf(type));
            builder.Append("</Controls>");
            return builder.ToString();
        }

        /// <summary>
        /// Get a control by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Control GetByID(string Id)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<Control>(this);
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
        /// Get a control by given id and coeFormId
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="coeFormId"></param>
        /// <returns></returns>
        public Control GetByIDAndCoeFormId(string Id, string coeFormId)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<Control>(this);
            _filteredList.RemoveFilter();//Just in case a previous filter has been applied
            _filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByIDAndCoeFormId);
            string[] filter = new string[2];
            filter[0] = Id;
            filter[1] = coeFormId;
            _filteredList.ApplyFilter(String.Empty, filter);
            if (_filteredList.Count > 0)
            {
                if (_filteredList[0] != null)
                    return _filteredList[0];
            }
            return null;
        }

        /// <summary>
        /// Get a control by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ControlList GetByPageID(string pageId)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<Control>(this);
            _filteredList.RemoveFilter();//Just in case a previous filter has been applied
            _filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByPageID);
            _filteredList.ApplyFilter(String.Empty, pageId);
            return _filteredList.Count > 0 ? this.ConvertToControlList(_filteredList) : null;
        }

        /// <summary>
        /// Converts the result of a filter to a list of controls.
        /// </summary>
        /// <param name="filteredList"></param>
        /// <returns></returns>
        private ControlList ConvertToControlList(FilteredBindingList<Control> filteredList)
        {
            ControlList retVal = ControlList.NewControlList();
            foreach (Control ctrl in filteredList)
                retVal.AddControl(ctrl);
            return retVal;
        }

        /// <summary>
        /// Get a control by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool Exists(COEPageControlSettingsService.Control control)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<Control>(this);
            _filteredList.RemoveFilter();//Just in case a previous filter has been applied
            _filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByIDAndCoeFormId);
            string[] filter = new string[2];
            filter[0] = control.ID;
            filter[1] = control.COEFormID.ToString();
            _filteredList.ApplyFilter(String.Empty, filter);

            foreach (COEPageControlSettingsService.Control currentControl in _filteredList)
            {
                if (control.PageID == currentControl.PageID && control.TypeOfControl != CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.ControlType.CompositeControl)
                    return true;
            }

            return false;
        }

        #endregion

        #region Filter Classes

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                string id = string.Empty;
                Control ctrl = null;
                //Filter is a int
                if (filter != null && filter is string)
                    id = filter.ToString().ToUpper();
                //assume item is the object it self. (See string.empty parameter)
                //Coverity Bug Fix CID 11464 
                if (item != null && item is Control)
                    ctrl = ((Control)item);
                /** CBSR - 127802, 127805
                 * Changed by - Soorya
                 * Date - 08-Jul-2010
                 * Purpose - To uppercase the control ID, for a correct comparison with the ID of the control that was selected by the user in page settings.
                 */
                if (ctrl != null && ctrl.ID.ToUpper() == id)
                    return true;
                return false;
                //End Of Change
            }

            public static bool GetByIDAndCoeFormId(object item, object filter)
            {
                string id = string.Empty;
                string coeFormId = string.Empty;
                Control ctrl = null;
                string[] filters = (string[])filter;
                //Filter is a int
                //Coverity bug Fix CID 11465 
                if (filter != null && filter is string[])
                {
                    id = filters[0].ToUpper();
                    coeFormId = filters[1].ToUpper();
                }
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is Control)
                    ctrl = ((Control)item);
                if (ctrl != null && ctrl.ID == id && ctrl.COEFormID.ToString() == coeFormId)
                    return true;
                return false;
            }

            public static bool GetByPageID(object item, object filter)
            {
                string id = string.Empty;
                Control ctrl = null;
                //Filter is a int
                //Coverity Bug Fix CID 11466 
                if (filter != null && filter is string)
                    id = filter.ToString().ToUpper();
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is Control)
                    ctrl = ((Control)item);
                if (ctrl != null && ctrl.PageID == id)
                    return true;
                else if (ctrl != null && (ctrl.PageID + ControlIdChangeUtility.PERMSUFFIX.ToUpper()) == id)
                    return true;
                else if (ctrl != null && (ctrl.PageID + ControlIdChangeUtility.TEMPSUFFIX.ToUpper()) == id)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Constructors

        public ControlList()
        {
            if (_filteredList == null)
                _filteredList = new FilteredBindingList<Control>(this);
        }

        /// <summary>
        /// Create a ControlList by given xml.
        /// </summary>
        /// <param name="xml"></param>
        private ControlList(string xml, string pageId)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Controls/Control");

            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Control.NewControl(xIterator.Current.OuterXml, pageId));
                } while (xIterator.Current.MoveToNext());
            }

            xIterator = xNavigator.Select("Controls/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;
        }

        #endregion

        /// <summary>
        /// Add a list of controls to the current list
        /// </summary>
        /// <param name="controlList"></param>
        internal void AddControls(ControlList controlList)
        {
            foreach (COEPageControlSettingsService.Control ctrl in controlList)
            {
                if (!this.Exists(ctrl))
                    this.Add(ctrl);
            }
        }

        /// <summary>
        /// Add a control to the current list
        /// </summary>
        /// <param name="ctrl"></param>
        internal void AddControl(Control ctrl)
        {
            if (!this.Exists(ctrl))
                this.Add(ctrl);
        }
    }
}

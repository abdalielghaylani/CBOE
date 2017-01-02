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
    public class PrivilegeList : BusinessListBase<PrivilegeList, Privilege>
    {
        #region Variables

        private Operators _operator = Operators.OR;
        [NonSerialized]
        private FilteredBindingList<Privilege> _filteredList;
        private string _id = string.Empty;
        private string _oldID = string.Empty;
        private string _xml = string.Empty;
        private ControlList _ctrlList;

        public enum Operators
        {
            OR = 0,
            AND = 1,
        }

        #endregion

        #region Properties

        /// <summary>
        /// PrivilegeList Identifier
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

        public COEPageControlSettingsService.ControlList Controls
        {
            get { return _ctrlList; }
        }

        //public AppSettingList AppSettingList
        //{
        //    get { return _appSettingList; }
        //}

        #endregion

        #region Factory Methods

        /// <summary>
        /// Return a Privilege list created given the input xml.
        /// </summary>
        /// <param name="xml">XML snippet that represets the object</param>
        /// <returns>A list of pages</returns>
        public static PrivilegeList NewPrivilegeList(string xml, string pageId)
        {
            return new PrivilegeList(xml, pageId);
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
            builder.Append("<Privileges>");
            builder.Append("<Operator>" + this._operator + "</Operator>");
            builder.Append("<ID>" + this._id + "</ID>");
            //Update privilege
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf(type));
            builder.Append("</Privileges>");
            return builder.ToString();
        }

        /// <summary>
        /// Get a privilege by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Privilege GetByID(string Id)
        {
            //TODO; Check why is happening this.
            if (_filteredList == null) _filteredList = new FilteredBindingList<Privilege>(this);
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
        /// Set a control into this.
        /// </summary>
        /// <param name="ctrl"></param>
        public void AddCtrl(Control ctrl)
        {
            if (this._ctrlList == null)
                this._ctrlList = new ControlList();
            this._ctrlList.Add(ctrl);
        }

        /// <summary>
        /// Returns a list privileges that is available to select with given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public PrivilegeList GetAvailablePrivList(PrivilegeList fullPrivList)
        {
            PrivilegeList availablePrivList = new PrivilegeList();
            foreach (Privilege priv in fullPrivList)
            {
                if (!this.Contains(priv))
                    availablePrivList.Add(priv);
            }
            return availablePrivList;
        }

        /// <summary>
        /// Returns a list privileges that has already existed in given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public PrivilegeList GetCurrentPrivList(PrivilegeList fullPrivList)
        {
            PrivilegeList currentPrivList = new PrivilegeList();
            foreach (Privilege priv in fullPrivList)
            {
                if (this.Contains(priv))
                    currentPrivList.Add(priv);
            }
            return currentPrivList;
        }

        /// <summary>
        /// Returns a list controls that is available to select with given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public ControlList GetAvailableCtrlList(ControlList fullCtrlList)
        {
            ControlList availableCtrlList = new ControlList();
            foreach (Control ctrl in fullCtrlList)
            {
                if (!_ctrlList.Contains(ctrl))
                    availableCtrlList.Add(ctrl);
            }
            return availableCtrlList;
        }

        /// <summary>
        /// Returns a list controls that has already existed in given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public ControlList GetCurrentCtrlList(ControlList fullCtrlList)
        {
            ControlList currentCtrlList = new ControlList();
            foreach (Control ctrl in fullCtrlList)
            {
                if (_ctrlList.Contains(ctrl))
                    currentCtrlList.Add(ctrl);
            }
            return currentCtrlList;
        }

        /// <summary>
        /// Create the privilegeList node of tree.
        /// </summary>
        /// <returns></returns>
        public Node CreatePrivListNode()
        {
            Node privListNode = new Node();
            privListNode.Text = ID;
            privListNode.DataKey = this;
            return privListNode;
        }

        #endregion

        #region Constructors

        public PrivilegeList()
        {
            if (_filteredList == null)
                _filteredList = new FilteredBindingList<Privilege>(this);
        }

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        private PrivilegeList(string xml, string pageId)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Privileges/Operator");
            if (xIterator.MoveNext())
            {
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    if (Enum.IsDefined(typeof(Operators), xIterator.Current.Value))
                        _operator = (Operators)Enum.Parse(typeof(Operators), xIterator.Current.Value);
                }
            }

            xIterator = xNavigator.Select("Privileges/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("Privileges/Privilege");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Privilege.NewPrivilege(xIterator.Current.OuterXml));
                } while (xIterator.Current.MoveToNext() && xIterator.Current.Name == "Privilege");
            }

            //xIterator = xNavigator.Select("Privileges/AppSettings");
            //if (xIterator.MoveNext())
            //{
            //    this._appSettingList = AppSettingList.NewAppSettingList(xIterator.Current.OuterXml, pageId);
            //}

            xIterator = xNavigator.Select("Privileges/Controls");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _ctrlList = ControlList.NewControlList(xIterator.Current.OuterXml, pageId);
        }

        #endregion

        #region Filter Classes

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                string id = string.Empty;
                Privilege priv = null;
                //Filter is a int
                //Coverity Bug Fix CID 11472 
                if (filter != null && filter is string)
                    id = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is Privilege)
                    priv = ((Privilege)item);
                if (priv != null && priv.ID == id)
                    return true;
                return false;
            }
        }

        #endregion
    }
}

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
    public class ControlSetting : BusinessBase<ControlSetting>
    {
        #region Constant

        public const string PROPERTY_NAME_ID = "ID";
        public const string PROPERTY_NAME_FRIENDLYNAME = "FriendlyName";

        #endregion

        #region Variables

        private string _id = string.Empty;
        private string _friendlyName = string.Empty;
        private string _description = string.Empty;
        private PrivilegeList _privsList = new PrivilegeList();
        private AppSettingList _appSettingList = new AppSettingList();
        private ControlList _ctrlList;

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
            set
            {
                _id = value;
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

        /// <summary>
        /// List of privileges associated to the current page.
        /// </summary>
        public PrivilegeList PrivilegeList
        {
            get { return _privsList; }
            set { _privsList = value; }
        }

        /// <summary>
        /// List of Application Settings associated to the current page.
        /// </summary>
        public AppSettingList AppSettingList
        {
            get { return _appSettingList; }
            set { _appSettingList = value; }
        }

        public ControlList CtrlList
        {
            get { return _ctrlList; }
            set { _ctrlList = value; }
        }


        #endregion

        #region Factory methods

        /// <summary>
        /// Create a ControlSetting by given xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ControlSetting NewControlSetting(string xml, string pageId)
        {
            return new ControlSetting(xml, pageId);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        private ControlSetting(string xml, string pageID)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("ControlSetting/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    _id = xIterator.Current.Value;
                    // Setting the controlsetting ID being processed.
                    ControlIdChangeUtility.SelectedControlSettingID = _id;
                }

            xIterator = xNavigator.Select("ControlSetting/FriendlyName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _friendlyName = xIterator.Current.Value;

            xIterator = xNavigator.Select("ControlSetting/Description");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _description = xIterator.Current.Value;

            xIterator = xNavigator.Select("ControlSetting/Privileges");
            if (xIterator.MoveNext())
            {
                this._privsList = PrivilegeList.NewPrivilegeList(xIterator.Current.OuterXml, _id);
            }

            xIterator = xNavigator.Select("ControlSetting/Controls");
            if (xIterator.MoveNext())
            {
                this._ctrlList = ControlList.NewControlList(xIterator.Current.OuterXml, pageID);
            }


            xIterator = xNavigator.Select("ControlSetting/AppSettings");
            if (xIterator.MoveNext())
            {
                this._appSettingList = AppSettingList.NewAppSettingList(xIterator.Current.OuterXml, _id);
            }

        }

        public ControlSetting()
        {
            
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
            builder.Append("<ControlSetting>");
            builder.Append("<ID>" + this._id + "</ID>");
            builder.Append(_privsList.UpdateSelf(type));
            builder.Append(_ctrlList.UpdateSelf(type));
            builder.Append(_appSettingList.UpdateSelf(type));
            builder.Append("</ControlSetting>");
            return builder.ToString();
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Returns a list privileges that is available to select with given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public PrivilegeList GetAvailablePrivList(PrivilegeList fullPrivList)
        {
            return this._privsList.GetAvailablePrivList(fullPrivList);
        }

        /// <summary>
        /// Returns a list privileges that already exists in given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public PrivilegeList GetCurrentPrivList(PrivilegeList fullPrivList)
        {
            return this._privsList.GetCurrentPrivList(fullPrivList);
        }

        /// <summary>
        /// Returns a list controls that is available to select 
        /// </summary>
        /// <returns></returns>
        public ControlList GetAvailableCtrlList(ControlList fullCtrlList)
        {
            ControlList availableCtrlList = new ControlList();
            //Coverity Bug Fix CID 11659 
            if (fullCtrlList != null)
            {
                foreach (Control ctrl in fullCtrlList)
                {
                    if (!_ctrlList.Contains(ctrl))
                        availableCtrlList.Add(ctrl);
                }
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
            //Coverity Bug Fix CID 11659 
            if (fullCtrlList != null)
            {
                foreach (Control ctrl in fullCtrlList)
                {
                    if (_ctrlList.Contains(ctrl))
                        currentCtrlList.Add(ctrl);
                }
            }
            return currentCtrlList;
        }


        /// <summary>
        /// Returns an AppSetting list that is available to select with given AppSettingList.
        /// </summary>
        /// <returns></returns>
        public AppSettingList GetAvailableAppSettingList(AppSettingList fullAppSettingList)
        {
            return this._appSettingList.GetAvailableAppSettingList(fullAppSettingList);
        }

        /// <summary>
        /// Returns a list privileges that already exists in given PrivilegeList.
        /// </summary>
        /// <returns></returns>
        public AppSettingList GetCurrentAppSettingList(AppSettingList fullAppSettingList)
        {
            return this._appSettingList.GetCurrentAppSettingList(fullAppSettingList);
        }

        /// <summary>
        /// Create the ControlSetting node of tree.
        /// </summary>
        /// <returns></returns>
        public Node CreateControlSettingNode()
        {
            Node controlSettingNode = new Node();
            controlSettingNode.Text = ID;
            controlSettingNode.DataKey = this;
            return controlSettingNode;
        }


        #endregion
    }
}

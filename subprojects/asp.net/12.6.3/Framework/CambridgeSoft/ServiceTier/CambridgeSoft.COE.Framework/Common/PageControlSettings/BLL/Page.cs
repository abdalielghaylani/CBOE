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
    public class Page : BusinessBase<Page>
    {

        #region Variables

        private string _id = string.Empty;
        private string _friendlyName = string.Empty;
        private string _description = string.Empty;
        private ControlSettingList _controlSettingList = new ControlSettingList();
        //private List<PrivilegeList> _privsList = new List<PrivilegeList>();
        //private AppSettingList _appsettingList = new AppSettingList();
        private ControlList _ctrlList;

        #endregion

        #region Properties

        /// <summary>
        /// Page Identifier
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
        /// List of privileges associated to the current page.
        /// </summary>
        //public List<PrivilegeList> ListOfPrivilegeList
        //{
        //    get { return _privsList; }
        //    set { _privsList = value; }
        //}

        /// <summary>
        /// List of Application Settings associated to the current page.
        /// </summary>
        //public AppSettingList AppSettingList
        //{
        //    get { return _appsettingList; }
        //    set { _appsettingList = value; }
        //}

        public ControlSettingList CtrlSettingList
        {
            get { return _controlSettingList; }
            set { _controlSettingList = value; }
        }

        public ControlList CtrlList
        {
            get { return _ctrlList; }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Create a Page by given xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Page NewPage(string xml, COEPageControlSettings.Type type)
        {
            return new Page(xml, type);
        }

        #endregion

        #region Methods

        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Get a ControlSetting by given id.
        /// </summary>
        /// <param name="controlSettingId"></param>
        /// <returns></returns>
        public ControlSetting GetControlSettingById(string controlSettingId)
        {
            foreach (ControlSetting cSetting in this._controlSettingList)
            {
                if (cSetting.ID == controlSettingId)
                {
                    return cSetting;
                    break;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove the given ControlSetting 
        /// </summary>
        /// <param name="controlSetting"></param>
        public void RemoveControlSetting(ControlSetting controlSetting)
        {
            ControlSettingList result = new ControlSettingList();
            foreach (ControlSetting cSetting in this._controlSettingList)
            {
                if (cSetting.ID != controlSetting.ID)
                    result.Add(cSetting);
            }
            this._controlSettingList = result;
        }

        /// <summary>
        /// Add or update the given ControlSetting.
        /// </summary>
        /// <param name="controlSetting"></param>
        public void AddORUpdateControlSetting(ControlSetting controlSetting)
        {
            ControlSetting cSetting = this.GetControlSettingById(controlSetting.ID);
            if (cSetting != null)
                RemoveControlSetting(cSetting);
            this._controlSettingList.Add(controlSetting);
        }

        /// <summary>
        /// Build this into custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            if (this._controlSettingList != null && this._controlSettingList.Count > 0)
            {
                builder.Append("<Page>");
                builder.Append("<ID>" + this._id + "</ID>");
                if (type == COEPageControlSettings.Type.Master)
                {
                    builder.Append("<FriendlyName>" + this._friendlyName + "</FriendlyName>");
                    builder.Append("<Description>" + this._friendlyName + "</Description>");
                    builder.Append(_ctrlList.UpdateSelf(type));
                }
                else if (type == COEPageControlSettings.Type.Custom)
                {
                    builder.Append(_controlSettingList.UpdateSelf(type));
                }
                builder.Append("</Page>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Create page node of tree.
        /// </summary>
        /// <returns></returns>
        public Node CreatePageNode()
        {
            Node pageNode = new Node();
            pageNode.Text = _friendlyName;
            pageNode.DataKey = this;
            if (this.CtrlSettingList != null)
            {
                foreach (ControlSetting controlSetting in this.CtrlSettingList)
                {
                    pageNode.Nodes.Add(controlSetting.CreateControlSettingNode());
                }
            }
            return pageNode;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Read data from xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        private Page(string xml, COEPageControlSettings.Type type)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Page/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("Page/FriendlyName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _friendlyName = xIterator.Current.Value;

            xIterator = xNavigator.Select("Page/Description");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _description = xIterator.Current.Value;

            if (type == COEPageControlSettings.Type.Custom)
            {
                XPathNodeIterator xIteratorControlSetting = xNavigator.Select("Page/ControlSettings");
                if (xIteratorControlSetting.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        this.CtrlSettingList = ControlSettingList.NewControlSettingList(xIteratorControlSetting.Current.OuterXml, _id);
            }
            else if (type == COEPageControlSettings.Type.Master)
            {
                xIterator = xNavigator.Select("Page/Controls");
                if (xIterator.MoveNext())
                    if (!string.IsNullOrEmpty(xIterator.Current.Value))
                        _ctrlList = ControlList.NewControlList(xIterator.Current.OuterXml, _id);

            }
        }
      
        #endregion
    }
}

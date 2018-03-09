using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.IO;
using Csla;
using Infragistics.WebUI.UltraWebNavigator;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    [Serializable()]
    public class PageList : BusinessListBase<PageList, Page>
    {
        #region Variables

        [NonSerialized]
        private FilteredBindingList<Page> _filteredList;
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

        #endregion

        #region Constructors

        private PageList()
        {
            if (_filteredList == null)
                _filteredList = new FilteredBindingList<Page>(this);
        }

        /// <summary>
        /// Create this object by given xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        private PageList(string xml, COEPageControlSettings.Type type)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Pages/Page");

            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Page.NewPage(xIterator.Current.OuterXml, type));
                } while (xIterator.Current.MoveToNext());
            }

            xIterator = xNavigator.Select("Pages/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Return a Pagelist created given the input xml.
        /// </summary>
        /// <param name="xml">XML snippet that represets the object</param>
        /// <returns>A list of pages</returns>
        public static PageList NewPageList(string xml, COEPageControlSettings.Type type)
        {
            return new PageList(xml, type);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a Page by given id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Page GetByID(string Id)
        {
            if (_filteredList == null) _filteredList = new FilteredBindingList<Page>(this);
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
        /// Build this into xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf(COEPageControlSettings.Type type)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Pages>");
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf(type));
            builder.Append("</Pages>");
            return builder.ToString();
        }

        /// <summary>
        /// Create page nodes of tree.
        /// </summary>
        /// <returns></returns>
        public Nodes CreatePageNodes()
        {
            Nodes nodes = new Nodes();
            if (_filteredList != null)
            {
                foreach (Page page in _filteredList)
                {
                    nodes.Add(page.CreatePageNode());
                }
            }
            return nodes;
        }

        #endregion

        #region Filter Classes

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                string id = string.Empty;
                Page page = null;
                //Filter is a int
                //Coverity Bug Fix CID 11471 
                if (filter != null && filter is string)
                    id = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item != null && item is Page)
                    page = ((Page)item);
                if (page != null && page.ID == id)
                    return true;
                return false;
            }
        }

        #endregion
    }
}

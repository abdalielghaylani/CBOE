using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    public class COEChemDrawToolbar : CompositeControl, ICOEGenerableControl, ICOEReadOnly
    {
        /*<configInfo>
         * <cssClass></cssClass>
         * <selectedItemCssClass></selectedItemCssClass>
         * <itemCssClass></itemCssClass>
         * <items>
         *  <item id="lalala" selected="true">
         *      <text>Draw Structure</text>
         *      <value>0</value>
         *      <cssClass></cssClass>
         *  </item>
         *  <item id="lelele" selected="false">
         *      <value></value>
         *      <cssClass></cssClass>
         * </items>
         * </configInfo>
         * 
         * 
         */

        #region Properties
        private List<COEChemDrawToolbarItem> _items;
        private HtmlInputHidden _selectedValue;
        private string _defaultSelectedValue = string.Empty;
        private string _selectedItemCssClass = "selected";
        private COEEditControl _editControl = COEEditControl.NotSet;
        public string SelectedItemCssClass
        {
            get
            {
                return this._selectedItemCssClass;
            }
            set
            {
                _selectedItemCssClass = value;
            }
        }

        public string Value
        {
            get
            {
                return this.GetData().ToString();
            }
            set
            {
                this.PutData(value);
            }
        }

        public List<COEChemDrawToolbarItem> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<COEChemDrawToolbarItem>();

                foreach (COEChemDrawToolbarItem currentItem in this._items)
                {
                    if (currentItem.Value == _selectedValue.Value)
                        currentItem.Selected = true;
                    else
                        currentItem.Selected = false;
                }

                return _items;
            }
            set
            {
                _items = value;
            }
        }
        #endregion
        #region Constructors
        public COEChemDrawToolbar()
        {
            _selectedValue = new HtmlInputHidden();
            _selectedValue.ID = "SelectedValue";
        }
        #endregion
        #region Events
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.Controls.Add(_selectedValue);

            foreach (COEChemDrawToolbarItem currentItem in this.Items)
            {
                this.Controls.Add(currentItem);
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "COEChemDrawToolbar"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "COEChemDrawToolbar",
                @"
function COEChemDrawToolbar_SelectItem(listItem) 
{
    chemdrawToolbar = ChemDrawToolbar_GetToolbar(listItem);
   
    for(currentItemIndex = 0; currentItemIndex < chemdrawToolbar.childNodes.length; currentItemIndex++) 
    {
        if(chemdrawToolbar.childNodes[currentItemIndex] == listItem) 
        {
           chemdrawToolbar.childNodes[currentItemIndex].id = '" + this._selectedItemCssClass + @"'; 
           listItem.parentNode.parentNode.getElementsByTagName('input')[0].value = chemdrawToolbar.childNodes[currentItemIndex].getElementsByTagName('img')[0].getAttribute('value');
        }
        else
           listItem.parentNode.childNodes[currentItemIndex].id = '';
    }
    return false;
}
function ChemDrawToolbar_GetToolbar(listItem)
{
    return listItem.parentNode;
}

function COEChemDrawToolbar_GetSelectedItemValue(chemdrawToolbar)
{
    return chemdrawToolbar.getElementsByTagName('input')[0].value;
}

function COEChemDrawToolbar_SetDrawStructureItemValue(chemdrawToolbar,value)
{
   var oldValue = value;
   switch(value)
    {
        case '1':value = 2; break;
        case '2':value = 1; break;
        default :value = 0; break;
    }
    
    var items = chemdrawToolbar.childNodes[1];
    for(currentItemIndex = 0; currentItemIndex < items.childNodes.length; currentItemIndex++) 
    {
        items.childNodes[currentItemIndex].id = '';
    }
    if(items.childNodes.length != 0)
		items.childNodes[value].id = '" + this._selectedItemCssClass + @"';
    chemdrawToolbar.getElementsByTagName('input')[0].value = oldValue * -1;
}

function COEChemDrawToolbar_SetSelectedItemValue(chemdrawToolbar, value)
{
    var items = chemdrawToolbar.childNodes[1];

    for(currentItemIndex = 0; currentItemIndex < items.childNodes.length; currentItemIndex++) 
    {
        if(items.childNodes[currentItemIndex].attributes('value').value == value) 
        {
           items.childNodes[currentItemIndex].id = '" + this._selectedItemCssClass + @"'; 
           chemdrawToolbar.getElementsByTagName('input')[0].value = value;
        }
        else
           items.childNodes[currentItemIndex].id = '';
    }
}
",
                    true);
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
             {
               foreach (COEChemDrawToolbarItem currentItem in this.Items)
                 {
                    currentItem.Enabled = false;
                 }
             }
            writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            this._selectedValue.RenderControl(writer);

            writer.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Ul);
            foreach (COEChemDrawToolbarItem currentItem in this.Items)
                currentItem.RenderControl(writer);

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return _selectedValue.Value;
        }

        public void PutData(object data)
        {
            this._selectedValue.Value = data.ToString();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if (style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode selectedItemCssClass = xmlData.SelectSingleNode("//COE:SelectedItemCSSClass", manager);
            if (selectedItemCssClass != null && selectedItemCssClass.InnerText.Length > 0)
                this._selectedItemCssClass = selectedItemCssClass.InnerText;

            XmlNode defaultSelectedValueNode = xmlData.SelectSingleNode("//COE:DefaultSelectedValue", manager);
            if (defaultSelectedValueNode != null && defaultSelectedValueNode.InnerText.Length > 0)
                _selectedValue.Value = _defaultSelectedValue = defaultSelectedValueNode.InnerText;

            XmlNode listItemsNode = xmlData.SelectSingleNode("//COE:ListItems", manager);
            if (listItemsNode != null && !string.IsNullOrEmpty(listItemsNode.InnerXml))
            {
                foreach (XmlNode currentItemXml in listItemsNode.SelectNodes("./COE:Item", manager))
                {
                    COEChemDrawToolbarItem currentItem = new COEChemDrawToolbarItem();
                    currentItem.SelectedCssClass = this.SelectedItemCssClass;

                    if (currentItemXml.Attributes["text"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["text"].Value))
                        currentItem.Title = currentItemXml.Attributes["text"].Value;

                    if (currentItemXml.Attributes["value"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["value"].Value))
                        currentItem.Value = currentItemXml.Attributes["value"].Value;

                    if (currentItemXml.Attributes["url"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["url"].Value))
                        currentItem.URL = currentItemXml.Attributes["url"].Value;

                    if (currentItemXml.Attributes["alt"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["alt"].Value))
                        currentItem.Alt = currentItemXml.Attributes["alt"].Value;

                    this.Items.Add(currentItem);
                }
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultSelectedValue;
            }
            set
            {
                _defaultSelectedValue = value;
            }
        }

        #endregion

        #region ICOEReadOnly Members
        /// <summary>
        /// EditControl Property implementation.
        /// </summary>
        public COEEditControl COEReadOnly
        {
            get
            {
                return _editControl;
            }
            set
            {
                _editControl = value;
            }
        }

        #endregion

    }

    public class COEChemDrawToolbarItem : WebControl
    {
        #region variables
        private string _value;
        private string _url;
        private string _alt;
        private bool _selected;
        private string _title;
        private string _selectedCssClass;
        #endregion

        #region properties

        internal string SelectedCssClass
        {
            get
            {
                return _selectedCssClass;
            }
            set
            {
                _selectedCssClass = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }


        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        public string Alt
        {
            get
            {
                return _alt;
            }
            set
            {
                _alt = value;
            }
        }

        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        #endregion

        #region Events
        protected override void Render(HtmlTextWriter writer)
        {
            if (Selected)
                writer.AddAttribute(HtmlTextWriterAttribute.Id, SelectedCssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Value, this.Value);

            if (this.Enabled)
            {
                if (this.Parent != null && !string.IsNullOrEmpty(((WebControl)this.Parent).Attributes["onclick"]))
                    this.Attributes["onclick"] = ((WebControl)this.Parent).Attributes["onclick"];

                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "COEChemDrawToolbar_SelectItem(this); " + this.Attributes["onclick"]);
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "cursor:pointer");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Li);

            // Coverity Fix CID - 11824
            using (HtmlImage image = new HtmlImage())
            {
                image.Src = this.URL;
                image.Alt = this.Alt;
                image.Attributes["title"] = this.Title;
                image.Attributes["value"] = this.Value;
                image.RenderControl(writer);
            }

            writer.RenderEndTag();
        }
        #endregion
    }

}

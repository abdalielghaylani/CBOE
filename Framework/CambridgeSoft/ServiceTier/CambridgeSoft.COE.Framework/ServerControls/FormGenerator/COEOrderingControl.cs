using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    [ToolboxData("<{0}:COEOrderingControl runat=server></{0}:COEOrderingControl>")]
    public class COEOrderingControl : CompositeControl, ICOEGenerableControl 
    {
        #region Variables
        private OrderByCriteria _orderByCriteria;
        private OrderByCriteria _availableOrderByCriteria;
        private HtmlAnchor _displayOrderingControlLink;
        private OrderingPreferences _preferencesControl;
        private string _xmlDataAsString;
        #endregion

        #region Properties
        public string Value
        {
            get { return _orderByCriteria.ToString(); }
        }
        #endregion

        #region Constructor
        public COEOrderingControl()
        {
            _orderByCriteria = new OrderByCriteria();
            _availableOrderByCriteria = new OrderByCriteria();
            _displayOrderingControlLink = new HtmlAnchor();
            _preferencesControl = new OrderingPreferences();
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return _preferencesControl.UpdateOrderByCriteria();
        }

        public void PutData(object data)
        {
            if(!(data is OrderByCriteria))
            {
                if(data is string)
                {
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(data.ToString());
                        _orderByCriteria = new OrderByCriteria(xmlDocument);
                    }
                    catch(Exception)
                    {
                        throw new Exception("COEOrderingControl expects to be bound to a " + typeof(OrderByCriteria).FullName + " object type");
                    }
                }
                else
                    throw new Exception("COEOrderingControl expects to be bound to a " + typeof(OrderByCriteria).FullName + " object type");
            }
            else
                _orderByCriteria = (OrderByCriteria) data;

            _preferencesControl.OrderByCriteria = _orderByCriteria;
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            _xmlDataAsString = xmlDataAsString;
        }

        private void LoadFromXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);
            manager.AddNamespace("COE", xmlDocument.DocumentElement.NamespaceURI);
            manager.AddNamespace("OBC", "COE.OrderByCriteria");

            XmlNode xmlData = xmlDocument.SelectSingleNode("//COE:fieldConfig", manager);
            // Coverity Fix CID - 13141, 10903 (from local server)
            if (xmlData != null)
            {
                XmlNode width = xmlData.SelectSingleNode("./COE:Width", manager);
                if (width != null && !string.IsNullOrEmpty(width.InnerText))
                {
                    this.Width = new Unit(width.InnerText);
                }

                XmlNode cssClass = xmlData.SelectSingleNode("./COE:CSSClass", manager);
                if (cssClass != null && !string.IsNullOrEmpty(cssClass.InnerText))
                {
                    this.CssClass = cssClass.InnerText;
                }

                XmlNode height = xmlData.SelectSingleNode("./COE:Height", manager);
                if (height != null && !string.IsNullOrEmpty(height.InnerText))
                {
                    this.Height = new Unit(height.InnerText);
                }

                XmlNode orderingOptions = xmlData.SelectSingleNode("./OBC:orderByCriteria", manager);
                if (orderingOptions != null && !string.IsNullOrEmpty(orderingOptions.InnerXml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(orderingOptions.OuterXml);
                    _availableOrderByCriteria = new OrderByCriteria(doc);
                }
            }
        }

        public string DefaultValue
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        #endregion

        #region CompositeControl Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LoadFromXml();
        }
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            _displayOrderingControlLink.ID = "DisplayOrderingControlLink";
            this.Controls.Add(_displayOrderingControlLink);
            _displayOrderingControlLink.InnerText = "Display Ordering Options";
            _displayOrderingControlLink.Attributes.Add("class", "OrderingControlLink");
            _preferencesControl.ID = "PreferencesControl";
            this.Controls.Add(_preferencesControl);
            _preferencesControl.AvailableOrderByCriteria = _availableOrderByCriteria;
            _displayOrderingControlLink.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            _preferencesControl.ShowingControlsIds.Add(_displayOrderingControlLink.ClientID);
            ChildControlsCreated = true;
        }
        #endregion
    }
    class OrderingPreferences : CompositeControl, INamingContainer, IPostBackDataHandler
    {
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _title = string.Empty;
        private string _footer = string.Empty;
        private string _contentsCssClass = "OrderingControlContents";
        private string _labelCssClass = "OrderingControlLabelCell";
        private string _listBoxCssClass = "OrderingControlListBox";
        private string _linkCssClass = "OrderingControlLink";

        private OrderByCriteria _orderByCriteria;
        private OrderByCriteria _availableOrderByCriteria;
        private List<string> _showingControlsIds = new List<string>();

        private HtmlSelect _availableOrderBys;
        private HtmlSelect _selectedOrderBys;
        #endregion

        #region Properties
        public OrderByCriteria OrderByCriteria
        {
            get
            {
                return _orderByCriteria;
            }
            set
            {
                _orderByCriteria = value;
            }
        }

        public List<string> ShowingControlsIds
        {
            get { return _showingControlsIds; }
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

        public string Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                _footer = value;
            }
        }

        public string ContentsCssClass
        {
            set
            {
                _contentsCssClass = value;
            }
        }

        public OrderByCriteria AvailableOrderByCriteria
        {
            get { return _availableOrderByCriteria; }
            set 
            {
                _availableOrderByCriteria = value;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }
        #endregion

        #region Constructor
        public OrderingPreferences()
        {
            _orderByCriteria = new OrderByCriteria();
            _availableOrderByCriteria = new OrderByCriteria();
            _selectedOrderBys = new HtmlSelect();
            _availableOrderBys = new HtmlSelect();
        }
        #endregion

        #region Life cycle Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            Page.RegisterRequiresPostBack(this);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page,
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);


            string moveSelectedElementsJS = @"
            function SelectElements(fromListBoxID, toListBoxID)
            {
                var fromListBox = document.getElementById(fromListBoxID);
                var toListBox = document.getElementById(toListBoxID);
                var i = 0;

                while(i < fromListBox.length)
                {
                    if(fromListBox.options[i].selected)
                    {
                        element = toListBox.appendChild(fromListBox.options[i]);
                        element.innerText = element.innerText + ' (' + element.value.split('_')[1] + ')';
                    }
                    else
                        i++;
                }
            }

            function UnSelectElements(fromListBoxID, toListBoxID)
            {
                var fromListBox = document.getElementById(fromListBoxID);
                var toListBox = document.getElementById(toListBoxID);
                var i = 0;

                while(i < fromListBox.length)
                {
                    if(fromListBox.options[i].selected)
                    {
                        element = toListBox.appendChild(fromListBox.options[i]);
                        element.innerText = element.innerText.replace(' (' + element.value.split('_')[1] + ')', '');
                    }
                    else
                        i++;
                }
            }

            function MoveUpSelected(listBoxID)
            {
                var listBox = document.getElementById(listBoxID);
                var selectedItems = 0;
                for(i=0;i<listBox.length;i++)
                {
                    if(listBox.options[i].selected)
                        selectedItems++;
                    if(selectedItems > 1)
                    {
                        alert('Select a single item for re-odering')
                        return;
                    }
                }
                if(listBox.selectedIndex < 0)
                {
                    alert('Select an item to move');
                    return;
                }
                if(listBox.selectedIndex > 0)
                {
                    listBox.insertBefore(listBox.options[listBox.selectedIndex], listBox.options[listBox.selectedIndex - 1]);
                }
            }

            function MoveDownSelected(listBoxID)
            {
                var listBox = document.getElementById(listBoxID);
                var selectedItems = 0;
                for(i=0;i<listBox.length;i++)
                {
                    if(listBox.options[i].selected)
                        selectedItems++;
                    if(selectedItems > 1)
                    {
                        alert('Select a single item for re-odering')
                        return;
                    }
                }
                if(listBox.selectedIndex < 0)
                {
                    alert('Select an item to move');
                    return;
                }
                if(listBox.selectedIndex < listBox.length - 1)
                {
                    if(listBox.selectedIndex <= listBox.length - 2)
                        listBox.insertBefore(listBox.options[listBox.selectedIndex], listBox.options[listBox.selectedIndex + 2]);
                    else
                        listBox.appendChild(listBox.options[listBox.selectedIndex]);
                }
            }

            function ChangeDirection(listBoxID)
            {
                var listBox = document.getElementById(listBoxID);
                if(listBox.selectedIndex < 0)
                {
                    alert('Select some items to change their ordering direction');
                    return;
                }
                for(i=0;i<listBox.length;i++)
                {
                    if(listBox.options[i].selected)
                    {
                        var currentDirection = listBox.options[i].value.split('_')[1];
                        var nextDirection = currentDirection == 'ASC'?'DESC':'ASC';
                        listBox.options[i].value = listBox.options[i].value.replace(currentDirection, nextDirection);
                        listBox.options[i].innerText = listBox.options[i].innerText.replace(currentDirection, nextDirection);
                    }
                }
            }
            ";
            
            ScriptManager scManager = ScriptManager.GetCurrent(this.Page);
            if(scManager != null)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(COEOrderingControl), "MoveSelectedElementsJS", moveSelectedElementsJS, true);
            }
            else
            {
                if(!Page.ClientScript.IsClientScriptBlockRegistered(typeof(COEOrderingControl), "MoveSelectedElementsJS"))
                    Page.ClientScript.RegisterClientScriptBlock(typeof(COEOrderingControl), "MoveSelectedElementsJS", moveSelectedElementsJS, true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            string onSubmitSelectAllJS = @"
            var listBox = document.getElementById('" + _selectedOrderBys.ClientID + @"');
            if(listBox != null && typeof(listBox) != 'undefinded' && listBox.length > 0)
            {
                for(i=0;i<listBox.length;i++)
                {
                    listBox.options[i].selected = true;
                }
            }
            ";

            ScriptManager scManager = ScriptManager.GetCurrent(this.Page);
            if(scManager != null)
            {
                ScriptManager.RegisterOnSubmitStatement(this.Page, typeof(COEOrderingControl), "OnSubmitSelectAllJS", onSubmitSelectAllJS);
            }
            if(!Page.ClientScript.IsOnSubmitStatementRegistered(typeof(COEOrderingControl), "OnSubmitSelectAllJS"))
                Page.ClientScript.RegisterOnSubmitStatement(typeof(COEOrderingControl), "OnSubmitSelectAllJS", onSubmitSelectAllJS);

            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            _selectedOrderBys.ID = "SelectedListBox";
            _availableOrderBys.ID = "AvailableListBox";
            this.Controls.Add(_availableOrderBys);
            this.Controls.Add(_selectedOrderBys);
            _selectedOrderBys.Size = 8;
            _availableOrderBys.Size = 8;
            _selectedOrderBys.Multiple = _availableOrderBys.Multiple = true;
            this.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            this.Style.Add(HtmlTextWriterStyle.Display, "none");
            
            foreach(OrderByCriteria.OrderByCriteriaItem item in _orderByCriteria.Items)
            {
                _selectedOrderBys.Items.Add(new ListItem(string.Format("{0} ({1})", item.ResultCriteriaItem.Alias, item.Direction), item.ID + "_" + item.Direction));
            }

            foreach(OrderByCriteria.OrderByCriteriaItem availableOrderCriteria in _availableOrderByCriteria.Items)
            {
                if(!IsCriteriaSelected(availableOrderCriteria))
                    _availableOrderBys.Items.Add(new ListItem(availableOrderCriteria.ResultCriteriaItem.Alias, availableOrderCriteria.ID + "_" + availableOrderCriteria.Direction));
            }
            
            ChildControlsCreated = true;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            this.RenderHeader(writer);
            this.RenderBody(writer);
            this.RenderFooter(writer);
        }

        protected override object SaveControlState()
        {
            object[] states = new object[2];
            states[0] = base.SaveControlState();
            states[1] = _orderByCriteria;
            return states;
        }

        protected override void LoadControlState(object savedState)
        {

            object[] states = savedState as object[];
            if(states != null && states.Length == 2)
            {
                base.LoadControlState(states[0]);
                _orderByCriteria = (OrderByCriteria) states[1];
            }
            else
                base.LoadControlState(savedState);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string initScript = @"
        <script type='text/javascript'>
            YAHOO.namespace('COEFormGenerator');

            function showControls" + this.ClientID + @"() {
                document.getElementById('" + this.ClientID + @"').style.visibility='hidden';
                document.getElementById('" + this.ClientID + @"').style.display='none';
                ";
            initScript += @"
            }

            function hideControls" + this.ClientID + @"() {
                document.getElementById('" + this.ClientID + @"').style.visibility='visible';
                document.getElementById('" + this.ClientID + @"').style.display='block';
                ";
            initScript += @"
            }

		    function initOrderingPref() {
			    YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @" = new YAHOO.widget.Panel('" + this.ClientID + @"', { width:'380px', 
                                                                                                              modal:false, 
                                                                                                              draggable:true,
                                                                                                              visible:false, 
                                                                                                              constraintoviewport:true,
                                                                                                              context:['" + Parent.ClientID + @"','tl','tl']} );
			    YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".render(document.body.form);

                YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".beforeHideEvent.subscribe(showControls" + this.ClientID + @");
                YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".beforeHideEvent.subscribe(ShowChemDraws);
                YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".beforeShowEvent.subscribe(hideControls" + this.ClientID + @");
                YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".beforeShowEvent.subscribe(HideChemDraws);
	    		";
            foreach(string showingControlId in _showingControlsIds)
            {
                initScript += @"
                YAHOO.util.Event.addListener('" + showingControlId + "', 'click', YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @".show, YAHOO.COEFormGenerator.orderingPreferences" + this.ClientID + @", true);";
            }

            initScript += @"

    		}

		    YAHOO.util.Event.addListener(window, 'load', initOrderingPref);
        </script>";

            writer.Write(initScript);
        }
        #endregion

        #region Private Methods
        private bool IsCriteriaSelected(OrderByCriteria.OrderByCriteriaItem availableOrderCriteria)
        {
            foreach(OrderByCriteria.OrderByCriteriaItem item in _orderByCriteria.Items)
            {
                if(item.ID == availableOrderCriteria.ID)
                {
                    return true;
                }
            }
            return false;
        }

        protected void RenderHeader(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "hd");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(string.IsNullOrEmpty(_title) ? "Ordering Preferences" : _title);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        protected void RenderFooter(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ft");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(string.IsNullOrEmpty(_footer) ? "Choose the way your query should be ordered." : _footer);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        protected void RenderBody(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "bd");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            this.RenderTable(writer);
            writer.RenderEndTag();
        }

        private void RenderTable(HtmlTextWriter writer)
        {
            _selectedOrderBys.Attributes.Add("class", _listBoxCssClass);
            _availableOrderBys.Attributes.Add("class", _listBoxCssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, _contentsCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            RenderPreferenceHeaderRow(writer);
            RenderPreferenceRow(_availableOrderBys, _selectedOrderBys, writer);
            writer.RenderEndTag();
        }

        private void RenderPreferenceHeaderRow(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "4");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            // Coverity Fix CID - 11828
            using (HtmlAnchor changeDirLink = new HtmlAnchor())
            {
                changeDirLink.Attributes.Add("class", _linkCssClass);
                changeDirLink.Attributes.Add("onclick", "javascript:ChangeDirection('" + _selectedOrderBys.ClientID + "');");
                changeDirLink.InnerText = "Swap direction";
                changeDirLink.RenderControl(writer);
            }

            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, _labelCssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Available");
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("");
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, _labelCssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Selected");
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        private void RenderPreferenceRow(Control availableOrderBys, Control selectedOrderBys, HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, _labelCssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            availableOrderBys.RenderControl(writer);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            // Coverity Fix CID - 11829
            using (Button selectButton = new Button())
            {
                selectButton.Text = "-->";
                selectButton.OnClientClick = "SelectElements('" + _availableOrderBys.ClientID + "', '" + _selectedOrderBys.ClientID + "');return false;";
                selectButton.RenderControl(writer);
                writer.WriteBreak();
                //Coverity Fix CID 11829 ASV
                using (Button unSelectButton = new Button())
                {
                    unSelectButton.Text = "<--";
                    unSelectButton.OnClientClick = "UnSelectElements('" + _selectedOrderBys.ClientID + "', '" + _availableOrderBys.ClientID + "');return false;";
                    unSelectButton.RenderControl(writer);
                }
                writer.RenderEndTag();
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, _labelCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            selectedOrderBys.RenderControl(writer);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            // Resource Leak , Reference - Coverity Fix CID - 11829
            using (Button upButton = new Button())
            {
                upButton.Text = "Up";
                upButton.OnClientClick = "MoveUpSelected('" + _selectedOrderBys.ClientID + "');return false;";
                upButton.RenderControl(writer);
                writer.WriteBreak();
                using (Button downButton = new Button()) //Coverity Fix CID 11829 ASV
                {
                    downButton.Text = "Down";
                    downButton.OnClientClick = "MoveDownSelected('" + _selectedOrderBys.ClientID + "');return false;";
                    downButton.RenderControl(writer);
                }
                writer.RenderEndTag();
            }


            writer.RenderEndTag();
        }

        internal OrderByCriteria UpdateOrderByCriteria()
        {
            _orderByCriteria.Items.Clear();
            foreach(ListItem item in _selectedOrderBys.Items)
            {
                for(int i = 0; i < _availableOrderByCriteria.Items.Count; i++)
                {
                    if(item.Value.Split('_')[0] == _availableOrderByCriteria.Items[i].ID.ToString())
                    {
                        _orderByCriteria.Items.Add(_availableOrderByCriteria.Items[i]);
                        _orderByCriteria.Items[_orderByCriteria.Items.Count - 1].OrderIndex = _selectedOrderBys.Items.IndexOf(item) + 1;
                        string direction = item.Text.Split('(')[1].Replace(")", "");
                        _orderByCriteria.Items[_orderByCriteria.Items.Count - 1].Direction = (OrderByCriteria.OrderByDirection) Enum.Parse(typeof(OrderByCriteria.OrderByDirection), direction);
                        break;
                    }
                }
            }
            return _orderByCriteria;
        }
        #endregion

        #region IPostBackDataHandler Members

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if(postCollection[_selectedOrderBys.UniqueID] != null)
            {
                foreach(string val in postCollection[_selectedOrderBys.UniqueID].Split(','))
                {
                    foreach(OrderByCriteria.OrderByCriteriaItem availableOrderCriteria in _availableOrderByCriteria.Items)
                    {
                        if(val.Split('_')[0] == availableOrderCriteria.ID.ToString())
                        {
                            _selectedOrderBys.Items.Add(new ListItem(string.Format("{0} ({1})", availableOrderCriteria.ResultCriteriaItem.Alias, val.Split('_')[1]), val));
                            break;
                        }
                    }
                }
                return true;
            }
            else
                return false;
        }

        public void RaisePostDataChangedEvent()
        {
            
        }

        #endregion
    }
}

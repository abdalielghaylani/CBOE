using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Collections;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    public class COEStateControl : CompositeControl, ICOEGenerableControl, ICOELabelable
    {
        #region Variables
        private Label _lit = new Label();
        private HtmlInputHidden _value;
        private List<State> _states;
        private DisplayType _displayType;

        public DisplayType CurrentDisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
        private string _itemCssClass;

        public string ItemCssClass
        {
            get
            {
                return _itemCssClass;
            }
            set
            {
                _itemCssClass = value;
            }
        }
	
        #endregion

        #region Properties
        public List<State> States
        {
            get
            {
                if(_states == null)
                    _states = new List<State>();

                return _states;
            }
            set
            {
                _states = value;
            }
        }
        public virtual string Value
        {
            get
            {
                return _value.Value;
            }
            set
            {
                _value.Value = value;
            }
        }
        #endregion

        #region Constructors
        public COEStateControl()
        {
            _value = new HtmlInputHidden();
            _value.ID = "value";

            this.CurrentDisplayType = DisplayType.ImageButton;
            this.Enabled = true;
        }
        #endregion

        #region Server Control Events
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.Controls.Clear();
            this.Controls.Add(_value);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                if (this.LabelStyles != null)
                {
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        _lit.Style.Add(key, (string)this.LabelStyles[key]);
                    }
                }
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.RenderControl(writer);
            }

            this.RenderChildren(writer);

            RenderControl(writer);
        }

        private void RenderControl(HtmlTextWriter writer)
        {
            switch(CurrentDisplayType)
            {
                case DisplayType.DropDown:
                    // Coverity Fix CID - 11830
                    using (DropDownList dropdownList = new DropDownList())
                    {
                        dropdownList.DataSource = this.States;
                        dropdownList.DataTextField = "Text";
                        dropdownList.DataValueField = "Value";
                        dropdownList.DataBind();
                        dropdownList.SelectedValue = this.Value;
                        dropdownList.Enabled = this.Enabled;
                        dropdownList.CssClass = this.CssClass;

                        if (this.Enabled)
                            dropdownList.Attributes["OnChange"] = @"document.getElementById('" + this._value.ClientID + @"').value = this.options[this.selectedIndex].value;";

                        dropdownList.RenderControl(writer);
                    }
                    break;
                default:
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    for(int currentStateIndex = 0; currentStateIndex < this.States.Count; currentStateIndex++)
                    {
                        State currentState = this.States[currentStateIndex];

                        //coverity fix
                        using (COEImageButton image = new COEImageButton())
                        {
                            image.ID = currentState.Value;
                            image.ImageUrl = currentState.Url;
                            image.CssClass = this._itemCssClass;
                            image.Attributes["value"] = currentState.Value;
                            image.Text = image.Attributes["text"] = currentState.Text;
                            image.Style.Add(HtmlTextWriterStyle.Display, Value == currentState.Value ? "inline-block" : "none");
                            image.Enabled = this.Enabled;

                            if (this.Enabled)
                            {
                                if (this is IPostBackEventHandler)
                                    image.Attributes["onclick"] = this.Page.ClientScript.GetPostBackEventReference(this, "Click");
                                else
                                    image.Attributes["onclick"] = @"OnImageButtonClicked(this, '" + this._value.ClientID + "', '" + this.States[(currentStateIndex + 1) % this.States.Count].Value + "');";
                            }
                            image.RenderControl(writer);
                        }
                    }
                    writer.RenderEndTag();
                    break;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(COEStateControl), "COEStateControl"))
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(COEStateControl), "COEStateControl",
                @"
                function OnImageButtonClicked(sender, hiddenFieldId, value)
                {
                    document.getElementById(hiddenFieldId).value = value;
                    for(currentControlIndex = 0; currentControlIndex < sender.parentElement.children.length; currentControlIndex++)
                    {
                        currentControl = sender.parentElement.children[currentControlIndex];

                        if(currentControl.getAttribute('value') == value)
                            currentControl.style.display = 'inline-block';
                        else
                            currentControl.style.display = 'none';
                    }
                }
;",
                true);
            }
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return Value;
        }

        public void PutData(object data)
        {
            Value = data.ToString();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if(style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++)
                {
                    if(styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if(cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode selectedItemCssClass = xmlData.SelectSingleNode("//COE:ItemCSSClass", manager);
            if(selectedItemCssClass != null && selectedItemCssClass.InnerText.Length > 0)
                this._itemCssClass = selectedItemCssClass.InnerText;

            /*XmlNode defaultSelectedValueNode = xmlData.SelectSingleNode("//COE:DefaultSelectedValue", manager);
            if(defaultSelectedValueNode != null && defaultSelectedValueNode.InnerText.Length > 0)
                _selectedValue.Value = _defaultSelectedValue = defaultSelectedValueNode.InnerText;*/

            XmlNode listItemsNode = xmlData.SelectSingleNode("//COE:States", manager);
            if(listItemsNode != null && !string.IsNullOrEmpty(listItemsNode.InnerXml))
            {
                foreach(XmlNode currentItemXml in listItemsNode.SelectNodes("./COE:State", manager))
                {
                    State currentItem = new State();

                    if(currentItemXml.Attributes["text"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["text"].Value))
                        currentItem.Text = currentItemXml.Attributes["text"].Value;

                    if(currentItemXml.Attributes["value"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["value"].Value))
                        currentItem.Value = currentItemXml.Attributes["value"].Value;

                    if(currentItemXml.Attributes["url"] != null && !string.IsNullOrEmpty(currentItemXml.Attributes["url"].Value))
                        currentItem.Url = currentItemXml.Attributes["url"].Value;

                    this.States.Add(currentItem);
                }
            }

            if(States.Count > 0 && string.IsNullOrEmpty(this._value.Value))
                this._value.Value = this.States[0].Value;

            XmlNode readOnlyNode = xmlData.SelectSingleNode("//COE:ReadOnly", manager);
            if(readOnlyNode != null && !string.IsNullOrEmpty(readOnlyNode.InnerText))
                this.Enabled = !bool.Parse(readOnlyNode.InnerText);

            XmlNode displayType = xmlData.SelectSingleNode("//COE:DisplayType", manager);
            if(displayType != null && !string.IsNullOrEmpty(displayType.InnerText))
                this.CurrentDisplayType = (DisplayType)Enum.Parse(typeof(DisplayType), displayType.InnerText);

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;
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

        public class State
        {
            #region variables
            private string _value;
            private string _text;
            private string _url;
            #endregion

            #region properties
            public string Url
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


            public string Text
            {
                get
                {
                    return _text;
                }
                set
                {
                    _text = value;
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

            #region Constructors
            public State()
            {
            }

            public State(string key, string value)
            {
                _value = key;
                _text = value;
            }

            public State(string key, string value, string url)
                : this(key, value)
            {
                _url = url;
            }
            #endregion
        }

        public enum DisplayType
        {
            DropDown,
            ImageButton,
        }

        #region ICOELabelable Members

        public string Label
        {
            get
            {
                if (ViewState[Constants.Label_VS] != null)
                    return (string)ViewState[Constants.Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Constants.Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[Constants.LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[Constants.LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[Constants.LabelStyles_VS] = value;
            }
        }


        #endregion
    }
}

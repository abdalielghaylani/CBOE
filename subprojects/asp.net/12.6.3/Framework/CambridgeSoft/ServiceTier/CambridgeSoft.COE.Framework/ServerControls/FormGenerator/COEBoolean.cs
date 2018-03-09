using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI;
using System.Collections;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    public class COEBoolean : CompositeControl, ICOEGenerableControl, ICOELabelable, ICOEDesignable
    {
        #region variables
        ICOEGenerableControl _controlToShow = null;
        string _trueCSSClass = string.Empty;
        string _falseCSSClass = string.Empty;

        string _trueValue = string.Empty;
        string _falseValue = string.Empty;
        string _dataSourceId = string.Empty;

        private DisplayType _currentDisplayType;
        #endregion

        #region Properties
        public DisplayType CurrentDisplayType
        {
            get
            {
                return _currentDisplayType;
            }
            set
            {
                _currentDisplayType = value;

                switch(_currentDisplayType)
                {
                    case DisplayType.DropDown:
                    COEDropDownList coeDropDownList = new COEDropDownList();
                    coeDropDownList.ID = this.ID;
                    coeDropDownList.AutoPostBack = true;
                    _controlToShow = coeDropDownList;
                    _controlToShow.LoadFromXml("<configInfo><fieldConfig><CSSClass>COEDropDownListStatus</CSSClass><CSSLabelClass>COELabel</CSSLabelClass><dataTextField>Value</dataTextField><dataValueField>Key</dataValueField></fieldConfig></configInfo>");
                    break;
                    default:
                    COEImageSwitch coeImageSwitch = new COEImageSwitch();
                    coeImageSwitch.ID = this.ID;
                    _controlToShow = coeImageSwitch;

                    _controlToShow.LoadFromXml("<configInfo><fieldConfig><PushedImageURL></PushedImageURL><PoppedImageURL></PoppedImageURL><PoppedValue>2</PoppedValue><PushedValue>1</PushedValue></fieldConfig></configInfo>");
                    break;
                }
            }
        }
	
        #endregion
        #region Constructors
        public COEBoolean()
        {
            this.CurrentDisplayType = DisplayType.DropDown;
            this._trueValue = "true";
            this._falseValue = "false";
        }
        #endregion
        #region ICOEGenerableControl Members

        public string Value
        {
            get {
                return this.GetData().ToString();
            }
        }

        public object GetData()
        {
            EnsureChildControls();
            return _controlToShow.GetData();

        }

        public void PutData(object data)
        {
            EnsureChildControls();
            _controlToShow.PutData(data);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode xmlNode = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                _labelCSSClass = xmlNode.InnerText;

            xmlNode = xmlData.SelectSingleNode("//COE:Width", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.Width = new Unit(xmlNode.InnerText);
            }

            xmlNode = xmlData.SelectSingleNode("//COE:Height", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.Height = new Unit(xmlNode.InnerText);
            }

            /*xmlNode = xmlData.SelectSingleNode("//COE:TrueCSSClass", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                this._trueCSSClass = xmlNode.InnerText;

            xmlNode = xmlData.SelectSingleNode("//COE:FalseCSSClass", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                this._falseCSSClass = xmlNode.InnerText;

            xmlNode = xmlData.SelectSingleNode("//COE:DisplayType", manager);

            _trueValue = xmlData.SelectSingleNode("//COE:TrueValue", manager).InnerText;
            _falseValue = xmlData.SelectSingleNode("//COE:FalseValue", manager).InnerText;
            */

            _dataSourceId = string.Empty;
            xmlNode = xmlData.SelectSingleNode("//COE:DataSourceId", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                _dataSourceId = xmlNode.InnerText;

            string displayType = string.Empty;
            XmlNode displayTypeNode = null;
            xmlNode = xmlData.SelectSingleNode("//COE:DisplayType", manager);  //Coverity Fix CID 13134 ASV
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                displayTypeNode = xmlData.SelectSingleNode("//COE:DisplayType", manager);

            if(displayTypeNode != null)
            {
                displayType = displayTypeNode.InnerText;
            }
            if (!string.IsNullOrEmpty(displayType)) //Coverity Fix CID 13134 ASV
                this.CurrentDisplayType = (DisplayType)Enum.Parse(typeof(DisplayType),displayType);

            ((WebControl)_controlToShow).ID = this.ID;

            _controlToShow.LoadFromXml(xmlDataAsString);
            
            /*xmlNode = xmlData.SelectSingleNode("//COE:CustomConfig", manager);
            if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
                _controlToShow.LoadFromXml(string.Format("<fieldConfig xmlns=\"{0}\">{1}</fieldConfig>", xmlNode.ParentNode.NamespaceURI, xmlNode.InnerXml));
             */
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

        #region ICOELabelable Members
        private string _labelCSSClass;
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

        #region ICOEDesignable Members

        public XmlNode GetConfigInfo()
        {
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";

            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            if (this.Style.Count > 0)
            {
                XmlNode style = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns);
                style.InnerText = this.Style.Value;
                xmlData.FirstChild.AppendChild(style);
            }

            if (this.Width != Unit.Empty)
            {
                XmlNode width = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Width", xmlns);
                width.InnerText = this.Width.ToString();
                xmlData.FirstChild.AppendChild(width);
            }

            if (this.Height != Unit.Empty)
            {
                XmlNode height = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Height", xmlns);
                height.InnerText = this.Height.ToString();
                xmlData.FirstChild.AppendChild(height);
            }

            if (!string.IsNullOrEmpty(this.CssClass))
            {
                XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
                cssClass.InnerText = this.CssClass;
                xmlData.FirstChild.AppendChild(cssClass);
            }

            if (!string.IsNullOrEmpty(this._dataSourceId))
            {
                XmlNode dataSourceId = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "DataSourceId", xmlns);
                dataSourceId.InnerText = this._dataSourceId;
                xmlData.FirstChild.AppendChild(dataSourceId);
            }


            return xmlData.FirstChild;
        }

        #endregion

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.Controls.Add((Control)this._controlToShow);
            if (_controlToShow is DataBoundControl)
            {
                ((DataBoundControl)_controlToShow).DataSourceID = this._dataSourceId;
                //((DataBoundControl)_controlToShow).DataBind();

            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                Label _lit = new Label();
                if (this.LabelStyles != null)
                {
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        _lit.Style.Add(key, (string)this.LabelStyles[key]);
                    }
                }
                _lit.CssClass = _labelCSSClass;
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.Text = this.Label;
                _lit.RenderControl(writer);
            }

            base.Render(writer);
        }

        public enum DisplayType
        {
            DropDown,
            ImageButton,
        }
    }
}

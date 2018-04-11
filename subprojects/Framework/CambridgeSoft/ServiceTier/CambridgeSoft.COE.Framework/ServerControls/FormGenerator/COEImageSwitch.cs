using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Xml;
using System.Web.UI;
using System.Collections;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    public class COEImageSwitch : ImageButton, ICOEGenerableControl, ICOELabelable
    {
        #region Variables
        private string _labelCSSClass;
        private string _pushedImageURL;
        private string _poppedImageURL;
        private string _pushedText;
        private string _poppedText;
        private string _pushedValue;
        private string _poppedValue;
        #endregion

        #region Properties
        private bool Pushed
        {
            get
            {
                if (this.Value == PushedValue)
                    return true;

                return false;
            }
            set
            {
                this.Value = value ? PushedValue : PoppedValue;
            }
        }
        public string PoppedValue
        {
            get { return _poppedValue; }
            set { _poppedValue = value; }
        }


        public string PushedValue
        {
            get { return _pushedValue; }
            set { _pushedValue = value; }
        }


        public string PoppedText
        {
            get { return _poppedText; }
            set { _poppedText = value; }
        }


        public string PushedText
        {
            get { return _pushedText; }
            set { _pushedText = value; }
        }

        public string PushedImageURL
        {
            get { return _pushedImageURL; }
            set { _pushedImageURL = value; }
        }

        public string PoppedImageURL
        {
            get { return _poppedImageURL; }
            set { _poppedImageURL = value; }
        }

        public bool IsPushed
        {
            get { return this.Pushed; }
            set { this.Pushed = value; }
        }

        public string Value
        {
            set {
                this.Attributes["value"] = value;
            }
            get
            {
                if (this.Attributes["value"] == null)
                    return this._poppedValue;

                return this.Attributes["value"];
            }
        }

        #endregion

        #region Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.RegisterRequiresControlState(this);
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (this.IsPushed)
            {
                if (string.IsNullOrEmpty(this.PushedImageURL))
                {
                    this.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(COEBoolean), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.ThumbUp.png");                    
                }
                else
                {
                    this.ImageUrl = this.PushedImageURL;
                }               
            }
            else
            {
                if (string.IsNullOrEmpty(this.PoppedImageURL))
                {
                    this.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(COEBoolean), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.ThumbDown.png");                    
                }
                else 
                {
                    this.ImageUrl = this.PoppedImageURL;
                }            
            }

            base.OnPreRender(e);
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

        protected override void OnClick(ImageClickEventArgs e)
        {
            this.IsPushed = !this.IsPushed;
            base.OnClick(e);
        }

        /*protected override void OnClick(EventArgs e)
        {
            this.IsPushed = !this.IsPushed;
            base.OnClick(e);
        }*/
        #endregion

        #region ICOEGenerableControl Members
        public object GetData()
        {
            return this.Value;
        }

        public void PutData(object data)
        {
            this.Value = data.ToString();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode xmlNode = xmlData.SelectSingleNode("//COE:Style", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                string[] styles = xmlNode.InnerText.Split(new char[1] { ';' });
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

            xmlNode = xmlData.SelectSingleNode("//COE:PushedImageURL", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PushedImageURL = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:PoppedImageURL", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PoppedImageURL = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:PoppedText", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PoppedText = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:PushedText", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PushedText = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:PoppedValue", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PoppedValue = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:PushedValue", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.PushedValue = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                this.CssClass = xmlNode.InnerText;

            xmlNode = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                _labelCSSClass = xmlNode.InnerText;

            xmlNode = xmlData.SelectSingleNode("//COE:Enable", manager);
            if (xmlNode != null && xmlNode.InnerText.Length > 0)
                this.Enabled = bool.Parse(xmlNode.InnerText);

            /*this.OnClientClick = string.Format(@"
                    this.attributes['pushed'] = !this.attributes['pushed']; 
                    if(this.attributes['pushed']) 
                    {{
                        this.style.backgroundImage = 'url({0})';
                        this.value = {1};
                    }} 
                    else 
                    {{
                        this.style.backgroundImage = 'url({2})';
                        this.value = {3}
                    }}", this.PushedImageURL, this.PushedText, this.PoppedImageURL, this.PoppedText);*/

            this.CausesValidation = false;
        }

        public string DefaultValue
        {
            get
            {
                return bool.TrueString;
            }
            set
            {
            }
        }
        #endregion

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

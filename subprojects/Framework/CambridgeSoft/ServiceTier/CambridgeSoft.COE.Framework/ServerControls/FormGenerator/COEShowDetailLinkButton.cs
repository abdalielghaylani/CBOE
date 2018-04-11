using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COEShowDetailLinkButton : LinkButton, ICOEGenerableControl, ICOEDisplayModeChanger {
        #region ICOEGenerableControl Members
        public object GetData() {
            string finalUrl = string.Empty;
            if(string.IsNullOrEmpty(this.PostBackUrl))
                finalUrl = this.PostBackUrl = DefaultValue;

            if(PostBackUrl.IndexOf("?") > 0)
                finalUrl = PostBackUrl.Substring(0, PostBackUrl.IndexOf("?"));

            string newQuery = string.Empty;

            if(new Uri(PostBackUrl).Query.Length > 0) {
                string[] queries = PostBackUrl.Substring(PostBackUrl.IndexOf("?") + 1, PostBackUrl.Length - PostBackUrl.IndexOf("?") - 1).Split('&');
                
                for(int i = 0; i < queries.Length; i++) {
                    if(!queries[i].Contains("CurrentIndex")) {
                        if(newQuery.Length > 1) {
                            newQuery += "&";
                        }
                        newQuery += queries[i];
                    }
                }
                if(newQuery.Length > 0)
                    newQuery += "&";

            }
            newQuery += "CurrentIndex=" + CurrentIndex;
            finalUrl = finalUrl + "?" + newQuery;
            return finalUrl;
        }

        public void PutData(object data) {
            //this.PostBackUrl = data.ToString();

            //this.PostBackUrl = builder.Uri.
            //this.CurrentIndex = int.Parse(currentIndexParam);
        }

        public void LoadFromXml(string xmlDataAsString) {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if(style != null && style.InnerText.Length > 0) {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++) {
                    if(styles[i].Length > 0) {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if(width != null && width.InnerText.Length > 0) {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if(height != null && height.InnerText.Length > 0) {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode txt = xmlData.SelectSingleNode("//COE:Text", manager);
            if(txt != null && txt.InnerText.Length > 0) {
                this.Text = txt.InnerText;
            }
            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if(cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;
        }

        public string DefaultValue {
            get {
                return Page.Request.Url.ToString();
            }
            set {
            }
        }
        #endregion

        #region ICOEDisplayModeChanger Members
        public int CurrentIndex {
            get
            {
                if(ViewState["CurrentIndex"] == null)
                    ViewState["CurrentIndex"] = 0;
                return (int) ViewState["CurrentIndex"];
            }
            set
            {
                ViewState["CurrentIndex"] = value;
            }
        }
        #endregion

        void ShowDetail(object sender, EventArgs e) {
            this.PostBackUrl = (string) GetData();
            Page.Response.Redirect(this.PostBackUrl);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Click += new EventHandler(ShowDetail);
        }
    }
}

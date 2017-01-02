using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Controls.MarkHitControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Xml;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Data;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COEMarkHitButton : MarkHitButton, ICOEGenerableControl, ICOEHitMarker {
        private string _cssClass;

        public event MarkingHitHandler MarkingHit;

        public string ColumnIDBindingExpression {
            get
            {
                return ViewState["ColumnIDBindingExpression"] as string;
            }
            set
            {
                ViewState["ColumnIDBindingExpression"] = value;
            }
        }

        #region ICOEGenerableControl Members
        public object GetData() {
            return this.Marked;
        }

        public void PutData(object data) {
            if(data is bool)
                this.Marked = (bool) data;
            else if (data is decimal)
            {
                this.Marked = ((decimal)data == 1);
            }
            else
            {
                string val = data.ToString();
                if (string.IsNullOrEmpty(val))
                    val = bool.FalseString;
                this.Marked = bool.Parse(val);
            }
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

            XmlNode renderMode = xmlData.SelectSingleNode("//COE:RenderMode", manager);
            if(renderMode != null && !string.IsNullOrEmpty(renderMode.InnerText))
            {
                this.RenderingMode = (RenderMode) Enum.Parse(typeof(RenderMode), renderMode.InnerText);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if(cssClass != null && cssClass.InnerText.Length > 0)
                _cssClass = cssClass.InnerText;

            XmlNode columnIDBindingExpression = xmlData.SelectSingleNode("//COE:ColumnIDBindingExpression", manager);
            if(columnIDBindingExpression != null && columnIDBindingExpression.InnerText.Length > 0)
                this.ColumnIDBindingExpression = columnIDBindingExpression.InnerText;
        }

        public string DefaultValue {
            get { return string.Empty; }
            set { }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Marking += new EventHandler(MarkButton_Click);
        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            base.CssClass = _cssClass;
        }

        void MarkButton_Click(object sender, EventArgs e) {
            MarkHitEventArgs args = new MarkHitEventArgs(ColumnIDValue, Marked, ColumnIDBindingExpression);

            CommandEventArgs cmdEventArgs = new CommandEventArgs("ApplyMarkAction", args);
            if(MarkingHit != null)
            {
                MarkingHit(this, args);
            }

            RaiseBubbleEvent(sender, cmdEventArgs);
            
        }
    }
}

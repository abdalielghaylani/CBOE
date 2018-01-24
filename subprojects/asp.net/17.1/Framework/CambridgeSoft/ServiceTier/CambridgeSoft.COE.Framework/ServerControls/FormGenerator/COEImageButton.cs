using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    public class COEImageButton : ImageButton, ICOEGenerableControl
    {

        #region variables
        private string _text;

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
        #endregion

        #region Control Life-cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            /*
            Label
            float:left;
            padding-right:5px;
            vertical-align:top;
            Image
            float:right;
            */

            if (this.Enabled)
            {
                if (string.IsNullOrEmpty(this.OnClientClick))
                    this.OnClientClick = this.Attributes["onclick"];

                if (string.IsNullOrEmpty(this.OnClientClick))
                    this.OnClientClick = ((System.Web.UI.Page)Context.CurrentHandler).ClientScript.GetPostBackClientHyperlink(this, "");

                this.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "hand");

            }
            else
            {
                this.OnClientClick = "return false;";
                this.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "arrow");
            }

            this.Style.Add(System.Web.UI.HtmlTextWriterStyle.Overflow, "hidden");

            /*if(!this.Enabled)
                this.Attributes.Add("disabled", "true");*/

            foreach(string currentStyleKey in Style.Keys)
                writer.AddStyleAttribute(currentStyleKey, Style[currentStyleKey]);

            foreach(String currentKey in Attributes.Keys)
                writer.AddAttribute(currentKey, Attributes[currentKey]);

            //The following attributes don't get rendered with previous loop, so we had to add them by hand
            writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, this.CssClass);

            writer.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Div);

            if(!string.IsNullOrEmpty(this.Text))
            {
                Label label = new Label();
                label.Text = this.Text;
                label.Style.Value = "padding-right:5px; padding-top:5px;";
                label.RenderControl(writer);
            }

            if(!string.IsNullOrEmpty(this.ImageUrl))
            {
                writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Src, this.ResolveUrl(this.ImageUrl));
                writer.AddStyleAttribute("position", "absolute");
                writer.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return null;
        }

        public void PutData(object data)
        {
            ;
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            return;
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
    }
}

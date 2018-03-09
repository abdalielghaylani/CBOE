using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ToolboxData("<{0}:COEDynamicImage runat=server></{0}:COEDynamicImage>")]
    public class COEDynamicImage : Image, ICOEGenerableControl, ICOELabelable, ICOERequireable
    {
        #region Variables

        private string _defaultValue = string.Empty;
        private string _handlerName = string.Empty;
		private string _mimeType = string.Empty;

        #endregion

        #region COEGenerableControl Members
        /// <summary>
        /// Allows to set if it is checked or not by default.
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }

        public object GetData()
        {
            return this.ImageUrl;
        }

        public void PutData(object data)
        {
            if(data.GetType() == typeof(byte[]))
            {
                Guid guid = Guid.NewGuid();
                this.Attributes.Add("GUID", guid.ToString());
                //ViewState[guid.ToString()] = data as string;
                this.Context.Cache[guid.ToString()] = data as byte[];
                this.ImageUrl = _handlerName + guid.ToString();

                if(this._mimeType != null)
                {
                    this.ImageUrl += "&MimeType=" + _mimeType;
                }

                this.Visible = true;
            }
            else
                this.Visible = false;
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///     &lt;fieldConfig&gt;
        ///         &lt;Style&gt;border-color:blue;&lt;/Style&gt;
        ///         &lt;Height&gt;&lt;/Height&gt;
        ///         &lt;Width&gt;&lt;/Width&gt;
        ///         &lt;Text&gt;&lt;/Text&gt;
        ///         &lt;CssClass&gt;&lt;/CssClass&gt;
        ///         &lt;CSSLabelClass&gt;&lt;/CSSLabelClass&gt;
        ///         &lt;ID&gt;32&lt;/ID&gt;
        ///     &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <Height></Height>
             *  <Width></Width>
             *  <ID>32</ID>
             * </fieldConfig>
             */
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

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode handlerName = xmlData.SelectSingleNode("//COE:HandlerName", manager);
            if (handlerName != null && handlerName.InnerText.Length > 0)
                _handlerName = handlerName.InnerText;

			XmlNode mimeType = xmlData.SelectSingleNode("//COE:MimeType", manager);
			if (mimeType != null && mimeType.InnerText.Length > 0)
				_mimeType = mimeType.InnerText;


            //XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            //if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
            //    this.LabelAttributes["class"] = cssLabelClass.InnerText;

            //XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            //if (labelStyle != null && labelStyle.InnerText.Length > 0)
            //{
            //    this.LabelAttributes["style"] = labelStyle.InnerText;
            //}
        }
        #endregion


        #region ICOELabelable Members
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary>
        public string Label
        {
            get
            {
                return "";
            }
            set
            {
                //this.Text = value;
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

        #region ICOERequireable Members
        /// <summary>
        /// Gets or sets if the control is required to have a value.
        /// </summary>
        public bool Required
        {
            get
            {
                if (ViewState["Required"] != null)
                    return (bool)ViewState["Required"];
                else
                    return false;
            }
            set
            {
                ViewState["Required"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable css style
        /// </summary>
        public string RequiredStyle
        {
            get
            {
                if (ViewState[Constants.RequiredStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[Constants.RequiredLabelStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredLabelStyle_VS] = value;
            }
        }

        #endregion


    }

    public class ServeImage : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request != null && context.Cache != null)
            {
                if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                {
                    byte[] img = (byte[])context.Cache[context.Request.QueryString["ID"]];
					string mimeType = "image/gif";
					if (context.Request.QueryString["MimeType"] != null)
					{
						mimeType = context.Request.QueryString["MimeType"];
					}
                    // For now is harcoded the type of supported image.
					HttpContext.Current.Response.ContentType = mimeType;
                    HttpContext.Current.Response.OutputStream.Write(img, 0, img.Length);
                }
            }
        }

        #endregion
    }
}


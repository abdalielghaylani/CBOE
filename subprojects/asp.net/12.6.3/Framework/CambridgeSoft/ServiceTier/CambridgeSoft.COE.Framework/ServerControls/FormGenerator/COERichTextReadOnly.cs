using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Xml;
using System.Collections;
using Infragistics.WebUI.WebHtmlEditor;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This is the read only version of <see cref="COETextArea"/>.
    /// </para>
    /// <para>
    /// The COETextArea class accepts every TextBox property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text of the text area?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;label&gt;Comments&lt;/label&gt;
    ///     &lt;bindingExpression&gt;PropertyList[@Name='COMMENTS' | Value]&lt;/bindingExpression&gt;
    ///     &lt;validationRuleList&gt;
    ///       &lt;validationRule validationRuleName="textLength" errorMessage="The length must be between 0 and 60"&gt;
    ///         &lt;params&gt;
    ///           &lt;param name="min" value="0"/&gt;
    ///           &lt;param name="max" value="60"/&gt;
    ///         &lt;/params&gt;
    ///       &lt;/validationRule&gt;
    ///     &lt;/validationRuleList&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSClass&gt;COETextAreaLongView&lt;/CSSClass&gt;
    ///       &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///       &lt;ID&gt;CommentsTextArea&lt;/ID&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;height&gt;50px&lt;/height&gt;
    ///       &lt;width&gt;653px&lt;/width&gt;
    ///       &lt;top&gt;125px&lt;/top&gt;
    ///       &lt;left&gt;15px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Text property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COERichTextReadOnly runat=server></{0}:COERichTextReadOnly>")]
    public class COERichTextReadOnly : Label, ICOEGenerableControl, ICOERequireable, ICOELabelable, ICOEHelpContainer
    {
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private Image _helpImage = null;
        #endregion

        #region Constructor
		public COERichTextReadOnly()
		{
            //this.TextMode = TextBoxMode.MultiLine;
            //this.ReadOnly = true;
            this.Height = 50;
        }
        #endregion

        #region COEGenerableControl Members
        /// <summary>
        /// Allows to set the default text.
        /// </summary>
        public string DefaultValue {
            get {
                return _defaultValue;
            }
            set {
                _defaultValue = value;
            }
        }

        /// <summary>
        /// <para>Gets the Text of the control.</para>
        /// </summary>
        /// <returns>A string with the control's text.</returns>
        public object GetData() {
            return this.Text;
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public void PutData(object data) {
			if (data != null)
			{
				//this.Text = GetHTML(data.ToString());
				//this.Text = data.ToString();
				SautinSoft.RtfToHtml.Converter convert = new SautinSoft.RtfToHtml.Converter();
				convert.Serial = "10008387053";
				ArrayList myImages = new ArrayList();
				//myImages.Add(new SautinImage("image1",null));
				convert.HtmlParts = SautinSoft.RtfToHtml.eHtmlParts.Html_body;
				this.Text = convert.ConvertString(data.ToString(), myImages);

			}
				//this.Text = "";
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COETextAreaLongView&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
        ///   &lt;ID&gt;CommentsTextArea&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString) {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <Height></Height>
             *  <Width></Width>
             *  <TextMode></TextMode>
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

			//XmlNode textMode = xmlData.SelectSingleNode("//COE:TextMode", manager);
			//if (textMode != null && textMode.InnerText.Length > 0)
			//{
			//    switch (textMode.InnerText.ToLower())
			//    {
			//        case "multiline":
			//            this.TextMode = TextBoxMode.MultiLine;
			//            break;
			//        case "password":
			//            this.TextMode = TextBoxMode.Password;
			//            break;
			//        default:
			//            this.TextMode = TextBoxMode.SingleLine;
			//            break;
			//    }
			//}

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if(labelStyle != null && labelStyle.InnerText.Length > 0) {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++) {
                    if(styles[i].Length > 0) {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }
        }
        #endregion

        #region ICOELabelable Members
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary>
        public string Label {
            get {
                if(ViewState[Constants.Label_VS] != null)
                    return (string)ViewState[Constants.Label_VS];
                else
                    return string.Empty;
            }
            set {
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


        #region Life Cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.Text = this.Label;
                _lit.RenderControl(writer);
			}
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
            if(this.ShowHelp)
            {
                _helpImage = new Image();
                _helpImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ICOEGenerableControl), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.help.png");
                _helpImage.ID = this.ClientID + "HelpImage";
                _helpImage.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
                _helpImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _helpImage.Attributes.Add("onclick", "showHide(YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip, " + _helpImage.ClientID + ");");
                _helpImage.RenderControl(writer);
                string yahooScript = "<script language='javascript' type='text/javascript'>YAHOO.namespace('COEFormGenerator');" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip = new YAHOO.widget.Tooltip(\"" + _helpImage.ClientID + "Tooltip\", " +
                                     "{ context:\"" + _helpImage.ClientID + "\", text:\"" + FormatText(this.HelpText) + "\"," +
                                     "showdelay: 20000," +
                                     "hidedelay: 20000," +
                                     "visible:false});" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip.isToBeShown = true;" +
                                     "</script>";
                writer.Write(yahooScript);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(this.ShowHelp)
                this.AddYUIScripts();
        }

		//protected override void OnPreRender(EventArgs e)
		//{
		//    Page.ClientScript.RegisterClientScriptInclude("wysiwyg", "JSAndCSS/wysiwyg.js");
		//    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "Test", @"addRichText('ctl00_ContentPlaceHolder_FormGenerator_DetailForm_0_Preparation0');", true);
		//    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "Test2", @"populateRichText('ctl00_ContentPlaceHolder_FormGenerator_DetailForm_0_Preparation0',true);", true);
		//}   
		#endregion

        #region ICOEHelpContainer Members
        public bool ShowHelp
        {
            get
            {
                if(ViewState["ShowHelp"] != null)
                    return (bool) ViewState["ShowHelp"];
                else
                    return false;
            }
            set
            {
                ViewState["ShowHelp"] = value;
            }
        }

        public string HelpText
        {
            get
            {
                if(ViewState["HelpText"] != null)
                    return (string) ViewState["HelpText"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }
        #endregion

        #region Private Methods
        private void AddYUIScripts()
        {
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page,
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);

            string showHideJS = @"
                function showHide(tooltip, contextid)
                {
                    var originalshowdelay = tooltip.cfg.getProperty('showdelay');
                    if(tooltip.isToBeShown)
                    {
                        tooltip.isToBeShown = false;
                        tooltip.cfg.setProperty('showdelay', 0);
                        tooltip.doShow(null, document.getElementById(contextid));
                    }
                    else
                    {
                        tooltip.isToBeShown = true;
                        tooltip.hide();
                    }

                    tooltip.cfg.setProperty('showdelay', originalshowdelay);
                }";
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(ICOEGenerableControl), "showHideJS"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(ICOEGenerableControl), "showHideJS", showHideJS, true);
            }
        }

        private string FormatText(string txt)
        {
            return "<span style='font-size:9px;'>" + Page.Server.HtmlEncode(txt) + "</span>";
        }
        #endregion

	//    public string GetHTML(string richText)
	//    {
	//        System.Windows.Forms.RichTextBox _rtfSource = new System.Windows.Forms.RichTextBox();
	//        string strReturn = "<div>";
	//        System.Drawing.Color clrForeColor = System.Drawing.Color.Black;
	//        System.Drawing.Color clrBackColor = System.Drawing.Color.Black;
	//        System.Drawing.Font fntCurrentFont = _rtfSource.Font;
	//        System.Windows.Forms.HorizontalAlignment altCurrent = System.Windows.Forms.HorizontalAlignment.Left;

	//        _rtfSource.Text = richText;

	//        for (int intPos = 0; intPos < _rtfSource.Text.Length - 1; intPos++)
	//        {
	//             _rtfSource.Select(intPos, 1);
    
	//            //Forecolor
	//            if (intPos == 0)
	//            {   
	//                strReturn += @"<span style=""color:" + HtmlColorFromColor(_rtfSource.SelectionColor) + @"""/>";
	//                clrForeColor = _rtfSource.SelectionColor;
	//            }
	//            else
	//            {
	//                if (_rtfSource.SelectionColor != clrForeColor)
	//                {
	//                  strReturn += "</span>";
	//                  strReturn += @"<span style=""color:" + HtmlColorFromColor(_rtfSource.SelectionColor) + @"""/>";
	//                  clrForeColor = _rtfSource.SelectionColor;
	//                }
	//            }

	//              //Background color
	//            if (intPos == 0)
	//            {
	//                strReturn += @"<span style=""background-color:" + HtmlColorFromColor(_rtfSource.SelectionBackColor) + @""">";
	//                clrBackColor = _rtfSource.SelectionBackColor;
	//            }
	//            else
	//            {
	//                if (_rtfSource.SelectionBackColor != clrBackColor)
	//                {
	//                  strReturn += "</span>";
	//                  strReturn += @"<span style=""background-color:" + HtmlColorFromColor(_rtfSource.SelectionBackColor) + @""">";
	//                  clrBackColor = _rtfSource.SelectionBackColor;
	//                }
	//            }

	//              //Font
	//            if (intPos == 0)
	//            {
	//                strReturn += @"<span style=""font:" + HtmlFontStyleFromFont(_rtfSource.SelectionFont) + @""">";
	//                fntCurrentFont = _rtfSource.SelectionFont;
	//            }
	//            else
	//            {
	//                if (_rtfSource.SelectionFont.GetHashCode().ToString() != fntCurrentFont.GetHashCode().ToString())
	//                {
	//                  strReturn += "</span>";
	//                  strReturn += @"<span style=""font:" + HtmlFontStyleFromFont(_rtfSource.SelectionFont) + @""">";
	//                  fntCurrentFont = _rtfSource.SelectionFont;
	//                }
	//            }
	//              //Alignment
	//            if (intPos == 0)
	//            {
	//                strReturn += @"<p style=""text-align:" + _rtfSource.SelectionAlignment.ToString() + @""">";
	//                altCurrent = _rtfSource.SelectionAlignment;
	//            }
	//            else
	//            {
	//                if (_rtfSource.SelectionAlignment != altCurrent)
	//                {
	//                  strReturn += "</p>";
	//                  strReturn += @"<p style=""text-align:" + _rtfSource.SelectionAlignment.ToString() + @""">";
	//                  altCurrent = _rtfSource.SelectionAlignment;
	//                }
	//            }
	//              strReturn += _rtfSource.Text.Substring(intPos, 1);	
	//        }

	//        //close all the spans
	//        strReturn += "</span>";
	//        strReturn += "</span>";
	//        strReturn += "</span>";
	//        strReturn += "</p>";
	//        strReturn += "</div>";
	//        strReturn = strReturn.Replace("\\r\\n", "<br />");
	//        strReturn = strReturn.Replace("\\par", "<br />");
	//        return strReturn;
	//    }


	//      //<summary>
	//      //Returns an HTML Formated Color string for the style from a system.drawing.color
	//      //</summary>
	//      //  <param name="clr">The color you wish to convert</param>
	//    private string HtmlColorFromColor(System.Drawing.Color clr)
	//    {
	//        string strReturn = string.Empty;

	//        if (clr.IsNamedColor) 
	//        {
	//          strReturn = clr.Name.ToLower();
	//            }
	//        else
	//        {
	//          strReturn = clr.Name;
	//          if (strReturn.Length > 6)
	//          {
	//            strReturn = strReturn.Substring(strReturn.Length - 6, 6);
	//          }
	//          strReturn = "#" + strReturn;
	//        }
	//        return strReturn;
	//    }



	//    //<summary>
	//    // Provides the font style per given font
	//    //</summary>
	//    //<param name="fnt">The font you wish to convert</param>
	//    private string HtmlFontStyleFromFont(System.Drawing.Font fnt) 
	//    {
	//        string strReturn = string.Empty;

	//        //style
	//        if (fnt.Italic)
	//          strReturn += "italic ";
	//        else
	//          strReturn += "normal ";

	//        //variant
	//        strReturn += "normal ";
		    
	//        //weight
	//        if (fnt.Bold)
	//          strReturn += "bold ";
	//        else
	//          strReturn += "normal ";

	//        //size
	//        strReturn += fnt.SizeInPoints + "pt/normal ";
		    
	//                //family
	//        strReturn += fnt.FontFamily.Name;
	//        return strReturn;
	//}


    }




}

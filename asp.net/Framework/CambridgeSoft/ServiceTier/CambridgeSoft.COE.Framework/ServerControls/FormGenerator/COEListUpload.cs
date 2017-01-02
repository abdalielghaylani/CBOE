using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    // <summary>
    /// <para>
    /// This class implements a FileUpload control to allow txt upload with a list of ids or an sdf file to search with.
    /// </para>
    /// <para>
    /// The COEListInput as ICOEGenerable control it provides GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text of the control?</item>
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
    ///   TO BE DONE
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEListInput runat=server></{0}:COEListInput>")]
    public class COEListUpload : FileUpload, ICOEGenerableControl, ICOELabelable, ICOERequireable
    {
        #region Variables
        private Label _lit;
        private TextBox _txt;

        private string _fileType = string.Empty;
        #endregion

        #region Properties
        public List<string> InputList
        {
            get
            {
                if (ViewState["InputList"] == null)
                    ViewState["InputList"] = new List<string>();

                return (List<string>)ViewState["InputList"];
            }
            set
            {
                ViewState["InputList"] = value;
            }
        }

        public string FileType
        {
            get
            {
                if (ViewState["FileType"] == null)
                    ViewState["FileType"] = string.Empty;

                return (string)ViewState["FileType"];
            }
            set
            {
                ViewState["FileType"] = value;
            }
        }

        #endregion

        #region Constructors
        public COEListUpload()
        {
            _lit = new Label();
            _txt = new TextBox();
            _txt.ID = "txtUpload";
        }
        #endregion

        #region Overriden Life cicle events
        protected override void OnLoad(EventArgs e)
        {

           base.OnLoad(e);

            if (this.PostedFile != null && this.PostedFile.InputStream.Length > 0)
            {
                _fileType = this.FileType = this.PostedFile.FileName.ToLower().Substring(this.PostedFile.FileName.Length - 3, 3);
                if (_fileType == "sdf" || _fileType == "csv" || _fileType == "txt")
                {
                    this.InputList = ParsingUtilities.ParseList(this.PostedFile.InputStream, _fileType);
                    _txt.Text = this.InputList.Count.ToString() + " structures read";
                }
            }


        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.RenderControl(writer);
                _txt.Style.Add(HtmlTextWriterStyle.Display, "block");
                _txt.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                _txt.Style.Add(HtmlTextWriterStyle.Width, "168px");
                _txt.RenderControl(writer);

                base.Style.Add(HtmlTextWriterStyle.Display, "block");
                base.Attributes.Add("onchange", "aspnetForm.submit();");
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
        }
        #endregion

        #region ICOEGenerableControl Members
        public object GetData()
        {
            if (this.InputList != null && this.InputList.Count > 0)
            {
                if (FileType == "sdf")
                    return string.Join("$$$$\r\n", InputList.ToArray());
                else
                    return string.Join(",", InputList.ToArray());
            }
            else
                return string.Empty;
        }

        public void PutData(object data)
        {
            if (data != null)
            {
                //Determine what to do
            }
        }

        public void LoadFromXml(string xmlDataAsString)
        {
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

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if (labelStyle != null && labelStyle.InnerText.Length > 0)
            {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }
        }

        public string DefaultValue
        {
            get { return null; }
            set { ; }
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
}

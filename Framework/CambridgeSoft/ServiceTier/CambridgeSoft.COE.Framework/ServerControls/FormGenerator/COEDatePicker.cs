using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using CambridgeSoft.COE.Framework.Properties;
using System.Collections;
using CambridgeSoft.COE.Framework.COEConfigurationService;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements an Infragistics' WebDateChooser control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEDatePicker class accepts every WebDateChooser property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// All dates are stored internally as Universal Time amd converted based on <see cref="Constants.DatesFormat"/>.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default date?</item>
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
    ///     &lt;formElement&gt;
    ///        &lt;label&gt;Synthesis Date&lt;/label&gt;
    ///        &lt;bindingExpression&gt;PropertyList[@Name='DATEENTERED' | Value]&lt;/bindingExpression&gt;
    ///        &lt;configInfo&gt;
    ///          &lt;fieldConfig&gt;
    ///          &lt;CSSClass&gt;COEDropDownList&lt;/CSSClass&gt;
    ///          &lt;CSSLabelClass&gt;COETextBox&lt;/CSSLabelClass&gt;
    ///          &lt;ID&gt;DateEnteredTextBox&lt;/ID&gt;
    ///          &lt;NullDateLabel&gt;Pick a date&lt;/NullDateLabel&gt;
    ///          &lt;Width&gt;150&lt;/Width&gt;
    ///          &lt;Height&gt;15&lt;/Height&gt;
    ///          &lt;FontSize&gt;11px&lt;/FontSize&gt;
    ///          &lt;ForeColor&gt;484848&lt;/ForeColor&gt;
    ///          &lt;BackColor&gt;FFFFFF&lt;/BackColor&gt;
    ///          &lt;FontNames&gt;Verdana&lt;/FontNames&gt;
    ///          &lt;/fieldConfig&gt;
    ///        &lt;/configInfo&gt;
    ///        &lt;displayInfo&gt;
    ///          &lt;height&gt;15px&lt;/height&gt;
    ///          &lt;width&gt;212px&lt;/width&gt;
    ///          &lt;top&gt;50px&lt;/top&gt;
    ///          &lt;left&gt;184px&lt;/left&gt;
    ///          &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker&lt;/type&gt;
    ///        &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Value property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDatePicker runat=server></{0}:COEDatePicker>")]
    public class COEDatePicker : Infragistics.WebUI.WebSchedule.WebDateChooser, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOECultureable, ICOEDesignable, ICOEReadOnly
    {
        #region Variables & COnstants
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private const string DEFAULT_CSSCLASS = "FETextBox";
        private const string DEFAULT_CSSCLASS_VIEWONLY = "FETextBoxViewMode";
        private const string DEFAULT_LABEL_CSSCLASS = "FELabel";
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="COEDatePicker"/> class
        /// </summary>
        public COEDatePicker()
        {
            //this.ForeColor = System.Drawing.Color.Empty;
            //this.BackColor = System.Drawing.Color.Empty;
              this.CalendarLayout.HideOtherMonthDays = true;
        }
        #endregion

        #region COEGenerableControl Members
        public string DateFormat
        {
            get { return ViewState["DateFormat"] as string; }
            set { ViewState["DateFormat"] = value; }
        }

        /// <summary>
        /// Allows to set the default date.
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

        /// <summary>
        /// Gets the date as string, formatted as defined in <see cref="Constants.DatesFormat"/>.
        /// </summary>
        /// <returns>A string value with the date.</returns>
        public object GetData()
        {
            //coverity fix
            object dateValue = this.Value;
            if (dateValue != null)
            {
                DateTime date = DateTime.SpecifyKind(DateTime.Parse(dateValue.ToString()), DateTimeKind.Utc);
                return date.ToUniversalTime().ToString(Resources.COEDatePickerDateTimeFormat);
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Sets the date.
        /// </summary>
        /// <param name="data">A string with the desired date.</param>
        public void PutData(object data)
        {
            DateTime date;

            if (data.ToString() != string.Empty)
            {
                if (DateTime.TryParse(data.ToString(), out date))
                {
                    this.Value = DateTime.SpecifyKind(date, DateTimeKind.Utc).ToUniversalTime();
                }
                else
                    throw new Exception(string.Format("Unsupported Date Format {0}", data.ToString()));
            }
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COEDropDownList&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COETextBox&lt;/CSSLabelClass&gt;
        ///   &lt;ID&gt;DateEnteredTextBox&lt;/ID&gt;
        ///   &lt;NullDateLabel&gt;Pick a date&lt;/NullDateLabel&gt;
        ///   &lt;Width&gt;150&lt;/Width&gt;
        ///   &lt;Height&gt;15&lt;/Height&gt;
        ///   &lt;FontSize&gt;11px&lt;/FontSize&gt;
        ///   &lt;ForeColor&gt;484848&lt;/ForeColor&gt;
        ///   &lt;BackColor&gt;FFFFFF&lt;/BackColor&gt;
        ///   &lt;FontNames&gt;Verdana&lt;/FontNames&gt;
        ///   &lt;/fieldConfig&gt;
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
            if (style != null && !string.IsNullOrEmpty(style.InnerText))
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
            if (width != null && !string.IsNullOrEmpty(width.InnerText))
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && !string.IsNullOrEmpty(height.InnerText))
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode maxDate = xmlData.SelectSingleNode("//COE:MaxDate", manager);
            if (maxDate != null && !string.IsNullOrEmpty(maxDate.InnerText))
            {
                DateTime dtObj;
                try
                {
                    dtObj = DateTime.Parse(maxDate.InnerText);
                }
                catch (Exception e)
                {
                    throw e;
                }
                this.MaxDate = dtObj;
            }

            XmlNode dateFormat = xmlData.SelectSingleNode("//COE:DateFormat", manager);
            if (dateFormat != null && !string.IsNullOrEmpty(dateFormat.InnerText))
            {
                if (dateFormat.InnerText.ToLower() == "short")
                    this.Format = Infragistics.WebUI.WebSchedule.DateFormat.Short;
                else if (dateFormat.InnerText.ToLower() == "long")
                    this.Format = Infragistics.WebUI.WebSchedule.DateFormat.Long;
                else
                    this.DateFormat = dateFormat.InnerText;
                /* Possible values are: Long | Short | a date format
                 * a date format example would be: dd/MMM/yyyy
                 * 
                 * For more information on the custom format strings see: http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
                 */
            }
            else
            {
                // Grab the default date format from configurations.
                this.DateFormat = this.GetDateFormatOverride();
            }

            XmlNode editable = xmlData.SelectSingleNode("//COE:Editable", manager);
            if (editable != null && !string.IsNullOrEmpty(editable.InnerText))
            {
                this.Editable = (editable.InnerText).ToLower().Equals("true") ? true : false;
                //CSBR-132835
                this.Enabled = this.Editable;
            }
            XmlNode allowNull = xmlData.SelectSingleNode("//COE:AllowNull", manager);
            if (allowNull != null && !string.IsNullOrEmpty(allowNull.InnerText))
            {
                this.AllowNull = (allowNull.InnerText).ToLower().Equals("true") ? true : false;
            }

            XmlNode autoCloseUp = xmlData.SelectSingleNode("//COE:AutoCloseUp", manager);
            if (autoCloseUp != null && !string.IsNullOrEmpty(autoCloseUp.InnerText))
            {
                this.AutoCloseUp = (autoCloseUp.InnerText).ToLower().Equals("true") ? true : false;
            }
            XmlNode firstDayOfWeek = xmlData.SelectSingleNode("//COE:FirstDayOfWeek", manager);
            if (firstDayOfWeek != null && !string.IsNullOrEmpty(firstDayOfWeek.InnerText))
            {
                this.CalendarLayout.FirstDayOfWeek = (FirstDayOfWeek)Enum.Parse(typeof(FirstDayOfWeek), firstDayOfWeek.InnerText);
            }

            XmlNode gridLines = xmlData.SelectSingleNode("//COE:GridLines", manager);
            if (gridLines != null && !string.IsNullOrEmpty(gridLines.InnerText))
            {
                this.CalendarLayout.ShowGridLines = (Infragistics.WebUI.WebSchedule.GridLinesType)Enum.Parse(typeof(Infragistics.WebUI.WebSchedule.GridLinesType), gridLines.InnerText);
            }
            // Possible values are: Both | Horizontal | None | Vertical
            XmlNode imgURL = xmlData.SelectSingleNode("//COE:PressedButtonImageURL", manager);
            if (imgURL != null && !string.IsNullOrEmpty(imgURL.InnerText))
            {
                this.DropButton.ImageUrl2 = imgURL.InnerText;
            }
            imgURL = xmlData.SelectSingleNode("//COE:ButtonImageURL", manager);
            if (imgURL != null && !string.IsNullOrEmpty(imgURL.InnerText))
            {
                this.DropButton.ImageUrl1 = imgURL.InnerText;
            }
            XmlNode nullDateLabel = xmlData.SelectSingleNode("//COE:NullDateLabel", manager);
            if (nullDateLabel != null && !string.IsNullOrEmpty(nullDateLabel.InnerText))
            {
                this.NullDateLabel = nullDateLabel.InnerText;
            }
            else
                this.NullDateLabel = string.Empty;

            XmlNode nullValue = xmlData.SelectSingleNode("//COE:NullValueRepresentation", manager);
            if (nullValue != null && !string.IsNullOrEmpty(nullValue.InnerText))
            {
                this.NullValueRepresentation = (Infragistics.WebUI.WebSchedule.NullValueRepresentation)Enum.Parse(typeof(Infragistics.WebUI.WebSchedule.NullValueRepresentation), nullValue.InnerText);
            }
            // Possible values are: DateTime_MinValue | DBNull | NotSet | Null
            XmlNode Compliant508 = xmlData.SelectSingleNode("//COE:Section508Compliant", manager);
            if (Compliant508 != null && !string.IsNullOrEmpty(Compliant508.InnerText))
            {
                this.Section508Compliant = (Compliant508.InnerText).ToLower().Equals("true") ? true : false;
            }

            XmlNode ShowDropDownXml = xmlData.SelectSingleNode("//COE:ShowDropDown", manager);
            if (ShowDropDownXml != null && !string.IsNullOrEmpty(ShowDropDownXml.InnerText))
            {
                this.ShowDropDown = ShowDropDownXml.InnerText.ToLower().Equals("true") ? true : false;
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && !string.IsNullOrEmpty(cssClass.InnerText))
                this.CssClass = cssClass.InnerText;

            XmlNode cssClass2 = xmlData.SelectSingleNode("//COE:CSSCalenderClass", manager);
            if (cssClass2 != null && !string.IsNullOrEmpty(cssClass2.InnerText))
                this.CalendarLayout.Calendar.CssClass = cssClass2.InnerText;
            
            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && !string.IsNullOrEmpty(cssLabelClass.InnerText))
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if (labelStyle != null && !string.IsNullOrEmpty(labelStyle.InnerText))
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

            XmlNode fontSize = xmlData.SelectSingleNode("//COE:FontSize", manager);
            if (fontSize != null && !string.IsNullOrEmpty(fontSize.InnerText))
                this.Font.Size = FontUnit.Parse(fontSize.InnerText);

            XmlNode foreColor = xmlData.SelectSingleNode("//COE:ForeColor", manager);
            if (foreColor != null && !string.IsNullOrEmpty(foreColor.InnerText) && foreColor.InnerText.Length > 1)
            {
                int red = int.Parse(foreColor.InnerText.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int gree = int.Parse(foreColor.InnerText.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int blue = int.Parse(foreColor.InnerText.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                this.ForeColor = System.Drawing.Color.FromArgb(red, gree, blue);
            }

            XmlNode backColor = xmlData.SelectSingleNode("//COE:BackColor", manager);
            if (backColor != null && !string.IsNullOrEmpty(backColor.InnerText) && backColor.InnerText.Length > 1)
            {
                int red = int.Parse(backColor.InnerText.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int gree = int.Parse(backColor.InnerText.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int blue = int.Parse(backColor.InnerText.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                this.BackColor = System.Drawing.Color.FromArgb(red, gree, blue);
            }

            XmlNode fontNames = xmlData.SelectSingleNode("//COE:FontNames", manager);
            if (fontNames != null && !string.IsNullOrEmpty(fontNames.InnerText))
                this.Font.Name = fontNames.InnerText;


            XmlNode defaultValue = xmlData.SelectSingleNode("//COE:DefaultDate", manager);
            if (defaultValue != null && !string.IsNullOrEmpty(defaultValue.InnerText))
            {
                if (defaultValue.InnerText.ToUpper() == "TODAY")
                    this.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }

            string prevMonthImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/ig_cal_grayP0.gif";
            this.CalendarLayout.PrevMonthImageUrl = prevMonthImageUrl;
            
            string nextMonthImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/ig_cal_grayN0.gif";
            this.CalendarLayout.NextMonthImageUrl = nextMonthImageUrl;
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

            if (this.MaxDate != null)
            {
                XmlNode maxDate = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "MaxDate", xmlns);
                maxDate.InnerText = this.MaxDate.ToString();
                xmlData.FirstChild.AppendChild(maxDate);
            }

            if (!string.IsNullOrEmpty(this.DateFormat) || this.Format != Infragistics.WebUI.WebSchedule.DateFormat.Short) // Short is the default value and it is preferred not to add the node in such case.
            {
                // Possible values are:  Long | Short | A date format
                XmlNode dateFormat = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "DateFormat", xmlns);
                dateFormat.InnerText = string.IsNullOrEmpty(this.DateFormat) ? this.Format.ToString() : this.DateFormat;
                xmlData.FirstChild.AppendChild(dateFormat);
            }

            XmlNode editable = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Editable", xmlns);
            editable.InnerText = this.Editable.ToString();
            xmlData.FirstChild.AppendChild(editable);

            XmlNode allowNull = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AllowNull", xmlns);
            allowNull.InnerText = this.AllowNull.ToString();
            xmlData.FirstChild.AppendChild(allowNull);

            XmlNode autoCloseUp = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AutoCloseUp", xmlns);
            autoCloseUp.InnerText = this.AutoCloseUp.ToString();
            xmlData.FirstChild.AppendChild(autoCloseUp);

            XmlNode firstDayOfWeek = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "FirstDayOfWeek", xmlns);
            firstDayOfWeek.InnerText = this.CalendarLayout.FirstDayOfWeek.ToString();
            xmlData.FirstChild.AppendChild(firstDayOfWeek);

            // Possible values are: Both | Horizontal | None | Vertical
            XmlNode gridLines = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "GridLines", xmlns);
            gridLines.InnerText = this.CalendarLayout.ShowGridLines.ToString();
            xmlData.FirstChild.AppendChild(gridLines);

            if (!string.IsNullOrEmpty(this.DropButton.ImageUrl2))
            {
                XmlNode imgURL2 = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "PressedButtonImageURL", xmlns);
                imgURL2.InnerText = this.DropButton.ImageUrl2;
                xmlData.FirstChild.AppendChild(imgURL2);
            }

            if (!string.IsNullOrEmpty(this.DropButton.ImageUrl1))
            {
                XmlNode imgURL = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ButtonImageURL", xmlns);
                imgURL.InnerText = this.DropButton.ImageUrl1;
                xmlData.FirstChild.AppendChild(imgURL);
            }

            if (!string.IsNullOrEmpty(this.NullDateLabel))
            {
                XmlNode nullDateLabel = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "NullDateLabel", xmlns);
                nullDateLabel.InnerText = this.NullDateLabel;
                xmlData.FirstChild.AppendChild(nullDateLabel);
            }

            // Possible values are: DateTime_MinValue | DBNull | NotSet | Null
            XmlNode nullValue = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "NullValueRepresentation", xmlns);
            nullValue.InnerText = this.NullValueRepresentation.ToString();
            xmlData.FirstChild.AppendChild(nullValue);

            XmlNode compliant508 = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Section508Compliant", xmlns);
            compliant508.InnerText = this.Section508Compliant.ToString();
            xmlData.FirstChild.AppendChild(compliant508);

            //Ensure there is a default CSSClass value for this form-field (be it read-only or editable) and its label
            if (string.IsNullOrEmpty(this.CssClass) && this.ReadOnly != true)
                this.CssClass = DEFAULT_CSSCLASS;
            else if (string.IsNullOrEmpty(this.CssClass) && this.ReadOnly == true)
                this.CssClass = DEFAULT_CSSCLASS_VIEWONLY;

            XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
            cssClass.InnerText = this.CssClass;
            xmlData.FirstChild.AppendChild(cssClass);

            if (string.IsNullOrEmpty(_lit.CssClass))
                _lit.CssClass = DEFAULT_LABEL_CSSCLASS;
            XmlNode cssLabelClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns);
            cssLabelClass.InnerText = _lit.CssClass;
            xmlData.FirstChild.AppendChild(cssLabelClass);

            if (!string.IsNullOrEmpty(_lit.Style.Value))
            {
                XmlNode labelStyle = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "LabelStyle", xmlns);
                labelStyle.InnerText = _lit.Style.Value;
                xmlData.FirstChild.AppendChild(labelStyle);
            }

            if (this.Font != null && this.Font.Size != null)
            {
                XmlNode fontSize = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "FontSize", xmlns);
                fontSize.InnerText = this.Font.Size.ToString();
                xmlData.FirstChild.AppendChild(fontSize);
            }

            if (this.ForeColor != System.Drawing.Color.Empty)
            {
                XmlNode foreColor = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ForeColor", xmlns);
                foreColor.InnerText = this.ForeColor.ToArgb().ToString();
                xmlData.FirstChild.AppendChild(foreColor);
            }

            if (this.BackColor != System.Drawing.Color.Empty)
            {
                XmlNode backColor = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "BackColor", xmlns);
                backColor.InnerText = this.BackColor.ToArgb().ToString();
                xmlData.FirstChild.AppendChild(backColor);
            }

            if (this.Font != null && !string.IsNullOrEmpty(this.Font.Name))
            {
                XmlNode fontNames = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "FontNames", xmlns);
                fontNames.InnerText = this.Font.Name;
                xmlData.FirstChild.AppendChild(fontNames);
            }

            return xmlData.FirstChild;
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

        #region ICOECultureable Members
        /// <summary>
        /// Sets the culture info for the control.
        /// </summary>
        public System.Globalization.CultureInfo DisplayCulture
        {
            set
            {
                try { base.CalendarLayout.Culture = value; }
                catch { base.CalendarLayout.Culture = CultureInfo.CurrentUICulture; }
                if (!string.IsNullOrEmpty(this.DateFormat))
                {
                    System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture(base.CalendarLayout.Culture.IetfLanguageTag);
                    ci.DateTimeFormat.ShortDatePattern = this.DateFormat;
                    base.CalendarLayout.Culture = ci;
                }
            }
        }

        #endregion

        #region ICOEReadOnly Members
        /// <summary>
        /// EditControl Property implementation.
        /// </summary>
        public COEEditControl COEReadOnly
        {
            get
            {
                return _editControl;
            }
            set
            {
                _editControl = value;
            }
        }

        #endregion

        #region Life Cycle Events

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.Enabled = false;
            }
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
        }

        #endregion

        #region Private Methods
        private string GetDateFormatOverride()
        {
            return ConfigurationUtilities.GetApplicationData(GUIShell.GUIShellUtilities.GetApplicationName()).DateFormat;
        }
        #endregion
    }
}

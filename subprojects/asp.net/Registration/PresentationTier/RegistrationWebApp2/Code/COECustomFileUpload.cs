using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Xml;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE FileUpload web control.
    /// </summary>
    /// <summary>
    /// <para>
    /// This class implements a FileUpload control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEFileUpload class accepts every FileUpload property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: Value to be displayed by default</item>
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
    ///        &lt;formElement name="AdvSearchLink"&gt;
    ///          &lt;showHelp>false&lt;/showHelp&gt;
    ///          &lt;isFileUpload>false&lt;/isFileUpload&gt;
    ///          &lt;helpText /&gt;
    ///          &lt;bindingExpression /&gt;
    ///          &lt;Id>VM_AdvSearchButton&lt;/Id&gt;
    ///          &lt;displayInfo&gt;
    ///            &lt;type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECustomFileUpload&lt;/type&gt;
    ///            &lt;assembly>CambridgeSoft.COE.RegistrationWebApp&lt;/assembly&gt;
    ///            &lt;visible>true&lt;/visible&gt;
    ///          &lt;/displayInfo&gt;
    ///          &lt;validationRuleList /&gt;
    ///          &lt;serverEvents&gt;
    ///            &lt;event handlerName="AdvSearchMaterial_Click" eventName="Click" /&gt;
    ///          &lt;/serverEvents&gt;
    ///          &lt;clientEvents /&gt;
    ///          &lt;configInfo&gt;
    ///            &lt;fieldConfig&gt;
    ///                  &lt;FileUploadLocation>C:\Reg\&lt;/FileUploadLocation&gt;
    ///                  &lt;VirtualPath>http://localhost/COERegisration/UploadFiles/&lt;/VirtualPath&gt;
    ///                  &lt;DisplayMode>Add&lt;/DisplayMode&gt; --Add/Edit/View
    ///                  &lt;Target>_blank&lt;/Target&gt;
    ///                  &lt;LinkText>Click to see the Link &lt;/LinkText&gt;
    ///            &lt;/fieldConfig&gt;
    ///          &lt;/configInfo&gt;
    ///          &lt;dataSource /&gt;
    ///          &lt;dataSourceId /&gt;
    ///          &lt;displayData /&gt;
    ///        &lt;/formElement&gt;
    /// </code>
    /// </summary>

    [ToolboxData("<{0}:COEFileUpload runat=server></{0}:COEFileUpload>")]
    class COECustomFileUpload : FileUpload, ICOEGenerableControl, ICOELabelable, ICOEReadOnly
    {

        #region Variables

        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private TextBox _lblFile = new TextBox();
        private HyperLink _hlFile = new HyperLink();
        private Panel _iFileIcon = new Panel();
        private string _fileuploadLocation = string.Empty;
        private string _virtualpath = string.Empty;
        private string _savedfilename = string.Empty;
        private string _mode = string.Empty; // Values should be Add/Edit/View
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region Properties

        /// <summary>
        /// Sets the CSS label class.
        /// </summary>
        /// <value>The CSS label class.</value>
        public string CSSLabelClass
        {
            set { _lit.CssClass = value; }
        }

        public string SavedFileName
        {
            get
            {
                if (ViewState["SavedFile"] != null)
                    return (string)ViewState["SavedFile"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["SavedFile"] = value;
            }
            
            //get { return _savedfilename; }
            //set { _savedfilename = value; }
        }

        public string FileUploadLocation
        {
            get
            {
                if (!string.IsNullOrEmpty(this._fileuploadLocation))
                {
                    _fileuploadLocation = (_fileuploadLocation.EndsWith("\\") ? _fileuploadLocation : _fileuploadLocation + "\\");
                }
                else
                {
                    _fileuploadLocation = Page.Server.MapPath("~/");
                }
                return _fileuploadLocation;
            }
            set
            {
                _fileuploadLocation = value;  
            }

        }

        public string Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            //return this.PostedFile.FileName;
            //
            HttpPostedFile thePostedFile = this.PostedFile;
            if (thePostedFile != null && thePostedFile.InputStream.Length > 0)
            {
                string filename = Path.GetFileName(thePostedFile.FileName);
                thePostedFile.SaveAs(FileUploadLocation + filename);
                //SavedFileName = string.IsNullOrEmpty(_virtualpath) ? (FileUploadLocation + filename) : (_virtualpath + "/" + filename);
                SavedFileName = filename;
            }
            else if (Mode.ToUpper() == "EDIT" || Mode.ToUpper() == "ADD")
            {
                if (Page.Request[this.ID + "FileNameLabel"] != null)
                    SavedFileName = Page.Request[this.ID + "FileNameLabel"].ToString();
            }

            return SavedFileName;

        }

        public void PutData(object data)
        {
            SavedFileName=(string)data;
            _lblFile.Text = SavedFileName;
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;configSetting bindingExpression="this.CSSLabelClass"&gt;COELabel&lt;/configSetting&gt;
        ///   &lt;configSetting bindingExpression="this.SpinButtons.Display"&gt;OnRight&lt;/configSetting&gt;
        /// &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);
            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:configSetting", manager))
            {
                if (configSetting != null && configSetting.InnerText.Length > 0)
                {
                    string bindingExpression = configSetting.Attributes["bindingExpression"].Value;
                    if (!string.IsNullOrEmpty(bindingExpression))
                    {
                        try
                        {   //Using COEDatabinder to find the object property given a bindingExpression in the current object
                            COEDataBinder dataBinder = new COEDataBinder(this);
                            dataBinder.SetProperty(bindingExpression, configSetting.InnerText);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
            }

            XmlNode UploadLocation = xmlData.SelectSingleNode("//COE:FileUploadLocation", manager);
            if (UploadLocation != null && UploadLocation.InnerText.Length > 0)
            {
                this._fileuploadLocation = UploadLocation.InnerText;
            }
            XmlNode VirtualPath = xmlData.SelectSingleNode("//COE:VirtualPath", manager);
            if (VirtualPath != null && VirtualPath.InnerText.Length > 0)
            {
                this._virtualpath = VirtualPath.InnerText;
            }
            
            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
            {
                _lit.CssClass = cssLabelClass.InnerText;
            }
            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
            {
                _lblFile.CssClass = cssClass.InnerText;
                this.CssClass = cssClass.InnerText;
            }
            XmlNode DisplayMode = xmlData.SelectSingleNode("//COE:DisplayMode", manager);
            if (DisplayMode != null && DisplayMode.InnerText.Length > 0)
                Mode = DisplayMode.InnerText;

            XmlNode Target = xmlData.SelectSingleNode("//COE:Target", manager);
            if (Target != null && Target.InnerText.Length > 0)
                _hlFile.Target = Target.InnerText;

            XmlNode LinkText = xmlData.SelectSingleNode("//COE:LinkText", manager);
            if (LinkText != null && LinkText.InnerText.Length > 0)
                _hlFile.Text = LinkText.InnerText;
        }

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

        #endregion

        #region ICOELabelable Members

        public string Label
        {
            get
            {
                if (ViewState[Label_VS] != null)
                    return (string)ViewState[Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[LabelStyles_VS] = value;
            }
        }


        #endregion

        #region Overriden Life cicle events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.Enabled = false;
                _lblFile.ReadOnly = true;
            }
            _lit.Text = this.Label;
            _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
            _lit.RenderControl(writer);
            _lblFile.ID = this.ID + "FileNameLabel";
            Table pnlViewMode = null;
            try
            {
                pnlViewMode = new Table();
                pnlViewMode.Rows.Add(new TableRow());
                pnlViewMode.Rows[0].Cells.Add(new TableCell());
                pnlViewMode.Rows[0].Cells.Add(new TableCell());
                switch (Mode.ToUpper())
                {
                    case "ADD":
                        _hlFile.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                        _lblFile.Text = SavedFileName;
                        _lblFile.Style.Add(HtmlTextWriterStyle.Display, "block");
                        _lblFile.RenderControl(writer);
                        //define lineBreak label locally and release it immediately
                        using (Label lineBreak = new Label()) // Coverity fix - CID 11843 
                        {
                            lineBreak.RenderControl(writer);
                        }
                        break;
                    case "EDIT":
                        _lblFile.Text = SavedFileName;
                        _lblFile.Style.Add(HtmlTextWriterStyle.Display, "block");
                        _lblFile.RenderControl(writer);
                        break;
                    case "VIEW":
                        if (string.IsNullOrEmpty(SavedFileName))
                            _hlFile.Text = string.IsNullOrEmpty(_hlFile.Text) ? "Click here" : _hlFile.Text;
                        else
                            _hlFile.Text = SavedFileName;
                        _hlFile.NavigateUrl = FormatUrl();
                        base.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                        if (_iFileIcon.Visible)
                        {
                            _iFileIcon.CssClass = "FileAttachClass";

                            _hlFile.Style.Add(HtmlTextWriterStyle.Display, "inline-block");
                        }
                        pnlViewMode.Rows[0].Cells[0].Controls.Add(_iFileIcon);
                        pnlViewMode.Rows[0].Cells[1].Controls.Add(_hlFile);
                        //pnlViewMode.Controls.Add(_hlFile);
                        //pnlViewMode.Controls.Add(_iFileIcon);
                        //pnlViewMode.Style.Add(HtmlTextWriterStyle.Display, "inline-block");
                        pnlViewMode.RenderControl(writer);
                        break;
                    default:
                        _lblFile.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                        _hlFile.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                        break;
                }
                base.Style.Add(HtmlTextWriterStyle.Display, "block");
                //base.Style.Add(HtmlTextWriterStyle.Width, "50%");
                base.Render(writer);
            }
            catch
            {
            }
            finally
            {
                if (pnlViewMode != null)
                    pnlViewMode.Dispose();
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
                if (ViewState[RequiredStyle_VS] != null)
                    return (string)ViewState[RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[RequiredLabelStyle_VS] != null)
                    return (string)ViewState[RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[RequiredLabelStyle_VS] = value;
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

        #region public methods

        public string FormatUrl()
        {
            string LinkUrl = string.Empty;
            _iFileIcon.Visible = false;
            if (SavedFileName.ToLower().StartsWith("http://") || SavedFileName.ToLower().StartsWith("https://") || SavedFileName.ToLower().StartsWith("\\\\") || SavedFileName.ToLower().StartsWith("www"))
            {
                LinkUrl = SavedFileName.ToLower().StartsWith("www") ? "http://" + SavedFileName : SavedFileName;
            }
            else if (!string.IsNullOrEmpty(this._virtualpath) && !string.IsNullOrEmpty(SavedFileName))
            {
                LinkUrl = this._virtualpath.EndsWith("/") ? this._virtualpath + SavedFileName : this._virtualpath + "/" + SavedFileName;
               _iFileIcon.Visible = true;
            }

            else
                LinkUrl = SavedFileName;
            return LinkUrl;

        }

       #endregion

        #region ViewState Constants
        public const string Label_VS = "Label";
        public const string CSSClass_VS = "CSSClass";
        public const string LabelStyles_VS = "LabelStyles";
        public const string RequiredStyle_VS = "RequiredStyle";
        public const string RequiredLabelStyle_VS = "RequiredLabelStyle";
        #endregion
    }
}

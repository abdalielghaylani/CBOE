using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ValidationPropertyAttribute("Value")]
    public class COEResourceLink : CompositeControl, ICOEGenerableControl, ICOEDesignable
    {
        #region Variables
        //private HtmlInputHidden _uHidden = new HtmlInputHidden();
        private HtmlInputText _uHidden = new HtmlInputText();
        private HtmlSelect _modeSelectDropDown = new HtmlSelect();
        string _height = string.Empty;
        string _width = "100%";
        string _modeSelectCSS = string.Empty;
        string _uncDivCSS = string.Empty;
        string _urlDivCSS = string.Empty;
        string _fileUploadCSS = string.Empty;
        string _urlTextBoxCSS = string.Empty;
        string _urlGoButtonCSS = string.Empty;
        string _urlFrameCSS = string.Empty;
        string _validatorCSS = string.Empty;
        //HtmlSelect _modeSelectDropDown = new HtmlSelect();
        #endregion

        #region Properties

        public string Value
        {
            get
            {
                return this.GetData().ToString();
            }
            set {
                this.PutData(value);
            }
        }

        public string UNCRegularExpression
        {
            get {
                return Resources.UNCRegularExpression;
            }
        }
        private Mode CurrentMode
        {
            get
            {
                return (Mode)Enum.Parse(typeof(Mode), _modeSelectDropDown.Items[_modeSelectDropDown.SelectedIndex].Value);
            }
            set { 

            }
        }

        private enum Mode { UNC, URL };

        #endregion

        #region Constructors
        public COEResourceLink()
        {
            _uHidden.ID = "UncHidden";
            _uHidden.Style.Value = "visibility:hidden;";
            
            _modeSelectDropDown.ID = "ModeSelectDropDown";
            _modeSelectDropDown.Items.Add(new ListItem(Mode.UNC.ToString()));
            _modeSelectDropDown.Items.Add(new ListItem(Mode.URL.ToString()));
            _modeSelectDropDown.Style.Value = "display:inline; float:left";
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {

            //return this._uHidden.Value;
            return _uHidden.Value;
        }

        public void PutData(object data)
        {
            this._uHidden.Value = data.ToString();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode xmlNode = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                this.CssClass = xmlNode.InnerText;
            }

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            xmlNode = xmlData.SelectSingleNode("//COE:Style", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                string[] styles = xmlNode.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++)
                {
                    if(styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();

                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            xmlNode = xmlData.SelectSingleNode("//COE:ModeSelectCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _modeSelectCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:UncCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _uncDivCSS = xmlNode.InnerText;
            }
            
            xmlNode = xmlData.SelectSingleNode("//COE:UrlCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _urlDivCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:FileUploadCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _fileUploadCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:UrlAddressTextBoxCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _urlTextBoxCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:UrlGoButtonCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _urlGoButtonCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:UrlFrameCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _urlFrameCSS = xmlNode.InnerText;
            }
            
            xmlNode = xmlData.SelectSingleNode("//COE:ValidationCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _validatorCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:ValidationCSS", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _validatorCSS = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:Width", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _width = xmlNode.InnerText;
            }

            xmlNode = xmlData.SelectSingleNode("//COE:Height", manager);
            if(xmlNode != null && xmlNode.InnerText.Length > 0)
            {
                _height = xmlNode.InnerText;
            }
        }

        public string DefaultValue
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            //this.Page.ClientScript.RegisterForEventValidation(this._uHidden.UniqueID);
            base.Render(writer);
        }
        
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            _modeSelectDropDown.Attributes["class"] = _modeSelectCSS;
            this.Controls.Add(_modeSelectDropDown);

            Panel UNCdiv = new Panel();
            UNCdiv.ID = "UNCDiv";

            Panel URLdiv = new Panel();
            URLdiv.ID = "URLDiv";

            this.Controls.Add(UNCdiv);
            this.Controls.Add(URLdiv);

            UNCdiv.CssClass = _uncDivCSS;
            UNCdiv.Style.Value = string.Format("overflow: hidden; visibility:{0};", CurrentMode == Mode.UNC ? "visible" : "hidden");
            UNCdiv.Width = new Unit(_width);
            UNCdiv.Height = new Unit(_height);

            URLdiv.CssClass = _urlDivCSS;
            URLdiv.Style.Value = string.Format("overflow: hidden; visibility:{0};", CurrentMode == Mode.URL ? "visible" : "hidden");
            URLdiv.Width = new Unit(_width);
            URLdiv.Height = new Unit(_height);

            HtmlInputFile UNCHtmlFileUpload = new HtmlInputFile();
            UNCHtmlFileUpload.ID = "UNCHtmlFileUpload";
            UNCdiv.Controls.Add(UNCHtmlFileUpload);
            UNCHtmlFileUpload.Attributes["class"] = _fileUploadCSS;
            UNCHtmlFileUpload.Style.Value = "display: inline; float: left; width: 100%";

            CustomValidator customValidator = new CustomValidator();
            customValidator.ID = "UNCCustomValidator";
            UNCdiv.Controls.Add(customValidator);
            customValidator.ControlToValidate = UNCHtmlFileUpload.ID;
            customValidator.ClientValidationFunction = "ClientValidate";
            //customValidator.ErrorMessage = "Invalid UNC Share";
            customValidator.Text = Resources.InvalidUNC;
            customValidator.ValidateEmptyText = true;
            customValidator.ServerValidate += new ServerValidateEventHandler(customValidator_ServerValidate);
            customValidator.CssClass = _validatorCSS;
            customValidator.Style.Value = "display:block; float:left;";
            customValidator.ToolTip = @"Format: \\server\share";

            UNCdiv.Controls.Add(_uHidden);

            TextBox URLTextBox = new TextBox();
            URLTextBox.ID = "URLTextBox";
            URLdiv.Controls.Add(URLTextBox);
            URLTextBox.CssClass = _urlTextBoxCSS;
            URLTextBox.Text = this._uHidden.Value;
            URLTextBox.Style.Value = "display: inline; float: left; width : 90%;";

            HtmlButton GoToButton = new HtmlButton();
            GoToButton.InnerText = "Go";
            GoToButton.ID = "GoToButton";
            URLdiv.Controls.Add(GoToButton);
            GoToButton.Attributes["class"] = _urlGoButtonCSS;
            GoToButton.Style.Value = "display: inline; width: 7%; float: left;";
            
            /*var urlFrame = getElementById('" + iframeBrowser.ClientID + @"');
    var urlTextBox = getElementById('" + URLTextBox.ClientID + @"');
    var uncHidden = getElementById('" + _uHidden.ClientID + @"');*/

            HtmlGenericControl iframeBrowser = new HtmlGenericControl("iframe");
            iframeBrowser.ID = "URIFrame";
            URLdiv.Controls.Add(iframeBrowser);
            iframeBrowser.Attributes["class"] = _urlFrameCSS;
            iframeBrowser.Style.Value = "width:100%;";

            _modeSelectDropDown.Attributes["onchange"] = "ModeChanged(this, getElementById('" + UNCdiv.ClientID + @"'), getElementById('" + URLdiv.ClientID + @"'));";
            GoToButton.Attributes["onclick"] = "GoToURL(getElementById('" + iframeBrowser.ClientID + @"'), getElementById('" + URLTextBox.ClientID + @"'), getElementById('" + _uHidden.ClientID + @"'));";

            if(!this.Page.ClientScript.IsStartupScriptRegistered("lololo" + this.ClientID))
                this.Page.ClientScript.RegisterStartupScript(typeof(string), "lololo" + this.ClientID,
                    "<script type=\"text/javascript\">" + 
                    @"ModeChanged(getElementById('" + _modeSelectDropDown.ClientID + @"'), getElementById('" + UNCdiv.ClientID + @"'), getElementById('" + URLdiv.ClientID + @"'));
                    </script>");

            if(!this.Page.ClientScript.IsClientScriptBlockRegistered("lalala"))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "lalala",
"<script type=\"text/javascript\"> " +
@"
function getElementById(controlID, oDoc) {
    if( document.getElementById ) {
	    return document.getElementById(controlID); 
	}
	
	if( document.all ) {
	    return document.all[controlID]; 
	}

    if( !oDoc ) { oDoc = document; }

    if( document.layers ) {
        if( oDoc.layers[controlID] ) { 
            return oDoc.layers[controlID]; 
        } 
        else {
            //repeatedly run through all child layers
                for( var x = 0, y; !y && x < oDoc.layers.length; x++ ) {
				    //on success, return that layer, else return nothing
                    y = getRefToDiv(controlID,oDoc.layers[x].document); 
                }
                return y; 
        } 
    }
	return false;
} 

function ClientValidate(source, clientside_arguments)
   {         
      /*var regularExpression = /" + this.UNCRegularExpression + @"/;
      if (clientside_arguments.Value == 'lalala' )
      {
         clientside_arguments.IsValid=true;
      }
      else {clientside_arguments.IsValid=false};*/
   }
function ModeChanged(modeDropDown, UNCDiv, URLDiv)
{
    /*var modeDropDown = document.getElementById('" + _modeSelectDropDown.ClientID + @"');
    var UNCDiv = getElementById('" + UNCdiv.ClientID + @"');
    var URLDiv = getElementById('" + URLdiv.ClientID + @"');*/

    if(modeDropDown && modeDropDown.options[modeDropDown.selectedIndex].text == '" + Mode.URL.ToString() + @"')
    {
        
        UNCDiv.style.visibility = 'hidden';
        UNCDiv.style.height = '0px';
        URLDiv.style.visibility = 'visible';
        URLDiv.style.height = '';
    }
    else if (modeDropDown)    
    {
        URLDiv.style.visibility = 'hidden';
        URLDiv.style.height = '0px';
        UNCDiv.style.visibility = 'visible';
        UNCDiv.style.height = '';
    }
    return true;
}
function GoToURL(urlFrame, urlTextBox, uncHidden)
{
    /*var urlFrame = getElementById('" + iframeBrowser.ClientID + @"');
    var urlTextBox = getElementById('" + URLTextBox.ClientID + @"');
    var uncHidden = getElementById('" + _uHidden.ClientID + @"');*/
    
    if(urlFrame && urlTextBox && uncHidden)
    {
        var source = 'http://' + urlTextBox.value;
        if((urlTextBox.value.toLowerCase().indexOf('http://') == 0 || urlTextBox.value.toLowerCase().indexOf('https://') == 0)
            source = urlTextBox.value

        if(uncHidden)
            uncHidden.value = source;

        urlFrame.src = source;
    }
    return true;
}
</script>            
");

            if(!Page.ClientScript.IsOnSubmitStatementRegistered("lululu" + this.ClientID))
                Page.ClientScript.RegisterOnSubmitStatement(typeof(string), "lululu" + this.ClientID,
@"
    var modeDropDown = getElementById('" + _modeSelectDropDown.ClientID + @"');
    var uncFileUpload = getElementById('" + UNCHtmlFileUpload.ClientID + @"');
    var uncHidden = getElementById('" + _uHidden.ClientID + @"');

    if(modeDropDown && modeDropDown.options[modeDropDown.selectedIndex].text == '" + Mode.UNC.ToString() + @"'&& uncFileUpload && uncHidden)
    {
        var selectedUNCString = uncFileUpload.value;

        uncHidden.value = selectedUNCString;

        uncFileUpload.value = '';
        uncFileUpload.disabled = true;
    }
");

            base.CreateChildControls();
        }

        void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if(CurrentMode == Mode.UNC)
                {
                    string data = this.GetData().ToString();
                    if(!string.IsNullOrEmpty(data))
                    {
                        Regex regExp = new Regex(this.UNCRegularExpression);
                        args.IsValid = regExp.IsMatch(this.GetData().ToString());
                    }
                }
                else
                {
                    args.IsValid = true;
                }
            }
            catch(Exception ex)
            {
                args.IsValid = false;
            }
        }

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

            if (string.IsNullOrEmpty(this.CssClass))
            {
                XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
                cssClass.InnerText = this.CssClass;
                xmlData.FirstChild.AppendChild(cssClass);
            }

            return xmlData.FirstChild;
        }

        #endregion
    }
}

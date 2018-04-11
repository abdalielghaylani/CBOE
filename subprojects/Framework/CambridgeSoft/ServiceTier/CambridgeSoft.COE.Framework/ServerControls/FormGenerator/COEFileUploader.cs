using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls
{
    [ToolboxData("<{0}:COEFileUploader runat=server></{0}:COEFileUploader>")]
    [Description("Control to upload files to the server")]
    public class COEFileUploader : CompositeControl, ICOEGenerableControl, ICOELabelable
    {
        #region Constants

        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string ELEMENTMIN     = "element-min.js";
        private const string UPLOADERMIN    = "uploader-min.js";

        #endregion

        #region Variables

        Panel UIElements;
        Panel UploaderContainer;
        Panel UploadFilesLink;
        Panel SelectedFileDisplay;
        Panel UploaderOverlay;
        Panel SelectFilesLink;
        Label SelectLinkLabel;
        Label UploadLinkLabel;
        Label ProgressLabel;
        TextBox ProgressReportTextBox;
       
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return new object();
        }

        public void PutData(object data)
        {
            
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            
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

        #region ICOELabelable Members

        public string Label
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

        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.ELEMENTMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.UPLOADERMIN);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page, 
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);

            if (this.Page.Master.Master == null)
            {
                if (((HtmlGenericControl)this.Page.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes["class"] == null)
                    ((HtmlGenericControl)this.Page.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes.Add("class", "yui-skin-sam");
            }
            else
            {
                if (((HtmlGenericControl)this.Page.Master.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes["class"] == null)
                    ((HtmlGenericControl)this.Page.Master.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes.Add("class", "yui-skin-sam");
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            UIElements = new Panel();
            UIElements.ID = "uiElements";
            UIElements.Style.Add("display","inline");
            
            UploaderContainer = new Panel();
            UploaderContainer.ID = "uploaderContainer";

            UploaderOverlay = new Panel();
            UploaderOverlay.ID = "uploaderOverlay";
            UploaderOverlay.Style.Add("position", "absolute");
            UploaderOverlay.Style.Add("z-index", "2");

            SelectFilesLink = new Panel();
            SelectFilesLink.ID = "selectFilesLink";
            SelectFilesLink.Style.Add("z-index", "1");

            SelectLinkLabel = new Label();
            SelectLinkLabel.Text = "Select File";

            SelectFilesLink.Controls.Add(SelectLinkLabel);

            UploaderContainer.Controls.Add(UploaderOverlay);
            UploaderContainer.Controls.Add(SelectFilesLink);

            UploadFilesLink = new Panel();
            UploadFilesLink.ID = "uploadFilesLink";
            UploadFilesLink.Style.Add("display", "inline");

            UploadLinkLabel = new Label();
            UploadLinkLabel.ID = "uploadLink";
            UploadLinkLabel.Text = "Upload File";
            UploadLinkLabel.Attributes.Add("OnClick", "upload(); return false;");

            UploadFilesLink.Controls.Add(UploadLinkLabel);
            
            SelectedFileDisplay = new Panel();
            SelectedFileDisplay.ID = "selectedFileDisplay";
            UploaderContainer.Style.Add("display", "inline");

            ProgressLabel = new Label();
            ProgressLabel.Text = "Progress:";

            ProgressReportTextBox = new TextBox();
            ProgressReportTextBox.ID = "progressReport";
            ProgressReportTextBox.ReadOnly = true;
            ProgressReportTextBox.Columns = 50;

            SelectedFileDisplay.Controls.Add(ProgressReportTextBox);
            SelectedFileDisplay.Controls.Add(ProgressLabel);

            UIElements.Controls.Add(UploaderContainer);
            UIElements.Controls.Add(UploadFilesLink);
            UIElements.Controls.Add(SelectedFileDisplay);

        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string script = @"
                <script type='text/javascript'>
                YAHOO.util.Event.onDOMReady(function () { 
	            var uiLayer = YAHOO.util.Dom.getRegion('" + this.SelectLinkLabel.ClientID + @"');
	            var overlay = YAHOO.util.Dom.get('" + this.UploaderOverlay.ClientID + @"');
	            YAHOO.util.Dom.setStyle(overlay, 'width', uiLayer.right-uiLayer.left + 'px');
	            YAHOO.util.Dom.setStyle(overlay, 'height', uiLayer.bottom-uiLayer.top + 'px');
	            });
                YAHOO.widget.Uploader.SWFURL = '" + Page.ClientScript.GetWebResourceUrl(typeof(COEFileUploader),"CambridgeSoft.COE.Framework.ServerControls.FormGenerator.JSAndCSS.uploader.swf")  + @"';
		        var uploader = new YAHOO.widget.Uploader('" +  this.UploaderOverlay.ClientID + @"');
                ";
            script += "</script>";
            writer.Write(script);
        }

        #endregion
    }
}

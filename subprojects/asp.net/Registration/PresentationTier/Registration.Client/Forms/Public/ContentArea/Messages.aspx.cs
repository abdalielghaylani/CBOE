using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Infragistics.WebUI.UltraWebListbar;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.GUIShell;

namespace RegistrationWebApp.Forms.Public.ContentArea
{
    public partial class Messages : GUIShellPage
    {
        #region Variables

        private string _messageCode = GUIShellTypes.MessagesCode.Error.ToString();
        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        static COELog _coeLog = COELog.GetSingleton("RegistartionWebApp");

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!Page.IsPostBack)
            {
                this.SetControlsAttributtes();
                this.ReadMessageToDisplay();
                this.CustomizeUserPreference();
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings
            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }
            #endregion
            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.PageTitleLabel.Text = Resource.Messages_Title_Label;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to display in the messages in the page. 
        /// </summary>
        private void ReadMessageToDisplay()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            bool displayBack = true;
            string goBackButtonText = String.Empty;
            string goBackButtonJScript = String.Empty;

            if (!string.IsNullOrEmpty(Request[GUIShellTypes.RequestVars.DisplayBack.ToString()]))
                bool.TryParse(Request[GUIShellTypes.RequestVars.DisplayBack.ToString()].ToString(), out displayBack);

            if (!displayBack)
            {
                goBackButtonText = Resource.Close_Button_Text;
                goBackButtonJScript = "window.close();";
            }
            else
            {
                goBackButtonText = Resource.Back_Button_Text;
                goBackButtonJScript = "history.back(-1);";
            }

            //Set GoBackButton Settings
            this.GoBackButton.Value = goBackButtonText;
            this.GoBackButton.Attributes.Add("onclick", goBackButtonJScript);

            //Read Message code from URL Request.
            _messageCode = Request[GUIShellTypes.RequestVars.MessageCode.ToString()].ToString();

            //According the type of Message, display an error message.
            if (_messageCode == GUIShellTypes.MessagesCode.Error.ToString())
                this.MessageLabel.Text = Resource.ErrorMessage_Label_Text;
            else if (_messageCode == GUIShellTypes.MessagesCode.PageNotFound.ToString())
                this.MessageLabel.Text = Resource.PageNotFound_Label_Text;
            else if (_messageCode == GUIShellTypes.MessagesCode.InvalidParameter.ToString())
                this.MessageLabel.Text = Resource.NoRecordsFound_Label_Text;
            else if (_messageCode == GUIShellTypes.MessagesCode.NoEnoughPrivileges.ToString())
            {
                this.MessageLabel.Text = Resource.NoEnoughPrivileges_Label_Text;
                this.MessageLabel.CssClass = "ErrorMessages";
            }
            else if (_messageCode == GUIShellTypes.MessagesCode.PageSettingsDisable.ToString())
            {
                this.MessageLabel.Text = Resource.PageSettingsDisable_Label_Text;
                this.MessageLabel.CssClass = "ErrorMessages";
            }
            else if (_messageCode == GUIShellTypes.MessagesCode.RecordNotFound.ToString())
            {
                this.MessageLabel.Text = Resource.RecordNotFound_Label_Text;
                this.MessageLabel.CssClass = "ErrorMessages";
            }
            else if (_messageCode == GUIShellTypes.MessagesCode.Unknown.ToString())
            {
                //Default
                this.MessageLabel.Text = Resource.UnknownErrorMessage_Label_Text;
                this.MessageLabel.CssClass = "ErrorMessages";
            }
            else
            {
                this.MessageLabel.Text = _messageCode;
                this.MessageLabel.CssClass = "ErrorMessages";
            }

            Exception serverEx = this.Server.GetLastError();

            //Add a more detailed exception message.
            if (serverEx != null)
            {
                //Log all unhandled exceptions.
                _coeLog.Log(serverEx.ToString());
                Exception baseException =serverEx.GetBaseException();

                if (baseException != null)
                    this.MessageLabel.Text += "<BR><BR>(" + baseException.Message + ")<BR>";
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to customize User Preference page , hide close button ,Menus &Logoff button
        /// </summary>
        private void CustomizeUserPreference()
        {
            string requestURL = this.Request.Url.Query;
            
            if (requestURL.Contains(Constants.ShowUserPreference_UrlParameter) && (requestURL.Contains("Ticket") || requestURL.Contains("ticket")))
            {
                this.GoBackButton.Visible = false;
                Control headerContainer = this.Master.FindControl("HeaderUserControl");
                if (headerContainer != null)
                {
                    Control menuOption = headerContainer.FindControl("GoToUltraWebMenu");
                    if (menuOption != null)
                        menuOption.Visible = false;
                    Control toolBarControl = headerContainer.FindControl("UltraWebToolbarControl");
                    if (toolBarControl != null)
                        toolBarControl.Visible = false;
                    Control logOffButton = headerContainer.FindControl("LogOffButton");
                    if (logOffButton != null)
                        logOffButton.Visible = false;
                    Control logoControl = headerContainer.FindControl("LogoContainer");
                    if (logoControl != null && logoControl is System.Web.UI.HtmlControls.HtmlGenericControl)
                        ((System.Web.UI.HtmlControls.HtmlGenericControl)logoControl).Attributes.Add("OnClick", "");
                }
            }
                    
        }

        #endregion
    }
}


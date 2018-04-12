using System;
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using Infragistics.WebUI.UltraWebGrid;
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.DocumentManager.Services.Types;

namespace DocManagerWeb.Forms.Public.ContentArea
{
    public partial class SearchDocument : GUIShellPage
    {
        #region GUIShell Variables

        DocManagerMaster _masterPage = null;
        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                this.InitSessionVars();
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetJScriptReference();
                    this.EmbedCBVForm();
                }

                this.SetPageTitle();
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
        }

        private void InitSessionVars()
        {
            // This is to ensure that the user is not forced to go to import a config. This assures there is a safe customization imported. Do not remove.
            if (Session[Constants.NewDocumentObject_Session] == null)
                Session[Constants.NewDocumentObject_Session] = Document.GetNewDocument();
        }

        private void EmbedCBVForm()
        {
            try
            {
                string CallerString = string.Empty;

                if (this.Request["Caller"] != null)
                    if (Request["Caller"] == "APRVD")
                        CallerString = "&SearchCriteriaId=12&SearchCriteriaValue=<>Approved";
                    else
                        CallerString = "&RememberPreviousState=true";

                this.SearchFrame.Attributes["src"] = Utilities.GetSearchPermURL() + CallerString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LeavePage()
        {
            try
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                if (_masterPage.PreviousPage.IsAbsoluteUri)
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else
                    Response.Redirect(_masterPage.PreviousPage.PathAndQuery);
            }
            finally
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            }
        }

        protected override void OnInit(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is DocManagerMaster)
            {
                _masterPage = (DocManagerMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            base.OnInit(e);
        }

        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            this.PageTitleLabel.Text = Resource.SearchPerm_Page_Title;
            this.GoHomeButton.Text = Resource.Back_Button_Text;
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.SearchDocumentRecord_Label_Text;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}

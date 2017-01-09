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
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Resources;
using CambridgeSoft.COE.Framework.COESearchService;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.SubmitRecord.ContentArea
{
    public partial class SearchCompound : GUIShellPage
    {
        #region GUIShell Variables

        GUIShellMaster _masterPage = null;
        private string _caller = string.Empty;

        #endregion

        #region Properties

        private string PreviousPageName
        {
            get { return ViewState["PreviousPageName"].ToString(); }
            set { ViewState["PreviousPageName"] = value; }
        }

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetJScriptReference();
                    this.SetPreviousPageName();
                    this.EmbedCBVForm();
                }
                this.SetPageTitle();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void EmbedCBVForm()
        {
            string caller = this.Request["Caller"].ToUpper();
            if (caller != null)
            {
                switch (caller)
                {
                    case "SR":
                        this.SearchComponentFrame.Attributes["src"] = RegUtilities.GetSearchComponentURL();
                        break;
                    case "RR":
                        this.SearchComponentFrame.Attributes["src"] = RegUtilities.GetSearchComponentRRURL();
                        break;
                    case "VM":
                        this.SearchComponentFrame.Attributes["src"] = RegUtilities.GetSearchComponentVMURL();
                        break;
                }
            }     
        }

        

        private void SetPreviousPageName()
        {
            if (Page.PreviousPage != null)
            {
                this.PreviousPageName = this.Page.PreviousPage.ToString().ToUpper();
                if (this.PreviousPageName.Equals(Constants.ReviewRegisterMixturePage))
                    this.GoHomeButton.CommandName = "GoToReviewRegisterMixture";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is GUIShellMaster)
            {
                _masterPage = (GUIShellMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                if (((Button)sender).CommandName.Equals("GoToReviewRegisterMixture"))
                    Server.Transfer(String.Format("{0}?{1}={2}", Resource.ReviewRegister_URL, PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter, Request[PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter]), false);
                else
                    Server.Transfer(String.Format("{0}?{1}={2}", Resource.SubmitMixture_URL,PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter, Request[PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter] ),false);
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            this.PageTitleLabel.Text = Resource.SearchComponent_Page_Title;
            this.GoHomeButton.Text = Resource.Cancel_Button_Text;
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.SearchComponent_Page_Title;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}

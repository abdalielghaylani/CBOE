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
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using System.Reflection;

namespace PerkinElmer.COE.Registration.Server.Forms.ComponentDuplicates.ContentArea
{
    public partial class ComponentDuplicatesSearch : GUIShellPage
    {
        #region GUIShell Variables

        GUIShellMaster _masterPage = null;
        private const string RDCALLERPARAMS = "&SearchCriteriaId=3&SearchCriteriaValue=>0&DefaultSearchAction=ListForm";
        private const string VMCALLERPARAMS = "&RememberPreviousState=true";

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
                    this.EmbedCBVForm();
                    RegUtilities.ClearNavigationPaths();
                    RegUtilities.UserNavigationPaths = Resource.ComponentDuplicatesSearchPage_URL;
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
            CallerEnum caller = CallerEnum.Undefined;
            try
            {
                caller = Request["Caller"] != null ? (CallerEnum)Enum.Parse(typeof(CallerEnum), Request["Caller"]) : CallerEnum.Undefined;
            }
            catch 
            {
                 caller = CallerEnum.Undefined;
            }            
            string aditionalParameters = string.Empty;
         
                switch (caller) 
                {
                    case CallerEnum.RD:
                        aditionalParameters = RDCALLERPARAMS;                        
                        break;
                    default:
                    case CallerEnum.VM:
                        aditionalParameters = VMCALLERPARAMS;
                        break;                        
                }

            this.ComponentDuplicatesSearchFrame.Attributes["src"] = RegUtilities.GetSearchDuplicatesComponentsURL() + aditionalParameters;
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
                Server.Transfer(Resource.Home_URL, false);
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
            this.PageTitleLabel.Text = Resource.ComponentsDuplicatesSearch_PageTitle;
            this.GoHomeButton.Text = Resource.Cancel_Button_Text;
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.ComponentsDuplicatesSearch_PageTitle;
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

        #region PageEmuns      
        
        private enum CallerEnum 
        {           
            Undefined,
            /// <summary>
            /// Review Duplicates
            /// </summary>
            RD,
            /// <summary>
            /// Search Duplicates
            /// </summary>
            SD,
            /// <summary>
            /// View Mixture
            /// </summary>
            VM
        }

        #endregion
    }
}

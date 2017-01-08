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
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Caching;


namespace PerkinElmer.CBOE.Registration.Client.Forms.ReviewRegister.ContentArea
{
    public partial class ViewMixtureSearch : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        //private string _controlToListen = String.Empty;
        //private string _groupToListen = String.Empty;
        private const int SERACHPERMFORMID = 4003;
        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                this.InitSessionVars();
                if(!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetJScriptReference();
                    this.EmbedCBVForm();
                }
                RegUtilities.ClearNavigationPaths();
                RegUtilities.UserNavigationPaths = Resource.ViewMixtureSearch_URL;
                
                this.SetPageTitle();
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        private void InitSessionVars()
        {
			// This is to ensure that the user is not forced to go to import a config. This assures there is a safe customization imported. Do not remove.
            if (Session[Constants.MultiCompoundObject_Session] == null)
                Session[Constants.MultiCompoundObject_Session] = RegUtilities.GetNewRegistryRecord();
        }

        private void EmbedCBVForm()
        {
            try
            {
                string CallerString = string.Empty;
                string cacheClearQuery = string.Empty;

                if (this.Request["Caller"] != null)
                    if(Request["Caller"] == "APRVD")
                        CallerString = "&SearchCriteriaId=12&SearchCriteriaValue=<>Approved";
                    else if (this.Request["Caller"] == "LASTSEARCHTODELETE" || this.Request["Caller"] == "LASTSEARCHTOLOCKED")
                    {
                        CallerString = "&" + GetDefaultSearchCriteriaOptions(true);
                    }
                    else 
                        CallerString = "&RememberPreviousState=true";

                if (ServerCache.Exists(PerkinElmer.CBOE.Registration.Client.Constants.ClearCache, typeof(PerkinElmer.CBOE.Registration.Client.Constants)))
                {
                    cacheClearQuery = "&" + Constants.ClearCache + "=" + Constants.ClearCache;
                    ServerCache.Remove(PerkinElmer.CBOE.Registration.Client.Constants.ClearCache, typeof(PerkinElmer.CBOE.Registration.Client.Constants));
                }

                this.SearchPermFrame.Attributes["src"] = RegUtilities.GetSearchPermURL() + CallerString + cacheClearQuery;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void LeavePage()
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                //string url = "../../Public/ContentArea/Home.aspx";
                //string url = _masterPage.AccordionControl.Groups.FromKey("SearchRegistryRecord").TargetUrl;
                if(_masterPage.PreviousPage.IsAbsoluteUri)
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else
                    Response.Redirect(_masterPage.PreviousPage.PathAndQuery);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            }
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if(this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster) this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            base.OnInit(e);
        }

        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch(Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (RegUtilities.OverrideDisplayErrorMessage)
                _masterPage.DisplayErrorMessage(RegUtilities.ErrorMessage, false);
            RegUtilities.ErrorMessage = string.Empty;
            RegUtilities.OverrideDisplayErrorMessage = false;
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
            this.PageTitleLabel.Text = Resource.SearchPerm_Page_Title;
            this.GoHomeButton.Text = Resource.Back_Button_Text;
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.SearchRegistryRecord_Label_Text;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if(!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
        private string GetDefaultSearchCriteriaOptions(bool addStatus)
        {
            string result = "SearchCriteriaId={0}&SearchCriteriaValue={1}&DefaultSearchAction=ListForm";
            int searchCriteriaID = -1;
            string searchCriteriaOp = ">0";

            SearchCriteria criteria = new SearchCriteria();
            COEFormBO formGroupBO = COEFormBO.Get(SERACHPERMFORMID);

            if (addStatus)
            {
                foreach (FormGroup.FormElement element in formGroupBO.COEFormGroup.QueryForms[0].Forms[0].LayoutInfo)
                {
                    if (element.Name == "STATUS")
                    {
                        searchCriteriaID = element.SearchCriteriaItem.ID;
                        break;
                    }
                }
            }
            if (searchCriteriaID == -1)
            {
                searchCriteriaOp = ">0";
                foreach (FormGroup.FormElement element in formGroupBO.COEFormGroup.QueryForms[0].Forms[0].LayoutInfo)
                {
                    if (element.Name == "MolWeight")
                    {
                        searchCriteriaID = element.SearchCriteriaItem.ID;
                        break;
                    }
                }
            }
            return string.Format(result, (searchCriteriaID == -1 ? 0 : searchCriteriaID), searchCriteriaOp);
        }

    }
}

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
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Resources;
using CambridgeSoft.COE.Framework.COESearchService;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace RegistrationWebApp.Forms.ReviewRegister.ContentArea
{
    public partial class ReviewRegisterSearch : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;

        private const int SERACHTEAMPFORMID = 4002;

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                this.InitSessionVars();
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetJScriptReference();
                    this.EmbedCBVForm();
                    //Fix for CSBR-149346 
                    //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
                    //Apart from this Bug, It rectified all flows from starting page to end at starting page in  Registration. 
                    Session["isSubmit"] = null;
                 }
                /*Fix for CBOE-879 & CBOE-880
                 * Previously when click on the Back button in the Review Register page is redirecting to the Submit Mixture page.
                 * For fixing this issue need to set the UserNavigation page as ReviewRegisterSearch
                 */
                RegUtilities.ClearNavigationPaths();
                RegUtilities.UserNavigationPaths = Resource.ReviewRegisterSearch_URL;

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
            if (Session[Constants.MultiCompoundObject_Session] == null)
                Session[Constants.MultiCompoundObject_Session] = RegUtilities.GetNewRegistryRecord();
        }

        private void EmbedCBVForm()
        {
            try
            {
                string url = string.Empty;
                string cacheClearQuery = string.Empty;

                if (this.Request["Caller"] != null)
                {
                    if (this.Request["Caller"].ToUpper() == "RR") // Review-Register
                        url = RegUtilities.GetSearchTempURL(false) + "&" + GetDefaultSearchCriteriaOptions(true);
                    else if (this.Request["Caller"].ToUpper() == "ALL") // Review ALL
                        url = RegUtilities.GetSearchTempURL(false) + "&" + GetDefaultSearchCriteriaOptions(false);
                    else if (this.Request["Caller"].ToUpper() == "AR") // Approved-Register
                        url = RegUtilities.GetSearchTempURL(false) + "&" + GetApprovedSearchCriteriaOptions();
                    /*Fix for CBOE-879 & CBOE-880
                     * For a new search or retrieve all in the Temporary Search page, no need for set the Restore Hitlist id as true
                     * if the Restore Hitlist is true then it will take the data from session, so it will 
                     * show the wrong data
                     * For fixing this issue changed the Restore Hitlist id as false
                    */
                    else if (this.Request["Caller"].ToUpper() == "ST") // Search Temp
                        url = RegUtilities.GetSearchTempURL(false);
                    else if (this.Request["Caller"].ToUpper() == "LASTSEARCH") // Last Search
                        url = RegUtilities.GetSearchTempURL(false) + "&RememberPreviousState=true";
                    else if (this.Request["Caller"].ToUpper() == "LASTSEARCHTODELETE") // Last Search
                        url = RegUtilities.GetSearchTempURL(false) + "&" + GetDefaultSearchCriteriaOptions(true);
                    else if (this.Request["Caller"].ToUpper() == "LASTSEARCHTOLOCKED") // Last Search
                        url = RegUtilities.GetSearchTempURL(false) + "&" + GetDefaultSearchCriteriaOptions(true);
                    //Fix for CSBR-149346 
                    //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
                    //Apart from this Bug, It rectified all flows from starting page to end at starting page in  Registration. 
                    else if (this.Request["Caller"].ToUpper() == "SP") // Search Parmanent
                        url = RegUtilities.GetSearchPermURL() + "&RememberPreviousState=true";
                }
                else if(string.IsNullOrEmpty(this.Request["COESavedHitListID"])) 
                {
                    string cOESavedHitListID = this.Request["COESavedHitListID"];
                    url = RegUtilities.GetSearchTempURL(true) + "&COESavedHitListID=" + cOESavedHitListID;

                }else
                    url = RegUtilities.GetSearchTempURL(true) + "&RememberPreviousState=true";
                
                  if (ServerCache.Exists(RegistrationWebApp.Constants.ClearCache, typeof(RegistrationWebApp.Constants)))
                  {
                      cacheClearQuery = "&" + Constants.ClearCache + "=" + Constants.ClearCache;
                      ServerCache.Remove(RegistrationWebApp.Constants.ClearCache, typeof(RegistrationWebApp.Constants));
                  }

                this.SearchTempFrame.Attributes["src"] = url + cacheClearQuery;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            /*string url = "../../Public/ContentArea/Home.aspx";
            Server.Transfer(url, false);*/
            //Fix for CSBR-157611
            //Clicking Back from Review Register and Search Temp not leading to Main Menu 
            bool bValidPage = true;
            if (_masterPage.PreviousPage !=null &&  _masterPage.PreviousPage.AbsolutePath.Contains("DeleteMarked.aspx"))
                bValidPage=false;
            if (bValidPage)
            {
                if (_masterPage.PreviousPage.IsAbsoluteUri)
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else
                    Server.Transfer(Resource.ReviewRegisterSearch_URL + "?Caller=ST", false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            catch (Exception exception)
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
            //Fix for CSBR-149346 
            //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
            //Apart from this Bug, It rectified all flows from starting page to end at starting page in  Registration. 
            if ((this.Request["Caller"].ToUpper() == "SP"))
            {
                this.PageTitleLabel.Text = Resource.SearchPerm_Page_Title;
                this.GoHomeButton.Text = Resource.Back_Button_Text;
            }
            else
            {
               this.PageTitleLabel.Text = Resource.SearchTemp_Page_Title;
               this.GoHomeButton.Text = Resource.Back_Button_Text;
            }
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

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Page Method

        private string GetDefaultSearchCriteriaOptions(bool addStatus)
        {
            string result = "SearchCriteriaId={0}&SearchCriteriaValue={1}&DefaultSearchAction=ListForm";
            int searchCriteriaID = -1;
            string searchCriteriaOp = "1"; //Status: Submitted

            SearchCriteria criteria = new SearchCriteria();

            COEFormBO formGroupBO = COEFormBO.Get(SERACHTEAMPFORMID);

            if (addStatus)
            {
                foreach (FormGroup.FormElement element in formGroupBO.COEFormGroup.QueryForms[0].Forms[0].LayoutInfo)
                {
                    if (element.Name == "Status")
                    {
                        searchCriteriaID = element.SearchCriteriaItem.ID;
                        break;
                    }
                }
            }

            if (searchCriteriaID == -1)
            {
                searchCriteriaOp = ">0"; //MW Greater or equal than 0
                foreach (FormGroup.FormElement element in formGroupBO.COEFormGroup.QueryForms[0].Forms[0].LayoutInfo)
                {
                    if (element.Name == "MW")
                    {
                        searchCriteriaID = element.SearchCriteriaItem.ID;
                        break;
                    }
                }
            }


            return string.Format(result, (searchCriteriaID == -1 ? 0 : searchCriteriaID), searchCriteriaOp);
        }

        private string GetApprovedSearchCriteriaOptions()
        {
            string result = "SearchCriteriaId={0}&SearchCriteriaValue=2&DefaultSearchAction=ListForm"; // Status: Approved
            int searchCriteriaID = -1;

            SearchCriteria criteria = new SearchCriteria();

            COEFormBO formGroupBO = COEFormBO.Get(SERACHTEAMPFORMID);

            foreach (FormGroup.FormElement element in formGroupBO.COEFormGroup.QueryForms[0].Forms[0].LayoutInfo)
            {
                if (element.Name == "Status")
                {
                    searchCriteriaID = element.SearchCriteriaItem.ID;
                    break;
                }
            }

            return string.Format(result, (searchCriteriaID == -1 ? 0 : searchCriteriaID));
        }
        
        #endregion

    }
}

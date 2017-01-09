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
using CambridgeSoft.COE.Registration.Services.BLL;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Resources;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Forms.SendToInventory
{
    public partial class SendToInventory : GUIShellPage
    {
        #region GUIShell Variables
        GUIShellMaster _masterPage = null;
        #endregion

        protected override void OnInit(EventArgs e)
        {
            try
            {
                if(this.Master is GUIShellMaster)
                {
                    _masterPage = (GUIShellMaster) this.Master;
                    _masterPage.ShowLeftPanel = false;
                }
                base.OnInit(e);
                //CSBR 117333 - Need to register the javascript page by invoking below function while initializing page
                SetJScriptReference();
            }
            catch(Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.InitSessionVars();

            if(!Page.IsPostBack)
                SetControlsAttributtes();

            string url = string.Empty;

            if(!string.IsNullOrEmpty(this.Page.Request["HitListID"]))
            {
                GetRegNumberListFromHitlistID.HitListID = int.Parse(this.Page.Request["HitListID"]);
                string regNumberList = GetRegNumberListFromHitlistID.Execute();
                
                if(!string.IsNullOrEmpty(regNumberList))
                {
                    string scriptKey = "OpenNewWindow";
                    string scriptFormat = "<script type=\"text/javascript\">WindowOnload(function(){{{0}}});</script>";
                    string windowAttributes = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable,height=740,width=1070px,top=0,left=100";

                    url = string.Format("{0}{1}{2}={3}&OpenAsModalFrame=false",
                                                    RegUtilities.GetSendToInventoryURL(),
                                                    RegUtilities.GetSendToInventoryURL().Contains("?") ? "&" : "?",
                                                    "RegIDList",
                                                    regNumberList);
                    
                    this.Page.ClientScript.RegisterClientScriptBlock(typeof(GUIShellPage),
                                                        scriptKey, string.Format(scriptFormat,
                                                        "window.open('" + url + "','Diag','" + windowAttributes + "');"));
                }
                else
                {
                    ShowConfirmationMessage(Resource.NoRegistriesMarked_Error_Text);
                }
            }
            else if (!string.IsNullOrEmpty(this.Page.Request["BatchID"]))
            {
                if (RegUtilities.GetUseFullContainerForm())
                {
                    url = string.Format("{0}{1}{2}={3}&RefreshOpenerLocation=false",
                                        RegUtilities.GetNewInventoryContainerURL(),
                                        RegUtilities.GetNewInventoryContainerURL().Contains("?") ? "&" : "?",
                                        "vRegBatchID",
                                        this.Page.Request["BatchID"]);
                }
                else
                {
                    RegistryRecord record = RegistryRecord.GetRegistryRecordByBatch(int.Parse(this.Page.Request["BatchID"]));
                    url = string.Format("{0}{1}{2}={3}&OpenAsModalFrame=false",
                                        RegUtilities.GetSendToInventoryURL(),
                                        RegUtilities.GetSendToInventoryURL().Contains("?") ? "&" : "?",
                                        "RegIDList",
                                        record.RegNumber.ID);
                }

                Response.Redirect(url, true);
            }
            else
                Server.Transfer(this.Page.Request.UrlReferrer.AbsolutePath, false);
        }

        private void InitSessionVars()
        {
            // This is to ensure that the user is not forced to go to import a config. This assures there is a safe customization imported. Do not remove.
            if (Session[Constants.MultiCompoundObject_Session] == null)
                Session[Constants.MultiCompoundObject_Session] = RegistryRecord.NewRegistryRecord();
        }

        protected override void SetControlsAttributtes()
        {
            this.DoneButton.Text = Resource.Done_Button_Text;
            this.HomeButton.Text = Resource.Home_Button_Text;
            this.Header.Title = this.PageTitleLabel.Text = "Send to Inventory";
        }

        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if(!string.IsNullOrEmpty(messageToDisplay))
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DoneButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Response.Redirect(string.Format("{0}?Caller=VM", Resources.Resource.ViewMixtureSearch_URL));
            }
            catch(Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        
        protected void HomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(Resource.RegisterMarked_LeavePage_Url_Text, false);
            }
            catch(Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        /*CSBR # - 117333
         * Changed by - Soorya Anwar
         * Date - 25-Jun-2010
         * Purpose - Used the SetJScriptReference() function from ViewMixture.aspx.cs inorder to register the commonUtilities.js file,
         *           from which the OpenNewWindow is to be invoked for popping up Inventory page as new window.
         */
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptUtilitiesKey = this.ID.ToString() + "CommonUtilitiesPage";
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptUtilitiesKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptUtilitiesKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonUtilities.js"));
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        //End of Change for CSBR - 117333
    }
}

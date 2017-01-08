using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebListbar;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Reflection;
using Resources;
using System.Configuration;
using PerkinElmer.CBOE.Registration.Client.Forms.Master;
using CambridgeSoft.COE.Framework.Controls.WebParts;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using System.Web.UI.WebControls.WebParts;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.CBOE.Registration.Client.Forms.Public.ContentArea
{
    public partial class Home : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        private int rowCounter = 0;

        #endregion

        #region Page Properties

        
        #endregion

        #region GUIShell Properties

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                RegUtilities.CleanSession(Session);

                this.InitSessionVars();

                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetHomeWebParts();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
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

        protected void DoLogOff(object sender, EventArgs e)
        {
            GUIShellUtilities.DoLogout();
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            _masterPage.SetPageTitle(Resources.Resource.Home_Page_Title);                        
            Control footerRow = _masterPage.FindControl(GUIShellTypes.FooterRowControlId);
            if (footerRow != null) footerRow.Visible = false;

            Control headerRow = _masterPage.FindControl(GUIShellTypes.HeaderRowControlId);
            if (headerRow != null) headerRow.Visible = false;

            //this.WelcomeLiteral.Text = Resource.Welcome_Label_Text + HttpContext.Current.User.Identity.Name.ToUpper();
            this.PE_Logo.ImageUrl = "/coecommonresources/Utility_Images/BackGroundImages/PKI_FTB_Logo_RGB_small.jpg";
            this.MainTextLiteral.Text = Resource.HomeMain_Label_Text;
            this.FrameworkVersionLiteral.Text = Resource.FrameworkVersion_Label_Text + ConfigurationUtilities.GetFrameworkFileVersion();
            this.RegVersionLiteral.Text = Resource.RegistrationVersion_Label_Text + CambridgeSoft.COE.Registration.Services.Common.RegSvcUtilities.GetFileVersion();

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void InitSessionVars()
        {
			// This is to ensure that the user is not forced to go to import a config. This assures there is a safe customization imported. Do not remove.
            if (Session[Constants.MultiCompoundObject_Session] == null)
                Session[Constants.MultiCompoundObject_Session] = RegUtilities.GetNewRegistryRecord();
        }

        private void SetHomeWebParts()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            ApplicationHome homeData = ConfigurationUtilities.GetApplicationHomeData(RegUtilities.GetApplicationName());
            int numberOfColumns = 1;
            WebPartManager webmgr = WebPartManager1;
            int wpUsed = 0;
            for (int i = 0; i < homeData.Groups.Count; i++)
            {
                CambridgeSoft.COE.Framework.Common.Group myGroup = homeData.Groups.Get(i);

                if (myGroup != null)
                {
                    HomeWebPart webpartPreCheck = new HomeWebPart();
                    webpartPreCheck.Group = myGroup;
                    if (webpartPreCheck.HasLinks())
                    {
                        string webPartNumber = string.Empty;
                        HomeWebPart webpart = null;
                        switch (myGroup.PageSectionTarget.ToUpper())
                        {
                            case "PANEL":
                                webPartNumber = GetWebPartNumber(numberOfColumns, wpUsed);
                                //the webpart will figure out what links can be shown base on the users permissions
                                webpart = (HomeWebPart)webmgr.WebParts["PanelWebPart" + webPartNumber];

                                if (bool.Parse(RegUtilities.GetConfigSetting(RegUtilities.Groups.RegAdmin, "EnableMixtures", true)) == false)
                                {
                                    myGroup.LinksData.Remove("SubmitMixture");
                                }else
                                {
                                    //I have found if I change the system setting then I can never get submitmixture back!! So I need to get the group back

                                     myGroup = homeData.Groups.Get(i);
                                     

                                }

                                webpart.Hidden = false;
                                webpart.Group = myGroup;
                                wpUsed = wpUsed + 1;
                                break;
                            case "DASHBOARD":
                                //Hardcode removal of approvals link for now.
                                if (bool.Parse(RegUtilities.GetConfigSetting(RegUtilities.Groups.RegAdmin, "ApprovalsEnabled", true)) == false)
                                {
                                    myGroup.CustomItems.Remove("PendingApprovalPermRegistries");
                                    //if approvals is not enabled remove the redundant link for temporary registries. it will always be the same as "alltempregistries"
                                    myGroup.CustomItems.Remove("SubmittedTempRegistries");
                                    myGroup.CustomItems.Remove("ApprovedTempRegistries");
                                    myGroup.CustomItems.Remove("TempRegistries");
                                }
                               
                           

                                webPartNumber = GetWebPartNumber(numberOfColumns, wpUsed);
                                //the webpart will figure out what links can be shown base on the users permissions
                                webpart = (HomeWebPart)webmgr.WebParts["DashWebPart" + webPartNumber];
                                webpart.Hidden = false;
                                webpart.Group = myGroup;
                                wpUsed = wpUsed + 1;

                                break;

                        }
                    }
                }
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private string GetWebPartNumber(int totalColumns, int counter)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            counter = counter + 1; // increment by one since we started at 0
            int curRow = -1;
            int curColumn = -1;
            if (counter % totalColumns == 0)
            {

                curRow = rowCounter;
                rowCounter = rowCounter + 1;
                curColumn = totalColumns - 1;
            }
            else
            {
                curRow = rowCounter;
                curColumn = (counter % totalColumns) - 1;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return curColumn + "_" + curRow; ;
            
        }

        #endregion
    }
}

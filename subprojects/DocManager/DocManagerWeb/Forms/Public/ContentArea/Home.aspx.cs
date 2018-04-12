using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Resources;
using System.Configuration;
using System.Web.UI.WebControls.WebParts;

using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.DocManagerWeb.Forms.Master;
using CambridgeSoft.COE.Framework.Controls.WebParts;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.DocumentManager.Services.Types;


using Infragistics.WebUI.UltraWebListbar;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace DocManagerWeb.Forms.Public.ContentArea
{
    public partial class Home : GUIShellPage
    {
        #region GUIShell Variables

        DocManagerMaster _masterPage = null;
        private int rowCounter = 0;

        #endregion

        #region Page Properties

        
        #endregion

        #region GUIShell Properties

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Utilities.CleanSession(Session);
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    this.SetHomeWebParts();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            base.OnInit(e);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            _masterPage.SetPageTitle(Resources.Resource.Home_Page_Title);                        
            Control footerRow = _masterPage.FindControl(GUIShellTypes.FooterRowControlId);
            if (footerRow != null) footerRow.Visible = false;

            Control headerRow = _masterPage.FindControl(GUIShellTypes.HeaderRowControlId);
            if (headerRow != null) headerRow.Visible = false;

            //this.WelcomeLiteral.Text = Resource.Welcome_Label_Text + HttpContext.Current.User.Identity.Name.ToUpper();
            this.CSoft_Logo.ImageUrl = Utilities.ThemesCommonImagesPath + "Clientlogo.jpg";
            this.MainTextLiteral.Text = Resource.HomeMain_Label_Text;
            this.FrameworkVersionLiteral.Text = Resource.FrameworkVersion_Label_Text + ConfigurationUtilities.GetFrameworkFileVersion();
            this.RegVersionLiteral.Text = Resource.DocManagerVersion_Label_Text + Utilities.GetFileVersion();

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetHomeWebParts()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            ApplicationHome homeData = ConfigurationUtilities.GetApplicationHomeData(Utilities.GetApplicationName());
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
                                webpart.Hidden = false;
                                webpart.Group = myGroup;
                                wpUsed = wpUsed + 1;
                                break;
                            case "DASHBOARD":
                                //Hardcode removal of approvals link for now.
                                if(bool.Parse(Utilities.GetConfigSetting("DocMgr", "ApprovalsEnabled", true)) == false)
                                    myGroup.CustomItems.Remove("PendingApprovalPermRegistries");                             

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
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private string GetWebPartNumber(int totalColumns, int counter)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return curColumn + "_" + curRow; ;
            
        }

        #endregion
    }
}

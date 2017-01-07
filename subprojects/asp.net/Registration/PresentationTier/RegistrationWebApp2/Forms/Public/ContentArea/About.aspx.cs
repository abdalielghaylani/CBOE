using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebListbar;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Resources;

namespace RegistrationWebApp.Forms.Public.ContentArea
{
    public partial class About : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
       
        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!Page.IsPostBack)
            {
                this.SetControlsAttributtes();
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page GUIShell Settings
            // To make easier to read the source code.
            //Fix for CSBR 133866
            //Hiding the Header user control of Master page in About.aspx page.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
                if (_masterPage.FindControl("HeaderUserControl") != null)
                    _masterPage.FindControl("HeaderUserControl").Visible = false;
            }

                
            #endregion

            base.OnInit(e);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.PageTitleLabel.Text = Resource.About_Title_Label;
            this.SupportTitleLabel.Text = Resources.Resource.Support_Label_Text;
            this.SupportContentLabel.Text = Resource.SupportContent_Label_Text;
            this.OrdersTitleLabel.Text = Resource.Orders_Label_Text;
            this.OrdersContentLabel.Text = Resource.OrdersContent_Label_Text;
            this.VersionTitileLabel.Text = Resource.Version_Label_Text;
            this.VersionContentLabel.Text = string.Format(Resource.VersionContent_Label_Text, CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion(),
                                                                            CambridgeSoft.COE.Registration.Services.Common.RegSvcUtilities.GetFileVersion());
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        #endregion
    }
}
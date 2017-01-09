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
using CambridgeSoft.COE.RegistrationAdmin.Services;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea
{
    public partial class ValidationRules : GUIShellPage
    {
        #region Variables
        RegistrationMaster _masterPage = null;
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    SetControlsAttributtes();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
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
            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Properties
        private ConfigurationRegistryRecord ConfigurationRegistryRecord {
            get {
                return (ConfigurationRegistryRecord) Session["ConfigurationRegistryRecord"];
            }
            set {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }
        #endregion

        #region Methods
        protected override void SetControlsAttributtes()
        {
            this.Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.ValidationRules_Page_Title;
        }

        //private void SaveConfigurationBO()
        //{
        //    this.ConfigurationRegistryRecord.CreateCoeFormTemplates();
        //    this.ConfigurationRegistryRecord = this.ValidationRules1.ConfigurationBO.Save();
        //}

        //private void RevertConfigurationBO()
        //{
        //    if (this.ValidationRules1.ConfigurationBO.IsDirty)
        //        this.ConfigurationRegistryRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
        //}
        #endregion

        #region Event Handlers
        //protected void Btn_Save_Click(object sender, EventArgs e)
        //{
        //    SaveConfigurationBO();
        //}       

        //protected void Btn_Revert_Click(object sender, EventArgs e)
        //{
        //    this.RevertConfigurationBO();
        //}
        #endregion

    }
}

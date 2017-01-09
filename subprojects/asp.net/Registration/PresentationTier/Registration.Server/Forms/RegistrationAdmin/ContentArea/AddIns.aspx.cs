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
using Csla.Web;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.ContentArea
{
    public partial class AddIns : GUIShellPage
    {

        #region Variables
        RegistrationMaster _masterPage = null;
        CslaDataSource _addInListDatasource = new CslaDataSource();
        #endregion

        #region Properties

        private ConfigurationRegistryRecord ConfigurationRegistryRecord
        {
            get
            {
                return (ConfigurationRegistryRecord) Session["ConfigurationRegistryRecord"];
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    SetControlsAttributtes();

                if (this.AddInList1.ConfigurationRegistryRecord.SelectedPropertyList != ConfigurationRegistryRecord.PropertyListType.AddIns)
                    this.AddInList1.ConfigurationRegistryRecord.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.AddIns;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }
        #endregion

        #region Methods

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
            this.SetJScriptReference();
            base.OnInit(e);

            this.Controls.Add(_addInListDatasource);
            _addInListDatasource.ID = "AddInListDS";
            _addInListDatasource.TypeAssemblyName = "CambridgeSoft.COE.Registration.Services";
            _addInListDatasource.TypeName = "AddInList";
            _addInListDatasource.SelectObject += new EventHandler<SelectObjectArgs>(AddInListDatasource_SelectObject);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void AddInListDatasource_SelectObject(object sender, SelectObjectArgs e)
        {
            if(this.ConfigurationRegistryRecord == null)
                this.ConfigurationRegistryRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

            e.BusinessObject = this.ConfigurationRegistryRecord.AddInList;
        }

        protected override void SetControlsAttributtes()
        {
            Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.AddIns_PageTitle;
        }


        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if(!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, this.Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SaveConfigurationBO()
        {
            //this.ConfigurationRegistryRecord.CreateCoeFormTemplates();
            this.ConfigurationRegistryRecord = this.AddInList1.ConfigurationRegistryRecord.Save();
            this.AddInList1.FillAddInList();
        }

        private void RevertConfigurationBO()
        {
            if(this.AddInList1.ConfigurationRegistryRecord.IsDirty)
                this.ConfigurationRegistryRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
        }

        #endregion

        #region Event Handlers

        //protected void Btn_Save_Click(object sender, EventArgs e)
        //{
        //    this.SaveConfigurationBO();
        //}

        //protected void Btn_Revert_Click(object sender, EventArgs e)
        //{
        //    this.RevertConfigurationBO();
        //}

        #endregion

    }
}

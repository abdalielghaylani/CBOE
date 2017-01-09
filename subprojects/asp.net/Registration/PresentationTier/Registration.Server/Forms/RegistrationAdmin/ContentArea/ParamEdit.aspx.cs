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
using CambridgeSoft.COE.Registration.Services.Types;
using Csla;
using CambridgeSoft.COE.Registration.Services.BLL;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.ContentArea
{
    public partial class ParamEdit : GUIShellPage
    {
        #region Variables
        RegistrationMaster _masterPage = null;
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.FillParameters();
                    this.SetControlsAttributtes();
                }
                this._masterPage.SetDefaultAction(this.BtnSave.UniqueID);
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

        private int SelValRuleID
        {
            get
            {
                return (int)Session["SelValRuleID"];
            }
        }

        private ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                if (Session["ConfigurationRegistryRecord"] == null)
                    return ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                else
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
            }
            set
            {
                if (value != null)
                    Session["ConfigurationRegistryRecord"] = value;
                else
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            }
        }

        private Property SelectedProperty
        {
            get
            {
                return (Property)Session["SelectedProperty"];
            }
            set
            {
                Session["SelectedProperty"] = value;
            }

        }

        #endregion

        #region Methods

        private void FillParameters()
        {
            this.UltraWebTreeParams.ClearAll();

            if (ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters != null)
            {

                foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters)
                {
                    this.UltraWebTreeParams.Nodes.Add(param.Name + " = " + param.Value);
                }

                //if (ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters.NewList != null)
                //{
                //    foreach (ParameterBO param in ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters.NewList)
                //    {
                //        this.UltraWebTreeParams.Nodes.Add(param.Name + " = " + param.Value);
                //    }
                //}

                for (int i = 0; i < UltraWebTreeParams.Nodes.Count; i++)
                {
                    UltraWebTreeParams.Nodes[i].Nodes.Add("select parameter");
                }
            }
        }

        protected override void SetControlsAttributtes()
        {
            this.BtnAddParameter.Text = Resources.Resource.Add_Parameter_Button_Text;
            this.BtnDeleteParameter.Text = Resources.Resource.Delete_Parameter_Button_Text;
            this.LblName.Text = Resources.Resource.Name_Label_Text;
            this.LblValue.Text = Resources.Resource.Value_Label_Text;
            this.PanelParamEdit.GroupingText = Resources.Resource.Edit_Parameters_Panel_Text;
            this.BtnSave.Text = Resources.Resource.Save_Button_Text;
            this.TxtName.Text = "validWord";
            this.TxtName.Enabled = false;
        }

        private void AddParameter()
        {
            string name = "validWord";
            string value = this.TxtValue.Text;
            
            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters.Add(CambridgeSoft.COE.Registration.Services.BLL.Parameter.NewParameter(name,value,true));

            Session["SelectedProperty"] = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name];

            this.FillParameters();
            this.TxtName.Text = "validWord";
            this.TxtName.Enabled = false;
            this.TxtValue.Text = string.Empty;
        }

        private void DeleteParameter()
        {
            if (UltraWebTreeParams.SelectedNode.Index != -1)
            {
                ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[SelValRuleID].Parameters.RemoveAt(UltraWebTreeParams.SelectedNode.Index);
            }
            Session["SelectedProperty"] = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name];
            this.FillParameters();
            this.TxtName.Text = "validWord";
            this.TxtName.Enabled = false;
        }

        private void SaveChanges()
        {
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/ValidationRules.aspx");
        }

        #endregion

        #region Event Handlers

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveChanges();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnAddParameter_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddParameter();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnDeleteParameter_Click(object sender, EventArgs e)
        {
            try
            {
                this.DeleteParameter();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

    }
}

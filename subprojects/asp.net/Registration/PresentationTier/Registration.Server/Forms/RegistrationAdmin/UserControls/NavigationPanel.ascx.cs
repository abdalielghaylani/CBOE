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
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.UserControls {
    public partial class NavigationPanel : System.Web.UI.UserControl
    {
        #region Properties
        private ConfigurationRegistryRecord ConfigurationRegistryRecord {
            get {
                if(Session["ConfigurationRegistryRecord"] == null) {
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                    return (ConfigurationRegistryRecord) Session["ConfigurationRegistryRecord"];
                } else
                    return (ConfigurationRegistryRecord) Session["ConfigurationRegistryRecord"];
            }
            set {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }
        #endregion

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e) {

        }
        #endregion

        #region Event Hndlers
        protected void LinkButton_BatchComponent_Click1(object sender, EventArgs e) 
        {
            try
            {
                GoTo(1);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        protected void LinkButton_Batch_Click(object sender, EventArgs e)
        {
            try
            {
                GoTo(0);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        protected void LinkButton_Compound_Click(object sender, EventArgs e) 
        {
            try
            {
                GoTo(2);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        protected void LinkButton_PropertyList_Click(object sender, EventArgs e) 
        {
            try
            {
                GoTo(3);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        protected void LinkButton_Structure_Click(object sender, EventArgs e)
        {
            try
            {
                GoTo(4);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        #endregion

        #region Methods
        public void SetSlectedTab(int selected)
        {
            try
            {
                switch (selected)
                {
                    case 0:

                        this.LiPropertyList.Attributes.Remove("class");
                        this.LiPropertyList.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatch.Attributes.Remove("class");
                        this.LiBatch.Attributes.Add("class", "SelectedTabsHeader");

                        this.LiBatchComponent.Attributes.Remove("class");
                        this.LiBatchComponent.Attributes.Add("class", "LiTabsHeader");

                        this.LiCompound.Attributes.Remove("class");
                        this.LiCompound.Attributes.Add("class", "LiTabsHeader");

                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "LiTabsHeader");

                        break;

                    case 1:


                        this.LiPropertyList.Attributes.Remove("class");
                        this.LiPropertyList.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatch.Attributes.Remove("class");
                        this.LiBatch.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatchComponent.Attributes.Remove("class");
                        this.LiBatchComponent.Attributes.Add("class", "SelectedTabsHeader");

                        this.LiCompound.Attributes.Remove("class");
                        this.LiCompound.Attributes.Add("class", "LiTabsHeader");

                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "LiTabsHeader");

                        break;

                    case 2:


                        this.LiPropertyList.Attributes.Remove("class");
                        this.LiPropertyList.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatch.Attributes.Remove("class");
                        this.LiBatch.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatchComponent.Attributes.Remove("class");
                        this.LiBatchComponent.Attributes.Add("class", "LiTabsHeader");

                        this.LiCompound.Attributes.Remove("class");
                        this.LiCompound.Attributes.Add("class", "SelectedTabsHeader");

                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "LiTabsHeader");

                        break;

                    case 3:

                        this.LiPropertyList.Attributes.Remove("class");
                        this.LiPropertyList.Attributes.Add("class", "SelectedTabsHeader");

                        this.LiBatch.Attributes.Remove("class");
                        this.LiBatch.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatchComponent.Attributes.Remove("class");
                        this.LiBatchComponent.Attributes.Add("class", "LiTabsHeader");

                        this.LiCompound.Attributes.Remove("class");
                        this.LiCompound.Attributes.Add("class", "LiTabsHeader");

                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "LiTabsHeader");

                        break;

                    case 4:

                        this.LiPropertyList.Attributes.Remove("class");
                        this.LiPropertyList.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatch.Attributes.Remove("class");
                        this.LiBatch.Attributes.Add("class", "LiTabsHeader");

                        this.LiBatchComponent.Attributes.Remove("class");
                        this.LiBatchComponent.Attributes.Add("class", "LiTabsHeader");

                        this.LiCompound.Attributes.Remove("class");
                        this.LiCompound.Attributes.Add("class", "LiTabsHeader");

                        this.LiStructure.Attributes.Remove("class");
                        this.LiStructure.Attributes.Add("class", "SelectedTabsHeader");

                        break;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        private void GoTo(int selected)
        {
            switch (selected)
            {
                case 0:
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Batch.aspx");
                    break;
                case 1:
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/BatchComponent.aspx");
                    break;
                case 2:
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Compound.aspx");
                    break;              
                case 3:
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
                    break;
                case 4:
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Structure.aspx");
                    break;
            }

        }
        #endregion
    }
}
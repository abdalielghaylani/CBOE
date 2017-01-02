using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.RegistrationAdmin;
using System.Xml;
using Resources;

namespace CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.UserControls
{
    public partial class PropertyList : System.Web.UI.UserControl
    {
        #region Variables

        private CourrentPageEnum _courrentPage;
       

        #endregion

        #region Properties

        public string DataSourceID
        {
            get
            {
                if (ViewState["DataSourceID"] == null)
                    ViewState["DataSourceID"] = string.Empty;

                return (string)ViewState["DataSourceID"];
            }
            set
            {
                ViewState["DataSourceID"] = value;
            }
        }

        public CourrentPageEnum CourrentPage
        {
            get
            {
                return _courrentPage;
            }
            set
            {
                _courrentPage = value;
            }
        }

        private ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                if (Session["BusinessObject"] == null)
                    Session["BusinessObject"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                return (ConfigurationRegistryRecord)Session["BusinessObject"];
            }
        }
        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {

            SetControlsAttributes();
            FillPropertyList();           

        }

        #endregion

        #region Private Methods

        internal void SetControlsAttributes()
        {
            this.PropertyListPanel.GroupingText = Resource.Property_List_Panel_Text;
            this.EnterNewPropertyPanel.GroupingText = Resource.Enter_New_Property_Panel_Text;
            this.LblName.Text = Resource.Name_Label_Text;
            this.LblType.Text = Resource.Type_Label_Text;
            this.LblPrecision.Text = Resource.Precision_Label_Text;
            this.BtnCancel.Text = Resource.Cancel_Button_Text;
            this.BtnAddProperty.Text = Resource.Add_Property_Button_Text;
            this.BtnCancelProp.Text = Resource.Cancel_Button_Text;
            this.BtnDeleteProp.Text = Resource.Delete_Property_Button_Text;
            this.Delete_Property_Panel_Text.GroupingText = Resource.DeleteProperty_Panel_Text;
            switch (_courrentPage)
            {
                case CourrentPageEnum.Compound: this.PropertyListPanel.GroupingText = Resource.Compound_Label_Panel_Text;
                    break;
                case CourrentPageEnum.Batch: this.PropertyListPanel.GroupingText = Resource.Batch_Label_Panel_Text;
                    break;
                case CourrentPageEnum.BatchComponent: this.PropertyListPanel.GroupingText = Resource.Batch_Component_Panel_Text;
                    break;
            }

        }
        private void AddProperty()
        {
            ConfigurationProperty confProperty = ConfigurationProperty.NewConfigurationProperty(this.TxtName.Text, this.DdlType.SelectedValue, this.TxtPresicion.Text);

            switch ((int)this._courrentPage)
            {
                case 0:
                    ConfigurationBO.BatchPropertyList.Add(confProperty);
                    break;
                case 1:
                    ConfigurationBO.BatchComponentList.Add(confProperty);
                    break;
                case 2:
                    ConfigurationBO.CompoundPropertyList.Add(confProperty);
                    break;
            }
        }
        private void ClearProperty()
        {
            this.TxtName.Text = string.Empty;
            this.TxtPresicion.Text = string.Empty;
        }

        private void FillPropertyList()
        {
            switch ((int)this._courrentPage)
            {
                case 0: this.FillBatchPropertyList();
                    break;
                case 1: this.FillBatchComponentPropertyList();
                    break;
                case 2: FillCompoundPropertyList();
                    break;
            }
        }
        private void FillBatchPropertyList()
        {
             this.Cbl_Properties.Items.Clear();
             for (int i = 0; i < (ConfigurationBO.BatchPropertyList.Count); i++)
             {
                 ListItem item = new ListItem(ConfigurationBO.BatchPropertyList[i].Name, ConfigurationBO.BatchPropertyList[i].Name);
                 this.Cbl_Properties.Items.Add(item);
             }

        }
        private void FillBatchComponentPropertyList()
        {
            this.Cbl_Properties.Items.Clear();
            for (int i = 0; i < (ConfigurationBO.BatchComponentList.Count); i++)
            {
                ListItem item = new ListItem(ConfigurationBO.BatchComponentList[i].Name, ConfigurationBO.BatchComponentList[i].Name);
                this.Cbl_Properties.Items.Add(item);
            }
        }
        private void FillCompoundPropertyList()
        {
            this.Cbl_Properties.Items.Clear();
            for (int i = 0; i < (ConfigurationBO.CompoundPropertyList.Count); i++) 
            {
                ListItem item = new ListItem(ConfigurationBO.CompoundPropertyList[i].Name, ConfigurationBO.CompoundPropertyList[i].Name);
                this.Cbl_Properties.Items.Add(item);

            }
           
        }

        #endregion

        #region Event Handlers

        protected void BtnAddProperty_Click(object sender, EventArgs e)
        {
            this.AddProperty();
        }
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            this.ClearProperty();
        }

    }
        #endregion

    #region Enums
    public enum CourrentPageEnum
    {
        Batch = 0,
        BatchComponent = 1,
        Compound = 2
    }

    #endregion

}
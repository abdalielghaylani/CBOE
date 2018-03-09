namespace Manager.Forms.DataViewManager.UserControls
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Web.UI.WebControls;

    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.Common;

    using Manager.Code;
    using Manager.Forms.DataViewManager.ContentArea;
    using System.Collections;
    using CambridgeSoft.COE.Framework.Common.Utility;

    /// <summary>
    /// The user control for manager database instance
    /// </summary>
    public partial class ManageDbInstance : System.Web.UI.UserControl
    {
        /// <summary>
        /// The database type list
        /// </summary>
        private static readonly List<DbType> DbTypeList = new List<DbType>();

        /// <summary>
        /// The command event handler to refresh parent's page
        /// </summary>
        public event CommandEventHandler RefreshParent = null;

        /// <summary>
        /// The command event handler for cancel process
        /// </summary>
        public event CommandEventHandler Cancel = null;

        /// <summary>
        /// Initialize for add database instance
        /// </summary>
        public void AddDbInstance()
        {
            if (!this.IsPostBack)
            {
                this.txtInstanceName.BackColor = SystemColors.Window;
                this.txtInstanceName.ReadOnly = false;
                this.txtInstanceName.Text = string.Empty;
                this.ddlInstanceType.SelectedIndex = 0;
                this.txtHostname.Text = string.Empty;
                this.txtPort.Text = string.Empty;
                this.txtSid.Text = string.Empty;
            }
        }

        /// <summary>
        /// Initialize for edit database instance
        /// </summary>
        /// <param name="instanceName">The instance name</param>
        public void EditDbInstance(string instanceName)
        {
            if (!this.IsPostBack)
            {
                COEInstanceBOList list = COEInstanceBOList.GetCOEInstanceBOList();
                COEInstanceBO instance = list.GetInstance(instanceName);
                if (instance != null)
                {
                    this.txtInstanceName.BackColor = SystemColors.Control;
                    this.txtInstanceName.ReadOnly = true;
                    this.txtInstanceName.Text = instance.InstanceName;
                    this.ddlInstanceType.SelectedIndex = DbTypeList.FindIndex(t => t.Name == instance.DbmsType.ToString());
                    this.dllDriverType.SelectedValue = COEConvert.PorcessDriverTypeForDisplay(instance.DriverType.ToString());
                    this.txtHostname.Text = instance.HostName;
                    this.txtPort.Text = instance.Port.ToString();
                    this.txtSid.Text = instance.SID;
                }
            }
        }

        /// <summary>
        /// Process while page loading
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // bind date for instance type
                var list = COEConfiguration.GetAllDBMSTypesInConfig();
                DbTypeList.Clear();
                int i = 0;
                foreach (var item in list)
                {
                    DbTypeList.Add(new DbType { Name = item.Name, Value = i++ });
                }

                this.ddlInstanceType.DataSource = DbTypeList;
                this.ddlInstanceType.DataBind();

                List<DisplayDriverType> driverTypeList = new List<DisplayDriverType>();

                foreach (var driverItem in Enum.GetNames(typeof(DriverType)))
                {
                    driverTypeList.Add(new DisplayDriverType { Name = COEConvert.PorcessDriverTypeForDisplay(driverItem.ToString()), Value = COEConvert.PorcessDriverTypeForDisplay(driverItem.ToString()) });
                }

                this.dllDriverType.DataSource = driverTypeList;
                this.dllDriverType.DataBind();
            }
        }

        /// <summary>
        /// Process while cancel button clicked
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected void BtnCancel_ButtonClicked(object sender, EventArgs e)
        {
            if (this.Cancel != null)
            {
                this.Cancel(sender, new CommandEventArgs("Cancel", string.Empty));
            }
        }

        /// <summary>
        /// Process while save button clicked
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected void BtnSave_ButtonClicked(object sender, EventArgs e)
        {
            // Validate the instance host name. The host name cannot be "localhost" or "127.0.0.1".
            if (!string.IsNullOrEmpty(txtHostname.Text))
            {
                if (txtHostname.Text.Trim().Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ||
                    txtHostname.Text.Trim().Equals("127.0.0.1", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.validator.ErrorMessage = @"Host name can not be 'localhost' or loopback address, please use computer name or IP address.";
                    this.validator.IsValid = false;
                    return;
                }
            }

            COEInstanceBO instance = null;
            // Check if this is new creation mode or modification mode.
            bool isNewPublishing = !this.txtInstanceName.ReadOnly;

            if (isNewPublishing)
            {
                var defaultGlobalUserName = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserName"];
                var defaultGlobalUserPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserPassword"];

                //check same instance name
                InstanceData instanceData = ConfigurationUtilities.GetInstanceData(txtInstanceName.Text.Trim().ToUpper());

                if (instanceData != null)
                {
                    this.validator.ErrorMessage = Resources.Resource.DataSourceExistsError;
                    this.validator.IsValid = false;
                    return;
                }

                // add instance
                instance = new COEInstanceBO(
                    Guid.NewGuid(),
                    txtInstanceName.Text.Trim().ToUpper(),
                    (DBMSType)Enum.Parse(typeof(DBMSType), ddlInstanceType.Text),
                    defaultGlobalUserName,
                    false,
                    true,
                    defaultGlobalUserPassword,
                    txtHostname.Text,
                    Convert.ToInt32(txtPort.Text),
                    txtSid.Text,
                    COEConvert.PorcessDriverTypeForSave(dllDriverType.Text));
            }
            else
            {
                var instanceList = COEInstanceBOList.GetCOEInstanceBOList();
                // update instance
                instance = instanceList.GetInstance(txtInstanceName.Text.Trim().ToUpper());
                instance.OldDefaultDatabase = instance.InstanceName + "." + instance.DatabaseGlobalUser;
                instance.DbmsType = (DBMSType)Enum.Parse(typeof(DBMSType), ddlInstanceType.Text);
                instance.DriverType = COEConvert.PorcessDriverTypeForSave(dllDriverType.Text);
                instance.HostName = txtHostname.Text;
                instance.Port = Convert.ToInt32(txtPort.Text);
                instance.SID = txtSid.Text;

                var globalDb = ConfigurationUtilities.GetDatabaseData(instance.InstanceName + "." + instance.DatabaseGlobalUser);
                instance.Password = globalDb.Password;
            }

            try
            {
                var publishResult = SpotfireServiceClient.PublishDBInstance(instance);

                if (!string.IsNullOrEmpty(publishResult))
                {
                    throw new Exception(publishResult);
                }

                CommandEventArgs commandEventArg = null;
                if (isNewPublishing)
                {
                    instance.Publish();
                    commandEventArg = new CommandEventArgs("InstanceAdded", string.Empty);
                }
                else
                {
                    instance.Update();
                    commandEventArg = new CommandEventArgs("InstanceUpdated", string.Empty);
                }

                // Remove the local cache of COEConfigurationBO
                COEConfigurationBO.RemoveLocalCache("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);

                if (this.RefreshParent != null)
                {
                    this.RefreshParent(sender, commandEventArg);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                errorMessage = errorMessage.Contains(Resources.Resource.DataSourceExistsError) ? Resources.Resource.DataSourceExistsError : errorMessage;
                errorMessage = errorMessage.Contains(Resources.Resource.DataSourceBusyError) ? Resources.Resource.DataSourceBusyMessage : errorMessage;
                this.validator.ErrorMessage = errorMessage;
                this.validator.IsValid = false;
            }
        }
    }
}

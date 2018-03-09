namespace Manager.Forms.DataViewManager.ContentArea
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Linq;
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.Utility;
    using CambridgeSoft.COE.Framework.GUIShell;
    using Manager.Code;
    using Utilities = Utilities;

    /// <summary>
    /// The manager database instance main page code
    /// </summary>
    public partial class ManageDbInstance : GUIShellPage
    {
        /// <summary>
        /// manager control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::Manager.Forms.DataViewManager.UserControls.ManageDbInstance manager;

        /// <summary>
        /// Gets Master
        /// </summary>
        /// <remarks>
        /// Auto-generated property.
        /// </remarks>
        public new Manager.Forms.Master.DataViewManager Master
        {
            get
            {
                return (Manager.Forms.Master.DataViewManager)base.Master;
            }
        }

        /// <summary>
        /// Gets the instances JSON format data 
        /// </summary>
        public string InstanceJson
        {
            get
            {
                COEInstanceBOList list = COEInstanceBOList.GetCOEInstanceBOList();
                List<DbInstance> instanceList = new List<DbInstance>();
                int i = 0;
                var orderList = (from x in list
                                 orderby x.InstanceName
                                 select x).ToList();

                foreach (var item in orderList)
                {
                    instanceList.Add(new DbInstance
                    {
                        ID = ++i,
                        Name = item.InstanceName,
                        TypeName = item.DbmsType.ToString(),
                        DriverName = COEConvert.PorcessDriverTypeForDisplay(item.DriverType.ToString()),
                        Username = item.DatabaseGlobalUser,
                        HostName = item.HostName,
                        Port = item.Port,
                        SID = item.SID
                    });
                }

                return new JavaScriptSerializer().Serialize(instanceList);
            }
        }

        /// <summary>
        /// Delete instance
        /// </summary>
        /// <param name="instanceName">The instance name</param>
        /// <returns>Error message string</returns>
        [WebMethod(EnableSession=true)]
        public static string DeleteInstance(string instanceName)
        {
            COEInstanceBOList list = COEInstanceBOList.GetCOEInstanceBOList();
            COEInstanceBO deleteInstance = list.GetInstance(instanceName);
            try
            {
                deleteInstance.Delete();
                SpotfireServiceClient.DeleteDBInstance(deleteInstance);

                // Clean Session variables.
                HttpContext.Current.Session.Remove(Constants.COEDataViewBO);
                // Remove the local cache of COEConfigurationBO
                COEConfigurationBO.RemoveLocalCache("CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            }
            catch (Csla.DataPortalException cslaEx)
            {
                if (cslaEx.BusinessException != null)
                {
                    return cslaEx.BusinessException.Message;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if the database instance exists or not.
        /// </summary>
        /// <param name="instanceName">The database instance name.</param>
        /// <returns>Return true if found, otherwise return false</returns>
        [WebMethod]
        public static bool IsDbInstanceExists(string instanceName)
        {
            COEInstanceBOList list = COEInstanceBOList.GetCOEInstanceBOList();
            COEInstanceBO instance = list.GetInstance(instanceName);

            return instance != null;
        }

        /// <summary>
        /// Publish instance to SPOTFIRE
        /// </summary>
        /// <param name="instanceName">The instance name</param>
        /// <returns>Error message string</returns>
        [WebMethod]
        public static string PublishInstance(string instanceName)
        {
            COEInstanceBOList list = COEInstanceBOList.GetCOEInstanceBOList();
            COEInstanceBO publishiInstance = list.GetInstance(instanceName);
            return SpotfireServiceClient.PublishDBInstance(publishiInstance);
        }

        /// <summary>
        /// Set page title
        /// </summary>
        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.SetPageTitle(Resources.Resource.ManageDbInstance_Page_Title);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        
        /// <summary>
        /// When page loading
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetControlsAttributtes();
            this.DoneImageButton.ButtonClicked += this.DoneImageButton_ButtonClicked;

            if (!Page.IsPostBack)
            {
                CambridgeSoft.COE.Framework.Common.InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
                hdMainDataSource.Value = mainInstance.Name;
            }
        }

        /// <summary>
        /// Event of Done Image Button Clicked
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        private void DoneImageButton_ButtonClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.GoHome();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Return to CBOE Home page
        /// </summary>
        private void GoHome()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(Constants.PublicContentAreaFolder + Resources.Resource.COE_HOME, false);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
    }

    /// <summary>
    /// The class of database instance
    /// </summary>
    public class DbInstance
    {
        /// <summary>
        /// Gets or sets the instance id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the instance name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the instance database type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the instance driver type name
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Gets or sets the instance data source
        /// </summary>
        public string Datasource { get; set; }

        /// <summary>
        /// Gets or sets the instance connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the instance user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the instance password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the instance host name
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the instance port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the instance's sid
        /// </summary>
        public string SID { get; set; }
    }

    /// <summary>
    /// The class of database type item
    /// </summary>
    public class DbType
    {
        /// <summary>
        /// Gets or sets the database type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database type value
        /// </summary>
        public int Value { get; set; }
    }

    public class DisplayDriverType
    {
        /// <summary>
        /// Gets or sets the driver type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the driver type value
        /// </summary>
        public string Value { get; set; }
    }
}

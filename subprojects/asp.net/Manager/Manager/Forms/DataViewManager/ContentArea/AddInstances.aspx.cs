namespace Manager.Forms.DataViewManager.ContentArea
{
    using System;
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using CambridgeSoft.COE.Framework.GUIShell;

    /// <summary>
    /// The Add/edit instance form page
    /// </summary>
    public partial class AddInstances : System.Web.UI.Page
    {
        /// <summary>
        /// The instance name
        /// </summary>
        private string instanceName = string.Empty;

        /// <summary>
        /// Call back for close pop window and fresh
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The command event args</param>
        internal void ClosePopWindowAndRefresh(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            // close this and refresh parent
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "redirect", "window.parent.location.href = 'ManageDbInstance.aspx';", true);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// call back for cancel pop window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The command event args</param>
        internal void CancelInstances(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "cancel", "window.parent.CloseModal(false);", true);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Process while page loading
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">the event args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SubscribeToUCEvents();
            if (!this.IsPostBack)
            {
                if (this.Page.Request != null)
                {
                    this.instanceName = this.Page.Request["InstanceName"];
                }

                if (string.IsNullOrEmpty(this.instanceName))
                {
                    this.DbManager.AddDbInstance();
                }
                else
                {
                    this.DbManager.EditDbInstance(this.instanceName);
                }
            }
        }

        /// <summary>
        /// Subscribe to user control event
        /// </summary>
        private void SubscribeToUCEvents()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (this.DbManager != null)
            {
                this.DbManager.RefreshParent += new CommandEventHandler(this.ClosePopWindowAndRefresh);
                this.DbManager.Cancel += new CommandEventHandler(this.CancelInstances);
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
    }
}
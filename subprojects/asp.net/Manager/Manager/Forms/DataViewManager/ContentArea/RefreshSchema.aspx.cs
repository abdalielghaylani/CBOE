using System;
using System.Reflection;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.GUIShell;

namespace Manager.Forms.DataViewManager.ContentArea
{
    public partial class RefreshSchema : GUIShellPage
    {
        #region Page life cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SubscribeToUCEvents();

            // CLear last error message.
            this.ErrorAreaUserControl.Text = string.Empty;
            this.ErrorAreaUserControl.Visible = false;
        }

        #endregion

        #region Event handlers

        private void RefreshSchemaUC_SchemaCancelRefresh(object sender, CommandEventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            //close current
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "cancel", "window.parent.CloseModal(false);", true);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void RefreshSchemaUserControl_ErrorOcurred(object sender, EventArgs e)
        {
            this.ErrorAreaUserControl.Text = this.RefreshSchemaUserControl.ErrorMessage;
            this.ErrorAreaUserControl.Visible = true;
        }
        
        #endregion

        #region Private Methods

        private void SubscribeToUCEvents()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (this.RefreshSchemaUserControl != null)
            {
                this.RefreshSchemaUserControl.Cancel += new CommandEventHandler(RefreshSchemaUC_SchemaCancelRefresh);
                this.RefreshSchemaUserControl.ErrorOcurred += new EventHandler<EventArgs>(RefreshSchemaUserControl_ErrorOcurred);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}

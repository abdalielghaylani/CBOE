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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

namespace Manager.Forms.Master
{
    public partial class DataViewManager : GUIShellMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal COEDataViewBO GetDataViewBO()
        {
            return this.Master.GetDataViewBO();
        }

        internal void DisplayMessagesPage(Constants.MessagesCode messagesCode, CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesButtonType messagesButtonType)
        {
            this.Master.DisplayMessagesPage(messagesCode, messagesButtonType);
        }

        internal void SetDataViewBO(CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO dataViewBO)
        {
            this.Master.SetDataViewBO(dataViewBO);
        }

        public override void SetPageTitle(string pageTitle)
        {
            this.HeaderUserControl.SetPageTitle(pageTitle);
        }

        internal void DisplayErrorMessage(string errorMessage)
        {
            this.Master.DisplayErrorMessage(errorMessage);
        }

        public override void DisplayErrorMessage(Exception exception, bool showBackLink)
        {
            this.Master.DisplayErrorMessage(exception, showBackLink);
        }
        
        public override void DisplayErrorMessage(string message, bool showBackLink)
        {
            this.Master.DisplayErrorMessage(message, showBackLink);
        }

        public override void DisplayConfirmationMessage(string message)
        {
            this.Master.DisplayConfirmationMessage(message);
        }

        public ValidCartridgeReadOnlyBO GetValidCartridgeBO()
        {
            return this.Master.GetValidCartridgeBO();
        }

        public void SetValidCartridgeBO(ValidCartridgeReadOnlyBO cartridgeBO)
        {
            this.Master.SetValidCartridgeBO(cartridgeBO);
        }

        public ValidCartridgeReadOnlyBO GetValidCartridgeBOEx()
        {
            var cartridgeBo = this.Master.GetValidCartridgeBO();

            // Create new object if it is not in session
            if (cartridgeBo == null)
            {
                cartridgeBo = ValidCartridgeReadOnlyBO.Get();
                SetValidCartridgeBO(cartridgeBo);
            }

            return cartridgeBo;
        }

        internal void ClearAllCurrentDVSessionVars()
        {
            this.Master.ClearAllCurrentDVSessionVars();
        }

        internal Control FindControlInPage(string controlID)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Control retVal = null;
            if(!string.IsNullOrEmpty(controlID))
                retVal = this.ContentPlaceHolder.FindControl(controlID);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return retVal;
        }
    }
}

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
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

namespace DocManagerWeb.Forms.ContentArea
{
    public partial class RecentActivities : GUIShellPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }
    }
}

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
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Resources;
using RegistrationWebApp.Forms.RegistrationAdmin.UserControls;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace RegistrationWebApp.Forms.RegistrationAdmin.ContentArea
{
    public partial class ConfigSettings : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Events
        protected override void OnInit(EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                #region Page Settings
                if (this.Master is RegistrationMaster)
                {
                    _masterPage = (RegistrationMaster)this.Master;
                    _masterPage.ShowLeftPanel = false;
                }
                #endregion
                base.OnInit(e);
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                COEConfigSettingManager.CurrentApplicationName = RegUtilities.GetApplicationName();
                LinkButtonBack.Style.Add("float", "right");
                LinkButtonBack.Style.Add("margin-bottom", "8px");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                // Fix for CSBR 123177
                // When the grid is not rendered on IE8 mode the grid lines are not shown correctly
                // MSDN related documentation: http://msdn.microsoft.com/en-us/library/cc288325%28VS.85%29.aspx
                HtmlMeta metatag = new HtmlMeta();
                metatag.Attributes.Add("http-equiv", "X-UA-Compatible");
                metatag.Attributes.Add("content", "ID=Edge");
                Page.Header.Controls.AddAt(0, metatag);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected override void OnPreInit(EventArgs e)
        {
            // It is necessary to set the Theme for the UltraWebGrid Skin to work inside the COEConfiguration server control
            this.Theme = base.StyleSheetTheme;
            base.OnPreInit(e);
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer(Resource.RegAdmin_URL);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        #endregion

        #region Methods

        protected override void SetControlsAttributtes()
        {

        }

        #endregion

    }
}

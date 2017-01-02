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
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace RegistrationWebApp.Forms.TableEditor.ContentArea
{
    public partial class TableEditor : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    this.SetControlsAttributtes();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void SetControlsAttributtes()
        {
            //this.PageTitleLabel.Text = Resource.TableEditor_Title_Text;
            this.LinkButtonGoHome.Text = Resource.Back_Button_Text;
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
                string buttonID = this.COETableManager1.GetDefaultButtonID();
                _masterPage.SetDefaultButton(buttonID == null ? this.LinkButtonGoHome.UniqueID : buttonID);
                CambridgeSoft.COE.Registration.Access.DalUtils.InvalidateCachedPicklists();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }
        protected void LinkButtonGoHome_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string url = "../../RegistrationAdmin/ContentArea/Default.aspx";
            Server.Transfer(url, false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}

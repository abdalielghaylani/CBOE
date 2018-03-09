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

namespace Manager.Forms.ConfigControlSettings
{
    public partial class ConfigControlSettings : GUIShellPage
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreInit(EventArgs e)
        {
            // It is necessary to set the Theme for the UltraWebGrid Skin to work inside the COEConfiguration server control
            this.Theme = base.StyleSheetTheme;
            base.OnPreInit(e);
        }

        #endregion

        #region Methods

        protected override void SetControlsAttributtes()
        {
           
        }

        #endregion

    }
}

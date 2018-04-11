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
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace RegistrationWebApp.Forms.RegistrationAdmin.UserControls
{
    public partial class ConfigurationSettingsUC : System.Web.UI.UserControl
    {

        #region Variables

        private SettingsGroup _webGridDataSource;

        #endregion

        #region Properties

        public SettingsGroup Ds
        {
            get { return _webGridDataSource; }
            set { _webGridDataSource = value; }
        }

        public Infragistics.WebUI.UltraWebGrid.UltraWebGrid UltraWebGrid
        {
            get { return this.UltraWebGridSettings; }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods

        public override void DataBind()
        {
            try
            {
                this.BindWebGrid();
                base.DataBind();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
            }
        }

        private void BindWebGrid()
        {
            DataTable settingsDT = new DataTable(this.Ds.Name);
            DataColumn nameDC = new DataColumn("Name");
            DataColumn valueDC = new DataColumn("Value");
            DataColumn descriptionDC = new DataColumn("Description");
            settingsDT.Columns.Add(nameDC);
            settingsDT.Columns.Add(valueDC);
            settingsDT.Columns.Add(descriptionDC);

            foreach (AppSetting appSetting in this.Ds.Settings)
            {
                DataRow row = settingsDT.NewRow();
                row[nameDC] = appSetting.Name;
                row[valueDC] = appSetting.Value;
                row[descriptionDC] = appSetting.Description;
                settingsDT.Rows.Add(row);
            }

            this.UltraWebGrid.DataSource = settingsDT;
            this.UltraWebGrid.DataBind();
        }

        #endregion
    }
}
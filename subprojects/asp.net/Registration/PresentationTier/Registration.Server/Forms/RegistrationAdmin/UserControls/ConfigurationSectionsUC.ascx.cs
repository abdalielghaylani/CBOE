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
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RegistrationWebApp.Forms.RegistrationAdmin.UserControls
{
    public partial class ConfigurationSectionsUC : System.Web.UI.UserControl
    {
        #region Properties

        public string SelectedTab
        {
            get { return ViewState["SelectedTab"] != null ? ViewState["SelectedTab"].ToString() : string.Empty; }
            set { ViewState["SelectedTab"] = value; }
        }

        private AppSettingsData DS
        {
            get { return Session["AppSettingsData"] != null ? (AppSettingsData)Session["AppSettingsData"] : null; }
            set { Session["AppSettingsData"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }  

        /// <summary>
        /// Bind and show information into the screen.
        /// </summary>
        /// <param name="data">Datasource with the data</param>
        /// <param name="selectedTabName"></param>
        public void DataBind(AppSettingsData data, string selectedTabName)
        {
            if (data != null)
            {
                if (data.SettingsGroup.Count > 0)
                {
                    this.SelectedTab = !string.IsNullOrEmpty(selectedTabName) ? selectedTabName : data.SettingsGroup.Get(0).Name;
                    this.DS = data;
                    foreach (SettingsGroup settingGroup in data.SettingsGroup)
                    {
                        this.AppSettingsUltraWebTab.Tabs.FromKey(settingGroup.Name + "TabItem").Text = settingGroup.Title;
                        Infragistics.WebUI.UltraWebGrid.UltraWebGrid grid = (Infragistics.WebUI.UltraWebGrid.UltraWebGrid)this.AppSettingsUltraWebTab.Tabs.FromKeyTab(settingGroup.Name + "TabItem").FindControl(settingGroup.Name + "UltraWebGrid");
                        grid.DataSource = this.CreateSettingsGroupDataTable(settingGroup);                        
                        grid.DataBind();
                        this.SetGridDisplayLayout(ref grid,settingGroup);                        
                    }
                }
            }
        }

        private DataTable CreateSettingsGroupDataTable(SettingsGroup settingsGroup)
        {
            DataTable settingsDT = new DataTable(settingsGroup.Name);

            DataColumn nameDC = new DataColumn("Name");
            DataColumn valueDC = new DataColumn("Value");
            DataColumn descriptionDC = new DataColumn("Description");
            settingsDT.Columns.Add(nameDC);
            settingsDT.Columns.Add(valueDC);
            settingsDT.Columns.Add(descriptionDC);

            foreach (AppSetting appSetting in settingsGroup.Settings)
            {
                DataRow row = settingsDT.NewRow();
                row[nameDC] = appSetting.Name;
                row[valueDC] = appSetting.Value;
                row[descriptionDC] = appSetting.Description;
                settingsDT.Rows.Add(row);
            }

            return settingsDT;
        }

        /// <summary>
        /// Unbind data to save it later
        /// </summary>
        /// <returns>The App info with the latest changes</returns>
        public AppSettingsData UnBind()
        {
            this.SaveChanges();
            return this.DS;
        }

        private void SaveChanges()
        {            
            foreach (SettingsGroup settingGroup in this.DS.SettingsGroup)
            {
                Infragistics.WebUI.UltraWebGrid.UltraWebGrid grid = (Infragistics.WebUI.UltraWebGrid.UltraWebGrid)this.AppSettingsUltraWebTab.Tabs.FromKeyTab(settingGroup.Name + "TabItem").FindControl(settingGroup.Name + "UltraWebGrid");
                if (grid != null)
                {
                    foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in grid.Rows)
                    {
                        if (row.Cells[0].Text != null && row.Cells[1].Text != null)
                        {
                            if (settingGroup.Settings.Get(row.Cells[0].Text) != null)
                                settingGroup.Settings.Get(row.Cells[0].Text).Value = row.Cells[1].Text;
                            else
                            {
                                AppSetting newSetting = new AppSetting();
                                newSetting.Name = row.Cells[0].Text;
                                newSetting.Value = row.Cells[1].Text;
                                if (row.Cells[2].Text != null)
                                    newSetting.Description = row.Cells[2].Text;
                                else
                                    newSetting.Description = "No description for this item";
                                settingGroup.Settings.Add(newSetting);
                            }
                        }
                    }
                }
            }

        }

        private void SetGridDisplayLayout(ref Infragistics.WebUI.UltraWebGrid.UltraWebGrid grid, SettingsGroup settingGroup) 
        {
            grid.DisplayLayout.GroupByBox.Prompt = settingGroup.Description;
            grid.DisplayLayout.AllowUpdateDefault = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;
            grid.DisplayLayout.AllowAddNewDefault = Infragistics.WebUI.UltraWebGrid.AllowAddNew.Yes;
            grid.DisplayLayout.AllowDeleteDefault = Infragistics.WebUI.UltraWebGrid.AllowDelete.Yes;
            grid.Rows[0].Cells[0].Column.Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;
            
        }
    }
}
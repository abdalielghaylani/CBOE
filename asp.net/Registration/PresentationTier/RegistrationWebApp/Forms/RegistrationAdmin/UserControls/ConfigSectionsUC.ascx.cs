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

namespace RegistrationWebApp.Forms.RegistrationAdmin.UserControls
{
    public partial class ConfigSectionsUC : System.Web.UI.UserControl
    {
        #region Variables

        private readonly string TITLEID = "ConfigSettingTitle";
        private readonly string TXTBOXID = "ConfigSettingTextBox";
        private readonly string DESCRIPTIONID = "ConfigSettingDescription";
        private readonly string SectionsDivId = "SectionsList";
        private readonly string GROUPIDKEY = "groupId";
        private readonly string SETTINGIDKEY = "settingId";
        private readonly string SectionLinkButton = "SectionLinkButton";

        #endregion

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

        #region Control events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            this.SettingsRepeater.ItemDataBound += new RepeaterItemEventHandler(SettingsRepeater_ItemDataBound);
            this.AppConfigSectionRepeater.ItemDataBound += new RepeaterItemEventHandler(AppConfigSectionRepeater_ItemDataBound);
            this.AppConfigSectionRepeater.ItemCommand += new RepeaterCommandEventHandler(AppConfigSectionRepeater_ItemCommand);
            base.OnInit(e);            
        }

        void AppConfigSectionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            this.SaveChanges();
            if (!string.IsNullOrEmpty(e.CommandName) && e.CommandSource is LinkButton)
                this.DataBind(this.DS, ((LinkButton)e.CommandSource).Attributes[GROUPIDKEY].ToString());
        }

        void SettingsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.FindControl(TITLEID) != null)
                ((Label)e.Item.FindControl(TITLEID)).Text = ((AppSetting)e.Item.DataItem).Name;
            if (e.Item.FindControl(TITLEID) != null)
            {
                if (((AppSetting)e.Item.DataItem).Description != string.Empty)
                    ((Label)e.Item.FindControl(DESCRIPTIONID)).Text = ((AppSetting)e.Item.DataItem).Description;
                else
                    ((Label)e.Item.FindControl(DESCRIPTIONID)).Text = Resources.Resource.NoDescription_Label_Text;
            }

            if (e.Item.FindControl(TXTBOXID) != null)
            {
                ((TextBox)e.Item.FindControl(TXTBOXID)).Text = ((AppSetting)e.Item.DataItem).Value;
                ((TextBox)e.Item.FindControl(TXTBOXID)).Attributes.Add(GROUPIDKEY, this.SelectedTab);
                ((TextBox)e.Item.FindControl(TXTBOXID)).Attributes.Add(SETTINGIDKEY, ((AppSetting)e.Item.DataItem).Name);
            }
        }

        void AppConfigSectionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.FindControl(SectionLinkButton) != null)
            {
                ((LinkButton)e.Item.FindControl(SectionLinkButton)).Text = ((SettingsGroup)e.Item.DataItem).Name;
                ((LinkButton)e.Item.FindControl(SectionLinkButton)).ToolTip = ((SettingsGroup)e.Item.DataItem).Description;
                ((LinkButton)e.Item.FindControl(SectionLinkButton)).Attributes.Add(GROUPIDKEY, ((SettingsGroup)e.Item.DataItem).Name);

                //if (e.Item.FindControl(SectionsDivId) != null)
                    //((HtmlGenericControl)e.Item.FindControl(SectionsDivId)).Attributes.Add("class", e.Item.ItemIndex == this.SelectedTab ? "SelectedTabsHeader" : "LiTabsHeader");
            }
        }

        #endregion

        #region Methods

        
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
                    this.AppConfigSectionRepeater.DataSource = data.SettingsGroup;
                    this.SettingsRepeater.DataSource = data.SettingsGroup.Get(this.SelectedTab).Settings;
                    this.SectionTitleLabel.Text = data.SettingsGroup.Get(this.SelectedTab).Title;
                    this.SectionDescriptionLabel.Text = data.SettingsGroup.Get(this.SelectedTab).Description;
                    this.AppConfigSectionRepeater.DataBind();
                    this.SettingsRepeater.DataBind();
                }
            }
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

        /// <summary>
        /// Save chanegs of the current page
        /// </summary>
        private void SaveChanges()
        {
            //Loop all the controls 
            foreach (RepeaterItem item in this.SettingsRepeater.Items)
            {
                SettingsGroup group = this.DS.SettingsGroup.Get(((TextBox)item.FindControl(TXTBOXID)).Attributes[GROUPIDKEY]);
                if (group != null)
                {
                    AppSetting setting = group.Settings.Get(((TextBox)item.FindControl(TXTBOXID)).Attributes[SETTINGIDKEY]);
                    if (setting != null)
                    {
                        string value = ((TextBox)item.FindControl(TXTBOXID)).Text;
                        if (!setting.Value.Equals(value))
                            setting.Value = value;
                    }
                }
            }
        }

        #endregion
    }
}
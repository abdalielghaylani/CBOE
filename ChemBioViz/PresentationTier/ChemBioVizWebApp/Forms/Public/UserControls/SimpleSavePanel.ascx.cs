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
using System.Collections.Generic;
using Resources;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls
{
    public partial class SimpleSavePanel : System.Web.UI.UserControl
    {
        #region Event Handlers
        public delegate void SimpleSavePanelEventHandler(object sender, SimpleSavePanelEventArgs eventArgs);
        public event SimpleSavePanelEventHandler Save = null;
        #endregion

        #region Properties
        public string PanelClientID
        {
            get { return this.SimpleSavePanelYUIPanel.ClientID; }
        }

        internal string CommandName
        {
            get { return this.SaveLinkButton.CommandName; }
            set { this.SaveLinkButton.CommandName = value; }
        }
        #endregion

        #region Lyfe Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.SetControlsAttributes();
            }
            this.CancelLinkButton.Attributes.Add("onclick", "SetSimpleSavePanelVisibility('" + SimpleSavePanelYUIPanel.ClientID + "', false); return false;");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string showjs = @"
                            var currentSimplePanel = '';
                            function SetSimpleSavePanelVisibility(panelId, visible)
                            {
                                var panel = document.getElementById(panelId);
                                HidePreviousPanel();
                                if(visible == true) {
                                    panel.style.display = 'block';
                                    HideChemDraws();
                                    currentSimplePanel = panelId;
                                }
                                else {
                                    panel.style.display = 'none';
                                    ShowChemDraws();
                                    currentSimplePanel = '';
                                }
                            }

                            function HidePreviousPanel()
                            {
                                if(currentSimplePanel != '')
                                {
                                    document.getElementById(currentSimplePanel).style.display = 'none';
                                }
                            }";
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(SimpleSavePanel), "SetSimpleSavePanelVisibility"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(SimpleSavePanel), "SetSimpleSavePanelVisibility", showjs, true);
            }
            
        }
        #endregion

        #region Event Handlers
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if(Save != null)
            {
                SimpleSavePanelEventArgs args = new SimpleSavePanelEventArgs(this.NameTextBox.Text, this.DescriptionTextBox.Text, this.IsPublicCheckBox.Checked);
                Save(sender, args);
            }
        }

        #endregion

        #region Private Methods
        private void SetControlsAttributes()
        {
            this.NameLabel.Text = Resource.Name_Label_Text;
            this.DescriptionLabel.Text = Resource.Description_Label_Text;
            this.IsPublicCheckBox.Text = Resource.IsPublic_Label_Text;
            this.SaveLinkButton.Text = Resource.Save_Button_Text;
            this.CancelLinkButton.Text = Resource.Cancel_Button_Text;
            this.NameRequiredField.Text = Resource.NameRequired_Error_Text;
            this.DescriptionRequiredField.Text = Resource.DescriptionRequired_Error_Text;
            this.NameRequiredField.ValidationGroup = this.ClientID;
            this.DescriptionRequiredField.ValidationGroup = this.ClientID;
            this.SaveLinkButton.ValidationGroup = this.ClientID;
        }

        //public void DataBind(int id, string name, string description)
        //{
        //    this.IDTextBox.Text = id.ToString();
        //    this.NameTextBox.Text = name;
        //    this.DescriptionTextBox.Text = description;
        //}
        #endregion
    }
    public class SimpleSavePanelEventArgs : EventArgs
    {
        public string Name;
        public string Description;
        public bool IsPublic;

        public SimpleSavePanelEventArgs(string name, string description, bool isPublic)
        {
            Name = name;
            Description = description;
            IsPublic = isPublic;
        }
    }
}
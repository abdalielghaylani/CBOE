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
    public partial class RecPanel : System.Web.UI.UserControl
    {
        #region Event Handlers
        public delegate void RecPanelEventHandler(object sender, RecPanelEventArgs eventArgs);
        public event RecPanelEventHandler Save;
        #endregion

        #region Properties
        public string PanelClientID
        {
            get { return this.RecPanelYUIPanel.ClientID; }
        }

        internal string CommandName
        {
            get { return this.OKLinkRecButton.CommandName; }
            set { this.OKLinkRecButton.CommandName = value; }
        }
        #endregion

        #region Lyfe Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //this.Page.Form.DefaultFocus = this.EnterRecNoText.ClientID;
                this.SetControlsAttributes();
            }

            this.CancelLinkRecButton.Attributes.Add("onclick", "SetRecPanelVisibility('" + RecPanelYUIPanel.ClientID + "', false); return false;");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string showjs = @"
                            var currentSimplePanel = '';
                            function SetRecPanelVisibility(panelId, visible)
                            {
                                var panel = document.getElementById(panelId);
                                HidePreviousPanel();
                                if(visible == true) {
                                    panel.style.display = 'block';
                                    HideChemDraws();
                                    currentSimplePanel = panelId;
                                    document.getElementById('" + this.EnterRecNoText.ClientID + @"').select();
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
            if (!Page.ClientScript.IsStartupScriptRegistered(typeof(RecPanel), "SetRecPanelVisibility"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(RecPanel), "SetRecPanelVisibility", showjs, true);
            }


        }
        #endregion

        #region Event Handlers
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save != null)
            {
                string recordNumber;
                if (this.EnterRecNoText.Text != null && IsInteger(this.EnterRecNoText.Text))
                    recordNumber = this.EnterRecNoText.Text;
                else
                    recordNumber = "0";

                RecPanelEventArgs args = new RecPanelEventArgs(recordNumber);
                Save(sender, args);
            }
        }
        public bool IsInteger(string data)
        {
            bool result = true;
            try
            {
                int.Parse(data);
            }
            catch (FormatException)
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region Private Methods
        private void SetControlsAttributes()
        {
            this.OKLinkRecButton.ValidationGroup = this.ClientID;
        }
        #endregion
    }


    public class RecPanelEventArgs : EventArgs
    {
        public int RecNo;
        public RecPanelEventArgs(string Recnumber)
        {
            Int32.TryParse(Recnumber, out RecNo);
        }
    }


}
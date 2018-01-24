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
using System.Collections.Generic;
using Resources;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls {
    public partial class ConfirmationPanel : System.Web.UI.UserControl {
        public delegate void HitListUnaryOperationHandler(object sender, HitListUnaryOperationEventArgs eventArgs);
        public event HitListUnaryOperationHandler ActionPerformed = null;

        public List<string> ShowingControlsID {
            get {
                if(ViewState["ShowingControlsID"] == null)
                    ViewState["ShowingControlsID"] = new List<string>();

                return (List<string>) ViewState["ShowingControlsID"];
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            this.ConfirmationYUIPanel.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            SubscribeToControlsEvents();
            RegisterStartUpScripts();

        }

        protected override void Render(HtmlTextWriter writer) {
            base.Render(writer);
            string jsInit = @"
        YAHOO.namespace('ConfirmationPanelUC');
        function initConfirmationPanelUC() {
            YAHOO.ConfirmationPanelUC.confirmationPanel = new YAHOO.widget.Panel('" + this.ConfirmationYUIPanel.ClientID + @"', { width:'500px', close:true, modal:true, draggable:true, fixedcenter:true, visible:false, constraintoviewport:true } );
            YAHOO.ConfirmationPanelUC.confirmationPanel.render(document.body.form);

            YAHOO.ConfirmationPanelUC.confirmationPanel.beforeHideEvent.subscribe(ShowChemDraws);
            YAHOO.ConfirmationPanelUC.confirmationPanel.beforeShowEvent.subscribe(HideChemDraws);";
            foreach(string showingControlId in this.ShowingControlsID) {
                jsInit += @"YAHOO.util.Event.addListener('" + showingControlId + @"', 'click', YAHOO.ConfirmationPanelUC.confirmationPanel.show, YAHOO.ConfirmationPanelUC.confirmationPanel, true);";
            }
            jsInit += @"
        }
        YAHOO.util.Event.addListener(window, 'load', initConfirmationPanelUC);";

            writer.Write("<script language='javascript'>" + jsInit + "</script>");
        }

        void OKButton_Click(object sender, EventArgs e) {
            PerformAction();
            if(ActionPerformed != null) {
                HitListUnaryOperationHandler currentEventRaised = ActionPerformed; // Copy to a temporary variable to be thread-safe.
                currentEventRaised(this, new HitListUnaryOperationEventArgs(int.Parse(this.HitlistIDHiddenField.Value), (HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeHiddenField.Value), this.ActionHiddenField.Value.ToUpper()));
            }
        }

        private void RegisterStartUpScripts() {
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(ConfirmationPanel), "dataBind")) {
                string dataBindJs = @"
                function dataBindConfirmationPanel(hitlistId, hitlistType, action) {
                    document.getElementById('" + this.ActionHiddenField.ClientID + @"').value = action;
                    document.getElementById('" + this.HitlistIDHiddenField.ClientID + @"').value = hitlistId;
                    document.getElementById('" + this.HitListTypeHiddenField.ClientID + @"').value = hitlistType;
                    if(action.toUpperCase() == 'RESTORE') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = '" + Resource.RestoreConfirmation_Label_Title + @"';
                        document.getElementById('" + this.ConfirmationLabel.ClientID + @"').innerText = '" + Resource.RestoreConfirmation_Label_Text + @"';
                    }
                    else if(action.toUpperCase() == 'DELETE') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = '" + Resource.DeleteConfirmation_Label_Title + @"';
                        document.getElementById('" + this.ConfirmationLabel.ClientID + @"').innerText = '" + Resource.DeleteConfirmation_Label_Text + @"';
                    }

                    document.getElementById('" + this.header.ClientID + @"').innerText += ' ' + hitlistId;

                    document.getElementById('" + this.OKButton.ClientID + @"').value = '" + Resource.OK_Button_Text + @"';
                }";
                Page.ClientScript.RegisterStartupScript(typeof(ConfirmationPanel), "dataBind", dataBindJs, true);
            }
        }

        private void SubscribeToControlsEvents() {
            this.OKButton.Click += new EventHandler(OKButton_Click);
        }

        private void PerformAction() {
            switch(this.ActionHiddenField.Value) {
                case "RESTORE":
                    COEHitListBO hitListBO = COEHitListBO.Get((HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeHiddenField.Value), int.Parse(this.HitlistIDHiddenField.Value));
                    break;
                case "DELETE":
                    COEHitListBO.Delete((HitListType)Enum.Parse(typeof(HitListType), this.HitListTypeHiddenField.Value), int.Parse(this.HitlistIDHiddenField.Value));
                    break;  
            }
        }
    }
}
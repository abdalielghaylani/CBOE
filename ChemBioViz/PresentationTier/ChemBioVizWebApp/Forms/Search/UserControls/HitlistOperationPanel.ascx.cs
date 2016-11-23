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
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.ContentArea;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls {
    public partial class HitlistOperationPanel : System.Web.UI.UserControl {
        public delegate void HitListBinaryOperationHandler(object sender, HitListBinaryOperationEventArgs eventArgs);
        public event HitListBinaryOperationHandler ActionPerformed = null;

        public List<string> ShowingControlsID {
            get {
                if(ViewState["ShowingControlsID"] == null)
                    ViewState["ShowingControlsID"] = new List<string>();

                return (List<string>) ViewState["ShowingControlsID"];
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            this.HitListOperationYUIPanel.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            SubscribeToControlsEvents();
            RegisterStartUpScripts();

        }

        protected override void Render(HtmlTextWriter writer) {
            base.Render(writer);
            string jsInit = @"
        YAHOO.namespace('HitlistOperationPanelUC');
        function initHitlistOperationPanelUC() {
            YAHOO.HitlistOperationPanelUC.hitlistOperationPanel = new YAHOO.widget.Panel('" + this.HitListOperationYUIPanel.ClientID + @"', { width:'500px', close:true, modal:true, draggable:true, fixedcenter:true, visible:false, constraintoviewport:true } );
            YAHOO.HitlistOperationPanelUC.hitlistOperationPanel.render(document.body.form);

            YAHOO.HitlistOperationPanelUC.hitlistOperationPanel.beforeHideEvent.subscribe(hideExchangeButton);
            YAHOO.HitlistOperationPanelUC.hitlistOperationPanel.beforeHideEvent.subscribe(ShowChemDraws);
            YAHOO.HitlistOperationPanelUC.hitlistOperationPanel.beforeShowEvent.subscribe(HideChemDraws);";
            foreach(string showingControlId in this.ShowingControlsID) {
                jsInit += @"YAHOO.util.Event.addListener('" + showingControlId + @"', 'click', YAHOO.HitlistOperationPanelUC.hitlistOperationPanel.show, YAHOO.HitlistOperationPanelUC.hitlistOperationPanel, true);";
            }
            jsInit += @"
        }
        YAHOO.util.Event.addListener(window, 'load', initHitlistOperationPanelUC);";

            writer.Write("<script language='javascript'>" + jsInit + "</script>");
        }

        void OKButton_Click(object sender, EventArgs e) {
            
            if(ActionPerformed != null) {
                HitListBinaryOperationHandler currentEventRaised = ActionPerformed; // Copy to a temporary variable to be thread-safe.
                currentEventRaised(this, new HitListBinaryOperationEventArgs(
                                                int.Parse(this.HitlistIDAHiddenField.Value),
                                                (HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeAHiddenField.Value),
                                                int.Parse(this.HitlistIDBHiddenField.Value),
                                                (HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeBHiddenField.Value),
                                                this.ActionHiddenField.Value.ToUpper(),
                                                PerformAction()));
            }
        }

        private void RegisterStartUpScripts() {
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(HitlistOperationPanel), "dataBind")) {
                string dataBindJs = @"
                function dataBindHitListOperationPanel(hitlistIdOperandA, hitlistTypeOperandA, hitlistIdOperandB, hitlistTypeOperandB, action, database) {
                    document.getElementById('" + this.ActionHiddenField.ClientID + @"').value = action;
                    document.getElementById('" + this.DatabaseHiddenField.ClientID + @"').value = database;
                    document.getElementById('" + this.HitlistIDAHiddenField.ClientID + @"').value = hitlistIdOperandA;
                    document.getElementById('" + this.HitListTypeAHiddenField.ClientID + @"').value = hitlistTypeOperandA;
                    document.getElementById('" + this.HitlistIDBHiddenField.ClientID + @"').value = hitlistIdOperandB;
                    document.getElementById('" + this.HitListTypeBHiddenField.ClientID + @"').value = hitlistTypeOperandB;
                    document.getElementById('" + this.HitListOperandALabel.ClientID + @"').innerText = '" + Resource.HitListID_Label_Text + @": ' + hitlistIdOperandA;
                    document.getElementById('" + this.HitListOperandBLabel.ClientID + @"').innerText = '" + Resource.HitListID_Label_Text + @": ' + hitlistIdOperandB;

                    if(action.toUpperCase() == 'UNION') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = '" + Resource.OperationConfirmation_Label_Title + @"';
                        document.getElementById('" + this.HitListOperationLabel.ClientID + @"').innerText = ' " + Resource.Union_Button_Text + @" ';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.visibility = 'hidden';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.display = 'none';
                    }
                    else if(action.toUpperCase() == 'INTERSECT') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = '" + Resource.OperationConfirmation_Label_Title + @"';
                        document.getElementById('" + this.HitListOperationLabel.ClientID + @"').innerText = ' " + Resource.Intersect_Button_Text + @" ';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.visibility = 'hidden';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.display = 'none';
                    }
                    else if(action.toUpperCase() == 'SUBTRACT') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = '" + Resource.OperationConfirmation_Label_Title + @"';
                        document.getElementById('" + this.HitListOperationLabel.ClientID + @"').innerText = ' " + Resource.Subtract_Button_Text + @" ';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.visibility = 'visible';
                        document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.display = 'inline';
                    }

                    document.getElementById('" + this.OKButton.ClientID + @"').value = '" + Resource.OK_Button_Text + @"';
                    document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').value = '" + Resource.ExchangeOperands_Button_Text + @"';
                }

                function hideExchangeButton(){
                    document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.visibility = 'hidden';
                    document.getElementById('" + this.ExchangeOperandsButton.ClientID + @"').style.display = 'none';
                }

                function exchangeOperands() {
                    dataBindHitListOperationPanel(document.getElementById('" + this.HitlistIDBHiddenField.ClientID + @"').value, document.getElementById('" + this.HitListTypeBHiddenField.ClientID + @"').value, document.getElementById('" + this.HitlistIDAHiddenField.ClientID + @"').value, document.getElementById('" + this.HitListTypeAHiddenField.ClientID + @"').value, document.getElementById('" + this.ActionHiddenField.ClientID + @"').value);
                }";
                Page.ClientScript.RegisterStartupScript(typeof(HitlistOperationPanel), "dataBind", dataBindJs, true);
            }
        }

        private void SubscribeToControlsEvents() {
            this.ExchangeOperandsButton.Attributes.Add("onclick", "exchangeOperands()");
            this.OKButton.Click += new EventHandler(OKButton_Click);
        }
        
        private int PerformAction() {
            HitListInfo operandA = new HitListInfo();
            operandA.HitListID = int.Parse(this.HitlistIDAHiddenField.Value);
            operandA.HitListType = (HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeAHiddenField.Value);
            operandA.Database = this.DatabaseHiddenField.Value;
            HitListInfo operandB = new HitListInfo();
            operandB.HitListID = int.Parse(this.HitlistIDBHiddenField.Value);
            operandB.HitListType = (HitListType) Enum.Parse(typeof(HitListType), this.HitListTypeBHiddenField.Value);
            operandB.Database = this.DatabaseHiddenField.Value;

            switch(this.ActionHiddenField.Value) {
                case "UNION":
                    return COEHitListOperationManager.UnionHitLists(operandA, operandB, ((ManageHitList) this.Page).BusinessObject.DataView.DataViewID).ID;
                case "SUBTRACT":
                    return COEHitListOperationManager.SubtractHitLists(operandA, operandB, ((ManageHitList) this.Page).BusinessObject.DataView.DataViewID).ID;
                case "INTERSECT":
                    return COEHitListOperationManager.IntersectHitList(operandA, operandB, ((ManageHitList) this.Page).BusinessObject.DataView.DataViewID).ID;
            }

            return -1;
        }
    }
    
    public class HitListBinaryOperationEventArgs : EventArgs {
        public int HitListIDA;
        public HitListType HitListTypeA;
        public int HitListIDB;
        public HitListType HitListTypeB;
        public int NewHitListID;
        public string Operation;

        public HitListBinaryOperationEventArgs(int hitListIDA, HitListType hitListTypeA, int hitListIDB, HitListType hitListTypeB, string operation, int newHitListID) {
            this.HitListIDA = hitListIDA;
            this.HitListTypeA = hitListTypeA;
            this.HitListIDB = hitListIDB;
            this.HitListTypeB = hitListTypeB;
            this.Operation = operation;
            this.NewHitListID = newHitListID;
        }
    }
}

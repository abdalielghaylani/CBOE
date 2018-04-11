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
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using Resources;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls {
    public partial class SavePanel : System.Web.UI.UserControl {
        #region Events
        public delegate void HitListUnaryOperationHandler(object sender, HitListUnaryOperationEventArgs eventArgs);
        public event HitListUnaryOperationHandler Saved = null;
        //public delegate SavedEventHandler 
        #endregion

        #region Properties
        private COEHitListBO HitListBO {
            get {
                return (COEHitListBO) Session["HitListBO"];
            }
            set {
                Session["HitListBO"] = value;
            }
        }

        private COESearchCriteriaBO SearchCriteriaBO {
            get {
                return (COESearchCriteriaBO) Session["SearchCriteriaBO"];
            }
            set {
                Session["SearchCriteriaBO"] = value;
            }
        }

        public List<string> ShowingControlsID {
            get {
                if(ViewState["ShowingControlsID"] == null)
                    ViewState["ShowingControlsID"] = new List<string>();

                return (List<string>) ViewState["ShowingControlsID"];
            }
        }
        #endregion

        #region Variables
        private bool _dataBound = false;
        #endregion

        #region Page Life Cycle Events
        protected void Page_Load(object sender, EventArgs e) {
            this.SavePanelYUIPanel.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            if(!Page.IsPostBack) {
                SetControlsAttributes();
            }
            if(!Page.ClientScript.IsStartupScriptRegistered("dataBind")) {
                string dataBindJs = @"
                //id-name-description-databasename-user-datecreated-numhits
                function dataBind(id,name,description,databasename,user,datecreated,numhits,hitlisttype) {
                    //document.getElementById('" + this.SavePanelYUIPanel.ClientID + @"').style.visibility = 'visible';
                    fillLabels('hitlist', hitlisttype);
                    document.getElementById('" + this.IDTextBox.ClientID + @"').value = id;
                    document.getElementById('" + this.TypeTextBox.ClientID + @"').value = hitlisttype;
                    document.getElementById('" + this.NameTextBox.ClientID + @"').value = name;
                    document.getElementById('" + this.DescriptionTextBox.ClientID + @"').value = description;
                    document.getElementById('" + this.DatabaseTextBox.ClientID + @"').value = databasename;
                    document.getElementById('" + this.UserTextBox.ClientID + @"').value = user;
                    document.getElementById('" + this.DateCreatedTextBox.ClientID + @"').value = datecreated;
                    document.getElementById('" + this.NumHitsTextBox.ClientID + @"').value = numhits;
                    //YAHOO.SavePanelUC.savePanel.show();
                }

                function fillLabels(savetype, hitlisttype) {
                    if(savetype == 'hitlist') {
                        document.getElementById('" + this.header.ClientID + @"').innerText = 'Save Hitlist';
                        document.getElementById('" + this.IDLabel.ClientID + @"').innerText = '" + Resource.HitListID_Label_Text + @"';
                        document.getElementById('" + this.TypeLabel.ClientID + @"').innerText = '" + Resource.HitListType_Label_Text + @"';
                        if(hitlisttype.toUpperCase() != 'TEMP')
                            document.getElementById('" + this.SaveButton.ClientID + @"').value = 'Edit';
                    }
                }
                ";
                Page.ClientScript.RegisterStartupScript(typeof(SavePanel), "dataBind", dataBindJs, true);
            }
        }

        protected override void Render(HtmlTextWriter writer) {
            base.Render(writer);
            string jsInit = @"
        YAHOO.namespace('SavePanelUC');
        function initSavePanelUC() {
            YAHOO.SavePanelUC.savePanel = new YAHOO.widget.Panel('" + this.SavePanelYUIPanel.ClientID + @"', { width:'300px', close:true, modal:true, draggable:true, fixedcenter:true, visible:" + _dataBound.ToString().ToLower() + @", constraintoviewport:true } );
            YAHOO.SavePanelUC.savePanel.render(document.body.form);

            YAHOO.SavePanelUC.savePanel.beforeHideEvent.subscribe(ShowChemDraws);";
            foreach(string showingControlId in this.ShowingControlsID) {
                jsInit += @"YAHOO.util.Event.addListener('" + showingControlId + @"', 'click', YAHOO.SavePanelUC.savePanel.show, YAHOO.SavePanelUC.savePanel, true);";
            }
        jsInit += @"
        }
        YAHOO.util.Event.addListener(window, 'load', initSavePanelUC);";
            if(_dataBound)
                jsInit += @"
        HideChemDraws();";
            
            writer.Write("<script language='javascript'>" + jsInit + "</script>");
        }
        #endregion

        #region Controls Events
        protected void SaveButton_Click(object sender, EventArgs e) {
            this.Save();
        }
        #endregion

        #region Private Methods
        private void SetControlsAttributes() {
            this.NameLabel.Text = Resource.Name_Label_Text;
            this.DatabaseLabel.Text = Resource.Database_Label_Text;
            this.UserLabel.Text = Resource.User_Label_Text;
            this.DescriptionLabel.Text = Resource.Description_Label_Text;
            this.footer.InnerText = Resource.Footer_SavePanel_Text;
            this.SaveButton.Text = Resource.Save_Button_Text;
            this.NumHitsLabel.Text = Resource.NumberOfHits_Label_Text;
            this.DateCreatedLabel.Text = Resource.DateCreated_Label_Text;


            this.DateCreatedTextBox.Attributes.Add("readonly", "readonly");
            this.IDTextBox.Attributes.Add("readonly", "readonly");
            this.UserTextBox.Attributes.Add("readonly", "readonly");
            this.TypeTextBox.Attributes.Add("readonly", "readonly");
        }

        public void DataBind(COEHitListBO hitListToSave) {
            this.SavePanelYUIPanel.Style.Add(HtmlTextWriterStyle.Visibility, "visible");
            ClearSessionVariables();
            this.header.InnerText = "Save Hitlist";
            this.IDLabel.Text = Resource.HitListID_Label_Text;
            this.TypeLabel.Text = Resource.HitListType_Label_Text;

            if(hitListToSave.HitListType != CambridgeSoft.COE.Framework.HitListType.SAVED)
                SaveButton.Text = "Save";

            this.IDTextBox.Text = hitListToSave.ID.ToString();
            this.TypeTextBox.Text = hitListToSave.HitListType.ToString();
            this.NameTextBox.Text = hitListToSave.Name;
            this.DescriptionTextBox.Text = hitListToSave.Description;
            this.DatabaseTextBox.Text = hitListToSave.DatabaseName;
            this.UserTextBox.Text = hitListToSave.UserID;
            this.DateCreatedTextBox.Text = hitListToSave.DateCreated.ToString();
            this.NumHitsTextBox.Text = hitListToSave.NumHits.ToString();

            _dataBound = true;

            this.HitListBO = hitListToSave;
        }

        public void DataBind(COESearchCriteriaBO searchCriteriaToSave) {
            this.SavePanelYUIPanel.Style.Add(HtmlTextWriterStyle.Visibility, "visible");
            ClearSessionVariables();
            this.TypeLabel.Visible = false;
            this.TypeTextBox.Visible = false;

            this.header.InnerText = "Save Query";
            this.IDLabel.Text = Resource.SearchCriteriaID_Label_Text;

            this.IDTextBox.Text = searchCriteriaToSave.ID.ToString();
            this.NameTextBox.Text = searchCriteriaToSave.Name;
            this.DescriptionTextBox.Text = searchCriteriaToSave.Description;
            this.DatabaseTextBox.Text = searchCriteriaToSave.DatabaseName;
            this.UserTextBox.Text = searchCriteriaToSave.UserName;
            this.DateCreatedTextBox.Text = searchCriteriaToSave.DateCreated.ToLongDateString();
            this.NumHitsTextBox.Text = searchCriteriaToSave.NumberOfHits.ToString();

            _dataBound = true;

            this.SearchCriteriaBO = searchCriteriaToSave;
        }

        private void Save() {
            // if both are nulls let's suppose its a hitlist
            if(HitListBO == null && SearchCriteriaBO == null) {
                HitListBO = COEHitListBO.Get((HitListType) Enum.Parse(typeof(HitListType), this.TypeTextBox.Text), int.Parse(this.IDTextBox.Text));
            }
            if(HitListBO != null) {
                this.UnBind(HitListBO);
                if(HitListBO.HitListType != CambridgeSoft.COE.Framework.HitListType.SAVED)
                    HitListBO = HitListBO.Save();
                else
                    HitListBO = HitListBO.Update();
            }
            if(SearchCriteriaBO != null) {
                this.UnBind(SearchCriteriaBO);
                if(SearchCriteriaBO.SearchCriteriaType != SearchCriteriaType.SAVED)
                    SearchCriteriaBO.Save();
                else
                    SearchCriteriaBO.Update();
            }
            
            ClearSessionVariables();
            if(Saved != null) {
                HitListUnaryOperationHandler currentEventRaised = Saved; // Copy to a temporary variable to be thread-safe.
                HitListType hitListType = string.IsNullOrEmpty(this.TypeTextBox.Text) ? HitListType.TEMP : (HitListType) Enum.Parse(typeof(HitListType), this.TypeTextBox.Text);
                currentEventRaised(this, new HitListUnaryOperationEventArgs(int.Parse(this.IDTextBox.Text), hitListType, "SAVE"));
            }
        }

        private void UnBind(COEHitListBO hitListBO) {
            hitListBO.Name = this.NameTextBox.Text;
            hitListBO.DatabaseName = this.DatabaseTextBox.Text;
            hitListBO.Description = this.DescriptionTextBox.Text;
            hitListBO.NumHits = int.Parse(this.NumHitsTextBox.Text);
        }

        private void UnBind(COESearchCriteriaBO searchCriteriaBO) {
            searchCriteriaBO.Name = this.NameTextBox.Text;
            searchCriteriaBO.DatabaseName = this.DatabaseTextBox.Text;
            searchCriteriaBO.Description = this.DescriptionTextBox.Text;
            searchCriteriaBO.NumberOfHits = int.Parse(this.NumHitsTextBox.Text);
        }

        private void ClearSessionVariables() {
            HitListBO = null;
            SearchCriteriaBO = null;
        }
        #endregion
    }
    public class HitListUnaryOperationEventArgs : EventArgs {
        public int HitListID;
        public HitListType HitListType;
        public string Operation;

        public HitListUnaryOperationEventArgs(int hitListID, HitListType hitListType, string operation) {
            this.HitListID = hitListID;
            this.HitListType = hitListType;
            this.Operation = operation;
        }
    }
}
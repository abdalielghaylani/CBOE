using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using Resources;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.ContentArea {
    public partial class ManageHitList : GUIShellPage {
        #region Properties
        public GenericBO BusinessObject {
            get {
                return (GenericBO) Session["BasePageBusinessObject"];
            }
            set {
                Session["BasePageBusinessObject"] = value;
            }
        }
        #endregion

        #region LifeCycle Events
        protected void Page_Load(object sender, EventArgs e) {
            RegisterClientScripts();
            SetControlsAttributtes();
            SubscribeToControlsEvents();
            BuildHitListTree();
            if(!Page.IsPostBack) {
                SetDisabledControls();
            }
            UncheckTreeNodes();
        }
        #endregion

        #region Event Handlers
        
        void SavePanel_Saved(object sender, HitListUnaryOperationEventArgs eventArgs) {
            if(eventArgs.Operation == "RESTORE")
                Redirect(eventArgs.HitListID, eventArgs.HitListType.ToString());
            else if(eventArgs.Operation == "DELETE" || eventArgs.Operation == "SAVE")
                BuildHitListTree();

        }

        void HitlistOperationPanel_ActionPerformed(object sender, HitListBinaryOperationEventArgs eventArgs) {
            Redirect(eventArgs.NewHitListID, HitListType.TEMP.ToString());
        }
        #endregion

        #region Private Methods
        private void RegisterClientScripts() {
            if(!Page.ClientScript.IsClientScriptBlockRegistered("updateHitList")) {
                string updateHitListJs = @"
                var checkedNodes = 0;
                var isFiredByUser = true;
                var checkedNodesList = new Array('', '');
                function NodeChecked(treeId, nodeId, bChecked)
                {
                    if(isFiredByUser) {
                        if(bChecked)
                            checkedNodes++;
                        else
                            checkedNodes--;
                            
                        var checkedNode = igtree_getNodeById(nodeId);
                        

                        if(checkedNodes < 2) {
                            document.getElementById('" + this.UnionButton.ClientID + @"').disabled = true;
                            document.getElementById('" + this.SubtractButton.ClientID + @"').disabled = true;
                            document.getElementById('" + this.IntersectButton.ClientID + @"').disabled = true;
                        }
                        else {
                            document.getElementById('" + this.UnionButton.ClientID + @"').disabled = false;
                            document.getElementById('" + this.SubtractButton.ClientID + @"').disabled = false;
                            document.getElementById('" + this.IntersectButton.ClientID + @"').disabled = false;
                        }

                        if(checkedNodes < 3) {
                            if(bChecked)
                                checkedNodesList[checkedNodes - 1] = checkedNode;
                            else {
                                if(checkedNodesList[0] == checkedNode) {
                                    checkedNodesList[0] = checkedNodesList[1];
                                    checkedNodesList[1] = '';
                                }
                                else
                                    checkedNodesList[1] = '';
                            }
                        }
                        else {
                            isFiredByUser = false;
                            checkedNode.setChecked(false);
                            checkedNodes--;
                            isFiredByUser = true;
                        }
                        if(checkedNodes == 1) {
                            document.getElementById('" + this.DeleteHitListButton.ClientID + @"').disabled = false;
                            document.getElementById('" + this.RestoreHitListButton.ClientID + @"').disabled = false;
                            var saved = (checkedNodesList[0] == '' && checkedNode.getParent().getText().lastIndexOf('SAVED') >= 0) || (checkedNodesList[0] != '' && checkedNodesList[0].getParent().getText().lastIndexOf('SAVED') >= 0);
                            var temp = (checkedNodesList[0] == '' && checkedNode.getParent().getText().lastIndexOf('TEMP') >= 0) || (checkedNodesList[0] != '' && checkedNodesList[0].getParent().getText().lastIndexOf('TEMP') >= 0);
                            
                            if(saved) {
                                document.getElementById('" + this.SaveHitListButton.ClientID + @"').disabled = true;
                                document.getElementById('" + this.EditHitListButton.ClientID + @"').disabled = false;
                            }
                            else if(temp) {
                                document.getElementById('" + this.SaveHitListButton.ClientID + @"').disabled = false;
                                document.getElementById('" + this.EditHitListButton.ClientID + @"').disabled = true;
                            }
                        }
                        else {
                            document.getElementById('" + this.DeleteHitListButton.ClientID + @"').disabled = true;
                            document.getElementById('" + this.RestoreHitListButton.ClientID + @"').disabled = true;
                            document.getElementById('" + this.SaveHitListButton.ClientID + @"').disabled = true;
                            document.getElementById('" + this.EditHitListButton.ClientID + @"').disabled = true;
                        }
                    }
                    return;
                }

                function NodeChanged(treeID, nodeID)
                {       
                    var node = igtree_getNodeById(nodeID);
                    if(node.hasChildren())
                        return false;
                    //id-name-description-databasename-user-datecreated-numhits
                    var hitListInfo = node.getTag().split('-');
                    document.getElementById('" + this.IDTextBox.ClientID + @"').value = hitListInfo[0];
                    if(node.getParent().getText().lastIndexOf('SAVED') >= 0) {
                        document.getElementById('" + this.TypeTextBox.ClientID + @"').value = 'SAVED';
                    }
                    else if(node.getParent().getText().lastIndexOf('TEMP') >= 0) {
                        document.getElementById('" + this.TypeTextBox.ClientID + @"').value = 'TEMP';
                    }
                    document.getElementById('" + this.NameTextBox.ClientID + @"').value = hitListInfo[1];
                    document.getElementById('" + this.DescriptionTextBox.ClientID + @"').value = hitListInfo[2];
                    document.getElementById('" + this.DatabaseTextBox.ClientID + @"').value = hitListInfo[3];
                    document.getElementById('" + this.UserTextBox.ClientID + @"').value = hitListInfo[4];
                    document.getElementById('" + this.DateCreatedTextBox.ClientID + @"').value = hitListInfo[5];
                    document.getElementById('" + this.NumHitsTextBox.ClientID + @"').value = hitListInfo[6];
                    
                }

                function bindSavePanel() {
                    //id-name-description-databasename-user-datecreated-numhits
                    var hitListInfo = checkedNodesList[0].getTag().split('-');
                    var id = hitListInfo[0];
                    var name = hitListInfo[1];
                    var description = hitListInfo[2];
                    var databasename = hitListInfo[3];
                    var user = hitListInfo[4];
                    var datecreated = hitListInfo[5];
                    var numhits = hitListInfo[6];
                    var hitlisttype = 'TEMP';
                    if(checkedNodesList[0].getParent().getText().lastIndexOf('SAVED') >= 0)
                        hitlisttype = 'SAVED';
                    dataBind(id,name,description,databasename,user,datecreated,numhits,hitlisttype)
                }

                function bindConfirmationPanel(action) {
                    //hitlistId, action {[RESTORE,DELETE]}
                    var hitListInfo = checkedNodesList[0].getTag().split('-');
                    var id = hitListInfo[0];                    
                    var hitlisttype = 'TEMP';
                    if(checkedNodesList[0].getParent().getText().lastIndexOf('SAVED') >= 0)
                        hitlisttype = 'SAVED';
                    
                    
                    dataBindConfirmationPanel(id,hitlisttype,action)
                }

                function bindHitListOperationPanel(action) {
                    var hitListInfo = checkedNodesList[0].getTag().split('-');
                    var idA = hitListInfo[0];                    
                    var hitlisttypeA = 'TEMP';
                    if(checkedNodesList[0].getParent().getText().lastIndexOf('SAVED') >= 0)
                        hitlisttypeA = 'SAVED';
                    
                    hitListInfo = checkedNodesList[1].getTag().split('-');
                    var idB = hitListInfo[0];                    
                    var hitlisttypeB = 'TEMP';
                    if(checkedNodesList[1].getParent().getText().lastIndexOf('SAVED') >= 0)
                        hitlisttypeB = 'SAVED';

                    dataBindHitListOperationPanel(parseInt(idA),hitlisttypeA,parseInt(idB),hitlisttypeB,action,'" + this.BusinessObject.DataView.Database + @"');
                }
                ";
                Page.ClientScript.RegisterClientScriptBlock(typeof(ManageHitList), "updateHitList", updateHitListJs, true);
            }
        }

        private void SubscribeToControlsEvents() {
            this.SavePanel.ShowingControlsID.Add(this.SaveHitListButton.ClientID);
            this.SavePanel.ShowingControlsID.Add(this.EditHitListButton.ClientID);

            this.SaveHitListButton.Attributes.Add("onclick", "bindSavePanel()");
            this.EditHitListButton.Attributes.Add("onclick", "bindSavePanel()");

            this.ConfirmationPanel.ShowingControlsID.Add(this.RestoreHitListButton.ClientID);
            this.ConfirmationPanel.ShowingControlsID.Add(this.DeleteHitListButton.ClientID);

            this.DeleteHitListButton.Attributes.Add("onclick", "bindConfirmationPanel('DELETE')");
            this.RestoreHitListButton.Attributes.Add("onclick", "bindConfirmationPanel('RESTORE')");

            this.HitlistOperationPanel.ShowingControlsID.Add(this.UnionButton.ClientID);
            this.HitlistOperationPanel.ShowingControlsID.Add(this.IntersectButton.ClientID);
            this.HitlistOperationPanel.ShowingControlsID.Add(this.SubtractButton.ClientID);

            this.UnionButton.Attributes.Add("onclick", "bindHitListOperationPanel('UNION')");
            this.IntersectButton.Attributes.Add("onclick", "bindHitListOperationPanel('INTERSECT')");
            this.SubtractButton.Attributes.Add("onclick", "bindHitListOperationPanel('SUBTRACT')");

            this.SavePanel.Saved += new SavePanel.HitListUnaryOperationHandler(SavePanel_Saved);
            this.ConfirmationPanel.ActionPerformed += new ConfirmationPanel.HitListUnaryOperationHandler(SavePanel_Saved);
            this.HitlistOperationPanel.ActionPerformed += new HitlistOperationPanel.HitListBinaryOperationHandler(HitlistOperationPanel_ActionPerformed);
        }

        private void UncheckTreeNodes() {
            foreach(Infragistics.WebUI.UltraWebNavigator.Node node in this.HitListUltraWebTree.Nodes[0].Nodes[0].Nodes) {
                node.Checked = false;
            }
            foreach(Infragistics.WebUI.UltraWebNavigator.Node node in this.HitListUltraWebTree.Nodes[0].Nodes[1].Nodes) {
                node.Checked = false;
            }
        }

        private void SetDisabledControls() {
            this.SaveHitListButton.Enabled = false;
            this.EditHitListButton.Enabled = false;
            this.DeleteHitListButton.Enabled = false;
            this.RestoreHitListButton.Enabled = false;
            this.UnionButton.Enabled = false;
            this.IntersectButton.Enabled = false;
            this.SubtractButton.Enabled = false;
        }

        private void BuildHitListTree() {
            HitListUltraWebTree.Nodes.Clear();
            HitListUltraWebTree.Nodes.Add("HitLists");
            HitListUltraWebTree.Nodes[0].CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
            HitListUltraWebTree.Nodes[0].Nodes.Add("TEMP HitLists {Date Time (NumHits)}");
            HitListUltraWebTree.Nodes[0].Nodes[0].CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
            int i = 0;
            foreach(COEHitListBO hitListBO in COEHitListBOList.GetRecentHitLists(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 10)) {
                //id-name-description-databasename-user-datecreated-numhits
                string tag = hitListBO.ID + "-" + hitListBO.Name + "-" + hitListBO.Description + "-" + hitListBO.DatabaseName + "-" + hitListBO.UserID + "-" + hitListBO.DateCreated + "-" + hitListBO.NumHits;
                HitListUltraWebTree.Nodes[0].Nodes[0].Nodes.Add(hitListBO.DateCreated + " " + hitListBO.DateCreated.Date.TimeOfDay + " (" + hitListBO.NumHits + ")", tag);
                if(++i > 15) {
                    i = 0;
                    break;
                }
            }

            HitListUltraWebTree.Nodes[0].Nodes.Add("SAVED HitLists {Name - Description (NumHits)}");
            HitListUltraWebTree.Nodes[0].Nodes[1].CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
            foreach(COEHitListBO hitListBO in COEHitListBOList.GetSavedHitListList(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID)) {
                //id-name-description-databasename-user-datecreated-numhits
                string tag = hitListBO.ID + "-" + hitListBO.Name + "-" + hitListBO.Description + "-" + hitListBO.DatabaseName + "-" + hitListBO.UserID + "-" + hitListBO.DateCreated + "-" + hitListBO.NumHits;
                HitListUltraWebTree.Nodes[0].Nodes[1].Nodes.Add(hitListBO.Name + " - " + hitListBO.Description + " (" + hitListBO.NumHits + ")", tag);
                if(++i > 15) {
                    i = 0;
                    break;
                }
            }
            HitListUltraWebTree.ExpandAll();
            HitListUltraWebTree.CompactRendering = true;
        }

        protected override void SetControlsAttributtes() {
            HitListUltraWebTree.CheckBoxes = true;
            HitListUltraWebTree.ClientSideEvents.NodeChecked = "NodeChecked";
            HitListUltraWebTree.ClientSideEvents.AfterNodeSelectionChange = "NodeChanged";

            this.HitListInfoLabel.Text = Resource.HitListInfo_Label_Text;

            this.IDLabel.Text = Resource.HitListID_Label_Text;
            this.TypeLabel.Text = Resource.HitListType_Label_Text;
            this.NameLabel.Text = Resource.Name_Label_Text;
            this.DatabaseLabel.Text = Resource.Database_Label_Text;
            this.UserLabel.Text = Resource.User_Label_Text;
            this.DescriptionLabel.Text = Resource.Description_Label_Text;
            this.NumHitsLabel.Text = Resource.NumberOfHits_Label_Text;
            this.DateCreatedLabel.Text = Resource.DateCreated_Label_Text;

            this.SaveHitListButton.Text = Resource.Save_Button_Text;
            this.DeleteHitListButton.Text = Resource.Delete_Button_Text;
            this.RestoreHitListButton.Text = Resource.Restore_Button_Text;
            this.EditHitListButton.Text = Resource.Edit_Button_Text;

            this.OperationsLabel.Text = Resource.Operations_Label_Text;
            this.UnionButton.Text = Resource.Union_Button_Text;
            this.SubtractButton.Text = Resource.Subtract_Button_Text;
            this.IntersectButton.Text = Resource.Intersect_Button_Text;
        }

        private void Redirect(int hitListID, string hitListType) {
            Server.Transfer(this.Page.ResolveUrl(Constants.GetSearchContentAreaFolder() + "/ChemBioVizSearch.aspx?HitListID=" + hitListID + "&HitListType=" + hitListType));
        }
        #endregion
    }
}

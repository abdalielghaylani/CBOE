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
using CambridgeSoft.COE.Framework.COEHitListService;
using Infragistics.WebUI.UltraWebListbar;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using Resources;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls
{
    public partial class ManagerPane : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Variables
        private COEHitListBOList _tempHitLists = null;
        private COEHitListBOList _savedHitLists = null;
        private COEHitListBOList _tempQueryHistoryLists = null;
        #endregion

        #region Properties
        private GenericBO BusinessObject
        {
            get
            {
                return ((ChemBioVizSearch) this.Page).BusinessObject;
            }
        }
        
        public int ResultHitListID
        {
            get
            {
                if(ViewState["ResultHitListID"] == null)
                    ViewState["ResultHitListID"] = -1;
                return (int) ViewState["ResultHitListID"];
            }
            set
            {
                ViewState["ResultHitListID"] = value;
            }
        }
        
        public HitListType ResultHitListType
        {
            get
            {
                if(ViewState["ResultHitListType"] == null)
                    ViewState["ResultHitListType"] = HitListType.TEMP;
                return (HitListType) ViewState["ResultHitListType"];
            }
            set
            {
                ViewState["ResultHitListType"] = value;
            }
        }

        private HitListInfo CurrentHitlistInfo
        {
            get
            {
                if (ViewState["CurrentHitlistInfo"] == null)
                    ViewState["CurrentHitlistInfo"] = new HitListInfo();
                return (HitListInfo)ViewState["CurrentHitlistInfo"];
            }
            set
            {
                ViewState["CurrentHitlistInfo"] = value;
            }
        }
        #endregion

        #region LyfeCycle methods
        protected void Page_Load(object sender, EventArgs e)
        {
            this.EditLinkButton.OnClientClick = "return ValidatePriorSubmit('" + this.SelectHitlistDropDownList.ClientID + 
                                                "', document.getElementById('" + this.NameTextBox.ClientID +
                                                "'), document.getElementById('" + this.DescriptionTextBox.ClientID + "'));";
            this.RestoreButton.OnClientClick = "return ValidatePriorSubmit('" + this.SelectHitlistDropDownList.ClientID + 
                                                "', forms[0]['" + this.RestoreTypeRadioButtonList.UniqueID + "'], null);";
            this.DeleteLinkButton.OnClientClick = "return ValidatePriorSubmit('" + this.SelectHitlistDropDownList.ClientID + "', null, null);";
            this.RecentQueriesRepeater.ItemDataBound += new RepeaterItemEventHandler(RecentQueriesRepeater_ItemDataBound);
            this.SavedQueriesRepeater.ItemDataBound += new RepeaterItemEventHandler(SavedQueriesRepeater_ItemDataBound);
            this.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        #endregion

        #region Event Handlers
        protected void RecentQueriesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            COEHitListBO dataItem = e.Item.DataItem as COEHitListBO;
            if (dataItem != null)
            {
                MenuButton menu = e.Item.FindControl("RecentQueryItemMenuButton") as MenuButton;
                if (menu != null) //Coverity Fix for CID : 11434
                {
                    foreach (MenuItem item in menu.MenuItemList)
                    {
                        item.CommandArgument = dataItem.HitListType.ToString() + "," + dataItem.HitListID.ToString();
                    }
                }
            }
        }

        protected void SavedQueriesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            COEHitListBO dataItem = e.Item.DataItem as COEHitListBO;
            if (dataItem != null)
            {
                MenuButton menu = e.Item.FindControl("SavedQueryItemMenuButton") as MenuButton;
                if (menu != null) //Coverity Fix for CID : 11435
                {
                    foreach (MenuItem item in menu.MenuItemList)
                    {
                        item.CommandArgument = dataItem.HitListType.ToString() + "," + dataItem.HitListID.ToString();
                    }
                }
            }
        }

        protected void Queries_ItemCommand(object source, CommandEventArgs e)
        {
            if (CommandRaised != null)
            {
                CommandRaised(this, new COENavigationPanelControlEventArgs(this.ID, e.CommandName, e.CommandArgument.ToString().Split(',')));
            }
        }

        public void EditButton_Click(object sender, EventArgs e)
        {
            this.EditQuery();
            this.FillGroups();
        }

        public void RestoreButton_Click(object sender, EventArgs e)
        {
            if(CommandRaised != null)
            {
                this.DoRestoreActions();
                this.FillGroups();
                CommandRaised(this, new COENavigationPanelControlEventArgs(this.ID, "RestoreHitlist", new string[2] { this.ResultHitListType.ToString(), this.ResultHitListID.ToString() }));
            }
        }

        public void DeleteButton_Click(object sender, EventArgs e)
        {
            this.DeleteQuery();
            this.FillGroups();
        }

        public void CancelButton_Click(object sender, EventArgs e)
        {
            this.SetDefaultValues();
        }
        #endregion

        #region Private Methods
        private void FillGroups()
        {
            DateTime dt = DateTime.Now.Date;
            DateTime newDate = DateTime.Now.Date;
            //Get the configuration value for the HitlistHistory
            if (Session["HitlistHistoryConfigDays"] != null)
            {
                string hitlistHistoryValue = Session["HitlistHistoryConfigDays"].ToString();
                dt = newDate.AddDays(-Convert.ToDouble(hitlistHistoryValue));
                //create new overide function for getting the data from DB based on the date configuration
                _tempHitLists = COEHitListBOList.GetRecentHitListsConfig(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 10, dt);
            }
            else
                _tempHitLists = COEHitListBOList.GetRecentHitLists(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 10);
            if (_tempHitLists.Count > 0)
                this.CurrentHitlistInfo = _tempHitLists[0].HitListInfo;
            if (Session["QueryHistoryConfigDays"] != null)
            {
                //Get the configuration value for the QueryHistory
                string queryHistoryValue = Session["QueryHistoryConfigDays"].ToString();
                dt = newDate.AddDays(-Convert.ToDouble(queryHistoryValue));
                _tempQueryHistoryLists = COEHitListBOList.GetRecentHitListsConfig(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 10, dt);
            }

            _savedHitLists = COEHitListBOList.GetSavedHitListList(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID);
            this.FillHistoryGroup();
            this.FillSavedGroup();
            this.FillAdvancedGroup();
        }

        private void FillSavedGroup()
        {
            this.SavedQueriesRepeater.Controls.Clear();
            this.SavedQueriesRepeater.DataSource = _savedHitLists;
            this.SavedQueriesRepeater.DataBind();
        }

        private void FillHistoryGroup()
        {
            this.RecentQueriesRepeater.Controls.Clear();
            if (Session["QueryHistoryConfigDays"] != null)
            {
                this.RecentQueriesRepeater.DataSource = _tempQueryHistoryLists;
            }
            else
                this.RecentQueriesRepeater.DataSource = _tempHitLists;
            this.RecentQueriesRepeater.DataBind();
        }

        private void FillAdvancedGroup()
        {
            this.SelectHitlistLabel.Text = Resource.SelectHitList_Label_Text;
            this.SelectHitlistDropDownList.Items.Clear();

            this.SelectHitlistDropDownList.Items.Add(new ListItem("Choose a hitlist...", "-1,TEMP"));
            string text = string.Empty;
            string value = string.Empty;

            foreach(COEHitListBO hitList in _tempHitLists)
            {
                text = string.Format("{0} - ({1}) - ({2})", hitList.Name, hitList.NumHits, hitList.DateCreated);
                value = string.Format("{0},{1},{2}", hitList.ID, hitList.HitListID, hitList.HitListType);
                
                this.SelectHitlistDropDownList.Items.Add(new ListItem(text, value));
            }
            foreach(COEHitListBO hitList in _savedHitLists)
            {
                text = string.Format("{0} - ({1}) - ({2})", hitList.Name, hitList.NumHits, hitList.DateCreated);
                value = string.Format("{0},{1},{2}", hitList.ID, hitList.HitListID, hitList.HitListType);

                this.SelectHitlistDropDownList.Items.Add(new ListItem(text, value));
            }
            this.SelectHitlistDropDownList.SelectedIndex = 0;

            #region Edit Attributes
            this.FillQueryPropertiesLabel.Text = Resource.FillQueryDetails_Label_Text;
            this.NameLabel.Text = Resource.Name_Label_Text;
            this.DescriptionLabel.Text = Resource.Description_Label_Text;
            this.IsPublicCheckBox.Text = Resource.IsPublic_Label_Text;
            this.EditLinkButton.Text = Resource.Edit_Button_Text;
            this.CancelEditLinkButton.Text = Resource.Cancel_Button_Text;
            #endregion

            #region Restore Attributes
            this.RestoreTypeLabel.Text = Resource.SelectRestoreType_Label_Text;
            this.RestoreTypeRadioButtonList.Items[0].Text = Resource.ReplaceType_Label_Text;
            this.RestoreTypeRadioButtonList.Items[1].Text = Resource.IntersectType_Label_Text;
            this.RestoreTypeRadioButtonList.Items[2].Text = Resource.SubtractType_Label_Text;
            this.RestoreTypeRadioButtonList.Items[3].Text = Resource.UnionType_Label_Text;
            this.RestoreButton.Text = Resource.Restore_Button_Text;
            this.CancelRestoreLinkButton.Text = Resource.Cancel_Button_Text;
            #endregion

            #region Delete Attributes
            this.ConfirmDeleteLabel.Text = Resource.DeleteConfirmation_Label_Text;
            this.DeleteLinkButton.Text = Resource.Delete_Button_Text;
            this.CancelDeleteLinkButton.Text = Resource.Cancel_Button_Text;
            #endregion
        }

        private int GetSelectedHitListIDFromDropDown()
        {
            return int.Parse(this.SelectHitlistDropDownList.SelectedValue.Split(',')[0]);
        }

        private int GetSelectedHitListHitListIDFromDropDown()
        {
            return int.Parse(this.SelectHitlistDropDownList.SelectedValue.Split(',')[1]);
        }

        private HitListType GetSelectedHitListTypeFromDropDown()
        {
            if(this.SelectHitlistDropDownList.SelectedIndex > 0)
                return (HitListType) Enum.Parse(typeof(HitListType), this.SelectHitlistDropDownList.SelectedValue.Split(',')[2]);
            else
                throw new Exception(Resource.NoSelectedHitList);
        }

        private void EditQuery()
        {
            COEHitListBO hitlistBO = COEHitListBO.Get(this.GetSelectedHitListTypeFromDropDown(), this.GetSelectedHitListIDFromDropDown());
            hitlistBO.Name = this.NameTextBox.Text;
            hitlistBO.Description = this.DescriptionTextBox.Text;
            hitlistBO.IsPublic = this.IsPublicCheckBox.Checked;
            hitlistBO.Update();

            if(hitlistBO.SearchCriteriaID > 0)
            {
                COESearchCriteriaBO searchCriteria = COESearchCriteriaBO.Get(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
                searchCriteria.Name = hitlistBO.Name;
                searchCriteria.Description = hitlistBO.Description;
                searchCriteria.IsPublic = hitlistBO.IsPublic;
                searchCriteria.Update();
            }
        }

        private void DoRestoreActions()
        {
            //ID,type
            int hitlistId = this.GetSelectedHitListHitListIDFromDropDown();
            HitListType hitlistType = this.GetSelectedHitListTypeFromDropDown();
            COEHitListBO resultBO = null;
            HitListInfo info = null;
            switch(this.RestoreTypeRadioButtonList.SelectedIndex)
            {
                case 0: //restore
                    this.ResultHitListID = hitlistId;
                    this.ResultHitListType = hitlistType;
                    break;
                case 1: //intersect
                    info = new HitListInfo();
                    info.HitListID = hitlistId;
                    info.HitListType = hitlistType;
                    resultBO = COEHitListOperationManager.IntersectHitList(this.CurrentHitlistInfo, info, this.BusinessObject.DataView.DataViewID);
                    this.ResultHitListID = resultBO.HitListID;
                    this.ResultHitListType = resultBO.HitListType;
                    break;
                case 2: //subtract
                    info = new HitListInfo();
                    info.HitListID = hitlistId;
                    info.HitListType = hitlistType;
                    resultBO = COEHitListOperationManager.SubtractHitLists(this.CurrentHitlistInfo, info, this.BusinessObject.DataView.DataViewID);
                    this.ResultHitListID = resultBO.HitListID;
                    this.ResultHitListType = resultBO.HitListType;
                    break;
                case 3: //union
                    info = new HitListInfo();
                    info.HitListID = hitlistId;
                    info.HitListType = hitlistType;
                    resultBO = COEHitListOperationManager.UnionHitLists(this.CurrentHitlistInfo, info, this.BusinessObject.DataView.DataViewID);
                    this.ResultHitListID = resultBO.HitListID;
                    this.ResultHitListType = resultBO.HitListType;
                    break;
            }
        }
        
        private void DeleteQuery()
        {
            COEHitListBO hitlistBO = COEHitListBO.Get(this.GetSelectedHitListTypeFromDropDown(), this.GetSelectedHitListIDFromDropDown());
            if(hitlistBO.SearchCriteriaID > 0)
            {
                COESearchCriteriaBO.Delete(hitlistBO.SearchCriteriaType, hitlistBO.SearchCriteriaID);
            }
            COEHitListBO.Delete(hitlistBO.HitListType, hitlistBO.HitListID);
            this.SelectHitlistDropDownList.SelectedIndex = 0;
        }

        private void SetDefaultValues()
        {
            //TODO: Set default values for restore contents.
        }
        #endregion

        #region Public Methods
        internal void ShowAdvancedGroup()
        {
            this.DefaultExpandedGroup.Value = "AdvancedContainer";
        }

        public override void DataBind()
        {
            this.FillGroups();
            base.DataBind();
        }
        #endregion

        #region ICOENavigationPanelControl Members
        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;
        #endregion

    }
}
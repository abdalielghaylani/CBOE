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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using System.Collections.Generic;

namespace Manager.Forms.DataViewManager.UserControls
{
    public partial class EditTags : System.Web.UI.UserControl
    {
        #region Constants
        private const string _rootNode = "rootNode";
        #endregion

        #region Properties
        /// <summary>
        /// Current selected Table.
        /// </summary>
        private string SelectedTable
        {
            get
            {
                string retVal = String.Empty;
                if (ViewState[Constants.SelectedTable] != null)
                    retVal = ViewState[Constants.SelectedTable].ToString() == _rootNode ? String.Empty : ViewState[Constants.SelectedTable].ToString();
                return retVal;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState[Constants.SelectedTable] = value;
            }
        }

        /// <summary>
        /// Current Selected tableId
        /// </summary>
        private int SelectedTableId
        {
            get
            {
                int retVal = -1;
                if (!string.IsNullOrEmpty(this.SelectedTable))
                    int.TryParse(Utilities.GetParamInDataKey(this.SelectedTable, Constants.DataKeysParam.ID), out retVal);
                return retVal;
            }
        }

        /// <summary>
        /// Current COEDataViewBO
        /// </summary>
        private COEDataViewBO DataViewBO
        {
            get
            {
                return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
            }
        }

        private string CancelURL
        {
            get
            {
                string retVal = String.Empty;
                if (ViewState["CancelURL"] != null)
                    retVal = (string)ViewState["CancelURL"];
                return retVal;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["CancelURL"] = value;
            }
        }

        private TableBO OriginalTable
        {
            get
            {
                return (TableBO)Session["OriginalTable"];
            }
            set
            {
                Session["OriginalTable"] = value;
            }
        }
        #endregion

        #region Page events
        protected override void OnInit(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            base.OnInit(e);
            this.AddTagButton.Click += new EventHandler(AddTagButton_Click);
            this.DeleteTagButton.Click += new EventHandler(DeleteTagButton_Click);
            this.OkImageButton.ButtonClicked += new EventHandler<EventArgs>(OkImageButton_ButtonClicked);
            this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);

            this.AddTagButton.Text = Resources.Resource.Add_Button_Text;
            this.DeleteTagButton.Text = Resources.Resource.Delete_Button_Text;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            ((GUIShellMaster)Page.Master).EnableYUI = true;

            if (string.IsNullOrEmpty(this.CancelURL))
            {
                this.CancelURL = Page.ResolveUrl("~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx");
                this.CancelURL += "?" + Constants.ParamCaller + "=" + this.SelectedTableId;
            }

            if (this.OriginalTable == null)
                this.OriginalTable = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId).Clone();

            ((GUIShellMaster)this.Page.Master).SetDefaultFocus(this.TagTextBox.ClientID);
            ((GUIShellMaster)this.Page.Master).SetDefaultButton(this.AddTagButton.UniqueID);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Event Handlers
        void AddTagButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TagTextBox.Text.Trim()))
            {
                TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);

                string tag = this.TagTextBox.Text.Trim();
                if (!table.Tags.Contains(tag))
                {
                    table.Tags.Add(tag);
                    ListItem newItem = new ListItem(tag);                   
                    this.TagsListBox.Items.Add(newItem);                 
                }

                SetToolTip();

                this.TagTextBox.Text = string.Empty;
            }
        }

        void DeleteTagButton_Click(object sender, EventArgs e)
        {
            int i = this.TagsListBox.Items.Count - 1;
            TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
            while (i >= 0)
            {
                if (this.TagsListBox.Items[i].Selected)
                {
                    string tag = this.TagsListBox.Items[i].Text;
                    if (table.Tags.Contains(tag))
                    {
                        table.Tags.Remove(tag);
                    }
                    this.TagsListBox.Items.RemoveAt(i);
                }
                i--;
            }
            SetToolTip();
        }

        void OkImageButton_ButtonClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            string urlRedirect = "~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx";

            this.Session.Remove("OriginalTable");

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(Page.ResolveUrl(urlRedirect));
        }

        void CancelImageButton_ButtonClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.RevertTagsEdition();
            this.Session.Remove("OriginalTable");
            Session["EditCancel"] = true;
            Server.Transfer(this.CancelURL, false);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods
        internal void DataBind(CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO dataViewBO, string selectedTableID)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            int tempTableId = int.MinValue;
            if (dataViewBO != null)
            {
                this.SaveItAsSession(dataViewBO);
                if (int.TryParse(selectedTableID, out tempTableId))
                    this.SelectedTable = "ID=" + tempTableId.ToString();
                this.BindTagList(dataViewBO.DataViewManager.Tables.GetTable(tempTableId));
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void BindTagList(TableBO table)
        {
            this.TagsListBox.DataSource = table.Tags;
            this.TagsListBox.DataBind();
            SetToolTip();
        }

        /// <summary>
        /// Adding tooltip for the tag present in the listbox
        /// </summary>
        private void SetToolTip()
        {
            foreach (ListItem item in TagsListBox.Items)
            {
                item.Attributes.Add("title", item.Text);
            }
        }
        
        public string GetAllTags()
        {
            //Get all tags currently on the database.
            List<string> tags = COEDataViewBO.GetAllTags();
            
            //Add the in-memory tags:
            foreach (TableBO tbl in this.DataViewBO.DataViewManager.Tables)
            {
                foreach (string tag in tbl.Tags)
                {
                    if (!tags.Contains(tag))
                        tags.Add(tag);
                }
            }
            string result = "dsLocalArray.liveData = [";
            foreach (string tag in tags)
            {
                result += string.Format("'{0}',", tag);
            }

            if (result.Length > 0 && !result.EndsWith("["))
                result = result.Remove(result.Length - 1);
            result += @"];
            ";
            return result;
        }

        /// <summary>
        /// Saves in Session the given DataViewBO
        /// </summary>
        /// <param name="dataViewBO">Dataviewbo to save as session</param>
        private void SaveItAsSession(COEDataViewBO dataViewBO)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (Session[Constants.COEDataViewBO] == null && dataViewBO != null)
                Session[Constants.COEDataViewBO] = dataViewBO;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void RevertTagsEdition()
        {
            TableBO table = this.DataViewBO.DataViewManager.Tables.GetTable(this.SelectedTableId);
            table.Tags = this.OriginalTable.Tags;
        }
        #endregion
    }
}
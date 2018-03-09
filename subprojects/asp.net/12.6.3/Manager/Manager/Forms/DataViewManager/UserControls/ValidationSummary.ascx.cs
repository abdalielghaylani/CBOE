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
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Collections.Generic;

public partial class ValidationSummaryUC : System.Web.UI.UserControl
{
    #region Properties

    private COEDataViewBO DataViewBO
    {
        get
        {
            //Coverity Fixes: CBOE-313
            COEDataViewBO coeDataViewBO = (COEDataViewBO)Session[Constants.COEDataViewBO];
            if (coeDataViewBO != null)
                return coeDataViewBO;
            return null;            
        }
        set
        {
            if (Session[Constants.COEDataViewBO] == null && value != null)
                Session[Constants.COEDataViewBO] = value;
        }
    }

    private COEDataViewManagerBO DataViewManager
    {
        get
        {
            //Coverity Fixes: CBOE-313 : CID-11778
            COEDataViewBO coeDataViewBO = this.DataViewBO;
            if (coeDataViewBO != null)
                return coeDataViewBO.DataViewManager;
            return null;            
        }
    }

    public bool IsValid
    {
        get { return _canSubmitForm; }
    }

    public bool IsMasterDataView
    {
        get
        {            
            //Coverity Fixes: CBOE-313 : CID-11779
            COEDataViewBO coeDataViewBO = this.DataViewBO;
            if (coeDataViewBO != null && coeDataViewBO.ID == Constants.MasterSchemaDataViewID)
                return true;
            return false;
           
        }
    }

    #endregion

    #region Variables

    private enum ColKeys_Table
    {
        ID,
        Name,
        Alias,
        DataBase,
        PrimaryKeyName,
        OrderIndex,
    }

    private enum ColKeys_Field
    {
        ID,
        Name,
        Alias,
        DataType,
        IndexType,
        MimeType,
        OrderIndex,
    }

    private enum ColKeys_Relationship
    {
        Parent,
        ParentKey,
        Child,
        ChildKey,
    }

    private bool _canSubmitForm = false;


    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
            this.SetControlsAttributes();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.UsersRepeater.ItemDataBound += new RepeaterItemEventHandler(UsersRepeater_ItemDataBound);
        this.RolesRepeater.ItemDataBound += new RepeaterItemEventHandler(RolesRepeater_ItemDataBound);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void RolesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (e.Item.DataItem is COERoleReadOnlyBO)
            {
                ((Label)e.Item.FindControl("RolesLabel")).Text = ((COERoleReadOnlyBO)e.Item.DataItem).RoleName;
            }
        }
        else if(e.Item.ItemType == ListItemType.Header)
        {
            ((Label)e.Item.FindControl("RolesTitleLabel")).Text = Resources.Resource.Role_Label_Text;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void UsersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (e.Item.DataItem is COEUserReadOnlyBO)
                ((Label)e.Item.FindControl("UserLabel")).Text = ((COEUserReadOnlyBO)e.Item.DataItem).UserID;
        }
        else if (e.Item.ItemType == ListItemType.Header)
            ((Label)e.Item.FindControl("UsersTitleLabel")).Text = Resources.Resource.User_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NameLabel.Text = Resources.Resource.Name_Label_Text;
        this.DescriptionLabel.Text = Resources.Resource.Description_Label_Text;
        this.NameDescriptionWebPanel.Header.Text = Resources.Resource.NameAndDescription_Label_Text;
        if (!IsMasterDataView)
            this.BaseTableTitleLabel.Text = Resources.Resource.BaseTable_Label_Text;
        else
            this.BaseTableTitleLabel.Visible = this.BaseTableValidateItem.Visible = this.BaseTableTextBoxWithPopUp.Visible = false;
        
        this.SecurityTitleLabel.Text = Resources.Resource.SecuritySettings_Label_Text;
        // Coverity Fixes: CBOE-313
        COEDataViewBO coeDataViewBO = DataViewBO;
        if (coeDataViewBO != null && coeDataViewBO.IsPublic)
            this.SecurityTitleLabel.Text += Resources.Resource.PublicDataView_Label_Text;
        else
            this.SecurityTitleLabel.Text += Resources.Resource.NotPublicDataView_Label_Text;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public void DataBind(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool isValidDV = false;
        try
        {
            isValidDV = dataViewBO.IsValid && dataViewBO.DataViewManager.IsValid;
        }
        catch { }
        //Save Datasource (just in case) for later re use and easier access.
        this.DataViewBO = dataViewBO;
        //Name & Description
        this.NameTextBoxWithPopUp.Text = dataViewBO.Name;
        this.DescriptionTextBoxWithPopUp.Text = dataViewBO.Description;
        this.NameAndDescriptionValidateItem.DataBind(dataViewBO, Constants.Validate.NameAndDescription, isValidDV);
        //Base Name
        if (!IsMasterDataView)
        {
            this.BaseTableTextBoxWithPopUp.Text = dataViewBO.DataViewManager.BaseTable;
            this.BaseTableValidateItem.DataBind(dataViewBO, Constants.Validate.BaseTable, isValidDV);
        }
        //Relationships Repeater
        this.RelationshipsInfoLabel.Text = dataViewBO.DataViewManager.Relationships.BrokenRules.Count > 0 ? Resources.Resource.SeeRelationshipsErrorsBelow : Resources.Resource.AllRelationshipsValid;

        this.RelationshipsValidateItem.DataBind(dataViewBO, Constants.Validate.Relationships, isValidDV);
        //Tables and Fields
        this.TablesAndFieldsInfoLabel.Text = dataViewBO.DataViewManager.Tables.BrokenRules.Count > 0 ? Resources.Resource.SeeTablesErrorsBelow : Resources.Resource.AllTablesAndFieldsValid;
        this.TablesFieldValidateItem.DataBind(dataViewBO, Constants.Validate.TablesAndFields, isValidDV);
        //Security
        if (!dataViewBO.IsPublic && dataViewBO.COEAccessRights != null)
        {
            if (dataViewBO.COEAccessRights.Users != null)
            {
                if (dataViewBO.COEAccessRights.Users.Count > 0)
                {
                    this.UsersRepeater.DataSource = dataViewBO.COEAccessRights.Users;
                    this.UsersRepeater.DataBind();
                }
            }
            if (dataViewBO.COEAccessRights.Roles != null)
            {
                if (dataViewBO.COEAccessRights.Roles.Count > 0)
                {
                    this.RolesRepeater.DataSource = dataViewBO.COEAccessRights.Roles;
                    this.RolesRepeater.DataBind();
                }
            }
        }
        this.SecurityValidateItem.DataBind(dataViewBO, Constants.Validate.Security, isValidDV);
        //Check if the form can be submitted - A pub property can be read from the page that includes this UC
        this.CheckIfIsValid();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void CheckIfIsValid()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        _canSubmitForm = this.NameAndDescriptionValidateItem.IsValid &&
                        this.BaseTableValidateItem.IsValid &&
                        this.RelationshipsValidateItem.IsValid &&
                        this.TablesFieldValidateItem.IsValid; //TODO: Add Secutiry validation.
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
}

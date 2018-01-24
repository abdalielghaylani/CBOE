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
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDataViewService;

public partial class ToolBar : System.Web.UI.UserControl
{
    #region Public Events
    public event EventHandler Submit;
    public event EventHandler Cancel;
    #endregion

    #region Properties
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

    public string DatabaseName
    {
        get
        {
            return this.CurrentSchemaHidden.Value;
        }
        set
        {
            this.CurrentSchemaHidden.Value = value;
        }
    }

    private bool IsMasterDV
    {
        get
        {
            return (this.DataViewBO == null || this.DataViewBO.ID == Constants.MasterSchemaDataViewID);
        }
    }

    public Forms_Public_UserControls_ImageButton SubmitButton
    {
        get
        {
            return this.DoneImageButton;
        }
    }
    //CSBR-162275: Disabling Refresh/Remove Schema buttons when there are no published schemas in Edit Master
    public Forms_Public_UserControls_ImageButton RefreshSchemaButton
    {
        get
        {
            return this.RefreshSchemaImageButton;
        }
    }

    public Forms_Public_UserControls_ImageButton RemoveSchemaButton
    {
        get
        {
            return this.RemoveSchemaImageButton;
        }
    }

    public string CurrentSchemaHiddenClientID
    {
        get
        {
            return this.CurrentSchemaHidden.ClientID;
        }
    }

    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        this.RemoveSchemaImageButton.ButtonClicked += new EventHandler<EventArgs>(RemoveSchemaImageButton_ButtonClicked);
        this.RefreshSchemaImageButton.ButtonClicked += new EventHandler<EventArgs>(RefreshSchemaImageButton_ButtonClicked);
        this.NameAndDescImageButton.ButtonClicked += new EventHandler<EventArgs>(NameAndDescImageButton_ButtonClicked);
        this.SecurityImageButton.ButtonClicked += new EventHandler<EventArgs>(SecurityImageButton_ButtonClicked);
        this.BaseTableImageButton.ButtonClicked += new EventHandler<EventArgs>(BaseTableImageButton_ButtonClicked);
        this.DoneImageButton.ButtonClicked += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DivForRegularDV.Visible = !(this.DivForMasterDV.Visible = IsMasterDV);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            if (!string.IsNullOrEmpty(Page.Request["schemaSelected"]))
            {
                this.CurrentSchemaHidden.Value = Page.Request["schemaSelected"];
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Event Handlers
    void BaseTableImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Response.Redirect(Page.ResolveUrl("~/Forms/DataViewManager/ContentArea/SelectBaseTable.aspx"), false);
    }

    void SecurityImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Response.Redirect(Page.ResolveUrl("~/Forms/DataViewManager/ContentArea/Security.aspx"), false);
    }

    void NameAndDescImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Response.Redirect(Page.ResolveUrl("~/Forms/DataViewManager/ContentArea/EnterNameDescription.aspx?" + Constants.Action + "=" + Constants.PageStates.Edit_DV + "&" + Constants.SelectedDataViewID + "=" + ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetDataViewBO().ID), false);
    }

    void RemoveSchemaImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        List<int> tablesIdToRemove = new List<int>();
        foreach (TableBO table in this.DataViewBO.DataViewManager.Tables)
        {
            if (table.DataBase == this.DatabaseName)
            {
                tablesIdToRemove.Add(table.ID);
            }
        }
        if (tablesIdToRemove.Count > 0)
        {
            this.DataViewBO.DataViewManager.Tables.Remove(tablesIdToRemove);
            this.DataViewBO.DataViewManager.Relationships.Remove(tablesIdToRemove);
        }

         this.UnPublishSchema(this.DatabaseName);        

        //CBOE-885 : DVM:Edit Master-Unknown error appears when user clicks on remove link of schema selector dropdown list
        // Added querystring IsMaster=true to below URL
        this.Response.Redirect("DataviewBoard.aspx?IsMaster=true");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    // Obsoleted event function
    void RefreshSchemaImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        COEDatabaseBO bo = COEDatabaseBO.Get(this.DatabaseName).RefreshPublish();

        if (bo != null && bo.COEDataView != null)
        {
            COEDataViewManagerBO man = COEDataViewManagerBO.NewManager(bo.COEDataView);

            this.DataViewBO.DataViewManager.Merge(man.Tables, man.Relationships);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        this.Response.Redirect("DataviewBoard.aspx?schemaSelected=" + this.DatabaseName);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        if (Cancel != null)
            Cancel(sender, e);
    }

    void DoneImageButton_ButtonClicked(object sender, EventArgs e)
    {
        if (Submit != null)
            Submit(sender, e);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set all the controls values.
    /// </summary>
    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RemoveSchemaImageButton.ButtonText = this.RemoveSchemaImageButton.ImageToolTip = Resources.Resource.RemoveSchema_Button_Text;
        this.RefreshSchemaImageButton.ButtonText = this.RefreshSchemaImageButton.ImageToolTip = Resources.Resource.RefreshSchema_Button_Text;
        this.AddSchemaImageButton.ButtonText = this.AddSchemaImageButton.ImageToolTip = Resources.Resource.AddSchema_Button_Text;
        this.SecurityImageButton.ButtonText = this.SecurityImageButton.ImageToolTip = Resources.Resource.Secutiry_Page_Title;
        this.NameAndDescImageButton.ButtonText = this.NameAndDescImageButton.ImageToolTip = Resources.Resource.ChangeNameDescription_Button_Text;
        this.BaseTableImageButton.ButtonText = this.BaseTableImageButton.ImageToolTip = Resources.Resource.SelectBaseTable_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private bool UnPublishSchema(string databaseName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool retVal = false;
        if (!string.IsNullOrEmpty(databaseName))
        {
            //Get the password to send as an argument to publish.
            //string password = this.PasswordTextBox.Text;
            COEDatabaseBO database = COEDatabaseBOList.GetList().GetDatabase(databaseName);
            COEDatabaseBO unPublishedDataBase;
            if (database != null)
            {
                try
                {
                    unPublishedDataBase = database.UnPublish();
                    if (unPublishedDataBase != null) //If it fails, keep it on the list with no changes.
                    {
                        retVal = true;
                    }
                }
                catch { }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Binds the details table with the given datasource.
    /// </summary>
    /// <param name="dataKey">Datakey of the selected table to display</param>
    public void BindToolbar(string databaseName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RemoveSchemaImageButton.Visible = !string.IsNullOrEmpty(databaseName);
        this.DatabaseName = databaseName;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    public void SetButtonControl(int tableCount)
    {
        try
        {
            this.BaseTableImageButton.Enabled = false;
            if ((string.IsNullOrEmpty(this.DataViewBO.BaseTable) || (Session["IsBaseEnable"] != null && Convert.ToBoolean(Session["IsBaseEnable"]).Equals(true))) && tableCount > 1)
            {
                this.BaseTableImageButton.Enabled = true;
                Session["IsBaseEnable"] = Convert.ToBoolean(1);
            }
        }
        catch
        {
            throw;
        }
    }
    #endregion
}

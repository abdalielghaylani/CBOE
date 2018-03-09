using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Web.UI;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;

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
            if (table.DataBase.Equals(this.DatabaseName, StringComparison.InvariantCultureIgnoreCase))
            {
                tablesIdToRemove.Add(table.ID);
            }
        }
        if (tablesIdToRemove.Count > 0)
        {
            this.DataViewBO.DataViewManager.Tables.Remove(tablesIdToRemove);
            this.DataViewBO.DataViewManager.Relationships.Remove(tablesIdToRemove);
        }

        // Add it to session, will be submitted when submiting the master dataview.
        // this.UnPublishSchema(this.DatabaseName);

        var schemaOnPublishing = Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>;

        if (schemaOnPublishing != null && schemaOnPublishing.ContainsKey(this.DatabaseName.ToUpper()))
        {
            schemaOnPublishing.Remove(this.DatabaseName.ToUpper());
            Session[Constants.COESchemasOnPublishing] = schemaOnPublishing;
        }
        else
        {
            var schemasOnRemoving = Session[Constants.COESchemasOnRemoving] as Collection<string>;
            if (schemasOnRemoving == null)
            {
                schemasOnRemoving = new Collection<string>();
            }

            if (!schemasOnRemoving.Contains(this.DatabaseName.ToUpper()))
            {
                schemasOnRemoving.Add(this.DatabaseName.ToUpper());
            }

            Session[Constants.COESchemasOnRemoving] = schemasOnRemoving;
        }

        //CBOE-885 : DVM:Edit Master-Unknown error appears when user clicks on remove link of schema selector dropdown list
        // Added querystring IsMaster=true to below URL
        this.Response.Redirect("DataviewBoard.aspx?IsMaster=true");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    // Obsoleted event function
    void RefreshSchemaImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        COEDatabaseBO bo=null;

        // Try to get the schema dataviews from session, if not found then get it rom database.
        if (Session[Constants.COESchemasOnPublishing] != null)
        {
            var schemasOnPublishing = Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>;

            if (schemasOnPublishing != null && schemasOnPublishing.ContainsKey(this.DatabaseName.ToUpper()))
            {
                bo = schemasOnPublishing[this.DatabaseName.ToUpper()].Clone();
            }
            else
            {
                bo = COEDatabaseBO.Get(this.DatabaseName).RefreshPublish();
            }
        }
        else
        {
            bo = COEDatabaseBO.Get(this.DatabaseName).RefreshPublish();
        }

        if (bo != null && bo.COEDataView != null)
        {
            COEDataViewManagerBO man = COEDataViewManagerBO.NewManager(bo.COEDataView);

            this.DataViewBO.DataViewManager.Merge(man.Tables, man.Relationships);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
        var instanceSchema = this.DatabaseName.Contains(".") ? this.DatabaseName : mainInstance.Name + "." + this.DatabaseName;
        this.Response.Redirect("DataviewBoard.aspx?schemaSelected=" + instanceSchema);
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
            if ((string.IsNullOrEmpty(this.DataViewBO.BaseTable)) && tableCount > 1)
            {
                this.BaseTableImageButton.Enabled = true;
            }
        }
        catch
        {
            throw;
        }
    }
    #endregion
}

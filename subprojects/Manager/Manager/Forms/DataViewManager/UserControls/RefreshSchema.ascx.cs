using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;

public partial class RefreshSchema : System.Web.UI.UserControl
{
    #region Public Events

    public event CommandEventHandler InstanceSchemaAdded = null;
    public event CommandEventHandler Cancel = null;
    public event EventHandler<EventArgs> ErrorOcurred = null;

    #endregion

    #region Properties

    private string InstanceName { get; set; }
    private string SchemaName { get; set; }
    private bool ShowError { get; set; }

    //This value must be set by the publishing service error message exception.
    public string ErrorMessage { get; set; }

    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
        }
    }

    #endregion

    #region Page Life Cycle Events

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.InstanceName = Session["InstanceName"].ToString();
        this.SchemaName = Session["SchemaName"].ToString();
        if(!Page.IsPostBack)
            SetControlsAttributtes();
        this.Page.Form.DefaultButton = this.RefreshSchemaButton.GetButtonUniqueID();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    
    #endregion

    #region Event Handlers

    protected void RefreshSchemaButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        if (DoRefreshSchema())
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "redirect",
                "window.parent.location.href = 'DataviewBoard.aspx?IsMaster=true&schemaSelected=" +
                string.Format("{0}.{1}", this.InstanceName, this.SchemaName) + "';",
                true);
        }
        else
        {
            if (ErrorOcurred != null)
                ErrorOcurred(this, new EventArgs());
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private bool DoRefreshSchema()
    {
        try
        {
            COEDatabaseBO bo = null;

            // get schema name for main instance and "instacne.schema" for remote instance
            string qualifiedInstanceSchema = Utilities.GetQualifyInstaceSchemaName(this.InstanceName, this.SchemaName);

            // Try to get the schema dataviews from session, if not found then get it from database.
            if (Session[Constants.COESchemasOnPublishing] != null)
            {
                var schemasOnPublishing = Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>;

                if (schemasOnPublishing != null && schemasOnPublishing.ContainsKey(qualifiedInstanceSchema.ToUpper()))
                {
                    bo = schemasOnPublishing[qualifiedInstanceSchema.ToUpper()].Clone();
                }
            }

            if (bo == null)
            {
                bo = COEDatabaseBO.Get(qualifiedInstanceSchema)
                    .RefreshPublish(this.AuthorizeCheckBox.Checked, this.GranterUserTextBox.Text,
                        this.PasswordTextBox.Text);
            }

            // Merge the schema dataview into master dataview
            if (bo != null && bo.COEDataView != null)
            {
                COEDataViewManagerBO mgr = COEDataViewManagerBO.NewManager(bo.COEDataView);
                this.DataViewBO.DataViewManager.Merge(mgr.Tables, mgr.Relationships);
            }
            else
            {
                throw new Exception("Failed to refresh the schema. The schema return a null data view.");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
            ShowError = true;
            return false;
        }

        return true;
    }

    protected void CancelSchemaButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        if(string.IsNullOrEmpty(this.SchemaName))
        {
            if(Cancel != null)
            {
                Cancel(sender, new CommandEventArgs("Cancel", this.SchemaName));
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void AuthorizeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (this.AuthorizeCheckBox.Checked)
        {
            this.PasswordTextBox.Enabled = true;
            this.GranterUserRequiredField.Enabled = true;
            this.GranterUserTextBox.Enabled = true;
            this.PasswordRequiredField.Enabled = true;
        }
        else
        {
            this.PasswordTextBox.Enabled = false;
            this.PasswordTextBox.Text = string.Empty;
            this.GranterUserRequiredField.Enabled = false;
            this.GranterUserTextBox.Enabled = false;
            this.GranterUserTextBox.Text = string.Empty;
            this.PasswordRequiredField.Enabled = false;
        }
    }

    #endregion

    #region Methods

    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.InstanceTextBox.Text = this.InstanceName;
        this.SchemaTextBox.Text = this.SchemaName;
        this.RefreshSchemaTitleLabel.Text = Resources.Resource.RefreshSchema_Label_Text;
        this.SchemaNameTitleLabel.Text = Resources.Resource.Database_Label_Text;
        this.GranterUserLabel.Text = Resources.Resource.GranterUser_Label_Text;
        this.PasswordTitleLabel.Text = Resources.Resource.Password_Label_Text;
        this.RefreshSchemaButton.ButtonText = Resources.Resource.Refresh_Button_Text;
        this.CancelImageButton.ButtonText = Resources.Resource.Cancel_Button_Text;
        this.PasswordRequiredField.ErrorMessage = Resources.Resource.PasswordRequired_Label_Text;
        this.GranterUserRequiredField.ErrorMessage = Resources.Resource.GranterUserRequired_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using Infragistics.WebUI.UltraWebGrid;

public partial class AddSchemas : System.Web.UI.UserControl
{
    #region Variables
    private enum RepeaterControls
    {
        AddSchemaTitleLabel,
        SchemaNameLabel,
        PasswordTextBox,
        PublishSchemaButton,
        SchemaNameTitleLabel,
        PasswordTitleLabel,
        PasswordRequiredFieldValidator,
    }

    // The command event args format is "InstanceName | SchemaName"
    private const string schemaToAddCommandEventArgsFormat = "{0}.{1}";
    private string _schemaToAdd = string.Empty;
    private string _instanceNameOfSchemaToAdd = string.Empty;
    private bool _showError = false;
    private string _errorMessage = string.Empty;
    #endregion

    #region Public Events
    public event CommandEventHandler InstanceSchemaAdded = null;
    public event CommandEventHandler Cancel = null;
    public event EventHandler<EventArgs> ErrorOcurred = null;
    #endregion

    #region Properties
    public string SchemaToAdd
    {
        get { return _schemaToAdd; }
        set { _schemaToAdd = value != null && !string.IsNullOrEmpty(value) ? value : String.Empty; }
    }

    public string InstanceOfSchemaToAdd
    {
        get { return _instanceNameOfSchemaToAdd; }
        set { _instanceNameOfSchemaToAdd = value != null && !string.IsNullOrEmpty(value) ? value : String.Empty; }
    }

    // Combine the instance name and schema name.
    public string InstanceSchemaToAdd
    {
        get { return string.Format(schemaToAddCommandEventArgsFormat, InstanceOfSchemaToAdd, SchemaToAdd); }
    }

    private COEDatabaseBOList Schemas
    {
        get
        {
            return ViewState[Constants.Schemas] == null ? null : (COEDatabaseBOList)ViewState[Constants.Schemas];
        }
        set
        {
            ViewState[Constants.Schemas] = value;
        }
    }

    private IList<string> Instances
    {
        get
        {
            return ViewState[Constants.Instances] == null ? null : (IList<string>)ViewState[Constants.Instances];
        }
        set
        {
            ViewState[Constants.Instances] = value;
        }
    }

    private Dictionary<string, COEDatabaseBOList> InstanceSchemas
    {
        get
        {
            return ViewState[Constants.InstanceSchemas] == null ? new Dictionary<string, COEDatabaseBOList>()
                : (Dictionary<string, COEDatabaseBOList>)ViewState[Constants.InstanceSchemas];
        }
        set
        {
            ViewState[Constants.InstanceSchemas] = value;
        }
    }

    private bool ShowError
    {
        get { return _showError; }
    }

    //This value must be set by the publishing service error message exception.
    public string ErrorMessage
    {
        get { return _errorMessage; }
    }
    #endregion

    #region Page Life Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!Page.IsPostBack)
            SetControlsAttributtes();
        this.Page.Form.DefaultButton = this.PublishSchemaButton.GetButtonUniqueID();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void InstanceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedInstance = this.InstanceDropDownList.SelectedItem.Text;

        try
        {
            this.SchemaDropDownList.Items.Clear();
            this.BindSchema(selectedInstance);
        }
        catch (Csla.DataPortalException cslaEx)
        {
            if (cslaEx.BusinessException != null)
            {
                this._errorMessage = cslaEx.BusinessException.Message;
            }
            else
            {
                this._errorMessage = Resources.Resource.FetchInstanceSchema_Error;
            }

            if (ErrorOcurred != null)
            {
                ErrorOcurred(this, new EventArgs());
            }
        }
        catch (Exception)
        {
            this._errorMessage = Resources.Resource.FetchInstanceSchema_Error;

            if (ErrorOcurred != null)
            {
                ErrorOcurred(this, new EventArgs());
            }
        }
    }
    #endregion

    #region Event Handlers

    protected void PublishSchemaButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        // Only when instance and schema selected.
        if (Page.IsValid
            && this.InstanceDropDownList.SelectedIndex > 0
            && this.SchemaDropDownList.SelectedIndex > 0)
        {
            //First try to publish it, then launch the add schemas event.
            this.InstanceOfSchemaToAdd = this.InstanceDropDownList.SelectedValue;
            this.SchemaToAdd = this.SchemaDropDownList.SelectedValue;
            bool published = this.PublishSchema(this.SchemaToAdd);

            if (published)
            {
                ((Forms_Public_UserControls_ImageButton)sender).CommandArgument = this.InstanceSchemaToAdd;

                if (!string.IsNullOrEmpty(this.InstanceOfSchemaToAdd)
                    && !string.IsNullOrEmpty(this.SchemaToAdd))
                {
                    if (InstanceSchemaAdded != null)
                    {
                        InstanceSchemaAdded(sender, new CommandEventArgs("InstanceSchemaAdded", this.InstanceSchemaToAdd));
                    }
                }
            }
            else
            {
                if (ErrorOcurred != null)
                    ErrorOcurred(this, new EventArgs());
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    
    protected void CancelSchemaButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(string.IsNullOrEmpty(this.SchemaToAdd))
        {
            if(Cancel != null)
            {
                Cancel(sender, new CommandEventArgs("Cancel", this.SchemaToAdd));
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
        this.AddSchemaTitleLabel.Text = Resources.Resource.AddSchema_Label_Text;
        this.SchemaNameTitleLabel.Text = Resources.Resource.Database_Label_Text;
        this.GranterUserLabel.Text = Resources.Resource.GranterUser_Label_Text;
        this.PasswordTitleLabel.Text = Resources.Resource.Password_Label_Text;
        this.PublishSchemaButton.ButtonText = Resources.Resource.Publish_Button_Text;
        this.CancelImageButton.ButtonText = Resources.Resource.Cancel_Button_Text;
        this.PasswordRequiredField.ErrorMessage = Resources.Resource.PasswordRequired_Label_Text;
        this.GranterUserRequiredField.ErrorMessage = Resources.Resource.GranterUserRequired_Label_Text;
        this.SchemaRequiredFieldValidator.ErrorMessage = Resources.Resource.SchemaRequired_LabelText;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private bool PublishSchema(string schemaName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        // UI setting validation
        if (this.InstanceDropDownList.Text == "Select data source" ||
            this.SchemaDropDownList.Text == "Select schema")
        {
            _errorMessage = "Both Instance and Schema must be specified.";
            _showError = true;
            return false;
        }

        bool retVal = false;

        // Get all schemas for the selected instance (data source)
        var schemas = this.InstanceSchemas[this.InstanceDropDownList.Text];

        // database is equal to schema here
        COEDatabaseBO schemaToPublish = schemas.GetDatabase(schemaName);

        if (schemaToPublish != null)
        {
            try
            {
                if (schemaToPublish.IsValid)
                {
                    // Set the IsPublishRelationships property to publish relationships
                    schemaToPublish.IsPublishRelationships = this.PublishRelationshipsCheckBox.Checked;

                    // Execute publish
                    COEDatabaseBO publishedSchema = schemaToPublish.TryPublish(
                        this.AuthorizeCheckBox.Checked, this.GranterUserTextBox.Text, this.PasswordTextBox.Text);

                    // If publish succeed, update sessions, else Keep no change
                    if (publishedSchema == null)
                    {
                        _errorMessage = "The publish function reutrn a NULL.";
                        _showError = true;
                    }
                    else
                    {
                        if (publishedSchema.COEDataView.Tables.Count < 1)
                        {
                            _errorMessage =
                                "No table can be published. Please select Authorize privilege and input proper granter account to grant SELECT privilege to COEUSER.";
                            _showError = true;
                        }
                        else
                        {
                            // Add the schema to session for temp saving, they will be saved on submitting.
                            var schemaOnPublishing =
                                Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>
                                ?? new Dictionary<string, COEDatabaseBO>();

                            var qualifiedSchemaName =
                                Utilities.GetQualifyInstaceSchemaName(
                                    this.InstanceDropDownList.SelectedValue, this.SchemaDropDownList.SelectedValue);

                            if (schemaOnPublishing.ContainsKey(qualifiedSchemaName.ToUpper()))
                            {
                                schemaOnPublishing.Remove(qualifiedSchemaName.ToUpper());
                            }

                            schemaOnPublishing.Add(qualifiedSchemaName.ToUpper(), publishedSchema);
                            Session[Constants.COESchemasOnPublishing] = schemaOnPublishing;

                            // Update schema list to re-bind the UI
                            schemas.Remove(schemaToPublish);
                            schemas.Add(publishedSchema);
                            this.DataBind(schemas);

                            // Everything is OK
                            retVal = true;
                        }
                    }

                    // Remove the local cache of COEConfigurationBO
                    COEConfigurationBO.RemoveLocalCache(
                        "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
                }
                else
                {
                    // For invalid schema to publish
                    _errorMessage = string.Empty;
                    foreach (Csla.Validation.BrokenRule rule in schemaToPublish.BrokenRulesCollection)
                    {
                        _errorMessage += rule.Description + "\n";
                    }
                    _showError = true;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.GetBaseException().Message;
                _showError = true;
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    public void SetInstanceSchemas(IList<string> instances)
    {
        this.Instances = instances;
    }

    public void BindDropDownList()
    {
        this.InstanceDropDownList.Items.Clear();
        this.InstanceDropDownList.Items.Add(new ListItem("Select data source", "-1"));

        this.SchemaDropDownList.Items.Clear();
        this.SchemaDropDownList.Items.Add(new ListItem("Select schema", "-1"));

        this.InstanceDropDownList.DataSource = Instances;
        this.InstanceDropDownList.DataBind();

        this.BindSchema(this.InstanceDropDownList.Items[0].Text);
    }

    public void BindSchema(string instance)
    {
        if (string.IsNullOrEmpty(instance) || instance == "Select data source")
        {
            this.SchemaDropDownList.Items.Clear();
            this.SchemaDropDownList.Items.Add(new ListItem("Select schema", "-1"));

            return;
        }

        if (this.InstanceSchemas != null &&
            !this.InstanceSchemas.ContainsKey(instance))
        {
            // update the DatabaseBOList with new instance and resaved to viewstate.
            var tempInstanceSchemas = this.InstanceSchemas;
            tempInstanceSchemas.Add(instance, COEDatabaseBOList.GetList(false, instance));

            this.InstanceSchemas = tempInstanceSchemas;
        }

        this.DataBind(this.InstanceSchemas[instance]);
    }

    public void DataBind(COEDatabaseBOList unpublishedSchemas)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Schemas = this.RemoveForbiddenSchemas(unpublishedSchemas);
        this.Schemas = this.RemoveSchemasInDataview(unpublishedSchemas);
        this.SchemaDropDownList.Items.Clear();
        this.SchemaDropDownList.Items.Add(new ListItem("Select schema", "-1"));
        this.SchemaDropDownList.DataSource = this.Schemas;
        this.SchemaDropDownList.DataValueField = this.SchemaDropDownList.DataTextField = "Name";
        this.SchemaDropDownList.DataBind();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private COEDatabaseBOList RemoveSchemasInDataview(COEDatabaseBOList unpublishedSchemas)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dvBO = ((Manager.Forms.DataViewManager.ContentArea.AddSchema) this.Page).GetDataViewBO();

        string[] schemasAlreadyInDV = dvBO.DataViewManager.Tables.GetDatabasesInTables();
        if(schemasAlreadyInDV != null && schemasAlreadyInDV.Length > 0)
        {
            foreach(string schema in schemasAlreadyInDV)
            {
                if(!string.IsNullOrEmpty(schema))
                {
                    unpublishedSchemas = this.RemoveSchema(unpublishedSchemas, schema);
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return unpublishedSchemas;
    }

    private COEDatabaseBOList RemoveForbiddenSchemas(COEDatabaseBOList unpublishedSchemas)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string forbiddenSchemas = FrameworkUtils.GetAppConfigSetting("Manager", "DVManager", "UnpublishableSchemas");
        if(!string.IsNullOrEmpty(forbiddenSchemas))
        {
            foreach(string schema in forbiddenSchemas.Split('|'))
            {
                if(!string.IsNullOrEmpty(schema))
                {
                    unpublishedSchemas = this.RemoveSchema(unpublishedSchemas, schema);
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return unpublishedSchemas;
    }

    private COEDatabaseBOList RemoveSchema(COEDatabaseBOList unpublishedSchemas, string schema)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int i = 0;
        schema = schema.Trim();
        while (unpublishedSchemas.Count > i)
        {
            if (GetInstanceSchemaName(unpublishedSchemas[i]) == schema)
                unpublishedSchemas.RemoveAt(i);
            else
                i++;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return unpublishedSchemas;
    }

    private string GetInstanceSchemaName(COEDatabaseBO coeDatabase)
    {
        return Utilities.GetQualifyInstaceSchemaName(coeDatabase.Instance, coeDatabase.Name);
    }

    #endregion
}

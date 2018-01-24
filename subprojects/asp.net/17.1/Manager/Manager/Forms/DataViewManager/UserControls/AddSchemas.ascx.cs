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
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

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
    private string _schemaToAdd = string.Empty;
    private bool _showError = false;
    private string _errorMessage = string.Empty;
    #endregion

    #region Public Events
    public event CommandEventHandler SchemaAdded = null;
    public event CommandEventHandler Cancel = null;
    public event EventHandler<EventArgs> ErrorOcurred = null;
    #endregion

    #region Properties
    public string SchemaToAdd
    {
        get { return _schemaToAdd; }
        set { _schemaToAdd = value != null && !string.IsNullOrEmpty(value) ? value : String.Empty; }
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
        if (!Page.IsPostBack)
            SetControlsAttributtes();
        this.Page.Form.DefaultButton = this.PublishSchemaButton.GetButtonUniqueID();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Event Handlers
    protected void PublishSchemaButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsValid && this.SchemaDropDownList.SelectedIndex > 0)
        {
            //First try to publish it, then lauch the add schemas event.
            this.SchemaToAdd = this.SchemaDropDownList.SelectedValue;
            bool published = this.PublishSchema(this.SchemaDropDownList.SelectedValue);
            if (published)
            {
                ((Forms_Public_UserControls_ImageButton)sender).CommandArgument = this.SchemaToAdd;
                if (!string.IsNullOrEmpty(this.SchemaToAdd))
                {
                    if (SchemaAdded != null)
                    {
                        SchemaAdded(sender, new CommandEventArgs("SchemaAdded", this.SchemaToAdd));
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
        if (string.IsNullOrEmpty(this.SchemaToAdd))
        {
            if (Cancel != null)
            {
                Cancel(sender, new CommandEventArgs("Cancel", this.SchemaToAdd));
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    private void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AddSchemaTitleLabel.Text = Resources.Resource.AddSchema_Label_Text;
        this.SchemaNameTitleLabel.Text = Resources.Resource.Database_Label_Text;
        this.PasswordTitleLabel.Text = Resources.Resource.Password_Label_Text;
        this.PublishSchemaButton.ButtonText = Resources.Resource.Publish_Button_Text;
        this.CancelImageButton.ButtonText = Resources.Resource.Cancel_Button_Text;
        this.PasswordRequiredField.ErrorMessage = Resources.Resource.PasswordRequired_Label_Text;
        this.SchemaRequiredFieldValidator.ErrorMessage = Resources.Resource.SchemaRequired_LabelText;
        this.PublishRelationshipsLabel.Text = Resources.Resource.PublishRelationships_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private bool PublishSchema(string databaseName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool retVal = false;
        if (!string.IsNullOrEmpty(databaseName))
        {
            //Get the password to send as an argument to publish.
            string password = this.PasswordTextBox.Text;
            //Get the check box value for publishing relationships of tables
            bool isPublishRelationships = this.PublishRelationshipsCheckBox.Checked;
            COEDatabaseBO database = this.Schemas.GetDatabase(databaseName);
            COEDatabaseBO publishedDataBase;
            if (database != null)
            {
                try
                {
                    //set the IsPublishRelationships property to publish relationships
                    database.IsPublishRelationships = isPublishRelationships;
                    if (database.IsValid)
                    {
                        publishedDataBase = database.Publish(password);
                        if (publishedDataBase != null) //If it fails, keep it on the list with no changes.
                        {
                            this.Schemas.Remove(database);
                            this.Schemas.Add(publishedDataBase);
                            retVal = true;
                        }
                        this.DataBind(this.Schemas);
                    }
                    else
                    {
                        _errorMessage = string.Empty;
                        foreach (Csla.Validation.BrokenRule rule in database.BrokenRulesCollection)
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
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
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
        COEDataViewBO dvBO = ((Manager.Forms.DataViewManager.ContentArea.AddSchema)this.Page).GetDataViewBO();

        string[] schemasAlreadyInDV = dvBO.DataViewManager.Tables.GetDatabasesInTables();
        if (schemasAlreadyInDV != null && schemasAlreadyInDV.Length > 0)
        {
            foreach (string schema in schemasAlreadyInDV)
            {
                if (!string.IsNullOrEmpty(schema))
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
        if (!string.IsNullOrEmpty(forbiddenSchemas))
        {
            foreach (string schema in forbiddenSchemas.Split('|'))
            {
                if (!string.IsNullOrEmpty(schema))
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
            if (unpublishedSchemas[i].Name == schema)
                unpublishedSchemas.RemoveAt(i);
            else
                i++;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return unpublishedSchemas;
    }
    #endregion
}

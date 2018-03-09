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


public partial class DuplicateTable : System.Web.UI.UserControl
{
    #region Constants
    private const string _defaultAliasWord = "_Alias";
    #endregion

    #region Properties
    private int TableID
    {
        get
        {
            int retVal;
            if (int.TryParse(this.SelectedTableIDHidden.Value, out retVal))
                return retVal;
            else
                return -1;
        }
        set
        {
            this.SelectedTableIDHidden.Value = value.ToString();
        }
    }

    /// <summary>
    /// Current COEDataViewBO
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO) Session[Constants.COEDataViewBO] : null;
        }
    }

    /// <summary>
    /// Client Id of AliasTextBox
    /// </summary>
    public string AliasTextBoxClientId
    {
        get { return this.AliasTextBox.ClientID; }
    }

    /// <summary>
    /// Client Id of NameTextBox
    /// </summary>
    public string NameTextBoxClientId
    {
        get { return this.NameTextBox.ClientID; }
    }

    /// <summary>
    /// Client Id of SelectedTableIDHidden
    /// </summary>
    public string SelectedTableIDHiddenClientID
    {
        get { return this.SelectedTableIDHidden.ClientID; }
    }

    /// <summary>
    /// Client Id of AliasRequiredField
    /// </summary>
    public string AliasRequiredFieldClientID
    {
        get { return this.AliasRequiredField.ClientID; }
    }

    /// <summary>
    /// Client Id of AliasRegExpValidator
    /// </summary>
    public string AliasRegExpValidatorClientID
    {
        get { return this.AliasRegExpValidator.ClientID; }
    }
    
    #endregion

    #region Public Events
    public event CommandEventHandler Duplicate;
    public event CommandEventHandler Cancel;
    #endregion

    #region Page Lyfe Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.DuplicateImageButton.ButtonClicked += new EventHandler<EventArgs>(DuplicateImageButton_ButtonClicked);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Event Handlers
    void DuplicateImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        //Coverity Fixes: CBOE-313 :CID-11773
        COEDataViewBO coeDataViewBO = this.DataViewBO;
        if (coeDataViewBO != null)
        {
            TableBO table = coeDataViewBO.DataViewManager.Tables.GetTable(this.TableID);
            if (table != null)
                table.Alias = this.AliasTextBox.Text;
        }
        if (Duplicate != null)
            Duplicate(this, new CommandEventArgs("Duplicate", this.TableID));

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(Cancel != null)
            Cancel(this, new CommandEventArgs("Cancel", this.TableID));
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AliasLabel.Text = Resources.Resource.Alias_Label_Text;
        this.NameLabel.Text = Resources.Resource.Name_Label_Text;
        this.TitleLabel.Text = Resources.Resource.DuplicateTable_Button_Text;
        this.DuplicateImageButton.ButtonText = Resources.Resource.Add_Button_Text;
        this.CancelImageButton.ButtonText = Resources.Resource.Cancel_Button_Text;

        this.AliasRequiredField.Text = "*";
        this.AliasRegExpValidator.Text = "*";
        this.AliasRequiredField.ToolTip = this.AliasRequiredField.ErrorMessage = Resources.Resource.AliasRequired_Label_Text;
        this.AliasRegExpValidator.ToolTip = this.AliasRegExpValidator.ErrorMessage = Resources.Resource.InvalidAlias_Label_Text;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Public Methods
    public string DataBind(TableBO table)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string script = string.Empty;
        this.TableID = table.ID;
        this.AliasTextBox.Text = table.Alias + _defaultAliasWord;
        this.NameTextBox.Text = table.Name;
        script += "document.getElementById('" + AliasTextBoxClientId + "').value = '" + table.Alias + _defaultAliasWord + "'; ";
        script += "document.getElementById('" + NameTextBoxClientId + "').value = '" + table.Name + "'; ";
        script += "document.getElementById('" + SelectedTableIDHiddenClientID + "').value = '" + table.ID + "'; ";
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return script;
    }
    #endregion
}
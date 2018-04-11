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

public partial class Forms_Public_UserControls_TextBoxWithPopUp : System.Web.UI.UserControl
{
    private string _summaryText = String.Empty;
    private string _fullText = String.Empty;
    private int _numberOFCharsToDisplayInSummary = 25;
    private string _imagesFolder = Utilities.ImagesBaseRelativeFolder();
    private int _defaultMaxLength = 100; //Default Values.
    private string _validationExpression = String.Empty;
    private string _errorMessage = string.Empty;
   
    public string ImagesFolder
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                _imagesFolder = value;
        }
    }

    public string Text
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _fullText = value;
                this.SetControlsAttributes();
            }
        }
        get
        {
            return this.SummaryTextBox.Text.Trim();
        }
    }

    public int NumberCharactersToDisplayInSummary
    {
        set
        {
            if (value > 0)
                _numberOFCharsToDisplayInSummary = value;
        }
    }

    public string SummaryTextCssClass
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.SummaryTextBox.CssClass = value;
        }
    }

    private bool DisplayTooltip
    {
        get { return _fullText.Length > _numberOFCharsToDisplayInSummary; }
    }

    public int MaxLength
    {
        set { this.SummaryTextBox.MaxLength = value; }
        get { return this.SummaryTextBox.MaxLength; }
    }

    public bool ReadOnly
    {
        set { this.SummaryTextBox.ReadOnly = value; }
        get { return this.SummaryTextBox.ReadOnly; }
    }

    public string ValidationExpression
    {
        set { _validationExpression = value; }
    }

    public string ErrorMessage
    {
        set { _errorMessage = value; }
    }

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        this.CheckToCreateValidator();
        
        if(this.DisplayTooltip)
        {
            string key = "Tooltip" + ClientID;
            string script = "YAHOO.namespace(\"coemanager\");";
            script += "YAHOO.coemanager.tooltip" + this.ClientID + " = new YAHOO.widget.Tooltip(\"tooltip" + this.ClientID + "\",{context:\"" + this.TextBoxImage.ClientID + "\", text:\"" + _fullText + "\"});";

            if(!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(Forms_Public_UserControls_TextBoxWithPopUp), key, script, true);
            if(ScriptManager.GetCurrent(this.Page) != null)
            {
                ScriptManager.RegisterClientScriptBlock(this, typeof(Forms_Public_UserControls_TextBoxWithPopUp), key, script, true);
            }
        }
    }

    #endregion

    #region Methods

    private void SetControlsAttributes()
    {
        this.SummaryTextBox.MaxLength = _defaultMaxLength;
        
        if (this.DisplayTooltip)
        {
            this.SummaryTextBox.Text = _fullText.Substring(0, _numberOFCharsToDisplayInSummary);
            this.TextBoxImage.Visible = true;
        }
        else
        {
            this.SummaryTextBox.Text = _fullText;
            this.TextBoxImage.Visible = false;
        }
        if (string.IsNullOrEmpty(this.SummaryTextBox.CssClass))
            this.SummaryTextBox.CssClass = this.ReadOnly ? "TextBoxWithPopUpRO" : "TextBoxWithPopUp";
    }

    private void CheckToCreateValidator()
    {
        if (!string.IsNullOrEmpty(_validationExpression) && !this.ReadOnly)
        {
            RegularExpressionValidator validator = new RegularExpressionValidator();
            validator.ControlToValidate = this.SummaryTextBox.ID;
            validator.ValidationExpression = _validationExpression;
            validator.ErrorMessage = validator.Text = "*";
            if(!string.IsNullOrEmpty(_errorMessage))
                validator.ToolTip = validator.ErrorMessage = _errorMessage;
            validator.CssClass = "TextWithPopUpError";
            this.TextBoxWithPop.Controls.Add(validator);
        }
    }

    #endregion
}

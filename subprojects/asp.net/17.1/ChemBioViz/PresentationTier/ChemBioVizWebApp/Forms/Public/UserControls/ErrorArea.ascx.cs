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

public partial class Forms_UserControls_ErrorArea : System.Web.UI.UserControl
{
    #region Variables

    private string _errorMessage = String.Empty;
    private bool _displayButton = true;

    #endregion

    #region Properties

    /// <summary>
    /// Text to display as an error message
    /// </summary>
    public string Text
    {
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                this.Visible = true;
                this.SetControlsText();
            }
        }
    }

    /// <summary>
    /// Display back/cancel button.
    /// </summary>
    public bool DisplayButton
    {
        set
        {
            if (_displayButton != value)
                _displayButton = value;
        }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            this.SetControlsText();
    }

    #endregion

    #region Methods

    private void SetControlsText()
    {
        this.ErrorMessageLabel.Text = _errorMessage;
        this.GoBackLinkLabel.Visible = _displayButton;
        this.ErrorsWebPanel.Header.Text = Resources.Resource.ErrorsPanel_Title_Text;
    }

    #endregion


}

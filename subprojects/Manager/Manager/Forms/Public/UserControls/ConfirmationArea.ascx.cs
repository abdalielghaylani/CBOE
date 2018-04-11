using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Forms_Public_UserControls_ConfirmationArea : System.Web.UI.UserControl
{
    #region Variables

    string _confirmationMessage = String.Empty;

    #endregion

    #region Properties

    public string Text
    {
        set
        {
            if (_confirmationMessage != value)
            {
                _confirmationMessage = value;
                this.Visible = true;
                this.SetControlText();
            }
        }
    }

    public string ConfirmationMessageLabelClientID
    {
        get { return this.ConfirmationMessageLabel.ClientID; }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //this.SetControlText();
        }
    }

    #endregion

    #region Methods

    private void SetControlText()
    {
        this.ConfirmationMessageLabel.Text = _confirmationMessage;
    }

    #endregion
}

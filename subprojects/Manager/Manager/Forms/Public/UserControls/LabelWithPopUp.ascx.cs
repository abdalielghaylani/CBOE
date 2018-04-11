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

public partial class Forms_Public_UserControls_LabelWithPopUp : System.Web.UI.UserControl
{
    private string _summaryText = String.Empty;
    private string _fullText = String.Empty;
    private int _numberOFCharsToDisplayInSummary = 25;
    private string _moreDetailsText = "...";
    //private string _imagesFolder = Utilities.im;//.ControlImagesFolderFullPath();

    //public string ImagesFolder
    //{
    //    set
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //            _imagesFolder = value;
    //    }
    //}

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
    }

    public int NumberCharactersToDisplayInSummary
    {
        set
        {
            if (value > 0)
                _numberOFCharsToDisplayInSummary = value;
        }
    }

    public string MoreDetailsText
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                _moreDetailsText = value;
        }
    }

    public string MoreDetailsTextCssClass
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.MoreDetailsLabel.CssClass = value;
        }
    }

    public string SummaryTextCssClass
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.SummaryLabel.CssClass = value;
        }
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
        this.MoreDetailsLabel.Attributes.Add("onMouseover", "ddrivetip('" + _fullText + "', 300)");
        this.MoreDetailsLabel.Attributes.Add("onMouseout", "hideddrivetip();");
        base.OnPreRender(e);
    }

    #endregion

    #region Methods

    private void SetControlsAttributes()
    {
        if (_fullText.Length > _numberOFCharsToDisplayInSummary)
        {
            this.SummaryLabel.Text = _fullText.Substring(0, _numberOFCharsToDisplayInSummary);
            this.MoreDetailsLabel.Visible = true;
            this.MoreDetailsLabel.Text = _moreDetailsText;
        }
        else
        {
            this.SummaryLabel.Text = _fullText;
            this.MoreDetailsLabel.Visible = false;
        }

        if (string.IsNullOrEmpty(this.SummaryLabel.CssClass))
            this.SummaryLabel.CssClass = "DarkText";
        if (string.IsNullOrEmpty(this.MoreDetailsLabel.CssClass))
            this.MoreDetailsTextCssClass = "DarkText";

    }


    #endregion

}

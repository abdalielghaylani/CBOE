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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;

public partial class NameDescriptionUC : System.Web.UI.UserControl
{
    #region Variables

    private string _userID = String.Empty;
    private string _isPublic = String.Empty;
    private string _baseTable = String.Empty;
    private readonly string _defaultName = "DataView_";
    private readonly string _defaultUniqueness = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

    #endregion

    #region Properties

    private int BasedOnID
    {
        get
        {
            int retVal = -1;
            if (ViewState[Constants.ID] != null)
                int.TryParse(ViewState[Constants.ID].ToString(), out retVal);
            return retVal;
        }
        set
        {
            ViewState[Constants.ID] = value;
        }

    }

    private string UserName
    {
        get
        {
            if (ViewState[Constants.User_ID] != null)
                return ViewState[Constants.User_ID] as string;
            else
                return String.Empty;
        }
        set
        {
            ViewState[Constants.User_ID] = value;
        }
    }

    private bool IsPublic
    {
        get
        {
            bool retVal = false;
            if (ViewState[Constants.Is_Public] != null)
                bool.TryParse(ViewState[Constants.Is_Public] as string, out retVal);
            return retVal;
        }
        set
        {
            ViewState[Constants.Is_Public] = value;
        }
    }

    private int FormGroup
    {
        get
        {
            int retVal = -1;
            if (ViewState[Constants.FormGroup] != null)
                int.TryParse(ViewState[Constants.FormGroup].ToString(), out retVal);
            return retVal;
        }
        set
        {
            ViewState[Constants.FormGroup] = value;
        }
    }

    private string Database
    {
        get
        {
            if (ViewState[Constants.DataBase] != null)
                return ViewState[Constants.DataBase] as string;
            else
                return String.Empty;
        }
        set
        {
            ViewState[Constants.DataBase] = value;
        }
    }

    private string BaseTable
    {
        get
        {
            if (ViewState[Constants.BaseTable] != null)
                return ViewState[Constants.BaseTable] as string;
            else
                return String.Empty;
        }
        set
        {
            ViewState[Constants.BaseTable] = value;
        }
    }

    public Constants.PageStates Action
    {
        get
        {
            return ViewState[Constants.Action] == null ? Constants.PageStates.Undefined : (Constants.PageStates)Enum.Parse(typeof(Constants.PageStates), ViewState[Constants.Action].ToString());
        }
        set
        {
            ViewState[Constants.Action] = value;
        }
    }

    public bool DataViewCreationFromSchema
    {
        get
        {
            return this.Action == Constants.PageStates.Clone_DV && this.BasedOnID == Constants.MasterSchemaDataViewID ? true : false;
        }
    }

    /// <summary>
    /// Current COEDataViewBO
    /// </summary>
    private COEDataViewBO DataViewBO
    {
        get
        {
            return Session[Constants.COEDataViewBO] != null ? (COEDataViewBO)Session[Constants.COEDataViewBO] : null;
        }
        set
        {
            if (Session[Constants.COEDataViewBO] == null && value != null)
                Session[Constants.COEDataViewBO] = value;
        }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        this.TitleLabel.Visible = !string.IsNullOrEmpty(this.TitleLabel.Text);
        base.OnPreRender(e);
    }
    #endregion

    #region Methods

    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NameLabel.Text = Resources.Resource.Name_Label_Text;
        this.DescriptionLabel.Text = Resources.Resource.Description_Label_Text;
        this.NameRegExpValidator.Text = "*";
        this.NameRequiredField.Text = "*";
        this.UniqueNameValidator.Text = "*";
        this.DescRegExpValidator.Text = "*";
        this.DescriptionRequiredFieldValidator.Text = "*";

        this.NameRegExpValidator.ToolTip = this.NameRegExpValidator.ErrorMessage = Resources.Resource.InvalidName_Label_Text;
        this.NameRequiredField.ToolTip = this.NameRequiredField.ErrorMessage = Resources.Resource.NameRequired_Label_Text;
        this.DescRegExpValidator.ToolTip = this.DescRegExpValidator.ErrorMessage = Resources.Resource.InvalidDescription_Label_Text;
        this.DescriptionRequiredFieldValidator.ToolTip = this.DescriptionRequiredFieldValidator.ErrorMessage = Resources.Resource.DescriptionRequired_Label_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to bind the information with it data source.
    /// </summary>
    /// <param name="dataViewBO">The DVBO to display</param>
    public void DataBind(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (dataViewBO != null && dataViewBO.DataViewManager != null)
        {
            string title = string.Empty;
            this.BasedOnID = dataViewBO.ID;
            if (!DataViewCreationFromSchema)
                title += " " + Resources.Resource.Database_Label_Text + ": " + dataViewBO.DataViewManager.DataBase;
            this.TitleLabel.Text = title;

            if (this.Action == Constants.PageStates.Edit_DV)
                this.NameTextBox.Text = dataViewBO.Name;
            else
                this.NameTextBox.Text = DataViewCreationFromSchema ? _defaultName + _defaultUniqueness : dataViewBO.Name + " " + _defaultUniqueness;

            this.DescriptionTextBox.Text = DataViewCreationFromSchema ? String.Empty : dataViewBO.Description;
            this.BasedOnID = dataViewBO.ID;
            //Not sure that we need the line below
            this.Database = dataViewBO.DataViewManager.DataBase;
            this.DataViewBO = dataViewBO;
        }
        else
        {
            //If null, display an empty control.
            this.TitleLabel.Text = Resources.Resource.DataViewFromSchema_Label_Text;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// UnBinds the info entered in the control. You get a fulfill object
    /// </summary>
    /// <returns>A COEDataViewBO obj with the information entered by the user</returns>
    public COEDataViewBO UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Return a new object with all the values of the all one (exept the ID, name and desc).
        COEDataViewBO retDataview = null;
        switch (this.Action)
        {
            case Constants.PageStates.Clone_DV:
                if (this.BasedOnID > 0)
                {
                    retDataview = COEDataViewBO.Clone(this.BasedOnID,
                                    Utilities.CleanNameString(this.NameTextBox.Text),
                                    Utilities.CleanString(this.DescriptionTextBox.Text),
                                    false);
                    retDataview.UserName = COEUser.Name;
                }
                else
                {
                    COEDataView coedv = new COEDataView();
                    coedv.DataViewHandling = COEDataView.DataViewHandlingOptions.USE_SERVER_DATAVIEW;
                    retDataview = COEDataViewBO.New(Utilities.CleanNameString(this.NameTextBox.Text), Utilities.CleanString(this.DescriptionTextBox.Text), coedv, null, string.Empty);
                    retDataview.IsPublic = true;
                    retDataview.UserName = COEUser.Name;
                }
                break;
            case Constants.PageStates.Edit_DV:

                //Coverity Fixes: CBOE-313 : CID-11774
                COEDataViewBO coeDataViewBO = this.DataViewBO;
                if (coeDataViewBO != null)
                {
                    coeDataViewBO.Name = Utilities.CleanNameString(this.NameTextBox.Text);
                    coeDataViewBO.Description = Utilities.CleanString(this.DescriptionTextBox.Text);
                    retDataview = coeDataViewBO;
                }
                break;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retDataview;
    }

    public void UniqueNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        this.UniqueNameValidator.ErrorMessage = string.Empty;
        COEDataViewBO bo = this.UnBind();
        if (bo != null && !bo.IsValid)     //Coverity Fixes: CBOE-313 : CID-11775
        {
            args.IsValid = false;
            int i = 1;
            foreach (Csla.Validation.BrokenRule rule in bo.BrokenRulesCollection)
            {
                if (i < bo.BrokenRulesCollection.Count)
                    this.UniqueNameValidator.ErrorMessage += string.Format("{0} <br />", rule.Description);
                else
                    this.UniqueNameValidator.ErrorMessage += rule.Description;
                i++;
            }
        }
        else
            args.IsValid = true;
    }

    #endregion
}

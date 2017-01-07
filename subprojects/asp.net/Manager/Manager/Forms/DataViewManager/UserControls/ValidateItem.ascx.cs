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
using Csla.Validation;
using System.Collections.Generic;

public partial class ValidateItem : System.Web.UI.UserControl
{
    #region Variables

    private List<string> _brokenRulesList = new List<string>();
    private bool _isValid = true;
    private Constants.Validate _itemType;
    private enum RepeaterControls
    {
        ErrorImage,
        ErrorMessageLabel,
        ResolveImageButton,
    }

    #endregion

    #region Properties

    public bool IsValid
    {
        get
        {
            return _isValid;
        }
    }

    private COEDataViewBO DataViewBO
    {
        get
        {
            COEDataViewBO coeDataViewBO = (COEDataViewBO)Session[Constants.COEDataViewBO];
            if (coeDataViewBO != null)
                return coeDataViewBO;
            return null;
        }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
    }

    protected override void OnInit(EventArgs e)
    {
        this.ErrorMessagesRepeater.ItemDataBound += new RepeaterItemEventHandler(ErrorMessagesRepeater_ItemDataBound);
        base.OnInit(e);
    }


    public void ValidationImageButton_ButtonClicked(object sender, EventArgs e)
    {
        if (sender is Forms_Public_UserControls_ImageButton)
        {
            string[] arguments = ((Forms_Public_UserControls_ImageButton)sender).CommandArgument.Split(',');
            string validationTypeString = arguments[0];
            if (!string.IsNullOrEmpty(validationTypeString))
            {
                Constants.Validate validationType = (Constants.Validate)Enum.Parse(typeof(Constants.Validate), validationTypeString);
                string urlRedirect = Utilities.GetPageURL(validationType);                
                COEDataViewBO dv = this.DataViewBO;

                //Coverity Fixes : CBOE-313 : CID-11777
                if (dv != null)
                {
                    if ((validationType == Constants.Validate.Relationships && arguments.Length > 1) || (validationType == Constants.Validate.TablesAndFields && !dv.DataViewManager.IsDefaultFieldInTable))
                    {
                        int tableId = ((Manager.Forms.Master.DataViewManager)Page.Master).GetDataViewBO().DataViewManager.Tables[int.Parse(arguments[1]) - 1].ID;
                        
                        Session["FreshTable"] = dv.DataViewManager.Tables.GetTable(tableId).Clone();
                        Session["DummyFreshTable"] = dv.DataViewManager.Tables.GetTable(tableId).Clone();
                        urlRedirect += "?" + Constants.ParamCaller + "=" + tableId + "&schemaSelected=" + dv.DataViewManager.Tables.GetTable(tableId).DataBase;
                    }
                    else if (validationType == Constants.Validate.BaseTable && arguments.Length > 1)
                        urlRedirect = Constants.DvManagerContentAreaFolder + "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + arguments[1] + "&schemaSelected=" + dv.DataViewManager.DataBase;
                    else if (validationType == Constants.Validate.TablesAndFields && arguments.Length > 1)
                        urlRedirect = Constants.DvManagerContentAreaFolder + "EditTableAndFields.aspx?" + Constants.ParamCaller + "=" + arguments[1];
                    Server.Transfer(urlRedirect, false);
                }
            }
        }
    }

    void ErrorMessagesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((Image)e.Item.FindControl(RepeaterControls.ErrorImage.ToString())).ImageUrl = Utilities.ImagesBaseRelativeFolder() + "Invalid.png";
        ((Label)e.Item.FindControl(RepeaterControls.ErrorMessageLabel.ToString())).Text = e.Item.DataItem.ToString();
        ((Forms_Public_UserControls_ImageButton)e.Item.FindControl(RepeaterControls.ResolveImageButton.ToString())).CommandArgument = _itemType.ToString();
        if(_itemType == Constants.Validate.Relationships)
        {
            if(e.Item.DataItem.ToString().LastIndexOf("index ") > 0)
            {
                string tableIndex = e.Item.DataItem.ToString().Substring(e.Item.DataItem.ToString().LastIndexOf("index ") + 6, 3);
                ((Forms_Public_UserControls_ImageButton) e.Item.FindControl(RepeaterControls.ResolveImageButton.ToString())).CommandArgument += "," + tableIndex;
            }
        }
        else if(_itemType == Constants.Validate.BaseTable)
        {
            if(e.Item.DataItem.ToString().LastIndexOf("primary key") > 0)
            {
                //Coverity Fixes: CBOE-313 :CID-11776
                COEDataViewBO coeDataViewBO = this.DataViewBO;
                if (coeDataViewBO != null)
                {
                    string tableIndex = coeDataViewBO.DataViewManager.BaseTableId.ToString();
                    ((Forms_Public_UserControls_ImageButton)e.Item.FindControl(RepeaterControls.ResolveImageButton.ToString())).CommandArgument += "," + tableIndex;
                }
            }
        }
        else if (_itemType == Constants.Validate.TablesAndFields)
        {
            if (e.Item.DataItem.ToString().LastIndexOf("tableId ") > 0)
            {               
                string startStr = e.Item.DataItem.ToString().Substring(e.Item.DataItem.ToString().LastIndexOf("tableId ") + 8);
                string tableIndex = startStr.Substring(0, startStr.IndexOf(" "));
                ((Forms_Public_UserControls_ImageButton)e.Item.FindControl(RepeaterControls.ResolveImageButton.ToString())).CommandArgument += "," + tableIndex;               
            }
        }
    }

    #endregion

    #region Methods

    private void SetControlsAttributes()
    {
        
    }

    public void DataBind(COEDataViewBO dataView, Constants.Validate itemToValidate, bool isValid)
    {
        _itemType = itemToValidate;
        if(!isValid)
        {

            switch(itemToValidate)
            {
                case Constants.Validate.NameAndDescription:
                    try
                    {
                        if(dataView.BrokenRulesCollection.Count > 0) //Just to be sure we have something to display
                        {
                            List<BrokenRule> brokenRuleList = this.GetNameAndDescBrokenRules(this.ConvertToList(dataView.BrokenRulesCollection));
                            if(brokenRuleList.Count > 0)
                                this.InValidObject(brokenRuleList, itemToValidate);
                            else
                                this.ValidObject();
                        }
                        else
                        {
                            if(!dataView.AreNameAndDescriptionValid)
                                this.InValidObject("Invalid Name and/or Description", itemToValidate);
                            else
                                this.ValidObject();
                        }
                    }
                    catch(Exception ex)
                    {
                        this.InValidObject(ex.Message, itemToValidate);
                    }
                    break;
                case Constants.Validate.BaseTable:
                    try
                    {
                        if(dataView.DataViewManager.BrokenRulesCollection.Count > 0) //Just to be sure we have something to display
                        {
                            List<BrokenRule> baseTableBrokenRuleList = this.GetBaseTableBrokenRules(this.ConvertToList(dataView.DataViewManager.BrokenRulesCollection));
                            if(baseTableBrokenRuleList.Count > 0)
                                this.InValidObject(baseTableBrokenRuleList, itemToValidate);
                            else
                                this.ValidObject();
                        }
                        else
                        {
                            string errors = string.Empty;
                            if(!dataView.DataViewManager.IsValidBaseTable(ref errors))
                                this.InValidObject(errors, itemToValidate);
                            else
                                this.ValidObject();
                        }
                    }
                    catch(Exception ex)
                    {
                        this.InValidObject(ex.Message, itemToValidate);
                    }
                    break;
                case Constants.Validate.TablesAndFields:
                    try
                    {
                        if (dataView.DataViewManager.BrokenRulesCollection.Count > 0) //Just to be sure we have something to display
                        {
                            List<BrokenRule> defaultFieldsBrokenRuleList = this.GetTablesAndFieldsBrokenRules(this.ConvertToList(dataView.DataViewManager.BrokenRulesCollection));
                            if (defaultFieldsBrokenRuleList.Count > 0)
                                this.InValidObject(defaultFieldsBrokenRuleList, itemToValidate);                           
                        }
                        else
                        {
                            if (dataView.DataViewManager.Tables.BrokenRules.Count > 0)
                            {
                                this.InValidObject(dataView.DataViewManager.Tables.BrokenRules, itemToValidate);
                                foreach (CustomBrokenRule rule in dataView.DataViewManager.Tables.CustomBrokenRules)
                                    this.InValidObject(rule.DetailedDescription, itemToValidate);
                            }
                            else
                            {
                                if (dataView.DataViewManager.Tables.IsValid)
                                    this.ValidObject();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        this.InValidObject(ex.Message, itemToValidate);
                    }
                    break;
                case Constants.Validate.Relationships:
                    try
                    {
                        List<BrokenRule> relationshipsBrokenRuleList = this.GetRelationshipsBrokenRules(this.ConvertToList(dataView.DataViewManager.BrokenRulesCollection));
                        if(relationshipsBrokenRuleList.Count > 0)
                        {
                            this.InValidObject(relationshipsBrokenRuleList, itemToValidate);
                        }
                        else
                            this.ValidObject();
                    }
                    catch(Exception ex)
                    {
                        this.InValidObject(ex.Message, itemToValidate);
                    }
                    break;
                case Constants.Validate.Security:
                    try
                    {
                        if(!dataView.IsPublic && dataView.COEAccessRights == null)
                            this.InValidObject("Invalid Security Settings", itemToValidate);
                        else if(!dataView.IsPublic && dataView.COEAccessRights.Roles == null && dataView.COEAccessRights.Users == null)
                            this.InValidObject("Invalid Security Settings", itemToValidate);
                        else
                            this.ValidObject();
                    }
                    catch(Exception ex)
                    {
                        this.InValidObject(ex.Message, itemToValidate);
                    }
                    break;
            }
        }
        else
            this.ValidObject();
    }

    private void InValidObject(List<BrokenRule> brokenRules, Constants.Validate itemToValidate)
    {
        this.MarkAsInvalid();
        foreach(BrokenRule rule in brokenRules)
        {
            if(rule.Description.Contains("\n"))
            {
                string[] errors = rule.Description.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string error in errors)
                {
                    _brokenRulesList.Add(error);
                }
            }
            else
                _brokenRulesList.Add(rule.Description);
        }
        //this.ChangeControlsAttributes();
        this.SetControlsAsInvalid(itemToValidate);
    }

    private void SetControlsAsValid()
    {
        this.ValidationImageButton.TypeOfButton = Forms_Public_UserControls_ImageButton.TypeOfButtons.Valid;
    }

    private void SetControlsAsInvalid(Constants.Validate itemType)
    {
        this.ValidationImageButton.TypeOfButton = Forms_Public_UserControls_ImageButton.TypeOfButtons.Invalid;
        //((Forms_Public_UserControls_ImageButton)this.ErrorMessagesRepeater.Items[0].FindControl("ResolveImageButton")).CommandArgument = itemType.ToString();
        //Display error messages.
        this.ErrorMessagesRepeater.DataSource = _brokenRulesList;
        this.ErrorMessagesRepeater.DataBind();
    }

    private List<BrokenRule> ConvertToList(BrokenRulesCollection brokenRules)
    {
        List<BrokenRule> retVal = new List<BrokenRule>();
        foreach (BrokenRule rule in brokenRules)
            retVal.Add(rule);
        return retVal;
    }

    private List<BrokenRule> GetNameAndDescBrokenRules(List<BrokenRule> allRules)
    {
        return allRules.FindAll(delegate(BrokenRule rule) { return rule.Property == "Name" || rule.Property == "Description"; });
    }

    private List<BrokenRule> GetBaseTableBrokenRules(List<BrokenRule> allRules)
    {
        return allRules.FindAll(delegate(BrokenRule rule) { return rule.Property == "BaseTable"; });
    }

    private List<BrokenRule> GetRelationshipsBrokenRules(List<BrokenRule> allRules)
    {
        return allRules.FindAll(delegate(BrokenRule rule) { return rule.Property == "Relationships"; });
    }

    private List<BrokenRule> GetTablesAndFieldsBrokenRules(List<BrokenRule> allRules)
    {
        return allRules.FindAll(delegate(BrokenRule rule) { return rule.Property == "TablesAndFields"; });
    }

    private void InValidObject(string brokenRuleDesc, Constants.Validate itemType)
    {
        this.MarkAsInvalid();
        if(!string.IsNullOrEmpty(brokenRuleDesc))
        {
            if(brokenRuleDesc.Contains("\n"))
            {
                string[] errors = brokenRuleDesc.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string error in errors)
                {
                    _brokenRulesList.Add(error);
                }
            }
            else
                _brokenRulesList.Add(brokenRuleDesc);
        }
        this.SetControlsAsInvalid(itemType);
    }

    private void ValidObject()
    {
        //this.InValidImage.Visible = this.InValidLabel.Visible = this.InValidMoreDetailsImage.Visible = false;
        this.SetControlsAsValid();
        this.MarkAsValid();
    }

    private void MarkAsValid()
    {
        if (!_isValid)
            _isValid = true;
    }

    private void MarkAsInvalid()
    {
        if (_isValid)
            _isValid = false;
    }

    #endregion
}


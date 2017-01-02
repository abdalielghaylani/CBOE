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
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace RegistrationWebApp.Forms.ViewMixture.ContentArea
{
    public partial class MoveDelete : GUIShellPage
    {

        #region Variables

        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        private int _selectedBatchID = int.MinValue;

        //private string _defaultAction = Resources.En_Resources.Move_Item_Text;
        private PageAction _defaultPageAction = PageAction.MOVE;

        private enum PageAction
        {
            MOVE,
            DELETE,
            REG_DELETE,
            REG_DELETE_TEMP,
        }

        #endregion

        #region Properties

        /// <summary>
        /// This is the controlID that you are interested in listen its public events
        /// </summary>
        /// <remarks>This control is a ICOENavigationControl type</remarks>
        protected string AccordionControlToListen
        {
            get { return this._controlToListen; }
            set { this._controlToListen = value; }
        }

        /// <summary>
        /// This is the groupID in the accordion that I'm interested.
        /// </summary>
        /// <remarks>The returned string is the Property Group Key</remarks>
        protected string AccordionGroupToListen
        {
            get { return this._groupToListen; }
            set { this._groupToListen = value; }
        }

        private int SelectedBatch
        {
            get { return _selectedBatchID; }
        }

        private PageAction Action
        {
            get
            {
                return (PageAction)ViewState["Action"];
            }
            set
            {
                ViewState["Action"] = value;
            }
        }

        private string ToDeleteRegistryNumber
        {
            get
            {
                return ViewState["ToDeleteRegNumber"].ToString();
            }
            set
            {
                if (value != string.Empty)
                    ViewState["ToDeleteRegNumber"] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (!Page.IsPostBack)
                {
                    this.ReadParams();
                    this.SetControlsAttributtes();
                }
                //
                this.SetPageTitle();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            this.SetJScriptReference();
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
                if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(MoveDelete), "ShowRowsScript"))
                {
                    string script = @"

                    function ShowRows(enable)
                    {
                        if(enable)
                        {
                            document.getElementById('" + this.ToImage.ClientID + @"').style.display = 'block';
                            document.getElementById('" + this.DestinyRegistryIDTextBox.ClientID + @"').style.display = 'block';
                            document.getElementById('" + this.DestinyRegistryIDLabel.ClientID + @"').style.display = 'block';
                            document.getElementById('" + this.MoveButton.ClientID + @"').style.display = 'block';
                            document.getElementById('" + this.SubmitButton.ClientID + @"').style.display = 'none';                            
                        }
                        else 
                        {
                            document.getElementById('" + this.ToImage.ClientID + @"').style.display = 'none';
                            document.getElementById('" + this.DestinyRegistryIDTextBox.ClientID + @"').style.display = 'none';
                            document.getElementById('" + this.DestinyRegistryIDLabel.ClientID + @"').style.display = 'none';
                            document.getElementById('" + this.MoveButton.ClientID + @"').style.display = 'none';
                            document.getElementById('" + this.SubmitButton.ClientID + @"').style.display = 'block';                         
                        }
                    }";

                    /*this.ActionRadioList.Items[0].Attributes.Add("onclick", "ShowRows(true);");
                    this.ActionRadioList.Items[1].Attributes.Add("onclick", "ShowRows(false);");*/

                    this.Page.ClientScript.RegisterStartupScript(typeof(MoveDelete), "ShowRowsScript", script, true);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Page Methods

        private void DeleteReg()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ToDeleteRegistryIDTextBox.Text))
                {
                    RegistryRecord.DeleteRegistryRecord(this.ToDeleteRegistryIDTextBox.Text);
                    this.ShowConfirmationMessage(Resource.DeletedRegistry_Label_Text);
                    this.AcceptButton.Visible = true;
                    this.CancelButton.Visible = false;
                    //this.ToDeleteRegistryIDTextBox.Text = string.Empty;
                    this.ToDeleteRegistryIDTextBox.Visible = this.ToDeleteRegistryIDLabel.Visible = false;
                    this.DeleteRegButton.Visible = false;
                }
                else
                {
                    _masterPage.DisplayErrorMessage(Resource.InvaidRegistryID_Label_Text, false);
                }
            }
            catch
            {
                _masterPage.DisplayErrorMessage(Resource.InvaidRegistryID_Label_Text, false);
            }
        }


        private void ReadParams()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (this.Request["Action"] != null)
                Action = (PageAction)Enum.Parse(typeof(PageAction), this.Request["Action"].ToUpper());
            else
                this.Action = PageAction.DELETE;

            if (!string.IsNullOrEmpty(this.Request["RegID"]))
                ToDeleteRegistryNumber = this.Request["RegID"];

            if (!string.IsNullOrEmpty(this.Request["BatchID"]))
            {
                int tempBatchId = int.MinValue;
                int.TryParse(this.Request["BatchID"].ToString(), out tempBatchId);
                _selectedBatchID = tempBatchId;
            }

            SetPageBehavior(Action, true);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (Action == PageAction.REG_DELETE)
                this.Page.Title = Resource.DeleteRegistry_Page_Title;
            else
                this.Page.Title = Resource.MoveDeleteBatch_Page_Title;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetPageBehavior(PageAction action, bool clearTextBox)
        {
            if (clearTextBox)
                this.SourceBatchIDTextBox.Text = this.SelectedBatch > int.MinValue ? this.SelectedBatch.ToString() : string.Empty;
            switch (action)
            {
                case PageAction.MOVE:
                    _defaultPageAction = PageAction.MOVE;
                    this.ToImage.Attributes.Add("style", "display:block;");
                    this.DestinyRegistryIDTextBox.Attributes.Add("style", "display:block;");
                    this.DestinyRegistryIDLabel.Attributes.Add("style", "display:block;");
                    this.MoveButton.Attributes.Add("style", "display:block;");
                    this.SubmitButton.Attributes.Add("style", "display:none;");
                    this.ToDeleteRegistry.Attributes.Add("style", "display:none;");
                    this.DeleteRegButton.Attributes.Add("style", "display:none;");
                    break;
                case PageAction.DELETE:
                    _defaultPageAction = PageAction.DELETE;
                    this.ToImage.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDTextBox.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDLabel.Attributes.Add("style", "display:none;");
                    this.MoveButton.Attributes.Add("style", "display:none;");
                    this.SubmitButton.Attributes.Add("style", "display:block;");
                    this.ToDeleteRegistry.Attributes.Add("style", "display:none;");
                    this.DeleteRegButton.Attributes.Add("style", "display:none;");
                    this.PageTitleLabel.Text = Resource.DeleteBatch_Page_Title;
                    break;
                case PageAction.REG_DELETE:
                    _defaultPageAction = PageAction.REG_DELETE;
                    //this.ActionRow.Attributes.Add("style", "display:none;");
                    this.ToImage.Attributes.Add("style", "display:none;");
                    this.FromRow.Attributes.Add("style", "visibility:hidden");
                    this.ToRegistryRow.Attributes.Add("style", "visibility:hidden");
                    this.MoveButton.Attributes.Add("style", "display:none;");
                    this.SubmitButton.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDTextBox.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDLabel.Attributes.Add("style", "display:none;");
                    this.PageTitleLabel.Text = Resource.DeleteRegistryLabel_Text;
                    this.ToDeleteRegistryIDTextBox.Text = ToDeleteRegistryNumber;
                    break;

                case PageAction.REG_DELETE_TEMP:
                    _defaultPageAction = PageAction.REG_DELETE_TEMP;
                    //this.ActionRow.Attributes.Add("style", "display:none;");
                    this.ToImage.Attributes.Add("style", "display:none;");
                    this.FromRow.Attributes.Add("style", "visibility:hidden");
                    this.ToRegistryRow.Attributes.Add("style", "visibility:hidden");
                    this.MoveButton.Attributes.Add("style", "display:none;");
                    this.SubmitButton.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDTextBox.Attributes.Add("style", "display:none;");
                    this.DestinyRegistryIDLabel.Attributes.Add("style", "display:none;");
                    this.PageTitleLabel.Text = Resource.DeleteRegistryLabel_Text;

                    this.ShowConfirmationMessage(Resource.DeletedRegistry_Label_Text);
                    this.AcceptButton.Visible = true;
                    this.CancelButton.Visible = false;
                    //this.ToDeleteRegistryIDTextBox.Text = string.Empty;
                    this.ToDeleteRegistryIDTextBox.Visible = this.ToDeleteRegistryIDLabel.Visible = false;
                    this.DeleteRegButton.Visible = false;

                    break;
            }
        }

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (Action == PageAction.REG_DELETE)
            {
                if (Session[Constants.MultiCompoundObject_Session] != null)
                {
                    RegistryRecord record = (RegistryRecord)Session[Constants.MultiCompoundObject_Session];
                    if (record.ComponentList.Count > 1)
                        this.PageTitleLabel.Text = Resource.DeleteRegistry_Page_Title;
                    else
                        this.PageTitleLabel.Text = Resource.DeleteRegistryButton_Text;
                    record = null;
                }
                this._masterPage.SetDefaultAction(this.DeleteRegButton.UniqueID);
            }
            else
            {

                if (Action == PageAction.MOVE)
                {
                    this.PageTitleLabel.Text = Resource.MoveDeleteBatch_Page_Title;
                    this._masterPage.SetDefaultAction(this.MoveButton.UniqueID);
                }
                else
                {
                    this.PageTitleLabel.Text = Resource.DeleteBatch_Page_Title;
                    this._masterPage.SetDefaultAction(this.SubmitButton.UniqueID);
                }
            }
            //Set Action items
           // this.ActionRadioList.Text = Resource.SelectAction_Label_Text + ":";
            //this.ActionRadioList.Items.Clear();
            if(RegUtilities.GetAllowMoveBatch())
            {
                ListItem moveItem = new ListItem(Resource.Move_Item_Text, Resource.Move_Item_Text.ToUpper(), true);
                moveItem.Attributes.Add("onclick", "ShowRows(true);");
               // this.ActionRadioList.Items.Add(moveItem);
            }
            ListItem removeItem = new ListItem(Resource.Delete_Item_Text, Resource.Delete_Item_Text.ToUpper(), true);
            removeItem.Attributes.Add("onclick", "ShowRows(false);");
            //this.ActionRadioList.Items.Add(removeItem);

            //if (string.IsNullOrEmpty(this.ActionRadioList.SelectedValue))
            //    this.ActionRadioList.SelectedValue = _defaultPageAction.ToString();

            this.SourceBatchIDLabel.Text = Resource.BatchID_Label_Text;
            this.DestinyRegistryIDLabel.Text = Resource.RegistryNumber_Label_Text;
            this.DestinyRegistryIDLabel.ToolTip = Resource.RegistryNumber_Label_ToolTip;
            //this.ActionTitleLabel.Text = Resource.SelectAction_Label_Text;
            this.SubmitButton.Text = Resource.DeleteBatch_Button_Text;
            this.MoveButton.Text = Resource.MoveBatch_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            /*CSBR#-127733.Purpose - To change button text from Accept to Done as a more suitable button caption.
             *Changed by Soorya Anwar on 12-Jul-2010
             */
            this.AcceptButton.Text = Resource.Done_Button_Text; //End of Change for 127733
            this.DeleteRegButton.Text = Resource.Delete_Button_Text;
            this.ToDeleteRegistryIDLabel.Text = Resource.RegistryNumber_Label_Text;

            //string js = "if(Page_IsValid) return confirm('{0}');";
            //this.SubmitButton.OnClientClick = string.Format(js, Resource.ConfirmDeleteBatch_Label_Text);
            //this.MoveButton.OnClientClick = string.Format(js, Resource.ConfirmMoveBatch_Label_Text);
            //this.DeleteRegButton.OnClientClick = string.Format(js, Resource.ConfirmDeleteRegistry_Label_Text);
            this.ToImage.ImageUrl = RegUtilities.ThemesCommonImagesPath + "Arrow_Right_B_h.png";
            this.SubmitButton.OnClientClick = "return ConfirmDeletingBatch();";
            this.MoveButton.OnClientClick = "return ConfirmMovingBatch();";
            this.DeleteRegButton.OnClientClick = "return ConfirmDeleteRegistry();";


            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, this.Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method subscribe to a public method in the customized control 
        /// to handle it here in a defined method.
        /// </summary>
        protected void SubscribeToEventsInAccordionControl()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return;
        }

        /// <summary>
        /// Load the control/s in the right accordion's panel
        /// </summary>
        /// <remarks>You should show as expanded the group that you are interested</remarks> 
        protected void LoadControlInAccordionPanel()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return;
        }


        private void MoveDeleteBatch()
        {
            //Get source RegistryID given the input.
            string batchID = this.SourceBatchIDTextBox.Text;
            //Cast to int
            int tempBatchID = int.MinValue;
            string toRegNum = this.DestinyRegistryIDTextBox.Text;
            RegistryRecord record = (RegistryRecord)Session[Constants.MultiCompoundObject_Session];
            if (!string.IsNullOrEmpty(batchID) && int.TryParse(batchID, out tempBatchID))
            {
                try
                {
                    if (!string.IsNullOrEmpty(toRegNum) && this.Action == PageAction.MOVE)
                    {
                        Batch.MoveBatch(tempBatchID, toRegNum);
                        if (record != null)
                        {
                            if (!string.IsNullOrEmpty(ToDeleteRegistryNumber) && record.RegNumber.RegNum.Trim().ToLower() == ToDeleteRegistryNumber.ToLower())
                                record.BatchList.Remove(record.BatchList.GetBatchById(tempBatchID));
                        }
                        this.ShowConfirmationMessage(Resource.MovedBatch_Label_Text);
                        this.AcceptButton.Visible = true;
                        this.CancelButton.Visible = false;
                        this.SourceBatchIDLabel.Visible = false;
                        this.SourceBatchIDTextBox.Visible = false;
                        this.MoveButton.Visible = false;
                        this.DestinyRegistryIDLabel.Visible = false;
                        this.DestinyRegistryIDTextBox.Visible = false;
                        this.ToImage.Visible = false;
                        this.CleanEnteredValues();
                    }
                    else if (!string.IsNullOrEmpty(tempBatchID.ToString()) && this.Action== PageAction.DELETE)
                    {
                        Batch.DeleteBatch(tempBatchID);
                        if (record != null)
                        {
                            if (!string.IsNullOrEmpty(ToDeleteRegistryNumber) && record.RegNumber.RegNum.Trim().ToLower() == ToDeleteRegistryNumber.ToLower())
                                record.BatchList.Remove(record.BatchList.GetBatchById(tempBatchID));
                        }
                        this.ShowConfirmationMessage(Resource.DeletedBatch_Label_Text);
                        this.AcceptButton.Visible = true;
                        this.CancelButton.Visible = false;
                        this.SourceBatchIDLabel.Visible = false;
                        this.SourceBatchIDTextBox.Visible = false;
                        this.MoveButton.Visible = false;
                        this.SubmitButton.Visible = false;
                        this.DestinyRegistryIDLabel.Visible = false;
                        this.DestinyRegistryIDTextBox.Visible = false;
                        this.ToImage.Visible = false;
                        this.CleanEnteredValues();
                    }
                    else
                    {
                        _masterPage.DisplayErrorMessage(Resource.InvaidRegistryID_Label_Text, false);
                    }
                    
                }
                catch (Exception ex)
                {
                    Exception baseEx = ex.GetBaseException();
                    if (baseEx != null)
                    {
                        if (baseEx.Message.Contains("Source_Destination_Match"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            _masterPage.DisplayErrorMessage(Resource.SourceAndDestinationMatch_ErrorMessage, false);
                        }
                        if (baseEx.Message.Contains("ORA-20030"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            _masterPage.DisplayErrorMessage(Resource.CannotMoveBatch_Label_Text, false);
                        }
                        else if (baseEx.Message.Contains("ORA-20039"))
                        {
                            SetPageBehavior(PageAction.DELETE, false);
                            _masterPage.DisplayErrorMessage(Resource.CannotDeleteBatch_Label_Text, false);
                        }
                        else if (baseEx.Message.Contains("20031"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            _masterPage.DisplayErrorMessage(Resource.RegNumDoesntExist_Label_Text, false);
                        }
                        else if (baseEx.Message.Contains("ORA-20020"))
                        {
                            SetPageBehavior(PageAction.DELETE, false);
                            _masterPage.DisplayErrorMessage(Resource.BatchDoesntExist_Label_Text, false);
                        }
                        else if (baseEx.Message.Contains("ORA-20032"))
                        {
                            SetPageBehavior(PageAction.DELETE, false);
                            _masterPage.DisplayErrorMessage(Resource.RegCompoundsDontMatch, false);
                        }
                        else if (baseEx.Message.Contains("ORA-20037"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            if (record != null)
                            {
                                if (!string.IsNullOrEmpty(ToDeleteRegistryNumber) && record.RegNumber.RegNum.Trim().ToLower() == ToDeleteRegistryNumber.ToLower())
                                    record.BatchList.Remove(record.BatchList.GetBatchById(tempBatchID));
                            }
                            this.AcceptButton.Visible = true;
                            this.CancelButton.Visible = false;
                            ShowConfirmationMessage(Resource.BatchMovedWithWarning_MessageText + Resource.BatchMoved20037Warning_MessageText);
                            this.SourceBatchIDLabel.Visible = false;
                            this.SourceBatchIDTextBox.Visible = false;
                            this.MoveButton.Visible = false;
                            this.DestinyRegistryIDLabel.Visible = false;
                            this.DestinyRegistryIDTextBox.Visible = false;
                            this.ToImage.Visible = false;                        
                            this.CleanEnteredValues();
                        }
                        else if (baseEx.Message.Contains("ORA-20038"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            if (record != null)
                            {
                                if (!string.IsNullOrEmpty(ToDeleteRegistryNumber) && record.RegNumber.RegNum.Trim().ToLower() == ToDeleteRegistryNumber.ToLower())
                                    record.BatchList.Remove(record.BatchList.GetBatchById(tempBatchID));
                            }
                            this.AcceptButton.Visible = true;
                            this.CancelButton.Visible = false;
                            ShowConfirmationMessage(Resource.BatchMovedWithWarning_MessageText + Resource.BatchMoved20038Warning_MessageText);
                            this.SourceBatchIDLabel.Visible = false;
                            this.SourceBatchIDTextBox.Visible = false;
                            this.MoveButton.Visible = false;
                            this.DestinyRegistryIDLabel.Visible = false;
                            this.DestinyRegistryIDTextBox.Visible = false;
                            this.ToImage.Visible = false;
                            this.CleanEnteredValues();
                        }
                        else if (baseEx.Message.Contains("ORA-20035"))
                        {
                            SetPageBehavior(PageAction.MOVE, false);
                            if (record != null)
                            {
                                if (!string.IsNullOrEmpty(ToDeleteRegistryNumber) && record.RegNumber.RegNum.Trim().ToLower() == ToDeleteRegistryNumber.ToLower())
                                    record.BatchList.Remove(record.BatchList.GetBatchById(tempBatchID));
                                
                                if (record.SameBatchesIdentity)
                                    ShowConfirmationMessage(Resource.BatchMovedWithWarning_MessageText + Resource.DifFragmentsList_MessageText);
                                else
                                    this.ShowConfirmationMessage(Resource.MovedBatch_Label_Text);
                            }
                            else
                                ShowConfirmationMessage(Resource.BatchMovedWithWarning_MessageText + Resource.DifFragmentsList_MessageText);

                            this.AcceptButton.Visible = true;
                            this.CancelButton.Visible = false;
                            this.SourceBatchIDLabel.Visible = false;
                            this.SourceBatchIDTextBox.Visible = false;
                            this.MoveButton.Visible = false;
                            this.DestinyRegistryIDLabel.Visible = false;
                            this.DestinyRegistryIDTextBox.Visible = false;
                            this.ToImage.Visible = false;
                            this.CleanEnteredValues();
                        }
                    }
                    else
                    {
                        _masterPage.DisplayErrorMessage(ex.Message, true);
                    }
                }
            }
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.AreaText = messageToDisplay;
            this.MessagesAreaUserControl.Visible = true;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Clear entered values after a submit.
        /// </summary>
        private void CleanEnteredValues()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.SourceBatchIDTextBox.Text = this.DestinyRegistryIDTextBox.Text = string.Empty;
            this.ToDeleteRegistryIDTextBox.Text = string.Empty;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Event Handlers

        protected void DeleteRegButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                //if(Page.IsValid)
                this.DeleteReg();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (Page.IsValid)
                {
                    this.MoveDeleteBatch();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (string.IsNullOrEmpty(ToDeleteRegistryNumber))
                    Server.Transfer("./ViewMixtureSearch.aspx");
                else
                    Server.Transfer("./ViewMixture.aspx?RegisteredObjectId=" + ToDeleteRegistryNumber);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (this.Action == PageAction.REG_DELETE_TEMP)
                    Server.Transfer(Resource.ReviewRegisterSearch_URL + "?Caller=LASTSEARCH");
                else if(this.Action == PageAction.REG_DELETE)
                    Server.Transfer(Resource.ViewMixtureSearch_URL + "?Caller=VM");
                else
                    Server.Transfer(Resource.ViewMixture_URL + "?RegisteredObjectId=" + ToDeleteRegistryNumber);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion


    }
}

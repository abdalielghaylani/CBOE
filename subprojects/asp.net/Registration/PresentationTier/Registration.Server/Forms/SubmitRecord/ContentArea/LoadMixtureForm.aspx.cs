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
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Text;
using Resources;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common;
using PerkinElmer.CBOE.Registration.Client.Forms.Master;
using System.Xml;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Controls.ChemDraw;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using PerkinElmer.CBOE.Registration.Client.Code;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.CBOE.Registration.Client.Forms.SubmitRecord.ContentArea
{
    public partial class LoadMixtureForm : System.Web.UI.Page
    {
        #region Page Variables
        int _formGroup = (int)FormGroups.MultiCompound;      
        #endregion

        #region GUIShell Variables
        RegistrationMaster _masterPage = null;
        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                }
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
            if(Request[PerkinElmer.CBOE.Registration.Client.Constants.FormGroup_UrlParameter] != null)
                int.TryParse(Request[PerkinElmer.CBOE.Registration.Client.Constants.FormGroup_UrlParameter], out _formGroup);

            if(this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster) this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            this.SetJScriptReference();
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	           
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                StringBuilder jscriptText = new StringBuilder();
                string jscriptKey = "ConfirmAllitemsDeletion";
                jscriptText.Append(@"<script language='javascript'>");
                jscriptText.Append(@"function igtbl_chkBoxChange(evnt,gn){");
                jscriptText.Append(@"if(event.propertyName=='checked' && gn=='" + this.SavedCompoundsUltraWebGrid.ClientID.Replace("_", "x") + "' && igtbl_srcElement(evnt).type=='checkbox')");
                jscriptText.Append(@"{");
                jscriptText.Append(@"var currentValue = parseInt(document.getElementById('" + this.SelectedItemsCountHiddenField.ClientID + "').value);");
                jscriptText.Append(@"if(igtbl_srcElement(evnt).checked){;");
                jscriptText.Append(@"document.getElementById('" + this.SelectedItemsCountHiddenField.ClientID + "').value = currentValue + 1; }");
                jscriptText.Append(@"else {");
                jscriptText.Append(@"document.getElementById('" + this.SelectedItemsCountHiddenField.ClientID + "').value = currentValue - 1; }");
                jscriptText.Append(@"}");
                jscriptText.Append("igtbl_chkBoxChangeCustomized(evnt,gn);");
                jscriptText.Append(@"}</script>");

                if (!this.ClientScript.IsClientScriptBlockRegistered(jscriptKey))
                    ClientScript.RegisterStartupScript(this.GetType(), jscriptKey, jscriptText.ToString());

                this.TotalItemsHiddenField.Value = this.SavedCompoundsUltraWebGrid.Rows.Count.ToString();
                this.DeleteButton.OnClientClick = "return ConfirmSelectedItemsDeletion('" + Resource.ConfirmSelectedItemsDeletion_Alert_Text + "','" + this.SelectedItemsCountHiddenField.ClientID + "','" + Resource.NoSelectedItems_Alert_Text + "');";
                this.SelectAllItemsLink.Attributes.Add("onclick", "SelectAllItems('" + this.SavedCompoundsUltraWebGrid.ClientID + "','DeleteColumn','" + this.SelectedItemsCountHiddenField.ClientID + "');");
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
                base.OnPreRenderComplete(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (Page.IsValid)
                {
                    this.DeleteCompound();
                }
                else
                {
                    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.InvalidPage, Resource.InvalidPageDetailed_Label_Text);
                    _masterPage.DisplayErrorMessage(Resource.InvalidPage_Label_Text, false);
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
                Server.Transfer(string.Format("{0}?{1}={2}",Resource.SubmitMixture_URL,PerkinElmer.CBOE.Registration.Client.Constants.RegistryTypeParameter, Request[PerkinElmer.CBOE.Registration.Client.Constants.RegistryTypeParameter]));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        private void DeleteCompound()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            bool haveDeletedItems = false;
            int deletedItems = 0;
            try
            {
                foreach(UltraGridRow currentRow in SavedCompoundsUltraWebGrid.Rows)
                {
                    if(bool.Parse(currentRow.Cells.FromKey(Constants.DeleteColumn).Value.ToString()))
                    {
                        //string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                        COEGenericObjectStorageBO.Delete(int.Parse(currentRow.Cells.FromKey(PerkinElmer.CBOE.Registration.Client.Constants.IdColumn).Value.ToString()));
                        haveDeletedItems = true;
                        deletedItems++;
                    }
                }
                if(haveDeletedItems)
                {
                    if(deletedItems == 1)
                    {
                        if(_formGroup == (int)FormGroups.MultiCompound)
                            this.ShowConfirmationMessage(Resource.DeletedMixture_Label_Text);
                        else
                            this.ShowConfirmationMessage(Resource.DeletedCompound_Label_Text);
                        this.SavedCompoundsUltraWebGrid.DataBind();
                    }
                    else if(deletedItems > 1)
                    {
                        if(_formGroup == (int)FormGroups.MultiCompound)
                            this.ShowConfirmationMessage(Resource.DeletedMixtures_Label_Text);
                        else
                            this.ShowConfirmationMessage(Resource.DeletedCompounds_Label_Text);
                        this.SavedCompoundsUltraWebGrid.DataBind();
                    }
                }
            }
            catch(Exception exception)
            {
                //if(ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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

        protected void SavedCompoundsUltraWebGrid_Click(object sender, ClickEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                UltraGridRow selectedRow=null;
                if (e.Cell != null && e.Cell.Key != "DeleteColumn")
                {
                    selectedRow = e.Cell.Row;
                }
                else if(e.Row != null)
                {
                    selectedRow = e.Row;
                }
                if(selectedRow != null)
                  this.SelectCompoundForm(int.Parse(selectedRow.Cells.FromKey(Constants.IdColumn).Value.ToString()));
            }
            catch(Exception exception)
            {
                //if(ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if(!string.IsNullOrEmpty(((COEGenericObjectStorageBO) e.Data).COEGenericObject))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(((COEGenericObjectStorageBO) e.Data).COEGenericObject);
                XmlNode xmlNode = document.DocumentElement.SelectSingleNode("//StructureAggregation ");

                if(xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
                {
                    Control aggregatedStructure = new CellItem(e.Row.Cells.FromKey("StructureColumn")).FindControl("AggregatedStructureChemDraw");
                    COEChemDrawEmbed chemdrawEmbed = (COEChemDrawEmbed) ((CellItem) ((TemplatedColumn) e.Row.Cells.FromKey("StructureColumn").Column).CellItems[e.Row.Index]).FindControl("AggregatedStructureChemDraw");
                    chemdrawEmbed.InlineData = xmlNode.InnerText;
                }
            }

            foreach(UltraGridCell cell in e.Row.Cells)
            {
                cell.Title = cell.Text;
            }
            if(e.Row.Cells.FromKey("DeleteColumn") != null)
                e.Row.Cells.FromKey("DeleteColumn").Title = Resource.SelectTemplate_Tooltip_Text; 
        }
        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.DeleteButton.Text = Resource.Delete_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.SelectAllItemsLink.InnerText = Resource.SelectAll_Button_Text;
            this.MessagesAreaUserControl.Visible = false;

            if(_formGroup == (int)FormGroups.MultiCompound)
            {
                this.CancelButton.CommandName = "GoToSubmitMixtures";
                DisplayStatusMessage(Resource.ChooseMixtureForm_Label_Text);
                this.PageTitleLabel.Text = Resource.LoadMixtureForm_Page_Title;
            }
            else
            {
                this.PageTitleLabel.Text = Resource.LoadCompoundForm_Page_Title;
                DisplayStatusMessage(Resource.ChooseCompoundForm_Label_Text);
            }
            this.SavedCompoundsLabel.Text = Resource.PreSavedCompounds_Label_Text;
            this.OtherSavedCompoundsLabel.Text = Resource.OtherSavedCompounds_Label_Text;
            /*string structureFormElement = "<?xml version=\"1.0\" encoding=\"utf-8\"?><formElement><Id>StructureColumn</Id><bindingExpression>StructureAggregation</bindingExpression><configInfo><fieldConfig><Height>135px</Height></fieldConfig></configInfo><displayInfo><type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type></displayInfo></formElement>";
            XmlDocument document = new XmlDocument();
            document.LoadXml(structureFormElement);

            COEGridView.FormElementField formElementField = new COEGridView.FormElementField(document.DocumentElement, "Structure", "");

            SavedCompoundsUltraWebGrid.Columns.Add(formElementField);
            */
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Header.Title = this.Page.Title = Resource.LoadCompoundForm_Label_Text;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        //private void ClearErrorMessage()
        //{
        //    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //    this.ErrorMessagesRow.Visible = false;
        //    this.ErrorMessageLabel.Text = string.Empty;
        //    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        //}

        #endregion

        #region Page Methods

        private void DisplayStatusMessage(string text)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SelectCompoundForm(int genericObjectID)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.SubmitMixture_URL, Constants.SavedObjectId_UrlParameter, genericObjectID.ToString(), PerkinElmer.CBOE.Registration.Client.Constants.RegistryTypeParameter, Request[PerkinElmer.CBOE.Registration.Client.Constants.RegistryTypeParameter]), true);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";
            //TODO: lalala
            /*if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, GUIShellTypes.ContentAreaCommonJscriptsPath);*/
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region DataSource Events

        protected void SavedCompoundsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                COEGenericObjectStorageBOList compoundFormList;

                string userName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ?
                            "unknown" : COEUser.Get();
                compoundFormList = COEGenericObjectStorageBOList.GetList(userName, _formGroup, true);

                foreach(COEGenericObjectStorageBO currentCompound in compoundFormList)
                {
                    if(currentCompound.Description.Length > Constants.DescriptionLongSummaryFieldLimit)
                        currentCompound.Description = currentCompound.Description.Substring(0, Constants.DescriptionLongSummaryFieldLimit) + "...";

                    if (currentCompound.Name.Length > Constants.NameLongSummaryFieldLimit)
                        currentCompound.Name = currentCompound.Name.Substring(0, Constants.NameLongSummaryFieldLimit) + "...";
                }

                //If no compound found, do not display the delete button.
                if(compoundFormList.Count == 0)
                    this.DeleteButton.Enabled = false;
                else
                    this.DeleteButton.Enabled = true;

                e.BusinessObject = compoundFormList;
            }
            catch(Exception exception)
            {
                //if(ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void OtherCompoundsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                string userName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ?
                        "unknown" : COEUser.Get();
                COEGenericObjectStorageBOList compoundFormList = COEGenericObjectStorageBOList.GetList(userName, true, _formGroup, true);
                if(compoundFormList.Count == 0 && !this.DeleteButton.Enabled)
                    this.DeleteButton.Enabled = false;

                foreach(COEGenericObjectStorageBO currentCompound in compoundFormList)
                {
                    if (currentCompound.Description.Length > Constants.DescriptionLongSummaryFieldLimit)
                        currentCompound.Description = currentCompound.Description.Substring(0, Constants.DescriptionLongSummaryFieldLimit) + "...";

                    if (currentCompound.Name.Length > Constants.NameLongSummaryFieldLimit)
                        currentCompound.Name = currentCompound.Name.Substring(0, Constants.NameLongSummaryFieldLimit) + "...";
                }
                e.BusinessObject = compoundFormList;
            }
            catch(Exception exception)
            {
                //if(ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        
        #endregion
    }
}

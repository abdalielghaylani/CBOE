using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using Infragistics.WebUI.UltraWebListbar;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.WebUI.UltraWebToolbar;
using CambridgeSoft.COE.Framework.Controls.COETableManager;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.GUIShell
{
    /// <summary>
    /// This will be the base page for all those pages who want to be part of the GUIShell.
    /// </summary>
    public abstract class GUIShellPage : System.Web.UI.Page
    {

        #region Properties

        public List<string> EmbedAppKeys
        {
            get
            {
                if (this.Context.Session != null)
                {

                    if (Session[GUIShellTypes.Embed] == null)
                        Session[GUIShellTypes.Embed] = new List<string>();
                    return (List<string>)Session[GUIShellTypes.Embed];
                }
                return null;
            }
        }

        /// <summary>
        /// Set the Theme defined ir the web.config or other if it was set in previous requests (added to the url) 
        /// </summary>
        public override string StyleSheetTheme
        {
            get
            {
                string retVal = base.StyleSheetTheme;
                EnsureTheming();
                if (this.Context.Session != null)
                {
                    if (Session[GUIShellTypes.Theme] != null)
                        retVal = Session[GUIShellTypes.Theme].ToString();
                }
                return retVal;
            }
            set
            {
                base.StyleSheetTheme = value;
            }
        }

        /// <summary>
        /// Get the images default folder (including the current applied theme)
        /// </summary>
        protected virtual string ThemeImagesFolderPath
        {
            get { return "~/App_Themes/" + this.StyleSheetTheme + "/Images/"; }
        }

        #region Dynamic Footer Settings

        /// <summary>
        /// Check to display or not the default footer.
        /// </summary>
        private bool UseDefaultFooter
        {
            get
            {
                bool retVal = true;
                if (HttpContext.Current.Application[GUIShellTypes.UseDefaultFooter] != null)
                    retVal = ((GUIShellTypes.DefaultFooter)HttpContext.Current.Application[GUIShellTypes.UseDefaultFooter]) == GUIShellTypes.DefaultFooter.Yes ? true : false;
                return retVal;
            }
        }

        /// <summary>
        /// Footer Name to display (according the type)
        /// </summary>
        private string CustomFooterName
        {
            get
            {
                string retVal = String.Empty;
                if (HttpContext.Current.Application[GUIShellTypes.CustomFooterName] != null)
                    retVal = HttpContext.Current.Application[GUIShellTypes.CustomFooterName].ToString();
                return retVal;
            }
        }

        /// <summary>
        /// Type of customized footer control to display
        /// </summary>
        private GUIShellTypes.FooterControlSupportedTypes CustomFooterType
        {
            get
            {
                GUIShellTypes.FooterControlSupportedTypes retVal = GUIShellTypes.FooterControlSupportedTypes.NotSet;
                if (HttpContext.Current.Application[GUIShellTypes.CustomFooterType] != null)
                    retVal = (GUIShellTypes.FooterControlSupportedTypes)HttpContext.Current.Application[GUIShellTypes.CustomFooterType];
                return retVal;
            }
        }

        /// <summary>
        /// Folder where you can get/put custom footer controls.
        /// </summary>
        private string CustomFooterFolder
        {
            get { return GUIShellTypes.CustomFooterFolder; } //TODO: Move this string to a resources/configurable file.
        }

        /// <summary>
        /// List of controls that are being to be disable for the current user in the current App
        /// </summary>
        public COEPageControlSettingsService.ControlList ControlsToDisable
        {
            get
            {
                COEPageControlSettingsService.ControlList retVal = null;
                if (this.Context.Session != null && !string.IsNullOrEmpty(COEAppName.Get()))
                    if (Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] != null)
                        retVal = (COEPageControlSettingsService.ControlList)Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()];
                return retVal;
            }
        }

        #endregion

        #region Dynamic Header Settings

        /// <summary>
        /// Check to display or not the default header.
        /// </summary>
        private bool UseDefaultHeader
        {
            get
            {
                bool retVal = true;
                if (HttpContext.Current.Application[GUIShellTypes.UseDefaultHeader] != null)
                    retVal = ((GUIShellTypes.DefaultHeader)HttpContext.Current.Application[GUIShellTypes.UseDefaultHeader]) == GUIShellTypes.DefaultHeader.Yes ? true : false;
                return retVal;
            }
        }

        /// <summary>
        /// Header name to display (according the type)
        /// </summary>
        private string CustomHeaderName
        {
            get
            {
                string retVal = String.Empty;
                if (HttpContext.Current.Application[GUIShellTypes.CustomHeaderName] != null)
                    retVal = HttpContext.Current.Application[GUIShellTypes.CustomHeaderName].ToString();
                return retVal;
            }
        }

        /// <summary>
        /// Type of customized footer control to display
        /// </summary>
        private GUIShellTypes.HeaderControlSupportedTypes CustomHeaderType
        {
            get
            {
                GUIShellTypes.HeaderControlSupportedTypes retVal = GUIShellTypes.HeaderControlSupportedTypes.NotSet;
                if (HttpContext.Current.Application[GUIShellTypes.CustomHeaderType] != null)
                    retVal = (GUIShellTypes.HeaderControlSupportedTypes)HttpContext.Current.Application[GUIShellTypes.CustomHeaderType];
                return retVal;
            }
        }

        /// <summary>
        /// Folder where you can get/put custom header controls.
        /// </summary>
        private string CustomHeaderFolder
        {
            get { return GUIShellTypes.CustomHeadersFolder; } //TODO: Move this string to a resources/configurable file.
        }

        #endregion

        /// <summary>
        /// Default value, when not overriden by a page is: /COEManager/Forms/Public/ContentArea/Home.aspx.
        /// </summary>
        public virtual string SessionLostRedirectURL
        {
            get
            {
                //return "/COEManager/Forms/Public/ContentArea/Home.aspx";
                return HttpContext.Current.Request.Url.ToString();
            }
        }

        /// <summary>
        /// Gets the file uploaded title.
        /// </summary>
        /// <value>The file upload title.</value>
        public string FileUploadTitle
        {
            get
            {
                return this.GetHiddenControlValue(GUIShellTypes.FileUploadTitleCtrlID);
            }
        }

        /// <summary>
        /// Gets the file upload description.
        /// </summary>
        /// <value>The file upload description.</value>
        public string FileUploadDescription
        {
            get
            {
                return this.GetHiddenControlValue(GUIShellTypes.FileUploadDescriptionCtrlID);
            }
        }

        /// <summary>
        /// Gets the file upload binding expression.
        /// </summary>
        /// <value>The file upload binding expression.</value>
        public string FileUploadBindingExpression
        {
            get
            {
                return this.GetHiddenControlValue(GUIShellTypes.FileUploadBindingExpression);
            }
        }

        /// <summary>
        /// Gets the size of the file upload max.
        /// </summary>
        /// <value>The size of the file upload max.</value>
        public string FileUploadMaxSize
        {
            get { return this.GetHiddenControlValue(GUIShellTypes.FileUploadMaxSize); }
        }

        /// <summary>
        /// Gets the file upload allowed formats.
        /// </summary>
        /// <value>The file upload allowed formats.</value>
        public string FileUploadAllowedFormats
        {
            get { return this.GetHiddenControlValue(GUIShellTypes.FileUploadAllowedFormats); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is able to bind posted files to the server.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is able to bind posted files; otherwise, <c>false</c>.
        /// </value>
        public bool ArePostedFilesToBind
        {
            get { return this.Page.Request.Files.Count > 0 && !string.IsNullOrEmpty(FileUploadBindingExpression); }
        }

        /// <summary>
        /// Determines whether [is A valid size] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [is A valid size] [the specified file]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAValidSize(HttpPostedFile file)
        {
            bool valid = true; //By default we allow everything
            if (!string.IsNullOrEmpty(this.FileUploadMaxSize))
            {
                if (file.ContentLength > Convert.ToUInt32(this.FileUploadMaxSize))
                    valid = false;
            }
            return valid;
        }

        /// <summary>
        /// Determines whether [is A valid type] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [is A valid type] [the specified file]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAValidType(HttpPostedFile file)
        {
            bool valid = true; //By default we allow everything
            if (!string.IsNullOrEmpty(this.FileUploadAllowedFormats))
            {
                if (!this.FileUploadAllowedFormats.Contains(file.ContentType))
                    valid = false;
            }
            return valid;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [file upload enable].
        /// </summary>
        /// <value><c>true</c> if [file upload enable]; otherwise, <c>false</c>.</value>
        public virtual bool FileUploadEnable
        {
            get { return _fileUploadEnable; }
            set { _fileUploadEnable = value; }
        }

        /// <summary>
        /// Gets the hidden control value located in the guishell.
        /// </summary>
        /// <param name="ctrlID">The CTRL ID.</param>
        /// <returns></returns>
        private string GetHiddenControlValue(string ctrlID)
        {
            string retVal = string.Empty;

            if (this.Master == null)
                return retVal;

            if (this.Master.FindControl(GUIShellTypes.MainFormID) != null)
                if (this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(ctrlID) != null)
                    retVal = ((HiddenField)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(ctrlID)).Value;
            return retVal;
        }

        /// <summary>
        /// Binds a posted file given a binding expression.
        /// </summary>
        /// <param name="bindExpression">The bind expression.</param>
        /// <param name="source">The source.</param>
        /// <remarks>This works togheter with Request.Files</remarks>
        public virtual void BindFile(string bindExpression, object source, out string message)
        {
            message = string.Empty;
            if (this.ArePostedFilesToBind)
            {
                int count = this.Request.Files.Count;
                byte[] rawImg = null;
                for (int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrEmpty(this.Request.Files[i].FileName) &&
                        this.Request.Files[i].ContentLength > 0)
                    {
                        if (this.IsAValidSize(this.Request.Files[i]) && this.IsAValidType(this.Request.Files[i]))
                        {
                            rawImg = new byte[this.Request.Files[i].ContentLength];
                            this.Request.Files[i].InputStream.Read(rawImg, 0, this.Request.Files[i].ContentLength);
                            break;
                        }
                        else
                        {
                            message = CambridgeSoft.COE.Framework.Properties.Resources.InvalidFileFormatSize;
                            break;
                        }

                    }
                }
                try
                {
                    if (string.IsNullOrEmpty(message))
                        GUIShellUtilities.BindToObject(bindExpression, ref source, rawImg);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        #endregion

        #region Variables

        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath;
        private bool _fileUploadEnable = false;
        private Panel _validationSummaryPanel = new Panel();
        private ValidationSummary _validationSummary = new ValidationSummary();
        private string _validationSummaryFooterText = string.Empty;
        private string _validationSummaryHeaderText = string.Empty;
        #endregion

        #region Methods to be implemented for pages who inherit this class

        protected abstract void SetControlsAttributtes();

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to put disable some controls according Session var COEPageSettings.
        /// </summary>
        private void SetPageControlsStatus()
        {

            //Coverity Fixes : CBOE-194 : CID-19049
            COEPageControlSettingsService.ControlList ctrlList = this.ControlsToDisable;

            if (ctrlList != null)
            {
                COEPageControlSettingsService.ControlList controlsToDisable = ctrlList.GetByPageID(this.Page.ToString());
                if (controlsToDisable != null)
                {
                    foreach (COEPageControlSettingsService.Control currentControl in controlsToDisable)
                        this.DisableControl(currentControl);
                }
                //TODO:
                //Get all the controls to disable from accordion control
            }
        }

        /// <summary>
        /// Method to find a control inside a ContentPlaceHolder
        /// </summary>
        /// <param name="controlID">The control's ID to search </param>
        /// <returns>The found control</returns>
        private Control FindControlInsidePage(string controlID)
        {
            if (this.Master == null)
                return null;

            return FindControlRecursively((Control)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(GUIShellTypes.ContentPlaceHolderID), controlID);
        }

        private Control FindControlRecursively(Control container, string name)
        {
            //Coverity Bug Fix :- CID : 11851  Jira Id :CBOE-194
            if (container != null)
            {
                if (container.ID == name)
                    return container;

                foreach (Control ctrl in container.Controls)
                {
                    Control foundCtrl = FindControlRecursively(ctrl, name);
                    if (foundCtrl != null)
                        return foundCtrl;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a COEFormGroup inside of a given place holder (part of the page)
        /// </summary>
        /// <param name="placeHolderId"></param>
        /// <returns></returns>
        private COEFormGroup FindFormInsidePlaceHolder(string placeHolderId)
        {
            Control placeHolder = this.FindControlInsidePage(placeHolderId);
            COEFormGroup retVal = null;
            if (placeHolder != null)
            {
                if (placeHolder.HasControls())
                {
                    if (placeHolder.Controls[0] is COEFormGroup)
                    {
                        retVal = (COEFormGroup)placeHolder.Controls[0];
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Method to find a group in a given accordion
        /// </summary>
        /// <param name="accordindID">The identifier of the accordion to search inside</param>
        /// <returns>The list of groups</returns>
        private Groups FindGroupInsideAccordion(string accordindID)
        {
            if (this.Master == null)
                return null;

            return ((UltraWebListbar)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(accordindID)).Groups;
        }

        /// <summary>
        /// Method to verify of the controls exists in the current page.
        /// </summary>
        /// <param name="controlID">The control identifier</param>
        /// <returns>A boolean indicating if the controls was found or not</returns>
        private bool ControlExists(string controlID)
        {
            if (this.Master == null)
                return false;

            Control controlFound = FindControlRecursively((Control)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(GUIShellTypes.ContentPlaceHolderID), controlID);
            if (controlFound != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a given COEFormgenerator exists inside of a COEFormGroup.
        /// </summary>
        /// <param name="controlId">COEFormGen id</param>
        /// <param name="placeHolderId">Holder in which where placed the COEFormGroup</param>
        /// <returns></returns>
        private bool ControlExistsInCOEForm(int formId, string placeHolderId)
        {
            Control placeHolder = this.FindControlInsidePage(placeHolderId);
            bool retVal = false;

            if (placeHolder != null)
            {
                if (placeHolder.HasControls())
                {
                    if (placeHolder.Controls[0] is COEFormGroup)
                        retVal = ((COEFormGroup)placeHolder.Controls[0]).FindControl(formId) != null ? true : false;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Check if the group exists in the given accordion.
        /// </summary>
        /// <param name="accordionID">The identifier ff the accordion to search</param>
        /// <param name="groupID">The identifier of the group</param>
        /// <returns></returns>
        private bool GroupExists(string accordionID, string groupID)
        {
            if (this.Master == null)
                return false;

            return ((UltraWebListbar)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(accordionID)).Groups.Exists(groupID);
        }

        /// <summary>
        /// Method to disable some controls inside the current page (if the control exists).
        /// </summary>
        /// <param name="controlID">Controls to disable</param>
        private void DisableControl(COEPageControlSettingsService.Control control)
        {
            switch (control.TypeOfControl)
            {
                case COEPageControlSettingsService.Control.ControlType.COEForm:
                    this.DisableCOEForm(control);
                    break;
                case COEPageControlSettingsService.Control.ControlType.COEGenerableControl:
                    this.DisableControlInCOEFormGenerator(control, true);
                    break;
                case CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.ControlType.COETableManagerChildTable:
                    this.DisableTableManagerChildTable(control);
                    break;
                default:
                    if (control.COEFormID >= 0)
                        this.DisableControlInCOEFormGenerator(control, false);
                    else if (!string.IsNullOrEmpty(control.PlaceHolderID))
                        this.DisableControlInCOEFormGroup(control);
                    else
                        this.DisableWebControl(control);
                    break;
            }
        }

        //TODO it should be possible to do this method more generic
        private void DisableTableManagerChildTable(COEPageControlSettingsService.Control control)
        {
            if (this.Master == null)
                return;

            COETableManager tableManager = (COETableManager)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(GUIShellTypes.ContentPlaceHolderID).FindControl(control.PlaceHolderID);
            if (tableManager.CurrentTable == control.ID.Replace("_ChildTable", ""))
                tableManager.CurrentShowTableMode = COETableManager.ModeOfShowTable.Single;
        }

        virtual protected void DisableWebControl(COEPageControlSettingsService.Control control)
        {
            // Coverity Fix CID - 11724
            Control controlInsidePage = null;
            if (this.ControlExists(control.ID))
            {
                controlInsidePage = this.FindControlInsidePage(control.ID);
                if (control.Action == CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.Actions.Hide)
                {
                    // Coverity Fix CID - 11724
                    if(controlInsidePage != null)
                        controlInsidePage.Visible = false;
                }
                else
                {                 
                    if (controlInsidePage != null)
                    {
                        if (controlInsidePage is WebControl)
                            ((WebControl)controlInsidePage).Enabled = false;
                        else
                        {
                            PropertyInfo enabledProperty = controlInsidePage.GetType().GetProperty("Enabled");
                            if (enabledProperty != null && enabledProperty.CanWrite)
                            {
                                enabledProperty.SetValue(controlInsidePage, false, null);
                            }
                            else
                            {
                                PropertyInfo disabledProperty = controlInsidePage.GetType().GetProperty("Disabled");
                                if (disabledProperty != null && disabledProperty.CanWrite)
                                {
                                    disabledProperty.SetValue(controlInsidePage, true, null);
                                }
                            }
                        }
                    }
                }
            }
        }


        virtual protected void DisableControlInCOEFormGenerator(CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control, bool normalizeName)
        {
            if (this.ControlExistsInCOEForm(control.COEFormID, control.PlaceHolderID))
            {
                //Control containerFormGenerator = ((COEFormGroup)this.FindControlInsidePage(control.PlaceHolderID).Controls[0]).FindControl(control.COEFormID);
                
                //Coverity Fixes: CBOE-194: CID11722
                Control formControl = this.FindControlInsidePage(control.PlaceHolderID);
                if (formControl != null && formControl.Controls.Count > 0)
                {
                    Control containerFormGenerator = ((COEFormGroup)formControl.Controls[0]).FindControl(control.COEFormID);                  
                    if (containerFormGenerator != null & containerFormGenerator is COEFormGenerator)
                    {
                        if (normalizeName)
                        {
                            if (containerFormGenerator.FindControl(control.ID) != null)
                            {
                                containerFormGenerator.FindControl(control.ID).Visible = false;
                                //Also hide containing div
                                if (containerFormGenerator.FindControl(control.ID).Parent is Panel)
                                {
                                    if (control.Action == COEPageControlSettingsService.Control.Actions.Hide)
                                        ((Panel)containerFormGenerator.FindControl(control.ID).Parent).Visible = false;
                                    else
                                        ((Panel)containerFormGenerator.FindControl(control.ID).Parent).Enabled = false;
                                }
                            }
                        }
                        else
                        {
                            if (control.TypeOfControl != CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.ControlType.COEWebGridColumn && control.TypeOfControl != CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.ControlType.CompositeControl)
                            {
                                if (containerFormGenerator.FindControl(control.ParentControlId) != null)
                                {
                                    ICOEGrid gridView = (ICOEGrid)containerFormGenerator.FindControl(control.ParentControlId);
                                    gridView.SetColumnVisibility(control.ID, false);
                                }


                            }
                            else if (control.TypeOfControl != COEPageControlSettingsService.Control.ControlType.CompositeControl)
                            {
                                if ((containerFormGenerator.Controls.Count > 0) && (containerFormGenerator.Controls[0].FindControl(control.ID) != null))
                                {
                                    if (control.Action == CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control.Actions.Hide)
                                        containerFormGenerator.Controls[0].FindControl(control.ID).Visible = false;
                                    else if (containerFormGenerator.Controls[0].FindControl(control.ID) is WebControl)
                                        ((WebControl)containerFormGenerator.Controls[0].FindControl(control.ID)).Enabled = false;
                                }
                            }
                            else
                            {
                                // If we need to find a control inside a composite control run a recursive search
                                // Coverity Fix CID - 10906 (from local server)
                                if (containerFormGenerator != null && containerFormGenerator.Controls[0].FindControl((((COEFormGenerator)containerFormGenerator).NormalizedFormElementName(control.ParentControlId))) != null)
                                {
                                    this.DisableControlInsideCompositeControl(containerFormGenerator.Controls[0].FindControl((((COEFormGenerator)containerFormGenerator).NormalizedFormElementName(control.ParentControlId))), control.ID, control.Action);
                                }
                            }
                        }
                    }
                }
            }
        }

        virtual protected void DisableControlInsideCompositeControl(Control baseControl, string controlID, COEPageControlSettingsService.Control.Actions action)
        {
            // Check for controlID
            foreach (Control currentControl in baseControl.Controls)
            {
                if (currentControl.FindControl(controlID) != null)
                {
                    if (action == COEPageControlSettingsService.Control.Actions.Hide)
                        currentControl.FindControl(controlID).Visible = false;
                    else if (currentControl.FindControl(controlID) is WebControl)
                        ((WebControl)currentControl.FindControl(controlID)).Enabled = false;
                }
                else
                {
                    if (currentControl.Controls.Count > 0)
                    {
                        this.DisableControlInsideCompositeControl(currentControl, controlID, action);
                    }
                }
            }
        }

        virtual protected void DisableCOEForm(CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control)
        {
            if (this.ControlExistsInCOEForm(Convert.ToInt32(control.ID), control.PlaceHolderID))
            {
                if (control.Action == COEPageControlSettingsService.Control.Actions.Hide)
                {
                    ((COEFormGroup)this.FindFormInsidePlaceHolder(control.PlaceHolderID)).SetFormGeneratorVisibility(Convert.ToInt32(control.ID), false);
                }
                else
                {
                    ((COEFormGroup)this.FindFormInsidePlaceHolder(control.PlaceHolderID)).SetFormGeneratorEnable(Convert.ToInt32(control.ID), false);
                }
            }
        }



        virtual protected void DisableControlInCOEFormGroup(CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control)
        {
            // TODO: Verify if control exists in COEFormGroup First
            if ((this.FindFormInsidePlaceHolder(control.PlaceHolderID) != null) && (this.FindFormInsidePlaceHolder(control.PlaceHolderID).FindControl(control.ID) != null))
            {
                if (control.Action == COEPageControlSettingsService.Control.Actions.Hide)
                    ((COEFormGroup)this.FindFormInsidePlaceHolder(control.PlaceHolderID)).FindControl(control.ID).Visible = false;
                else if (((COEFormGroup)this.FindFormInsidePlaceHolder(control.PlaceHolderID)).FindControl(control.ID) is WebControl)
                    ((WebControl)((COEFormGroup)this.FindFormInsidePlaceHolder(control.PlaceHolderID)).FindControl(control.ID)).Enabled = false;

            }
        }

        /// <summary>
        /// Method to disable a group inside an accordion (if the group exists)
        /// </summary>
        /// <param name="accordionID">The id of the accordion in which the group is inside</param>
        /// <param name="groupID"></param>
        private void DisableGroup(string accordionID, string groupID)
        {
            if (GroupExists(accordionID, groupID))
            {
                //Coverity Fixes: CBOE-194 : CID:11723
                Groups groups = this.FindGroupInsideAccordion(accordionID);
                if (groups != null && groups.Count > 0)
                    groups.Remove(groups.FromKey(groupID));
            }           
        }

        /// <summary>
        /// Method to transfer to the Messages page and display a message according the forbiden action
        /// </summary>
        /// <param name="pageName"></param>
        protected virtual void DisablePage(string pageName, string errorMessage)
        {
            //TODO: This url must come from a Config/App variable, it mustn't be hardcoded.
            string url = "~/Forms/Public/ContentArea/Messages.aspx?MessageCode=" + errorMessage
                        + "&" + GUIShellTypes.RequestVars.DisplayBack.ToString() + "=false";
            Server.Transfer(url);
        }

        /// <summary>
        /// Method to make pages always refresh from Server side (always serve expired pages)
        /// </summary>
        private void SetExpiredPage()
        {
            this.Page.Response.Buffer = true;
            this.Page.Response.ExpiresAbsolute = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            this.Page.Response.Expires = 0;
        }

        /// <summary>
        /// Disables the entire footer user control.
        /// </summary>
        public virtual void DisableFooter()
        {
            if (this.Master == null)
                return;

            Control footerContainer = this.Master.FindControl(GUIShellTypes.FooterContainerID);
            if (footerContainer != null)
                footerContainer.Visible = false;
        }

        /// <summary>
        /// Disables the Entire header User Control.
        /// </summary>
        public virtual void DisableHeader()
        {
            if (this.Master == null)
                return;

            Control headerContainer = this.Master.FindControl(GUIShellTypes.HeaderRowControlId);
            if (headerContainer != null)
                headerContainer.Visible = false;
        }

        /// <summary>
        /// Disables the content of a table inside the Footer 
        /// </summary>
        private void DisableFooterInsideContent()
        {
            if (this.Page.Master == null)
                return;

            Control footerTable = this.Page.Master.FindControl(GUIShellTypes.FooterUserControlID).FindControl(GUIShellTypes.FooterRowControlId);
            if (footerTable != null)
                footerTable.Visible = false;
        }

        /// <summary>
        /// Disables the content of a table inside the header 
        /// </summary>
        private void DisableHeaderInsideContent()
        {
            if (this.Page.Master == null)
                return;

            Control headerTable = this.Page.Master.FindControl(GUIShellTypes.HeaderUserControlID).FindControl(GUIShellTypes.HeaderMainContentTableID);
            if (headerTable != null)
                headerTable.Visible = false;
        }

        /// <summary>
        /// Gets the Footer UserControl
        /// </summary>
        /// <returns></returns>
        private Control GetFooter()
        {
            if (this.Page.Master == null)
                return null;

            return this.Page.Master.FindControl(GUIShellTypes.FooterRowControlId);
        }

        /// <summary>
        /// Gets the Header UserControl
        /// </summary>
        /// <returns></returns>
        private Control GetHeader()
        {
            if (this.Page.Master == null)
                return null;

            return this.Page.Master.FindControl(GUIShellTypes.HeaderRowControlId);
        }

        /// <summary>
        /// Check what footer to display. Disable or disable sections according this.
        /// </summary>
        private void CheckFooterToDisplay()
        {
            if (this.Page.Master == null)
                return;

            if (!this.UseDefaultFooter)
            {
                if (this.Page.Master.FindControl(GUIShellTypes.FooterUserControlID).Visible) //Can be 
                {
                    this.DisableFooterInsideContent();
                    this.LoadCustomFooter();
                }
            }
        }

        /// <summary>
        /// Check which footer to display. Disable or disable sections according this.
        /// </summary>
        private void CheckHeaderToDisplay()
        {
            if (this.Page.Master == null)
                return;

            if (!this.UseDefaultHeader)
            {
                if (this.Page.Master.FindControl(GUIShellTypes.HeaderUserControlID).Visible) //Can be invisible x page, like Login page.
                {
                    this.DisableHeaderInsideContent();
                    this.LoadCustomHeader();
                }
            }
        }

        /// <summary>
        /// Load given footer according the type of it.
        /// </summary>
        /// <param name="writer"></param>
        private void LoadCustomFooter()
        {
            switch (this.CustomFooterType)
            {
                case GUIShellTypes.FooterControlSupportedTypes.Composite:
                    this.RenderCompositeControl("footer");
                    break;
                //Possible further implementations. Not tested!
                //case GUIShellTypes.FooterControlSupportedTypes.Html:
                //    this.RenderHtmlControl(writer);
                //    break;
                //case GUIShellTypes.FooterControlSupportedTypes.Inc:
                //    this.RenderIncludeCode(writer);
                //    break;
                //case GUIShellTypes.FooterControlSupportedTypes.UserControl:
                //    this.RenderUserControl(writer);
                //    break;
            }
        }
        /// <summary>
        /// Load given header according the type of it.
        /// </summary>
        /// <param name="writer"></param>
        private void LoadCustomHeader()
        {
            switch (this.CustomHeaderType)
            {
                case GUIShellTypes.HeaderControlSupportedTypes.Composite:
                    this.RenderCompositeControl("header");
                    break;
                //Possible further implementations. Not tested!
                //case GUIShellTypes.FooterControlSupportedTypes.Html:
                //    this.RenderHtmlControl(writer);
                //    break;
                //case GUIShellTypes.FooterControlSupportedTypes.Inc:
                //    this.RenderIncludeCode(writer);
                //    break;
                //case GUIShellTypes.FooterControlSupportedTypes.UserControl:
                //    this.RenderUserControl(writer);
                //    break;
            }
        }


        /// <summary>
        /// Renders a composite/webcontrol given in a dll.
        /// </summary>
        private void RenderCompositeControl(string type)
        {
            //Load Control
            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(Server.MapPath(this.CustomFooterFolder + this.CustomFooterName));
            //Instantiate control checkinf the current page.
            string controlsPerPage = type == "footer" ? HttpContext.Current.Application[GUIShellTypes.CustomFootersPerPage] as string : HttpContext.Current.Application[GUIShellTypes.CustomHeadersPerPage] as string;
            string assemblyName = this.GetAssemblyToApply(this.Page.ToString(), controlsPerPage);
            if (!string.IsNullOrEmpty(assemblyName))
            {
                Control controlToAdd = (Control)assembly.CreateInstance(assemblyName);
                //Coverity Fix CID 20283 ASV
                if (controlToAdd != null)
                {
                    Control theContainer = null;
                    Control theControl = null;
                    if (type == "header")
                    {
                        theControl = this.GetHeader();
                        if (theControl != null)
                            theContainer = theControl.FindControl(GUIShellTypes.HeaderContainerID);
                    }
                    else if (type == "footer")
                    {
                        theControl = this.GetFooter();
                        if (theControl != null)
                            theContainer = theControl.FindControl(GUIShellTypes.FooterContainerID);
                    }
                    if (theContainer != null)
                        theContainer.Controls.Add(controlToAdd);

                }
                ///code before Coverity Fix
                //if (type == "header")
                //{
                //    Control headerContainer = this.GetHeader().FindControl(GUIShellTypes.HeaderContainerID);
                //    if (controlToAdd != null && headerContainer != null)
                //        headerContainer.Controls.Add(controlToAdd);
                //}
                //else if (type == "footer")
                //{
                //    Control footerContainer = this.GetFooter().FindControl(GUIShellTypes.FooterContainerID);
                //    if (controlToAdd != null && footerContainer != null)
                //        footerContainer.Controls.Add(controlToAdd);
                //}
            }
        }

        /// <summary>
        /// Read an entry in the web.config and returns the footer assebly name to load given the current page.
        /// </summary>
        /// <param name="pageName">Name of the page to look it's assebly name to apply</param>
        /// <returns>The found assemblyName</returns>
        private string GetAssemblyToApply(string pageName, string controlsPageList)
        {
            string retVal = String.Empty;
            if (!string.IsNullOrEmpty(pageName) && !string.IsNullOrEmpty(controlsPageList))
            {
                string keysSplitter = "|";
                string keySplitter = ",";
                if (controlsPageList.Contains(pageName.ToUpper()))
                {
                    int pageNameIndex = controlsPageList.IndexOf(pageName.ToUpper());
                    int delimiterPipe = controlsPageList.IndexOf(keysSplitter, pageNameIndex);
                    if (pageNameIndex > -1 && delimiterPipe > -1)
                    {
                        string key = controlsPageList.Substring(pageNameIndex, delimiterPipe - pageNameIndex);
                        string[] splittedKey = key.Split(keySplitter.ToCharArray());
                        if (splittedKey.Length == 2)
                            retVal = splittedKey[1];
                    }
                }
                else
                {
                    //Apply some default footer or throw an exception.
                }
            }
            return retVal;
        }

        /// <summary>
        /// Checks if a theme is already as session or comming from the url to set it.
        /// </summary>
        private void EnsureTheming()
        {
            if (HttpContext.Current.Session != null)
            {
                //Apply a Skin to the App given a Theme name. What we call skin, in .net language is a Theme. 
                string theme = string.Empty;
                if (HttpContext.Current.Application[GUIShellTypes.Themes] != null) //The App is the only one that knows about the available themes. GUIShell just set it.
                {
                    if (Session[GUIShellTypes.Theme] == null)
                    {
                        if (!string.IsNullOrEmpty(Request[GUIShellTypes.Theme]))
                        {
                            theme = Request[GUIShellTypes.Theme].ToUpper().ToString();
                            if (HttpContext.Current.Application[GUIShellTypes.Themes].ToString().Contains(theme))
                                Session[GUIShellTypes.Theme] = theme;
                        }
                    }
                }
            }
        }

        #endregion

        #region Page Events

        protected override void OnPreInit(EventArgs e)
        {
            //if(Context.Session != null)
            //{
            //    if(Context.Session.IsNewSession)
            //    {
            //        string sCookieHeader = Page.Request.Headers["Cookie"];

            //        if((null != sCookieHeader) && (sCookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
            //        {
            //            if(HttpContext.Current.Session["CslaPrincipal"] == null && (Page.Request.Cookies["DisableInactivity"] == null || string.IsNullOrEmpty(Page.Request.Cookies["DisableInactivity"].Value)))
            //            {
            //                CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
            //                memProvider.LogOut();
            //                HttpCookie disableInactivityCookie = new HttpCookie("DisableInactivity", "true");
            //                disableInactivityCookie.Expires = DateTime.Now.AddMinutes(1);
            //                disableInactivityCookie.Path = "/";
            //                Page.Response.Cookies.Add(disableInactivityCookie);
            //                Page.Response.Redirect("/COEManager/Forms/Public/ContentArea/Login.aspx?Inactivity=true&ReturnURL=" + this.SessionLostRedirectURL);
            //            }
            //        }
            //    }
            //}

            //Coverity Fixes : CBOE-194 : CID-19048
            COEPageControlSettingsService.ControlList ctrlList = this.ControlsToDisable;
            if (ctrlList != null)
            {
                //Check if the user is allowed to see the current page before loading controls, etc.
                if (ctrlList.GetByID(this.Page.ToString()) != null)
                    this.DisablePage(this.Page.ToString(), GUIShellTypes.MessagesCode.NoEnoughPrivileges.ToString());
            }
            //Starts Embeddable Code.
            //Check if the embeddeable Session, App or request is present to don't display header and footer. 
            //bool IsEmbeddable = false;
            string splitter = "|";
            if (this.EmbedAppKeys != null)
            {
                if (this.EmbedAppKeys.Count == 0) //First time somebody is retrieving a page of the App or Session time out.
                {
                    if (HttpContext.Current.Application[GUIShellTypes.Embed] != null)
                    {
                        foreach (string key in HttpContext.Current.Application[GUIShellTypes.Embed].ToString().Split(splitter.ToCharArray()))
                            this.AddEmbedKey(key);
                    }
                }
            }

            //A embed value comming from the URL can overwrite the App value.
            if (!string.IsNullOrEmpty(Request[GUIShellTypes.Embed]))
                this.AddEmbedKey(Request[GUIShellTypes.Embed]);

            if (IsEmbeddable())
            {
                this.DisableFooter();
                this.DisableHeader();
            }
            //Ends Embeddable Code.
            this.EnsureTheming();


            //Set Master Page
            if (!string.IsNullOrEmpty(Request["AppTheme"]))
            {
                switch (Request["AppTheme"])
                {
                    case "Reg":
                        this.Page.MasterPageFile = "~/Forms/Master/Registration.Master";
                        break;
                }
            }

            base.OnPreInit(e);
        }

        /// <summary>
        /// Checks if the current key is embeddable
        /// </summary>
        /// <returns></returns>
        public bool IsEmbeddable()
        {
            bool retVal = false;
            if (this.EmbedAppKeys != null)
            {
                foreach (string key in this.EmbedAppKeys)
                {
                    retVal = HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains(key);
                    if (retVal)
                        break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Add key to the list of embeddables pages/keys
        /// </summary>
        /// <param name="key"></param>
        private void AddEmbedKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (key != GUIShellTypes.Embed_Apps_WildCard)
                {
                    if (!this.EmbedAppKeys.Contains(key.ToUpper()))
                        this.EmbedAppKeys.Add(key.ToUpper());
                }
                else
                {
                    this.EmbedAppKeys.Clear();
                    this.EmbedAppKeys.Add(GUIShellTypes.Embed_Apps_WildCard);
                }
            }
        }

        protected override void CreateChildControls()
        {
            if (this.FileUploadEnable && this.Master != null)
            {
                //Add the required control to sync uploaded files with the page (in case the provides is GUIShellPage)
                if (!ExistsCtrlPlaceHolder(GUIShellTypes.FileUploadTitleCtrlID))
                    this.Master.FindControl(GUIShellTypes.MainFormID).Controls.Add(this.CreateCtrl(GUIShellTypes.FileUploadTitleCtrlID));

                if (!ExistsCtrlPlaceHolder(GUIShellTypes.FileUploadDescriptionCtrlID))
                    this.Master.FindControl(GUIShellTypes.MainFormID).Controls.Add(this.CreateCtrl(GUIShellTypes.FileUploadDescriptionCtrlID));

                if (!ExistsCtrlPlaceHolder(GUIShellTypes.FileUploadBindingExpression))
                    this.Master.FindControl(GUIShellTypes.MainFormID).Controls.Add(this.CreateCtrl(GUIShellTypes.FileUploadBindingExpression));

                if (!ExistsCtrlPlaceHolder(GUIShellTypes.FileUploadAllowedFormats))
                    this.Master.FindControl(GUIShellTypes.MainFormID).Controls.Add(this.CreateCtrl(GUIShellTypes.FileUploadAllowedFormats));

                if (!ExistsCtrlPlaceHolder(GUIShellTypes.FileUploadMaxSize))
                    this.Master.FindControl(GUIShellTypes.MainFormID).Controls.Add(this.CreateCtrl(GUIShellTypes.FileUploadMaxSize));
            }
            if (_validationSummaryHeaderText != string.Empty)
                InsertValidationSummary(this.Form.Controls);
            base.CreateChildControls();
        }

        /// <summary>
        /// Creates the input hidden control.
        /// </summary>
        /// <param name="id">The id of the control.</param>
        /// <returns>An input hidden control</returns>
        private HiddenField CreateCtrl(string id)
        {
            HiddenField ctrl = new HiddenField();
            ctrl.ID = id;
            ctrl.EnableViewState = true;
            return ctrl;
        }

        /// <summary>
        /// Checks if the contorl exists in the default GUIShell content place holder
        /// </summary>
        /// <param name="ctrlID">The control id to check</param>
        /// <returns></returns>
        private bool ExistsCtrlPlaceHolder(string ctrlID)
        {
            if (this.Master == null)
                return false;

            return this.Master.FindControl(GUIShellTypes.MainFormID).FindControl(GUIShellTypes.ContentPlaceHolderID).FindControl(ctrlID) != null;
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Master != null && this.Master.FindControl(GUIShellTypes.MainBodyID) != null) //If the current form has a Body called as we want
            {
                //The line below is to set the jscript to open the App as a desktop App.
                string scriptKey = "CheckToOpenNewWindow";
                if (HttpContext.Current.Application[GUIShellTypes.EnablePopUpForBrowserVersions] != null)
                {
                    if (!HttpContext.Current.Application[GUIShellTypes.EnablePopUpForBrowserVersions].ToString().Contains("-1")) //-1 don't opens a value.
                    {
                        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(scriptKey))
                        {
                            string scriptFormat = "<script type=\"text/javascript\">WindowOnload(function(){{{0}}});</script>";
                            this.Page.ClientScript.RegisterClientScriptBlock(typeof(GUIShellPage),
                                                                    scriptKey, string.Format(scriptFormat,
                                                                                            "CheckToOpenNewWindow('" + HttpContext.Current.Application[GUIShellTypes.EnablePopUpForBrowserVersions] as string + "')"));
                        }
                    }
                }
                //Set all pages as expired to force to refresh from the server.
                this.SetExpiredPage();
            }
            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Check the status of some controls according to the COEPageSettings xml.
            // Moved here to avoid problems when controls were re-created after PreRender ( Fix for CSBR-129880)
            this.SetPageControlsStatus();
            base.Render(writer);
        }

        private void InsertValidationSummary(ControlCollection parent)
        {
            /*
            <div id="panel1">
                <div class="hd">
                    Panel #1 from Markup &mdash; This Panel is Draggable</div>
                <div class="bd">
                    <asp:ValidationSummary ID="lalala" runat="server" HeaderText="Validation Summary"
                        ValidationGroup="*" />
                </div>
                <div class="ft">
                    End of Panel #1</div>
            </div>*/
            _validationSummaryPanel.Controls.Clear();

            parent.Add(_validationSummaryPanel);

            Panel header = new Panel();
            _validationSummaryPanel.Controls.Add(header);

            Literal literal = new Literal();
            literal.Text = _validationSummaryHeaderText;
            header.Controls.Add(literal);
            header.CssClass = "hd";
            header.Style.Add(HtmlTextWriterStyle.FontSize, "100%");

            Panel body = new Panel();
            _validationSummaryPanel.Controls.Add(body);
            body.CssClass = "bd";

            Panel footer = new Panel();
            _validationSummaryPanel.Controls.Add(footer);

            literal = new Literal();
            literal.Text = _validationSummaryFooterText;
            footer.Controls.Add(literal);
            footer.CssClass = "ft";
            footer.Style.Add(HtmlTextWriterStyle.FontSize, "100%");

            body.Controls.Add(_validationSummary);

            if (!Page.ClientScript.IsStartupScriptRegistered("OverwriteValidations"))
            {
                string overrideValidatorJS = @"
        YAHOO.namespace('ChemOfficeEnterprise.ValidationSummary');

		function initChemOfficeEnterpriseValidationSummary() {
			// Instantiate a Panel from markup
			YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1 = new YAHOO.widget.Panel('" + _validationSummaryPanel.ClientID + @"', { width:'500px', visible:false, constraintoviewport:true, modal:true, fixedcenter:true } );
			YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1.render();
            YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1.beforeShowEvent.subscribe(HideChemDraws);
            YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1.beforeHideEvent.subscribe(HideValidationSummary);

            //Show validation summary dialog box if client-side validation is not enabled.
            if(document.getElementById('" + _validationSummary.ClientID + @"').style.display != 'none')
                YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1.show();
		}
        
        //Mozilla does not hides the validator automatically.
        function HideValidationSummary()
        {
            ShowChemDraws();
            document.getElementById('" + _validationSummary.ClientID + @"').style.display = 'none';
        }

        function ValidationSummaryOnSubmitEx(validationGroup)
        {
            original_ValidationSummaryOnSubmit(validationGroup);

            if(document.getElementById('" + _validationSummary.ClientID + @"').style.display != 'none')
                YAHOO.ChemOfficeEnterprise.ValidationSummary.panel1.show();
        }

        YAHOO.util.Event.addListener(window, 'load', initChemOfficeEnterpriseValidationSummary);

        if(typeof(ValidationSummaryOnSubmit) != 'undefined')
        {
            /* Replace ValidationSumaryOnSubmit from asp.net native validation script */
            var original_ValidationSummaryOnSubmit = ValidationSummaryOnSubmit;
            ValidationSummaryOnSubmit = ValidationSummaryOnSubmitEx;
        }
";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "OverwriteValidations", overrideValidatorJS, true);
            }
        }

        /// <summary>
        /// This method happens after Page_Load so we can read values/variables of the current loading page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoadComplete(EventArgs e)
        {
            //Footer/header dynamic loading.
            this.CheckHeaderToDisplay();
            this.CheckFooterToDisplay();
            base.OnLoadComplete(e);
        }

        protected override void InitializeCulture()
        {
            Culture = string.Empty;
            if (this.Page.Session != null)
                if (this.Page.Session["DisplayCulture"] == null || string.IsNullOrEmpty(this.Page.Session["DisplayCulture"].ToString()))
                    this.Page.Session["DisplayCulture"] = GUIShellUtilities.GetCulture();

            base.InitializeCulture();
        }

        protected void AddModalValidationSummary(string headerText, string footerText)
        {
            _validationSummaryPanel = new Panel();
            _validationSummaryPanel.ID = "ValidationSummaryPanel";
            _validationSummary = new ValidationSummary();
            _validationSummary.ID = "ValidationSummary";
            _validationSummary.DisplayMode = ValidationSummaryDisplayMode.BulletList;
            _validationSummaryHeaderText = headerText != string.Empty ? headerText : "Validation Summary";
            _validationSummaryFooterText = footerText;
        }
        #endregion
    }

    /// <summary>
    /// Base page for master pages that are part of a GUIShell based app.
    /// </summary>
    public abstract class GUIShellMaster : System.Web.UI.MasterPage
    {
        #region Variables

        private string _accordionControlID = "UltraWebListbarControl";
        private string _toolbarControlID = "UltraWebToolbarControl";
        protected bool _showLeftPanel = true;
        private bool _enableYUI = true;

        protected StringDictionary _controlsThatShowsProgressControl = new StringDictionary();
        #endregion

        #region Methods to be implemented for master pages who inherit from this class

        public abstract void DisplayErrorMessage(string message, bool showBackLink);      
        public abstract void DisplayErrorMessage(Exception exception, bool showBackLink);
        public abstract void DisplayConfirmationMessage(string message);

        #endregion

        #region Properties

        /// <summary>
        /// Hide/Show left panel (accordion control) 
        /// </summary>
        public bool ShowLeftPanel
        {
            get { return _showLeftPanel; }
            set { _showLeftPanel = value; }
        }

        /// <summary>
        /// Sets all the required css/js for the YUI lib.
        /// </summary>
        public bool EnableYUI
        {
            get { return _enableYUI; }
            set { _enableYUI = value; }
        }

        /// <summary>
        /// Gets the controlID of the main form.
        /// </summary>
        /// <value>The page body.</value>
        private Control PageBody
        {
            get { return this.Page.Form.FindControl(GUIShellTypes.MainFormID); }
        }

        /// <summary>
        /// Gets the accordion control commonly located in the left panel (for some apps).
        /// </summary>
        /// <value>The accordion control.</value>
        public UltraWebListbar AccordionControl
        {
            get { return (UltraWebListbar)this.FindControl("LeftPanelContainer").FindControl(this.AccordionControlID); }
        }

        /// <summary>
        /// Gets the toolbar control.
        /// </summary>
        /// <value>The toolbar.</value>
        public UltraWebToolbar Toolbar
        {
            get { return (UltraWebToolbar)this.Page.Form.FindControl(this.ToolbarControlID); }
        }

        /// <summary>
        /// Gets or sets the accordion control ID (left panel).
        /// </summary>
        /// <value>The accordion control ID.</value>
        public virtual string AccordionControlID
        {
            get { return _accordionControlID; }
            set { _accordionControlID = value; }
        }

        /// <summary>
        /// Gets or sets the toolbar control ID.
        /// </summary>
        /// <value>The toolbar control ID.</value>
        public virtual string ToolbarControlID
        {
            get { return _toolbarControlID; }
            set { _toolbarControlID = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the default control that will be triggered when pressed enter in the page.
        /// </summary>
        /// <param name="controlUniqueId">The control unique id.</param>
        /// <example>Usually this method is associated with a Submit button when you want if users hit enter follow the Submit button workflow</example>
        public void SetDefaultAction(string controlUniqueId)
        {
            this.Page.Form.DefaultButton = controlUniqueId;
        }

        /// <summary>
        /// Sets the default focus on a given control.
        /// </summary>
        /// <param name="controlUniqueId">The control unique id.</param>
        public void SetDefaultFocus(string controlUniqueId)
        {
            this.Page.Form.DefaultFocus = controlUniqueId;
        }
        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        public void SetJScriptReference(string jsKey, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string jScriptKey = string.IsNullOrEmpty(jsKey) ? this.Page.ToString() : jsKey;
                if (!Page.ClientScript.IsClientScriptIncludeRegistered(jsKey))
                    Page.ClientScript.RegisterClientScriptInclude(jScriptKey, this.Page.ResolveUrl(url));
            }
        }

        /// <summary>
        /// Sets the CTRL tool tip text to display when on mouse over the control.
        /// </summary>
        /// <param name="contextCtrlID">The Client control ID of the control that will display the given text</param>
        /// <param name="text">The text to display</param>
        /// <remarks>This call relies on the YAHOO.widget.Tooltip control dependencies - Check YUI dependecies for more details</remarks>
        /// <example>this.Page.Master.SetCtrlToolTip(this.CancelButton.ClientID, "Text to Display on mouse over");</example>
        public void SetCtrlToolTip(string contextCtrlID, string text)
        {
            StringBuilder jsText = new StringBuilder();
            string JSId = contextCtrlID + "CtrlToolTip";
            jsText.Append("<script>");
            jsText.Append("YAHOO.namespace(\"coefw.ttip\");");
            jsText.Append("YAHOO.coefw.ttip." + JSId + " = new YAHOO.widget.Tooltip(\"" + JSId + "\", { effect:{effect:YAHOO.widget.ContainerEffect.FADE, duration:0.5}, constraintoviewport:true, hidedelay:50, showdelay: 400, context:\"" + contextCtrlID + "\", text:\"" + text + "\"});");
            jsText.Append("</script>");
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(JSId))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(GUIShellMaster), JSId, jsText.ToString());
        }

        /// <summary>
        /// Makes the control display a modal progress control.
        /// </summary>
        /// <param name="contextCtrlID">The Client control ID of the control that will display the given text</param>
        /// <param name="text">The text to display. If not set "Searching..." would be used</param>
        /// <param name="imageURL">The image url to display. If not set a progress bar like image would be used.</param>
        /// <remarks>This call relies on the YAHOO.widget.Panel control dependencies - Check YUI dependecies for more details</remarks>
        /// <example>this.Page.Master.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Submitting", "/COECommonResources/SpinningBenzene.gif");</example>
        public void MakeCtrlShowProgressModal(string contextCtrlID, string text, string imageURL)
        {
            this.MakeCtrlShowProgressModal(contextCtrlID, text, imageURL, true);
        }

        /// <summary>
        /// Makes the control display a modal progress control.
        /// </summary>
        /// <param name="contextCtrlID">The Client control ID of the control that will display the given text</param>
        /// <param name="text">The text to display. If not set "Searching..." would be used</param>
        /// <param name="imageURL">The image url to display. If not set a progress bar like image would be used.</param>
        /// <param name="causesValidation">Specifies if validation should be triggered previous to showing the modal.</param>
        /// <remarks>This call relies on the YAHOO.widget.Panel control dependencies - Check YUI dependecies for more details</remarks>
        /// <example>this.Page.Master.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Submitting", "/COECommonResources/SpinningBenzene.gif");</example>
        public void MakeCtrlShowProgressModal(string contextCtrlID, string text, string imageURL, bool causesValidation)
        {
            if (string.IsNullOrEmpty(text))
                text = "Searching...";
            if (string.IsNullOrEmpty(imageURL))
                imageURL = this.Page.ClientScript.GetWebResourceUrl(typeof(GUIShellMaster), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.searching.gif");

            string stringValue = text + ";" + imageURL + ";" + causesValidation;
            if (!_controlsThatShowsProgressControl.ContainsKey(contextCtrlID))
                _controlsThatShowsProgressControl.Add(contextCtrlID, stringValue);
            else
                _controlsThatShowsProgressControl[contextCtrlID] = stringValue;
        }

        /// <summary>
        /// This Method will set all the attributes of the Logo (Header).
        /// </summary>
        /// <param name="COELogoParam">Object with all the info to display in the Logo Section</param>
        public virtual void SetLogoAttributes(COELogo COELogoParam, HtmlTableCell logoCell)
        {
            if (COELogoParam != null)
            {
                logoCell.Attributes.Add("OnClick", "window.location.href=" + "'" + COELogoParam.LogoItem[0].URL + "'");
                logoCell.Attributes.Add("Title", COELogoParam.LogoItem[0].ToolTip);
            }
        }

        /// <summary>
        /// This Method will set all the attributes of the Logo (Header).
        /// </summary>
        /// <param name="COELogoParam">Object with all the info to display in the Logo Section</param>
        public virtual void SetLogoAttributes(COELogo COELogoParam, HtmlGenericControl logoCell)
        {
            if (COELogoParam != null)
            {
                logoCell.Attributes.Add("OnClick", "window.location.href=" + "'" + COELogoParam.LogoItem[0].URL + "'");
                logoCell.Attributes.Add("Title", COELogoParam.LogoItem[0].ToolTip);
            }
        }

        /// <summary>
        /// This method sets all the items for the COEMenu
        /// </summary>
        /// <param name="COEMenuParam">Object with all the info to display in the Menu Section</param>
        public virtual void SetToolBarAttributtes(COEMenu toolBarInfo, UltraWebToolbar toolbar)
        {
            for (int i = 0; i < toolBarInfo.MenuItem.Count; i++)
            {
                if (!Boolean.Parse(toolBarInfo.MenuItem[i].HasChildItems.ToLower().Trim()))
                {
                    TBarButton currentTBarButton = new TBarButton();
                    currentTBarButton.Text = toolBarInfo.MenuItem[i].TitleText;
                    if (!string.IsNullOrEmpty(toolBarInfo.MenuItem[i].URL))
                        currentTBarButton.TargetURL = toolBarInfo.MenuItem[i].URL;
                    else
                    {
                        currentTBarButton.AutoPostBack = true;
                    }
                    //currentTBarButton.AutoPostBack = true;
                    currentTBarButton.Key = toolBarInfo.MenuItem[i].Key;
                    currentTBarButton.TargetFrame = toolBarInfo.MenuItem[i].TargetFrame;
                    currentTBarButton.ToolTip = toolBarInfo.MenuItem[i].ToolTip;
                    toolbar.Items.Add(currentTBarButton);
                }
                else
                {
                    //We need to investigate about TBButtonGroup control for this part of the code.
                }
            }
            toolbar.CausesValidation = false;
        }

        /// <summary>
        /// Sets the menu attributes (text, targeturl, target, etc) given a valid datasource.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="gotoMenu">The goto menu.</param>
        public virtual void SetMenuAttributes(COEMenu menu, UltraWebMenu gotoMenu)
        {
            #region Menu
            //Set Menu info from COEMenuObj
            //Get root node.
            int parentLevel = -1;
            string filterExpresion = "ParentLevel = {0} AND ParentKey = '{1}'";

            DataRow[] rootLevelRow = menu.MenuItem.Select(string.Format(filterExpresion, parentLevel, String.Empty));
            if (rootLevelRow.Length == 1)
            {
                Infragistics.WebUI.UltraWebNavigator.Item rootLevelItem = new Infragistics.WebUI.UltraWebNavigator.Item();
                rootLevelItem.Text = ((COEMenu.MenuItemRow)rootLevelRow[0]).TitleText;
                if (!string.IsNullOrEmpty(((COEMenu.MenuItemRow)rootLevelRow[0]).URL))
                    rootLevelItem.TargetUrl = ((COEMenu.MenuItemRow)rootLevelRow[0]).URL;

                if (parentLevel != 0)
                    parentLevel = 0;
                //First Level nodes
                foreach (DataRow firstLevelRow in menu.MenuItem.Select(string.Format(filterExpresion, parentLevel, ((COEMenu.MenuItemRow)rootLevelRow[0]).Key)))
                {
                    if (parentLevel != 1)
                        parentLevel = 1;
                    Infragistics.WebUI.UltraWebNavigator.Item firstLevelItem = new Infragistics.WebUI.UltraWebNavigator.Item();
                    firstLevelItem.Text = ((COEMenu.MenuItemRow)firstLevelRow).TitleText;
                    if (!string.IsNullOrEmpty(((COEMenu.MenuItemRow)firstLevelRow).URL))
                        firstLevelItem.TargetUrl = ((COEMenu.MenuItemRow)firstLevelRow).URL;
                    if (!string.IsNullOrEmpty(((COEMenu.MenuItemRow)firstLevelRow).TargetFrame))
                        firstLevelItem.TargetFrame = ((COEMenu.MenuItemRow)firstLevelRow).TargetFrame;
                    foreach (DataRow secondLevelRow in menu.MenuItem.Select(string.Format(filterExpresion, parentLevel, ((COEMenu.MenuItemRow)firstLevelRow).Key)))
                    {
                        //Second Level nodes
                        Infragistics.WebUI.UltraWebNavigator.Item secondLevelItem = new Infragistics.WebUI.UltraWebNavigator.Item();
                        secondLevelItem.Text = ((COEMenu.MenuItemRow)secondLevelRow).TitleText;
                        if (!string.IsNullOrEmpty(((COEMenu.MenuItemRow)secondLevelRow).URL))
                            secondLevelItem.TargetUrl = ((COEMenu.MenuItemRow)secondLevelRow).URL;
                        if (!string.IsNullOrEmpty(((COEMenu.MenuItemRow)secondLevelRow).TargetFrame))
                            secondLevelItem.TargetFrame = ((COEMenu.MenuItemRow)secondLevelRow).TargetFrame;
                        firstLevelItem.Items.Add(secondLevelItem);
                    }
                    rootLevelItem.Items.Add(firstLevelItem);
                }
                gotoMenu.Items.Add(rootLevelItem);
            }
            else
            {
                throw new Exception("Invalid information in coeMenuObj. Just one item can be root(in this kind of menu).");
            }
            #endregion
        }

        /// <summary>
        /// This method will help the contents pages to find a Control inside a group of the accordion
        /// </summary>
        /// <param name="groupKey">The group ID</param>
        /// <param name="controlID">The controlID to find inside</param>
        /// <returns>Found control</returns>
        /// <remarks>We use FindControl, but it is extended. It can find control inside others.
        /// For instance, can find a TextBox control inside a UserControl inside a group</remarks>
        public virtual Control GetControlInsideAnAccordionGroup(string groupKey, string controlID)
        {
            return (Control)this.AccordionControl.Groups.FromKey(groupKey).FindControl(controlID);
        }

        /// <summary>
        /// Method to set the default control (when you click enter)
        /// </summary>
        /// <param name="controlId">The Unique controlID</param>
        public virtual void SetDefaultButton(string controlId)
        {
            this.Page.Form.DefaultButton = controlId;
        }

        /// <summary>
        /// Add script call to open the application in a new clean window.
        /// </summary>
        /// <remarks>this MUST be modified for YUI calls in order to avoid overwrite previous onload events </remarks>
        public void SetBodyOnLoadScript()
        {
            ((HtmlGenericControl)this.PageBody).Attributes.Add("onload", "CheckToOpenNewWindow();");
        }

        /// <summary>
        /// Sets the Page Title
        /// </summary>
        /// <param name="pageName">Text to use as PageTitle</param>
        /// <remarks>If the param is empty or null, it try to get it from Configuration (MISC-AppPageTitle)</remarks>
        public virtual void SetPageTitle(string pageName)
        {
            if (string.IsNullOrEmpty(pageName)) //Try to get it from Configuration.
                pageName = GUIShellUtilities.GetDefaultPagesTitle();
            if (!string.IsNullOrEmpty(pageName))
                this.Page.Title = this.Page.Header.Title = pageName;
        }

        /// <summary>
        /// Sets the same page title for all pages.
        /// </summary>
        /// <remarks>It will check if the value of MISC-EnableSamePagesTitle is true before setting the title</remarks>
        public virtual void SetSamePageTitle()
        {
            if (GUIShellUtilities.EnableSamePagesTitle())
                SetPageTitle(string.Empty);
        }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            SetSamePageTitle(); //Set if it's requried to apply same pages title according config settings.
            if (EnableYUI)
            {
                //Set YUI settings
                //string yuiCssFolder = "http" + (Page.Request.ServerVariables["HTTPS"].ToLower() != "off" ? "s" : "") + "://" + Page.Request.ServerVariables["SERVER_NAME"] + "/COECommonResources/YUI/";
                string yuiCssFolder = "/COECommonResources/YUI/";
                string yuiJSFolder = yuiCssFolder;
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("yuiloader-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("yuiloader-min", ResolveClientUrl(yuiJSFolder + "yuiloader-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("dom-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("dom-min", ResolveClientUrl(yuiJSFolder + "dom-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("yahoo-dom-events"))
                    this.Page.ClientScript.RegisterClientScriptInclude("yahoo-dom-events", ResolveClientUrl(yuiJSFolder + "yahoo-dom-event.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("event-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("event-min", ResolveClientUrl(yuiJSFolder + "event-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("dragdrop-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("dragdrop-min", ResolveClientUrl(yuiJSFolder + "dragdrop-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("container-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("container-min", ResolveClientUrl(yuiJSFolder + "container-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("element-beta-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("element-beta-min", ResolveClientUrl(yuiJSFolder + "element-beta-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("button-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("button-min", ResolveClientUrl(yuiJSFolder + "button-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("datasource-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("datasource-min", ResolveClientUrl(yuiJSFolder + "datasource-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("autocomplete-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("autocomplete-min", ResolveClientUrl(yuiJSFolder + "autocomplete-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("yahoo-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("yahoo-min", ResolveClientUrl(yuiJSFolder + "yahoo-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("paginator-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("paginator-min", ResolveClientUrl(yuiJSFolder + "paginator-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("event-delegate-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("event-delegate-min", ResolveClientUrl(yuiJSFolder + "event-delegate-min.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("datatable-min"))
                    this.Page.ClientScript.RegisterClientScriptInclude("datatable-min", ResolveClientUrl(yuiJSFolder + "datatable-min.js"));

                if (ScriptManager.GetCurrent(this.Page) != null)
                {
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "yuiloader-min", ResolveClientUrl(yuiJSFolder + "yuiloader-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "dom-min", ResolveClientUrl(yuiJSFolder + "dom-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "event-min", ResolveClientUrl(yuiJSFolder + "event-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "yahoo-dom-events", ResolveClientUrl(yuiJSFolder + "yahoo-dom-event.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "dragdrop-min", ResolveClientUrl(yuiJSFolder + "dragdrop-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "container-min", ResolveClientUrl(yuiJSFolder + "container-min.js"));

                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "element-beta-min", ResolveClientUrl(yuiJSFolder + "element-beta-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "button-min", ResolveClientUrl(yuiJSFolder + "button-min.js"));

                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "datasource-min", ResolveClientUrl(yuiJSFolder + "datasource-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "autocomplete-min", ResolveClientUrl(yuiJSFolder + "autocomplete-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "yahoo-min", ResolveClientUrl(yuiJSFolder + "yahoo-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "paginator-min", ResolveClientUrl(yuiJSFolder + "paginator-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "event-delegate-min", ResolveClientUrl(yuiJSFolder + "event-delegate-min.js"));
                    ScriptManager.RegisterClientScriptInclude(this.Page, typeof(string), "datatable-min", ResolveClientUrl(yuiJSFolder + "datatable-min.js"));
                }

                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.CONTAINER);
                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.FONTSMIN);
                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.AUTOCOMPLETE);
                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.PAGINATOR);
                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.DATATABLE);

                //Add style to the body to know how to handle the background color change.
                if (this.FindControl(GUIShellTypes.MainBodyID) != null)
                    ((HtmlGenericControl)this.FindControl(GUIShellTypes.MainBodyID)).Attributes.Add("class", "yui-skin-sam");

                AddProgressModalScripts();
            }
            base.OnPreRender(e);
        }

        private void AddProgressModalScripts()
        {
            StringBuilder jsText = new StringBuilder();
            string JSId = "MasterProgressModal";
            jsText.Append("<script laguage=\"javascript\" type=\"text/javascript\">");
            jsText.Append("function HideChemDraws() {");
            jsText.Append("    if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {");
            jsText.Append("        for(i = 0; i < cd_objectArray.length; i++) {");
            jsText.Append("             cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'hidden';");
            jsText.Append("        }");
            jsText.Append("    }");
            jsText.Append("}");
            jsText.Append("function ShowChemDraws() {");
            jsText.Append("     if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {");
            jsText.Append("         for(i = 0; i < cd_objectArray.length; i++) {");
            jsText.Append("             cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'visible';");
            jsText.Append("         }");
            jsText.Append("     }");
            jsText.Append("}");
            jsText.Append("YAHOO.namespace(\"ChemOfficeEnterprise.ProgressModal\");");
            jsText.Append("function initProgressModal() {");
            jsText.Append("YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + " = new YAHOO.widget.Panel(\"" + JSId + "\", { width:\"240px\",fixedcenter:false,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true,zIndex:17000});");
            jsText.Append("YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + ".setBody(\"<img id='MasterProgressModalImage' alt='Searching...' src='/COECommonResources/blank.gif' /> \" );");
            jsText.Append("YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + ".setHeader(\"<span id='MasterProgressModalSpan' style='font-size:10px;'>Searching...</span>\");");
            jsText.Append("YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + ".render(document.body);");
            jsText.Append("YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + ".beforeShowEvent.subscribe(HideChemDraws)");
            string[] array = new string[_controlsThatShowsProgressControl.Keys.Count];
            _controlsThatShowsProgressControl.Keys.CopyTo(array, 0);
            foreach (string controlID in array)
            {
                WebControl ctrl = this.GetWebControlFromClientID(controlID);
                if (ctrl != null)
                {
                    if (ctrl.Attributes["onclick"] == null)
                        ctrl.Attributes["onclick"] = string.Empty;
                    this.RemovePreviousModalCallFromControl(ctrl);
                    this.AddModalCallToControl(ctrl, controlID, JSId);
                }
            }
            jsText.Append("}");
            jsText.Append("YAHOO.util.Event.addListener(window, \"load\", initProgressModal);");
            jsText.Append("</script>");

            if (!this.Page.ClientScript.IsStartupScriptRegistered(JSId))
                this.Page.ClientScript.RegisterStartupScript(typeof(GUIShellMaster), JSId, jsText.ToString());
        }

        private void AddModalCallToControl(WebControl ctrl, string controlID, string JSId)
        {
            string[] textAndImgAndValidation = _controlsThatShowsProgressControl[controlID].Split(';');
            string modalCall = "document.getElementById('MasterProgressModalSpan').innerHTML = document.getElementById('MasterProgressModalImage').title = document.getElementById('MasterProgressModalImage').alt = '" + textAndImgAndValidation[0] + "'; document.getElementById('MasterProgressModalImage').src = '" + textAndImgAndValidation[1] + "'; YAHOO.ChemOfficeEnterprise.ProgressModal." + JSId + ".show();";
            if (bool.Parse(textAndImgAndValidation[2]))
            {
                modalCall = "if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate(); if(typeof(Page_IsValid) == 'undefined' || Page_IsValid) { " + modalCall + " }";
            }
            ctrl.Attributes["onclick"] = ctrl.Attributes["onclick"].Insert(0, modalCall);
        }

        private void RemovePreviousModalCallFromControl(WebControl ctrl)
        {
            if (ctrl.Attributes["onclick"].LastIndexOf("YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.show(); }") > 0)
                ctrl.Attributes["onclick"] = ctrl.Attributes["onclick"].Remove(0, ctrl.Attributes["onclick"].LastIndexOf("YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.show(); }") + 70);
            else if (ctrl.Attributes["onclick"].LastIndexOf("YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.show();") > 0)
                ctrl.Attributes["onclick"] = ctrl.Attributes["onclick"].Remove(0, ctrl.Attributes["onclick"].LastIndexOf("YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.show();") + 68);
        }

        private WebControl GetWebControlFromClientID(string controlID)
        {
            string[] containers = controlID.Split('_');
            Control control = this;
            foreach (string id in containers)
            {
                control = control.FindControl(id);
                if (control == null)
                    break;
            }
            return control as WebControl;
        }
    }

    /// <summary>
    /// Global.asax base file to be used in GUIShell Apps
    /// </summary>
    public interface IGUIShellGlobal
    {
        #region Methods to be implemented for classes who inherit this class

        void Application_Start_Code();
        void Application_End_Code();
        void Application_Error_Code();
        void Application_AuthenticateRequest_Code(HttpRequest request, HttpResponse response, HttpContext context);
        void Session_Start_Code();
        void Session_End_Code();

        #endregion

    }
}






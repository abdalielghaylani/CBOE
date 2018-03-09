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
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Infragistics.WebUI.UltraWebNavigator;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebToolbar;

public partial class Forms_Master_MasterPage : GUIShellMaster
{
    #region Properties

    public COEMenu COEMenuItems
    {
        get { return COEMenuObject; }
    }

    

    #endregion

    #region Variables

    COELogo COELogoObject;
    COEMenu COEMenuObject;
    COEMenu COEToolbarObject;
    COEMenu COEFooterObject;

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected override void OnInit(EventArgs e)
    {
        if (Request != null && Request.Browser != null && (Request.Browser.Browser.ToLower() == ("mozilla") || ((Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer") && Request.Browser.MajorVersion >= 10)))
            Page.ClientTarget = "iecustom";
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        _showLeftPanel = false;
        this.SetJScriptReference();
        if (!Page.IsPostBack)
        {
            this.LoadInformation();
            this.SetHeaderAttributes();
            //this.SetFooterAttributes();
        }
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!_showLeftPanel)
            this.LeftPanelCell.Visible = _showLeftPanel;
        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private bool IsEmbeddable()
    {
        bool retVal = false;
        if(Session != null)
        {
            foreach(string key in (List<string>) Session[GUIShellTypes.Embed])
            {
                retVal = HttpContext.Current.Request.Url.ToString().Contains(key);
                if(retVal)
                    break;
            }
        }
        return retVal;
    }

    /// <summary>
    /// Set the reference to the JScript Page.
    /// </summary>
    private void SetJScriptReference()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string jScriptKey = "COEManagerJs";
        if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
            Page.ClientScript.RegisterClientScriptInclude(jScriptKey, this.Page.ResolveUrl(Constants.PublicCommonJScriptsPath));
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// This Method is just for testing purpose.
    /// In the future, all the info about the App will come (for example) from a Business Object 
    /// calling the Configuration Services.
    /// This Method loads all the information into the typed DataSets.
    /// </summary>
    private void LoadInformation()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        #region CoeLogo Info SetUp

        COELogoObject = new COELogo();
        COELogo.LogoItemRow logoRow = COELogoObject.LogoItem.NewLogoItemRow();
        logoRow.ToolTip = "CambridgeSoft Enterprise Manager";
        logoRow.URL = this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + Resources.Resource.COE_HOME;
        COELogoObject.LogoItem.AddLogoItemRow(logoRow);

        #endregion

        #region CoeMenuObject Info Set Up

        COEMenuObject = new COEMenu();

        /*  GOTO menu */
        COEMenu.MenuItemRow rootSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        rootSectionItemRow.ParentLevel = -1;
        rootSectionItemRow.Key = "GoTo_0";
        rootSectionItemRow.ParentKey = String.Empty;
        rootSectionItemRow.TitleText = rootSectionItemRow.ToolTip = Resources.Resource.GoTo_MenuItem_Text;
        rootSectionItemRow.URL = "";
        rootSectionItemRow.TargetFrame = "_self";
        rootSectionItemRow.HasChildItems = bool.TrueString;
        COEMenuObject.MenuItem.AddMenuItemRow(rootSectionItemRow);

        /* Goto - DataView Manager */
        COEMenu.MenuItemRow dataviewManagerSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        dataviewManagerSectionItemRow.ParentLevel = 0;
        dataviewManagerSectionItemRow.Key = "DataViewManager_0_0";
        dataviewManagerSectionItemRow.ParentKey = "GoTo_0";
        dataviewManagerSectionItemRow.TitleText = dataviewManagerSectionItemRow.ToolTip = Resources.Resource.DataViewManager_MenuItem_Text;
        dataviewManagerSectionItemRow.URL = "";
        dataviewManagerSectionItemRow.TargetFrame = "_self";
        dataviewManagerSectionItemRow.HasChildItems = bool.TrueString;
        COEMenuObject.MenuItem.AddMenuItemRow(dataviewManagerSectionItemRow);

        /* GoTo - DataView Manager - Create from another DV */
        COEMenu.MenuItemRow createFromAnotherDVItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        createFromAnotherDVItemRow.ParentLevel = 1;
        createFromAnotherDVItemRow.Key = "CreateFromAnotherDV_0_0_0";
        createFromAnotherDVItemRow.ParentKey = "DataViewManager_0_0";
        createFromAnotherDVItemRow.TitleText = createFromAnotherDVItemRow.ToolTip = Resources.Resource.CreateFromAnotherDV_MenuItem_Text;
        createFromAnotherDVItemRow.URL = Constants.DvManagerContentAreaFolder 
                                        + "NewDataView.aspx?" + Constants.Action + "=" + Constants.PageStates.Clone_DV.ToString();
        createFromAnotherDVItemRow.TargetFrame = "_self";
        createFromAnotherDVItemRow.HasChildItems = bool.FalseString;
        createFromAnotherDVItemRow.ImagePath = "database_register.png";
        createFromAnotherDVItemRow.Description = "Create a new dataview based on another existing dataview";
        COEMenuObject.MenuItem.AddMenuItemRow(createFromAnotherDVItemRow);

        /* GoTo - DataViewManager - Create from master dataView */
        COEMenu.MenuItemRow createFromMasterDVItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        createFromMasterDVItemRow.ParentLevel = 1;
        createFromMasterDVItemRow.Key = "CreateFromMasterDV_0_0_1";
        createFromMasterDVItemRow.ParentKey = "DataViewManager_0_0";
        createFromMasterDVItemRow.TitleText = createFromMasterDVItemRow.ToolTip = Resources.Resource.CreateFromMasterDV_MenuItem_Text;
        createFromMasterDVItemRow.URL = Constants.DvManagerContentAreaFolder 
                                        + "EnterNameDescription.aspx?" + Constants.Action + "=" + Constants.PageStates.Clone_DV.ToString() +
                                        "&" + Constants.SelectedDataViewID + "=" + Constants.MasterSchemaDataViewID.ToString();
        createFromMasterDVItemRow.TargetFrame = "_self";
        createFromMasterDVItemRow.HasChildItems = bool.FalseString;
        createFromMasterDVItemRow.ImagePath = "database_register.png";
        createFromMasterDVItemRow.Description = "Create a new dataview based on the master dataview";
        COEMenuObject.MenuItem.AddMenuItemRow(createFromMasterDVItemRow);

        /* Goto - DataViewManager - Edit dataView */
        COEMenu.MenuItemRow editExistingDVItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        editExistingDVItemRow.ParentLevel = 1;
        editExistingDVItemRow.Key = "EditDV_0_0_2";
        editExistingDVItemRow.ParentKey = "DataViewManager_0_0";
        editExistingDVItemRow.TitleText = editExistingDVItemRow.ToolTip = Resources.Resource.EditDataView_MenuItem_Text;
        editExistingDVItemRow.URL = Constants.DvManagerContentAreaFolder 
                                    + "NewDataView.aspx?" + Constants.Action + "=" + Constants.PageStates.Edit_DV.ToString();
        editExistingDVItemRow.TargetFrame = "_self";
        editExistingDVItemRow.HasChildItems = bool.FalseString;
        editExistingDVItemRow.ImagePath = "data_view.png";
        editExistingDVItemRow.Description = "Edit an existing data view";
        COEMenuObject.MenuItem.AddMenuItemRow(editExistingDVItemRow);

        /* Goto - DataViewManager - Edit MasterDataview */
        COEMenu.MenuItemRow editMasterDataViewItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        editMasterDataViewItemRow.ParentLevel = 1;
        editMasterDataViewItemRow.Key = "EditMasterDataView_0_0_3";
        editMasterDataViewItemRow.ParentKey = "DataViewManager_0_0";
        editMasterDataViewItemRow.TitleText = editMasterDataViewItemRow.ToolTip = Resources.Resource.EditMasterDataView_MenuItem_Text;
        editMasterDataViewItemRow.URL = Constants.DvManagerContentAreaFolder + "DataviewBoard.aspx?IsMaster=true";
        editMasterDataViewItemRow.TargetFrame = "_self";
        editMasterDataViewItemRow.HasChildItems = bool.FalseString; 
        editMasterDataViewItemRow.Description = "Edit current master dataview";
        editMasterDataViewItemRow.ImagePath = "data_view.png";
        COEMenuObject.MenuItem.AddMenuItemRow(editMasterDataViewItemRow);

        #region Form Designer items
        /* GoTo - Form Designer */
        COEMenu.MenuItemRow formDesignerSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        formDesignerSectionItemRow.ParentLevel = 0;
        formDesignerSectionItemRow.Key = "FormDesigner_0_1";
        formDesignerSectionItemRow.ParentKey = "GoTo_0";
        formDesignerSectionItemRow.TitleText = formDesignerSectionItemRow.ToolTip = Resources.Resource.FormDesigner_MenuItem_Text;
        formDesignerSectionItemRow.URL = String.Empty;
        formDesignerSectionItemRow.TargetFrame = "_self";
        formDesignerSectionItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(formDesignerSectionItemRow);

        /* GoTo - Form Designer - Edit Form*/
        COEMenu.MenuItemRow editFormItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        editFormItemRow.ParentLevel = 1;
        editFormItemRow.Key = "EditForm_0_1_0";
        editFormItemRow.ParentKey = "FormDesigner_0_1";
        editFormItemRow.TitleText = editFormItemRow.ToolTip = editFormItemRow.Description = Resources.Resource.FormDesignerEdit_MenuItem_Text;
        editFormItemRow.URL = "../FormDesigner/Default.aspx";
        editFormItemRow.TargetFrame = "_self";
        editFormItemRow.HasChildItems = bool.FalseString;
        editFormItemRow.ImagePath = "icon.gif";
        COEMenuObject.MenuItem.AddMenuItemRow(editFormItemRow);
        #endregion

        #region Security items

        COEMenu.MenuItemRow adminSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        adminSectionItemRow.ParentLevel = 0;
        adminSectionItemRow.ParentKey = "GoTo_0";
        adminSectionItemRow.Key = "Security_0_0";
        adminSectionItemRow.TitleText = adminSectionItemRow.ToolTip = adminSectionItemRow.Description = "Security";
        adminSectionItemRow.URL = "";
        adminSectionItemRow.TargetFrame = "_self";
        adminSectionItemRow.HasChildItems = bool.TrueString;
        COEMenuObject.MenuItem.AddMenuItemRow(adminSectionItemRow);

        COEMenu.MenuItemRow manageUsersSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        manageUsersSectionItemRow.ParentLevel = 1;
        manageUsersSectionItemRow.ParentKey = "Security_0_0";
        manageUsersSectionItemRow.Key = "ManageUsers_0_0_0";
        manageUsersSectionItemRow.TitleText = manageUsersSectionItemRow.ToolTip = manageUsersSectionItemRow.Description = "Manage Users";
        manageUsersSectionItemRow.URL =  Constants.SecurityManagerContentAreaFolder + "ManageUsers.aspx?appName=COE";
        manageUsersSectionItemRow.TargetFrame = "_self";
        manageUsersSectionItemRow.HasChildItems = bool.FalseString;
        manageUsersSectionItemRow.ImagePath = "icon.gif";
        COEMenuObject.MenuItem.AddMenuItemRow(manageUsersSectionItemRow);

        COEMenu.MenuItemRow manageRolesSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        manageRolesSectionItemRow.ParentLevel = 1;
        manageRolesSectionItemRow.ParentKey = "Security_0_0";
        manageRolesSectionItemRow.Key = "ManageUsers_0_0_1";
        manageRolesSectionItemRow.TitleText = manageRolesSectionItemRow.ToolTip = manageRolesSectionItemRow.Description = "Manage Roles";
        manageRolesSectionItemRow.URL = Constants.SecurityManagerContentAreaFolder + "ManageRoles.aspx";
        manageRolesSectionItemRow.TargetFrame = "_self";
        manageRolesSectionItemRow.HasChildItems = bool.FalseString;
        manageRolesSectionItemRow.ImagePath = "icon.gif";
        COEMenuObject.MenuItem.AddMenuItemRow(manageRolesSectionItemRow);

        #endregion

        #endregion

        #region CoeToolBar Info SetUp

        COEToolbarObject = new COEMenu();

        COEMenu.MenuItemRow homeItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        homeItemRow.ParentLevel = -1;
        homeItemRow.ParentKey = String.Empty;
        homeItemRow.TitleText = "Home";
        homeItemRow.URL = ConfigurationManager.AppSettings["HomeLinkURL"];
        homeItemRow.URL += ConfigurationManager.AppSettings["HomeLinkURL"].Contains("?") ? "&" : "?";
        homeItemRow.TargetFrame = "_self";
        homeItemRow.Key = "Home";
        homeItemRow.ToolTip = "Home Page";
        homeItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(homeItemRow);

        COEMenu.MenuItemRow mainItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        mainItemRow.ParentLevel = -1;
        homeItemRow.ParentKey = String.Empty;
        mainItemRow.TitleText = "Main";
        mainItemRow.URL = Constants.PublicContentAreaFolder + Resources.Resource.COE_HOME;
        mainItemRow.TargetFrame = "_self";
        mainItemRow.Key = "Main";
        mainItemRow.ToolTip = "Main Page";
        mainItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(mainItemRow);

        COEMenu.MenuItemRow helpItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        helpItemRow.ParentLevel = -1;
        helpItemRow.ParentKey = String.Empty;
        helpItemRow.Key = "Help";
        helpItemRow.TitleText = "Help";
        helpItemRow.URL = this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + "Help.aspx";
        helpItemRow.TargetFrame = "_self";
        helpItemRow.ToolTip = "Help Page";
        helpItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(helpItemRow);

        COEMenu.MenuItemRow aboutItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        aboutItemRow.ParentLevel = -1;
        aboutItemRow.ParentKey = String.Empty;
        aboutItemRow.Key = "About";
        aboutItemRow.TitleText = "About";
        aboutItemRow.URL = this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + "About.aspx";
        aboutItemRow.TargetFrame = "_self";
        aboutItemRow.ToolTip = "About";
        aboutItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(aboutItemRow);

        #endregion

        #region CoeFooter Info SetUp

        COEFooterObject = new COEMenu();

        COEMenu.MenuItemRow homeFooterItemRow = COEFooterObject.MenuItem.NewMenuItemRow();
        homeFooterItemRow.ParentLevel = -1;
        homeFooterItemRow.ParentKey = String.Empty;
        homeFooterItemRow.TitleText = "Home";
        homeFooterItemRow.URL = ConfigurationManager.AppSettings["HomeLinkURL"];
        homeFooterItemRow.URL += ConfigurationManager.AppSettings["HomeLinkURL"].Contains("?") ? "&" : "?";
        homeFooterItemRow.TargetFrame = "_self";
        homeFooterItemRow.Key = "Home";
        homeFooterItemRow.ToolTip = "Home Page";
        homeFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(homeFooterItemRow);

        COEMenu.MenuItemRow mainFooterItemRow = COEFooterObject.MenuItem.NewMenuItemRow();
        mainFooterItemRow.ParentLevel = -1;
        mainFooterItemRow.ParentKey = String.Empty;
        mainFooterItemRow.TitleText = "Main";
        mainFooterItemRow.URL = this.Page.ResolveUrl(Constants.PublicContentAreaFolder)  + Resources.Resource.COE_HOME;
        mainFooterItemRow.TargetFrame = "_self";
        mainFooterItemRow.Key = "Main";
        mainFooterItemRow.ToolTip = "Main Page";
        mainFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(mainFooterItemRow);

        COEMenu.MenuItemRow helpFooterItemRow = COEFooterObject.MenuItem.NewMenuItemRow();
        helpFooterItemRow.ParentLevel = -1;
        helpFooterItemRow.ParentKey = String.Empty;
        helpFooterItemRow.Key = "Help";
        helpFooterItemRow.TitleText = "Help";
        helpFooterItemRow.URL = Constants.PublicContentAreaFolder + "Help.aspx";
        helpFooterItemRow.TargetFrame = "_self";
        helpFooterItemRow.ToolTip = "Help Page";
        helpFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(helpFooterItemRow);

        COEMenu.MenuItemRow aboutFooterItemRow = COEFooterObject.MenuItem.NewMenuItemRow();
        aboutFooterItemRow.ParentLevel = -1;
        aboutFooterItemRow.ParentKey = String.Empty;
        aboutFooterItemRow.Key = "About";
        aboutFooterItemRow.TitleText = "About";
        aboutFooterItemRow.URL = this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + "About.aspx";
        aboutFooterItemRow.TargetFrame = "_self";
        aboutFooterItemRow.ToolTip = "About";
        aboutFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(aboutFooterItemRow);

        #endregion

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// This Method will set all the attributes of the Header user control.
    /// </summary>
    /// <remarks>To binf the information with the UC, the UC must implement the ICOEHeaderUC interface</remarks>
    private void SetHeaderAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control headerUC = this.HeaderContentPlaceHolder.FindControl("HeaderUserControl");
        if(headerUC != null && headerUC is ICOEHeaderUC)
            ((ICOEHeaderUC)headerUC).DataBind(COELogoObject, COEMenuObject, COEToolbarObject);
        else
            throw new Exception("Unsupported kind of Header UC. You must implement ICOEHeaderUC interface");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// This Method will set all the attributes of the Footer user control.
    /// </summary>
    /// <remarks>To binf the information with the UC, the UC must implement the ICOEFooterUC interface</remarks>
    private void SetFooterAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ////Check if we have to load a custom footer. This comes from the App Config file, for now the global.asax
        //if (!this.UseDefaultFooter)
        //{
        //    //According the type of control, load it.
        //    switch (this.CustomFooterType)
        //    {
        //        case GUIShellTypes.FooterControlSupportedTypes.Html:
        //            this.FooterContainer.InnerHtml = Response.Write("/UserControls/Custom/" + this.CustomFooterName);
        //            break;
        //    }
        //}
        //else
        //{
        //    if (this.FooterUserControl is ICOEFooterUC)
        //        ((ICOEFooterUC)this.FooterUserControl).DataBind(COEFooterObject);
        //    else
        //        throw new Exception("Unsupported kind of Footer UC. You must implement ICOEFooterUC interface");
        //}
        //Check if we have to load a custom footer. This comes from the App Config file, for now the global.asax

        Control footerUC = this.FooterContentPlaceHolder.FindControl("FooterUserControl");
        if(footerUC != null && footerUC is ICOEFooterUC)
            ((ICOEFooterUC) footerUC).DataBind(COEFooterObject);
        else
            throw new Exception("Unsupported kind of Footer UC. You must implement ICOEFooterUC interface");
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to add a user contol inside the left panel container.
    /// </summary>
    /// <param name="controlToAddURL">User Control URL to dinamically load</param>
    public void AddControlToLeftPanel(string controlToAddURL)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control controlToAdd = LoadControl(controlToAddURL);
        if (controlToAdd != null)
        {
            if (controlToAdd is ICOELeftPanelUC) //We must be sure this interface is implemented.
            {
                this.LeftPanelContainer.Controls.Add(controlToAdd);
                _showLeftPanel = true;
            }
            else
                throw new Exception("Unsupported Control Type");
        }
        else
            _showLeftPanel = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to find a controls inside the left panel.
    /// </summary>
    /// <param name="controlID"> The control Id to search</param>
    /// <returns>if found, the control itself. If not a null control</returns>
    public ICOELeftPanelUC FindLeftPanelControl(string controlID)
    {
        if (this.LeftPanelContainer.HasControls())
        {
            //I had to do this cause FindControl didn't work for me. - myControl = this.LeftPanelContainer.FindControl(controlID);
            foreach (Control currentControl in this.LeftPanelContainer.Controls)
            {
                //Coverity Fixes: CBOE-313
                if ( currentControl !=null && !String.IsNullOrEmpty(currentControl.ID))
                {
                    if (currentControl.ID == controlID && currentControl is ICOELeftPanelUC)
                        return (ICOELeftPanelUC)currentControl;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Method to display confirmation messages in the tope of page .
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <remarks>If you want to hide the control, pass it a null or empty parameter</remarks>
    public override void DisplayConfirmationMessage(string message)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control confirmationArea = this.FindControl(Constants.ContentPlaceHolderID).FindControl(Constants.ConfirmationAreaUCID);
        if (confirmationArea != null)
        {
            if (confirmationArea is Forms_Public_UserControls_ConfirmationArea)
            {
                if (!string.IsNullOrEmpty(message))
                    ((Forms_Public_UserControls_ConfirmationArea)confirmationArea).Text = message;
                else
                    ((Forms_Public_UserControls_ConfirmationArea)confirmationArea).Text = String.Empty;
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }



    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="exception">exception from wich message should be displayed</param>
    /// <param name="showBackLink">obsolete?</param>
    public override void DisplayErrorMessage(Exception ex, bool showBackLink)
    {
        string message = "An unexpected error has occurred.\r";

        //Coverity Fixes: CBOE-313
        if (ex != null)
        {
            if (ex.GetBaseException() != null)
            {
                message = ex.GetBaseException().Message;
            }
            else
            {
                message = ex.Message;
            }
        }
        DisplayErrorMessage(message, showBackLink);
    }


   


    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <remarks>If you want to hide the control, pass it a null or empty parameter</remarks>
    public void DisplayErrorMessage(string message)
    {
        this.DisplayErrorMessage(message, false);
    }

    public override void DisplayErrorMessage(string message, bool showBackLink)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control confirmationArea = this.FindControl(Constants.ContentPlaceHolderID).FindControl(Constants.ErrorAreaUCID);
        if(confirmationArea != null)
        {
            if(confirmationArea is Forms_Public_UserControls_ErrorArea)
            {
                if(!string.IsNullOrEmpty(message))
                {
                    ((Forms_Public_UserControls_ErrorArea) confirmationArea).Text = message;
                    ((Forms_Public_UserControls_ErrorArea) confirmationArea).Visible = true;
                }
                else
                {
                    ((Forms_Public_UserControls_ErrorArea) confirmationArea).Text = String.Empty;
                    ((Forms_Public_UserControls_ErrorArea) confirmationArea).Visible = false;
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
   
    /// <summary>
    /// Method to be redirected to the messages page and display a message
    /// </summary>
    /// <param name="textToDisplay">Text to display</param>
    /// <param name="displayBackButton">Display or not the back button</param>
    public void DisplayMessagesPage(Constants.MessagesCode messageToDisplay, GUIShellTypes.MessagesButtonType buttonType)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = "~/Forms/Public/ContentArea/Messages.aspx?";
        url += GUIShellTypes.RequestVars.MessageCode.ToString() + "=" + messageToDisplay.ToString();
        url += "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() + "=" + buttonType.ToString();
        Server.Transfer(url, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to clean all current DataViews session variables.
    /// </summary>
    /// <remarks>We actually just remove COEDataView, COEDataViewBO, COEDataViewBOList</remarks>
    public void ClearAllCurrentDVSessionVars()
    {
        if (Session[Constants.COEDataView] != null)
            Session.Remove(Constants.COEDataView);

        // Remove the schems on publishing from session.
        if (Session[Constants.COESchemasOnPublishing] != null)
            Session.Remove(Constants.COESchemasOnPublishing);
        // Remove the schemas on removing from session.
        if (Session[Constants.COESchemasOnRemoving] != null)
            Session.Remove(Constants.COESchemasOnRemoving);

        if (Session[Constants.COEDataViewBO] != null)
            Session.Remove(Constants.COEDataViewBO);
        if (Session[Constants.COEDataViewBOList] != null)
            Session.Remove(Constants.COEDataViewBOList);
        if (Session[Constants.DataViewFrom] != null)
            Session.Remove(Constants.DataViewFrom);
        if (Session[Constants.TableListBO] != null)
            Session.Remove(Constants.TableListBO);
        if(Session["AlreadyEnteredName"] != null)
            Session.Remove("AlreadyEnteredName");
        if(Session["OriginalTable"] != null)
            Session.Remove("OriginalTable");
        if (Session["FreshTable"] != null)
            this.Session.Remove("FreshTable");
        if (Session["FreshRels"] != null)
            this.Session.Remove("FreshRels");
        if (Session["DummyFreshTable"] != null)
            this.Session.Remove("DummyFreshTable");
        if (Session["DummyFreshRels"] != null)
            this.Session.Remove("DummyFreshRels");
    }

    /// <summary>
    /// Method to set the Page Title
    /// </summary>
    /// <param name="pageName">Text to use as PageTitle</param>
    public override void SetPageTitle(string pageName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(pageName))
        {
            if(this.IsEmbeddable()) //Header is not shown so we are going to display the title inside the content.
                this.PageTitleLabel.Text = pageName;
            else
            {
                this.HeaderUserControl.SetPageTitle(pageName);
                this.PageTitleLabel.Visible = false;
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to find a control inside the current contentplaceholder.
    /// </summary>
    /// <param name="controlID">The id of the control to search</param>
    /// <returns>The found control</returns>
    public Control FindControlInPage(string controlID)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control retVal = null;
        if (!string.IsNullOrEmpty(controlID))
            retVal = this.FindControl(Constants.MainBodyName).FindControl(Constants.MainFormName).FindControl(Constants.ContentPlaceHolderID).FindControl(controlID);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    public COEDataViewBO GetDataViewBO()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO retVal = null;
        if (Session[Constants.COEDataViewBO] != null)
            retVal = ((COEDataViewBO)Session[Constants.COEDataViewBO]);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    public void SetDataViewBO(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (dataViewBO != null)
            Session[Constants.COEDataViewBO] = dataViewBO;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public ValidCartridgeReadOnlyBO GetValidCartridgeBO()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ValidCartridgeReadOnlyBO retVal = null;
        if (Session[Constants.ValidCartridgeBO] != null)
            retVal = ((ValidCartridgeReadOnlyBO)Session[Constants.ValidCartridgeBO]);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    public void SetValidCartridgeBO(ValidCartridgeReadOnlyBO cartridgeBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (cartridgeBO != null)
            Session[Constants.ValidCartridgeBO] = cartridgeBO;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public void DisableLoggedOnMenus()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ((UltraWebMenu) this.HeaderContentPlaceHolder.FindControl("HeaderUserControl").FindControl("GoToUltraWebMenu")).Enabled = false;
        ((UltraWebToolbar) this.HeaderContentPlaceHolder.FindControl("HeaderUserControl").FindControl("Toolbar")).Enabled = false;
        Control footerUC = this.FooterContentPlaceHolder.FindControl("FooterUserControl");
        if(footerUC != null)
            footerUC.Visible = false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

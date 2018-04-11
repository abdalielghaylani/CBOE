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

using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla.Web;
using CambridgeSoft.COE.ChemBioViz.Services;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using Csla.Core;
using Infragistics.WebUI.UltraWebListbar;
using Resources;

public partial class ChemBioVizMasterPage : GUIShellMaster
{
    #region Variables

    COELogo COELogoObject;
    COEMenu COEMenuObject;
    COEMenu COEToolbarObject;
    COEMenu COEFooterObject;

    public COEFormGroup _formGroup;
    public CslaDataSource _cslaDataSource = new CslaDataSource();
    public ResultsCriteria _resultsCriteria;
    public PagingInfo _pagingInfo;

	internal COENavigationPane COENavigationPaneObject;

    #endregion

    #region Properties

    public COEFormGroup FromGroup
    {
        get { return this._formGroup; }
    }

    public CslaDataSource CSLADataSource
    {
        get { return this._cslaDataSource; }
    }

    public ResultsCriteria ResultCriteria
    {
        get { return _resultsCriteria; }
    }

    public PagingInfo PagingInfo
    {
        get { return _pagingInfo; }
        set { _pagingInfo = value; }
    }

    public string StructuresCount
    {
        get
        {
            return HttpContext.Current.Application[Constants.StructuresCount] != null ? HttpContext.Current.Application[Constants.StructuresCount].ToString() : string.Empty;
        }
    }

    public LeftPanelOptions.PanelState LeftPanelState
    {
        get
        {
            if(!string.IsNullOrEmpty(this.LeftPanelStateHidden.Value))
                ViewState["LeftPanelState"] = this.LeftPanelStateHidden.Value.ToLower() != "false" ? LeftPanelOptions.PanelState.Collapsed : LeftPanelOptions.PanelState.Expanded;
            return ViewState["LeftPanelState"] == null ? LeftPanelOptions.PanelState.Expanded : (LeftPanelOptions.PanelState) ViewState["LeftPanelState"];
        }
        set
        {
            ViewState["LeftPanelState"] = value;
        }
    }
    #endregion

    #region Event Handlers

    protected override void OnInit(EventArgs e)
    {
        if (Request != null && Request.Browser != null && (Request.Browser.Browser.ToLower() == ("mozilla") || ((Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer") && Request.Browser.MajorVersion >= 10)))
            Page.ClientTarget = "iecustom";
        //((HtmlGenericControl)this.FindControl("MainBody")).Attributes.Add("onload","StartMenu()");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        //this.SetJScriptReference();
        string jsPath = !string.IsNullOrEmpty(this.Page.Request["callingApp"]) ? Constants.GetPublicCommonJSsPath(this.Page.Request["callingApp"]) : Constants.GetPublicCommonJSsPath();
        base.SetJScriptReference("COEChemBioVizJs", jsPath);
        
        //if (!Page.IsPostBack)
        //{
        this.LoadInformation();
        this.SetHeaderAttributes();
        this.SetNavigationPaneAttributtes(COENavigationPaneObject);
            //this.SetFooterAttributes();
            //jhs panel
            
        //}
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        switch(this.LeftPanelState)
        {
            case LeftPanelOptions.PanelState.Expanded:
                this.Panel.Src = Utilities.ImagesBaseFullFolder(this.Page.StyleSheetTheme) + "Collapse.png";
                this.Panel.Alt = Resource.Collapse_Label_Text;
                this.Panel.Style.Add(HtmlTextWriterStyle.Width, "200px");
                break;
            case LeftPanelOptions.PanelState.Collapsed:
                this.Panel.Src = Utilities.ImagesBaseFullFolder(this.Page.StyleSheetTheme) + "Expand.png";
                this.Panel.Alt = Resource.Expand_Label_Text;
                this.Panel.Style.Add(HtmlTextWriterStyle.Width, "");
                break;
        }
        this.UltraWebListbarControl.ClientSideEvents.InitializeListbar = "initializeBar";

        this.WriteJsCodeForPanel();
        if(!_showLeftPanel || this.AccordionControl.Controls.Count <= 0)
            this.HideLeftPanel();    
        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Writes the required code to Expand/Collapse the left panel
    /// </summary>
    private void WriteJsCodeForPanel()
    {
        string leftPanelCode = "LeftPanelCode";
        //LeftPanelState
        if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(ChemBioVizMasterPage), leftPanelCode))
        {
            string script = "<script type=\"text/javascript\">";
            script += "var _pinned = " + (this.LeftPanelState == LeftPanelOptions.PanelState.Collapsed ? "false;" : "true;");
            script += "function toggleContainer(){";
            script += "var theListBar = iglbar_getListbarById('" + this.UltraWebListbarControl.ID + "');";
            script += "var panel = document.getElementById('" + this.Panel.ClientID + "');";
            script += " if(_pinned) {";
            script += " theListBar.Element.style.display = 'none';";
            script += "  panel.src = '" + Utilities.ImagesBaseFullFolder(this.Page.StyleSheetTheme) + "Expand.png" +"';";
            script += " panel.alt = '" + Resource.Expand_Label_Text + "';";
            script += " panel.style.width = '';";
            script += " document.getElementById('" + this.LeftPanelContainer.ClientID + "').style.width = '';";
            script += "} else {";
            script += "theListBar.Element.style.display = '';";
            script += "  panel.src = '" + Utilities.ImagesBaseFullFolder(this.Page.StyleSheetTheme) + "Collapse.png" + "';";
            script += " panel.alt = '" + Resource.Collapse_Label_Text + "';";
            script += " panel.style.width = '200px';";
            script += " document.getElementById('" + this.LeftPanelContainer.ClientID + "').style.width = '200px';";
            script += "}";
            script += "document.getElementById('" + this.LeftPanelStateHidden.ClientID + "').value = new String(_pinned);";
            script += "_pinned = !_pinned;";
            script += "}";
            script += "function initializeBar(oListBar){";
            script += "  oListBar.Element.style.display = '" + (this.LeftPanelState == LeftPanelOptions.PanelState.Collapsed ? "none';" : "';");
            script += " document.getElementById('" + this.LeftPanelContainer.ClientID + "').style.width = '" + (this.LeftPanelState == LeftPanelOptions.PanelState.Collapsed ? "15px" : "200px") + "';";
            script += "}";
            script += "</script>";
            this.Page.ClientScript.RegisterStartupScript(typeof(ChemBioVizMasterPage), leftPanelCode, script);
        }
    }

    private void HideLeftPanel()
    {
        this.LeftPanelContainer.Visible = false;
        this.MainContent.Style["width"] = "98%";
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
        logoRow.URL = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Home.aspx";
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

        /* Goto - Search */
        COEMenu.MenuItemRow searchSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        searchSectionItemRow.ParentLevel = 0;
        searchSectionItemRow.Key = "Search_0_0";
        searchSectionItemRow.ParentKey = "GoTo_0";
        searchSectionItemRow.TitleText = searchSectionItemRow.ToolTip = Resources.Resource.Search_MenuItem_Text;
        searchSectionItemRow.URL = String.Empty;
        searchSectionItemRow.TargetFrame = "_self";
        searchSectionItemRow.HasChildItems = bool.TrueString;
        COEMenuObject.MenuItem.AddMenuItemRow(searchSectionItemRow);

        /* GoTo - Search - Search */
        COEMenu.MenuItemRow searchComponentItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        searchComponentItemRow.ParentLevel = 1;
        searchComponentItemRow.Key = "SearchComponent_0_0_0";
        searchComponentItemRow.ParentKey = "Search_0_0";
        searchComponentItemRow.TitleText = searchComponentItemRow.ToolTip = Resources.Resource.SearchComponent_MenuItem_Text;
        searchComponentItemRow.URL = this.Page.ResolveUrl(Constants.GetSearchContentAreaFolder()) + "/ChemBioVizSearch.aspx"; 
        searchComponentItemRow.TargetFrame = "_self";
        searchComponentItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(searchComponentItemRow);

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
        mainItemRow.URL = Constants.GetPublicContentAreaFolder() + "Home.aspx";
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
        helpItemRow.URL = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Help.aspx";
        helpItemRow.TargetFrame = "_self";
        helpItemRow.ToolTip = "Help Page";
        helpItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(helpItemRow);

        COEMenu.MenuItemRow aboutItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        aboutItemRow.ParentLevel = -1;
        aboutItemRow.ParentKey = String.Empty;
        aboutItemRow.Key = "About";
        aboutItemRow.TitleText = "About";
        aboutItemRow.URL = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "About.aspx";
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
        mainFooterItemRow.URL = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Home.aspx";
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
        helpFooterItemRow.URL = Constants.GetPublicContentAreaFolder() + "Help.aspx";
        helpFooterItemRow.TargetFrame = "_self";
        helpFooterItemRow.ToolTip = "Help Page";
        helpFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(helpFooterItemRow);

        COEMenu.MenuItemRow aboutFooterItemRow = COEFooterObject.MenuItem.NewMenuItemRow();
        aboutFooterItemRow.ParentLevel = -1;
        aboutFooterItemRow.ParentKey = String.Empty;
        aboutFooterItemRow.Key = "About";
        aboutFooterItemRow.TitleText = "About";
        aboutFooterItemRow.URL = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "About.aspx";
        aboutFooterItemRow.TargetFrame = "_self";
        aboutFooterItemRow.ToolTip = "About";
        aboutFooterItemRow.HasChildItems = "false";
        COEFooterObject.MenuItem.AddMenuItemRow(aboutFooterItemRow);

        #endregion

		#region CoeNavigation Pane Info SetUp

		COENavigationPaneObject = new COENavigationPane();

        COENavigationPane.NavigationPaneItemRow preferencesRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        preferencesRow.ID = "Preferences";
        preferencesRow.Title = "Preferences";
        preferencesRow.EnableToolTip = "Preferences";
        preferencesRow.DisableToolTip = "Preferences";
        preferencesRow.TargetURL = "";
        preferencesRow.PageOwnerID = "ChemBioVizSearch";
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(preferencesRow);

		COENavigationPane.NavigationPaneItemRow queryManageRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
		queryManageRow.ID = "QueryManagement";
		queryManageRow.Title = "Query Management";
		queryManageRow.EnableToolTip = "Manage Queries and Hitlists";
		queryManageRow.DisableToolTip = "Manage Queries and Hitlists";
        queryManageRow.TargetURL = "";
        queryManageRow.PageOwnerID = "ChemBioVizSearch";
		COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(queryManageRow);
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
        if (this.HeaderUserControl is ICOEHeaderUC)
            ((ICOEHeaderUC)this.HeaderUserControl).DataBind(COELogoObject, COEMenuObject, COEToolbarObject);
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
        if (this.FooterUserControl is ICOEFooterUC)
            ((ICOEFooterUC)this.FooterUserControl).DataBind(COEFooterObject);
        else
            throw new Exception("Unsupported kind of Footer UC. You must implement ICOEFooterUC interface");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to add a user contol inside the left panel container.
    /// </summary>
    /// <param name="controlToAddURL">User Control URL to dinamically load</param>
    //public void AddControlToLeftPanel(string controlToAddURL)
    //{
    //    Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
    //    Control controlToAdd = LoadControl(controlToAddURL);
    //    if (controlToAdd != null)
    //    {
    //        if (controlToAdd is ICOELeftPanelUC) //We must be sure this interface is implemented.
    //        {
    //            this.LeftPanelContainer.Controls.Add(controlToAdd);
    //            _showLeftPanel = true;
    //        }
    //        else
    //            throw new Exception("Unsupported Control Type");
    //    }
    //    else
    //        _showLeftPanel = false;
    //    Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    //}

    /// <summary>
    /// Method to find a controls inside the left panel.
    /// </summary>
    /// <param name="controlID"> The control Id to search</param>
    /// <returns>if found, the control itself. If not a null control</returns>
    //public ICOELeftPanelUC FindLeftPanelControl(string controlID)
    //{
    //    if (this.LeftPanelContainer.HasControls())
    //    {
    //        //I had to do this cause FindControl didn't work for me. - myControl = this.LeftPanelContainer.FindControl(controlID);
    //        foreach (Control currentControl in this.LeftPanelContainer.Controls)
    //        {
    //            if (!String.IsNullOrEmpty(currentControl.ID))
    //            {
    //                if (currentControl.ID == controlID && currentControl is ICOELeftPanelUC)
    //                    return (ICOELeftPanelUC)currentControl;
    //            }
    //        }
    //    }
    //    return null;
    //}

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
            if (confirmationArea is Forms_UserControls_ConfirmationArea)
            {
                if (!string.IsNullOrEmpty(message))
                    ((Forms_UserControls_ConfirmationArea)confirmationArea).Text = message;
                else
                    ((Forms_UserControls_ConfirmationArea)confirmationArea).Text = String.Empty;
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }


    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="exception">exception from wich message should be displayed</param>
    /// <param name="showBackLink">obsolete</param>
    public override void DisplayErrorMessage(Exception ex, bool showBackLink)
    {
        string message = "An unexpected error has occurred.\r";

        if (ex != null) //Coverity Fix for CID : 11848
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


    public override void DisplayErrorMessage(string message, bool showBackLink)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control confirmationArea = this.FindControl(Constants.ContentPlaceHolderID).FindControl(Constants.ErrorAreaUCID);
        if (confirmationArea != null)
        {
            if (confirmationArea is Forms_UserControls_ErrorArea)
            {
                if (!string.IsNullOrEmpty(message))
                    ((Forms_UserControls_ErrorArea)confirmationArea).Text = message;
                else
                    ((Forms_UserControls_ErrorArea)confirmationArea).Text = String.Empty;
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

    #endregion

	#region jhspanel

	/// <summary>
	/// This method sets all the Accordion's Panels.
	/// </summary>
	/// <param name="COENavigationPaneParam">Object with all the info to display in the COENavigationPane Section</param>
	/// <remarks>This method doesn't add the control inside an accordion panel</remarks>
	private void SetNavigationPaneAttributtes(COENavigationPane COENavigationPaneParam)
	{
		for (int i = 0; i < COENavigationPaneParam.NavigationPaneItem.Count; i++)
		{
			Infragistics.WebUI.UltraWebListbar.Group currentGroup = this.AccordionControl.Groups.Add(COENavigationPaneParam.NavigationPaneItem[i].Title.ToString(), COENavigationPaneParam.NavigationPaneItem[i].ID.ToString());
            if(!(this.Page is ChemBioVizSearch))
                currentGroup.Enabled = false;
            else
            {
                if(COENavigationPaneParam.NavigationPaneItem[i].ID == "ExportHits" && ((ChemBioVizSearch) this.Page).CurrentMode == CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.CurrentFormEnum.QueryForm)
                {
                    currentGroup.Enabled = false;
                }
            }
			currentGroup.Text = COENavigationPaneParam.NavigationPaneItem[i].Title;
			currentGroup.TextAlign = GUIShellTypes.GroupTitleTextAlign;
			// By default, all groups are colapsed. Each content page must expand each group.
			currentGroup.Expanded = false;
			currentGroup.TargetUrl = COENavigationPaneParam.NavigationPaneItem[i].TargetURL;
			currentGroup.ToolTip = COENavigationPaneParam.NavigationPaneItem[i].EnableToolTip;
			currentGroup.Tag = COENavigationPaneParam.NavigationPaneItem[i].DisableToolTip.ToString();
		}
	}

	/// <summary>
	/// This methos adds a control inside a accordion panel. This method is called by the GUIShell pages.
	/// </summary>
	/// <param name="currentControlItemRow">The control's info to display</param>
	/// <param name="expandedPanel">Status of the current Accordion Panel</param>
    public ICOENavigationPanelControl AddControlToAccordionPanel(COENavigationPane.ControlItemRow currentControlItemRow, bool expandedPanel)
	{
		if (currentControlItemRow != null)
		{
			Infragistics.WebUI.UltraWebListbar.Group groupToAddControl = null;
			if (this.AccordionControl.Groups.Exists(currentControlItemRow.ParentID))
			{
				groupToAddControl = this.AccordionControl.Groups.FromKey(currentControlItemRow.ParentID);
				if (currentControlItemRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
				{
				    groupToAddControl.UserControlUrl = currentControlItemRow.ControlSource;
                    groupToAddControl.ReCreateChildControls();
                    groupToAddControl.UserControl.ID = currentControlItemRow.ID;
					groupToAddControl.Expanded = expandedPanel;
                    groupToAddControl.UserControl.Parent.ID = currentControlItemRow.ID + "Parent";
                    return (ICOENavigationPanelControl) groupToAddControl.UserControl;
				}

				//#if !DEBUG
				//Remove groups 
				/*int numberOfGroups = this.AccordionControl.Groups.Count;
				for (int i = 0; i < numberOfGroups; i++)
				{
					if (groupToAddControl != this.AccordionControl.Groups[i])
					{
						this.AccordionControl.Groups.RemoveAt(i);
						if (this.AccordionControl.Groups.Count == 1)
							i = numberOfGroups;
						else
							i--;
					}
				}*/
			}
		}
        return null;
	}

    ///// <summary>
    ///// This method ocurrs when a accordion group is exapanded
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //protected void UltraWebListbarControl_GroupClicked(object sender, WebListbarGroupEvent e)
    //{
    //    //string ticket = Request[RegistrationWebApp.Constants.Ticket_UrlParameter] != null ? RegistrationWebApp.Constants.Ticket_UrlParameter + "=" + Request[RegistrationWebApp.Constants.Ticket_UrlParameter] : string.Empty;
    //    string ticket = "";
    //    try
    //    {
    //        if (!string.IsNullOrEmpty(ticket) && e.Group.TargetUrl.Contains("?"))
    //            Server.Transfer(e.Group.TargetUrl + "&" + ticket, false);
    //        else
    //            Server.Transfer(e.Group.TargetUrl + "?" + ticket, false);
    //    }
    //    catch (Exception exception)
    //    {
    //        //if (ExceptionPolicy.HandleException(exception, GUIShellTypes.PoliciesNames.Reg_UIExceptionPolicy.ToString()))
    //        //    throw;
    //        //else
    //        //    Server.Transfer(Resource.Messages_Page_URL + "?MessageCode=" + CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString() + "&" + ticket);
    //    }
    //    //}
    //}

	public virtual ICOENavigationPanelControl GetICOENavigationPanelControlInsideAnAccordionGroup(string groupKey)
	{
		return (ICOENavigationPanelControl)this.AccordionControl.Groups.FromKey(groupKey).UserControl;
	}

	/// <summary>
	/// Method to display a table with all the Accordion Panels.
	/// </summary>
	/// <returns>An HTML Table with all the links inside</returns>
	/// <remarks>The Accordion Panel must have set previously</remarks>
	public HtmlTable GetAcordionPanelActionsTable()
	{
		HtmlTable linksTable = new HtmlTable();
		foreach (Infragistics.WebUI.UltraWebListbar.Group currentGroup in this.AccordionControl.Groups)
		{
			HtmlTableRow currentRow = new HtmlTableRow();
			HtmlTableCell currentCell = new HtmlTableCell();
			HtmlTableCell currentImageCell = new HtmlTableCell();
			HtmlAnchor currentLink = new HtmlAnchor();
			Image currentImage = new Image();
			string splitter = "/";
			string goToURL = HttpContext.Current.Request.Url.Scheme.ToString()
						+ "://"
						+ HttpContext.Current.Request.Url.Authority.ToString()
						+ Page.TemplateSourceDirectory
						+ splitter
						+ currentGroup.TargetUrl.Split(splitter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[currentGroup.TargetUrl.Split(splitter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length - 1];

			currentLink.HRef = goToURL;
			currentLink.Title = currentGroup.ToolTip;
			currentLink.InnerText = currentGroup.Text;
			currentLink.Attributes.Add("class", "HomePageLinks");

			currentImage.ImageUrl = "~/Forms/ContentArea/Images/rightArrow2.gif";
			currentImage.ToolTip = currentGroup.ToolTip;
			currentImage.Attributes.Add("OnClick", "window.location.href=" + "'" + goToURL + "'");
			currentImage.CssClass = "HomePageLinkImages";

			currentCell.Controls.Add(currentLink);
			currentImageCell.Controls.Add(currentImage);
			currentRow.Controls.Add(currentCell);
			currentRow.Controls.Add(currentImageCell);
			linksTable.Controls.Add(currentRow);
		}
		return linksTable;
	}


	#endregion


}


using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Infragistics.WebUI.UltraWebListbar;
using Infragistics.WebUI.UltraWebToolbar;
using CambridgeSoft.COE.Framework.GUIShell;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using Infragistics.WebUI.Misc;
using Resources;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.DocumentManager.Services.Types;

public partial class DocManagerMaster : GUIShellMaster
{
    #region Constants
    private const string SUBMIT_DOCS = "SUBMIT_DOCS";
    private const string SEARCH_DOCS = "SEARCH_DOCS";
    private const string RECENT_ACTIVITIES = "VIEW_HISTORY";
    private const string DELETE_MY_DOCS = "DELETE_MY_DOCS";
    private const string DELETE_ALL_DOCS = "DELETE_ALL_DOCS";

    
    #endregion

    #region Variables

    COELogo COELogoObject;
    COEMenu COEMenuObject;
    COEMenu COEToolbarObject;
    COENavigationPane COENavigationPaneObject;

    #endregion

    #region Properties

    public string FormGroupIdKey;
    
    public int PageFormGroupID
    {
        get
        {
            string queryStringValue = Request.QueryString[FormGroupIdKey];
            if (Request.QueryString[FormGroupIdKey] == null) //If the query string is null, so the id (default) must be taken from the page
                Session[FormGroupIdKey] = Utilities.GetCOEFormID(FormGroupIdKey);
            else if (Session[FormGroupIdKey] == null ||
                    (!string.IsNullOrEmpty(queryStringValue) && Request.QueryString[FormGroupIdKey] != Session[FormGroupIdKey].ToString()))
                Session[FormGroupIdKey] = int.Parse(Request.QueryString[FormGroupIdKey]);
            return (int)Session[FormGroupIdKey];
        }
    }
    private Uri[] UrlHistory
    {
        get
        {
            if (Session["UrlHistory"] == null)
                Session["UrlHistory"] = new Uri[2] { new Uri(Request.Url.Scheme + "://" + Request.Url.Host + this.ResolveUrl(Resource.Home_URL)), null };

            return (Uri[])Session["UrlHistory"];
        }
    }

    public Uri CurrentPage
    {
        get
        {
            return UrlHistory[0];
        }
        set
        {
            if (UrlHistory[0] == null || !UrlHistory[0].Equals(value))
            {
                if (UrlHistory[1] == null || !UrlHistory[1].Equals(UrlHistory[0]))
                    UrlHistory[1] = UrlHistory[0];

                UrlHistory[0] = value;
            }
        }
    }

    public Uri PreviousPage
    {
        get
        {
            return UrlHistory[1] == null ? Request.Url : UrlHistory[1];
        }
    }
    
    #endregion

    #region Event Handlers
    
    protected override void OnInit(EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.LoadInformation();
            this.SetHeaderAttributes();
            this.SetNavigationPaneAttributtes(COENavigationPaneObject);

            CurrentPage = this.GetCurrentPage();
            this.Panel.Src = Utilities.ThemesCommonImagesPath + "Collapse.png";
        }
        this.ErrorControlHidden(true);
        this.SetJScriptReference("DocManagerUtils", Constants.PublicCommonJScriptsPath);
        if (Session["PluginDownloadURL"] == null)
        {
            Session["PluginDownloadURL"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(CambridgeSoft.COE.Framework.Common.COEAppName.Get()).PluginDownloadURL;
        }
        //First time keep in session the value so no need to read several time the same value.
        if (Session["ShowLeftPanel"] == null)
            Session["ShowLeftPanel"] = Utilities.GetDisplayLeftPanelVisibility();
        //Set the value, but could be overwritten by the page. E.g Home page. Default value is the given by config.
        _showLeftPanel = (bool)Session["ShowLeftPanel"];
        base.OnInit(e);
    }

    private Uri GetCurrentPage()
    {
        return new Uri(Request.Url.Scheme +
                        "://" +
                        Request.Url.Authority +
                        Page.ResolveUrl(((System.Web.UI.TemplateControl)(Page)).AppRelativeVirtualPath) +
                        (string.IsNullOrEmpty(Page.ClientQueryString) ? string.Empty : "?" + Page.ClientQueryString));

        //return Request.Url;
    }

    protected override void OnPreRender(EventArgs e)
    {
        this.WriteJsCodeForPanel();
        //If there is no group added, no need to show the accordion (the space can be re-used).
        if (!this.ShowLeftPanel)
            this.HideLeftPanel();

        base.OnPreRender(e);
    }

    #endregion

    #region Methods

    private void HideLeftPanel()
    {
        this.LeftPanelContainer.Visible = false;
        this.MainContentContainer.Width = new Unit("100%");
        this.ContentAreaRow.ColSpan = 2;
    }

    /// <summary>
    /// Writes the required code to Expand/Collapse the left panel
    /// </summary>
    private void WriteJsCodeForPanel()
    {
        string leftPanelCode = "LeftPanelCode";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(DocManagerMaster), leftPanelCode))
        {
            string script = "<script type=\"text/javascript\">";
            script += "var _pinned = true;";
            script += "function toggleContainer(){";
            script += "var theListBar = iglbar_getListbarById('" + this.UltraWebListbarControl.ID + "');";
            script += "var panel = document.getElementById('" + this.Panel.ClientID + "');";
            script += " if(_pinned) {";
            script += " theListBar.Element.style.display = 'none';";
            script += "  panel.src = '" + this.Page.ResolveClientUrl(Utilities.ThemesCommonImagesPath + "Expand.png") + "';";
            script += "  panel.parentNode.style.height = panel.style.height = '705px';";
            script += "  panel.parentNode.style.width = panel.style.width = '15px';";
            script += " panel.alt = '" + Resource.Expand_Label_Text + "';";
            script += "} else {";
            script += "theListBar.Element.style.display = '';";
            script += "  panel.src = '" + this.Page.ResolveClientUrl(Utilities.ThemesCommonImagesPath + "Collapse.png") + "';";
            script += "  panel.parentNode.style.height = panel.style.height = '19px';";
            script += "  panel.parentNode.style.width = panel.style.width = '257px';";
            script += " panel.alt = '" + Resource.Collapse_Label_Text + "';";
            script += "}";
            script += "document.getElementById('" + this.LeftPanelStateHidden.ClientID + "').value = new String(_pinned);";
            script += "_pinned = !_pinned;";
            script += "}";
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(DocManagerMaster), leftPanelCode, script);
        }
    }


    /// <summary>
    /// This Method is just for testing purpose.
    /// In the future, all the info about the Shell will come (for example) from a Business Object 
    /// calling the Configuration Services.
    /// This Method loads all the information in the typed DataSets.
    /// </summary>
    private void LoadInformation()
    {
        #region CoeLogo Info SetUp

        COELogoObject = new COELogo();
        COELogo.LogoItemRow logoRow = COELogoObject.LogoItem.NewLogoItemRow();
        //logoRow.ToolTip = "CambridgeSoft DocManager Enterprise 12.1.0";
        logoRow.ToolTip = GetVersion();
        logoRow.URL = this.Page.ResolveClientUrl(@"~/Forms/Public/ContentArea/Home.aspx?");
        COELogoObject.LogoItem.AddLogoItemRow(logoRow);

        #endregion

        #region Toolbar Info SetUp

        COEToolbarObject = new COEMenu();

        COEMenu.MenuItemRow homeItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        homeItemRow.Key = "Home";
        homeItemRow.ParentKey = String.Empty;
        homeItemRow.TitleText = "Home";
        homeItemRow.URL = Utilities.GetConfigSetting("MISC", "HomeLinkURL");
        if (string.IsNullOrEmpty(homeItemRow.URL))
            homeItemRow.URL = this.Page.ResolveUrl(Resource.ManagerHome_URL);
        homeItemRow.URL += homeItemRow.URL.Contains("?") ? "&" : "?";
        homeItemRow.TargetFrame = "_self";
        homeItemRow.Key = "Home";
        homeItemRow.ToolTip = "Home Page";
        homeItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(homeItemRow);

        COEMenu.MenuItemRow mainItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        mainItemRow.Key = "Main";
        mainItemRow.ParentKey = String.Empty;
        mainItemRow.TitleText = "Main";
        mainItemRow.URL = this.Page.ResolveUrl(Resource.Home_URL);
        mainItemRow.TargetFrame = "_self";
        mainItemRow.Key = "Main";
        mainItemRow.ToolTip = "Main Page";
        mainItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(mainItemRow);

        COEMenu.MenuItemRow helpItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        helpItemRow.Key = "Help";
        helpItemRow.ParentKey = String.Empty;
        helpItemRow.TitleText = "Help";
        helpItemRow.URL = this.Page.ResolveUrl(Resource.UserHelp_URL);
        helpItemRow.Key = "Help";
        helpItemRow.TargetFrame = "_blank";
        helpItemRow.ToolTip = "Help Page";
        helpItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(helpItemRow);
        
        COEMenu.MenuItemRow aboutItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        aboutItemRow.Key = "About";
        aboutItemRow.ParentKey = String.Empty;
        aboutItemRow.TitleText = "About";
        aboutItemRow.URL = this.Page.ResolveUrl(Resource.About_URL);
        aboutItemRow.TargetFrame = "_self";
        aboutItemRow.Key = "About";
        aboutItemRow.ToolTip = "About";
        aboutItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(aboutItemRow);

        #endregion

        #region CoeNavigation Pane Info SetUp

        COENavigationPaneObject = new COENavigationPane();        

        COENavigationPane.NavigationPaneItemRow SubmitNewDocumentRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        SubmitNewDocumentRow.ID = "SubmitNewDocument";
        SubmitNewDocumentRow.Title = "Submit New Document";
        SubmitNewDocumentRow.EnableToolTip = "Submit New Document";
        SubmitNewDocumentRow.DisableToolTip = "Submit New Document";
        SubmitNewDocumentRow.TargetURL = this.Page.ResolveUrl(Resource.SubmitNewDocument_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(SubmitNewDocumentRow);

        COENavigationPane.NavigationPaneItemRow searchDocumentsRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        searchDocumentsRow.ID = "SearchDocuments";
        searchDocumentsRow.Title = "Search Documents";
        searchDocumentsRow.EnableToolTip = "Search Documents";
        searchDocumentsRow.DisableToolTip = "Review details of documents";
        searchDocumentsRow.TargetURL = this.Page.ResolveUrl(Resource.SearchDocuments_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(searchDocumentsRow);

        COENavigationPane.NavigationPaneItemRow RecentActivitiesRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        RecentActivitiesRow.ID = "RecentActivities";
        RecentActivitiesRow.Title = "Recent Activities";
        RecentActivitiesRow.EnableToolTip = "Recent Activities";
        RecentActivitiesRow.DisableToolTip = "Recent Activities";
        RecentActivitiesRow.TargetURL = this.Page.ResolveUrl(Resource.RecentActivities_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(RecentActivitiesRow);
                
        #endregion

        #region CoeMenuObject Info Set Up

        COEMenuObject = new COEMenu();

        /*  GOTO menu */
        COEMenu.MenuItemRow rootSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        rootSectionItemRow.ParentLevel = -1;
        rootSectionItemRow.Key = "GoTo_0";
        rootSectionItemRow.ParentKey = String.Empty;
        rootSectionItemRow.TitleText = rootSectionItemRow.ToolTip = Resource.GoTo_MenuItem_Text;
        rootSectionItemRow.URL = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
        rootSectionItemRow.TargetFrame = "_self";
        rootSectionItemRow.HasChildItems = bool.TrueString;
        COEMenuObject.MenuItem.AddMenuItemRow(rootSectionItemRow);

        /* Goto - Submit New Document */
        if (HttpContext.Current.User.IsInRole(SUBMIT_DOCS))
        {
            COEMenu.MenuItemRow submitNewDocumentItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
            submitNewDocumentItemRow.ParentLevel = 0;
            submitNewDocumentItemRow.Key = "SubmitNewDocument_0_0";
            submitNewDocumentItemRow.ParentKey = "GoTo_0";
            submitNewDocumentItemRow.TitleText = Resource.SubmitDocument_MenuItem_Text;
            submitNewDocumentItemRow.ToolTip = Resource.SubmitDocument_MenuItem_Text;
            submitNewDocumentItemRow.URL = this.Page.ResolveUrl(Resource.SubmitNewDocument_URL + "?Caller=SN");            
            submitNewDocumentItemRow.TargetFrame = "_self";
            submitNewDocumentItemRow.HasChildItems = bool.FalseString;
            COEMenuObject.MenuItem.AddMenuItemRow(submitNewDocumentItemRow);
        }

        /* Goto - Search Documents */
        if (HttpContext.Current.User.IsInRole(SEARCH_DOCS))
        {
            COEMenu.MenuItemRow searchDocumentsSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
            searchDocumentsSectionItemRow.ParentLevel = 0;
            searchDocumentsSectionItemRow.Key = "SearchDocuments_0_1";
            searchDocumentsSectionItemRow.ParentKey = "GoTo_0";
            searchDocumentsSectionItemRow.TitleText = Resource.Search_MenuItem_Text;
            searchDocumentsSectionItemRow.ToolTip = Resource.Search_MenuItem_Tooltip;
            searchDocumentsSectionItemRow.URL = this.Page.ResolveUrl(Resource.SearchDocuments_URL + "?Caller=SD");
            searchDocumentsSectionItemRow.TargetFrame = "_self";
            searchDocumentsSectionItemRow.HasChildItems = bool.FalseString;
            COEMenuObject.MenuItem.AddMenuItemRow(searchDocumentsSectionItemRow);
        }

        /* Goto - Recent Activities */
        if (HttpContext.Current.User.IsInRole(RECENT_ACTIVITIES))
        {
            COEMenu.MenuItemRow recentActivitiesItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
            recentActivitiesItemRow.ParentLevel = 0;
            recentActivitiesItemRow.Key = "RecentActivities_0_2";
            recentActivitiesItemRow.ParentKey = "GoTo_0";
            recentActivitiesItemRow.TitleText = Resource.RecentActivities_MenuItem_Text;
            recentActivitiesItemRow.ToolTip = Resource.RecentActivities_MenuItem_Tooltip;
            recentActivitiesItemRow.URL = this.Page.ResolveUrl(Resource.RecentActivities_URL + "?Caller=RA");
            recentActivitiesItemRow.TargetFrame = "_self";
            recentActivitiesItemRow.HasChildItems = bool.FalseString;
            COEMenuObject.MenuItem.AddMenuItemRow(recentActivitiesItemRow);
        }
        
        #endregion
    }

    public static string GetVersion()
    {
        System.Reflection.Assembly MyAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Version AppVersion = MyAssembly.GetName().Version;
        return string.Format("{0} {1}", Resource.Logo_Image_Tooltip,AppVersion.ToString());
    }

    /// <summary>
    /// This method sets all the Accordion's Panels.
    /// </summary>
    /// <param name="COENavigationPaneParam">Object with all the info to display in the COENavigationPane Section</param>
    /// <remarks>This method doesn't add the control inside an accordion panel</remarks>
    private void SetNavigationPaneAttributtes(COENavigationPane COENavigationPaneParam)
    {
        for (int i = 0; i < COENavigationPaneParam.NavigationPaneItem.Count; i++)
        {
            Group currentGroup = this.AccordionControl.Groups.Add(COENavigationPaneParam.NavigationPaneItem[i].Title.ToString(), COENavigationPaneParam.NavigationPaneItem[i].ID.ToString());
            currentGroup.ReCreateChildControls(); // This is a Infragistics tip - After creating a group call this method
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
    public void AddControlToAccordionPanel(COENavigationPane.ControlItemRow currentControlItemRow, bool expandedPanel)
    {
        if (currentControlItemRow != null)
        {
            Group groupToAddControl = null;
            if (this.AccordionControl.Groups.Exists(currentControlItemRow.ParentID))
            {
                groupToAddControl = this.AccordionControl.Groups.FromKey(currentControlItemRow.ParentID);
                if (currentControlItemRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
                {
                    groupToAddControl.UserControlUrl = currentControlItemRow.ControlSource;
                    groupToAddControl.ReCreateChildControls(); //Instantiate control inside group.  
                    ((ICOENavigationPanelControl)groupToAddControl.UserControl).ID = currentControlItemRow.ID;
                    groupToAddControl.Expanded = expandedPanel;
                }

                //#if !DEBUG
                //Remove groups 
                int numberOfGroups = this.AccordionControl.Groups.Count;
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
                }
            }
        }
    }

    /// <summary>
    /// This method ocurrs when a accordion group is exapanded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UltraWebListbarControl_GroupClicked(object sender, WebListbarGroupEvent e)
    {
        string ticket = Request[Constants.Ticket_UrlParameter] != null ? Constants.Ticket_UrlParameter + "=" + Request[Constants.Ticket_UrlParameter] : string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(ticket) && e.Group.TargetUrl.Contains("?"))
                Server.Transfer(e.Group.TargetUrl + "&" + ticket, false);
            else
                Server.Transfer(e.Group.TargetUrl + "?" + ticket, false);
        }
        catch (Exception exception)
        {
            if (ExceptionPolicy.HandleException(exception, Constants.DOC_GUI_POLICY))
                throw;
            else
                Server.Transfer(Resource.Messages_Page_URL + "?MessageCode=" + CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString() + "&" + ticket);
        }
        //}
    }

    public virtual ICOENavigationPanelControl GetICOENavigationPanelControlInsideAnAccordionGroup(string groupKey)
    {
        try { return (ICOENavigationPanelControl)this.AccordionControl.Groups.FromKey(groupKey).UserControl; }
        catch { return null; }
    }

    /// <summary>
    /// Method to display a table with all the Accordion Panels.
    /// </summary>
    /// <returns>An HTML Table with all the links inside</returns>
    /// <remarks>The Accordion Panel must have set previously</remarks>
    public HtmlTable GetAcordionPanelActionsTable()
    {
        HtmlTable linksTable = new HtmlTable();
        foreach (Group currentGroup in this.AccordionControl.Groups)
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

    /// <summary>
    /// Method to display confirmation messages in the top of page .
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <remarks>If you want to hide the control, pass it a null or empty parameter</remarks>
    public override void DisplayConfirmationMessage(string message)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control confirmationArea = this.FindControl(Constants.ContentPlaceHolderID).FindControl(Constants.MessagesAreaUCID);
        if (confirmationArea != null)
        {
            if (confirmationArea is DocManagerWeb.Forms.Public.UserControls.MessagesArea)
                ((DocManagerWeb.Forms.Public.UserControls.MessagesArea)confirmationArea).AreaText = !string.IsNullOrEmpty(message) ? message : String.Empty;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <remarks>If you want to hide the control, pass it a null or empty parameter</remarks>
    public override void DisplayErrorMessage(string message, bool showBackLink)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(message))
        {
            string friendlyMessage = Utilities.ConvertToFriendly(message);
            //Find ctrl
            Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(Constants.ErrorYUIPanelID);
            if (ErrorCtrl != null)
            {
                if (ErrorCtrl is ErrorControl)
                {
                    ((ErrorControl)ErrorCtrl).Hidden = false;
                    ((ErrorControl)ErrorCtrl).BodyText = friendlyMessage;
                    ((ErrorControl)ErrorCtrl).ErrorDescription = message;
                    ((ErrorControl)ErrorCtrl).FooterText = "Send report";
                    ((ErrorControl)ErrorCtrl).EmailAccount = Utilities.GetConfigSetting("MISC", "EmailAccount");
                    ((ErrorControl)ErrorCtrl).EmailSubject = Utilities.GetConfigSetting("MISC", "EmailSubject");
                    ((ErrorControl)ErrorCtrl).HeaderText = Utilities.GetConfigSetting("MISC", "HeaderText");
                    ((ErrorControl)ErrorCtrl).HeaderTextClass = Utilities.GetConfigSetting("MISC", "HeaderTextClass");
                    ((ErrorControl)ErrorCtrl).BodyTextClass = Utilities.GetConfigSetting("MISC", "BodyTextClass");
                    ((ErrorControl)ErrorCtrl).FooterTextClass = Utilities.GetConfigSetting("MISC", "FooterTextClass");
                    ((ErrorControl)ErrorCtrl).GoBackLink = false;
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    //0806modified
    public void DisplayErrorMessageByException(Exception ex, bool showBackLink)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(ex.Message))
        {
            string friendlyMessage = Utilities.ConvertToFriendly(ex.Message);
            //Find ctrl
            Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(Constants.ErrorYUIPanelID);
            if (ErrorCtrl != null)
            {
                if (ErrorCtrl is ErrorControl)
                {
                    ((ErrorControl)ErrorCtrl).Hidden = false;
                    ((ErrorControl)ErrorCtrl).ClipBoardMessage = Utilities.GetClipboardMessageByException(ex);
                    ((ErrorControl)ErrorCtrl).BodyText = friendlyMessage;
                    ((ErrorControl)ErrorCtrl).ErrorDescription = ex.Message;
                    ((ErrorControl)ErrorCtrl).FooterText = "Send report";
                    ((ErrorControl)ErrorCtrl).EmailAccount = Utilities.GetConfigSetting("MISC", "EmailAccount");
                    ((ErrorControl)ErrorCtrl).EmailSubject = Utilities.GetConfigSetting("MISC", "EmailSubject");
                    ((ErrorControl)ErrorCtrl).HeaderText = Utilities.GetConfigSetting("MISC", "HeaderText");
                    ((ErrorControl)ErrorCtrl).HeaderTextClass = Utilities.GetConfigSetting("MISC", "HeaderTextClass");
                    ((ErrorControl)ErrorCtrl).BodyTextClass = Utilities.GetConfigSetting("MISC", "BodyTextClass");
                    ((ErrorControl)ErrorCtrl).FooterTextClass = Utilities.GetConfigSetting("MISC", "FooterTextClass");
                    ((ErrorControl)ErrorCtrl).GoBackLink = false;
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    
    /// <summary>
    /// Error Control visibility
    /// </summary>
    /// <param name="status"></param>
    private void ErrorControlHidden(bool status)
    {
        Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(Constants.ErrorYUIPanelID);
        if (ErrorCtrl is ErrorControl)
            ((ErrorControl)ErrorCtrl).Hidden = status;
    }    

    /// <summary>
    /// Method to be redirected to the messages page and display a message
    /// </summary>
    /// <param name="textToDisplay">Text to display</param>
    /// <param name="displayBackButton">Display or not the back button</param>
    public void DisplayMessagesPage(GUIShellTypes.MessagesCode messageToDisplay, GUIShellTypes.MessagesButtonType buttonType)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = Resource.Messages_Page_URL + "?";
        url += GUIShellTypes.RequestVars.MessageCode.ToString() + "=" + messageToDisplay.ToString();
        url += "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() + "=" + buttonType.ToString();
        Server.Transfer(url, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Load the control/s in the right accordion's panel
    /// </summary>
    /// <remarks>You should show as expanded the group that you are interested</remarks> 
    public virtual void LoadControlInAccordionPanel(COENavigationPane COENavigationPaneObject)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (COENavigationPaneObject != null)
        {
            for (int i = 0; i < COENavigationPaneObject.ControlItem.Count; i++)
            {
                this.AddControlToAccordionPanel(COENavigationPaneObject.ControlItem[i], true);
            }
        }
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
            throw new Exception(Resource.UnsupportedHeader_Label_Text);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

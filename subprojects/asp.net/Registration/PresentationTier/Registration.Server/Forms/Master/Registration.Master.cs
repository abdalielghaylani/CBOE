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
using RegistrationWebApp;
using PerkinElmer.COE.Registration.Server;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Controls;

public partial class RegistrationMaster : GUIShellMaster
{
    #region Constants
    private const string ADD_COMPOUND_TEMP = "ADD_COMPOUND_TEMP";
    private const string CREATE_MIXTURE = "ADD_COMPONENT";
    private const string SEARCH_TEMP = "SEARCH_TEMP";
    private const string SEARCH_REG = "SEARCH_REG";
    private const string CONFIG_REG = "CONFIG_REG";
    
    #endregion

    #region Variables

    COELogo COELogoObject;
    COEMenu COEMenuObject;
    COEMenu COEToolbarObject;
    COENavigationPane COENavigationPaneObject;

    #endregion

    #region Properties

    public string FormGroupIdKey;
    /*{
        set 
        {
            ViewState["FormGroupIdKey"] = value;
            Session[FormGroupIdKey] = null;
        }
        get 
        { 
            if(ViewState["FormGroupIdKey"] == null)
                ViewState["FormGroupIdKey"] = string.Empty;

            return ViewState["FormGroupIdKey"].ToString();
        }
    }*/
    public int PageFormGroupID
    {
        get
        {
            string queryStringValue = Request.QueryString[FormGroupIdKey];
            if (Request.QueryString[FormGroupIdKey] == null) //If the query string is null, so the id (default) must be taken from the page
                Session[FormGroupIdKey] = RegUtilities.GetCOEFormID(FormGroupIdKey);
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

    public string AvailableRecord
    {
        get
        {
            if (Session[Constants.AvailableRecord] == null)
                Session[Constants.AvailableRecord] = "YES";

            return Session[Constants.AvailableRecord] as string;
        }
    }
    #endregion

    #region Event Handlers
    /*protected void PreInit(EventArgs e)
    {
        if(!Page.IsPostBack)
            CurrentPage = this.Request.Url;
    }*/

    protected override void OnInit(EventArgs e)
    {
        if (Request !=null && Request.Browser!=null && (Request.Browser.Browser.ToLower()==("mozilla") || ((Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer") && Request.Browser.MajorVersion >= 10)))
            Page.ClientTarget = "iecustom";
        if (!Page.IsPostBack)
        {
            this.LoadInformation();
            this.SetHeaderAttributes();
            this.SetNavigationPaneAttributtes(COENavigationPaneObject);

            CurrentPage = this.GetCurrentPage();
            this.Panel.Src = RegUtilities.ThemesCommonImagesPath + "Collapse.png";
        }
        this.ErrorControlHidden(true);
        this.SetJScriptReference("RegistrationUtils", PerkinElmer.COE.Registration.Server.Constants.PublicCommonJScriptsPath);
        if (Session["PluginDownloadURL"] == null)
        {
            Session["PluginDownloadURL"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(CambridgeSoft.COE.Framework.Common.COEAppName.Get()).PluginDownloadURL;
        }
        //First time keep in session the value so no need to read several time the same value.
        if (Session["ShowLeftPanel"] == null)
            Session["ShowLeftPanel"] = RegUtilities.GetDisplayLeftPanelVisibility();
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
        if (this.AvailableRecord == "NO" && !this.Request.Url.AbsoluteUri.Contains(Page.ResolveUrl(Resource.RegAdmin_ImportCustomization_URL)))
        {
            Session[Constants.AvailableRecord] = null;
            Response.Redirect(string.Format("{0}?{1}=YES", Resource.RegAdmin_ImportCustomization_URL, Constants.MustImportCustomization_URLParameter));
        }
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
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(RegistrationMaster), leftPanelCode))
        {
            string script = "<script type=\"text/javascript\">";
            script += "var _pinned = true;";
            script += "function toggleContainer(){";
            script += "var theListBar = iglbar_getListbarById('" + this.UltraWebListbarControl.ID + "');";
            script += "var panel = document.getElementById('" + this.Panel.ClientID + "');";
            script += " if(_pinned) {";
            script += " theListBar.Element.style.display = 'none';";
            script += "  panel.src = '" + this.Page.ResolveClientUrl(RegUtilities.ThemesCommonImagesPath + "Expand.png") + "';";
            script += "  panel.parentNode.style.height = panel.style.height = '705px';";
            script += "  panel.parentNode.style.width = panel.style.width = '15px';";
            script += " panel.alt = '" + Resource.Expand_Label_Text + "';";
            script += "} else {";
            script += "theListBar.Element.style.display = '';";
            script += "  panel.src = '" + this.Page.ResolveClientUrl(RegUtilities.ThemesCommonImagesPath + "Collapse.png") + "';";
            script += "  panel.parentNode.style.height = panel.style.height = '19px';";
            script += "  panel.parentNode.style.width = panel.style.width = '257px';";
            script += " panel.alt = '" + Resource.Collapse_Label_Text + "';";
            script += "}";
            script += "document.getElementById('" + this.LeftPanelStateHidden.ClientID + "').value = new String(_pinned);";
            script += "_pinned = !_pinned;";
            script += "}";
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(RegistrationMaster), leftPanelCode, script);
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
        logoRow.ToolTip = Resource.Logo_Image_Tooltip;
        //logoRow.URL = "Home.aspx?";
        logoRow.URL = this.Page.ResolveClientUrl(@"~/Forms/Public/ContentArea/Home.aspx?");
        COELogoObject.LogoItem.AddLogoItemRow(logoRow);

        #endregion

        #region Toolbar Info SetUp

        COEToolbarObject = new COEMenu();

        COEMenu.MenuItemRow homeItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        homeItemRow.Key = "Home";
        homeItemRow.ParentKey = String.Empty;
        homeItemRow.TitleText = "Home";
        homeItemRow.URL = RegUtilities.GetConfigSetting(RegUtilities.Groups.Misc, "HomeLinkURL");
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

        //COEMenu.MenuItemRow logOffItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        //logOffItemRow.Key = "LogOff";
        //logOffItemRow.ParentKey = String.Empty;
        //logOffItemRow.TitleText = "Log Off";
        //logOffItemRow.URL = string.Empty;
        //logOffItemRow.TargetFrame = "_self";
        //logOffItemRow.Key = "LogOff";
        //logOffItemRow.ToolTip = "Log Off";
        //logOffItemRow.HasChildItems = "false";
        //COEToolbarObject.MenuItem.AddMenuItemRow(logOffItemRow);

        // Fix for CSBR-157658
        // Reworking on the About link to open in a new window to resolve the menu alignement issue.
        COEMenu.MenuItemRow aboutItemRow = COEToolbarObject.MenuItem.NewMenuItemRow();
        aboutItemRow.Key = "About";
        aboutItemRow.ParentKey = String.Empty;
        aboutItemRow.TitleText = "About";
        aboutItemRow.URL = "javascript:window.open('" + this.Page.ResolveUrl(Resource.About_URL) + "','','scrollbars=no,height=600,width=800,menubar=no,toolbar=no,location=no,status=no');";       
        aboutItemRow.TargetFrame = "_self"; 
        aboutItemRow.Key = "About";
        aboutItemRow.ToolTip = "About";
        aboutItemRow.HasChildItems = "false";
        COEToolbarObject.MenuItem.AddMenuItemRow(aboutItemRow);

        #endregion

        #region CoeNavigation Pane Info SetUp

        COENavigationPaneObject = new COENavigationPane();

        COENavigationPane.NavigationPaneItemRow registerRecordRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        registerRecordRow.ID = "SubmitRecord";
        registerRecordRow.Title = "Submit Record";
        registerRecordRow.EnableToolTip = "Add new Record";
        registerRecordRow.DisableToolTip = "To submit a new record you must cancel or finish the current action";
        registerRecordRow.TargetURL = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + "?RegistryType=Mixture" + (!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("?{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(registerRecordRow);

        COENavigationPane.NavigationPaneItemRow registerComponentRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        registerComponentRow.ID = "SubmitComponent";
        registerComponentRow.Title = "Submit Component";
        registerComponentRow.EnableToolTip = "Add new Component";
        registerComponentRow.DisableToolTip = "To submit a new component you must cancel or finish the current action";
        registerComponentRow.TargetURL = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + "?RegistryType=Component" + (!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("&{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(registerComponentRow);


        COENavigationPane.NavigationPaneItemRow registerMixtureRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        registerMixtureRow.ID = "SubmitMixture";
        registerMixtureRow.Title = "Submit Mixture";
        registerMixtureRow.EnableToolTip = "Create a mixture from registered components";
        registerMixtureRow.DisableToolTip = "To submit a new mixture you must cancel or finish the current action";
        registerMixtureRow.TargetURL = this.Page.ResolveUrl(Resource.SubmitMixture_URL) + "?RegistryType=Mixture" + (!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("?{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(registerMixtureRow);

        COENavigationPane.NavigationPaneItemRow viewMixtureRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        viewMixtureRow.ID = "ReviewRegister";
        viewMixtureRow.Title = "Review / Register";
        viewMixtureRow.EnableToolTip = "Review / Register";
        viewMixtureRow.DisableToolTip = "To Review/Register a component you must cancel or finish the current action";
        viewMixtureRow.TargetURL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=RR");
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(viewMixtureRow);

        COENavigationPane.NavigationPaneItemRow viewMixture2Row = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        viewMixture2Row.ID = "MixtureDuplicateResolution";
        viewMixture2Row.Title = "Mixture Duplicate Resolution";
        viewMixture2Row.EnableToolTip = "Mixture Duplicate Resolution";
        viewMixture2Row.DisableToolTip = "To Review/Register a mixture you must select a duplicate action for the record";
        viewMixture2Row.TargetURL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=RR");
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(viewMixture2Row);

        COENavigationPane.NavigationPaneItemRow viewComponent2Row = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        viewComponent2Row.ID = "ComponentDuplicateResolution";
        viewComponent2Row.Title = "Component Duplicate Resolution";
        viewComponent2Row.EnableToolTip = "Component Duplicate Resolution";
        viewComponent2Row.DisableToolTip = "To Review/Register a component you must select a duplicate action for the record";
        viewComponent2Row.TargetURL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=RR");
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(viewComponent2Row);

        COENavigationPane.NavigationPaneItemRow searchTempRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        searchTempRow.ID = "SearchTempRecord";
        searchTempRow.Title = "Search Temp";
        searchTempRow.EnableToolTip = "Search temporary registries";
        searchTempRow.DisableToolTip = "To search a temp component you must cancel or finish the current action";
        searchTempRow.TargetURL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=ST");
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(searchTempRow);

        COENavigationPane.NavigationPaneItemRow searchRegistryRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        searchRegistryRow.ID = "SearchRegistryRecord";
        searchRegistryRow.Title = "Registry Record";
        searchRegistryRow.EnableToolTip = "Registry Record";
        searchRegistryRow.DisableToolTip = "Review details of currect registry";
        searchRegistryRow.TargetURL = this.Page.ResolveUrl(Resource.ViewMixtureSearch_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(searchRegistryRow);

        /*
        COENavigationPane.NavigationPaneItemRow moveDeleteRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        moveDeleteRow.ID = "MoveDelete";
        moveDeleteRow.Title = "Move Delete";
        moveDeleteRow.EnableToolTip = "Move Delete";
        moveDeleteRow.DisableToolTip = "Move or delete a Registry, Compound or Batch";
        moveDeleteRow.TargetURL = this.Page.ResolveUrl(Resource.MoveDelete_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(moveDeleteRow);
         * */

        COENavigationPane.NavigationPaneItemRow tableEditorRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        tableEditorRow.ID = "TableEditor";
        tableEditorRow.Title = searchRegistryRow.EnableToolTip = "Settings - Table Editor";
        tableEditorRow.DisableToolTip = "Edit tables content";
        tableEditorRow.TargetURL = this.Page.ResolveUrl(Resource.TableEditor_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(tableEditorRow);

        COENavigationPane.NavigationPaneItemRow regAdminRow = COENavigationPaneObject.NavigationPaneItem.NewNavigationPaneItemRow();
        regAdminRow.ID = "RegAdmin";
        regAdminRow.Title = searchRegistryRow.EnableToolTip = "Settings - Reg Admin";
        regAdminRow.DisableToolTip = "Edit Registration settings (object, database, etc)";
        regAdminRow.TargetURL = this.Page.ResolveUrl(Resource.RegAdmin_URL);
        COENavigationPaneObject.NavigationPaneItem.AddNavigationPaneItemRow(regAdminRow);

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

        ///this code is obsolete
        ///* Goto - Submit Record */
        //if (HttpContext.Current.User.IsInRole(ADD_COMPOUND_TEMP))
        //{
        //COEMenu.MenuItemRow submitRecordSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        //submitRecordSectionItemRow.ParentLevel = 0;
        //submitRecordSectionItemRow.Key = "SubmitRecord_0_0";
        //submitRecordSectionItemRow.ParentKey = "GoTo_0";
        //submitRecordSectionItemRow.TitleText = Resource.SubmitRecord_MenuItem_Text;
        //submitRecordSectionItemRow.ToolTip = Resource.SubmitRecord_MenuItem_Tooltip;
        //submitRecordSectionItemRow.URL = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + "?RegistryType=Mixture" +(!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("?{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
        //submitRecordSectionItemRow.TargetFrame = "_self";
        //submitRecordSectionItemRow.HasChildItems = bool.FalseString;
        //COEMenuObject.MenuItem.AddMenuItemRow(submitRecordSectionItemRow);
        //}

        /* Goto - Submit Record */
        if (HttpContext.Current.User.IsInRole(ADD_COMPOUND_TEMP))
        {
            COEMenu.MenuItemRow submitComponentSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
            submitComponentSectionItemRow.ParentLevel = 0;
            submitComponentSectionItemRow.Key = "SubmitComponent_0_0";
            submitComponentSectionItemRow.ParentKey = "GoTo_0";
            submitComponentSectionItemRow.TitleText = Resource.SubmitComponent_MenuItem_Text;
            submitComponentSectionItemRow.ToolTip = Resource.SubmitComponent_MenuItem_Tooltip;
            submitComponentSectionItemRow.URL = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + "?RegistryType=Component" + (!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("&{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
            submitComponentSectionItemRow.TargetFrame = "_self";
            submitComponentSectionItemRow.HasChildItems = bool.FalseString;
            COEMenuObject.MenuItem.AddMenuItemRow(submitComponentSectionItemRow);
        }

        /* Goto - Submit Record */
        if (HttpContext.Current.User.IsInRole(CREATE_MIXTURE) && RegUtilities.GetMixturesEnabled())
        {
            COEMenu.MenuItemRow submitMixtureSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
            submitMixtureSectionItemRow.ParentLevel = 0;
            submitMixtureSectionItemRow.Key = "SubmitMixture_0_0";
            submitMixtureSectionItemRow.ParentKey = "GoTo_0";
            submitMixtureSectionItemRow.TitleText = Resource.SubmitMixture_MenuItem_Text;
            submitMixtureSectionItemRow.ToolTip = Resource.SubmitMixture_MenuItem_Tooltip;
            submitMixtureSectionItemRow.URL = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + "?RegistryType=Mixture" + (!string.IsNullOrEmpty(FormGroupIdKey) ? string.Format("&{0}={1}", FormGroupIdKey, this.PageFormGroupID) : string.Empty);
            submitMixtureSectionItemRow.TargetFrame = "_self";
            submitMixtureSectionItemRow.HasChildItems = bool.FalseString;
            COEMenuObject.MenuItem.AddMenuItemRow(submitMixtureSectionItemRow);
        }

        /* Goto - Review Register */
        if (HttpContext.Current.User.IsInRole(SEARCH_TEMP))
        {
        COEMenu.MenuItemRow reviewRecordSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        reviewRecordSectionItemRow.ParentLevel = 0;
        reviewRecordSectionItemRow.Key = "ReviewRecord_0_1";
        reviewRecordSectionItemRow.ParentKey = "GoTo_0";
        reviewRecordSectionItemRow.TitleText = Resource.ReviewRecord_MenuItem_Text;
        reviewRecordSectionItemRow.ToolTip = Resource.ReviewRecord_MenuItem_Tooltip;
        reviewRecordSectionItemRow.URL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=ALL");
        reviewRecordSectionItemRow.TargetFrame = "_self";
        reviewRecordSectionItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(reviewRecordSectionItemRow);
        }

        /* Goto - Search Temp */
        if (HttpContext.Current.User.IsInRole(SEARCH_TEMP))
        {
        COEMenu.MenuItemRow searchTempSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        searchTempSectionItemRow.ParentLevel = 0;
        searchTempSectionItemRow.Key = "SearchTemp_0_2";
        searchTempSectionItemRow.ParentKey = "GoTo_0";
        searchTempSectionItemRow.TitleText = Resource.SearchTemp_MenuItem_Text;
        searchTempSectionItemRow.ToolTip = Resource.SearchTemp_MenuItem_Tooltip;
        searchTempSectionItemRow.URL = this.Page.ResolveUrl(Resource.ReviewRegisterSearch_URL + "?Caller=ST");
        searchTempSectionItemRow.TargetFrame = "_self";
        searchTempSectionItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(searchTempSectionItemRow);
        }


        /* GoTo - Search Perm */
        if (HttpContext.Current.User.IsInRole(SEARCH_REG))
        {
        COEMenu.MenuItemRow searchSectionItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        searchSectionItemRow.ParentLevel = 0;
        searchSectionItemRow.Key = "Search_0_2";
        searchSectionItemRow.ParentKey = "GoTo_0";
        searchSectionItemRow.TitleText = Resource.Search_MenuItem_Text;
        searchSectionItemRow.ToolTip = Resource.Search_MenuItem_Tooltip;
        searchSectionItemRow.URL = this.Page.ResolveUrl(Resource.ViewMixtureSearch_URL);
        searchSectionItemRow.TargetFrame = "_self";
        searchSectionItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(searchSectionItemRow);
        }

        /* GoTo - Settings - Table Editor*/
        if (HttpContext.Current.User.IsInRole(CONFIG_REG))
        {
        COEMenu.MenuItemRow regAdminItemRow = COEMenuObject.MenuItem.NewMenuItemRow();
        regAdminItemRow.ParentLevel = 0;
        regAdminItemRow.Key = "RegAdmin_0_3";
        regAdminItemRow.ParentKey = "GoTo_0";
        regAdminItemRow.TitleText = Resource.RegAdmin_MenuItem_Text;
        regAdminItemRow.ToolTip = Resource.RegAdmin_MenuItem_Tooltip;
        regAdminItemRow.URL = this.Page.ResolveUrl(Resource.RegAdmin_URL);
        regAdminItemRow.TargetFrame = "_self";
        regAdminItemRow.HasChildItems = bool.FalseString;
        COEMenuObject.MenuItem.AddMenuItemRow(regAdminItemRow);
        }



        #endregion
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
        string ticket = Request[PerkinElmer.COE.Registration.Server.Constants.Ticket_UrlParameter] != null ? PerkinElmer.COE.Registration.Server.Constants.Ticket_UrlParameter + "=" + Request[PerkinElmer.COE.Registration.Server.Constants.Ticket_UrlParameter] : string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(ticket) && e.Group.TargetUrl.Contains("?"))
                Server.Transfer(e.Group.TargetUrl + "&" + ticket, false);
            else
                Server.Transfer(e.Group.TargetUrl + "?" + ticket, false);
        }
        catch (Exception exception)
        {
            if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
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
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control confirmationArea = this.FindControl(PerkinElmer.COE.Registration.Server.Constants.ContentPlaceHolderID).FindControl(PerkinElmer.COE.Registration.Server.Constants.MessagesAreaUCID);
        if (confirmationArea != null)
        {
            if (confirmationArea is PerkinElmer.COE.Registration.Server.Forms.Public.UserControls.MessagesArea)
                ((PerkinElmer.COE.Registration.Server.Forms.Public.UserControls.MessagesArea)confirmationArea).AreaText = !string.IsNullOrEmpty(message) ? message : String.Empty;
        }
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="exception">exception from wich message should be displayed</param>
    /// <param name="showBackLink">obsolete</param>
    public override void DisplayErrorMessage(Exception ex, bool showBackLink)
    {
        string message = "An unexpected error has occurred.\r";
        
        if (ex.GetBaseException() != null){
            message = ex.GetBaseException().Message;
        }
        else
        {
            message = ex.Message;
        }

        DisplayErrorMessage(message, showBackLink);
    }
    
    /// <summary>
    /// Method to display errorn messages.
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <remarks>If you want to hide the control, pass it a null or empty parameter</remarks>
    public override void DisplayErrorMessage(string message, bool showBackLink)
    {
       
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(message))
        {
            string friendlyMessage = RegUtilities.ConvertToFriendly(message);
            //Find ctrl
            Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(PerkinElmer.COE.Registration.Server.Constants.ErrorYUIPanelID);
            if (ErrorCtrl != null)
            {
                if (ErrorCtrl is ErrorControl)
                {
                    ((ErrorControl)ErrorCtrl).Hidden = false;
                    ((ErrorControl)ErrorCtrl).BodyText = friendlyMessage;
                    ((ErrorControl)ErrorCtrl).ErrorDescription = message;
                    ((ErrorControl)ErrorCtrl).FooterText = "Send report";
                    ((ErrorControl)ErrorCtrl).EmailAccount = RegUtilities.GetConfigSetting("MISC", "EmailAccount");
                    ((ErrorControl)ErrorCtrl).EmailSubject = RegUtilities.GetConfigSetting("MISC", "EmailSubject");
                    ((ErrorControl)ErrorCtrl).HeaderText = RegUtilities.GetConfigSetting("MISC", "HeaderText");
                    ((ErrorControl)ErrorCtrl).HeaderTextClass = RegUtilities.GetConfigSetting("MISC", "HeaderTextClass");
                    ((ErrorControl)ErrorCtrl).BodyTextClass = RegUtilities.GetConfigSetting("MISC", "BodyTextClass");
                    ((ErrorControl)ErrorCtrl).FooterTextClass = RegUtilities.GetConfigSetting("MISC", "FooterTextClass");
                    ((ErrorControl)ErrorCtrl).GoBackLink = false;
                    ((ErrorControl)ErrorCtrl).ShowAllMessage = true;
                }
            }
        }
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }  

    /// <summary>
    /// Method to display errorn messages which cna dispaly more than 400 characters
    /// </summary>
    /// <param name="message"></param>
    /// <param name="showBackLink"></param>
    /// <param name="blnShowMessage"></param>
    public void DisplayFullErrorMessage(string message, bool showBackLink, bool blnShowMessage)
    {

        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(message))
        {
            string friendlyMessage = RegUtilities.ConvertToFriendly(message);
            //Find ctrl
            Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(PerkinElmer.COE.Registration.Server.Constants.ErrorYUIPanelID);
            if (ErrorCtrl != null)
            {
                if (ErrorCtrl is ErrorControl)
                {
                    ((ErrorControl)ErrorCtrl).Hidden = false;
                    ((ErrorControl)ErrorCtrl).BodyText = friendlyMessage;
                    ((ErrorControl)ErrorCtrl).ErrorDescription = message;
                    ((ErrorControl)ErrorCtrl).FooterText = "Send report";
                    ((ErrorControl)ErrorCtrl).EmailAccount = RegUtilities.GetConfigSetting("MISC", "EmailAccount");
                    ((ErrorControl)ErrorCtrl).EmailSubject = RegUtilities.GetConfigSetting("MISC", "EmailSubject");
                    ((ErrorControl)ErrorCtrl).HeaderText = RegUtilities.GetConfigSetting("MISC", "HeaderText");
                    ((ErrorControl)ErrorCtrl).HeaderTextClass = RegUtilities.GetConfigSetting("MISC", "HeaderTextClass");
                    ((ErrorControl)ErrorCtrl).BodyTextClass = RegUtilities.GetConfigSetting("MISC", "BodyTextClass");
                    ((ErrorControl)ErrorCtrl).FooterTextClass = RegUtilities.GetConfigSetting("MISC", "FooterTextClass");
                    ((ErrorControl)ErrorCtrl).GoBackLink = false;
                    ((ErrorControl)ErrorCtrl).ShowAllMessage = blnShowMessage;
                }
            }
        }
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

  
    //0806modified
    public void DisplayErrorMessageByException(Exception ex, bool showBackLink)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(ex.Message))
        {
            string friendlyMessage = RegUtilities.ConvertToFriendly(ex.Message);
            //Find ctrl
            Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(PerkinElmer.COE.Registration.Server.Constants.ErrorYUIPanelID);
            if (ErrorCtrl != null)
            {
                if (ErrorCtrl is ErrorControl)
                {
                    ((ErrorControl)ErrorCtrl).Hidden = false;
                    ((ErrorControl)ErrorCtrl).ClipBoardMessage = RegUtilities.GetClipboardMessageByException(ex);
                    ((ErrorControl)ErrorCtrl).BodyText = friendlyMessage;
                    ((ErrorControl)ErrorCtrl).ErrorDescription = ex.Message;
                    ((ErrorControl)ErrorCtrl).FooterText = "Send report";
                    ((ErrorControl)ErrorCtrl).EmailAccount = RegUtilities.GetConfigSetting("MISC", "EmailAccount");
                    ((ErrorControl)ErrorCtrl).EmailSubject = RegUtilities.GetConfigSetting("MISC", "EmailSubject");
                    ((ErrorControl)ErrorCtrl).HeaderText = RegUtilities.GetConfigSetting("MISC", "HeaderText");
                    ((ErrorControl)ErrorCtrl).HeaderTextClass = RegUtilities.GetConfigSetting("MISC", "HeaderTextClass");
                    ((ErrorControl)ErrorCtrl).BodyTextClass = RegUtilities.GetConfigSetting("MISC", "BodyTextClass");
                    ((ErrorControl)ErrorCtrl).FooterTextClass = RegUtilities.GetConfigSetting("MISC", "FooterTextClass");
                    ((ErrorControl)ErrorCtrl).GoBackLink = false;
                }
            }
        }
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    //0806modified

    /// <summary>
    /// Error Control visibility
    /// </summary>
    /// <param name="status"></param>
    private void ErrorControlHidden(bool status)
    {
        Control ErrorCtrl = this.FindControl(GUIShellTypes.MainFormID).FindControl(PerkinElmer.COE.Registration.Server.Constants.ErrorYUIPanelID);
        if (ErrorCtrl is ErrorControl)
            ((ErrorControl)ErrorCtrl).Hidden = status;
    }

    /// <summary>
    /// Method to be redirected to the messages page and display a message
    /// </summary>
    /// <param name="textToDisplay">Text to display</param>
    /// <param name="displayBackButton">Display or not the back button</param>
    public void DisplayMessagesPage(MessagesCode messageToDisplay, GUIShellTypes.MessagesButtonType buttonType)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = Resource.Messages_Page_URL + "?";
        url += GUIShellTypes.RequestVars.MessageCode.ToString() + "=" + messageToDisplay.ToString();
        url += "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() + "=" + buttonType.ToString();
        Server.Transfer(url, false);
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to be redirected to the messages page and display a message
    /// </summary>
    /// <param name="textToDisplay">Text to display</param>
    /// <param name="displayBackButton">Display or not the back button</param>
    public void DisplayMessagesPage(GUIShellTypes.MessagesCode messageToDisplay, GUIShellTypes.MessagesButtonType buttonType)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string url = Resource.Messages_Page_URL + "?";
        url += GUIShellTypes.RequestVars.MessageCode.ToString() + "=" + messageToDisplay.ToString();
        url += "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() + "=" + buttonType.ToString();
        Server.Transfer(url, false);
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to customize the changes required for User Preference in master page(for the user preference page)
    /// </summary>
    public void CustomizeUserPreference()
    {
        this.LeftPanelContainer.Visible = false;
        this.MainContentContainer.Width = new Unit("100%");
        this.mainContentTable.Style.Add("width", "730px");
        this.HeaderUserControl.CustomizeUserPreference();
    }


    /// <summary>
    /// Load the control/s in the right accordion's panel
    /// </summary>
    /// <remarks>You should show as expanded the group that you are interested</remarks> 
    public virtual void LoadControlInAccordionPanel(COENavigationPane COENavigationPaneObject)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (COENavigationPaneObject != null)
        {
            for (int i = 0; i < COENavigationPaneObject.ControlItem.Count; i++)
            {
                this.AddControlToAccordionPanel(COENavigationPaneObject.ControlItem[i], true);
            }
        }
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// This Method will set all the attributes of the Header user control.
    /// </summary>
    /// <remarks>To binf the information with the UC, the UC must implement the ICOEHeaderUC interface</remarks>
    private void SetHeaderAttributes()
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.HeaderUserControl is ICOEHeaderUC)
            ((ICOEHeaderUC)this.HeaderUserControl).DataBind(COELogoObject, COEMenuObject, COEToolbarObject);
        else
            throw new Exception(Resource.UnsupportedHeader_Label_Text);
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Returns the list of sequences given a personId (mostly the current logged user)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SequenceListByPersonIDCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Get the list of the sequences to be aplied to a Registry.
        e.BusinessObject = CambridgeSoft.COE.Registration.Services.Types.SequenceList.GetSequenceListByPersonID(1, CambridgeSoft.COE.Framework.Common.COEUser.ID);
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void SequenceListAllCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        e.BusinessObject = CambridgeSoft.COE.Registration.Services.Types.SequenceList.GetSequenceList(1);
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Returns all the projects (including inactive, assigned to other users, etc)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ProjectsAllCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        e.BusinessObject = CambridgeSoft.COE.Registration.Services.Types.ProjectList.GetProjectList();
        RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.GUIShell;
using Csla.Web;
using Resources;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;


public partial class ChemBioVizSearch : GUIShellPage
{
    #region GUIShell Variables
    ChemBioVizMasterPage _masterPage = null;
    #endregion

    #region Variables
    private COEFormGroup _formGroup;
    private COELog _coeLog = COELog.GetSingleton("ChemBioVizSearchWeb");
    private const string _cdpJavascriptDetection = "cdpJavascriptDetection";
    private bool _exportedHits = false;
    private bool _isPageRenderedForDetectingCDP = false;
    private const string _chemdrawjs = "/COECommonResources/ChemDraw/chemdraw.js";
    private const string _chemofficeenterprisejs = "/COECommonResources/ChemDraw/ChemOfficeEnterprise.js";
    #endregion

    #region Properties
    private bool RestoringHitList
    {
        get { return (ViewState["RestoringHitList"] != null); }
        set
        {
            if (value)
                ViewState["RestoringHitList"] = value;
            else
                ViewState["RestoringHitList"] = null;
        }
    }

    private bool Browsing
    {
        get { return (ViewState["Browsing"] == null ? false : (bool)ViewState["Browsing"]); }
        set
        {
            ViewState["Browsing"] = value;
            this.BusinessObject.Browse = value;
        }
    }

    private string ConfigurationSectionName
    {
        get
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["COESearchRuntimeConfiguration"]))
                return ConfigurationManager.AppSettings["COESearchRuntimeConfiguration"];

            if (!string.IsNullOrEmpty(Request.QueryString["COESearchRuntimeConfiguration"]))
                return Request.QueryString["COESearchRuntimeConfiguration"];

            return null;
        }
    }

    private ChemDrawOptions.ChemDrawPolicy CDPDetectionPolicy
    {
        get
        {
            ChemDrawOptions options = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetChemDrawOptions(this.ApplicationName, ParentApplicationName, this.FormGroupId.ToString());
            return options == null ? ChemDrawOptions.ChemDrawPolicy.Detect : options.ChemDrawPluginPolicy;
        }
    }

    private bool ShowSearchPreferences
    {
        get
        {
            SearchPreferences searchPreferences = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetSearchPreferences(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return searchPreferences == null || string.IsNullOrEmpty(searchPreferences.Visible) || !(searchPreferences.Visible.ToUpper() == "NO");
        }
    }

    private bool EnableSearchPreferences
    {
        get
        {
            SearchPreferences searchPreferences = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetSearchPreferences(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return searchPreferences == null || string.IsNullOrEmpty(searchPreferences.Enabled) || !(searchPreferences.Enabled.ToUpper() == "NO");
        }
    }

    private bool ShowQueryManagement
    {
        get
        {
            QueryManagement queryManagement = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetQueryManagement(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return queryManagement == null || string.IsNullOrEmpty(queryManagement.Visible) || !(queryManagement.Visible.ToUpper() == "NO");
        }
    }

    private bool EnableQueryManagement
    {
        get
        {
            QueryManagement queryManagement = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetQueryManagement(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return queryManagement == null || string.IsNullOrEmpty(queryManagement.Enabled) || !(queryManagement.Enabled.ToUpper() == "NO");
        }
    }

    private LeftPanelOptions.PanelState DefaultPanelState
    {
        get
        {
            LeftPanelOptions.PanelState defaultPanelState = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDefaultPanelState(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return defaultPanelState;
        }
    }

    private bool ShowExportMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.ExportMenu == null || string.IsNullOrEmpty(menuOptions.ExportMenu.Visible) || !(menuOptions.ExportMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowQueryMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.QueryMenu == null || string.IsNullOrEmpty(menuOptions.QueryMenu.Visible) || !(menuOptions.QueryMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowRestoreMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.RestoreMenu == null || string.IsNullOrEmpty(menuOptions.RestoreMenu.Visible) || !(menuOptions.RestoreMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowRefineMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.RefineMenu == null || string.IsNullOrEmpty(menuOptions.RefineMenu.Visible) || !(menuOptions.RefineMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowMarkedMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.MarkedMenu == null || string.IsNullOrEmpty(menuOptions.MarkedMenu.Visible) || !(menuOptions.MarkedMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowPrintMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.PrintMenu == null || string.IsNullOrEmpty(menuOptions.PrintMenu.Visible) || !(menuOptions.PrintMenu.Visible.ToUpper() == "NO");
        }
    }

    //COEAdvanceExport- CBVExcel
    private bool ShowSendToMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.SendToMenu == null || string.IsNullOrEmpty(menuOptions.SendToMenu.Visible) || !(menuOptions.SendToMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool ShowResultsPerPageMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.ResultsPerPageMenu == null || string.IsNullOrEmpty(menuOptions.ResultsPerPageMenu.Visible) || !(menuOptions.ResultsPerPageMenu.Visible.ToUpper() == "NO");
        }
    }

    private bool EnableExportMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.ExportMenu == null || string.IsNullOrEmpty(menuOptions.ExportMenu.Enabled) || !(menuOptions.ExportMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnableQueryMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.QueryMenu == null || string.IsNullOrEmpty(menuOptions.QueryMenu.Enabled) || !(menuOptions.QueryMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnableRestoreMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.RestoreMenu == null || string.IsNullOrEmpty(menuOptions.RestoreMenu.Enabled) || !(menuOptions.RestoreMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnableRefineMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.RefineMenu == null || string.IsNullOrEmpty(menuOptions.RefineMenu.Enabled) || !(menuOptions.RefineMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnableMarkedMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.MarkedMenu == null || string.IsNullOrEmpty(menuOptions.MarkedMenu.Enabled) || !(menuOptions.MarkedMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnablePrintMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.PrintMenu == null || string.IsNullOrEmpty(menuOptions.PrintMenu.Enabled) || !(menuOptions.PrintMenu.Enabled.ToUpper() == "NO");
        }
    }

    //COEAdvanceExport- CBVExcel
    private bool EnableSendToMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.SendToMenu == null || string.IsNullOrEmpty(menuOptions.SendToMenu.Enabled) || !(menuOptions.SendToMenu.Enabled.ToUpper() == "NO");
        }
    }

    private bool EnableResultsPerPageMenu
    {
        get
        {
            MenuOptions menuOptions = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetMenuOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());

            return menuOptions == null || menuOptions.ResultsPerPageMenu == null || string.IsNullOrEmpty(menuOptions.ResultsPerPageMenu.Enabled) || !(menuOptions.ResultsPerPageMenu.Enabled.ToUpper() == "NO");
        }
    }

    private ActionLinkCollection ActionLinks
    {
        get
        {
            return CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetActionLinks(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());
        }
    }

    internal int ExpireHitlistHistoryDays
    {
        get
        {
            int retVal = int.MaxValue;

            if (!string.IsNullOrEmpty(Request["ExpireHitlistHistoryDays"]))
            {
                retVal = int.Parse(Request["ExpireHitlistHistoryDays"]);
                Session["HitlistHistoryConfigDays"] = retVal;
            }
            return retVal;
        }
    }

    internal int ExpireQueryHistoryDays
    {
        get
        {
            int retVal = int.MaxValue;

            if (!string.IsNullOrEmpty(Request["ExpireQueryHistoryDays"]))
            {
                retVal = int.Parse(Request["ExpireQueryHistoryDays"]);
                Session["QueryHistoryConfigDays"] = retVal;
            }

            return retVal;
        }
    }
    private int MarkedHitsMax
    {
        get
        {
            int retVal = int.MaxValue;

            NamedElementCollection<CambridgeSoft.COE.Framework.Common.NameValueConfigurationElement> elcol = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetCBVGeneralOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString());
            if (elcol != null)
            {
                CambridgeSoft.COE.Framework.Common.NameValueConfigurationElement el = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetCBVGeneralOptions(this.ApplicationName, ParentApplicationName, FormGroupId.ToString()).Get("MarkedHitsMax");
                if (el != null && !string.IsNullOrEmpty(el.Value))
                    retVal = int.Parse(el.Value);
            }

            if (!string.IsNullOrEmpty(Request["MarkedHitsMax"]))
                retVal = int.Parse(Request["MarkedHitsMax"]);

            return retVal;
        }
    }

    private string ApplicationName
    {
        get
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppName"]))
                return ConfigurationManager.AppSettings["AppName"];

            return null;
        }
    }
    private string ParentApplicationName
    {
        get
        {
            if (Session["AppName"] != null)
                return Session["AppName"].ToString();
            else if (!string.IsNullOrEmpty(Request["AppName"]))
                return Request["AppName"];

            return string.Empty;
        }
        set
        {
            Session["AppName"] = value;
        }
    }

    private int FormGroupId
    {
        get
        {
            if (ViewState["FormGroupId"] == null)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FormGroupId"]))
                    ViewState["FormGroupId"] = int.Parse(ConfigurationManager.AppSettings["FormGroupId"]);

                if (!string.IsNullOrEmpty(ConfigurationSectionName) && ConfigurationUtilities.GetInstance(ConfigurationSectionName).FormGroupID != 0)
                    ViewState["FormGroupId"] = ConfigurationUtilities.GetInstance(ConfigurationSectionName).FormGroupID;

                if (!string.IsNullOrEmpty(Request.QueryString["FormGroupId"]))
                    ViewState["FormGroupId"] = Session["FormGroupId"] = int.Parse(Request.QueryString["FormGroupId"]);

                if (Session["FormGroupId"] != null)
                    ViewState["FormGroupId"] = int.Parse(Session["FormGroupId"].ToString());

                if (ViewState["FormGroupId"] == null)
                {
                    Server.Transfer(Page.ResolveUrl("~/Forms/Public/ContentArea/SelectForm.aspx"));
                }
            }

            return (int)ViewState["FormGroupId"];
        }
    }

    private bool Embed
    {
        get
        {
            if (ViewState["Embed"] == null)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Embed"]))
                    ViewState["Embed"] = bool.Parse(ConfigurationManager.AppSettings["Embed"]);

                if (!string.IsNullOrEmpty(Request.QueryString["Embed"]))
                    ViewState["Embed"] = bool.Parse(Request.QueryString["Embed"]);

            }

            return (bool)ViewState["Embed"];
        }
    }

    private bool RememberPreviousState
    {
        get
        {
            if (ViewState["RememberPreviousState"] == null)
            {
                ViewState["RememberPreviousState"] = false;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["RememberPreviousState"]))
                    ViewState["RememberPreviousState"] = bool.Parse(ConfigurationManager.AppSettings["RememberPreviousState"]);

                if (!string.IsNullOrEmpty(Request.QueryString["RememberPreviousState"]))
                    ViewState["RememberPreviousState"] = bool.Parse(Request.QueryString["RememberPreviousState"]);

            }

            return (bool)ViewState["RememberPreviousState"];
        }
        set
        {
            ViewState["RememberPreviousState"] = value;
        }
    }

    private bool AllowFullScan
    {
        get
        {
            if (ViewState["AllowFullScan"] == null)
            {
                ViewState["AllowFullScan"] = false;
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["AllowFullScan"]))
                    ViewState["AllowFullScan"] = bool.Parse(HttpContext.Current.Request.QueryString["AllowFullScan"]);
            }

            return (bool)ViewState["AllowFullScan"];
        }
    }

    private bool KeepRecordCountSynchronized
    {
        get
        {
            if (Session["KeepRecordCountSynchronized"] == null || Request.QueryString["KeepRecordCountSynchronized"] != null)
            {
                Session["KeepRecordCountSynchronized"] = Request.QueryString["KeepRecordCountSynchronized"] != null && bool.Parse(Request.QueryString["KeepRecordCountSynchronized"]);
            }

            return (bool)Session["KeepRecordCountSynchronized"];
        }
    }

    internal bool FilterChildData
    {
        get
        {
            if (Session["FilterChildData"] == null || Request.QueryString["FilterChildData"] != null)
            {
                Session["FilterChildData"] = Request.QueryString["FilterChildData"] != null && bool.Parse(Request.QueryString["FilterChildData"]);
            }

            return (bool)Session["FilterChildData"];
        }
        set
        {
            Session["FilterChildData"] = value;
        }
    }

    internal bool HighlightSubStructures
    {
        get
        {
            if (Session["HighlightSubStructures"] == null || Request.QueryString["HighlightSubStructures"] != null)
            {
                Session["HighlightSubStructures"] = Request.QueryString["HighlightSubStructures"] != null && bool.Parse(Request.QueryString["HighlightSubStructures"]);
            }

            return (bool)Session["HighlightSubStructures"];
        }
        set
        {
            Session["HighlightSubStructures"] = value;
        }
    }

    private bool ReturnPartialHitlist
    {
        get
        {
            if (Session["ReturnPartialHitlist"] == null)
            {
                Session["ReturnPartialHitlist"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).ReturnPartialHitlist.ToLower() == "yes";
            }

            return (bool)Session["ReturnPartialHitlist"];
        }
    }

    private bool SaveQueryHistory
    {
        get
        {
            if (Session["SaveQueryHistory"] == null)
            {
                Session["SaveQueryHistory"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).SaveQueryHistory.ToLower() == "yes";
            }

            return (bool)Session["SaveQueryHistory"];
        }
    }

    private int MaxRecordCount
    {
        get
        {

            if (Session["MaxRecordCount"] == null)
            {
                Session["MaxRecordCount"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).MaxRecordCount;
            }

            return (int)Session["MaxRecordCount"];
        }

    }

    private int CommitSize
    {
        get
        {
            if (Session["CommitSize"] == null)
            {
                Session["CommitSize"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).PartialHitlistCommitSize;
            }

            return (int)Session["CommitSize"];
        }
    }

    private int SafeExportSize
    {
        get
        {

            if (Session["SafeExportSize"] == null)
            {
                Session["SafeExportSize"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).SafeExportSize;
            }

            return (int)Session["SafeExportSize"];
        }
    }

    /// <summary>
    /// <see cref="FormGroup.CurrentFormEnum"/>  Mode to go after a search.
    /// </summary>
    public FormGroup.CurrentFormEnum SearchAction
    {
        get
        {
            if (ViewState["SearchAction"] == null)
            {
                ViewState["SearchAction"] = FormGroup.CurrentFormEnum.DetailForm;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultSearchAction"]))
                    ViewState["SearchAction"] = Enum.Parse(typeof(FormGroup.CurrentFormEnum), ConfigurationManager.AppSettings["DefaultSearchAction"]);

                if (!string.IsNullOrEmpty(ConfigurationSectionName))
                    ViewState["SearchAction"] = ConfigurationUtilities.GetInstance(ConfigurationSectionName).DefaultSearchAction;

                if (!string.IsNullOrEmpty(Request.QueryString["DefaultSearchAction"]))
                {
                    FormGroup.CurrentFormEnum requestedDefaultSearchAction = (FormGroup.CurrentFormEnum)Enum.Parse(typeof(FormGroup.CurrentFormEnum), Request.QueryString["DefaultSearchAction"]);
                    if (requestedDefaultSearchAction != FormGroup.CurrentFormEnum.QueryForm)
                        ViewState["SearchAction"] = requestedDefaultSearchAction;
                }
            }
            if (Session["SearchAction"] != null)
                ViewState["SearchAction"] = Session["SearchAction"];

            return (FormGroup.CurrentFormEnum)ViewState["SearchAction"];
        }
        set
        {
            Session["SearchAction"] = value;
        }
    }

    /// <summary>
    /// Default form index to display in list view.
    /// </summary>
    public int DefaultListViewFormIndex
    {
        get
        {
            if (Session["DefaultListViewFormIndex"] == null)
            {
                Session["DefaultListViewFormIndex"] = 0;
            }

            return (int)Session["DefaultListViewFormIndex"];
        }
        set
        {
            Session["DefaultListViewFormIndex"] = value;
        }
    }

    /// <summary>
    /// Default form index to display in details view.
    /// </summary>
    public int DefaultDetailsViewFormIndex
    {
        get
        {
            if (Session["DefaultDetailsViewFormIndex"] == null)
            {
                Session["DefaultDetailsViewFormIndex"] = 0;
            }

            return (int)Session["DefaultDetailsViewFormIndex"];
        }
        set
        {
            Session["DefaultDetailsViewFormIndex"] = value;
        }
    }

    /// <summary>
    /// Selected form index to display in list view.
    /// </summary>
    public int SelectedListViewFormIndex
    {
        get
        {
            if (Session["SelectedListViewFormIndex"] == null)
            {
                Session["SelectedListViewFormIndex"] = this.DefaultListViewFormIndex;
            }

            return (int)Session["SelectedListViewFormIndex"];
        }
        set
        {
            Session["SelectedListViewFormIndex"] = value;
        }
    }

    /// <summary>
    /// Selected form index to display in details view.
    /// </summary>
    public int SelectedDetailsViewFormIndex
    {
        get
        {
            if (Session["SelectedDetailsViewFormIndex"] == null)
            {
                Session["SelectedDetailsViewFormIndex"] = this.DefaultDetailsViewFormIndex;
            }

            return (int)Session["SelectedDetailsViewFormIndex"];
        }
        set
        {
            Session["SelectedDetailsViewFormIndex"] = value;
        }
    }

    private int DefaultQueryForm
    {
        get
        {
            if (ViewState["DefaultQueryForm"] == null)
            {
                ViewState["DefaultQueryForm"] = 0;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultQueryForm"]))
                    ViewState["DefaultQueryForm"] = int.Parse(ConfigurationManager.AppSettings["DefaultQueryForm"]);

                if (!string.IsNullOrEmpty(ConfigurationSectionName))
                    ViewState["DefaultQueryForm"] = ConfigurationUtilities.GetInstance(ConfigurationSectionName).DefaultQueryForm;

                if (!string.IsNullOrEmpty(Request.QueryString["DefaultQueryForm"]))
                    ViewState["DefaultQueryForm"] = int.Parse(Request.QueryString["DefaultQueryForm"]);
            }

            return (int)ViewState["DefaultQueryForm"];
        }
    }

    private int BufferSize
    {
        get
        {
            if (ViewState["BufferSize"] == null)
            {
                ViewState["BufferSize"] = 10;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BufferSize"]))
                    ViewState["BufferSize"] = int.Parse(ConfigurationManager.AppSettings["BufferSize"]);

                if (!string.IsNullOrEmpty(ConfigurationSectionName) && ConfigurationUtilities.GetInstance(ConfigurationSectionName).BufferSize != 0)
                    ViewState["BufferSize"] = ConfigurationUtilities.GetInstance(ConfigurationSectionName).BufferSize;
            }

            return (int)ViewState["BufferSize"];
        }
    }

    /// <summary>
    /// Quantity of records per page to display.
    /// </summary>
    public int GridPageSize
    {
        get
        {
            if (Session["GridPageSize"] == null)
            {
                Session["GridPageSize"] = this.DefaultGridPageSize;
            }

            return (int)Session["GridPageSize"];
        }
        set
        {
            Session["GridPageSize"] = value;
        }
    }

    /// <summary>
    /// Default quantity of records per page to display.
    /// </summary>
    public int DefaultGridPageSize
    {
        get
        {
            if (Session["DefaultGridPageSize"] == null)
            {
                Session["DefaultGridPageSize"] = this.GetHitsForUser();

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GridPageSize"]))
                    Session["DefaultGridPageSize"] = int.Parse(ConfigurationManager.AppSettings["GridPageSize"]);

                if (!string.IsNullOrEmpty(ConfigurationSectionName) && ConfigurationUtilities.GetInstance(ConfigurationSectionName).GridPageSize != 0)
                    Session["DefaultGridPageSize"] = ConfigurationUtilities.GetInstance(ConfigurationSectionName).GridPageSize;
            }

            return (int)Session["DefaultGridPageSize"];
        }
        set
        {
            Session["DefaultGridPageSize"] = value;
        }
    }

    /// <summary>
    /// The business object. <see cref="GenericBO"/>
    /// </summary>
    public GenericBO BusinessObject
    {
        get
        {
            if (_formGroup != null)
            {
                if (Session["BasePageBusinessObject"] == null || ((GenericBO)Session["BasePageBusinessObject"]).FormGroupID != _formGroup.FormGroupDescription.Id)
                {
                    GenericBO bo = GenericBO.GetGenericBO(this.ApplicationName, _formGroup.FormGroupDescription);
                    bo.CurrentFormType = _formGroup.CurrentDisplayGroup;
                    bo.CurrentFormIndex = _formGroup.CurrentFormIndex;
                    bo.ReturnPartialHitlist = this.ReturnPartialHitlist;
                    bo.SaveQueryHistory = this.SaveQueryHistory;
                    bo.CommitSize = this.CommitSize;
                    bo.MaxRecordCount = this.MaxRecordCount;
                    bo.PageSize = this.BufferSize;
                    bo.AllowFullScan = this.AllowFullScan;
                    bo.KeepRecordCountSyncrhonized = this.KeepRecordCountSynchronized;
                    bo.MarkedHitsMax = this.MarkedHitsMax;
                    bo.ExpireHitlistHistoryDays = this.ExpireHitlistHistoryDays;
                    bo.ExpireQueryHistoryDays = this.ExpireQueryHistoryDays;
                    bo.PagingInfo.FilterChildData = this.FilterChildData;
                    bo.PagingInfo.HighlightSubStructures = this.HighlightSubStructures;
                    Session["BasePageBusinessObject"] = bo;
                }
            }

            return (GenericBO)Session["BasePageBusinessObject"];
        }
        set
        {
            Session["BasePageBusinessObject"] = value;
        }
    }

    /// <summary>
    /// <see cref="FormGroup.CurrentFormEnum"/> current mode.
    /// </summary>
    public FormGroup.CurrentFormEnum CurrentMode
    {
        get
        {
            return this._formGroup.CurrentDisplayGroup;
        }
        set
        {
            this.SetPageMode(value);
        }
    }

    private int PageOffset
    {
        get
        {
            if (ViewState["PageOffset"] == null)
                ViewState["PageOffset"] = 0;

            return (int)ViewState["PageOffset"];
        }
        set
        {
            ViewState["PageOffset"] = value;
        }
    }

    private string URLReferrer
    {
        get
        {
            if (ViewState["URLReferrer"] == null)
                ViewState["URLReferrer"] = Request.QueryString["DefaultQueryForm"];

            return (string)ViewState["URLReferrer"];
        }
        set
        {
            ViewState["URLReferrer"] = value;
        }
    }

    private int PreviousFormGroupId
    {
        get
        {
            if (Session["PreviousFormGroupId"] == null)
                return -1;

            else
                return (int)Session["PreviousFormGroupId"];
        }
        set
        {
            Session["PreviousFormGroupId"] = value;
        }
    }

    private FormGroup.CurrentFormEnum PreviousDisplayGroup
    {
        get
        {
            if (Session["CurrentDisplayGroup"] == null)
                Session["CurrentDisplayGroup"] = FormGroup.CurrentFormEnum.QueryForm;

            return (FormGroup.CurrentFormEnum)Session["CurrentDisplayGroup"];
        }
        set
        {
            Session["CurrentDisplayGroup"] = value;
        }
    }

    private int PreviousPageIndex
    {
        get
        {
            if (Session["PreviousPageIndex"] == null)
                Session["PreviousPageIndex"] = 0;

            return (int)Session["PreviousPageIndex"];
        }
        set
        {
            Session["PreviousPageIndex"] = value;
        }
    }

    private string SortFields
    {
        get
        {
            return this.Page.Session["SortFields"] as string;
        }
        set
        {
            this.Page.Session["SortFields"] = value;
        }
    }

    private string SortDirections
    {
        get
        {
            return this.Page.Session["SortDirections"] as string;
        }
        set
        {
            this.Page.Session["SortDirections"] = value;
        }
    }

    private HitListInfo HitlistToRefine
    {
        get
        {
            return ViewState["HitlistToRefine"] as HitListInfo;
        }
        set
        {
            ViewState["HitlistToRefine"] = value;
        }
    }
    #endregion

    #region Lyfe Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        this.LoadControlInAccordionPanel();
        this.ClearErrorMessage();
        this.NoRecordsFoundLabel.Visible = this.NoRecordsFoundDiv.Visible = false;
        if (!Page.IsPostBack)
        {
            // Clear cache if any changes made at registration admin panel.
            if (Request.QueryString[Constants.ClearCache] != null)
                Utilities.CleanCache();

            Utilities.CleanLocalCache();
            RefreshControlSettings(false);

            SetControlsAttributtes();
            if (Request.QueryString["AppName"] != null)
            {
                ParentApplicationName = Request.QueryString["AppName"];
            }
            Session.Remove("ExportTemplateId");
            if (this.ExportControl.PageDataview.DataViewID != this.BusinessObject.DataView.DataViewID)
            {
                Utilities.CleanCache();
                this.ExportControl.PageDataview = this.BusinessObject.DataView;
            }
            if (Request.QueryString["CurrentIndex"] == null)
            {
                this.BusinessObject.RefreshDatabaseRecordCount();
                this.RestorePreviousState();
            }
            _masterPage.LeftPanelState = this.DefaultPanelState;
        }
        //JHS 2/25/2012 - As far as I can tell this is not even being used properly by the control
        //I am cancelling this out for 12.3.2.  The correct behavior would be to use this criteria if there is no template.
        // this.ExportControl.PageResultsCriteria = this.BusinessObject.ResultsCriteria;

        if (this.CurrentMode != FormGroup.CurrentFormEnum.QueryForm)
        {
            UpdateTimer();
        }

        this.SetTitleText();

        if (!Page.ClientScript.IsStartupScriptRegistered(typeof(ChemBioVizSearch), "PrintFormGenerator"))
        {
            string absoluteUriStart = this.Request.Url.AbsoluteUri.Replace(this.Request.Url.PathAndQuery, "");
            string htmlStr = "<html><head>";
            htmlStr += "<link rel='STYLESHEET' type='text/css' href='" + absoluteUriStart + Page.ResolveUrl("~/App_Themes/" + this.Page.StyleSheetTheme + "/StyleSheet.css") + "'>";
            htmlStr += "<link rel='STYLESHEET' type='text/css' href='" + absoluteUriStart + Page.ResolveUrl("~/App_Themes/" + this.Page.StyleSheetTheme + "/COEForms" + this.Page.StyleSheetTheme + ".css") + "'>";
            htmlStr += "<script type='text/javascript' src='" + _chemdrawjs + "'>";
            htmlStr += "<\\/script>";
            htmlStr += "\" + document.getElementById('ctl00_Head1').innerHTML + \"</head>";
            htmlStr += "<body><script type='text/javascript'>cd_includeWrapperFile();<\\/script><div style='width:900px;'>\" + prtContent.innerHTML + \"</div>";
            htmlStr += "</body></html>";
            string printScript = "function PrintFormGenerator()" + "\n" +
            "{" + "\n" +
            "   if (document.getElementById('" + this._formGroup.ClientID + "_formGroupValidationSummaryPanel') !=null)" + "\n" +
            "    document.getElementById('" + this._formGroup.ClientID + "_formGroupValidationSummaryPanel').style.visibility = 'hidden';" + "\n" +
            "    var prtContent = document.getElementById('" + this._formGroup.ClientID + "');" + "\n" +
            "    var htmlContent = \"" + htmlStr + "\";" + "\n" +
            "    var WinPrint = window.open('','printWindow','width=5,height=5,toolbar=no,scrollbars=yes,status=no');" + "\n" +
            "    WinPrint.document.write(htmlContent);" + "\n" +
            "    WinPrint.document.close();" + "\n" +
            "    WinPrint.focus();" + "\n" +
            "    WinPrint.print();" + "\n" +
            "    WinPrint.close();" + "\n" +
            "}";

            Page.ClientScript.RegisterStartupScript(typeof(ChemBioVizSearch), "PrintFormGenerator", printScript, true);
        }

        if (!Page.IsPostBack)
            this.BusinessObject.KeepRecordCountSyncrhonized = this.KeepRecordCountSynchronized;

        this.CoePagerControl.RecNumber.Attributes.Add("onclick", "SetRecPanelVisibility('" + this.RecNoPanel.PanelClientID + "', true); return false;");
        this.CoePagerControlBottom.RecNumber.Attributes.Add("onclick", "SetRecPanelVisibility('" + this.RecNoPanelBottom.PanelClientID + "', true); return false;");

        if (expcontrolvisibility.Value == "true")
        {
            ExportControlDiv.Style.Add("display", "block");
        }
        else if (expcontrolvisibility.Value == "false")
        {
            ExportControlDiv.Style.Add("display", "none");
        }
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        int serchableRecordCount = (this.SearchImageButton.Visible && this.BusinessObject.HitListToRefine != null) ? this.BusinessObject.HitListToRefine.RecordCount : this.BusinessObject.DatabaseRecordCount;
        this.TotalSearchableRecordsLabel.Text = string.Format(Resource.TotalSearcheableRecords_Label_Text, serchableRecordCount);

        if (this.SearchImageButton.Visible)
        {
            _masterPage.MakeCtrlShowProgressModal(this.SearchImageButton.GetControlsInsideClientIDs()[0], string.Format("Searching over {0} records...", serchableRecordCount), string.Empty);
            _masterPage.MakeCtrlShowProgressModal(this.RetrieveAllImageButton.GetControlsInsideClientIDs()[0], string.Format("Retrieving {0} records...", serchableRecordCount), string.Empty, false);
        }
        base.OnLoadComplete(e);
    }

    /// <summary>
    /// There's no way, as for now, to refresh controls settings automatically. So if user modifies session variables or configuration entries, control settings are not updated unless we force re-calculation here.
    /// </summary>
    /// <param name="forceRefresh">if set to <c>true</c> we will refresh regardless of session state</param>
    private void RefreshControlSettings(bool forceRefresh)
    {
        if (HttpContext.Current.Session != null)
        {
            //Previously this was always going back to the database which is wasteful. Now it only goes back if 1) the page control info is not available in session or 2) the requestor sets it to refresh
            if (HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] == null || forceRefresh)
            {
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList ctrls = CambridgeSoft.COE.Framework.COEPageControlSettingsService.COEPageControlSettings.GetControlListToDisableForCurrentUser(COEAppName.Get().ToString());
                if (ctrls != null)
                    HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] = ctrls;
            }
        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        #region Page GUIShell Settings
        // To make easier to read the source code.
        _masterPage = (ChemBioVizMasterPage)this.Master;
        #endregion

        this.LoadFormGenerator();

        base.OnPreInit(e);
    }

    protected override void OnError(EventArgs e)
    {
        Exception ex = this.Server.GetLastError();
        if (ex != null) //Coverity fix 
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GENERAL ERROR:");
            System.Diagnostics.Debug.Indent();
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            foreach (object key in ex.Data.Keys)
            {
                System.Diagnostics.Debug.WriteLine("Exception.Data:");
                System.Diagnostics.Debug.Indent();
                System.Diagnostics.Debug.WriteLine("Key: " + key.ToString() + ", Value: " + ex.Data[key].ToString());
                System.Diagnostics.Debug.Unindent();
            }
            System.Diagnostics.Debug.Unindent();
#endif
            if (ex.Message.Contains("COEStructureQuery expects to be bound to a CambridgeSoft.COE.Framework.Common.SearchCriteria+StructureCriteria object type"))
            {
                Server.Transfer("ChemBioVizSearch.aspx");
            }
        }

        this.ShowErrorInMessagesPage(Constants.MessagesCode.Unknown, GUIShellTypes.MessagesButtonType.Back);
    }

    protected override void OnPreRenderComplete(EventArgs e)
    {

        base.OnPreRenderComplete(e);
        bool ajaxEnabled = ScriptManager.GetCurrent(this) != null;

        if (!Page.IsPostBack)
        {
            if (Request.QueryString["CurrentIndex"] != null)
            {
                PageOffset = int.Parse(Request.QueryString["CurrentIndex"]) + 1;
                this.CurrentMode = FormGroup.CurrentFormEnum.DetailForm;
            }
        }
        this.SetGridSizeDDLSelection();
        string yuiJSFolder = Utilities.YUIJsRelativeFolder();

        //This code is to catch an error thrown by the ValidationSummary when you click the view details link. CSBR: 98818.
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ChemBioVizSearch), "ValidationSummary"))
        {
            string script = "<script type=\"text/javascript\">";
            script += "function ErrorHandling(sMessage, sUrl, sLine){if(sMessage.indexOf('is null or not an object') > -1){return true;}else{return false;}}";
            script += "onerror=ErrorHandling;";
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(ChemBioVizSearch), "ValidationSummary", script);
            if (ajaxEnabled)
            {
                ScriptManager.RegisterClientScriptBlock(this, typeof(ChemBioVizSearch), "ValidationSummary", script, false);
            }
        }


        if (_exportedHits)
        {
            string keyExport = "exportHitsJS";

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ChemBioVizSearch), keyExport))
            {
                string expScript = "<script type=\"text/javascript\">";
                expScript += "function myfunction(){window.open ('Export.aspx','mywindow','location=no,status=no,scrollbars=no,toolbar=no,resizable=no,width=350,height=150,top=400,left=500,menubar=no,dialog=yes,minimizable=no');}";
                expScript += "YAHOO.util.Event.addListener(window, \"load\", myfunction);";
                expScript += "</script>";
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(ChemBioVizSearch), keyExport, expScript);
                if (ajaxEnabled)
                {
                    ScriptManager.RegisterClientScriptBlock(this, typeof(ChemBioVizSearch), keyExport, expScript, false);
                }
            }
        }

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ChemBioVizSearch), "ResizeIFrame"))
        {
            string expScript = "<script type=\"text/javascript\">";
            expScript += "var originalHeight, originalWidth, originalBodyWidth;";
            expScript += "function ResizeIFrame() { if (window.frameElement) { element = window.frameElement.id; if (element !='') { originalHeight=parent.document.getElementById(element).height;parent.document.getElementById(element).height = YAHOO.util.Dom.getDocumentHeight(); originalWidth=parent.document.getElementById(element).style.width; parent.document.getElementById(element).style.width = YAHOO.util.Dom.getDocumentWidth(); originalBodyWidth=parent.document.body.style.width; parent.document.body.style.width = YAHOO.util.Dom.getDocumentWidth(); } } }";
            expScript += "YAHOO.util.Event.addListener(window, \"load\", ResizeIFrame);";
            expScript += "function ResetValues() {  if (window.frameElement) { parent.document.getElementById(element).height = originalHeight; parent.document.getElementById(element).style.width = originalWidth; parent.document.body.style.width = originalBodyWidth; } }";
            expScript += "YAHOO.util.Event.addListener(window, \"unload\", ResetValues);";
            expScript += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(ChemBioVizSearch), "ResizeIFrame", expScript);
            if (ajaxEnabled)
            {
                ScriptManager.RegisterClientScriptBlock(this, typeof(ChemBioVizSearch), "ResizeIFrame", expScript, false);
            }
        }

        string scriptMarkedCallback = @"<script language='javascript' type='text/javascript'>
    function OnCallbackComplete(result, context)
    {        
        var markedHitsMax = new Number(" + this.MarkedHitsMax + @");
        if(context.split(',')[0] == 'ApplyMark')
        {
            var markedLabel = document.getElementById('" + this.MarkedCountLabel.ClientID + @"').innerHTML;
            var originalCount = new Number(markedLabel.substring(markedLabel.indexOf(': ') + 1).replace(/ /, ''));
            var newCount = new Number(originalCount);            

            if(result == 'false')
                newCount = originalCount - 1;
            else
                newCount = originalCount + 1;
            
            if(markedHitsMax >= newCount)
            {
                document.getElementById('" + this.MarkedCountLabel.ClientID + @"').innerHTML = markedLabel.replace(originalCount, newCount);
                
                var actionLinksContainer = document.getElementById('" + this.ActionLinksContainer.ClientID + @"');
                if(actionLinksContainer != null)
                {
                    for(actionLinkIndex = 0; actionLinkIndex < actionLinksContainer.childNodes.length; actionLinkIndex++)
                    {
                        if(actionLinksContainer.childNodes[actionLinkIndex].nodeType == 1)
                            enableAnchor(actionLinksContainer.childNodes[actionLinkIndex], newCount > 0);
                    }
                }
            }
            else
            {
                document.getElementById(context.split(',')[1]).checked = false;
            }
        }
    }
    
    function enableAnchor(anchor, enabled)
    {        
        if(enabled)
        {            
            anchor.removeAttribute('disabled');
            anchor.style.cssText = 'cursor:pointer;';
        }
        else 
        {            
            anchor.setAttribute('disabled', 'disabled');
            anchor.style.cssText = 'color:grey;cursor:default;';
        }        
    }
    </script>";

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ChemBioVizSearch), "MarkedCallback"))
        {
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(ChemBioVizSearch), "MarkedCallback", scriptMarkedCallback);
        }
        if (ajaxEnabled)
        {
            ScriptManager.RegisterClientScriptBlock(this, typeof(ChemBioVizSearch), "MarkedCallback", scriptMarkedCallback, false);
        }
        if (this.CurrentMode == FormGroup.CurrentFormEnum.ListForm)
        {
            this.OverrideMenuItemsEnabled();
            /*CSBR-160201: Queries used are listed twice under Recent Queries
             *      DisplayRecordCount essentially triggers a search.  Because it was being used in multiple places this was actually resulting in 2 searches. 
             *      It was removed from other spots like SetExportAlerts which is now an obsolete methods.  It was also modified to do a check for browse to fix the following bug
             *CSBR-160192: New Query->Retieve All showing no hits after deleting the single result 
             *      Use of the Browse check will pull back the DatabaseRecordCount.  In some scenarios the retrieveall was being done, but because there is no hitlist for a retrieveall
             *      we would end up with an unusual state where the record count here would be 0 and no rows would be displayed.
             */
            /*CBOE-202 	PF: CBOE > Back button in search registry results table - Start 
             *      If the RequiresRestoringState and HitListID >= 0 then 
             *      set the CurrentHitList RecordCount otherwise set the DatabaseRecordCount
             *      in the Pager control
            */
            if (this.RequiresRestoringState && this.BusinessObject.HitListID>=0)
                this.DisplayRecordCount(this.BusinessObject.CurrentHitList.RecordCount);
            else
                this.DisplayRecordCount((!this.BusinessObject.Browse) ? this.BusinessObject.CurrentHitList.RecordCount : this.BusinessObject.DatabaseRecordCount);
        }

        /* CSBR- 156518: Clicking on "Clear" button of ‘Bulk Register Marked’ page is not clearing  the entered data 
          Code changes done by jogi
         * This code will register the corresponding Javascript file with the Page Object. which contains the coe_clearForm() function
         * that is called when clicked on clear button, Which will clear the data 
         */
        if (this.ClearImageButton.Visible)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("ChemOfficeEnterprise.js"))
                Page.ClientScript.RegisterClientScriptInclude("ChemOfficeEnterprise.js", "/COECommonResources/ChemDraw/ChemOfficeEnterprise.js");

        }
        /* End of CSBR-156518 */

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        switch (this.CDPDetectionPolicy)
        {
            case ChemDrawOptions.ChemDrawPolicy.Detect:
                if (Session["isCDP"] == null || this.FormGroupId != this.PreviousFormGroupId)
                {
                    if (Page.Request.Cookies.Get("isCDP") != null)
                    {
                        Session["isCDP"] = bool.Parse(Page.Request.Cookies.Get("isCDP").Value);
                    }
                    else
                    {
                        _isPageRenderedForDetectingCDP = true;
                    }
                }
                else
                {
                    if (Page.ClientScript.IsStartupScriptRegistered(_cdpJavascriptDetection))
                    {
                        string cdpDetectionScript = @"";
                        Page.ClientScript.RegisterStartupScript(typeof(ChemBioVizSearch), _cdpJavascriptDetection, cdpDetectionScript, true);
                    }
                }
                break;
            case ChemDrawOptions.ChemDrawPolicy.Available:
                Session["isCDP"] = true;
                break;
            case ChemDrawOptions.ChemDrawPolicy.Unavailable:
                Session["isCDP"] = false;
                break;
        }

        // Update chemdrawembed info in session
        if (Session["ShowPluginDownload"] == null)
        {
            Session["ShowPluginDownload"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).ShowPluginDownload.ToUpper() == "YES";
        }

        if (Session["PluginDownloadURL"] == null)
        {
            Session["PluginDownloadURL"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).PluginDownloadURL;
        }

        if (Session["DownloadChemDrawImageSrc"] == null)
        {
            Session["DownloadChemDrawImageSrc"] = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetApplicationData(this.ApplicationName).DownloadChemDrawImageSrc;
        }

        #region Image Buttons
        this.SearchImageButton.ButtonClicked += new EventHandler<EventArgs>(SearchImageButton_ButtonClicked);
        this.RetrieveAllImageButton.ButtonClicked += new EventHandler<EventArgs>(RetrieveAllImageButton_ButtonClicked);
        this.HitsMenuButton.Click += new EventHandler(HitsImageButton_ButtonClicked);
        this.RefineImageMenuButton.Command += new CommandEventHandler(Refine_Command);
        this.MarkedMenuButton.Command += new CommandEventHandler(MarkedMenuButton_Command);
        this.QueryMenuButton.Command += new CommandEventHandler(QueryMenuButton_Command);
        this.NewQueryMenuButton.Click += new EventHandler(NewQuery_ButtonClicked);
        this.SearchQueryMenuButton.Command += new CommandEventHandler(SearchQueryMenuButton_Command);
        this.RecNoPanel.Save += new RecPanel.RecPanelEventHandler(SaveButton_Click);
        this.RecNoPanelBottom.Save += new RecPanel.RecPanelEventHandler(SaveButton_Click);
        this.ExportControl.ExportList += new ExportControl.ExportControlEventHandler(ExportControl_ExportList);
        #endregion

        _formGroup.MarkingHit += new MarkingHitHandler(FormGroup_MarkingHit);
        _formGroup.MarkAllHits += new MarkAllHitsHandler(FormGroup_MarkAllHits);
        _formGroup.OrderCommand += new CommandEventHandler(FormGroup_OrderCommand);
         System.Reflection.Assembly theAssembly = System.Reflection.Assembly.GetAssembly(typeof(GenericBO)); //Coverity Fix 
        if (theAssembly != null)
        {
            ChemBioVizSearchCSLADataSource.TypeAssemblyName = theAssembly.FullName;
            ChemBioVizSearchCSLADataSource.TypeName = typeof(GenericBO).FullName;
        }

        ScriptManager sm = ScriptManager.GetCurrent(this.Page);
        if (sm != null)
        {
            sm.RegisterPostBackControl(this.RecNoPanel);
            sm.RegisterPostBackControl(this.RecNoPanelBottom);
        }
    }


    protected override void OnUnload(EventArgs e)
    {
        this.BusinessObject.AllowFullScan = this.AllowFullScan;
        this.BusinessObject.HitListToRefine = this.HitlistToRefine = null;
        base.OnUnload(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        if (!_isPageRenderedForDetectingCDP)
        {
            base.Render(writer);
        }
        else
        {
            string searchFolder = string.IsNullOrEmpty(this.Page.Request["callingApp"]) ? this.Page.ResolveUrl(Constants.GetSearchContentAreaFolder()) : Constants.GetSearchContentAreaFolder(this.Page.Request["callingApp"]);
            string cdpDetectionScript = @"
            <script language='javascript' type='text/javascript'>
                    var isPluginInstalled  = false;
                    if (cd_currentUsing == 2 || cd_currentUsing == 3) {
                        isPluginInstalled =	cd_isCDPluginInstalled();		
                    }
                    else if (cd_currentUsing == 1) {
                        isPluginInstalled =	cd_isCDActiveXInstalled();
                    }
                    
                    document.cookie = " + "\"isCDP=\" + isPluginInstalled + \"; path=/;\";" + @"
                    var url = '" + this.ResolveUrl(this.Page.AppRelativeVirtualPath) + "';" + @"
                    var queryString = '?" + this.Request.QueryString.ToString() + "';" + @"
                    if(location.search.length > 0)
                        url += location.search;
                    else
                        url += queryString;
                    window.location.href = url;
            </script>";

            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.RenderBeginTag(HtmlTextWriterTag.Head);
            string chemdrawjs = "<script language=\"JavaScript\" src=\"" + _chemdrawjs + "\"></script>";
            string chemofficejs = "<script language=\"JavaScript\" src=\"" + _chemofficeenterprisejs + "\"></script>";
            writer.Write(chemdrawjs);
            writer.Write(chemofficejs);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Form);
            writer.Write(cdpDetectionScript);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
    #endregion

    #region Event Handlers
    void FormGroup_MarkingHit(object sender, MarkHitEventArgs eventArgs)
    {
        if (eventArgs.Marked)
        {
            if (MarkHit(eventArgs.ColumnIDValue, eventArgs.ColumnIDBindingExpression))
                UpdateMarkedCount();
        }
        else
        {
            UnMarkHit(eventArgs.ColumnIDValue, eventArgs.ColumnIDBindingExpression);
            UpdateMarkedCount();
        }
    }

    void FormGroup_MarkAllHits(object sender, MarkAllHitsEventArgs eventArgs)
    {
        if (eventArgs.Marked)
            this.MarkAllHits();
        else
            this.UnMarkAllHits();
        UpdateMarkedCount();
    }

    void FormGroup_OrderCommand(object sender, CommandEventArgs e)
    {
        string[] fieldAndOrder = e.CommandArgument.ToString().Split(',');
        this.SortFields = string.Empty;
        this.SortDirections = string.Empty;
        string columnToOrderWith = fieldAndOrder[0];
        string direction = fieldAndOrder[1];
        bool isChildCriteria = false;
        OrderByCriteria newCriteria = new OrderByCriteria();
        OrderByCriteria.OrderByCriteriaItem item = new OrderByCriteria.OrderByCriteriaItem();
        item.ID = 1;
        item.OrderIndex = 1;
        item.Direction = (OrderByCriteria.OrderByDirection)Enum.Parse(typeof(OrderByCriteria.OrderByDirection), direction);
        bool criteriaFound = false;
        foreach (ResultsCriteria.ResultsCriteriaTable table in this.BusinessObject.ResultsCriteria.Tables)
        {
            foreach (ResultsCriteria.IResultsCriteriaBase criteria in table.Criterias)
            {
                if (criteria.Alias == columnToOrderWith)
                {
                    item.ResultCriteriaItem = criteria;
                    item.TableID = table.Id;
                    criteriaFound = true;
                    isChildCriteria = table.Id != this.BusinessObject.DataView.Basetable;
                    newCriteria.Items.Add(item);
                    this.SortFields += item.ResultCriteriaItem.Alias + ",";
                    this.SortDirections += item.Direction.ToString() + ",";
                    break;
                }
            }
            if (criteriaFound)
                break;
        }

        if (!isChildCriteria)
            this.BusinessObject.OrderByCriteria = newCriteria;
        else
        {
            int i = 0;
            while (i < this.BusinessObject.OrderByCriteria.Items.Count)
            {
                if (this.BusinessObject.OrderByCriteria.Items[i].TableID != this.BusinessObject.DataView.Basetable)
                {
                    this.BusinessObject.OrderByCriteria.Items.RemoveAt(i);
                }
                else
                {
                    this.SortFields += this.BusinessObject.OrderByCriteria.Items[i].ResultCriteriaItem.Alias + ",";
                    this.SortDirections += this.BusinessObject.OrderByCriteria.Items[i].Direction.ToString() + ",";
                    i++;
                }
            }
            if (newCriteria.Items.Count > 0)
                this.BusinessObject.OrderByCriteria.Items.Add(newCriteria.Items[0]);
        }
        if (this.SortFields.Length > 0)
        {
            this.SortFields = this.SortFields.Remove(this.SortFields.Length - 1);
            this.SortDirections = this.SortDirections.Remove(this.SortDirections.Length - 1);
        }

        /*     Fix for CSBR-161742,CSBR-165389,CSBR-165394
        *      Column sort will be performed when WebGrid is visible to the users 
        *      Grid visibility is based on RecordCount > 0
        *      RecordCount > 0 will be either from 'RetrieveAll' or 'Search HitCount '
        *      Trigger the method Depending upon the last action performed. 
        */

        if (this.BusinessObject.CurrentHitList.RecordCount > 0)
            this.Search();
        else
            this.RetrieveAll();
    }

    void NewQuery_ButtonClicked(object sender, EventArgs e)
    {
        this.NewQuery();
    }

    void SearchQueryMenuButton_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "RestoreLastHitlist":
                COEHitListBOList hlrBOList = COEHitListBOList.GetRecentHitLists(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 1);
                this.RestoreHitList(hlrBOList[0].HitListType, hlrBOList[0].ID);
                break;
            case "PerformLastQuery":
                COESearchCriteriaBOList list = COESearchCriteriaBOList.GetRecentSearchCriteria(COEUser.Name, this.BusinessObject.DataView.DataViewID, this.BusinessObject.DataView.Database, 1);

                if (list.Count > 0)
                {
                    SearchCriteria baseSearchCriteria = SearchFormGroupAdapter.GetSearchCriteria(_formGroup.FormGroupDescription.QueryForms[_formGroup.CurrentFormIndex]);
                    foreach (SearchCriteria.SearchCriteriaItem item in list[0].SearchCriteria.Items)
                    {
                        if (item.ID >= 0)
                            SearchFormGroupAdapter.PopulateSearchCriteria(baseSearchCriteria, item.ID, item.Criterium.Value);
                    }
                    this.BusinessObject.SearchCriteria = baseSearchCriteria;
                    this.CurrentMode = this.SearchAction;
                }
                break;
            case "RestoreLastQueryToForm":
                this.Refine(false);
                break;
        }
    }

    void QueryMenuButton_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "SaveQuery":
                COEHitListBO hl = COEHitListBO.Get(this.BusinessObject.CurrentHitList.HitListType, this.BusinessObject.CurrentHitList.HitListID);
                if (hl != null && hl.HitListID > 0) //It is not a Retrieve All.
                {
                    COESearchCriteriaBO sc = null;
                    if (hl.SearchCriteriaID > 0)
                    {
                        sc = COESearchCriteriaBO.Get(hl.SearchCriteriaType, hl.SearchCriteriaID);
                        sc.Name = ((SimpleSavePanelEventArgs)e.CommandArgument).Name;
                        sc.Description = ((SimpleSavePanelEventArgs)e.CommandArgument).Description;
                        sc.IsPublic = ((SimpleSavePanelEventArgs)e.CommandArgument).IsPublic;
                        sc = sc.Save();
                    }

                    hl.Name = ((SimpleSavePanelEventArgs)e.CommandArgument).Name;
                    hl.Description = ((SimpleSavePanelEventArgs)e.CommandArgument).Description;
                    hl.IsPublic = ((SimpleSavePanelEventArgs)e.CommandArgument).IsPublic;
                    if (sc != null)
                    {
                        hl.SearchCriteriaID = sc.ID;
                        hl.SearchCriteriaType = sc.SearchCriteriaType;
                    }
                    hl = hl.Save();


                    this.UpdateMarkedCount();
                    _formGroup.DataBind();
                }
                break;
            case "AdvancedQuery":
                _masterPage.AccordionControl.Groups.FromKey("QueryManagement").Expanded = true;
                //CSBR-157199: Clicking on Advanced Query available in Hits Page leads nowhere
                //This line is required to actually expand the panel.  QueryManagement does not do that as per Infragistics documentation
                _masterPage.AccordionControl.SelectedGroup = _masterPage.AccordionControl.Groups.FromKey("QueryManagement").Index;
                ((ManagerPane)_masterPage.AccordionControl.Groups.FromKey("QueryManagement").UserControl).ShowAdvancedGroup();
                break;
        }
        if (_masterPage.AccordionControl.Groups.FromKey("QueryManagement") != null)
        {
            ((ManagerPane)_masterPage.AccordionControl.Groups.FromKey("QueryManagement").UserControl).DataBind();
        }
    }

    void MarkedMenuButton_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "SaveMarked":
                COEHitListBO bo = this.BusinessObject.MarkedHitList;

                bo.Name = ((SimpleSavePanelEventArgs)e.CommandArgument).Name;
                bo.Description = ((SimpleSavePanelEventArgs)e.CommandArgument).Description;
                bo.HitListType = HitListType.SAVED;
                bo.Save();
                this.UpdateMarkedCount();
                _formGroup.DataBind();
                break;
            case "ShowMarked":
                this.RestoreHitList(HitListType.MARKED, this.BusinessObject.MarkedHitList.ID);
                break;
            case "ShowAll":
                this.RestoreHitList(BusinessObject.LastSearchHitList);
                break;
        }
    }

    void Refine_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "EditCurrent":
                this.Refine(false);
                break;
            case "RefineOverHitList":
                this.Refine(true);
                break;
        }
    }

    void HitsImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.GoToHits();
    }

    public void SearchImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.Validate();
        if (Page.IsValid)
            this.Search();
    }

    void RetrieveAllImageButton_ButtonClicked(object sender, EventArgs e)
    {
        this.RetrieveAll();
    }

    protected void GridSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.GridPageSize = int.Parse(GridSizeDropDownList.SelectedValue);                
        this.BusinessObject.PageSize = this.DefaultGridPageSize = this.GridPageSize;
        if (this.HitlistToRefine != null)
            this.BusinessObject.HitListToRefine = this.HitlistToRefine;

        PagingInfo info = this.GetPagingInfoClone();
        info.FilterChildData = this.FilterChildData;
        info.HighlightSubStructures = this.HighlightSubStructures;
        this.BusinessObject.PagingInfo = info;

        this.SetPageMode(this.CurrentMode);
        _formGroup.DataBind();
    }

    protected void ButtonUnMarkAllHits_Click(object sender, EventArgs e)
    {
        this.UnMarkAllHits();
        this.UpdateMarkedCount();
    }

    protected void FormsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.ChangeCurrentFormIndex(((DropDownList)sender).SelectedIndex);
    }

    protected void CoePagerControl_CurrentPageChanged(object sender, PageChangedEventArgs eventArgs)
    {
        try
        {
            this.BusinessObject.AllowFullScan = true;
            this.SetCurrentPage(eventArgs.CurrentPage);
            this.UpdateMarkedCount();
            this.PreviousPageIndex = eventArgs.CurrentPage;
        }
        catch (Exception) { }
    }

    public int GetHitsForUser()
    {
        int hits=0;
        string userName = this.User.Identity.Name;
        if (userName != string.Empty)
        {
            string name = this.BusinessObject.UserNameExists(userName, hits);
            if (name != "")
            {
                int ID = Convert.ToInt16(name);
                hits = this.BusinessObject.GetHits(ID);
            }
            else
            {
                hits = 10;
            }
        }
        return hits;

    }

    void Preferences_CommandRaised(object sender, COENavigationPanelControlEventArgs e)
    {
        if (e.EventType == "SetPreferences")
        {
            PreferencesPane prefs = (PreferencesPane)sender;
            string userID = this.BusinessObject.UserNameExists(this.User.Identity.Name, prefs.HitsPerPage);
            int hit = this.BusinessObject.GetHits(Convert.ToInt16(userID));
            this.DefaultGridPageSize = hit;
            this.DefaultListViewFormIndex = prefs.ListViewFormIndex;
            this.DefaultDetailsViewFormIndex = prefs.DetailsViewFormIndex;
            this.SearchAction = prefs.IsListSearchAction ? FormGroup.CurrentFormEnum.ListForm : FormGroup.CurrentFormEnum.DetailForm;
            this.FilterChildData = prefs.FilterChildData;
            this.HighlightSubStructures = prefs.HighlightSubStructures;

            bool applyChanges = e.CustomInfo.Length > 0 && e.CustomInfo[0].ToLower() == "yes";
            if (applyChanges)
            {
                this.SelectedListViewFormIndex = this.DefaultListViewFormIndex;
                this.SelectedDetailsViewFormIndex = this.DefaultDetailsViewFormIndex;
                this.BusinessObject.PageSize = this.GridPageSize = this.DefaultGridPageSize;
                if (this.HitlistToRefine != null)
                    this.BusinessObject.HitListToRefine = this.HitlistToRefine;

                PagingInfo info = this.GetPagingInfoClone();
                info.FilterChildData = this.FilterChildData;
                info.HighlightSubStructures = this.HighlightSubStructures;
                this.BusinessObject.PagingInfo = info;

                this.SetPageMode(this.CurrentMode);
                _formGroup.DataBind();
            }
        }
    }

    void QueryManagement_CommandRaised(object sender, COENavigationPanelControlEventArgs e)
    {

        HitListType hitlistType = (HitListType)Enum.Parse(typeof(HitListType), e.CustomInfo[0]);
        int hitlistId = int.Parse(e.CustomInfo[1]);
        COEHitListBO hl = null;
        switch (e.EventType)
        {
            case "RestoreHitlist":
                Session["RestoringHitList"] = "RestoringHitList";
                this.RestoreHitList(hitlistType, hitlistId);
                break;
            case "PerformQuery":
                hl = COEHitListBO.Get(hitlistType, hitlistId);
                if (hl.SearchCriteriaID > 0)
                {

                    COESearchCriteriaBO sc = COESearchCriteriaBO.Get(hl.SearchCriteriaType, hl.SearchCriteriaID);
                    SearchCriteria baseSearchCriteria = SearchFormGroupAdapter.GetSearchCriteria(_formGroup.FormGroupDescription.QueryForms[_formGroup.CurrentFormIndex]);
                    foreach (SearchCriteria.SearchCriteriaItem item in sc.SearchCriteria.Items)
                    {
                        if (item.ID >= 0)
                            SearchFormGroupAdapter.PopulateSearchCriteria(baseSearchCriteria, item);
                    }
                    this.BusinessObject.SearchCriteria = baseSearchCriteria;
                    this.CurrentMode = this.SearchAction;
                    this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = 1;
                }
                break;
            case "RestoreQueryToForm":

                this.Browsing = false;

                if (!string.IsNullOrEmpty(URLReferrer))
                {
                    Response.Redirect(URLReferrer);
                }

                BusinessObject.CurrentFormType = FormGroup.CurrentFormEnum.QueryForm;
                BusinessObject.CurrentFormIndex = _formGroup.CurrentFormIndex;

                CurrentMode = FormGroup.CurrentFormEnum.QueryForm;

                hl = COEHitListBO.Get(hitlistType, hitlistId);
                if (hl.SearchCriteriaID > 0)
                {

                    COESearchCriteriaBO sc = COESearchCriteriaBO.Get(hl.SearchCriteriaType, hl.SearchCriteriaID);
                    SearchCriteria baseSearchCriteria = SearchFormGroupAdapter.GetSearchCriteria(_formGroup.FormGroupDescription.QueryForms[_formGroup.CurrentFormIndex]);
                    foreach (SearchCriteria.SearchCriteriaItem item in sc.SearchCriteria.Items)
                    {
                        if (item.ID >= 0)
                            SearchFormGroupAdapter.PopulateSearchCriteria(baseSearchCriteria, item);
                    }
                    this.BusinessObject.SearchCriteria = baseSearchCriteria;
                    this.BusinessObject.CurrentHitList = null;
                }
                _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                break;
        }

        if (_masterPage.AccordionControl.Groups.FromKey("QueryManagement") != null)
            ((ManagerPane)_masterPage.AccordionControl.Groups.FromKey("QueryManagement").UserControl).DataBind();

        _formGroup.DataBind();
    }
    #endregion

    #region Methods
    private void OverrideMenuItemsEnabled()
    {
        if (this.CurrentMode == FormGroup.CurrentFormEnum.QueryForm)
        {
            bool areHitLists = COEHitListBOList.GetRecentHitLists(this.BusinessObject.DataView.Database, COEUser.Name, this.BusinessObject.DataView.DataViewID, 1).Count > 0;
            bool areSearchCriterias = COESearchCriteriaBOList.GetRecentSearchCriteria(COEUser.Name, this.BusinessObject.DataView.DataViewID, this.BusinessObject.DataView.Database, 1).Count > 0;

            this.SearchQueryMenuButton.MenuItemList[0].Enabled = areHitLists;
            this.SearchQueryMenuButton.MenuItemList[1].Enabled = this.SaveQueryHistory && areSearchCriterias;
            this.SearchQueryMenuButton.MenuItemList[2].Enabled = areSearchCriterias;
        }
        else if (this.CurrentMode == FormGroup.CurrentFormEnum.ListForm)
        {
            //Do not allow to save a hiltist when there actually is no hitlist (IE: retrieve all button was pressed)
            COEHitListBO hl = COEHitListBO.Get(this.BusinessObject.CurrentHitList.HitListType, this.BusinessObject.CurrentHitList.HitListID);
            bool areCurrentHL = (hl == null || hl.HitListID > 0);
            this.QueryMenuButton.MenuItemList[0].Enabled = areCurrentHL;
        }

    }

    private PagingInfo GetPagingInfoClone()
    {
        PagingInfo info = new PagingInfo();
        info.Start = this.BusinessObject.PagingInfo.Start;
        info.End = this.BusinessObject.PagingInfo.End;
        info.RecordCount = this.BusinessObject.PagingInfo.RecordCount;
        info.HitListID = this.BusinessObject.PagingInfo.HitListID;
        info.HitListType = this.BusinessObject.PagingInfo.HitListType;
        info.KeepAlive = this.BusinessObject.PagingInfo.KeepAlive;
        info.PagingInfoID = this.BusinessObject.PagingInfo.PagingInfoID;
        info.TransactionId = this.BusinessObject.PagingInfo.TransactionId;
        info.FilterChildData = this.BusinessObject.PagingInfo.FilterChildData;
        return info;
    }

    private bool RequiresRestoringState
    {
        get
        {
            return RememberPreviousState &&
                    PreviousFormGroupId != -1 &&
                    PreviousFormGroupId == FormGroupId;
        }
    }

    private void SetGridSizeDDLSelection()
    {
        if (GridSizeDropDownList.Items.FindByValue(GridPageSize.ToString()) == null)
        {
            if (int.Parse(this.GridSizeDropDownList.Items[0].Value) > this.GridPageSize)
            {
                this.GridSizeDropDownList.Items.Insert(0, new ListItem(GridPageSize.ToString(), GridPageSize.ToString()));
            }
            else
            {
                for (int i = 0; i < GridSizeDropDownList.Items.Count; i++)
                {
                    if (int.Parse(this.GridSizeDropDownList.Items[i].Value) > this.GridPageSize)
                    {
                        this.GridSizeDropDownList.Items.Insert(i, new ListItem(GridPageSize.ToString(), GridPageSize.ToString()));
                        break;
                    }
                }
            }
        }

        this.GridSizeDropDownList.SelectedValue = GridPageSize.ToString();
    }

    private bool RestorePreviousState()
    {
        bool requiresRestoringState = this.RequiresRestoringState;

        //Restore search state
        if (!RequiresRestoringState/* || noResultsSearchPerformed*/)
        {
            this.PreviousFormGroupId = FormGroupId;

            if (Request.QueryString["HitListID"] == null)
                this.BusinessObject = null;

            CurrentMode = FormGroup.CurrentFormEnum.QueryForm;
            _formGroup.CurrentFormIndex = this.GetQueryFormIndex(this.DefaultQueryForm);
            _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
            this.PreviousPageIndex = this.CoePagerControl.CurrentPage;
            this.GridPageSize = this.DefaultGridPageSize;

            if (Request.QueryString["SearchCriteriaValue"] != null)
            {
                int searchCriteriaId = int.Parse(Request.QueryString["SearchCriteriaId"]);
                string searchCriteriaValue = Request.QueryString["SearchCriteriaValue"];

                BusinessObject.CleanSearchCriteria();
                SearchFormGroupAdapter.PopulateSearchCriteria(BusinessObject.SearchCriteria, searchCriteriaId, searchCriteriaValue);

                this.CurrentMode = this.SearchAction;
                _formGroup.DataBind();
            }
            else
            {
                BusinessObject.CleanSearchCriteria();
            }
        }
        else
        {
            int tempIndex = PreviousPageIndex;
            CurrentMode = PreviousDisplayGroup;
            this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = tempIndex;

            if (Request.QueryString["HitListID"] != null && Request.QueryString["HitListType"] != null)
                this.BusinessObject.HitListToRestore = COEHitListBO.Get((HitListType)Enum.Parse(typeof(HitListType), Request.QueryString["HitListType"]), int.Parse(Request.QueryString["HitListID"])).HitListInfo;
            else
            {
                this.Browsing = true;
                this.BusinessObject.AllowFullScan = true;
            }
            if (BusinessObject.TotalRecordCount == 0)
                this.Refine(false);

        }


        return true;
    }

    private void SetCurrentPage(int currentPage)
    {

        PreviousPageIndex = currentPage;

        if (_formGroup.CurrentDisplayMode == FormGroup.DisplayMode.Edit)
        {
            _formGroup.Update();
        }

        BusinessObject.CurrentRecordIndex = currentPage * CoePagerControl.PageSize;

        _formGroup.SetFormGeneratorsPageIndex(BusinessObject.CurrentRecordIndex - BusinessObject.PagingInfo.Start);
    }

    private void UpdateTimer()
    {
        if (this.BusinessObject != null)
        {
            this.CoePagerControl.IsStillGettingResults = this.CoePagerControlBottom.IsStillGettingResults = this.MainPageTimer.Enabled = this.BusinessObject.RequiresRefreshingHitList;

            MaxRecordsReachedLabel.Visible = !this.Browsing && this.BusinessObject.MaxRecordCount > 0 && this.CoePagerControl.RecordCount >= this.BusinessObject.MaxRecordCount;

            if (this.BusinessObject.RequiresRefreshingHitList)
                DisplayRecordCount(this.BusinessObject.RefreshHitList());
        }
        else
            DisableTimer();
    }

    private void DisableTimer()
    {
        if (this.CoePagerControl != null)
            this.CoePagerControl.IsStillGettingResults = this.CoePagerControlBottom.IsStillGettingResults = false;

        if (this.MainPageTimer != null)
            this.MainPageTimer.Enabled = false;
    }

    private void UpdateMarkedCount()
    {
        int count = this.BusinessObject.GetMarkedCount();
        MarkedCountLabel.Text = Resource.MarkedHitsCount + ": " + count.ToString();
        MarkedCountLabel.Visible = true;
        if (this.ActionLinks != null)
        {
            foreach (ActionLink currentActionLink in this.ActionLinks)
                EnableActionLink(currentActionLink, count != 0);
        }
    }

    private void EnableActionLink(ActionLink actionLink, bool enabled)
    {
        actionLink.Enabled = enabled;

        HtmlAnchor anchor = (HtmlAnchor)(ActionLinksContainer.FindControl(actionLink.Id));
        if (anchor != null)
        {
            anchor.Disabled = !enabled;
            anchor.Attributes.Add("confirmationMessage", actionLink.ConfirmationMessage == null ? string.Empty : actionLink.ConfirmationMessage);
            anchor.Attributes["onclick"] = "if(this.getAttribute(\'disabled\')) return false; else if(this.getAttribute(\'confirmationMessage\') != '') return window.confirm(\'' + this.getAttribute(\'confirmationMessage\') + '\'); else return true;";
            if (enabled)
            {
                anchor.Style.Value = "cursor:pointer;";
            }
            else
            {
                anchor.Style.Value = "color:grey;cursor:default;";
            }
        }
    }

    private int GetQueryFormIndex(int DefaultQueryForm)
    {
        COEFormGroup theCOEFormGroup = this._formGroup;
        if (theCOEFormGroup != null && theCOEFormGroup.QueryFormCollection != null && theCOEFormGroup.QueryFormCollection.Displays != null && 
            theCOEFormGroup.QueryFormCollection.Displays.Displays != null)
        {
            for (int index = 0; index < theCOEFormGroup.QueryFormCollection.Displays.Displays.Count; index++)
            {
                FormGroup.Display currentDisplay = theCOEFormGroup.QueryFormCollection[index];
                //Coverity Fix for CID : 11713
                if (currentDisplay != null && currentDisplay.Id == DefaultQueryForm)
                    return index;
            }
        }

        return 0;
    }

    public override Control FindControl(string id)
    {
        Control result = base.FindControl(id);
        if (result == null)
            result = _masterPage.FindControlInPage(id);

        return result;
    }

    private void UnMarkHit(string hitID, string columnIDBindingExpression)
    {
        this.BusinessObject.UnMarkHit(int.Parse(hitID), columnIDBindingExpression);
    }

    private void UnMarkAllHits()
    {
        this.BusinessObject.UnMarkAllHits();

        _formGroup.DataBind();
    }

    private void MarkAllHits()
    {
        this.BusinessObject.MarkAllHits();
        _formGroup.DataBind();
    }

    private bool MarkHit(string hitID, string columnIDBindingExpression)
    {
        return this.BusinessObject.MarkHit(int.Parse(hitID), columnIDBindingExpression);
    }

    private void Search()
    {
        this.Browsing = false;
        this.SelectedListViewFormIndex = this.DefaultListViewFormIndex;
        this.SelectedDetailsViewFormIndex = this.DefaultDetailsViewFormIndex;
        _formGroup.Update();
        if (this.HitlistToRefine != null)
            this.BusinessObject.HitListToRefine = this.HitlistToRefine;
        this.BusinessObject.Dataset = null;// empty the dataset if any previous data persists
        this.CurrentMode = this.SearchAction;
        this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = 1;
    }

    private void RetrieveAll()
    {
        if (this.HitlistToRefine != null)
            this.RestoreHitList(this.HitlistToRefine.HitListType, this.HitlistToRefine.HitListID);
        else
        {


            this.Browsing = true;  //set the search to browse mode (retrieve all)
            this.BusinessObject.AllowFullScan = true;
            _formGroup.Update();
            this.BusinessObject.SearchCriteria = null; //must clear any search criteria in the form or it performs a retreive all but gets confused because there is search criteria
            this.SelectedListViewFormIndex = this.DefaultListViewFormIndex;
            this.SelectedDetailsViewFormIndex = this.DefaultDetailsViewFormIndex;

            this.BusinessObject.HitListToRefine = null; //we are not doing a refine here
            this.BusinessObject.Dataset = null; // empty the dataset if any previous data persists
            this.CurrentMode = this.SearchAction;
            this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = 1;
        }
        //Fix for CSBR-167906. Here when a user Retrieve all new query export control div should be handled.
        ExportControlDiv.Style.Add("display", "none");
        expcontrolvisibility.Value = "false";
    }

    private void Refine(bool overcurrent)
    {
        this.Browsing = false;

        if (!string.IsNullOrEmpty(URLReferrer))
            Response.Redirect(URLReferrer);

        BusinessObject.CurrentFormType = FormGroup.CurrentFormEnum.QueryForm;
        BusinessObject.CurrentFormIndex = _formGroup.CurrentFormIndex;

        CurrentMode = FormGroup.CurrentFormEnum.QueryForm;
        if (overcurrent)
        {
            if (this.BusinessObject.CurrentHitList.HitListID > 0)
                this.BusinessObject.HitListToRefine = this.HitlistToRefine = this.BusinessObject.CurrentHitList;

            _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
        }
        else
        {
            this.BusinessObject.HitListToRefine = this.HitlistToRefine = null;
            COESearchCriteriaBOList list = COESearchCriteriaBOList.GetRecentSearchCriteria(COEUser.Name, this.BusinessObject.DataView.DataViewID, this.BusinessObject.DataView.Database, 1);

            if (list.Count > 0)
            {
                SearchCriteria baseSearchCriteria = SearchFormGroupAdapter.GetSearchCriteria(_formGroup.FormGroupDescription.QueryForms[_formGroup.CurrentFormIndex]);
                foreach (SearchCriteria.SearchCriteriaItem item in list[0].SearchCriteria.Items)
                {
                    if (item.ID >= 0)
                        SearchFormGroupAdapter.PopulateSearchCriteria(baseSearchCriteria, item.ID, item.Criterium.Value);
                }
                this.BusinessObject.SearchCriteria = baseSearchCriteria;
                this.BusinessObject.CurrentHitList = null;
            }
            _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
        }
        ExportControlDiv.Style.Add("display", "none");
        expcontrolvisibility.Value = "false";
    }

    private void NewQuery()
    {
        this.Browsing = false;
        this.BusinessObject.HitListToRefine = this.HitlistToRefine = null;

        if (!string.IsNullOrEmpty(URLReferrer))
            Response.Redirect(URLReferrer);

        // Fix for CSBR-134432
        this.RestorePreviousState();
        // End Fix

        BusinessObject.CurrentFormType = FormGroup.CurrentFormEnum.QueryForm;
        BusinessObject.CurrentFormIndex = _formGroup.CurrentFormIndex;
        BusinessObject.CleanSearchCriteria();
        this.RestoringHitList = false;
        CurrentMode = FormGroup.CurrentFormEnum.QueryForm;
        _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
        //Fix for CSBR-167906. Here when a user performs new query export control div should be handled.
        ExportControlDiv.Style.Add("display", "none");
        expcontrolvisibility.Value = "false";
    }

    private void GoToHits()
    {
        CurrentMode = FormGroup.CurrentFormEnum.ListForm;
    }

    private void ExportHits(string exportCommand, ResultsCriteria exportRC)
    {

        if (exportCommand.Remove(0, 6).Trim().Equals("XML", StringComparison.OrdinalIgnoreCase))
        {
            COEAdvancedExport coeAdvxml = new COEAdvancedExport();
            PagingInfo exportPI = new PagingInfo();

            exportPI.HitListID = this.BusinessObject.GetHitList().HitListID;
            if (exportPI.HitListID > 0)
                exportPI.RecordCount = this.BusinessObject.GetHitList().RecordCount;

            else
                exportPI.RecordCount = this.BusinessObject.TotalRecordCount;

            string exportedXMLCriteria = coeAdvxml.GetData(this.BusinessObject.DataViewId, this.BusinessObject.SearchCriteria, exportRC, exportPI, Csla.ApplicationContext.User.Identity.Name.ToString(), "SARSheetExport", exportCommand.Remove(0, 6));


            _exportedHits = true;
            Session["ExportType"] = "Export";
            Session["exportedData"] = exportedXMLCriteria;
            //save exported hitlist
            Session["exportedDataFormat"] = "xml";
            Session["exportedDataMIME"] = "application/vnd.xml";
            Session["exportedCount"] = exportPI.RecordCount;

            // It doesn't come from a retrieve all
            if (exportPI.HitListID > 0)
            {
                COEHitListBO sHitListBO = COEHitListBO.Get(HitListType.TEMP, exportPI.HitListID);
                sHitListBO.Name = "Export Hits";
                sHitListBO.Description = "Hits Exported " + DateTime.Now.ToString();
                sHitListBO = sHitListBO.Save();
            }

        }
        else
        {

            COEExport coex = new COEExport();
            PagingInfo exportPI = new PagingInfo();
            exportPI.HitListID = this.BusinessObject.GetHitList().HitListID;
            if (exportPI.HitListID > 0)
                exportPI.RecordCount = this.BusinessObject.GetHitList().RecordCount;

            else
                exportPI.RecordCount = this.BusinessObject.DatabaseRecordCount;


            string exportedData = coex.GetData(exportRC, exportPI, this.BusinessObject.DataViewId, exportCommand.Remove(0, 6));

            //CSBR-138818 Replacing <sub> with null while export.
            if (exportedData != null)
                exportedData = exportedData.Replace("<sub>", "").Replace("</sub>", "");

            _exportedHits = true;
            Session["ExportType"] = "Export";
            Session["exportedData"] = exportedData;
            //save exported hitlist
            Session["exportedDataFormat"] = "sdf";
            Session["exportedDataMIME"] = "chemical/x-mdl-sdfile";
            Session["exportedCount"] = exportPI.RecordCount;

            if (exportCommand.Contains("Excel"))
            {
                Session["exportedDataFormat"] = "xls";
                Session["exportedDataMIME"] = "application/vnd.ms-excel";
            }

            // It doesn't come from a retrieve all
            if (exportPI.HitListID > 0)
            {
                COEHitListBO sHitListBO = COEHitListBO.Get(HitListType.TEMP, exportPI.HitListID);
                sHitListBO.Name = "Export Hits";
                sHitListBO.Description = "Hits Exported " + DateTime.Now.ToString();
                sHitListBO = sHitListBO.Save();
            }
        }
        CurrentMode = this.CurrentMode;
    }

    private void RestoreHitList(HitListType type, int hitlistID)
    {
        COEHitListBO hitListBO = COEHitListBO.Get(type, hitlistID);
        RestoreHitList(hitListBO.HitListInfo);
    }

    private void RestoreHitList(HitListInfo hitlist)
    {
        BusinessObject.CreateNewHitList = (Session["RestoringHitList"] != null) ? false : true;
        Session.Remove("RestoringHitList");
        BusinessObject.HitListToRestore = hitlist;
        this.BusinessObject.AllowFullScan = true;
        this.CurrentMode = this.SearchAction;
        this.Browsing = false;
        this.RestoringHitList = true;
        this.DisplayRecordCount(hitlist.HitListID < 0 ? BusinessObject.DatabaseRecordCount : hitlist.RecordCount);
        this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = 1;
        _formGroup.DataBind();
    }

    private void SetPageMode(FormGroup.CurrentFormEnum currentFormEnum)
    {
        _formGroup.CurrentDisplayGroup = currentFormEnum;
        PreviousDisplayGroup = currentFormEnum;

        switch (this.CurrentMode)
        {
            case FormGroup.CurrentFormEnum.QueryForm:
                this.LabelForGridSize.Visible = this.GridSizeDropDownList.Visible = false;
                this.MainPageTimer.Enabled = false;
                _formGroup.Visible = true;
                this.MaxRecordsReachedLabel.Visible = false;
                this.MarkedCountLabel.Visible = false;
                this.ErrorContainerDiv.Visible = this.CoePagerControl.Visible = this.CoePagerControlBottom.Visible = false;
                this.SearchQueryMenuButton.Visible = this.ClearImageButton.Visible = this.RetrieveAllImageButton.Visible = this.SearchImageButton.Visible = true;
                this.PrintMenuButton.Visible = this.MarkedMenuButton.Visible = this.QueryMenuButton.Visible = this.NewQueryMenuButton.Visible = this.RefineImageMenuButton.Visible = this.HitsMenuButton.Visible = this.imgExport.Visible = this.lnkbtnExport.Visible = false;
                this.RetrieveAllImageButton.UseSubmitBehavior = this.SearchImageButton.UseSubmitBehavior = true;
                this.ActionLinksContainer.Visible = false;
                if (_masterPage.AccordionControl.Groups.FromKey("ExportHits") != null)
                    _masterPage.AccordionControl.Groups.FromKey("ExportHits").Enabled = false;
                _masterPage.SetDefaultButton(this.SearchImageButton.GetButtonUniqueID());
                break;
            case FormGroup.CurrentFormEnum.DetailForm:
                UpdateMarkedCount();
                this.LabelForGridSize.Visible = this.GridSizeDropDownList.Visible = false;
                this.MainPageTimer.Enabled = true;
                _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                _formGroup.CurrentFormIndex = this.SelectedDetailsViewFormIndex;
                this.CoePagerControl.Visible = this.CoePagerControlBottom.Visible = true;
                this.ClearImageButton.Visible = this.SearchImageButton.Visible = false;
                this.SearchQueryMenuButton.Visible = this.RetrieveAllImageButton.Visible = this.MarkedMenuButton.Visible = this.QueryMenuButton.Visible = this.RefineImageMenuButton.Visible = this.imgExport.Visible = this.lnkbtnExport.Visible = false;
                this.PrintMenuButton.Visible = this.NewQueryMenuButton.Visible = this.HitsMenuButton.Visible = true;
                this.CoePagerControl.PageSize = this.CoePagerControlBottom.PageSize = 1;
                this.BusinessObject.PageSize = this.BufferSize;
                this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = this.BusinessObject.PagingInfo.Start + PageOffset - 1;
                this.ActionLinksContainer.Visible = false;
                _formGroup.DataBind();
                _masterPage.SetDefaultButton(this.HitsMenuButton.GetButtonUniqueID());
                break;

            case FormGroup.CurrentFormEnum.ListForm:
                UpdateMarkedCount();
                this.LabelForGridSize.Visible = this.GridSizeDropDownList.Visible = true;
                this.MainPageTimer.Enabled = true;
                _formGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                _formGroup.CurrentFormIndex = this.SelectedListViewFormIndex;
                this.CoePagerControl.Visible = this.CoePagerControlBottom.Visible = true;
                this.SearchQueryMenuButton.Visible = this.HitsMenuButton.Visible = this.ClearImageButton.Visible = this.RetrieveAllImageButton.Visible = this.SearchImageButton.Visible = false;
                this.PrintMenuButton.Visible = this.QueryMenuButton.Visible = this.RefineImageMenuButton.Visible = this.NewQueryMenuButton.Visible = this.imgExport.Visible = this.lnkbtnExport.Visible = true;
                this.BusinessObject.PageSize = this.CoePagerControl.PageSize = this.CoePagerControlBottom.PageSize = this.GridPageSize;
                this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = (this.BusinessObject.PagingInfo.Start + this.BusinessObject.PageSize) / this.BusinessObject.PageSize;
                this.MarkedCountLabel.Visible = this.MarkedMenuButton.Visible = this.ActionLinksContainer.Visible = this.HasMarkHitFormElement;
                _masterPage.SetDefaultButton(this.NewQueryMenuButton.GetButtonUniqueID());
                break;
        }

        if (_masterPage.AccordionControl.Groups.FromKey("QueryManagement") != null)
        {
            _masterPage.AccordionControl.Groups.FromKey("QueryManagement").Enabled = this.EnableQueryManagement;
        }
        this.ConfigureMenuButtons();
        this.FillFormsDropDown(this._formGroup.CurrentCollection);
    }

    private void ConfigureMenuButtons()
    {
        this.SearchQueryMenuButton.Enabled &= this.EnableRestoreMenu;
        this.RefineImageMenuButton.Enabled &= this.EnableRefineMenu;
        this.QueryMenuButton.Enabled &= this.EnableQueryMenu;
        this.MarkedMenuButton.Enabled &= this.EnableMarkedMenu;

        this.lnkbtnExport.Enabled &= this.EnableExportMenu;
        this.imgExport.Enabled &= this.EnableExportMenu;
        this.PrintMenuButton.Enabled &= this.EnablePrintMenu;
        this.GridSizeDropDownList.Enabled &= this.EnableResultsPerPageMenu;

        this.SearchQueryMenuButton.Visible &= this.ShowRestoreMenu;
        this.RefineImageMenuButton.Visible &= this.ShowRefineMenu;
        this.QueryMenuButton.Visible &= this.ShowQueryMenu;
        this.QueryMenuButton.MenuItemList[1].Enabled = this.EnableQueryManagement;

        this.MarkedMenuButton.Visible &= this.ShowMarkedMenu;
        this.lnkbtnExport.Visible &= this.ShowExportMenu;
        this.imgExport.Visible &= this.ShowExportMenu;
        this.PrintMenuButton.Visible &= this.ShowPrintMenu;
        this.LabelForGridSize.Visible = this.GridSizeDropDownList.Visible &= this.ShowResultsPerPageMenu;

    }

    private void LoadFormGenerator()
    {
        if (Session["FormGroup"] == null || !string.IsNullOrEmpty(Request.QueryString["FormGroupId"]))
        {
            COEFormBO coeFormService = COEFormBO.Get(FormGroupId);
            Session["FormGroup"] = coeFormService.COEFormGroup;
        }

        PlaceHolder placeHolder = GetFormGeneratorHolder();

        _formGroup = new COEFormGroup();
        _formGroup.ID = "FormGenerator";
        _formGroup.FormGroupDescription = (FormGroup)Session["FormGroup"];

        if (!Page.IsPostBack)
            _formGroup.DataSourceId = ChemBioVizSearchCSLADataSource.ID;

        placeHolder.Controls.Clear();
        placeHolder.Controls.Add(_formGroup);
    }

    private PlaceHolder GetFormGeneratorHolder()
    {
        PlaceHolder placeHolder = (PlaceHolder)this.FindControl("FormGeneratorHolder");

        if (placeHolder == null)
        {
            placeHolder = new PlaceHolder();
            placeHolder.ID = "FormGeneratorHolder";
            this.Controls.Add(placeHolder);
        }

        return placeHolder;
    }

    private void ClearErrorMessage()
    {
        ErrorContainerDiv.Visible = false;
        this.errorLabel.Text = string.Empty;
        this.StackTraceLabel.Text = string.Empty;
    }

    private void ShowErrorInMessagesPage(Constants.MessagesCode code, GUIShellTypes.MessagesButtonType buttonType)
    {
        string url = this.ResolveUrl("~/Forms/Public/ContentArea/Messages.aspx");
        url += "?" + GUIShellTypes.RequestVars.MessageCode.ToString() + "=" + code.ToString();
        url += "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() + "=" + buttonType.ToString();
        Server.Transfer(url, false);
    }

    private void FillFormsDropDown(COEFormCollection collection)
    {
        FormsDropDownList.Items.Clear();

        if (collection.Displays.Displays.Count > 0)
        {
            for (int index = 0; index < collection.Displays.Displays.Count; index++)
            {
                FormGroup.Display currentDisplay = collection.Displays.Displays[index];
                if ((currentDisplay.Name != null) && (currentDisplay.Name != String.Empty))
                {
                    this.FormsDropDownList.Items.Add(new ListItem(currentDisplay.Name.ToString(), index.ToString()));
                }
                else
                {
                    this.FormsDropDownList.Items.Add(new ListItem("Form_" + currentDisplay.Id.ToString(), index.ToString()));
                }
                switch (this.CurrentMode)
                {
                    case FormGroup.CurrentFormEnum.QueryForm:
                        if (currentDisplay.Id == this.DefaultQueryForm)
                            this.FormsDropDownList.Items[this.FormsDropDownList.Items.Count - 1].Selected = true;
                        break;
                    case FormGroup.CurrentFormEnum.ListForm:
                        if (currentDisplay.Id == this.SelectedListViewFormIndex)
                            this.FormsDropDownList.Items[this.FormsDropDownList.Items.Count - 1].Selected = true;
                        break;
                    case FormGroup.CurrentFormEnum.DetailForm:
                        if (currentDisplay.Id == this.SelectedDetailsViewFormIndex)
                            this.FormsDropDownList.Items[this.FormsDropDownList.Items.Count - 1].Selected = true;
                        break;
                }
            }
        }
        else
            this.FormsDropDownList.Items.Add(new ListItem("No Forms To Display", "0"));

        this.FormsDropDownList.Enabled = this.FormsDropDownList.Visible = (collection.Displays.Displays.Count > 1);
    }

    private void DisplayRecordCount(int recordCount)
    {
        this.ClearErrorMessage();
        if (recordCount == 0)
        {
            this._formGroup.Visible = false;

            // CBOE-1791, if there are zero records then all action links should be disabled
            for (int i = 0; i < this.ActionLinks.Count; i++)
            {
                this.ActionLinks[i].Enabled = false;
            }

            this.CoePagerControl.Visible = this.CoePagerControlBottom.Visible = false;
            this.NoRecordsFoundLabel.Text = Resources.Resource.NoRecordsFound_Label_Text;
            this.NoRecordsFoundLabel.Visible = this.NoRecordsFoundDiv.Visible = true;
        }
        else
        {
            this._formGroup.Visible = true;
            this.CoePagerControl.Visible = this.CoePagerControlBottom.Visible = true;
            this.CoePagerControl.RecordCount = this.CoePagerControlBottom.RecordCount = recordCount;
        }
    }

    protected override void SetControlsAttributtes()
    {
        this.MaxRecordsReachedLabel.Text = Resources.Resource.MaxRecordsReachedString;
        this.GridSizeDropDownList.ClearSelection();
        this.LabelForGridSize.InnerText = Resource.HitsPerPage_Label_Text;
    }

    private bool HasMarkHitFormElement
    {
        get
        {
            foreach (FormGroup.Display currentDisplay in this._formGroup.CurrentCollection.Displays.Displays)
            {
                if (currentDisplay is FormGroup.ListDisplay)
                {
                    foreach (ResultsCriteria.ResultsCriteriaTable currentTable in ((FormGroup.ListDisplay)currentDisplay).ResultsCriteria.Tables)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase currentCriteria in currentTable.Criterias)
                            if (currentCriteria is ResultsCriteria.Marked)
                                return true;
                    }
                }
            }
            return false;
        }
    }

    private void BuildActionLinkButtons()
    {
        this.ActionLinksContainer.Controls.Clear();
        if (this.ActionLinks != null)
        {
            int i = 0;
            foreach (ActionLink linkConfig in this.ActionLinks)
            {
                if (this.UserHasPrivilege(linkConfig.Privileges))
                {
                    HtmlAnchor link = new HtmlAnchor();
                    link.ID = !string.IsNullOrEmpty(linkConfig.Id) ? linkConfig.Id : (++i).ToString();
                    this.ActionLinksContainer.Controls.Add(link);
                    link.HRef = string.Format(linkConfig.HRef, this.BusinessObject.MarkedHitList.ID);
                    link.InnerText = linkConfig.Text;
                    link.Target = !string.IsNullOrEmpty(linkConfig.Target) ? linkConfig.Target : "_parent";
                    link.Title = linkConfig.Tooltip;
                    link.Attributes.Add("class", linkConfig.CssClass);
                    link.Attributes.Add("confirmationMessage", linkConfig.ConfirmationMessage == null ? string.Empty : linkConfig.ConfirmationMessage);
                    link.Disabled = !linkConfig.Enabled;
                    link.Attributes["onclick"] = "if(this.getAttribute(\'disabled\')) return false; else if(this.getAttribute(\'confirmationMessage\') != '') return window.confirm(\'' + this.getAttribute(\'confirmationMessage\') + '\'); else return true;";
                    if (linkConfig.Enabled)
                    {
                        link.Style.Value = "cursor:pointer;";
                    }
                    else
                    {
                        link.Style.Value = "color:grey;cursor:default;";
                    }
                }
            }
        }
    }

    private bool UserHasPrivilege(string privileges)
    {
        string[] privilegeList = privileges.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
        bool hasPriv = false;
        if (privilegeList.Length > 0)
        {
            foreach (string priv in privilegeList)
            {
                if (COEPrincipal.HasPrivilege(priv.Trim(), string.Empty))
                {
                    hasPriv = true;
                    break;
                }
            }
        }
        else
            hasPriv = true;

        return hasPriv;
    }

    private void SetTitleText()
    {
        //The GUiShell page will set up the first part of the string according a configuration setting. 
        switch (this.CurrentMode)
        {
            case FormGroup.CurrentFormEnum.DetailForm:
                this.Page.Title += Resources.Resource.Details_Page_Title;
                break;
            case FormGroup.CurrentFormEnum.ListForm:
                this.Page.Title += Resources.Resource.List_Page_Title;
                break;
            case FormGroup.CurrentFormEnum.QueryForm:
                this.Page.Title += Resources.Resource.Query_Page_Title;
                break;
        }
    }

    private void ChangeCurrentFormIndex(int index)
    {
        switch (this.CurrentMode)
        {
            case FormGroup.CurrentFormEnum.DetailForm:
                this.SelectedDetailsViewFormIndex = index;
                break;
            case FormGroup.CurrentFormEnum.ListForm:
                this.SelectedListViewFormIndex = index;
                break;
        }
        _formGroup.CurrentFormIndex = index;
        _formGroup.SetFormGeneratorsPageIndex(BusinessObject.CurrentRecordIndex - BusinessObject.PagingInfo.Start);
    }
    #endregion

    #region GUIShell Methods
    /// <summary>
    /// Load the control/s in the right accordion's panel
    /// </summary>
    /// <remarks>You should show as expanded the group that you are interested</remarks> 
    protected void LoadControlInAccordionPanel()
    {
        ICOENavigationPanelControl prefsUserControl = _masterPage.AddControlToAccordionPanel(this.GetPreferencesRow(), false);
        if (this.ShowSearchPreferences)
        {
            ((PreferencesPane)prefsUserControl).BindData(_formGroup.FormGroupDescription);
            prefsUserControl.CommandRaised += new EventHandler<COENavigationPanelControlEventArgs>(Preferences_CommandRaised);
        }
        else if (_masterPage.AccordionControl.Groups.FromKey("Preferences") != null)
            _masterPage.AccordionControl.Groups.Remove(_masterPage.AccordionControl.Groups.FromKey("Preferences"));

        ICOENavigationPanelControl queryManagementUserControl = _masterPage.AddControlToAccordionPanel(this.GetHitListRow(), false);
        if (this.ShowQueryManagement)
            queryManagementUserControl.CommandRaised += new EventHandler<COENavigationPanelControlEventArgs>(QueryManagement_CommandRaised);
        else if (_masterPage.AccordionControl.Groups.FromKey("QueryManagement") != null)
            _masterPage.AccordionControl.Groups.Remove(_masterPage.AccordionControl.Groups.FromKey("QueryManagement"));
    }

    private COENavigationPane.ControlItemRow GetHitListRow()
    {
        COENavigationPane.ControlItemRow navigationControlRow = _masterPage.COENavigationPaneObject.ControlItem.NewControlItemRow();
        navigationControlRow.ParentID = "QueryManagement";
        navigationControlRow.ControlType = GUIShellTypes.SupportedControlTypes.UserControl.ToString();
        if (navigationControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
            navigationControlRow.ControlSource = GUIShellTypes.UCPath + "QueryManagerPane.ascx";
        navigationControlRow.ID = "QueryManagerPane";
        return navigationControlRow;
    }

    /// <summary>
    /// Method which load the Control Info in a DataSet (just for testing purpose)
    /// This Data Object will come from the Configuration Services
    /// </summary>
    private COENavigationPane.ControlItemRow GetPreferencesRow()
    {
        COENavigationPane.ControlItemRow navigationControlRow = _masterPage.COENavigationPaneObject.ControlItem.NewControlItemRow();
        navigationControlRow.ParentID = "Preferences";
        navigationControlRow.ControlType = GUIShellTypes.SupportedControlTypes.UserControl.ToString();
        if (navigationControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
            navigationControlRow.ControlSource = GUIShellTypes.UCPath + "PreferencesPane.ascx";
        navigationControlRow.ID = "PreferencesPane";

        return navigationControlRow;
    }
    #endregion

    #region CSLA Datasource Events
    protected void CslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
        if (CurrentMode == FormGroup.CurrentFormEnum.QueryForm)
        {
            this.BusinessObject.CurrentFormType = _formGroup.CurrentDisplayGroup;
            this.BusinessObject.CurrentFormIndex = _formGroup.CurrentFormIndex;
            this.BusinessObject.CleanSearchCriteria();

            foreach (DictionaryEntry entry in e.Values)
            {
                if (entry.Value is GenericBO)
                {
                    this.BusinessObject.SearchCriteria = ((GenericBO)entry.Value).SearchCriteria;
                    this.BusinessObject.OrderByCriteria = ((GenericBO)entry.Value).OrderByCriteria;
                }
            }
        }

        COEDataMapper.Map(e.Values, this.BusinessObject);
    }

    protected void CslaDataSource_SelectObject(object sender, SelectObjectArgs e)
    {
        try
        {
            FormGroup.Display display = _formGroup.CurrentCollection[_formGroup.CurrentFormIndex];
            BusinessObject.CurrentFormType = _formGroup.CurrentDisplayGroup;
            BusinessObject.CurrentFormIndex = _formGroup.CurrentFormIndex;

            switch (CurrentMode)
            {
                case FormGroup.CurrentFormEnum.QueryForm:
                    e.BusinessObject = BusinessObject;
                    break;
                case FormGroup.CurrentFormEnum.DetailForm:
                case FormGroup.CurrentFormEnum.ListForm:
                    e.BusinessObject = BusinessObject;

                    if (!this.RestoringHitList)
                    {
                        int recordCount = BusinessObject.TotalRecordCount;
                        recordCount = CoePagerControlBottom.RecordCount = recordCount;
                        CoePagerControl.RecordCount = recordCount;

                        DisplayRecordCount(CoePagerControl.RecordCount);
                        UpdateTimer();
                    }
                    break;
            }
            this.BuildActionLinkButtons();
            if (_masterPage.AccordionControl.Groups.FromKey("QueryManagement") != null)
            {
                ((ManagerPane)_masterPage.AccordionControl.Groups.FromKey("QueryManagement").UserControl).DataBind();
            }
        }
        catch (Exception exception)
        {
            if (exception.Message.Contains("ORA-29400"))
                DisplayRecordCount(0);
            else
                throw exception;
        }
    }

    #endregion

    protected void SaveButton_Click(object sender, RecPanelEventArgs args)
    {
        int currentpage = 0;
        if (CurrentMode == FormGroup.CurrentFormEnum.ListForm)
        {
            currentpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(args.RecNo.ToString()) / this.GridPageSize));
        }
        else if (CurrentMode == FormGroup.CurrentFormEnum.DetailForm)
        {
            currentpage = args.RecNo;
        }
        else
        {
            currentpage = 1;
        }
        if (currentpage > 0)
            this.CoePagerControl.CurrentPage = this.CoePagerControlBottom.CurrentPage = currentpage;
    }

    protected void ExportControl_ExportList(object sender, ExportControlEventArgs eventArgs)
    {

        string expformat = eventArgs.ExportFormat;
        switch (expformat)
        {
            case "SDF Nested":
                expformat = "ExportSDFNested";
                break;
            case "SDF Flat":
                expformat = "ExportSDFFlatFileUncorrelated";
                break;
            default:
                break;
        }

        ResultsCriteria exportRC = eventArgs.outputRes.Clone();
        ExportHits(expformat, exportRC);
        // CSBR- 165188: Counter fix for this bug due to fix made for 161910
        ExportControlDiv.Style.Add("display", "none");
        expcontrolvisibility.Value = "false";
    }

    public override string ToString()
    {
        // get the formgroupid 
        string _formGroupid = Request.QueryString["FormGroupId"];
        string _pagename = CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlIdChangeUtility.PAGENAME;
        string _suffix = null;
        if (!string.IsNullOrEmpty(_formGroupid))
        {
            // if the _formGroupid is for permanent search suffix _perm  to the pagename else suffix _temp
            if (_formGroupid.Equals(CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlIdChangeUtility.PERMSEARCHGROUPID))
                _suffix = CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlIdChangeUtility.PERMSUFFIX;
            else if (_formGroupid.Equals(CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlIdChangeUtility.TEMPSEARCHGROUPID))
                _suffix = CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlIdChangeUtility.TEMPSUFFIX;
        }
        return string.Concat(_pagename, _suffix);

    }



}

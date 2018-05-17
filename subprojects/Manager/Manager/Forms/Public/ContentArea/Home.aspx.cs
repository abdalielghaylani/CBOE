using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.WebParts;
using System.Xml;
using System.Data;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using Infragistics.WebUI.Misc;
using CambridgeSoft.COE.Framework.Controls.ChemDraw;

public partial class Forms_ContentArea_Home : GUIShellPage
{
    #region Variables
    private int rowCounter = 0;
    #endregion

    #region Events Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.SetHomeWebParts();
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
			//Added condition to check if return url is not null
			//return url will be opened in new window.
			if (Request.QueryString["ReturnURL"] != null && ! Request.QueryString["ReturnURL"].Contains("Home.aspx"))
            {
                string ReturnURL = Page.ResolveUrl(Request.QueryString["ReturnURL"].ToString());
                String WindowOpenScript = "window.open('" + ReturnURL + "','newwindow','location=1,status=1,scrollbars=1,toolbar=1,resizable=1,width=1024px,height=768px,top=100px,left=100px')";
               if (!this.Page.ClientScript.IsClientScriptBlockRegistered("WindowOpenScript"))
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "WindowOpenScript", WindowOpenScript, true);
            }
            //if we're in catalog mode, show our special catalog
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Session["isCDP"] == null)
        {
            if (Page.Request.Cookies.Get("isCDP") != null)
            {
                Session["isCDP"] = bool.Parse(Page.Request.Cookies.Get("isCDP").Value);
            }
            else
            {
                string url = "/cfserverasp/source/chemdraw.js";
                if (!Page.ClientScript.IsClientScriptIncludeRegistered("chemdraw"))
                {
                    Page.ClientScript.RegisterClientScriptInclude("chemdraw", url);
                }

                if (!Page.ClientScript.IsClientScriptBlockRegistered("cdpJavascriptDetection"))
                {
                    string cdpDetectionScript = @"

                        cd_setIsCDPPluginCookie();";

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Forms_ContentArea_Home), "cdpJavascriptDetection", cdpDetectionScript, true);
                }
            }
        }
        else
        {
            if (Page.ClientScript.IsClientScriptBlockRegistered("cdpJavascriptDetection"))
            {
                string cdpDetectionScript = @"";
                Page.ClientScript.RegisterClientScriptBlock(typeof(Forms_ContentArea_Home), "cdpJavascriptDetection", cdpDetectionScript, true);
            }
        }
    }
    #endregion

    #region Methods
    protected void DoLogOff(object sender, EventArgs e)
    {
        GUIShellUtilities.DoLogout();
    }

    private void SetHomeWebParts()
    {
            //COESpotFireSettingsBO SpotfireSetting = ConfigurationUtilities.GetSpotFireSettings(false);
            COEHomeSettings homeData = ConfigurationUtilities.GetHomeData();
            int numberOfColumns = Convert.ToInt16(homeData.GridColumns);
            WebPartManager webmgr = WebPartManager1;
            int wpUsed = 0;
            for (int i = 0; i < homeData.Groups.Count; i++)
            {                
                    Group myGroup = homeData.Groups.Get(i);

                    if (myGroup != null && myGroup.Name != "COEMANAGER_DV")
                    {
                        // Make sure the DataView Manager is not displayed for non-SpotfireUsers 
                        //if ((SpotfireSetting.SpotfireUser != "" ) || (SpotfireSetting.SpotfireUser == "" && myGroup.Name != "COEMANAGER_DV"))
                        //{
                            HomeWebPart webpartPreCheck = new HomeWebPart();
                            webpartPreCheck.Group = myGroup;
                            if (webpartPreCheck.HasLinks())
                            {
                                string webPartNumber = GetWebPartNumber(numberOfColumns, wpUsed);
                                //the webpart will figure out what links can be shown base on the users permissions
                                HomeWebPart webpart = (HomeWebPart)webmgr.WebParts["HomeWebPart" + webPartNumber];
                                webpart.Hidden = false;
                                webpart.Group = myGroup;
                                wpUsed = wpUsed + 1;

                            }
                        //}                        
                  }
            }        
    }

    private string GetWebPartNumber(int totalColumns, int counter)
    {
        counter = counter + 1; // increment by one since we started at 0
        int curRow = -1;
        int curColumn = -1;
        if (counter % totalColumns == 0)
        {
            curRow = rowCounter;
            rowCounter = rowCounter + 1;
            curColumn = totalColumns - 1;
        }
        else
        {
            curRow = rowCounter;
            curColumn = (counter % totalColumns) - 1;
        }
        return curColumn + "_" + curRow;
    }
    #endregion

    #region GUIShell Methods
    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.Home_Page_Title);
        this.Page.Title = Resources.Resource.COEManager_Page_Title;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
}

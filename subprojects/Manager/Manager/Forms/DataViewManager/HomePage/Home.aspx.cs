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

public partial class Forms_ContentArea_DVHome : GUIShellPage
{
    #region Variables
    private int rowCounter = 0;
    #endregion

    #region Events Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.SetHomeWebParts();
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

        if(Session["isCDP"] == null)
        {
            if(Page.Request.Cookies.Get("isCDP") != null)
            {
                Session["isCDP"] = bool.Parse(Page.Request.Cookies.Get("isCDP").Value);
            }
            else
            {
                string url = "/cfserverasp/source/chemdraw.js";
                if(!Page.ClientScript.IsClientScriptIncludeRegistered("chemdraw"))
                {
                    Page.ClientScript.RegisterClientScriptInclude("chemdraw", url);
                }

                if(!Page.ClientScript.IsClientScriptBlockRegistered("cdpJavascriptDetection"))
                {
                    string cdpDetectionScript = @"

                        cd_setIsCDPPluginCookie();";

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Forms_ContentArea_Home), "cdpJavascriptDetection", cdpDetectionScript, true);
                }
            }
        }
        else
        {
            if(Page.ClientScript.IsClientScriptBlockRegistered("cdpJavascriptDetection"))
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
        ApplicationHome homeData = ConfigurationUtilities.GetApplicationHomeData("MANAGER");
        int numberOfColumns = 1;
        WebPartManager webmgr = WebPartManager1;
        int wpUsed = 0;
        for(int i = 0; i < homeData.Groups.Count; i++)
        {
            Group myGroup = homeData.Groups.Get(i);

            if((myGroup != null) && (myGroup.COEIdentifier == "COEMANAGER_DV")) //isolate just to dvmanager panels
            {
                HomeWebPart webpartPreCheck = new HomeWebPart();
                webpartPreCheck.Group = myGroup;
                if(webpartPreCheck.HasLinks())
                {
                    string webPartNumber = string.Empty;
                    HomeWebPart webpart = null;
                    switch(myGroup.PageSectionTarget.ToUpper())
                    {
                        case "PANEL":
                            webPartNumber = GetWebPartNumber(numberOfColumns, wpUsed);
                            //the webpart will figure out what links can be shown base on the users permissions
                            webpart = (HomeWebPart) webmgr.WebParts["PanelWebPart" + webPartNumber];
                            webpart.Hidden = false;
                            webpart.Group = myGroup;
                            wpUsed = wpUsed + 1;
                            break;
                        case "DASHBOARD":
                            webPartNumber = GetWebPartNumber(numberOfColumns, wpUsed);
                            //the webpart will figure out what links can be shown base on the users permissions
                            webpart = (HomeWebPart) webmgr.WebParts["DashWebPart" + webPartNumber];
                            webpart.Hidden = false;
                            webpart.Group = myGroup;
                            wpUsed = wpUsed + 1;
                            break;
                    }
                }
            }
        }
    }

    private string GetWebPartNumber(int totalColumns, int counter)
    {
        counter = counter + 1; // increment by one since we started at 0
        int curRow = -1;
        int curColumn = -1;
        if(counter % totalColumns == 0)
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
        ;
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

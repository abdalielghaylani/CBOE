using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Reflection;
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

public partial class Forms_ContentArea_Home2 : GUIShellPage
{
    #region Variables
    private int rowCounter = 0;
    Forms_Master_MasterPage _masterPage = null;

    #endregion

    #region Events Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.SetHomeWebParts();
            //if we're in catalog mode, show our special catalog
           
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Page GUIShell Settings
        // To make easier to read the source code.
        _masterPage = (Forms_Master_MasterPage)this.Master;
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private void SetHomeWebParts()
    {
        

        COEHomeSettings homeData = ConfigurationUtilities.GetHomeData();
        int numberOfColumns = Convert.ToInt16(homeData.GridColumns);
        WebPartManager webmgr = WebPartManager1;
        for (int i = 0; i < homeData.Groups.Count; i++)
        {   
            Group myGroup = homeData.Groups.Get(i);
           
            if (myGroup != null)
            {
                string webPartNumber = GetWebPartNumber(numberOfColumns, i);
                //the webpart will figure out what links can be shown base on the users permissions
                HomeWebPart webpart = (HomeWebPart)webmgr.WebParts["HomeWebPart" + webPartNumber];
               
                webpart.Hidden = false;
                webpart.Group = myGroup;
                
            }


        }
    }

       
        private string GetWebPartNumber(int totalColumns, int counter){
            counter = counter + 1; // increment by one since we started at 0
            int curRow = -1;
            int curColumn = -1;
            if (counter % totalColumns == 0)
            {

                curRow = rowCounter;
                rowCounter = rowCounter + 1;
                curColumn = totalColumns - 1;
                    
                    
            }else{
                
               curRow = rowCounter;
               curColumn = (counter % totalColumns) - 1;
            }

            return curColumn + "_" + curRow; ;
        }
        
    

    #endregion

    #region GUIShell Methods

    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        _masterPage.SetPageTitle(Resources.Resource.Home_Page_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion


 




}

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

public partial class COEHitlistManagerTest : System.Web.UI.Page
{
    string AppName = "SAMPLE";

    protected void Page_Load(object sender, EventArgs e)
    {
        PopulateHitLists("TEMP");

    }
    protected void drpHitlistType_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateHitLists(drpHitlistType.Text);
        
    }
    protected void PopulateHitLists(string HitlistType)
    {
        COEHitListBOList HitListList = null;
        if (HitlistType == "TEMP")
        {
            HitListList = COEHitListBOList.GetTempHitListList(CambridgeSoft.COE.Framework.COEConfigurationService.COEConfiguration.GetDatabaseNameFromAppName(AppName));
        }
        else
        {
            HitListList = COEHitListBOList.GetSavedHitListList(CambridgeSoft.COE.Framework.COEConfigurationService.COEConfiguration.GetDatabaseNameFromAppName(AppName));
        }
        drpOwnersList.Items.Clear();
        for (int i = 0; i < HitListList.Count; i++)
              drpHitlistID.Items.Add(HitListList[i].ID.ToString());

    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string navigateurl = "COEHitlistManagerServerControl.aspx?CurrentHitListID=" + drpHitlistID.Text + "&CurrentHitListType=" + drpHitlistType.Text;
    //    Session["Selected_Item"] = "COEHitlistManagerServerControl";
        Response.Redirect(navigateurl);
    }
}

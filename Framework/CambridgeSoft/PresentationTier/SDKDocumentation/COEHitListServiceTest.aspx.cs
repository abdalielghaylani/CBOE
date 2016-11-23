using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using System.Configuration;

public partial class COEHitListServiceTest : System.Web.UI.Page
{

    enum eRecordToManage
    {
        Saved,
        Current
    }
    public enum eMode
    {
        Restore,
        Manage,
        Save
    }


    #region ControlProperties
    /// <summary>
    /// Currently Selected Main Tab .
    /// </summary>
    public int CurrentTab
    {
        get
        {
            if (ViewState["CurrentTab"] != null)
            {

                return (int)ViewState["CurrentTab"];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ViewState["CurrentTab"] = value;
        }
    }
    /// <summary>
    /// Maintain the selected Item from the Webcombo inside Manage Tab
    /// </summary>
    public int SelectedHitlistManage
    {
        get
        {
            if (ViewState["SelectedHitlistManage"] != null)
            {
                return (int)ViewState["SelectedHitlistManage"];
            }
            else
            {
                return -1;
            }
        }
        set
        {
            ViewState["SelectedHitlistManage"] = value;
        }
    }
    /// <summary>
    /// Application Name that has been selected.
    /// </summary>
    public string AppName
    {
        get
        {
            //the appName should come from the ini file or set by the calling applicaton in csla globalcontext["AppName"]
            //string appName = COEAppName.Get();
            //if (ViewState["AppName"] != null)
            //    appName = (string)ViewState["AppName"];
            //return appName;
            return "SAMPLE";
        }
        set
        {
            ViewState["AppName"] = value;
        }
    }
    /// <summary>
    /// CurrentHitlistID, that is currently control is working on.
    /// </summary>
    public int CurrentHitListID
    {
        get
        {
            if (ViewState["CurrentHitListID"] != null)
            {
                if (((int)ViewState["CurrentHitListID"]) != 0)
                    return (int)ViewState["CurrentHitListID"];
            }
            if ((!DesignMode) && AppName != string.Empty)
            {
                COEHitListBO hitListBO = COEHitListBO.New(COEConfiguration.GetDatabaseNameFromAppName(AppName), HitListType.TEMP);
                hitListBO.Save();
                CurrentHitListType = HitListType.TEMP;
                ViewState["CurrentHitListID"] = hitListBO.ID;
                return hitListBO.ID;
            }
            return -1;
        }
        set
        {
            if (ViewState["CurrentHitListID"] != null)
            {
                if (((int)ViewState["CurrentHitListID"]) != (int)value)
                {
                    if (((int)ViewState["CurrentHitListID"]) != 0)
                        LastHitListID = (int)ViewState["CurrentHitListID"];
                }
            }
            ViewState["CurrentHitListID"] = value;
        }
    }
    /// <summary>
    /// CurrentHitlistID, that is currently control is working on.
    /// </summary>
    public HitListType CurrentHitListType
    {
        get
        {
            if (ViewState["CurrentHitListType"] != null)
            {
                return (HitListType)ViewState["CurrentHitListType"];
            }
            else
            {
                return HitListType.TEMP;
            }
        }
        set
        {
            if (ViewState["CurrentHitListType"] != null)
                LastHitlistType = (HitListType)ViewState["CurrentHitListType"];
            ViewState["CurrentHitListType"] = value;
        }
    }
    /// <summary>
    /// ResolvedHitListID (Resolved Hitlist After operations), that is currently control is working on.
    /// </summary>
    public int ResolvedHitListID
    {
        get
        {
            if (ViewState["ResolvedHitListID"] != null)
            {
                return (int)ViewState["ResolvedHitListID"];
            }
            return -1;
        }
        set
        {
            ViewState["ResolvedHitListID"] = value;
        }
    }
    /// <summary>
    /// ResolvedHitlistType (HitlistType of the Resolved Hitlist), that is currently control is working on.
    /// </summary>
    public HitListType ResolvedHitListType
    {
        get
        {
            if (ViewState["ResolvedHitListType"] != null)
            {
                return (HitListType)ViewState["ResolvedHitListType"];
            }
            else
            {
                return HitListType.TEMP;
            }
        }
        set
        {
            ViewState["ResolvedHitListType"] = value;
        }
    }
    /// <summary>
    /// LastHitListID (Hitlist that has been used before the current one), that is currently control is having.
    /// </summary>
    public int LastHitListID
    {
        get
        {
            if (ViewState["LastHitListID"] != null)
            {
                return (int)ViewState["LastHitListID"];
            }
            return -1;
        }
        set
        {
            ViewState["LastHitListID"] = value;
        }
    }
    /// <summary>
    /// LastHitlistType (HitlistType of the Last Hitlist), that is currently control is having.
    /// </summary>
    public HitListType LastHitlistType
    {
        get
        {
            if (ViewState["LastHitlistType"] != null)
            {
                return (HitListType)ViewState["LastHitlistType"];
            }
            else
            {
                return HitListType.TEMP;
            }
        }
        set
        {
            ViewState["LastHitlistType"] = value;
        }
    }
    /// <summary>
    /// Manage tab can handle 2 type of records Either Saved (Can be selected from DropDown),
    /// or Current Record (select the CheckBox). This property keep track of that Record.
    /// </summary>
    private eRecordToManage RecordToManage
    {
        get
        {
            if (ViewState["RecordToManage"] != null)
                return (eRecordToManage)ViewState["RecordToManage"];
            else
                return eRecordToManage.Current;
        }
        set
        {
            ViewState["RecordToManage"] = value;
        }
    }

    #endregion



    protected void Page_Load(object sender, EventArgs e)
    {
        //ViewState["CurrentHitListID"] = "123";
        //ViewState["CurrentHitListType"] = "SAVED";
        if (!IsPostBack)
        {
            PopulateCombos();
        }
        else
        {
            if (SelectedHitlistManage != -1 && WebCombo1.Rows.Count > 0)
                WebCombo1.SelectedIndex = SelectedHitlistManage;
            else
                WebCombo1.SelectedIndex = 0;
        }
    }

    protected void PopulateCombos()
    {
        COEHitListBOList savedHitList = COEHitListBOList.GetSavedHitListList(CambridgeSoft.COE.Framework.COEConfigurationService.COEConfiguration.GetDatabaseNameFromAppName(AppName));

        WebCombo1.DataSource = savedHitList;
        WebCombo2.DataSource = savedHitList;
        WebCombo3.DataSource = savedHitList;

        WebCombo1.DataValueField = "ID";
        WebCombo2.DataValueField = "ID";
        WebCombo3.DataValueField = "ID";

        WebCombo1.DataTextField = "Name";
        WebCombo2.DataTextField = "Name";
        WebCombo3.DataTextField = "Name";

        WebCombo1.DataBind();
        WebCombo2.DataBind();
        WebCombo3.DataBind();

    }
    protected void btnPerformOperation_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        if (WebCombo1.SelectedRow != null && chkManageCurrentRecord.Checked == false)
            return;

        COEHitListBO obj = null;

        HitListInfo infoObj1 = new HitListInfo();
        HitListInfo infoObj2 = new HitListInfo();

        infoObj1.Database = infoObj2.Database = COEConfiguration.GetDatabaseNameFromAppName(AppName);

        HitListType infoObj1Type;

        if (RecordToManage == eRecordToManage.Saved)
        {

            infoObj1.HitListID = Convert.ToInt32(WebCombo1.SelectedRow.Cells.FromKey("ID").Value.ToString());
            infoObj1Type = HitListType.SAVED;
        }
        else
        {
            infoObj1.HitListID = CurrentHitListID;
            infoObj1Type = CurrentHitListType;
        }
        infoObj2.HitListID = Convert.ToInt32(WebCombo2.SelectedRow.Cells.FromKey("ID").Value.ToString());
        switch (rdOperations.SelectedIndex)
        {
            case 0:
                obj = COEHitListBO.NewTempFromIntersection(infoObj2, HitListType.SAVED, infoObj1, infoObj1Type);
                break;
            case 1:
                obj = COEHitListBO.NewTempFromSubtraction(infoObj2, HitListType.SAVED, infoObj1, infoObj1Type);
                break;
            case 2:
                obj = COEHitListBO.NewTempFromUnion(infoObj2, HitListType.SAVED, infoObj1, infoObj1Type);
                break;
        }
        obj.Save();
        ResolvedHitListID = obj.ID;
        ResolvedHitListType = obj.HitListType;


    }
    protected void btnEdit_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        if (WebCombo1.SelectedRow == null && chkManageCurrentRecord.Checked == false) 
            return;
        
            int hitListID;
            HitListType hitListType;
            if (RecordToManage == eRecordToManage.Saved)
            {
                hitListID = Convert.ToInt32(WebCombo1.SelectedRow.Cells.FromKey("ID").Value.ToString());
                hitListType = HitListType.SAVED;
            }
            else
            {
                hitListID = CurrentHitListID;
                hitListType = CurrentHitListType;
            }

            COEHitListBO hitListObj = COEHitListBO.Get(COEConfiguration.GetDatabaseNameFromAppName(AppName), hitListType, hitListID);

            hitListObj.Name = txtEditListName.Text;
            hitListObj.Description = txtEditDescription.Text;
            hitListObj.IsPublic = (Boolean)chkEditIsPublic.Checked;
            hitListObj.Save();
        }
        //Refresh the Control.

    protected void btnDelete_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {

        int hitListID;
        HitListType hitListType;
        if (RecordToManage == eRecordToManage.Saved)
        {
            hitListID = Convert.ToInt32(WebCombo1.SelectedRow.Cells.FromKey("ID").Value.ToString());
            hitListType = HitListType.SAVED;
        }
        else
        {
            hitListID = CurrentHitListID;
            hitListType = CurrentHitListType;
        }

        COEHitListBO.Delete(COEConfiguration.GetDatabaseNameFromAppName(AppName), hitListType, hitListID);
        PopulateCombos();
    }
    protected void btnRestore_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        int hitlistID;
        if (rdSaved.Checked)
        {
            hitlistID = Convert.ToInt32(WebCombo3.SelectedRow.Cells.FromKey("ID").Value.ToString());
            CurrentHitListID = hitlistID;
            CurrentHitListType = HitListType.SAVED;
        }
        else
        {
            if (LastHitListID != -1)
            {
                CurrentHitListID = LastHitListID;
                CurrentHitListType = LastHitlistType;
            }
        }
        PopulateSaveTabFields(CurrentHitListID,CurrentHitListType);
    }
    protected void btnSave_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        if (CurrentHitListType != HitListType.SAVED)
        {
            COEHitListBO objSavedHitlist = COEHitListBO.NewSavedFromTempHitList(CurrentHitListID);
            CurrentHitListID = objSavedHitlist.ID;
            CurrentHitListType = objSavedHitlist.HitListType;
            objSavedHitlist.Name = txtSaveDesc.Text;
            objSavedHitlist.Description = txtSaveListName.Text; ;
            objSavedHitlist.IsPublic = chkSaveIsPublic.Checked;
            objSavedHitlist.Save();
        }
    }
    protected void WebCombo1_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {
        if (!chkManageCurrentRecord.Checked)
        {
           //SelectedHitlistManage = e.Row.Index;
            //RecordToManage = eRecordToManage.Saved;

            //int hitListID = Convert.ToInt32(WebCombo1.SelectedRow.Cells.FromKey("ID").Value.ToString());
            //HitListType hitListType = HitListType.SAVED;

            PopulateManageTabFields();
            //COEHitListBO hitListObj = COEHitListBO.Get(COEConfiguration.GetDatabaseNameFromAppName(AppName), hitListType, hitListID);
            //COEHitListBO objSavedHitlist = COEHitListBO.NewSavedFromTempHitList(CurrentHitListID);
        }

    }

    protected void PopulateManageTabFields()
    {
        int hitListID;
        HitListType hitListType;

        if (chkManageCurrentRecord.Checked)
        {
            RecordToManage = eRecordToManage.Current;
            hitListID = CurrentHitListID;
            hitListType = CurrentHitListType;
        }
        else
        {
            SelectedHitlistManage = WebCombo1.SelectedRow.Index;
            RecordToManage = eRecordToManage.Saved;

            hitListID = Convert.ToInt32(WebCombo1.SelectedRow.Cells.FromKey("ID").Value.ToString());
            hitListType = HitListType.SAVED;


        }

        COEHitListBO hitListObj = COEHitListBO.Get(COEConfiguration.GetDatabaseNameFromAppName(AppName), hitListType, hitListID);
        txtEditDescription.Text = hitListObj.Description;
        txtEditListName.Text = hitListObj.Name;
        chkEditIsPublic.Checked = (Boolean)hitListObj.IsPublic;

        txtDeleteDesc.Text = hitListObj.Description;
        txtDeleteListName.Text = hitListObj.Name;
        chkDeleteIsPublic.Checked = (Boolean)hitListObj.IsPublic;

    }

    protected void PopulateSaveTabFields(int hitListID, HitListType hitListType)
    {
        COEHitListBO hitListObj = COEHitListBO.Get(COEConfiguration.GetDatabaseNameFromAppName(AppName), hitListType, hitListID);
        txtSaveDesc.Text = hitListObj.Description;
        txtSaveListName.Text = hitListObj.Name;
        chkSaveIsPublic.Checked = (Boolean)hitListObj.IsPublic;

    
    }


    protected void chkManageCurrentRecord_CheckedChanged(object sender, EventArgs e)
    {
        RecordToManage = eRecordToManage.Current;
        PopulateManageTabFields();
        if (chkManageCurrentRecord.Checked)
        {
            WebCombo1.Enabled = false;
        }
        else
        {
            WebCombo1.Enabled = true;
        }

    }
}
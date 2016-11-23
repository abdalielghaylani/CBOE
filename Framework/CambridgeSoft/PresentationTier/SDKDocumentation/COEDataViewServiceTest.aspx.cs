using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEConfigurationService;

public partial class DataViewService : System.Web.UI.Page
{
    private ModeOfAction CurrentMode = ModeOfAction.Update;
    private string DatabaseName;    

    public enum ModeOfAction
    {
        Add, Update, Delete
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            List<string> listApps = COEConfiguration.GetDatabaseByDatabaseType(CambridgeSoft.COE.Framework.Common.DBMSType.ORACLE);
            listApps.Add("All");
            for (int i = 0; i < listApps.Count; i++)
            {
                drpAppList.Items.Add(listApps[i]);

            }
        }
        DatabaseName = drpAppList.Text;

    }

    protected void clearFields()
    {
        this.txtCOEDataView.Text = "";
        this.txtDesc.Text = "";
        this.txtFormGroup.Text = "";
        this.txtName.Text = "";
        this.chkIsPublic.Checked = false;
        this.dtDateCreated = null;

    }

    protected void EnableFields()
    {
        txtFormGroup.ReadOnly = false;
        txtUserName.ReadOnly = false;
        dtDateCreated.ReadOnly = false;

    }

    protected void DisableFields()
    {
        txtFormGroup.ReadOnly = true;
        txtUserName.ReadOnly = true;
        dtDateCreated.ReadOnly = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        switch (UltraWebTab1.SelectedTab)
        {
            case 0:
                COEDataViewBO coeDataViewObject = COEDataViewBO.New(DatabaseName);
                PerformUpdate(coeDataViewObject);
                EnableFields();
                clearFields();
                PopulateRecords();
                DisableFields();
                break;

            case 1:
                CurrentMode = ModeOfAction.Update;
                CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO coeDataViwBO = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO.Get(DatabaseName, Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Value));
                PerformUpdate(coeDataViwBO);
                break;

            case 2:
                CurrentMode = ModeOfAction.Delete;
                CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO.Delete(DatabaseName,Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Text));
                PopulateRecords();
                break;
        }

    }

    protected COEDataView BuildCOEDataViewFromXML(string xmlstring)
    {
        //Load DataViewSerialized.XML
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlstring);
        COEDataView coeDataView = new COEDataView();
        coeDataView.GetFromXML(doc);
        return coeDataView;
    }

    protected void PerformUpdate(COEDataViewBO coeDataViewObject)
    {
        coeDataViewObject.Name = txtName.Text;
        coeDataViewObject.FormGroup = Convert.ToInt32(txtFormGroup.Text);
        coeDataViewObject.Description = txtDesc.Text;

        coeDataViewObject.IsPublic = (bool)chkIsPublic.Checked;
        //this really should come from the logged in user..
        coeDataViewObject.UserName = txtUserName.Text;
        coeDataViewObject.COEDataView = this.BuildCOEDataViewFromXML(txtCOEDataView.Text);
     
        coeDataViewObject.Save();

    }

    protected void UltraWebTab1_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {
        switch (UltraWebTab1.SelectedTab)
        {
            case 0:
                CurrentMode = ModeOfAction.Add;
                btnSave.Text = "Add";
                clearFields();
                break;

            case 1:
                CurrentMode = ModeOfAction.Update;
                btnSave.Text = "Save";
                break;

            case 2:
                CurrentMode = ModeOfAction.Delete;
                btnSave.Text = "Delete";
                break;                
        }


    }

    protected void PopulateRecords()
    {
        try
        {

            COEDataViewBOList lstDataView = null;
            if (!drpAppList.SelectedValue.ToString().ToUpper().Equals("ALL"))
            {
                Csla.ApplicationContext.GlobalContext["DatabaseName"] = drpAppList.SelectedValue.ToString();
                // ConfigurationManager.AppSettings.Set("AppName", comboBox1.SelectedValue.ToString());
                DatabaseName = drpAppList.Text;//COEConfiguration.GetDatabaseNameFromAppName(comboBox1.SelectedValue.ToString());

                lstDataView = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewDataList(DatabaseName);

            }
            else
            {
                lstDataView = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBOList.GetDataViewListforAllDatabases();
            }

            drpSelectRecord.DataSource = lstDataView;
            drpSelectRecord.DataValueField = "ID";
            drpSelectRecord.DataTextField = "Name";
            drpSelectRecord.DataBind();


        }
        catch (Exception ex)
        {

        }

    }
    protected void drpAppList_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateRecords();
    }
    protected void FillData()
    {
        try
        {
            clearFields();
            CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO coeDataViwBO = CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO.Get(DatabaseName, Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Value));
            txtFormGroup.Text = coeDataViwBO.FormGroup.ToString();
            txtName.Text = coeDataViwBO.Name;
            txtUserName.Text = coeDataViwBO.UserName.ToString();
            txtDesc.Text = coeDataViwBO.Description.ToString();
            txtCOEDataView.Text = coeDataViwBO.COEDataView.ToString();
            chkIsPublic.Checked = coeDataViwBO.IsPublic;
            dtDateCreated.Value = coeDataViwBO.DateCreated;
        }
        catch (Exception ex)
        { 
        }
    }
    protected void drpSelectRecord_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {

        UltraWebTab1.SelectedTab = 1;
        CurrentMode = ModeOfAction.Update;
        btnSave.Text = "Save";
        FillData();

    }
}
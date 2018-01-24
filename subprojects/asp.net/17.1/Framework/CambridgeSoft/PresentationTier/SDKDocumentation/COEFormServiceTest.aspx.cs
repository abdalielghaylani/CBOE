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
using CambridgeSoft.COE.Framework.COEFormService;
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
        this.txtCOEForm.Text = "";
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
                EnableFields();
                COEFormBO COEFormObject = COEFormBO.New(DatabaseName);
                PerformUpdate(COEFormObject);
                clearFields();
                PopulateRecords();
                DisableFields();
                break;

            case 1:
                CurrentMode = ModeOfAction.Update;
                CambridgeSoft.COE.Framework.COEFormService.COEFormBO coeFormBO = CambridgeSoft.COE.Framework.COEFormService.COEFormBO.Get(DatabaseName, Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Value));
                PerformUpdate(coeFormBO);

                break;

            case 2:
                CurrentMode = ModeOfAction.Delete;
                CambridgeSoft.COE.Framework.COEFormService.COEFormBO.Delete(DatabaseName,Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Text));
                PopulateRecords();
                break;
        }

    }

    //protected COEForm BuildCOEFormFromXML(string xmlstring)
    //{
    //    //Load DataViewSerialized.XML
    //    XmlDocument doc = new XmlDocument();
    //    doc.LoadXml(xmlstring);
    //    COEForm COEForm = new COEForm();
    //    //COEForm.GetFromXML(doc);
    //    //return COEForm;
    //}

    protected void PerformUpdate(COEFormBO COEFormObject)
    {
        COEFormObject.Name = txtName.Text;
        COEFormObject.FormGroup = Convert.ToInt32(txtFormGroup.Text);
        COEFormObject.Description = txtDesc.Text;

        COEFormObject.IsPublic = (bool)chkIsPublic.Checked;
        //this really should come from the logged in user..
        COEFormObject.UserName = txtUserName.Text;
        COEFormObject.COEForm = COEForm.CreateCOEForm(txtCOEForm.Text);
     
        COEFormObject.Save();

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

            COEFormBOList lstDataView = null;
            if (!drpAppList.SelectedValue.ToString().ToUpper().Equals("ALL"))
            {
                Csla.ApplicationContext.GlobalContext["DatabaseName"] = drpAppList.SelectedValue.ToString();
                // ConfigurationManager.AppSettings.Set("AppName", comboBox1.SelectedValue.ToString());
                DatabaseName = drpAppList.Text;//COEConfiguration.GetDatabaseNameFromAppName(comboBox1.SelectedValue.ToString());
                lstDataView = CambridgeSoft.COE.Framework.COEFormService.COEFormBOList.GetCOEFormBOList(DatabaseName);

            }
            else
            {
                lstDataView = CambridgeSoft.COE.Framework.COEFormService.COEFormBOList.GetCOEFormBOListbyAllDatabases();
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
            CambridgeSoft.COE.Framework.COEFormService.COEFormBO coeFormBO = CambridgeSoft.COE.Framework.COEFormService.COEFormBO.Get(DatabaseName, Convert.ToInt32(drpSelectRecord.SelectedRow.Cells.FromKey("ID").Value));
            txtFormGroup.Text = coeFormBO.FormGroup.ToString();
            txtName.Text = coeFormBO.Name;
            txtUserName.Text = coeFormBO.UserName.ToString();
            txtDesc.Text = coeFormBO.Description.ToString();
            chkIsPublic.Checked = coeFormBO.IsPublic;
            dtDateCreated.Value = coeFormBO.DateCreated;
            txtCOEForm.Text = coeFormBO.COEForm.ToString();
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
using System;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using System.Collections.Generic;

public partial class COEDatabasePublishingServiceTest : System.Web.UI.Page
{
    CambridgeSoft.COE.Framework.COEDatabasePublishingService.DatabasePublishingManager PublishManager;
    string SelectedOwner;
    string dataBase;
    private DataTable userList = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        lblError.Text = "";
        bool Isauthenticated = false;

        try
        {
            if (Csla.ApplicationContext.AuthenticationType == "Windows")
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
                Isauthenticated = true;
            }
            else
            {
                //in this case you are logging in with the appName will get and set all the privilges
                //the a user has. these privileges are gathtered from cs_security and is oracle only at this time
                //this inforamtion could come from a login screen
                //           Isauthenticated = DoLogin();
            }
            //       if (Isauthenticated == false)
            //       {
            ////           DisableApplication();
            //       }
            //       else
            if (!IsPostBack)
                LoadOwners();
        }
        catch (Exception ex)
        {
        //    DisableApplication();
        }
        
    }

    private void LoadOwners()
    {
        List<string> myAppNameList = null;

        //This access is for the Command objects
        //PublishManager = new COEDatabasePublishing();
        PublishManager = new DatabasePublishingManager();
        myAppNameList = PublishManager.GetDatabase();

        DataTable OwnerTable = new DataTable();
        DataColumn OwnersColumn = new DataColumn("Database");
        OwnerTable.Columns.Add(OwnersColumn);

        for (int i = 0; i < myAppNameList.Count; i++)
        {
            DataRow row = OwnerTable.NewRow();
            row[0] = myAppNameList[i].ToString();
            OwnerTable.Rows.Add(row);
        }
        
        drpOwners.DataSource = OwnerTable;
        drpOwners.DataTextField = "Database";
        drpOwners.DataValueField = "Database";
        drpOwners.DataBind();
    }

    protected void drpOwners_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {
        SelectedOwner = drpOwners.SelectedCell.Text;
        usersListBox.Items.Clear();
        PublishManager = new DatabasePublishingManager();
        dataBase = this.drpOwners.SelectedCell.Text;
        userList = PublishManager.GetUserList(dataBase);
        if (userList != null)
        {
            for (int i = 0; i < userList.Rows.Count; i++)
            {
                usersListBox.Items.Add(userList.Rows[i][0].ToString());
            }
        }
        txtPassword.Text = "";
    }
    protected void btnPublishDatabase_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        if (drpOwners.SelectedCell != null)
        {
            string password = this.txtPassword.Text;
            string writtenXML = "";
            password = txtPassword.Text;

            dataBase = this.drpOwners.SelectedCell.Text;
            PublishManager = new DatabasePublishingManager();
            // Fields of Dataview table in Database
            //PublishManager.Username =  UserName.ToString();
            //PublishManager.Name = Name.ToString();
            //PublishManager.Description = Description.ToString();
            //PublishManager.Ispublic = IsPublic;

            List<string> userGrantList = new List<string>();

            for (int i = 0; i < this.usersListBox.Items.Count; i++)
            {
                if (usersListBox.Items[i].Selected == true)
                    userGrantList.Add(usersListBox.Items[i].ToString());
            }

            writtenXML = PublishManager.PublishDatabase(dataBase, password, userGrantList);

            if (writtenXML == null)
            {
                lblError.Text = "The Password you entered is incorrect. Please re-type the password ";
                txtPassword.Text = "";
            }
            else
            {
                XmlDocument xmldoc = new XmlDocument();
                string path = System.Web.HttpContext.Current.Server.MapPath("COESchemaView.xml");
                xmldoc.LoadXml(writtenXML);
                System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
                xmldoc.Save(path);


                this.UltraWebTab1.Tabs.GetTab(0).Text = " " + dataBase + " ";
                this.UltraWebTab1.Tabs.GetTab(0).ContentPane.TargetUrl = @"COESchemaView.xml";
                this.UltraWebTab1.Visible = true;
                WebPanel1.Visible = false;
                tblDataView.Visible = true;
                lnkbtnBack.Visible = true;
            }
        }
    }
    protected void lnkbtnBack_Click(object sender, EventArgs e)
    {
        WebPanel1.Visible = true;
        tblPublishDatabase.Visible = true;
        tblDataView.Visible = false;
        UltraWebTab1.Visible = false;
        lnkbtnBack.Visible = false;
    }
}

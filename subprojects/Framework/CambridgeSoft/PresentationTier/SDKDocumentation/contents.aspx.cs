using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class contents : System.Web.UI.Page
{
    string path;
    string appPath ;
    string csfilepath; 
    string aspxfilepath;
    string selectedNodeTag;

    protected void Page_Init(object sender, EventArgs e)
    {
           path = Server.MapPath(Request.ApplicationPath);
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        path = Server.MapPath(Request.ApplicationPath);
       
        selectedNodeTag = (string)Session["Selected_Item"];
    //    selectedNodeTag = "DatabasePublishingServiceTest";

        if (selectedNodeTag != null && selectedNodeTag != "")
        {
            appPath = selectedNodeTag + ".aspx";
            csfilepath = path + @"\" + selectedNodeTag + ".aspx.cs";
            aspxfilepath = path + "\\CodeFiles\\" + selectedNodeTag + ".txt";
      //      aspxfilepath = path + "\\" + selectedNodeTag + ".aspx";
            
            this.UltraWebTab1.Tabs.GetTab(0).ContentPane.TargetUrl = appPath;
            this.readFile(aspxfilepath);
           // this.readFile(appPath);


        }


    }
    public void readFile(string s)
    {
        using (StreamReader sr = new StreamReader(s))
        {
            String line;
            String cscode = "";
            // Read and display lines from the file until the end of 
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                cscode = cscode + "\n" +  line;
            }
            txtCode.Text = cscode;
        }
    }


    protected void lnkaspxcode_Click(object sender, EventArgs e)
    {
    //    string targeturl = path + @"/CodeFiles/DatabasePublishingServiceTest.txt";

        this.readFile(aspxfilepath);

    }
    protected void lnkcscode_Click(object sender, EventArgs e)
    {
        this.readFile(csfilepath);
    }
}
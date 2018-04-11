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
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.Controls.COEWebGrid;
using CambridgeSoft.COE.Framework.Common;
public partial class wg2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       // Response.Write("ViewState['Pattern']=" + ViewState["Pattern"] + "<br>");

   
    }

    protected void InitializeDataSource()
    {
        DataSet dataSet = new DataSet();

        System.Data.OracleClient.OracleConnection orConn = Openconnection();

        System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
        cmd.Connection = orConn;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"Select * from MOLTABLE where id >= 28 and id <= 53";//super Parent

        System.Data.OracleClient.OracleDataAdapter da = new System.Data.OracleClient.OracleDataAdapter(cmd);
        da.Fill(dataSet);

        DataView dv = dataSet.Tables[0].DefaultView;

        dataSet.Tables.Clear();
        DataTable dt = dv.ToTable();

        System.Data.OracleClient.OracleCommand cmd2 = new System.Data.OracleClient.OracleCommand();
        cmd2.Connection = orConn;
        cmd2.CommandType = CommandType.Text;
        cmd2.CommandText = "Select * from GRAPHICS";//Parent

        System.Data.OracleClient.OracleDataAdapter da1 = new System.Data.OracleClient.OracleDataAdapter(cmd2);
        da1.Fill(dataSet);

        dv = dataSet.Tables[0].DefaultView;
        DataTable dt1 = dv.ToTable();

        dataSet.Tables.RemoveAt(0);

        dt.TableName = "TABLE1";
        dt1.TableName = "Table2";


        dataSet.Tables.Add(dt);
        dataSet.Tables.Add(dt1);

        dataSet.Relations.Add("TestRelation1", dataSet.Tables["Table1"].Columns["ID"], dataSet.Tables["Table2"].Columns["ID"]);

        List<string> lst = new List<string>();
        lst.Add("BASE64_CDX");
        COEWebGrid1.SetCDXFieldList(lst);
        COEWebGrid1.DataSource = dataSet;
        COEWebGrid1.DataBind();
  
    }


    private System.Data.OracleClient.OracleConnection Openconnection()
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        System.Data.OracleClient.OracleConnection dbConnection = new System.Data.OracleClient.OracleConnection(connString.ToString());
        try
        {
            dbConnection.Open();
            return dbConnection;
        }
        catch (System.Data.OracleClient.OracleException ex)
        {
            throw new ApplicationException("Failed to connect to Oracle Database! Error: " + ex.Message.ToString(), ex);

        }
    }



    protected void Button1_Click(object sender, EventArgs e)
    {
        COEWebGrid1.BindExternally = true;
        InitializeDataSource();

    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        COEWebGrid1.BindExternally = false;
        COEWebGrid1.DataBind();

    }
}

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


public partial class Export : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
        //ConfirmationMessage.Text = string.Format(Resources.Resource.SaveExportedRecord_Title_Text, Session["exportedCount"].ToString(), Session["exportedDataFormat"].ToString());

        bool blnIsAdvanceExport = false;
        if (Session["ExportType"] != null)
        {
            if (Convert.ToString(Session["ExportType"]).Trim().Equals("AdvanceExport"))
                blnIsAdvanceExport = true;
        }

        if (blnIsAdvanceExport)
        {
            ConfirmationMessage.Text = "Are you sure you want to export the search result?";
        }
        else
        {
            ConfirmationMessage.Text = string.Format(Resources.Resource.SaveExportedRecord_Title_Text, Session["exportedCount"].ToString(), Session["exportedDataFormat"].ToString());
        }
	}

    public override string StyleSheetTheme
	{
		get
		{
			return "";
		}
		set
		{
			base.StyleSheetTheme = value;
		}
	}
}


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
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search
{
    public partial class ContentTransfer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ClearContent();
            Response.Charset = "";
            Response.ContentType = Session["exportedDataMIME"].ToString();
            Response.AppendHeader("content-disposition", string.Format("attachment; filename=Exported{0}HitsFromDV{1}.{2}", 
                                                                        Session["exportedCount"].ToString(), 
                                                                        ((GenericBO)Session["BasePageBusinessObject"]).DataViewId.ToString(), 
                                                                        Session["exportedDataFormat"].ToString()));
            string exportedData = Session["exportedData"].ToString();
            Response.Write(exportedData);
            Response.End();
        }
    }
}

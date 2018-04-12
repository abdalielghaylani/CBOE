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
using System.IO;
using CambridgeSoft.COE.DocumentManager.Services.Types ;

public partial class BinaryWriter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Validate Parameters
            int docId;
            if (Request["docId"] != null && int.TryParse(Request["docId"].ToString(), out docId))
            {
                // Get document Information 
                Document currDoc = Document.GetDocumentByID(int.Parse(Request["docId"].ToString()));
                string name = currDoc.Name;
                string mimeType = GetMimeFromFileExtension(Path.GetExtension(name.Trim().ToLower()));

                // Render Document
                Response.ContentType = mimeType;
                string disposition = "inline";
                if (Request["disposition"] != null && Request["disposition"].ToString() == "1")
                {
                    disposition = "attachment";
                }
                Response.AppendHeader("Content-disposition", disposition+";filename=" + name);
                Response.BinaryWrite((byte[])currDoc.BinaryContent);
                Response.Flush();
                //Response.Close();
            }
        }
    }

    private string GetMimeFromFileExtension(string extension)
    {
        switch(extension)
        {
            case ".docx":
            case ".doc":
                return "application/msword";
            case ".xlsx":
            case ".xls":
                return "application/vnd.ms-excel";
            case ".ppt":
            case ".pptx":
            case ".pps":
            case ".ppsx":
                return "application/vnd.ms-powerpoint";
            case ".pdf":
                return "application/pdf";
            case ".ogg":
                return "application/ogg";
            case ".zip":
                return "application/zip";
            case ".odt":
                return "application/vnd.oasis.opendocument.text";
            case ".odp":
                return "application/vnd.oasis.opendocument.presentation";
            case ".ods":
                return "application/vnd.oasis.opendocument.spreadsheet";
            case ".odg":
                return "application/vnd.oasis.opendocument.graphics";
            case ".css":
                return "text/css";
            case ".csv":
                return "text/csv";
            case ".txt":
                return "text/plain";
            case ".xml":
                return "text/xml";
            case ".gif":
                return "image/gif";
            case ".jpg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".tif":
            case ".tiff":
                return "image/tiff";
            case ".svg":
            case ".svgz":
                return "image/svg+xml";
            case ".ico":
                return "image/vnd.microsoft.icon";
            default:
                return string.Empty;
        }
    }
}

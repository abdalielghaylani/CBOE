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
//
using Util;
using System.IO;
using System.Text;
using System.Xml;
using System.Net;
//
public partial class LoginRequestHandler : System.Web.UI.Page
{
    /// <summary>
    /// 
    /// </summary>
    private XMLUtil xmlUtil = new XMLUtil();
   // TextWriter tw; 

    #region XML read/write
    /// <summary>
    /// 
    /// </summary>
    /// <param name="XMLResponse"></param>
    /// <returns></returns>
    private System.Collections.Specialized.NameValueCollection ReadFromRequest_User_Pass_ReturnURL_(String XMLRequest)
    {
        
        string tempXMLRequest = XMLRequest.Replace("&", "&amp;").Replace("&amp;amp;","&amp;");

        System.Collections.Specialized.NameValueCollection values = new System.Collections.Specialized.NameValueCollection();
        StringBuilder sb = new StringBuilder();

        StringReader stream = new StringReader(tempXMLRequest);
        XmlTextReader reader = new XmlTextReader(stream); 
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "userid")
                { values.Add("UserId", reader.ReadString()); }
                if (reader.Name == "password")
                { values.Add("Password", reader.ReadString()); }
               if (reader.Name == "returnURL")
                { values.Add("ReturnURL", reader.ReadString()); }
                if (reader.Name == "appUserName")
                { values.Add("appUserName", reader.ReadString()); }
            }
        }
        //
        reader.Close();
        //
        return values ;
    }


    private void ResponseToRequest(bool success,string returnURL,string targetURL)
    {
        byte[] buf = new byte[8192];
        HttpWebRequest req = null;
        HttpWebResponse rsp = null;
        StringBuilder sb = new StringBuilder();

        try
        {
         
            string fileName = success ? System.Configuration.ConfigurationManager.AppSettings["xmlDir"].ToString() + "LoginResponseSucces.xml" : System.Configuration.ConfigurationManager.AppSettings["xmlDir"].ToString() + "LoginResponseFail.xml";            
            byte[] returnXML = System.Text.UTF8Encoding.UTF8.GetBytes(string.Format(this.xmlUtil.GetTextFromXMLFile(fileName), targetURL));
            Response.Write(string.Format(this.xmlUtil.GetTextFromXMLFile(fileName), targetURL));
        }
        catch (WebException webEx)
        {
           Response.Write(webEx.ToString());
      
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());

        }
        finally
        {
            if (req != null) req.GetRequestStream().Close();
        }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the page's content type to JPEG files
        // and clear all response headers.
        Response.ContentType = "text/xml";
        Response.Clear();

        // Buffer response so that page is sent
        // after processing is complete.
        Response.BufferOutput = true;
     try
       {


           if (!this.IsPostBack)
           {
               Stream resStream = Request.InputStream;
               byte[] buf = new byte[8192];
               StringBuilder sb = new StringBuilder();
               ShoppingSessionDAO dao = new ShoppingSessionDAO();
               //
               //if (resStream != null)
              if (this.Request.Params.Get("loginRequest") != null) 
               {

                  sb.Append(Server.UrlDecode(this.Request.Params["loginRequest"].ToString()));

                   if (sb.Length > 1)
                   {
                       //
                       //*****************************************************************
                       //*****************************************************************
                       //*****************************************************************
                       //  TO DO : validation of the pass         
                       bool validRequest = this.ReadFromRequest_User_Pass_ReturnURL_(sb.ToString()).Count > 0 ? true : false;
                       //  TO DO : validation of the pass         
                       //*****************************************************************
                       //*****************************************************************
                       //*****************************************************************                    
                       // XML Credentials
                       string inputUserName = this.ReadFromRequest_User_Pass_ReturnURL_(sb.ToString()).Get("userid");
                       string inputAppUserName = this.ReadFromRequest_User_Pass_ReturnURL_(sb.ToString()).Get("appUserName");
                       string inputPassword = this.ReadFromRequest_User_Pass_ReturnURL_(sb.ToString()).Get("password");
                       // Configured Credentials
                       string procUser = System.Configuration.ConfigurationManager.AppSettings["procUser"].ToString();
                       string procPass = System.Configuration.ConfigurationManager.AppSettings["procPass"].ToString();
                       string username = System.Configuration.ConfigurationManager.AppSettings["username"].ToString();
                       string userval = System.Configuration.ConfigurationManager.AppSettings["userval"].ToString();

                           if (inputPassword == procPass)
                       {
                           // Insert row, including hash for session
                           string hashcode = System.Guid.NewGuid().ToString();
                           hashcode = hashcode.Replace("-", "");
                           int SessionId = dao.CreateShoppingSession(inputAppUserName, hashcode);
                           string targetURLStem = System.Configuration.ConfigurationManager.AppSettings["targetURL"].ToString();                                                      
                           string returnURL = this.ReadFromRequest_User_Pass_ReturnURL_(sb.ToString()).Get("returnURL");
                           returnURL = Server.UrlEncode(returnURL);
                           string targetURL = string.Format("{0}ShoppingCartConfirmation.aspx?userid={1}&showAll=true&ru={2}&hashcode={3}&sessionid={4}", targetURLStem, inputAppUserName, returnURL, hashcode, SessionId);
                           this.ResponseToRequest(validRequest,returnURL, targetURL);

                       } else
                           {

                               this.ResponseToRequest(false, "", "");              
                  
                           }
                   }
               }
               else
               {
                   this.ResponseToRequest(false, "", "");
               }
           }
       }
       catch (WebException ex)
       {
           Response.Write(ex.ToString());
           //   tw.WriteLine(ex2.ToString());
           //Response.End();
       }
     catch (Exception ex2)
       {
           Response.Write(ex2.ToString());
        //   tw.WriteLine(ex2.ToString());
           //Response.End();
       }
       finally
       {
           Response.Flush();
          // tw.Close();
       }
    }
}

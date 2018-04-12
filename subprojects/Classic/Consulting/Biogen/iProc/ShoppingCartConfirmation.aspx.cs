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
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
//

public partial class ShoppingCartConfirmation : System.Web.UI.Page
{
    //
    ShoppingCartDAO dao = new ShoppingCartDAO();
    ShoppingSessionDAO dao2 = new ShoppingSessionDAO();
    //    
    protected void Page_Load(object sender, EventArgs e)
    {        

        // :: PATCH for keep returnURL "visible" at any moment and most important when needed
        string userid = "";
        string hashcode = "";
        string sessionid = "";
        int ValidSession = 0;
        int CheckSession = 0;
        int CreateSession = 1;
        if (!this.IsPostBack)
        {
            try
            {


                // Case 1: Check to see if linked in from ChemInv.
                if (Request.QueryString["cslaunch"] != null && Request.QueryString["csuserid"] != null)
                {

                    if (this.Session["userid"].ToString().ToUpper() != Request.QueryString["csuserid"].ToString().ToUpper())
                    {
                        Response.Redirect("errorPage.aspx?msgid=2");
                    }
                    else
                    {
                        CheckSession = 1;
                    }
                }
                // check to see if there is a valid session
                if (Request.QueryString["validsession"] != null)
                {
                    if (this.Session["validsession"].ToString() == "1" & CheckSession != 1)
                    {
                        ValidSession = 1;
                        CreateSession = 0;
                    }
                    // If not valid session then get request string values
                }
                if (CheckSession == 1)
                {
                    if (this.Session["sessionid"] != null && this.Session["hashcode"] != null && this.Session["userid"] != null)
                    {
                        userid = this.Session["userid"].ToString();
                        hashcode = this.Session["hashcode"].ToString();
                        sessionid = this.Session["sessionid"].ToString();
                        ValidSession = 1;
                        CreateSession = 0;
                    }
                }

                if (ValidSession != 1 && CreateSession == 1)
                {
                    if (this.Request.QueryString["userid"] != null && this.Request.QueryString["sessionid"] != null && this.Request.QueryString["hashcode"] != null)
                    {
                        userid = this.Request.QueryString["userid"].ToString();
                        hashcode = this.Request.QueryString["hashcode"].ToString();
                        sessionid = this.Request.QueryString["sessionid"].ToString();
                        ValidSession = dao2.CheckShoppingSession(userid, hashcode, sessionid);
                    }


                }

                if (ValidSession == 1 && CheckSession != 1 && CreateSession == 1)
                {
                    this.Session.Add("validsession", 1);
                    this.Session.Add("userid", userid);
                    this.Session.Add("cartsessionvars", "userid=" + userid + "&hashcode=" + hashcode + "&sessionid=" + sessionid);
                    this.Session.Add("returnURL", this.Request.QueryString["ru"].ToString());
                }

                //    Response.Write("<BR>" + ValidSession + "<BR>");
            }
            catch (Exception ex)
            {
                //Response.Write(ex.ToString());
                    Response.Redirect("errorPage.aspx?msgid=2");
            }
            //if (ValidSession != 1) Response.Redirect("errorPage.aspx?msgid=2");
        }
        else
        {
            CreateSession = 0; // this should be 0 if it is a postback.
        }
     
         
        
        ImageButton ib = new ImageButton();
        ib.ImageUrl = "Images/logo_cs_chemacxdb_r.gif";
        ib.PostBackUrl = System.Configuration.ConfigurationManager.AppSettings["serverURL"].ToString() + "chemacx/inputtoggle.asp?formgroup=base_form_group&dataaction=db&dbname=chemacx";

     

        Label nl = new Label();
        nl.Text = "<BR><BR>";
        


        ButtonsSection.Controls.Add(ib);
        ButtonsSection.Controls.Add(nl);

            int viewStatus;
            if (this.Request.QueryString["view"]=="")
            {
                viewStatus = 0; // show new
            }
            else
            {
                if (this.Request.QueryString["view"]!=null  )
                {
                    viewStatus = Convert.ToInt16(this.Request.QueryString["view"]);
                } else {
                    viewStatus = 0;
                }
            }

            //
            //if userid param is not provided, the cartid must be in the query string
            string showAll = this.Request.QueryString["showAll"] != null ? this.Request.QueryString["showAll"].ToString() : "false";
            string id4search = "0";
            //individual shopping cart display
            if (showAll == "false")
            {
                //individual display
                id4search = this.Request.QueryString["cartid"];
                this.BindShoppingCartData(id4search, viewStatus);
            }
            else
            {
                Label links = new Label();
                links.Font.Names = new string[] { "verdana", "arial", "helvetica", "sans-serif" };
                links.Font.Size = 12;
                links.Font.Bold = true;
                if (viewStatus!=0 ) {
                links.Text = "<a href='ShoppingCartConfirmation.aspx?view=0&showAll=true'>View New</a>&nbsp;&nbsp;&nbsp;";
                } else {
                    links.Text = "View New&nbsp;&nbsp;&nbsp;";
                }
                if (viewStatus!=1 ) {
                links.Text += "<a href='ShoppingCartConfirmation.aspx?view=1&showAll=true'>View Sent</a>&nbsp;&nbsp;&nbsp;";
                } else {
                    links.Text += "View Sent&nbsp;&nbsp;&nbsp;";
                }
                if (viewStatus != 2)
                {
                    links.Text += "<a href='ShoppingCartConfirmation.aspx?view=2&showAll=true'>View Cancelled</a>&nbsp;&nbsp;&nbsp;";
                }
                else
                {
                    links.Text += "View Cancelled";
                }
                links.Text += "<a href=\"" + Session["returnURL"].ToString()  + "\" > Return to iProcurement</a>";
                HiddenField hf = new HiddenField();
                hf.ID = "oracleCart";
                hf.Value = "";
                ButtonsSection.Controls.Add(hf);                   
                    ButtonsSection.Controls.Add(links);
                //display all shoppingcarts for the specified user
                //id4search = this.Request.QueryString["userid"];
                // :: PATCH for keep userid "visible" at any moment and most important when needed on Save State button click event

                //BindShoppingCartsOf(userid)
                this.BindShoppingCartsOf(Session["userid"].ToString(),viewStatus);
         
            }
            //
        
                
    }        

    private void BindShoppingCartsOf(string userid, int cartstatus)
    {
        //ShoppingCartDAO dao = new ShoppingCartDAO();
        {
            ArrayList shopcarts = dao.GetShoppingCartsOf(userid,cartstatus);
            this.GridView1.DataSource = shopcarts;            
            this.GridView1.DataBind();
            this.GridView1.CellPadding = 8;
            this.GridView1.Visible = true;
            this.GridView1.Font.Names = new string[] { "verdana", "arial", "helvetica", "sans-serif" };
            this.GridView1.Font.Size = 10;
           
           
        }
    }

    private void BindShoppingCartData(string cartid, int viewStatus)
    {
        //
        {
            ShoppingCartTO shopcart = dao.GetShoppingCartById(cartid);
            this.GridView1.Visible = false;
            //
            this.BindXMLData(shopcart.XML);

            ReadShoppingCartXML(shopcart.XML);
            HiddenField hf = new HiddenField();
            hf.ID = "oracleCart";
            hf.Value = shopcart.XML;
            ButtonsSection.Controls.Add(hf);
            HiddenField hf2 = new HiddenField();
            hf2.ID = "returnURL";
            hf2.Value = Server.UrlEncode(Session["returnURL"].ToString());
            ButtonsSection.Controls.Add(hf);
            ButtonsSection.Controls.Add(hf2);
            
            
            
            Button SubmitButton = new Button();
            SubmitButton.Text = "Submit to iProcurement";//"Submit to iProcurement";
            SubmitButton.PostBackUrl = Session["returnURL"].ToString();
            SubmitButton.OnClientClick = "submitCart(" + shopcart.CartId +",'" + Session["cartsessionvars"].ToString() + "');";
            SubmitButton.Font.Names =  new string[] { "verdana", "arial", "helvetica", "sans-serif" };
            SubmitButton.Font.Size = 10;
            if (shopcart.Status == "0" ) { SubmitButton.Enabled = true; }
            else { SubmitButton.Enabled = false; }

            ButtonsSection.Controls.Add(SubmitButton);
            
        
            Button CancelCartButton = new Button();
            CancelCartButton.Text = "Cancel Cart";
            CancelCartButton.OnClientClick = "clearCart();cancelCart(" + shopcart.CartId + ",'" + Session["cartsessionvars"].ToString() + "');";
            CancelCartButton.PostBackUrl = "ShoppingCartConfirmation.aspx?view=" + shopcart.Status.ToString() + "&showAll=true";
            CancelCartButton.Font.Names = new string[] { "verdana", "arial", "helvetica", "sans-serif" };
            CancelCartButton.Font.Size = 10;
            if (shopcart.Status == "0" ) { CancelCartButton.Enabled = true; }
            else { CancelCartButton.Enabled = false; }
            ButtonsSection.Controls.Add(CancelCartButton);



            Button CancelButton = new Button();
            CancelButton.Text = "Return to Cart List";
            CancelButton.OnClientClick = "clearCart();";
            CancelButton.PostBackUrl = "ShoppingCartConfirmation.aspx?view=" + shopcart.Status.ToString() + "&showAll=true";
            CancelButton.Font.Names = new string[] { "verdana", "arial", "helvetica", "sans-serif" };
            CancelButton.Font.Size = 10;
            ButtonsSection.Controls.Add(CancelButton);
       
      


        }
    }

    private void BindXMLData(string xml)
    {
        DataSet ds = new DataSet();
        DataTable dt = ds.Tables.Add();
        DataRow dr;

        dt.Columns.Add("DUNS");
        dt.Columns.Add("Item ID");
        dt.Columns.Add("Item Description");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Price");
        

        StringReader sr = new StringReader(xml);
        XmlDocument xDoc = new XmlDocument();

        xDoc.LoadXml(xml);

        XmlElement qty;
        XmlElement itemDescription;
        XmlElement orderLine;
        XmlElement catNumber;
        XmlElement price;
        XmlElement DUNS;

        foreach (XmlNode node in xDoc.SelectNodes("/response/body/OrderLinesDataElements/orderLine"))
        {
            orderLine = node as XmlElement;
            itemDescription = (XmlElement) orderLine.SelectSingleNode("item/itemDescription");
            qty = (XmlElement)orderLine.SelectSingleNode("item");
            catNumber = (XmlElement)orderLine.SelectSingleNode("item/itemNumber/supplierItemNumber/itemID");
            price = (XmlElement)orderLine.SelectSingleNode("price/unitPrice");
            DUNS = (XmlElement)orderLine.SelectSingleNode("supplier/supplierDUNS");

                    dr = ds.Tables[0].NewRow();
                    dr["DUNS"] = DUNS.InnerText;
                    dr["Item Id"] = catNumber.InnerText;
                    dr["Item Description"] = itemDescription.InnerText;
                    dr["Quantity"] = qty.GetAttribute("quantity").ToString();
                    dr["Price"] = price.InnerText;
                    ds.Tables[0].Rows.Add(dr);            
        }

      

        
       // ds.ReadXml(sr);
        DataGrid dg = new DataGrid();
        dg.CellPadding = 8;
        dg.DataSource = ds;
        dg.DataBind();
        dg.Font.Names = new string[] { "verdana", "arial", "helvetica", "sans-serif" };
        dg.Font.Size = 10;
        this.Controls.Add(dg);        
    }



   

    private void ReadShoppingCartXML(string xml)
    {
        //temp file creation is needed in order to xDoc Load the XML file
        //XmlDocument xDoc = new XmlDocument();
       // xDoc.LoadXml(xml);
       // string timestamp = DateTime.Now.ToBinary().ToString();
       // string tempFileName = String.Format("ShoppingCart_UpdatedOn{0}.xml", timestamp);
     //   string pathTempFile = String.Format("C:\\Program Files\\Microsoft Visual Studio 8\\Common7\\IDE\\aa_Temp_XML_PunchOut\\{0}", tempFileName);
     //   xDoc.Save(pathTempFile);
    //    XPathDocument doc = new XPathDocument(pathTempFile);
    //    XPathNavigator nav = doc.CreateNavigator();
        // Compile a standard XPath expression
   //     XPathExpression expr;
        //expr = nav.Compile("/response/body/OrderLinesDataElements/orderLine/price/unitPrice");
        //select all the itemDescription on the xml doc
   //     expr = nav.Compile("//itemDescription");
        //expr = nav.Compile("item[@itemID and not (@itemID=19043060)]");
   //     XPathNodeIterator iterator = nav.Select(expr);
        // Iterate on the node set
      
   
    }


   
}

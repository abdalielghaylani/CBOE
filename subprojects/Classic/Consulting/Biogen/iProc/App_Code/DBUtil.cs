using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;

//
using System.Data.OracleClient;
//
using System.Collections;
//
namespace Util
{
    /// <summary>
    /// Summary description for ConnManager
    /// </summary>
    public class ConnManager
    {
        /// <summary>
        /// Returns a CLOSED OracleConnection object
        /// </summary>
        /// <returns></returns>
        public OracleConnection GetConnection()
        {
            // add username/password read here.

            return new OracleConnection("User Id=" + System.Configuration.ConfigurationManager.AppSettings["username"].ToString() + ";Password=" + System.Configuration.ConfigurationManager.AppSettings["userval"].ToString() + ";Data Source=" +System.Configuration.ConfigurationManager.AppSettings["oradb"].ToString()+ ";");
        }

        public ConnManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

    /// <summary>
    /// Summary description for TransferObjects
    /// </summary>
    public class ShoppingCartTO
    {
        private int _cart_id;
        private string _userId;
        private DateTime _createdDate;
        private string _xml;
        private string _status;

        public int CartId
        {
            get { return this._cart_id; }
            set { this._cart_id = value; }
        }

        public string UserId
        {
            get { return this._userId; }
            set { this._userId = value; }
        }

        public DateTime CreatedDate
        {
            get { return this._createdDate; }
            set { this._createdDate = value; }
        }

        public string XML
        {
            get { return this._xml; }
            set { this._xml = value; }
        }

        public string Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        public ShoppingCartTO()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

    public class ShoppingSessionDAO
    {
        private ConnManager connManager;

        public ShoppingSessionDAO()
        {
            this.connManager = new ConnManager();
        }

        // This function creates a session id for the login request.

        public int CreateShoppingSession(string userid, string sessionName)
        {
            TextWriter tw = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["xmlDir"].ToString() + "logfile_dbutils.txt");   
        
           int returnVal=-1;
           this.connManager = new ConnManager();
           OracleConnection ora = this.connManager.GetConnection();
            try {
             
                ora.Open();
            OracleCommand cmd = new OracleCommand("CHEMINVDB2.CREATEPROCSESSION", ora);
            cmd.CommandType= CommandType.StoredProcedure;
            // add return value
            cmd.Parameters.Add(new OracleParameter("return_value", OracleType.Int32));
            cmd.Parameters["return_value"].Direction = ParameterDirection.ReturnValue;

            // add userid and session name inputs
            cmd.Parameters.Add(new OracleParameter("puserid",OracleType.VarChar));
            cmd.Parameters["puserid"].Direction=ParameterDirection.Input;
            cmd.Parameters["puserid"].Value=userid;

            cmd.Parameters.Add(new OracleParameter("psessionname",OracleType.VarChar));
            cmd.Parameters["psessionname"].Direction=ParameterDirection.Input;
            cmd.Parameters["psessionname"].Value=sessionName;
            
            cmd.ExecuteNonQuery();
            returnVal = Convert.ToInt32(cmd.Parameters["return_value"].Value);
            } catch (Exception ex) {
                returnVal = -1;
                tw.WriteLine(ex.ToString());
            } finally {
                ora.Close();
                tw.Close();
            }
            return returnVal;            
        }
        public int CheckShoppingSession(string userid, string sessionName, string sessionid)
        {
            int returnVal = -1;
            this.connManager = new ConnManager();
            OracleConnection ora = this.connManager.GetConnection();
            try
            {
                ora.Open();
                OracleCommand cmd = new OracleCommand("CHEMINVDB2.CHECKPROCSESSION", ora);
                cmd.CommandType = CommandType.StoredProcedure;
                // add return value
                cmd.Parameters.Add(new OracleParameter("return_value", OracleType.Int32));
                cmd.Parameters["return_value"].Direction = ParameterDirection.ReturnValue;

                // add userid and session name inputs
                cmd.Parameters.Add(new OracleParameter("puserid", OracleType.VarChar));
                cmd.Parameters["puserid"].Direction = ParameterDirection.Input;
                cmd.Parameters["puserid"].Value = userid;

                cmd.Parameters.Add(new OracleParameter("psessionname", OracleType.VarChar));
                cmd.Parameters["psessionname"].Direction = ParameterDirection.Input;
                cmd.Parameters["psessionname"].Value = sessionName;

                cmd.Parameters.Add(new OracleParameter("psessionid", OracleType.VarChar));
                cmd.Parameters["psessionid"].Direction = ParameterDirection.Input;
                cmd.Parameters["psessionid"].Value = sessionid;

                cmd.ExecuteNonQuery();
                returnVal = Convert.ToInt32(cmd.Parameters["return_value"].Value);
            }
            catch (Exception ex)
            { 
    
                returnVal = -1;
            }
            finally
            {
                ora.Close();
            }
            return returnVal;
        }

        public int CheckShoppingSession2(string userid, string sessionName, string sessionid, string cartid, string status)
        {
            int returnVal = -1;
            this.connManager = new ConnManager();
            OracleConnection ora = this.connManager.GetConnection();
            try
            {
                ora.Open();
                OracleCommand cmd = new OracleCommand("CHEMINVDB2.CHECKPROCSESSION2", ora);
                cmd.CommandType = CommandType.StoredProcedure;
                // add return value
                cmd.Parameters.Add(new OracleParameter("return_value", OracleType.Int32));
                cmd.Parameters["return_value"].Direction = ParameterDirection.ReturnValue;

                // add userid and session name inputs
                cmd.Parameters.Add(new OracleParameter("puserid", OracleType.VarChar));
                cmd.Parameters["puserid"].Direction = ParameterDirection.Input;
                cmd.Parameters["puserid"].Value = userid;

                cmd.Parameters.Add(new OracleParameter("psessionname", OracleType.VarChar));
                cmd.Parameters["psessionname"].Direction = ParameterDirection.Input;
                cmd.Parameters["psessionname"].Value = sessionName;

                cmd.Parameters.Add(new OracleParameter("psessionid", OracleType.VarChar));
                cmd.Parameters["psessionid"].Direction = ParameterDirection.Input;
                cmd.Parameters["psessionid"].Value = sessionid;

                cmd.Parameters.Add(new OracleParameter("pcartid", OracleType.VarChar));
                cmd.Parameters["pcartid"].Direction = ParameterDirection.Input;
                cmd.Parameters["pcartid"].Value = cartid;

                cmd.Parameters.Add(new OracleParameter("pstatus", OracleType.VarChar));
                cmd.Parameters["pstatus"].Direction = ParameterDirection.Input;
                cmd.Parameters["pstatus"].Value = status;

                cmd.ExecuteNonQuery();
                returnVal = Convert.ToInt32(cmd.Parameters["return_value"].Value);
            }
            catch (Exception ex)
            {
                returnVal = -1;
            }
            finally
            {
                ora.Close();
            }
            return returnVal;
        }


    }
    /// <summary>
    /// Summary description for SessionDAO
    /// </summary>
    public class ShoppingCartDAO
    {
        private ConnManager connManager;

        public ShoppingCartDAO()
        {
            this.connManager = new ConnManager();
        }

        public ArrayList GetShoppingCartsOf(string username, int cartstatus)
        {
            string tempCartStatus;
            if (cartstatus == 1 ||  cartstatus == 3)
            {
                tempCartStatus = "1,3";
            }
            else
            {
                tempCartStatus = cartstatus.ToString();
            }

            try
            {
                //
                ArrayList list = new ArrayList();
                OracleConnection ora = this.connManager.GetConnection();
                ora.Open();
                string SELECT_FROM_SHOPPINGCARTS = String.Format("select * from cheminvdb2.shopping_carts where userid = '{0}' and status in ({1}) order by create_date asc", username, tempCartStatus);
                OracleCommand cmd = new OracleCommand(SELECT_FROM_SHOPPINGCARTS, ora);
                //cmd.CommandType = CommandType.Text;                
                //
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                ora.Close();
                return this.ConvertToShoppingCartOBjList(dt);
                //                
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing GetShoppingCartsOf SP.Details:" + ex.ToString());
            }
        }

        public int UpdateShoppingCartStatus(string cartid,string status)
        {
            try
            {
                //
                OracleConnection ora = this.connManager.GetConnection();
                ora.Open();
                string UPDATE_SHOPPINGCART = String.Format("update cheminvdb2.shopping_carts set status = {0} where cart_id = {1}",status, cartid);
                OracleCommand cmd = new OracleCommand(UPDATE_SHOPPINGCART, ora);
                //cmd.CommandType = CommandType.Text;                
                //
                return cmd.ExecuteNonQuery();
                /*OracleDataAdapter oda = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                ora.Close();
                return this.ConvertToShoppingCartOBj(dt);*/
                //                
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing UpdateShoppingCartStatus SP.Details:" + ex.ToString());
            }
        }

        public ShoppingCartTO GetShoppingCartById(string id)
        {
            try
            {
                //
                OracleConnection ora = this.connManager.GetConnection();
                ora.Open();
                string SELECT_FROM_SHOPPINGCARTS = String.Format("select * from cheminvdb2.shopping_carts where cart_id = {0}",id);
                OracleCommand cmd = new OracleCommand(SELECT_FROM_SHOPPINGCARTS, ora);
                //cmd.CommandType = CommandType.Text;                
                //
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                ora.Close();
                return this.ConvertToShoppingCartOBj(dt);
                //                
            }
            catch (Exception ex)
            {
                throw new Exception("Error Executing GetShoppingCartById SP.Details:" + ex.ToString());
            }
        }

        public ShoppingCartTO ConvertToShoppingCartOBj(DataTable dt)
        {
            ShoppingCartTO shop = new ShoppingCartTO();
            shop.CartId = Int32.Parse(dt.Rows[0]["CART_ID"].ToString());
            shop.UserId = dt.Rows[0]["USERID"].ToString();
            shop.CreatedDate = DateTime.Parse(dt.Rows[0]["CREATE_DATE"].ToString());
            shop.XML = dt.Rows[0]["CART_XML"].ToString();
 
            /*switch (Int32.Parse(dt.Rows[0]["STATUS"].ToString())) 
            { 
                case 0: 
                    shop.Status = "New";
                    break;
                case 1: 
                    shop.Status = "Sent";
                    break;
                case 2:
                    shop.Status = "Cancelled";
                    break;
                default:
                    shop.Status = "Unknown";
                    break;
            }
            */
            shop.Status = dt.Rows[0]["STATUS"].ToString(); 
            //
            return shop;
        }

        public ArrayList ConvertToShoppingCartOBjList(DataTable dt)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ShoppingCartTO shop = new ShoppingCartTO();
                shop.CartId = Int32.Parse(dt.Rows[i]["CART_ID"].ToString());
                shop.UserId = dt.Rows[i]["USERID"].ToString();
                shop.CreatedDate = DateTime.Parse(dt.Rows[i]["CREATE_DATE"].ToString());
                shop.XML = dt.Rows[i]["CART_XML"].ToString();
                shop.Status = dt.Rows[i]["STATUS"].ToString();
                //
                list.Add(shop);
            }
            //
            return list;
        }                  
    }
}
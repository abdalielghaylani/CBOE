using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Security;
using System.Runtime.InteropServices;
using System.Xml;
using CambridgeSoft.COE.Security.Services.Utlities;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.IdentityModel.Protocols;
//using Microsoft.IdentityModel.Protocols.OpenIdConnect;
//using Microsoft.IdentityModel.Tokens;


namespace CambridgeSoft.COE.Security.Services
{

	/// <summary>
	/// The SingleSignOn allows you to Authenticate a user and receive an authentication ticket.
	/// It then allows you to get data from that ticket.
	/// </summary>
	/// 
	/// <example>
	/// <code>
	/// // Create the class for single sign on
	/// SingleSignOn.SingleSignOn aws = new SingleSignOn.SingleSignOn();
	/// 
	/// // Authenticate user named John with his password but don't get a ticket
	/// bool authenticated = aws.Authenticate("John", "password");
	/// 
	/// // Authenticate user named John with his password and get a ticket
	/// string encryptedAuth = aws.GetAuthenticatedTicket("John", "password");
	///
	/// //Get the expiration date of the ticket
	/// string expiration = aws.GetTicketExpirationDate(encryptedAuth).ToString();
	/// </code>
	/// </example>


	[WebService(Namespace = "http://cambridgesoft.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class SingleSignOn : System.Web.Services.WebService
	{

		SingleSignOnProvider ssop = new SingleSignOnProvider();
		ICOESSO objSSO;
		private SSOConfigurationExemptUsersCollection exemptUsersList =  SSOConfigurationProvider.GetConfig().ExemptUsers;
		static readonly Int32 _timeOut = GetTimeOut();



		private Int32 TimeOut = 20;

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleSignOn"/> class.
		/// </summary>
		//public SingleSignOn()
		//{

			//Uncomment the following line if using designed components 
			//InitializeComponent(); 
		//}
		
		/// <summary>
		/// Get an encrypted authentication ticket.
		/// </summary>
		/// <param name="userName">The userName you wish to authenticate.</param>
		/// <param name="password">The password for the userName you wish to authenticate.</param>
		/// <returns><c>Encrypted Ticket</c> as a string for a valid user</returns>
		[WebMethod(Description = "Takes a userName and generates and encrypted ticket.")]
		public string GetAuthenticationTicket(string userName, string password)
		{
            bool isValidUser = ValidateUser(userName, password);
            
            if (isValidUser)
			{
				if ((_timeOut  > 0) && (_timeOut < 500))
				{
					TimeOut = _timeOut;
				}

				FormsAuthenticationTicket authTicket;
				string userData = "Good";                

                string audience = ConfigurationManager.AppSettings["Audience"];
                if (string.IsNullOrEmpty(audience))
                {
                    password = GetCSPassword(userName, password);
                }
                else
                {
                    password = GetCSPassword(userName, "AzureAD");
                }

				if((SSOConfigurationProvider.GetConfig().GetSettings["ENCRYPT_USER_PASSWORDS"] != null) && (SSOConfigurationProvider.GetConfig().GetSettings["ENCRYPT_USER_PASSWORDS"].Value.ToUpper() == "YES"))
				{
                    //this is not implemented currently
					password = password;
					
				}
                //leaving this here in case it turns out it was used, but I don't think so
				//userData += " | password=" + password;

                userData += " | cssecuritypassword=" + password;
                
				authTicket = new FormsAuthenticationTicket(1,
				  userName,
				  DateTime.Now,
				  DateTime.Now.AddMinutes(TimeOut),
				  true, userData, FormsAuthentication.FormsCookiePath);
				//try putting in password
				
				string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
				return encryptedTicket;
			}
			else
			{

				//throw new Exception("Not a valid user");
				return "INVALID_USER";
			}
		
		}

        /// <summary>
        /// Get an expiry date of SSO authentication.
        /// </summary>
        /// <param name="userName">The userName you wish to authenticate.</param>
        /// <param name="password">The password for the userName you wish to authenticate.</param>
        /// <returns><c>Days to expire</c> as a integer for a valid user</returns>
        [WebMethod(Description = "Takes a userName and Password and generates days to expire.")]
        public int GetNumberOfDaysToExpire(string userName, string password)
        {
            int expirydate;
            expirydate = GetExpiryDate(userName, password);
            return expirydate;
        }

		/// <summary>
		/// Authenticates the specified user against the default provider or specified exempt provider.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <returns><c>true</c> if the user has been authenticated; otherwise returns <c>false</c></returns>
		[WebMethod(Description = "Tells you whether a user is authenticated or not.")]
		public bool Authenticate(string userName, string password)
		{

			bool isValidUser = ValidateUser(userName, password);

			if (isValidUser)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// Takes a Single Sign On ticket and get the unencrypted password from it.
        /// Use of this method is discouraged, but it is needed until such time we retire classic ChemOffice Enterprise
        /// </summary>
        /// <param name="encryptedTicket">The encrypted ticket.</param>
        /// <returns>Unencrypted password used for classic cssecurity</returns>
		[WebMethod(Description = "Verifies an encrypted ticket and returns cs secrutiy password (unencrypted) )")]
		public string GetCSSecurityPwd(string encryptedTicket)
		{
            string password = string.Empty;

			try
			{
				//if ticket is valid
				if (ValidateTicket(encryptedTicket))
				{
					try
					{
						password = GetCSPasswordFromTicket(encryptedTicket);
					}
					catch
					{
						return "ERROR: NO_PASSWORD_IN_TICKET";
					}
				}
				//if not then return an error
				else
				{
					return "ERROR: INVALID_TICKET";
				}
			}
			catch
			{
				return "ERROR: UNSPECIFIED";
			}

            return password;
		}

        private string GetCSPasswordFromTicket(string encryptedTicket)
        {

            FormsAuthenticationTicket decryptedTicket;

            string password = string.Empty;
            try
            {
                decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
                string[] userDataArray = decryptedTicket.UserData.Split(new string[]{"|"},StringSplitOptions.RemoveEmptyEntries) ;
                
                foreach (string v in userDataArray)
                {
                     if (v.IndexOf("cssecuritypassword=") > 0)
                    {
                        password = v.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1].ToString();
                        break;
                    }
                }

                return password;
            }
            catch
            {
                //log reason for failure or whatever here if you like
                throw new System.Exception("Not a valid user");
            }

            
        }


		/// <summary>
		/// Determines whether the specified encrypted ticket is valid.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>
		/// 	<c>true</c> if the specified encrypted ticket is authenticated; otherwise, <c>false</c>.
		/// </returns>
		[WebMethod(Description = "Can be used to confrim Authentication.  Will return false if the ticket is not valid or if it is expired.")]
		public bool ValidateTicket(string encryptedTicket)
		{

			if (encryptedTicket.Trim().Length <= 0)
				return false;

			FormsAuthenticationTicket decryptedTicket;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch
			{
				//log reason for failure or whatever here if you like
				return false;
			}

			if (decryptedTicket.Expired)
				return false;

			return true;

		}


		/// <summary>
		/// Renews the ticket if it is old.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>A new encrypted ticket, which should be used in place of the old one.</returns>
		[WebMethod(Description = "Used to renew an authentication ticket to keep it alive. Returns a new ticket.")]
		public string RenewTicketIfOld(string encryptedTicket)
		{
            string returnTicket = string.Empty;

			if (encryptedTicket.Trim().Length <= 0)
				return "This is an invalid ticket";

			FormsAuthenticationTicket decryptedTicket;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch (System.Exception err)
			{
				//log reason for failure or whatever here if you like
				throw err;
			}

			if (decryptedTicket.Expired)
			{
			
				returnTicket = FormsAuthentication.Encrypt(FormsAuthentication.RenewTicketIfOld(decryptedTicket));
			}
			else
			{
                this.ThrowSoapEx("Your ticket was not expired.");
			}
            return returnTicket;
		}

		/// <summary>
		/// Renews the ticket no matter what.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>A new encrypted ticket, which should be used in place of the old one.</returns>
		[WebMethod(Description = "Used to renew an authentication ticket to keep it alive. Returns a new ticket.")]
		public string RenewTicket(string encryptedTicket)
		{
            string returnTicket = string.Empty;

			if (encryptedTicket.Trim().Length <= 0)
				return "This is an invalid ticket";

			FormsAuthenticationTicket decryptedTicket;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch (System.Exception err)
			{
				//log reason for failure or whatever here if you like
				throw err;
			}

			try 
			{
				returnTicket = FormsAuthentication.Encrypt(FormsAuthentication.RenewTicketIfOld(decryptedTicket));
			}
			catch (System.Exception err)
			{
				this.ThrowSoapEx(err.Message);
			}
            return returnTicket;
		}

		/// <summary>
		/// Gets the user from ticket.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>The userName from the ticket.</returns>
		[WebMethod(Description = "Get a userName from an encrypted ticket.")]
		public string GetUserFromTicket(string encryptedTicket)
		{
			FormsAuthenticationTicket decryptedTicket = null;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch
			{
				//log reason for failure or whatever here if you like
				this.ThrowSoapEx("Not a valid user");
			}
            return decryptedTicket.Name;
		}

		/// <summary>
		/// Gets the user from ticket.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>The userName from the ticket.</returns>
		[WebMethod(Description = "Get the userData from an encrypted ticket.")]
		public string GetUserDataFromTicket(string encryptedTicket)
		{
			FormsAuthenticationTicket decryptedTicket = null;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch
			{
				//log reason for failure or whatever here if you like
				this.ThrowSoapEx("Not a valid user");
			}
            return decryptedTicket.UserData;
		}

		/// <summary>
		/// Gets the expiration date/time of the authentication ticket as a string.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>The expiration date and time of the ticket</returns>
		[WebMethod(Description = "Get a userName from an encrypted ticket.")]
		public DateTime GetTicketExpirationDate(string encryptedTicket)
		{
			FormsAuthenticationTicket decryptedTicket = null;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch (System.Exception e)
			{
				//log reason for failure or whatever here if you like
				this.ThrowSoapEx(e.Message);
			}

            return decryptedTicket.Expiration;
		}

		/// <summary>
		/// Gets the date date/time that the ticket was issued as a string.
		/// </summary>
		/// <param name="encryptedTicket">The encrypted ticket as a string.</param>
		/// <returns>The issue date and time of the ticket</returns>
		[WebMethod(Description = "Get a userName from an encrypted ticket.")]
		public DateTime GetTicketIssueDate(string encryptedTicket)
		{
			FormsAuthenticationTicket decryptedTicket = null;

			try
			{
				decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch (System.Exception e)
			{
				//log reason for failure or whatever here if you like
				this.ThrowSoapEx(e.Message);
			}
            return decryptedTicket.IssueDate;
		}

		/// <summary>
		/// Confirm you can connect to the webservice
		/// </summary>
		/// <param name="s">The s that you are passing in.</param>
		/// <returns>The string you input.</returns>
		[WebMethod(Description = "I am here to make sure you can connect to me.")]
		public string EchoString(string s)
		{
			return s;
		}

		/// <summary>
		/// Gets the name of the cookie used for SingleSignOn from configuration.
		/// </summary>
		/// <returns>The name of the cookie used for the authentication.</returns>
        /// <remarks>This cookie is needed to be shared from classic and .Net implementations.</remarks>
		[WebMethod(Description = "This will be used by web based applications for storing the authentication info.")]
		public string GetCookieName()
		{
			return FormsAuthentication.FormsCookieName.ToString();
		}

        /// <summary>
        /// Checks whether the user exists in the configured provider.
        /// </summary>
        /// <param name="userName">The userName you are looking for.</param>
        /// <returns>
        /// <c>true</c> if the user can be found via the provider; otherwise, <c>false</c>.
        /// </returns>
		[WebMethod(Description = "This will be used to make sure a username exists without authentication")]
		public bool UserExists(string userName)
		{
			return checkUserExists(userName);
		}

		//these are for debugging
		///// <summary>
		///// Test Encryption
		///// </summary>
		///// <param name="p">The string to encypt.</param>
		///// <returns>encrytped string</returns>
		//[WebMethod(Description = "Encrypt a string")]
		//public string Encrypt(string p)
		//{
		//    return CambridgeSoft.COE.Security.Services.Utlities.Encryptor.Encrypt(p);
		//}

		///// <summary>
		///// Test Decryption
		///// </summary>
		///// <param name="p">The string to decrypt.</param>
		///// <returns>true/false</returns>
		//[WebMethod(Description = "Decrypt a string")]
		//public string Decrypt(string p)
		//{
		//    return CambridgeSoft.COE.Security.Services.Utlities.Encryptor.Decrypt(p);
		//}

        /// <summary>
        /// Gets the user information used to populate ChemBioOfficeEnterprise security interface.
        /// It is mapped via configuration
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Information about the user.
        /// <list>
        /// <item>Username</item>
        /// <item>Email</item>
        /// <item>First Name</item>
        /// <item>Last Name</item>
        /// <item>Middle Name</item>
        /// </list>
        /// </returns>
		[WebMethod(Description = "Get User Information so you can manipulate locally")]
		public XmlDocument GetUserInfo(string userName)
		{
            XmlDocument result = new XmlDocument();
			if (IsExemptUser(userName))
			{
				objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
			}
			else
			{
				objSSO = ssop.SSOChoose();
			}
            try
            {
                result = objSSO.GetUserInfo(userName);
            }
            catch (Exception ex)
            {
                this.ThrowSoapEx(ex.Message);
            }

            return result;
		}

        /// <summary>
        ///     Same call as <see cref="GetUserInfo"/> but returns a string
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Information about the user. <see cref="GetUserInfo"/> for details.</returns>
		[WebMethod(Description = "Get User Information so you can manipulate locally")]
		public string GetUserInfoAsString(string userName)
		{
            XmlDocument userInfo = new XmlDocument();
			if (IsExemptUser(userName))
			{
				
				objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
			}
			else
			{
				objSSO = ssop.SSOChoose();
			}
            try
            {
                userInfo = objSSO.GetUserInfo(userName);
            }
            catch (Exception ex)
            {
                this.ThrowSoapEx(ex.Message);
            }
			return userInfo.InnerXml.ToString();
		}

        /// <summary>
        /// Determines whether a user is an exempt user based on the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        /// 	<c>true</c> if the user is exempt; otherwise, <c>false</c>.
        /// </returns>
		[WebMethod(Description = "Checks whether a user is exempt")]
		public bool IsExemptUser(string userName)
		{
			return CheckIsExemptUser(userName);
		}

        /// <summary>
        /// Looks at configuration for the default authentication provider used by SSO
        /// </summary>
        /// <returns>
        /// <list>
        /// <item>ORACLE</item>
        /// <item>COELDAP</item>
        /// </list>
        /// </returns>
		[WebMethod(Description = "Gets the default authentication provider")]
		public string GetDefaultAuthenticationProvider()
		{
			return SSOConfigurationProvider.GetConfig().GetSettings["DEFAULT_PROVIDER"].Value.ToString();
		}


        /// <summary>
        /// Adds an existing user to the exempt list including the provider.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password of the user 
        /// (used for authenticating the user but not added to the configuration).
        /// </param>
        /// <param name="provider">The provider for which the user will be authenticated against.</param>
        /// <returns>The username if succesfully added or an error string starting with ERROR: on failure</returns>
        /// <remarks>
        /// The username and password are used to authenticate against the provider before 
        /// being added to the exempt users, so the user must exist.
        /// </remarks>
		[WebMethod(Description = "Adds an exempt user to the list")]
		public string AddExemptUser(string userName, string password, string provider)
		{
			//make sure it is a valid provider
			if (!(ssop.IsValidProvider(provider)))
			{
				return "ERROR: INVALID PROVIDER";
			}
			//objSSO = ssop.SSOChoose(provider);
			if (!(this.ValidateUser(userName, password)))
			{
				return "ERROR: USER COULD NOT BE VALIDATED";
			}

			//the make sure the user can be authenticated
			
			//then add it to the list
			SSOConfigurationExemptUsersCollection euc = new SSOConfigurationExemptUsersCollection();
			euc = exemptUsersList;

			SSOConfigurationExemptUser eu = new SSOConfigurationExemptUser();
			eu.userName = userName.ToUpper();
			eu.ssoProvider = provider;

			euc.Add(eu);
			Configuration config = SSOConfig.OpenConfig();
			SSOConfigurationProvider ssoProviderConfig = (SSOConfigurationProvider)config.GetSection("SSOConfiguration/ProviderConfiguration");
			ssoProviderConfig.ExemptUsers = euc;
			config.Save();
			ConfigurationManager.RefreshSection("SSOConfiguration/ProviderConfiguration");
			//exemptUsersList = SSOConfigurationProvider.GetConfig().ExemptUsers;
			//exemptUsersList = euc;
			return userName.ToUpper();
		}

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>Encrypted password understood by ChemBioOffice Enterprise</returns>
        /// <remarks>This function is primarily an internal CBOE function used by classic goods</remarks>
        [WebMethod(Description = "Encrypts a string - like a password for use in a config file")]
        public string EncryptString(string password)
        {
            return Encryptor.Encrypt(password);
        }

		private bool checkUserExists(string userName)
		{
            bool result = false;
			if (CheckIsExemptUser(userName))
			{
				objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
			}
			else
			{
				objSSO = ssop.SSOChoose();
			}
            try
            {
                result = objSSO.checkUserExists(userName);
            }
            catch (Exception ex)
            {
                this.ThrowSoapEx(ex.Message);
            }

            return result;
			
		}
		private bool ValidateUser(string userName, string password)
		{
            bool returnVal = false;
			if (CheckIsExemptUser(userName))
			{
				objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
			}
			else
			{
				objSSO = ssop.SSOChoose();
			}
            try
            {
                string audience = ConfigurationManager.AppSettings["Audience"];
                string issuer = ConfigurationManager.AppSettings["Issuer"];
                if (!string.IsNullOrEmpty(audience))
                {
                    returnVal = Validate(password, issuer, audience, userName);
                }
                else
                {
                    returnVal = objSSO.ValidateUser(userName, password);
                }

            }
            catch (Exception ex)
            {
                this.ThrowSoapEx(ex.Message);
            }
            return returnVal;
		}

        private bool Validate(string token, string issuer, string audience, string userName)
        {
            //try
            //{
            //    ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(issuer, new OpenIdConnectConfigurationRetriever());
            //    OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;
            //    string tenant = ConfigurationManager.AppSettings["Tenant"];
            //    TokenValidationParameters validationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = false,
            //        ValidateIssuer = false,
            //        IssuerSigningKeys = config.SigningKeys, //.net core calls it "IssuerSigningKeys" and "SigningKeys"
            //        ValidateLifetime = false
            //    };
            //    JwtSecurityTokenHandler tokendHandler = new JwtSecurityTokenHandler();
            //    SecurityToken jwt = null;
            //    var result = tokendHandler.ValidateToken(token, validationParameters, out jwt);
            //    Dictionary<string, object> valueColl = ((JwtSecurityToken)jwt).Payload;
            //    if (valueColl["upn"].ToString().Split('@')[0].ToUpper() != userName.ToUpper())
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            return true;
        }
		private bool CheckIsExemptUser(string userName)
		{
			if (exemptUsersList[userName.ToUpper()] == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
        private string GetCSPassword(string userName, string password)
        {
            string pwd = string.Empty;
			if (CheckIsExemptUser(userName))
			{
				objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
			}
			else
			{
				objSSO = ssop.SSOChoose();
			}
            try
            {
                pwd = objSSO.GetCSSecurityPassword(userName, password);
            }
            catch (Exception ex)
            {
                this.ThrowSoapEx(ex.Message);
            }
            return pwd;
        }
		private string GetExemptUserProvider(string userName)
		{
			return exemptUsersList[userName.ToUpper()].ssoProvider;
		}
		private static int GetTimeOut()
		{
			Int32 tout = 20;
			if (SSOConfigurationProvider.GetConfig().GetSettings["TICKET_TIMEOUT"] != null)
			{
				tout = Convert.ToInt32(SSOConfigurationProvider.GetConfig().GetSettings["TICKET_TIMEOUT"].Value.ToString());
			} 

			return tout;
		
		}

        private void ThrowSoapEx(string message)
        {
            // Build the detail element of the SOAP fault.
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);

            // Append the two child elements to the detail node.
            node.InnerText = message;

            //Throw the exception.    
            SoapException se = new SoapException(message, SoapException.ClientFaultCode, Context.Request.Url.AbsoluteUri, node);

            throw se;

        }

        private int GetExpiryDate(string userName, string password)
        {
            if (CheckIsExemptUser(userName))
            {
                objSSO = ssop.SSOChoose(GetExemptUserProvider(userName));
            }
            else
            {
                objSSO = ssop.SSOChoose();
            }
            return objSSO.GetCSExpiryDate(userName,password);
        }
    
    }
}




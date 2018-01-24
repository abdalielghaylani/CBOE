using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace CambridgeSoft.COE.Security.Services
{
	public interface ICOESSO
	{
        /// <summary>
        /// Should check whether the user exists for the current provider
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>return true if the user exists, otherwise false</returns>
		bool checkUserExists(string userName);

        /// <summary>
        /// Should validate the user using username and password
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>return true if the user can be validated, otherwise false</returns>
		bool ValidateUser(string userName, string password);


        /// <summary>
        /// Return an xml document that contains the followin
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>and xml document</returns>
        /// <remarks>This is only required for those providers that are expected to be used with CS Security</remarks>
        
        XmlDocument GetUserInfo(string userName);

        /// <summary>
        /// Gets the CS security password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        /// <remarks>This is only required for those providers that are expected to be used with CS Security.</remarks>
        string GetCSSecurityPassword(string userName, string Password);

        /// <summary>
        /// Gets the CS security password expiry date.
        /// </summary>
        int GetCSExpiryDate(string userName, string Password);
	}
}

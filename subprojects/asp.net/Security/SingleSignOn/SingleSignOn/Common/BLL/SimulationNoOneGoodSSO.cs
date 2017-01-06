using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace CambridgeSoft.COE.Security.Services
{
	public class SimulationNoOneGoodSSO : ICOESSO
	{
		#region ICOESSO Members

		public bool checkUserExists(string userName)
		{
			if (!(userName == "jordan"))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool ValidateUser(string userName, string password)
		{
			if ((userName == "jordan") && (password == "password"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsExemptUser(string userName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public XmlDocument GetUserInfo(string userName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

        public string GetCSSecurityPassword(string userName, string password)
        {
            return password;
        }

		#endregion
	}
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SingleSignOn;
using System.Reflection;

namespace CambridgeSoft.COE.Security.Services
{
	class SingleSignOnProvider
	{

		public ICOESSO SSOChoose()
		{
			string sourceSSO;

			//sourceSSO = ConfigurationManager.AppSettings.Get("SSOProvider").ToString();
			//SSOConfigurationLDAP.GetConfig().GetLDAPSettings[key].Value.ToString();
			sourceSSO = SSOConfigurationProvider.GetConfig().GetSettings["DEFAULT_PROVIDER"].Value.ToString();

			//sourceSSO = "coeldap";
			switch (sourceSSO.ToLower())
			{
				case "simulationeveryonegood":
					return new SimulationEveryoneGoodSSO();

				case "simulationnoonegood":
					return new SimulationNoOneGoodSSO();

				case "coeldapclassic":
					return new COELDAPClassic();

                case "coeldap":
                    return new COELDAP();

				case "cssecurity":
					return new CSSecurity();
				default:
                    try
                    {
                        string s = sourceSSO;

                        string[] sarr = s.Split(new char[] { ',' }); ;



                        ICOESSO theObj;
                        if (sarr.Length == 1)
                        {
                            Type theType = Type.GetType(s);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                        }
                        else
                        {
                            Assembly assembly = Assembly.Load(sarr[0]);
                            Type theType = assembly.GetType(sarr[1]);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                        }
                        return theObj;

                    }
                    catch 
                    {

                    }
                    return new SimulationNoOneGoodSSO();
			}
		}
		public ICOESSO SSOChoose(string provider)
		{

			switch (provider.ToLower())
			{
				case "simulationeveryonegood":
					return new SimulationEveryoneGoodSSO();

				case "simulationnoonegood":
					return new SimulationNoOneGoodSSO();

				case "coeldap":
					return new COELDAP();

				case "cssecurity":
					return new CSSecurity();
                
				default:

                    try
                    {
                        string s = provider;

                        string[] sarr = s.Split(new char[] { ',' }); ;
                        ICOESSO theObj;
                        if (sarr.Length == 1)
                        {
                            Type theType = Type.GetType(s);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                        }
                        else
                        {
                            Assembly assembly = Assembly.Load(sarr[0]);
                            Type theType = assembly.GetType(sarr[1]);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                        }
                        return theObj; 

                    }
                    catch
                    {

                    }

					return new SimulationNoOneGoodSSO();
			}
		}
		public bool IsValidProvider(string provider)
		{

			switch (provider.ToLower())
			{
				case "simulationeveryonegood":
					return true;

				case "simulationnoonegood":
					return true;

				case "coeldap":
					return true;

				case "cssecurity":
					return true;

				default:

                    try
                    {

                        string s = provider;

                        string[] sarr = s.Split(new char[] { ',' }); ;
                        ICOESSO theObj;
                        if (sarr.Length == 1)
                        {
                            Type theType = Type.GetType(s);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                            return true;
                        }
                        else
                        {
                            Assembly assembly = Assembly.Load(sarr[0]);
                            Type theType = assembly.GetType(sarr[1]);
                            theObj = (ICOESSO)Activator.CreateInstance(theType);
                            return false;
                        }
                       


                    }
                    catch
                    {

                    }
					return false;
			}
		}
	
	}


	
}

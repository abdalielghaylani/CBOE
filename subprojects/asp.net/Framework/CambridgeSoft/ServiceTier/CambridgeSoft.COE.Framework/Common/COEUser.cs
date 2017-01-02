using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CambridgeSoft.COE.Framework.Common
{
    //this class is for convenience since we are not clear where the user is coming from yet.

    [Serializable]
    public  static class COEUser
    {
       
        public static string Get()
        {
            string retVal = "UnknownUser";
            //return the authenticated user's name
            if (Csla.ApplicationContext.User != null && Csla.ApplicationContext.User.Identity != null && !string.IsNullOrEmpty(Csla.ApplicationContext.User.Identity.Name))
            {
                retVal = Csla.ApplicationContext.User.Identity.Name;
            }
            else
            {
                //non-authenticated users may become associated with various aliases:
                if (Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] != null)
                {
                    //the preferred alias is the "TEMPUSERNAME" variable, if a value exists
                    retVal = Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"].ToString();
                }
                else
                {
                    //the second-preferred alias is the DNS name of the remote client, if contextually available
                    if (System.Web.HttpContext.Current != null)
                        // For some reason on my W2008R2 dev system when first lauching
                        // registration the Current.Request object raises an error
                        // later requests do not.  This needs further investigation.
                        try
                        {
                            retVal = System.Web.HttpContext.Current.Request.UserHostName;
                        }
                        catch
                        {
                            retVal = System.Net.Dns.GetHostName();
                        }
                    else
                        retVal = System.Net.Dns.GetHostName();
                }
            }
            return retVal;
        }

        public static string GetToken()
        {
            return Csla.ApplicationContext.User.Identity.Name;
        }


        public static int ID
        {
            get
            {
                if (Csla.ApplicationContext.GlobalContext["USER_PERSONID"] == null)
                    return ((CambridgeSoft.COE.Framework.COESecurityService.COEIdentity)Csla.ApplicationContext.User.Identity).ID;

                return System.Convert.ToInt16(Csla.ApplicationContext.GlobalContext["USER_PERSONID"].ToString());
            }
           
        }


        public static string Name
        {
            get
            {
              
                return Get();
            }
           
        }

        public static int SessionID
        {
            get
            {
                if (Csla.ApplicationContext.GlobalContext["COESESSIONID"] != null){
                    return System.Convert.ToInt16(Csla.ApplicationContext.GlobalContext["COESESSIONID"].ToString());
                }
                else
                {
                    return -1;
                }
                
            }

        }

        public static string RoleIDs
        {
            get
            {
                if (Csla.ApplicationContext.GlobalContext["USERROLEIDS"] != null)
                {
                    return Csla.ApplicationContext.GlobalContext["USERROLEIDS"].ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

        }

        public static string ClientIdentifier
        {
            get
            {
                if(Csla.ApplicationContext.GlobalContext["COEClientIdentifier"] == null)
                {
                    if (System.Web.HttpContext.Current == null || string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserHostName))
                        return "UnknownClientIdentifier";
                    else
                        return ReverseLookup(System.Web.HttpContext.Current.Request.UserHostAddress);
                }
                
                return Csla.ApplicationContext.GlobalContext["COEClientIdentifier"].ToString();

            }

        }

        /// <summary>
        /// Gets the host name from ip address both in IPv4 and IPV6 enabled environments
        /// </summary>
        /// <param name="ip">ip address of the request</param>
        /// <returns>returns the DNS host name</returns>
        public static string ReverseLookup(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return ip;

            try
            {
                return Dns.GetHostEntry(ip).HostName;
            }
            catch (SocketException) { return ip; }
        }

        public static string CurrentIP
        {
            get
            {
                if(System.Web.HttpContext.Current == null || string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserHostAddress))
                    return string.Empty;
                else
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
            }
        }

    }
}

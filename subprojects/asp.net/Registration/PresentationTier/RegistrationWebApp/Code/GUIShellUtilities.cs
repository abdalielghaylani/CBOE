using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web.UI;
using System.Diagnostics;
using System.Web;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.GUIShell
{
    public class Utilities
    {
        //#region RegEx Const Strings

        //private const string numbersRexEx = "[0-9]";

        //#endregion

        //#region Public Methods

        //public static string Encode(string str)
        //{
        //    byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str);
        //    return Convert.ToBase64String(encbuff);
        //}

        //public static string Decode(string str)
        //{
        //    byte[] decbuff = Convert.FromBase64String(str);
        //    return System.Text.Encoding.UTF8.GetString(decbuff);
        //}

        ///// <summary>
        ///// Checks if this id is the kind of the ids that the app supports.
        ///// </summary>
        ///// <param name="id">The ID to check</param>
        ///// <returns>True in case of successful, throw a exception in error case.</returns>
        ///// <exception cref="InvalidCompoundIDType"></exception>
        //public static bool IsAValidCompoundIDType(int id)
        //{
        //    bool retval = false;
        //    string compoundID = String.Empty;

        //    //Clean string.
        //    compoundID = CleanString(id.ToString());

        //    //Verify it is the type of IDs that a compund has.
        //    if (Regex.IsMatch(compoundID, numbersRexEx))
        //        retval = true;
        //    else
        //        throw new InvalidCompoundIDType();

        //    return retval;
        //}


        ///// <summary>
        ///// This Method saves messages into the log file (trace.axd)
        ///// </summary>
        ///// <param name="logCategory">Category of the message</param>
        ///// <param name="message">Text to write</param>
        //public static void WriteToLog(GUIShellTypes.LogsCategories logCategory, string message)
        //{
        //    int adminLogCategory = int.Parse(ConfigurationManager.AppSettings[GUIShellTypes.AppSettings.LogCategoryMessages.ToString()].ToString());
        //    if (HttpContext.Current.Trace.IsEnabled)
        //        if (((int)logCategory) >= adminLogCategory)
        //            HttpContext.Current.Trace.Warn(logCategory.ToString(), message);
        //}

        //#endregion

        //#region Private Methods

        ///// <summary>
        ///// Here we can clean the string from malicous characters as SQL Inyection, etc...
        ///// </summary>
        ///// <param name="stringToClean">Dirty string</param>
        ///// <returns>Clean string</returns>
        //private static string CleanString(string stringToClean)
        //{
        //    return stringToClean;
        //}

        //#endregion
    }
}


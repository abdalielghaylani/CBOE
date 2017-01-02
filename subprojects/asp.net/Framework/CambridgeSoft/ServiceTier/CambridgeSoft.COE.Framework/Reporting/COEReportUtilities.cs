using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEReportingService
{
   public static class COEReportUtilities
   {
       #region Methods
       public static string BuildCOEReportTableName(string owner)
        {
            if (!string.IsNullOrEmpty(owner))
                return owner + "." + Resources.COEReportTableName;
            else
                return Resources.COEReportTableName;
        }
       internal static string BuildCOEReportTypeTableName(string owner)
       {
           if (!string.IsNullOrEmpty(owner))
               return owner + "." + Resources.COEReportTableName + "TYPE";
           else
               return Resources.COEReportTableName + "TYPE";
       }

       #endregion
   }
}

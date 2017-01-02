using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Helpers
{
    public static class PickListPickerHelper
    {

        public static int intDomainId = 0;
        public static string strDomainName = string.Empty;

        public static int intUnitId = 0;
        public static string strUnitName = string.Empty;


        public static bool GetPickListDoamainDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ID, DESCRIPTION FROM REGDB.PICKLISTDOMAIN ORDER BY ID");
            DataTable dtRoles = DALHelper.ExecuteQueryForRegDB(query.ToString());
            intDomainId = 0;
            strDomainName = string.Empty;

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                intDomainId = Convert.ToInt32(dtRoles.Rows[0]["ID"]);
                strDomainName = Convert.ToString(dtRoles.Rows[0]["DESCRIPTION"]);
                return true;
            }
            return false;
        }

         public static bool GetVW_UnitDetails()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ID, UNIT FROM REGDB.VW_Unit ORDER BY ID");
            DataTable dtRoles = DALHelper.ExecuteQueryForRegDB(query.ToString());
            intUnitId = 0;
            strUnitName = string.Empty;

            if (dtRoles != null && dtRoles.Rows.Count > 0)
            {
                intUnitId = Convert.ToInt32(dtRoles.Rows[0]["ID"]);
                strUnitName = Convert.ToString(dtRoles.Rows[0]["UNIT"]);
                return true;
            }
            return false;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;



namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Base class for accessing PickList and PickListDomain tables.
    /// </summary>
    public class DAL : DALBase

    {
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEPickListPicker");

        internal virtual Csla.Data.SafeDataReader GetPickListNameValueList(int domainId) {
            return null;
        }

        internal virtual Csla.Data.SafeDataReader GetPickListNameValueList(string sql)
        {
            return null;
        }

        internal virtual int GetDomainPickListId(string domainPickListName) {
            return -1;
        }

        internal virtual Csla.Data.SafeDataReader GetAllPickListDomains()
        {
            return null;
        }

        internal virtual Csla.Data.SafeDataReader GetPicklistDomain(int domainId)
        {
            return null;
        }

        internal virtual bool ValidateSqlColumn(string table, string column)
        {
            return false;
        }

    }
}
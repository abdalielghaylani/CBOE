using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Framework.COEGenericObjectStorageService
{
    /// <summary>
    /// Utilitarian class for COEGenericObjectStorage service.
    /// </summary>
    public static class COEGenericObjectStorageUtilities
    {
        /// <summary>
        /// Builds the sql string for creating the generic object storage table.
        /// </summary>
        /// <param name="owner">The schema owner</param>
        /// <param name="tableName">The desired name</param>
        public static void BuildCOEGenericObjectStorageTableName(string owner, ref string tableName)
        {
            if (owner.Length > 0)
            {
                tableName = owner + "." + Resources.COEGenericObjectStorageTableName;
            }
            else
            {
                tableName = Resources.COEGenericObjectStorageTableName;
            }
        }
    }
}

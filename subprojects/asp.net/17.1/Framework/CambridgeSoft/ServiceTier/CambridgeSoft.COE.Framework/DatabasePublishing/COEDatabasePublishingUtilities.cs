using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    public static class COEDatabasePublishingUtilities
    {
        public static void BuildTableName(string owner, ref string tableName)
        {
            if (owner.Length > 0)
            {
                tableName = owner + "." + Resources.COESchemaTableName;
            }
            else
            {
                tableName = Resources.COESchemaTableName;
            }
        }
    }
}

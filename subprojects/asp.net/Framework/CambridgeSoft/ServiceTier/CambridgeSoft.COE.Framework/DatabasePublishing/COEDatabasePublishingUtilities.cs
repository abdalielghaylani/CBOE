using CambridgeSoft.COE.Framework.Properties;

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

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COEHitListService
{
    public static class HitListUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableName"></param>
        public static void BuildTempHitListFullTableName(string owner,ref string  tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COETempHitListTableName;
                }
                else
                {
                    tableName = Resources.COETempHitListTableName;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableName"></param>
        public static  void BuildTempHitListIDFullTableName(string owner,ref string  tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COETempHitListIDTableName;
                }
                else
                {
                    tableName = Resources.COETempHitListIDTableName;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableName"></param>
        public static void BuildSavedHitListFullTableName(string owner, ref string tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COESavedHitListTableName;
                }
                else
                {
                    tableName = Resources.COESavedHitListTableName;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableName"></param>
        public static void BuildSavedHitListIDFullTableName(string owner, ref string tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COESavedHitListIDTableName;
                }
                else
                {
                    tableName = Resources.COESavedHitListIDTableName;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Get HITLIST table name
        /// </summary>
        /// <param name="type">temp or saved</param>
        /// <returns>table name</returns>
        public static string GetHitListFullTableName(HitListType type)
        {
            string hitlistFullTableName = string.Empty;

            if (type == HitListType.TEMP)
            {
                BuildTempHitListFullTableName(Resources.CentralizedStorageDB, ref hitlistFullTableName);
            }
            else
            {
                BuildSavedHitListFullTableName(Resources.CentralizedStorageDB, ref hitlistFullTableName);
            }

            return hitlistFullTableName;
        }

        /// <summary>
        /// Get child hit list sequence name
        /// </summary>
        /// <param name="owner">Oracle database name</param>
        /// <returns>full sequence name</returns>
        public static string GetChildHitListIDSequenceName(string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                return Resources.COEChildHitListIdSequence;
            }

            return owner + "." + Resources.COEChildHitListIdSequence;
        }

        /// <summary>
        /// Get TEMPCHITLISTID table name
        /// </summary>
        /// <param name="owner">Oracle database name</param>
        /// <returns>table name</returns>
        public static string GetTempChildHitListIdTableName(string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                return Resources.COETempChildHitListIdTableName;
            }

            return owner + "." + Resources.COETempChildHitListIdTableName;
        }

        /// <summary>
        /// Get TEMPCHITLIST table name
        /// </summary>
        /// <param name="owner">Oracle database name</param>
        /// <returns>table name</returns>
        public static string GetTempChildHitListTableName(string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                return Resources.COETempChildHitListTableName;
            }

            return owner + "." + Resources.COETempChildHitListTableName;
        }
    }
}

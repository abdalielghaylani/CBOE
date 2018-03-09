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
        public static void BuildTempHitListTableName(string owner,ref string  tableName)
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
        public static  void BuildTempHitListIDTableName(string owner,ref string  tableName)
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
        public static void BuildSavedHitListTableName(string owner, ref string tableName)
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
        public static void BuildSavedHitListIDTableName(string owner, ref string tableName)
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
        
    }
}

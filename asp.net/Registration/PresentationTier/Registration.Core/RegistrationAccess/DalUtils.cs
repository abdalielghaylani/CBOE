using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using System.Collections;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

namespace CambridgeSoft.COE.Registration.Access
{
    public class DalUtils
    {
        #region [ DAL helpers ]

        private static DALFactory _dalFactory;

        /// <summary>
        /// Retrieves an instance of te core Registration DAL class
        /// </summary>
        /// <param name="dalBase">the 'out' parameter that will be populated</param>
        /// <param name="serviceName">the calling assembly's key (name)</param>
        public static void GetRegistrationDAL(
            ref RegistrationOracleDAL dalBase, string serviceName
            )
        {
            if (string.IsNullOrEmpty(serviceName)) serviceName = Constants.SERVICENAME;
            string databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get());
            if (_dalFactory == null)
                _dalFactory = new DALFactory();

            string typeName = typeof(RegistrationOracleDAL).FullName;
            _dalFactory.GetDAL<RegistrationOracleDAL>(
                ref dalBase, serviceName, databaseName, true, typeName);
        }

        #endregion

        #region [ Picklists ]

        private const string PICKLIST_CACHE_MANAGER_NAME = "Picklist Cache Manager";
        private const string CACHEKEY_TEMPLATE = "{0}.{1}";

        /// <summary>
        /// This method can be used to force an on-demand fetch of picklist data from the repository
        /// the next time it is requested by clearing this specific cache.
        /// </summary>
        public static void InvalidateCachedPicklists()
        {
            /* TODO: Determine how cache-clearing should be triggered
             * While some of the picklists are created as custom tables (table editor),
             * for which we could provide a 'listener', others are defined from other
             * applciations, such as "Manage Users" being part of the 'Manager' app.
            */
            CacheManager picklistCacheManager =
                CacheFactory.GetCacheManager(PICKLIST_CACHE_MANAGER_NAME);
            if (picklistCacheManager != null)
                picklistCacheManager.Flush();
        }

        /// <summary>
        /// Normalizes the picklistCode to its case-insensitive equivalent.
        /// </summary>
        public static string NormalizePicklistCode(string picklistCode)
        {
            if (string.IsNullOrEmpty(picklistCode))
                throw new ArgumentNullException("picklistCode");

            string normalizedPicklistCode = string.Empty;
            PickListNameValueList allPicklistDomains = PickListNameValueList.GetAllPickListDomains(Constants.REGDB);

            foreach (DictionaryEntry de in allPicklistDomains.KeyValueList)
            {
                if (string.Compare(de.Key.ToString(), picklistCode, true) == 0)
                {
                    normalizedPicklistCode = de.Key.ToString().ToUpper();
                    break;
                }
                if (string.Compare(de.Value.ToString(), picklistCode, true) == 0)
                {
                    normalizedPicklistCode = de.Value.ToString().ToUpper();
                    break;
                }
            }

            return normalizedPicklistCode;
        }

        /// <summary>
        /// Retrieves a Picklist by its code, from underlying cache.
        /// </summary>
        /// <remarks>
        /// The 'code' can either be the picklistdomain's ID or Description property. When using
        /// the Description property, it is used in a case-insensitive manner for ease of use.
        /// </remarks>
        /// <param name="picklistCode">The code of the Picklist to retrieve</param>
        /// <returns>The retrieved Picklist</returns>
        public static Picklist GetPicklistByCode(string picklistCode)
        {
            if (string.IsNullOrEmpty(picklistCode))
                throw new ArgumentNullException("picklistCode");

            CacheManager picklistCacheManager = CacheFactory.GetCacheManager(PICKLIST_CACHE_MANAGER_NAME);
            if (picklistCacheManager == null)
                return null;

            Picklist picklist = null;
            object cachedObject = picklistCacheManager.GetData(
                string.Format(CACHEKEY_TEMPLATE, picklistCode.ToUpper(), COEUser.Name));

            if (cachedObject != null)
                picklist = cachedObject as Picklist;
            else
            {
                string normalizedPicklistCode = NormalizePicklistCode(picklistCode);
                if (normalizedPicklistCode == string.Empty)
                    throw new ArgumentException("Invalid picklistCode");

                int picklistId = -1;
                if (int.TryParse(picklistCode, out picklistId))
                    picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(picklistId));
                else
                    picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(normalizedPicklistCode));

                picklistCacheManager.Add(
                    string.Format(CACHEKEY_TEMPLATE, normalizedPicklistCode, COEUser.Name), picklist);
            }

            return picklist;
        }

        #endregion

    }
}

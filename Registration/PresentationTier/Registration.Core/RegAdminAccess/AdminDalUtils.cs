using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.RegistrationAdmin.Access
{
    class AdminDalUtils
    {
        #region [ DAL helpers ]

        private static DALFactory _dalFactory;

        /// <summary>
        /// Retrieves an instance of the core RegAdmin DAL class
        /// </summary>
        /// <param name="dalBase">the 'out' parameter that will be populated</param>
        /// <param name="serviceName">the calling assembly's key (name)</param>
        public static void GetRegDal(
            ref RegAdminOracleDAL dalBase, string serviceName
            )
        {
            if (string.IsNullOrEmpty(serviceName)) serviceName = Constants.ADMINSERVICENAME;
            string databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get());
            if (_dalFactory == null)
                _dalFactory = new DALFactory();

            string typeName = typeof(RegAdminOracleDAL).FullName;
            _dalFactory.GetDAL<RegAdminOracleDAL>(
                ref dalBase, serviceName, databaseName, true, typeName);
        }

        #endregion
    }
}

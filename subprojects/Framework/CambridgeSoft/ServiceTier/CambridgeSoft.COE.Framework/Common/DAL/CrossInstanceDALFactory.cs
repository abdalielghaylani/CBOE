// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrossInstanceDALFactory.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.Common
{
    using System;
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.Properties;

    /// <summary>
    /// Cross instance DAL factory.
    /// </summary>
    public class CrossInstanceDALFactory
    {
        /// <summary>
        /// Gets the instance global user database connection object.
        /// </summary>
        /// <typeparam name="T">The database connection type.</typeparam>
        /// <param name="instanceName">The database instance name.</param>
        /// <param name="serviceName">The BO service name.</param>
        /// <returns>
        /// The global user database connection on the instance will be returned.
        /// </returns>
        public static T GetInstanceGlobalDAL<T>(string instanceName, string serviceName) where T : DALBase
        {
            T instanceDal = default(T);

            DALFactory dalFactory = new DALFactory();
            var instanceData = ConfigurationUtilities.GetInstanceData(instanceName);
            if (instanceData == null)
            {
                throw new ApplicationException(string.Format("Data source name '{0}' is not found", instanceName));
            }

            var instanceGlobalDbName = instanceData.DatabaseGlobalUser;
            if (!instanceData.IsCBOEInstance)
            {
                instanceGlobalDbName = instanceData.InstanceName + "." + instanceData.DatabaseGlobalUser;
            }

            dalFactory.GetDAL<T>(ref instanceDal, serviceName, instanceGlobalDbName, true);

            return instanceDal;
        }

        /// <summary>
        /// Gets the COEDB connection DAL
        /// </summary>
        /// <typeparam name="T">The database connection type.</typeparam>
        /// <param name="serviceName">The BO service name.</param>
        /// <returns>
        /// The COEDB user database connection will be returned.
        /// </returns>
        public static T GetCOEDBDAL<T>(string serviceName) where T : DALBase
        {
            T coeDAL = default(T);
            DALFactory dalFactory = new DALFactory();
            dalFactory.GetDAL<T>(ref coeDAL, serviceName, Resources.CentralizedStorageDB, true);

            return coeDAL;
        }
    }
}

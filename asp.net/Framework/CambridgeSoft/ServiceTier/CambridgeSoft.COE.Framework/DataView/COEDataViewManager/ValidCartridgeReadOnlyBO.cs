// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidCartridgesReadOnlyBO.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CambridgeSoft.COE.Framework.COELoggingService;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Properties;

    using Csla;
    using Csla.Data;

    /// <summary>
    /// The readonly business object of Valid Cartridges
    /// </summary>
    public class ValidCartridgeReadOnlyBO : ReadOnlyBase<ValidCartridgeReadOnlyBO>
    {
        #region member variables

        private List<Cartridge> cartridgeList = new List<Cartridge>();

        private const string ServiceName = "COEDataView";

        [NonSerialized]
        private DAL coeDAL;

        [NonSerialized]
        private DALFactory dalFactory;

        [NonSerialized]
        private static COELog coeLog = COELog.GetSingleton("COEDataView");

        #endregion

        #region properties

        public bool ExistValidCartridgeTable { get; private set; }

        #endregion

        #region ReadOnlyBase members

        protected override object GetIdValue()
        {
            return -1;
        }

        #endregion

        #region database access

        private void DataPortal_Fetch()
        {
            coeLog.LogStart("Fetching valid cartridge in ValidCartridgeReadOnlyBO", 1);

            if (coeDAL == null)
            {
                LoadDAL();
            }

            if (coeDAL != null)
            {
                try
                {
                    using (var dr = coeDAL.GetValidCartridge())
                    {
                        FetchObject(dr);
                    }
                }
                catch (Exception)
                {
                    this.ExistValidCartridgeTable = false;
                }
            }
            else
            {
                throw new System.Security.SecurityException(
                    string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
        }

        private void FetchObject(SafeDataReader dr)
        {
            while (dr.Read())
            {
                var cartridge = new Cartridge
                                    {
                                        ID = dr.GetInt16("CARTRIDGE_ID"),
                                        Name = dr.GetString("CARTRIDGE_NAME"),
                                        Schema = dr.GetString("CARTRIDGE_SCHEMA")
                                    };

                this.cartridgeList.Add(cartridge);
            }

            this.ExistValidCartridgeTable = true;
        }

        private void LoadDAL()
        {
            if (this.dalFactory == null)
            {
                this.dalFactory = new DALFactory();
            }

            this.dalFactory.GetDAL(ref coeDAL, ServiceName, COEDatabaseName.Get(), true);
        }

        #endregion

        #region cartridge check method

        /// <summary>
        /// Get the readonly business object of Valid Cartridges
        /// </summary>
        /// <returns></returns>
        public static ValidCartridgeReadOnlyBO Get()
        {
            return DataPortal.Fetch<ValidCartridgeReadOnlyBO>();
        }

        /// <summary>
        /// Check valid cartridge by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsCartridgeValid(int id)
        {
            return this.cartridgeList.Any(cartridge => cartridge.ID == id);
        }

        /// <summary>
        /// Check valid cartridge by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsCartridgeValid(string name)
        {
            return this.cartridgeList.Any(cartridge => cartridge.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region internal class

        [Serializable]
        private class Cartridge
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public string Schema { get; set; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Csla;

using CambridgeSoft.COE.Framework.Controls.WebParts;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.RegSystem
{
    /// <summary>
    /// Base Registration CustomItem class to be used by those items that will be shown in the dashboard.
    /// </summary>
    public class BaseRegCustomItem : CustomItem
    {
        #region Variables

        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL _coeRegistrationDAL = null;

        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();

        #endregion

        #region DALLoader

        /// <summary>
        /// Load the DAL
        /// </summary>
        internal void LoadDAL(ConnStringType connStringType)
        {
            string typeName = typeof(RegistrationOracleDAL).FullName;
            string serviceName = null;
            string databaseName = null;
            
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            switch (connStringType)
            {
                case ConnStringType.PROXY:
                    {
                        //logged in user must have credential
                        serviceName = Common.Constants.SERVICENAME;
                        databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                        break;
                    }

                case ConnStringType.OWNER:
                    {
                        serviceName = Constants.SERVICENAME;
                        databaseName = Constants.REGDB;
                        break;
                    }
            }

            _dalFactory.GetDAL<RegistrationOracleDAL>(
                ref _coeRegistrationDAL, serviceName, databaseName, true, typeName
            );

        }

        #endregion

        #region Properties

        /// <summary>
        /// Reqruied DAL for database calls.
        /// </summary>
        internal RegistrationOracleDAL COERegDALProxy
        {
            get
            {
                this.LoadDAL(ConnStringType.PROXY);
                return _coeRegistrationDAL;
            }
        }

        internal RegistrationOracleDAL COERegDALOwner
        {
            get
            {
                this.LoadDAL(ConnStringType.OWNER);
                return _coeRegistrationDAL;
            }
        }

        /// <summary>
        /// style attribute to apply to the text
        /// </summary>
        internal string StyleForText
        {
            get 
            { 
                return base.configData.Get("styleForText") != null ? base.configData.Get("styleForText").Value : string.Empty;
            }
        }

        /// <summary>
        /// Formatted text to display
        /// </summary>
        internal string FormattedText
        {
            get 
            { 
                return base.configData.Get("text") != null ? base.configData.Get("text").Value : string.Empty;
            }
        }

        /// <summary>
        /// CSSClass to apply to the text
        /// </summary>
        internal string CSSClassForText
        {
            get
            {
                return base.configData.Get("cssForText") != null ? base.configData.Get("cssForText").Value : string.Empty;
            }
        }


        #endregion

        #region CustomItem required
        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return "BaseRegCustomItem";
        }
        #endregion

        #region Methods

        /// <summary>
        /// Sets the text (firt it formats the text) to display into the dashboard
        /// </summary>
        /// <param name="text">Text to be formatted</param>
        /// <returns>Full string that will be shown into the dashboard</returns>
        internal string SetText(string text)
        {
            string retVal = "<p";
            if (!string.IsNullOrEmpty(this.StyleForText))
                retVal += " style='" + this.StyleForText + "'";
            if (!string.IsNullOrEmpty(this.CSSClassForText))
                retVal += " class='" + this.CSSClassForText + "'";
            retVal += ">" + string.Format(this.FormattedText, text);
            retVal += "</p>";
            return retVal;
        }

        #endregion
    }

    /// <summary>
    /// Returns the number of registered registries for the dashaboard.
    /// </summary>
    public class GetPermRegistriesCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return this.SetText(this.COERegDALProxy.GetPermRegistriesCount().ToString());
        }

        #endregion
    }

    public class GetApprovedRegistriesCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            int waitingToApprove = this.COERegDALProxy.GetPermRegistriesCount() - this.COERegDALProxy.GetApprovedRegistriesCount();
            return this.SetText(waitingToApprove.ToString());
        }

        #endregion
    }

    /// <summary>
    /// Get the number of temporary registries in the Db for the dashboard.
    /// </summary>
    public class GetTempRegistriesCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return this.SetText(this.COERegDALProxy.GetSubmittedTempRegistriesCount().ToString());
        }

        #endregion
    }

    /// <summary>
    /// Get the number of approved temporary registries in the Db for the dashboard.
    /// </summary>
    public class GetApprovedTempRegistriesCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return this.SetText(this.COERegDALProxy.GetApprovedTempRegistriesCount().ToString());
        }

        #endregion
    }

    /// <summary>
    /// Get the number of temporary registries in the Db for the dashboard.
    /// </summary>
    public class GetAllTempRegistriesCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return this.SetText(this.COERegDALOwner.GetTempRegistriesCount().ToString());
        }

        #endregion
    }

    /// <summary>
    /// Get the number of duplicates components in the Db for the dashboard.
    /// </summary>
    public class GetDuplicateComponentsCount : BaseRegCustomItem
    {
        #region Methods

        /// <summary>
        /// Return a string to be shown in the item
        /// </summary>
        /// <returns>String to be shown</returns>
        public override string GetCustomItem()
        {
            return this.SetText(this.COERegDALOwner.GetDuplicateComponentsCount().ToString());
        }

        #endregion
    }

    
}

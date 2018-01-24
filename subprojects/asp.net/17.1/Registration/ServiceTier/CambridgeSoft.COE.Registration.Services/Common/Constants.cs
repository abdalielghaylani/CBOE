using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.Services.Common
{
    /// <summary>
    /// Class to contain shared Registration constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// For Property objects, the expected string format for a DateTime
        /// </summary>
        public const string DATE_FORMAT = "yyyy/MM/dd hh:mm:ss";

        /// <summary>
        /// For Property objects, the expected culture against which dates
        /// and numeric values should be validated.
        /// </summary>
        public const string DEFAULT_CULTURE = "en-US";

        /// <summary>
        /// The service name for this API
        /// </summary>
        public const string SERVICENAME = "COERegistration";

        /// <summary>
        /// The repository name for COE Registration
        /// </summary>
        public const string REGDB = "REGDB";

    }
}

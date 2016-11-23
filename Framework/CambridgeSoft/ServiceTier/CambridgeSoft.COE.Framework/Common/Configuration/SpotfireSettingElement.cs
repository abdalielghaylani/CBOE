// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpotfireSettingElement.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// the ConfigurationElement for spotfire setting
    /// </summary>
    public class SpotfireSettingElement : ConfigurationElement
    {
        /// <summary>
        /// the const value of the setting in xml
        /// </summary>
        public const string ElementName = "SpotfireSetting";

        /// <summary>
        /// Gets or sets the Url
        /// </summary>
        [ConfigurationProperty("Url", IsRequired = true, IsKey = true)]
        public string Url
        {
            get
            {
                return (string)this["Url"];
            }

            set
            {
                this["Url"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the User
        /// </summary>
        [ConfigurationProperty("User")]
        public string User
        {
            get
            {
                return (string)this["User"];
            }

            set
            {
                this["User"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [ConfigurationProperty("Password")]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }

            set
            {
                this["Password"] = value;
            }
        }
    }
}
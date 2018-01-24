using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.Configuration
{
    public class SpotfireSettingElement : ConfigurationElement
    {
        public const string ElementName = "SpotfireSetting";

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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common {
    public class NameValueConfigurationElement : COENamedConfigurationElement {
        private const string _value = "value";

        public NameValueConfigurationElement() {
        }

        [ConfigurationProperty(_value, IsRequired = true)]
        public string Value {
            get {
                return (string) base[_value];
            }
            set {
                base[_value] = value;
            }
        }
    }
}

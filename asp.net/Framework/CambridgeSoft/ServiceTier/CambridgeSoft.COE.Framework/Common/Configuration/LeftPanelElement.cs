using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    public class LeftPanelElement : COEConfigurationElement
    {
        private const string visible = "visible";
        [ConfigurationProperty(visible)]
        public string Visible
        {
            get { return (string) base[visible]; }
            set { base[visible] = value; }
        }

        private const string enabled = "enabled";
        [ConfigurationProperty(enabled)]
        public string Enabled
        {
            get { return (string) base[enabled]; }
            set { base[enabled] = value; }
        }
    }
}

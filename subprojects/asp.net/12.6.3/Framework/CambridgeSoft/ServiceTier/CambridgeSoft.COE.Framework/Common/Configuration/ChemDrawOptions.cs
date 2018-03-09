using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Common
{
    public class ChemDrawOptions : COEConfigurationElement
    {

        private const string chemDrawPluginPolicy = "chemDrawPluginPolicy";
        [ConfigurationProperty(chemDrawPluginPolicy)]
        public ChemDrawPolicy ChemDrawPluginPolicy
        {
            get { return (ChemDrawPolicy) base[chemDrawPluginPolicy]; }
            set { base[chemDrawPluginPolicy] = value; }
        }

        [DefaultValue(Detect)]
        public enum ChemDrawPolicy
        {
            Detect,
            Available,
            Unavailable
        }
    }
}

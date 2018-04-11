using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Common
{
    public class LeftPanelOptions : COEConfigurationElement
    {
        private const string searchPreferences = "searchPreferences";
        [ConfigurationProperty(searchPreferences)]
        public SearchPreferences SearchPreferences
        {
            get { return (SearchPreferences) base[searchPreferences]; }
            set { base[searchPreferences] = value; }
        }

        private const string queryManagement = "queryManagement";
        [ConfigurationProperty(queryManagement)]
        public QueryManagement QueryManagement
        {
            get { return (QueryManagement) base[queryManagement]; }
            set { base[queryManagement] = value; }
        }

        private const string exportManagement = "exportManagement";
        [ConfigurationProperty(exportManagement)]
        public ExportManagement ExportManagement
        {
            get { return (ExportManagement)base[exportManagement]; }
            set { base[exportManagement] = value; }
        }

        private const string defaultPanelState = "defaultPanelState";
        [ConfigurationProperty(defaultPanelState)]
        [DefaultValue(PanelState.NotSet)]
        public PanelState DefaultPanelState
        {
            get { return (PanelState) base[defaultPanelState]; }
            set { base[defaultPanelState] = value; }
        }

        [DefaultValue(NotSet)]
        public enum PanelState
        {
            NotSet,
            Expanded,
            Collapsed
        }
    }
}

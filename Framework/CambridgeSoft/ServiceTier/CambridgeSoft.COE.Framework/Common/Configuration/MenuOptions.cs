using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    public class MenuOptions : COEConfigurationElement
    {
        private const string restoreMenu = "restoreMenu";
        [ConfigurationProperty(restoreMenu, IsRequired = false)]
        public MenuElement RestoreMenu
        {
            get { return (MenuElement)base[restoreMenu]; }
            set { base[restoreMenu] = value; }
        }

        private const string refineMenu = "refineMenu";
        [ConfigurationProperty(refineMenu, IsRequired = false)]
        public MenuElement RefineMenu
        {
            get { return (MenuElement)base[refineMenu]; }
            set { base[refineMenu] = value; }
        }

        private const string queryMenu = "queryMenu";
        [ConfigurationProperty(queryMenu, IsRequired = false)]
        public MenuElement QueryMenu
        {
            get { return (MenuElement)base[queryMenu]; }
            set { base[queryMenu] = value; }
        }

        private const string markedMenu = "markedMenu";
        [ConfigurationProperty(markedMenu, IsRequired = false)]
        public MenuElement MarkedMenu
        {
            get { return (MenuElement)base[markedMenu]; }
            set { base[markedMenu] = value; }
        }

        private const string exportMenu = "exportMenu";
        [ConfigurationProperty(exportMenu, IsRequired = false)]
        public MenuElement ExportMenu
        {
            get { return (MenuElement)base[exportMenu]; }
            set { base[exportMenu] = value; }
        }

        private const string printMenu = "printMenu";
        [ConfigurationProperty(printMenu, IsRequired = false)]
        public MenuElement PrintMenu
        {
            get { return (MenuElement)base[printMenu]; }
            set { base[printMenu] = value; }
        }

        private const string resultsPerPageMenu = "resultsPerPageMenu";
        [ConfigurationProperty(resultsPerPageMenu, IsRequired = false)]
        public MenuElement ResultsPerPageMenu
        {
            get { return (MenuElement)base[resultsPerPageMenu]; }
            set { base[resultsPerPageMenu] = value; }
        }

        // Added a new menu option Send To on search listing page of ChemBioViz Web
        private const string sendToMenu = "sendToMenu";
        [ConfigurationProperty(sendToMenu, IsRequired = false)]
        public MenuElement SendToMenu
        {
            get { return (MenuElement)base[sendToMenu]; }
            set { base[sendToMenu] = value; }
        }
    }
}

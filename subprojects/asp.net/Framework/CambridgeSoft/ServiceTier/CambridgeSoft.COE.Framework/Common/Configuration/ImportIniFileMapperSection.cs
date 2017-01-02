using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common.Configuration
{
    /// <summary>
    /// This is the object representation of the xml file specifying how the old INI
    /// settings should be mapped into new xml configurations.
    /// </summary>
    public class ImportIniFileMapperSection : COEConfigurationSection
    {
        private const string INI_SECTIONS = "IniSections";

        [ConfigurationProperty(INI_SECTIONS)]
        public IniSectionMapperElementCollection IniSectionMapperElementCollection
        {
            get { return (IniSectionMapperElementCollection)base[INI_SECTIONS]; }
            set { base[INI_SECTIONS] = value; }
        }
    }

    public class IniSectionMapperElementCollection : COEConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IniSectionMapperElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IniSectionMapperElement)element).SectionName;
        }

        public new IniSectionMapperElement this[string sectionName]
        {
            get { return BaseGet(sectionName) as IniSectionMapperElement; }
        }
    }

    public class IniSectionMapperElement: COEConfigurationElement
    {
        private const string SETTINGS = "IniSettings";
        private const string SECTION_NAME = "name";

        [ConfigurationProperty(SECTION_NAME)]
        public string SectionName
        {
            get { return (string)base[SECTION_NAME]; }
            set { base[SECTION_NAME] = value; }
        }

        [ConfigurationProperty(SETTINGS)]
        public IniSettingMapperElementCollection IniSettingsMapperElementCollection
        {
            get { return (IniSettingMapperElementCollection)base[SETTINGS]; }
            set { base[SETTINGS] = value; }
        }
    }

    public class IniSettingMapperElementCollection : COEConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IniSettingMapperElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IniSettingMapperElement)element).IniSettingName;
        }

        public new IniSettingMapperElement this[string iniSettigName]
        {
            get { return BaseGet(iniSettigName) as IniSettingMapperElement; }
        }
    }

    public class IniSettingMapperElement : COEConfigurationElement
    {
        private const string MAPPERS = "mappers";

        private const string TYPE = "type";
        private const string INI_SETTING_NAME = "IniSettingName";
        private const string XML_SETTING_NAME = "XMLSettingName";
        private const string APP_Name = "appName";
        private const string GROUP = "group";

        [ConfigurationProperty(MAPPERS)]
        public MapperElementCollection MapperElementCollection
        {
            get { return (MapperElementCollection)base[MAPPERS]; }
            set { base[MAPPERS] = value; }
        }

        [ConfigurationProperty(TYPE)]
        public string Type
        {
            get { return (string)base[TYPE]; }
            set { base[TYPE] = value; }
        }

        [ConfigurationProperty(INI_SETTING_NAME)]
        public string IniSettingName
        {
            get { return (string)base[INI_SETTING_NAME]; }
            set { base[INI_SETTING_NAME] = value; }
        }

        [ConfigurationProperty(XML_SETTING_NAME)]
        public string XMLSettingName
        {
            get { return (string)base[XML_SETTING_NAME]; }
            set { base[XML_SETTING_NAME] = value; }
        }

        [ConfigurationProperty(APP_Name)]
        public string AppName
        {
            get { return (string)base[APP_Name]; }
            set { base[APP_Name] = value; }
        }

        [ConfigurationProperty(GROUP)]
        public string Group
        {
            get { return (string)base[GROUP]; }
            set { base[GROUP] = value; }
        }
    }

    public class MapperElementCollection : COEConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MapperElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MapperElement)element).IniSettingValue;
        }

        public new MapperElement this[string iniSettingValue]
        {
            get { return BaseGet(iniSettingValue) as MapperElement; }
        }
    }

    public class MapperElement : COEConfigurationElement
    {
        private const string INI_SETTING_VALUE = "IniSettingValue";
        private const string XML_SETTING_VALUE = "XMLSettingValue";

        [ConfigurationProperty(INI_SETTING_VALUE)]
        public string IniSettingValue
        {
            get { return (string)base[INI_SETTING_VALUE]; }
            set { base[INI_SETTING_VALUE] = value; }
        }

        [ConfigurationProperty(XML_SETTING_VALUE)]
        public string XMLSettingValue
        {
            get { return (string)base[XML_SETTING_VALUE]; }
            set { base[XML_SETTING_VALUE] = value; }
        }
    }
}

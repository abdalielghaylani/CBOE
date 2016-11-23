using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Configuration;
namespace ChemBioVizExcelAddIn
{
    public class AppConfigSetting
    {
        private AppConfigSetting()
        { }

        public static string ReadSetting(string key)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public static void WriteSetting(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".config", ""));
                if (!KeyExists(key, config.FilePath))
                {
                    AddKey(key, value);
                }
                else
                {
                    config.AppSettings.Settings[key].Value = value;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveKey(string key)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".config", ""));
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddKey(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".config", ""));
                config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string getConfigFilePath()
        {
            //return Assembly.GetExecutingAssembly().Location + ".config";
            return AppDomain.CurrentDomain.BaseDirectory + "..\\..\\app.config";
        }
        private static bool KeyExists(string strKey, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            //xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");
            // Attempt to locate the requested setting.
            if (appSettingsNode != null)
            {
                foreach (XmlNode childNode in appSettingsNode)
                {
                    if (childNode != null)
                    {
                        //Coverity fix - CID 19226
                        XmlAttribute objXmlAttribute = childNode.Attributes["key"];
                        if (objXmlAttribute != null)
                        {
                            if (objXmlAttribute.Value == strKey)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
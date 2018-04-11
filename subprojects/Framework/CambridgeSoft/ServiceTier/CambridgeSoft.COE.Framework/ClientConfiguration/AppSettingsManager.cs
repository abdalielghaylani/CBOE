using System;
using System.Configuration;
using System.Xml;
namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public class AppSettingsManager
    {
        private AppSettingsManager()
        { }

        public static string Read(string key)
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


        public static void Write(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.ToLower().Replace(".config", ""));
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

        private static void AddKey(string key, string value)
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
            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes != null)
                {
                    if (childNode.Attributes["key"] != null && childNode.Attributes["key"].Value == strKey)
                        return true;
                }
            }
            return false;
        }
    }
}

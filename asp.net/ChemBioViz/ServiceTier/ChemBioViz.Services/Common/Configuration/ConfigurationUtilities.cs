using System;
using System.Data;
using System.Configuration;
using System.Web;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common.Messaging;

/// <summary>
/// Summary description for ConfigurationUtilities
/// </summary>
public class ConfigurationUtilities
{
    #region Variables
    private static ConfigurationUtilities _instance = null;
    private string _configurationSectionName = string.Empty;
    private COESearchRuntimeConfiguration _configurationSection;
    #endregion

    #region Properties
    public string ConfigurationSectionName
    {
        get {
            return _configurationSectionName;
        }
        set {
            if (value == null)
                value = string.Empty;

            if (_configurationSectionName.Trim() != value.Trim())
            {
                _configurationSectionName = value;
                _configurationSection = null;
            }
        }
    }

    public COESearchRuntimeConfiguration ConfigurationSection
    {
        get {
            if (_configurationSection == null)
            {
                COEConfigurationBO theCOEConfigurationBO = COEConfigurationBO.Get(ConfigurationManager.AppSettings["AppName"], _configurationSectionName);
                if (theCOEConfigurationBO != null) //Coverity Fix 
                {
                    if (theCOEConfigurationBO.ConfigurationSection is COESearchRuntimeConfiguration) //Resolve coverity Downcaste exception by using "is" operator
                    {
                        _configurationSection = (COESearchRuntimeConfiguration)theCOEConfigurationBO.ConfigurationSection;
                    }
                }
                if (_configurationSection == null)
                    throw new Exception(string.Format("Configuration section {0} of type {1} not found", _configurationSectionName, typeof(COESearchRuntimeConfiguration).Name));
            }

            return _configurationSection;
        }
    }

    public int FormGroupID
    {
        get
        {
            return ConfigurationSection.FormGroupId;
        }
    }

    public FormGroup.CurrentFormEnum DefaultSearchAction
    {
        get
        {
            return ConfigurationSection.DefaultSearchAction;
        }
    }

    public int BufferSize
    {
        get
        {
            return ConfigurationSection.BufferSize;
        }
    }

    public int GridPageSize
    {
        get
        {
            return ConfigurationSection.GridPageSize;
        }
    }

    public int DefaultQueryForm
    {
        get
        {
            return ConfigurationSection.DefaultQueryForm;
        }
    }

    #endregion

    #region Singleton methods
    public static ConfigurationUtilities GetInstance(string configurationSectionName)
    {
        if (_instance == null || _instance._configurationSectionName.Trim() != configurationSectionName.Trim())
            _instance = new ConfigurationUtilities(configurationSectionName);

        return _instance;
    }

    private ConfigurationUtilities(string configurationSectionName)
    {
        this.ConfigurationSectionName = configurationSectionName;
    }
    #endregion
}

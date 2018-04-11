using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    
    class SettingReaderUtilities
    {
        /// <summary>
        /// Method to check if the all the application settings variables are set to the desired value
        /// </summary>
        /// <param name="appSettingList">The list of AppSetting items to check</param>
        /// <param name="logOperator">the operator to be applied for each item</param>
        /// <returns>boolean value set to true after applying logOperator for each application setting</returns>
        internal static bool MatchedAppSettings(AppSettingList appSettingList, AppSettingList.Operators logOperator)
        {
            bool retVal = logOperator == CambridgeSoft.COE.Framework.COEPageControlSettingsService.AppSettingList.Operators.OR ? false : true;

            if (appSettingList.Count > 0)
            {
                foreach (AppSetting appSettingItem in appSettingList)
                {
                    bool isAppSettingSet = SettingReaderUtilities.IsSettingValueSet(appSettingItem.Key, appSettingItem.Value, appSettingItem.Type);
                    switch (logOperator)
                    {
                        case AppSettingList.Operators.AND:
                            retVal &= isAppSettingSet; break;
                        case AppSettingList.Operators.OR:
                            retVal |= isAppSettingSet; break;
                        default:
                            retVal |= isAppSettingSet; break;
                    }
                }
            }
            else
            {
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Method to check if the application settings variable is set to the specified value on the given type/s
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <param name="value">The variable value to be compared</param>
        /// <param name="type">The source where the variable value will be read</param>
        /// <returns>boolean value set to true if the setting value is set on the highest priority reader</returns>
        internal static bool IsSettingValueSet(string key, string value, string type)
        {
            bool retVal = false;
            // Parse and Instantiate Types
            List<ISettingReader> typesList = SettingReaderUtilities.GetOrderedTypeList(type);
            string currentValue = string.Empty;
            foreach (ISettingReader settingReader in typesList)
            {
                currentValue = settingReader.getData(key);
                // Compare only if the value is not empty
                if (currentValue!=string.Empty)
                    retVal = (currentValue.ToLower() == value.ToLower());
            }
            return retVal;
        }

        private static List<ISettingReader> GetOrderedTypeList(string typeList)
        {
            List<ISettingReader> retVal = new List<ISettingReader>();
            string[] typeListArr = typeList.Split('|');
            // Check if we have to return all the IAppSettingReader classes
            if (typeListArr.Length == 1 && typeListArr[0] == Enum.GetName(typeof(AppSettingTypes), AppSettingTypes.All))
            {
                foreach (string reader in Enum.GetNames(typeof(AppSettingTypes)))
                {
                    if (reader.ToLower() != "all")
                        retVal.Add(GetReader(reader));
                }
            }
            else
            {
                foreach (string appSettingReaderName in typeListArr)
                {
                    try
                    {
                        retVal.Add(GetReader(appSettingReaderName));
                    }
                    catch (Exception ex)
                    {
                           
                    }
                }
            }
            if (retVal.Count > 1)
            {
                    retVal.Sort(delegate(ISettingReader ir1, ISettingReader ir2)
                    {
                      return ir1.Priority.CompareTo(ir2.Priority);
                    });
            }
            return retVal;
        }

        /// <summary>
        /// Method to get an instance of the specified ISettingReader Type
        /// </summary>
        /// <param name="type">The type of reader to instantiate.</param>
        /// <returns>ISettingReader object</returns>
        private static ISettingReader GetReader(string type)
        {
            if (type==Enum.GetName(typeof(AppSettingTypes), AppSettingTypes.Session))
            {
                return new SessionReader();
            }
            else if (type==Enum.GetName(typeof(AppSettingTypes), AppSettingTypes.COEConfiguration))
            {
                return new COEConfigurationReader();
            }
            else if (type == Enum.GetName(typeof(AppSettingTypes), AppSettingTypes.Application))
            {
                return new ApplicationVariableReader();
            }
            else
            {
                return new COEConfigurationReader();
            }
        }

    }
}

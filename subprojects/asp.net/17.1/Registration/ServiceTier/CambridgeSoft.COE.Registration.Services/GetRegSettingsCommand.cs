using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;




namespace CambridgeSoft.COE.Registration.Services
{

    /// <summary>
    /// class to allow clients to get registration configuration settings in 3-tier mode
    /// </summary>
   [Serializable]
    public class GetRegSettingsCommand: RegistrationCommandBase
    {
        string result = string.Empty;
        string _appName = string.Empty;
        string _group = string.Empty;
        string _key = string.Empty;


       /// <summary>
       /// Main method called from client
       /// </summary>
       /// <param name="appName">the application name </param>
       /// <param name="group">the configuration section</param>
       /// <param name="key">the item whose value is desired</param>
       /// <returns>value for item as text</returns>
        public static string Execute(string appName, string group, string key )
        {
            string _result = string.Empty;
            try
            {
                GetRegSettingsCommand cmd = new GetRegSettingsCommand(appName, group, key);
                cmd = DataPortal.Execute<GetRegSettingsCommand>(cmd);
                _result = cmd.result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return _result;
        }

        private GetRegSettingsCommand(string appName, string group, string key)
        {
            _appName = appName;
            _group = group;
            _key = key;
        }

        protected override void DataPortal_Execute()
        {   
            
            try
            {

                result = CambridgeSoft.COE.Registration.Services.Common.RegSvcUtilities.GetSystemSettings(_group, _key);
                    //FrameworkUtils.GetAppConfigSetting(_appName, _group, _key, false);
            }
            catch (Exception ex)
            {
                
                COEExceptionDispatcher.HandleBLLException(ex);
            }


        }
    }

   
}

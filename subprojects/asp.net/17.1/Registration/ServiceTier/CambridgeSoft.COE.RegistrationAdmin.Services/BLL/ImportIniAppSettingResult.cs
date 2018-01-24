using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class ImportIniAppSettingResult : ImportIniResult
    {
        /// <summary>
        /// Constructor, initializing the successful and skipped message template
        /// </summary>
        public ImportIniAppSettingResult()
        {
            base.SuccessMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tSUCCESSFUL\tINI setting '{0}' with value '{1}', was migrated to XML setting '{2}' with value '{3}'";
            base.SkipMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tSKIPPED\tINI setting '{0}' was already migrated and was bypassed";
            base.FailureMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tFAILED\t";
        }

        /// <summary>
        /// The failure message template
        /// </summary>
        public static class ImportIniAppSettingFailureTemplate
        {
            public const string INI_SECTION_NOT_FOUND = "INI section '{0}' is not found from INI file";
            public const string INI_SETTING_NOT_FOUND = "INI setting '{0}' under section '{1}' is not found from INI file";
            public const string INI_SETTING_VALUE_NOT_MATCH = "INI setting '{0}' with value '{1}' doesn't have a matching value specified in the mapper file";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class Consts
    {
        public const string apiVersion = "1.0";

        public const string apiPrefix = "api/v{version:apiVersion}/";

        public const string ssoCookieName = "COESSO";

        public const string COEFORMSFOLDERNAME = "COEForms";

        public const string COEDATAVIEWSFOLDERNAME = "COEDataViews";

        public const string COETABLESFORLDERNAME = "COETables";

        public const string COEOBJECTCONFIGFILENAME = "COEObjectConfig.xml";

        public const string CONFIGSETTINGSFILENAME = "ConfigurationSettings.xml";

        public const string IMPORTFILESPATH = "\\Config\\default";

        public const string EXPORTFILESPATH = "\\COERegistrationExportFiles\\";

        public const string PrivilegeConfigReg = "CONFIG_REG";
    }
}
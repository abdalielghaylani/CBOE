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

        public const string REGDB = "REGDB";

        public const string PRIVILEGEMANAGETABLES = "MANAGE_TABLES";

        public const string CHEMBIOVIZAPLPICATIONNAME = "CHEMBIOVIZ";

        #region COE FORM Index
        public const int MIXTURESUBFORMINDEX = 1000;

        public const int MIXTURESEARCHFORM = 0;

        public const int COMPOUNDSUBFORMINDEX = 1001;

        public const int COMPOUNDSEARCHFORM = 1;

        // id of the coeform in which formelements for Structure level properties reside
        public const int STRUCTURESUBFORMINDEX = 1;

        public const int STRUCTURESEARCHFORM = 4;

        public const int BATCHSUBFORMINDEX = 1002;

        public const int BATCHSEARCHFORM = 2;

        public const int BATCHCOMPONENTSUBFORMINDEX = 1003;

        public const int BATCHCOMPONENTSEARCHFORM = 3;

        public const int TEMPORARYBASEFORM = 0;

        public const int TEMPORARYCHILDFORM = 1;

        #endregion
    }
}
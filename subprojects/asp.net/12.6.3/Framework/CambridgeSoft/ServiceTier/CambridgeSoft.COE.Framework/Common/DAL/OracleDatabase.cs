using Oracle.DataAccess.Client;
using System.Globalization;

namespace CambridgeSoft.COE.Framework.Common
{
    public class OracleDatabase : Microsoft.Practices.EnterpriseLibrary.Data.OracleDataAccessClient.OracleODPDatabase
    {
        public OracleDatabase(string connstring)
            : base(connstring)
        { }

        public override System.Data.Common.DbConnection CreateConnection()
        {
            System.Data.Common.DbConnection result = base.CreateConnection();
            result.StateChange += new System.Data.StateChangeEventHandler(result_StateChange);
            return result;
        }

        void result_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if(e.CurrentState == System.Data.ConnectionState.Open)
                ((OracleConnection) sender).ClientId = COEUser.Name;

            
            // This method allows setting all Oracle NLS values
            // by mapping language and territory information from
            // .NET culture info
                 //setOracleNLSLanguageAndTerritoryFromThreadInfo();
            // The above call may cause problems due to changing
            // of language in Oracle error messages as well to
            // unforseen effects of other NLS parameter changes.
            // Commented out for 12.4 release, but will be introduced
            // in 12.5 release where it can be more carefully tested.


            // This is needed for specifically setting the correct 
            // decimal and group separator in cases where the 
            // language and territory of the current culture cannot
            // be cleanly mapped to corresponding ORACLE NLS values
            // should be here even after languge and territory call 
            // above 
            setOracleNLSNumericCharactersFromThreadInfo();

        }


        /// <summary>
        /// Sets the Oracle NumericCharaters property based on the .NET
        /// current thread culture information (Regional Settings)
        /// </summary>
        private void setOracleNLSNumericCharactersFromThreadInfo()
        {
            // Coverity Fix CID - 11811
            using (OracleGlobalization oraSettings = OracleGlobalization.GetClientInfo())
            {
                NumberFormatInfo nf = CultureInfo.CurrentCulture.NumberFormat;
                oraSettings.NumericCharacters = nf.NumberDecimalSeparator + nf.NumberGroupSeparator;
                OracleGlobalization.SetThreadInfo(oraSettings);
            }
        }
        
        /// <summary>
        /// Sets the Oracle Language and Territory based on the .NET
        /// current thread culture information (CultureID)
        /// </summary>
        private void setOracleNLSLanguageAndTerritoryFromThreadInfo()
        {
            // Coverity Fix CID - 11810
            using (OracleGlobalization oraSettings = OracleGlobalization.GetClientInfo())
            {
                string cultureLanguage = CultureInfo.CurrentCulture.Name.Split(new char[] { '-' })[0];
                oraSettings.Language = getOracleNLSLanguageFromThreadCultureLanguage(cultureLanguage);
                string cultureCountry = CultureInfo.CurrentCulture.Name.Split(new char[] { '-' })[1];
                oraSettings.Territory = getOracleNLSTerritoryFromThreadCultureCountry(cultureCountry);
                OracleGlobalization.SetThreadInfo(oraSettings);                
            }
            
        }

        /// <summary>
        /// Maps the first two characters of the .NET CultureId (eg. en-UK) to a valid
        /// Oracle NLS language value.  If no maping if found it defaults to ENGLISH
        /// </summary>
        /// <param name="cultureLanguage">.NET culture language as two lowercase character code</param>
        /// <returns>Correspoinding Oracle NLS_LANGUAGE value</returns>
        private string getOracleNLSLanguageFromThreadCultureLanguage(string cultureLanguage)
        {
            string NLSLanguage = "ENGLISH";

            switch (cultureLanguage) 
            {
                case "en": NLSLanguage = "ENGLISH"; break;
                case "es": NLSLanguage = "SPANISH"; break;
                case "fr": NLSLanguage = "FRENCH"; break;
                case "de": NLSLanguage = "GERMAN"; break;
                case "it": NLSLanguage = "ITALIAN"; break;
                case "ja": NLSLanguage = "JAPANESE"; break;
                case "nl": NLSLanguage = "DUTCH"; break;
                case "fi": NLSLanguage = "FINNISH"; break;
                case "nb": NLSLanguage = "NORWEGIAN"; break;
                case "pt": NLSLanguage = "PORTUGUESE"; break;
                case "sw": NLSLanguage = "SWEDISH"; break;
                case "be": NLSLanguage = "RUSSIAN"; break;
                case "bg": NLSLanguage = "BULGARIAN"; break;
                case "ca": NLSLanguage = "CATALAN"; break;
                case "zh": NLSLanguage = "SIMPLIFIED CHINESE"; break;
                case "hr": NLSLanguage = "CROATIAN"; break;
                case "cs": NLSLanguage = "CZECH"; break;
                case "da": NLSLanguage = "DANISH"; break;
                case "et": NLSLanguage = "ESTONIAN"; break;
                case "eu": NLSLanguage = "SPANISH"; break;
                case "gl": NLSLanguage = "SPANISH"; break;
                case "el": NLSLanguage = "GREEK"; break;
                case "he": NLSLanguage = "HEBREW"; break;
                case "hi": NLSLanguage = "HINDI"; break;
                case "hu": NLSLanguage = "HUNGARIAN"; break;
                case "is": NLSLanguage = "ICELANDIC"; break;
                case "id": NLSLanguage = "INDONESIAN"; break;
                case "kn": NLSLanguage = "KANNADA"; break;
                case "ko": NLSLanguage = "KOREAN"; break;
                case "lv": NLSLanguage = "LATVIAN"; break;
                case "lt": NLSLanguage = "LITHUANIAN"; break;
                case "mk": NLSLanguage = "MACEDONIAN"; break;
                case "ms": NLSLanguage = "MALAY"; break;
                case "mr": NLSLanguage = "MARATHI"; break;
                case "pl": NLSLanguage = "POLISH"; break;
                case "pa": NLSLanguage = "PUNJABI"; break;
                case "ro": NLSLanguage = "ROMANIAN"; break;
                case "ru": NLSLanguage = "RUSSIAN"; break;
                case "sk": NLSLanguage = "SLOVAK"; break;
                case "sl": NLSLanguage = "SLOVENIAN"; break;
                case "sv": NLSLanguage = "FINNISH"; break;
                case "ta": NLSLanguage = "TAMIL"; break;
                case "te": NLSLanguage = "TELUGU"; break;
                case "th": NLSLanguage = "THAI"; break;
                case "tr": NLSLanguage = "TURKISH"; break;
                case "uk": NLSLanguage = "UKRAINIAN"; break;
            }

            return NLSLanguage;
        }

        /// <summary>
        /// Maps the last two characters of the .NET CultureId (eg. en-UK) to a valid
        /// Oracle NLS territory value.  If no maping if found it defaults to AMERICA
        /// </summary>
        /// <param name="cultureLanguage">.NET country/region as two uppercase character code</param>
        /// <returns>Correspoinding Oracle NLS_TERRIROTY value</returns>
        private string getOracleNLSTerritoryFromThreadCultureCountry(string cultureCountry)
        {
            string NLSTerritory = "AMERICA";

            switch (cultureCountry)
            {
                case "US": NLSTerritory = "AMERICA"; break;
                case "ES": NLSTerritory = "SPAIN"; break;
                case "CH": NLSTerritory = "SWITZERLAND"; break;
                case "FR": NLSTerritory = "FRANCE"; break;
                case "GB": NLSTerritory = "UNITED KINGDOM"; break;
                case "DE": NLSTerritory = "GERMANY"; break;
                case "IT": NLSTerritory = "ITALY"; break;
                case "CA": NLSTerritory = "CANADA"; break;
                case "JP": NLSTerritory = "JAPAN"; break;
                case "NO": NLSTerritory = "NORWAY"; break;
                case "NL": NLSTerritory = "THE NETHERLANDS"; break;
                case "CN": NLSTerritory = "CHINA"; break;
                case "FI": NLSTerritory = "FINLAND"; break;
                case "BG": NLSTerritory = "BULGARIA"; break;
                case "HK": NLSTerritory = "HONG KONG"; break;
                case "CHS": NLSTerritory = "CHINA"; break;
                case "SG": NLSTerritory = "SINGAPORE"; break;
                case "TW": NLSTerritory = "TAIWAN"; break;
                case "CHT": NLSTerritory = "CHINA"; break;
                case "HR": NLSTerritory = "CROATIA"; break;
                case "CZ": NLSTerritory = "CZECH REPUBLIC"; break;
                case "DK": NLSTerritory = "DENMARK"; break;
                case "BE": NLSTerritory = "BELGIUM"; break;
                case "AU": NLSTerritory = "AUSTRALIA"; break;
                case "IE": NLSTerritory = "IRELAND"; break;
                case "NZ": NLSTerritory = "NEW ZEALAND"; break;
                case "PH": NLSTerritory = "PHILIPPINES"; break;
                case "ZA": NLSTerritory = "SOUTH AFRICA"; break;
                case "EE": NLSTerritory = "ESTONIA"; break;
                case "LU": NLSTerritory = "LUXEMBOURG"; break;
                case "AT": NLSTerritory = "AUSTRIA"; break;
                case "GR": NLSTerritory = "GREECE"; break;
                case "IN": NLSTerritory = "INDIA"; break;
                case "IL": NLSTerritory = "ISRAEL"; break;
                case "HU": NLSTerritory = "HUNGARY"; break;
                case "IS": NLSTerritory = "ICELAND"; break;
                case "ID": NLSTerritory = "INDONESIA"; break;
                case "KR": NLSTerritory = "KOREA"; break;
                case "LV": NLSTerritory = "LATVIA"; break;
                case "LT": NLSTerritory = "LITHUANIAMK"; break;
                case "PL": NLSTerritory = "POLAND"; break;
                case "BR": NLSTerritory = "BRAZIL"; break;
                case "PT": NLSTerritory = "PORTUGAL"; break;
                case "RO": NLSTerritory = "ROMANIA"; break;
                case "RU": NLSTerritory = "RUSSIA"; break;
                case "SK": NLSTerritory = "SLOVAKIA"; break;
                case "SI": NLSTerritory = "SLOVENIA"; break;
                case "AR": NLSTerritory = "ARGENTINA"; break;
                case "CL": NLSTerritory = "CHILE"; break;
                case "CO": NLSTerritory = "COLOMBIA"; break;
                case "CR": NLSTerritory = "COSTA RICA"; break;
                case "EC": NLSTerritory = "ECUADOR"; break;
                case "SV": NLSTerritory = "EL SALVADOR"; break;
                case "GT": NLSTerritory = "GUATEMALA"; break;
                case "MX": NLSTerritory = "MEXICO"; break;
                case "NI": NLSTerritory = "NICARAGUA"; break;
                case "PA": NLSTerritory = "PANAMA"; break;
                case "PE": NLSTerritory = "PERU"; break;
                case "PR": NLSTerritory = "PUERTO RICO"; break;
                case "VE": NLSTerritory = "VENEZUELA"; break;
                case "SE": NLSTerritory = "SWEDEN"; break;
                case "SY": NLSTerritory = "SYRIA"; break;
                case "TH": NLSTerritory = "THAILAND"; break;
                case "TR": NLSTerritory = "TURKEY"; break;
                case "UA": NLSTerritory = "UKRAINE"; break;
            }

            return NLSTerritory;
        }




    }
}
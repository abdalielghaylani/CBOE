using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    // CLASS FOR IMPLEMENTING FIX FOR BUG:CBOE-2505
    //MANIPULATED FILES FOR THE FIX : PAGECONTROLSETTINGS.CS,CONTROLSETTING.CS,CONTROL.CS,CHEMBIOVIZSEARCH.aspx
    //NOTE : REFER THE TOSTRING() FUNCTION IN ChemBioVizSearch.aspx.
    public static class ControlIdChangeUtility
    {
        private const string CONTROL_SETTING_ID_SEARCHTEMP = "SearchTempRegistry";
        private const string CONTROL_SETTING_ID_SEARCHPERMANENT = "SearchRegistry";
        private const string APP_NAME = "CHEMBIOVIZ";
        private const COEPageControlSettings.Type CUSTOMTYPE = COEPageControlSettings.Type.Custom;
        private static string _selectedapp;
        private static string _selectedcontrolsettingid;
        private static COEPageControlSettings.Type _selectedtype;
        public const string TEMPSUFFIX = "_temp";
        public const string PERMSUFFIX = "_perm";
        public const string PAGENAME = "ASP.forms_search_contentarea_chembiovizsearch_aspx";
        public const string PERMSEARCHGROUPID = "4003";
        public const string TEMPSEARCHGROUPID = "4002";
        public static COEPageControlSettings.Type SelectedType
        {
            set
            {
                _selectedtype = value;
            }
        }

        public static string SelectedApp
        {
            set
            {
                _selectedapp = value;
            }
        }

        public static string SelectedControlSettingID
        {
            set
            {
                _selectedcontrolsettingid = value;
            }
        }
        /// <summary>
        /// Changes the control Id depending on the specified condition 
        /// </summary>
        /// <param name="controlid">The controlid to change if the condition satisfies </param>
        /// <returns></returns>
       
        public static string ChangeControlID(string controlid)
        {
            string strcontrolid = controlid;
            if (!string.IsNullOrEmpty(_selectedapp))
            {
               //condition to check if the current appname is "CHEMBIOVIZ" and current type is "CUSTOM"
                if (_selectedapp.Equals(APP_NAME) && _selectedtype == CUSTOMTYPE)
                {

                    if (!string.IsNullOrEmpty(_selectedcontrolsettingid))
                    {
                        // suffix _temp or _perm to the id depending on the controlsetting(i:e SearchTempRegistry or SearchRegistry) respectively
                        
                        if (_selectedcontrolsettingid.Equals(CONTROL_SETTING_ID_SEARCHTEMP))
                            strcontrolid = string.Concat(controlid, TEMPSUFFIX);

                        else if (_selectedcontrolsettingid.Equals(CONTROL_SETTING_ID_SEARCHPERMANENT))
                            strcontrolid = string.Concat(controlid, PERMSUFFIX);
                    }

                }
            }
            return strcontrolid;
        }
    }
}

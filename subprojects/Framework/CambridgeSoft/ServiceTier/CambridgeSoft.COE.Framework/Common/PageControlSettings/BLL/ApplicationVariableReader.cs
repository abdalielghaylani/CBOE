using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    class ApplicationVariableReader : ISettingReader
    {
        #region ISettingReader Members

        /// <summary>
        /// Priority 2.-
        /// </summary>
        public int Priority
        {
            get { return 2; }
        }

        public string getData(string variableName)
        {
            string retVal = String.Empty;
            if (variableName != null && variableName != string.Empty)
            {
                if (HttpContext.Current.Application[variableName] != null)
                {
                    retVal = HttpContext.Current.Application[variableName].ToString();
                }
            }
            return retVal;
        }
        #endregion
    }
}

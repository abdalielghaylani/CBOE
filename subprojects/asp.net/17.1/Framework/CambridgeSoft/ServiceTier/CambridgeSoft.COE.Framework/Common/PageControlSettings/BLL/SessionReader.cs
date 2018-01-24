using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CambridgeSoft.COE.Framework.COEPageControlSettingsService
{
    class SessionReader : ISettingReader
    {
        #region Variables
        private int _priority;
        #endregion

        #region Properties
        /// <summary>
        /// Priority of the reader
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }
        #endregion

        #region Constructors
        public SessionReader()
        { 
            // Probably this priority could be configured
            this._priority = 1;
        }
        #endregion

        #region ISettingReader Members
        public string getData(string variableName)
        {
            string retVal = String.Empty;
            if (variableName!=null && variableName!=string.Empty)
            {
                if (HttpContext.Current.Session[variableName] != null)
                {
                    retVal = HttpContext.Current.Session[variableName].ToString();
                }
            }
            return retVal;
        }
        #endregion
    }
}

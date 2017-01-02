using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    /// <summary>
    /// Extends Calculation to include the Python script path as a property
    /// </summary>
    [Serializable]
    public class PythonCalculation: Calculation
    {
        private string _pyScriptPath = string.Empty;
        /// <summary>
        /// For calculations requiring Python engine scripts, the path to that script.
        /// </summary>
        public string PyScriptPath
        {
            get { return _pyScriptPath; }
            set { _pyScriptPath = value; }
        }

        private string _pyScriptContent = string.Empty;
        /// <summary>
        /// For calculations requiring Python engine scripts, the contents of a Python script.
        /// </summary>
        public string PyScriptContent
        {
            get { return _pyScriptContent; }
            set { _pyScriptContent = value; }
        }

        private string _pyScrtipReturnParam = string.Empty;
        /// <summary>
        /// Property to set the python return parameter name to get the value after execution.
        /// </summary>
        public string PyScriptReturnParam
        {
            get { return _pyScrtipReturnParam; }
            set { _pyScrtipReturnParam = value; }

        }

    }
}

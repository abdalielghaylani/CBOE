using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemBioViz.NET.Exceptions
{
    public class CustomSettingsProviderException : Exception
    {
        #region Constructors
        public CustomSettingsProviderException() : base() {}
        public CustomSettingsProviderException(string message) : base(message) { }
        public CustomSettingsProviderException(string message, Exception ex): base(message, ex) {}
        #endregion
    }
}

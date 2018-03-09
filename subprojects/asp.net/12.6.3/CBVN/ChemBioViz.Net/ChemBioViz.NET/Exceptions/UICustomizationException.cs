using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemBioViz.NET.Exceptions
{
    public class UICustomizationException : Exception
    {
        #region Constructors
        public UICustomizationException(): base() { }
        public UICustomizationException(string message) : base(message) { }
        public UICustomizationException(string message, Exception ex): base(message, ex){ }
        #endregion
    }
}

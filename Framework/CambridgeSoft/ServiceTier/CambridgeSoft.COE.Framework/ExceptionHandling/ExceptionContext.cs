using System;
using System.Data.Common;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    /// <remarks>
    /// This class encapsulates the extra information relating to a exception that need to be logged in.
    /// </remarks>
    public class ExceptionContext
    {
        private DbCommand _Command;
        private string _RegistryRecordsXML;

        public DbCommand Command
        {
            get
            {
                return _Command;
            }
            set
            {
                _Command = value;
            }
        }

        public string RegistryRecordsXML
        {
            get
            {
                return this._RegistryRecordsXML;
            }
            set
            {
                this._RegistryRecordsXML = value;
            }
        }
    }
}

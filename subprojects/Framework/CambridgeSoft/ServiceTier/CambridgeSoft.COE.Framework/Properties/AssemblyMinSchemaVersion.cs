using System;
using System.Runtime.InteropServices;

namespace CambridgeSoft.COE.Framework
{
    // Summary:
    //     Specifies the the minium coedb schema version that the current frameowrk requires
    [ComVisible(true)]
    [AttributeUsage(System.AttributeTargets.All, Inherited = false)]
    public sealed class AssemblyMinSchemaVersion : Attribute
    {
        private Version _version;
        public AssemblyMinSchemaVersion(string version)
        {
            _version = new Version(version);
        }



        public Version Version
        {
            get
            {
                return _version;
            }
        }
    }
}

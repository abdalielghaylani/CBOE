using System;
using System.Runtime.InteropServices;

namespace CambridgeSoft.COE.Framework
{
    // Summary:
    //     Specifies the the minium framework version that the is required on clients running 3 tier
    
    [ComVisible(true)]
    [AttributeUsage(System.AttributeTargets.All, Inherited = false)]
    public sealed class AssemblyMinClientVersion : Attribute
    {
        private Version _version;
        public AssemblyMinClientVersion(string version)
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

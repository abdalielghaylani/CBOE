using System;
using System.Runtime.Serialization;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COESecurityService
{
    /// <summary>
    /// Exception raised for empties registry records
    /// </summary>
    [Serializable]
    public class IncompatibleVersions : Exception
    {
        public IncompatibleVersions()  { }
        public IncompatibleVersions(string message) : base(message.ToString()) { }
        public IncompatibleVersions(string versionType, Version currentVersion, Version minimumVersion)
        {
            switch (versionType)
	        {
	            case "SCHEMA":
                    throw new IncompatibleVersions(string.Format(Resources.IncompatibleVersionsSchema,currentVersion, minimumVersion));
                    break;
                case "FRAMEWORK":
                    throw new IncompatibleVersions(string.Format(Resources.IncompatibleVersionsFramework,currentVersion, minimumVersion));
                    break;
	         }
        }


    }
}

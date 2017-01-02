using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{   [Serializable]
    public class VersionInfo
    {
        private Version _serverFrameworkVersion = null;
        private Version _minRequiredClientFrameworkVersion = null;
        private Version _minOracleSchemaVersion = null;
        private Version _serverOracleSchemaVersion = null;
        private Version _clientFrameworkVersion = null;

        public VersionInfo(Version serverFrameworkVersion, Version minRequiredClientFrameworkVersion, Version minOracleSchemaVersion, Version serverOracleSchemaVersion)
        {
            _serverFrameworkVersion = serverFrameworkVersion;
            _minRequiredClientFrameworkVersion = minRequiredClientFrameworkVersion;
            _minOracleSchemaVersion = minOracleSchemaVersion;
            _serverOracleSchemaVersion = serverOracleSchemaVersion;

        }

        public VersionInfo(Version serverFrameworkVersion, Version minRequiredClientFrameworkVersion, Version minOracleSchemaVersion, Version serverOracleSchemaVersion, Version clientFrameworkVersion)
        {
            _serverFrameworkVersion = serverFrameworkVersion;
            _minRequiredClientFrameworkVersion = minRequiredClientFrameworkVersion;
            _minOracleSchemaVersion = minOracleSchemaVersion;
            _serverOracleSchemaVersion = serverOracleSchemaVersion;
            _clientFrameworkVersion = clientFrameworkVersion;

        }


        public Version ServerFrameworkVersion
        {
            get { return _serverFrameworkVersion; }
            set { _serverFrameworkVersion = value; }
        }

        public Version MinRequiredClientFrameworkVersion
        {
            get { return _minRequiredClientFrameworkVersion; }
            set { _minRequiredClientFrameworkVersion = value; }
        }

        public Version MinOracleSchemaVersion
        {
            get { return _minOracleSchemaVersion; }
            set { _minOracleSchemaVersion = value; }
        }

        public Version ServerOracleSchemaVersion
        {
            get { return _serverOracleSchemaVersion; }
            set { _serverOracleSchemaVersion = value; }
        }

        public Version ClientFrameworkVersion
        {
            get { return _clientFrameworkVersion; }
            set { _clientFrameworkVersion = value; }
        }


    }

}

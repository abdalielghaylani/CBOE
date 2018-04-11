using System;
using System.Collections.Generic;
using System.Text;


namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    public class DBMSInfo
    {
        private DBMSType dBMSType;
        private string paramMarker;

        public DBMSType DBMSType
        {
            get { return this.dBMSType; }
            set { this.dBMSType = value; }
        }

        public string ParamMarker
        {
            get { return this.paramMarker; }
            set { this.paramMarker = value; }
        }
	
    }
}

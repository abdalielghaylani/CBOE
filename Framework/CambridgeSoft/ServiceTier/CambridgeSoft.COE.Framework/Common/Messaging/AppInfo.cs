using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    public class AppInfo
    {
        private string appName;

        [XmlElement(ElementName = "AppName", IsNullable = false, DataType = "string")]
        public string AppName
        {
            get { return this.appName; }
            set { this.appName = value; }
        }


        public void GetFromXML(XmlDocument xmlRequest)
        {

        }
    }
}

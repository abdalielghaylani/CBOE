using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace CambridgeSoft.COE.Framework.COEFormService
{
    public static class COEFormGroupUtilities
    {
        
        public static string BuildCOEFormTableName(string owner)
        {
            if (!string.IsNullOrEmpty(owner))
                return  owner + "." + Resources.COEFormTableName;
            else
            {
                return Resources.COEFormTableName;
            }
        }

        public static string BuildCOEFormTypeTableName(string owner) {
            if(!string.IsNullOrEmpty(owner))
                return owner + "." + Resources.COEFormTypeTableName;
            else {
                return Resources.COEFormTypeTableName;
            }
        }
    }
}
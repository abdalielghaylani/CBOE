using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Framework.Export
{
    class COEExportTemplateUtilities
    {
        public static void BuildExportTemplateTableName(string owner, ref string tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COEExportTemplateTableName;
                }
                else
                {
                    tableName = Resources.COEExportTemplateTableName;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static ResultsCriteria DeserializeCOEResultCriteria(string coeResultCriteriaString)
        {
            try
            {
                //to deserialize
                XmlSerializer deSerializer = new XmlSerializer(typeof(ResultsCriteria), "COE.ResultsCriteria");
                StringReader stringReader = new StringReader(coeResultCriteriaString);
                ResultsCriteria myCOEResultCriteria = (ResultsCriteria)deSerializer.Deserialize(stringReader);
                return myCOEResultCriteria;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static string SerializeCOEResultCriteria(ResultsCriteria coeResultCriteria)
        {

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ResultsCriteria), "COE.ResultsCriteria");
                StringWriter stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, coeResultCriteria);
                string serializedCOEResultCriteria = stringWriter.ToString();
                return serializedCOEResultCriteria;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}


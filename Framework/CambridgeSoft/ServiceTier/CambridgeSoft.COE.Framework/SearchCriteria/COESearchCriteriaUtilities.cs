using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    public static class COESearchCriteriaUtilities
    {
        public static void BuildSearchCriteriaTableName(string owner, ref string tableName)
        {
            try
            {
                if (owner.Length > 0)
                {
                    tableName = owner + "." + Resources.COESearchCriteriaTableName;
                }
                else
                {
                    tableName = Resources.COESearchCriteriaTableName;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static void BuildSavedSearchCriteriaTable(string owner, ref string tablename) 
        {
            try
            {
                if (owner.Length > 0)
                {
                    tablename = owner + "." + Resources.COESavedSearchCriteriaTableName;
                }
                else
                {
                    tablename = Resources.COESavedHitListIDTableName;
                }
            }
            catch 
            {
                throw;
            }
        }        

        public static SearchCriteria DeserializeCOESearchCriteria(string coeSearchCriteriaString)
        {
            try
            {
                //to deserialize
                XmlSerializer deSerializer = new XmlSerializer(typeof(SearchCriteria), "COE.SearchCriteria");
                StringReader stringReader = new StringReader(coeSearchCriteriaString);
                SearchCriteria myCOESearchCriteria = (SearchCriteria)deSerializer.Deserialize(stringReader);
                return myCOESearchCriteria;
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public static string SerializeCOESearchCriteria(SearchCriteria coeSearchCriteria)
        {

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SearchCriteria), "COE.SearchCriteria");
                StringWriter stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, coeSearchCriteria);
                string serializedCOESearchCriteria = stringWriter.ToString();
                return serializedCOESearchCriteria;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

    }
}

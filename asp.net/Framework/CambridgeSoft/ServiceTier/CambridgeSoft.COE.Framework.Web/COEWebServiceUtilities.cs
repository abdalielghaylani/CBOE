using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;

namespace CambridgeSoft.COE.Framework.Web
{
    /// <summary>
    /// Utilitarian class used for serializing and deserializing objects.
    /// </summary>
    public static class COEWebServiceUtilities
    {
        /// <summary>
        /// Deserializes a SearchResponse object.
        /// </summary>
        /// <param name="inputString">Serialized search response</param>
        /// <returns>Deserialized <see cref="SearchResponse"/></returns>
        public static SearchResponse DeserializeCOESearchResponse(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(SearchResponse), "COE.SearchResponse");
            StringReader stringReader = new StringReader(inputString);
            SearchResponse myObjectOutput = (SearchResponse) deSerializer.Deserialize(stringReader);
            return myObjectOutput;

        }

        /// <summary>
        /// Deserializes a COEDataView object.
        /// </summary>
        /// <param name="inputString">Serialized COEDataView</param>
        /// <returns>Deserialized <see cref="COEDataView"/></returns>
        public static COEDataView DeserializeCOEDataView(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(COEDataView), "COE.COEDataView");
            StringReader stringReader = new StringReader(inputString);
            COEDataView myObjectOutput = (COEDataView) deSerializer.Deserialize(stringReader);
            return myObjectOutput;
        }

        /// <summary>
        /// Deserializes a ResultsCriteria object.
        /// </summary>
        /// <param name="inputString">Serialized ResultsCriteria</param>
        /// <returns>Deserialized <see cref="ResultsCriteria"/></returns>
        public static ResultsCriteria DeserializeCOEResultsCriteria(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(ResultsCriteria), "COE.ResultsCriteria");
            StringReader stringReader = new StringReader(inputString);
            ResultsCriteria myObjectOutput = (ResultsCriteria) deSerializer.Deserialize(stringReader);
            return myObjectOutput;
        }

        /// <summary>
        /// Deserializes a SearchCriteria object.
        /// </summary>
        /// <param name="inputString">Serialized SearchCriteria</param>
        /// <returns>Deserialized <see cref="SearchCriteria"/></returns>
        public static SearchCriteria DeserializeCOESearchCriteria(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(SearchCriteria), "COE.SearchCriteria");
            StringReader stringReader = new StringReader(inputString);
            SearchCriteria myObjectOutput = (SearchCriteria) deSerializer.Deserialize(stringReader);
            return myObjectOutput;
        }

        /// <summary>
        /// Deserializes a PagingInfo object.
        /// </summary>
        /// <param name="inputString">Serialized PagingInfo</param>
        /// <returns>Deserialized <see cref="PagingInfo"/></returns>
        public static PagingInfo DeserializeCOEPagingInfo(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(PagingInfo), "COE.PagingInfo");
            StringReader stringReader = new StringReader(inputString);
            PagingInfo myObjectOutput = (PagingInfo) deSerializer.Deserialize(stringReader);
            return myObjectOutput;
        }

        /// <summary>
        /// Deserializes a HitListInfo Object.
        /// </summary>
        /// <param name="inputString">Serialized HitListInfo</param>
        /// <returns>Deserialized <see cref="HitListInfo"/></returns>
        public static HitListInfo DeserializeCOEHitListInfo(string inputString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(HitListInfo), "COE.HitListInfo");
            StringReader stringReader = new StringReader(inputString);
            HitListInfo myObjectOutput = (HitListInfo) deSerializer.Deserialize(stringReader);
            return myObjectOutput;
        }

        /// <summary>
        /// Serializes a SearchResponse object.
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="SearchResponse"/></param>
        /// <returns>Serialized SearchResponse</returns>
        public static string SerializeCOESearchResponse(SearchResponse inputObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchResponse), "COE.SearchResponse");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }

        /// <summary>
        /// Serializes a COEDataView object.
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="COEDataView"/></param>
        /// <returns>Serialized COEDataView</returns>
        public static string SerializeCOEDataView(COEDataView inputObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(COEDataView), "COE.COEDataView");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }

        /// <summary>
        /// Serializes a ResultsCriteria object
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="ResultsCriteria"/></param>
        /// <returns>Serialized ResultsCriteria</returns>
        public static string SerializeCOEResultsCriteria(ResultsCriteria inputObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResultsCriteria), "COE.ResultsCriteria");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }

        /// <summary>
        /// Serializes a SearchCriteria object.
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="SearchCriteria"/></param>
        /// <returns>Serialized SearchCriteria</returns>
        public static string SerializeCOESearchCriteria(SearchCriteria inputObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchCriteria), "COE.SearchCriteria");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }

        /// <summary>
        /// Serializes a PagingInfo object.
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="PagingInfo"/></param>
        /// <returns>Serialized PagingInfo</returns>
        public static string SerializeCOEPagingInfo(PagingInfo inputObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PagingInfo), "COE.PagingInfo");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }

        /// <summary>
        /// Serializes a HitListInfo object.
        /// </summary>
        /// <param name="inputObject">Deserialized <see cref="HitListInfo"/></param>
        /// <returns>Serialized HitListInfo</returns>
        public static string SerializeCOEHitListInfo(HitListInfo inputObject)
        {
            //to deserialize
            XmlSerializer serializer = new XmlSerializer(typeof(HitListInfo), "COE.HitListInfo");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, inputObject);
            string serializedObject = stringWriter.ToString();
            return serializedObject;
        }
    }
}

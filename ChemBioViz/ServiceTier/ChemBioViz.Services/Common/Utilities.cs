using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web;

using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Security.Cryptography;

namespace CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class Utilities
    {

        /// <summary>
        /// By default DataSet.GetXML gets DateTime and Date in format of CCYY-MM-DDThh:mm:ss and CCYY-MM-DD,
        /// which can not be parse by Oracle. Need to reformat them to be recognized by Oracle.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// 
        public static string ReformatXMLDate(string xml)
        {
            string ret = "";
            using (StringWriter stringWriter = new StringWriter()) //Coverity Fix for CID : 11806
            {

                using (XmlTextReader xmlTextReader = new XmlTextReader(xml, XmlNodeType.Element, null))
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.Indentation = 4;
                        xmlTextWriter.WriteStartDocument();
                        string elementName = "";


                        while (xmlTextReader.Read())
                        {
                            switch (xmlTextReader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    xmlTextWriter.WriteStartElement(xmlTextReader.Name);
                                    elementName = xmlTextReader.Name;
                                    break;
                                case XmlNodeType.Text:
                                    if ((elementName.ToUpper() == "CREATION_DATE") || (elementName.ToUpper() == "ENTRY_DATE") || (elementName.ToUpper() == "LAST_MOD_DATE"))
                                    {
                                        //The DateTime can be changed to any format as MM-dd-yyyy or dd-MM-yyyy or .ToShortDateString() ...
                                        //DateTime dateTime = DateTime.Parse(xmlTextReader.Value.ToString());
                                        //xmlTextWriter.WriteString(dateTime.ToString("MM-dd-yyyy"));
                                        xmlTextWriter.WriteString(XmlConvert.ToDateTime(xmlTextReader.Value, XmlDateTimeSerializationMode.Local).ToString());
                                    }
                                    else
                                        xmlTextWriter.WriteString(xmlTextReader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    xmlTextWriter.WriteEndElement();
                                    break;
                            }
                        }
                    }
                }
                ret = stringWriter.ToString();
            }
            return ret;
        }
    }
    /// <summary>
    /// Class to contain all necesaries constants.
    /// </summary>
    public static class Constants
    {
        public const string DatesFormat = "yyyy/MM/dd hh:mm:ss";
    }
}

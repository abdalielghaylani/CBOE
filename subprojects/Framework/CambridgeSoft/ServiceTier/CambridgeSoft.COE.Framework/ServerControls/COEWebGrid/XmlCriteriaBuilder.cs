using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.COEWebGrid
{

    public class XmlCriteriaBuilder
    {
        #region Constructior

        public XmlCriteriaBuilder()
        {

        }
        #endregion

        StringReader stringReader = null;
        string topXmlTag = @"<?xml version=""1.0"" encoding=""utf-8"" ?>";


        public COEDataView BuildDataViewFromXML(string dataViewXml)
        {
            //if (dataViewXml.Contains(topXmlTag))
            //{
            //    dataViewXml = dataViewXml.Replace(topXmlTag, string.Empty);
            //}
            //else
            //{
            //    dataViewXml.Insert(0, topXmlTag);
            //}
            stringReader = new StringReader(dataViewXml);
            XmlDocument doc = new XmlDocument();
            doc.Load(stringReader);
            COEDataView dataView = null;
            try
            {
                dataView = new COEDataView(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataView;
        }

        /// <summary>
        ///     Builds a SearchCriteria object from xml file
        /// </summary>
        /// <param name="searchCriteriaXml"></param>
        public SearchCriteria BuildSearchCriteriaFromXML(string searchCriteriaXml)
        {
            //if (searchCriteriaXml.Contains(topXmlTag))
            //{
            //    searchCriteriaXml = searchCriteriaXml.Replace(topXmlTag, string.Empty);
            //}
            //else
            //{
            //    searchCriteriaXml.Insert(0, topXmlTag);
            //}
            
            stringReader = new StringReader(searchCriteriaXml);
            XmlDocument doc = new XmlDocument();
            doc.Load(stringReader);
            SearchCriteria searchCriteria = null;
            try
            {
                searchCriteria = new SearchCriteria(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return searchCriteria;
        }

        /// <summary>
        ///     Builds a ResultsCriteria object from xml file
        /// </summary>
        /// <param name="resultsCriteriaXml"></param>
        public ResultsCriteria BuildResultsCriteriaFromXML(string resultsCriteriaXml)
        {
            //if (resultsCriteriaXml.Contains(topXmlTag))
            //{
            //    resultsCriteriaXml = resultsCriteriaXml.Replace(topXmlTag, string.Empty);
            //}
            //else
            //{
            //    resultsCriteriaXml.Insert(0, topXmlTag);
            //}
            
            stringReader = new StringReader(resultsCriteriaXml);
            XmlDocument doc = new XmlDocument();
            doc.Load(stringReader);
            ResultsCriteria resultsCriteria = null;
            try
            {
                resultsCriteria = new ResultsCriteria(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultsCriteria;
        }

        /// <summary>
        ///     Builds a ResultsCriteria object from xml file
        /// </summary>
        /// <param name="resultsCriteriaXml"></param>
        public PagingInfo BuildPagingInfoFromXML(string pagingInfoXml)
        {
            //if (pagingInfoXml.Contains(topXmlTag))
            //{
            //    pagingInfoXml = pagingInfoXml.Replace(topXmlTag, string.Empty);
            //}
            //else
            //{
            //    pagingInfoXml.Insert(0, topXmlTag);
            //}
            
            stringReader = new StringReader(pagingInfoXml);
            XmlDocument doc = new XmlDocument();
            doc.Load(stringReader);
            PagingInfo pagingInfo = null;
            try
            {
                pagingInfo = new PagingInfo(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pagingInfo;
        }
    }
}

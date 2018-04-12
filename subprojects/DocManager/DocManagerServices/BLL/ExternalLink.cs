using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    /// <summary>
    /// Domain object used to hold information about an 'instance' of a external links of the document.
    /// </summary>
    [Serializable()]
    public class ExternalLink : BusinessBase<ExternalLink>
    {
        #region Variables

        private string _id = string.Empty;
        private string _documentID = string.Empty;
        private string _externalApplication = string.Empty;
        private string _externalLinkType = string.Empty;
        private string _externalID = string.Empty;
        private string _externalFieldName = string.Empty;
        private string _linkSubmitter = string.Empty;
        private DateTime _linkSubmittedDate;
        
        #endregion

        #region Properties

        /// <summary>
        /// Control Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public string ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }

        /// <summary>
        /// Document ID
        /// </summary>
        public string DocumentID
        {
            get
            {
                CanReadProperty(true);
                return _documentID;
            }
        }

        /// <summary>
        /// Name of External Application
        /// </summary>
        public string ExternalApplication
        {
            get
            {
                CanReadProperty(true);
                return _externalApplication;
            }
        }

        /// <summary>
        /// External Link types
        /// Registration Registry Level
        /// Registration Batch Level
        /// Inventory Containers
        /// DrugDeg Parent Compounds
        /// DrugDeg Degradant Compounds
        /// </summary>
        public string ExternalLinkType
        {
            get
            {
                CanReadProperty(true);
                return _externalLinkType;
            }
        }

        /// <summary>
        /// External ID
        /// </summary>
        public string ExternalID
        {
            get
            {
                CanReadProperty(true);
                return _externalID;
            }
        }

        /// <summary>
        /// External Field name, which is useful for linking the other application
        /// </summary>
        public string ExternalFieldName
        {
            get
            {
                CanReadProperty(true);
                return _externalFieldName;
            }
        }

        /// <summary>
        /// name of the Link Submitter
        /// </summary>
        public string LinkSubmitter
        {
            get
            {
                CanReadProperty(true);
                return _linkSubmitter;
            }
        }

        /// <summary>
        /// Date of Link Submitter
        /// </summary>
        public DateTime LinkSubmittedDate
        {
            get
            {
                CanReadProperty(true);
                return _linkSubmittedDate;
            }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Create a Control by given xml.
        /// </summary>
        /// <param name="xml"></param>
        private ExternalLink(string xml)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("ExternalLink/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/DocumentID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _documentID = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/ExternalApplication");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _externalApplication = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/ExternalLinkType");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _externalLinkType = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/ExternalID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _externalID = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/ExternalFieldName");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _externalFieldName = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/LinkSubmitter");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _linkSubmitter = xIterator.Current.Value;

            xIterator = xNavigator.Select("ExternalLink/LinkSubmittedDate");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _linkSubmittedDate = Convert.ToDateTime(xIterator.Current.Value);
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Call the ExternalLink constructor
        /// </summary>
        /// Example:
        /// <ExternalLink>
        /// <ID></ID>
        /// <DocumentID></ DocumentID>
        /// <ExternalApplication></ ExternalApplication>
        /// <ExternalLinkType></ExternalLinkType>
        /// <ExternalID></ExternalID>
        /// <ExternalFieldName></ExternalFieldName>
        /// <LinkSubmitter></ LinkSubmitter>
        /// <LinkSubmittedDate></ LinkSubmittedDate>
        /// </ExternalLink>
        /// <param name="xml"></param>
        /// <returns>ExternalLink object</returns>
        public static ExternalLink NewExternalLink(string xml)
        {
            return new ExternalLink(xml);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build this into the custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>ExternalLink xml</returns>
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            builder.Append("<ExternalLink>");
            builder.Append("<ID>" + this._id + "</ID>");
            builder.Append("<DocumentID>" + this._documentID + "</ DocumentID>");
            builder.Append("<ExternalApplication>" + this._externalApplication + "</ ExternalApplication>");
            builder.Append("<ExternalLinkType>" + this._externalLinkType + "</ExternalLinkType>");
            builder.Append("<ExternalID>" + this._externalID + "</ExternalID>");
            builder.Append("<ExternalFieldName>" + this._externalFieldName + "</ExternalFieldName>");
            builder.Append("<LinkSubmitter>" + this._linkSubmitter + "</ LinkSubmitter>");
            builder.Append("<LinkSubmittedDate>" + this._linkSubmittedDate + "</ LinkSubmittedDate>");

            builder.Append("</ExternalLink>");
            return builder.ToString();
        }

        /// <summary>
        /// Get the Id value of the document
        /// </summary>
        /// <returns>id value</returns>
        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.DocumentManager.Services.COEDocumentManagerService;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
	[Serializable()]
	public class DocumentList : BusinessListBase<DocumentList, Document>
	{
		#region Variables

		[NonSerialized, NotUndoable]
		private DAL _coeDAL = null;
		[NonSerialized, NotUndoable]
		private DALFactory _dalFactory = new DALFactory();
		private string _serviceName = "COEDocumentManager";
		private string _id = string.Empty;

		#endregion

		#region Properties

		/// <summary>
		/// Identifier
		/// </summary>
		[System.ComponentModel.DataObjectField(true, true)]
		public string ID
		{
			get { return _id; }
		}

		#endregion

		#region Factory Methods

        /// <summary>
        /// call the Constructor.
        /// </summary>
        /// <param name="xml"></param>
        /// Example:
        /// <Document>
        /// <ID>0</ID>
        /// <Content></Content>
        /// <Name></Name>
        /// <Type></Type>
        /// <Size>0</Size>
        /// <Location></Location>
        /// <Title></Title>
        /// <Author></Author>
        /// <Submitter></Submitter>
        /// <Comments></Comments>
        /// <DateSubmitted></DateSubmitted>
        /// <ExternalLinks></ExternalLinks>
        /// <Structures></Structures>
        /// <Properties>
        /// <Property name="REG_RLS_PROJECT_ID" type="String"></Property>
        /// <Property name="REPORT_NUMBER" type="String"></Property>
        /// <Property name="MAIN_AUTHOR" type="String"></Property>
        /// <Property name="STATUS" type="String"></Property>
        /// <Property name="WRITER" type="String"></Property>
        /// <Property name="DOCUMENT_DATE" type="DateTime"></Property>
        /// <Property name="DOCUMENT_CLASS" type="String"></Property>
        /// <Property name="SEC_DOC_CAT" type="String"></Property>
        /// <Property name="ABSTRACT" type="String"></Property>
        /// </Properties>
        /// <AddIns>
        /// <AddIn assembly="CambridgeSoft.COE.DocumentManager.Services.DocManagerAddIns, Version=11.0.1.0, Culture=neutral, PublicKeyToken=0b391c4fd383b398" class="CambridgeSoft.COE.DocumentManager.Services.DocManagerAddIns.GetStructuresFromDocument">
        /// <Event eventName="Inserting" eventHandler="OnInsertingHandler"/>
        /// <AddInConfiguration>
        /// <MolServerTempPath>C:\test\</MolServerTempPath>
        /// </AddInConfiguration>
        /// </AddIn>
        /// </AddIns>
        /// </Document>
        /// <returns>DocumentList Object</returns>
		public static DocumentList NewDocumentList(string xml)
		{
			return new DocumentList(xml);
		}


		#endregion

		#region Xml methods

		/// <summary>
		/// Build this into the custom xml.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Document Xml</returns>
		public string UpdateSelf()
		{
			StringBuilder builder = new StringBuilder("");
			builder.Append("<Documents>");
			for (int i = 0; i < this.Count; i++)
				builder.Append(this[i].UpdateSelf());
			builder.Append("</Documents>");
			return builder.ToString();
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Create a ControlList by given xml.
		/// </summary>
		/// <param name="xml"></param>
		private DocumentList(string xml)
			: this()
		{
			XPathDocument xDocument = new XPathDocument(new StringReader(xml));
			XPathNavigator xNavigator = xDocument.CreateNavigator();
			XPathNodeIterator xIterator = xNavigator.Select("Documents/Document");

			if (xIterator.MoveNext())
			{
				do
				{
					this.Add(Document.NewDocument(xIterator.Current.OuterXml));
				} while (xIterator.Current.MoveToNext());
			}

			xIterator = xNavigator.Select("Documents/ID");
			if (xIterator.MoveNext())
				if (!string.IsNullOrEmpty(xIterator.Current.Value))
					_id = xIterator.Current.Value;
		}

        /// <summary>
        /// Default Constructor
        /// </summary>
		private DocumentList()
		{

		}

		#endregion

		#region Methods

		/// <summary>
		/// Load DAL
		/// </summary>
		private void LoadDAL()
		{
			if (_dalFactory == null) { _dalFactory = new DALFactory(); }
			string _databaseName = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
			_dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, _databaseName, true);
		}

		/// <summary>
		/// Get all info
		/// </summary>
		/// <param name="criteria"></param>
		private void DataPortal_Fetch()
		{
			if (_coeDAL == null)
				LoadDAL();

			//Get the full privileges for given application.
			//_xmlPrivileges = _coeDAL.GetConfigurationXML();
			string xmlvar = _coeDAL.GetDocumentList();
			//this.InitializeFromXML(xmlvar);
		}

		#endregion
	}
}

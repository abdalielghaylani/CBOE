using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.DocumentManager.Services.COEDocumentManagerService;
using CambridgeSoft.COE.DocumentManager.Services.AddIns;
using CambridgeSoft.COE.Framework.Services;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class Document : BusinessBase<Document>, IDocument, IDisposable
    {
        #region Variables

        [NonSerialized, NotUndoable]
        private DAL _coeDAL = null;
        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDocumentManager";


        private int _id = 0;
        private string _xml;
        private string _addInsXml;
        private byte[] _binaryContent;
        private string _name = string.Empty;
        private string _type = string.Empty;
        private int _size = 0;
        private string _location = string.Empty;
        private string _title = string.Empty;
        private string _author = string.Empty;
        private string _submitter = string.Empty;
        private string _comments = string.Empty;
        private DateTime _dateSubmitted;
        private ExternalLinkList _externalLinkList;
        private PropertyList _propertyList;
        private StructureList _structureList;
        private bool _isEditable;

        /// <summary>
        /// Returns true if the user is allowed to edit the record based on RLS restrictions.
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable; }
        }

        #endregion

        #region Published Events

        public event EventHandler Loaded;
        public event EventHandler Inserting;
        public event EventHandler Inserted;
        public event EventHandler Updating;
        public event EventHandler Updated;
        public event EventHandler Deleting;
        public event EventHandler Deleted;

        protected void OnLoaded(object sender, EventArgs args)
        {
            if (Loaded != null && !this.IsNew)
                Loaded(sender, args);
        }

        protected void OnInserting(object sender, EventArgs args)
        {
            if (Inserting != null)
                Inserting(sender, args);
        }

        protected void OnInserted(object sender, EventArgs args)
        {
            if (Inserted != null)
                Inserted(sender, args);
        }

        protected void OnUpdating(object sender, EventArgs args)
        {
            if (Updating != null)
                Updating(sender, args);
        }

        protected void OnUpdated(object sender, EventArgs args)
        {
            if (Updated != null)
                Updated(sender, args);
        }

        protected void OnDeleting(object sender, EventArgs args)
        {
            if (Deleting != null)
                Deleting(sender, args);
        }

        protected void OnDeleted(object sender, EventArgs args)
        {
            if (Deleted != null)
                Deleted(sender, args);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Control Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public Int32 ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                CanWriteProperty(true);
                _id = value;
            }
        }


        /// <summary>
        /// string format Content of the document
        /// </summary>
        //public string Content
        //{
        //    get
        //    {
        //        CanReadProperty(true);
        //        return _content;
        //    }
        //}

        /// <summary>
        /// Binary Content of the document
        /// </summary>
        public byte[] BinaryContent
        {
            get
            {
                CanReadProperty(true);
                return _binaryContent;
            }
            set
            {
                CanWriteProperty(true);
                _binaryContent = value;
                //when the binaryContent is set the size should be set also
                _size = _binaryContent.Length;
                //_content = Convert.ToBase64String(_binaryContent);

                // Extracting structure from Document
                List<byte[]> byteArray = ChemDrawDocumentsFromOfficeX.GetChemicalObjects(_binaryContent);
                this._structureList.Clear();
                Structure s = Structure.NewStructure();
                foreach (byte[] stru in byteArray)
                {
                    s.Value = System.Convert.ToBase64String(stru, 0, stru.Length);
                    this._structureList.Add(s);
                }

                PropertyHasChanged();
            }
        }

        /// <summary>
        ///Name of the document
        /// </summary>
        public string Name
        {
            get
            {
                CanReadProperty(true);
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                _name = value;
                _type = GetFileType(_name);
                PropertyHasChanged();
            }

        }
        

        /// <summary>
        /// Type of the document
        /// </summary>
        public string Type
        {
            get
            {
                CanReadProperty(true);
                return _type;
            }
        }

        /// <summary>
        /// size of the document
        /// </summary>
        public int Size
        {
            get
            {
                CanReadProperty(true);
                return _size;
            }
        }

        /// <summary>
        /// phisical location of the document
        /// </summary>
        public string Location
        {
            get
            {
                CanReadProperty(true);
                return _location;
            }
            set
            {
                CanWriteProperty(true);
                _location = value;
                PropertyHasChanged();
            }
        }

        /// <summary>
        /// Title of the document
        /// </summary>
        public string Title
        {
            get
            {
                CanReadProperty(true);
                return _title;
            }
            set
            {
                CanWriteProperty(true);
                _title = value;
                PropertyHasChanged();
            }
        }

        /// <summary>
        /// Document Author
        /// </summary>
        public string Author
        {
            get
            {
                CanReadProperty(true);
                return _author;
            }
            set
            {
                CanWriteProperty(true);
                _author = value;
                PropertyHasChanged();
            }
        }

        /// <summary>
        /// Name of the document submitter
        /// </summary>
        public string Submitter
        {
            get
            {
                CanReadProperty(true);
                return _submitter;
            }
        }

        /// <summary>
        /// Document comments
        /// </summary>
        public string Comments
        {
            get
            {
                CanReadProperty(true);
                return _comments;
            }
            set
            {
                CanWriteProperty(true);
                _comments = value;
                PropertyHasChanged();
            }
        }


        /// <summary>
        /// Submitted date of the document
        /// </summary>
        public DateTime DateSubmitted
        {
            get
            {
                CanReadProperty(true);
                return _dateSubmitted;
            }
        }

        /// <summary>
        /// External links of the document
        /// </summary>
        public ExternalLinkList ExternalLinkList
        {
            get { return _externalLinkList; }
        }

        /// <summary>
        /// Property list of the document
        /// </summary>
        public PropertyList PropertyList
        {
            get { return _propertyList; }
        }

        //public Structure Structure
        //{
        //    get { return _structure; }
        //}

        /// <summary>
        /// list of structures
        /// </summary>
        public StructureList StructureList
        {
            get { return _structureList; }
        }

        /// <summary>
        /// xml format of the document object
        /// </summary>
        public string Xml
        {
            get
            {
                if (IsDirty)
                {
                    UpdateSelf();
                }
                return _xml;
            }
            set
            {
                _xml = value;
                PropertyHasChanged();
            }
        }

        public string AddInsXml
        {
            get
            {
                CanReadProperty(true);
                return _addInsXml;
            }
            set
            {
                CanWriteProperty(true);
                _addInsXml = value;
                PropertyHasChanged();
            }
        }

        #endregion

        #region Contructors

        private Document()
        {

        }

        /// <summary>
        /// document constructor to initialize the document properties
        /// </summary>
        /// <param name="xml"></param>
        private Document(string xml)
        {
            InitializeFromXml(xml);
        }

        public void InitializeFromXml(string xml)
        {
            InitializeFromXml(xml, false, true);
        }

        public void InitializeFromXml(string xml, bool isNew, bool isClean)
        {
            //this.Updated = null;
            //this.Updating = null;
            //this.Inserted = null;
            //this.Inserting = null;
            //this.Loaded = null;

            _xml = xml;

            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Document/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Document/Content");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                {
                    //_content = xIterator.Current.Value;
                    //set the binary from the base64
                    //_binaryContent = Convert.FromBase64String(_content);
                    _binaryContent = Convert.FromBase64String(xIterator.Current.Value);
                }
            xIterator = xNavigator.Select("Document/Name");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _name = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Type");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _type = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Size");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _size = Convert.ToInt32(xIterator.Current.Value);

            xIterator = xNavigator.Select("Document/Location");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _location = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Title");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _title = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Author");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _author = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Submitter");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _submitter = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/Comments");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _comments = xIterator.Current.Value;

            xIterator = xNavigator.Select("Document/DateSubmitted");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _dateSubmitted = Convert.ToDateTime(xIterator.Current.Value);

            xIterator = xNavigator.Select("Document/ExternalLinks");
            if (xIterator.MoveNext())
                //
                //if (!string.IsNullOrEmpty(xIterator.Current.Value))
                _externalLinkList = ExternalLinkList.NewExternalLinkList(xIterator.Current.OuterXml);

            xIterator = xNavigator.Select("Document/Properties");
            if (xIterator.MoveNext())
                //this check doesn't seem to work
                //removing the check...could be that checking the innerxml instead of value would be better
                //if (!string.IsNullOrEmpty(xIterator.Current.Value))
                _propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, isClean);

            //xIterator = xNavigator.Select("Document/Structure");
            //if (xIterator.MoveNext())
            //    if (!string.IsNullOrEmpty(xIterator.Current.Value))
            //        _structure = Structure.NewStructure(xIterator.Current.OuterXml, isNew, isClean);


            xIterator = xNavigator.Select("Document/Structures");
            if (xIterator.MoveNext())
                //this check doesn't seem to work
                //removing the check...could be that checking the innerxml instead of value would be better
                //if (!string.IsNullOrEmpty(xIterator.Current.Value))
                _structureList = StructureList.NewStructureList(xIterator.Current.OuterXml);

            xIterator = xNavigator.Select("Document/AddIns");
            if (xIterator.MoveNext())
            {
                _addInsXml = xIterator.Current.OuterXml;
                AddInsManager addInsManager = AddInsManager.GetManager(this);

                addInsManager.Add(this, xIterator.Current.OuterXml);
            }

            //this might allow us to keep the correct xml
            UpdateSelf();

            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();
            else
                MarkNew();
        }


        #endregion

        #region Factory methods

        public static Document NewDocument(string xml)
        {
            return new Document(xml);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the file type
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetFileType(string filename)
        {
            string[] sarray = new string[] { "." };
            string[] farray = filename.Split(sarray, StringSplitOptions.None);
            return farray[farray.Length - 1];
        }

        /// <summary>
        /// Build this into the custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            builder.Append("<Document>");
            builder.Append("<ID>" + this._id + "</ID>");
            //builder.Append("<Content>" + this._content + "</Content>");
            if (this._binaryContent != null)
                builder.Append("<Content>" + Convert.ToBase64String(this._binaryContent) + "</Content>");
            else
                builder.Append("<Content>" + string.Empty + "</Content>");
            builder.Append("<Name>" + this._name + "</Name>");
            builder.Append("<Type>" + this._type + "</Type>");
            builder.Append("<Size>" + this._size + "</Size>");
            builder.Append("<Location>" + this._location + "</Location>");
            builder.Append("<Title>" + this._title + "</Title>");
            builder.Append("<Author>" + this._author + "</Author>");
            builder.Append("<Submitter>" + this._submitter + "</Submitter>");
            builder.Append("<Comments>" + this._comments + "</Comments>");
            builder.Append("<DateSubmitted>" + this._dateSubmitted + "</DateSubmitted>");
            //builder.Append("<ExternalLinks>" + this._externalLinkList.UpdateSelf() + "</ExternalLinks>");
            if (this._externalLinkList != null)
                builder.Append(this._externalLinkList.UpdateSelf());
            //builder.Append("<Structures>" + this._structureList.UpdateSelf() + "</Structures>");
            if (this._structureList != null)
                builder.Append(this._structureList.UpdateSelf());
            // builder.Append("<Properties>" + this._propertyList.UpdateSelf() + "</Properties>");
            if (this._propertyList != null)
                builder.Append(this._propertyList.UpdateSelf());

            if (this._addInsXml != null)
                builder.Append(this._addInsXml);


            builder.Append("</Document>");
            _xml = builder.ToString();
            return _xml;
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        public static Document GetDocumentByID(Int32 ID)
        {
            Document doc = DataPortal.Fetch<Document>(new Criteria(ID));
            return doc;
        }

        public static Document GetNewDocument()
        {
            Document doc = DataPortal.Fetch<Document>(new Criteria(0));
            return doc;
        }

        public static Document InsertNewDocument(Document doc)
        {
            Document d = DataPortal.Create<Document>(doc);
            d.Save();
            return d;
        }

        public static Document UpdateDocument(Document doc)
        {
            Document d = DataPortal.Update<Document>(doc);
            d.Save();
            return d;
        }

        public static void DeleteDocumentByID(Int32 ID)
        {
            DataPortal.Delete(new Criteria(ID));

        }

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
        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();
            if (criteria.ID != 0)
            {
                this.InitializeFromXml(_coeDAL.GetDocumentByID(criteria.ID));
            }
            else
            {
                this.InitializeFromXml(_coeDAL.GetEmptyDocObject());

                //add the current user as the submitter to the object
                this.SetSubmitter(Csla.ApplicationContext.User.Identity.Name);
            }
            OnLoaded(this, new EventArgs());
        }

        private void SetSubmitter(string userName)
        {
            this._submitter = userName;
        }

        /// <summary>
        /// Get all info
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void DataPortal_Create(Document document)
        {
            if (_coeDAL == null)
                LoadDAL();

            this.InitializeFromXml(_coeDAL.GetEmptyDocObject());
            this.InitializeFromXml(document.Xml, true, false);
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Insert()
        {
            OnInserting(this, new EventArgs());
            Int32 docid = _coeDAL.AddNewDocument(this);
            this.InitializeFromXml(_coeDAL.GetDocumentByID(docid), false, false);

            //TODO: Add Inserting code here
            OnInserted(this, new EventArgs());
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Update()
        {
            if (_coeDAL == null)
                LoadDAL();
            OnUpdating(this, new EventArgs());
            Int32 docid = _coeDAL.UpdateDocument(this);
            this.InitializeFromXml(_coeDAL.GetDocumentByID(docid), false, false);
            OnUpdated(this, new EventArgs());
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_DeleteSelf()
        {
            if (_coeDAL == null)
                LoadDAL();
            OnDeleting(this, new EventArgs());
            _coeDAL.DeleteDocument(this);
            OnDeleted(this, new EventArgs());
        }

        [Transactional(TransactionalTypes.Manual)]
        private void DataPortal_Delete(Criteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();
            OnDeleting(this, new EventArgs());
            _coeDAL.DeleteDocumentByID(criteria.ID);
            OnDeleted(this, new EventArgs());
        }

        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            private Int32 _id;

            public Int32 ID
            {
                get { return _id; }
            }


            public Criteria(Int32 id)
            {
                _id = id;
            }
        }

        //[Serializable()]
        //private class EmptyCriteria
        //{
        //    private bool _empty;

        //    public bool Empty
        //    {
        //        get { return _empty; }
        //    }


        //    public EmptyCriteria(bool empty)
        //    {
        //        _empty = empty;
        //    }
        //}

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Inserted = null;
            this.Inserting = null;
            this.Loaded = null;            
            this.Updating = null;
        }

        #endregion

    }
}

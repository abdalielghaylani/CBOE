using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class TableBO : Csla.BusinessBase<TableBO>
    {
        #region Variables

        private int _id = int.MinValue;
        private string _name = String.Empty;
        private string _alias = String.Empty;
        private int _primaryKey = int.MinValue;        
        private bool _isView = false;
        private string _database = String.Empty;
        private string _xml = String.Empty;
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("COETableBO");
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COETableBO";
        private bool _fromMasterSchema = false;
        private FieldListBO _fields;
        private int _highestID = -1;
        private TagList _tags;
        #endregion

        #region Enums
        /// <summary>
        /// Object's properies names. Used for avoiding string hardcoding
        /// </summary>
        private enum Properties
        {
            ID,
            Name,
            Alias,
            DataBase,
            PrimaryKey,
            PrimaryKeyName,
            Xml,
            FromMasterSchema,
            IsView,
        }
        #endregion

        #region Properties
        /// <summary>
        /// Table's id
        /// </summary>
        [System.ComponentModel.DataObjectField(true)]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                CanWriteProperty(Properties.ID.ToString(), true);
                if (value != _id)
                {
                    _id = value;
                    PropertyHasChanged(Properties.ID.ToString());
                }
            }
        }

        /// <summary>
        /// Table's name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Table's alias
        /// </summary>
        public string Alias
        {
            get
            {
                return System.Web.HttpUtility.HtmlDecode(_alias);
            }
            set
            {
                CanWriteProperty(Properties.Alias.ToString(), true);
                if (value == null)
                {
                    _alias = String.Empty;
                    PropertyHasChanged(Properties.Alias.ToString());
                }
                if (value != _alias)
                {
                    _alias = System.Web.HttpUtility.HtmlEncode(value);
                    PropertyHasChanged(Properties.Alias.ToString());
                }
            }
        }

        /// <summary>
        /// Table's database
        /// </summary>
        public string DataBase
        {
            get
            {
                return _database;
            }
        }        

        /// <summary>
        /// Table's primary key id
        /// </summary>
        public int PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
            set
            {
                CanWriteProperty(Properties.PrimaryKey.ToString(), true);
                if (value != _primaryKey)
                {
                    _primaryKey = value;
                    PropertyHasChanged(Properties.PrimaryKey.ToString());
                }
            }
        }
        
        /// <summary>
        /// Table's primary key name
        /// </summary>
        public string PrimaryKeyName
        {
            get
            {
                string retVal = String.Empty;
                if (_primaryKey >= 0)
                {
                    FieldBO PKfield = _fields.GetField(_primaryKey);
                    retVal = string.IsNullOrEmpty(PKfield.Alias) ? PKfield.Name : PKfield.Name + " (" + PKfield.Alias + ")";
                }
                return retVal;
            }
        }

        /// <summary>
        /// Indicates if the table was inherited (not selected by the user)
        /// </summary>
        public bool FromMasterSchema
        {
            get
            {
                return _fromMasterSchema;
            }
            set
            {
                if (value != _fromMasterSchema)
                {
                    _fromMasterSchema = value;
                    PropertyHasChanged(Properties.FromMasterSchema.ToString());
                }
            }
        }

        /// <summary>
        /// Xml representation of the object
        /// </summary>
        public string XML
        {
            get
            {
                CanWriteProperty(Properties.Xml.ToString(), true);
                return _xml;
            }
        }

        /// <summary>
        /// List of fields
        /// </summary>
        public FieldListBO Fields
        {
            get
            {
                return _fields;
            }
        }

        /// <summary>
        /// Gets the list of tags
        /// </summary>
        public TagList Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        /// <summary>
        /// Indicates if this is a view as opposite to a table
        /// </summary>
        public bool IsView
        {
            get
            {
                return _isView;
            }
        }

        /// <summary>
        /// Indicates if the object is new
        /// </summary>
        public bool IsNew
        {
            get
            {
                return base.IsNew;
            }
        }

        /// <summary>
        /// Indicates if the object is in a valid state
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return base.IsValid && _fields.IsValid && !string.IsNullOrEmpty(_name) && _id >= 0 && !string.IsNullOrEmpty(_database);
            }
        }

        /// <summary>
        /// Indicates if the object is dirty
        /// </summary>
        public override bool IsDirty
        {
            get { return base.IsDirty || this.Fields.IsDirty || this.Tags.IsDirty; }
        }

        /// <summary>
        /// Gets the highest id used
        /// </summary>
        public int HighestID
        {
            get
            {
                return _highestID;
            }
            set
            {
                if (value > _highestID)
                    _highestID = value;
                _fields.HighestID = value; //Childs
            }
        }

        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string OrderIndex
        {
            get
            {
                return string.Format("{0:000}", ((TableListBO)base.Parent).IndexOf(this) + 1);
            }
        }
       
        #endregion

        #region Overrided Methods
        /// <summary>
        /// Unique identifier of the object. This value is used by CSLA's implementation of System.Object overrides, such us Equals()
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return string.Format("{0}.{1} {2}", _database, _name, _alias);
        }

        /// <summary>
        /// Business rules should be added here
        /// </summary>
        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }
        #endregion

        #region Constructor

        private TableBO() 
        {
            _tags = new TagList();
            this.MarkAsChild();
            //this.MarkDirty();
        }

        internal TableBO(COEDataView.DataViewTable table) : this()
        {
            _id = table.Id;
            _name = table.Name;
            _alias = System.Web.HttpUtility.HtmlEncode(table.Alias);
            _database = table.Database;
            _isView = table.IsView;
            int.TryParse(table.PrimaryKey, out _primaryKey);
            if (table.Fields != null)
                _fields = FieldListBO.NewFieldList(table.Fields, table.Database);

            List<string> tempTags = new List<string>();
            if (table.Tags != null)
            {
                foreach (string tag in table.Tags)
                    tempTags.Add(tag);
            }
            _tags = new TagList(tempTags);
        }

        internal TableBO(COEDataView.DataViewTable table, bool isClean) : this()
        {
            _id = table.Id;
            _name = table.Name;
            _alias = System.Web.HttpUtility.HtmlEncode(table.Alias);
            _database = table.Database;
            _isView = table.IsView;
            int.TryParse(table.PrimaryKey, out _primaryKey);
            if(table.Fields != null)
                _fields = FieldListBO.NewFieldList(table.Fields, table.Database, isClean);

            List<string> tempTags = new List<string>();
            if (table.Tags != null)
            {
                foreach (string tag in table.Tags)
                    tempTags.Add(tag);
            }
            _tags = new TagList(tempTags);

            if(isClean)
                this.MarkClean();
            else
                this.MarkDirty();
        }

        internal TableBO(TableBO table)
        {
            
            _alias = table.Alias;
            _database = table.DataBase;
            _fromMasterSchema = table.FromMasterSchema;
            _id = table.ID;
            _isView = table.IsView;
            _name = table.Name;
            _primaryKey = table.PrimaryKey;
            _fields = new FieldListBO();
            foreach(FieldBO field in table.Fields)
            {
                _fields.Add(field);
            }
            
            List<string> tempTags = new List<string>();

            if (table.Tags != null)
            {
                foreach (string tag in table.Tags)
                    tempTags.Add(tag);
            }
            _tags = new TagList(tempTags);

            this.MarkAsChild();
        }

        #endregion

        #region Validaton Rules

        private void AddCommonRules() { }

        #endregion

        #region Factory Methods

        public static TableBO NewTable(COEDataView.DataViewTable table)
        {
            try
            {
                return new TableBO(table);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static TableBO NewTable(COEDataView.DataViewTable table, bool isClean)
        {
            try
            {
                return new TableBO(table, isClean);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static TableBO NewTable(TableBO table)
        {
            try
            {
                return new TableBO(table);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        #endregion

        #region Data Access

        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            COEExceptionDispatcher.HandleBLLException(ex);
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();
            if (criteria._table != null)
                this.Initialize(criteria._table, true, true);
        }

        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            internal COEDataView.DataViewTable _table;

            public Criteria(COEDataView.DataViewTable table)
            {
                _table = table;
            }
        }

        #endregion

        #region DALLoader

        private void LoadDAL()
        {
            if(_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion

        #region Methods

        private void Initialize(COEDataView.DataViewTable table, bool isNew, bool isClean)
        {
            _id = table.Id;
            _name = table.Name;
            _alias = System.Web.HttpUtility.HtmlEncode(table.Alias);
            _isView = table.IsView;
            int.TryParse(table.PrimaryKey, out _primaryKey);
            List<string> tempTags = new List<string>();
            if (table.Tags != null)
            {
                foreach (string tag in table.Tags)
                {
                    tempTags.Add(tag);
                }
            }
            _tags = new TagList(tempTags);

            if (!isNew)
                MarkOld();
            if (isClean)
                MarkClean();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<table");
            builder.Append(" id=\"" + _id.ToString() + "\"");
            builder.Append(" name=\"" + _name + "\"");
            builder.Append(" alias=\"" + _alias + "\"");
            builder.Append(" database=\"" + _database + "\"");
            builder.Append(" isView=\"" + (_isView ? "1" : "0") + "\"");
            builder.Append(" primaryKey=\"" + _primaryKey.ToString() + "\"");
            builder.Append(" >");
            foreach (FieldBO field in _fields)
                builder.Append(field.ToString());

            if (_tags != null && _tags.Count > 0)
            {
                builder.Append("<tags>");
                foreach (string tag in _tags)
                {
                    builder.Append("<tag>");
                    builder.Append(tag);
                    builder.Append("</tag>");
                }
                builder.Append("</tags>");
            }

            builder.Append("</table>");
            return builder.ToString();
        }

        internal string ToString(bool noMasterSchemFields)
        {
            StringBuilder builder = new StringBuilder("<table");
            builder.Append(" id=\"" + _id.ToString() + "\"");
            builder.Append(" name=\"" + _name + "\"");
            builder.Append(" alias=\"" + _alias + "\"");
            builder.Append(" database=\"" + _database + "\"");
            builder.Append(" isView=\"" + (_isView ? "1" : "0") + "\"");
            builder.Append(" primaryKey=\"" + _primaryKey.ToString() + "\"");
            builder.Append(" >");
            foreach (FieldBO field in _fields)
            {
                if (noMasterSchemFields)
                {
                    if (!field.FromMasterSchema)
                        builder.Append(field.ToString());
                }
                else
                {
                    builder.Append(field.ToString());
                }
            }

            if (_tags != null && _tags.Count > 0)
            {
                builder.Append("<tags>");
                foreach (string tag in _tags)
                {
                    builder.Append("<tag>");
                    builder.Append(tag);
                    builder.Append("</tag>");
                }
                builder.Append("</tags>");
            }

            builder.Append("</table>");
            return builder.ToString();
        }

        public int GetHighestIdField() 
        {
            int highestID = -1;
            foreach (FieldBO field in this._fields) 
            {
                if (field.ID > highestID)
                    highestID = field.ID;
            }
            return highestID;
        }

        #endregion

    }

    [Serializable]
    public class TagList : System.Collections.IEnumerable, IEnumerable<string>
    {
        List<string> _implementation;
        bool _isDirty = false;

        public bool IsDirty { get { return _isDirty; } }
        public TagList()
        {
            _implementation = new List<string>();
        }

        public TagList(List<string> list)
        {
            _implementation = list;
        }

        public void Add(string val)
        {
            if (!_implementation.Contains(val))
            {
                _implementation.Add(val);
                _isDirty = true;
            }
        }

        public void Remove(string val)
        {
            if (_implementation.Contains(val))
            {
                _implementation.Remove(val);
                _isDirty = true;
            }
        }

        public bool Contains(string val)
        {
            return _implementation.Contains(val);
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _implementation.GetEnumerator();
        }

        #endregion

        public override int GetHashCode()
        {
            return _implementation.GetHashCode();
        }

        public int Count
        {
            get { return _implementation.Count; }
        }

        #region IEnumerable<string> Members

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return _implementation.GetEnumerator();
        }

        #endregion
    }
}

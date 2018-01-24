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
    [Serializable]
    public class FieldBO : Csla.BusinessBase<FieldBO>
    {
        #region Variables

        private int _id = int.MinValue;
        private string _name = String.Empty;
        internal string _alias = String.Empty;
        private string _database = String.Empty;
        private COEDataView.AbstractTypes _dataType = COEDataView.AbstractTypes.Integer;
        private COEDataView.IndexTypes _indexType = COEDataView.IndexTypes.NONE;
        private COEDataView.MimeTypes _mimeType = COEDataView.MimeTypes.NONE;
        private bool _fromMasterSchema = false;
        private int _lookupFieldId = int.MinValue;
        private int _lookupDisplayFieldId = int.MinValue;
        private string _xml = String.Empty;
        private int _highestID = -1;
        private int _sortOrder = -1;
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("FieldBO");
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "FieldBO";
        private readonly string _fieldsSeparator = "|";
        private bool _visible = true;
        private bool _isDefault = false;
        private bool _isUniqueKey = false;
        private COEDataView.SortDirection _lookupSortOrder = COEDataView.SortDirection.ASCENDING;
        //new variable for checking field has index - PP on 29Jan2013
        private bool _isIndexed = false;
        // Added variable "_isDefaultQuery" for property "IsDefaultQuery
        private bool _isDefaultQuery = false;
        private string _indexName = string.Empty;	//CBOE-529 added variable _indexname for indexname proerty  ASV 27032013
        #endregion

        #region Enums
        /// <summary>
        /// Object's properties names. Used to avoid string hardcoding
        /// </summary>
        private enum Properties
        {
            ID,
            Name,
            Alias,
            DataType,
            IndexType,
            MimeType,
            LookupFieldId,
            LookupDisplayFieldId,
            Xml,
            FromMasterSchema,
            Database,
            Visible,
            IsDefault,
            LookupSortOrder,
            SortOrder,
            IsUniqueKey,
            IsIndexed,
            IsDefaultQuery,
            IndexName			//CBOE-529 added indexname property enum
        }

        #endregion

        #region Properties
        /// <summary>
        /// Field id
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
        /// Name of the field
        /// </summary>
        public string Name
        {
            get
            {
                _name = _name.Replace("%", "&#37;").Replace("<", "&lt;").Replace(">", "&gt;");
                //_name = System.Web.HttpUtility.HtmlDecode(_name);
                return _name;
            }
            set
            {
                CanWriteProperty(Properties.Name.ToString(), true);
                if (value == null)
                {
                    string key = this.GetKey();
                    _name = String.Empty;
                    PropertyHasChanged(Properties.Name.ToString());
                }
                else if (value != _name)
                {
                    _name = ReplaceSpecialCharacters(value);
                    PropertyHasChanged(Properties.Name.ToString());
                    //_name = System.Web.HttpUtility.HtmlEncode(value);
                    //PropertyHasChanged(Properties.Name.ToString());
                }
            }
        }

        

        /// <summary>
        /// Alias of the field
        /// </summary>
        public string Alias
        {
            get
            {
                //Session object not null means call from Web application otherwise call from DeskTop application
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session.Count > 0)
                {
                    string result = _alias;
                    int iAmp = 0;
                    string strCurrentValue = result;
                    do
                    {
                        iAmp = strCurrentValue.IndexOf("&amp;"); //Gets index of "&amp;" string
                        if (iAmp > -1 && strCurrentValue.Length >= (iAmp + 5 + 3))
                        {
                            //Checking next 3 or 4 characters whether they are "lt;", "gt;", "#37;" or "amp;" for replacing "&smp;" with "&" character
                            // Added 5 for "&amp;" characters and 3 for "lt;", "gt;", "#37;" characters in "iAmp" variable
                            if (strCurrentValue.Substring(iAmp + 5, 3) == "lt;" || strCurrentValue.Substring(iAmp + 5, 3) == "gt;"
                                || (strCurrentValue.Length >= (iAmp + 5 + 4) && (strCurrentValue.Substring(iAmp + 5, 4) == "#37;" || strCurrentValue.Substring(iAmp + 5, 4) == "amp;")))
                            {
                                strCurrentValue = strCurrentValue.Remove(iAmp + 1, 4);
                                result = result.Remove(iAmp + 1, 4);
                                iAmp = strCurrentValue.IndexOf("&amp;");
                            }
                            //If next 3 or 4 characters are not "lt;", "gt;", "#37;" or "amp;" then keeping "&amp;" as it is and moving to next part of the input string
                            //Replacing "&amp;" with "XXXXX" string to complete the While loop otherwise loop will go in Infinite loop
                            else if (strCurrentValue.Substring(iAmp, 5) == "&amp;")
                            {
                                strCurrentValue = strCurrentValue.Remove(iAmp, 5);
                                strCurrentValue = strCurrentValue.Insert(iAmp, "XXXXX");
                            }
                        }
                        else
                        {
                            iAmp = -1;
                        }
                    }
                    while (iAmp > -1);

                    _alias = result;
                }
                //When call came from Desktop or Window application then below "ElseIf" will get executed. 
                // If string contains any one of "lt;", "gt;", "#37;" or "amp;" strings then applying HtmlDecoding 
                else if (_alias.Contains("&lt;") || _alias.Contains("&gt;") || _alias.Contains("&amp;") || _alias.Contains("&#37;"))
                {
                    _alias = System.Web.HttpUtility.HtmlDecode(_alias);
                    _alias = System.Web.HttpUtility.HtmlDecode(_alias);
                }

                return _alias;
            }
            set
            {
                CanWriteProperty(Properties.Alias.ToString(), true);
                if (value == null)
                {
                    string key = this.GetKey();
                    _alias = String.Empty;
                    PropertyHasChanged(Properties.Alias.ToString());
                }
                else
                {
                    _alias = ReplaceSpecialCharacters(value);
                    PropertyHasChanged(Properties.Alias.ToString());
                }

            }
        }
        /// <summary>
        /// Replace the Special Characters
        /// </summary>
        /// <param name="aliasValue"></param>
        /// <returns></returns>
        private string ReplaceSpecialCharacters(string aliasValue)
        {
            string result = aliasValue;
            int iAmp = 0;
            string strCurrentValue = result;
            bool isEncode = false;
            do
            {
                if (strCurrentValue.IndexOf("&lt;") > -1 || strCurrentValue.IndexOf("&gt;") > -1 || strCurrentValue.IndexOf("&amp;") >-1 || strCurrentValue.IndexOf("&#37;") > -1)
                {
                    isEncode = true;
                }
                iAmp = strCurrentValue.IndexOf("&amp;"); //Gets index of "&amp;" string
                if (iAmp > -1 && strCurrentValue.Length >= (iAmp + 5 + 3))
                {
                    //Checking next 3 or 4 characters whether they are "lt;", "gt;", "#37;" or "amp;" for replacing "&smp;" with "&" character
                    // Added 5 for "&amp;" characters and 3 for "lt;", "gt;", "#37;" characters in "iAmp" variable
                    if (strCurrentValue.Substring(iAmp + 5, 3) == "lt;" || strCurrentValue.Substring(iAmp + 5, 3) == "gt;"
                        || (strCurrentValue.Length >= (iAmp + 5 + 4) && (strCurrentValue.Substring(iAmp + 5, 4) == "#37;" || strCurrentValue.Substring(iAmp + 5, 4) == "amp;")))
                    {
                        strCurrentValue = strCurrentValue.Remove(iAmp + 1, 4);
                        result = result.Remove(iAmp + 1, 4);
                        iAmp = strCurrentValue.IndexOf("&amp;");
                        isEncode = true; //If Encode string is already exist then set to True
                    }
                    //If next 3 or 4 characters are not "lt;", "gt;", "#37;" or "amp;" then keeping "&amp;" as it is and moving to next part of the input string
                    //Replacing "&amp;" with "XXXXX" string to complete the While loop otherwise loop will go in Infinite loop
                    else if (strCurrentValue.Substring(iAmp, 5) == "&amp;")
                    {
                        strCurrentValue = strCurrentValue.Remove(iAmp, 5);
                        strCurrentValue = strCurrentValue.Insert(iAmp, "XXXXX");
                    }
                }
                else
                {
                    iAmp = -1;
                }
            }
            while (iAmp > -1);

            //If string is not encoded and string contains "<", ">", "&" or "%" character then applying HtmlEncoding to the string.
            if (!isEncode && (result.Contains("<") || result.Contains(">") || result.Contains("&") || result.Contains("%")))
            {
                result = System.Web.HttpUtility.HtmlEncode(result);
            }
            return result;
        }

        /// <summary>
        /// Field's database
        /// </summary>
        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                CanWriteProperty(Properties.Database.ToString(), true);
                if (value != _database)
                {
                    _database = value;
                    PropertyHasChanged(Properties.Database.ToString());
                }
            }
        }

        /// <summary>
        /// Field's datatype
        /// </summary>
        public COEDataView.AbstractTypes DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                CanWriteProperty(Properties.DataType.ToString(), true);
                if (value != _dataType)
                {
                    _dataType = value;
                    PropertyHasChanged(Properties.DataType.ToString());
                }
            }
        }

        /// <summary>
        /// Indicates if the field was inherited (not selected by the user)
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
        /// Field's index type
        /// </summary>
        public COEDataView.IndexTypes IndexType
        {
            get
            {
                return _indexType;
            }
            set
            {
                CanWriteProperty(Properties.IndexType.ToString(), true);
                if (value != _indexType)
                {
                    _indexType = value;
                    PropertyHasChanged(Properties.IndexType.ToString());
                }
            }
        }

        /// <summary>
        /// Field's mime type
        /// </summary>
        public COEDataView.MimeTypes MimeType
        {
            get
            {
                return _mimeType;
            }
            set
            {
                CanWriteProperty(Properties.MimeType.ToString(), true);
                if (value != _mimeType)
                {
                    _mimeType = value;
                    PropertyHasChanged(Properties.MimeType.ToString());
                }
            }
        }

        /// <summary>
        /// If the field is a lookup field, the joining field
        /// </summary>
        public int LookupFieldId
        {
            get
            {
                return _lookupFieldId;
            }
            set
            {
                CanWriteProperty(Properties.LookupFieldId.ToString(), true);
                if (value != _lookupFieldId)
                {
                    _lookupFieldId = value;
                    PropertyHasChanged(Properties.LookupFieldId.ToString());
                }
            }
        }

        /// <summary>
        /// If the field is a lookup field, the field to use as display field
        /// </summary>
        public int LookupDisplayFieldId
        {
            get
            {
                return _lookupDisplayFieldId;
            }
            set
            {
                CanWriteProperty(Properties.LookupDisplayFieldId.ToString(), true);
                if (value != _lookupDisplayFieldId)
                {
                    _lookupDisplayFieldId = value;
                    PropertyHasChanged(Properties.LookupDisplayFieldId.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the field's visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                CanWriteProperty(Properties.Visible.ToString(), true);
                if (value != _visible)
                {
                    _visible = value;
                    PropertyHasChanged(Properties.Visible.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the field's default
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                CanWriteProperty(Properties.IsDefault.ToString(), true);
                if (value != _isDefault)
                {
                    _isDefault = value;
                    PropertyHasChanged(Properties.IsDefault.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the fields index applied value
        /// </summary>
        public bool IsIndexed
        {
            get
            {
                return _isIndexed;
            }
            set
            {
                CanWriteProperty(Properties.IsIndexed.ToString(), true);
                if (value != _isIndexed)
                {
                    _isIndexed = value;
                    PropertyHasChanged(Properties.IsIndexed.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets if the field is a unique key
        /// </summary>
        public bool IsUniqueKey
        {
            get
            {
                return _isUniqueKey;
            }
            set
            {
                CanWriteProperty(Properties.IsUniqueKey.ToString(), true);
                if (value != _isUniqueKey)
                {
                    _isUniqueKey = value;
                    PropertyHasChanged(Properties.IsUniqueKey.ToString());
                }
            }
        }

        /// <summary>
        /// If the field is a lookup field, the sort order of it
        /// </summary>
        public COEDataView.SortDirection LookupSortOrder
        {
            get
            {
                return _lookupSortOrder;
            }
            set
            {
                CanWriteProperty(Properties.LookupSortOrder.ToString(), true);
                if (value != _lookupSortOrder)
                {
                    _lookupSortOrder = value;
                    PropertyHasChanged(Properties.LookupSortOrder.ToString());
                }
            }
        }

        /// <summary>
        /// Keeps the value of the Highest ID (Tables and Field) in order to have a quick access and asignation when you create a new field.
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
                {
                    _highestID = value;
                }
            }
        }

        /// <summary>
        /// Order of the field in the table. Mostly for presentation stuff.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                CanWriteProperty(Properties.SortOrder.ToString(), true);
                if (value < 0)
                    _sortOrder = -1;
                else if (value != _sortOrder)
                {
                    _sortOrder = value;
                    PropertyHasChanged(Properties.SortOrder.ToString());
                }
            }
        }

        /// <summary>
        /// Gets if the field is new
        /// </summary>
        public bool IsNew
        {
            get
            {
                return base.IsNew;
            }
        }

        /// <summary>
        /// Indicates if the field is in a valid state
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return base.IsValid && !string.IsNullOrEmpty(_name) && _id >= 0;
            }
        }

        /// <summary>
        /// Indicates if the field is dirty
        /// </summary>
        public override bool IsDirty
        {
            get { return base.IsDirty; }
        }

        /// <summary>
        /// Index of the current object in the collection that belongs.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string OrderIndex
        {
            get
            {
                return string.Format("{0:00}", ((FieldListBO)base.Parent).IndexOf(this) + 1);
            }
        }

        /// <summary>
        /// Gets or sets the field's Default Query
        /// </summary>
        public bool IsDefaultQuery
        {
            get
            {
                return _isDefaultQuery;
            }
            set
            {
                CanWriteProperty(Properties.IsDefaultQuery.ToString(), true);
                if (value != _isDefaultQuery)
                {
                    _isDefaultQuery = value;
                    PropertyHasChanged(Properties.IsDefaultQuery.ToString());
                }
            }
        }

        //CBOE-529 IndexName proerty   ASV 27032013
        public string IndexName
        {
            get
            {
                return _indexName;
            }
            set
            {
                CanWriteProperty(Properties.IndexName.ToString(), true);
                if (value != _indexName)
                {
                    _indexName = value;
                    PropertyHasChanged(Properties.IndexName.ToString());
                }
            }

        }

        /// <summary>
        /// If the data field can be specified in the ORDER BY clause of a query.
        /// </summary>
        /// <returns></returns>
        public bool IsSortable()
        {
            // Structure field is not sortable.
            bool isStructureField = COEDataView.IsStructureIndexType(this.IndexType) || COEDataView.IsStructureContentType(this.MimeType);
            if (isStructureField)
            {
                return false;
            }

            // You cannot specify LOB columns in the ORDER BY clause of a query
            return (this._dataType == COEDataView.AbstractTypes.Boolean)
                   || (this._dataType == COEDataView.AbstractTypes.Date)
                   || (this._dataType == COEDataView.AbstractTypes.Integer)
                   || (this._dataType == COEDataView.AbstractTypes.Real)
                   || (this._dataType == COEDataView.AbstractTypes.Text);
        }

        #endregion

        #region Overrided methods
        /// <summary>
        /// Unique identifier of the object. This value is used by CSLA's implementation of System.Object overrides, such us Equals()
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Business rules should be added here
        /// </summary>
        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }
        #endregion

        #region Constructors

        internal FieldBO(string name, string alias, string database)
        {
            _name = name;
            _alias = System.Web.HttpUtility.HtmlEncode(alias);
            _database = database;
            this.MarkAsChild();
            //this.MarkDirty();
        }

        internal FieldBO(COEDataView.Field field, string database)
            : this(field.Name, field.Alias, database)
        {
            _id = field.Id;
            _dataType = field.DataType;
            _indexType = field.IndexType;
            _lookupFieldId = field.LookupFieldId;
            _lookupDisplayFieldId = field.LookupDisplayFieldId;
            _xml = field.ToString();
            _visible = field.Visible;
            _isDefault = field.IsDefault;
            _lookupSortOrder = field.LookupSortOrder;
            _mimeType = field.MimeType;
            _isUniqueKey = field.IsUniqueKey;
            _isDefaultQuery = field.IsDefaultQuery; //Value of IsDefaultQuery property of FIELD class is set to "_isDefaultQuery" variable of FIELDBO class.
            _isIndexed = field.IsIndexed;
            _indexName = field.IndexName;		//CBOE-529 Value of IndexName property of FIELD class is set to "_indexName" variable of FIELDBO class.  ASV 27032013
        }

        internal FieldBO(COEDataView.Field field, string database, bool isClean)
            : this(field, database)
        {
            if (isClean)
                this.MarkClean();
            else
                this.MarkDirty();
        }
        #endregion

        #region Validaton Rules

        private void AddCommonRules()
        {
            //Examples from other class
            //ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            //ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 50));
            //ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            //ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
        }

        //public List<BrokenRuleDescription> GetBrokenRulesDescription()
        //{
        //    List<BrokenRuleDescription> brokenRules = new List<BrokenRuleDescription>();

        //    if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
        //    {
        //        brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
        //    }
        //    return brokenRules;
        //}

        #endregion

        #region Factory Methods

        public static FieldBO NewField(COEDataView.Field field, string database)
        {
            try
            {
                return new FieldBO(field, database);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static FieldBO NewField(COEDataView.Field field, string database, bool isClean)
        {
            try
            {
                return new FieldBO(field, database, isClean);
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
            if (criteria._field != null)
                this.Initialize(criteria._field, true, true);
        }

        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            internal COEDataView.Field _field;

            public Criteria(COEDataView.Field field)
            {
                _field = field;
            }
        }

        #endregion

        #region DALLoader

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }

        #endregion

        #region Methods

        private string GetKey()
        {
            return _database + _fieldsSeparator + _name + _fieldsSeparator + _alias;
        }

        internal void AddTextToAlias(string text)
        {
            _alias += text;
        }

        private void Initialize(COEDataView.Field field, bool isNew, bool isClean)
        {
            _id = field.Id;
            _name = field.Name;
            _alias = System.Web.HttpUtility.HtmlEncode(field.Alias);
            _dataType = field.DataType;
            _indexType = field.IndexType;
            _mimeType = field.MimeType; //
            _lookupFieldId = field.LookupFieldId;
            _lookupDisplayFieldId = field.LookupDisplayFieldId;
            _visible = field.Visible;
            _isDefault = field.IsDefault;
            _lookupSortOrder = field.LookupSortOrder;
            _xml = field.ToString();
            _isUniqueKey = field.IsUniqueKey;
            _isIndexed = field.IsIndexed;
            _isDefaultQuery = field.IsDefaultQuery; //Value of IsDefaultQuery property of FIELD class is set to "_isDefaultQuery" variable of FIELDBO class
            _indexName = field.IndexName;		//CBOE-529 Value of IndexName property of FIELD class is set to "_indexName" variable of FIELDBO class
            if (!isNew)
                MarkOld();
            if (isClean)
                MarkClean();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<fields");
            builder.Append(" id=\"" + _id.ToString() + "\"");
            builder.Append(" name=\"" + _name + "\"");
            builder.Append(" alias=\"" + _alias + "\"");
            builder.Append(" dataType=\"" + _dataType.ToString().ToUpper() + "\"");
            builder.Append(" indexType=\"" + _indexType.ToString().ToUpper() + "\"");
            builder.Append(" mimeType=\"" + _mimeType.ToString().ToUpper() + "\"");
            builder.Append(" visible=\"");
            builder.Append(_visible ? "1" : "0");
            builder.Append("\"");
            builder.Append(" isDefault=\"");
            builder.Append(_isDefault ? "1" : "0");
            builder.Append("\"");
            builder.Append(" isUniqueKey=\"");
            builder.Append(_isUniqueKey ? "1" : "0");
            builder.Append("\"");
            builder.Append(" isIndexed=\"");
            builder.Append(_isIndexed ? "1" : "0");
            builder.Append("\"");
            builder.Append(" isDefaultQuery=\"");
            builder.Append(_isDefaultQuery ? "1" : "0");
            builder.Append("\"");

            if (_lookupDisplayFieldId >= 0)
                builder.Append(" lookupDisplayFieldId=\"" + _lookupDisplayFieldId.ToString() + "\"");
            if (_lookupFieldId >= 0)
                builder.Append(" lookupFieldId=\"" + _lookupFieldId.ToString() + "\"");
            if (_lookupDisplayFieldId >= 0 && _lookupFieldId >= 0)
                builder.Append(" lookupSortOrder=\"" + _lookupSortOrder.ToString().ToUpper() + "\"");
            if (_sortOrder > -1)
                builder.Append(" sortOrder=\"" + _sortOrder.ToString() + "\"");
            builder.Append(" indexname=\"" + _indexName + "\"");		//CBOE-529 added value of indexname 	ASV 27032013
            builder.Append(" />");
            return builder.ToString();
        }

        #endregion
    }
}

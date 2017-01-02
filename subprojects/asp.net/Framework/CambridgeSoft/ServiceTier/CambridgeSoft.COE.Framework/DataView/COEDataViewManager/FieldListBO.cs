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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class FieldListBO : BusinessListBase<FieldListBO, FieldBO>
    {
        #region Variables
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("COEDataViewTableListBO");
        [NonSerialized]
        private SortedBindingList<FieldBO> _sortedList;
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDataViewTableListBO";
        private int _highestID = -1;
        private NameValueCollection _simpleFieldKeys;
        private Dictionary<string, string> _fullFieldKeys;
        private NameValueCollection _lookupFields;
        private Dictionary<int, FieldBO> _idToField;
        private readonly string _fieldsSeparator = "|";
        [NonSerialized]
        List<CustomBrokenRule> _customBrokenRules = new List<CustomBrokenRule>();

        #endregion

        #region Events
        protected override void InsertItem(int index, FieldBO item)
        {
            this.AddEntryToLists(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, FieldBO item)
        {
            this.UpdateEntryFromLists(this[index], item);
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this.RemoveEntryFromLists(this[index]);
            base.RemoveItem(index);
        }

        #endregion

        #region Properties

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
                foreach (FieldBO field in this)
                    field.HighestID = value;
            }
        }

        public Dictionary<string, string> NameValue
        {
            get
            {
                return _fullFieldKeys;
            }
        }

        public List<BrokenRule> BrokenRules
        {
            get
            {
                List<BrokenRule> brokenRules = new List<BrokenRule>();
                foreach (FieldBO field in this)
                {
                    foreach (BrokenRule rule in field.BrokenRulesCollection)
                        brokenRules.Add(rule);
                }
                return brokenRules;
            }
        }

        /// <summary>
        /// This list contains the error ocurred in object validations
        /// </summary>
        /// <remarks>We cannot create BrokenRules because that class has no Constructor/Public method. It works with ValidationRules</remarks>
        public List<CustomBrokenRule> CustomBrokenRules
        {
            get
            {
                return _customBrokenRules;
            }
        }

        #endregion

        #region Overrided Properties

        public override bool IsValid
        {
            get
            {
                return base.IsValid;
            }
        }

        /// <summary>
        /// Indicates if the object was changed or removed.
        /// </summary>
        public bool IsDirty
        {
            get { if (this.DeletedList.Count > 0) return true; else return base.IsDirty; }
        }

        #endregion

        #region Factory Methods

        internal static FieldListBO NewFieldList(List<COEDataView.Field> fields, string database)
        {
            return new FieldListBO(fields, database);
        }

        internal static FieldListBO NewFieldList(List<COEDataView.Field> fields, string database, bool isClean)
        {
            return new FieldListBO(fields, database, isClean);
        }
        #endregion

        #region DALLoader

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
        }
        #endregion

        #region Misc Methods

        public SortedBindingList<FieldBO> ApplySort(string property, ListSortDirection direction)
        {
            if (IsAValidProperty(property))
                _sortedList.ApplySort(property, direction);
            return _sortedList;
        }

        private bool IsAValidProperty(string property)
        {
            bool retVal = true;
            //TODO: Check that is a valid Property.
            return retVal;
        }

        private void CheckHighestID(int id)
        {
            if (id > _highestID)
                _highestID = id;
        }

        public FieldBO GetField(int Id)
        {
            FieldBO result = null;
            try
            {
                _idToField.TryGetValue(Id, out result);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;// I have to move this in order to maintain style
        }

        /// <summary>
        /// Returns a field given the database name and the field's name 
        /// </summary>
        /// <param name="database">name of the database/schema</param>
        /// <param name="name">Name of the fields</param>
        /// <returns>Found field or null</returns>
        /// <remarks>We do NOT check for alias cause this method is meant to be used to compare against masterDV fields.</remarks>
        public FieldBO GetField(string database, string name)
        {
            FieldBO result = null;
            try
            {
                string foundFieldId = _simpleFieldKeys[this.BuildKey(database, name)];
                if (!string.IsNullOrEmpty(foundFieldId))
                {
                    if (foundFieldId.Contains(","))
                        foundFieldId = foundFieldId.Remove(foundFieldId.IndexOf(","));
                    _idToField.TryGetValue(int.Parse(foundFieldId), out result);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        /// <summary>
        /// Returns a field given the database name and the field's name 
        /// </summary>
        /// <param name="database">name of the database/schema</param>
        /// <param name="name">Name of the field</param>
        /// <param name="alias">Alias of the field</param>
        /// <returns>Found field or null</returns>
        /// <remarks>We do NOT check for alias cause this method is meant to be used to compare against masterDV fields.</remarks>
        public FieldBO GetField(string database, string name, string alias)
        {
            FieldBO result = null;
            try
            {
                string foundFieldId = null;
                if (_fullFieldKeys.ContainsKey(this.BuildKey(database, name, alias)))
                    foundFieldId = _fullFieldKeys[this.BuildKey(database, name, alias)];

                if (!string.IsNullOrEmpty(foundFieldId))
                {
                    _idToField.TryGetValue(int.Parse(foundFieldId), out result);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        internal List<FieldBO> GetFields(int fieldID)
        {
            List<FieldBO> result = new List<FieldBO>();
            try
            {
                FieldBO field = this.GetField(fieldID);
                if (field != null)
                {
                    string foundFieldId = null;
                    if (_fullFieldKeys.ContainsKey(this.BuildKey(field.Database, field.Name, field.Alias)))
                        foundFieldId = _fullFieldKeys[this.BuildKey(field.Database, field.Name, field.Alias)];

                    if (!string.IsNullOrEmpty(foundFieldId))
                    {
                        foreach (string id in foundFieldId.Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            FieldBO fld = null;
                            _idToField.TryGetValue(int.Parse(id), out fld);
                            if (fld != null)
                                result.Add(fld);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        internal List<FieldBO> GetFieldsByLookup(int lookupField)
        {
            List<FieldBO> result = new List<FieldBO>();
            try
            {
                string lookupAsString = lookupField.ToString();
                string[] foundKeys = _lookupFields.GetValues(lookupAsString);
                if (foundKeys != null)
                {
                    foreach (string key in foundKeys)
                    {
                        if (_fullFieldKeys.ContainsKey(key))
                            result.Add(this.GetField(int.Parse(_fullFieldKeys[key])));
                    }
                }

            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }
        /// <summary>
        /// This method checks if this field involve in lookup. if no then it will remove the entry from _lookupFields.
        /// </summary>
        /// <param name="fieldId">Id of Field.</param>
        public void RemoveEntryFromLookUp(int fieldId)
        {
            try
            {
                if (fieldId != null)
                {
                    string[] foundKeys = _lookupFields.GetValues(Convert.ToString(fieldId));
                    if (foundKeys != null && foundKeys.Length > 0)
                    {
                        _lookupFields.Remove(Convert.ToString(fieldId));
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }
        public List<FieldBO> GetAllFieldsByLookup()
        {
            List<FieldBO> result = new List<FieldBO>();
            try
            {
                foreach (string id in _lookupFields.AllKeys)
                {
                    foreach (string keyList in _lookupFields.GetValues(id))
                    {
                        int index = keyList.LastIndexOf('|');
                        string keyListTrimmed = keyList.Substring(0, index).ToString();
                        foreach (string key in _fullFieldKeys.Keys)
                        {
                            int indexKey = key.LastIndexOf('|');
                            string keyTrimmed = key.Substring(0, indexKey).ToString();
                            if (keyTrimmed == keyListTrimmed)
                            {
                                FieldBO field = this.GetField(int.Parse(_fullFieldKeys[key]));
                                if (!result.Contains(field))
                                    result.Add(field);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        internal bool Exists(int Id)
        {
            return _idToField.ContainsKey(Id);
        }

        internal bool Exists(FieldBO fieldBO)
        {
            return _fullFieldKeys.ContainsKey(this.BuildKey(fieldBO.Database, fieldBO.Name, fieldBO.Alias));
        }

        /// <summary>
        /// Method to clone a table (and fields inside) and add it to the current TableListBO.
        /// This is to create Table Alias
        /// </summary>
        /// <param name="tableID">Id of the table that the clone will be based on</param>
        public FieldBO CloneAndAddField(int fieldID, string wordToAdd, int dvHighestID)
        {
            FieldBO newField = null;
            try
            {
                newField = this.GetField(fieldID).Clone();
                if (newField.ID <= _highestID)
                {
                    if (_highestID <= dvHighestID)
                        _highestID = dvHighestID;

                    newField.ID = dvHighestID;// this.GetID();
                    //if(this.IDChanged != null)
                    //    this.IDChanged(this, new CustomEventArgs(newField.ID));
                }
                //We call this method in order to make sure each table/field is unique. See more details in method's comments
                if (string.IsNullOrEmpty(wordToAdd))
                    wordToAdd = "_Alias"; //TODO: Move to Resources file.
                if (string.IsNullOrEmpty(newField.Alias))
                    wordToAdd = newField.Name + wordToAdd;
                newField.AddTextToAlias(wordToAdd);
                while (_fullFieldKeys.ContainsKey(this.BuildKey(newField.Database, newField.Name, newField.Alias)))
                {
                    newField.AddTextToAlias("1");
                }

                newField.LookupDisplayFieldId = int.MinValue;
                newField.LookupFieldId = int.MinValue;
                //Add the value to the hash table for better searching performance
                this.Add(newField);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return newField;
        }

        public void Remove(int fieldId)
        {
            try
            {
                if (fieldId >= 0)
                {
                    FieldBO field = this.GetField(fieldId);
                    if (field != null)
                    {
                        this.RemoveEntryFromLists(field);
                        base.Remove(field);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private void AddEntryToLists(FieldBO field)
        {
            try
            {
                string fullkey = this.BuildKey(field.Database, field.Name, field.Alias);
                if (!_fullFieldKeys.ContainsKey(fullkey))
                {
                    field.PropertyChanged += new PropertyChangedEventHandler(field_PropertyChanged);

                    _simpleFieldKeys.Add(this.BuildKey(field.Database, field.Name), field.ID.ToString());
                    _fullFieldKeys.Add(fullkey, field.ID.ToString());
                    if (field.LookupDisplayFieldId >= 0)
                        _lookupFields.Add(field.LookupDisplayFieldId.ToString(), fullkey);
                    if (field.LookupFieldId >= 0)
                        _lookupFields.Add(field.LookupFieldId.ToString(), fullkey);
                    _idToField.Add(field.ID, field);
                    this.CheckHighestID(field.ID);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        void field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "ID")
                {
                    FieldBO field = sender as FieldBO;
                    // Coverity Fix CID - 10312 (from local server)
                    if (field != null)
                    {
                        int newId = field.ID;
                        string key = this.BuildKey(field.Database, field.Name, field.Alias);
                        if (_fullFieldKeys.ContainsKey(key))
                        {
                            string oldIdStr = _fullFieldKeys[key];
                            field.ID = int.Parse(oldIdStr);
                            this.RemoveEntryFromLists(field);
                            field.ID = newId;
                            this.AddEntryToLists(field);
                        }
                    }
                }
                if (e.PropertyName == "Alias")
                {
                    FieldBO field = sender as FieldBO;
                    string newId = (field != null) ? field.ID.ToString() : string.Empty;

                    string oldFullKey = string.Empty;
                    string newFullKey = string.Empty;
                    if (_fullFieldKeys.ContainsValue(newId))
                    {
                        foreach (string key in _fullFieldKeys.Keys)
                        {
                            if (field != null && _fullFieldKeys[key] == newId)
                            {
                                newFullKey = this.BuildKey(field.Database, field.Name, field._alias);
                               
                                oldFullKey = key;
                                _fullFieldKeys.Remove(oldFullKey);
                                if (!_fullFieldKeys.ContainsKey(newFullKey))
                                    _fullFieldKeys.Add(newFullKey, newId);
                                /* CSBR-162170 : Creating an Alias Field is automatically displaying it as Lookup in Table Summary
                                 * Checking whether the field lookupid field has a valid value so as to differentiate between the alias with lookup and alias without lookup */
                                if (field.LookupFieldId != int.MinValue) // fixed 162170
                                {
                                    //Fix for CSBR-160543:Lookup relation is lost on dataview validation summary after changing a Alias name & validation message is appearing asking the user to specify the relationship for Lookup table
                                    string lookUpKey = field.LookupFieldId.ToString();
                                    if (field.LookupFieldId != -1)
                                    {
                                        string[] oldValues = _lookupFields.GetValues(lookUpKey);

                                        if (oldValues != null)
                                        {
                                            if (oldValues.Length < 2 && lookUpKey.Equals(lookUpKey))
                                                _lookupFields.Remove(lookUpKey);
                                            else
                                            {
                                                List<string> newValues = new List<string>();
                                                for (int i = 0; i < oldValues.Length; i++)
                                                {
                                                    if (oldValues[i] != field.ID.ToString())
                                                        newValues.Add(oldValues[i]);
                                                }
                                                _lookupFields[lookUpKey] = string.Join(",", newValues.ToArray());
                                            }

                                        }
                                        _lookupFields.Add(lookUpKey, newFullKey);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (e.PropertyName == "LookupFieldId")
                {
                    ////Coverity Bug Fix :- CID : 11498  Jira Id :CBOE-194
                    FieldBO newField = sender as FieldBO;
                    if (newField != null)
                    {
                        string fullkey = this.BuildKey(newField.Database, newField.Name, newField.Alias);
                        if (_fullFieldKeys.ContainsKey(fullkey))
                        {
                            FieldBO field = _idToField[int.Parse(_fullFieldKeys[fullkey])];
                            string key = field.LookupFieldId.ToString();

                            string[] oldValues = _lookupFields.GetValues(key);

                            if (oldValues != null)
                            {
                                if (oldValues.Length < 2 && key.Equals(fullkey))
                                    _lookupFields.Remove(key);
                                else
                                {
                                    List<string> newValues = new List<string>();
                                    for (int i = 0; i < oldValues.Length; i++)
                                    {
                                        if (oldValues[i] != field.ID.ToString())
                                            newValues.Add(oldValues[i]);
                                    }
                                    _lookupFields[key] = string.Join(",", newValues.ToArray());
                                }
                            }
                        }
                        _lookupFields.Add(newField.LookupFieldId.ToString(), fullkey);
                    }
                }
                else if (e.PropertyName == "LookupDisplayFieldId")
                {
                    //FieldBO newField = sender as FieldBO;
                    ////Coverity Bug Fix :- CID : 11498  Jira Id :CBOE-194
                    FieldBO newField = sender as FieldBO;
                    if (newField != null)
                    {
                        string fullkey = this.BuildKey(newField.Database, newField.Name, newField.Alias);
                        if (_fullFieldKeys.ContainsKey(fullkey))
                        {
                            FieldBO field = _idToField[int.Parse(_fullFieldKeys[fullkey])];
                            string key = field.LookupDisplayFieldId.ToString();

                            string[] oldValues = _lookupFields.GetValues(key);

                            if (oldValues != null)
                            {
                                if (oldValues.Length < 2 && key.Equals(fullkey))
                                    _lookupFields.Remove(key);
                                else
                                {
                                    List<string> newValues = new List<string>();
                                    for (int i = 0; i < oldValues.Length; i++)
                                    {
                                        if (oldValues[i] != field.ID.ToString())
                                            newValues.Add(oldValues[i]);
                                    }
                                    _lookupFields[key] = string.Join(",", newValues.ToArray());
                                }
                            }
                        }
                        _lookupFields.Add(newField.LookupDisplayFieldId.ToString(), fullkey);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private string BuildKey(string database, string name, string alias)
        {
            return database + _fieldsSeparator + name + _fieldsSeparator + alias;
        }

        private string BuildKey(string database, string name)
        {
            return database + _fieldsSeparator + name;
        }

        private void RemoveEntryFromLists(FieldBO field)
        {
            try
            {
                _idToField.Remove(field.ID);
                string simpleKey = this.BuildKey(field.Database, field.Name);
                string[] oldValues = _simpleFieldKeys.GetValues(simpleKey);
                if (oldValues != null)
                {
                    if (oldValues.Length < 2)
                        _simpleFieldKeys.Remove(simpleKey);
                    else
                    {
                        List<string> newValues = new List<string>();
                        for (int i = 0; i < oldValues.Length; i++)
                        {
                            if (oldValues[i] != field.ID.ToString())
                                newValues.Add(oldValues[i]);
                        }
                        _simpleFieldKeys[simpleKey] = string.Join(",", newValues.ToArray());
                    }
                }

                string fullKey = this.BuildKey(field.Database, field.Name, field.Alias);

                if (_fullFieldKeys.ContainsKey(fullKey))
                {
                    _fullFieldKeys.Remove(fullKey);
                }

                if (field.LookupFieldId >= 0)
                {
                    string key = field.LookupFieldId.ToString();
                    oldValues = _lookupFields.GetValues(key);
                    if (oldValues != null)
                    {
                        if (oldValues.Length < 2)
                            _lookupFields.Remove(key);
                        else
                        {
                            List<string> newValues = new List<string>();
                            for (int i = 0; i < oldValues.Length; i++)
                            {
                                if (oldValues[i] != field.ID.ToString())
                                    newValues.Add(oldValues[i]);
                            }
                            _lookupFields[key] = string.Join(",", newValues.ToArray());
                        }
                    }
                }
                if (field.LookupDisplayFieldId >= 0)
                {
                    string key = field.LookupDisplayFieldId.ToString();
                    oldValues = _lookupFields.GetValues(key);
                    if (oldValues != null)
                    {
                        if (oldValues.Length < 2)
                            _lookupFields.Remove(key);
                        else
                        {
                            List<string> newValues = new List<string>();
                            for (int i = 0; i < oldValues.Length; i++)
                            {
                                if (oldValues[i] != field.ID.ToString())
                                    newValues.Add(oldValues[i]);
                            }
                            _lookupFields[key] = string.Join(",", newValues.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private void UpdateEntryFromLists(FieldBO oldField, FieldBO newField)
        {
            this.RemoveEntryFromLists(oldField);
            this.AddEntryToLists(newField);
        }

        /// <summary>
        /// Get the next available id to assing to field
        /// </summary>
        /// <returns></returns>
        private int GetID()
        {
            return ++_highestID;
        }

        internal bool ExistsFieldKey(string database, string name)
        {
            bool retVal = false;
            try
            {
                string[] foundKeys = _simpleFieldKeys.GetValues(this.BuildKey(database, name));
                if (foundKeys != null)
                    retVal = foundKeys.Length > 0 ? true : false;
                return retVal;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return retVal;// To maintain Consistency in the Code.
        }

        private void AddCustomBrokenRule(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                bool mustAdd = true;
                //TODO: Improve this loop below to avoid looping. Maybe the CustomBroken class can have an ID that is a hash of the Description (a lot faster to compare!)
                foreach (CustomBrokenRule brokenRule in _customBrokenRules)
                {
                    mustAdd &= !brokenRule.DetailedDescription.Equals(errorMessage);
                    if (!mustAdd) break;
                }
                if (mustAdd)
                    this.CustomBrokenRules.Add(CustomBrokenRule.NewRule(COEDataViewManagerUtilities.ErrNumbers.InvalidField, errorMessage));
            }
        }

        internal void AddField(FieldBO field)
        {
            if (!_fullFieldKeys.ContainsKey(this.BuildKey(field.Database, field.Name, field.Alias)))
            {
                this.Add(field);
            }
        }

        private void InitializeArrays()
        {
            if (_sortedList == null)
                _sortedList = new SortedBindingList<FieldBO>(this);
            if (_fullFieldKeys == null)
                _fullFieldKeys = new Dictionary<string, string>();
            if (_simpleFieldKeys == null)
                _simpleFieldKeys = new NameValueCollection();
            if (_idToField == null)
                _idToField = new Dictionary<int, FieldBO>();
            if (_lookupFields == null)
                _lookupFields = new NameValueCollection();
        }
        #endregion

        #region Constructor

        internal FieldListBO()
        {
            InitializeArrays();
        }

        internal FieldListBO(List<COEDataView.Field> fields, string database)
            : this()
        {
            FieldBO newField;
            foreach (COEDataView.Field field in fields)
            {
                newField = FieldBO.NewField(field, database);
                //Add the value to the hash table for better searching performance
                this.Add(newField);
                this.CheckHighestID(newField.ID);
            }
        }

        internal FieldListBO(List<COEDataView.Field> fields, string database, bool isClean)
            : this()
        {
            FieldBO newField;
            foreach (COEDataView.Field field in fields)
            {
                newField = FieldBO.NewField(field, database, isClean);
                //Add the value to the hash table for better searching performance
                this.Add(newField);
                this.CheckHighestID(newField.ID);
            }
        }
        #endregion
    }
}

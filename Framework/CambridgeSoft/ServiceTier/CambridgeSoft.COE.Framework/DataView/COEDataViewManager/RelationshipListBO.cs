using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using Csla.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable()]
    public class RelationshipListBO : BusinessListBase<RelationshipListBO, RelationshipBO>
    {
        #region Variables
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("RelationshipListBO");
        [NonSerialized]
        private SortedBindingList<RelationshipBO> _sortedList;
        private Dictionary<string, RelationshipBO> _fullRelationshipKeys;
        private NameValueCollection _parentKeyList;
        private NameValueCollection _childKeyList;
        private Dictionary<string, RelationshipBO> _parentKeyAndChildKeyList;
        private NameValueCollection _parentIdList;
        private NameValueCollection _childIdList;
        private string _relationshipSeparator = "|";
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "RelationshipListBO";

        #endregion

        #region Overrided Properties

        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.CheckRelationShips();
            }
        }

        public bool IsDirty
        {
            get { if(DeletedList.Count > 0) return true; else return base.IsDirty; }
        }
        #endregion

        #region Properties

        public List<BrokenRule> BrokenRules
        {
            get
            {
                List<BrokenRule> brokenRules = new List<BrokenRule>();
                foreach(RelationshipBO relation in this)
                {
                    foreach(BrokenRule rule in relation.BrokenRulesCollection)
                        brokenRules.Add(rule);
                }
                return brokenRules;
            }
        }

        #endregion

        #region Factory Methods

        public static RelationshipListBO NewRelationShipListBO(List<COEDataView.Relationship> relationships)
        {
            try
            {
                return new RelationshipListBO(relationships);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static RelationshipListBO NewRelationShipListBO(List<COEDataView.Relationship> relationships, bool isClean)
        {
            try
            {
                return new RelationshipListBO(relationships, isClean);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static RelationshipListBO NewRelationShipListBO()
        {
            try 
            {
                return new RelationshipListBO();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
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
        protected override void InsertItem(int index, RelationshipBO item)
        {
            if(!this.Contains(item))
            {
                this.AddRelationshipToLists(item);
                base.InsertItem(index, item);
            }
        }

        protected override void SetItem(int index, RelationshipBO item)
        {
            this.UpdateRelationshipFromLists(this[index], item);
            base.SetItem(index, item);
        }

        private bool CheckRelationShips()
        {
            bool valid = true;
            foreach(RelationshipBO currentRelationship in this)
            {
                valid &= currentRelationship.IsValid;
                if(!valid)
                    break;
            }
            return valid;
        }

        public SortedBindingList<RelationshipBO> ApplySort(string property, ListSortDirection direction)
        {
            if(IsAValidProperty(property))
                _sortedList.ApplySort(property, direction);
            return _sortedList;
        }

        /// <summary>
        /// Gets all the relationships where the given table is involved.
        /// </summary>
        /// <param name="Id">The involved table</param>
        /// <returns>A list of relationships</returns>
        public List<RelationshipBO> GetByParentOrChildId(int Id)
        {
            List<RelationshipBO> list = new List<RelationshipBO>();
            try
            {
                if(_parentIdList[Id.ToString()] != null)
                {
                    foreach(string key in _parentIdList.GetValues(Id.ToString()))
                    {
                        RelationshipBO relation = null;
                        if(_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                        {
                            if(!list.Contains(relation))
                            list.Add(relation);
                    }
                }
                }

                if(_childIdList[Id.ToString()] != null)
                {
                    foreach(string key in _childIdList.GetValues(Id.ToString()))
                    {
                        RelationshipBO relation = null;
                        if(_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                        {
                            if(!list.Contains(relation))
                            list.Add(relation);
                    }
                }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
       }

        /// <summary>
        /// Gets all the relationships where the given table is a child.
        /// </summary>
        /// <param name="Id">Child table id.</param>
        /// <returns>A list of relationships.</returns>
        public List<RelationshipBO> GetByChildId(int Id)
        {
            List<RelationshipBO> list = new List<RelationshipBO>();
            try
            {
                if (_childIdList[Id.ToString()] != null)
                {
                    foreach (string key in _childIdList.GetValues(Id.ToString()))
                    {
                        RelationshipBO relation = null;
                        if (_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                            list.Add(relation);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets all the relationships where the given field is involved.
        /// </summary>
        /// <param name="Id">The involved field id.</param>
        /// <returns>A list of relationships</returns>
        public List<RelationshipBO> GetByParentKeyOrChildKeyId(int Id)
        {
            List<RelationshipBO> list = new List<RelationshipBO>();
            try
            {
                if (_parentKeyList[Id.ToString()] != null)
                {
                    foreach (string key in _parentKeyList.GetValues(Id.ToString()))
                    {
                        RelationshipBO relation = null;
                        if (_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                            list.Add(relation);
                    }
                }

                if (_childKeyList[Id.ToString()] != null)
                {
                    foreach (string key in _childKeyList.GetValues(Id.ToString()))
                    {
                        RelationshipBO relation = null;
                        if (_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                            list.Add(relation);
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Gets a specific relationship
        /// </summary>
        /// <param name="parent">Parent table id</param>
        /// <param name="parentKey">Parent field id</param>
        /// <param name="child">Child table id</param>
        /// <param name="childKey">Child field id</param>
        /// <returns>A relationship if present, null otherwhise</returns>
        public RelationshipBO Get(int parent, int parentKey, int child, int childKey)
        {
            RelationshipBO result = null;
            try
            {
                _fullRelationshipKeys.TryGetValue(this.BuildRelationshipKey(parent, parentKey, child, childKey), out result);
                
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        /// <summary>
        /// Gets a relationship where the given fields are parent and child.
        /// </summary>
        /// <param name="parentKey">Parent field id</param>
        /// <param name="childKey">Child field id</param>
        /// <returns>A relationship if present, null otherwhise.</returns>
        public RelationshipBO Get(int parentKey, int childKey)
        {
            RelationshipBO result = null;
            try
            {
                _parentKeyAndChildKeyList.TryGetValue(this.BuildRelationshipKey(parentKey, childKey), out result);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return result;
        }

        /// <summary>
        /// Gets all the relationships where the given field is child.
        /// </summary>
        /// <param name="childKey">The child field id</param>
        /// <returns>A list of relationships.</returns>
        public List<RelationshipBO> GetByChildKey(int childKey)
        {
            List<RelationshipBO> list = new List<RelationshipBO>();
            try
            {
                if (_childKeyList[childKey.ToString()] != null)
                {
                    foreach (string key in _childKeyList.GetValues(childKey.ToString()))
                    {
                        RelationshipBO relation = null;
                        if (_parentKeyAndChildKeyList.TryGetValue(key, out relation))
                            list.Add(relation);
                    }
                }
                
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return list;
        }

        /// <summary>
        /// Removes a relationship.
        /// </summary>
        /// <param name="parent">The parent table id</param>
        /// <param name="parentKey">the parent field id</param>
        /// <param name="child">The child table id</param>
        /// <param name="childKey">The child field id</param>
        public void Remove(int parent, int parentKey, int child, int childKey)
        {
            try
            {
                RelationshipBO relationToRemove = this.Get(parent, parentKey, child, childKey);
                if (relationToRemove != null)
                {
                    this.RemoveRelationshipFromLists(relationToRemove);
                    this.Remove(relationToRemove);
                }
            }
            catch (Exception ex)
            {
               COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Removes a relationship.
        /// </summary>
        /// <param name="parentKey">Parent field id</param>
        /// <param name="childKey">Child field id</param>
        public void Remove(int parentKey, int childKey)
        {
            try
            {
                RelationshipBO relationToRemove = this.Get(parentKey, childKey);
                if(relationToRemove != null)
                {
                    this.RemoveRelationshipFromLists(relationToRemove);
                    this.Remove(relationToRemove);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Remove all the relationships where the given table ids are parent or child.
        /// </summary>
        /// <param name="tablesIds">The list of table ids.</param>
        public void Remove(List<int> tablesIds)
        {
            try
            {
                foreach (int tableId in tablesIds)
                {
                    List<RelationshipBO> relationships = this.GetByParentOrChildId(tableId);
                    for (int i = relationships.Count; i > 0; i--)
                    {
                        RelationshipBO relationToRemove = relationships[i - 1];
                        this.RemoveRelationshipFromLists(relationToRemove);
                        this.Remove(relationToRemove);
                    }
                }
            }
            catch (Exception ex)
            {
               COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private bool IsAValidProperty(string property)
        {
            bool retVal = true;
            //TODO: Check that is a valid Property.
            return retVal;
        }

        #endregion

        #region Constructor

        private RelationshipListBO()
        {
            InitializeArrays();
        }

        private RelationshipListBO(List<COEDataView.Relationship> relationships)
            : this()
        {
            foreach(COEDataView.Relationship relationship in relationships)
                this.Add(RelationshipBO.NewRelationship(relationship));
        }

        private RelationshipListBO(List<COEDataView.Relationship> relationships, bool isClean)
            : this()
        {
            foreach(COEDataView.Relationship relationship in relationships)
                this.Add(RelationshipBO.NewRelationship(relationship, isClean));
        }

        #endregion

        #region Lists
        private void AddRelationshipToLists(RelationshipBO relationship)
        {
            string keyRelationshipKey = this.BuildRelationshipKey(relationship.ParentKey, relationship.ChildKey);
            if(_parentKeyAndChildKeyList.ContainsKey(keyRelationshipKey))
            {
                throw new ValidationException("RelationshipsListBO: There are more relationships with the same ParentKey and ChildKey(" + relationship.ParentKey.ToString() + "," + relationship.ChildKey.ToString() + ")");
            }
            _fullRelationshipKeys.Add(this.BuildRelationshipKey(relationship.Parent, relationship.ParentKey, relationship.Child, relationship.ChildKey), relationship);
            _parentKeyAndChildKeyList.Add(keyRelationshipKey, relationship);
            _parentKeyList.Add(relationship.ParentKey.ToString(), keyRelationshipKey);
            _childKeyList.Add(relationship.ChildKey.ToString(), keyRelationshipKey);
            _parentIdList.Add(relationship.Parent.ToString(), keyRelationshipKey);
            _childIdList.Add(relationship.Child.ToString(), keyRelationshipKey);
        }

        private void InitializeArrays()
        {
            if(_fullRelationshipKeys == null)
                _fullRelationshipKeys = new Dictionary<string, RelationshipBO>();
            if(_parentKeyAndChildKeyList == null)
                _parentKeyAndChildKeyList = new Dictionary<string, RelationshipBO>();
            if(_parentKeyList == null)
                _parentKeyList = new NameValueCollection();
            if(_childKeyList == null)
                _childKeyList = new NameValueCollection();
            if(_parentIdList == null)
                _parentIdList = new NameValueCollection();
            if(_childIdList == null)
                _childIdList = new NameValueCollection();
            if(_sortedList == null)
                _sortedList = new SortedBindingList<RelationshipBO>(this);
        }

        private void RemoveRelationshipFromLists(RelationshipBO relationship)
        {
            string keyToRemove = this.BuildRelationshipKey(relationship.ParentKey, relationship.ChildKey);
            _fullRelationshipKeys.Remove(this.BuildRelationshipKey(relationship.Parent, relationship.ParentKey, relationship.Child, relationship.ChildKey));
            _parentKeyAndChildKeyList.Remove(keyToRemove);
            string currentKey = relationship.ParentKey.ToString();
            if(_parentKeyList.GetValues(currentKey) != null)
            {
                if(_parentKeyList.GetValues(currentKey).Length < 2)
                    _parentKeyList.Remove(currentKey);
                else
                {
                    string[] oldValues = _parentKeyList.GetValues(currentKey);
                    List<string> newValues = new List<string>();
                    for(int i = 0; i < oldValues.Length; i++)
                    {
                        if(oldValues[i] != keyToRemove)
                            newValues.Add(oldValues[i]);
                    }
                    _parentKeyList[currentKey] = string.Join(",", newValues.ToArray());
                }
            }
            currentKey = relationship.ChildKey.ToString();
            if(_childKeyList.GetValues(currentKey) != null)
            {
                if(_childKeyList.GetValues(currentKey).Length < 2)
                    _childKeyList.Remove(currentKey);
                else
                {
                    string[] oldValues = _childKeyList.GetValues(currentKey);
                    List<string> newValues = new List<string>();
                    for(int i = 0; i < oldValues.Length; i++)
                    {
                        if(oldValues[i] != keyToRemove)
                            newValues.Add(oldValues[i]);
                    }
                    _childKeyList[currentKey] = string.Join(",", newValues.ToArray());
                }
            }
            currentKey = relationship.Parent.ToString();
            if(_parentIdList.GetValues(currentKey) != null)
            {
                if(_parentIdList.GetValues(currentKey).Length < 2)
                    _parentIdList.Remove(currentKey);
                else
                {
                    string[] oldValues = _parentIdList.GetValues(currentKey);
                    List<string> newValues = new List<string>();
                    for(int i = 0; i < oldValues.Length; i++)
                    {
                        if(oldValues[i] != keyToRemove)
                            newValues.Add(oldValues[i]);
                    }
                    _parentIdList[relationship.Parent.ToString()] = string.Join(",", newValues.ToArray());
                }
            }
            currentKey = relationship.Child.ToString();
            if(_childIdList.GetValues(currentKey) != null)
            {
                if(_childIdList.GetValues(currentKey).Length < 2)
                    _childIdList.Remove(currentKey);
                else
                {
                    string[] oldValues = _childIdList.GetValues(currentKey);
                    List<string> newValues = new List<string>();
                    for(int i = 0; i < oldValues.Length; i++)
                    {
                        if(oldValues[i] != keyToRemove)
                            newValues.Add(oldValues[i]);
                    }
                    _childIdList[currentKey] = string.Join(",", newValues.ToArray());
                }
            }
        }

        private void UpdateRelationshipFromLists(RelationshipBO oldRelationship, RelationshipBO newRelationship)
        {
            try
            {
                RemoveRelationshipFromLists(oldRelationship);
                AddRelationshipToLists(newRelationship);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private string BuildRelationshipKey(int parent, int parentKey, int child, int childKey)
        {
            return parent + _relationshipSeparator + parentKey + _relationshipSeparator + child + _relationshipSeparator + childKey;
        }

        private string BuildRelationshipKey(int parentKey, int childKey)
        {
            return parentKey + _relationshipSeparator + childKey;
        }
        #endregion

    }
}

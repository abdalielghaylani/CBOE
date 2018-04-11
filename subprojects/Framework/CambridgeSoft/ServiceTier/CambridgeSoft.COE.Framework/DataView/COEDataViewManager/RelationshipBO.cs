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
    public class RelationshipBO : Csla.BusinessBase<RelationshipBO>
    {
        #region Variables
        private int _parent = int.MinValue;
        private int _parentKey = int.MinValue;
        private int _child = int.MinValue;
        private int _childKey = int.MinValue;
        private COEDataView.JoinTypes _joinType = COEDataView.JoinTypes.INNER;
        private string _xml = String.Empty;
        private string _idString = string.Empty;
        [NonSerialized]
        private COELog _coeLog = COELog.GetSingleton("RelationshipBO");
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "RelationshipBO";
        private bool _fromMasterSchema = false;
        #endregion

        #region Enums
        /// <summary>
        /// Properties nomaes for the BO, for avoiding string hardcoding
        /// </summary>
        private enum Properties
        {
            Parent,
            ParentKey,
            Child,
            ChildKey,
            JoinType,
            Xml,
            FromMasterSchema,
        }
        #endregion

        #region Properties
        /// <summary>
        /// Id of the parent table
        /// </summary>
        public int Parent
        {
            get
            {
                CanReadProperty(Properties.Parent.ToString(), true);
                return _parent;
            }
            set
            {
                CanWriteProperty(Properties.Parent.ToString(), true);
                if (value != _parent)
                {
                    _parent = value;
                    PropertyHasChanged(Properties.Parent.ToString());
                }
            }
        }

        /// <summary>
        /// Id of the parent field
        /// </summary>
        public int ParentKey
        {
            get
            {
                CanReadProperty(Properties.ParentKey.ToString(), true);
                return _parentKey;
            }
            set
            {
                CanWriteProperty(Properties.ParentKey.ToString(), true);
                if (value != _parentKey)
                {
                    _parentKey = value;
                    PropertyHasChanged(Properties.ParentKey.ToString());
                }
            }
        }

        /// <summary>
        /// Id of the child table
        /// </summary>
        public int Child
        {
            get
            {
                CanReadProperty(Properties.Child.ToString(), true);
                return _child;
            }
            set
            {
                CanWriteProperty(Properties.Child.ToString(), true);
                if (value != _child)
                {
                    _child = value;
                    PropertyHasChanged(Properties.Child.ToString());
                }
            }
        }

        /// <summary>
        /// Id of the child field
        /// </summary>
        public int ChildKey
        {
            get
            {
                CanReadProperty(Properties.ChildKey.ToString(), true);
                return _childKey;
            }
            set
            {
                CanWriteProperty(Properties.ChildKey.ToString(), true);
                if (value != _childKey)
                {
                    _childKey = value;
                    PropertyHasChanged(Properties.ChildKey.ToString());
                }
            }
        }

        /// <summary>
        /// Join type. Can be INNER or OUTER
        /// </summary>
        public COEDataView.JoinTypes JoinType
        {
            get
            {
                CanReadProperty(Properties.JoinType.ToString(), true);
                return _joinType;
            }
            set
            {
                CanWriteProperty(Properties.JoinType.ToString(), true);
                if (value != _joinType)
                {
                    _joinType = value;
                    PropertyHasChanged(Properties.JoinType.ToString());
                }
            }
        }

        /// <summary>
        /// Xml representation for the object
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
        /// Indicates if the object is new.
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
                return base.IsValid;
            }
        }

        /// <summary>
        /// Indicates if the object is dirty (has been modified)
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
                CanReadProperty(true);
                return string.Format("{0:00}", ((RelationshipListBO)base.Parent).IndexOf(this) + 1);
            }
        }

        /// <summary>
        /// Indicates if the relationship was inherited (not selected by the user)
        /// </summary>
        public bool FromMasterSchema
        {
            get
            {
                CanReadProperty(Properties.FromMasterSchema.ToString(), true);
                return _fromMasterSchema;
            }
            set
            {
                if(value != _fromMasterSchema)
                {
                    _fromMasterSchema = value;
                    PropertyHasChanged(Properties.FromMasterSchema.ToString());
                }
            }
        }
        #endregion

        #region Overrided methods
        /// <summary>
        /// Unique identifier of the object. This value is used by CSLA's implementation of System.Object overrides, such us Equals()
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return _idString;
        }

        /// <summary>
        /// Business rules should be added in here
        /// </summary>
        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }
        #endregion

        #region Constructor

        internal RelationshipBO(COEDataView.Relationship relationship)
        {
            _parent = relationship.Parent;
            _parentKey = relationship.ParentKey;
            _child = relationship.Child;
            _childKey = relationship.ChildKey;
            _joinType = relationship.JoinType;
            _xml = relationship.ToString();
            _idString = _parentKey.ToString() + "|" + _childKey.ToString();
            this.MarkAsChild();
        }

        internal RelationshipBO(COEDataView.Relationship relationship, bool isClean)
        {
            _parent = relationship.Parent;
            _parentKey = relationship.ParentKey;
            _child = relationship.Child;
            _childKey = relationship.ChildKey;
            _joinType = relationship.JoinType;
            _xml = relationship.ToString();
            _idString = _parentKey.ToString() + "|" + _childKey.ToString();
            this.MarkAsChild();
            if(isClean)
                this.MarkClean();
            else
                this.MarkDirty();
        }

        internal RelationshipBO(int parent, int parentKey, int child, int childKey, COEDataView.JoinTypes joinType)
        {
            _parent = parent;
            _parentKey = parentKey;
            _child = child;
            _childKey = childKey;
            _joinType = joinType;
            _idString = _parentKey.ToString() + "|" + _childKey.ToString();
            this.MarkAsChild();
        }

        internal RelationshipBO(int parentKey, int childKey, COEDataView.JoinTypes joinType)
        {
            _parentKey = parentKey;
            _childKey = childKey;
            _joinType = joinType;
            _idString = _parentKey.ToString() + "|" + _childKey.ToString();
            this.MarkAsChild();
        }

        #endregion

        #region Validaton Rules

        private void AddCommonRules()
        {
            //Examples from other class
            //ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 50));
            //ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            //ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
        }

        #endregion

        #region Factory Methods

        public static RelationshipBO NewRelationship(COEDataView.Relationship relationship)
        {
            try
            {
                return new RelationshipBO(relationship);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static RelationshipBO NewRelationship(COEDataView.Relationship relationship, bool isClean)
        {
            try
            {
                return new RelationshipBO(relationship, isClean);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static RelationshipBO NewRelationship(int parent, int parentKey, int child, int childKey, COEDataView.JoinTypes joinType)
        {
            try
            {
                return new RelationshipBO(parent, parentKey, child, childKey, joinType);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return null;
        }

        public static RelationshipBO NewRelationship(int parentKey, int childKey, COEDataView.JoinTypes joinType)
        {
            try
            {
                return new RelationshipBO(parentKey, childKey, joinType);
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
            COEExceptionDispatcher.HandleBLLException(ex);//Removed throw ex as This mehod is re-throwing exception.
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            if (_coeDAL == null)
                LoadDAL();
            if (criteria._relationship != null)
                this.Initialize(criteria._relationship, true, true);
        }
        #endregion

        #region Criterias

        [Serializable()]
        private class Criteria
        {
            internal COEDataView.Relationship _relationship;

            public Criteria(COEDataView.Relationship relationship)
            {
                _relationship = relationship;
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

        private void Initialize(COEDataView.Relationship relationship, bool isNew, bool isClean)
        {
            _parent = relationship.Parent;
            _parentKey = relationship.ParentKey;
            _child = relationship.Child;
            _childKey = relationship.ChildKey;
            _joinType = relationship.JoinType;
            _xml = relationship.ToString();

            if (!isNew)
                MarkOld();
            if (isClean)
                MarkClean();
        }

        /// <summary>
        /// Creates a string formatted as xml that represents the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(!_fromMasterSchema)
            {
                StringBuilder builder = new StringBuilder("<relationship");
                builder.Append(" parentkey=\"" + _parentKey.ToString() + "\"");
                builder.Append(" childkey=\"" + _childKey.ToString() + "\"");
                //Add the parent and child until changes in other apps that expect these values
                builder.Append(" parent=\"" + _parent.ToString() + "\"");
                builder.Append(" child=\"" + _child.ToString() + "\"");
                builder.Append(" jointype=\"" + _joinType.ToString() + "\"");
                builder.Append(" />");
                return builder.ToString();
            }
            else
                return string.Empty;
        }

        #endregion
    }
}

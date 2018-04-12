using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Data;
using System.Web;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class Property : BusinessBase<Property>
    {
        #region Business Methods

        protected int _id;
        protected string _name;
        protected string _type;
        protected string _value;
        protected string _precision;
        protected ValidationRuleList _validationRuleList;
        protected string _validationRulesXml;
        protected int _sortOrder;
        private bool _sortOrderUpdate = false;


        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public ValidationRuleList ValRuleList
        {
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _validationRuleList = value;
                    PropertyHasChanged();
                }

            }
            get
            {
                CanReadProperty(true);
                return _validationRuleList;
            }
        }
        protected override object GetIdValue()
        {
            return _id;
        }

        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
                _sortOrderUpdate = true;
            }
        }

        public virtual string Type
        {
            get
            {
                return _type;
            }
            set
            {
                CanWriteProperty(true);
                if (_type != value)
                {
                    _type = value;
                    PropertyHasChanged();
                }
            }
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                CanWriteProperty(true);
                if (_name != value)
                {
                    _name = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Precision
        {
            get
            {
                return _precision;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                CanWriteProperty(true);
                if (_value != value)
                {
                    _value = value;
                    PropertyHasChanged();
                }
            }
        }

        public override bool IsValid
        {
            get
            {
                this.ValidationRules.CheckRules();

                return base.IsValid;
            }
        }

        public override bool IsDirty
        {

            get
            {
                if (this._validationRuleList != null)
                    return base.IsDirty || this._validationRuleList.IsDirty;
                else
                    return base.IsDirty;
            }
        }

        public override string ToString()
        {
            return this._id + " - " + (string.IsNullOrEmpty(this._type) ? string.Empty : this._type + " ") + this._name + (!string.IsNullOrEmpty(this._value) ? " = " + this._value : string.Empty);
        }

        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
            {
                brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
            }
        }

        public bool SortOrderIsUpdate
        {
            get
            {
                return _sortOrderUpdate;
            }
        }

        #endregion

        #region Validation Rules

        protected override void AddInstanceBusinessRules()
        {
            base.AddInstanceBusinessRules();

            if (!string.IsNullOrEmpty(_validationRulesXml))
                ValidationRulesFactory.GetInstance().AddInstanceRules(this.ValidationRules, "Value", _validationRulesXml);
        }

        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Property", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("property");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("property");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("property");
        }

        #endregion

        #region Factory Methods

        public static Property NewProperty()
        {
            Property property = new Property();

            return property;
        }

        public static Property NewProperty(string xml, bool isClean)
        {
            return new Property(xml, isClean);
        }

        protected Property()
            : base()
        {
            this._type = this._value = string.Empty;
        }

        private Property(string xml, bool isClean)
            : this()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.SelectSingleNode("Property");
            _validationRulesXml = string.Empty;
            XmlNode validationRuleListNode = node.SelectSingleNode("validationRuleList");
            if (node.ChildNodes.Count > 0 && validationRuleListNode != null)
            {
                _validationRulesXml = validationRuleListNode.OuterXml;
                this._validationRuleList = ValidationRuleList.NewValidationRuleList(validationRuleListNode);

                node.RemoveChild(validationRuleListNode);
            }

            _value = node.InnerText != null ? node.InnerText.Trim() : null;
            _name = node.Attributes["name"] != null ? node.Attributes["name"].Value : null;
            _type = node.Attributes["type"] != null ? node.Attributes["type"].Value : null;
            _precision = (node.Attributes["precision"] != null && !string.IsNullOrEmpty(node.Attributes["precision"].Value)) ? node.Attributes["precision"].Value : null;

            if (node.Attributes["sortOrder"] != null && !string.IsNullOrEmpty(node.Attributes["sortOrder"].Value))
            {
                _sortOrder = int.Parse(node.Attributes["sortOrder"].Value);
            }
            else
            {
                _sortOrder = -1;
            }

            if (isClean)
            {
                MarkClean();
            }
            else
            {
                MarkDirty();
            }
            AddInstanceBusinessRules();
        }

        public static Property GetProperty(int id)
        {
            if (!CanGetObject())
            {

                throw new System.Security.SecurityException("User not authorized to view a Property");
            }
            return DataPortal.Fetch<Property>(new Criteria(id));
        }

        protected static void DeleteProperty(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Property");
            }
            DataPortal.Delete(new Criteria(id));
        }

        public override Property Save()
        {
            if (IsDeleted && !CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a Property");
            }
            else if (IsNew && !CanAddObject())
            {
                throw new System.Security.SecurityException("User not authorized to add a Property");
            }
            else if (!CanEditObject())
            {
                throw new System.Security.SecurityException("User not authorized to update a Property");
            }
            return base.Save();
        }
        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            public Criteria(int id)
            { _id = id; }
        }

        [RunLocal()]
        private void DataPortal_Create(Criteria criteria)
        {
            _id = 1;
            ValidationRules.CheckRules();
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            //this.RaiseListChangedEvents = false;
            //using (OracleConnection cn = new OracleConnection("Data Source=sunnyora;User ID=regdb;Password=Oracle;"))
            //{
            //    cn.Open();
            //    using (OracleCommand cmd = cn.CreateCommand())
            //    {

            //        cmd.CommandText = "Select ID, IDENTIFIER_TYPE, IDENTIFIER_DESCRIPTOR from Propertys where ID = " + criteria.Id;

            //        using (SafeDataReader reader = new SafeDataReader(cmd.ExecuteReader()))
            //        {
            //            Fetch(reader);
            //        }
            //    }
            //}
            // this.RaiseListChangedEvents = true;
        }

        private void Fetch(SafeDataReader reader)
        {
            if (reader.Read())
            {
                //this._id = reader.GetInt32("ID");
                //this._type = reader.GetInt32("IDENTIFIER_TYPE");
                //this._description = reader.GetString("IDENTIFIER_DESCRIPTOR");
            }
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            //using (OracleConnection cn = new OracleConnection("Data Source=sunnyora;User ID=regdb;Password=Oracle;"))
            //{
            //    cn.Open();
            //    using (OracleCommand cmd = cn.CreateCommand())
            //    {
            //        cmd.CommandText = "PropertyAdd";
            //        Insert(cmd);
            //    }
            //}
            // update child objects
            //_resources.Update(this);
        }

        //private void Insert(OracleCommand cmd)
        //{
        //    //cmd.CommandType = CommandType.StoredProcedure;
        //    ////cmd.Parameters.Add("@dbparam", param);

        //    //cmd.ExecuteNonQuery();

        //}

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            //if (base.IsDirty)
            //{
            //    using (OracleConnection cn = new OracleConnection("Data Source=sunnyora;User ID=regdb;Password=Oracle;"))
            //    {
            //        cn.Open();
            //        using (OracleCommand cmd = cn.CreateCommand())
            //        {
            //            cmd.CommandText = "PropertyUpdate";
            //            Update(cmd);
            //        }
            //    }
            //}
            // update child objects
            //_resources.Update(this);
        }

        //private void Update(OracleCommand cmd)
        //{
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    //cmd.Parameters.Add("@ID", _id);

        //    cmd.ExecuteNonQuery();
        //}

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id));
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(Criteria criteria)
        {
            //using (OracleConnection cn = new OracleConnection("Data Source=sunnyora;User ID=regdb;Password=Oracle;"))
            //{
            //    cn.Open();
            //    using (OracleCommand cmd = cn.CreateCommand())
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = "PropertyDelete";
            //        cmd.Parameters.Add("@ID", criteria.Id);
            //        cmd.ExecuteNonQuery();
            //    }
            //}
        }

        #endregion

        #region Xml

        public virtual string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Property");
            if (!string.IsNullOrEmpty(_name))
                builder.Append(" name=\"" + _name + "\"");
            if (!string.IsNullOrEmpty(_type))
                builder.Append(" type=\"" + _type + "\"");
            if (this.IsDirty)
                builder.Append(" update=\"yes\"");
            if (IsDeleted)
                builder.Append(" deleted=\"yes\"");
            builder.Append(">");
            builder.Append(_value);
            builder.Append("</Property>");

            return builder.ToString();
        }
        #endregion
    }
}


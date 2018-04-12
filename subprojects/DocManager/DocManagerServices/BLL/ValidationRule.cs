using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;

using Csla;
using Csla.Data;
using Csla.Validation;

//this is temporary until all the commands are converted for using the DAL
//using Oracle.DataAccess.Types;
//using Oracle.DataAccess.Client;
//using CambridgeSoft.COE.Registration.Services;
//using CambridgeSoft.COE.Registration.Services.BLL;


namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
    public class ValidationRule : BusinessBase<ValidationRule>
    {
        #region Business Methods

        private string _specialFolder = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + @"SimulationFolder\Registration\";

        private int _id;
        private string _name;
        private string _min; //to be removed
        private string _max; // to be removed
        private int _maxLength; //to be removed
        private string _error;
        //private ParamNameValueList _parameters;
        private ParameterList _parameters;
        private int _editLevelAdded;

        internal int EditLevelAdded
        {
            get { return _editLevelAdded; }
            set { _editLevelAdded = value; }
        }

        public void Delete()
        {
            if (this.IsChild)
                //throw new NotSupportedException(Resources.ChildDeleteException);
                MarkDeleted();

        }
        internal void DeleteChild()
        {
            if (!this.IsChild)
                //throw new NotSupportedException(Resources.NoDeleteRootException);
                MarkDeleted();
        }


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

        protected override object GetIdValue()
        {
            return _id;
        }


        public string Name
        {
            get
            {
                CanReadProperty(true);
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string MIN
        {
            get
            {
                CanReadProperty(true);
                //if the type of the current validation is supposed to have a min, then return Params["Min"]
                //else throw new exception with a message that explains the min param has no meaning.
                return _min; //_min should be removed as property
            }
            set
            {
                _min = value;
            }
        }

        public string MAX
        {
            get
            {
                CanReadProperty(true);
                //if the type of the current validation is supposed to have a max, then return Params["Max"]
                //else throw new exception with a message that explains the min param has no meaning.

                return _max;
            }
            set
            {
                _max = value; //_max should be removed as property
            }
        }

        public int MaxLength
        {
            get
            {
                CanReadProperty(true);
                //if the type of the current validation is supposed to have a max, then return Params["MaxLength"]
                //else throw new exception with a message that explains the max length param has no meaning.
                return _maxLength;
            }
            set
            {
                _maxLength = value;
            }
        }

        public override bool IsValid
        {
            //get { return base.IsValid && _resources.IsValid; }
            get { return base.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty || _parameters.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
        }

        public string Error
        {
            get
            {
                CanReadProperty(true);
                return _error;
            }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _error = value;
                    PropertyHasChanged();
                }
            }
        }


        //public ParamNameValueList Parameters
        //{
        //    get
        //    {
        //        CanReadProperty(true);
        //        return _parameters;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            CanWriteProperty(true);
        //            _parameters = value;
        //            PropertyHasChanged();
        //        }
        //    }
        //}

        public ParameterList Parameters
        {
            get
            {
                CanReadProperty(true);
                return _parameters;
            }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    _parameters = value;
                    PropertyHasChanged();
                }
            }
        }




        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "ValidationRule", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("validationRule");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("validationRule");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("validationRule");
        }

        #endregion

        #region Factory Methods

        public static ValidationRule NewValidationRule()
        {
            ValidationRule validationRule = new ValidationRule();

            //validationRule.Description = null;
            //validationRule.Value = null;

            return validationRule;
        }

        public static ValidationRule NewValidationRule(string xml)
        {
            XmlDocument valRuleXml = new XmlDocument();

            valRuleXml.LoadXml(xml);

            ValidationRule validationRule = new ValidationRule(true);

            if (valRuleXml.FirstChild.Attributes["validationRuleName"] != null)
                validationRule._name = valRuleXml.FirstChild.Attributes["validationRuleName"].Value;

            if (valRuleXml.FirstChild.Attributes["errorMessage"] != null)
                validationRule._error = valRuleXml.FirstChild.Attributes["errorMessage"].Value;

            validationRule._parameters = ParameterList.NewParameterList(valRuleXml.FirstChild["params"]);

            return validationRule;
        }
        public static ValidationRule NewValidationRule(string name, string error, ParameterList parameters, bool isClean)
        {
            ValidationRule validationRule = new ValidationRule(name, error, parameters, isClean);
            return validationRule;

        }


        private ValidationRule(string name, string error, ParameterList parameters, bool isClean)
        {
            this._name = name;
            this._error = error;
            if (parameters == null)
                _parameters = ParameterList.NewParameterList();
            else
                this._parameters = parameters;
            MarkAsChild();
            if (isClean)
                MarkClean();
        }

        private ValidationRule()
        {

            MarkAsChild();
            MarkClean();

        }
        private ValidationRule(bool isOld)
        {
            MarkAsChild();
            MarkClean();
            if (isOld)
            {
                MarkOld();
            }
            else
            {
                MarkNew();
                MarkDirty();
            }
        }


        public static ValidationRule GetValidationRule(int id)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view a ValidationRule");
            }
            return DataPortal.Fetch<ValidationRule>(new Criteria(id));
        }

        protected static void DeleteValidationRule(int id)
        {
            if (!CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a ValidationRule");
            }
            DataPortal.Delete(new Criteria(id));
        }

        public override ValidationRule Save()
        {
            if (IsDeleted && !CanDeleteObject())
            {
                throw new System.Security.SecurityException("User not authorized to remove a ValidationRule");
            }
            else if (IsNew && !CanAddObject())
            {
                throw new System.Security.SecurityException("User not authorized to add a ValidationRule");
            }
            else if (!CanEditObject())
            {
                throw new System.Security.SecurityException("User not authorized to update a ValidationRule");
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

            //        cmd.CommandText = "Select ID, IDENTIFIER_TYPE, IDENTIFIER_DESCRIPTOR from ValidationRules where ID = " + criteria.Id;

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
            //        cmd.CommandText = "ValidationRuleAdd";
            //        Insert(cmd);
            //    }
            //}
            // update child objects
            //_resources.Update(this);
        }

        //private void Insert(OracleCommand cmd)
        //{
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    //cmd.Parameters.Add("@dbparam", param);

        //    cmd.ExecuteNonQuery();

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
            //            cmd.CommandText = "ValidationRuleUpdate";
            //            Update(cmd);
            //        }
            //    }
            //}
            // update child objects
            //_resources.Update(this);
        }

        //private void Update(OracleCommand cmd)
        //{
        //    //cmd.CommandType = CommandType.StoredProcedure;
        //    ////cmd.Parameters.Add("@ID", _id);

        //    //cmd.ExecuteNonQuery();
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
            //        cmd.CommandText = "ValidationRuleDelete";
            //        cmd.Parameters.Add("@ID", criteria.Id);
            //        cmd.ExecuteNonQuery();
            //    }
            //}
        }

        #endregion

        #region Xml

        internal string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<validationRule");
            if (!string.IsNullOrEmpty(_name))
                builder.Append(" validationRuleName=\"" + _name + "\"");
            builder.Append(">");
            if (!string.IsNullOrEmpty(_min))
                builder.Append("<parameter name=\"Min\">" + _min + "</parameter");
            if (!string.IsNullOrEmpty(_max))
                builder.Append("<parameter name=\"Min\">" + _max + "</parameter");
            builder.Append("</validationRule>");

            return builder.ToString();
        }

        public string UpdateSelfConfig(bool propertyIsNew)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<validationRule");
            if (!string.IsNullOrEmpty(_name))
                builder.Append(" validationRuleName=\"" + _name + "\"");
            if (!String.IsNullOrEmpty(this._error))
                builder.Append(" errorMessage=\"" + _error + "\"");
            if (!propertyIsNew)
            {
                if (!IsDeleted && IsNew)
                {
                    builder.Append(" insert=\"yes\"");
                }
                //else if (IsDirty && !IsDeleted)
                //{
                //    builder.Append(" update=\"yes\"");
                //}
                else if (IsDeleted && !IsNew)
                {
                    builder.Append(" delete=\"yes\"");
                    builder.Append(">");
                    builder.Append("</validationRule>");
                    return builder.ToString();
                }
            }
            builder.Append(">");
            {
                if (_parameters != null)
                {

                    builder.Append(_parameters.UpdateSelfConfig(this.IsNew));
                }
            }
            builder.Append("</validationRule>");
            return builder.ToString();

        }

        #endregion
    }
}

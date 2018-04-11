//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Xml;
//using System.Xml.XPath;
//using System.Data;
//using CambridgeSoft.COE.Registration;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Framework.COEConfigurationService;
//using Csla;
//using Csla.Data;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Services.Types
//{
//    [Serializable()]
//    public class Utilization : BusinessBase<Utilization>
//    {
//        #region Business Methods

//        private int _id;
//        private string _value;

//        [System.ComponentModel.DataObjectField(true, true)]
//        public int ID
//        {
//            get
//            {
//                CanReadProperty(true);
//                return _id;
//            }
//            set
//            {
//                _id = value;
//            }
//        }

//        protected override object GetIdValue()
//        {
//            return _id;
//        }

//        public override bool IsValid
//        {
//            //get { return base.IsValid && _resources.IsValid; }
//            get { return base.IsValid; }
//        }

//        public override bool IsDirty
//        {
//            get { return base.IsDirty; }
//            //get { return base.IsDirty || _resources.IsDirty; }
//        }
//        #endregion

//        #region Validation Rules

//        protected override void AddBusinessRules()
//        {
//            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
//            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
//        }

//        #endregion

//        #region Authorization Rules

//        protected override void AddAuthorizationRules()
//        {
//            AuthorizationRules.AllowWrite(
//              "Utilization", "ADD_IDENTIFIER");
//        }

//        public static bool CanAddObject()
//        {
//            //return Csla.ApplicationContext.User.IsInRole("utilization");
//            return true;
//        }

//        public static bool CanGetObject()
//        {
//            return true;
//        }

//        public static bool CanDeleteObject()
//        {
//            return true;
//            //return Csla.ApplicationContext.User.IsInRole("utilization");
//        }

//        public static bool CanEditObject()
//        {
//            return true;
//            //return Csla.ApplicationContext.User.IsInRole("utilization");
//        }

//        #endregion

//        #region Factory Methods

//        [COEUserActionDescription("CreateUtilization")]
//        public static Utilization NewUtilization()
//        {
//            try
//            {
//                Utilization utilization = new Utilization();

//                //utilization.Description = null;
//                //utilization.Value = null;

//                return utilization;
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        [COEUserActionDescription("CreateUtilization")]
//        public static Utilization NewUtilization(string xml)
//        {
//            try
//            {
//                Utilization utilization = new Utilization();

//                XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
//                XPathNavigator xNavigator = xDocument.CreateNavigator();

//                XPathNodeIterator xIterator = xNavigator.Select("Utilization/Value");
//                xIterator.MoveNext();

//                utilization._value = xIterator.Current.Value;

//                return utilization;
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
        
//        private Utilization()
//        {
//            //MarkAsChild(); 
//        }


//        [COEUserActionDescription("GetUtilization")]
//        public static Utilization GetUtilization(int id)
//        {
//            try
//            {
//                if (!CanGetObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to view a Utilization");
//                }
//                return DataPortal.Fetch<Utilization>(new Criteria(id));
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        protected static void DeleteUtilization(int id)
//        {
//            if (!CanDeleteObject())
//            {
//                throw new System.Security.SecurityException("User not authorized to remove a Utilization");
//            }
//            DataPortal.Delete(new Criteria(id));
//        }

//        [COEUserActionDescription("SaveUtilization")]
//        public override Utilization Save()
//        {
//            try
//            {
//                if (IsDeleted && !CanDeleteObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to remove a Utilization");
//                }
//                else if (IsNew && !CanAddObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to add a Utilization");
//                }
//                else if (!CanEditObject())
//                {
//                    throw new System.Security.SecurityException("User not authorized to update a Utilization");
//                }
//                return base.Save();
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }
//        #endregion

//        #region Data Access

//        [Serializable()]
//        private class Criteria
//        {
//            private int _id;
//            public int Id
//            {
//                get { return _id; }
//            }

//            public Criteria(int id)
//            { _id = id; }
//        }

//        [RunLocal()]
//        private void DataPortal_Create(Criteria criteria)
//        {
//            _id = 1;
//            ValidationRules.CheckRules();
//        }

//        private void DataPortal_Fetch(Criteria criteria)
//        {
//        }

//        private void Fetch(SafeDataReader reader)
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_Insert()
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_Update()
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        protected override void DataPortal_DeleteSelf()
//        {
//        }

//        [Transactional(TransactionalTypes.TransactionScope)]
//        private void DataPortal_Delete(Criteria criteria)
//        {
//        }

//        #endregion

//        #region Xml

//        internal string UpdateSelf()
//        {
//            StringBuilder builder = new StringBuilder("");

//            builder.Append("<Utilization>");
//            builder.Append("<ID>" + this._id + "</ID>");
//            builder.Append("</Utilization>");

//            return builder.ToString();
//        }

//        #endregion
//    }
//}

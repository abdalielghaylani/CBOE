//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using System.Xml;
//using System.Xml.XPath;

//using CambridgeSoft.COE.Framework.COEConfigurationService;
//using CambridgeSoft.COE.Framework.Common;
//using CambridgeSoft.COE.Framework.ExceptionHandling;
//using CambridgeSoft.COE.Registration.Services;
//using CambridgeSoft.COE.Registration.Services.Common;

//using Csla;
//using Csla.Data;
//using Csla.Validation;

//namespace CambridgeSoft.COE.Registration.Services.Types
//{

//    [Serializable()]
//    public class NotebookNameValueList : NameValueListBase<int, string>
//    {
//        #region Factory Method

//        private static NotebookNameValueList _list;
//        private int _selectedNotebookID;
//        private string _selectedNotebookName;

//        public int SelectedNotebookID
//        {
//            get
//            {
//                //CanReadProperty(true);
//                return _selectedNotebookID;
//            }
//            set
//            {
//                _selectedNotebookID = value;
//            }
//        }

//        public string SelectedNotebookName
//        {
//            get
//            {
//                //CanReadProperty(true);
//                return _selectedNotebookName;
//            }
//            set
//            {
//                _selectedNotebookName = value;
//            }
//        }

//        [COEUserActionDescription("GetNotebookNameValueList")]
//        public static NotebookNameValueList GetNotebookNameValueList()
//        {
//            try
//            {
//                if (_list == null)
//                    _list = DataPortal.Fetch<NotebookNameValueList>(new Criteria(typeof(NotebookNameValueList)));
//                return _list;
//            }
//            catch (Exception exception)
//            {
//                COEExceptionDispatcher.HandleBLLException(exception);
//                return null;
//            }
//        }

//        public static void InvalidateCache()
//        {
//            _list = null;
//        }

//        private NotebookNameValueList()
//        { /* require use of factory methods */}

//        #endregion

//        #region Data Access

//        [NonSerialized, NotUndoable]
//        private COERegistrationService.OracleDataAccessClientDAL _regDal = null;
//        /// <summary>
//        /// DAL implementation.
//        /// </summary>
//        private COERegistrationService.OracleDataAccessClientDAL RegDal
//        {
//            get
//            {
//                if (_regDal == null)
//                    RegSvcUtilities.GetRegistrationDAL(ref _regDal, Constants.SERVICENAME);
//                return _regDal;
//            }
//        }
        
//        protected override void DataPortal_Fetch(object criteria)
//        {
//            //to fetch NotebookId and NotebookName in datareader.
//            RaiseListChangedEvents = false;
//            IsReadOnly = false;

//            using (SafeDataReader dr = this.RegDal.GetNoteBookNameValueList())
//            {
//                while (dr.Read())
//                {
//                    Add(new NameValueListBase<int, string>.NameValuePair(dr.GetInt32(0), dr.GetString(1)));
//                }
//            }

//            IsReadOnly = true;
//            RaiseListChangedEvents = true;
//        }
//        #endregion

//    }
//}

using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using System.Data;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.COESearchService
{
    /// <summary>
    /// COESearchUtilities is the class used for perfoming searches through the COEFramework.
    /// </summary>
    /// 

    [Serializable]
    public class COESearchUtilities : BLLBase
    {

        public static JumpToList GetByBaseTable(Int32 dataviewID, bool matchBaseTableOnly, JumpToListType jumpToListType)
        {
            JumpToCommand j = DataPortal.Execute<JumpToCommand>(new JumpToCommand(dataviewID, matchBaseTableOnly, jumpToListType));
            return j.JumpToList;
        }

        public static JumpToList GetByBaseTable(Int32 dataviewID, bool matchBaseTableOnly, params JumpToListType[] jumpToListTypes)
        {
            JumpToCommand j = DataPortal.Execute<JumpToCommand>(new JumpToCommand(dataviewID, matchBaseTableOnly, jumpToListTypes));
            return j.JumpToList;
        }

        public static JumpToList GetByFieldId(Int32 dataviewID, Int32 fieldID, JumpToListType jumpToListType)
        {
            JumpToCommand j = DataPortal.Execute<JumpToCommand>(new JumpToCommand(dataviewID, fieldID, jumpToListType));
            return j.JumpToList;
        }

        public static JumpToList GetByFieldId(Int32 dataviewID, Int32 fieldID, params JumpToListType[] jumpToListTypes)
        {
            JumpToCommand j = DataPortal.Execute<JumpToCommand>(new JumpToCommand(dataviewID, fieldID, jumpToListTypes));
            return j.JumpToList;
        }
    }

    [Serializable]
    class JumpToCommand : CommandBase
    {
        #region Properties

        private string _serviceName = "COESearch";

        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESearch");


        private int _dataviewid;
        private bool _matchBaseTableOnly;
        private Int32 _fieldID;
        private JumpToListType _jumptolisttype;
        private JumpToListType[] _jumptolisttypeList;
        private JumpToList _jumpToList;

        public JumpToList JumpToList
        {
            get { return _jumpToList; }
            set { _jumpToList = value; }
        }

        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }
        internal string action;

        #endregion

        #region Constructor

        public JumpToCommand(Int32 dataviewId, bool matchBaseTableOnly, JumpToListType jumpToListType)
        {
            SetDatabaseName();
            _dataviewid = dataviewId;
            _matchBaseTableOnly = matchBaseTableOnly;
            _jumptolisttype = jumpToListType;
            _jumpToList = new JumpToList();
            action = "GetByBaseTable";
        }

        public JumpToCommand(Int32 dataviewId, bool matchBaseTableOnly, JumpToListType[] jumpToListTypes)
        {
            SetDatabaseName();
            _dataviewid = dataviewId;
            _matchBaseTableOnly = matchBaseTableOnly;
            _jumptolisttypeList = jumpToListTypes;
            _jumpToList = new JumpToList();
            action = "GetByBaseTableList";

        }

        public JumpToCommand(Int32 dataviewId, Int32 fieldID, JumpToListType jumpToListType)
        {
            SetDatabaseName();
            _dataviewid = dataviewId;
            _fieldID = fieldID;
            _jumptolisttype = jumpToListType;
            _jumpToList = new JumpToList();
            action = "GetByFieldId";
        }

        public JumpToCommand(Int32 dataviewId, Int32 fieldID, JumpToListType[] jumpToListTypes)
        {
            SetDatabaseName();
            _dataviewid = dataviewId;
            _fieldID = fieldID;
            _jumptolisttypeList = jumpToListTypes;
            _jumpToList = new JumpToList();
            action = "GetByFieldIdList";

        }

        #endregion


        #region Methods

        protected override void DataPortal_Execute()
        {
            //Logic to create DAL
            //Query db to get data to construct messaging type
            //_JumpToList = transform ds/datatable to jumptolist
            if (_coeDAL == null)
                LoadDAL();

            switch (action)
            {
                case "GetByBaseTable":
                    try
                    {
                        DataSet ds = _coeDAL.GetByBaseTable(_dataviewid, _matchBaseTableOnly, _jumptolisttype);

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            _jumpToList = new JumpToList();
                            JumpTo j = null;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                j = new JumpTo();
                                j.SourceDataviewId = Int32.Parse(dr["SourceDataViewId"].ToString());
                                j.SourceDataviewName = dr["SourceDataViewName"].ToString();
                                j.SourceTableId = Int32.Parse(dr["SourceTableId"].ToString());
                                j.SourceTablealias = dr["SourceTableName"].ToString();
                                j.SourceFieldId = Int32.Parse(dr["SourceFieldId"].ToString());
                                j.SourceFieldAlias = dr["SourceFieldName"].ToString();

                                j.TargetDataviewId = Int32.Parse(dr["TargetDataViewId"].ToString());
                                j.TargetDataviewName = dr["TargetDataViewName"].ToString();
                                j.TargetTableId = Int32.Parse(dr["TargetTableId"].ToString());
                                j.TargetTableAlias = dr["TargetTableName"].ToString();
                                j.TargetFieldId = Int32.Parse(dr["TargetFieldId"].ToString());
                                j.TargetFieldAlias = dr["TargetFieldName"].ToString();

                                _jumpToList.Add(j);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    break;
                case "GetByBaseTableList":

                    break;
                case "GetByFieldId":
                    try
                    {
                        DataSet ds = _coeDAL.GetByFieldId(_dataviewid, _fieldID, _jumptolisttype);

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            _jumpToList = new JumpToList();
                            JumpTo j = null;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                j = new JumpTo();
                                j.SourceDataviewId = Int32.Parse(dr["SourceDataViewId"].ToString());
                                j.SourceDataviewName = dr["SourceDataViewName"].ToString();
                                j.SourceTableId = Int32.Parse(dr["SourceTableId"].ToString());
                                j.SourceTablealias = dr["SourceTableName"].ToString();
                                j.SourceFieldId = Int32.Parse(dr["SourceFieldId"].ToString());
                                j.SourceFieldAlias = dr["SourceFieldName"].ToString();

                                j.TargetDataviewId = Int32.Parse(dr["TargetDataViewId"].ToString());
                                j.TargetDataviewName = dr["TargetDataViewName"].ToString();
                                j.TargetTableId = Int32.Parse(dr["TargetTableId"].ToString());
                                j.TargetTableAlias = dr["TargetTableName"].ToString();
                                j.TargetFieldId = Int32.Parse(dr["TargetFieldId"].ToString());
                                j.TargetFieldAlias = dr["TargetFieldName"].ToString();

                                _jumpToList.Add(j);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    break;
                case "GetByFieldIdList":

                    break;
            }
        }

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        #endregion

    }

}

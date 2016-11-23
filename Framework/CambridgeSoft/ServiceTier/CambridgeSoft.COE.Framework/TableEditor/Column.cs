using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    /// <summary>
    /// This class represents table column information for creating business object.
    /// </summary>
    [Serializable]
    public class Column
    {
        #region Variables

        string _fieldName;
        string _validationRule;
        object _fieldValue;
        DbType _fieldType;
        static Dictionary<DbType, Type> dicType = null;

        #endregion

        #region Constructor

        public Column(string _fieldName, DbType FieldType)
        {
            FieldName = _fieldName;
            this.FieldType = FieldType;
        }
        public Column(string _fieldName, DbType FieldType,string _validationRule)
        {
            FieldName = _fieldName;
            this.FieldType = FieldType;
            ValidationRule = _validationRule;
        }

        #endregion

        #region  Static properties
        /// <summary>
        /// data type cast
        /// </summary>
        public static Dictionary<DbType, Type> DataTypeCast
        {
            get
            {
                if (dicType == null)
                {
                    dicType = new Dictionary<DbType, Type>();
                    dicType.Add(DbType.Int64, typeof(Int64));
                    dicType.Add(DbType.Double, typeof(Double));
                    dicType.Add(DbType.AnsiString, typeof(String));
                    dicType.Add(DbType.DateTime, typeof(DateTime));
                }
                return dicType;
            }
        }

        #endregion

        #region Properties

        public string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }

        public string ValidationRule
        {
            get
            {
                return _validationRule;
            }
            set
            {
                _validationRule = value;
            }
        }

        public object FieldValue
        {
            get
            {
                return _fieldValue;
            }
            set
            {
                _fieldValue = value;
            }
        }

        public DbType FieldType
        {
            get
            {
                return _fieldType;
            }
            set
            {
                _fieldType = value;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using Csla;
using Csla.Data;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Essentially the gateway for fetching 
    /// </summary>
    [Serializable()]
    public class PicklistDomain : RegistrationBusinessBase<Picklist>
    {
        [Serializable()]
        private class Criteria
        {
            private string _Description;
            public string Description
            {
                get
                {
                    return _Description;
                }
                protected set
                {
                    _Description = value;
                    return;
                }
            }

            private int _Id;
            public int Id
            {
                get
                {
                    return _Id;
                }
                protected set
                {
                    _Id = value;
                    return;
                }
            }
            
            protected Criteria(int vId, string vDescription)
            {
                Id = vId;
                Description = vDescription;
                return;
            }

            public static Criteria GetCriteria(string vDescription)
            {
                return new Criteria(-1, vDescription);
            }
            public static Criteria GetCriteria(int vId)
            {
                return new Criteria(vId, null);
            }
        }

        private string _extTableName;
        /// <summary>
        /// The name of the database table or view from which the picklist's values
        /// will be derived.
        /// </summary>
        public string TableName { get { return _extTableName; } }

        private string _extIdColumn;
        /// <summary>
        /// The column that will contain the value members for the picklist.
        /// </summary>
        public string IdColumn { get { return _extIdColumn; } }

        private string _extDiplayCol;
        /// <summary>
        /// The column that will contain the display members for the picklist.
        /// </summary>
        public string DisplayColumn { get { return _extDiplayCol; } }

        private string _extSqlFilter;
        /// <summary>
        /// Any filter statement (where clause) required to derive the proper subset
        /// of records serving as this picklist's item-set.
        /// </summary>
        public string SqlFilter { get { return _extSqlFilter; } }

        private string _sql;
        /// <summary>
        /// The constructed SQL statement that will be used to derive the picklist's
        /// data items.
        /// </summary>
        public string PickListDomainSql 
        { 
            get
            {
                _sql = FrameworkUtils.ReplaceSpecialTokens(_sql, true);
                return _sql;
            } 
        }

        private int _id;
        public int Identifier
        {
            get
            {
                if (_id == 0)
                {
                    _id = IdList[0];
                }
                return _id; 
            }
        }

        private string _Description = null;
        /// <summary>
        /// General-purpose description of the picklist.
        /// </summary>
        public string Description
        {
            get
            {
                CanReadProperty(true);
                return _Description;
            }
            protected set
            {
                _Description = value;
                return;
            }
        }

        private List<int> _IdList = new List<int>();
        public List<int> IdList
        {
            get
            {
                CanReadProperty(true);
                return _IdList;
            }
            protected set
            {
                _IdList = value;
                return;
            }
        }
        
        // Required by abstract inheritance (obsolete on CLSA 3.5)
        protected override object GetIdValue()
        {
            return _IdList;
        }

        [COEUserActionDescription("GetPicklistDomain")]
        public static PicklistDomain GetPicklistDomain(string vDescription)
        {
            try
            {
                return DataPortal.Fetch<PicklistDomain>(PicklistDomain.Criteria.GetCriteria(vDescription));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetPicklistDomain")]
        public static PicklistDomain GetPicklistDomain(int vId)
        {
            try
            {
                return DataPortal.Fetch<PicklistDomain>(PicklistDomain.Criteria.GetCriteria(vId));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private void DataPortal_Fetch(Criteria vCriteria)
        {
            SafeDataReader dr = null;

            try
            {
                if (vCriteria.Description == null)
                    dr = this.RegDal.GetPicklistDomains(vCriteria.Id);
                else
                    dr = this.RegDal.GetPicklistDomains(vCriteria.Description);

                while (dr.Read())
                {
                    IdList.Add(dr.GetInt32(0));
                    if (Description == null)
                    {
                        Description = dr.GetString(1);
                    }
                    this._extTableName = dr.GetString("EXT_TABLE");
                    this._extIdColumn = dr.GetString("EXT_ID_COL");
                    this._extDiplayCol = dr.GetString("EXT_DISPLAY_COL");
                    this._extSqlFilter = dr.GetString("EXT_SQL_FILTER");
                }
            }
            finally
            {
                dr.Close();
            }
            this.BuildSql();
                
            return;
        }

        private void BuildSql()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(this._extTableName))
            {
                sqlBuilder.Append("SELECT ");
                sqlBuilder.Append(this._extIdColumn);
                sqlBuilder.Append(" AS KEY, ");
                sqlBuilder.Append(this._extDiplayCol);
                sqlBuilder.Append(" AS VALUE FROM ");
                sqlBuilder.Append(this._extTableName);
                if (!string.IsNullOrEmpty(this._extSqlFilter))
                    sqlBuilder.Append(" " + this._extSqlFilter);
            }
            else
            {
                sqlBuilder.Append("SELECT ID AS KEY, PICKLISTVALUE AS VALUE FROM REGDB.PICKLIST");
                sqlBuilder.Append(" WHERE PICKLISTDOMAINID = " + this._IdList[0].ToString());
            }

            this._sql = sqlBuilder.ToString();
        }

    }
}

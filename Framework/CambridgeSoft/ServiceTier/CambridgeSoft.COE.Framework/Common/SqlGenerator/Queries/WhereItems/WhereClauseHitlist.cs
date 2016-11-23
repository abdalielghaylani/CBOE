using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereClauseHitlist : WhereClauseBinaryOperation
    {
        #region Variables
        private HitListType _hitlistType;
        private string _sourceJoiningFieldStr;
        #endregion

        #region Properties
        public HitListType HitlistType
        {
            get { return _hitlistType; }
            set { _hitlistType = value; }
        }

        public string SourceJoiningFieldStr
        {
            get { return _sourceJoiningFieldStr; }
            set { _sourceJoiningFieldStr = value; }
        }
        #endregion

        #region Constructors
        public WhereClauseHitlist()
        {
            _hitlistType = HitListType.TEMP;
            this.dataField = new Field();
            this.val = new Value();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Builds a string of type: hitlist.hitlistid = :values.Count
        /// Extra joins needed are added in the processof.
        /// 
        /// The final string, including processor work would look like:
        /// hitlist1.hitlistid = :values.count
        /// [and datafield.dependantstr = sourceJoiningField.dependantstr]
        /// and hitlist1.id = sourcePkField                                   --added in processor
        /// [sourcejoiningField JOIN to sourcePkField]                        --added in processor
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder();
            string hitlistTableName = "\"hitlist1\"";

            builder.Append(hitlistTableName);
            builder.Append(".\"HITLISTID\" = ");

            builder.Append(this.ParameterHolder);
            if (this.UseParametersByName)
                builder.Append(values.Count);

            val.Type = dataField.FieldType;
            values.Add(val);

            return builder.ToString();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    public class WhereClauseStructureList : WhereClauseNAryOperation
    {
        #region Variables
        private string[] _molecules;
        #endregion

        #region Properties
        public string[] Molecules
        {
            get
            {
                return _molecules;
            }
            set
            {
                _molecules = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseStructureList()
            : base()
        {
            this.dataField = new Field();
			this.values = new Value[0];
            _molecules = new string[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the string representation of a list of where clause structure (OR concatenated) for the given database and
        /// adds its parameter values to the values list.
        /// </summary>
        /// <param name="databaseType">The database to get the string for.</param>
        /// <param name="values">Already existing parameter values or a query.</param>
        /// <returns>The string representation.</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            /*
             dbms_lob.substr("TempTBL".QUERY, 32, 1) = "CanonicalTBL".CTEXT
             AND "SUBSTANCE"."ROWID" = "CanonicalTBL"."RID"
             */
            if (databaseType != DBMSType.ORACLE)
            {
                throw new Exception("This clause only works in Oracle implementations");
            }
            if(_molecules.Length > 0)
            {
                StringBuilder builder = new StringBuilder("dbms_lob.substr(\"TempTBL\".QUERY, 32, 1) = \"CanonicalTBL\".CTEXT AND ");
                builder.Append(this.dataField.Table.GetAlias());
                builder.Append(".\"ROWID\" = \"CanonicalTBL\".\"RID\"");
                return builder.ToString();
            }
            else
                return string.Empty;
        }
        #endregion
    }
}

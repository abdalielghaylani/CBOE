using System;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    /// <summary>
    /// This class is intended to be inherited and implemented. This one represents an operation or keyword
    /// of a SQL select statement.
    /// Let's suppose we have the following select:
    /// SELECT EMPLOYEEID FROM EMPLOYEES WHERE FIRSTNAME='DAVID' AND LASTNAME LIKE ('GOSAL')
    /// Here we shoud have 2 Where clause items 1 for the equal and other for the like.
    /// </summary>
    public abstract class WhereClauseItem : WhereClauseBase {
        #region Variables
        private string parameterHolder;
        private bool useParametersByName;
        private bool negate;
        #endregion

        #region Properties
        /// <summary>
        /// The character to use in the resulting prepared statement for indicating a parameter position (i.e. the parameter holder).
        /// </summary>
        public string ParameterHolder {
            get {
                return this.parameterHolder;
            }
            set {
                this.parameterHolder = value;
            }
        }

        /// <summary>
        /// Indicates the way parameters are specified: 
        ///		if true parameters are specified by name (and an ordinal is appended to te character)
        ///		otherwise parameters are specified by position (and nothing is appended)
        /// </summary>
        public bool UseParametersByName {
            get {
                return this.useParametersByName;
            }
            set {
                this.useParametersByName = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool Negate {
            get { return negate; }
            set { negate = value; }
        }
        #endregion

        #region Constants
        private const string notString = "NOT ";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public WhereClauseItem() {
            this.ParameterHolder = ":";
            this.UseParametersByName = true;
            this.Negate = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Basically execute should call the method GetDependantString after setting the database type.
        /// if any other functionality is needed the method may be overriden.
        /// </summary>
        /// <param name="databaseType">The database type. I.E.: Oracle 9i, MSSQL 2005, ... All of 
        /// them should be in an enumeration.</param>
        /// <param name="values">The list of values to add in it its values.</param>
        /// <returns>A string representing this part of the clause select. I.E.: FIRSTNAME='david'</returns>
        public override string Execute(DBMSType databaseType, List<Value> values) {
            return (this.Negate) ? notString + this.GetDependantString(databaseType, values) :
                                   this.GetDependantString(databaseType, values);
        }
        #endregion
    }
}

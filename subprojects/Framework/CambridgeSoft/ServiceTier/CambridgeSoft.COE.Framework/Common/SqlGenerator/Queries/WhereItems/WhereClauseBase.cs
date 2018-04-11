using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// </summary>
    public abstract class WhereClauseBase : ICloneable
    {
        #region Variables
        private string _hint;
        private string _aggregateFunctionName;
        #endregion

        #region Properties
        public string Hint {
            get {
                return _hint;
            }
            set {
                _hint = value;
            }
        }

        public string AggregateFunctionName
        {
            get
            {
                return _aggregateFunctionName;
            }
            set
            {
                _aggregateFunctionName = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// </summary>
        public WhereClauseBase()
        {
            _hint = string.Empty;
            _aggregateFunctionName = string.Empty;
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
        public virtual string Execute(DBMSType databaseType, List<Value> values)
        {
            return this.GetDependantString(databaseType, values);
        }

        /// <summary>
        /// This method must be overriden to return a representation of this part of the select clause.
        /// If different strings are needed for different databases, here should be implemented.
        /// </summary>
        /// <param name="databaseType">The database to generate code to.</param>
        /// <param name="values">The list of values to add in it its values.</param>
        /// <returns>A string representing this part of the clause select for the specific 
        /// database. I.E.: FIRSTNAME='david' or FIRSTNAME="david"</returns>
        protected abstract string GetDependantString(DBMSType databaseType, List<Value> values);

        protected string GetFullName(Field dataField)
        {
            StringBuilder builder = new StringBuilder();
            bool applyFunction = !string.IsNullOrEmpty(_aggregateFunctionName);
            if (applyFunction)
            {
                builder.Append(_aggregateFunctionName);
                builder.Append("(");
            }
            builder.Append(dataField.GetFullyQualifiedNameString());
            if (applyFunction)
                builder.Append(")");

            return builder.ToString();
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a copy of the WhereClause object.
        /// </summary>
        /// <returns>The cloned WhereClause object</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}

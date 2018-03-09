using System;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// Base class for select clause items, like fields or functions to return in a select.
	/// This class is intended to be inherited and implemented, and not instanced directly (as it's abstract). 
	/// It represents a field to be added to the select clause, wich needs certain format. 
	/// Let's suppose we have the following select:
	/// SELECT SUBSTANCEID, MOLWEIGHT(SUBSTANCEID) FROM SUBSTANCES
	/// To build this query we would need to instance two SelectClauseItems: one for the substanceid column, 
	/// and one for the function molweight wich operates on column substanceid.
    /// </summary>
    public abstract class SelectClauseItem : ICloneable
	{
		#region Variables
        //set default vakue of the alias to string.empty to avoid exception for aggregate clause 28-jun-2013
		private string alias = string.Empty;
        private bool visible = true;
        /// <summary>
        /// Name of the DataBase Field to be included in the SELECT clause.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected IColumn dataField;

        /// <summary>
        /// Id of the DataBase Field.
        /// </summary>
        protected int fieldId;
		#endregion

		#region Properties
		public  virtual string Alias {
			get  
            {
                if (!string.IsNullOrEmpty(alias) && alias.Length > 30)
                {
                    string hashStr = GetHashCode().ToString();
                    return alias.Substring(0, 29 - hashStr.Length) + "|" + hashStr;
                }
                return alias; 
            }
			set { alias = value; }
		}

        internal string FullAlias
        {
            get
            {
                return alias;
            }
        }

		public abstract string Name {
			get;
		}

        public virtual bool Visible
        {
            get {
                return this.visible;
            }
            set {
                this.visible = value;
            }
        }

        /// <summary>
        /// Name of the DataBase Field to be included in the SELECT clause.
        /// We recommend using fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        public virtual IColumn DataField
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
		#endregion

		#region Methods
		/// <summary>
		/// Basically execute should call the method GetDependantString after setting the database type.
		/// if any other functionality is needed the method may be overriden.
		/// </summary>
        /// <param name="dataBaseType">The database type. I.E.: Oracle 9i, MSSQL 2005, a member of 
		///  enumeration DBMSType enum.</param>
		/// <returns>A string representing this part of the clause select. I.E.: MolWeight(MoleculeID)</returns>
		public virtual string Execute(DBMSType dataBaseType, List<Value>values) 	{
			return GetDependantString(dataBaseType, values);
		}

		/// <summary>
		/// This method must be overriden to return a representation of this part of the select clause.
		/// If different strings are needed for different databases, this should be solved here.
		/// </summary>
		/// <returns>A string representing this part of the clause select for the specific 
		/// database. I.E.: CONVERT(101, '6/8/2006') or (DateTime)"6/8/2006" "</returns>		
        protected abstract string GetDependantString(DBMSType dataBaseType, List<Value> values);

		//protected abstract string GetNameString(DBMSType dataBaseType);
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a copy of the SelectClauseItem object.
        /// </summary>
        /// <returns>The cloned SelectClauseItem object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is SelectClauseItem)
            {
                // Two select clause items are defined to be equal if they emit 
                // the same SQL output to the select clause, but we need to make
                // sure to include the alias part of the clause in the comparison
                // The Execute() method of a SelectClauseItem does not include the
                // alias so we need to add it to the comparison.
                
                // Build the comparison strings for the passed in object (obj)
                SelectClauseItem sci = (SelectClauseItem)obj;
                string objSQL = sci.Execute(DBMSType.ORACLE, new List<Value>());
                string objAlias = (sci.Alias == null) ? string.Empty : sci.Alias;

                // Build the comparison strings for the current (this) object
                string thisSQL = this.Execute(DBMSType.ORACLE, new List<Value>());
                string thisAlias = (this.Alias == null) ? string.Empty : this.Alias;

                // Compare and return boolean
                return (objSQL + objAlias == thisSQL + thisAlias);            
            }

            return false;
        }
    }
}

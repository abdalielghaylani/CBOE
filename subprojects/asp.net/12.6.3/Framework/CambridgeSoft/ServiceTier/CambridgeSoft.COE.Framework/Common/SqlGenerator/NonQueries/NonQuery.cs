using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries
{
	/// <summary>
	/// Abstract superclass of non queries (inserts and updates)
	/// </summary>
	public abstract class NonQuery
	{
		#region Variables
		private List<Field> fieldsList;
		private List<Value> valuesList;
		private Table mainTable;
		private DBMSType dataBaseType;

		private string parameterHolder;
		private bool useParametersByName;
        private bool useParameters;
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
		///		if true parameters are specified by name (and an ordinal is appended to te character. ie ':1')
		///		otherwise parameters are specified by position (and nothing is appended. ie ':')
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
        /// Determines whether produce a prepared statment (with parameter holders and actual parameters in the paramValues List), or a plain query.
        /// </summary>
        public bool UseParameters
        {
            get { return useParameters; }
            set { useParameters = value; }
        }
		/// <summary>
		/// A List of columns to be updated/inserted.
		/// </summary>
		public List<Field> Fields {
			get {
				return this.fieldsList;
			}
			set {
				this.fieldsList = value;
			}
		}

		/// <summary>
		/// The values to be inserted or updated to. 
		/// These parameters will be also needed when executing the prepared statement.
		/// In the case of an insert with a select query, the parameters of the query are copied here too.
		/// </summary>
		public List<Value> ParamValues {
			get {
				return this.valuesList;
			}
			set {
				this.valuesList = value;
			}
		}

		/// <summary>
		/// Main table upon wich is to be executed the non query.
		/// </summary>
		public Table MainTable {
			get {
				return this.mainTable;
			}
			set {
				this.mainTable = value;
			}
		}

		/// <summary>
		/// Target Database for wich the nonquery will be generated. Posible values = ORACLE, MSSQLSERVER, ACCESS
		/// </summary>
		public DBMSType DataBaseType {
			get { 
				return dataBaseType; 
			}
			set { 
				dataBaseType = value; 
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		public NonQuery() {
			this.fieldsList = new List<Field>();
			this.valuesList = new List<Value>();
			this.UseParametersByName = true;
			this.parameterHolder = ":";
            this.useParameters = true;
		}
		#endregion

		#region Methods
		/// <summary>
        /// Returns the non query itself as a string, and the list of parameters.
		/// </summary>
        /// <param name="databaseType">The DBMSType.</param>
        /// <returns>The string of the NON-QUERY statement.</returns>
		public abstract string GetDependantString(DBMSType databaseType);
		
        /// <summary>
        /// Overriden.
        /// Returns the NON-QUERY itself as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            if (this.UseParameters)
                return GetDependantString(this.dataBaseType);
            else
                return DeparametrizedQuery(GetDependantString(this.dataBaseType));
		}
        private string DeparametrizedQuery(string query)
        {
            string replacedQuery = query;

            int position = 0;
            int currentParameter = 0;
            foreach (Value currentValue in this.ParamValues)
            {
                if ((position = this.ReplaceNextParamterHolder(ref replacedQuery, position, currentParameter, currentValue)) == -1)
                    throw new SQLGeneratorException(Resources.CannotReplaceParamInLiteral.Replace("&currentVal", currentValue.Val));
                currentParameter++;
            }

            return replacedQuery;
        }

        private int ReplaceNextParamterHolder(ref string verbatim, int currentPosition, int nextParamOrdinal, Value nextParamValue)
        {
            string parameterHolderString = this.parameterHolder + (this.UseParametersByName ? nextParamOrdinal.ToString() : "");

            int nextHolderPosition = verbatim.IndexOf(parameterHolderString, currentPosition);

            if (nextHolderPosition >= 0)
            {
                string originalVerbatim = verbatim;

                verbatim = originalVerbatim.Substring(0, nextHolderPosition);
                verbatim = verbatim + nextParamValue.ToString();
                if (this.UseParametersByName)
                {
                    verbatim += (nextHolderPosition + 1 < originalVerbatim.Length ?
                                            originalVerbatim.Substring(nextHolderPosition + 2, originalVerbatim.Length - nextHolderPosition - 2) :
                                            string.Empty);
                }
                else
                    verbatim += (nextHolderPosition < originalVerbatim.Length ?
                                            originalVerbatim.Substring(nextHolderPosition + 1, originalVerbatim.Length - nextHolderPosition - 1) :
                                            string.Empty);
            }
            return nextHolderPosition;
        }
		#endregion
	}
}

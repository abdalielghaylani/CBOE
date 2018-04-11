using System;
using System.Text;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries
{
	/// <summary>
	/// A wrapper for a join clause. Expose methods to modify the join clause.
	/// </summary>
	public class JoinClause
	{
		#region Properties
		/// <summary>
		/// The main table name of a query. In SQL terms it defines wich is the table after FROM clause.
		/// </summary>
		public ISource MainSource {
			get {
				return this.mainSource;
			}
			set {
				mainSource = value;
			}
		}

        /// <summary>
        /// List of relationships.
        /// </summary>
        public List<Relation> Relations
        {
            get { return this.relations; }
        }
		#endregion

		#region Variables
		/// <summary>
		/// The list of relations of a query. This is later rendered into "join", "inner join", or a list of tables.
		/// </summary>
		private List<Relation> relations;

		/// <summary>
		/// The main table of a query. In SQL terms it defines wich is the table after FROM clause.
		/// </summary>
		private ISource mainSource;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes its members to its default values.
		/// </summary>
		public JoinClause() {
			this.relations = new List<Relation>();
			this.mainSource = null;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adds a relation to the join clause. Depending on the database type will be rendered as JOIN or
		/// a list of tables and wheres.
		/// </summary>
		/// <param name="relation">The relation to add.</param>
		public void AddRelation(Relation relation) {
			if(!relation.Child.Table.Equals(this.mainSource))
				if(!this.relations.Contains(relation))
					this.relations.Add(relation);
		}

		/// <summary>
		/// Removes a relation from the join clause.
		/// </summary>
		/// <param name="relation">The relation to remove.</param>
		public void RemoveRelation(Relation relation) {
			this.relations.Remove(relation);
		}

		/// <summary>
		/// Removes a relation from the join clause.
		/// </summary>
		/// <param name="position">The position where is located the relation to remove.</param>
		public void RemoveRelation(int position) {
			this.relations.RemoveAt(position);
		}

		/// <summary>
		/// Gets the from part of a query, regarding to the join.
		/// </summary>
		/// <param name="dataBaseType">Determines which is the underlying database to generate SQL to.</param>
		/// <returns>The from part of a query. This can be a list of tables, a set of JOIN or a set of INNER JOIN.</returns>
		public string FromToString(DBMSType dataBaseType, List<Value> paramValues) {
			StringBuilder builder = new StringBuilder();

			if (mainSource != null) {
				if (mainSource.GetAlias() != string.Empty) {
                    //builder.Append("(");
                    builder.Append(mainSource.ToString(paramValues));
					builder.Append(" ");
					builder.Append(mainSource.GetAlias());
                    //builder.Append(")");
				}
                else
                    builder.Append(mainSource.ToString());

                //if(this.MainSource.ParamValues != null && this.MainSource.ParamValues.Count > 0)
                //    paramValues.AddRange(MainSource.ParamValues);
			}

			switch (dataBaseType) {
				case DBMSType.ORACLE:
                    StringBuilder plainTables = new StringBuilder(mainSource.GetAlias());
                    for (int i = 0; i < relations.Count; i++)
                    {
                        if (!plainTables.ToString().Contains(relations[i].Child.Table.GetAlias()))
                        {
                            plainTables.AppendFormat(",{0}", relations[i].Child.Table.GetAlias());
                            
                            builder.Append(", ");
                            builder.Append(relations[i].Child.Table.ToString());
                            if (relations[i].Child.Table.GetAlias().Trim() != string.Empty)
                            {
                                builder.Append(" ");
                                builder.Append(relations[i].Child.Table.GetAlias());
                            }

                            if (relations[i].Child.Table.ParamValues != null && relations[i].Child.Table.ParamValues.Count > 0)
                                paramValues.AddRange(relations[i].Child.Table.ParamValues);
                        }

                        if (!plainTables.ToString().Contains(relations[i].Parent.Table.GetAlias()))
                        {
                            plainTables.AppendFormat(",{0}", relations[i].Parent.Table.GetAlias());

                            builder.Append(", ");
                            builder.Append(relations[i].Parent.Table.ToString());
                            if (relations[i].Parent.Table.GetAlias().Trim() != string.Empty)
                            {
                                builder.Append(" ");
                                builder.Append(relations[i].Parent.Table.GetAlias());
                            }

                            if (relations[i].Parent.Table.ParamValues != null && relations[i].Parent.Table.ParamValues.Count > 0)
                                paramValues.AddRange(relations[i].Parent.Table.ParamValues);
                        }
                    }
					if (builder.ToString().Trim() == string.Empty)
						builder.Append("DUAL");
					break;
				case DBMSType.SQLSERVER:
					for (int i = 0; i < relations.Count; i++) {
						builder.Append(" ");
						if (relations[i].InnerJoin) {
							builder.Append("JOIN ");
						} else {
							builder.Append("LEFT OUTER JOIN ");
						}

						builder.Append(relations[i].Child.Table.ToString());

						if (relations[i].Child.Table.GetAlias().Trim() != string.Empty) {
							builder.Append(" ");
							builder.Append(relations[i].Child.Table.GetAlias());
						}
						if(relations[i].Child.Table.ParamValues != null && relations[i].Child.Table.ParamValues.Count > 0)
							paramValues.AddRange(relations[i].Child.Table.ParamValues);

						builder.Append(" ON ");
						builder.Append(relations[i].Parent.GetFullyQualifiedNameString());
						builder.Append(" = ");
						builder.Append(relations[i].Child.GetFullyQualifiedNameString());
					}
					break;
				case DBMSType.MSACCESS:
					for (int i = 0; i < relations.Count; i++) {
						builder.Append(" ");
						if (relations[i].InnerJoin) {
							builder.Append("INNER JOIN ");
						} else {
							builder.Append("LEFT OUTER JOIN ");
						}
						builder.Append(relations[i].Child.Table.ToString());
						if (relations[i].Child.Table.GetAlias().Trim() != "") {
							builder.Append(" ");
							builder.Append(relations[i].Child.Table.GetAlias());
						}

						if(relations[i].Child.Table.ParamValues != null && relations[i].Child.Table.ParamValues.Count > 0)
							paramValues.AddRange(relations[i].Child.Table.ParamValues);

						builder.Append(" ON ");
						builder.Append(relations[i].Parent.GetFullyQualifiedNameString());
						builder.Append(" = ");
						builder.Append(relations[i].Child.GetFullyQualifiedNameString());
					}
					break;
			}
			return builder.ToString();
		}

		/// <summary>
		/// Gets the where part of a join, regarding to the join.
		/// </summary>
		/// <returns>If the underlying database needs the "old sql style" a list of WHERE/AND is returned.</returns>
		public string WhereToString(DBMSType dataBaseType) {
			StringBuilder builder = new StringBuilder(string.Empty);
			switch (dataBaseType) {
				case DBMSType.ORACLE:
                    for (int i = 0; i < relations.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(relations[i].Parent.FieldName) &&
                            !string.IsNullOrEmpty(relations[i].Child.FieldName))
                        {
                            if(builder.Length > 0)
                                builder.Append(" AND ");

                            builder.Append(relations[i].Parent.GetFullyQualifiedNameString());
                            if (!relations[i].InnerJoin && !relations[i].LeftJoin)
                                builder.Append("(+)");

                            builder.Append(" = ");

                            builder.Append(relations[i].Child.GetFullyQualifiedNameString());
                            if (!relations[i].InnerJoin && relations[i].LeftJoin)
                                builder.Append("(+)");

                        }
                    }
					break;
			}
			return builder.ToString();
		}

		/// <summary>
		/// Determines whether the generated string of this join clause will be empty, based upon the underlying dbms
		/// </summary>
		/// <param name="dataBaseType">the target database for wich the join clause is supposed to be generated</param>
		/// <returns>true if nothing would be generated for the specified underlying database</returns>
		public bool IsEmpty(DBMSType dataBaseType) {
			if(this.MainSource == null &&
				(this.relations == null || this.relations.Count == 0) &&
				dataBaseType != DBMSType.ORACLE)
				return true;

			return false;
		}
		#endregion
	}
}

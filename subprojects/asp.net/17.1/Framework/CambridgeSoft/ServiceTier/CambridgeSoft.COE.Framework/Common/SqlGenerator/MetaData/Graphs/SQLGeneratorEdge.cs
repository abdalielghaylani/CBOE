using System;
using QuickGraph.Concepts;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs
{
    /// <summary>
    /// This class links SQLGenerator and QuickGraphLibrary. 
	/// The edges of the graph store relationships between tables (although not using Relation class).
    /// </summary>
	public class SQLGeneratorEdge : IEdge
    {
        #region Variables
        private SQLGeneratorVertex m_Source;
        private SQLGeneratorVertex m_Target;

        private int parentFieldId;
        private int childFieldId;
		private bool innerJoin;
        #endregion

        #region Properties
		/// <summary>
		/// The Key of the parent table that is used for linking to the other table.
		/// </summary>
        public int ParentFieldId {
            get {
                return this.parentFieldId;
            }
            set {
                this.parentFieldId = value;
            }
        }
        /// <summary>
        /// The Key of the Child table that is used for linking to the other table
        /// </summary>
		public int ChildFieldId {
            get {
                return this.childFieldId;
            }
            set {
				this.childFieldId = value;
            }
        }

		/// <summary>
		/// Determines whether it is an inner or outer join what is represented here.
		/// </summary>
		public bool InnerJoin {
			get {
				return this.innerJoin;
			}
			set {
				this.innerJoin = value;
			}
		}
        #endregion

        #region Constructors
		/// <summary>
		/// Default Constructors. Receives two SQLGenerator Vertices
		/// </summary>
		/// <param name="source">Source Vertex of the relationship. This Vertex contains the parent table name</param>
		/// <param name="target">Destination Vertex of the relationship. This vertex contains the child table name</param>
        public SQLGeneratorEdge(IVertex source, IVertex target) {
            if(source == null || !(source is SQLGeneratorVertex))
                throw new SQLGeneratorException(Resources.InvalidSource, new ArgumentNullException());
            if(target == null || !(source is SQLGeneratorVertex))
                throw new SQLGeneratorException(Resources.InvalidTarget, new ArgumentNullException());
            m_Source = (SQLGeneratorVertex) source;
            m_Target = (SQLGeneratorVertex) target;
        }
        #endregion

        #region IEdge Members
		/// <summary>
		/// Source Vertex of the relation. This Vertex contains the name of the Parent table 
		/// </summary>
        public IVertex Source {
            get {
                return m_Source;
            }
        }

		/// <summary>
		/// Target Vertex of the relation. This Vertex Contains the name of the Child Table
		/// </summary>
        public IVertex Target {
            get {
                return m_Target;
            }
        }

        #endregion

        #region IComparable Members
		/// <summary>
		/// Compare operator. Required for implementing IEdge interface.
		/// </summary>
		/// <param name="obj">Object to wich current instance is compared to</param>
		/// <returns></returns>
        public int CompareTo(object obj) {
            if(!(obj is SQLGeneratorEdge))
                throw new SQLGeneratorException(Resources.CannotCompare, new ArgumentException());
            return GetHashCode().CompareTo(obj.GetHashCode());
        }

        #endregion
    }
}

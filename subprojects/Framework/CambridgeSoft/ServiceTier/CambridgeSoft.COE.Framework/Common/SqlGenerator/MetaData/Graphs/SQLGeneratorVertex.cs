using System;
using QuickGraph.Concepts;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs
{
	/// <summary>
	/// Custom Vertex class required for integrating QuickGraph Library.
	/// SQLGeneratorVertex contains the name of the table it represents.
	/// </summary>
    public class SQLGeneratorVertex : IVertex
    {
        #region Properties
		/// <summary>
		/// The name of the table that is represented by this vertex in the graph
		/// </summary>
        public int TableId {
            get {
                return tableId;
            }
            set {
                tableId = value;
            }
        }
        #endregion

        #region Variables
        //private Field field;
        private int tableId;
        #endregion

        #region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
        public SQLGeneratorVertex() { }

        public SQLGeneratorVertex(int tableId) 
        {
            this.tableId = tableId;
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object obj) {
            if(!(obj is SQLGeneratorVertex))
                throw new SQLGeneratorException(Resources.CannotCompare, new ArgumentException());
            return this.tableId.CompareTo(((SQLGeneratorVertex)obj).tableId);
        }

        #endregion
    }
}

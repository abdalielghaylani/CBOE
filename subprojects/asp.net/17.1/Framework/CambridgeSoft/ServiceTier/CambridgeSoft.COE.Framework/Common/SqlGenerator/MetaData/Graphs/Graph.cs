using System;
using QuickGraph.Concepts;
using QuickGraph.Representations;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Visitors;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs
{
    /// <summary>
    /// Wrapper class to encapsulate the Graphs Library to be used.
    /// Used for representing the required table relations.
    /// </summary>
    public class Graph
    {
        #region Variables
        private SQLGeneratorAdjacencyGraph relationsGraph;
        #endregion

        #region Attributes
		/// <summary>
		/// This Property exposes the quickgraph lib graph that's being used. 
		/// For debugging Purposes only.
		/// </summary>
		public AdjacencyGraph adjacencyGraph {
            get {
                return relationsGraph;
            }
        }
        #endregion

        #region Constructors
		/// <summary>
		/// Default constructor.
		/// </summary>
        public Graph() {
            this.relationsGraph = new SQLGeneratorAdjacencyGraph(new SQLGeneratorVertexEdgeProvider(), true);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a Table to the Graph that stores tables and relationships between them.
        /// </summary>
		/// <param name="TableId">The identifier of the table. This int is stored inside the vertex</param>
        public void AddTable(int TableId) {
            SQLGeneratorVertex vertex = relationsGraph.AddVertex(TableId);
        }

        /// <summary>
        /// Adds an arist between two nodes to the Graph. - Review if Relation class will be used, or what
        /// </summary>
        /// <param name="relation">this objects stores the names of the two tables to link, as well as the keys that relates them.
		/// The names of the tables are converted to vertices before adding the edge. An exception is raised if the tables names 
		/// are not included as vertices on the graph.
		/// </param>
        public void AddRelation(Relation relation) {

            if(relation.Parent.Table.GetType() == typeof(Table)) {
                SQLGeneratorVertex parent = this.FindVertex(((Table) relation.Parent.Table).TableId);
                SQLGeneratorVertex child = this.FindVertex(((Table) relation.Child.Table).TableId);

                SQLGeneratorEdge edge = (SQLGeneratorEdge) relationsGraph.AddEdge(parent, child);
                edge.ParentFieldId = relation.Parent.FieldId;
                edge.ChildFieldId = relation.Child.FieldId;
                edge.InnerJoin = relation.InnerJoin;

                edge = (SQLGeneratorEdge) relationsGraph.AddEdge(child, parent);
                edge.ParentFieldId = relation.Child.FieldId;
                edge.ChildFieldId = relation.Parent.FieldId;
                edge.InnerJoin = relation.InnerJoin;
            }
        }

        private SQLGeneratorVertex FindVertex(int tableId) {
            SQLGeneratorVertex vertex = relationsGraph.FindVertex(tableId);
            //foreach(SQLGeneratorVertex currentVertex in relationsGraph.Vertices)
            //    if(currentVertex.TableId == tableId)
            //        return currentVertex;
            if(vertex == null)
                throw new SQLGeneratorException(Resources.VertexNotFound.Replace("&id", tableId.ToString()));

            return vertex;
        }

        /// <summary>
        /// Returns an optimized path between starting in OriginNode and ending in DestinationNode
        /// </summary>
		/// <param name="originTableId">The source table id.</param>
		/// <param name="destinationTableId">The destination table id.</param>
        /// <returns></returns>
        public List<Relation> GetPath(int originTableId, int destinationTableId) {
            BreadthFirstSearchAlgorithm dephFirstSearchAlgorithm = new BreadthFirstSearchAlgorithm(this.adjacencyGraph);
            PredecessorRecorderVisitor predecesorRecVisitor = new PredecessorRecorderVisitor();

            dephFirstSearchAlgorithm.RegisterPredecessorRecorderHandlers(predecesorRecVisitor);

            IVertex originVertex = FindVertex(originTableId);
            dephFirstSearchAlgorithm.Compute(originVertex);

            List<Relation> resultRelationList = new List<Relation>();


            IVertex currentNode = FindVertex(destinationTableId);
            while(predecesorRecVisitor.Predecessors[currentNode] != null) {
                Relation resultRelation = new Relation();

                SQLGeneratorEdge linkingEdge = (SQLGeneratorEdge) predecesorRecVisitor.Predecessors[currentNode];

                //resultRelation.Parent.Table.TableId = ((SQLGeneratorVertex) linkingEdge.Source).TableId;
                resultRelation.Parent.FieldId = linkingEdge.ParentFieldId;

                //resultRelation.Child.Table.TableId = ((SQLGeneratorVertex) linkingEdge.Target).TableId;
                resultRelation.Child.FieldId = linkingEdge.ChildFieldId;
				resultRelation.InnerJoin = linkingEdge.InnerJoin;

				if(!resultRelationList.Contains(resultRelation)) {
					resultRelationList.Insert(0, resultRelation);
				}
                currentNode = linkingEdge.Source;
            }

            return resultRelationList;

            //resultRelation.
        }
        #endregion
    }
}

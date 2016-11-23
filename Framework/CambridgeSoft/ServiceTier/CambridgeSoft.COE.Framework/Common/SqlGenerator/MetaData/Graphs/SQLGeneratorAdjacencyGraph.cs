using System;
using System.Collections.Generic;
using System.Text;
using QuickGraph.Representations;
using QuickGraph.Concepts;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs
{
    public class SQLGeneratorAdjacencyGraph : AdjacencyGraph
    {
        private Dictionary<int, SQLGeneratorVertex> _vertexDict = new Dictionary<int, SQLGeneratorVertex>();
        public SQLGeneratorAdjacencyGraph(SQLGeneratorVertexEdgeProvider provider, bool allowParallelEdges)
            : base(provider, allowParallelEdges)
        {
        }

        public SQLGeneratorVertex AddVertex(int tableId)
        {
            //Coverity Bug Fix :- CID : 13985 Jira Id :CBOE-194
            SQLGeneratorVertex vertex = base.AddVertex() is SQLGeneratorVertex ? base.AddVertex() as SQLGeneratorVertex : null;
            if (vertex != null)
            {
                vertex.TableId = tableId;

                if (!_vertexDict.ContainsKey(tableId))
                    _vertexDict.Add(tableId, vertex);
            }
            return vertex;
        }

        public override void RemoveVertex(IVertex v)
        {
            if (v is SQLGeneratorVertex)
            {
                SQLGeneratorVertex vertex = v as SQLGeneratorVertex;
                if (_vertexDict.ContainsKey(vertex.TableId))
                    _vertexDict.Remove(vertex.TableId);
            }
            base.RemoveVertex(v);
        }
        public SQLGeneratorVertex FindVertex(int tableId)
        {
            if (_vertexDict.ContainsKey(tableId))
                return _vertexDict[tableId];
            else
                return null;
        }
    }
}

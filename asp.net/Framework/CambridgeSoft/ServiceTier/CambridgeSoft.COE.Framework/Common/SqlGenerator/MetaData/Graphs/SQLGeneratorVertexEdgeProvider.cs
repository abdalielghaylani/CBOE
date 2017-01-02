using System;
using QuickGraph.Concepts.Providers;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs
{
	/// <summary>
	/// Custom Edge and Vertex Creator, required for integrating QuickGraph Library into SQLGenerator.
	/// Its only purpose is to create SQLGeneratorVertexs and SQLGeneratorEdges.
	/// </summary>
    public class SQLGeneratorVertexEdgeProvider : IVertexAndEdgeProvider
    {
        #region IVertexProvider Members

        QuickGraph.Concepts.IVertex IVertexProvider.ProvideVertex() {
            return new SQLGeneratorVertex();
        }

        #endregion

        #region IEdgeProvider Members

        QuickGraph.Concepts.IEdge IEdgeProvider.ProvideEdge(QuickGraph.Concepts.IVertex u, QuickGraph.Concepts.IVertex v) {
            return new SQLGeneratorEdge(u, v);
        }

        #endregion
    }
}

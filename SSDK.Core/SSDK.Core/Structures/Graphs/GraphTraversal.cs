using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    /// <summary>
    /// Represents a traversal of a given graph.
    /// </summary>
    public sealed class GraphTraversal<T>
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of traversal that occured on the graph.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Gets the graph that the traversal happened on.
        /// </summary>
        /// <remarks>
        /// Note that this field is a direct reference to the graph, so any
        /// changes to the graph may conflict with this traversal.
        /// </remarks>
        public Graph<T> On { get; private set; }

        /// <summary>
        /// Gets the vertex states according to vertex index
        /// </summary>
        public int[] VertexStates { get; private set; }

        /// <summary>
        /// Gets the edge states according to edge index
        /// </summary>
        public int[] EdgeStates { get; private set; }
        #endregion

        #region Methods
        #region Constructor
        public GraphTraversal(Graph<T> on, string type=null)
        {
            Type = type;
            On = on;
            VertexStates = new int[on.Vertices.Count];
            EdgeStates = new int[on.Edges.Count];

            // Update vertices and edges of graph
            on.UpdateIndexReferences();
        }
        #endregion
        #endregion

        public override string ToString()
        {
            if (Type == null) return $"Graph-Traversal(On {On})";
            return $"{Type}-Traversal(On {On})";
        }
    }
}

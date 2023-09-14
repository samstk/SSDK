using SSDK.Core.Structures.Primitive;
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

        /// <summary>
        /// Gets the weight states according to vertex
        /// </summary>
        public UncontrolledNumber[] VertexWeights { get; private set; }

        /// <summary>
        /// Typically unmodified except on certain algorithms such as
        /// topological sort, where the configuration is the topological order
        /// (indices)
        /// </summary>
        public int[] Configuration { get; set; }

        #endregion

        #region Methods
        #region Constructor
        public GraphTraversal(Graph<T> on, string type=null)
        {
            Type = type;
            On = on;
            VertexStates = new int[on.Vertices.Count];
            EdgeStates = new int[on.Edges.Count];
            VertexWeights = new UncontrolledNumber[on.Vertices.Count];

            // Update vertices and edges of graph
            on.UpdateIndexReferences();
        }

        #endregion
        #region Modification
        /// <summary>
        /// Resets all vertex/edge states in the traversal.
        /// Does not change configuration.
        /// </summary>
        /// <param name="newType">if specified, overwrites the current type reference</param>
        public void Reset(string newType = null)
        {
            if (newType != null)
            {
                Type = newType;
            }

            VertexStates = new int[VertexStates.Length];
            EdgeStates = new int[EdgeStates.Length];
            VertexWeights = new UncontrolledNumber[VertexWeights.Length];
        }
        #endregion
        #region Searching
        /// <summary>
        /// Gets the path between the initially traversed vertex and the target vertex.
        /// </summary>
        /// <param name="target">the target vertex that was reached in the traversal</param>
        /// <param name="desiredEdgeState">
        /// the edge state that allows a path to be made
        /// (defaults to 1 as this is generally used for discovery edges)
        /// </param>
        /// <returns>a list of edges that is a path from the initial vertex to the target vertex</returns>
        public List<GraphEdge<T>> GetPathBackFrom(GraphVertex<T> target, int desiredEdgeState=1)
        {
            List<GraphEdge<T>> path = new List<GraphEdge<T>>();

            GraphVertex<T> currentVertex = target;

            // Continue path until complete or invalid.
            while (true)
            {
                // Check all edges to current vertex for discovery edge.

                if (currentVertex.HasEdgesTo)
                {
                    GraphEdge<T> discoveryEdge = null;
                    foreach (GraphEdge<T> edge in currentVertex.EdgesTo)
                    {
                        if (EdgeStates[edge.LatestIndex] == desiredEdgeState)
                        {
                            discoveryEdge = edge;
                            break;
                        }
                    }

                    if(discoveryEdge != null)
                    {
                        path.Add(discoveryEdge);
                        currentVertex = discoveryEdge.TraverseBackFrom(currentVertex);
                        continue;
                    }
                }
                break; // Invalid path as no edge from current vertex is a discovery edge.
            }

            path.Reverse();

            return path;
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

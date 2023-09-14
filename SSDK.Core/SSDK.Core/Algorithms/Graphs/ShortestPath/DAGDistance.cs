using SSDK.Core.Algorithms.Sorting;
using SSDK.Core.Structures.Graphs;
using SSDK.Core.Structures.Linear;
using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Graphs.ShortestPath
{
    /// <summary>
    /// A helper/extension class that contains the algorithm for DAG distances (shortest path) search on a weighted
    /// graph.
    /// </summary>
    public static class DAGDistance
    {
        #region DAG Constants
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in DAG distances.
        /// </summary>
        public const int VTX_UNVISITED = 0;
        /// <summary>
        /// A constant for the GT state of the vertex when visited in DAG distances.
        /// </summary>
        public const int VTX_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in DAG distances.
        /// </summary>
        public const int EDGE_UNEXPLORED = 0;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in DAG distances.
        /// </summary>
        public const int EDGE_DISCOVERY = 1;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in DAG distances,
        /// but the distance was higher than another edge that 'discovered' it.
        /// </summary>
        public const int EDGE_CROSS = 2;
        #endregion

        /// <summary>
        /// Performs DAG distances shortest path algorithm on the graph and given vertex (weighted).
        /// If graph either undirected or cyclic, then methods applied to the resulting traversal may
        /// result in stack overflow or insufficent memory.
        /// </summary>
        /// <param name="v">the vertex to start from</param>
        /// <param name="vTarget">
        /// if true, then the algorithm stops when the given vertex (vTarget) is reached.
        /// </param>
        /// <returns>the graph traversal of the search</returns>
        /// <remarks>
        /// It is assumed that the graph is connected, edges are undirected and edge weights are not negative.
        /// </remarks>
        public static GraphTraversal<T> ShortestPathSearchDAG<T>(this Graph<T> graph, GraphVertex<T> v, GraphVertex<T> vTarget = null)
        {
            // Sort topological and reset traversal
            GraphTraversal<T> traversal = graph.SortTopologically();
            traversal.Reset("DAG Distances");

            int[] discoveryEdges = new int[graph.Vertices.Count];

            // Set initial distances
            foreach(GraphVertex<T> vertex in graph.Vertices)
            {
                traversal.VertexWeights[vertex.LatestIndex] = (vertex == v ? 0 : UncontrolledNumber.Infinity);
            }


            
            foreach(int topologicalIndex in traversal.Configuration) 
            {
                GraphVertex<T> vertex = graph.Vertices[topologicalIndex];
                if (vertex == vTarget) break; // Vertex was reached.

                if(vertex.HasEdgesFrom)
                {
                    foreach(GraphEdge<T> edge in vertex.EdgesFrom)
                    {
                        // 'Relax edge'
                        GraphVertex<T> to = edge.Traverse(vertex);

                        // Calculate new distance
                        UncontrolledNumber newDistance = traversal.VertexWeights[vertex.LatestIndex]
                            + edge.GetDistance();

                        // Check if this becomes newest shortest path for vertex.
                        if (newDistance < traversal.VertexWeights[to.LatestIndex])
                        {
                            // Set distance for vertex
                            traversal.VertexWeights[to.LatestIndex] = newDistance;

                            // Discovery edge
                            int oldEdge = discoveryEdges[to.LatestIndex];
                            if(oldEdge != 0)
                                traversal.EdgeStates[oldEdge-1] = EDGE_CROSS;
                            traversal.EdgeStates[edge.LatestIndex] = EDGE_DISCOVERY;
                            discoveryEdges[to.LatestIndex] = edge.LatestIndex + 1;
                        }
                        else
                        {
                            // Cross-edge
                            traversal.EdgeStates[edge.LatestIndex] = EDGE_CROSS;
                        }
                    }
                }
            }

            return traversal;
        }
    }
}

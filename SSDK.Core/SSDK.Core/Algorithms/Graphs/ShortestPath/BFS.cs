using SSDK.Core.Structures.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Graphs.ShortestPath
{
    /// <summary>
    /// A helper/extension class that contains the algorithm for BFS.
    /// </summary>
    public static class BFS
    {
        #region BFS Constants
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in BFS.
        /// </summary>
        public const int VTX_BFS_UNVISITED = 0;
        /// <summary>
        /// A constant for the GT state of the vertex when visited in BFS.
        /// </summary>
        public const int VTX_BFS_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in BFS.
        /// </summary>
        public const int EDGE_BFS_UNEXPLORED = 0;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in BFS.
        /// </summary>
        public const int EDGE_BFS_DISCOVERY = 1;
        /// <summary>
        /// A constant for the GT state of the cross edge when it leads to a visited vertex in BFS.
        /// </summary>
        public const int EDGE_BFS_CROSS = 2;
        #endregion

        /// <summary>
        /// Performs breadth-first search (bfs) on the graph and given vertex (unweighted).
        /// </summary>
        /// <param name="v">the vertex to start from</param>
        /// <param name="vTarget">
        /// if true, then the algorithm stops when the given vertex (vTarget) is reached.
        /// </param>
        /// <returns>the graph traversal of the search</returns>
        public static GraphTraversal<T> BreadthFirstSearch<T>(this Graph<T> graph, GraphVertex<T> v, GraphVertex<T> vTarget = null)
        {
            GraphTraversal<T> traversal = new GraphTraversal<T>(graph, "BFS");

            // Create BFS Queue for algorithm
            Queue<GraphVertex<T>> bfsQueue = new Queue<GraphVertex<T>>();
            bfsQueue.Enqueue(v);

            while (bfsQueue.Count > 0)
            {
                GraphVertex<T> vertex = bfsQueue.Dequeue();

                // Mark vertex as visited
                traversal.VertexStates[vertex.LatestIndex] = VTX_BFS_VISITED;

                if (vertex == vTarget) return traversal; // Stop here if target is reached.

                if (vertex.HasEdgesFrom)
                {
                    foreach (GraphEdge<T> edge in vertex.EdgesFrom)
                    {
                        if (traversal.EdgeStates[edge.LatestIndex] == EDGE_BFS_UNEXPLORED)
                        {
                            GraphVertex<T> reachedVertex = edge.Traverse(vertex);
                            if (traversal.VertexStates[reachedVertex.LatestIndex] == VTX_BFS_UNVISITED)
                            {
                                // Mark edge as discovery-edge
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_BFS_DISCOVERY;

                                // Enqueue so that vertex is searched on next depth.
                                bfsQueue.Enqueue(reachedVertex);

                                // Mark vertex as visited
                                traversal.VertexStates[reachedVertex.LatestIndex] = VTX_BFS_VISITED;
                            }
                            else
                            {
                                // Mark edge as cross-edge
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_BFS_CROSS;
                            }
                        }
                    }
                }
            }

            return traversal;
        }
    }
}

using SSDK.Core.Structures.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Graphs.Exploration
{
    /// <summary>
    /// A helper/extension class that contains the algorithm for DFS search on an arbitrary graph and node.
    /// </summary>
    public static class DFS
    {
        #region DFS Constants
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in DFS.
        /// </summary>
        public const int VTX_UNVISITED = 0;

        /// <summary>
        /// A constant for the GT state of the vertex when visited in DFS.
        /// </summary>
        public const int VTX_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in DFS.
        /// </summary>
        public const int EDGE_UNEXPLORED = 0;

        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in DFS.
        /// </summary>
        public const int EDGE_DISCOVERY = 1;

        /// <summary>
        /// A constant for the GT state of the edge when it leads to a discovered vertex.
        /// </summary>
        public const int EDGE_BACK = 2;

        #endregion
        /// <summary>
        /// Performs depth-first search (dfs) on the graph and given vertex (unweighted).
        /// </summary>
        /// <param name="v">the vertex to start from</param>
        /// <returns>the graph traversal of the search</returns>
        public static GraphTraversal<T> DepthFirstSearch<T>(this Graph<T> graph, GraphVertex<T> v)
        {
            GraphTraversal<T> traversal = new GraphTraversal<T>(graph, "DFS");

            // Create DFS stack for algorithm.
            Stack<GraphVertex<T>> dfsStack = new();
            dfsStack.Push(v);

            while (dfsStack.Count > 0)
            {
                GraphVertex<T> vertex = dfsStack.Pop();

                // Mark vertex as visited
                traversal.VertexStates[vertex.LatestIndex] = VTX_VISITED;

                if (vertex.EdgesFrom != null)
                {
                    // Check all outgoing edges for unexplored vertices
                    foreach (GraphEdge<T> edge in vertex.EdgesFrom)
                    {
                        if (traversal.EdgeStates[edge.LatestIndex] == EDGE_UNEXPLORED)
                        {
                            GraphVertex<T> reachedVertex = edge.Traverse(vertex);
                            if (traversal.VertexStates[reachedVertex.LatestIndex] == EDGE_UNEXPLORED)
                            {
                                // Mark edge as discovery edge
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_DISCOVERY;
                                dfsStack.Push(reachedVertex); // Add to stack for next depth-first step (marked as visited)
                            }
                            else
                            {
                                // Mark edge as back edge.
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_BACK;
                            }
                        }
                    }
                }
            }

            return traversal;
        }
    }
}

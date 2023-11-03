using SSDK.Core.Helpers;
using SSDK.Core.Structures.Graphs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Sorting
{
    /// <summary>
    /// A helper/extension class that contains the methods for a topological sort.
    /// </summary>
    public static class TopologicalSort
    {
        #region SORT CONSTANTS
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in Topological Sort.
        /// </summary>
        public const int VTX_UNVISITED = 0;
        /// <summary>
        /// A constant for the GT state of the vertex when visited in Topological Sort.
        /// </summary>
        public const int VTX_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in Topological Sort.
        /// </summary>
        public const int EDGE_UNEXPLORED = 0;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in Topological Sort.
        /// </summary>
        public const int EDGE_DISCOVERY = 1;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' 
        /// but vertex to is already visited in Topological Sort.
        /// </summary>
        public const int EDGE_FORWARD = 2;
        #endregion
        /// <summary>
        /// Performs DAG topological sort on the graph. Assumes
        /// the graph is directed and acyclic.
        /// </summary>
        /// <param name="on">the graph to perform the sort on</param>
        /// <returns>a traversal with configuration (vertices) set to the topological ordering</returns>
        public static GraphTraversal<T> SortTopologically<T>(this Graph<T> on)
        {
            // Generate basic traversal
            GraphTraversal<T> traversal = new GraphTraversal<T>(on, null, null, false, "Topological Sort");
            traversal.Configuration = new int[on.Vertices.Count];

            int left = on.Vertices.Count;
            Stack<GraphVertex<T>> explorationStack = new Stack<GraphVertex<T>>();

            // Push initial vertices to stack
            foreach (GraphVertex<T> vertex in on.Vertices)
            {
                explorationStack.Push(vertex);
            }

            while (explorationStack.Count > 0)
            {
                GraphVertex<T> vertex = explorationStack.Pop();
                
                // Seek next DFS element
                if (traversal.VertexStates[vertex.LatestIndex] == VTX_UNVISITED)
                {
                    // Mark v as visited
                    traversal.VertexStates[vertex.LatestIndex] = VTX_VISITED;

                    // Visit all edges to discover or cross
                    if (vertex.HasEdgesFrom)
                    {
                        foreach (GraphEdge<T> edge in vertex.EdgesFrom)
                        {
                            GraphVertex<T> to = edge.Traverse(vertex);

                            if (traversal.VertexStates[to.LatestIndex] == VTX_UNVISITED)
                            {
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_DISCOVERY;
                                // Add to stack for next iteration
                                explorationStack.Push(to);
                            }
                            else
                            {
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_FORWARD;
                            }
                        }
                    }
                    // 'Label' element with number n
                    left--;
                    traversal.Configuration[left] = vertex.LatestIndex;
                }
            }

            return traversal;
        }
    }
}


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
    /// A helper/extension class that contains the algorithm for Dijkstra's shortest path search on a weighted
    /// graph.
    /// </summary>
    public static class Dijkstra
    {
        #region Dijkstra Constants
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in Dijkstra.
        /// </summary>
        public const int VTX_UNVISITED = 0;
        /// <summary>
        /// A constant for the GT state of the vertex when visited in Dijkstra.
        /// </summary>
        public const int VTX_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in Dijkstra.
        /// </summary>
        public const int EDGE_UNEXPLORED = 0;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in Dijkstra.
        /// </summary>
        public const int EDGE_DISCOVERY = 1;
        #endregion

        /// <summary>
        /// Performs dijkstra's shortest path search on the graph and given vertex (weighted).
        /// </summary>
        /// <param name="v">the vertex to start from</param>
        /// <param name="vTarget">
        /// if true, then the algorithm stops when the given vertex (vTarget) is reached.
        /// </param>
        /// <returns>the graph traversal of the search</returns>
        /// <remarks>
        /// It is assumed that the graph is connected, edges are undirected and edge weights are not negative.
        /// </remarks>
        public static GraphTraversal<T> ShortestPathSearchDijkstra<T>(this Graph<T> graph, GraphVertex<T> v, GraphVertex<T> vTarget = null)
        {
            GraphTraversal<T> traversal = new GraphTraversal<T>(graph, "Dijkstra");

            // Create Priority Queue for algorithm
            FPriorityQueue<(GraphVertex<T>,GraphEdge<T>), UncontrolledNumber> priorityQueue = new FPriorityQueue<(GraphVertex<T>, GraphEdge<T>), UncontrolledNumber>();

            FPriorityQueueItem<(GraphVertex<T>, GraphEdge<T>), UncontrolledNumber>[] vertexQueueItems
                = new FPriorityQueueItem<(GraphVertex<T>, GraphEdge<T>), UncontrolledNumber>[graph.Vertices.Count];
            // Insert all vertices into priority queue
            foreach (GraphVertex<T> vertex in graph.Vertices)
            {
                if (vertex == v)
                {
                    vertexQueueItems[vertex.LatestIndex] = priorityQueue.Enqueue((vertex,null),
                    traversal.VertexWeights[vertex.LatestIndex] = 0
                    );
                }
                else vertexQueueItems[vertex.LatestIndex] = priorityQueue.Enqueue((vertex, null),
                    traversal.VertexWeights[vertex.LatestIndex] = UncontrolledNumber.Infinity
                    );
            }

            while (priorityQueue.Count > 0)
            {
                (GraphVertex<T> vertex, GraphEdge<T> traversedEdge) = priorityQueue.Dequeue();

                if(traversedEdge != null)
                {
                    // Edge must be discovery edge.
                    traversal.EdgeStates[traversedEdge.LatestIndex] = EDGE_DISCOVERY;
                }

                if (vertex == vTarget) break; // Vertex was reached.

                if (vertex.HasEdgesFrom)
                    foreach (GraphEdge<T> edge in vertex.EdgesFrom)
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

                            // Replace priority in queue with new distance.
                            if(traversedEdge == edge) // Keep current edge
                            {
                                priorityQueue.SetPriority(vertexQueueItems[to.LatestIndex], newDistance);
                            }
                            else
                            {
                                // Update with new leading edge.
                                vertexQueueItems[to.LatestIndex].Element = (to, edge);
                            }
                        }
                    }
            }

            return traversal;
        }
    }
}

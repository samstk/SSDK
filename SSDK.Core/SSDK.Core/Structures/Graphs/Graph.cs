using KBS.Core.Arithmetic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    /// <summary>
    /// A graph contains a set of nodes (vertices) and a collection of edges (i.e. v1->v2 or v1<=>v2)
    /// It can be used to represents networks of information, such as maps.
    /// </summary>
    public class Graph<T>
    {
        #region Properties & Fields

        /// <summary>
        /// A list of all edges contained within the graph.
        /// </summary>
        private List<GraphEdge<T>> _Edges = new List<GraphEdge<T>>();

        /// <summary>
        /// A list of all edges contained within the graph (read-only).
        /// </summary>
        public ReadOnlyCollection<GraphEdge<T>> Edges
        {
            get
            {
                return _Edges.AsReadOnly();
            }
        }

        /// <summary>
        /// A list of all vertices in the graph.
        /// </summary>
        private List<GraphVertex<T>> _Vertices = new List<GraphVertex<T>>();

        /// <summary>
        /// A list of all edges contained within the graph (read-only).
        /// </summary>
        public ReadOnlyCollection<GraphVertex<T>> Vertices
        {
            get
            {
                return _Vertices.AsReadOnly();
            }
        }

        #endregion
        #region Methods
        #region Constructors
        public Graph()
        {
            
        }
        #endregion
        #region Cloning
        /// <summary>
        /// Creates a deep-copy of this graph.
        /// </summary>
        /// <returns>an exact clone of this graph (all vertices and edges)</returns>
        public Graph<T> Clone()
        {
            UpdateIndexReferences();

            Graph<T> newGraph = new Graph<T>();

            foreach(GraphVertex<T> vertex in this.Vertices)
            {
                GraphVertex<T> newVert = newGraph.Add(vertex.Value);
                newVert.LatestIndex = Vertices.Count;
            }

            foreach(GraphEdge<T> edge in this.Edges)
            {
                if(edge.Multiway)
                {
                    if(edge.AltWeight != null)
                        newGraph.Join(edge.VertexFrom, edge.VertexTo, edge.AltWeight);
                    else newGraph.Join(edge.VertexFrom, edge.VertexTo, edge.Weight);
                }
                else
                {
                    if (edge.AltWeight != null)
                        newGraph.CreatePath(edge.VertexFrom.LatestIndex, edge.VertexTo.LatestIndex, edge.AltWeight);
                    else newGraph.CreatePath(edge.VertexFrom.LatestIndex, edge.VertexTo.LatestIndex, edge.Weight);
                }
            }

            return newGraph;
        }
        #endregion
        #region Conversions
        /// <summary>
        /// Gets the transposed graph, which is an exact copy of this graph, except with
        /// all edges reversed.
        /// </summary>
        /// <returns></returns>
        public Graph<T> Transpose()
        {
            Graph<T> graph = this.Clone();
            UpdateIndexReferences();

            Graph<T> newGraph = new Graph<T>();

            foreach (GraphVertex<T> vertex in this.Vertices)
            {
                GraphVertex<T> newVert = newGraph.Add(vertex.Value);
                newVert.LatestIndex = Vertices.Count;
            }

            foreach (GraphEdge<T> edge in this.Edges)
            {
                if (edge.Multiway)
                {
                    if (edge.AltWeight != null)
                        newGraph.Join(edge.VertexTo, edge.VertexFrom, edge.AltWeight);
                    else newGraph.Join(edge.VertexTo, edge.VertexFrom, edge.Weight);
                }
                else
                {
                    if (edge.AltWeight != null)
                        newGraph.CreatePath(edge.VertexTo.LatestIndex, edge.VertexFrom.LatestIndex, edge.AltWeight);
                    else newGraph.CreatePath(edge.VertexTo.LatestIndex, edge.VertexFrom.LatestIndex, edge.Weight);
                }
            }

            return newGraph;
        }


        #endregion

        #region Modification
        /// <summary>
        /// Adds a single vertex with the given value.
        /// </summary>
        /// <param name="vertexValue">value of the vertex</param>
        /// <returns>the vertex that was added</returns>
        public GraphVertex<T> Add(T vertexValue)
        {
            GraphVertex<T> newVertex = new GraphVertex<T>(vertexValue, this);
            _Vertices.Add(newVertex);
            return newVertex;
        }

        /// <summary>
        /// Joins two vertices together in a multi-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex to join</param>
        /// <param name="vertex2">second vertex to join</param>
        /// <param name="weight">the 'distance' between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> Join(GraphVertex<T> vertex1, GraphVertex<T> vertex2, UncontrolledNumber weight)
        {
            GraphEdge<T> newEdge = new GraphEdge<T>(vertex1, vertex2, weight, true);
            _Edges.Add(newEdge);
            vertex1.AddEdge(newEdge);
            vertex2.AddEdge(newEdge);
            return newEdge;
        }

        /// <summary>
        /// Joins two vertices together in a multi-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex to join</param>
        /// <param name="vertex2">second vertex to join</param>
        /// <param name="altWeight">the 'distance' function between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> Join(GraphVertex<T> vertex1, GraphVertex<T> vertex2, Func<UncontrolledNumber> altWeight)
        {
            GraphEdge<T> newEdge = new GraphEdge<T>(vertex1, vertex2, altWeight, true);
            _Edges.Add(newEdge);
            vertex1.AddEdge(newEdge);
            vertex2.AddEdge(newEdge);
            return newEdge;
        }

        /// <summary>
        /// Joins two vertices together in a multi-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex index to join</param>
        /// <param name="vertex2">second vertex index to join</param>
        /// <param name="weight">the 'distance' between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> Join(int vertex1, int vertex2, UncontrolledNumber weight)
        {
            return Join(Vertices[vertex1], Vertices[vertex2], weight);
        }

        /// <summary>
        /// Joins two vertices together in a multi-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex index to join</param>
        /// <param name="vertex2">second vertex index to join</param>
        /// <param name="altWeight">the 'distance' function between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> Join(int vertex1, int vertex2, Func<UncontrolledNumber> altWeight)
        {
            return Join(Vertices[vertex1], Vertices[vertex2], altWeight);
        }

        /// <summary>
        /// Creates a path from one vertex to another in a single-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex from</param>
        /// <param name="vertex2">second vertex to</param>
        /// <param name="weight">the 'distance' between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> CreatePath(GraphVertex<T> vertexFrom, GraphVertex<T> vertexTo, UncontrolledNumber weight)
        {
            GraphEdge<T> newEdge = new GraphEdge<T>(vertexFrom, vertexTo, weight, false);
            _Edges.Add(newEdge);
            vertexFrom.AddEdge(newEdge);
            vertexTo.AddEdge(newEdge);
            return newEdge;
        }

        /// <summary>
        /// Creates a path from one vertex to another in a single-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex from</param>
        /// <param name="vertex2">second vertex to</param>
        /// <param name="altWeight">the 'distance' function between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> CreatePath(GraphVertex<T> vertexFrom, GraphVertex<T> vertexTo, Func<UncontrolledNumber> altWeight)
        {
            GraphEdge<T> newEdge = new GraphEdge<T>(vertexFrom, vertexTo, altWeight, false);
            _Edges.Add(newEdge);
            vertexFrom.AddEdge(newEdge);
            vertexTo.AddEdge(newEdge);
            return newEdge;
        }

        /// <summary>
        /// Creates a path from one vertex to another in a single-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex index from</param>
        /// <param name="vertex2">second vertex index to</param>
        /// <param name="weight">the 'distance' between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> CreatePath(int vertexFrom, int vertexTo, UncontrolledNumber weight)
        {
            return CreatePath(Vertices[vertexFrom], Vertices[vertexTo], weight);
        }


        /// <summary>
        /// Creates a path from one vertex to another in a single-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex index from</param>
        /// <param name="vertex2">second vertex index to</param>
        /// <param name="altWeight">the 'distance' function between the two vertices</param>
        /// <returns>the graph edge joining the two vertices</returns>
        public GraphEdge<T> CreatePath(int vertexFrom, int vertexTo, Func<UncontrolledNumber> altWeight)
        {
            return CreatePath(Vertices[vertexFrom], Vertices[vertexTo], altWeight);
        }

        /// <summary>
        /// Removes the given edge from the graph.
        /// </summary>
        /// <param name="edge">the edge to remove</param>
        public void RemoveEdge(GraphEdge<T> edge)
        {
            edge.VertexFrom.RemoveEdgeReferences(edge);
            edge.VertexTo.RemoveEdgeReferences(edge);
            _Edges.Remove(edge);
        }

        #endregion

        #region Search
        /// <summary>
        /// Gets the first edge matching from=v1 and to=v2, or if multiway is allowed, then an edge
        /// that matches accordingly.
        /// </summary>
        /// <param name="v1">the vertex from</param>
        /// <param name="v2">the vertex to</param>
        /// <param name="allowMultiway">if true, then v1 can be to and v2 can be from</param>
        /// <returns></returns>
        public GraphEdge<T> GetEdge(GraphVertex<T> v1, GraphVertex<T> v2, bool allowMultiway=true)
        {
            if (allowMultiway)
            {
                // Get an edge that matches (allowing for multi-way paths)
                foreach (GraphEdge<T> edge in Edges)
                {
                    if(edge.VertexFrom == v1 && edge.VertexTo == v2
                        || edge.VertexFrom == v2 && edge.VertexTo == v1 && edge.Multiway)
                    {
                        return edge;
                    }
                }
            }
            else
            {
                // Get an edge that matches.
                foreach (GraphEdge<T> edge in Edges)
                {
                    if(edge.VertexFrom == v1 && edge.VertexTo == v2)
                    {
                        return edge;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Graph Traversal Algorithms
        /// <summary>
        /// Updates all edge and vertices indices to ensure they have an exact reference.
        /// </summary>
        public void UpdateIndexReferences()
        {
            // Update edge indices
            for(int i = 0; i<_Edges.Count; i++)
            { 
                _Edges[i].LatestIndex = i;
            }

            // Update vertex indices
            for(int i = 0; i<_Vertices.Count; i++)
            {
                _Vertices[i].LatestIndex = i;
            }
        }

        #region DFS Constants
        /// <summary>
        /// A constant for the GT state of the vertex when unvisited in DFS.
        /// </summary>
        public const int VTX_DFS_UNVISITED = 0;
        /// <summary>
        /// A constant for the GT state of the vertex when visited in DFS.
        /// </summary>
        public const int VTX_DFS_VISITED = 1;

        /// <summary>
        /// A constant for the GT state of the edge when unexplored in DFS.
        /// </summary>
        public const int EDGE_DFS_UNEXPLORED = 0;
        /// <summary>
        /// A constant for the GT state of the edge when 'discovered' in DFS.
        /// </summary>
        public const int EDGE_DFS_DISCOVERY = 1;
        /// <summary>
        /// A constant for the GT state of the edge when it leads to a discovered vertex.
        /// </summary>
        public const int EDGE_DFS_BACK = 2;
        #endregion
        /// <summary>
        /// Performs depth-first search (dfs) on the graph and given vertex (unweighted).
        /// </summary>
        /// <param name="v">the vertex to start from</param>
        /// <returns>the graph traversal of the search</returns>
        public GraphTraversal<T> DepthFirstSearch(GraphVertex<T> v)
        {
            GraphTraversal<T> traversal = new GraphTraversal<T>(this, "DFS");

            // Create DFS stack for algorithm.
            Stack <GraphVertex<T>> dfsStack = new ();
            dfsStack.Push(v);

            while (dfsStack.Count > 0)
            {
                GraphVertex<T> vertex = dfsStack.Pop();

                // Mark vertex as visited
                traversal.VertexStates[vertex.LatestIndex] = VTX_DFS_VISITED;

                if (vertex.EdgesFrom != null)
                {
                    // Check all outgoing edges for unexplored vertices
                    foreach(GraphEdge<T> edge in vertex.EdgesFrom)
                    {
                        if (traversal.EdgeStates[edge.LatestIndex] == EDGE_DFS_UNEXPLORED)
                        {
                            GraphVertex<T> reachedVertex = edge.Traverse(vertex);
                            if (traversal.VertexStates[reachedVertex.LatestIndex] == EDGE_DFS_UNEXPLORED)
                            {
                                // Mark edge as discovery edge
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_DFS_DISCOVERY;
                                dfsStack.Push(reachedVertex); // Add to stack for next depth-first step (marked as visited)
                            }
                            else
                            {
                                // Mark edge as back edge.
                                traversal.EdgeStates[edge.LatestIndex] = EDGE_DFS_BACK;
                            }
                        }
                    }
                }
            }

            return traversal;
        }

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
        public GraphTraversal<T> BreadthFirstSearch(GraphVertex<T> v, GraphVertex<T> vTarget=null)
        {
            GraphTraversal<T> traversal = new GraphTraversal<T>(this, "BFS");

            // Create BFS Queue for algorithm
            Queue<GraphVertex<T>> bfsQueue = new Queue<GraphVertex<T>>();
            bfsQueue.Enqueue(v);

            while (bfsQueue.Count > 0)
            {
                GraphVertex<T> vertex = bfsQueue.Dequeue();

                // Mark vertex as visited
                traversal.VertexStates[vertex.LatestIndex] = VTX_BFS_VISITED;

                if (vertex == vTarget) return traversal; // Stop here if target is reached.
                
                if(vertex.HasEdgesFrom)
                {
                    foreach(GraphEdge<T> edge in vertex.EdgesFrom)
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
        #endregion

        #endregion

        public override string ToString()
        {
            return $"Graph(V: {Vertices.Count}, E: {Edges.Count})";
        }
    }
}

﻿using KBS.Core.Arithmetic;
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
        /// Creates a path from one vertex to another in a single-way edge.
        /// </summary>
        /// <param name="vertex1">first vertex to join</param>
        /// <param name="vertex2">second vertex to join</param>
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
        /// Removes the given vertex from the graph, and all of its edges.
        /// </summary>
        /// <param name="vertex">the vertex to remove</param>
        public void RemoveVertex(GraphVertex<T> vertex)
        {
            vertex.DestroyEdges();
            _Vertices.Remove(vertex);
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
        /// Performs depth-first search (dfs) on the graph and given vertex.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
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
                                dfsStack.Push(reachedVertex); // Add to stack for next depth-first step.
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
        #endregion

        #endregion

        public override string ToString()
        {
            return $"Graph(V: {Vertices.Count}, E: {Edges.Count})";
        }
    }
}

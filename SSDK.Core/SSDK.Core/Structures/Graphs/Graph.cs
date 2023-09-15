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
            GraphVertex<T> newVertex = new GraphVertex<T>(vertexValue, this) { LatestIndex = _Vertices.Count };
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
            GraphEdge<T> newEdge = new GraphEdge<T>(vertex1, vertex2, weight, true) { LatestIndex = _Edges.Count };
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
            GraphEdge<T> newEdge = new GraphEdge<T>(vertex1, vertex2, altWeight, true) { LatestIndex = _Edges.Count };
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
            GraphEdge<T> newEdge = new GraphEdge<T>(vertexFrom, vertexTo, weight, false) { LatestIndex = _Edges.Count };
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
            GraphEdge<T> newEdge = new GraphEdge<T>(vertexFrom, vertexTo, altWeight, false) { LatestIndex = _Edges.Count };
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
        /// Using this method will mess up index references, so use 
        /// UpdateIndexReferences afterwards to avert this issue
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

        #region Graph Traversal Algorithms & Optimisation
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
        #endregion
        #endregion

        public override string ToString()
        {
            return $"Graph(V: {Vertices.Count}, E: {Edges.Count})";
        }
    }
}

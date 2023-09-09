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
        #endregion

        public override string ToString()
        {
            return $"Graph(V: {Vertices.Count}, E: {Edges.Count})";
        }
    }
}

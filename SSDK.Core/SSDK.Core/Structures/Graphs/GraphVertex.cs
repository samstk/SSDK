using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    public class GraphVertex<T>
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the root associatied with this vertex.
        /// </summary>
        public Graph<T> Root { get; private set; }

        /// <summary>
        /// Gets or sets the value associated with this vertex
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The list of all edges from this vertex to another.
        /// </summary>
        private List<GraphEdge<T>> _EdgesFrom;

        /// <summary>
        /// Gets the list of all edges from this vertex to another (read-only)
        /// </summary>
        public ReadOnlyCollection<GraphEdge<T>> EdgesFrom { 
            get
            {
                if (_EdgesFrom == null) return new ReadOnlyCollection<GraphEdge<T>>(new List<GraphEdge<T>>());
                return _EdgesFrom.AsReadOnly();
            } 
        }

        /// <summary>
        /// The list of all edges from another vertex to this vertex.
        /// </summary>
        private List<GraphEdge<T>> _EdgesTo;

        /// <summary>
        /// Gets the list of all edges from another vertex to this vertex (read-only)
        /// </summary>
        public ReadOnlyCollection<GraphEdge<T>> EdgesTo
        {
            get
            {
                if (_EdgesTo == null) return new ReadOnlyCollection<GraphEdge<T>>(new List<GraphEdge<T>>());
                return _EdgesTo.AsReadOnly();
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public GraphVertex(T value, Graph<T> root)
        {
            Value = value;
            Root = root;
            if (Root == null)
                throw new ArgumentNullException("Root must be a non-null graph");
        }
        #endregion

        #region Edges
        /// <summary>
        /// Adds the edge to the list of edges from and edges to if it is relevant.
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(GraphEdge<T> edge)
        {
            if(edge.Multiway)
            {
                // Since multi-way just add if any vertices in the edge match this.
                if (edge.VertexFrom == this || edge.VertexTo == this)
                {
                    if (_EdgesFrom == null) _EdgesFrom = new List<GraphEdge<T>>();
                    if (_EdgesTo == null) _EdgesTo = new List<GraphEdge<T>>();
                    _EdgesFrom.Add(edge);
                    _EdgesTo.Add(edge);
                }
                else throw new InvalidOperationException("An edge was attempted to be added to a vertex, which was not included in the edge.");
            }
            else
            {
                if(edge.VertexFrom == this)
                {
                    if (_EdgesFrom == null) _EdgesFrom = new List<GraphEdge<T>>();
                    _EdgesFrom.Add(edge);
                }
                else if (edge.VertexTo == this)
                {
                    if (_EdgesTo == null) _EdgesTo = new List<GraphEdge<T>>();
                    _EdgesTo.Add(edge);
                }
                else throw new InvalidOperationException("An edge was attempted to be added to a vertex, which was not included in the edge.");
            }
        }

        /// <summary>
        /// Destroys all edges in this vertex, removing them from the Root graph.
        /// </summary>
        public void DestroyEdges()
        {
            HashSet<GraphEdge<T>> edges = new HashSet<GraphEdge<T>>();

            // Accumulate all edges
            if (_EdgesFrom != null) foreach (GraphEdge<T> edge in _EdgesFrom) edges.Add(edge);
            if (_EdgesTo != null) foreach (GraphEdge<T> edge in _EdgesTo) edges.Add(edge);

            _EdgesFrom = null;
            _EdgesTo = null;

            // Remove all unique edges
            foreach(GraphEdge<T> edge in edges)
            {
                Root.RemoveEdge(edge);
            }
        }
        /// <summary>
        /// Removes an edge reference in this vertex.
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdgeReferences(GraphEdge<T> edge)
        {
            if (edge.Multiway)
            {
                // Since multi-way just remove if any vertices in the edge match this.
                if (edge.VertexFrom == this || edge.VertexTo == this)
                {
                    if (_EdgesFrom != null) _EdgesFrom.Remove(edge);
                    if (_EdgesTo != null) _EdgesTo.Remove(edge);
                }
                else throw new InvalidOperationException("An edge was attempted to be removed from a vertex which was not included in the edge.");
            }
            else
            {
                if (edge.VertexFrom == this)
                {
                    if (_EdgesFrom != null) _EdgesFrom.Remove(edge);
                }
                else if (edge.VertexTo == this)
                {
                    if (_EdgesTo != null) _EdgesTo.Remove(edge);
                }
                else throw new InvalidOperationException("An edge was attempted to be removed from a vertex which was not included in the edge.");
            }
        }
        #endregion
        #endregion

        public override string ToString()
        {
            return $"Graph-Vertex({Value})";
        }
    }
}

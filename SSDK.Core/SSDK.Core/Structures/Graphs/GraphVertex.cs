using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    public struct GraphVertex<T>
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
        private List<GraphEdge<T>> _Edges;

        /// <summary>
        /// Gets the list of all edges from this vertex to another (read-only)
        /// </summary>
        public ReadOnlyCollection<GraphEdge<T>> Edges { 
            get
            {
                if (_Edges == null) return new ReadOnlyCollection<GraphEdge<T>>(new List<GraphEdge<T>>());
                return _Edges.AsReadOnly();
            } 
        }
        #endregion

        #region Methods
        #region Constructors
        public GraphVertex(T value, Graph<T> root)
        {
            Value = value;
            Root = root;
            _Edges = null; // Keep unassigned at this point in time.
        }
        #endregion
        #endregion
    }
}

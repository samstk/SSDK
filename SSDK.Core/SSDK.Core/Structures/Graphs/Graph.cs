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
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    /// <summary>
    /// Represents a traversable edge from one vertex to another, with
    /// Multiway indicating that the edge can be traversed from both directions.
    /// </summary>
    public struct GraphEdge<T>
    {
        /// <summary>
        /// Gets the edge that this graph edge travels from.
        /// </summary>
        public GraphVertex<T> VertexFrom { get; private set; }

        /// <summary>
        /// Gets the edge that this graph edge travels to.
        /// </summary>
        public GraphVertex<T> VertexTo { get; private set; }

        /// <summary>
        /// Gets or sets whether the graph edge can be traversed both ways
        /// (i.e. from -> to and to -> from instead of just from -> to)
        /// </summary>
        public bool Multiway { get; set; } = false;

        public GraphEdge(GraphVertex<T> from, GraphVertex<T> to, bool multiway=false)
        {
            VertexFrom = from;
            VertexTo = to;
            Multiway = multiway;
        }
    }
}

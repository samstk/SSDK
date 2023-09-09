using KBS.Core.Arithmetic;
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
        #region Properties & Fields
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

        /// <summary>
        /// Gets or sets the weight of the edge, which generally represents
        /// the standard distance between these vertices.
        /// </summary>
        public UncontrolledNumber Weight { get; set; }

        public Func<UncontrolledNumber> AltWeight { get; set; }
        #endregion

        #region Methods
        #region Constructors
        public GraphEdge(GraphVertex<T> from, GraphVertex<T> to, UncontrolledNumber weight, Func<UncontrolledNumber> altWeight=null, bool multiway=false)
        {
            VertexFrom = from;
            VertexTo = to;
            Multiway = multiway;
            Weight = weight;
            AltWeight = altWeight;
        }
        public GraphEdge(GraphVertex<T> from, GraphVertex<T> to, Func<UncontrolledNumber> altWeight = null, bool multiway = false)
        {
            VertexFrom = from;
            VertexTo = to;
            Multiway = multiway;
            Weight = 0;
            AltWeight = altWeight;
        }
        #endregion
        #endregion
    }
}

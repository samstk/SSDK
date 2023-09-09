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
    public class GraphEdge<T>
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the edge number, which is a unique number for the edge within
        /// a single graph.
        /// </summary>
        public long EdgeNo { get; private set; }
        /// <summary>
        /// Gets the edge that this graph edge travels from.
        /// </summary>
        public GraphVertex<T> VertexFrom { get; private set; }

        /// <summary>
        /// Gets the edge that this graph edge travels to.
        /// </summary>
        public GraphVertex<T> VertexTo { get; private set; }

        /// <summary>
        /// Gets whether the graph edge can be traversed both ways
        /// (i.e. from -> to and to -> from instead of just from -> to)
        /// </summary>
        public bool Multiway { get; private set; } = false;

        /// <summary>
        /// Gets or sets the weight of the edge, which generally represents
        /// the standard distance between these vertices.
        /// </summary>
        public UncontrolledNumber Weight { get; set; }

        /// <summary>
        /// Gets or sets the alternate weight function, which is calculated when
        /// the edge is traversed.
        /// </summary>
        public Func<UncontrolledNumber> AltWeight { get; set; }
        #endregion

        #region Methods
        #region Constructors
        public GraphEdge(GraphVertex<T> from, GraphVertex<T> to, UncontrolledNumber weight, bool multiway=false)
        {
            VertexFrom = from;
            VertexTo = to;
            Multiway = multiway;
            Weight = weight;
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

        /// <summary>
        /// Traverses the edge starting from a given node, assuming
        /// that the parameter is either vertex from or vertex to.
        /// </summary>
        public GraphVertex<T> Traverse(GraphVertex<T> from)
        {
            if (from == VertexFrom)
            {
                return VertexTo;
            }
            else if (Multiway)
            {
                return VertexFrom;
            }
            else throw new InvalidOperationException("Attempted to traverse graph edge backwards on a single-way edge.");
        }

        /// <summary>
        /// Returns the distance of the edge between the two vertices.
        /// (i.e. the weight of the edge)
        /// </summary>
        /// <returns>the calculated weight (alt weight or standard weight)</returns>
        public UncontrolledNumber GetDistance()
        {
            return AltWeight != null ? AltWeight() : Weight;
        }

        public override string ToString()
        {
            if(Multiway)
                return $"Graph-Edge({VertexFrom.Value}<->{VertexTo.Value})";
            return $"Graph-Edge({VertexFrom.Value}->{VertexTo.Value})";
        }
        #endregion
    }
}

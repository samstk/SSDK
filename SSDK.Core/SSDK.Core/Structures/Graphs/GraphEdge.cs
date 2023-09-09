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
        /// Gets or sets the latest index that this edge was found on in the root graph.
        /// Updated generally during graph traversal constructor.
        /// </summary>
        public int LatestIndex { get; set; } = -1;
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
        /// Traverses the edge back ending from a given node
        /// (i.e. in a single-way edge, the vertex from must be the end of the edge), 
        /// assuming that the parameter is either vertex from or vertex to.
        /// </summary>
        public GraphVertex<T> TraverseBackFrom(GraphVertex<T> from)
        {
            if (from == VertexTo)
            {
                return VertexFrom;
            }
            else if (Multiway)
            {
                return VertexTo;
            }
            else throw new InvalidOperationException("Attempted to traverse graph edge backwards on a single-way edge.");
        }


        /// <summary>
        /// Returns true if the edge can be traversed from the given vertex.
        /// </summary>
        /// <param name="from">the vertex to traverse from</param>
        /// <returns>true if a traversal can happen from the given vertex to the other</returns>
        public bool CanTraverse(GraphVertex<T> from)
        {
            return from == VertexFrom || Multiway;
        }

        /// <summary>
        /// Returns true if the edge can be traversed back from from the given vertex.
        /// </summary>
        /// <param name="from">the vertex to traverse from</param>
        /// <returns>true if a traversal can happen from the given vertex to the other</returns>
        public bool CanTraverseBackFrom(GraphVertex<T> from)
        {
            return from == VertexTo || Multiway;
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

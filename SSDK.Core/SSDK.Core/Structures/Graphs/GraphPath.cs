using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    /// <summary>
    /// Represents a path formed from multiple edges
    /// </summary>
    public sealed class GraphPath<T>
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the edge list storing the path. Edges are in order, and
        /// store the path in which was taken (e.g. A->B->C)
        /// </summary>
        /// <remarks>
        /// Do not modify this list, instead use GraphPath methods.
        /// </remarks>
        public List<GraphEdge<T>> Edges { get; private set; } = new List<GraphEdge<T>>();

        /// <summary>
        /// Gets the initial vertex of this path
        /// </summary>
        public GraphVertex<T> InitialVertex { get; private set; }

        private int _EdgeCountSinceVertexGeneration = 0;
        private int _EdgeCountSinceWeightGeneration = 0;

        /// <summary>
        /// Gets whether the path was weighted.
        /// </summary>
        public bool IsWeighted { get; private set; }

        private GraphVertex<T>[] _LastComputedVertices = new GraphVertex<T>[0];
        private UncontrolledNumber _LastComputedDistance = 0;
        /// <summary>
        /// Gets or sets the distance, either stored by the traversal algorithm or
        /// calculated from the single path back from the initial vertex.
        /// </summary>
        public UncontrolledNumber Distance
        {
            get
            {
                if (!IsWeighted)
                    return Edges.Count;
                else
                {
                    if (CacheVertices && _EdgeCountSinceWeightGeneration == Edges.Count)
                        return _LastComputedDistance;

                    UncontrolledNumber totalWeight = 0;
                    foreach (GraphEdge<T> edge in Edges)
                    {
                        totalWeight += edge.Weight;
                    }
                    _LastComputedDistance = totalWeight;
                    _EdgeCountSinceWeightGeneration = Edges.Count;
                    return totalWeight;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the vertex list is cached.
        /// Set to false if necessary to avoid conflicts with multi-thread operations.
        /// </summary>
        public bool CacheVertices { get; set; } = true;

        /// <summary>
        /// Computes the vertices connected in the path in order.
        /// </summary>
        public GraphVertex<T>[] Vertices
        {
            get
            {
                if (CacheVertices && _EdgeCountSinceVertexGeneration == Edges.Count)
                    return _LastComputedVertices;

                if (Edges.Count == 0) return new GraphVertex<T>[0];

                GraphVertex<T>[] vertices = new GraphVertex<T>[Edges.Count + 1];
                GraphVertex<T> currentVertex = InitialVertex;
                int i = 0;
                vertices[i++] = currentVertex;
                while (i < vertices.Length)
                {
                    GraphVertex<T> nextVertex = Edges[i - 1].Traverse(currentVertex);
                    currentVertex = vertices[i++] = nextVertex;
                }
                _LastComputedVertices = vertices;
                _EdgeCountSinceVertexGeneration = Edges.Count;
                return vertices;
            }
        }
        #endregion
        #region Constructor
        public GraphPath(bool isWeighted)
        {
            IsWeighted = isWeighted;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Adds a single edge to the path
        /// </summary>
        /// <param name="edge">the edge to add</param>
        /// <remarks>
        /// Upon addition of the first edge, the InitialVertex is assumed
        /// to be the VertexFrom field of the edge. Use (direction <= 0)
        /// to use VertexTo instead.
        /// </remarks>
        public void Add(GraphEdge<T> edge, int direction = 1)
        {
            if (Edges.Count == 0)
            {
                InitialVertex = direction > 0 ? edge.VertexFrom : edge.VertexTo;
            }
            Edges.Add(edge);
        }

        /// <summary>
        /// Adds all edges from the given path
        /// </summary>
        /// <param name="path">the path containing edges to add to this path</param>
        public void AddFrom(GraphPath<T> path)
        {
            AddRange(path.Edges);
        }

        /// <summary>
        /// Adds all edges from the given list of edges
        /// </summary>
        /// <param name="edges">the list of edges to add</param>
        public void AddRange(List<GraphEdge<T>> edges)
        {
            foreach (GraphEdge<T> edge in edges) Add(edge);
        }

        /// <summary>
        /// Removes the last edge from the path.
        /// </summary>
        /// <remarks>
        /// Multi-thread operations may interfere with Vertex list generation.
        /// Use CacheVertices=false<br/>
        /// </remarks>
        public void Remove()
        {
            if (Edges.Count > 0)
            { 
                Edges.RemoveAt(Edges.Count - 1);
                _EdgeCountSinceVertexGeneration = -1;
                _EdgeCountSinceWeightGeneration = -1;
            }
            if (Edges.Count == 0) InitialVertex = null;
        }

        /// <summary>
        /// Removes the given edge from the path
        /// </summary>
        /// <param name="edge">the edge to remove</param>
        public void Remove(GraphEdge<T> edge)
        {
            Edges.Remove(edge);
            _EdgeCountSinceVertexGeneration = -1;
            _EdgeCountSinceWeightGeneration = -1;
            if (Edges.Count == 0) InitialVertex = null;
        }

        /// <summary>
        /// Removes the given edge from the path at given index.
        /// </summary>
        /// <param name="index">the index to remove from the edge list</param>
        public void RemoveAt(int index)
        {
            Edges.RemoveAt(index);
            _EdgeCountSinceVertexGeneration = -1;
            _EdgeCountSinceWeightGeneration = -1;
            if (Edges.Count == 0) InitialVertex = null;
        }

        /// <summary>
        /// Reverses the path and invalidates the cache for vertices.
        /// </summary>
        public void Reverse()
        {
            Edges.Reverse();
            _EdgeCountSinceVertexGeneration = -1;
            _EdgeCountSinceWeightGeneration = -1;
        }

        /// <summary>
        /// Creates a new path by merging the other path onto this path
        /// (path -> other)
        /// </summary>
        /// <param name="other">the other path to merge exactly onto this path</param>
        /// <returns>a new path which is the merged result</returns>
        /// <remarks>
        /// The Merge method does not check whether this path and the other path
        /// is connected, and should be checked beforehand to ensure correctness
        /// of paths (i.e. the last edge's vertex does not connect to the start
        /// of the other path)<br/>
        /// Additionally, the resulting path assumes that the two paths
        /// are both weighted or both unweighted
        /// </remarks>
        public GraphPath<T> Merge(GraphPath<T> other)
        {
            GraphPath<T> newPath = new GraphPath<T>(IsWeighted);
            newPath.AddFrom(this);
            newPath.AddFrom(other);
            return newPath;
        }

        public override string ToString()
        {
            StringBuilder path = new StringBuilder();
            bool added = false;
            foreach(GraphVertex<T> vertex in Vertices)
            {
                if (added)
                    path.Append("->");
                path.Append(vertex.ToString());
                added = true;
            }
            return path.ToString();
        }
        #endregion
    }
}

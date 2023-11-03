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
    /// Represents a traversal of a given graph.
    /// </summary>
    public sealed class GraphTraversal<T>
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of traversal that occured on the graph.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Gets the graph that the traversal happened on.
        /// </summary>
        /// <remarks>
        /// Note that this field is a direct reference to the graph, so any
        /// changes to the graph may conflict with this traversal.
        /// </remarks>
        public Graph<T> On { get; private set; }

        /// <summary>
        /// Gets the initial vertex where the traversal started.
        /// </summary>
        public GraphVertex<T> InitialVertex { get; private set; }

        /// <summary>
        /// Gets the vertex which was the target of the traversal.
        /// </summary>
        public GraphVertex<T> EndingVertex { get; private set; }

        /// <summary>
        /// Gets whether this traversal depended on edge weights
        /// </summary>
        public bool IsWeighted { get; private set; }

        /// <summary>
        /// Gets the vertex states according to vertex index
        /// </summary>
        public int[] VertexStates { get; private set; }

        /// <summary>
        /// Gets the edge states according to edge index
        /// </summary>
        public int[] EdgeStates { get; private set; }

        private bool _DistanceSet = false;
        private UncontrolledNumber _Distance;

        /// <summary>
        /// Gets or sets the distance, either stored by the traversal algorithm or
        /// calculated from the single path back from the initial vertex.
        /// </summary>
        public UncontrolledNumber Distance
        {
            get
            {
                if (_DistanceSet)
                    return _Distance;

                if (EndingVertex == null || InitialVertex == null)
                    return 0;

                GraphPath<T> path = GetPathBack();
                return path.Distance;
            }
            set
            {
                _DistanceSet = true;
                _Distance = value;
            }
        }

        /// <summary>
        /// Gets the weight states according to vertex
        /// </summary>
        public UncontrolledNumber[] VertexWeights { get; private set; }

        /// <summary>
        /// Gets the weight states according to edge
        /// </summary>
        public UncontrolledNumber[] EdgeWeights { get; private set; }

        /// <summary>
        /// Typically unmodified except on certain algorithms such as
        /// topological sort, where the configuration is the topological order
        /// (indices)
        /// </summary>
        public int[] Configuration { get; set; }

        #endregion

        #region Methods
        #region Constructor
        public GraphTraversal(Graph<T> on, GraphVertex<T> initial, GraphVertex<T> final, bool isWeighted, string type=null)
        {
            Type = type;
            On = on;
            VertexStates = new int[on.Vertices.Count];
            EdgeStates = new int[on.Edges.Count];
            VertexWeights = new UncontrolledNumber[on.Vertices.Count];
            EdgeWeights = new UncontrolledNumber[on.Edges.Count];
            InitialVertex = initial;
            EndingVertex = final;
            IsWeighted = isWeighted;
        }

        #endregion
        #region Modification
        /// <summary>
        /// Resets all vertex/edge states in the traversal.
        /// Does not change configuration.
        /// </summary>
        /// <param name="newType">if specified, overwrites the current type reference</param>
        public void Reset(string newType = null)
        {
            if (newType != null)
            {
                Type = newType;
            }

            VertexStates = new int[VertexStates.Length];
            EdgeStates = new int[EdgeStates.Length];
            VertexWeights = new UncontrolledNumber[VertexWeights.Length];
            EdgeWeights = new UncontrolledNumber[EdgeWeights.Length];
        }
        #endregion
        #region Searching
        /// <summary>
        /// Gets the path between the initially traversed vertex and the target vertex.
        /// </summary>
        /// <param name="desiredEdgeState">
        /// the edge state that allows a path to be made
        /// (defaults to 1 as this is generally used for discovery edges)
        /// </param>
        /// <returns>a list of edges that is a path from the initial vertex to the target vertex</returns>
        public GraphPath<T> GetPathBack(int desiredEdgeState=1)
        {
            GraphPath<T> path = new GraphPath<T>(IsWeighted);
            HashSet<GraphEdge<T>> explored = new HashSet<GraphEdge<T>>();

            GraphVertex<T> currentVertex = EndingVertex;

            if (currentVertex == null)
                return path;

            // Continue path until complete or invalid.
            while (true)
            {
                // Check all edges to current vertex for discovery edge.

                if (currentVertex == InitialVertex)
                    break;

                if (currentVertex.HasEdgesTo)
                {
                    GraphEdge<T> discoveryEdge = null;
                    foreach (GraphEdge<T> edge in currentVertex.EdgesTo)
                    {
                        if (EdgeStates[edge.LatestIndex] == desiredEdgeState)
                        {
                            discoveryEdge = edge;
                            break;
                        }
                    }

                    if(discoveryEdge != null && !explored.Contains(discoveryEdge))
                    {
                        explored.Add(discoveryEdge);
                        path.Add(discoveryEdge);
                        currentVertex = discoveryEdge.TraverseBackFrom(currentVertex);
                        continue;
                    }
                }
                break; // Invalid path as no edge from current vertex is a discovery edge.
            }

            path.Reverse();

            return path;
        }

        /// <summary>
        /// Gets all paths between the initially traversed vertex and the target vertex, of
        /// the minimum distance represented in the Distance property.
        /// </summary>
        /// <param name="target">the target vertex that was reached in the traversal</param>
        /// <param name="desiredEdgeState">
        /// the edge state that allows a path to be made
        /// (defaults to 1 as this is generally used for discovery edges)
        /// </param>
        /// <returns>a list of edges that is a path from the initial vertex to the target vertex</returns>
        public List<GraphPath<T>> GetAllPathsBack(int desiredEdgeState = 1)
        {
            List<GraphPath<T>> paths = new List<GraphPath<T>>();
            HashSet<GraphEdge<T>> explored = new HashSet<GraphEdge<T>>();

            GraphVertex<T> currentVertex = EndingVertex;

            if (currentVertex == null)
                return paths;

            paths = DFSPathSearch(currentVertex, explored, desiredEdgeState);
            if (paths == null)
                return new List<GraphPath<T>>();
            return paths;
        }

        private List<GraphPath<T>> DFSPathSearch(GraphVertex<T> currentVertex, HashSet<GraphEdge<T>> explored, int desiredEdgeState = 1)
        {
            List<GraphPath<T>> paths = null;
            foreach(GraphEdge<T> edge in currentVertex.EdgesTo)
            {
                if (EdgeStates[edge.LatestIndex] == desiredEdgeState && !explored.Contains(edge))
                {
                    explored.Add(edge);
                    GraphVertex<T> nextVertex = edge.Traverse(currentVertex);
                    if (nextVertex == InitialVertex)
                    {
                        // Initialise paths if needed
                        if (paths == null) 
                            paths = new List<GraphPath<T>>();

                        GraphPath<T> path = new GraphPath<T>(IsWeighted);
                        path.Add(edge);
                        paths.Add(path);
                    }
                    else {
                        List<GraphPath<T>> subpaths = DFSPathSearch(nextVertex, explored, desiredEdgeState);

                        // Check to see if there are any subpaths, and if there are then create new paths.
                        if (subpaths == null)
                            continue;

                        // Initialise paths if needed
                        if (paths == null)
                            paths = new List<GraphPath<T>>();

                        // If there are any subpaths then join this edge onto them.
                        foreach(GraphPath<T> path in subpaths)
                        {
                            path.Add(edge);
                        }

                        paths.AddRange(subpaths);
                    } 
                }
            }
            return paths;
        }
        #endregion
        #endregion

        public override string ToString()
        {
            if (Type == null) return $"Graph-Traversal(On {On})";
            return $"{Type}-Traversal(On {On})";
        }
    }
}

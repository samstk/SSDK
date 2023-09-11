using SSDK.Core.Structures.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Graphs
{
    /// <summary>
    /// Contains graph, vertex & edge helper/extension methods.
    /// </summary>
    public static class GraphHelper
    {

        /// <summary>
        /// Gets the total distance from a path (a list of edges)
        /// </summary>
        /// <typeparam name="T">the type of element stored in the graph</typeparam>
        /// <param name="path">the list of edges that make the path</param>
        /// <returns>the total distance</returns>
        public static UncontrolledNumber GetTotalDistance<T>(this IEnumerable<GraphEdge<T>> path)
        {
            UncontrolledNumber distance = 0;
            foreach(GraphEdge<T> edge in path)
            {
                distance += edge.GetDistance();
            }
            return distance;
        }
    }
}

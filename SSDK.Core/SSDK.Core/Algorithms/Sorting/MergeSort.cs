using SSDK.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Sorting
{
    /// <summary>
    /// A helper/extension class that contains the methods for a merge sort
    /// algorithm, which runs in O(n log2(n)) time.
    /// </summary>
    public static class MergeSort
    {
        /// <summary>
        /// Performs merge sort on the given array, assuming the elements
        /// are comparable.
        /// </summary>
        /// <param name="array">the array to perform the sort on</param>
        /// <param name="descendingOrder">if true, then sort in descending order</param>
        public static void SortViaMerge<T>(this T[] array, bool descendingOrder=false)
            where T : IComparable
        {
            array.SortViaMerge(0, array.Length-1, descendingOrder);
        }

        #region SortViaMerge Dependencies
        /// <summary>
        /// Computes the merge sort algorithm onto the array with the given start and end (inclusive)
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to sort</param>
        /// <param name="start">the element index to start at</param>
        /// <param name="end">the element index to end at</param>
        /// <param name="descendingOrder">if true, then sort in descending order</param>
        private static void SortViaMerge<T>(this T[] array, int start, int end, bool descendingOrder)
            where T : IComparable
        {
            if (start < end) {
                int subproblem = (start + end) / 2;
                array.SortViaMerge(start, subproblem, descendingOrder);
                array.SortViaMerge(subproblem + 1, end, descendingOrder);
                array.Merge(start, subproblem + 1, end, descendingOrder);
            }
        }
        /// <summary>
        /// Computes the merge algorithm onto the array with the given start, middle and end (inclusive)
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to sort</param>
        /// <param name="start">the element index to start at</param>
        /// <param name="middle">the middle index</param>
        /// <param name="end">the element index to end at</param>
        /// <param name="descendingOrder">if true, then sort in descending order</param>
        private static void Merge<T>(this T[] array, int start, int middle, int end, bool descendingOrder)
            where T : IComparable
        {
            // Check if array is already sorted.
            T[] nextArray = new T[end-start+1];

            int o = 0;
            int j = start;
            int k = middle;

            // Fill in array using smallest of each index.
            while (o < nextArray.Length)
            {
                if (j >= middle)
                {
                    // Rest of the elements are at k
                    nextArray[o++] = array[k++];
                }
                else if (k > end)
                {
                    // Rest of the elements are at j
                    nextArray[o++] = array[j++];
                }
                else
                {
                    if (array[j].CompareTo(array[k]) < 0)
                    {
                        nextArray[o++] = array[j++];
                    }
                    else nextArray[o++] = array[k++];
                }
            }
            
            Array.Copy(nextArray, 0, array, start, nextArray.Length);
        }
        #endregion
    }
}

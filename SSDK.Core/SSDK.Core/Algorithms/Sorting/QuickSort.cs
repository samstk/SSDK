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
    /// algorithm, which runs in O(n log n) time.
    /// </summary>
    public static class QuickSort
    {
        private static Random RG = new Random();
        /// <summary>
        /// Performs quick sort on the given array, assuming the elements
        /// are comparable. Estimation of O(n^2) running time.
        /// </summary>
        /// <param name="array">the array to perform the sort on</param>
        /// <param name="descendingOrder">if true, then sort in descending order</param>
        public static void SortViaQuickSort<T>(this T[] array, bool descendingOrder = false)
            where T : IComparable
        {
            array.SortViaQuickSort(0, array.Length - 1, descendingOrder);
        }

        #region SortViaQuickSort Dependencies
        /// <summary>
        /// Computes the merge sort algorithm onto the array with the given start and end (inclusive).
        /// This function is reoccuring./
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to sort</param>
        /// <param name="start">the element index to start at</param>
        /// <param name="end">the element index to end at</param>
        /// <param name="descendingOrder">if true, then sort in descending order</param>
        private static void SortViaQuickSort<T>(this T[] array, int start, int end, bool descendingOrder)
            where T : IComparable
        {
            // Pick random pivot
            int searchLength = end - start + 1;
            int pivot = RG.Next(start, end + 1);
            
            T target = array[pivot];

            // Allocate maximum length of search (for each equals)
            T[] equals = new T[searchLength];
            T[] less = new T[searchLength];
            T[] greater = new T[searchLength];
            int l = 0, g = 0, e = 0;
            

            // Categorise elements of the array. 
            for(int i = start; i<=end; i++)
            {
                T compare = array[i];
                int result = compare.CompareTo(target);
                if(result < 0)
                {
                    less[l++] = compare;
                }
                else if (result > 0)
                {
                    greater[g++] = compare;
                }
                else
                {
                    equals[e++] = compare;
                }
            }

            if (descendingOrder)
            {
                // Swap less and greater
                T[] gl = greater;
                int gg = g;
                greater = less;
                less = gl;
                g = l;
                l = gg;
            }

            // Sort L and G
            if (l > 1) less.SortViaQuickSort(0, l-1, descendingOrder);
            if (g > 1) greater.SortViaQuickSort(0, g-1, descendingOrder);

            // Join L, E and G
            if (l > 0) Array.Copy(less, 0, array, 0, l);
            if (e > 0) Array.Copy(equals, 0, array, l, e);
            if (g > 0) Array.Copy(greater, 0, array, l + e, g);
        }
        #endregion
    }
}

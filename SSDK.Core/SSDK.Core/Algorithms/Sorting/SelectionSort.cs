using SSDK.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Sorting
{
    /// <summary>
    /// A helper/extension class that contains the methods for an insertion sort
    /// algorithm, which runs in O(n^2) time.
    /// </summary>
    public static class SelectionSort
    {
        /// <summary>
        /// Performs insertion sort on the given array, assuming the elements
        /// are comparable. Estimation of O(n^2) running time.
        /// </summary>
        /// <param name="array">the array to perform the sort on</param>
        /// <param name="descendingOrder">if true, then sort in descendingOrder</param>
        public static void SortViaSelection<T>(this T[] array, bool descendingOrder = false)
            where T : IComparable
        {
            if (descendingOrder)
            {
                int n = array.Length;
                while (n > 1)
                {
                    int maxIndex = 0;
                    // Search for max index
                    for (int i = 1; i < array.Length; i++)
                    {
                        if (array[i].CompareTo(array[maxIndex]) < 0)
                        {
                            maxIndex = i;
                        }
                    }

                    array.Exchange(maxIndex, n - 1);
                    n--; // Repeat until array is sorted
                }
            }
            else
            {
                int n = array.Length;
                while (n > 1)
                {
                    int maxIndex = 0;
                    // Search for max index
                    for (int i = 1; i < array.Length; i++)
                    {
                        if (array[i].CompareTo(array[maxIndex]) > 0)
                        {
                            maxIndex = i;
                        }
                    }

                    array.Exchange(maxIndex, n - 1);
                    n--; // Repeat until array is sorted
                }
            }

        }
    }
}

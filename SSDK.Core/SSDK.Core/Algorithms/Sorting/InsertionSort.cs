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
    public static class InsertionSort
    {
        /// <summary>
        /// Performs insertion sort on the given array, assuming the elements
        /// are comparable. Estimation of O(n^2) running time.
        /// </summary>
        /// <param name="array">the array to perform the sort on</param>
        /// <param name="descendingOrder">if true, then sort in descendingOrder</param>
        public static void SortViaInsertion<T>(this T[] array, bool descendingOrder = false) 
            where T : IComparable
        {
            
            if(descendingOrder)
            {
                for (int j = 1; j < array.Length; j++)
                {
                    T element = array[j];

                    // Swap to correct position in array
                    int i = j - 1;

                    while (i > -1 && array[i].CompareTo(element) < 0)
                    {
                        array[i + 1] = array[i];
                        i--;
                    }

                    // Re-instate key
                    array[i + 1] = element;
                }
            }
            else
            {
                for (int j = 1; j < array.Length; j++)
                {
                    T element = array[j];

                    // Swap to correct position in array
                    int i = j - 1;

                    while (i > -1 && array[i].CompareTo(element) > 0)
                    {
                        array[i + 1] = array[i];
                        i--;
                    }

                    // Re-instate key
                    array[i + 1] = element;
                }
            }
            
        }
    }
}

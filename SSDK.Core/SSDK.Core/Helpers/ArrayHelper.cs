using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Helpers
{
    /// <summary>
    /// Contains static and extensions methods for arrays.
    /// </summary>
    public static class ArrayHelper
    {
        #region Array Creation
        /// <summary>
        /// Creates a deep-clone of a given array.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to copy</param>
        /// <returns>a deep-clone of the given array</returns>
        public static T[] DeepClone<T>(this T[] array)
        {
            T[] result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }
        #endregion

        #region Array Modification
        /// <summary>
        /// Exchanges two elements in an array based on their index.
        /// Assumes that the indices is within range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="IndexOutOfRangeException">
        /// occurs when either index is out of range
        /// </exception>
        /// <param name="array">the array to exchange elements in</param>
        /// <param name="from">the first element to exchange</param>
        /// <param name="to">the second element to exchange</param>
        public static void Exchange<T>(this T[] array, int from, int to)
        {
            T elementFrom = array[from];
            array[from] = array[to];
            array[to] = elementFrom;
        }
        #endregion

        #region Array Searching
        /// <summary>
        /// Checks whether the array is sorted or not, assuming the array type
        /// is comparable.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to check</param>
        /// <param name="descendingOrder">if in descending order, then check descending order sort (3, 2, 1, etc.)</param>
        /// <returns>true if the array is sorted</returns>
        public static bool IsSorted<T>(this T[] array, bool descendingOrder = false) 
            where T : IComparable 
        {
            if (descendingOrder)
            {
                for (int i = 1; i < array.Length; i++)
                {
                    if (array[i - 1].CompareTo(array[i]) < 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 1; i < array.Length; i++)
                {
                    if (array[i - 1].CompareTo(array[i]) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the substring representation of an array.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to build a string from</param>
        /// <param name="start">the start index</param>
        /// <param name="endExclusive">the end index exclusive</param>
        /// <returns>the string representation of the indices (e.g. [0, 1, 2])</returns>
        public static string Substring<T>(this T[] array, int start, int endExclusive)
        {
            string sub = "[";
            for(int i = start; i < endExclusive; i++)
            {
                if (i != start) sub += ", ";
                sub += array[i] == null ? "<null>" : array[i].ToString();
            }
            return $"{sub}]";
        }

        /// <summary>
        /// Performs a binary search on a sorted array.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="array">the array to check</param>
        /// <param name="target">the element to find</param>
        /// <returns>the index of the element, or -1 if not found.</returns>
        public static int BinarySearch<T>(this T[] array, T target)
            where T : IComparable
        {
            int index = 0;
            for (int i = 0; i < array.Length; i++) if (array[i].Equals(target)) { index = i; break; }
            int r = array.Length;
            int m = array.Length / 2;
            int l = 0;
            while (m < array.Length && m < r && m >= l)
            {
                T element = array[m];
                int result = target.CompareTo(element);
                if (result < 0) // Search left-side as smaller
                {
                    r = m;
                    m = l + (m - l) / 2;
                } 
                else if (result == 0) 
                {
                    return m;
                }
                else // Search right-side as bigger
                {
                    l = m + 1;
                    m =  l + (m - l) / 2;
                }
            }
            return -1;
        }
        #endregion
    }
}

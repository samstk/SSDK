using SSDK.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Algorithms.Sorting
{
    /// <summary>
    /// A helper/extension class that contains the methods for an bucket sort
    /// algorithm, which runs in O(n + k) time. where k is the number of buckets.
    /// </summary>
    public static class BucketSort
    {
        /// <summary>
        /// Performs bucket sort on the given array, assuming the elements
        /// are comparable. Estimation of O(n + k) running time.
        /// </summary>
        /// <param name="array">the array to perform the sort on</param>
        /// <param name="bucketCount">the number of buckets the bucket selector generates</param>
        /// <param name="bucketSelector">a function which puts the element of an array into a bucket</param>
        /// <returns>
        /// an integer array containing the start positions of all buckets.
        /// </returns>
        public static int[] SortViaBucket<T>(this T[] array, int bucketCount, Func<T, int> bucketSelector)
            where T : IComparable
        {
            // Initialise buckets
            LinkedList<T>[] buckets = array.GetBuckets(bucketCount, bucketSelector);

            int[] bucketIndices = new int[bucketCount];

            // Copy elements out to main array
            int index = 0;
            int bucketIndex = 0;
            foreach(LinkedList<T> bucket in buckets)
            {
                bucketIndices[bucketIndex++] = index;
                foreach(T element in bucket)
                {
                    array[index++] = element;
                }
            }
            
            return bucketIndices;
        }

        /// <summary>
        /// Performs bucket sort on the given array, assuming the elements
        /// are comparable. Estimation of O(n + k) running time.
        /// Instead of modifying the original array and returning the indices of the
        /// buckets, the buckets are simply returned.
        /// </summary>
        /// <param name="array">the array to look through</param>
        /// <param name="bucketCount">the number of buckets the bucket selector generates</param>
        /// <param name="bucketSelector">a function which puts the element of an array into a bucket</param>
        /// <returns>
        /// an linked list array containing all buckets.
        /// </returns>
        public static LinkedList<T>[] GetBuckets<T>(this T[] array, int bucketCount, Func<T, int> bucketSelector)
            where T : IComparable
        {
            // Initialise buckets
            LinkedList<T>[] buckets = new LinkedList<T>[bucketCount];
            for (int i = 0; i < bucketCount; i++) buckets[i] = new LinkedList<T>();

            // Iterate through array and append to bucket.
            foreach (T element in array)
            {
                buckets[bucketSelector(element)].AddLast(element);
            }

            return buckets;
        }
    }
}

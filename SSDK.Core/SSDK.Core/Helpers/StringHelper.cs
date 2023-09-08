using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Helpers
{
    /// <summary>
    /// Contains methods for helping with strings
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Inserts a string at a position by overwriting the original string.
        /// </summary>
        /// <param name="original">the original string</param>
        /// <param name="index">the index to insert at</param>
        /// <param name="toInsert">the string to insert</param>
        public static string InsertOverwriteCenter(this string original, int index, string toInsert)
        {
            int length = toInsert.Length;
            int target = index - length / 2;
            if (target < 0) target = 0;
            if (target > original.Length) target = original.Length;
            return original.Remove(target, length).Insert(target, toInsert);
        }
    }
}

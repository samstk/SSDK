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
        /// Splits a string, that should be in the format of a command line, into
        /// several arguments.
        /// </summary>
        /// <param name="commandLine">the line representing the full command</param>
        /// <returns>a number of arguments derived from the command line</returns>
        public static string[] SplitToCommandArgs(this string commandLine)
        {
            List<string> args = new List<string>();
            string argBuilder = "";
            bool quoteMode = false; // For when argument is quoted.
            bool escape = false;
            // Iterate through string and extract arguments.
            // following the basic format: abc" asdc"
            foreach(char c in commandLine)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!quoteMode)
                    {
                        if (argBuilder.Length > 0)
                        {
                            // Add arg and clear builder string.
                            args.Add(argBuilder);
                            argBuilder = "";
                        }
                    }
                }
                else if (escape)
                {
                    // Simply add trailing character
                    argBuilder += c;
                    escape = false;
                }
                else {
                    if (c == '"')
                    {
                        quoteMode = !quoteMode;
                    }
                    else if (c == '\\')
                    {
                        escape = true;
                    }
                    else argBuilder += c;
                }
            }
            
            // Check if final argument needs adding.
            if (argBuilder.Length > 0)
                args.Add(argBuilder);

            // Return array conversion
            return args.ToArray();
        }
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

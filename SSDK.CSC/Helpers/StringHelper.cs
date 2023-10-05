using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Helpers
{
    /// <summary>
    /// Contains helper methods for the string builder class.
    /// </summary>
    public static class StringHelper
    {
        
        /// <summary>
        /// Appends a new line only if there is existing content in the builder, and a new line 
        /// was not previously started.
        /// </summary>
        /// <param name="builder">the string builder</param>
        public static void StartNewLine(this StringBuilder builder, char indentChar='\t')
        {
            
            if (builder.Length > 0 && builder[builder.Length-1] != '\n' && builder[builder.Length-1] != indentChar)
                builder.AppendLine();
        }

        /// <summary>
        /// Appends a new line only if there is existing content in the builder, and a new line 
        /// was not previously started.
        /// </summary>
        /// <param name="builder">the string builder</param>
        public static void StartNewWord(this StringBuilder builder, char whitespace=' ', char[] additionalWordSeperators=null)
        {
            if (builder.Length > 0 && !char.IsWhiteSpace(builder[builder.Length - 1]) 
                && (additionalWordSeperators == null || !additionalWordSeperators.Contains(builder[builder.Length - 1])))
                builder.Append(whitespace);
        }

        /// <summary>
        /// The current amount of indents for the builder
        /// </summary>
        public static int CurrentIndents = 0;

        /// <summary>
        /// Continues the builder string.
        /// If any indents occured with ContinueWithAndOpen, then the string is prefixed with the indent char.
        /// </summary>
        /// <remarks>
        /// Compatible with single-thread only.
        /// Assumes that if the last character is a tab character, then it can continue without appending anything.
        /// </remarks>
        /// <param name="builder">the string builder</param>
        public static void Continue(this StringBuilder builder, char indentChar = '\t')
        {
            if (CurrentIndents > 0 && builder[builder.Length - 1] != indentChar) builder.Append("".PadLeft(CurrentIndents, indentChar));
        }

        /// <summary>
        /// Removes the last indent from the builder
        /// </summary>
        /// <param name="builder">the string builder</param>
        public static void SetLastIndent(this StringBuilder builder, char indentChar = '\t')
        {
            if(builder.Length > 0 && builder[builder.Length-1] == indentChar)
                builder.Length--;
        }


        /// <summary>
        /// Continues the builder string with the given string.
        /// If any indents occured with ContinueWithAndOpen, then the string is prefixed with the indent char.
        /// </summary>
        /// <remarks>
        /// Compatible with single-thread only.
        /// </remarks>
        /// <param name="builder">the string builder</param>
        /// <param name="with">the string to append</param>
        public static void ContinueWith(this StringBuilder builder, string with, char indentChar='\t')
        {
            builder.Continue(indentChar);
            builder.Append(with);
        }

        /// <summary>
        /// Continues the builder string with the given string line.
        /// If any indents occured with ContinueWithAndOpen, then the string is prefixed with the indent char.
        /// </summary>
        /// <remarks>
        /// Compatible with single-thread only.
        /// </remarks>
        /// <param name="builder">the string builder</param>
        /// <param name="with">the string to append</param>
        public static void ContinueWithLine(this StringBuilder builder, string with, char indentChar = '\t')
        {
            builder.Continue(indentChar);
            builder.AppendLine(with);
        }

        /// <summary>
        /// Continues the builder string with the given string.
        /// If any indents occured with ContinueWithAndOpen previously, then the string is prefixed with the indent char.
        /// Appends a new line afterwards, and increases the increment.
        /// </summary>
        /// <remarks>
        /// Compatible with single-thread only.
        /// </remarks>
        /// <param name="builder">the string builder</param>
        /// <param name="with">the string to append</param>
        public static void ContinueWithAndOpen(this StringBuilder builder, string with, char indentChar='\t')
        {
            builder.Continue(indentChar);
            builder.AppendLine(with);

            builder.Open();
        }

        /// <summary>
        /// Continues the builder string with the given string.
        /// If any indents occured with ContinueWithAndOpen, then the string is prefixed with the indent char (minus 1).
        /// Appends a new line afterwards, and decreases the increment.
        /// Expects the indent to be at least one.
        /// </summary>
        /// <remarks>
        /// Compatible with single-thread only.
        /// </remarks>
        /// <param name="builder">the string builder</param>
        /// <param name="with">the string to append</param>
        public static void ContinueWithAsClose(this StringBuilder builder, string with, char indentChar = '\t')
        {
            builder.Close();

            builder.Continue(indentChar);
            builder.AppendLine(with);
        }

        /// <summary>
        /// Opens a new level of indentation
        /// </summary>
        /// <param name="builder">the string builder</param>
        public static void Open(this StringBuilder builder)
        {
            CurrentIndents++;
        }

        /// <summary>
        /// Closes the current level of indentation
        /// </summary>
        /// <param name="builder">the string builder</param>
        public static void Close(this StringBuilder builder)
        {
            CurrentIndents--;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Exceptions
{
    /// <summary>
    /// An invalid flag exception, that should be thrown from the FlagIntepreter class.
    /// An error that occurs when a flag does not exist in the dictionary.
    /// </summary>
    public class InvalidFlagException : Exception
    {
        /// <summary>
        /// Constructs an invalid flag exception with the given flag and reference.
        /// </summary>
        /// <param name="flag">the flag that is invalid</param>
        public InvalidFlagException(string flag)
            : base($"The flag '{flag}' did not exist in the program's pre-defined list of flags") { }
    }
}

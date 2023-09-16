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
    public class MultipleFlagErrorsException : Exception
    {
        /// <summary>
        /// Gets all flag exceptions that caused this exception to be thrown.
        /// </summary>
        public List<Exception> InnerExceptions { get; private set; }

        /// <summary>
        /// Constructs an invalid flag exception with the given flag and reference.
        /// </summary>
        /// <param name="flag">the flag that is invalid</param>
        public MultipleFlagErrorsException(List<Exception> innerExceptions) : base(innerExceptions.CompileMultiexceptionMessage("One or more flag or argument errors were detected"))
        {
            InnerExceptions = innerExceptions;
        }
    }
}

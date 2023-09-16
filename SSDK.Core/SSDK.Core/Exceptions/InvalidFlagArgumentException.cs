using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Exceptions
{
    /// <summary>
    /// An invalid flag argument exception that An error that occurs when an argument is not compatible with the given flag.
    /// </summary>
    public class InvalidFlagArgumentException : Exception
    {
        /// <summary>
        /// The flag that had an invalid argument.
        /// </summary>
        public string Flag { get; private set; }

        /// <summary>
        /// The flag argument that was invalid.
        /// </summary>
        public string Argument { get; private set; }

        /// <summary>
        /// The reason why the argument was invalid.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Constructs an invalid flag exception with the given flag and reference.
        /// </summary>
        /// <param name="flag">the flag that is invalid</param>
        /// <param name="arg">the arg of the flag</param>
        /// <param name="reason">the reason why the arguement is invalid</param>
        public InvalidFlagArgumentException(string flag, string arg, string reason)
            : base($"An invalid argument '{arg}' was applied to the flag '{flag}'. {reason}")
        {
            Flag = flag;
            Argument = arg;
            Reason = reason;
        }
    }
}

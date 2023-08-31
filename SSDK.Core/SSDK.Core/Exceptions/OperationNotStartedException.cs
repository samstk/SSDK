using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when a operation was never
    /// started, but it was ended.
    /// </summary>
    public sealed class OperationNotStartedException : Exception
    {
        /// <summary>
        /// Gets the operation that was never started, which caused this exception.
        /// </summary>
        public string Operation { get; private set; }
        public OperationNotStartedException(string operation) : base($"The operation '{operation}' was never started.") {
            Operation = operation;
        }
    }
}

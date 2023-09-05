using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when existing data conflicted
    /// with the 
    /// </summary>
    public sealed class ConflictingDataException : Exception
    {
        public string Reason { get; set; }
        public ConflictingDataException(string reason) : base($"Existing data conflicted with the operation: {reason}")
        {
            Reason = reason;   
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Exceptions
{
    /// <summary>
    /// Depicts a conflict within the same scope due to
    /// multiples aliases being defined with the same name.
    /// </summary>
    public class AliasConflictException : Exception
    {
        public AliasConflictException(string alias, string location) : base($"Multiple aliases with the same name '{alias}' was defined in the script ({location})") { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# indexor, which is a property of a given class which uses
    /// property-like syntax to allow index retrieval.
    /// e.g. array[i]
    /// </summary>
    public sealed class CSharpIndexer : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the variable declarations indicating the parameters of this indexor.
        /// </summary>
        public CSharpVariable[] Parameters { get; private set; }
        #endregion
    }
}

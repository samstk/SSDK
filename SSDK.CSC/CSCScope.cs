using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC
{
    /// <summary>
    /// Represents a scope which contains all known symbols.
    /// </summary>
    public sealed class CSCScope
    {
        /// <summary>
        /// Gets the list of immediate symbols that can be referenced in this scope.
        /// </summary>
        private List<CSCSymbol> _ImmediateSymbols = new List<CSCSymbol>();

        /// <summary>
        /// Gets the parent scope in which additional symbols
        /// can be found.
        /// </summary>
        public CSCScope ParentScope { get; private set; }

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <param name="parentScope">the scope which this scope can also access.</param>
        public CSCScope(CSCScope parentScope)
        {
            ParentScope = parentScope;
        }

        /// <summary>
        /// Adds a symbol to the list of immediate symbols that can be references in this scope.
        /// </summary>
        /// <param name="symbol"></param>
        public void AddSymbol(CSCSymbol symbol)
        {

        }
    }
}

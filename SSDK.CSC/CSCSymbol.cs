using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC
{
    /// <summary>
    /// Represents a identifier symbol used in CSharp.
    /// e.g. 
    /// </summary>
    public sealed class CSCSymbol
    {
        /// <summary>
        /// Gets the parent of this identifier.
        /// (i.e. another symbol as a 'parent' of this symbol, 
        /// for example, System.Console.WriteLine has three symbols,
        /// and WriteLine's parent is Console)
        /// </summary>
        public CSCSymbol Parent { get; private set; }

        /// <summary>
        /// Gets the identifier as a string
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Gets the type of this symbol.
        /// </summary>
        public SymbolType SymbolType { get; private set; }

        /// <summary>
        /// Gets the children of this symbol.
        /// </summary>
        public List<CSCSymbol> Children { get; private set; } = new List<CSCSymbol>();

        /// <summary>
        /// Creates a new symbol 
        /// </summary>
        /// <param name="parent">parent of the symbol. e.g. Console in Console.WriteLine</param>
        /// <param name="identifier">the identifier of the symbol (e.g. WriteLine)</param>
        /// <param name="symbolType">the type of the symbol (e.g. Namespace)</param>
        public CSCSymbol(CSCSymbol parent, string identifier, SymbolType symbolType)
        {
            Parent = parent;
            parent?.Children.Add(this);
            Identifier = identifier;
            SymbolType = symbolType;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode() + (Parent != null ? Parent.GetHashCode() : 0);
        }

        public override bool Equals(object? obj)
        {
            CSCSymbol other = obj as CSCSymbol;
            if(other != null)
            {
                return other.Identifier == Identifier && other.Parent == Parent;
            }
            return false;
        }

        public override string ToString()
        {
            string txt = Identifier;

            // Form p3.p2.p1.p0 recursively.
            CSCSymbol parent = Parent;
            while(parent != null)
            {
                txt = parent.ToString() + "." + txt;
                parent = parent.Parent;
            }
            return txt;
        }
    }

    public enum SymbolType
    {
        Class,
        Function,
        Variable,
        Namespace
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// Represents a symbol for a member of a script.
    /// </summary>
    public sealed class CSharpMemberSymbol
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name of the field, which is the identifier that
        /// was used to define it.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the usage list of this symbol.
        /// </summary>
        public List<CSharpComponent> Usages { get; private set; } = new List<CSharpComponent>();
    
        /// <summary>
        /// Gets the parent of this symbol (i.e. the component symbol holding this symbol)
        /// </summary>
        public CSharpMemberSymbol Parent { get; private set; }

        /// <summary>
        /// Gets the component that is accessed by this symbol.
        /// </summary>
        public CSharpComponent Component { get; private set; }

        /// <summary>
        /// Gets the children of this symbol (i.e. symbols created with this as its parent)
        /// </summary>
        public List<CSharpMemberSymbol> ChildSymbols { get; private set; } = new List<CSharpMemberSymbol>();

        /// <summary>
        /// Gets the symbols loaded under this symbol (i.e. with a non-alias using directive).
        /// </summary>
        public List<CSharpMemberSymbol> LoadedSymbols { get; private set; } = new List<CSharpMemberSymbol>();

        /// <summary>
        /// Gets whether this component is accessible or not outside its children's scope.
        /// </summary>
        public bool IsAccessible { get; private set; }
        #endregion
        /// <summary>
        /// Creates a member symbol
        /// </summary>
        /// <param name="name">the name of the symbol</param>
        /// <param name="parent">the parent symbol</param>
        /// <param name="component">the component defining this symbol</param>
        /// <param name="isAccessible">
        /// if true, then components outside this symbol should be
        /// able to access it by name
        /// </param>
        public CSharpMemberSymbol(string name, CSharpMemberSymbol parent, CSharpComponent component, bool isAccessible=true)
        {
            Name = name;
            Parent = parent;
            Component = component;
            Parent?.ChildSymbols.Add(this);
            IsAccessible = isAccessible;
        }

        public override string ToString()
        {
            return Parent == null ? Name : $"{Parent}.{Name}";
        }
    }
}

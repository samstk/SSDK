using Microsoft.CodeAnalysis;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public CSharpComponent Component { get; internal set; }

        /// <summary>
        /// If this symbol is a symbol for an alias, then this will
        /// represent the symbol that should actually be referenced instead.
        /// </summary>
        public CSharpMemberSymbol AliasTo { get; internal set; }

        /// <summary>
        /// If not null, then this symbol was merged with another symbol, and
        /// this list contains additional components apart from Component that
        /// formed this symbol.
        /// </summary>
        public List<CSharpComponent> AdditionalComponents { get; private set; }

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
        
        /// <summary>
        /// Gets the unique id set upon creation.
        /// </summary>
        public int UniqueID { get; private set; }
        private static int UniqueIdIncrementor = 0;
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
            UniqueID = UniqueIdIncrementor++;
        }

        /// <summary>
        /// Merges this symbol with another symbol
        /// </summary>
        /// <remarks>
        /// Does not check, but assumes the names are the same.
        /// Additionally updates references of parent and children.
        /// </remarks>
        /// <param name="other">the other symbol to merge</param>
        /// <returns>
        /// A new symbol resulting from the merge of both symbols,
        /// where no child or parent references to old symbols are replaced.
        /// </returns>
        public CSharpMemberSymbol Merge(CSharpMemberSymbol other)
        {
            CSharpMemberSymbol sym = new CSharpMemberSymbol(Name, Parent, Component, IsAccessible);
            sym.AdditionalComponents = new List<CSharpComponent>();
            if(other.Component != null)
                sym.AdditionalComponents.Add(other.Component);
            if (AdditionalComponents != null)
                sym.AdditionalComponents.AddRange(AdditionalComponents);
            if (other.AdditionalComponents != null)
                sym.AdditionalComponents.AddRange(other.AdditionalComponents);

            // Update component symbols
            if (sym.Component != null)
                sym.Component.Symbol = sym;

            foreach (CSharpComponent component in sym.AdditionalComponents)
                component.Symbol = sym;

            // Update child references
            foreach (CSharpMemberSymbol child in ChildSymbols)
            {
                child.Parent = sym;
                sym.ChildSymbols.Add(child);
            }

            foreach (CSharpMemberSymbol child in other.ChildSymbols)
            {
                child.Parent = sym;
                sym.ChildSymbols.Add(child);
            }

            // Update parent references
            if (Parent != null)
            {
                Parent.ChildSymbols.Remove(this);
            }

            if (other.Parent != null)
            {
                other.Parent.ChildSymbols.Remove(other);
            }

            return sym;
        }

        /// <summary>
        /// Gets the best-matching symbol from the current symbol's scope
        /// </summary>
        /// <param name="target">the target to find</param>
        /// <param name="types">
        /// The types of the context in which the symbol must be most compatible with.
        /// Use null if the symbol is compatible with any context usage (no overloading)
        /// </param>
        /// <returns>if found, then the symbol given the target, else null</returns>
        public CSharpMemberSymbol FindBestMatchingSymbol(string target, CSharpMemberSymbol[] types)
        {
            if (target.Length == 0) return null;
            return FindBestMatchingSymbol(target.GetTargetSections(), types: types);
        }
        /// <summary>
        /// Gets the best-matching symbol from the current symbol's scope
        /// </summary>
        /// <param name="targetSections">the target sections to find</param>
        /// <param name="sectionsFound">the sections found</param>
        /// <param name="lookType">
        /// the current method of looking:<br/>
        /// 0 = Ascend to parent only then switch to mode 1
        /// 1 = Look through children or ascend to parent
        /// 2 = Look through descendants only.
        /// </param>
        /// <param name="types">
        /// the types of the context in which the symbol must be most compatible with.
        /// </param>
        /// <returns>if found, then the symbol given the target, else null</returns>
        public CSharpMemberSymbol FindBestMatchingSymbol(string[] targetSections, int sectionsFound=0, int lookType=0, bool allowLoadedSymbolLookup=true, CSharpMemberSymbol[] types=null)
        {
            string sectionToFind = targetSections[sectionsFound];
            if (lookType == 0)
            { 
                // Go up one scope, as the function which is called on a particular symbol should never search its
                // children
                if (Parent != null)
                    return Parent.FindBestMatchingSymbol(targetSections, sectionsFound, 1, allowLoadedSymbolLookup);
            }
            else if (lookType == 1 || lookType == 2)
            {
                double bestCompatibility = 0;
                CSharpMemberSymbol bestCompatibleSymbol = null;

                // Look through direct descendants for immediate section
                if (IsAccessible)
                {
                    foreach (CSharpMemberSymbol child in ChildSymbols)
                    {
                        if (child.Name == sectionToFind)
                        {
                            if (sectionsFound + 1 == targetSections.Length)
                            {
                                if (types != null && (child.Component is CSharpMethod ||
                                    child.Component is CSharpDelegate))
                                {
                                    double compat = child.GetCompatibility(types);
                                    if (compat > bestCompatibility)
                                    {
                                        bestCompatibility = compat;
                                        bestCompatibleSymbol = child;
                                    }
                                }
                                else return child.GetActualSymbol();
                            }
                            // Look through all descendants of symbol to find rest of sections.
                            CSharpMemberSymbol sym = child.FindBestMatchingSymbol(targetSections, sectionsFound + 1, 2, false);
                            if (sym != null)
                                return sym.GetActualSymbol();
                        }
                    }
                }

                if (allowLoadedSymbolLookup)
                {
                    foreach (CSharpMemberSymbol child in LoadedSymbols)
                    {
                        if (child.Name == sectionToFind)
                        {
                            if (types != null && (child.Component is CSharpMethod ||
                                child.Component is CSharpDelegate))
                            {
                                double compat = child.GetCompatibility(types);
                                if (compat > bestCompatibility)
                                {
                                    bestCompatibility = compat;
                                    bestCompatibleSymbol = child;
                                }
                            }
                            else return child.GetActualSymbol();

                            // Look through all descendants of symbol to find rest of sections.
                            CSharpMemberSymbol sym = child.FindBestMatchingSymbol(targetSections, sectionsFound + 1, 2, false);
                            if (sym != null)
                                return sym.GetActualSymbol();
                        }
                    }
                }

                if (bestCompatibleSymbol != null)
                    return bestCompatibleSymbol.GetActualSymbol();

                if (lookType == 1 && Parent != null)
                {
                    // Go up one scope, as the symbol might be found at a higher level.
                    return Parent.FindBestMatchingSymbol(targetSections, sectionsFound, 1, allowLoadedSymbolLookup);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the actual symbol (i.e. if an alias exists, only use the referenced symbol)
        /// </summary>
        /// <returns>the actual symbol that should be used</returns>
        public CSharpMemberSymbol GetActualSymbol()
        {
            if (AliasTo != null) return AliasTo.GetActualSymbol();
            return this;
        }

        /// <summary>
        /// Gets the compatibility (0-1) for a delegate or method's 
        /// </summary>
        /// <param name="types">
        /// the types of the context in which the symbol must be most compatible with.
        /// </param>
        /// <returns>a value ranging from 0 (least) to 1 (most) compatible with the types</returns>
        public double GetCompatibility(CSharpMemberSymbol[] types)
        {
            if(types == null)
                return 1; // First in - first serve

            if (Component is CSharpDelegate)
            {

            }

            return 0;
        }

        /// <summary>
        /// Merges all member symbols with the same full name (e.g. System.Text),
        /// if it is a mergable component (i.e. a namespace, or a struct/class with partial on it)
        /// </summary>
        /// <param name="mappings">a mapping of full names to member symbols</param>
        internal void MergeMemberSymbols(Dictionary<string, CSharpMemberSymbol> mappings)
        {
            if (Parent != null)
            {
                string qualifiedName = GetQualifyingName();
                if (mappings.ContainsKey(qualifiedName))
                {
                    mappings[qualifiedName] = mappings[qualifiedName].Merge(this);
                }
                else
                {
                    mappings.Add(qualifiedName, this);
                }
            }

            foreach(CSharpMemberSymbol child in ChildSymbols.ToArray())
            {
                if(child.Component == null ||
                   child.Component is CSharpNamespace ||
                   child.Component is CSharpClass ||
                   child.Component is CSharpStruct ||
                   child.Component is CSharpEnum)
                {
                    child.MergeMemberSymbols(mappings);
                }
            }
        }

        /// <summary>
        /// Ensures all classes inherit object members
        /// </summary>
        /// <param name="objectSymbol">the symbol to inherit all children of</param>
        internal void InheritObjectMembers(CSharpMemberSymbol objectSymbol)
        {
            if(Component is CSharpClass ||
                   Component is CSharpStruct ||
                   Component is CSharpEnum)
            {
                LoadFromSymbol(objectSymbol);
            }
            foreach (CSharpMemberSymbol child in ChildSymbols.ToArray())
            {
                if (child.Component == null ||
                   child.Component is CSharpNamespace ||
                   child.Component is CSharpClass ||
                   child.Component is CSharpStruct ||
                   child.Component is CSharpEnum)
                {
                    child.InheritObjectMembers(objectSymbol);
                }
            }
        }


        /// <summary>
        /// Loads this symbol to the nearest ancestor block if it exists
        /// (excl classes/structs/namespaces)
        /// </summary>
        internal void LoadIntoBlockSymbol()
        {
            CSharpMemberSymbol parent = Parent;
            while(parent != null)
            {
                if (parent.Component is CSharpStatementBlock)
                {
                    parent.LoadedSymbols.Add(this);
                    break;
                }
                else if (parent.Component is not CSharpExpression && parent.Component is not CSharpStatement)
                    break;
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Loads this symbol's children into this symbol's loaded symbols list.
        /// </summary>
        internal void LoadFromSymbol(CSharpMemberSymbol symbol)
        {
            if (symbol == null) return;
            LoadedSymbols.AddRange(symbol.ChildSymbols);
        }

        /// <summary>
        /// Gets the c# type resulting from accessing the symbol.
        /// </summary>
        /// <returns>the c# type of the symbol</returns>
        public CSharpType GetComponentType(CSharpProject project)
        {
            CSharpType type = _GetComponentType(project);
            if(type != null)
            {
                if(type.ReferencedSymbol == null)
                {
                    type.ResolveMembers(project);
                }
                return type;
            }
            throw new Exception("Unhandled case");
        }

        private CSharpType _GetComponentType(CSharpProject project)
        {
            return Component?.GetComponentType(project);
        }

        public override string ToString()
        {
            return Parent == null ? $"{Name}[{UniqueID}]" : $"{Parent}.{Name}[{UniqueID}]";
        }

        /// <summary>
        /// Gets the qualifying name of the symbol (without root)
        /// </summary>
        /// <returns>the qualifying full name of the symbol</returns>
        public string GetQualifyingName()
        {
            string parent = Parent == null ? "" : Parent.GetQualifyingName();
            string thisName = Name;
            if(Component != null && Component is CSharpClass)
            {
                CSharpClass cl = Component as CSharpClass;
                if(cl.TypeParameters.Length > 0)
                {
                    thisName = $"{thisName}<{("".PadLeft(cl.TypeParameters.Length - 1, ','))}>";
                }
            }
            if (parent.Length > 0)
                return $"{parent}.{thisName}";

            return Parent == null ? "" : thisName;
        }

        /// <summary>
        /// Creates a type referencing the class / struct component of this symbol.
        /// </summary>
        /// <returns>the simple type referencing the symbol</returns>
        public CSharpType AsType(CSharpType[] genericTypes=null)
        {
            if (genericTypes == null) genericTypes = CSharpType.Empty;
            if (Component is CSharpClass)
                return new CSharpType(Name, genericTypes) { ReferencedSymbol = this };
            return null;
        }
    }
}

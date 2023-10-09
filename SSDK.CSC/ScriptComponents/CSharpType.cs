using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# declared type, which may be used to define variables/fields/properties, parameters or arguments.
    /// </summary>
    public sealed class CSharpType : CSharpExpression
    {
        /// <summary>
        /// Gets the context's type name used in the code.
        /// If array dimensions > 0, then the name is simply the string
        /// representation of the element type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the referenced type symbol
        /// </summary>
        /// <remarks>
        /// Only applies to an simple type, and is generated after
        /// ResolveMembers is called on the project.
        /// </remarks>
        public CSharpMemberSymbol ReferencedSymbol { get; internal set; }

        /// <summary>
        /// Gets the access type when array dimensions > 0, or
        /// the type being accessed has a namespace included (e.g. System.String)
        /// </summary>
        public CSharpType AccessType { get; private set; }

        /// <summary>
        /// Gets the generic type params for this type declaration.
        /// </summary>
        public CSharpType[] GenericTypes { get; private set; }
        
        /// <summary>
        /// Gets the referenced symbols for the generic types.
        /// </summary>
        public CSharpMemberSymbol[] ReferencedGenericTypes { get; private set; }

        /// <summary>
        /// Is true if the syntax defined some generic types.
        /// </summary>
        public bool HasGenericTypes { get; private set; } = false;

        /// <summary>
        /// Gets the array dimensions of this type. If zero, then
        /// it is not an array.
        /// </summary>
        public int ArrayDimensions { get; private set; } = 0;

        /// <summary>
        /// If true, then the element type contains the actual
        /// type, and this is a pointer to that type (i.e. elementType*)
        /// </summary>
        public bool IsPointer { get; private set; } = false;

        /// <summary>
        /// If true, then this type is simply a wrapper for a scope
        /// reference. The name of this type is that of the scope,
        /// and the access type is set to the actual type (or another ref)
        /// </summary>
        public bool IsRefOnly { get; private set; } = false;

        /// <summary>
        /// Creates a new c# type reference with the given context name and generic types.
        /// </summary>
        /// <param name="name">the name used in the context</param>
        /// <param name="genericTypes">the generic parameters</param>
        public CSharpType(string name, CSharpType[] genericTypes)
        {
            Name = name;
            GenericTypes = genericTypes;
            HasGenericTypes = genericTypes.Length > 0;
        }
        
        /// <summary>
        /// Creates a new c# type with a special name (i.e.. a keyword)
        /// </summary>
        /// <param name="name">the name of the type</param>
        /// <returns>the c# type</returns>
        internal static CSharpType Special(string name)
        {
            return new CSharpType(name, Empty);
        }

        internal static CSharpType[] Empty = new CSharpType[0];

        internal CSharpType(TypeSyntax typeSyntax)
        {
            if (typeSyntax is GenericNameSyntax)
            {
                GenericNameSyntax genericNameSyntax = (GenericNameSyntax)typeSyntax;
                Name = genericNameSyntax.Identifier.ToString();
                GenericTypes = genericNameSyntax.TypeArgumentList.ToTypes();
                HasGenericTypes = true;
            }
            else if (typeSyntax is IdentifierNameSyntax)
            {
                Name = ((IdentifierNameSyntax)typeSyntax).Identifier.ToString();
                GenericTypes = Empty;
            }
            else if (typeSyntax is PredefinedTypeSyntax)
            {
                Name = ((PredefinedTypeSyntax)typeSyntax).Keyword.ToString();
                GenericTypes = Empty;
            }
            else if (typeSyntax is ArrayTypeSyntax)
            {
                ArrayTypeSyntax array = ((ArrayTypeSyntax)typeSyntax);
                AccessType = new CSharpType(array.ElementType);
                Name = AccessType.ToString();
                ArrayDimensions = array.RankSpecifiers[0].Rank;
                GenericTypes = Empty;
            }
            else if (typeSyntax is PointerTypeSyntax)
            {
                PointerTypeSyntax pointer = ((PointerTypeSyntax)typeSyntax);
                AccessType = pointer.ElementType.ToType();
                IsPointer = true;
                Name = "Pointer";
            }
            else if (typeSyntax is QualifiedNameSyntax)
            {
                QualifiedNameSyntax name = ((QualifiedNameSyntax)typeSyntax);
                Name = ((IdentifierNameSyntax)name.Left).Identifier.ToString();
                AccessType = name.Right.ToType();
                IsRefOnly = true;
            }
            else throw new Exception("Unhandled case");
        }

        internal CSharpType(TypeConstraintSyntax typeSyntax) : this(typeSyntax.Type) { }

        internal CSharpType(TypeParameterSyntax typeSyntax)
        {
            throw new Exception("Unhandled case");
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is not CSharpType)
                return false;

            CSharpType other = (CSharpType)obj;

            if(!(HasGenericTypes != other.HasGenericTypes
                || ArrayDimensions != other.ArrayDimensions
                || IsPointer != other.IsPointer
                || IsRefOnly != other.IsRefOnly
                || AccessType != other.AccessType
                || GenericTypes.Length != other.GenericTypes.Length))
            {
                return false;
            }

            for(int i = 0; i<GenericTypes.Length; i++)
            {
                if (other.GenericTypes[i] != GenericTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder suffix = new StringBuilder();
            if(GenericTypes.Length > 0)
            {
                suffix.Append("<");
                foreach(CSharpType type in GenericTypes)
                {
                    if (suffix.Length > 1)
                        suffix.Append(", ");
                    suffix.Append(type.ToString());
                } 
                suffix.Append(">");
            }

            if(ArrayDimensions > 0)
            {
                suffix.Append("[");
                for (int i = 1; i < ArrayDimensions; i++)
                    suffix.Append(",");
                suffix.Append("]");
            }

            return $"{Name}{suffix.ToString()}";
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessType(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            // Technically, a type in the CSC package is simply a reference to a symbol, so
            // it must be set in the ResolveMembers.
            Symbol = new CSharpMemberSymbol("type<", parentSymbol, this, false);

            for (int i = 0; i < GenericTypes.Length; i++)
            {
                GenericTypes[i]?.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            ReferencedGenericTypes = new CSharpMemberSymbol[GenericTypes.Length];
            for(int i = 0; i<GenericTypes.Length; i++)
            {
                GenericTypes[i]?.ResolveMembers(project);
                ReferencedGenericTypes[i] = GenericTypes[i].ReferencedSymbol;
            }
            if(!IsPointer && ArrayDimensions == 0)
            {
                ReferencedSymbol = Symbol.FindBestMatchingSymbol(Name, ReferencedGenericTypes);
            }
            else if (IsPointer) {
                ReferencedSymbol = Symbol.FindBestMatchingSymbol("Pointer", new CSharpMemberSymbol[0]);
            }
            else if (ArrayDimensions > 0)
            {
                ReferencedSymbol = Symbol.FindBestMatchingSymbol("Array", new CSharpMemberSymbol[0]);
            }
        }

        /// <summary>
        /// Selects the most generic type of both types
        /// </summary>
        /// <param name="type1">the first type to compare</param>
        /// <param name="type2">the second type to compare</param>
        /// <returns>the most generic type</returns>
        /// <remarks>
        /// Pointers are not supported for inherited classes. Assumes
        /// that both types are comparable (i.e. one is equal or inherited from the other).
        /// Requires member references to exist.
        /// </remarks>
        public static CSharpType MostGeneric(CSharpType type1, CSharpType type2)
        {
            if (type1 == null) return type2;
            if (type2 == null) return null;

            if (type1.IsPointer && type2.IsPointer || type1.IsRefOnly && type2.IsRefOnly)
                return MostGeneric(type1.AccessType, type2.AccessType);

            return type1;
        }

        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return this;
        }
    }
}

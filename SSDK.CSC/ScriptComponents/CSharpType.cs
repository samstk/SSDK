﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        /// Gets the access type when array dimensions > 0, or
        /// the type being accessed has a namespace included (e.g. System.String)
        /// </summary>
        public CSharpType AccessType { get; private set; }

        /// <summary>
        /// Gets the generic type params for this type declaration.
        /// </summary>
        public CSharpType[] GenericTypes { get; private set; }

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
        public bool Pointer { get; private set; } = false;

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
                Pointer = true;
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
                || Pointer != other.Pointer
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
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# class, which may contain methods, properties, fields, and sub-classes
    /// </summary>
    public sealed class CSharpEnum : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the symbol that represents this component.
        /// </summary>
        /// <remarks>
        /// ResolveMembers must be called on the project before being set.
        /// </remarks>
        public CSharpMemberSymbol Symbol { get; private set; }

        /// <summary>
        /// Gets the name of the class
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the access modifier applied to this enum.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.Internal;

        /// <summary>
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        // <summary>
        /// Gets the reference type of this class 
        /// </summary>
        public CSharpType ObjectType { get; private set; }

        /// <summary>
        /// Gets the possible values of this enum.
        /// </summary>
        public CSharpEnumValue[] Values { get; private set; }
        
        /// <summary>
        /// Gets the syntax that formed this enum.
        /// </summary>
        public EnumDeclarationSyntax Syntax { get; private set; }

        /// <summary>
        /// Gets all types inherited by this enum.
        /// </summary>
        public CSharpType[] Inherits { get; private set; }
        #endregion

        internal CSharpEnum(EnumDeclarationSyntax syntax)
        {
            Name = syntax.Identifier.ToString();

            (_, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Attributes = syntax.AttributeLists.ToAttributes();

            if (syntax.BaseList == null || syntax.BaseList.Types.Count == 0)
                Inherits = CSharpType.Empty;
            else Inherits = syntax.BaseList.ToTypes();

            Values = new CSharpEnumValue[syntax.Members.Count];
            for(int i = 0; i<Values.Length; i++)
            {
                Values[i] = new CSharpEnumValue(syntax.Members[i], this);
            }
            Syntax = syntax;
        }

        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol(Name, parentSymbol, this);

            foreach (CSharpEnumValue value in Values)
            {
                value.CreateMemberSymbols(project, Symbol);
            }
            ObjectType = new CSharpType(Name, CSharpType.Empty) { ReferencedSymbol = Symbol };
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            
        }
        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return ObjectType;
        }
    }
}

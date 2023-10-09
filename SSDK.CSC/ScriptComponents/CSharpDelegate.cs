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
    /// A C# delegate, which may define the format for an anonymous function.
    /// </summary>
    public sealed class CSharpDelegate : CSharpComponent
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
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the type of this method as a generic Func or Action type.
        /// </summary>
        public CSharpType FuncType { get; private set; }

        /// <summary>
        /// Gets the access modifier applied to this delegate.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.DefaultOrNone;

        /// <summary>
        /// Gets the syntax that constructed this delegate.
        /// </summary>
        public DelegateDeclarationSyntax Syntax { get; private set; }

        /// <summary>
        /// Gets the name of the delegate
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the return type of the delegate
        /// </summary>
        public CSharpType ReturnType { get; private set; }

        /// <summary>
        /// Gets the parameters of the delegate
        /// </summary>
        public CSharpVariable[] Parameters { get; private set; }

        /// <summary>
        /// Gets the type parameters of the delegate
        /// </summary>
        public string[] TypeParameters { get; private set; }

        /// <summary>
        /// Gets the type constraints on the parameters.
        /// </summary>
        public Dictionary<string, CSharpType[]> TypeConstraints { get; private set; }

        #endregion

        internal CSharpDelegate(DelegateDeclarationSyntax syntax)
        {
            Syntax = syntax;

            (_, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Name = syntax.Identifier.ToString();

            Attributes = syntax.AttributeLists.ToAttributes();

            ReturnType = syntax.ReturnType.ToType();

            Parameters = syntax.ParameterList.ToParameters();

            TypeParameters = syntax.TypeParameterList.ToNames();

            TypeConstraints = syntax.ConstraintClauses.ToTypeConstraints();
        }

        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol(Name, parentSymbol, this);
            ReturnType?.CreateMemberSymbols(project, Symbol);
            foreach (CSharpVariable variable in Parameters)
            {
                variable.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            ReturnType?.ResolveMembers(project);
            CSharpType[] types = new CSharpType[Parameters.Length];
            int i = 0;
            foreach (CSharpVariable variable in Parameters)
            {
                variable.ResolveMembers(project);
                types[i++] = variable.Type;
            }

            if (ReturnType == null)
            {
                FuncType = new CSharpType("Action", types);
            }
            else
            {
                FuncType = new CSharpType("Func", types);
            }
            FuncType?.CreateMemberSymbols(project, Symbol);
            FuncType?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"{(AccessModifier.ToReadablePrefix())} delegate {ReturnType} {Name}({Parameters.ToReadableString()})";
        }

        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return FuncType;
        }
    }
}

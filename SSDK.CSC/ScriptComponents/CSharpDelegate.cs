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
        /// Gets the access modifier applied to this class.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.Internal;

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

            ReturnType = syntax.ReturnType.ToType();

            Parameters = syntax.ParameterList.ToParameters();

            TypeParameters = syntax.TypeParameterList.ToNames();

            TypeConstraints = syntax.ConstraintClauses.ToTypeConstraints();
        }

        public override string ToString()
        {
            return $"{(AccessModifier.ToReadablePrefix())} delegate {ReturnType} {Name}({Parameters.ToReadableString()})";
        }
    }
}

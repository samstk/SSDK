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
    public sealed class CSharpDelegate
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
        #endregion

        internal CSharpDelegate(DelegateDeclarationSyntax syntax)
        {
            Syntax = syntax;

            (_, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Name = syntax.Identifier.ToString();
        }
    }
}

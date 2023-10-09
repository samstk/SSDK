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
    /// A C# attribute, which may be applied to a component.
    /// </summary>
    public sealed class CSharpAttribute : CSharpComponent
    {
        #region Properties & Fields
        internal static CSharpAttribute[] Empty = new CSharpAttribute[0];
        /// <summary>
        /// Gets the used type of the attribute
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the list of expressions depicts the parameters
        /// of this attribute.
        /// </summary>
        public CSharpExpression[] Parameters { get; private set; }
        #endregion

        /// <summary>
        /// Creates an c# attribute from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpAttribute(AttributeSyntax syntax)
        {
            Type = syntax.Name.ToType();
            Parameters = syntax.ArgumentList.ToExpressions();
        }

        public override string ToString()
        {
            return $"[{Type}({Parameters.ToReadableString()})]";
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("attr[", parentSymbol, this, false);
            Type?.CreateMemberSymbols(project, Symbol);
            foreach (CSharpExpression param in Parameters)
            {
                param.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            
            foreach (CSharpExpression param in Parameters)
            {
                param.ResolveMembers(project);
            }

            
        }
    }
}

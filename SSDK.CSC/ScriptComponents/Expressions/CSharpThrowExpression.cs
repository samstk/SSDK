using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A c# throw expression
    /// </summary>
    public sealed class CSharpThrowExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression of the exception to be thrown...
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion

        /// <summary>
        /// Creates the literal value from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpThrowExpression(ThrowExpressionSyntax syntax)
        {
            Syntax = syntax;
            Expression = syntax.Expression.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessThrowExpression(this, result);
        }

        public override string ToString()
        {
            return $"throw {Expression}";
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("throw(", parentSymbol, this, false);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Expression?.ResolveMembers(project);
        }
    }
}

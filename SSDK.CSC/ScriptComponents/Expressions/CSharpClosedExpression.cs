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
    /// A c# closed expression (i.e. an expression within parentheses)
    /// </summary>
    public sealed class CSharpClosedExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression closed in these parentheses.
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion
        /// <summary>
        /// Creates the literal value from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpClosedExpression(ParenthesizedExpressionSyntax syntax)
        {
            Syntax = syntax;
            Expression = syntax.Expression.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessClosedExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("(", parentSymbol, this, false);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Expression?.ResolveMembers(project);
        }

        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return Expression.GetComponentType(project);
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# return statement for a given expression.
    /// </summary>
    /// <remarks>
    /// Statement Syntax may not be stored for a return statement resulting
    /// from the arrow operator (e.g. public static int X() => 0);
    /// </remarks>
    public sealed class CSharpReturnStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression returned
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion


        /// <summary>
        /// Creates the return statement from the given syntax.
        /// </summary>
        /// <param name="returnSyntax">the syntax to create from</param>
        internal CSharpReturnStatement(ReturnStatementSyntax returnSyntax)
        {
            if (returnSyntax.Expression != null)
                Expression = returnSyntax.Expression.ToExpression();
            Syntax = returnSyntax;
        }

        /// <summary>
        /// Creates the return statement from the given syntax.
        /// </summary>
        /// <param name="expression">the syntax to create from</param>
        internal CSharpReturnStatement(ExpressionSyntax expression, StatementSyntax syntax)
        {
            if(expression != null)
                Expression = expression.ToExpression();
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessReturnStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("return", parentSymbol, this, false);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Expression?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return Expression != null ? $"return {Expression};" : "return;";
        }
    }
}

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
    /// A C# postfix unary operator expression (e.g. i++)
    /// </summary>
    public sealed class CSharpPostfixUnaryExpression : CSharpExpression
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the expression to apply the operator to.
        /// </summary>
        public CSharpExpression On { get; private set; }

        /// <summary>
        /// Gets the operator of this postfix unary expression
        /// </summary>
        public string Operator { get; private set; }
        #endregion

        /// <summary>
        /// Creates the postfix unary expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpPostfixUnaryExpression(PostfixUnaryExpressionSyntax syntax)
        {
            Syntax = syntax;
            On = syntax.Operand.ToExpression();
            Operator = syntax.OperatorToken.Text;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessPostfixUnaryExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("expr<a?>", parentSymbol, this, false);
            On?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            On?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"{On}{Operator}";
        }
    }
}
